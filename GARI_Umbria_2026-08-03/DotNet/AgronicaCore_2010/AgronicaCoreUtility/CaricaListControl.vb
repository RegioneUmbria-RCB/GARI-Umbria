Imports System.Text
Imports System.Web.UI.WebControls
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreContabHLP
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CaricaListControl
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function Riempi_cblOTE(piva As String, sa_cod As String, objP_Server As AgronicaCoreParametri) As DropDownList

        Dim DTCodici As DataTable
        Dim objCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim Testo As String
        Dim Testo2 As String

        '###########################################################
        '### Orientamento Tecnico Economico
        '###########################################################
        Dim i As Integer
        Dim arr_ote As New List(Of String)
        Dim trovato As Boolean

        'Leggo le informazioni sull'impresa selezionata
        DTCodici = objCodici.Leggi(piva,
                                   sa_cod,
                                   0, "", "",
                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                   "id_cod < 2000 OR id_cod >= 3000",
                                   "",
                                   objP_Server)

        For i = 0 To DTCodici.Rows.Count - 1

            If DTCodici.Rows(i).Item("Id_Cod") = enum_CodiciAnagrafe.OTE Then

                Testo = DTCodici.Rows(i).Item("Val_Cod")
                Testo2 = DTCodici.Rows(i).Item("Id_Cod")

                'Imposto la posizione nella combo delle provincie
                'Indice = CmbOTE.Items.IndexOf(CmbOTE.Items.FindByValue(Testo))
                'CmbOTE.SelectedIndex = Indice


                'splitto con la pipe
                Dim jj As Integer
                Dim ote_app As String
                Dim OTE_CODICI_SPLIT As String()
                OTE_CODICI_SPLIT = Testo.Split("|")

                Dim ObjOTE As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R
                Dim DtOTE As DataTable

                For jj = 0 To OTE_CODICI_SPLIT.Length - 1
                    ote_app = OTE_CODICI_SPLIT(jj)
                    If Trim(ote_app) <> "" Then

                        DtOTE = ObjOTE.Leggi(ote_app,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "",
                                        objP_Server)

                        If DtOTE.Rows.Count <> 0 Then

                            'aggiungo l'elemento all'interno della lista
                            'ListOTE.Items.Add(New ListItem(DtOTE.Rows(0).Item("OTE_des"), DtOTE.Rows(0).Item("Ote_cod")))

                            arr_ote.Add(DtOTE.Rows(0).Item("Ote_cod"))

                        End If
                    End If
                Next
            End If

        Next

        Dim DTRs As DataTable

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R

        'Leggo le imprese associate al profilo selezionato
        DTRs = objCOM.Leggi("",
                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "", "OTE_ordine ASC", objP_Server)

        objCOM = Nothing

        '----- Riempio il controllo con i dati del datatable

        Dim otes As New DropDownList()

        If DTRs IsNot Nothing AndAlso DTRs.Rows.Count > 0 Then
            ' Itero sui valori base OTE
            For i = 0 To DTRs.Rows.Count - 1
                trovato = False

                'Itero sui valori OTE selezionati
                For Each check In arr_ote
                    If check = DTRs.Rows(i).Item("OTE_COD") Then
                        trovato = True
                    End If
                Next

                Dim listItem = New ListItem(DTRs.Rows(i).Item("OTE_DES"), DTRs.Rows(i).Item("OTE_COD"))
                If trovato Then
                    listItem.Selected = True
                End If
                otes.Items.Add(listItem)
            Next
        End If

        Return otes

    End Function

    ''' <summary>
    ''' Converte un listItemCollection in una stringa JSON che rappresenta un array di oggetti
    ''' </summary>
    ''' <param name="items"></param>
    ''' <param name="Alias_Cod"></param>
    ''' <param name="Alias_Des"></param>
    ''' <returns></returns>
    Public Shared Function itemsCollection_AsJsonArray(ByVal items As ListItemCollection, ByVal Alias_Cod As String, ByVal Alias_Des As String, Optional ByVal stringaDaSostituireInDescrizione As String = "") As String

        Dim trova As String = ""
        Dim sostituisci As String = ""

        If stringaDaSostituireInDescrizione <> "" Then
            trova = stringaDaSostituireInDescrizione.Split("|")(0)
            sostituisci = stringaDaSostituireInDescrizione.Split("|")(1)
        End If

        Dim rval As New StringBuilder
        Dim primo As Boolean = False
        rval.Append("[")
        For Each item As ListItem In items
            If primo Then
                rval.Append(",")
            End If
            primo = True

            Dim itemText As String = item.Text
            If trova <> "" Then
                itemText = itemText.Replace(trova, sostituisci)
            End If
            rval.Append("{ """ & Escape(Alias_Cod) & """: """ & Escape(item.Value) & """, """ & Escape(Alias_Des) & """: """ & Escape(itemText) & """ } ")
        Next
        rval.Append("]")



        Return rval.ToString
    End Function


    Public Shared Function Escape(ByVal s As String) As String
        Return AgronicaCoreUtility.jSon.Escape(s)
    End Function

    Public Shared Function itemsCollection_toDataTable(ByVal items As ListItemCollection, Optional ByVal Alias_Cod As String = "Name", Optional ByVal Alias_Des As String = "Mark") As DataTable
        'Dim rval As New StringBuilder
        'Dim primo As Boolean = False
        'rval.Append("[")
        'For Each item As ListItem In items
        '    If primo Then
        '        rval.Append(",")
        '    End If
        '    primo = True
        '    rval.Append("{ """ & Alias_Cod & """: """ & item.Value & """, """ & Alias_Des & """: """ & item.Text & """ } ")
        'Next
        'rval.Append("]")
        'Return rval.ToString

        'Create a DataTable
        Dim dt As DataTable = New DataTable()


        'Add columns
        dt.Columns.Add(Alias_Des, Type.GetType("System.String"))
        dt.Columns.Add(Alias_Cod, Type.GetType("System.String"))


        'Loop through ListItemCollection
        For Each item As ListItem In items
            Dim dr As DataRow = dt.NewRow()


            dr.Item(Alias_Des) = item.Text
            dr.Item(Alias_Cod) = item.Value


            dt.Rows.Add(dr)
        Next

        Return dt
    End Function


    '######################################################################
    'Le seguenti variabili hanno senso per il caricamento delle combo
    'per scegliere se aggiungere una riga vuota.
    'Se non servono, passare come default:
    'PrimaRiga_Flag As Boolean = false
    'PrimaRiga_Text As String = ""
    'PrimaRiga_Value As String = ""
    Private Shared Sub AAA_Esempio(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objImprese.Leggi(Piva,
                                enumSelezioneVariabile.Selezione_JoinCompleta,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Rag_Soc"),
                                                 Dt.Rows(i).Item("PIVA")))

            Next

        End If

    End Sub


    Public Shared Sub PrincipiAttivi(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objImprese As New AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objImprese.Leggi(0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("PA_DES"),
                                                  Dt.Rows(i).Item("PA_COD")))

            Next

        End If

    End Sub


    '######################################################################
    Public Shared Sub DettaglioPersonalizzato_Impianto(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Dim objCac As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
        Dt = objCac.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.DettaglioSpeciePersonalizzato,
                                    "",
                                    0,
                                    2,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    "",
                                    objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("InfoAgg_Des"),
                                                  Dt.Rows(i).Item("InfoAgg_Cod")))

            Next
        End If

    End Sub



    '######################################################################
    Public Shared Sub DestinazioniUso(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objMS As New AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objMS.DestinazioniUso_Leggi(xFiltroAggiuntivo,
                                        xOrderBy,
                                        objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("descrizione"),
                                                  Dt.Rows(i).Item("codice")))


            Next
        End If

    End Sub

    '######################################################################
    Public Shared Sub DestinazioniUso_Programmazione(ByRef Controllo As ListControl,
                                                    ByVal PrimaRiga_Flag As Boolean,
                                                    ByVal PrimaRiga_Text As String,
                                                    ByVal PrimaRiga_Value As String,
                                                    ByVal Programmazione_Cod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPE.DestinazioniUso(Programmazione_Cod,
                                   xFiltroAggiuntivo,
                                   xOrderBy,
                                   objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("descrizione"),
                                                  Dt.Rows(i).Item("codice")))
            Next
        End If

    End Sub



    '######################################################################
    Public Shared Sub SpecieVegetali_Programmazione(ByRef Controllo As ListControl,
                                                    ByVal PrimaRiga_Flag As Boolean,
                                                    ByVal PrimaRiga_Text As String,
                                                    ByVal PrimaRiga_Value As String,
                                                    ByVal Programmazione_Cod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPE.SpecieVegetali(Programmazione_Cod,
                                   xFiltroAggiuntivo,
                                   xOrderBy,
                                   objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                                  Dt.Rows(i).Item("Veg_Cod")))
            Next
        End If

    End Sub

    '######################################################################
    Public Shared Sub Cultivar_Programmazione(ByRef Controllo As ListControl,
                                                    ByVal PrimaRiga_Flag As Boolean,
                                                    ByVal PrimaRiga_Text As String,
                                                    ByVal PrimaRiga_Value As String,
                                                    ByVal Programmazione_Cod As Integer,
                                                    ByVal Veg_Cod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPE.Cultivar(Programmazione_Cod,
                            Veg_Cod,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Cul_Des"),
                                                  Dt.Rows(i).Item("Cul_Cod")))
            Next
        End If

    End Sub

    Public Shared Sub Centri_Programmazione(ByRef Controllo As ListControl,
                                                 ByVal PrimaRiga_Flag As Boolean,
                                                 ByVal PrimaRiga_Text As String,
                                                 ByVal PrimaRiga_Value As String,
                                                 ByVal Programmazione_Cod As Integer,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPE.Centri(Programmazione_Cod,
                          xFiltroAggiuntivo,
                                   xOrderBy,
                                   objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Sa_Nome"),
                                                  Dt.Rows(i).Item("Sa_Cod")))
            Next
        End If

    End Sub

    Public Shared Sub Campi_Programmazione(ByRef Controllo As ListControl,
                                             ByVal PrimaRiga_Flag As Boolean,
                                             ByVal PrimaRiga_Text As String,
                                             ByVal PrimaRiga_Value As String,
                                             ByVal Programmazione_Cod As Integer,
                                             ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPE.Campi(Programmazione_Cod,
                          Sa_Cod,
                          xFiltroAggiuntivo,
                                   xOrderBy,
                                   objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Campo_Des"),
                                                 Piva & "/" & Dt.Rows(i).Item("Sa_Cod") & "/" & Dt.Rows(i).Item("Campo_Cod")))
            Next
        End If

    End Sub

    '######################################################################
    Public Shared Sub Macrousi_Programmazione(ByRef Controllo As ListControl,
                                              ByVal PrimaRiga_Flag As Boolean,
                                              ByVal PrimaRiga_Text As String,
                                              ByVal PrimaRiga_Value As String,
                                              ByVal Programmazione_Cod As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPE.Macrousi(Programmazione_Cod,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Macrouso_Des"),
                                                 Dt.Rows(i).Item("Macrouso_Cod")))
            Next
        End If

    End Sub

    '######################################################################
    Public Shared Sub Utilizzi_Programmazione(ByRef Controllo As ListControl,
                                              ByVal PrimaRiga_Flag As Boolean,
                                              ByVal PrimaRiga_Text As String,
                                              ByVal PrimaRiga_Value As String,
                                              ByVal Programmazione_Cod As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPE.Utilizzi(Programmazione_Cod,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des_Agea"),
                                                 Dt.Rows(i).Item("Veg_Cod_Agea")))
            Next
        End If

    End Sub

    '######################################################################
    Public Shared Sub Indirizzi_ImpresaCentro(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Int32,
                                                ByVal Tipo_Indirizzo As Int32,
                                                ByVal Cod_Indirizzo As Int32,
                                                ByVal xFiltroAggiuntivo1 As String,
                                                ByVal xFiltroAggiuntivo2 As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String
        Dim objInd As New AgronicaCoreAnagrafeDAL.Indirizzi_Read

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objInd.IndirizziImpresaCentri(Piva,
                                            Sa_Cod,
                                            Tipo_Indirizzo,
                                            Cod_Indirizzo,
                                             xFiltroAggiuntivo1,
                                             xFiltroAggiuntivo2,
                                            xOrderBy,
                                            objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Cod_indirizzo")
                x_Des = CStr(Dt.Rows(i).Item("ind_des")) & " " &
                        CStr(Dt.Rows(i).Item("frz_des")) & " " &
                        CStr(Dt.Rows(i).Item("com_des")) & " " &
                        CStr(Dt.Rows(i).Item("pro_cod"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

    End Sub


    '######################################################################
    Public Shared Sub Connessioni(ByRef Controllo As ListControl,
                             ByVal PrimaRiga_Flag As Boolean,
                             ByVal PrimaRiga_Text As String,
                             ByVal PrimaRiga_Value As String,
                                    ByVal ID_DB As Integer,
                                    ByVal TipoDB As enum_Tipo_DB,
                                    ByVal Server As String,
                                    ByVal DB As String,
                                    ByVal Provider As String,
                                    ByVal UserId As String,
                                    ByVal Password As String,
                                    ByVal PivaSuperUser As String,
                                    ByVal Note As String,
                                    ByVal Progressivo As Integer,
                                    ByVal Descrizione As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreDataProvider.Connessioni

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = obj.Leggi(ID_DB,
                        TipoDB,
                        Server,
                        DB,
                        Provider,
                        UserId,
                        Password,
                        PivaSuperUser,
                        Note,
                        Progressivo,
                        Descrizione,
                        Validita_Inizio,
                        Validita_Fine,
                        xFiltroAggiuntivo,
                        xOrderBy,
                        objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Dim chiave As String = Dt.Rows(i).Item("ID_DB")
                chiave &= "|" & Dt.Rows(i).Item("TipoDB")
                chiave &= "|" & Dt.Rows(i).Item("Server")
                chiave &= "|" & Dt.Rows(i).Item("DB")
                chiave &= "|" & Dt.Rows(i).Item("PivaSuperUser")
                chiave &= "|" & Dt.Rows(i).Item("Progressivo")

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Descrizione"), chiave))


            Next

        End If

    End Sub



    '######################################################################
    'leggi sotto i valori per Tipo_Value 
    Public Shared Sub Imprese_Sezionali(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Tipo_Value As Integer,
                                            ByVal Piva As String,
                                            ByVal Sezionale_Cod As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim Cod, Des As String
        Dim objImprese As New AgronicaCoreContabDAL.Imprese_Sezionali_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objImprese.Leggi(Piva,
                                Sezionale_Cod,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                'Tipo_Value = 0 -> Sezionale_Cod
                'Tipo_Value = 1 -> Sezionale_Cod | LiquidazioneIva
                'Tipo_Value = 2 -> Sezionale_Cod | RegimeIva
                'Tipo_Value = 3 -> Sezionale_Cod | LiquidazioneIva | RegimeIva
                'Tipo_Value = 4 -> Sezionale_Cod | InteresseDebitoIva_Perc
                'Tipo_Value = 5 -> Sezionale_Cod | InteresseDebitoIva_Perc | LiquidazioneIva | RegimeIva
                'Tipo_Value = 6 -> Sezionale_Cod | LiquidazioneIva | ChkDefault
                Select Case Tipo_Value
                    Case 0
                        Cod = Dt.Rows(i).Item("Sezionale_Cod")
                    Case 1
                        Cod = CStr(Dt.Rows(i).Item("Sezionale_Cod")) & "|" & CStr(Dt.Rows(i).Item("LiquidazioneIva"))
                    Case 2
                        Cod = CStr(Dt.Rows(i).Item("Sezionale_Cod")) & "|" & CStr(Dt.Rows(i).Item("RegimeIva"))
                    Case 3
                        Cod = CStr(Dt.Rows(i).Item("Sezionale_Cod")) & "|" & CStr(Dt.Rows(i).Item("LiquidazioneIva")) & "|" & CStr(Dt.Rows(i).Item("RegimeIva"))
                    Case 4
                        Cod = CStr(Dt.Rows(i).Item("Sezionale_Cod")) & "|" & CStr(Dt.Rows(i).Item("InteresseDebitoIva_Perc"))
                    Case 5
                        Cod = CStr(Dt.Rows(i).Item("Sezionale_Cod")) & "|" & CStr(Dt.Rows(i).Item("InteresseDebitoIva_Perc")) & "|" & CStr(Dt.Rows(i).Item("LiquidazioneIva")) & "|" & CStr(Dt.Rows(i).Item("RegimeIva"))
                    Case 6
                        Cod = CStr(Dt.Rows(i).Item("Sezionale_Cod")) & "|" & CStr(Dt.Rows(i).Item("LiquidazioneIva")) & "|" & CStr(Dt.Rows(i).Item("ChkDefault"))
                    Case 7
                        Cod = CStr(Dt.Rows(i).Item("Sezionale_Cod")) & "|" & CStr(Dt.Rows(i).Item("InteresseDebitoIva_Perc")) & "|" & CStr(Dt.Rows(i).Item("LiquidazioneIva")) & "|" & CStr(Dt.Rows(i).Item("RegimeIva")) & "|" & CStr(Dt.Rows(i).Item("EsigibilitaIva"))
                    Case Else
                        Cod = Dt.Rows(i).Item("Sezionale_Cod")
                End Select


                Des = Dt.Rows(i).Item("Sezionale_Des")

                Controllo.Items.Add(New ListItem(Des, Cod))

            Next

        Else
            'l'utente non gestisce i sezionali o non li ha ancora inseriti in tabella
            '-> si inserisce la rag_soc come default
            Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim rag_soc As String = objImp.RagSoc_from_Piva(Piva, objParametri)

            'Tipo_Value = 0 -> Sezionale_Cod
            'Tipo_Value = 1 -> Sezionale_Cod | LiquidazioneIva
            'Tipo_Value = 2 -> Sezionale_Cod | RegimeIva
            'Tipo_Value = 3 -> Sezionale_Cod | LiquidazioneIva | RegimeIva
            'Tipo_Value = 4 -> Sezionale_Cod | InteresseDebitoIva_Perc
            'Tipo_Value = 5 -> Sezionale_Cod | InteresseDebitoIva_Perc | LiquidazioneIva | RegimeIva
            'Tipo_Value = 6 -> Sezionale_Cod | LiquidazioneIva | ChkDefault

            Select Case Tipo_Value
                Case 0
                    Cod = 0
                Case 1
                    Cod = CStr(0) & "|" & CStr(0)
                Case 2
                    Cod = CStr(0) & "|" & CStr(0)
                Case 3
                    Cod = CStr(0) & "|" & CStr(0) & "|" & CStr(0)
                Case 4
                    Cod = CStr(0) & "|" & CStr(0)
                Case 5
                    Cod = CStr(0) & "|" & CStr(0) & "|" & CStr(0) & CStr(0) & "|" & CStr(0)
                Case 6
                    Cod = CStr(0) & "|" & CStr(0) & "|" & CStr(0)
                Case Else
                    Cod = 0
            End Select

            Controllo.Items.Add(New ListItem(rag_soc, Cod))

        End If


    End Sub

    Public Shared Sub CBI_Causale(
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByRef Controllo As ListControl,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreMetaSchemaDAL.CBI_Causali_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = obj.Leggi(
                        -1,
                        xFiltroAggiuntivo,
                        xOrderBy,
                        objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("CBI_Causali_DESBreve"),
                                                  Dt.Rows(i).Item("CBI_Causali_Cod")))

            Next

        End If

    End Sub


    '######################################################################
    Public Shared Sub PianoContiEco_AnnoContabile(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal Piva_Ricl As String,
                                                ByVal Ric_Cod As Integer,
                                                ByVal Cod_Conto As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objAnno As New AgronicaCoreContabDAL.PianoConti_Economici_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objAnno.PianoContiEco_AnnoContabile(Piva_Ricl,
                                                Ric_Cod,
                                                Cod_Conto,
                                                xFiltroAggiuntivo,
                                                xOrderBy,
                                                objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Anno"),
                                                  Dt.Rows(i).Item("Anno")))

            Next

        End If

    End Sub

    '######################################################################
    'DEFAULT
    'PrimaRiga_Value = -1
    Public Shared Sub PianoContiEco_Riclassificazioni_2(ByRef Controllo As ListControl,
                                                        ByVal PrimaRiga_Flag As Boolean,
                                                        ByVal PrimaRiga_Text As String,
                                                        ByVal PrimaRiga_Value As String,
                                                        ByVal Piva_Ricl As String,
                                                        ByVal Anno As Integer,
                                                        ByVal Ric_Cod As Integer,
                                                        ByVal Cod_Conto As Integer,
                                                        ByVal Cod_Contatto As String,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objAnno As New AgronicaCoreContabDAL.PianoConti_Economici_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objAnno.PianoContiEco_Riclassificazioni_2(Piva_Ricl,
                                                    Anno,
                                                    Ric_Cod,
                                                    Cod_Conto,
                                                    "",
                                                    xFiltroAggiuntivo,
                                                    xOrderBy,
                                                    objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Ric_Des"),
                                                  Dt.Rows(i).Item("Ric_Cod")))

            Next

        End If

    End Sub

    '######################################################################
    Public Shared Sub PianoContiPat_Riclassificazioni(ByRef Controllo As ListControl,
                                                        ByVal PrimaRiga_Flag As Boolean,
                                                        ByVal PrimaRiga_Text As String,
                                                        ByVal PrimaRiga_Value As String,
                                                        ByVal Piva_Ricl As String,
                                                        ByVal Anno As Integer,
                                                        ByVal Ric_Cod As Integer,
                                                        ByVal Cod_Conto As Integer,
                                                        ByVal Cod_Contatto As String,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPC As New AgronicaCoreContabDAL.PianoConti_Patrimoniali_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPC.PianoContiPat_Riclassificazioni(Piva_Ricl,
                                                    Anno,
                                                    Ric_Cod,
                                                    Cod_Conto,
                                                    "",
                                                    xFiltroAggiuntivo,
                                                    xOrderBy,
                                                    objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Ric_Des_Pat"),
                                                  Dt.Rows(i).Item("Ric_Cod_Pat")))

            Next

        End If

    End Sub

    '######################################################################
    'Default:
    'PrimaRiga_Value=-1
    'Dare_Avere As String = "", _
    'Cod_Contatto As String = "", _
    'Flag_DareAvere_nelCod As Boolean = False
    Public Shared Sub PianoContiEco_Conti(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Piva_ricl As String,
                                            ByVal Ric_Cod As Integer,
                                            ByVal Anno As Integer,
                                            ByVal Cod_Conto As Integer,
                                            ByVal Imputabile As Integer,
                                            ByVal Flag_UE As Integer,
                                            ByVal Dare_Avere As String,
                                            ByVal Cod_Contatto As String,
                                            ByVal Flag_DareAvere_nelCod As Boolean,
                                            ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objAnno As New AgronicaCoreContabDAL.PianoConti_Economici_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objAnno.PianoConti_ContoEconomico(Piva_ricl,
                                                Anno,
                                                "",
                                                Dare_Avere,
                                                Imputabile,
                                                Ric_Cod,
                                                "",
                                                Cod_Conto,
                                                Flag_UE,
                                                Cod_Contatto,
                                                xFiltroAggiuntivo,
                                                xOrderBy,
                                                objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            Dim Des, Cod As String

            For i = 0 To Dt.Rows.Count - 1

                Des = CStr(Dt.Rows(i).Item("Id_Riclassificazione")) & " - " & CStr(Dt.Rows(i).Item("Conto_Descr"))

                If Not Flag_DareAvere_nelCod Then
                    Cod = Dt.Rows(i).Item("Cod_Conto")
                Else
                    Cod = CStr(Dt.Rows(i).Item("Cod_Conto")) & "|" & CStr(Dt.Rows(i).Item("Dare_Avere"))
                End If

                Controllo.Items.Add(New ListItem(Des, Cod))

            Next

        End If

    End Sub

    '######################################################################
    'Default:
    'PrimaRiga_Value=-1
    'Dare_Avere As String = "", _
    'Cod_Contatto As String = "", _
    Public Shared Sub PianoContiEcoPat_PiuAnni(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal Piva_Ricl As String,
                                                ByVal AnnoMin As Integer,
                                                ByVal Codifica_Conto As Integer,
                                                ByVal Ric_Cod As Integer,
                                                ByVal Flag_UE As Integer,
                                                ByVal Cod_Contatto As String,
                                                ByVal Dare_Avere As String,
                                                ByVal Imputabile As Integer,
                                                ByVal FiltroContoDescr As String,
                                                ByVal TipoValue As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPC As New AgronicaCoreContabDAL.PianoConti_EcoPat_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPC.Leggi_ContiEcoPat_DistinctCodContoPiuAnni(Piva_Ricl,
                                                            AnnoMin,
                                                           Codifica_Conto,
                                                           Ric_Cod,
                                                              Flag_UE,
                                                            Cod_Contatto,
                                                            Dare_Avere,
                                                            Imputabile,
                                                            FiltroContoDescr,
                                                             xFiltroAggiuntivo,
                                                            xOrderBy,
                                                            objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            Dim Des, Cod As String

            For i = 0 To Dt.Rows.Count - 1

                Des = CStr(Dt.Rows(i).Item("Tipo")) & " - " & CStr(Dt.Rows(i).Item("Id_Riclassificazione")) & " - " & CStr(Dt.Rows(i).Item("Conto_Descr"))

                Select Case TipoValue
                    Case 0
                        Cod = CStr(Dt.Rows(i).Item("Cod_Conto"))
                    Case 1
                        Cod = CStr(Dt.Rows(i).Item("Cod_Conto")) & "|" & CStr(Dt.Rows(i).Item("Dare_Avere"))
                    Case 2
                        Cod = CStr(Dt.Rows(i).Item("Cod_Conto")) & "|" & CStr(Dt.Rows(i).Item("Tipo"))
                    Case 3
                        Cod = CStr(Dt.Rows(i).Item("Cod_Conto")) & "|" & CStr(Dt.Rows(i).Item("Tipo")) & "|" & CStr(Dt.Rows(i).Item("Dare_Avere"))
                    Case Else
                        Cod = CStr(Dt.Rows(i).Item("Cod_Conto"))
                End Select

                Controllo.Items.Add(New ListItem(Des, Cod))

            Next

        End If

    End Sub


    '######################################################################
    Public Shared Sub PianoContiPat_Conti(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Piva_ricl As String,
                                            ByVal Ric_Cod As Integer,
                                            ByVal Anno As Integer,
                                            ByVal Cod_Conto As Integer,
                                            ByVal Imputabile As Integer,
                                            ByVal Flag_UE As Integer,
                                            ByVal Dare_Avere As String,
                                            ByVal Flag_DareAvere_nelCod As Boolean,
                                            ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objSP As New AgronicaCoreContabDAL.PianoConti_Patrimoniali_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objSP.PianoConti_StatoPatrimoniale(Piva_ricl,
                                                Anno,
                                                "",
                                                Dare_Avere,
                                                Imputabile,
                                                Ric_Cod,
                                                "",
                                                Cod_Conto,
                                                Flag_UE,
                                                 xFiltroAggiuntivo,
                                                xOrderBy,
                                                objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            Dim Des, Cod As String

            For i = 0 To Dt.Rows.Count - 1

                Des = CStr(Dt.Rows(i).Item("Id_Riclassificazione")) & " - " & CStr(Dt.Rows(i).Item("Conto_Pat_Descr"))

                If Not Flag_DareAvere_nelCod Then
                    Cod = Dt.Rows(i).Item("Cod_Conto_Pat")
                Else
                    Cod = CStr(Dt.Rows(i).Item("Cod_Conto_Pat")) & "|" & CStr(Dt.Rows(i).Item("Dare_Avere"))
                End If

                Controllo.Items.Add(New ListItem(Des, Cod))

            Next

        End If

    End Sub

    '######################################################################
    'Tipo_Desc: per decidere come valorizzare la descrizione
    '0 =descrizione e codice
    ' 1 =solo descrizione
    Public Shared Sub Capitolato_Privato(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal InfoAgg_Cod As String,
                                            ByVal Tipo_Desc As Int32,
                                            ByVal OrderBy_Cod1_Des2 As Int32,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod, x_Des As String
        Dim obj As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = obj.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CapitolatoPrivato,
                        InfoAgg_Cod,
                        0,
                        OrderBy_Cod1_Des2,
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        xFiltroAggiuntivo,
                        xOrderBy,
                        objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = CStr(Dt.Rows(i).Item("InfoAgg_Cod"))

                Select Case Tipo_Desc
                    Case 0 'descrizione e codice
                        x_Des = CStr(Dt.Rows(i).Item("InfoAgg_Des")) & " (" & CStr(Dt.Rows(i).Item("InfoAgg_Cod")) & ")"
                    Case 1 'solo descrizione
                        x_Des = CStr(Dt.Rows(i).Item("InfoAgg_Des"))
                    Case Else 'default: descrizione e codice
                        x_Des = CStr(Dt.Rows(i).Item("InfoAgg_Des")) & " (" & CStr(Dt.Rows(i).Item("InfoAgg_Cod")) & ")"
                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If


    End Sub

    '######################################################################
    'Tipo_Desc: per decidere come valorizzare la descrizione
    '0 =codice e descrizione 
    ' 1 =solo codice
    ' 2 =solo descrizione
    '
    'Veg_Cod_0Tutti: Come si può intuire con 0 non filtro il Veg_Cod.. (mi immagino che per il terreno nudo non riesco ad avere piani semina filtrati)
    '
    ' 2017/10/24 Simone: Ho inserito come richiesto n piani semina definiti per l'impianto. Questo significa che i piani semina inseriti in precedenza
    ' per retrocompatibilità li ho anche con il metodo PianiSemina
    '
    Public Shared Sub Piano_Semina_Tabellato(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Veg_Cod_0Tutti As Integer,
                                            ByVal Tipo_Desc As Int32,
                                            ByVal OrderBy_Cod1_Des2 As Int32,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod, x_Des As String
        Dim obj As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
        Dim strFiltro As String = ""

        If Veg_Cod_0Tutti <> 0 Then
            strFiltro = "CodiceAux_1 =" & Veg_Cod_0Tutti.ToString
        End If

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If



        Dt = obj.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Piano_Semina,
                        "",
                        0,
                        OrderBy_Cod1_Des2,
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        strFiltro,
                        xOrderBy,
                        objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = CStr(Dt.Rows(i).Item("InfoAgg_Cod"))

                Select Case Tipo_Desc
                    Case 0 'codice e descrizione 
                        x_Des = " (" & CStr(Dt.Rows(i).Item("InfoAgg_Cod")) & ")" & CStr(Dt.Rows(i).Item("InfoAgg_Des"))
                    Case 1 'solo codice
                        x_Des = CStr(Dt.Rows(i).Item("InfoAgg_Cod"))
                    Case 2 'solo descrizione
                        x_Des = CStr(Dt.Rows(i).Item("InfoAgg_Des"))
                    Case Else 'default: codice e descrizione 
                        x_Des = " (" & CStr(Dt.Rows(i).Item("InfoAgg_Cod")) & ")" & CStr(Dt.Rows(i).Item("InfoAgg_Des"))
                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If


    End Sub


    '######################################################################
    Public Shared Sub Prodotto_LineeClassiProduzioni(ByRef Controllo As ListControl,
                                                         ByVal PrimaRiga_Flag As Boolean,
                                                         ByVal PrimaRiga_Text As String,
                                                         ByVal PrimaRiga_Value As String,
                                                         ByVal Piva As String,
                                                         ByVal Mat_Cod As Integer,
                                                         ByVal Linea_Classe_Cod As Integer,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objClass As New AgronicaCoreContabDAL.Linee_Classi_Produzioni_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objClass.LeggiClassiProdotto(Piva,
                                            Linea_Classe_Cod,
                                            Mat_Cod,
                                            xFiltroAggiuntivo,
                                            xOrderBy,
                                            objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Linea_Classe_Des"),
                                                  Dt.Rows(i).Item("Linea_Classe_Cod")))

            Next

        End If

    End Sub

    '######################################################################
    Public Shared Sub BIO_Organismi_Controllo(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal Organismo_Cod As Integer,
                                                ByVal Organismo_Sigla As String,
                                                ByVal Codice As String,
                                                ByVal TipoDes012 As Integer,
                                                ByVal TipoCod012 As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objBio As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrganismiControllo_R
        Dim des As String = ""
        Dim cod As String = ""

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objBio.Leggi(Organismo_Cod,
                          Organismo_Sigla,
                          Codice,
                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                          xFiltroAggiuntivo,
                          xOrderBy,
                          objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Select Case TipoDes012

                    Case 0 'codice - desc
                        des = Dt.Rows(i).Item("Codice") & " - " & Dt.Rows(i).Item("Organismo_Des")
                    Case 1 'sigla: desc
                        des = Dt.Rows(i).Item("Organismo_Sigla") & ": " & Dt.Rows(i).Item("Organismo_Des")
                    Case 2 'codice - desc (sigla)
                        des = Dt.Rows(i).Item("Codice") & " - " & Dt.Rows(i).Item("Organismo_Des") & " (" & Dt.Rows(i).Item("Organismo_Sigla") & ")"
                End Select

                Select Case TipoCod012

                    Case 0 'Organismo_Cod
                        cod = Dt.Rows(i).Item("Organismo_Cod")
                    Case 1 'Organismo_Sigla
                        cod = Dt.Rows(i).Item("Organismo_Sigla")
                    Case 2 'Codice
                        cod = Dt.Rows(i).Item("Codice")
                End Select

                Controllo.Items.Add(New ListItem(des, cod))

            Next

        End If

    End Sub

    '###############################################################################################
    'default:
    'Flag_VisualizzaArtEsclusi = true (visualizza tutto, anche gli articoli di esclusione iva)
    Public Shared Sub IVA_aliquote(ByRef Controllo As ListControl,
                                   ByVal PrimaRiga_Flag As Boolean,
                                   ByVal PrimaRiga_Text As String,
                                   ByVal PrimaRiga_Value As String,
                                   ByVal Flag_VisualizzaArtEsclusi As Boolean,
                                   ByVal Flag_AggiungiAliquotaValue As Boolean,
                                   ByRef objParametri As AgronicaCoreParametri)

        Dim i As Integer
        Dim x_Cod, x_Des As String
        Dim Dt_IVA As DataTable
        Dim objIVA As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R
        Dim Filtro As String = ""

        If Not Flag_VisualizzaArtEsclusi Then
            'gli articoli esclusione iva hanno il codice dal 61 in su
            Filtro = " Codice > 60"
        End If

        Dt_IVA = objIVA.Leggi(0, -1, -1, -1, NATURA_ESCLUSIONE_NOFILTRO, Filtro, "", objParametri)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        For i = 0 To Dt_IVA.Rows.Count - 1

            x_Cod = CStr(Dt_IVA.Rows(i).Item("Codice"))
            If Flag_AggiungiAliquotaValue Then
                x_Cod &= "|" & CStr(Dt_IVA.Rows(i).Item("Aliquota"))
            End If

            x_Des = CStr(Dt_IVA.Rows(i).Item("Descrizione"))

            Controllo.Items.Add(New ListItem(x_Des, x_Cod))

        Next

    End Sub





    '###############################################################################
    'Non va bene usare questa caricacombo per caricare tutte le imprese,
    'perché non viene tenuto conto del filtro di visibilità associato all'utente.
    'Per caricare tutte le imprese occorre usare CaricaCombo_ImpreseUtente_Optimize
    'Questa caricacombo è utile se si deve caricare la combo con una sola impresa,
    'così la lettura è più veloce e non si devono fare filtri inutili.
    Public Shared Sub Imprese(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt_Imprese As DataTable
        Dim i As Integer
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Dt_Imprese = NewCom_Imprese_Leggi(objServer, objSession, objPage, _
        '                                    Piva, _
        '                                    FiltroAggiuntivo, _
        '                                    False, _
        '                                    0, 0)

        Dt_Imprese = objImprese.Leggi_3(Piva,
                                False,
                                0,
                                0,
                                False,
                                False,
                                False,
                                False,
                                False,
                                False,
                                False,
                                False,
                                False,
                                False,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri)


        If Not IsNothing(Dt_Imprese) AndAlso Dt_Imprese.Rows.Count <> 0 Then

            For i = 0 To Dt_Imprese.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt_Imprese.Rows(i).Item("Rag_Soc"),
                                                  Dt_Imprese.Rows(i).Item("PIVA")))

            Next

        End If


        'If TypeOf Controllo Is System.Web.UI.WebControls.CheckBoxList = True Then

        'ElseIf TypeOf Controllo Is System.Web.UI.WebControls.DropDownList = True Then

        'ElseIf TypeOf Controllo Is System.Web.UI.WebControls.ListBox = True Then

        'ElseIf TypeOf Controllo Is System.Web.UI.WebControls.RadioButtonList = True Then

        'Else
        '    Throw New Exception("Tipo controllo non valido!")
        'End If

    End Sub

    '###############################################################################
    'Non va bene usare questa caricacombo per caricare tutte le imprese,
    'perché non viene tenuto conto del filtro di visibilità associato all'utente.
    'Per caricare tutte le imprese occorre usare CaricaCombo_ImpreseUtente_Optimize
    'Questa caricacombo è utile se si deve caricare la combo con una sola impresa,
    'così la lettura è più veloce e non si devono fare filtri inutili.
    Public Shared Sub Gerarchia_Imprese(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal PivaFiglio As String,
                                ByVal pivaPadre As String,
                                ByVal Livelli As List(Of Integer),
                                ByVal foglia As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal TipoImpresa As List(Of Integer) = Nothing
                                )

        Dim Dt_Imprese As DataTable
        Dim i As Integer
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Dt_Imprese = NewCom_Imprese_Leggi(objServer, objSession, objPage, _
        '                                    Piva, _
        '                                    FiltroAggiuntivo, _
        '                                    False, _
        '                                    0, 0)

        Dt_Imprese = objImprese.Leggi_Gerarchia_Imprese(PivaFiglio, pivaPadre, Livelli, foglia,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri, TipoImpresa)


        If Not IsNothing(Dt_Imprese) AndAlso Dt_Imprese.Rows.Count <> 0 Then

            For i = 0 To Dt_Imprese.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt_Imprese.Rows(i).Item("Rag_Soc"),
                                                  Dt_Imprese.Rows(i).Item("PIVA")))

            Next

        End If


        'If TypeOf Controllo Is System.Web.UI.WebControls.CheckBoxList = True Then

        'ElseIf TypeOf Controllo Is System.Web.UI.WebControls.DropDownList = True Then

        'ElseIf TypeOf Controllo Is System.Web.UI.WebControls.ListBox = True Then

        'ElseIf TypeOf Controllo Is System.Web.UI.WebControls.RadioButtonList = True Then

        'Else
        '    Throw New Exception("Tipo controllo non valido!")
        'End If

    End Sub


    '###############################################################################
    'corrisponde a CaricaCombo_ImpreseUtente_Optimize
    Public Shared Sub ImpreseConFiltroUtente(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByRef passa_Dt_Imprese As Boolean = False,
                                            Optional ByRef Dt_Imprese As DataTable = Nothing,
                                            Optional ByVal bInsertCodiceSocio As Boolean = False)

        Dim i As Integer
        Dim x_Cod, x_Des As String
        'Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim objUtentiProf As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        'Dim FiltroxUtente As String = ""
        Dim ClassJoin As New JoinFiltrone
        Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        '''<summary>Usata per attribuire visibilità nulla ad un utente.</summary>
        Dim pivaFittizia = "###########"

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        ''----------------------------------------------------------------
        ''--- Filtro associato all'utente 
        ''----------------------------------------------------------------

        ''se ci sono più imprese sotto al superuser
        'Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R

        'Dim DtImpreseVisibili As DataTable
        'DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)

        'If Not DtImpreseVisibili Is Nothing Then
        '    For i = 0 To DtImpreseVisibili.Rows.Count - 1
        '        FiltroxUtente &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
        '    Next
        '    If FiltroxUtente <> "" Then
        '        FiltroxUtente = " Imprese.piva IN (" & Left(FiltroxUtente, FiltroxUtente.Length - 1) & ") "
        '    End If
        'End If

        'FiltroxUtente = objUtentiProf.Leggi_FiltroUtenteSQL(objParametri_Utenti.UtenteUsername, _
        '                                                        0, _
        '                                                        "", "", _
        '                                                        objParametri_Utenti)

        'If FiltroxUtente <> "" Then
        '    'se l'utente ha un filtro associato

        '    'concateno al filtro scelto, la stringa dei permessi dell'utente
        '    If xFiltroAggiuntivo <> "" Then

        '        'modifica del 25/07/2011 by maga:
        '        'qualcuno ha cambiato il salvataggio della stringa sql del filtro utente:
        '        'nella tabella [Utenti_Profili] nel campo [Descrizione_2]
        '        'a volte la scritta inizia con AND altre volte no
        '        If FiltroxUtente.StartsWith("AND") = True OrElse FiltroxUtente.StartsWith(" AND") = True Then
        '            xFiltroAggiuntivo &= " " & FiltroxUtente & " "
        '        Else
        '            xFiltroAggiuntivo &= " AND  (" & FiltroxUtente & ")"
        '        End If

        '    Else
        '        xFiltroAggiuntivo = FiltroxUtente
        '    End If

        'Per il momento imposto sempre a true perché può capitare
        'che il filtro associato all'utente vada a controllare il campo Padre e/o Foglia
        '(di GerarchiaImprese), ma non essendo specificata la tabella GerarchiaImprese
        'prima del nome del campo, la funzione ImpostaVariabiliJOIN_xFiltroUtente non la trova e non imposta il join
        ClassJoin.bGerarchiaImprese = True

        'End If

        classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente(xFiltroAggiuntivo, ClassJoin)

        Dt_Imprese = classFiltrone.CreaDTFiltrone(objParametri_Server,
                                                  xFiltroAggiuntivo,
                                                  enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                  xOrderBy,
                                                  ClassJoin)

        Dim filtroSQL = objUtentiProf.Leggi_FiltroUtenteSQL(objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline,
                                                            "", "", objParametri_Utenti)

        If Not filtroSQL.Contains(pivaFittizia) AndAlso Not IsNothing(Dt_Imprese) AndAlso Dt_Imprese.Rows.Count <> 0 Then

            For i = 0 To Dt_Imprese.Rows.Count - 1

                x_Cod = Dt_Imprese.Rows(i).Item("PIVA")

                If bInsertCodiceSocio Then
                    Dim CodiceSocio = Dt_Imprese.Rows(i).Item("CodiceSocio")
                    x_Des = Trim(Left(Dt_Imprese.Rows(i).Item("Rag_Soc"), 65)) & " (" & If(Not IsDBNull(CodiceSocio) AndAlso CodiceSocio <> "", Gias.CodiceSocio & ": " & Dt_Imprese.Rows(i).Item("CodiceSocio") & " - ", "") & Gias.PartitaIvaAbbr & ": " & Dt_Imprese.Rows(i).Item("PivaReale") & ")"
                Else
                    x_Des = Dt_Imprese.Rows(i).Item("Rag_Soc") & " ( Partita IVA : " & Dt_Imprese.Rows(i).Item("PivaReale") & ")"
                End If

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        If Not passa_Dt_Imprese Then
            Dt_Imprese = Nothing
        End If

    End Sub

    Public Shared Sub ImpreseConFiltroUtente2(ByRef Controllo As ListControl,
                                             ByVal Padre As String,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        Dim Dt_Imprese As DataTable
        Dim i As Integer
        Dim x_Cod, x_Des As String
        'Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        ' Dim objUtentiProf As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        'Dim FiltroxUtente As String
        Dim ClassJoin As New JoinFiltrone
        Dim classFiltrone As New AgronicaCoreUtility.Filtrone

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        ''----------------------------------------------------------------
        ''--- Filtro associato all'utente 
        ''----------------------------------------------------------------

        'Per il momento imposto sempre a true perché può capitare
        'che il filtro associato all'utente vada a controllare il campo Padre e/o Foglia
        '(di GerarchiaImprese), ma non essendo specificata la tabella GerarchiaImprese
        'prima del nome del campo, la funzione ImpostaVariabiliJOIN_xFiltroUtente non la trova e non imposta il join
        ClassJoin.bGerarchiaImprese = True

        classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente(xFiltroAggiuntivo, ClassJoin)

        Dt_Imprese = classFiltrone.CreaDTFiltrone(objParametri_Server,
                                                    xFiltroAggiuntivo,
                                                    enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                    xOrderBy,
                                                    ClassJoin)



        If Not IsNothing(Dt_Imprese) AndAlso Dt_Imprese.Rows.Count <> 0 Then

            For i = 0 To Dt_Imprese.Rows.Count - 1
                x_Cod = Dt_Imprese.Rows(i).Item("PIVA")
                x_Des = Dt_Imprese.Rows(i).Item("Rag_Soc")
                Dim ok As Boolean = True
                If Padre <> "" Then
                    Dim imp As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                    Dim padri As String = imp.Ricava_Stringa_PivePadre(x_Cod, "", objParametri_Server)
                    If padri.IndexOf(Padre) >= 0 Then
                        ok = True
                    Else
                        ok = False
                    End If
                End If

                If ok Then
                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                End If

            Next

        End If

    End Sub


    Public Shared Sub ParticelleCatastali(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           )

        '----- Definisco le variabili

        Dim objCOM As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim DT As DataTable

        Dim Testo As String
        Dim Valore As String

        Dim StrCodProvincia As String
        Dim StrCodComune As String
        Dim StrSezione As String
        Dim StrFoglio As String
        Dim StrNumero As String
        Dim StrSubalterno As String

        '-----

        'Leggo le imprese associate al profilo selezionato			
        DT = objCOM.Leggi(0, CStr(Piva), CInt(Sa_Cod), 0, "", "", "", 0, 0, "",
                          enumSelezioneVariabile.Selezione_JoinDescrizioni,
                          "",
                          "",
                          objParametri)

        'Elimino gli oggetti COM
        objCOM = Nothing

        '----- Riempio la combo con i dati del recordset

        'Pulisco la combo
        Controllo.Items.Clear()

        'Se il recordset non è chiuso allora ...	
        If DT.Rows.Count > 0 Then

            'Inserisco una riga vuota
            Controllo.Items.Add(New ListItem("", ""))

            'Inserisco i record trovati
            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                Testo = ""

                StrCodProvincia = Left("___" & DT.Rows(i).Item("Prov") & "__________", 10)
                StrCodComune = Left("___" & DT.Rows(i).Item("Com") & "__________", 10)

                If DT.Rows(i).Item("Sezione") = "0" Then
                    StrSezione = Left("__________", 6)
                Else
                    StrSezione = Left("__" & DT.Rows(i).Item("Sezione") & "__________", 6)
                End If

                StrFoglio = Left("_" & DT.Rows(i).Item("Foglio") & "__________", 7)
                StrNumero = Left("_" & DT.Rows(i).Item("Numero") & "__________", 7)

                If DT.Rows(i).Item("Subalterno") = "0" Then
                    StrSubalterno = Left("__________", 7)
                Else
                    StrSubalterno = Left("__" & DT.Rows(i).Item("Subalterno") & "__________", 7)
                End If

                Testo = StrCodProvincia & " : " &
                        StrCodComune & " : " &
                        StrSezione & " : " &
                        StrFoglio & " : " &
                        StrNumero & " / " &
                        StrSubalterno


                '----- Costruisco la stringa del valore
                Valore = ""

                Valore = "" & DT.Rows(i).Item("Prov") &
                        "£" & DT.Rows(i).Item("Com") &
                        "£" & DT.Rows(i).Item("Sezione") &
                        "£" & DT.Rows(i).Item("Foglio") &
                        "£" & DT.Rows(i).Item("Numero") &
                        "£" & DT.Rows(i).Item("Subalterno")

                'Inserisco la voce nella combobox
                Controllo.Items.Add(New ListItem(Testo, Valore))

            Next

        End If

    End Sub




    '###############################################################################
    'Tipo_Value = 1 ---> ex CaricaCombo_CentriAziendali
    'Tipo_Value = 2 ---> ex CaricaCombo_CentriAziendali2
    Public Shared Sub Centri_Aziendali(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Piva As String,
                                        ByVal Flag_SoloCentriAttivi As Boolean,
                                        ByVal Tipo_Value As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String = ""
        Dim x_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then

            If PrimaRiga_Value = "" Then
                Select Case Tipo_Value

                    Case 2
                        PrimaRiga_Value = ""

                    Case 1
                        'NOTA
                        'VALUE costituito da Piva/Sa_Cod
                        PrimaRiga_Value = "xxxxxxxxxxx/-1"

                End Select

            End If

            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))

        End If


        'Verifico se sono richiesti solo i centri ancora attivi
        If Flag_SoloCentriAttivi Then
            'funzione che imposta le date della finestra temporale con le date che servono 
            'per la lettura
            objParametri.ImpostaFinestre_con_SalvataggioTemporale(Date.Now, Date.Now)
        End If

        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        Dt = objCentri.Leggi(Piva,
                                0,
                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri)

        'se avevo modificato la finestra temporale per la query
        'reimposto i valori iniziali
        If Flag_SoloCentriAttivi Then
            'funzione che reimposta i valori iniziali della finestra temporale
            objParametri.ResettaFinestra()
        End If



        'Dt = NewCom_CentriAziendali_Leggi(objServer, objSession, objPage, _
        '                                    Piva, _
        '                                    0, _
        '                                    FinestraInizio, , _
        '                                    "", "")


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                Select Case Tipo_Value

                    Case 2
                        x_Cod = Dt.Rows(i).Item("Sa_Cod")

                    Case 1
                        'NOTA
                        'VALUE costituito da Piva/Sa_Cod
                        x_Cod = Dt.Rows(i).Item("Piva") & "/" & CStr(Dt.Rows(i).Item("Sa_Cod"))

                End Select

                x_Des = CStr(Dt.Rows(i).Item("Sa_Nome"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If


        'Dt.Dispose()
        Dt = Nothing


    End Sub


    '###############################################################################
    'Tipo_Value = 1 ---> ex CaricaCombo_CentriAziendali
    'Tipo_Value = 2 ---> ex CaricaCombo_CentriAziendali2
    Public Shared Sub Carica_Centri_Aziendali_Apeezzamenti_Impianti(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Piva As String,
                                        ByVal Flag_SoloCentriAttivi As Boolean,
                                        ByVal Tipo_Value As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        )

        Dim Dt As DataTable
        Dim i As Integer
        'Dim FinestraInizio As Date
        Dim x_Cod As String = ""
        Dim x_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then

            If PrimaRiga_Value = "" Then
                Select Case Tipo_Value

                    Case 2
                        PrimaRiga_Value = ""

                    Case 1
                        'NOTA
                        'VALUE costituito da Piva/Sa_Cod
                        PrimaRiga_Value = "xxxxxxxxxxx/-1"

                End Select

            End If

            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))

        End If


        'Verifico se sono richiesti solo i centri ancora attivi
        If Flag_SoloCentriAttivi Then
            'funzione che imposta le date della finestra temporale con le date che servono 
            'per la lettura
            objParametri.ImpostaFinestre_con_SalvataggioTemporale(Date.Now, Date.Now)
        End If

        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        Dt = objCentri.Leggi(Piva,
                                0,
                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri)

        'se avevo modificato la finestra temporale per la query
        'reimposto i valori iniziali
        If Flag_SoloCentriAttivi Then
            'funzione che reimposta i valori iniziali della finestra temporale
            objParametri.ResettaFinestra()
        End If



        'Dt = NewCom_CentriAziendali_Leggi(objServer, objSession, objPage, _
        '                                    Piva, _
        '                                    0, _
        '                                    FinestraInizio, , _
        '                                    "", "")


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                Select Case Tipo_Value

                    Case 2
                        x_Cod = Dt.Rows(i).Item("Sa_Cod")

                    Case 1
                        'NOTA
                        'VALUE costituito da Piva/Sa_Cod
                        x_Cod = Dt.Rows(i).Item("Piva") & "/" & CStr(Dt.Rows(i).Item("Sa_Cod"))

                End Select

                x_Des = CStr(Dt.Rows(i).Item("Sa_Nome"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))


                ' Leggo gli Appezzamenti relativi al Centro Aziendale
                Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

                Dim Dt2 As New DataTable
                Dt2 = objAppezza.Leggi(Piva, x_Cod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

                Dim j As Integer
                If Not IsNothing(Dt2) Then

                    For j = 0 To Dt2.Rows.Count - 1

                        Dim voce As String = "   - " & CStr(Dt2.Rows(j).Item("App_Nome")) & " {" & CStr(Dt2.Rows(j).Item("Sup_App")) & " ha}"
                        Dim id As String = "a5§" & Piva & "§" & x_Cod & "§0§" & CStr(Dt2.Rows(j).Item("Appezza")) & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"

                        'Aggiungo la voce dell'appezzamento
                        Controllo.Items.Add(New ListItem(voce, id))


                        ' Leggo gli Impianti relativi all'Appezzamento
                        Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        Dim Dt3 As New DataTable
                        Dt3 = objImpianto.Leggi(Piva, x_Cod, Dt2.Rows(j).Item("Appezza"), 0, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri)

                        Dim z As Integer

                        If Not IsNothing(Dt3) Then
                            For z = 0 To Dt3.Rows.Count - 1

                                Dim voce2 As String = "      -- " & CStr(Dt3.Rows(z).Item("Validita_Inizio")) & " | " & CStr(Dt3.Rows(z).Item("Veg_Des")) & " | " & CStr(Dt3.Rows(z).Item("Cul_Des")) & " {" & CStr(Dt3.Rows(z).Item("Sup_Imp")) & " ha}"
                                Dim id2 As String = "a8§" & Piva & "§" & x_Cod & "§0§" & CStr(Dt2.Rows(j).Item("Appezza")) & "§" & CStr(Dt3.Rows(z).Item("Id_Reg")) & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"


                                'Aggiungo la voce dell'appezzamento
                                Controllo.Items.Add(New ListItem(voce2, id2))

                            Next

                        End If

                    Next

                End If

            Next

        End If

        Dt = Nothing

    End Sub


    '###############################################################################
    'Seleziona tutti i fabbricati di un'impresa o Centro Aziendale 
    'Optional ByVal Fabbricato_Cod As Integer = 0
    '--------------------------------------------
    'Se Flag_CodCentroFabbricato = true, come value mette fabbricato_cod|sa_cod
    'Se Flag_CodCentroFabbricato = false, come value mette fabbricato_cod
    Public Sub Fabbricati(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Fabbricato_Cod As Integer,
                                ByVal Tipo_Fabbricato As Integer,
                                ByVal Flag_CodCentroFabbricato As Boolean,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByVal Validita_Metti_AGRODATAFINE_e_funziona_come_prima_e_non_rompermi As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String = ""
        Dim x_Des As String = ""


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Select Case Tipo_Fabbricato

            Case enum_FabbricatiTipi.essiccatoio
                xFiltroAggiuntivo &= " (Fabbricati.Tipo_Fabbricato_Cod =" & CInt(enum_FabbricatiTipi.essiccatoio) & ") "

            Case MAGAZZINO
                xFiltroAggiuntivo &= " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

            Case STALLA
                xFiltroAggiuntivo &= " ( (Fabbricati.Tipo_Fabbricato_Cod >= 170 AND Fabbricati.Tipo_Fabbricato_Cod < 180) OR Fabbricati.Tipo_Fabbricato_Cod = 70 ) "

            Case FABBRICATI_NO_STALLE
                xFiltroAggiuntivo &= " (Fabbricati.Tipo_Fabbricato_Cod < 170 OR Fabbricati.Tipo_Fabbricato_Cod >= 180) AND Fabbricati.Tipo_Fabbricato_Cod <> 70  "

        End Select

        If Validita_Metti_AGRODATAFINE_e_funziona_come_prima_e_non_rompermi <> AGRODATAFINE Then
            If xFiltroAggiuntivo.Trim <> "" Then
                xFiltroAggiuntivo &= " and "
            End If

            xFiltroAggiuntivo &= "Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Metti_AGRODATAFINE_e_funziona_come_prima_e_non_rompermi) & " and Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Metti_AGRODATAFINE_e_funziona_come_prima_e_non_rompermi)

        End If

        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

        Dt = objFabbricati.Leggi_2(Piva,
                                   Sa_Cod,
                                   Fabbricato_Cod,
                                   0,
                                   xFiltroAggiuntivo,
                                   xOrderBy,
                                   objParametri)


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                Select Case Flag_CodCentroFabbricato

                    Case True
                        x_Cod = CStr(Dt.Rows(i).Item("Fabbricato_Cod")) & "|" & CStr(Dt.Rows(i).Item("Sa_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Fabbricato_Des")) & " (" & CStr(Dt.Rows(i).Item("Sa_Nome")) & ")"

                    Case False
                        x_Cod = Dt.Rows(i).Item("Fabbricato_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Fabbricato_Des"))

                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub

    '###############################################################################
    'Seleziona i magazzini e le vasche
    '--------------------------------------------
    'Tipo_Value_Desc =0 -> codice: fabbricato_cod|sa_cod desc: magazzino (centro az.)
    'Tipo_Value_Desc =1 -> codice: fabbricato_cod desc: magazzino
    'Tipo_Value_Desc =2 -> codice: fabbricato_cod desc: magazzino (fabbricato_cod)
    Public Shared Sub Magazzini_e_Vasche(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Piva_1 As String,
                                        ByVal Sa_Cod_1 As Int32,
                                        ByVal Fabbricato_Cod As Int32,
                                        ByVal Tipo_Fabbricato As Int32,
                                        ByVal Piva_2 As String,
                                        ByVal Sa_Cod_2 As Int32,
                                        ByVal Vas_Cod As Int32,
                                        ByVal Flag_Indirizzo As Boolean,
                                         ByVal Tipo_Value_Desc As Integer,
                                        ByVal xFiltroAggiuntivoMag As String,
                                        ByVal xFiltroAggiuntivoVasca As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If xFiltroAggiuntivoMag <> "" Then
            xFiltroAggiuntivoMag &= " AND "
        End If

        Select Case Tipo_Fabbricato

            Case MAGAZZINO
                xFiltroAggiuntivoMag &= " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

            Case STALLA
                xFiltroAggiuntivoMag &= " ( (Fabbricati.Tipo_Fabbricato_Cod >= 170 AND Fabbricati.Tipo_Fabbricato_Cod < 180) OR Fabbricati.Tipo_Fabbricato_Cod = 70 ) "

            Case FABBRICATI_NO_STALLE
                xFiltroAggiuntivoMag &= " (Fabbricati.Tipo_Fabbricato_Cod < 170 OR Fabbricati.Tipo_Fabbricato_Cod >= 180) AND Fabbricati.Tipo_Fabbricato_Cod <> 70  "

        End Select


        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

        Dt = objFabbricati.Magazzini_UNION_Vasche(Piva_1,
                                                    Sa_Cod_1,
                                                    Fabbricato_Cod,
                                                   Piva_1,
                                                    Sa_Cod_1,
                                                    Vas_Cod,
                                                    Flag_Indirizzo,
                                                    xFiltroAggiuntivoMag,
                                                    xFiltroAggiuntivoVasca,
                                                    xOrderBy,
                                                    objParametri)


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                'Tipo_Value_Desc =0 -> codice: fabbricato_cod|sa_cod desc: magazzino (centro az.)
                'Tipo_Value_Desc =1 -> codice: fabbricato_cod desc: magazzino
                'Tipo_Value_Desc =2 -> codice: fabbricato_cod desc: magazzino (fabbricato_cod)
                Select Case Tipo_Value_Desc

                    Case 0
                        x_Cod = CStr(Dt.Rows(i).Item("Codice")) & "|" & CStr(Dt.Rows(i).Item("Sa_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Descrizione")) & " (" & CStr(Dt.Rows(i).Item("Sa_Nome")) & ")"

                    Case 1
                        x_Cod = Dt.Rows(i).Item("Codice")
                        x_Des = CStr(Dt.Rows(i).Item("Descrizione"))

                    Case 2
                        x_Cod = CStr(Dt.Rows(i).Item("Codice"))
                        x_Des = CStr(Dt.Rows(i).Item("Descrizione")) & " (" & CStr(Dt.Rows(i).Item("Codice")) & ")"

                    Case Else
                        x_Cod = CStr(Dt.Rows(i).Item("Codice")) & "|" & CStr(Dt.Rows(i).Item("Sa_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Descrizione")) & " (" & CStr(Dt.Rows(i).Item("Sa_Nome")) & ")"

                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub

    '###############################################################################
    'Tipo_Value_Desc = 0 Cal_Cod - Cal_Des
    'Tipo_Value_Desc = 1 Cal_Cod - Cal_Des (Cal_Cod)
    Public Shared Sub Registri_VociDiRiepilogo(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal PIVA As String,
                                                ByVal Cal_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Tipo_Cod As Integer,
                                                ByVal ChkRegistri As Integer,
                                                ByVal ChkRegistri_Vinificazione As Integer,
                                                ByVal Tipo_Value_Desc As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                )


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R

        Dt = objMP.Voci_di_Riepilogo(PIVA,
                                    Cal_Cod,
                                     Mat_Cod,
                                     Tipo_Cod,
                                     ChkRegistri,
                                    ChkRegistri_Vinificazione,
                                    xFiltroAggiuntivo,
                                    xOrderBy,
                                    objParametri)


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                Select Case Tipo_Value_Desc

                    Case 0
                        x_Cod = Dt.Rows(i).Item("Cal_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Cal_Des"))
                    Case 1
                        x_Cod = Dt.Rows(i).Item("Cal_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Cal_Des")) & " (" & CStr(Dt.Rows(i).Item("Cal_Cod")) & ") "
                    Case Else
                        x_Cod = Dt.Rows(i).Item("Cal_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Cal_Des"))
                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub

    '###############################################################################
    'mat_cod =0
    'Tipo_Value_Desc = 0 mat_Cod - mat_Des
    'Tipo_Value_Desc = 1 mat_Cod - mat_Des (cod_articolo)
    'Tipo_Value_Desc = 2 mat_Cod - mat_Des Cod.cod_articolo (mat_cod)
    'Tipo_Value_Desc = 3 mat_Cod - mat_Des (mat_Cod)
    Public Shared Sub MateriePrimeByVoceDiRiepilogo(ByRef Controllo As ListControl,
                                                    ByVal PrimaRiga_Flag As Boolean,
                                                    ByVal PrimaRiga_Text As String,
                                                    ByVal PrimaRiga_Value As String,
                                                    ByVal Cal_Cod As Integer,
                                                    ByVal Id_Report As Integer,
                                                    ByVal ChkRegistri As Integer,
                                                    ByVal ChkRegistri_Vinificazione As Integer,
                                                    ByVal PIVA As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Flag_Pubblico As Boolean,
                                                    ByVal Tipo_Value_Desc As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    )


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R

        Dt = objMP.LeggiMateriePrimeByVoceDiRiepilogo(Cal_Cod,
                                                        Id_Report,
                                                        ChkRegistri,
                                                        ChkRegistri_Vinificazione,
                                                        PIVA,
                                                        Elem_Cod,
                                                        Mat_Cod,
                                                        Flag_Pubblico,
                                                        xFiltroAggiuntivo,
                                                        xOrderBy,
                                                        objParametri)


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1


                'Tipo_Value_Desc = 2 mat_Cod - mat_Des Cod.cod_articolo (mat_cod)
                Select Case Tipo_Value_Desc
                    Case 0
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des"))
                    Case 1
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt.Rows(i).Item("Cod_articolo")) & ")"
                    Case 2
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " cod." & CStr(Dt.Rows(i).Item("Cod_articolo")) & " (" & CStr(Dt.Rows(i).Item("Mat_Cod")) & ")"
                    Case 3
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt.Rows(i).Item("Mat_Cod")) & ")"
                    Case Else
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des"))
                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub


    '###############################################################################
    'mat_cod =0
    'Tipo_Value_Desc = 0 mat_Cod - mat_Des
    'Tipo_Value_Desc = 1 mat_Cod - mat_Des (cod_articolo)
    'Tipo_Value_Desc = 2 mat_Cod - mat_Des Cod.cod_articolo (mat_cod)
    'Tipo_Value_Desc = 3 mat_Cod - mat_Des (mat_Cod)
    Public Shared Sub MateriePrimeByLinee(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Piva As String,
                                            ByVal Cau_Mov As String,
                                            ByVal Id_Report As Integer,
                                            ByVal Linea_Cod As Integer,
                                             ByVal Tipo_Value_Desc As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Dim objlinee As New AgronicaCoreContabDAL.Linee_Produzioni_R

        Dt = objlinee.MateriePrime_byLineeProduzioni(Piva,
                                                    Cau_Mov,
                                                    Id_Report,
                                                    Linea_Cod,
                                                    xFiltroAggiuntivo,
                                                    xOrderBy,
                                                    objParametri)


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1


                'Tipo_Value_Desc = 2 mat_Cod - mat_Des Cod.cod_articolo (mat_cod)
                Select Case Tipo_Value_Desc
                    Case 0
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des"))
                    Case 1
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt.Rows(i).Item("Cod_articolo")) & ")"
                    Case 2
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " cod." & CStr(Dt.Rows(i).Item("Cod_articolo")) & " (" & CStr(Dt.Rows(i).Item("Mat_Cod")) & ")"
                    Case 3
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt.Rows(i).Item("Mat_Cod")) & ")"
                    Case Else
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des"))
                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub

    '###############################################################################
    Public Shared Sub MateriePrimeByLinee_LogOmni(ByRef Controllo As ListControl,
                                                    ByVal PrimaRiga_Flag As Boolean,
                                                    ByVal PrimaRiga_Text As String,
                                                    ByVal PrimaRiga_Value As String,
                                                    ByVal Piva As String,
                                                    ByVal Linea_Cod As Integer,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Id_Report As Integer,
                                                    ByVal Tipo_Value_Desc As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    )

        '  ByVal Cau_Mov As String, _


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objlinee As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R

        Dt = objlinee.MateriePrime_JoinReport_byLinee(Piva,
                                                        0,
                                                        Linea_Cod,
                                                        enum_Omni_Modulo_Generazione.Cantine,
                                                        0, 0,
                                                        Elem_Cod,
                                                        0,
                                                        Id_Report,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        xFiltroAggiuntivo,
                                                        xOrderBy,
                                                        objParametri)


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1


                'Tipo_Value_Desc = 2 mat_Cod - mat_Des Cod.cod_articolo (mat_cod)
                Select Case Tipo_Value_Desc
                    Case 0
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des"))
                    Case 1
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt.Rows(i).Item("Cod_articolo")) & ")"
                    Case 2
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " cod." & CStr(Dt.Rows(i).Item("Cod_articolo")) & " (" & CStr(Dt.Rows(i).Item("Mat_Cod")) & ")"
                    Case 3
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt.Rows(i).Item("Mat_Cod")) & ")"
                    Case 4
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des")) & " cod." & CStr(Dt.Rows(i).Item("Cod_articolo")) & " (" & CStr(Dt.Rows(i).Item("Mat_Cod")) & ")"
                        If Dt.Rows(i).Item("ChkScollegamento") = 1 Then
                            x_Des &= " --- SCOLLEGATA"
                        End If
                    Case Else
                        x_Cod = Dt.Rows(i).Item("Mat_Cod")
                        x_Des = CStr(Dt.Rows(i).Item("Mat_Des"))
                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub


    '###############################################################################
    Public Shared Sub LineeByMateriePrime_LogOmni(ByRef Controllo As ListControl,
                                                    ByVal PrimaRiga_Flag As Boolean,
                                                    ByVal PrimaRiga_Text As String,
                                                    ByVal PrimaRiga_Value As String,
                                                    ByVal Piva As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Id_Report As Integer,
                                                    ByVal Tipo_Value_Desc As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objlinee As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R

        Dt = objlinee.Linee_JoinReport_byMateriePrime(Piva,
                                                        0,
                                                        0,
                                                        enum_Omni_Modulo_Generazione.Cantine,
                                                        0, 0,
                                                        Elem_Cod,
                                                        Mat_Cod,
                                                        Id_Report,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        xFiltroAggiuntivo,
                                                        xOrderBy,
                                                        objParametri)


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                Select Case Tipo_Value_Desc
                    Case 0
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Linea_Des"))
                    Case 1
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Denominazione")) & " " & CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod")) & ") "
                    Case 2
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod")) & ") "
                    Case 3
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Denominazione")) & " " & CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod")) & ") " & CStr(Dt.Rows(i).Item("Linea_Classe_Des"))
                    Case Else
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Linea_Des"))
                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub

    '###############################################################################################
    Public Shared Sub MagazzinoConferimento_OrgReferente(ByRef Controllo As ListControl,
                                                        ByVal PrimaRiga_Flag As Boolean,
                                                        ByVal PrimaRiga_Text As String,
                                                        ByVal PrimaRiga_Value As String,
                                                        ByVal PivaOrganismoReferente As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        )

        Dim i As Integer
        Dim x_Cod, x_Des As String
        Dim Dt_Mag As DataTable
        Dim FiltroAggiuntivoMag As String = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "


        'Dt_Mag = NewCom_Fabbricati_Leggi(objServer, objSession, objPage, _
        '                                PivaOrganismoReferente, , , , , , _
        '                                FiltroAggiuntivoMag, _
        '                                " ORDER BY Sa_Nome, Fabbricati.Fabbricato_Des ")

        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

        'Dt_Mag = objFabbricati.Leggi_2(PivaOrganismoReferente, _
        '                           0, _
        '                           0, _
        '                           0, _
        '                           FiltroAggiuntivoMag, _
        '                           " Sa_Nome, Fabbricati.Fabbricato_Des ", _
        '                           objParametri)

        Dt_Mag = objFabbricati.Leggi_Magazzini_Organismoreferente(PivaOrganismoReferente,
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  FiltroAggiuntivoMag,
                                                                  "",
                                                                  objParametri)


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        For i = 0 To Dt_Mag.Rows.Count - 1

            'x_Cod = CStr(Dt_Mag.Rows(i).Item("Fabbricato_Cod")) & "|" & CStr(Dt_Mag.Rows(i).Item("Sa_Cod"))
            'x_Des = CStr(Dt_Mag.Rows(i).Item("Fabbricato_Des")) & " (" & CStr(Dt_Mag.Rows(i).Item("Sa_Nome")) & ")"

            x_Cod = CStr(Dt_Mag.Rows(i).Item("Fabbricato_Cod")) & "|" & CStr(Dt_Mag.Rows(i).Item("Sa_Cod") & "|" & CStr(Dt_Mag.Rows(i).Item("Piva")))
            x_Des = CStr(Dt_Mag.Rows(i).Item("Fabbricato_Des")) & " (" & CStr(Dt_Mag.Rows(i).Item("rag_soc")) & ")"


            Controllo.Items.Add(New ListItem(x_Des, x_Cod))

        Next


    End Sub


    '###############################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' default 
    ''' Optional ByVal Flag_NoSemilavorati As Boolean = True, _
    ''' Optional ByVal Flag_AltriBeni As Boolean = False, _
    ''' Optional ByVal Flag_PrimaRiga As Boolean = True, _
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub CategorieMagazzino(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Elem_Cod As Integer,
                                ByVal Cau_Mov As String,
                                ByVal Lav_Cod As Integer,
                                ByVal Flag_NoSemilavorati As Boolean,
                                ByVal Flag_AltriBeni As Boolean,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri)

        Dim dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim FiltroAggGlob As String = ""

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Select Case Cau_Mov

            Case CAU_SCARICO, CAU_TRASFERIMENTO

                '===============================================================================================
                'Escludo la Possibilità di Effettuare uno Scarico di Macchine/Attrezzature + Consistenze Animali
                '-----------------------------------------------------------------------

                '=====================================================================================================
                'Escludo la Possibilità di Effettuare dei Trasferimenti Assurdi
                '-----------------------------------------------------------------------------------------------------

                FiltroAggGlob = " (Elem_Cod <> " & CStr(MACCHINE) & " AND Elem_Cod <> " & CStr(ZOO_CONSISTENZA) & ") "


            Case CAU_CARICO

                '=====================================================================================================
                'Escludo la Possibilità di Effettuare un Carico di Lavorati Aziendali Senza Imputazione di Magazzino
                '-----------------------------------------------------------------------------------------------------

                If Flag_NoSemilavorati Then
                    FiltroAggGlob = " (Elem_Cod <> " & CStr(MACCHINE) & " AND Elem_Cod <> " & CStr(ZOO_CONSISTENZA) & " AND Elem_Cod <> " & CStr(SEMILAVORATI_VEGETALI) & " AND Elem_Cod <> " & CStr(SEMILAVORATI_ANIMALI) & ") "
                Else
                    FiltroAggGlob = " (Elem_Cod <> " & CStr(MACCHINE) & " AND Elem_Cod <> " & CStr(ZOO_CONSISTENZA) & ") "
                End If


            Case CAU_CONFERIMENTO, CAU_ACCETTAZIONE_BENI

                '=====================================================================================================
                'Solo i Lavorati/Trasformati Aziendali Vegetali/Animali sono Validi
                '-----------------------------------------------------------------------------------------------------

                FiltroAggGlob = " (Elem_Cod = " & CStr(SEMILAVORATI_VEGETALI) & " OR Elem_Cod = " & CStr(SEMILAVORATI_ANIMALI) & " OR Elem_Cod = " & CStr(TRASFORMATI_VEGETALI) & " OR Elem_Cod = " & CStr(TRASFORMATI_ANIMALI) & " ) "

        End Select

        If xFiltroAggiuntivo <> "" Then
            'FiltroAggGlob &= " AND " & xFiltroAggiuntivo
            If FiltroAggGlob <> "" Then
                FiltroAggGlob &= " AND " & xFiltroAggiuntivo
            Else
                FiltroAggGlob &= xFiltroAggiuntivo
            End If

        End If

        Dim objCatergorieR As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
        Dim NumTotale As Integer

        dt = objCatergorieR.Leggi(Elem_Cod,
                                    "",
                                    False,
                                    FiltroAggGlob,
                                    "",
                                    objParametri)

        NumTotale = 0

        If Not IsNothing(dt) Then

            NumTotale = dt.Rows.Count

            For i = 0 To dt.Rows.Count - 1

                x_Cod = dt.Rows(i).Item("elem_cod")
                x_Des = dt.Rows(i).Item("NomeComune")

                If Not IsDBNull(x_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Cod)) Then

                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))

                End If

            Next


            Select Case Lav_Cod
                Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA
                    'Fittizio
                    Controllo.Items.Add(New ListItem(Gias.Servizi, SERVIZI))
            End Select

            Select Case Cau_Mov
                Case CAU_CONFERIMENTO, CAU_TRASFERIMENTO
                    'do nothing
                Case Else
                    If Flag_AltriBeni Then
                        Controllo.Items.Add(New ListItem(Gias.AltriBeniStrumentali, ALTRI_BENI))
                    End If
            End Select

        End If

        dt = Nothing

    End Sub

    '######################################################################
    'x Fruttagel
    Public Shared Sub Prefisso_BolleAccettazione_FRG(ByRef Controllo As ListControl,
                                                    ByVal PrimaRiga_Flag As Boolean,
                                                    ByVal PrimaRiga_Text As String,
                                                    ByVal PrimaRiga_Value As String,
                                                    ByVal Descr_Magazzino As String)

        Dim Anno As Integer
        Dim Prefisso_FRG As String
        Dim Codice_Alfonsine1_Larino7 As Integer
        Dim objADD As New AgronicaCoreContabHLP.AccettazioneDaDiversi

        Codice_Alfonsine1_Larino7 = objADD.CodStabilimentoFRG_from_DescStabilimentoFRG(Descr_Magazzino)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        For Anno = 2010 To Date.Now.Year

            Prefisso_FRG = Right(CStr(Anno), 2) + CStr(Codice_Alfonsine1_Larino7)

            Controllo.Items.Add(New ListItem(Prefisso_FRG,
                                             Prefisso_FRG))


        Next



    End Sub


    '######################################################################
    'x Fruttagel
    Public Shared Sub Prefisso_BolleAccettazioneFRG_from_Codice(ByRef Controllo As ListControl,
                                                                                ByVal PrimaRiga_Flag As Boolean,
                                                                                ByVal PrimaRiga_Text As String,
                                                                                ByVal PrimaRiga_Value As String,
                                                                                ByVal Codice_Alfonsine1_Larino7 As Integer)

        Dim Anno As Integer
        Dim Prefisso_FRG As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        For Anno = 2010 To Date.Now.Year

            Prefisso_FRG = Right(CStr(Anno), 2) + CStr(Codice_Alfonsine1_Larino7)

            Controllo.Items.Add(New ListItem(Prefisso_FRG,
                                             Prefisso_FRG))


        Next



    End Sub

    '######################################################################
    'maga: non ho capito perché legge anche i beni di confezionamento animale
    Public Shared Sub Imballaggi(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Piva As String,
                                        ByVal RicercaNomeProdotto As String,
                                        ByVal RicercaCodArticolo As String,
                                        ByVal Cod_Articolo As String,
                                        ByVal Mat_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )


        Dim Dt_Materie_Prime As DataTable
        Dim i As Integer
        Dim x_Mat_Cod As Integer
        Dim x_Mat_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If xFiltroAggiuntivo <> "" Then
            xFiltroAggiuntivo &= " AND "
        End If
        xFiltroAggiuntivo &= " chkimballaggio=1 "

        If RicercaCodArticolo <> "" Then
            xFiltroAggiuntivo &= " AND Cod_Articolo LIKE '%" & RicercaCodArticolo & "%'"
        End If


        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        'Beni confezione VEGETALI - elem_cod=205
        Dt_Materie_Prime = objMP.MateriePrime_Anagrafica(
                                                Piva,
                                              0,
                                              BENI_CONFEZ_VEGETALE,
                                              Mat_Cod,
                                              Cod_Articolo,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              RicercaNomeProdotto,
                                               0, "", False,
                                              xFiltroAggiuntivo,
                                              "",
                                              objParametri_Server,
                                              objParametri_Utenti)



        If Not IsNothing(Dt_Materie_Prime) Then
            For i = 0 To Dt_Materie_Prime.Rows.Count - 1

                x_Mat_Cod = Dt_Materie_Prime.Rows(i).Item("Mat_Cod")
                x_Mat_Des = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"

                If Not IsDBNull(x_Mat_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Mat_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Mat_Des, x_Mat_Cod))
                End If

            Next
        End If

        'Beni confezione ANIMALI - elem_cod=305
        Dt_Materie_Prime = objMP.MateriePrime_Anagrafica(
                                         Piva,
                                       0,
                                       BENI_CONFEZ_ANIMALE,
                                       Mat_Cod,
                                       Cod_Articolo,
                                       0,
                                       0,
                                       0,
                                       0,
                                       0,
                                       0,
                                       0,
                                       RicercaNomeProdotto,
                                        0, "", False,
                                       xFiltroAggiuntivo,
                                       "",
                                       objParametri_Server,
                                       objParametri_Utenti)



        If Not IsNothing(Dt_Materie_Prime) Then
            For i = 0 To Dt_Materie_Prime.Rows.Count - 1

                x_Mat_Cod = Dt_Materie_Prime.Rows(i).Item("Mat_Cod")
                x_Mat_Des = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"

                If Not IsDBNull(x_Mat_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Mat_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Mat_Des, x_Mat_Cod))
                End If

            Next
        End If

        Dt_Materie_Prime = Nothing

    End Sub

    '######################################################################
    'uguale alla imballaggi, ma cerca anche contenitori e confezioni
    Public Shared Sub BeniConfezionamentoVegetale(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal Piva As String,
                                                ByVal RicercaNomeProdotto As String,
                                                ByVal RicercaCodArticolo As String,
                                                ByVal Cod_Articolo As String,
                                                ByVal Mat_Cod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                )


        Dim Dt_Materie_Prime As DataTable
        Dim i As Integer
        Dim x_Mat_Cod As Integer
        Dim x_Mat_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        '''xFiltroAggiuntivo &= " chkimballaggio=1 "

        If RicercaCodArticolo <> "" Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo &= " AND "
            End If
            xFiltroAggiuntivo &= " Cod_Articolo LIKE '%" & RicercaCodArticolo & "%'"
        End If


        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        'Beni confezione VEGETALI - elem_cod=205
        Dt_Materie_Prime = objMP.MateriePrime_Anagrafica(
                                                Piva,
                                              0,
                                              BENI_CONFEZ_VEGETALE,
                                              Mat_Cod,
                                              Cod_Articolo,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              RicercaNomeProdotto,
                                               0, "", False,
                                              xFiltroAggiuntivo,
                                              "",
                                              objParametri_Server,
                                              objParametri_Utenti)



        If Not IsNothing(Dt_Materie_Prime) Then
            For i = 0 To Dt_Materie_Prime.Rows.Count - 1

                x_Mat_Cod = Dt_Materie_Prime.Rows(i).Item("Mat_Cod")
                x_Mat_Des = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"

                If Not IsDBNull(x_Mat_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Mat_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Mat_Des, x_Mat_Cod))
                End If

            Next
        End If

        Dt_Materie_Prime = Nothing

    End Sub

    ''''######################################################################
    Public Shared Sub ProdottiConferiti(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Piva As String,
                                        ByVal RicercaNomeProdotto As String,
                                        ByVal RicercaCodArticolo As String,
                                        ByVal Cod_Articolo As String,
                                        ByVal Mat_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        Dim Dt_Materie_Prime As DataTable
        Dim i As Integer
        Dim x_Mat_Cod As Integer
        Dim x_Mat_Des As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If RicercaCodArticolo <> "" Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo &= " AND "
            End If
            xFiltroAggiuntivo &= " Cod_Articolo LIKE '%" & RicercaCodArticolo & "%'"
        End If


        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        'Aggiungo i Trasformati vegetali
        Dt_Materie_Prime = objMP.MateriePrime_ProdottiConferiti(
                                              Piva,
                                              0,
                                              TRASFORMATI_VEGETALI,
                                              Mat_Cod,
                                              Cod_Articolo,
                                              0, 0,
                                              RicercaNomeProdotto,
                                              xFiltroAggiuntivo,
                                              "",
                                              objParametri_Server,
                                              objParametri_Utenti)



        If Not IsNothing(Dt_Materie_Prime) Then
            For i = 0 To Dt_Materie_Prime.Rows.Count - 1

                x_Mat_Cod = Dt_Materie_Prime.Rows(i).Item("Mat_Cod")
                x_Mat_Des = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"

                If Not IsDBNull(x_Mat_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Mat_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Mat_Des, x_Mat_Cod))
                End If

            Next
        End If



        'Aggiungo i Trasformati animali 
        Dt_Materie_Prime = objMP.MateriePrime_ProdottiConferiti(
                                              Piva,
                                              0,
                                              TRASFORMATI_ANIMALI,
                                              Mat_Cod,
                                              Cod_Articolo,
                                              0, 0,
                                              RicercaNomeProdotto,
                                              xFiltroAggiuntivo,
                                              "",
                                              objParametri_Server,
                                              objParametri_Utenti)



        If Not IsNothing(Dt_Materie_Prime) Then
            For i = 0 To Dt_Materie_Prime.Rows.Count - 1

                x_Mat_Cod = Dt_Materie_Prime.Rows(i).Item("Mat_Cod")
                x_Mat_Des = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des")) & " (" & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"

                If Not IsDBNull(x_Mat_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Mat_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Mat_Des, x_Mat_Cod))
                End If

            Next
        End If


        'Dt_Materie_Prime.Dispose()
        Dt_Materie_Prime = Nothing

    End Sub


    '######################################################################
    'valori di default
    'Cod_Progetto As Integer = CODPROGETTO_NONDEFINITO
    'Lotto As String = LOTTO_NONDEFINITO
    Public Sub Materie_Prime(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Cau_Mov As String,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Id_Destinazione As Integer,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Flag_Negativo As Boolean,
                                        ByVal RicercaNomeProdotto As String,
                                        ByVal RicercaLottoAccettazione As String,
                                        ByVal RicercaCodArticolo As String,
                                        ByVal Cod_Articolo As String,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Udm_Cod As Integer,
                                        ByVal Cal_Cod As Integer,
                                        ByVal Cod_Progetto As Integer,
                                        ByVal Fase_Cod As Integer,
                                        ByVal Lotto As String,
                                        ByVal Veg_Cod As Integer,
                                        ByVal Cul_Cod As Integer,
                                        ByVal Gen_Cod As Integer,
                                        ByVal Spe_Cod As Integer,
                                        ByVal Raz_Cod As Integer,
                                        ByVal Ipro_Cod As Integer,
                                        ByVal Cat_Cod As Integer,
                                        ByVal Data_Giacenza As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal passa_Dt_Materie_Prime As Boolean = False,
                                        Optional ByRef Dt_Materie_Prime As DataTable = Nothing,
                                        Optional ByVal Flag_QtaNoZero As Boolean = False,
                                        Optional ByVal Flag_QtaMaggioreZero As Boolean = False,
                                        Optional ByVal LeggiAlias As Boolean = False,
                                        Optional ByVal gruppiMerceDefaultPerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = Nothing,
                                        Optional ByVal inibisciVisibilitaGruppiMerce As Boolean = False)

        Dim i As Integer
        Dim x_Mat_Cod As Integer
        Dim x_Mat_Des As String = ""


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Select Case Cau_Mov

            Case CAU_CARICO

                '#############################################
                '##############     CARICO    ################
                '#############################################

                Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                If RicercaCodArticolo <> "" Then
                    If xFiltroAggiuntivo <> "" Then
                        xFiltroAggiuntivo &= " AND "
                    End If
                    xFiltroAggiuntivo &= " Cod_Articolo LIKE '%" & RicercaCodArticolo & "%'"
                End If
                If Not LeggiAlias Then
                    If xFiltroAggiuntivo <> "" Then
                        xFiltroAggiuntivo &= " AND "
                    End If
                    xFiltroAggiuntivo &= " Materie_Prime.ChkAlias = 0 "
                End If
                Dt_Materie_Prime = objMP.MateriePrime_Anagrafica(
                                                        Piva,
                                                      0,
                                                      Elem_Cod,
                                                      Mat_Cod,
                                                      Cod_Articolo,
                                                      Veg_Cod,
                                                      Cul_Cod,
                                                      Gen_Cod,
                                                      Spe_Cod,
                                                      Raz_Cod,
                                                      Ipro_Cod,
                                                      Cat_Cod,
                                                      RicercaNomeProdotto,
                                                       0, "", False,
                                                      xFiltroAggiuntivo,
                                                      "",
                                                      objParametri_Server,
                                                      objParametri_Utenti,
                                                      gruppiMerceDefaultPerCategoria, inibisciVisibilitaGruppiMerce)



            Case CAU_SCARICO

                '#############################################
                '##############     SCARICO    ###############
                '#############################################

                'leggo le materie prime in magazzino

                'Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                'Dt_Materie_Prime = objMP.MateriePrime_Giacenze( _
                '                            Piva, _
                '                            Sa_Cod, _
                '                            Elem_Cod, _
                '                            0, _
                '                            Mat_Cod, _
                '                            0, _
                '                            Id_Destinazione, _
                '                            Cal_Cod, _
                '                            Cod_Progetto, _
                '                            0, _
                '                            Lotto, _
                '                            Cod_Articolo, _
                '                            Veg_Cod, _
                '                            Cul_Cod, _
                '                            0, _
                '                            0, _
                '                            0, _
                '                            0, _
                '                            0, _
                '                            RicercaNomeProdotto, _
                '                            RicercaLottoAccettazione, _
                '                            RicercaCodArticolo, _
                '                            0, "", False, _
                '                            "", "", _
                '                            objParametri_Server)

                If xFiltroAggiuntivo <> "" Then
                    xFiltroAggiuntivo = " AND " & xFiltroAggiuntivo
                End If

                Dim xFiltroAggiuntivo_MateriePrime As String = ""
                Dim xFiltroAggiuntivo_Semilavorati As String = ""
                Dim xFiltroAggiuntivo_TrasformatiVeg As String = ""
                Dim xFiltroAggiuntivo_TrasformatiAnimali As String = ""

                Dim isFreshAndFood As Boolean = False
                If Elem_Cod = 0 OrElse Elem_Cod = SEMILAVORATI_VEGETALI OrElse Elem_Cod = TRASFORMATI_VEGETALI OrElse Elem_Cod = TRASFORMATI_ANIMALI Then
                    Dim w_Modulo_Anagrafe_Log As Integer = 0
                    Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
                    Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(Piva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                                                               "", "",
                                                               objParametri_Server)
                    If dtAnagrafeLog IsNot Nothing AndAlso dtAnagrafeLog.Rows.Count > 0 Then
                        w_Modulo_Anagrafe_Log = CInt(dtAnagrafeLog.Rows(0).Item("Modulo_Generazione"))
                    End If
                    dtAnagrafeLog = Nothing
                    If w_Modulo_Anagrafe_Log = 2 OrElse w_Modulo_Anagrafe_Log = 3 OrElse w_Modulo_Anagrafe_Log = 5 Then
                        isFreshAndFood = True
                    End If
                End If

                If Elem_Cod = 0 Then

                    xFiltroAggiuntivo &= " AND Movimenti_Dettagli.Elem_Cod IN ( " & Agro_SQL_SaveNum(ALTRE_MATERIE) & ", " &
                            Agro_SQL_SaveNum(SEMILAVORATI_VEGETALI) & ", " &
                            Agro_SQL_SaveNum(MATERIE_VEGETALI) & ", " &
                            Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & ", " &
                            Agro_SQL_SaveNum(TRASFORMATI_VEGETALI) & ", " &
                            Agro_SQL_SaveNum(SEMILAVORATI_ANIMALI) & ", " &
                            Agro_SQL_SaveNum(MATERIE_ANIMALI) & ", " &
                            Agro_SQL_SaveNum(BENI_CONFEZ_ANIMALE) & ", " &
                            Agro_SQL_SaveNum(TRASFORMATI_ANIMALI) & ", " &
                            Agro_SQL_SaveNum(MANGIMI) & ", " &
                            Agro_SQL_SaveNum(FARMACI) & ", " &
                            Agro_SQL_SaveNum(RICAMBI) & ", " &
                            Agro_SQL_SaveNum(ALTRI_BENI_AMMORTIZZABILI) & ", " &
                            Agro_SQL_SaveNum(CARBURANTI) & ", " &
                            Agro_SQL_SaveNum(CAT_MAG_SERVIZI_PROFESSIONALI) & ") "

                End If

                'If RicercaNomeProdotto <> "" Then
                '    StrSQL.Append(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaNomeProdotto) & "%'   " & vbCrLf)
                'End If
                If RicercaNomeProdotto <> "" Then
                    'If xFiltroAggiuntivo <> "" Then
                    '    xFiltroAggiuntivo &= " AND "
                    'End If

                    Dim strLikeDescr = " AND (Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaNomeProdotto) & "%' OR Materie_Prime.Cod_Articolo like '%" & Agro_SQL_SaveText(RicercaNomeProdotto) & "%' )  " & vbCrLf
                    xFiltroAggiuntivo_MateriePrime &= strLikeDescr
                    xFiltroAggiuntivo_Semilavorati &= strLikeDescr
                    xFiltroAggiuntivo_TrasformatiVeg &= strLikeDescr
                    xFiltroAggiuntivo_TrasformatiAnimali &= strLikeDescr
                End If

                'If RicercaLottoAccettazione <> "" Then
                '    StrSQL.Append(" AND Movimenti_Dettagli.Lotto like '%" & Agro_SQL_SaveText(RicercaLottoAccettazione) & "%'   " & vbCrLf)
                'End If
                If RicercaLottoAccettazione <> "" Then
                    'If xFiltroAggiuntivo <> "" Then
                    '    xFiltroAggiuntivo &= " AND "
                    'End If
                    xFiltroAggiuntivo &= " AND Movimenti_Dettagli.Lotto like '%" & Agro_SQL_SaveText(RicercaLottoAccettazione) & "%'   " & vbCrLf
                End If

                'If RicercaCodArticolo <> "" Then
                '    StrSQL.Append(" AND Materie_Prime.Cod_Articolo like '%" & Agro_SQL_SaveText(RicercaCodArticolo) & "%'   " & vbCrLf)
                'End If
                If RicercaCodArticolo <> "" Then
                    'If xFiltroAggiuntivo <> "" Then
                    '    xFiltroAggiuntivo &= " AND "
                    'End If
                    xFiltroAggiuntivo &= " AND Materie_Prime.Cod_Articolo like '%" & Agro_SQL_SaveText(RicercaCodArticolo) & "%'   " & vbCrLf
                End If

                Dim objG As New AgronicaCoreContabDAL.Giacenze_R
                Dt_Materie_Prime = objG.SchedaGiacenzeMagazzino(Data_Giacenza,
                                                    Piva,
                                                    Sa_Cod,
                                                    Id_Destinazione,
                                                    Elem_Cod,
                                                    0,
                                                    Mat_Cod,
                                                    Cal_Cod,
                                                    Cod_Progetto,
                                                    0,
                                                    0,
                                                    Lotto,
                                                    Flag_QtaNoZero,
                                                    xFiltroAggiuntivo,
                                                    "", "", "", "", "", "", xFiltroAggiuntivo_MateriePrime, "",
                                                    xFiltroAggiuntivo_Semilavorati, xFiltroAggiuntivo_TrasformatiVeg,
                                                    "",
                                                    objParametri_Server, objParametri_Utenti, xFiltroAggiuntivo_Semilavorati, isFreshAndFood:=isFreshAndFood,
                                                    xFiltroAggiuntivo_14:=xFiltroAggiuntivo_Semilavorati, Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero,
                                                    xFiltroAggiuntivo_15:=xFiltroAggiuntivo_TrasformatiVeg, gruppiMerceDefaultPerCategoria:=gruppiMerceDefaultPerCategoria, inibisciVisibilitaGruppiMerce:=inibisciVisibilitaGruppiMerce)


        End Select

        If Not IsNothing(Dt_Materie_Prime) Then

            For i = 0 To Dt_Materie_Prime.Rows.Count - 1

                If Flag_Negativo Then
                    x_Mat_Cod = -Dt_Materie_Prime.Rows(i).Item("Mat_Cod")
                Else
                    x_Mat_Cod = Dt_Materie_Prime.Rows(i).Item("Mat_Cod")
                End If

                Select Case Cau_Mov
                    Case CAU_CARICO
                        x_Mat_Des = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des"))
                        If Not String.IsNullOrWhiteSpace(CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo"))) Then
                            x_Mat_Des = x_Mat_Des & " (" & Gias.CodArticolo & ": " & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"
                        End If
                    Case CAU_SCARICO
                        x_Mat_Des = CStr(Dt_Materie_Prime.Rows(i).Item("Descrizione_Prodotto"))
                        If Not String.IsNullOrWhiteSpace(CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo"))) Then
                            x_Mat_Des = x_Mat_Des & " (" & Gias.CodArticolo & ": " & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"
                        End If
                End Select


                If Not IsDBNull(x_Mat_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Mat_Cod)) Then

                    Controllo.Items.Add(New ListItem(x_Mat_Des, x_Mat_Cod))

                End If

            Next
        End If

        'Dt_Materie_Prime.Dispose()
        If Not passa_Dt_Materie_Prime Then
            Dt_Materie_Prime = Nothing
        End If

    End Sub


    '######################################################################
    'tipo_Value: 
    '1 -> mat_cod
    '2 -> mat_cod | elem_cod
    Public Shared Sub Materie_PrimeXReport(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Piva As String,
                                        ByVal Pro_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Id_Report As Integer,
                                        ByVal Tipo As Integer,
                                        ByVal Tipo_Value As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         )

        Dim Dt_Materie_Prime As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_R

        Dt_Materie_Prime = objMP.Leggi_2(Piva,
                                            Pro_Cod,
                                            Mat_Cod,
                                            Id_Report,
                                            Tipo,
                                            xFiltroAggiuntivo,
                                             "",
                                              objParametri_Server)


        If Not IsNothing(Dt_Materie_Prime) Then

            For i = 0 To Dt_Materie_Prime.Rows.Count - 1

                Select Case Tipo_Value
                    Case 2
                        x_Cod = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Cod")) & "|" & CStr(Dt_Materie_Prime.Rows(i).Item("elem_Cod"))
                    Case Else
                        x_Cod = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Cod"))
                End Select

                Select Case Dt_Materie_Prime.Rows(i).Item("Elem_Cod")
                    Case SEMILAVORATI_VEGETALI, SEMILAVORATI_ANIMALI, SEMENTI, ALTRE_MATERIE
                        x_Des = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des")) &
                        " (" & Gias.CodArticolo & ": " & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"
                    Case Else
                        x_Des = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des"))
                End Select

                If Not IsDBNull(x_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Cod)) Then

                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))

                End If

            Next

        End If

        Dt_Materie_Prime = Nothing

    End Sub



    '######################################################################
    Public Shared Sub Linee_Produzioni_Preparazioni(ByRef Controllo As ListControl,
                                                    ByVal PrimaRiga_Flag As Boolean,
                                                    ByVal PrimaRiga_Text As String,
                                                    ByVal PrimaRiga_Value As String,
                                                    ByVal Piva As String,
                                                    ByVal Id_Report As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Dim objLinee As New AgronicaCoreContabDAL.Linee_Produzioni_R

        Dt = objLinee.LineeProduzioni_UNION_LineePreparazioni_By_MatCod_IdReport(
                                                                Piva,
                                                                Id_Report,
                                                                Mat_Cod,
                                                                xFiltroAggiuntivo,
                                                                "",
                                                                objParametri_Server)

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod")) & "|" & CStr(Dt.Rows(i).Item("preparazione_cod"))
                x_Des = CStr(Dt.Rows(i).Item("Descrizione"))

                If Not IsDBNull(x_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                End If

            Next

        End If


    End Sub

    '######################################################################
    Public Shared Sub LineeProduzioni_ContattiTerzi(ByRef Controllo As ListControl,
                                                   ByVal PrimaRiga_Flag As Boolean,
                                                   ByVal PrimaRiga_Text As String,
                                                   ByVal PrimaRiga_Value As String,
                                                   ByVal Piva As String,
                                                   ByVal Piva_Contatto As String,
                                                   ByVal Cod_Contatto As String,
                                                   ByVal Linea_Cod As Integer,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                   )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objLinee As New AgronicaCoreContabDAL.Linee_Produzioni_R

        Dt = objLinee.LineeProduzioni_ContattiTerzi(Piva,
                                                   Piva_Contatto,
                                                   Cod_Contatto,
                                                   0,
                                                  xFiltroAggiuntivo,
                                                  xOrderBy,
                                                  objParametri_Server)

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = CStr(Dt.Rows(i).Item("Cod_Contatto"))
                x_Des = CStr(Dt.Rows(i).Item("Rag_Soc"))

                If x_Des = "" Then
                    x_Des = CStr(Dt.Rows(i).Item("Nome")) & " " & CStr(Dt.Rows(i).Item("Cognome"))
                End If

                If Not IsDBNull(x_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                End If

            Next

        End If


    End Sub

    '######################################################################
    Public Shared Sub Linee_Produzioni_TipoDefault(ByRef Controllo As ListControl,
                                                   ByVal PrimaRiga_Flag As Boolean,
                                                   ByVal PrimaRiga_Text As String,
                                                   ByVal PrimaRiga_Value As String,
                                                   ByVal Piva As String,
                                                   ByVal Tipo_Default As Integer,
                                                   ByVal Tipo_Value_Desc As Integer,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                   )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objLinee As New AgronicaCoreContabDAL.Linee_Produzioni_R

        Dt = objLinee.LineeProduzioni_TipoDefault(Piva,
                                                  Tipo_Default,
                                                  xFiltroAggiuntivo,
                                                  "",
                                                  objParametri_Server)

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))

                ' x_Des = CStr(Dt.Rows(i).Item("Linea_Des"))
                Select Case Tipo_Value_Desc
                    Case 0
                        x_Des = CStr(Dt.Rows(i).Item("Linea_Des"))
                    Case 1
                        x_Des = CStr(Dt.Rows(i).Item("Denominazione")) & " " & CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod")) & ") "
                    Case 2
                        x_Des = CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod")) & ") "
                    Case 3
                        x_Des = CStr(Dt.Rows(i).Item("Denominazione")) & " " & CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod")) & ") "
                    Case 4
                        x_Des = CStr(Dt.Rows(i).Item("Denominazione")) & " " & CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod_Des")) & ") "
                    Case Else
                        x_Des = CStr(Dt.Rows(i).Item("Linea_Des"))
                End Select

                If Not IsDBNull(x_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                End If

            Next

        End If


    End Sub

    '######################################################################
    Public Shared Sub Linee_Produzioni(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Piva As String,
                                        ByVal Cau_Mov As String,
                                        ByVal Id_Report As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Tipo_Value_Desc As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Dim objLinee As New AgronicaCoreContabDAL.Linee_Produzioni_R

        Dt = objLinee.LineeProduzioni(
                                    Piva,
                                    Cau_Mov,
                                    Id_Report,
                                    Mat_Cod,
                                    xFiltroAggiuntivo,
                                    "",
                                    objParametri_Server)

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                Select Case Tipo_Value_Desc
                    Case 0
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Linea_Des"))
                    Case 1
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Denominazione")) & " " & CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod")) & ") "
                    Case 2
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod")) & ") "
                    Case 3
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Denominazione")) & " " & CStr(Dt.Rows(i).Item("Linea_Des")) & " (" & CStr(Dt.Rows(i).Item("Linea_Cod")) & ") " & CStr(Dt.Rows(i).Item("Linea_Classe_Des"))
                    Case Else
                        x_Cod = CStr(Dt.Rows(i).Item("Linea_Cod"))
                        x_Des = CStr(Dt.Rows(i).Item("Linea_Des"))
                End Select

                If Not IsDBNull(x_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                End If

            Next

        End If


    End Sub


    '######################################################################
    Public Shared Sub Partite_LineeProduzioni(ByRef Controllo As ListControl,
                                              ByVal PrimaRiga_Flag As Boolean,
                                              ByVal PrimaRiga_Text As String,
                                              ByVal PrimaRiga_Value As String,
                                              ByVal Piva As String,
                                              ByVal Linea_Cod As Integer,
                                              ByVal Id_Report As Integer,
                                              ByVal Flag_ScartaPartiteNonValorizzate As Boolean,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objLinee As New AgronicaCoreContabDAL.MovimentixReport_R

        Dt = objLinee.Leggi_Partite_LineeProduzioni(Piva,
                                                  Linea_Cod,
                                                  Id_Report,
                                                  Flag_ScartaPartiteNonValorizzate,
                                                  xFiltroAggiuntivo,
                                                  xOrderBy,
                                                  objParametri_Server)

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = CStr(CStr(Dt.Rows(i).Item("Id_Agenda")) & "|" & CStr(Dt.Rows(i).Item("Id_Mov")) & "|" & CStr(Dt.Rows(i).Item("Id_Report")))
                x_Des = CStr(Dt.Rows(i).Item("Descrizione"))

                If Not IsDBNull(x_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                End If

            Next

        End If


    End Sub

    '###############################################################################
    'OBSOLETA:
    'USARE ProdottiAnagrafica oppure ProdottiMagazzino
    'default:
    'Flag_VisualizzaProCod As Boolean = True
    'Public Shared Sub Prodotti(ByRef Controllo As ListControl, _
    '                            ByVal PrimaRiga_Flag As Boolean, _
    '                            ByVal PrimaRiga_Text As String, _
    '                            ByVal PrimaRiga_Value As String, _
    '                            ByVal Cau_Mov As String, _
    '                            ByVal Piva As String, _
    '                            ByVal Sa_Cod As Integer, _
    '                            ByVal Id_Destinazione As Integer, _
    '                            ByVal Elem_Cod As Integer, _
    '                            ByVal Flag_Negativo As Boolean, _
    '                            ByVal RicercaTesto As String, _
    '                            ByVal Flag_VisualizzaProCod As Boolean, _
    '                            ByVal Pro_Cod As Integer, _
    '                            ByVal Udm_Cod As Integer, _
    '                            ByVal DataFiltroFormulati As Date, _
    '                            ByVal PUA_RegolamentoCod As Integer, _
    '                            ByVal xFiltroAggiuntivo As String, _
    '                            ByVal xOrderBy As String, _
    '                            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                            ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            )


    '    Dim Dt_Categorie As DataTable
    '    Dim Dt_Giacenze As DataTable
    '    Dim Dt_Prodotti, Dt_FertAzi As DataTable
    '    Dim i, j As Integer
    '    Dim x_Pro_Cod As Integer
    '    Dim x_Pro_Des As String
    '    Dim NomeTabella As String
    '    Dim NomeCodice As String
    '    Dim NomeDescrizione As String


    '    'Pulisco il controllo
    '    Controllo.Items.Clear()

    '    If PrimaRiga_Flag Then
    '        Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
    '    End If

    '    Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
    '    Dim objCat As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

    '    Select Case Cau_Mov

    '        Case CAU_CARICO

    '            '#############################################
    '            '##############     CARICO    ################
    '            '#############################################

    '            'Leggo l'anagrafica prodotti

    '            'legge le categorie di magazzino
    '            Dt_Categorie = objCat.Leggi(Elem_Cod, _
    '                                         CAU_MAGAZZINO, _
    '                                          False, _
    '                                          "", "", _
    '                                          objParametri_server)

    '            Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R


    '            'la categoria di magazzino è 1
    '            For i = 0 To Dt_Categorie.Rows.Count - 1

    '                NomeTabella = Dt_Categorie.Rows(i).Item("Tabella")
    '                NomeCodice = Dt_Categorie.Rows(i).Item("Tabella_Cod")
    '                NomeDescrizione = Dt_Categorie.Rows(i).Item("Tabella_Des")

    '                Select Case Elem_Cod

    '                    Case FERTILIZZANTI

    '                        'per i fertilizzanti non uso più la funzione NewCom_LeggiTabella_da_CategorieMagazzino
    '                        'perché oltre alla tabella fertilizzanti, devo leggere anche la tabella materie_prime 
    '                        '(per i fertilizzanti aziendali)

    '                        'Dt_Prodotti = NewCom_Fertilizzanti_Completa_Leggi(objServer, objSession, objPage, _
    '                        '                                                   Piva, _
    '                        '                                                   0, _
    '                        '                                                   True, _
    '                        '                                                   RicercaTesto)

    '                        Dim TipoRichiesto As Integer = 0

    '                        Select Case PUA_RegolamentoCod
    '                            Case 0
    '                                TipoRichiesto = 0
    '                            Case 1
    '                                TipoRichiesto = 6
    '                            Case Else
    '                                TipoRichiesto = 7
    '                        End Select

    '                        Dt_Prodotti = objFert.Leggi_Completa(0, _
    '                                                            RicercaTesto, _
    '                                                            TipoRichiesto, _
    '                                                            True, _
    '                                                            Piva, _
    '                                                            0, _
    '                                                            objParametri_server.FinestraTemporaleInizio, _
    '                                                            objParametri_server.FinestraTemporaleFine, _
    '                                                            "", "  " & NomeDescrizione & " ", _
    '                                                            objParametri_server, _
    '                                                            PUA_RegolamentoCod)

    '                    Case Else

    '                        Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella, _
    '                                                                             NomeCodice, _
    '                                                                             NomeDescrizione, _
    '                                                                             RicercaTesto, _
    '                                                                             0, "", " " & NomeDescrizione & " ", _
    '                                                                             objParametri_server)





    '                        '''''''''''''''''''''''''''''modifica webservice
    '                        Dim XML_Credenziali As System.Xml.XmlElement
    '                        Dim StrCredenziali As String = ""
    '                        Dim StrParametri As String = ""
    '                        Dim Parametri As String = ""
    '                        Dim strErr As String = ""
    '                        Dim LastFr_Des As String = ""


    '                        Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
    '                        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

    '                        Dim objAgroWebConfig As New AgroWebConfig
    '                        Dim XmlDoc As New System.Xml.XmlDocument


    '                        objWs.NewWS(ObjDownloadWs, _
    '                                        objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci, _
    '                                        objParametri_utenti)

    '                        Try



    '                            XmlDoc = New System.Xml.XmlDocument

    '                            Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

    '                            Dim str_fr_cod As String = ""

    '                            Dim jj As Integer
    '                            For jj = 0 To Dt_Prodotti.Rows.Count - 1
    '                                If str_fr_cod.Length = 0 Then
    '                                    str_fr_cod = Dt_Prodotti.Rows(jj).Item("fr_cod")
    '                                Else
    '                                    str_fr_cod = str_fr_cod & "," & Dt_Prodotti.Rows(jj).Item("fr_cod")
    '                                End If
    '                            Next
    '                            objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                            StrCredenziali, _
    '                                                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo, _
    '                                                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo), _
    '                                                            HttpContext.Current.Session("ASG_ProgressivoGIAS"), _
    '                                                            HttpContext.Current.Session("ASG_SuperUser_Username").ToString, _
    '                                                            HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

    '                            XmlDoc.LoadXml(StrCredenziali)

    '                            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

    '                            'Dim grfi_Cod As Integer = 0
    '                            objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                                   StrParametri, _
    '                                                                   str_fr_cod, _
    '                                                                   strErr)

    '                            XML_Credenziali.InnerXml = StrParametri

    '                            Parametri = XmlDoc.OuterXml

    '                            Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)
    '                            Dim dt As DataTable
    '                            dt = ObjDownloadWs.Leggi_Formulati_Info_DT(Parametri, strErr)
    '                            For jj = 0 To Dt_Prodotti.Rows.Count - 1

    '                                Dim dr As DataRow() = dt.Select("fr_cod = " & Dt_Prodotti.Rows(jj).Item("fr_cod"))

    '                                Dt_Prodotti.Rows(jj).Item("data_reg") = dr(0).Item("data_reg")
    '                                Dt_Prodotti.Rows(jj).Item("Data_fine_comm") = dr(0).Item("Data_fine_comm")
    '                                Dt_Prodotti.Rows(jj).Item("Data_fine_usoscorte") = dr(0).Item("Data_fine_usoscorte")
    '                                Dt_Prodotti.Rows(jj).Item("revocato") = dr(0).Item("revocato")
    '                            Next
    '                            ''verifico se è tutto ok

    '                            ObjDownloadWs.Dispose()

    '                        Catch ex As Exception

    '                        End Try


    '                        '''''''''''''''''''''' fine modifica webservice




    '                End Select


    '                If Not IsNothing(Dt_Prodotti) Then

    '                    Select Case Elem_Cod

    '                        Case FORMULATI

    '                            'nel caso dei formulati devo filtrare per evitare di visualizzare prodotti revocati, ecc
    '                            'Dt_Prodotti = objFito.Filtra_Formulati(Dt_Prodotti, DataFiltroFormulati, objParametri)
    '                            Dt_Prodotti = objFito.Filtra_Formulati_2(Dt_Prodotti, DataFiltroFormulati, objParametri_server)

    '                    End Select

    '                    'NumTotale = Dt_Prodotti.Rows.Count

    '                    For j = 0 To Dt_Prodotti.Rows.Count - 1

    '                        Select Case Elem_Cod

    '                            Case FERTILIZZANTI
    '                                'value salvato nella modalità gestita dalla formprodotto
    '                                If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
    '                                    x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
    '                                ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
    '                                    x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
    '                                Else
    '                                    'errore, qui non dovrebbe mai entrare
    '                                    x_Pro_Cod = 0
    '                                End If

    '                            Case Else
    '                                x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

    '                        End Select

    '                        If Not IsDBNull(x_Pro_Cod) Then

    '                            If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then

    '                                If Flag_VisualizzaProCod = True Then
    '                                    x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                                 " (" & CStr(Math.Abs(x_Pro_Cod)) & ")"
    '                                Else
    '                                    x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                                End If

    '                                Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))

    '                            End If
    '                        End If


    '                    Next
    '                End If

    '            Next

    '            'Dt_Categorie.Dispose()

    '            'If Not IsNothing(Dt_Prodotti) Then
    '            '    Dt_Prodotti.Dispose()
    '            'End If

    '            Dt_Categorie = Nothing
    '            Dt_Prodotti = Nothing


    '        Case CAU_SCARICO

    '            '#############################################
    '            '##############     SCARICO    ###############
    '            '#############################################

    '            Dim Dt_Temp As New DataTable
    '            Dim DrTemp As DataRow

    '            Dt_Temp.Columns.Add(New DataColumn("Des", GetType(String)))
    '            Dt_Temp.Columns.Add(New DataColumn("Cod", GetType(Integer)))


    '            'leggo i prodotti in magazzino

    '            Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R


    '            'legge le giacenze con le categorie di magazzino
    '            'Dt_Giacenze = objGiacenze.Giacenze_Leggi(Piva, _
    '            '                                        Sa_Cod, _
    '            '                                        Id_Destinazione, _
    '            '                                        0, 0, _
    '            '                                        Elem_Cod, _
    '            '                                        0, 0, 0, 0, 0, 0, "", _
    '            '                                        0, 0, 0, _
    '            '                                         AGRODATAFINE, _
    '            '                                        "", "", _
    '            '                                        objParametri)

    '            Dt_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataFiltroFormulati, _
    '                                                              Piva, _
    '                                                              Sa_Cod, _
    '                                                              Id_Destinazione, _
    '                                                              Elem_Cod, _
    '                                                              0, 0, 0, 0, 0, 0, _
    '                                                              LOTTO_NONDEFINITO, _
    '                                                              False, _
    '                                                              "", "", "", "", "", "", "", "", "", "", "", "", objParametri_server)


    '            For i = 0 To Dt_Giacenze.Rows.Count - 1

    '                NomeTabella = Dt_Giacenze.Rows(i).Item("Tabella")
    '                NomeCodice = Dt_Giacenze.Rows(i).Item("Tabella_Cod")
    '                NomeDescrizione = Dt_Giacenze.Rows(i).Item("Tabella_Des")

    '                Select Case Elem_Cod

    '                    Case FERTILIZZANTI

    '                        'per i fertilizzanti non uso più la funzione NewCom_LeggiTabella_da_CategorieMagazzino
    '                        'perché oltre alla tabella fertilizzanti, devo leggere anche la tabella materie_prime 
    '                        '(per i fertilizzanti aziendali)

    '                        'Dt_Prodotti = NewCom_Fertilizzanti_Completa_Leggi(objServer, objSession, objPage, _
    '                        '                                                   Piva, _
    '                        '                                                   0, _
    '                        '                                                   True, _
    '                        '                                                   RicercaTesto, _
    '                        '                                                   Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                        '                                                   Dt_Giacenze.Rows(i).Item("Mat_Cod"))


    '                        Dt_Prodotti = objFert.Leggi_Completa(Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                                                    RicercaTesto, _
    '                                                    0, _
    '                                                    True, _
    '                                                    Piva, _
    '                                                    Dt_Giacenze.Rows(i).Item("Mat_Cod"), _
    '                                                    objParametri_server.FinestraTemporaleInizio, _
    '                                                    objParametri_server.FinestraTemporaleFine, _
    '                                                    "", " " & NomeDescrizione & " ", _
    '                                                    objParametri_server, _
    '                                                    0)

    '                    Case Else

    '                        'Dt_Prodotti = NewCom_LeggiTabella_da_CategorieMagazzino(objServer, objSession, objPage, _
    '                        '                                                        NomeTabella, _
    '                        '                                                        NomeCodice, _
    '                        '                                                        NomeDescrizione, _
    '                        '                                                        RicercaTesto, _
    '                        '                                                        Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                        '                                                        "")

    '                        Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella, _
    '                                                     NomeCodice, _
    '                                                     NomeDescrizione, _
    '                                                     RicercaTesto, _
    '                                                     Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                                                     "", " " & NomeDescrizione & " ", _
    '                                                     objParametri_server)

    '                End Select


    '                If Not IsNothing(Dt_Prodotti) Then

    '                    For j = 0 To Dt_Prodotti.Rows.Count - 1

    '                        Select Case Elem_Cod

    '                            Case FERTILIZZANTI
    '                                'value salvato nella modalità gestita dalla formprodotto
    '                                If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
    '                                    x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
    '                                ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
    '                                    x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
    '                                Else
    '                                    'errore, qui non dovrebbe mai entrare
    '                                    x_Pro_Cod = 0
    '                                End If

    '                            Case Else
    '                                x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

    '                        End Select

    '                        If Not IsDBNull(x_Pro_Cod) Then
    '                            'If IsNothing(Cmb.Items.FindByValue(x_Pro_Cod)) Then

    '                            If Flag_VisualizzaProCod = True Then
    '                                x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                            " (" & CStr(Math.Abs(x_Pro_Cod)) & ")"
    '                            Else
    '                                x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                            End If

    '                            'salvo in un dt di appoggio (da utilizzare per il dataview)
    '                            'e dopo l'ordinamento del dataview carico la combo
    '                            DrTemp = Dt_Temp.NewRow
    '                            DrTemp.Item("Des") = x_Pro_Des
    '                            DrTemp.Item("Cod") = x_Pro_Cod
    '                            Dt_Temp.Rows.Add(DrTemp)

    '                            'Cmb.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))

    '                            'NumTotale += 1

    '                            'End If
    '                        End If

    '                    Next

    '                End If

    '            Next 'giacenze

    '            If Dt_Temp.Rows.Count <> 0 Then

    '                'uso il dataview per ordinare
    '                Dim Dv As New DataView

    '                Dt_Temp.TableName = "prodotti"
    '                Dv.Table = Dt_Temp
    '                Dv.Sort = "Des ASC"

    '                For i = 0 To Dt_Temp.Rows.Count - 1

    '                    x_Pro_Cod = Dv.Item(i).Item("Cod")
    '                    x_Pro_Des = CStr(Dv.Item(i).Item("Des"))

    '                    If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
    '                        Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
    '                    End If

    '                Next

    '            End If

    '            Dt_Giacenze = Nothing
    '            Dt_Prodotti = Nothing


    '    End Select


    'End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	12/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Calibri(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objCalibri As New AgronicaCoreMetaSchemaDAL.CalibriFrutti_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Dt = objCalibri.Leggi(0, _
        '                      enumSelezioneVariabile.Selezione_TabellaCompleta, _
        '                      "", _
        '                      "", _
        '                      objParametri)

        Dt = objCalibri.LeggiOTabelle_Parametri(0,
                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                              "",
                              "",
                              objParametri)


        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Descrizione"),
                                                  Dt.Rows(i).Item("Tabella_Par_Cod")))

            Next

        End If

    End Sub

    Public Shared Sub CalibriOTabelle_Parametri(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objCalibri As New AgronicaCoreMetaSchemaDAL.CalibriFrutti_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objCalibri.Leggi(0,
                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                              "",
                              "",
                              objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Cal_Des"),
                                                  Dt.Rows(i).Item("Cal_Cod")))

            Next

        End If

    End Sub
    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Provincia"></param>
    ''' <param name="VisualizzaCodiceISTAT"></param>
    ''' <param name="Ordina_Alfabetico1_Istat2"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	12/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Comuni(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Provincia As String,
                                ByVal VisualizzaCodiceISTAT As Boolean,
                                ByVal Ordina_Alfabetico1_Istat2 As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objComuni As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim stringaFiltro As String
        Dim stringaOrderBy As String

        'Pulisco il controllo
        Controllo.Items.Clear()
        Controllo.Attributes.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Imposto il filtro aggiuntivo
        If Provincia = "" Then

            stringaFiltro = "Prov <> 'E00'"

        Else

            stringaFiltro = "Prov <> 'E00' and Comuni_Prov = '" & Provincia & "' "

        End If

        'Aggiungo il filtro per provincia al filtro aggiuntivo
        If xFiltroAggiuntivo <> "" Then
            xFiltroAggiuntivo = xFiltroAggiuntivo & "AND " & stringaFiltro
        Else
            xFiltroAggiuntivo = stringaFiltro
        End If


        'Inserisco l'ordinamento aggiuntivo
        If xOrderBy <> "" Then
            stringaOrderBy = xOrderBy
        Else
            If Ordina_Alfabetico1_Istat2 = 2 Then
                'Ordino per codice ISTAT
                stringaOrderBy = "Com asc"
            Else
                'Ordino i record
                stringaOrderBy = "Localita asc"
            End If
        End If


        Dt = objComuni.Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                             xFiltroAggiuntivo, stringaOrderBy, objParametri, "")

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1
                If VisualizzaCodiceISTAT Then
                    Controllo.Items.Add(New ListItem("(" & Dt.Rows(i).Item("Com") & ") : " &
                                                    Dt.Rows(i).Item("Localita"),
                                                    Dt.Rows(i).Item("Prov") & "|" & Dt.Rows(i).Item("Com") & "|" & Dt.Rows(i).Item("Comuni_prov").ToString.ToLower))
                Else
                    Controllo.Items.Add(New ListItem(CStr(Dt.Rows(i).Item("Localita")).ToLower,
                                                    Dt.Rows(i).Item("Prov") & "|" & Dt.Rows(i).Item("Com") & "|" & Dt.Rows(i).Item("Comuni_prov").ToString.ToLower))
                End If

            Next

        End If

    End Sub




    Public Shared Sub StatoImpianto(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim i As Integer

        Dim objG As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
        Dim dt As DataTable
        Dim filtro As String = " Grfi_Cod > 100 "
        Dim orderby As String = " Grfi_DES "

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        dt = objG.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, orderby, objParametri)

        For i = 0 To dt.Rows.Count - 1
            Controllo.Items.Add(New ListItem(dt.Rows(i).Item("Grfi_DES"),
                                       dt.Rows(i).Item("Grfi_COD")))

        Next


    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Grsp_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	12/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub GruppoAvversita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Veg_Cod As Integer,
                                ByVal Grsp_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.GruppoAvversita()"

        Dim Dt As DataTable
        Dim i As Integer
        Dim StrSQL As New StringBuilder
        Dim objGrupColxSpecVeg As New AgronicaCoreMetaSchemaDAL.GruppoColturaleXSpecieVegetali_R

        'Creo un oggetto di tipo DataProvider perché altrimenti non è possibile chiamare il metodo 
        'EseguiQuery_Lettura
        Dim Array_VegCod As Integer() = Nothing

        If (Veg_Cod = 0) AndAlso (Grsp_Cod = 0) Then

            StrSQL.Length = 0

            StrSQL.Append("SELECT Av_Gru, Av_Gru_Des FROM GruppoAvversita ")
            StrSQL.Append(" where  (Av_Gru_Des NOT LIKE '%non usare%') AND (Av_Gru_Des NOT LIKE '%(#)%') AND (Av_Gru_Des_Lat NOT LIKE '%non usare%') AND (Av_Gru_Des_Lat NOT LIKE '%(#)%')  ")


            'Inserisco eventuali filtri aggiuntivi
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append("ORDER BY Av_Gru_Des ")
            End If


        ElseIf ((Veg_Cod <> 0) AndAlso (Grsp_Cod <> 0)) OrElse
               ((Veg_Cod <> 0) AndAlso (Grsp_Cod = 0)) Then

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT GruppoAvversita.Av_Gru, GruppoAvversita.Av_Gru_Des ")
            StrSQL.Append(" FROM SpecieVegetali INNER JOIN ")
            StrSQL.Append(" SpecieVegetalixAvversita ON SpecieVegetali.Veg_Cod = SpecieVegetalixAvversita.Veg_Cod INNER JOIN ")
            StrSQL.Append(" GruppoAvversita INNER JOIN ")
            StrSQL.Append(" AvversitaxGruppoAvversita ON GruppoAvversita.Av_Gru = AvversitaxGruppoAvversita.Av_Gru INNER JOIN ")
            StrSQL.Append(" Avversita ON AvversitaxGruppoAvversita.Av_Cod = Avversita.Av_Cod ON ")
            StrSQL.Append(" SpecieVegetalixAvversita.Av_Cod = Avversita.Av_Cod  ")
            StrSQL.Append(" WHERE SpecieVegetali.Veg_Cod = " & Veg_Cod)

            'Inserisco eventuali filtri aggiuntivi
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append("ORDER BY Av_Gru_Des ")
            End If

        ElseIf (Veg_Cod = 0) AndAlso (Grsp_Cod <> 0) Then

            objGrupColxSpecVeg.VegCod_from_GrspCod(Grsp_Cod, Array_VegCod, objParametri)

            If Array_VegCod IsNot Nothing Then

                For i = 0 To Array_VegCod.Length - 1

                    Veg_Cod = Array_VegCod(i)

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT DISTINCT GruppoAvversita.Av_Gru, GruppoAvversita.Av_Gru_Des  ")
                    StrSQL.Append(" FROM SpecieVegetali INNER JOIN ")
                    StrSQL.Append(" SpecieVegetalixAvversita ON SpecieVegetali.Veg_Cod = SpecieVegetalixAvversita.Veg_Cod INNER JOIN ")
                    StrSQL.Append(" GruppoAvversita INNER JOIN ")
                    StrSQL.Append(" AvversitaxGruppoAvversita ON GruppoAvversita.Av_Gru = AvversitaxGruppoAvversita.Av_Gru INNER JOIN ")
                    StrSQL.Append(" Avversita ON AvversitaxGruppoAvversita.Av_Cod = Avversita.Av_Cod ON ")
                    StrSQL.Append(" SpecieVegetalixAvversita.Av_Cod = Avversita.Av_Cod  ")
                    StrSQL.Append(" WHERE SpecieVegetali.Veg_Cod = " & Veg_Cod)

                    'Inserisco eventuali filtri aggiuntivi
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append("ORDER BY Av_Gru_Des ")
                    End If
                Next

            End If

        End If

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Av_Gru_Des"),
                                                 Dt.Rows(i).Item("Av_Gru")))

            Next

        End If

    End Sub



    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Gru_Cod"></param>
    ''' <param name="Flag_FiltroUtente"></param>
    ''' <param name="Gru_Cod_daModificare"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	12/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub GruppoVegetale(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Gru_Cod As Integer,
                                ByVal Flag_FiltroUtente As Boolean,
                                ByVal Gru_Cod_daModificare As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri_Server As AgronicaCoreParametri,
                                ByRef objParametri_Utenti As AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer

        Dim objGruVeg As New AgronicaCoreMetaSchemaDAL.GruppoVegetale_R

        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer
        Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If Flag_FiltroUtente Then

            Dt = objGruVeg.GruppoVegetale_GestioneFiltroUtente_Leggi(Gru_Cod,
                                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                     xFiltroAggiuntivo,
                                                                     xOrderBy,
                                                                     objParametri_Utenti)

        Else

            Dt = objGruVeg.Leggi(Gru_Cod,
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 xFiltroAggiuntivo,
                                 xOrderBy,
                                 objParametri_Server)

        End If


        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                'se sono in info o modifica, e quindi ho passato il Gru_Cod_daModificare
                'faccio il controllo
                If Gru_Cod_daModificare <> 0 Then

                    'se all'interno delle specie filtrate, trovo il veg_cod da modificare
                    If Dt.Rows(i).Item("Gru_Cod") = Gru_Cod_daModificare Then
                        Flag_VegCodTrovatoNelFiltroUtente = True
                    End If
                Else
                    'non è stato passato il Gru_Cod_daModificare
                    'quindi imposto il flag, come se avessi trovato il gruppo veg
                    'così non devo aggiungere niente
                    Flag_VegCodTrovatoNelFiltroUtente = True
                End If

                x_Cod = Dt.Rows(i).Item("Gru_Cod")

                x_Des = CStr(Dt.Rows(i).Item("gru_DES"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing


        'se sono in info o modifica, e quindi ho passato il Gru_Cod_daModificare
        If Gru_Cod_daModificare <> 0 Then

            'se all'interno dei gruppi filtrati, non è stato trovato il Gru_Cod_daModificare
            If Not Flag_VegCodTrovatoNelFiltroUtente Then

                x_Des = objGruVeg.GruDes_from_GruCod(Gru_Cod_daModificare, objParametri_Server)

                Controllo.Items.Add(New ListItem(x_Des, Gru_Cod_daModificare))

            End If

        End If

    End Sub



    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	12/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Macrouso(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objMacrousi.Leggi("", "",
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Macrouso_Des"),
                                                  Dt.Rows(i).Item("Macrouso_Cod")))

            Next

        End If

    End Sub

    '#############################################################################################
    Public Shared Sub MacrousiImpresa(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String,
                                      ByVal Piva As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objMacrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objMacrousi.Leggi_Macrousi(Piva,
                                        xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Macrouso_Des"),
                                                 Dt.Rows(i).Item("Macrouso_Cod")))

            Next

        End If

    End Sub


    '#############################################################################################
    Public Shared Sub UtilizziImpresa(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String,
                                      ByVal Piva As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objMacrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objMacrousi.Leggi_Utilizzi(Piva,
                                        xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des_Agea"),
                                                 Dt.Rows(i).Item("Veg_Cod_Agea")))

            Next

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	12/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub PChiave(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )
        Dim Dt As DataTable
        Dim i As Integer
        Dim objPChiaveImpresa As New AgronicaCoreAnagrafeDAL.ParoleChiave_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPChiaveImpresa.Leggi("",
                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                    xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("ParolaChiave")))

            Next

        End If

    End Sub



    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	12/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub PianiSemina(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim StrSQL As New StringBuilder

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.PianiSemina()"

        StrSQL.Length = 0
        StrSQL.Append(" SELECT DISTINCT val_cod ")
        StrSQL.Append(" FROM Reg_Impianti_Codici ")
        StrSQL.Append(" WHERE (id_cod = 1071) ")

        'Inserisco eventuali filtri aggiuntivi
        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If

        If xOrderBy <> "" Then
            StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        Else
            StrSQL.Append(" ORDER BY val_cod")
        End If

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("val_cod"),
                                                  Dt.Rows(i).Item("val_cod")))

            Next

        End If

    End Sub



    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="TestoRicerca"></param>
    ''' <param name="TipoRichiesto"></param>
    ''' <param name="IncludiAziendali"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Flag_CodNegativo"></param>
    ''' <param name="Flag_IncludiNPK"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	12/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Fertilizzanti_2(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal TestoRicerca As String,
                                ByVal TipoRichiesto As Integer,
                                ByVal IncludiAziendali As Boolean,
                                ByVal Piva As String,
                                ByVal Flag_CodNegativo As Boolean,
                                ByVal Flag_IncludiNPK As Boolean,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        'NOTA
        'Il TipoRichiesto consente di selezionare solo i fertilizzanti specifici
        'per la particolare applicazione
        '
        '   0 = Tutti i fertilizzanti
        '   1 = Trattamenti Antibutteratura
        '   2 = Concimazione Fogliare
        '   3 = Fertirrigazione
        '   4 = Concimazione Organica
        '   5 = Concimazione in pieno Campo
        '   ecc...

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String

        Controllo.Items.Clear()

        ''Inserisco una riga vuota
        'Cmb.Items.Add(New ListItem("", ""))

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objFertilizzanti As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

        Dt = objFertilizzanti.Leggi_Completa(0,
                                             TestoRicerca,
                                             TipoRichiesto,
                                             IncludiAziendali,
                                             Piva,
                                             0,
                                             AGRODATAINIZIO,
                                             AGRODATAFINE,
                                             xFiltroAggiuntivo,
                                             xOrderBy,
                                             objParametri)

        If Not IsNothing(Dt) Then

            'NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                If Not Flag_CodNegativo Then
                    'value salvato nella modalità gestita dalle operazioni di campagna
                    x_Cod = CStr(Dt.Rows(i).Item("Fer_Cod")) & "/" & CStr(Dt.Rows(i).Item("Mat_Cod"))
                Else

                    'value salvato nella modalità gestita dalla formprodotto
                    If Dt.Rows(i).Item("Fer_Cod") <> 0 Then
                        x_Cod = CStr(Dt.Rows(i).Item("Fer_Cod"))
                    ElseIf Dt.Rows(i).Item("Mat_Cod") <> 0 Then
                        x_Cod = "-" & CStr(Dt.Rows(i).Item("Mat_Cod"))
                    Else
                        'errore, qui non dovrebbe mai entrare
                        x_Cod = "0"
                    End If
                End If

                If Not Flag_IncludiNPK Then
                    x_Des = CStr(Dt.Rows(i).Item("Fer_Des"))
                Else
                    x_Des = CStr(Dt.Rows(i).Item("Fer_Des")) &
                            " --- <" &
                            CStr(Dt.Rows(i).Item("N")) & "-" &
                            CStr(Dt.Rows(i).Item("P2O5")) & "-" &
                            CStr(Dt.Rows(i).Item("K2O")) & "-" &
                            CStr(Dt.Rows(i).Item("MgO")) &
                            ">"
                End If

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub


    '###############################################################################
    Public Shared Sub Fitofarmaci(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Fr_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri)


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objFormulati As New AgronicaCoreMetaSchemaDAL.Formulati_R
        Dt = objFormulati.Leggi(Fr_Cod,
                                AGRODATAINIZIO, AGRODATAFINE,
                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                xFiltroAggiuntivo,
                                 xOrderBy,
                                 objParametri)


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Fr_Cod")

                x_Des = "(" & CStr(Dt.Rows(i).Item("Fr_Cod")) & ") " & Dt.Rows(i).Item("Fr_Des") '& " --- " & Dt.Rows(i).Item("Denominazione") & " - N=" & CStr(Dt.Rows(i).Item("N")) & " P=" & CStr(Dt.Rows(i).Item("P2O5")) & " K=" & CStr(Dt.Rows(i).Item("K2O"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If



    End Sub







    Public Shared Sub Rifiuti(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByVal VisualizzaEsempio As Boolean,
                            ByRef objParametri As AgronicaCoreParametri)


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String = ""
        Dim Esempio As String

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objRifiuti As New AgronicaCoreMetaSchemaDAL.CatalogoEuropeoRifiuti_R
        Dt = objRifiuti.Leggi("", "",
                                AGRODATAINIZIO, AGRODATAFINE,
                                xFiltroAggiuntivo,
                                 xOrderBy,
                                 objParametri)


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Codice")

                If Not IsDBNull(Dt.Rows(i).Item("Esempi")) Then
                    Esempio = Dt.Rows(i).Item("Esempi")
                Else
                    Esempio = ""
                End If

                Select Case VisualizzaEsempio
                    Case True
                        x_Des = "(" & CStr(Dt.Rows(i).Item("Cer_Cod")) & ") " & Dt.Rows(i).Item("Cer_Des") & " (Es." & Esempio & ")"
                    Case False
                        x_Des = "(" & CStr(Dt.Rows(i).Item("Cer_Cod")) & ") " & Dt.Rows(i).Item("Cer_Des")
                End Select

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

    End Sub


    Public Sub TutteSpecieColtivate_SenzaControlloData(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal ConsideraTerrenoNudo As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               ByVal leggiAncheBloccati As Boolean
                               )

        '----- Variabili
        Dim i As Integer
        Dim Flag_TerrenoNudo As Boolean = False
        Dim Dt As DataTable
        Dim StrSQL As New StringBuilder

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.TutteSpecieColtivate_3()"


        '----- Inizializzo

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Genero la query SQL
        StrSQL.Length = 0
        StrSQL.Append(" SELECT DISTINCT    ")
        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ")
        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS Veg_Des ")

        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA LEFT OUTER JOIN ")
        StrSQL.Append(" SpecieVegetali INNER JOIN ")
        StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

        StrSQL.Append(" WHERE   1=1  ")
        If Piva <> "" Then
            StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If

        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If

        If Not leggiAncheBloccati Then
            StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")
        End If


        If Not ConsideraTerrenoNudo Then
            StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If


        If xOrderBy <> "" Then
            StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        Else
            StrSQL.Append(" ORDER BY VEG_DES")
        End If


        Dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                                  Dt.Rows(i).Item("Veg_Cod")))
            Next

        End If

    End Sub



    Public Sub TutteSpecieColtivate_3(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Data_Da As Date,
                               ByVal Data_A As Date,
                               ByVal ConsideraTerrenoNudo As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               ByVal leggiAncheBloccati As Boolean
                               )

        TutteSpecieColtivate_3_Data_Da_A(Controllo,
                                PrimaRiga_Flag,
                                PrimaRiga_Text,
                                PrimaRiga_Value,
                                Piva,
                                Sa_Cod,
                                Data_Da,
                                Data_A,
                                ConsideraTerrenoNudo,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri, leggiAncheBloccati
                               )

    End Sub



    Public Sub TutteSpecieColtivate_3_Data_Da_A(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Data_Da As Date,
                               ByVal Data_A As Date,
                               ByVal ConsideraTerrenoNudo As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               ByVal leggiAncheBloccati As Boolean
                               )

        '----- Variabili
        Dim i As Integer
        Dim Flag_TerrenoNudo As Boolean = False
        Dim Dt As DataTable
        Dim StrSQL As New StringBuilder

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.TutteSpecieColtivate_3_Data_Da_A()"


        '----- Inizializzo

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Genero la query SQL
        StrSQL.Length = 0
        StrSQL.Append(" SELECT DISTINCT    ")
        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ")
        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS Veg_Des ")

        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN ")
        StrSQL.Append(" SpecieVegetali INNER JOIN ")
        StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

        StrSQL.Append(" WHERE   1=1  ")
        If Piva <> "" Then
            StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
        Else
            Dim objUtility As New AgronicaCoreUtility.Varie

            Dim Oggetto As String = "TutteSpecieColtivate_3_Data_Da_A con Piva uguale a stringa vuota"

            Dim TestoMail As String = "Stack di chiamata completo con parametri (PrimaRiga_Flag: " & PrimaRiga_Flag & " - PrimaRiga_Text: " & PrimaRiga_Text & " - PrimaRiga_Value: " & PrimaRiga_Value & " - Piva: " & Piva & " - Sa_Cod: " & Sa_Cod & " - Data_Da: " & Data_Da & " - Data_A: " & Data_Da & " - ConsideraTerrenoNudo: " & ConsideraTerrenoNudo & " - xFiltroAggiuntivo: " & xFiltroAggiuntivo & " - xOrderBy: " & xOrderBy & "): <br> <br>"

            objUtility.Log_X_Segnalazioni_Speciali(Oggetto, TestoMail, objParametri)
        End If


        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        Else
            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim FiltroCentri As String = ""
            Dim DtCentriVisibili As DataTable
            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To DtCentriVisibili.Rows.Count - 1
                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                Next
                If FiltroCentri <> "" Then
                    StrSQL.Append(" AND Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If
        End If

        If Data_Da <> AGRODATAINIZIO Then
            StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Da) & " ")
        End If
        If Data_A <> AGRODATAFINE Then
            StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_A) & " ")
        End If

        'If Data_A <> AGRODATAINIZIO Then
        '    StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_A) & " ")
        'End If
        'If Data_Da <> AGRODATAFINE Then
        '    StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Da) & " ")
        'End If

        If Not leggiAncheBloccati Then
            StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")
        End If

        'If ConsideraTerrenoNudo = False Then
        '    StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
        'End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If

        'Aggiungo le destinazioni d'uso
        If ConsideraTerrenoNudo Then

            'DESTINAZIONI USO SPECIFICATE
            StrSQL.Append(" UNION ")

            StrSQL.Append(" SELECT DISTINCT -Reg_Impianti_Codici.id_cod as Veg_Cod, Codici_Anagrafe.descrizione as Veg_Des    ")
            StrSQL.Append(" FROM   Appezzamento INNER JOIN ")
            StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
            'StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN ")
            StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA LEFT OUTER JOIN ")
            StrSQL.Append(" Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND ")
            StrSQL.Append(" Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg INNER JOIN  ")
            StrSQL.Append(" Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  ")

            StrSQL.Append(" WHERE   1=1  ")
            If Piva <> "" Then
                StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                    End If
                End If
            End If

            If Data_Da <> AGRODATAINIZIO Then
                StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Da) & " ")
            End If
            If Data_A <> AGRODATAFINE Then
                StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_A) & " ")
            End If

            If Not leggiAncheBloccati Then
                StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")
            End If

            StrSQL.Append(" AND Reg_Impianti.CUL_COD = 0 ")
            StrSQL.Append(" AND (Reg_Impianti_Codici.id_cod >=3000 and Reg_Impianti_Codici.id_cod<4000) ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'TERRENI NUDI SENZA DESTINAZIONI USO SPECIFICATE
            StrSQL.Append(" UNION ")

            StrSQL.Append(" SELECT DISTINCT 0 as Veg_Cod, 'Terreno Nudo' as Veg_Des ")
            StrSQL.Append(" FROM   Appezzamento INNER JOIN ")
            StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
            StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA LEFT OUTER JOIN ")
            StrSQL.Append(" Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND ")
            StrSQL.Append(" Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg INNER JOIN  ")
            StrSQL.Append(" Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  ")

            StrSQL.Append(" WHERE   1=1  ")
            If Piva <> "" Then
                StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                    End If
                End If
            End If

            If Data_Da <> AGRODATAINIZIO Then
                StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Da) & " ")
            End If
            If Data_A <> AGRODATAFINE Then
                StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_A) & " ")
            End If

            If Not leggiAncheBloccati Then
                StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")
            End If

            StrSQL.Append(" AND Reg_Impianti.CUL_COD = 0 ")
            StrSQL.Append(" AND NOT EXISTS (SELECT * FROM Reg_Impianti_Codici RIC WHERE Reg_Impianti.PIVA = RIC.PIVA ")
            StrSQL.Append("                 AND Reg_Impianti.SA_COD = RIC.sa_cod AND   Reg_Impianti.APPEZZA = RIC.appezza AND Reg_Impianti.ID_REG = RIC.Id_Reg ")
            StrSQL.Append("                 AND RIC.id_cod >= 3000 AND RIC.id_cod < 4000) ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


        End If

        If xOrderBy <> "" Then
            StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        Else
            StrSQL.Append(" ORDER BY VEG_DES")
        End If

        'Dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                If CInt(Dt.Rows(i).Item("Veg_Cod")) <= 0 Then
                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                                     "0/" & -Dt.Rows(i).Item("Veg_Cod")))
                Else
                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                  Dt.Rows(i).Item("Veg_Cod")))
                End If

            Next

        End If


    End Sub


    Public Sub TutteSpecieColtivate_DaProgrammazione_DataDa_DataA(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Data_Da As Date,
                               ByVal Data_A As Date,
                               ByVal ConsideraTerrenoNudo As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               )

        '----- Variabili
        Dim i As Integer
        Dim Flag_TerrenoNudo As Boolean = False
        Dim Dt As DataTable
        Dim StrSQL As New StringBuilder

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.TutteSpecieColtivate_DaProgrammazione_DataDa_DataA()"


        '----- Inizializzo

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Genero la query SQL
        StrSQL.Length = 0
        StrSQL.AppendLine(" SELECT DISTINCT    ")
        StrSQL.AppendLine(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ")
        StrSQL.AppendLine(" ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS Veg_Des ")

        StrSQL.AppendLine(" FROM         Programmazione_Entita ")
        StrSQL.AppendLine(" INNER JOIN Programmazione_Testata ON Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
        StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod ")
        StrSQL.AppendLine(" INNER JOIN Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod AND Programmazione_Entita.CUL_COD = Cultivar.Cul_Cod ")

        StrSQL.AppendLine(" WHERE   1=1  ")
        StrSQL.AppendLine(" AND Programmazione_Testata.Tipo_Pianificazione = 0 ")
        If Piva <> "" Then
            StrSQL.AppendLine(" AND     Programmazione_Entita.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If


        If Sa_Cod <> 0 Then
            StrSQL.AppendLine(" AND     Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        Else
            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim FiltroCentri As String = ""
            Dim DtCentriVisibili As DataTable
            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To DtCentriVisibili.Rows.Count - 1
                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                Next
                If FiltroCentri <> "" Then
                    StrSQL.AppendLine(" AND Programmazione_Entita.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If
        End If

        If Data_Da <> AGRODATAINIZIO Then
            StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Da) & " ")
        End If
        If Data_A <> AGRODATAFINE Then
            StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_A) & " ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If

        'Aggiungo le destinazioni d'uso
        If ConsideraTerrenoNudo Then

            'DESTINAZIONI USO SPECIFICATE
            StrSQL.AppendLine(" UNION ")

            StrSQL.AppendLine(" SELECT DISTINCT -Programmazione_Entita.id_cod as Veg_Cod, Codici_Anagrafe.descrizione as Veg_Des    ")
            StrSQL.AppendLine(" FROM   Programmazione_Entita  ")
            StrSQL.AppendLine(" INNER JOIN Programmazione_Testata ON Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            StrSQL.AppendLine(" INNER JOIN Codici_Anagrafe ON Programmazione_Entita.id_cod = Codici_Anagrafe.codice  ")

            StrSQL.AppendLine(" WHERE   1=1  ")
            StrSQL.AppendLine(" AND Programmazione_Testata.Tipo_Pianificazione = 0 ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND     Programmazione_Entita.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND     Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine(" AND Programmazione_Entita.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                    End If
                End If
            End If

            If Data_Da <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Da) & " ")
            End If
            If Data_A <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_A) & " ")
            End If

            StrSQL.AppendLine(" AND Programmazione_Entita.CUL_COD = 0 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

        End If

        If xOrderBy <> "" Then
            StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        Else
            'StrSQL.AppendLine(" ORDER BY VEG_DES")
        End If

        'Dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            If xOrderBy = "" Then
                Dt.DefaultView.Sort = "VEG_DES ASC"
                Dt = Dt.DefaultView.ToTable
            End If

            For i = 0 To Dt.Rows.Count - 1

                If CInt(Dt.Rows(i).Item("Veg_Cod")) <= 0 Then
                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                                     "0/" & -Dt.Rows(i).Item("Veg_Cod")))
                Else
                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                  Dt.Rows(i).Item("Veg_Cod")))
                End If

            Next

        End If


    End Sub

    Public Sub TutteSpecieColtivate_AncheBloccate(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Data_Selezionata As Date,
                               ByVal ConsideraTerrenoNudo As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               )

        '----- Variabili
        Dim i As Integer
        Dim Flag_TerrenoNudo As Boolean = False
        Dim objDP As New AgronicaCoreDataProvider.DataProvider
        Dim Dt As DataTable
        Dim StrSQL As New StringBuilder

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.TutteSpecieColtivate_3()"


        '----- Inizializzo

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Genero la query SQL
        StrSQL.Length = 0
        StrSQL.Append(" SELECT DISTINCT    ")
        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ")
        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS Veg_Des ")

        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA LEFT OUTER JOIN ")
        StrSQL.Append(" SpecieVegetali INNER JOIN ")
        StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

        StrSQL.Append(" WHERE   1=1  ")
        If Piva <> "" Then
            StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If


        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If
        StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
        StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")


        If Not ConsideraTerrenoNudo Then
            StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If


        If xOrderBy <> "" Then
            StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        Else
            StrSQL.Append(" ORDER BY VEG_DES")
        End If


        Dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        objDP = Nothing


        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                                  Dt.Rows(i).Item("Veg_Cod")))
            Next

        End If

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="ConsideraTerrenoNudo"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	12/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub TutteSpecieColtivate_4(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal ConsideraTerrenoNudo As Boolean,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               ByVal leggiAncheBloccati As Boolean
                                )

        '----- Variabili
        Dim i As Integer
        Dim Flag_TerrenoNudo As Boolean = False
        Dim objDP As New AgronicaCoreDataProvider.DataProvider
        Dim Dt As DataTable
        Dim StrSQL As New StringBuilder

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.TutteSpecieColtivate_4()"


        '----- Inizializzo

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Genero la query SQL
        StrSQL.Length = 0
        StrSQL.Append(" SELECT DISTINCT    ")
        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ")
        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS Veg_Des ")

        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA LEFT OUTER JOIN ")
        StrSQL.Append(" SpecieVegetali INNER JOIN ")
        StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

        StrSQL.Append(" WHERE   Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
        StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
        StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

        If Not leggiAncheBloccati Then
            StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")
        End If


        If Not ConsideraTerrenoNudo Then
            StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If


        If xOrderBy <> "" Then
            StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        Else
            StrSQL.Append(" ORDER BY VEG_DES")
        End If


        Dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        objDP = Nothing


        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                                  Dt.Rows(i).Item("Veg_Cod")))
            Next

        End If

    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="ConsideraTerrenoNudo"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks></remarks>
    Public Shared Sub CaricaCombo_TutteSpecieColtivate(ByRef Controllo As ListControl,
                              ByVal PrimaRiga_Flag As Boolean,
                              ByVal PrimaRiga_Text As String,
                              ByVal PrimaRiga_Value As String,
                              ByVal Piva As String,
                              ByVal Sa_Cod As Integer,
                              ByVal Data_Selezionata As Date,
                              ByVal ConsideraTerrenoNudo As Boolean,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              )

        '----- Variabili
        Dim i As Integer
        Dim Flag_TerrenoNudo As Boolean = False
        Dim objDP As New AgronicaCoreDataProvider.DataProvider
        Dim Dt As DataTable
        Dim StrSQL As New StringBuilder

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.CaricaCombo_TutteSpecieColtivate()"

        objParametri.FinestraTemporaleFine = Data_Selezionata
        objParametri.FinestraTemporaleInizio = Data_Selezionata


        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dt = objImpianti.Leggi(Piva, Sa_Cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                               "", "", objParametri)

        '----- Inizializzo

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        Else
            Controllo.Items.Add(New ListItem("", "X"))
        End If

        Dim ElencoVarieta As String = ""
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                If Dt.Rows(i).Item("Cul_Cod") = 0 Then
                    If ConsideraTerrenoNudo Then
                        Flag_TerrenoNudo = True
                    End If
                Else
                    ElencoVarieta = ElencoVarieta & "," &
                           Dt.Rows(i).Item("Cul_Cod")
                End If
            Next
            If ElencoVarieta <> "" Then
                'Elimino la virgola iniziale
                ElencoVarieta = Mid(ElencoVarieta, 2)

                'Genero la query SQL
                StrSQL.Append(" SELECT DISTINCT  ")
                StrSQL.Append("         SpecieVegetali.VEG_DES,")
                StrSQL.Append("         SpecieVegetali.VEG_COD ")
                StrSQL.Append(" FROM    Cultivar INNER JOIN SpecieVegetali ")
                StrSQL.Append("         ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD ")
                StrSQL.Append("")
                StrSQL.Append(" WHERE   (Cultivar.CUL_COD IN (" & ElencoVarieta & ")) ")
                StrSQL.Append(" ORDER BY SpecieVegetali.VEG_DES")

                Dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)



                If Dt.Rows.Count > 0 Then

                    For i = 0 To Dt.Rows.Count - 1
                        Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                          Dt.Rows(i).Item("Veg_Cod")))
                    Next
                End If

            End If

            'Aggiungo la voce relativa al terreno nudo
            If Flag_TerrenoNudo Then
                Controllo.Items.Add(New ListItem(Str_TerrenoNudo, "0"))
            End If
        Else
            ''nessun impianto
        End If
    End Sub



    '##############################################################################
    'carica tutte le specie coltivate nell'intervallo di date specificato
    Public Shared Sub TutteSpecieColtivate_5(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal ConsideraTerrenoNudo As Boolean,
                                            ByVal Data_Inizio As Date,
                                            ByVal Data_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.TutteSpecieColtivate_3()"


        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        '----- Variabili
        Dim i As Integer
        Dim Dt As DataTable
        Dim des, cod As String
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data_Inizio, Data_Fine)

        Dt = objImpianti.Leggi_SpecieVarieta(Piva,
                                            Sa_Cod,
                                            0, 0, 0, 0,
                                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                            xFiltroAggiuntivo,
                                            xOrderBy,
                                            objParametri_Server)

        objParametri_Server.ResettaFinestra()


        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                des = Dt.Rows(i).Item("Veg_des")
                cod = Dt.Rows(i).Item("veg_cod")

                If IsNothing(Controllo.Items.FindByValue(cod)) Then
                    Controllo.Items.Add(New ListItem(des, cod))
                End If

            Next

        End If

        'Aggiungo la voce relativa al terreno nudo
        If ConsideraTerrenoNudo Then
            If objImpianti.Esiste_TerrenoNudo(Piva, Sa_Cod, 0, 0, "", "", objParametri_Server) Then
                Controllo.Items.Add(New ListItem(Str_TerrenoNudo, "0"))
            End If
        End If


    End Sub


    Public Sub TutteSpecieColtivate_DaProgrammazione(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Data_Da As Date,
                               ByVal Data_A As Date,
                               ByVal ConsideraTerrenoNudo As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               ByVal leggiAncheBloccati As Boolean
                               )

        TutteSpecieColtivate_DaProgrammazione_DataDa_DataA(Controllo,
                                PrimaRiga_Flag,
                                PrimaRiga_Text,
                                PrimaRiga_Value,
                                Piva,
                                Sa_Cod,
                                Data_Da,
                                Data_A,
                                ConsideraTerrenoNudo,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri
                               )

    End Sub

    Public Shared Sub CaricaCheckBoxList_SpecieVegetale_Optimize(ByRef Controllo As ListControl,
                            ByVal Gru_Cod As Integer,
                                                        ByVal Flag_FiltroUtente As Boolean,
                                                        ByVal FiltroAggiuntivo As String,
                                                        ByVal Ordinamento As String,
                                                        ByVal Veg_Cod_daModificare As Integer,
                                                        ByVal GruCod_Rif_VegCod_daModificare As Integer,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False

        Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

        If Flag_FiltroUtente Then

            Dt = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(0,
                                                                    Gru_Cod,
                                                                    "",
                                                                    "",
                                                                    FiltroAggiuntivo,
                                                                    Ordinamento,
                                                                    objParametri_Utenti)

        Else

            Dt = objSpecVeg.Leggi(0,
                    Gru_Cod,
                    "",
                    "",
                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    FiltroAggiuntivo,
                    Ordinamento,
                    objParametri_Utenti)

        End If

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            'uso il dataview per ordinare
            Dim Dv As New DataView

            Dt.TableName = "specie"
            Dv.Table = Dt
            Dv.Sort = "Veg_Des ASC"

            If Not IsNothing(Dv) Then

                For i = 0 To NumTotale - 1

                    'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
                    'faccio il controllo
                    If Veg_Cod_daModificare <> 0 Then

                        'se all'interno delle specie filtrate, trovo il veg_cod da modificare
                        If Dv.Item(i).Item("Veg_Cod") = Veg_Cod_daModificare Then
                            Flag_VegCodTrovatoNelFiltroUtente = True
                        End If

                    Else
                        'non è stato passato il veg_cod
                        'quindi imposto il flag, come se avessi trovato la specie
                        'così non devo aggiungere niente
                        Flag_VegCodTrovatoNelFiltroUtente = True
                    End If

                    x_Cod = Dv.Item(i).Item("Veg_Cod")

                    x_Des = CStr(Dv.Item(i).Item("Veg_DES"))

                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))

                Next

            End If

            Dv = Nothing

        Else
            NumTotale = 0
        End If

        Dt = Nothing


        'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
        If Veg_Cod_daModificare <> 0 Then

            ''se l'utente ha il filtro specie impostato
            'If Flag_FiltroUtenteImpostato = True Then

            'se all'interno delle specie filtrate, non è stato trovato il veg_cod da modificare
            If Not Flag_VegCodTrovatoNelFiltroUtente Then

                x_Des = ""

                'se il Veg_Cod_daModificare è valorizzato, devo sapere anche il gru_cod
                '(se non l'ho passato me lo ricavo e intanto che ci sono ricavo la descrizione della specie)
                'però solo nel caso di gru_cod è <> 0 (ovvero impianto e campo)
                If Gru_Cod <> 0 AndAlso GruCod_Rif_VegCod_daModificare = 0 Then
                    Dim objSp As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                    GruCod_Rif_VegCod_daModificare = objSp.GruCod_and_VegDes_from_VegCod(x_Des, Veg_Cod_daModificare, objParametri_Server)
                End If

                'se il gru_cod della specie da modificare è lo stesso di quello selezionato,
                'allora inserisco la specie vegetale da modificare nel menù
                If Gru_Cod = GruCod_Rif_VegCod_daModificare Then

                    'se la descrizione non l'ho già ricavata sopra
                    If x_Des = "" Then
                        x_Des = objSpecVeg.VegDes_from_VegCod(Veg_Cod_daModificare, objParametri_Server)
                    End If

                    Controllo.Items.Add(New ListItem(x_Des, Veg_Cod_daModificare))

                End If

            End If

        End If

    End Sub










    Public Shared Sub CaricaCombo_VarietaColtivate_con_Visibilita_Utente(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal Flag_FiltroUtente As Boolean,
                            ByVal Veg_Cod As Integer,
                            ByVal Cul_Cod_da_Modificare As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )



        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer
        Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False


        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If
        'Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim objVarieta As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        If Flag_FiltroUtente Then

            Dt = objVarieta.GestioneFiltroUtente_Leggi(0,
                                                        Veg_Cod,
                                                        "",
                                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        xFiltroAggiuntivo,
                                                        xOrderBy,
                                                        objParametri_Utenti)

        Else
            Dt = objVarieta.Leggi(0,
                    Veg_Cod,
                    "",
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    xFiltroAggiuntivo,
                    xOrderBy,
                    objParametri_Server)
        End If


        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            'uso il dataview per ordinare
            Dim Dv As New DataView

            Dt.TableName = "varieta"
            Dv.Table = Dt
            Dv.Sort = "cul_des ASC"

            If Not IsNothing(Dv) Then

                For i = 0 To NumTotale - 1

                    'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
                    'faccio il controllo
                    If Cul_Cod_da_Modificare <> 0 Then

                        ''se l'utente ha il filtro specie impostato
                        'If Flag_FiltroUtenteImpostato = True Then

                        'se all'interno delle specie filtrate, trovo il veg_cod da modificare
                        If Dv.Item(i).Item("Cul_Cod") = Cul_Cod_da_Modificare Then
                            Flag_VegCodTrovatoNelFiltroUtente = True
                        End If
                        'Else
                        '    'non c'è un filtro impostato
                        '    'quindi imposto il flag, come se avessi trovato la specie
                        '    'così non devo aggiungere niente
                        '    Flag_VegCodTrovatoNelFiltroUtente = True
                        'End If
                    Else
                        'non è stato passato il veg_cod
                        'quindi imposto il flag, come se avessi trovato la specie
                        'così non devo aggiungere niente
                        Flag_VegCodTrovatoNelFiltroUtente = True
                    End If

                    x_Cod = Dv.Item(i).Item("Cul_Cod")

                    x_Des = CStr(Dv.Item(i).Item("Cul_DES"))


                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                Next

            End If

            Dv = Nothing

        Else
            NumTotale = 0
        End If

        Dt = Nothing


        'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
        If Cul_Cod_da_Modificare <> 0 Then
            'se all'interno delle specie filtrate, non è stato trovato il veg_cod da modificare
            If Not Flag_VegCodTrovatoNelFiltroUtente Then

                x_Des = ""
                x_Des = objVarieta.CulDes_from_CulCod(Cul_Cod_da_Modificare, objParametri_Server)

                Controllo.Items.Add(New ListItem(x_Des, Cul_Cod_da_Modificare))
            End If

        End If

    End Sub




    Public Sub CaricaCombo_TutteVarietaColtivate(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Veg_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.CaricaCombo_TutteVarietaColtivate()"

        '----- Variabili
        Dim i As Integer
        ' Dim Flag_TerrenoNudo As Boolean = False
        Dim objDP As New AgronicaCoreDataProvider.DataProvider
        Dim Dt As DataTable
        Dim StrSQL As New StringBuilder


        '----- Inizializzo

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
            'Else
            '    Controllo.Items.Add(New ListItem("", "X"))
        End If

        'Genero la query SQL
        StrSQL.Append(" SELECT DISTINCT  ")
        StrSQL.Append("         Cultivar.Cul_Des,")
        StrSQL.Append("         Cultivar.Cul_COD ")

        StrSQL.Append(" FROM    Cultivar INNER JOIN Reg_Impianti ")
        StrSQL.Append("         ON Cultivar.CUL_COD = Reg_Impianti.CUL_COD  ")

        If Campo_Cod <> 0 OrElse xFiltroAggiuntivo <> "" Then
            StrSQL.Append("  INNER JOIN  Appezzamento ON Appezzamento.piva = Reg_Impianti.piva AND Appezzamento.sa_cod = Reg_Impianti.sa_cod AND Appezzamento.appezza = Reg_Impianti.appezza ")
        End If

        StrSQL.Append(" ")

        StrSQL.Append(" WHERE   Cultivar.VEG_COD = " & Agro_SQL_SaveNum(Veg_Cod))
        If Piva <> "" Then
            StrSQL.Append(" AND     Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If
        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
        Else
            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim FiltroCentri As String = ""
            Dim DtCentriVisibili As DataTable
            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To DtCentriVisibili.Rows.Count - 1
                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                Next
                If FiltroCentri <> "" Then
                    StrSQL.Append(" AND Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If
        End If

        If Campo_Cod <> 0 Then
            StrSQL.Append(" AND     Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append(" AND (" & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ) ")
        End If

        StrSQL.Append(" AND NOT ( Reg_Impianti.Validita_Inizio > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
        StrSQL.Append(" AND NOT ( Reg_Impianti.Validita_Fine < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

        StrSQL.Append(" ORDER BY Cultivar.Cul_Des")

        objDP.SettaParametriPrecedenti(DammiParametriCollezionati)
        Dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        If Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Cul_Des"),
                                  Dt.Rows(i).Item("Cul_Cod")))

            Next

        End If

    End Sub




    Public Sub CaricaCombo_TutteVarietaColtivate2(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Veg_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )

        '----- Variabili
        Dim i As Integer
        Dim Flag_TerrenoNudo As Boolean = False
        Dim objDP As New AgronicaCoreDataProvider.DataProvider
        Dim Dt As DataTable
        Dim StrSQL As New StringBuilder

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.CaricaCombo_TutteSpecieColtivate()"

        'objParametri.FinestraTemporaleFine = Data_Selezionata
        'objParametri.FinestraTemporaleInizio = Data_Selezionata


        'Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        'Dt = objImpianti.Leggi(Piva, Sa_Cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, _
        '                       "", "", objParametri)

        '----- Inizializzo

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        Else
            Controllo.Items.Add(New ListItem("", "X"))
        End If

        'Genero la query SQL
        StrSQL.Append(" SELECT DISTINCT  ")
        StrSQL.Append("         Cultivar.Cul_Des,")
        StrSQL.Append("         Cultivar.Cul_COD ")
        StrSQL.Append(" FROM    Cultivar INNER JOIN Reg_Impianti ")
        StrSQL.Append("         ON Cultivar.CUL_COD = Reg_Impianti.CUL_COD ")
        StrSQL.Append("")
        StrSQL.Append(" WHERE   Cultivar.VEG_COD = " & Agro_SQL_SaveNum(Veg_Cod))
        If Piva <> "" Then
            StrSQL.Append(" AND     Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If
        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
        End If
        StrSQL.Append(" AND NOT ( Reg_Impianti.Validita_Inizio > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
        StrSQL.Append(" AND NOT ( Reg_Impianti.Validita_Fine < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

        StrSQL.Append(" ORDER BY Cultivar.Cul_Des")

        Dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        If Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Cul_Des"),
                                  Dt.Rows(i).Item("Cul_Cod")))
            Next
        End If

    End Sub



    '##############################################################################
    'carica tutte le varietà coltivate nella finestra temporale
    Public Shared Sub TutteVarietaColtivate_5(ByRef Controllo As ListControl,
                                                            ByVal PrimaRiga_Flag As Boolean,
                                                            ByVal PrimaRiga_Text As String,
                                                            ByVal PrimaRiga_Value As String,
                                                            ByVal Piva As String,
                                                            ByVal Sa_Cod As Integer,
                                                            ByVal Veg_Cod As Integer,
                                                             ByVal Data_Inizio As Date,
                                                            ByVal Data_Fine As Date,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            )

        'ByVal ConsideraTerrenoNudo As Boolean, _

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.CaricaCombo_TutteVarietaColtivate_3()"

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        '----- Variabili
        Dim i As Integer
        Dim Dt As DataTable
        Dim des, cod As String
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data_Inizio, Data_Fine)

        Dt = objImpianti.Leggi_SpecieVarieta(Piva,
                                            Sa_Cod,
                                            0, 0, Veg_Cod, 0,
                                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                            xFiltroAggiuntivo,
                                            xOrderBy,
                                            objParametri_Server)

        objParametri_Server.ResettaFinestra()


        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                des = Dt.Rows(i).Item("cul_des")
                cod = Dt.Rows(i).Item("cul_cod")

                If IsNothing(Controllo.Items.FindByValue(cod)) Then
                    Controllo.Items.Add(New ListItem(des, cod))
                End If

            Next

        End If

        ''Aggiungo la voce relativa al terreno nudo
        'If ConsideraTerrenoNudo = True Then
        '    Controllo.Items.Add(New ListItem("Terreno Nudo", _
        '                                        "0"))
        'End If


    End Sub





    '###############################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	13/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub SpecieVegetale_OpMultiAziendali(ByRef Controllo As ListControl,
                                                    ByVal PrimaRiga_Flag As Boolean,
                                                    ByVal PrimaRiga_Text As String,
                                                    ByVal PrimaRiga_Value As String,
                                                    ByVal Veg_Cod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    )

        'Azzero il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Dim x_Des As String

        Dim objVegDes As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

        x_Des = objVegDes.VegDes_from_VegCod(Veg_Cod, objParametri)

        Controllo.Items.Add(New ListItem(x_Des, Veg_Cod))

    End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="TestoRicerca"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	14/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub InsettiUtili(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal TestoRicerca As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objInsetti.Leggi(0,
                            objParametri.FinestraTemporaleInizio,
                            objParametri.FinestraTemporaleFine,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) Then

            If TestoRicerca <> "" Then

                Dim Dr As DataRow()

                Dr = Dt.Select("Ins_Des LIKE '%" & TestoRicerca & "%'")

                If Dr.Length <> 0 Then

                    For i = 0 To Dr.Length - 1

                        Controllo.Items.Add(New ListItem(Dr(i)("ins_des"),
                                                          Dr(i)("ins_cod")))

                    Next

                End If

            Else

                If Dt.Rows.Count <> 0 Then

                    For i = 0 To Dt.Rows.Count - 1

                        Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("ins_des"),
                                                          Dt.Rows(i).Item("ins_cod")))

                    Next

                End If

            End If

        End If

    End Sub


    Public Shared Sub InsettiUtiliXAvversitaEGruppi(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal TestoRicerca As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objInsetti.Leggi(0,
                            objParametri.FinestraTemporaleInizio,
                            objParametri.FinestraTemporaleFine,
                            enumSelezioneVariabile.Selezione_JoinCompleta,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) Then

            If TestoRicerca <> "" Then

                Dim Dr As DataRow()

                Dr = Dt.Select("Ins_Des LIKE '%" & TestoRicerca & "%'")

                If Dr.Length <> 0 Then

                    For i = 0 To Dr.Length - 1

                        Controllo.Items.Add(New ListItem(Dr(i)("ins_des"),
                                                          Dr(i)("ins_cod")))

                    Next

                End If

            Else

                If Dt.Rows.Count <> 0 Then

                    For i = 0 To Dt.Rows.Count - 1

                        Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("ins_des"),
                                                          Dt.Rows(i).Item("ins_cod")))

                    Next

                End If

            End If

        End If

    End Sub


    Public Shared Sub InsettiUtiliSenzaAvversitaEGruppi(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal TestoRicerca As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objInsetti.LeggiSenzaAversitaCollegate(0,
                            objParametri.FinestraTemporaleInizio,
                            objParametri.FinestraTemporaleFine,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) Then

            If TestoRicerca <> "" Then

                Dim Dr As DataRow()

                Dr = Dt.Select("Ins_Des LIKE '%" & TestoRicerca & "%'")

                If Dr.Length <> 0 Then

                    For i = 0 To Dr.Length - 1

                        Controllo.Items.Add(New ListItem(Dr(i)("ins_des"),
                                                          Dr(i)("ins_cod")))

                    Next

                End If

            Else

                If Dt.Rows.Count <> 0 Then

                    For i = 0 To Dt.Rows.Count - 1

                        Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("ins_des"),
                                                          Dt.Rows(i).Item("ins_cod")))

                    Next

                End If

            End If

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Av_Cod"></param>
    ''' <param name="Uso"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	14/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub TrappolexSpecieVegetalixAvversita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Veg_Cod As Integer,
                                ByVal Av_Cod As Integer,
                                ByVal Uso As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreMetaSchemaDAL.Trappole_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        dt = obj.Leggi(0, Uso, Veg_Cod, Av_Cod,
                       enumSelezioneVariabile.Selezione_JoinDescrizioni,
                       xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then

            For i = 0 To dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(dt.Rows(i).Item("TRAP_DES"),
                                                  dt.Rows(i).Item("TRAP_COD")))

            Next

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Causale"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Destinazione"></param>
    ''' <param name="Uso"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Av_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametriServer"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	14/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub TrappoleMagazzino(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByRef Causale As enum_Agenda_Causali,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Destinazione As Integer,
                                ByVal Uso As Integer,
                                ByVal Veg_Cod As Integer,
                                ByVal Av_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametriServer As AgronicaCoreParametri,
                                ByRef objParametriUtenti As AgronicaCoreParametri)

        '--------------------------------------
        'NOTA: USO vale: 
        ' 1 = Installazione Trappole
        ' 2 = Cattura di Massa
        ' 3 = Confusione Sessuale
        ' 4 = Disorientamento Sessuale
        '--------------------------------------

        Dim i, j As Integer

        Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim objTrappole As New AgronicaCoreMetaSchemaDAL.Trappole_R

        Dim DtGiacenze As DataTable
        Dim DtTrappole As DataTable


        Dim Dt As New DataTable
        Dim Dr As DataRow

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Valore", GetType(String)))

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Select Case Causale

            Case enum_Agenda_Causali.CARICO

                DtTrappole = objTrappole.Leggi(0, Uso, Veg_Cod, Av_Cod,
                                             enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                             "",
                                             "TRAP_DES Asc",
                                             objParametriServer)

                If DtTrappole IsNot Nothing Then
                    For i = 0 To DtTrappole.Rows.Count - 1
                        Dr = Dt.NewRow

                        Dr.Item("Descrizione") = DtTrappole.Rows(i).Item("trap_des")
                        Dr.Item("Valore") = DtTrappole.Rows(i).Item("trap_cod")

                        Dt.Rows.Add(Dr)
                    Next
                End If


            Case enum_Agenda_Causali.SCARICO

                'DtGiacenze = objGiacenze.LeggiGiacenze(CStr(Piva), _
                '                                       CInt(Sa_Cod), _
                '                                       CInt(197), _
                '                                       0, 0, 0, _
                '                                       Destinazione, _
                '                                       0, 0, 0, "", _
                '                                       "", "", objParametriServer)


                DtGiacenze = New AgronicaCoreContabDAL.Giacenze_R().SchedaGiacenzeMagazzino(
                                     AGRODATAFINE,
                                     Piva,
                                     Sa_Cod,
                                     Destinazione,
                                      CInt(197),
                                      0,
                                      0,
                                       0, 0, 0, 0,
                                      LOTTO_NONDEFINITO, False,
                                      xFiltroAggiuntivo, "", "", "", "", "", "", "", "", "", "", xOrderBy,
                                     objParametriServer, objParametriUtenti)



                If DtGiacenze IsNot Nothing Then

                    For i = 0 To DtGiacenze.Rows.Count - 1
                        DtTrappole = objTrappole.Leggi(CInt(DtGiacenze.Rows(i).Item("Pro_Cod")),
                                                        Uso,
                                                        Veg_Cod,
                                                        Av_Cod,
                                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "", "", objParametriServer)

                        If DtTrappole IsNot Nothing Then

                            For j = 0 To DtTrappole.Rows.Count - 1

                                Dr = Dt.NewRow

                                Dr.Item("Descrizione") = DtTrappole.Rows(j).Item("trap_des")
                                Dr.Item("Valore") = DtTrappole.Rows(j).Item("trap_cod")

                                Dt.Rows.Add(Dr)

                            Next

                        End If

                    Next

                End If

        End Select


        Select Case Dt.Rows.Count

            Case 1
                Controllo.Items.Add(New ListItem(Dt.Rows(0).Item("Descrizione"), Dt.Rows(0).Item("Valore")))

            Case Is > 1
                'uso il dataview per ordinare 
                Dim Dv As New DataView

                Dt.TableName = "Prodotti"
                Dv.Table = Dt
                Dv.Sort = "Descrizione ASC"

                For i = 0 To Dv.Count - 1
                    If i = 0 OrElse Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
                        'Aggiungo l'elemento al primo giro oppure quando un elemento differisce dal precedente
                        Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
                    End If

                Next

        End Select

        objGiacenze = Nothing
        objTrappole = Nothing

        'Elimino il recordset
        DtGiacenze = Nothing
        DtTrappole = Nothing

    End Sub

    Public Shared Sub InsettiUtiliMagazzino(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByRef Causale As enum_Agenda_Causali,
                               ByRef Piva As String,
                               ByRef Sa_Cod As Integer,
                               ByRef Destinazione As Integer,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametriServer As AgronicaCoreParametri,
                               ByRef objParametriUtenti As AgronicaCoreParametri)


        Dim i, j As Integer

        Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R

        Dim DtGiacenze As DataTable
        Dim DtInsetti As DataTable


        Dim Dt As New DataTable
        Dim Dr As DataRow

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Valore", GetType(String)))

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Select Case Causale

            Case enum_Agenda_Causali.CARICO

                DtInsetti = objInsetti.Leggi(0,
                                             AGRODATAINIZIO, AGRODATAFINE,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "",
                                             "Ins_DES Asc",
                                             objParametriServer)

                If DtInsetti IsNot Nothing Then
                    For i = 0 To DtInsetti.Rows.Count - 1
                        Dr = Dt.NewRow

                        Dr.Item("Descrizione") = DtInsetti.Rows(i).Item("ins_des")
                        Dr.Item("Valore") = DtInsetti.Rows(i).Item("ins_cod")

                        Dt.Rows.Add(Dr)
                    Next
                End If


            Case enum_Agenda_Causali.SCARICO

                DtGiacenze = New AgronicaCoreContabDAL.Giacenze_R().SchedaGiacenzeMagazzino(
                                     AGRODATAFINE,
                                     Piva,
                                     Sa_Cod,
                                     Destinazione,
                                      INSETTI,
                                      0,
                                      0,
                                       0, 0, 0, 0,
                                      LOTTO_NONDEFINITO, False,
                                      xFiltroAggiuntivo, "", "", "", "", "", "", "", "", "", "", xOrderBy,
                                     objParametriServer, objParametriUtenti)


                If DtGiacenze IsNot Nothing Then

                    For i = 0 To DtGiacenze.Rows.Count - 1
                        DtInsetti = objInsetti.Leggi(CInt(DtGiacenze.Rows(i).Item("Pro_Cod")),
                                                     AGRODATAINIZIO, AGRODATAFINE,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "", objParametriServer)

                        If DtInsetti IsNot Nothing Then

                            For j = 0 To DtInsetti.Rows.Count - 1

                                Dr = Dt.NewRow

                                Dr.Item("Descrizione") = DtInsetti.Rows(j).Item("ins_des")
                                Dr.Item("Valore") = DtInsetti.Rows(j).Item("ins_cod")

                                Dt.Rows.Add(Dr)

                            Next

                        End If

                    Next

                End If

        End Select


        Select Case Dt.Rows.Count

            Case 1
                Controllo.Items.Add(New ListItem(Dt.Rows(0).Item("Descrizione"), Dt.Rows(0).Item("Valore")))

            Case Is > 1
                'uso il dataview per ordinare 
                Dim Dv As New DataView

                Dt.TableName = "Prodotti"
                Dv.Table = Dt
                Dv.Sort = "Descrizione ASC"

                For i = 0 To Dv.Count - 1
                    If i = 0 OrElse Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
                        Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
                    End If

                Next

        End Select


    End Sub

    Public Shared Sub InsettiUtiliXAvversitaEGruppi_Magazzino(ByRef Controllo As ListControl,
                           ByVal PrimaRiga_Flag As Boolean,
                           ByVal PrimaRiga_Text As String,
                           ByVal PrimaRiga_Value As String,
                           ByRef Causale As enum_Agenda_Causali,
                           ByRef Piva As String,
                           ByRef Sa_Cod As Integer,
                           ByRef Destinazione As Integer,
                           ByRef SoloSenzaAvversita As Boolean,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametriServer As AgronicaCoreParametri,
                           ByRef objParametriUtenti As AgronicaCoreParametri)

        Dim i, j As Integer

        Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R

        Dim DtGiacenze As DataTable
        Dim DtInsetti As DataTable


        Dim Dt As New DataTable
        Dim Dr As DataRow

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Valore", GetType(String)))

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Select Case Causale

            Case enum_Agenda_Causali.CARICO

                If SoloSenzaAvversita Then

                    DtInsetti = objInsetti.LeggiSenzaAversitaCollegate(0,
                                                              AGRODATAINIZIO, AGRODATAFINE,
                                                              xFiltroAggiuntivo,
                                                              "Ins_DES Asc",
                                                              objParametriServer)

                Else

                    DtInsetti = objInsetti.Leggi(0,
                                                 AGRODATAINIZIO, AGRODATAFINE,
                                                 enumSelezioneVariabile.Selezione_JoinCompleta,
                                                 xFiltroAggiuntivo,
                                                 "Ins_DES Asc",
                                                 objParametriServer)
                End If


                If DtInsetti IsNot Nothing Then
                    For i = 0 To DtInsetti.Rows.Count - 1
                        Dr = Dt.NewRow

                        Dr.Item("Descrizione") = DtInsetti.Rows(i).Item("ins_des")
                        Dr.Item("Valore") = DtInsetti.Rows(i).Item("ins_cod")

                        Dt.Rows.Add(Dr)
                    Next
                End If


            Case enum_Agenda_Causali.SCARICO

                Dim FiltroTmp As String = ""
                If xFiltroAggiuntivo <> "" AndAlso Not Trim(xFiltroAggiuntivo).StartsWith("AND") Then
                    FiltroTmp = " AND " & xFiltroAggiuntivo
                End If
                DtGiacenze = New AgronicaCoreContabDAL.Giacenze_R().SchedaGiacenzeMagazzino(
                                     AGRODATAFINE,
                                     Piva,
                                     Sa_Cod,
                                     Destinazione,
                                      INSETTI,
                                      0,
                                      0,
                                       0, 0, 0, 0,
                                      LOTTO_NONDEFINITO, False,
                                      FiltroTmp, "", "", "", "", "", "", "", "", "", "", xOrderBy,
                                     objParametriServer, objParametriUtenti)


                If DtGiacenze IsNot Nothing Then

                    For i = 0 To DtGiacenze.Rows.Count - 1

                        If SoloSenzaAvversita Then

                            DtInsetti = objInsetti.LeggiSenzaAversitaCollegate(CInt(DtGiacenze.Rows(i).Item("Pro_Cod")),
                                                                      AGRODATAINIZIO, AGRODATAFINE,
                                                                      xFiltroAggiuntivo,
                                                                      "Ins_DES Asc",
                                                                      objParametriServer)

                        Else

                            DtInsetti = objInsetti.Leggi(CInt(DtGiacenze.Rows(i).Item("Pro_Cod")),
                                                                                 AGRODATAINIZIO, AGRODATAFINE,
                                                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                                    xFiltroAggiuntivo, "", objParametriServer)

                        End If

                        If DtInsetti IsNot Nothing Then

                            For j = 0 To DtInsetti.Rows.Count - 1

                                Dr = Dt.NewRow

                                Dr.Item("Descrizione") = DtInsetti.Rows(j).Item("ins_des")
                                Dr.Item("Valore") = DtInsetti.Rows(j).Item("ins_cod")

                                Dt.Rows.Add(Dr)

                            Next

                        End If

                    Next

                End If

        End Select


        Select Case Dt.Rows.Count

            Case 1
                Controllo.Items.Add(New ListItem(Dt.Rows(0).Item("Descrizione"), Dt.Rows(0).Item("Valore")))

            Case Is > 1
                'uso il dataview per ordinare 
                Dim Dv As New DataView

                Dt.TableName = "Prodotti"
                Dv.Table = Dt
                Dv.Sort = "Descrizione ASC"

                For i = 0 To Dv.Count - 1
                    If i = 0 OrElse Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
                        'Aggiungo l'elemento al primo giro oppure quando un elemento differisce dal precedente
                        Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
                    End If

                Next

        End Select

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Trap_Cod"></param>
    ''' <param name="Uso"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	14/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub AvversitaxTrappole(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Veg_Cod As Integer,
                                ByVal Trap_Cod As Integer,
                                ByVal Uso As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                )

        '--------------------------------------
        'NOTA: USO vale: 
        ' 1 = Installazione Trappole
        ' 2 = Cattura di Massa
        ' 3 = Confusione Sessuale
        ' 4 = Disorientamento Sessuale
        '--------------------------------------

        Dim Dt = Leggi_AvversitaxTrappole(Veg_Cod,
                                          Trap_Cod,
                                          Uso,
                                          xFiltroAggiuntivo,
                                          xOrderBy,
                                          objParametri)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then

            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))

        End If

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then

                For i = 0 To Dt.Rows.Count - 1

                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("AV_DES_VOL"),
                                                          Dt.Rows(i).Item("AV_COD")))

                Next

            End If

            Dt = Nothing

        End If

    End Sub

    Public Function Leggi_AvversitaxTrappole(ByVal Veg_Cod As Integer,
                                             ByVal Trap_Cod As Integer,
                                             ByVal Uso As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Dim StrSQL As New StringBuilder

        Dim Dt As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.Leggi_AvversitaxTrappole()"

        StrSQL.Length = 0

        StrSQL.Append(" SELECT DISTINCT Avversita.* ")
        StrSQL.Append(" FROM TrappoleXAvversita, Avversita, SpecieVegetaliXAvversita,Trappole ")
        StrSQL.Append(" WHERE SpecieVegetaliXAvversita.Av_Cod = Avversita.Av_Cod ")
        StrSQL.Append(" AND SpecieVegetaliXAvversita.Av_Cod = TrappoleXAvversita.Av_Cod ")
        StrSQL.Append(" AND Trappole.Trap_Cod = TrappoleXAvversita.Trap_Cod ")
        StrSQL.Append(" AND (Av_Des_Vol NOT LIKE '%non usare%') AND (Av_Des_Vol NOT LIKE '%(#)%') AND (Av_Des_Lat NOT LIKE '%non usare%') AND (Av_Des_Lat NOT LIKE '%(#)%') ")

        If Uso <> 0 Then
            StrSQL.Append(" AND Trappole.Uso = " & Uso.ToString & " ")
        End If

        If Veg_Cod <> 0 Then
            StrSQL.Append(" AND SpecieVegetaliXAvversita.Veg_Cod=" & Veg_Cod.ToString & " ")
        End If

        If Trap_Cod <> 0 Then
            StrSQL.Append(" AND Trappole.Trap_Cod = " & Trap_Cod.ToString & " ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
        End If

        If xOrderBy <> "" Then
            StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & " ")
        End If

        Dim objDP As New AgronicaCoreDataProvider.DataProvider

        Dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        objDP = Nothing

        Return Dt

    End Function

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Trap_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	14/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub DittexTrappole(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Trap_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim objDP As New AgronicaCoreDataProvider.DataProvider
        Dim StrSQL As New StringBuilder

        Dim Dt As DataTable
        Dim i As Integer

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.DittexTrappole()"

        StrSQL.Length = 0

        StrSQL.Append(" SELECT Ditte.* ")
        StrSQL.Append(" FROM TrappoleXDitta, Ditte ")
        StrSQL.Append(" WHERE TrappoleXDitta.Ditta_Cod = Ditte.Ditta_Cod ")

        If Trap_Cod <> 0 Then
            StrSQL.Append(" AND TrappoleXDitta.Trap_Cod=" & Trap_Cod.ToString & " ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
        End If

        If xOrderBy <> "" Then
            StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & " ")
        End If

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        objDP = Nothing

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then

                For i = 0 To Dt.Rows.Count - 1

                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("DITTA_DES"),
                                                      Dt.Rows(i).Item("DITTA_COD")))

                Next

            End If

            Dt = Nothing

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Ind_Mat_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	19/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub UdmIndiciMaturita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Ind_Mat_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objUdm As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objUdm.Leggi(Ind_Mat_Cod, 0,
                        enumSelezioneVariabile.Selezione_JoinCompleta,
                        "", "", objParametri)

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then

                For i = 0 To Dt.Rows.Count - 1

                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Udm_sim"),
                                                      Dt.Rows(i).Item("Udm_cod")))

                Next

            Else
                'se non ci sono udm allora inserisco l'elemento indefinito
                Controllo.Items.Add(New ListItem("Indefinito", -1))

            End If

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Fabbricato_Cod"></param>
    ''' <param name="Cod_Progetto"></param>
    ''' <param name="Gen_Cod"></param>
    ''' <param name="Spe_Cod"></param>
    ''' <param name="Ipro_Cod"></param>
    ''' <param name="Raz_Cod"></param>
    ''' <param name="Data_1"></param>
    ''' <param name="Data_2"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	20/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub ConsistenzeRaggruppamenti(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Fabbricato_Cod As Integer,
                                ByVal Cod_Progetto As Integer,
                                ByVal Gen_Cod As Integer,
                                ByVal Spe_Cod As Integer,
                                ByVal Ipro_Cod As Integer,
                                ByVal Raz_Cod As Integer,
                                ByVal Data_1 As Date,
                                ByVal Data_2 As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i, j As Integer
        Dim objConsistenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim objIprod As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R

        Dim Ipro_Des As String

        Dim Doppio As Boolean = False

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objConsistenze.LeggiConsistenzeRaggruppamenti(CStr(Piva),
                                                            CInt(Sa_Cod),
                                                            0, 0, 0, 0,
                                                            CInt(Fabbricato_Cod),
                                                            0,
                                                            CInt(Cod_Progetto),
                                                            0, "",
                                                            CInt(Gen_Cod),
                                                            CInt(Spe_Cod),
                                                            CInt(Ipro_Cod),
                                                            CInt(Raz_Cod),
                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            "", "", objParametri)

        objConsistenze = Nothing


        'Verifico se il recordset e' aperto
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For j = 0 To Dt.Rows.Count - 1

                Doppio = False
                For i = 0 To Controllo.Items.Count - 1
                    If Controllo.Items(i).Value = Dt.Rows(j).Item("Gen_Cod") & "|" &
                                            Dt.Rows(j).Item("Spe_Cod") & "|" &
                                            Dt.Rows(j).Item("IPro_Cod") Then
                        Doppio = True
                        Exit For
                    End If
                Next


                If Not Doppio Then

                    Ipro_Des = objIprod.StallaIProDes_from_StallaIProCod(CInt(Dt.Rows(j).Item("Gen_Cod")),
                                                                        CInt(Dt.Rows(j).Item("Spe_Cod")),
                                                                        CInt(Dt.Rows(j).Item("IPro_Cod")),
                                                                        objParametri)


                    Controllo.Items.Add(New ListItem(Ipro_Des,
                                                Dt.Rows(j).Item("Gen_Cod") & "|" &
                                                Dt.Rows(j).Item("Spe_Cod") & "|" &
                                                Dt.Rows(j).Item("IPro_Cod")))
                End If

            Next

        End If


        Dt = Nothing

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Gen_Cod"></param>
    ''' <param name="Spe_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	20/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Lista_IndirizziProd_Animali(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Gen_Cod As Integer,
                                ByVal Spe_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objIndProd As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objIndProd.Leggi(CInt(Gen_Cod),
                            CInt(Spe_Cod),
                            -1,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "", "", objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("IPRO_DES"),
                                                  Dt.Rows(i).Item("Gen_Cod") & "|" &
                                                  Dt.Rows(i).Item("Spe_Cod") & "|" &
                                                  Dt.Rows(i).Item("IPro_Cod")))

            Next

        End If

    End Sub



    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Gen_Cod"></param>
    ''' <param name="Spe_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	20/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Lista_Tipi_Stalla(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Gen_Cod As Integer,
                                ByVal Spe_Cod As Integer,
                                ByVal IPro_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objCOM As New AgronicaCoreZooDAL.Lista_tipi_stalla_R  'Object    'New Agro_Zoo_AD.Lista_tipi_stalla_R
        Dim Tipo As String()

        Dim Descr As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        Dt = objCOM.Leggi(CInt(Gen_Cod),
                          CInt(Spe_Cod),
                          CInt(IPro_Cod),
                          "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                          "IPro_Cod=" & IPro_Cod,
                          "",
                          objParametri)

        'Elimino gli oggetti COM
        objCOM = Nothing

        'Pulisco la combo
        Controllo.Items.Clear()

        'Se il recordset non è chiuso allora ...	
        If Dt.Rows.Count > 0 Then

            'filtro si ipro_cod

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1
                'Controllo il numero di dettagli
                Tipo = Split(Dt.Rows(i).Item("Cod_Fabb"), ".")

                If UBound(Tipo, 1) <= 4 Then

                    'TIPO Ricovero
                    Dim objListaTi As New AgronicaCoreZooDAL.Lista_tipi_fabbricati_R
                    Descr = objListaTi.Lista_Tipi_Fabbricati_Descr_from_COD_FABB(CStr(Dt.Rows(i).Item("Cod_Fabb")), objParametri)

                    If Descr <> "" Then

                        Controllo.Items.Add(New ListItem(Descr,
                                                   Dt.Rows(i).Item("Gen_Cod") & "|" &
                                                   Dt.Rows(i).Item("Spe_Cod") & "|" &
                                                   Dt.Rows(i).Item("IPro_Cod") & "|" &
                                                   Dt.Rows(i).Item("Cod_Fabb")))

                    End If

                End If

            Next

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Gen_Cod"></param>
    ''' <param name="Spe_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	20/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Lista_Sottotipi_Stalla(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Gen_Cod As Integer,
                                ByVal Spe_Cod As Integer,
                                ByVal IPro_Cod As Integer,
                                ByVal Cod_Fabb As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objCOM As New AgronicaCoreZooDAL.Lista_tipi_stalla_R

        Dim FabbSplit As String()
        Dim Lunghezza As Integer
        Dim TipoSplit As String()
        Dim bOk As Boolean
        Dim Descr As String


        FabbSplit = Split(CStr(Cod_Fabb), ".")
        Lunghezza = UBound(FabbSplit, 1) + 1

        If Lunghezza = 5 Then
            'Pulisco il controllo
            Controllo.Items.Clear()

            If PrimaRiga_Flag Then
                Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
            End If

            'Leggo le imprese associate al profilo selezionato			
            Dt = objCOM.Leggi(CInt(Gen_Cod),
                              CInt(Spe_Cod),
                              CInt(IPro_Cod),
                              "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                              "",
                              "",
                              objParametri)

            'Elimino gli oggetti COM
            objCOM = Nothing

            'Pulisco la combo
            Controllo.Items.Clear()
            If Dt.Rows.Count > 0 Then
                Dim j As Integer
                For i = 0 To Dt.Rows.Count - 1
                    'Controllo il numero di dettagli
                    TipoSplit = Split(Dt.Rows(i).Item("Cod_Fabb"), ".")

                    'Primo Controllo: il dettaglio ha almeno 5 livelli
                    If UBound(TipoSplit, 1) > 4 Then

                        'Secondo Controllo: il Sottotipo ha la stessa radice del Tipo
                        bOk = True

                        For j = 0 To Lunghezza - 1

                            If CInt(TipoSplit(j)) <> CInt(FabbSplit(j)) Then
                                'Radice Diversa
                                bOk = False
                                Exit For

                            End If

                        Next j

                        If bOk Then


                            'duplicato in agronicacorZoo lista_Tipi_fabbricati_r Lista_Tipi_Fabbricati_Descr_from_COD_FABB
                            Dim objListaT As New AgronicaCoreZooDAL.Lista_tipi_fabbricati_R
                            'SOTTOTIPO Ricovero
                            Descr = objListaT.Lista_Tipi_Fabbricati_Descr_from_COD_FABB(CStr(Dt.Rows(i).Item("Cod_Fabb")), objParametri)

                            If Descr <> "" Then

                                Controllo.Items.Add(New ListItem(Descr,
                                                            Dt.Rows(i).Item("Gen_Cod") & "|" &
                                                            Dt.Rows(i).Item("Spe_Cod") & "|" &
                                                            Dt.Rows(i).Item("IPro_Cod") & "|" &
                                                            Dt.Rows(i).Item("Cod_Fabb")))

                            End If

                        End If

                    End If

                Next

            End If

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Gen_Cod"></param>
    ''' <param name="Spe_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	20/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Lista_Razze_Animali(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Gen_Cod As Integer,
                                ByVal Spe_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objRazze As New AgronicaCoreZooDAL.Lista_Razze_Animali_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objRazze.Leggi(Gen_Cod,
                            Spe_Cod,
                            -1,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "", "", objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Raz_Des"),
                                       Dt.Rows(i).Item("Gen_Cod") & "|" &
                                       Dt.Rows(i).Item("Spe_Cod") & "|" &
                                       Dt.Rows(i).Item("Raz_Cod")))

            Next

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	04/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Regolamento(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objReg As New AgronicaCoreMetaSchemaDAL.Regolamenti_R

        'Recupero il recordset
        Dt = objReg.Leggi(0,
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          xFiltroAggiuntivo, xOrderBy,
                          objParametri)

        If Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Select Case Dt.Rows(i).Item("Reg_COD")

                    Case 1
                        Controllo.Items.Insert(0, New ListItem(Dt.Rows(i).Item("Reg_DES"),
                                                   Dt.Rows(i).Item("Reg_COD")))

                    Case Else
                        Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Reg_DES"),
                                                   Dt.Rows(i).Item("Reg_COD")))

                End Select


            Next

        End If

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	04/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Ditte(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objDitte As New AgronicaCoreMetaSchemaDAL.Ditte_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objDitte.Leggi(0, "", "", "", objParametri)


        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Ditta_DES"),
                                                  Dt.Rows(i).Item("Ditta_COD")))

            Next

        End If

    End Sub

    Public Shared Sub SpecieVegetale_DaFiltroSementieri_Optimize(
                ByRef Controllo As ListControl,
                ByVal PrimaRiga_Flag As Boolean,
                ByVal PrimaRiga_Text As String,
                ByVal PrimaRiga_Value As String,
                ByVal id_Specie As Integer,
                ByVal id_SottoSpecie As Integer,
                ByVal id_Gruppo As Integer,
                ByVal id_Genotipo As Integer,
                ByVal LetteraIniziale As String,
                ByVal Flag_FiltroUtente As Boolean,
                ByVal Veg_Cod As Integer,
                ByVal Veg_Cod_daModificare As Integer,
                ByVal GruCod_Rif_VegCod_daModificare As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                )

        Dim Dt As DataTable
        Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False


        Controllo.Items.Clear()

        Dt = SpecieVegetale_Optimize_GetDatatable(id_Specie, id_SottoSpecie, id_Gruppo, id_Genotipo, 0, Flag_FiltroUtente, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti, Nothing)

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        For Each dRow As DataRow In Dt.Rows
            Controllo.Items.Add(New ListItem(dRow("veg_des"), dRow("veg_cod")))
        Next

    End Sub


    Private Shared Function SpecieVegetale_Optimize_GetDatatable(ByVal id_Specie As Integer, ByVal id_SottoSpecie As Integer, ByVal id_gruppo As Integer, ByVal id_genotipo As Integer, ByVal Gru_Cod As Integer, ByVal Flag_FiltroUtente As Boolean, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objSpecVeg As AgronicaCoreMetaSchemaDAL.SpecieVegetali_R) As DataTable

        Dim objSpecVegSementi As New AgronicaCoreSementieriDAL.ClassiDiSpecie_R

        Dim dt As DataTable

        If id_Specie <> -1 Then

            dt = objSpecVegSementi.SpecieAgronicaFromClasseDiSpecie(
                id_Specie,
                id_SottoSpecie,
                id_gruppo,
                id_genotipo,
                objParametri_Server
            )


        Else

            If objSpecVeg Is Nothing Then
                objSpecVeg = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            End If

            If Flag_FiltroUtente Then

                dt = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(0,
                                                                        Gru_Cod,
                                                                        "",
                                                                        "",
                                                                        xFiltroAggiuntivo,
                                                                        xOrderBy,
                                                                        objParametri_Utenti)

            Else

                dt = objSpecVeg.Leggi(0,
                        Gru_Cod,
                        "",
                        "",
                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                        xFiltroAggiuntivo,
                        xOrderBy,
                        objParametri_Utenti)

            End If
        End If

        Return dt
    End Function
    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Gru_Cod"></param>
    ''' <param name="LetteraIniziale"></param>
    ''' <param name="Flag_FiltroUtente"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Veg_Cod_daModificare"></param>
    ''' <param name="GruCod_Rif_VegCod_daModificare"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	04/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub SpecieVegetale_Optimize(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Gru_Cod As Integer,
                                ByVal LetteraIniziale As String,
                                ByVal Flag_FiltroUtente As Boolean,
                                ByVal Veg_Cod As Integer,
                                ByVal Veg_Cod_daModificare As Integer,
                                ByVal GruCod_Rif_VegCod_daModificare As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer
        Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False


        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
        Dt = SpecieVegetale_Optimize_GetDatatable(-1, -1, -1, -1, Gru_Cod, Flag_FiltroUtente, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti, objSpecVeg)


        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            'uso il dataview per ordinare
            Dim Dv As New DataView

            Dt.TableName = "specie"
            Dv.Table = Dt
            Dv.Sort = "Veg_Des ASC"

            If Not IsNothing(Dv) Then

                For i = 0 To NumTotale - 1

                    'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
                    'faccio il controllo
                    If Veg_Cod_daModificare <> 0 Then

                        ''se l'utente ha il filtro specie impostato
                        'If Flag_FiltroUtenteImpostato = True Then

                        'se all'interno delle specie filtrate, trovo il veg_cod da modificare
                        If Dv.Item(i).Item("Veg_Cod") = Veg_Cod_daModificare Then
                            Flag_VegCodTrovatoNelFiltroUtente = True
                        End If
                        'Else
                        '    'non c'è un filtro impostato
                        '    'quindi imposto il flag, come se avessi trovato la specie
                        '    'così non devo aggiungere niente
                        '    Flag_VegCodTrovatoNelFiltroUtente = True
                        'End If
                    Else
                        'non è stato passato il veg_cod
                        'quindi imposto il flag, come se avessi trovato la specie
                        'così non devo aggiungere niente
                        Flag_VegCodTrovatoNelFiltroUtente = True
                    End If

                    x_Cod = Dv.Item(i).Item("Veg_Cod")

                    If x_Cod = 0 Then
                        x_Des = Str_TerrenoNudo
                    Else
                        x_Des = CStr(Dv.Item(i).Item("Veg_DES"))
                    End If

                    If LetteraIniziale = "" Then
                        Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                    Else
                        If UCase(Mid(x_Des, 1, 1)) = UCase(LetteraIniziale) Then
                            Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                        End If
                    End If

                Next

            End If

            Dv = Nothing

        Else
            NumTotale = 0
        End If

        Dt = Nothing


        'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
        If Veg_Cod_daModificare <> 0 Then

            ''se l'utente ha il filtro specie impostato
            'If Flag_FiltroUtenteImpostato = True Then

            'se all'interno delle specie filtrate, non è stato trovato il veg_cod da modificare
            If Not Flag_VegCodTrovatoNelFiltroUtente Then

                x_Des = ""

                'se il Veg_Cod_daModificare è valorizzato, devo sapere anche il gru_cod
                '(se non l'ho passato me lo ricavo e intanto che ci sono ricavo la descrizione della specie)
                'però solo nel caso di gru_cod è <> 0 (ovvero impianto e campo)
                If Gru_Cod <> 0 AndAlso GruCod_Rif_VegCod_daModificare = 0 Then
                    GruCod_Rif_VegCod_daModificare = objSpecVeg.GruCod_and_VegDes_from_VegCod(x_Des, Veg_Cod_daModificare, objParametri_Server)
                End If

                'se il gru_cod della specie da modificare è lo stesso di quello selezionato,
                'allora inserisco la specie vegetale da modificare nel menù
                If Gru_Cod = GruCod_Rif_VegCod_daModificare Then

                    'se la descrizione non l'ho già ricavata sopra
                    If x_Des = "" Then


                        x_Des = objSpecVeg.VegDes_from_VegCod(Veg_Cod_daModificare, objParametri_Server)
                    End If

                    Controllo.Items.Add(New ListItem(x_Des, Veg_Cod_daModificare))

                End If

            End If

            'End If

        End If

    End Sub



    Public Shared Sub SpecieVegetale_Optimize_x_PDC(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByRef objParametri_Server As AgronicaCoreParametri)

        Dim Dt As DataTable

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
        Dt = objSpecVeg.Leggi_x_PDC(objParametri_Server)
        For Each dRow As DataRow In Dt.Rows
            Controllo.Items.Add(New ListItem(dRow("veg_des"), dRow("veg_cod")))
        Next

    End Sub

    ' @Paolo: aggiungo per unire il Gruppo Vegetale descrizione al testo della select Specie Vegetale
    Public Shared Sub SpecieVegetale_Optimize_x_PDC_Modificata(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByRef objParametri_Server As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim Testo_select As String

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
        Dt = objSpecVeg.Leggi_x_PDC_Modificata(objParametri_Server)
        For Each dRow As DataRow In Dt.Rows
            'Controllo.Items.Add(New ListItem(dRow("veg_des"), dRow("veg_cod")))
            Testo_select = dRow("veg_des").ToUpper() & " --- (" & dRow("Gruppo_Veg_Desc") & ")"
            Controllo.Items.Add(New ListItem(Testo_select, dRow("veg_cod")))
        Next

    End Sub

    ' @Paolo: aggiungo per unire il Gruppo Vegetale Codice al valore della select Specie Vegetale
    Public Shared Sub SpecieVegetale_Optimize_x_PDC_Modificata2(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByRef objParametri_Server As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim Testo_select As String

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
        Dt = objSpecVeg.Leggi_x_PDC_Modificata(objParametri_Server)
        For Each dRow As DataRow In Dt.Rows
            'Controllo.Items.Add(New ListItem(dRow("veg_des"), dRow("veg_cod")))
            Testo_select = dRow("veg_des").ToUpper() & " --- (" & dRow("Gruppo_Veg_Desc") & ")"
            Controllo.Items.Add(New ListItem(Testo_select, dRow("veg_cod") & "|" & dRow("Gru_Cod")))
        Next

    End Sub





    'Public Shared Sub SpecieVegetale_Coltivate_Optimize(ByRef Controllo As ListControl, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Integer, _
    '                        ByVal Data_Selezionata As Date, _
    '                        ByVal ConsideraTerrenoNudo As Boolean, _
    '                        ByVal PrimaRiga_Flag As Boolean, _
    '                        ByVal PrimaRiga_Text As String, _
    '                        ByVal PrimaRiga_Value As String, _
    '                        ByVal Gru_Cod As Integer, _
    '                        ByVal LetteraIniziale As String, _
    '                        ByVal Flag_FiltroUtente As Boolean, _
    '                        ByVal Veg_Cod As Integer, _
    '                        ByVal Veg_Cod_daModificare As Integer, _
    '                        ByVal GruCod_Rif_VegCod_daModificare As Integer, _
    '                        ByVal DettagliTerrenoNudo As Boolean, _
    '                        ByVal xFiltroAggiuntivo As String, _
    '                        ByVal xOrderBy As String, _
    '                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                        )

    '    Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Coltivate_Optimize()"
    '    Dim objDP As New AgronicaCoreDataProvider.DataProvider
    '    Dim Dt As DataTable
    '    Dim StrSQL As New System.Text.StringBuilder
    '    'Genero la query SQL


    '    If DettagliTerrenoNudo Then


    '        StrSQL.Length = 0
    '        StrSQL.Append(" (   ")
    '        StrSQL.Append(" SELECT DISTINCT    ")
    '        StrSQL.Append(" SpecieVegetali.Veg_Cod AS Veg_Cod, ")
    '        StrSQL.Append(" SpecieVegetali.Veg_Des AS Veg_Des, ")
    '        StrSQL.Append(" 0 as id_cod  ")

    '        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
    '        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
    '        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN ")
    '        StrSQL.Append(" SpecieVegetali INNER JOIN ")
    '        StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

    '        StrSQL.Append(" WHERE   1=1  ")
    '        If Piva <> "" Then
    '            StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        End If


    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If

    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")

    '        StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")

    '        If ConsideraTerrenoNudo = False Then
    '            StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
    '        End If

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If


    '        StrSQL.Append(" ) UNION ( ")


    '        StrSQL.Append(" SELECT DISTINCT    ")
    '        StrSQL.Append(" Reg_Impianti.CUL_COD as veg_cod , ISNULL(Codici_Anagrafe.descrizione, '') AS veg_des, ISNULL(Codici_Anagrafe.codice, 0) AS id_cod ")

    '        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
    '        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
    '        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ")

    '        StrSQL.Append("  LEFT OUTER JOIN ")
    '        StrSQL.Append(" Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND  ")
    '        StrSQL.Append(" Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg LEFT OUTER JOIN ")
    '        StrSQL.Append(" Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice AND Reg_Impianti_Codici.id_cod >= 3000 AND Reg_Impianti_Codici.id_cod <= 4000 ")
    '        StrSQL.Append(" WHERE     (Reg_Impianti.CUL_COD = 0) ")

    '        If Piva <> "" Then
    '            StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        End If


    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If

    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")


    '        StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")


    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        StrSQL.Append(" )   ")

    '        If xOrderBy <> "" Then
    '            strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        Else
    '            StrSQL.Append(" ORDER BY VEG_DES")
    '        End If

    '        Dt = objDP.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

    '        Dim lista As String = ""
    '        Dim i As Integer = 0
    '        For i = 0 To Dt.Rows.Count - 1
    '            lista &= ", " & Dt.Rows(i).Item("Veg_Cod")
    '        Next

    '        If xFiltroAggiuntivo = "" Then
    '            xFiltroAggiuntivo &= " SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
    '        Else
    '            xFiltroAggiuntivo &= "and SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
    '        End If

    '        'SpecieVegetale_Optimize_x_PDC(Controllo, PrimaRiga_Flag, PrimaRiga_Text, _
    '        '                             PrimaRiga_Value, objParametri_Server)

    '        SpecieVegetale_Optimize(Controllo, PrimaRiga_Flag, PrimaRiga_Text, _
    '                                     PrimaRiga_Value, Gru_Cod, LetteraIniziale, Flag_FiltroUtente, Veg_Cod, _
    '                                     Veg_Cod_daModificare, GruCod_Rif_VegCod_daModificare, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti)



    '        For i = 0 To Dt.Rows.Count - 1
    '            If Dt.Rows(i).Item("id_cod") <> "0" Then

    '                Dim x_Cod As String = Dt.Rows(i).Item("id_cod")

    '                Dim x_Des As String = Dt.Rows(i).Item("Veg_DES")

    '                Controllo.Items.Add(New ListItem(x_Des, "0/" & x_Cod))

    '            End If
    '        Next


    '    Else



    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT DISTINCT    ")
    '        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ")
    '        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS Veg_Des ")

    '        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
    '        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
    '        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA LEFT OUTER JOIN ")
    '        StrSQL.Append(" SpecieVegetali INNER JOIN ")
    '        StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

    '        StrSQL.Append(" WHERE   1=1  ")
    '        If Piva <> "" Then
    '            StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        End If


    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If
    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
    '        StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")

    '        If ConsideraTerrenoNudo = False Then
    '            StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
    '        End If

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If


    '        If xOrderBy <> "" Then
    '            strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        Else
    '            StrSQL.Append(" ORDER BY VEG_DES")
    '        End If


    '        Dt = objDP.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

    '        Dim lista As String = ""
    '        Dim i As Integer = 0
    '        For i = 0 To Dt.Rows.Count - 1
    '            lista &= ", " & Dt.Rows(i).Item("Veg_Cod")
    '        Next

    '        If xFiltroAggiuntivo = "" Then
    '            xFiltroAggiuntivo &= " SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
    '        Else
    '            xFiltroAggiuntivo &= "and SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
    '        End If

    '        SpecieVegetale_Optimize_x_PDC(Controllo, PrimaRiga_Flag, PrimaRiga_Text, _
    '                                     PrimaRiga_Value, objParametri_Server)





    '    End If




    'End Sub

    Public Sub SpecieVegetale_Coltivate_Optimize(ByRef Controllo As ListControl,
                        ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal Data_Selezionata As Date,
                        ByVal ConsideraTerrenoNudo As Boolean,
                        ByVal PrimaRiga_Flag As Boolean,
                        ByVal PrimaRiga_Text As String,
                        ByVal PrimaRiga_Value As String,
                        ByVal Gru_Cod As Integer,
                        ByVal LetteraIniziale As String,
                        ByVal Flag_FiltroUtente As Boolean,
                        ByVal Veg_Cod As Integer,
                        ByVal Veg_Cod_daModificare As Integer,
                        ByVal GruCod_Rif_VegCod_daModificare As Integer,
                        ByVal DettagliTerrenoNudo As Boolean,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByVal leggiAncheBloccati As Boolean,
                        Optional ByVal Campo_Cod As Integer = 0,
                        Optional ByVal FiltroCampi As String = "",
                        Optional ByVal Id_Budget As Integer = 0,
                        Optional ByVal Filtra_Validita_Esercizi As Boolean = False
                        )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Coltivate_Optimize()"
        'Dim objDP As New AgronicaCoreDataProvider.DataProvider
        Dim Dt As DataTable
        Dim StrSQL As New StringBuilder
        'Genero la query SQL
        Dim i As Integer
        Dim TabellaPrefisso As String = ""

        If DettagliTerrenoNudo Then

            If Id_Budget <> 0 Then
                TabellaPrefisso = "Budget_"
            End If

            StrSQL.Length = 0
            StrSQL.Append(" (   ")
            StrSQL.Append(" SELECT DISTINCT    ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod AS Veg_Cod, ")
            StrSQL.Append(" SpecieVegetali.Veg_Des AS Veg_Des, ")
            StrSQL.Append(" 0 as id_cod  ")

            StrSQL.Append(" FROM         " & TabellaPrefisso & "Appezzamento INNER JOIN ")
            StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti ON " & TabellaPrefisso & "Appezzamento.PIVA = " & TabellaPrefisso & "Reg_Impianti.PIVA AND " & TabellaPrefisso & "Appezzamento.SA_COD = " & TabellaPrefisso & "Reg_Impianti.SA_COD AND ")
            StrSQL.Append(" " & TabellaPrefisso & "Appezzamento.APPEZZA = " & TabellaPrefisso & "Reg_Impianti.APPEZZA INNER JOIN ")
            StrSQL.Append(" SpecieVegetali INNER JOIN ")
            StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON " & TabellaPrefisso & "Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            If Filtra_Validita_Esercizi Then
                StrSQL.Append(" INNER JOIN " & TabellaPrefisso & "Imprese_Progetti ON " & TabellaPrefisso & "Reg_Impianti.PIVA = " & TabellaPrefisso & "Imprese_Progetti.PIVA AND " & TabellaPrefisso & "Reg_Impianti.SA_COD = " & TabellaPrefisso & "Imprese_Progetti.SA_COD AND ")
                StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti.APPEZZA = " & TabellaPrefisso & "Imprese_Progetti.APPEZZA AND " & TabellaPrefisso & "Reg_Impianti.ID_REG = " & TabellaPrefisso & "Imprese_Progetti.ID_REG")
            End If

            StrSQL.Append(" WHERE   1=1  ")
            If Piva <> "" Then
                StrSQL.Append(" And     " & TabellaPrefisso & "Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If


            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable

                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri_Server)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND " & TabellaPrefisso & "Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                    End If
                End If

            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If
            If FiltroCampi <> "" Then
                StrSQL.Append(" AND    " & FiltroCampi & " ")
            End If

            StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
            StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")

            If Not leggiAncheBloccati Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Appezzamento.Blk_Flag <> -1 ")
            End If


            If Not ConsideraTerrenoNudo Then
                StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
            End If

            If Filtra_Validita_Esercizi Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
                StrSQL.Append(" AND     " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
            End If

            If Id_Budget <> 0 Then
                StrSQL.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If


            ' terreni nudi con solo destinazioni
            StrSQL.Append(" ) UNION ( ")


            StrSQL.Append(" SELECT DISTINCT    ")
            StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti.CUL_COD as veg_cod , ISNULL(Codici_Anagrafe.descrizione, '') AS veg_des, ISNULL(Codici_Anagrafe.codice, 0) AS id_cod ")

            StrSQL.Append(" FROM         " & TabellaPrefisso & "Appezzamento INNER JOIN ")
            StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti ON " & TabellaPrefisso & "Appezzamento.PIVA = " & TabellaPrefisso & "Reg_Impianti.PIVA AND " & TabellaPrefisso & "Appezzamento.SA_COD = " & TabellaPrefisso & "Reg_Impianti.SA_COD AND ")
            StrSQL.Append(" " & TabellaPrefisso & "Appezzamento.APPEZZA = " & TabellaPrefisso & "Reg_Impianti.APPEZZA ")

            If Filtra_Validita_Esercizi Then
                StrSQL.Append(" INNER JOIN " & TabellaPrefisso & "Imprese_Progetti ON " & TabellaPrefisso & "Reg_Impianti.PIVA = " & TabellaPrefisso & "Imprese_Progetti.PIVA AND " & TabellaPrefisso & "Reg_Impianti.SA_COD = " & TabellaPrefisso & "Imprese_Progetti.SA_COD AND ")
                StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti.APPEZZA = " & TabellaPrefisso & "Imprese_Progetti.APPEZZA AND " & TabellaPrefisso & "Reg_Impianti.ID_REG = " & TabellaPrefisso & "Imprese_Progetti.ID_REG")
            End If

            StrSQL.Append(" LEFT OUTER JOIN ")
            StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici ON " & TabellaPrefisso & "Reg_Impianti.PIVA = " & TabellaPrefisso & "Reg_Impianti_Codici.PIVA AND " & TabellaPrefisso & "Reg_Impianti.SA_COD = " & TabellaPrefisso & "Reg_Impianti_Codici.sa_cod AND  ")
            StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti.APPEZZA = " & TabellaPrefisso & "Reg_Impianti_Codici.appezza AND " & TabellaPrefisso & "Reg_Impianti.ID_REG = " & TabellaPrefisso & "Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.Append(" INNER JOIN Codici_Anagrafe ON " & TabellaPrefisso & "Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  ")
            StrSQL.Append(" WHERE " & TabellaPrefisso & "Reg_Impianti.CUL_COD = 0 ")

            StrSQL.Append(" AND " & TabellaPrefisso & "Reg_Impianti_Codici.id_cod >= 3000 AND " & TabellaPrefisso & "Reg_Impianti_Codici.id_cod <= 4000 ")

            If Piva <> "" Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable

                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri_Server)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND " & TabellaPrefisso & "Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                    End If
                End If

            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If
            If FiltroCampi <> "" Then
                StrSQL.Append(" AND    " & FiltroCampi & " ")
            End If

            StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
            StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")

            If Not leggiAncheBloccati Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Appezzamento.Blk_Flag <> -1 ")
            End If

            If Filtra_Validita_Esercizi Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
                StrSQL.Append(" AND     " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
            End If

            If Id_Budget <> 0 Then
                StrSQL.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            ' terreni nudi senza destinazioni
            StrSQL.Append(" ) UNION ( ")


            StrSQL.Append(" SELECT DISTINCT    ")
            'StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti.CUL_COD as veg_cod , ISNULL(Codici_Anagrafe.descrizione, '') AS veg_des, ISNULL(Codici_Anagrafe.codice, 0) AS id_cod ")
            StrSQL.Append(" 0 as veg_cod , 'Terreno Nudo' AS veg_des, 0 AS id_cod ")

            StrSQL.Append(" FROM         " & TabellaPrefisso & "Appezzamento INNER JOIN ")
            StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti ON " & TabellaPrefisso & "Appezzamento.PIVA = " & TabellaPrefisso & "Reg_Impianti.PIVA AND " & TabellaPrefisso & "Appezzamento.SA_COD = " & TabellaPrefisso & "Reg_Impianti.SA_COD AND ")
            StrSQL.Append(" " & TabellaPrefisso & "Appezzamento.APPEZZA = " & TabellaPrefisso & "Reg_Impianti.APPEZZA ")

            If Filtra_Validita_Esercizi Then
                StrSQL.Append(" INNER JOIN " & TabellaPrefisso & "Imprese_Progetti ON " & TabellaPrefisso & "Reg_Impianti.PIVA = " & TabellaPrefisso & "Imprese_Progetti.PIVA AND " & TabellaPrefisso & "Reg_Impianti.SA_COD = " & TabellaPrefisso & "Imprese_Progetti.SA_COD AND ")
                StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti.APPEZZA = " & TabellaPrefisso & "Imprese_Progetti.APPEZZA AND " & TabellaPrefisso & "Reg_Impianti.ID_REG = " & TabellaPrefisso & "Imprese_Progetti.ID_REG")
            End If

            StrSQL.Append("  LEFT OUTER JOIN ")
            StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti_Codici ON " & TabellaPrefisso & "Reg_Impianti.PIVA = " & TabellaPrefisso & "Reg_Impianti_Codici.PIVA AND " & TabellaPrefisso & "Reg_Impianti.SA_COD = " & TabellaPrefisso & "Reg_Impianti_Codici.sa_cod AND  ")
            StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti.APPEZZA = " & TabellaPrefisso & "Reg_Impianti_Codici.appezza AND " & TabellaPrefisso & "Reg_Impianti.ID_REG = " & TabellaPrefisso & "Reg_Impianti_Codici.Id_Reg LEFT OUTER JOIN ")
            StrSQL.Append(" Codici_Anagrafe ON " & TabellaPrefisso & "Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice  ")
            StrSQL.Append(" WHERE " & TabellaPrefisso & "Reg_Impianti.CUL_COD = 0 ")


            StrSQL.Append(" AND NOT EXISTS (SELECT * FROM " & TabellaPrefisso & "Reg_Impianti_Codici RIC WHERE " & TabellaPrefisso & "Reg_Impianti.PIVA = RIC.PIVA ")
            StrSQL.Append(" AND " & TabellaPrefisso & "Reg_Impianti.SA_COD = RIC.sa_cod AND   " & TabellaPrefisso & "Reg_Impianti.APPEZZA = RIC.appezza AND " & TabellaPrefisso & "Reg_Impianti.ID_REG = RIC.Id_Reg ")
            StrSQL.Append(" AND RIC.id_cod >= 3000 AND RIC.id_cod <= 4000 ) ")

            If Piva <> "" Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If


            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable

                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri_Server)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND " & TabellaPrefisso & "Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                    End If
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If
            If FiltroCampi <> "" Then
                StrSQL.Append(" AND    " & FiltroCampi & " ")
            End If

            StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
            StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")

            If Not leggiAncheBloccati Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Appezzamento.Blk_Flag <> -1 ")
            End If

            If Filtra_Validita_Esercizi Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
                StrSQL.Append(" AND     " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
            End If

            If Id_Budget <> 0 Then
                StrSQL.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            StrSQL.Append(" )   ")

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.Append(" ORDER BY VEG_DES")
            End If


            'Dt = objDP.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            Dt = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

            Dim lista As String = ""

            For i = 0 To Dt.Rows.Count - 1
                Select Case Dt.Rows(i).Item("Veg_Cod")
                    Case Is <> 0
                        lista &= ", " & Dt.Rows(i).Item("Veg_Cod")
                    Case 0
                        If Dt.Rows(i).Item("Id_Cod") = 0 Then
                            lista &= ", " & Dt.Rows(i).Item("Veg_Cod")
                        End If
                End Select
            Next

            If xFiltroAggiuntivo = "" Then
                xFiltroAggiuntivo &= " SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
            Else
                xFiltroAggiuntivo &= "and SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
            End If

            SpecieVegetale_Optimize(Controllo, PrimaRiga_Flag, PrimaRiga_Text,
                                         PrimaRiga_Value, Gru_Cod, LetteraIniziale, Flag_FiltroUtente, Veg_Cod,
                                         Veg_Cod_daModificare, GruCod_Rif_VegCod_daModificare, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti)


            Dim objUtentiImpo As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
            Dim DT_GruCod As DataTable = objUtentiImpo.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI,
                                            0, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "", objParametri_Utenti)

            Dim visibilitaSuTerrreniNudi As Boolean = False
            For Each dr As DataRow In DT_GruCod.Rows
                If dr.Item("ID_0") = 0 Then
                    visibilitaSuTerrreniNudi = True
                    Exit For
                End If
            Next

            If visibilitaSuTerrreniNudi OrElse DT_GruCod.Rows.Count = 0 Then
                For Each dr As DataRow In Dt.Rows
                    If dr.Item("id_cod") <> "0" Then
                        Dim x_Cod As String = dr.Item("id_cod")
                        Dim x_Des As String = dr.Item("Veg_DES")
                        Controllo.Items.Add(New ListItem(x_Des, "0/" & x_Cod))
                    End If
                Next
            End If



        Else



            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT    ")
            StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ")
            StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS Veg_Des ")

            StrSQL.Append(" FROM         " & TabellaPrefisso & "Appezzamento INNER JOIN ")
            StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti ON " & TabellaPrefisso & "Appezzamento.PIVA = " & TabellaPrefisso & "Reg_Impianti.PIVA AND " & TabellaPrefisso & "Appezzamento.SA_COD = " & TabellaPrefisso & "Reg_Impianti.SA_COD AND ")
            StrSQL.Append(" " & TabellaPrefisso & "Appezzamento.APPEZZA = " & TabellaPrefisso & "Reg_Impianti.APPEZZA LEFT OUTER JOIN ")
            StrSQL.Append(" SpecieVegetali INNER JOIN ")
            StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON " & TabellaPrefisso & "Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            If Filtra_Validita_Esercizi Then
                StrSQL.Append(" INNER JOIN " & TabellaPrefisso & "Imprese_Progetti ON " & TabellaPrefisso & "Reg_Impianti.PIVA = " & TabellaPrefisso & "Imprese_Progetti.PIVA AND " & TabellaPrefisso & "Reg_Impianti.SA_COD = " & TabellaPrefisso & "Imprese_Progetti.SA_COD AND ")
                StrSQL.Append(" " & TabellaPrefisso & "Reg_Impianti.APPEZZA = " & TabellaPrefisso & "Imprese_Progetti.APPEZZA AND " & TabellaPrefisso & "Reg_Impianti.ID_REG = " & TabellaPrefisso & "Imprese_Progetti.ID_REG")
            End If

            StrSQL.Append(" WHERE   1=1  ")
            If Piva <> "" Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If


            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable

                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri_Server)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND " & TabellaPrefisso & "Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                    End If
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If
            If FiltroCampi <> "" Then
                StrSQL.Append(" AND    " & FiltroCampi & " ")
            End If

            StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
            StrSQL.Append(" AND     " & TabellaPrefisso & "Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")

            If Not leggiAncheBloccati Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Appezzamento.Blk_Flag <> -1 ")
            End If


            If Not ConsideraTerrenoNudo Then
                StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
            End If

            If Filtra_Validita_Esercizi Then
                StrSQL.Append(" AND     " & TabellaPrefisso & "Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
                StrSQL.Append(" AND     " & TabellaPrefisso & "Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Selezionata) & " ")
            End If

            If Id_Budget <> 0 Then
                StrSQL.Append(" And " & TabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.Append(" ORDER BY VEG_DES")
            End If


            'Dt = objDP.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            Dt = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

            Dim lista As String = ""

            For i = 0 To Dt.Rows.Count - 1
                lista &= ", " & Dt.Rows(i).Item("Veg_Cod")
            Next

            If xFiltroAggiuntivo = "" Then
                xFiltroAggiuntivo &= " SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
            Else
                xFiltroAggiuntivo &= "and SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
            End If

            SpecieVegetale_Optimize(Controllo, PrimaRiga_Flag, PrimaRiga_Text,
                                         PrimaRiga_Value, Gru_Cod, LetteraIniziale, Flag_FiltroUtente, Veg_Cod,
                                         Veg_Cod_daModificare, GruCod_Rif_VegCod_daModificare, xFiltroAggiuntivo, xOrderBy, objParametri_Server, objParametri_Utenti)

        End If




    End Sub


    'Public Shared Sub SpecieVegetale_Coltivate_Optimize_Data_Da_A(ByRef Controllo As ListControl, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Integer, _
    '                        ByVal Data_Da As Date, _
    '                        ByVal Data_A As Date, _
    '                        ByVal ConsideraTerrenoNudo As Boolean, _
    '                        ByVal PrimaRiga_Flag As Boolean, _
    '                        ByVal PrimaRiga_Text As String, _
    '                        ByVal PrimaRiga_Value As String, _
    '                        ByVal Gru_Cod As Integer, _
    '                        ByVal LetteraIniziale As String, _
    '                        ByVal Flag_FiltroUtente As Boolean, _
    '                        ByVal Veg_Cod As Integer, _
    '                        ByVal Veg_Cod_daModificare As Integer, _
    '                        ByVal GruCod_Rif_VegCod_daModificare As Integer, _
    '                        ByVal DettagliTerrenoNudo As Boolean, _
    '                        ByVal xFiltroAggiuntivo As String, _
    '                        ByVal xOrderBy As String, _
    '                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                        )

    '    Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Coltivate_Optimize()"
    '    Dim objDP As New AgronicaCoreDataProvider.DataProvider
    '    Dim Dt As DataTable
    '    Dim StrSQL As New System.Text.StringBuilder
    '    'Genero la query SQL


    '    If DettagliTerrenoNudo Then


    '        StrSQL.Length = 0
    '        StrSQL.Append(" (   ")
    '        StrSQL.Append(" SELECT DISTINCT    ")
    '        StrSQL.Append(" SpecieVegetali.Veg_Cod AS Veg_Cod, ")
    '        StrSQL.Append(" SpecieVegetali.Veg_Des AS Veg_Des, ")
    '        StrSQL.Append(" 0 as id_cod  ")

    '        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
    '        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
    '        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN ")
    '        StrSQL.Append(" SpecieVegetali INNER JOIN ")
    '        StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

    '        StrSQL.Append(" WHERE   1=1  ")
    '        If Piva <> "" Then
    '            StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        End If


    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If

    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_A) & " ")
    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Da) & " ")

    '        StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")

    '        If ConsideraTerrenoNudo = False Then
    '            StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
    '        End If

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If


    '        StrSQL.Append(" ) UNION ( ")


    '        StrSQL.Append(" SELECT DISTINCT    ")
    '        StrSQL.Append(" Reg_Impianti.CUL_COD as veg_cod , ISNULL(Codici_Anagrafe.descrizione, '') AS veg_des, ISNULL(Codici_Anagrafe.codice, 0) AS id_cod ")

    '        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
    '        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
    '        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ")

    '        StrSQL.Append("  LEFT OUTER JOIN ")
    '        StrSQL.Append(" Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND  ")
    '        StrSQL.Append(" Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg LEFT OUTER JOIN ")
    '        StrSQL.Append(" Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice AND Reg_Impianti_Codici.id_cod >= 3000 AND Reg_Impianti_Codici.id_cod <= 4000 ")
    '        StrSQL.Append(" WHERE     (Reg_Impianti.CUL_COD = 0) ")

    '        If Piva <> "" Then
    '            StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        End If


    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If

    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_A) & " ")
    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Da) & " ")


    '        StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")


    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        StrSQL.Append(" )   ")

    '        If xOrderBy <> "" Then
    '            strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        Else
    '            StrSQL.Append(" ORDER BY VEG_DES")
    '        End If

    '        Dt = objDP.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

    '        Dim lista As String = ""
    '        Dim i As Integer = 0
    '        For i = 0 To Dt.Rows.Count - 1
    '            lista &= ", " & Dt.Rows(i).Item("Veg_Cod")
    '        Next

    '        If xFiltroAggiuntivo = "" Then
    '            xFiltroAggiuntivo &= " SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
    '        Else
    '            xFiltroAggiuntivo &= "and SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
    '        End If

    '        SpecieVegetale_Optimize_x_PDC(Controllo, PrimaRiga_Flag, PrimaRiga_Text, _
    '                                     PrimaRiga_Value, objParametri_Server)


    '        For i = 0 To Dt.Rows.Count - 1
    '            If Dt.Rows(i).Item("id_cod") <> "0" Then

    '                Dim x_Cod As String = Dt.Rows(i).Item("id_cod")

    '                Dim x_Des As String = Dt.Rows(i).Item("Veg_DES")

    '                Controllo.Items.Add(New ListItem(x_Des, "0/" & x_Cod))

    '            End If
    '        Next


    '    Else



    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT DISTINCT    ")
    '        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ")
    '        StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS Veg_Des ")

    '        StrSQL.Append(" FROM         Appezzamento INNER JOIN ")
    '        StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
    '        StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA LEFT OUTER JOIN ")
    '        StrSQL.Append(" SpecieVegetali INNER JOIN ")
    '        StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

    '        StrSQL.Append(" WHERE   1=1  ")
    '        If Piva <> "" Then
    '            StrSQL.Append(" AND     Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        End If


    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND     Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If
    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_A) & " ")
    '        StrSQL.Append(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Da) & " ")
    '        StrSQL.Append(" AND     Appezzamento.Blk_Flag <> -1 ")

    '        If ConsideraTerrenoNudo = False Then
    '            StrSQL.Append(" AND SpecieVegetali.Veg_Cod <> 0 ")
    '        End If

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If


    '        If xOrderBy <> "" Then
    '            strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        Else
    '            StrSQL.Append(" ORDER BY VEG_DES")
    '        End If


    '        Dt = objDP.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

    '        Dim lista As String = ""
    '        Dim i As Integer = 0
    '        For i = 0 To Dt.Rows.Count - 1
    '            lista &= ", " & Dt.Rows(i).Item("Veg_Cod")
    '        Next

    '        If xFiltroAggiuntivo = "" Then
    '            xFiltroAggiuntivo &= " SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
    '        Else
    '            xFiltroAggiuntivo &= "and SpecieVegetali.Veg_Cod in (-99999 " & lista & ")"
    '        End If

    '        SpecieVegetale_Optimize_x_PDC(Controllo, PrimaRiga_Flag, PrimaRiga_Text, _
    '                                     PrimaRiga_Value, objParametri_Server)





    '    End If




    'End Sub



    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Gen_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	04/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Lista_Specie_Animali(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Gen_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim objAnimali As New AgronicaCoreZooDAL.Lista_Specie_Animali_R
        Dim DT As DataTable

        'Leggo le imprese associate al profilo selezionato			
        DT = objAnimali.Leggi(CInt(Gen_Cod),
                          CInt(0),
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            "",
                            objParametri)

        objAnimali = Nothing

        '----- Riempio la combo con i dati del recordset

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Se il recordset non è chiuso allora ...	
        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

            'Inserisco una riga vuota
            'Cmb.Items.Add(New ListItem("", "0"))

            'Inserisco i record trovati
            Dim i As Integer

            For i = 0 To DT.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Spe_DES"),
                           DT.Rows(i).Item("Gen_Cod") & "|" &
                           DT.Rows(i).Item("Spe_Cod")))

            Next

        End If

    End Sub



    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Richiesto_UdmCod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	04/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub IndiciMaturita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Veg_Cod As Integer,
                                ByVal Richiesto_UdmCod As Boolean,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )



        Dim objIndici As New AgronicaCoreMetaSchemaDAL.IndiciMaturita_R
        Dim ObjUdm As AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_R = Nothing

        Dim Dt As DataTable
        Dim DtUdm As DataTable

        Dim Udm_Cod As Integer
        Dim Udm_Sim As String

        Dim i As Integer

        If Richiesto_UdmCod Then
            ObjUdm = New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_R
        End If

        'Leggo le imprese associate al profilo selezionato
        Dt = objIndici.Leggi(CInt(Veg_Cod),
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "", "", objParametri)

        objIndici = Nothing

        '----- Riempio la combo con i dati del recordset

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                If Richiesto_UdmCod Then

                    DtUdm = ObjUdm.Leggi(CInt(Dt.Rows(i).Item("Ind_Mat_Cod")),
                                          0,
                                         enumSelezioneVariabile.Selezione_JoinCompleta,
                                         "", "", objParametri)

                    If DtUdm IsNot Nothing AndAlso DtUdm.Rows.Count > 0 Then
                        Udm_Cod = DtUdm.Rows(0).Item("Udm_Cod")
                        Udm_Sim = " (" & DtUdm.Rows(0).Item("Udm_Sim") & ")"
                    Else
                        Udm_Cod = 0
                        Udm_Sim = ""
                    End If

                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Ind_Mat_Des") & Udm_Sim,
                                               Dt.Rows(i).Item("Ind_Mat_Cod") & "|" & Udm_Cod.ToString))

                Else

                    Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Ind_Mat_Des"),
                                               Dt.Rows(i).Item("Ind_Mat_Cod")))

                End If

            Next

        End If

        Dt = Nothing

        If Richiesto_UdmCod Then
            ObjUdm = Nothing
            DtUdm = Nothing
        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Filtro_SaCod"></param>
    ''' <param name="Cod_Rapporto"></param>
    ''' <param name="Cliente"></param>
    ''' <param name="Fornitore"></param>
    ''' <param name="Dipendente"></param>
    ''' <param name="Terzista"></param>
    ''' <param name="Legale"></param>
    ''' <param name="Agente"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	07/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub RapportiContabili(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Filtro_SaCod As Boolean,
                                ByVal Cod_Rapporto As Integer,
                                ByVal Cliente As Boolean,
                                ByVal Fornitore As Boolean,
                                ByVal Dipendente As Boolean,
                                ByVal Terzista As Boolean,
                                ByVal Legale As Boolean,
                                ByVal Agente As Boolean,
                                ByVal Consulente As Boolean,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim i As Integer
        Dim DT_RappCont As DataTable
        Dim FiltroFinale As String


        'se voglio applicare solo il filtro sa_cod
        If Filtro_SaCod Then
            'FiltroFinale = "AND (Sa_Cod = 0 OR Sa_Cod = " & CStr(Sa_Cod) & ")"
            'dopo l'introduzione dei core non ci vuole più l'and
            FiltroFinale = " (Sa_Cod = 0 OR Sa_Cod = " & CStr(Sa_Cod) & ")"
        Else
            'altrimenti applico l'eventuale filtro passato
            FiltroFinale = xFiltroAggiuntivo
        End If

        Dim objRappContabiliR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

        DT_RappCont = objRappContabiliR.Contatti_RapportiContabili_Leggi(
                                                                SACOD_CONTATTO_NONDEFINITO,
                                                                Cod_Rapporto,
                                                                Cliente,
                                                                Fornitore,
                                                                Dipendente,
                                                                Terzista,
                                                                Legale,
                                                                Agente,
                                                                Consulente,
                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                FiltroFinale,
                                                                "",
                                                                objParametri)


        Controllo.Items.Clear()

        If DT_RappCont.Rows.Count <> 0 Then

            If PrimaRiga_Flag Then
                Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
            End If

            For i = 0 To DT_RappCont.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT_RappCont.Rows(i).Item("Rapporto_Des"),
                                        DT_RappCont.Rows(i).Item("Cod_Rapporto")))


            Next

        End If

    End Sub


    '######################################################################
    'Le seguenti variabili hanno senso per il caricamento delle combo
    'per scegliere se aggiungere una riga vuota.
    'Se non servono, passare come default:
    'PrimaRiga_Flag As Boolean = false
    'PrimaRiga_Text As String = ""
    'PrimaRiga_Value As String = ""
    Public Shared Sub Campi(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal NomeCentro As Boolean,
                                ByVal Piva As String,
                                ByVal SaCod As Integer,
                                ByVal TipoCampo As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        'Tipo Campo:
        '0 = tutti
        '1 = solo campi non squadro
        '2 = solo campi squadro

        Dim Testo As String

        Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
        Dim TempDt As DataTable

        Dim objCampi2 As New AgronicaCoreAnagrafeDAL.CampixParticelle_R

        Dim i As Integer

        'Leggo i campi associati al centro selezionato			
        TempDt = objCampi.Leggi(CStr(Piva),
                                CInt(SaCod),
                                0,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "", "", objParametri)

        objCampi = Nothing

        Dim uguale As Integer = 0
        For i = 0 To TempDt.Rows.Count - 1
            If i > 0 Then
                If uguale <> CInt(TempDt.Rows(i).Item("Sa_Cod")) Then
                    uguale = -1
                    Exit For
                End If
            End If
            uguale = CInt(TempDt.Rows(i).Item("Sa_Cod"))
        Next



        '----- Riempio la combo con i dati del recordset

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Se il recordset non è chiuso allora ...	
        If TempDt IsNot Nothing AndAlso TempDt.Rows.Count > 0 Then

            'If PrimaRiga_Flag = False Then
            '    Controllo.Items.Add(New ListItem("", "xxxxxxxxxxx/-1/-1"))
            'End If

            Dim AggiungiCampo As Boolean = False

            'Inserisco i record trovati
            For i = 0 To TempDt.Rows.Count - 1

                AggiungiCampo = False

                Select Case TipoCampo

                    Case 0

                        AggiungiCampo = True

                    Case 1  'solo campi non squadro

                        If objCampi2.Campo_Definito_Come_Squadro(TempDt.Rows(i).Item("Piva"), TempDt.Rows(i).Item("Sa_Cod"), TempDt.Rows(i).Item("Campo_Cod"), objParametri) = False Then

                            AggiungiCampo = True

                        End If

                    Case 2  'solo campi squadro

                        If objCampi2.Campo_Definito_Come_Squadro(TempDt.Rows(i).Item("Piva"), TempDt.Rows(i).Item("Sa_Cod"), TempDt.Rows(i).Item("Campo_Cod"), objParametri) Then

                            AggiungiCampo = True

                        End If

                End Select


                If AggiungiCampo Then

                    'NOTA
                    'VALUE costituito da Piva/Sa_Cod/Campo_Cod

                    Dim pivac As String = TempDt.Rows(i).Item("Piva")
                    Dim sa_codc As Integer = TempDt.Rows(i).Item("Sa_Cod")
                    Dim campocodc As Integer = TempDt.Rows(i).Item("Campo_Cod")

                    Testo = pivac &
                            "/" &
                            sa_codc &
                            "/" &
                            campocodc

                    Dim descrizione As String = TempDt.Rows(i).Item("Campo_Des")
                    If uguale = -1 Then
                        Dim Sa_Nome As String = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(pivac, sa_codc, objParametri)
                        If Sa_Nome.Length > 5 Then
                            Sa_Nome = Sa_Nome.Substring(0, 4) & "."
                        End If
                        descrizione = descrizione & " (" & Sa_Nome & ")"
                    End If
                    Controllo.Items.Add(New ListItem(
                                        descrizione,
                                        Testo))

                End If

            Next

        End If

        'Elimino il recordset
        TempDt = Nothing

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Grfi_Cod"></param>
    ''' <param name="Cerca_GrfiDes"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	08/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Finalita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Veg_Cod As Integer,
                                ByVal Grfi_Cod As Integer,
                                ByVal Cerca_GrfiDes As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

        Dt = objGruppoFinalita.Leggi(Grfi_Cod,
                                    Veg_Cod,
                                    Cerca_GrfiDes,
                                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                     xFiltroAggiuntivo,
                                     xOrderBy,
                                     objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Grfi_COD")

                x_Des = CStr(Dt.Rows(i).Item("Grfi_DES"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	01/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub GruppoFinalita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.GruppoFinalita()"

        Dim i As Integer

        Dim objG As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
        Dim dt As DataTable
        Dim filtro As String = " Grfi_Cod < 100 "
        Dim orderby As String = " Grfi_DES "

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        dt = objG.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, orderby, objParametri)

        For i = 0 To dt.Rows.Count - 1
            Controllo.Items.Add(New ListItem(dt.Rows(i).Item("Grfi_DES"),
                                       dt.Rows(i).Item("Grfi_COD")))

        Next

    End Sub

    Public Shared Sub Copertura(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal Gru_Cod As Integer,
                            ByVal Cop_Cod As Integer,
                            ByVal Cerca_CopDes As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objCop As New AgronicaCoreMetaSchemaDAL.Copertura_R

        Dt = objCop.Leggi(Gru_Cod,
                          Cop_Cod,
                          Cerca_CopDes,
                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Cop_Cod")

                x_Des = CStr(Dt.Rows(i).Item("Cop_DES"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	19/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub UnitaMisuraAgenda(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim elementiDaCaricare As String = ""
        Select Case Elem_Cod

            Case FERTILIZZANTI

                elementiDaCaricare = "2,3,4,29,101,104,304"

                'Controllo.Items.Add(New ListItem("grammi", "3"))
                'Controllo.Items.Add(New ListItem("chilogrammi", "2"))
                'Controllo.Items.Add(New ListItem("quintali", "4"))
                'Controllo.Items.Add(New ListItem("tonnellate", "304"))
                'Controllo.Items.Add(New ListItem("millilitri", "101"))
                'Controllo.Items.Add(New ListItem("litri", "29"))
                'Controllo.Items.Add(New ListItem("centimetri cubi", "104"))


            Case FORMULATI

                elementiDaCaricare = "2,3,29,101,104"

                Controllo.Items.Add(New ListItem("", 0))
                'Controllo.Items.Add(New ListItem("chilogrammi", 2))
                'Controllo.Items.Add(New ListItem("grammi", 3))
                'Controllo.Items.Add(New ListItem("litri", 29))
                'Controllo.Items.Add(New ListItem("centimetri cubi", 104))
                'Controllo.Items.Add(New ListItem("millilitri", 101))

            Case TRAPPOLE

        End Select

        Dim leggiUDM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim dtUDM As DataTable =
        leggiUDM.Leggi(
            0,
            0,
            "",
            "",
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            " UDM_COD in (" & elementiDaCaricare & ")",
            "",
            objParametri
        )

        For Each Riga As DataRow In dtUDM.Rows
            Controllo.Items.Add(New ListItem(Riga("udm_sim"), Riga("udm_cod")))

        Next



    End Sub


    ''' -----------------------------------------------------------------------------
    Public Shared Sub UnitaMisuraAgendaMagazzino(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Cau_Mov As String,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Fabbricato_Cod As Integer,
                                                ByVal Flag_ProCod_Negativo As Boolean,
                                                ByVal Pro_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametriServer As AgronicaCoreParametri,
                                                ByRef objParametriUtenti As AgronicaCoreParametri)

        Dim Dt As DataTable = Nothing

        Dim x_ProCod As Integer
        Dim x_MatCod As Integer

        Dim x_Cod As Integer
        Dim x_Des As String

        Dim i As Integer

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Select Case Cau_Mov

            Case CAU_CARICO

                Dim objCM As New AgronicaCoreMetaSchemaDAL.CategorieXUnitaMisura_R

                Dt = objCM.Leggi(Elem_Cod,
                                 0,
                                 Cau_Mov,
                                 False,
                                 enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                 "",
                                 "",
                                 objParametriServer)

                objCM = Nothing

            Case CAU_SCARICO

                Select Case Flag_ProCod_Negativo
                    Case True
                        If Pro_Cod < 0 Then
                            x_MatCod = -Pro_Cod
                            x_ProCod = 0
                        Else
                            x_ProCod = Pro_Cod
                            x_MatCod = 0
                        End If
                    Case False
                        x_ProCod = Pro_Cod
                        x_MatCod = Mat_Cod
                End Select

                'legge record statico
                'Dim objGia As New AgronicaCoreContabDAL.Giacenze_R
                'Dt = objGia.Giacenze_Leggi(Piva, _
                '                         Sa_Cod, _
                '                         Fabbricato_Cod, _
                '                          0, 0, _
                '                         Elem_Cod, _
                '                         Pro_Cod, _
                '                         Mat_Cod, _
                '                         0, 0, 0, 0, LOTTO_NONDEFINITO, 0, 0, 0, _
                '                         AGRODATAFINE, _
                '                         xFiltroAggiuntivo, _
                '                         xOrderBy, _
                '                         objParametriServer)

                Dt = New AgronicaCoreContabDAL.Giacenze_R().SchedaGiacenzeMagazzino(
                                     AGRODATAFINE,
                                     Piva,
                                     Sa_Cod,
                                     Fabbricato_Cod,
                                      Elem_Cod,
                                      Pro_Cod,
                                      Mat_Cod,
                                       0, 0, 0, 0,
                                      LOTTO_NONDEFINITO, False,
                                      xFiltroAggiuntivo, "", "", "", "", "", "", "", "", "", "", xOrderBy,
                                     objParametriServer, objParametriUtenti)


        End Select


        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Udm_Cod")
                x_Des = CStr(Dt.Rows(i).Item("Udm_Des"))

                If Not IsDBNull(x_Cod) AndAlso
                   IsNothing(Controllo.Items.FindByValue(x_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                End If

            Next

        End If









    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks></remarks>
    Public Shared Sub UnitaMisuraPrincipiAttivi(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      )


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("mg/kg", "2151"))
        Controllo.Items.Add(New ListItem("g/kg", "2011"))
        Controllo.Items.Add(New ListItem("µg/Kg", "5001034"))


    End Sub

    Public Shared Sub UnitaMisuraPrincipiAttivi_x_Griglia_varie(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      )


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If
        Controllo.Items.Add(New ListItem("mg/kg", "2151"))
        Controllo.Items.Add(New ListItem("g/kg", "2011"))
        Controllo.Items.Add(New ListItem("%", "1"))
        Controllo.Items.Add(New ListItem("µg/Kg", "5001034"))

        ' aggiunti per analisi del terreno 
        Controllo.Items.Add(New ListItem("permille", "32"))
        Controllo.Items.Add(New ListItem("ppm", "33"))
        Controllo.Items.Add(New ListItem("meq/hg", "34"))
        Controllo.Items.Add(New ListItem("mS/cm", "35"))
        Controllo.Items.Add(New ListItem("n", "38"))
        Controllo.Items.Add(New ListItem("g/100g", "5001035"))

        'analisi varie, proposta Fruttagel
        Controllo.Items.Add(New ListItem("g/l", "2003"))

        'analisi del vino (per GIV)
        Controllo.Items.Add(New ListItem("mg/l", "5001006"))
        Controllo.Items.Add(New ListItem("% in peso", "30"))
        Controllo.Items.Add(New ListItem("% in volume", "31"))
        Controllo.Items.Add(New ListItem("°C", "15"))
        Controllo.Items.Add(New ListItem("°Babo", "2103"))
        Controllo.Items.Add(New ListItem("°Brix", "182"))
        Controllo.Items.Add(New ListItem("meq/kg", "5001041"))
        Controllo.Items.Add(New ListItem("ml/100ml di A.A.", "5001007"))
        Controllo.Items.Add(New ListItem("PH", "130"))

        'analisi merceologiche
        Controllo.Items.Add(New ListItem("g", "3"))
        Controllo.Items.Add(New ListItem("kgf", "5001050"))
        Controllo.Items.Add(New ListItem("hue", "5001051"))

        Controllo.Items.Add(New ListItem("non definito", "0"))

    End Sub
    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="NotaGruppo_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Note_Intervento(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal NotaGruppo_Cod As Integer,
                                ByVal NotaUtilizzo_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   Optional ByVal meno1tutti_0nonVisibili_1soloVisibili As Integer = -1,
                                        Optional ByRef GruppiDes As String = ""
                               )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim ObjContab As New AgronicaCoreContabDAL.Note_Intervento_R

        Dt = ObjContab.Leggi_con_Utilizzo(0,
                             NotaGruppo_Cod,
                             NotaUtilizzo_Cod,
                             enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                             xFiltroAggiuntivo,
                             xOrderBy,
                             objParametri,
                             meno1tutti_0nonVisibili_1soloVisibili)

        ObjContab = Nothing

        'Inserisco i record trovati
        For i = 0 To Dt.Rows.Count - 1
            Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Nota_Des"),
                                       Dt.Rows(i).Item("Nota_Cod")))
            If InStr(GruppiDes, Dt.Rows(i).Item("NotaGruppo_Des")) = 0 Then
                GruppiDes &= Dt.Rows(i).Item("NotaGruppo_Des") & ","
            End If
        Next

        If GruppiDes <> "" Then
            GruppiDes = Left(GruppiDes, GruppiDes.Length - 1)
        End If

        Dt.Dispose()
        Dt = Nothing

    End Sub


    '######################################################################
    Public Shared Sub TipoAgricoltura(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Agricoltura Integrata", "1"))
        Controllo.Items.Add(New ListItem("Agricoltura in Conversione al Biologico", "2"))
        Controllo.Items.Add(New ListItem("Agricoltura Biologica", "3"))

    End Sub


    Public Shared Function TipoMacchineCompresso(
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String



        Dim coreM As New AgronicaCoreMetaSchemaDAL.Macchine_R

        Dim dtMac As DataTable = coreM.LeggiTipoMacchineCompresso("", "", "", objParametri)

        Dim rval As New List(Of String)


        'Se il recordset non è chiuso allora ...	
        If dtMac.Rows.Count > 0 Then


            'Inserisco i record trovati
            For Each dr As DataRow In dtMac.Rows

                rval.Add("{ ""class_code"": """ & dr("macchinaDettaglio2TipoCod") & """, ""class_desc"":""" & jSon.Escape(dr("macchinaDettaglio2TipoDes")) & """, ""class_tipoPiuDettaglio1_Desc"": """ & jSon.Escape(dr("macchinaTipoDes") & "-" & dr("macchinaDettaglio1TipoDes")) & """ } ")

            Next

        End If

        Return "[" & String.Join(",", rval) & "]"

    End Function


    '######################################################################
    Public Shared Sub TipoMacchine(ByRef Controllo As ListControl,
                                ByVal Livello As Integer,
                                ByVal StrLivello1 As String,
                                ByVal StrLivello2 As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional primaRigaVuota As Boolean = True
                                )


        Controllo.Items.Clear()

        Dim coreM As New AgronicaCoreMetaSchemaDAL.Macchine_R
        'le leggo tutte..
        Dim dtMac As DataTable = coreM.Leggi("", objParametri)

        '----- Riempio la combo con i dati del recordset

        'Pulisco la combo
        '  Cmb.Items.Clear()


        'Se il recordset non è chiuso allora ...	
        If dtMac.Rows.Count > 0 Then

            If primaRigaVuota Then
                'Inserisco una riga vuota
                Controllo.Items.Add(New ListItem("", ""))
            End If

            'Inserisco i record trovati
            For Each dr As DataRow In dtMac.Rows
                Select Case Livello
                    Case 1
                        If dr("CLASS_CODE").ToString().Length = 2 Then
                            Controllo.Items.Add(New ListItem(dr("CLASS_DESC").ToString,
                                                   dr("CLASS_CODE").ToString))
                        End If
                    Case 2
                        Dim Vettore As String()
                        Vettore = Split(dr("CLASS_CODE").ToString, ".")
                        If dr("CLASS_CODE").ToString.Length = 5 AndAlso Vettore(0) = StrLivello1 Then
                            Controllo.Items.Add(New ListItem(dr("CLASS_DESC").ToString, Vettore(1)))
                        End If
                    Case 3
                        Dim Vettore As String()
                        Vettore = Split(dr("CLASS_CODE").ToString(), ".")
                        If dr("CLASS_CODE").ToString.Length = 8 Then
                            If Vettore(0) = StrLivello1 AndAlso Vettore(1) = StrLivello2 Then
                                Controllo.Items.Add(New ListItem(dr("CLASS_DESC").ToString, Vettore(2)))

                            End If
                        End If

                End Select

            Next

        End If

        'abilito la combo e la disabilito in seguito solo se è vuota
        Controllo.Enabled = True
        'Se la combo è vuota la disabilito
        If Controllo.Items.Count = 1 Then
            Controllo.Enabled = False
        End If

    End Sub




    '######################################################################
    Public Shared Sub MarcheMacchine(ByRef Controllo As ListControl,
                                     ByRef objParametri As AgronicaCoreParametri)
        Controllo.Items.Clear()
        Dim core As New AgronicaCoreMetaSchemaDAL.Ditte_R
        Dim dtRes As DataTable = core.Leggi(0, "M", "", "", objParametri)
        Controllo.DataSource = dtRes
        Controllo.DataTextField = "Ditta_Des"
        Controllo.DataValueField = "Ditta_Cod"
        Controllo.DataBind()
        'aggiungo una riga vuota in testa
        Controllo.Items.Insert(0, New ListItem("", "0"))
    End Sub


    '######################################################################
    Public Shared Sub Stalle_UDM(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String
                                )
        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Numero", "1"))
        Controllo.Items.Add(New ListItem("Litri", "2"))
        Controllo.Items.Add(New ListItem("Quintali", "3"))
        Controllo.Items.Add(New ListItem("Tonnellate", "4"))

    End Sub


    '######################################################################
    Public Shared Sub Lista_Categorie_Animali(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Gen_Cod As Integer,
                                ByVal Spe_Cod As Integer,
                                ByVal IPro_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objCatAnimali As New AgronicaCoreZooDAL.Lista_Categorie_Animali_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        Dt = objCatAnimali.Leggi(CInt(Gen_Cod), CInt(Spe_Cod), CInt(IPro_Cod), CInt(-1),
                                 0, "", "",
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "", "", objParametri)

        objCatAnimali = Nothing

        '----- Riempio la combo con i dati del recordset

        'Se il recordset non è chiuso allora ...	
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Cat_Des"),
                                           Dt.Rows(i).Item("Gen_Cod") & "|" &
                                           Dt.Rows(i).Item("Spe_Cod") & "|" &
                                           Dt.Rows(i).Item("Ipro_Cod") & "|" &
                                           Dt.Rows(i).Item("Cat_Cod")))

            Next

        End If

        'Elimino il recordset
        Dt = Nothing


    End Sub


    '######################################################################
    Public Shared Sub Stalle_TipoFabbricato(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal SubChiave As String,
                                            ByVal Livello As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri)

        Dim objListaTipiFab As New AgronicaCoreZooDAL.Lista_tipi_fabbricati_R
        Dim Dt As DataTable
        Dim i As Integer

        'Leggo le imprese associate al profilo selezionato			
        Dt = objListaTipiFab.Leggi("",
                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                   "", "", objParametri)

        objListaTipiFab = Nothing

        '----- Numero di caratteri a seconda del livello richiesto

        'Voglio solo gli elementi del livello selezionato
        Dim Nchar As Integer

        Nchar = Livello + Livello - 1

        Dim CharSubChiave As Integer = Len(SubChiave)

        '----- Riempio la combo con i dati del recordset

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Se il recordset non è chiuso allora ...	
        If Dt IsNot Nothing AndAlso Dt.Rows.Count - 1 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                If Len(Dt.Rows(i).Item("Cod_Fabb")) = Nchar Then

                    If SubChiave = "" Then

                        Controllo.Items.Add(New ListItem(
                                            Dt.Rows(i).Item("Descr"),
                                            Dt.Rows(i).Item("Cod_Fabb")))

                    Else

                        If Left(Dt.Rows(i).Item("Cod_Fabb"), CharSubChiave) = SubChiave Then

                            Controllo.Items.Add(New ListItem(
                                            Dt.Rows(i).Item("Descr"),
                                            Dt.Rows(i).Item("Cod_Fabb")))

                        End If

                    End If

                End If

            Next

        End If

        Dt = Nothing

    End Sub


    '######################################################################
    Public Shared Sub Stalle_Attributi(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal CodFabb As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Testo As String

        Dim objAttrib As New AgronicaCoreZooDAL.Lista_attrib_stalla_R
        Dim Dt As DataTable
        Dim i As Integer

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        Dt = objAttrib.Leggi(CStr(CodFabb), 0,
                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                              "", "", objParametri)

        objAttrib = Nothing

        '----- Riempio la combo con i dati del recordset

        'Se il datatable non è chiuso allora ...	
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                'NOTA
                'VALUE costituito da CodFabb:AttCod:UdMCod

                Testo = Dt.Rows(i).Item("COD_FABB") &
                        "|" &
                        Dt.Rows(i).Item("Att_Cod") &
                        "|" &
                        Dt.Rows(i).Item("Udm_Cod")

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Att_Des"),
                                           Testo))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub


    '######################################################################
    Public Shared Sub Stati(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim Dt As DataTable
        Dim i As Integer

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        Dt = objIstat.Leggi("E00", "", "", "", "",
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "", "", objParametri)
        objIstat = Nothing

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Localita"),
                                           Dt.Rows(i).Item("Com")))

            Next

        End If

    End Sub

    '######################################################################

    Public Sub RisorseUmanePerConferimenti(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Piva As String,
                                        ByVal Cod_Contatto As String,
                                        ByVal Cod_RisUm As Integer,
                                        ByVal CodRapporto() As Integer,
                                        ByVal RicercaRagSoc As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )


        Dim Dt_Contatti As New DataTable
        Dim Dt_ContattiW As New DataTable
        Dim i As Integer
        Dim Rag_Soc As String
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If RicercaRagSoc <> "" Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo &= " AND "
            End If
            xFiltroAggiuntivo &= " Contatti.Rag_Soc LIKE '%" & Agro_SQL_SaveText(RicercaRagSoc) & "%'"
        End If

        For Each codiceRapporto In CodRapporto

            Dt_ContattiW = objContatti.Contatti_Contatto_Leggi(Piva,
                                                            Cod_Contatto,
                                                            Cod_RisUm,
                                                            codiceRapporto,
                                                            False,
                                                            False,
                                                            0,
                                                            0,
                                                            False,
                                                            0,
                                                            ID_CF_NOFILTRO,
                                                            0,
                                                            "",
                                                            False,
                                                            0, 0, 0, 0, 0,
                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            xFiltroAggiuntivo, xOrderBy, objParametri)

            Dt_Contatti.Merge(Dt_ContattiW)

        Next

        Dim dv As New DataView(Dt_Contatti)
        dv.Sort = "Rag_Soc"
        For i = 0 To dv.Table.Rows.Count - 1

            Rag_Soc = CStr(Dt_Contatti.Rows(i).Item("Rag_Soc"))

            Dim li As New ListItem(Rag_Soc, Dt_Contatti.Rows(i).Item("Cod_Contatto"))
            If Not Controllo.Items.Contains(li) Then
                Controllo.Items.Add(li)
            End If

        Next

    End Sub

    '######################################################################
    'come Contatti_2, ma ha il Cod_RisUm come value
    'Tipo_Text: 0 mette rag_soc + rapporto_des / 1 mette solo rag_soc
    Public Shared Sub Contatti(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal Cod_Contatto As String,
                                ByVal Cod_RisUm As Integer,
                                ByVal Cod_Rapporto As Integer,
                                ByVal FlagPubblico As Boolean,
                                ByVal FlagIndirizzi As Boolean,
                                ByVal TipoIndirizzo As Integer,
                                ByVal CodIndirizzo As Integer,
                                ByVal FlagRubrica As Boolean,
                                ByVal CodRubrica As Integer,
                                ByVal ID_CF As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional ByVal Tipo_Text As Integer = 0
                                )


        Dim Dt_Contatti As DataTable
        Dim i As Integer
        Dim Rag_Soc As String
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt_Contatti = objContatti.Contatti_Contatto_Leggi(Piva,
                                                        Cod_Contatto,
                                                        Cod_RisUm,
                                                        Cod_Rapporto,
                                                        FlagPubblico,
                                                        FlagIndirizzi,
                                                        TipoIndirizzo,
                                                        CodIndirizzo,
                                                        FlagRubrica,
                                                        CodRubrica,
                                                        ID_CF,
                                                        0,
                                                        "",
                                                        False,
                                                        0, 0, 0, 0, 0,
                                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        xFiltroAggiuntivo, xOrderBy, objParametri)

        For i = 0 To Dt_Contatti.Rows.Count - 1

            Rag_Soc = CStr(Dt_Contatti.Rows(i).Item("Rag_Soc")) &
                   CStr(Dt_Contatti.Rows(i).Item("Cognome")) & " " &
                       CStr(Dt_Contatti.Rows(i).Item("Nome"))

            Select Case Tipo_Text
                Case 0
                    Rag_Soc &= " (" & Dt_Contatti.Rows(i).Item("Rapporto_Des") & ")"
                Case 1
                    'non aggiungo  Rapporto_Des
                Case Else
                    Rag_Soc &= " (" & Dt_Contatti.Rows(i).Item("Rapporto_Des") & ")"
            End Select


            Controllo.Items.Add(New ListItem(Rag_Soc,
                                        Dt_Contatti.Rows(i).Item("Cod_RisUm")))


        Next

    End Sub


    '######################################################################
    'come Contatti, ma ha il Cod_Contatto come value
    Public Shared Sub Contatti_2(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal Cod_Contatto As String,
                                ByVal Cod_RisUm As Integer,
                                ByVal Cod_Rapporto As Integer,
                                ByVal FlagPubblico As Boolean,
                                ByVal FlagIndirizzi As Boolean,
                                ByVal TipoIndirizzo As Integer,
                                ByVal CodIndirizzo As Integer,
                                ByVal FlagRubrica As Boolean,
                                ByVal CodRubrica As Integer,
                                ByVal ID_CF As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim Dt_Contatti As DataTable
        Dim i As Integer
        Dim Rag_Soc As String
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt_Contatti = objContatti.Contatti_Contatto_Leggi(Piva,
                                                        Cod_Contatto,
                                                        Cod_RisUm,
                                                        Cod_Rapporto,
                                                        FlagPubblico,
                                                        FlagIndirizzi,
                                                        TipoIndirizzo,
                                                        CodIndirizzo,
                                                        FlagRubrica,
                                                        CodRubrica,
                                                        ID_CF,
                                                        0,
                                                        "",
                                                        False,
                                                        0, 0, 0, 0, 0,
                                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        xFiltroAggiuntivo, xOrderBy, objParametri)

        For i = 0 To Dt_Contatti.Rows.Count - 1

            Rag_Soc = CStr(Dt_Contatti.Rows(i).Item("Rag_Soc")) &
                   CStr(Dt_Contatti.Rows(i).Item("Cognome")) & " " &
                       CStr(Dt_Contatti.Rows(i).Item("Nome"))

            Controllo.Items.Add(New ListItem(Rag_Soc.Replace("""", "'") & " (" & Dt_Contatti.Rows(i).Item("Rapporto_Des") & ")",
                                        Dt_Contatti.Rows(i).Item("Cod_Contatto")))


        Next

    End Sub

    '######################################################################
    'ha il Cod_Contatto come value e i contatti sono in distinct (non ripetuti x risorse umana)
    Public Shared Sub Contatti_4(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal Cod_Contatto As String,
                                ByVal Cod_RisUm As Integer,
                                ByVal Progressivo As String,
                                ByVal Cod_Rapporto As Integer,
                                ByVal FlagPubblico As Boolean,
                                ByVal ID_CF As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim Dt_Contatti As DataTable
        Dim i As Integer
        Dim Rag_Soc As String
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt_Contatti = objContatti.Leggi_Distinct_Contatti(Piva,
                                                        Cod_Contatto,
                                                        Cod_RisUm,
                                                        Cod_Rapporto,
                                                        FlagPubblico,
                                                        ID_CF,
                                                        0,
                                                        "",
                                                        False,
                                                        Progressivo,
                                                        0, 0, 0, 0, 0, 0, 0,
                                                        xFiltroAggiuntivo, xOrderBy,
                                                        objParametri)

        For i = 0 To Dt_Contatti.Rows.Count - 1

            Rag_Soc = CStr(Dt_Contatti.Rows(i).Item("Rag_Soc")) &
                   CStr(Dt_Contatti.Rows(i).Item("Cognome")) & " " &
                       CStr(Dt_Contatti.Rows(i).Item("Nome"))

            Controllo.Items.Add(New ListItem(Rag_Soc,
                                        Dt_Contatti.Rows(i).Item("Cod_Contatto")))


        Next

    End Sub


    '##########################################################################################
    'carica i contatti, non con filtro sul cod_rapporto, ma come filtro sulla configurazione del rapporto contabile
    Public Sub Combo_RapportiContabili_Evento(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef Controllo As ListControl,
                                                    ByVal Piva As String,
                                                    ByVal Cod_Rapporto As Integer,
                                                    ByVal FiltroAgg As String)

        Select Case Cod_Rapporto

            Case 0 'tutti

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 0, 0, 0, , , )

                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_CLIENTE

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 1, 0, 0, 0, 0, , , )
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                1, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)

            Case COD_FORNITORE

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 1, 0, 0, 0, , , )

                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 2, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_CLIENTE_FORNITORE

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Fornitore = 1)", , )

                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= " (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Fornitore = 1)"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)



            Case COD_DIPENDENTE

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 1, 0, 0, , , )
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 1, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_TERZISTA

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 0, 1, 0, , , )
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 1, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_DIPENDENTE_TERZISTA

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Dipendente = 1 OR Rapporti_Contabili.Terzista = 1)", , )

                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Dipendente = 1 OR Rapporti_Contabili.Terzista = 1)"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_TECNICO

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cod_Rapporto = -6)", , )

                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = -6)"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_CENTRO_MACCHINE

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cod_Rapporto = -7 )", , )

                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = -7 )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_LAB_ANALISI

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cod_Rapporto = -8 )", , )


                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = -8 )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_LEGALE

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 0, 0, 1, , , )
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 1, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)

            Case COD_CLIENTEFORNITORE 'rapporto contabile usato solo da fruttagel
                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = " & CStr(COD_CLIENTEFORNITORE) & " )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)

            Case COD_SOCIO
                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = " & CStr(COD_SOCIO) & " )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_TRASPORTATORE
                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = " & CStr(COD_TRASPORTATORE) & " )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)

            Case COD_TECNICORESPONSABILE
                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = " & CStr(COD_TECNICORESPONSABILE) & " )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)

            Case COD_AGENTE
                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Agente = 1 )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_REFERENTEAZIENDALE
                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = " & CStr(COD_REFERENTEAZIENDALE) & " )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case COD_FORNITORE_ORTOFRUTTA
                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = " & CStr(COD_FORNITORE_ORTOFRUTTA) & " )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


            Case Is > 0

                'CaricaCombo_Contatti_SPOSTATA(objServer, objSession, objPage, _
                '                  Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")), 0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cod_Rapporto = " & SQL_SaveNum(Cod_Rapporto) & " )", , )

                If FiltroAgg <> "" Then
                    FiltroAgg &= " AND "
                End If
                FiltroAgg &= "  (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " )"
                AgronicaCoreUtility.CaricaListControl.Contatti_3(
                                Controllo,
                                True, "", "",
                                Piva,
                                0, 0, 0, 0, 0, 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                FiltroAgg, "",
                                objParametri_Server)


        End Select


    End Sub




    '######################################################################
    'era la CaricaCombo_Contatti del modulo CaricaCombo delle stampe
    'value: Cod_RisUm|Piva
    Public Shared Sub Contatti_3(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal Cliente As Integer,
                                ByVal Fornitore As Integer,
                                ByVal Dipendente As Integer,
                                ByVal Terzista As Integer,
                                ByVal Legale As Integer,
                                ByVal Agente As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim Dt_Contatti As DataTable
        Dim i As Integer
        Dim Cod, Text, Rag_Soc As String
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        'Pulisco la combo
        Controllo.Items.Clear()

        'il carica combo faceva:
        'Cmb.Items.Add(New ListItem("", ""))

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        objParametri.ImpostaFinestre_con_SalvataggioTemporale(Validita_Inizio, Validita_Fine)

        If Agente <> 0 Then
            If Trim(xFiltroAggiuntivo) = "" Then
                xFiltroAggiuntivo &= " Rapporti_Contabili.Agente = 1 "
            Else
                xFiltroAggiuntivo &= " AND Rapporti_Contabili.Agente = 1 "
            End If
        End If

        Dt_Contatti = objContatti.Contatti_Contatto_Leggi(Piva,
                                                        "",
                                                        0,
                                                        0,
                                                        True,
                                                        False,
                                                        0,
                                                        0,
                                                        False,
                                                        0,
                                                        ID_CF_NOFILTRO,
                                                        0,
                                                        "",
                                                        False,
                                                        Cliente,
                                                        Fornitore,
                                                        Dipendente,
                                                        Terzista,
                                                        Legale,
                                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        xFiltroAggiuntivo, xOrderBy,
                                                        objParametri)

        objParametri.ResettaFinestra()

        If Not IsNothing(Dt_Contatti) AndAlso Dt_Contatti.Rows.Count > 0 Then

            For i = 0 To Dt_Contatti.Rows.Count - 1

                Rag_Soc = CStr(Dt_Contatti.Rows(i).Item("Rag_Soc")) &
                       CStr(Dt_Contatti.Rows(i).Item("Cognome")) & " " &
                           CStr(Dt_Contatti.Rows(i).Item("Nome"))

                Cod = Dt_Contatti.Rows(i).Item("Cod_RisUm") & "|" & Dt_Contatti.Rows(i).Item("Piva")

                Text = Rag_Soc & " (" & Dt_Contatti.Rows(i).Item("Rapporto_Des") & ")"

                Controllo.Items.Add(New ListItem(Text, Cod))

            Next

        End If

    End Sub


    '######################################################################
    Public Shared Sub Provincie(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal VisualizzaCodiceISTAT As Boolean,
                                ByVal Ordina_Alfabetico1_Istat2 As Integer,
                                ByVal Sigla As String,
                                ByVal Reg As String,
                                ByVal Prov As String,
                                ByVal Com As String,
                                ByVal Provincia As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal Stato_Country As String = "IT"
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String
        Dim NumTotale As Integer
        Dim Ordinamento As String

        If Ordina_Alfabetico1_Istat2 = 2 Then
            'Ordino per codice ISTAT
            Ordinamento = "Prov ASC"
        Else
            'nella query l'order by è sulla provincia
            Ordinamento = "Provincia ASC"
        End If


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objListaProv As New AgronicaCoreMetaSchemaDAL.Lista_Province_R

        Dt = objListaProv.Leggi(Sigla,
                                Reg,
                                Prov,
                                Com,
                                Provincia,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                xFiltroAggiuntivo,
                                Ordinamento,
                                objParametri,
                                Stato_Country)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                If VisualizzaCodiceISTAT Then

                    x_Cod = Dt.Rows(i).Item("Sigla")
                    x_Des = "(" & Dt.Rows(i).Item("Sigla") & " " & Dt.Rows(i).Item("Prov") & ") : " & Dt.Rows(i).Item("Provincia")

                Else

                    x_Cod = Dt.Rows(i).Item("Sigla")
                    x_Des = Dt.Rows(i).Item("Provincia")

                End If

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    '######################################################################
    Public Shared Sub Province_and_Comuni(ByRef CmbPro As DropDownList,
                                          ByRef CmbCom As DropDownList,
                                          ByVal Sigla_Provincia As String,
                                          ByVal Istat_Comune As String,
                                          ByVal VisualizzaCodiceISTAT As Boolean,
                                          ByVal Ordina_Alfabetico1_Istat2 As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal Stato_Country As String = "IT")

        Dim ChiaveComuni As String
        Dim Istat_Provincia As String

        'Carico la combo delle provincie
        Provincie(CmbPro, True, "", "",
                  VisualizzaCodiceISTAT, Ordina_Alfabetico1_Istat2,
                  "", "", "", "", "", "", "", objParametri, Stato_Country)

        'Imposto la posizione sulla combo delle provincie
        CmbPro.SelectedIndex =
            CmbPro.Items.IndexOf(
                CmbPro.Items.FindByValue(
                    Sigla_Provincia))

        If Sigla_Provincia <> "" Then
            'Carico la combo dei comuni appartenenti alla provincia selezionata
            Call Comuni(CmbCom, True, "", "", Sigla_Provincia, VisualizzaCodiceISTAT, Ordina_Alfabetico1_Istat2, "", "", objParametri)
        End If

        'Costruisco la chiave per la combo dei comuni
        Dim objIstatR As New AgronicaCoreMetaSchemaDAL.Istat_R
        Istat_Provincia = objIstatR.CodIstat_from_Provincia(Sigla_Provincia, objParametri)
        ChiaveComuni = Istat_Provincia & "|" & Istat_Comune & "|" & Sigla_Provincia.ToLower

        'Imposto la posizione sulla combo dei comuni
        CmbCom.SelectedIndex =
            CmbCom.Items.IndexOf(
                CmbCom.Items.FindByValue(
                    ChiaveComuni))

    End Sub


    '######################################################################
    Public Shared Sub Regioni(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal REG As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As String
        Dim x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objListaReg As New AgronicaCoreMetaSchemaDAL.Lista_Regioni_R

        Dt = objListaReg.Leggi(REG,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri)

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("REG")
                x_Des = Dt.Rows(i).Item("Regione_Des")

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="ContoTerzi"></param>
    ''' <param name="Piva"></param>
    ''' <param name="TestoRicerca"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub Macchinari(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal ContoTerzi As Boolean,
                                ByVal Piva As String,
                                ByVal TestoRicerca As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.Macchinari()"

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim i As Integer
        Dim objMacchine As New AgronicaCoreMetaSchemaDAL.Macchine_R

        'CARICO TUTTI I MACCHINARI CHE HANNO UN PREZZO

        StbSQL.Length = 0

        StbSQL.Append(" SELECT Parco_Macchine.Class_Code, Parco_Macchine.Mac_Cod, Parco_Macchine.Mac_Des, ")
        StbSQL.Append(" Parco_Macchine.Piva, Parco_Macchine.Sa_Cod, Parco_Macchine.Modello, Parco_Macchine.Stato_Utilizzo, Macchine.CLASS_DESC ")
        StbSQL.Append(" FROM Parco_Macchine INNER JOIN ")
        StbSQL.Append(" Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")
        StbSQL.Append(" INNER JOIN UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA  ")
        StbSQL.Append(" WHERE (Macchine.CLASS_DESC LIKE '%" & TestoRicerca & "%')")
        StbSQL.Append(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

        If ContoTerzi Then
            StbSQL.Append(" AND Parco_Macchine.Sa_Cod = -1 ")
        Else
            StbSQL.Append(" AND Parco_Macchine.Sa_Cod = 0 AND Parco_Macchine.Piva = " & Piva)
        End If

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Se il datatable non è vuoto allora ...	
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            '----- Riempio la combo con i dati del datatable

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(objMacchine.MacchinaTipo_from_MacchinaCod(Dt.Rows(i).Item("Class_Code"), objParametri) & " - " & Dt.Rows(i).Item("Mac_Des"),
                                           Dt.Rows(i).Item("Piva") & "|" & Dt.Rows(i).Item("Sa_Cod") & "|" & Dt.Rows(i).Item("Mac_Cod")))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

        objMacchine = Nothing

    End Sub


    Public Sub Macchinari_2(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal Piva As String,
                            ByVal TestoRicerca As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional descrizioneCompleta As Boolean = False,
                            Optional caricaPubbliche As Boolean = False
                            )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.Macchinari()"

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim i As Integer
        Dim objMacchine As New AgronicaCoreMetaSchemaDAL.Macchine_R

        'CARICO TUTTI I MACCHINARI CHE HANNO UN PREZZO

        StbSQL.Length = 0

        StbSQL.Append(" SELECT Parco_Macchine.Class_Code, Parco_Macchine.Mac_Cod, Parco_Macchine.Mac_Des, ")
        StbSQL.Append(" Parco_Macchine.Piva, Parco_Macchine.Sa_Cod, Parco_Macchine.Modello, Parco_Macchine.Stato_Utilizzo, Macchine.CLASS_DESC ")

        If descrizioneCompleta Then
            StbSQL.Append(", Class_Desc COLLATE DATABASE_DEFAULT ")
            StbSQL.Append("  + CASE WHEN ISNULL(Ditte.Ditta_Des, '') <> '' THEN ' - ' + Ditte.Ditta_Des COLLATE DATABASE_DEFAULT ELSE '' END")
            StbSQL.Append("  + CASE WHEN ISNULL(Modello, '') <> '' THEN ' - ' + Modello COLLATE DATABASE_DEFAULT ELSE '' END")
            StbSQL.Append("  + CASE WHEN ISNULL(Parco_Macchine.Mac_Des, '') <> '' THEN ' - ' + Parco_Macchine.Mac_Des COLLATE DATABASE_DEFAULT ELSE '' END AS Descrizione_Completa")
        End If

        StbSQL.Append(" FROM Parco_Macchine INNER JOIN ")
        StbSQL.Append(" Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")
        StbSQL.Append(" INNER JOIN UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA  ")
        If descrizioneCompleta Then
            StbSQL.Append(" LEFT JOIN Ditte ON Ditte.Ditta_Cod = Parco_Macchine.Ditta_Cod ")
        End If

        StbSQL.Append(" WHERE 1=1 ")
        If TestoRicerca <> "" Then
            StbSQL.Append(" AND  Parco_Macchine.Mac_Des LIKE '%" & TestoRicerca & "%'")
        End If
        StbSQL.Append(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

        If Piva <> "" Then
            StbSQL.Append($" AND (Parco_Macchine.Piva = '{Agro_SQL_SaveText(Piva)}' {IIf(caricaPubbliche, "OR Parco_Macchine.Sa_Cod = -1", "")} ) ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
        End If

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Se il datatable non è vuoto allora ...	
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            '----- Riempio la combo con i dati del datatable

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1
                Dim descrizioneMacchina As String
                If descrizioneCompleta Then
                    descrizioneMacchina = Trim(Dt.Rows(i).Item("Descrizione_Completa"))
                Else
                    descrizioneMacchina = objMacchine.MacchinaTipo_from_MacchinaCod(Dt.Rows(i).Item("Class_Code"), objParametri) & " - " & Dt.Rows(i).Item("Mac_Des")
                End If
                Controllo.Items.Add(New ListItem(descrizioneMacchina,
                                           Dt.Rows(i).Item("Piva") & "|" & Dt.Rows(i).Item("Sa_Cod") & "|" & Dt.Rows(i).Item("Mac_Cod")))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

        objMacchine = Nothing

    End Sub




    '######################################################################
    'usare la funzione ProdottiMagazzino
    ' -----------------------------------------------------------------------------
    'usare la funzione ProdottiMagazzino
    'Public Shared Sub ProdottiMagazzino_OLD(ByRef Controllo As ListControl, _
    '                            ByVal PrimaRiga_Flag As Boolean, _
    '                            ByVal PrimaRiga_Text As String, _
    '                            ByVal PrimaRiga_Value As String, _
    '                            ByRef Causale As enum_Agenda_Causali, _
    '                            ByRef Piva As String, _
    '                            ByRef Sa_Cod As Integer, _
    '                            ByRef Destinazione As Integer, _
    '                            ByRef Elem_Cod As Integer, _
    '                            ByVal xFiltroAggiuntivo As String, _
    '                            ByVal xOrderBy As String, _
    '                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            )



    '    Dim objCOMCat As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
    '    Dim DT As DataTable
    '    Dim objProdotti As New AgronicaCoreAnagrafeDAL.Prodotti_R
    '    Dim DTProdotti As DataTable
    '    Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
    '    Dim DTGiacenze As DataTable

    '    Dim NomeTabella As String
    '    Dim NomeCodice As String
    '    Dim NomeDescrizione As String


    '    'Pulisco la lista
    '    Controllo.Items.Clear()

    '    If PrimaRiga_Flag Then
    '        Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
    '    End If

    '    Select Case Causale
    '        Case enum_Agenda_Causali.CARICO
    '            'Leggo le imprese associate al profilo selezionato

    '            DT = objCOMCat.Leggi(CInt(Elem_Cod), _
    '                                     "", _
    '                                     False, _
    '                                        "", "", _
    '                                        objParametri)

    '            If DT.Rows.Count > 0 Then

    '                NomeTabella = DT.Rows(0).Item("Tabella")
    '                NomeCodice = DT.Rows(0).Item("Tabella_Cod")
    '                NomeDescrizione = DT.Rows(0).Item("Tabella_Des")

    '                Dim filtroSTR As String
    '                If xFiltroAggiuntivo <> "" Then
    '                    xFiltroAggiuntivo = NomeDescrizione & " LIKE '%" & xFiltroAggiuntivo & "%'"
    '                End If


    '                DTProdotti = objProdotti.Leggi(CStr(NomeTabella), _
    '                                                                   "", _
    '                                                                   0, _
    '                                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                                   xFiltroAggiuntivo, _
    '                                                                   NomeDescrizione & " Asc", _
    '                                                                   objParametri)


    '                If DTProdotti.Rows.Count > 0 Then
    '                    'Inserisco i record trovati
    '                    Dim i As Integer
    '                    For i = 0 To DTProdotti.Rows.Count - 1
    '                        Controllo.Items.Add(New ListItem(DTProdotti.Rows(i).Item(NomeDescrizione), _
    '                                                   DTProdotti.Rows(i).Item(NomeCodice)))

    '                    Next
    '                End If

    '            End If


    '        Case enum_Agenda_Causali.SCARICO

    '            DTGiacenze = objGiacenze.LeggiGiacenze(CStr(Piva), CInt(Sa_Cod), CInt(Elem_Cod), 0, 0, 0, Destinazione, 0, 0, 0, "", _
    '                            "", "", objParametri)

    '            If DTGiacenze.Rows.Count > 0 Then
    '                Dim i As Integer
    '                For i = 0 To DTGiacenze.Rows.Count - 1

    '                    'Leggo i prodotti associate al profilo selezionato			
    '                    DT = objCOMCat.Leggi(CInt(DTGiacenze.Rows(i).Item("Elem_Cod")), _
    '                                            "", _
    '                                            False, _
    '                                            "", _
    '                                            "", _
    '                                            objParametri)

    '                    If DT.Rows.Count > 0 Then

    '                        NomeTabella = DT.Rows(0).Item("Tabella")
    '                        NomeCodice = DT.Rows(0).Item("Tabella_Cod")
    '                        NomeDescrizione = DT.Rows(0).Item("Tabella_Des")

    '                        DTProdotti = objProdotti.Leggi(CStr(NomeTabella), _
    '                                                       CStr(NomeCodice), _
    '                                                       CInt(DTGiacenze.Rows(i).Item("Pro_Cod")), _
    '                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                       "", _
    '                                                       "", _
    '                                                       objParametri)

    '                    End If 'fine if di DTProdotti
    '                Next
    '            End If

    '    End Select

    'End Sub

    '######################################################################
    Public Shared Sub Prodotti()

        'la funzione si trova in AgronicaCoreVarieBIZ.CaricaListControl_2010.Prodotti


    End Sub

    '######################################################################
    ' 	[gunelli]	12/05/2011
    '---------------------------------------------------------------
    'Valori opzionali:
    'Ricerca_Nome_Prodotto As String = ""
    'Flag_ClasseTossicologica As Boolean = False
    'Flag_PrincipiAttivi As Boolean = False
    'Al momento ignora i flag sopra
    '-----------------------------------------------------------------------------------
    Public Shared Sub ProdottiAnagrafica(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Data_Verifica As Date,
                                        ByRef Elem_Cod As Integer,
                                        ByVal Ricerca_Nome_Prodotto As String,
                                        ByVal Flag_ClasseTossicologica As Boolean,
                                        ByVal Flag_PrincipiAttivi As Boolean,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        'Dim objFitoPA As New AgronicaCoreMetaSchemaDAL.FormulatixPrincipiAttivi_R
        Dim objCategorie As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
        Dim objProdotti As New AgronicaCoreAnagrafeDAL.Prodotti_R
        Dim DTProdotti As DataTable
        Dim DT_Cat As DataTable
        Dim NomeTabella As String
        Dim NomeCodice As String
        Dim NomeDescrizione As String

        Dim Classe_Tossicologica As String = ""
        Dim Principi_Attivi As String = ""

        Dim Valore As String
        Dim Descrizione As String


        'Pulisco la lista
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        DT_Cat = objCategorie.Leggi(CInt(Elem_Cod),
                             CAU_MAGAZZINO,
                            False,
                            "", "",
                            objParametri)

        If Not IsNothing(DT_Cat) AndAlso DT_Cat.Rows.Count > 0 Then

            NomeTabella = DT_Cat.Rows(0).Item("Tabella")
            NomeCodice = DT_Cat.Rows(0).Item("Tabella_Cod")
            NomeDescrizione = DT_Cat.Rows(0).Item("Tabella_Des")

            Dim Filtro_Prod As String
            Filtro_Prod = NomeDescrizione & " LIKE '%" & Ricerca_Nome_Prodotto & "%'"

            If NomeTabella.ToLower = "materie_prime" Then
                Filtro_Prod &= " AND Elem_cod =" & CStr(Elem_Cod) & ""
            End If

            DTProdotti = objProdotti.Leggi(CStr(NomeTabella),
                                             NomeCodice,
                                            0,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            Filtro_Prod,
                                            NomeDescrizione & " Asc",
                                            objParametri)


            If Not IsNothing(DTProdotti) AndAlso DTProdotti.Rows.Count > 0 Then

                Dim j As Integer

                For j = 0 To DTProdotti.Rows.Count - 1

                    Select Case Elem_Cod

                        Case FORMULATI

                            '(12/11/2013) commentata perché i dati non sono più nel metaschema locale 
                            'e nelle chiamate a questa funzione al momento viene passato sempre
                            'Flag_ClasseTossicologica = false 
                            'Flag_PrincipiAttivi = false 

                            ''se c'è aggiungo la classe tossicologica
                            'If Flag_ClasseTossicologica = True Then

                            '    Classe_Tossicologica = ""

                            '    If Not IsDBNull(DTProdotti.Rows(j).Item("NewCLTOSS_Cod")) AndAlso CStr(DTProdotti.Rows(j).Item("NewCLTOSS_Cod")) <> "" Then
                            '        Classe_Tossicologica = " --- (" & CStr(DTProdotti.Rows(j).Item("NewCLTOSS_Cod")) & ")"
                            '    End If

                            'End If

                            ''se c'è aggiungo principi attivi
                            'If Flag_PrincipiAttivi = True Then

                            '    Principi_Attivi = objFitoPA.PrincipiAttivi_from_FrCod(CInt(DTProdotti.Rows(j).Item("fr_cod")), _
                            '                                                             False, _
                            '                                                             "", "", _
                            '                                                             objParametri)

                            '    If Principi_Attivi <> "" Then
                            '        Principi_Attivi = " --- <" & Principi_Attivi & ">"
                            '    End If

                            'End If

                            ' Se è un Formulato aggiungo Fr_Cod
                            Descrizione = DTProdotti.Rows(j).Item(NomeDescrizione) & "  (" & CStr(DTProdotti.Rows(j).Item(NomeCodice)) & ")" & Classe_Tossicologica & Principi_Attivi
                            Valore = DTProdotti.Rows(j).Item(NomeCodice)

                            '----------------

                        Case Else

                            Descrizione = DTProdotti.Rows(j).Item(NomeDescrizione)
                            Valore = DTProdotti.Rows(j).Item(NomeCodice)

                    End Select

                    If IsNothing(Controllo.Items.FindByValue(Valore)) Then
                        Controllo.Items.Add(New ListItem(Descrizione, Valore))
                    End If


                Next


            End If

        End If

    End Sub




    ''######################################################################
    '' 	[gunelli]	12/05/2011
    '' -----------------------------------------------------------------------------
    ''NOTA
    ''Il TipoRichiesto consente di selezionare solo i formulati specifici
    ''per la particolare applicazione
    ''
    ''   0 = Tutti i formulati
    ''   1 = Trattamenti Antiparassitari
    ''   2 = Diserbo
    ''   3 = Trattamenti Fitoregolatori
    ''   4 = Geodisinfestazione (Trattamenti Antiparassitari e Diserbo)
    ''   ecc...
    ''---------------------------------------------------------------
    ''Valori opzionali:
    ''Ricerca_Nome_Prodotto As String = ""
    ''  Veg_Cod As Integer = 0
    ''strPA As String = ""
    ''Flag_ClasseTossicologica As Boolean = False
    ''Flag_PrincipiAttivi As Boolean = False
    ''Flag_NPK As Boolean = False, _
    ''TipoRichiesto As Integer = 0
    '' xFiltroAggiuntivo_Classificazione_PA As String = ""
    ''-----------------------------------------------------------------------------------
    'Public Shared Sub ProdottiMagazzino(ByRef Controllo As ListControl, _
    '                                    ByVal PrimaRiga_Flag As Boolean, _
    '                                    ByVal PrimaRiga_Text As String, _
    '                                    ByVal PrimaRiga_Value As String, _
    '                                    ByRef Piva As String, _
    '                                    ByRef Sa_Cod As Integer, _
    '                                    ByRef Destinazione As Integer, _
    '                                    ByVal Data_Verifica As Date, _
    '                                    ByRef Elem_Cod As Integer, _
    '                                    ByVal Ricerca_Nome_Prodotto As String, _
    '                                    ByVal TipoRichiesto As Integer, _
    '                                    ByVal Veg_Cod As Integer, _
    '                                    ByVal strPA As String, _
    '                                    ByVal Flag_ClasseTossicologica As Boolean, _
    '                                    ByVal Flag_PrincipiAttivi As Boolean, _
    '                                    ByVal Flag_NPK As Boolean, _
    '                                    ByVal xFiltroAggiuntivo_Classificazione_PA As String, _
    '                                    ByVal xFiltroAggiuntivo_Giacenze As String, _
    '                                    ByVal xOrderBy As String, _
    '                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                    )

    '    Dim objFitoPA As New AgronicaCoreMetaSchemaDAL.FormulatixPrincipiAttivi_R

    '    Dim StrFormulati As String = ""

    '    Dim Classe_Tossicologica As String
    '    Dim Principi_Attivi As String

    '    Dim Valore As String
    '    Dim Descrizione As String

    '    'Pulisco la lista
    '    Controllo.Items.Clear()

    '    If PrimaRiga_Flag Then
    '        Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
    '    End If


    '    'Per i Formulati è possibile scegliere il Tipo in base alla classificazione
    '    If TipoRichiesto <> 0 Then

    '        Dim objFitoxClass As New AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R
    '        Dim Dt_Formulati As DataTable

    '        Dt_Formulati = objFitoxClass.LeggiXClassificazione_PA(CStr(Ricerca_Nome_Prodotto), _
    '                                                                CInt(TipoRichiesto), _
    '                                                                CInt(Veg_Cod), _
    '                                                                CStr(strPA), _
    '                                                                AGRODATAINIZIO, _
    '                                                                CDate(Data_Verifica), _
    '                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                                xFiltroAggiuntivo_Classificazione_PA, _
    '                                                                "", _
    '                                                                objParametri _
    '                                                                )

    '        If Not Dt_Formulati Is Nothing AndAlso Dt_Formulati.Rows.Count > 0 Then

    '            Dim i As Integer
    '            For i = 0 To Dt_Formulati.Rows.Count - 1
    '                StrFormulati = StrFormulati & Dt_Formulati.Rows(i).Item("fr_cod") & ","
    '            Next

    '            Dt_Formulati = Nothing

    '        End If

    '        If StrFormulati <> "" Then
    '            StrFormulati = Left(StrFormulati, StrFormulati.Length - 1)
    '            'la query giacenze non ha l'and
    '            StrFormulati = " AND pro_cod IN (" & StrFormulati & ")"
    '            ''nei core non ci va l'and
    '            'StrFormulati = " pro_cod IN (" & StrFormulati & ")"
    '        End If

    '    End If


    '    '====================================================
    '    '==========           SCARICO           =============
    '    '====================================================


    '    ' Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
    '    Dim objG As New AgronicaCoreContabDAL.Giacenze_R
    '    Dim DT_Giacenze As DataTable
    '    Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R
    '    Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
    '    Dim NewCLTOSS_Cod As String
    '    Dim Denominazione As String
    '    Dim N As String
    '    Dim P2O5 As String
    '    Dim K2O As String
    '    Dim MgO As String
    '    Dim xFiltroAggiuntivo_1 As String = ""
    '    Dim xFiltroAggiuntivo_3 As String = ""
    '    Dim xFiltroAggiuntivo_4 As String = ""
    '    Dim xFiltroAggiuntivo_5 As String = ""
    '    Dim xFiltroAggiuntivo_6 As String = ""
    '    Dim xFiltroAggiuntivo_7 As String = ""
    '    Dim xFiltroAggiuntivo_8 As String = ""

    '    'DTGiacenze = objGiacenze.LeggiGiacenze(CStr(Piva), CInt(Sa_Cod), CInt(Elem_Cod), 0, 0, 0, Destinazione, 0, 0, 0, "", _
    '    '                "", "", objParametri)

    '    xFiltroAggiuntivo_Giacenze &= " " & StrFormulati

    '    If Ricerca_Nome_Prodotto <> "" Then
    '        Select Case Elem_Cod
    '            Case COADIUVANTI
    '                xFiltroAggiuntivo_1 = " AND Coadiuvante.Coad_Des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
    '            Case FERTILIZZANTI
    '                xFiltroAggiuntivo_3 = " AND Fertilizzanti.Fer_des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
    '                xFiltroAggiuntivo_7 = " AND Materie_Prime.Mat_Des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
    '            Case FORMULATI
    '                xFiltroAggiuntivo_4 = " AND Formulati.Fr_des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
    '            Case INNESCHI
    '                xFiltroAggiuntivo_5 = " AND Avversita.Av_des_Vol LIKE '%" & Ricerca_Nome_Prodotto & "%'"
    '            Case INSETTI
    '                xFiltroAggiuntivo_6 = " AND InsettiUtili.Ins_Des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
    '            Case TRAPPOLE
    '                xFiltroAggiuntivo_8 = " AND Trappole.Trap_Des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
    '        End Select
    '    End If


    '    DT_Giacenze = objG.SchedaGiacenzeMagazzino(Data_Verifica, _
    '                                                Piva, _
    '                                                Sa_Cod, _
    '                                                Destinazione, _
    '                                                Elem_Cod, _
    '                                                0, _
    '                                                0, 0, 0, 0, 0, _
    '                                                LOTTO_NONDEFINITO, _
    '                                                True, _
    '                                                xFiltroAggiuntivo_Giacenze, _
    '                                                xFiltroAggiuntivo_1, _
    '                                                "", _
    '                                                xFiltroAggiuntivo_3, _
    '                                                xFiltroAggiuntivo_4, _
    '                                                xFiltroAggiuntivo_5, _
    '                                                xFiltroAggiuntivo_6, _
    '                                                xFiltroAggiuntivo_7, _
    '                                                xFiltroAggiuntivo_8, _
    '                                                "", "", _
    '                                                "", _
    '                                                objParametri)

    '    If Not IsNothing(DT_Giacenze) AndAlso DT_Giacenze.Rows.Count > 0 Then

    '        Dim i As Integer

    '        For i = 0 To DT_Giacenze.Rows.Count - 1

    '            'NomeTabella = DT_Giacenze.Rows(i).Item("Tabella")
    '            'NomeCodice = DT_Giacenze.Rows(i).Item("Tabella_Cod")
    '            'NomeDescrizione = DT_Giacenze.Rows(i).Item("Tabella_Des")

    '            'Select Case Elem_Cod

    '            '    Case FERTILIZZANTI

    '            '        'per i fertilizzanti non uso più la funzione NewCom_LeggiTabella_da_CategorieMagazzino
    '            '        'perché oltre alla tabella fertilizzanti, devo leggere anche la tabella materie_prime 
    '            '        '(per i fertilizzanti aziendali)

    '            '        Dt_Prodotti = NewCom_Fertilizzanti_Completa_Leggi(objServer, objSession, objPage, _
    '            '                                                           Piva, _
    '            '                                                           0, _
    '            '                                                           True, _
    '            '                                                           Testo_Ricerca, _
    '            '                                                           DT_Giacenze.Rows(i).Item("Pro_Cod"), _
    '            '                                                           DT_Giacenze.Rows(i).Item("Mat_Cod"))


    '            '    Case Else

    '            '        Dt_Prodotti = NewCom_LeggiTabella_da_CategorieMagazzino(objServer, objSession, objPage, _
    '            '                                                                NomeTabella, _
    '            '                                                                NomeCodice, _
    '            '                                                                NomeDescrizione, _
    '            '                                                                Testo_Ricerca, _
    '            '                                                                DT_Giacenze.Rows(i).Item("Pro_Cod"), _
    '            '                                                                "")

    '            'End Select

    '            'If Not IsNothing(Dt_Prodotti) Then

    '            '    For j = 0 To Dt_Prodotti.Rows.Count - 1


    '            Select Case Elem_Cod

    '                Case FORMULATI

    '                    ' Se è un Formulato aggiungo Fr_Cod

    '                    'se c'è aggiungo la classe tossicologica
    '                    If Flag_ClasseTossicologica = True Then

    '                        Classe_Tossicologica = ""

    '                        NewCLTOSS_Cod = objFito.NewCLTossCod_from_FrCod(DT_Giacenze.Rows(i).Item("pro_cod"), _
    '                                                                                                objParametri)

    '                        If Not IsDBNull(NewCLTOSS_Cod) AndAlso NewCLTOSS_Cod <> "" Then
    '                            Classe_Tossicologica = " --- (" & NewCLTOSS_Cod & ")"
    '                        End If

    '                    End If

    '                    'se richiesto aggiungo i pa
    '                    If Flag_PrincipiAttivi = True Then

    '                        Principi_Attivi = objFitoPA.PrincipiAttivi_from_FrCod(DT_Giacenze.Rows(i).Item("pro_cod"), _
    '                                                                                 False, _
    '                                                                                 "", "", _
    '                                                                                 objParametri)

    '                        If Principi_Attivi <> "" Then
    '                            Principi_Attivi = " --- <" & Principi_Attivi & ">"
    '                        End If
    '                    End If

    '                    Descrizione = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto") & _
    '                    "  (" & CStr(DT_Giacenze.Rows(i).Item("pro_cod")) & ")" & Classe_Tossicologica & Principi_Attivi

    '                    Valore = DT_Giacenze.Rows(i).Item("pro_cod")


    '                Case FERTILIZZANTI

    '                    If Flag_NPK = True Then

    '                        objFert.Titoli_from_FerCod(DT_Giacenze.Rows(i).Item("pro_cod"), _
    '                                                       Denominazione, _
    '                                                        N, _
    '                                                        P2O5, _
    '                                                        K2O, _
    '                                                        MgO, _
    '                                                        objParametri)

    '                        Descrizione = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto") & _
    '                                     " --- <" & _
    '                                     N & "-" & _
    '                                     P2O5 & "-" & _
    '                                     K2O & "-" & _
    '                                     MgO & _
    '                                     ">"
    '                    Else
    '                        Descrizione = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto")
    '                    End If

    '                    Valore = DT_Giacenze.Rows(i).Item("pro_cod") & "/" & DT_Giacenze.Rows(i).Item("mat_cod")

    '                Case Else

    '                    Descrizione = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto")
    '                    Valore = DT_Giacenze.Rows(i).Item("pro_cod")

    '            End Select

    '            If IsNothing(Controllo.Items.FindByValue(Valore)) Then
    '                Controllo.Items.Add(New ListItem(Descrizione, Valore))
    '            End If

    '            '            Next

    '            'End If

    '        Next

    '    End If 'dt giacenze




    '    'Dim Dr_new() As DataRow
    '    'Dim Dt_new As New DataTable

    '    'Select Case DT.Rows.Count

    '    '    Case 1

    '    '        If TipoRichiesto <> 0 Then

    '    '            Dt_new = DT.Clone
    '    '            Dr_new = DT.Select("Valore IN (" & StrFormulati & ")")

    '    '            If Not Dr_new Is Nothing Then
    '    '                For i = 0 To UBound(Dr_new)
    '    '                    Dt_new.ImportRow(Dr_new(i))
    '    '                Next
    '    '            End If

    '    '            If Dt_new.Rows.Count > 0 Then
    '    '                Cmb.Items.Add(New ListItem(Dt_new.Rows(0).Item("Descrizione"), Dt_new.Rows(0).Item("Valore")))
    '    '            End If

    '    '        Else

    '    '            Cmb.Items.Add(New ListItem(DT.Rows(0).Item("Descrizione"), DT.Rows(0).Item("Valore")))

    '    '        End If

    '    '    Case Is > 1

    '    '        If TipoRichiesto <> 0 Then

    '    '            Dt_new = DT.Clone
    '    '            Dr_new = DT.Select("Valore IN (" & StrFormulati & ")")

    '    '            If Not Dr_new Is Nothing Then
    '    '                For i = 0 To UBound(Dr_new)
    '    '                    Dt_new.ImportRow(Dr_new(i))
    '    '                Next
    '    '            End If

    '    '            'uso il dataview per ordinare 
    '    '            Dim Dv As New DataView

    '    '            Dt_new.TableName = "Prodotti"
    '    '            Dv.Table = Dt_new
    '    '            Dv.Sort = "Descrizione ASC"

    '    '            For i = 0 To Dv.Count - 1
    '    '                Select Case i
    '    '                    Case 0
    '    '                        Cmb.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
    '    '                    Case Else
    '    '                        If Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
    '    '                            Cmb.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
    '    '                        End If
    '    '                End Select
    '    '            Next

    '    '        Else

    '    '            'uso il dataview per ordinare 
    '    '            Dim Dv As New DataView

    '    '            DT.TableName = "Prodotti"
    '    '            Dv.Table = DT
    '    '            Dv.Sort = "Descrizione ASC"

    '    '            For i = 0 To Dv.Count - 1
    '    '                Select Case i
    '    '                    Case 0
    '    '                        Cmb.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
    '    '                    Case Else
    '    '                        If Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
    '    '                            Cmb.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
    '    '                        End If
    '    '                End Select
    '    '            Next

    '    '        End If

    '    'End Select


    'End Sub



    '######################################################################
    '11/07/2018: riattivata dalla vecchia funzione ma non utilizzata
    'adesso si usa sempre la AgronicaCoreVarieBIZ.CaricaListControl_2010.Prodotti 
    'eventualmente questa può servire per caricamenti veloci, solo descrizioni, senza chiamare il ws
    Public Shared Sub ProdottiMagazzinoBasica(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByRef Piva As String,
                                        ByRef Sa_Cod As Integer,
                                        ByRef Destinazione As Integer,
                                        ByVal Data_Verifica As Date,
                                        ByRef Elem_Cod As Integer,
                                        ByVal Ricerca_Nome_Prodotto As String,
                                        ByVal xFiltroAggiuntivo_Giacenze As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametriServer As AgronicaCoreParametri,
                                        ByRef objParametriUtenti As AgronicaCoreParametri)

        '                         ByVal TipoRichiesto As Integer, _
        '                                ByVal Veg_Cod As Integer, _
        '                                ByVal strPA As String, _
        '                                ByVal Flag_ClasseTossicologica As Boolean, _
        '                                ByVal Flag_PrincipiAttivi As Boolean, _
        '                                ByVal Flag_NPK As Boolean, _
        '                                ByVal xFiltroAggiuntivo_Classificazione_PA As String, _

        'Dim objFitoPA As New AgronicaCoreMetaSchemaDAL.FormulatixPrincipiAttivi_R

        Dim StrFormulati As String = ""

        'Dim Classe_Tossicologica As String
        'Dim Principi_Attivi As String

        Dim Valore As String
        Dim Descrizione As String

        'Pulisco la lista
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        ''Per i Formulati è possibile scegliere il Tipo in base alla classificazione
        'If TipoRichiesto <> 0 Then

        '    Dim objFitoxClass As New AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R
        '    Dim Dt_Formulati As DataTable

        '    Dt_Formulati = objFitoxClass.LeggiXClassificazione_PA(CStr(Ricerca_Nome_Prodotto), _
        '                                                            CInt(TipoRichiesto), _
        '                                                            CInt(Veg_Cod), _
        '                                                            CStr(strPA), _
        '                                                            AGRODATAINIZIO, _
        '                                                            CDate(Data_Verifica), _
        '                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
        '                                                            xFiltroAggiuntivo_Classificazione_PA, _
        '                                                            "", _
        '                                                            objParametriServer _
        '                                                            )

        '    If Not Dt_Formulati Is Nothing AndAlso Dt_Formulati.Rows.Count > 0 Then

        '        Dim i As Integer
        '        For i = 0 To Dt_Formulati.Rows.Count - 1
        '            StrFormulati = StrFormulati & Dt_Formulati.Rows(i).Item("fr_cod") & ","
        '        Next

        '        Dt_Formulati = Nothing

        '    End If

        '    If StrFormulati <> "" Then
        '        StrFormulati = Left(StrFormulati, StrFormulati.Length - 1)
        '        'la query giacenze non ha l'and
        '        StrFormulati = " AND pro_cod IN (" & StrFormulati & ")"
        '        ''nei core non ci va l'and
        '        'StrFormulati = " pro_cod IN (" & StrFormulati & ")"
        '    End If

        'End If


        '====================================================
        '==========           SCARICO           =============
        '====================================================


        ' Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        Dim DT_Giacenze As DataTable
        Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R
        Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
        'Dim NewCLTOSS_Cod As String
        'Dim Denominazione As String
        'Dim N As String
        'Dim P2O5 As String
        'Dim K2O As String
        'Dim MgO As String
        Dim xFiltroAggiuntivo_1 As String = ""
        Dim xFiltroAggiuntivo_3 As String = ""
        Dim xFiltroAggiuntivo_4 As String = ""
        Dim xFiltroAggiuntivo_5 As String = ""
        Dim xFiltroAggiuntivo_6 As String = ""
        Dim xFiltroAggiuntivo_7 As String = ""
        Dim xFiltroAggiuntivo_8 As String = ""

        xFiltroAggiuntivo_Giacenze &= " " & StrFormulati

        If Ricerca_Nome_Prodotto <> "" Then
            Select Case Elem_Cod
                Case COADIUVANTI
                    xFiltroAggiuntivo_1 = " AND Coadiuvante.Coad_Des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
                Case FERTILIZZANTI
                    xFiltroAggiuntivo_3 = " AND Fertilizzanti.Fer_des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
                    xFiltroAggiuntivo_7 = " AND Materie_Prime.Mat_Des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
                Case FORMULATI
                    xFiltroAggiuntivo_4 = " AND Formulati.Fr_des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
                Case INNESCHI
                    xFiltroAggiuntivo_5 = " AND Avversita.Av_des_Vol LIKE '%" & Ricerca_Nome_Prodotto & "%'"
                Case INSETTI
                    xFiltroAggiuntivo_6 = " AND InsettiUtili.Ins_Des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
                Case TRAPPOLE
                    xFiltroAggiuntivo_8 = " AND Trappole.Trap_Des LIKE '%" & Ricerca_Nome_Prodotto & "%'"
            End Select
        End If


        DT_Giacenze = objG.SchedaGiacenzeMagazzino(Data_Verifica,
                                                    Piva,
                                                    Sa_Cod,
                                                    Destinazione,
                                                    Elem_Cod,
                                                    0,
                                                    0, 0, 0, 0, 0,
                                                    LOTTO_NONDEFINITO,
                                                    True,
                                                    xFiltroAggiuntivo_Giacenze,
                                                    xFiltroAggiuntivo_1,
                                                    "",
                                                    xFiltroAggiuntivo_3,
                                                    xFiltroAggiuntivo_4,
                                                    xFiltroAggiuntivo_5,
                                                    xFiltroAggiuntivo_6,
                                                    xFiltroAggiuntivo_7,
                                                    xFiltroAggiuntivo_8,
                                                    "", "",
                                                    "",
                                                    objParametriServer, objParametriUtenti)

        If Not IsNothing(DT_Giacenze) AndAlso DT_Giacenze.Rows.Count > 0 Then

            Dim i As Integer

            For i = 0 To DT_Giacenze.Rows.Count - 1

                Select Case Elem_Cod

                    Case FORMULATI

                        '' Se è un Formulato aggiungo Fr_Cod

                        ''se c'è aggiungo la classe tossicologica
                        'If Flag_ClasseTossicologica = True Then

                        '    Classe_Tossicologica = ""

                        '    NewCLTOSS_Cod = objFito.NewCLTossCod_from_FrCod(DT_Giacenze.Rows(i).Item("pro_cod"), _
                        '                                                                            objParametriServer)

                        '    If Not IsDBNull(NewCLTOSS_Cod) AndAlso NewCLTOSS_Cod <> "" Then
                        '        Classe_Tossicologica = " --- (" & NewCLTOSS_Cod & ")"
                        '    End If

                        'End If

                        ''se richiesto aggiungo i pa
                        'If Flag_PrincipiAttivi = True Then

                        '    Principi_Attivi = objFitoPA.PrincipiAttivi_from_FrCod(DT_Giacenze.Rows(i).Item("pro_cod"), _
                        '                                                             False, _
                        '                                                             "", "", _
                        '                                                             objParametri)

                        '    If Principi_Attivi <> "" Then
                        '        Principi_Attivi = " --- <" & Principi_Attivi & ">"
                        '    End If
                        'End If

                        Descrizione = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto") &
                        "  (" & CStr(DT_Giacenze.Rows(i).Item("pro_cod")) & ")" '& Classe_Tossicologica & Principi_Attivi

                        Valore = DT_Giacenze.Rows(i).Item("pro_cod")


                    Case FERTILIZZANTI

                        'If Flag_NPK = True Then

                        '    objFert.Titoli_from_FerCod(DT_Giacenze.Rows(i).Item("pro_cod"), _
                        '                                   Denominazione, _
                        '                                    N, _
                        '                                    P2O5, _
                        '                                    K2O, _
                        '                                    MgO, _
                        '                                    objParametriServer)

                        '    Descrizione = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto") & _
                        '                 " --- <" & _
                        '                 N & "-" & _
                        '                 P2O5 & "-" & _
                        '                 K2O & "-" & _
                        '                 MgO & _
                        '                 ">"
                        'Else
                        '    Descrizione = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto")
                        'End If

                        Descrizione = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto")

                        Valore = DT_Giacenze.Rows(i).Item("pro_cod") & "/" & DT_Giacenze.Rows(i).Item("mat_cod")

                    Case Else

                        Descrizione = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto")
                        Valore = DT_Giacenze.Rows(i).Item("pro_cod")

                End Select

                If IsNothing(Controllo.Items.FindByValue(Valore)) Then
                    Controllo.Items.Add(New ListItem(Descrizione, Valore))
                End If

                '            Next

                'End If

            Next

        End If 'dt giacenze




        'Dim Dr_new() As DataRow
        'Dim Dt_new As New DataTable

        'Select Case DT.Rows.Count

        '    Case 1

        '        If TipoRichiesto <> 0 Then

        '            Dt_new = DT.Clone
        '            Dr_new = DT.Select("Valore IN (" & StrFormulati & ")")

        '            If Not Dr_new Is Nothing Then
        '                For i = 0 To UBound(Dr_new)
        '                    Dt_new.ImportRow(Dr_new(i))
        '                Next
        '            End If

        '            If Dt_new.Rows.Count > 0 Then
        '                Cmb.Items.Add(New ListItem(Dt_new.Rows(0).Item("Descrizione"), Dt_new.Rows(0).Item("Valore")))
        '            End If

        '        Else

        '            Cmb.Items.Add(New ListItem(DT.Rows(0).Item("Descrizione"), DT.Rows(0).Item("Valore")))

        '        End If

        '    Case Is > 1

        '        If TipoRichiesto <> 0 Then

        '            Dt_new = DT.Clone
        '            Dr_new = DT.Select("Valore IN (" & StrFormulati & ")")

        '            If Not Dr_new Is Nothing Then
        '                For i = 0 To UBound(Dr_new)
        '                    Dt_new.ImportRow(Dr_new(i))
        '                Next
        '            End If

        '            'uso il dataview per ordinare 
        '            Dim Dv As New DataView

        '            Dt_new.TableName = "Prodotti"
        '            Dv.Table = Dt_new
        '            Dv.Sort = "Descrizione ASC"

        '            For i = 0 To Dv.Count - 1
        '                Select Case i
        '                    Case 0
        '                        Cmb.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
        '                    Case Else
        '                        If Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
        '                            Cmb.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
        '                        End If
        '                End Select
        '            Next

        '        Else

        '            'uso il dataview per ordinare 
        '            Dim Dv As New DataView

        '            DT.TableName = "Prodotti"
        '            Dv.Table = DT
        '            Dv.Sort = "Descrizione ASC"

        '            For i = 0 To Dv.Count - 1
        '                Select Case i
        '                    Case 0
        '                        Cmb.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
        '                    Case Else
        '                        If Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
        '                            Cmb.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
        '                        End If
        '                End Select
        '            Next

        '        End If

        'End Select


    End Sub



    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Ore(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        For i = 0 To 23

            x_Cod = i
            x_Des = Right("00" + CStr(i), 2)

            Controllo.Items.Add(New ListItem(x_Des, x_Cod))

        Next

    End Sub


    '######################################################################
    Public Shared Sub Conformita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim objCOM As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim DT As DataTable

        'Leggo le imprese associate al profilo selezionato
        DT = objCOM.Leggi(0, "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "creatore = 'AGRONICA' and gruppo = 'CONFORMITA'",
                            "",
                            objParametri)

        objCOM = Nothing


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Se il datatable non è chiuso allora ...	
        If DT.Rows.Count > 0 Then

            Dim i As Integer
            'Inserisco i record trovati
            For i = 0 To DT.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("descrizione"),
                                           DT.Rows(i).Item("codice")))

            Next

        End If

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Carica la combo dei produttori direttamente dal datatable passato. Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="dt"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Prodotto(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal dt As DataTable,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Se il datatable non è chiuso allora ...	
        If dt.Rows.Count > 0 Then

            Dim i As Integer
            'Inserisco i record trovati
            For i = 0 To dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(dt.Rows(i).Item("Mat_Des"),
                                           dt.Rows(i).Item("Mat_Cod")))

            Next

        End If

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Carica la combo dei corpi estranei direttamente dal datatable passato. Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="dt"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub CorpiEstranei(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal dt As DataTable,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Se il datatable non è chiuso allora ...	
        If dt.Rows.Count > 0 Then

            Dim i As Integer
            'Inserisco i record trovati
            For i = 0 To dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(dt.Rows(i).Item("Desc_CorpoEstraneo"),
                                           dt.Rows(i).Item("Cod_CorpoEstraneo")))

            Next

        End If

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Carica la combo dell'appezzamento direttamente dal datatable passato. Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="dt"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Appezzamento(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal dt As DataTable,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Se il datatable non è chiuso allora ...	
        If dt.Rows.Count > 0 Then
            Dim i As Integer
            'Inserisco i record trovati
            Dim descrizione As String
            Dim Codice As String

            For i = 0 To dt.Rows.Count - 1

                descrizione = CStr(dt.Rows(i).Item("Validita_Inizio_Impianti")) & " " &
                                CStr(dt.Rows(i).Item("Veg_Des")) & " " &
                                CStr(dt.Rows(i).Item("Cul_Des")) & " " &
                                "Sup. " & CStr(dt.Rows(i).Item("Sup_Imp")) & " Ha"

                Codice = dt.Rows(i).Item("Piva") & "|" &
                        CStr(dt.Rows(i).Item("Sa_Cod")) & "|" &
                        CStr(dt.Rows(i).Item("Appezza")) & "|" &
                        CStr(dt.Rows(i).Item("Id_Reg"))

                Controllo.Items.Add(New ListItem(descrizione,
                                           Codice))

            Next

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Carica la combo dei produttori direttamente dal datatable passato. Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="dt"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Produttori(ByRef Controllo As ListControl,
                                 ByVal PrimaRiga_Flag As Boolean,
                                 ByVal PrimaRiga_Text As String,
                                 ByVal PrimaRiga_Value As String,
                                 ByVal dt As DataTable,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If dt.Rows.Count > 0 Then

            Dim i As Integer
            'Inserisco i record trovati
            For i = 0 To dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(dt.Rows(i).Item("Rag_Soc"),
                                                 dt.Rows(i).Item("Figlio")))
            Next

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub CodiciTerreno(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.CodiciTerreno()"

        Dim strSql As New StringBuilder
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim Dt As New DataTable
        Dim i As Integer = 0

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        strSql.Length = 0
        strSql.Append(" SELECT Codice, Descrizione FROM Codici_Anagrafe ")
        strSql.Append(" WHERE gruppo = 'TERRENO' AND codice >= 3000 ")
        strSql.Append(" ORDER BY Descrizione ")

        'Recupero il datatable
        Dt = objDataProvider.EseguiQuery_Lettura(objParametri, strSql.ToString, NomeRoutine)

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Descrizione"),
                                           Dt.Rows(i).Item("Codice")))

            Next

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Codice"></param>
    ''' <param name="Gruppo"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Codici(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Codice As Integer,
                                ByVal Gruppo As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim DT As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R


        DT = objCodiceAnagrafe.Leggi(Codice,
                                     Gruppo,
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     xFiltroAggiuntivo,
                                     "",
                                     objParametri)



        If Not IsNothing(DT) Then

            NumTotale = DT.Rows.Count

            For i = 0 To DT.Rows.Count - 1

                x_Cod = DT.Rows(i).Item("codice")
                x_Des = CStr(DT.Rows(i).Item("descrizione"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        DT = Nothing

    End Sub



    '###############################################################################
    '''Usato
    '''Versione tipo core ma non standard
    '''spostare nel core: AgronicaCoreUtility
    Public Shared Sub CaricaCombo_ParticelleCatastali_xAppezzamento(ByRef Cmb As DropDownList,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Appezza As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        '----- Definisco le variabili

        Dim objCOM As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R  'Agro_Anagrafe_AD.AppezzaxParticelle_R
        Dim DT As DataTable

        Dim Testo As String
        Dim Valore As String

        Dim StrCodProvincia As String
        Dim StrCodComune As String
        Dim StrSezione As String
        Dim StrFoglio As String
        Dim StrNumero As String
        Dim StrSubalterno As String

        '-----

        'Leggo le imprese associate al profilo selezionato			
        DT = objCOM.LeggiParticelle_Da_Appezzamento(CStr(Piva), CInt(Sa_Cod), CInt(Appezza),
                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    "", "", objParametri)

        '----- Riempio la combo con i dati del recordset

        'Pulisco la combo
        Cmb.Items.Clear()

        'Se il recordset non è chiuso allora ...	
        If DT.Rows.Count > 0 Then

            'Inserisco una riga vuota
            Cmb.Items.Add(New ListItem("", ""))

            'Inserisco i record trovati
            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1
                '----- Definisco il testo

                Testo = ""

                StrCodProvincia = Left("_" & DT.Rows(i).Item("Prov") & "_", 10)
                StrCodComune = Left("_" & DT.Rows(i).Item("Com") & "_", 10)

                If DT.Rows(i).Item("Sezione") = "0" Then
                    StrSezione = Left("_", 6)
                Else
                    StrSezione = Left("_" & DT.Rows(i).Item("Sezione") & "_", 6)
                End If

                StrFoglio = Left("_" & DT.Rows(i).Item("Foglio") & "_", 7)
                StrNumero = Left("_" & DT.Rows(i).Item("Numero") & "_", 7)

                If DT.Rows(i).Item("Subalterno") = "0" Then
                    StrSubalterno = Left("_", 7)
                Else
                    StrSubalterno = Left("_" & DT.Rows(i).Item("Subalterno") & "_", 7)
                End If

                Testo = StrCodProvincia & " : " &
                        StrCodComune & " : " &
                        StrSezione & " : " &
                        StrFoglio & " : " &
                        StrNumero & " / " &
                        StrSubalterno


                '----- Costruisco la stringa del valore
                Valore = ""

                Valore = "" & DT.Rows(i).Item("Prov") &
                        "£" & DT.Rows(i).Item("Com") &
                        "£" & DT.Rows(i).Item("Sezione") &
                        "£" & DT.Rows(i).Item("Foglio") &
                        "£" & DT.Rows(i).Item("Numero") &
                        "£" & DT.Rows(i).Item("Subalterno")

                'Inserisco la voce nella combobox
                Cmb.Items.Add(New ListItem(Testo, Valore))

                'Rs.MoveNext()

            Next

            'Chiudo il recordset

        End If

        'Elimino il recordset
        'Rs = Nothing

    End Sub



    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub CodiciAnagrafe_CentroAziendale(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim objCOM As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim DTRs As DataTable

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        DTRs = objCOM.Leggi(0, "",
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                           " UPPER(gruppo) = 'CENTRO' OR UPPER(creatore) = 'CRPA' ",
                            "",
                            objParametri)

        objCOM = Nothing

        '----- Riempio la combo con i dati del recordset

        'Se il datatable non è chiuso allora ...	
        If DTRs.Rows.Count > 0 Then

            Dim i As Integer
            For i = 0 To DTRs.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DTRs.Rows(i).Item("descrizione"),
                           DTRs.Rows(i).Item("codice")))

            Next
        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub CodiciAnagrafe_Fabbricato(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim objCOM As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim DT As DataTable


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        DT = objCOM.Leggi_2(0, "FABBRICATO",
                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "gruppo = 'FABBRICATO'",
                             "",
                             objParametri)

        objCOM = Nothing

        '----- Riempio la combo con i dati del datatable

        'Se il datatable non è chiuso allora ...	
        If DT.Rows.Count > 0 Then

            Dim i As Integer
            'Inserisco i record trovati
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("descrizione"),
                                            DT.Rows(i).Item("codice")))

            Next

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    'usare BIO ORGANISMI CONTTROLLO
    'Public Shared Sub OrganismiControllo(ByRef Controllo As ListControl, _
    '                            ByVal PrimaRiga_Flag As Boolean, _
    '                            ByVal PrimaRiga_Text As String, _
    '                            ByVal PrimaRiga_Value As String, _
    '                            ByVal xFiltroAggiuntivo As String, _
    '                            ByVal xOrderBy As String, _
    '                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            )

    '    'Dim objCOM As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
    '    Dim DTRs As DataTable

    '    'Pulisco il controllo
    '    Controllo.Items.Clear()

    '    If PrimaRiga_Flag Then
    '        Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
    '    End If

    '    ''Leggo le imprese associate al profilo selezionato			
    '    'DTRs = objCOM.Leggi(0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '    '                    "creatore = 'AGRONICA' and gruppo = 'ORGANISMO'", _
    '    '                    "", _
    '    '                    objParametri)



    '    Dim obj As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrganismiControllo_R
    '    DTRs = obj.Leggi(0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)




    '    'objCOM = Nothing


    '    'Se il datatable non è chiuso allora ...	
    '    If DTRs.Rows.Count > 0 Then

    '        'Inserisco i record trovati
    '        Dim i As Integer
    '        For i = 0 To DTRs.Rows.Count - 1

    '            Controllo.Items.Add(New ListItem(DTRs.Rows(i).Item("descrizione"), _
    '                                       DTRs.Rows(i).Item("codice")))

    '        Next
    '    End If

    'End Sub


    '######################################################################
    Public Shared Sub TipoCentro(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim objCOM As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim DTRs As DataTable

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        DTRs = objCOM.Leggi(0, "",
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "creatore = 'CSA' and gruppo = 'TIPO_CA'",
                            "",
                            objParametri)

        objCOM = Nothing



        '----- Riempio la combo con i dati del recordset

        'Se il datatable non è chiuso allora ...	
        If DTRs.Rows.Count > 0 Then

            Dim i As Integer

            'Inserisco i record trovati
            For i = 0 To DTRs.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DTRs.Rows(i).Item("descrizione"),
                                           DTRs.Rows(i).Item("codice")))

            Next

        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub TipoAttivita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Produzione vegetale", "PV"))
        Controllo.Items.Add(New ListItem("Produzione zootecnica", "PZ"))
        Controllo.Items.Add(New ListItem("Produzione vegetale e zootecnica", "PVZ"))
        Controllo.Items.Add(New ListItem("Preparazione vegetale", "TPV"))
        Controllo.Items.Add(New ListItem("Preparazione zootecnica", "TPZ"))
        Controllo.Items.Add(New ListItem("Preparazione vegetale e zootecnica", "TPVZ"))
        Controllo.Items.Add(New ListItem("Importazione", "I"))
        Controllo.Items.Add(New ListItem("Raccolta spontanea", "RS"))
        Controllo.Items.Add(New ListItem("Produzione / Preparazione", "P/TP"))
        Controllo.Items.Add(New ListItem("Preparazione / Importazione", "TP/I"))
        Controllo.Items.Add(New ListItem("Altro", "@"))

    End Sub

    '######################################################################
    Public Shared Sub CentroEsterno(ByRef CentroEsterno As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal PivaPadre As String,
                                ByVal Flag_SoloCentriAttivi As Boolean,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )
        Dim DT As DataTable
        Dim i As Integer
        Dim x_Cod As String = ""
        Dim x_Des As String
        'Pulisco il controllo
        CentroEsterno.Items.Clear()

        If PrimaRiga_Flag Then
            CentroEsterno.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Verifico se sono richiesti solo i centri ancora attivi
        If Flag_SoloCentriAttivi Then
            'funzione che imposta le date della finestra temporale con le date che servono 
            'per la lettura
            objParametri.ImpostaFinestre_con_SalvataggioTemporale(Date.Now, Date.Now)
        End If

        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        DT = objCentri.Leggi_CentriEsterni(PivaPadre,
                                            objParametri)

        'se avevo modificato la finestra temporale per la query
        'reimposto i valori iniziali
        If Flag_SoloCentriAttivi Then
            'funzione che reimposta i valori iniziali della finestra temporale
            objParametri.ResettaFinestra()
        End If

        If Not IsNothing(DT) Then

            For i = 0 To DT.Rows.Count - 1

                x_Cod = DT.Rows(i).Item("PIVA") & "_" & CStr(DT.Rows(i).Item("Sa_Cod"))

                x_Des = CStr(DT.Rows(i).Item("rag_soc") & " - " & DT.Rows(i).Item("Sa_Nome"))

                CentroEsterno.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        'Dt.Dispose()
        DT = Nothing

    End Sub
    '######################################################################

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub TitoloPossesso(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem(Gias.Altro, CStr(enum_TitoloPossesso.Altro)))
        Controllo.Items.Add(New ListItem(Gias.Proprieta, CStr(enum_TitoloPossesso.Proprieta)))
        Controllo.Items.Add(New ListItem(Gias.ComodatoUso, CStr(enum_TitoloPossesso.Comodato)))
        Controllo.Items.Add(New ListItem(Gias.AffittoConContratto, CStr(enum_TitoloPossesso.AffittoContratto)))
        Controllo.Items.Add(New ListItem(Gias.AffittoSenzaContratto, CStr(enum_TitoloPossesso.AffittoSenzaContratto)))
        Controllo.Items.Add(New ListItem(Gias.InContoTerzi, CStr(enum_TitoloPossesso.InContoTerzi)))
        Controllo.Items.Add(New ListItem(Gias.InConvenzione, CStr(enum_TitoloPossesso.InConvenzione)))
        Controllo.Items.Add(New ListItem(Gias.InCompartecipazione, CStr(enum_TitoloPossesso.InCompartecipazione)))

    End Sub
    Public Shared Sub NGGetProvincieAndComuni(ByRef Cmb_Provincia As DropDownList,
                                          ByRef Cmb_Comune As DropDownList,
                                          ByVal xPiva As String,
                                          ByVal xSa_Cod As Integer,
                                          ByRef objParametri_Server As AgronicaCoreParametri)
        Dim objIstatR As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim xSiglaProvincia As String = ""
        Dim DescProv As String = ""
        Dim Errore As String = ""

        Dim CodiceIstat_Provincia As String = "000"
        Dim CodiceIstat_Comune As String = "000"
        Dim Provincia_Sigla As String = ""

        Dim objCentrixIndirizziR As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

        Dim inserimentoNuovoCentro = xSa_Cod = 0
        If Not inserimentoNuovoCentro Then
            Call objCentrixIndirizziR.Recupera_Indirizzo_ElementiGerarchia(
                                        enum_GerarchiaImpresa_Elementi.Gerarchia_Centro,
                                        Provincia_Sigla,
                                        CodiceIstat_Provincia,
                                        CodiceIstat_Comune,
                                        Errore,
                                        xPiva,
                                        xSa_Cod,
                                        0, 0, objParametri_Server)
        End If

        If Not String.IsNullOrEmpty(CodiceIstat_Provincia) Then
            DescProv = objIstatR.Provincia_from_CodIstat(CodiceIstat_Provincia, xSiglaProvincia, objParametri_Server)
        End If

        Province_and_Comuni(Cmb_Provincia,
                            Cmb_Comune,
                            xSiglaProvincia,
                            CodiceIstat_Comune,
                            True,
                            1, objParametri_Server)
    End Sub
    Public Shared Sub NGStati(ByRef cmb_Stato As DropDownList, ByRef objParametri_Server As AgronicaCoreParametri)
        Dim DT_Nazioni As DataTable
        Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
        DT_Nazioni = objNazioni.Leggi("", "", "Descrizione", objParametri_Server)

        For i = 0 To DT_Nazioni.Rows.Count - 1
            cmb_Stato.Items.Add(New ListItem(DT_Nazioni.Rows(i).Item("Descrizione"), DT_Nazioni.Rows(i).Item("Codice")))
        Next

        ' setto Italia come default
        cmb_Stato.SelectedIndex = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue("IT"))

    End Sub
    Public Shared Sub NGGetStatiAndComuni(ByRef Cmb_Provincia As DropDownList,
                                          ByRef Cmb_Comune As DropDownList,
                                          ByVal xPiva As String,
                                          ByVal xSa_Cod As Integer,
                                          ByRef objParametri_Server As AgronicaCoreParametri)
        Dim objIstatR As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim xSiglaProvincia As String = ""
        Dim DescProv As String = ""
        Dim Errore As String = ""

        Dim CodiceIstat_Provincia As String = ""
        Dim CodiceIstat_Comune As String = ""
        Dim Provincia_Sigla As String = ""

        Dim objCentrixIndirizziR As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
        Call objCentrixIndirizziR.Recupera_Indirizzo_ElementiGerarchia(
                                        enum_GerarchiaImpresa_Elementi.Gerarchia_Centro,
                                        Provincia_Sigla,
                                        CodiceIstat_Provincia,
                                        CodiceIstat_Comune,
                                        Errore,
                                        xPiva,
                                        xSa_Cod,
                                        0, 0, objParametri_Server)

        If Not String.IsNullOrEmpty(CodiceIstat_Provincia) Then
            DescProv = objIstatR.Provincia_from_CodIstat(CodiceIstat_Provincia, xSiglaProvincia, objParametri_Server)
        End If

        Province_and_Comuni(Cmb_Provincia,
                            Cmb_Comune,
                            xSiglaProvincia,
                            CodiceIstat_Comune,
                            True,
                            1, objParametri_Server)
    End Sub

    '######################################################################
    Public Shared Sub MetodoProduzione(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Integrato", CStr(enum_MetodoProduzione.Integrato)))
        Controllo.Items.Add(New ListItem("In Conversione", CStr(enum_MetodoProduzione.InConversione)))
        Controllo.Items.Add(New ListItem("Biologico", CStr(enum_MetodoProduzione.Biologico)))

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub OrientamentoProduttivo_OLD(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                )

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Cerealicolo", "10"))
        Controllo.Items.Add(New ListItem("Orticolo", "20"))
        Controllo.Items.Add(New ListItem("Frutticolo", "30"))
        Controllo.Items.Add(New ListItem("Viticolo", "40"))
        Controllo.Items.Add(New ListItem("Olivicolo", "50"))
        Controllo.Items.Add(New ListItem("Floricolo - Vivaistico", "60"))
        Controllo.Items.Add(New ListItem("Colture industriali", "70"))
        Controllo.Items.Add(New ListItem("Foraggero", "80"))
        Controllo.Items.Add(New ListItem("Zootecnico", "90"))
        Controllo.Items.Add(New ListItem("Altro", "99"))

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[gunelli]	11/10/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub OrientamentoProduttivo(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                )

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objBio As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrientamentoProduttivo_R
        Dim DT As DataTable
        Dim i As Integer
        Dim Cod, Val As String

        DT = objBio.Leggi(0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1
                Cod = DT.Rows(i).Item("Orientamento_Cod")
                Val = DT.Rows(i).Item("Orientamento_Des")
                Controllo.Items.Add(New ListItem(Val, Cod))
            Next
        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub OTE(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )
        Dim DTRs As DataTable

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        'Dim objCOM As New AgronicaCoreAnagrafeDAL.OTE_Read
        Dim objCOM As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R

        'Leggo le imprese associate al profilo selezionato	
        DTRs = objCOM.Leggi("",
                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "", "OTE_ordine ASC", objParametri)

        objCOM = Nothing

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è vuoto allora ...	
        If DTRs IsNot Nothing AndAlso DTRs.Rows.Count > 0 Then
            Dim i As Integer
            For i = 0 To DTRs.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DTRs.Rows(i).Item("OTE_DES"),
                           DTRs.Rows(i).Item("OTE_COD")))

            Next
        End If

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Esposizione(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        ' Controllo.Items.Add(New ListItem("...", "..."))
        Controllo.Items.Add(New ListItem("nord", "nord"))
        Controllo.Items.Add(New ListItem("nord-est", "nord-est"))
        Controllo.Items.Add(New ListItem("est", "est"))
        Controllo.Items.Add(New ListItem("sud-est", "sud-est"))
        Controllo.Items.Add(New ListItem("sud", "sud"))
        Controllo.Items.Add(New ListItem("sud-ovest", "sud-ovest"))
        Controllo.Items.Add(New ListItem("ovest", "ovest"))
        Controllo.Items.Add(New ListItem("nord-ovest", "nord-ovest"))

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Ubicazione(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Controllo.Items.Add(New ListItem("...", "..."))
        Controllo.Items.Add(New ListItem("Pianura", "pianura"))
        Controllo.Items.Add(New ListItem("Mezza Costa", "mezza costa"))
        Controllo.Items.Add(New ListItem("Collina", "collina"))
        Controllo.Items.Add(New ListItem("Montagna", "montagna"))

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub QualitaCatasto(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.QualitaCatasto_R    'Agro_Anagrafe_AD.QualitaCatasto_R
        Dim DT As DataTable

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        DT = objCOM.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            "",
                            objParametri)

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è chiuso allora ...	
        If DT.Rows.Count > 0 Then

            'Inserisco i record trovati
            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Qualita_DES"),
                                           DT.Rows(i).Item("Qualita_COD")))

            Next
        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo Caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	31/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub GruppoColturale(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim i As Integer

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.GruppoColturale()"

        StbSQL.Length = 0

        StbSQL.Append("SELECT Grsp_Cod, Grsp_Des FROM GruppoColturale ")
        StbSQL.Append("ORDER BY Grsp_Des ")

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = objDataProvider.EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Elimino l'oggetto
        objDataProvider = Nothing

        'Se il datatable non è chiuso allora ...
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            '----- Riempio la combo con i dati del datatable

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Grsp_Des"),
                                           Dt.Rows(i).Item("Grsp_Cod")))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub

    Public Sub GruppoOperazioni(ByRef Controllo As ListControl,
                                   ByVal PrimaRiga_Flag As Boolean,
                                   ByVal PrimaRiga_Text As String,
                                   ByVal PrimaRiga_Value As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   )

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim i As Integer

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.GruppoColturale()"

        StbSQL.Length = 0

        StbSQL.Append("SELECT * from  GruppoOperazioni ")
        StbSQL.Append(" where 1=1 ")
        If xFiltroAggiuntivo <> "" Then
            StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
        End If
        If xOrderBy <> "" Then
            StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
        Else
            StbSQL.Append("ORDER BY GRU_DES ")
        End If


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Se il datatable non è chiuso allora ...
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            '----- Riempio la combo con i dati del datatable

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Dim Tipo As String = Dt.Rows(i).Item("Tipo")
                Dim des As String = Dt.Rows(i).Item("GRU_DES")
                Dim Gru_Cod As String = Dt.Rows(i).Item("GRU_COD")

                Controllo.Items.Add(New ListItem(des,
                                               Gru_Cod))
            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub

    'filtro solo quelle usate in agenda
    Public Sub GruppoOperazioni_MenuAgenda(ByRef Controllo As ListControl,
                                   ByVal PrimaRiga_Flag As Boolean,
                                   ByVal PrimaRiga_Text As String,
                                   ByVal PrimaRiga_Value As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   )

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim i As Integer

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.GruppoColturale()"

        StbSQL.Length = 0

        StbSQL.Append("SELECT * from  GruppoOperazioni ")
        StbSQL.Append(" where 1=1 ")

        'filtro solo quelle usate nel menu agenda
        StbSQL.Append("  and (tipo = 'C' or tipo='E' or tipo = 'P') ")



        If xFiltroAggiuntivo <> "" Then
            StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
        End If
        If xOrderBy <> "" Then
            StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
        Else
            StbSQL.Append("ORDER BY GRU_DES ")
        End If


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Se il datatable non è chiuso allora ...
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            '----- Riempio la combo con i dati del datatable

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Dim Tipo As String = Dt.Rows(i).Item("Tipo")
                Dim des As String = Dt.Rows(i).Item("GRU_DES")
                Dim Gru_Cod As String = Dt.Rows(i).Item("GRU_COD")

                If (Tipo = "E" AndAlso Gru_Cod <> "6") AndAlso (Tipo = "E" AndAlso Gru_Cod <> "10") Then
                    'non li aggiungo
                    'per ora non li uso in agenda
                    'aggiungo solo le contabili e registrazioni magazzino
                ElseIf (Tipo = "C" AndAlso Gru_Cod = "5") OrElse (Tipo = "C" AndAlso Gru_Cod = "100") Then
                    'non li aggiungo
                    'per ora non li uso in agenda
                    'Altre Operazioni Colturali(non gestite)
                    'Linee Produzione Vegetale
                Else

                    'cambio nome
                    If Gru_Cod = "10" Then
                        des = "Movimenti Magazzino"
                    End If


                    Controllo.Items.Add(New ListItem(des,
                                               Gru_Cod))

                End If




            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub


    Public Shared Sub TipoOperazioniColturali_MenuAgenda(ByRef Controllo As ListControl,
                           ByVal PrimaRiga_Flag As Boolean,
                           ByVal PrimaRiga_Text As String,
                           ByVal PrimaRiga_Value As String,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           )

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If
        'C
        'E
        'Z
        'P
        'V
        Controllo.Items.Add(New ListItem("Colturali", "C"))
        Controllo.Items.Add(New ListItem("Contabilità e Magazzino", "E"))
        Controllo.Items.Add(New ListItem("Parco Macchine", "P"))

        'per ora non li uso in agenda
        'Controllo.Items.Add(New ListItem("Zootecniche", "Z"))
        'Controllo.Items.Add(New ListItem("Visite Ispettive", "V"))

    End Sub


    Public Shared Sub Operazioni(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByVal Lav_Cod As Integer,
                               ByVal P As Integer,
                               ByVal Gru_Op As Integer,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.Operazioni()"

        Dim Dt As DataTable
        Dim i As Integer

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dim objOperazioni As New AgronicaCoreMetaSchemaDAL.Operazioni_R

        Dt = objOperazioni.Leggi(Lav_Cod, P, Gru_Op,
                                 "", 0, "", "",
                                 False, False, False, False,
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 xFiltroAggiuntivo,
                                 xOrderBy,
                                 objParametri)

        '----- Riempio il controllo con i dati del datatable

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("LAV_DES"),
                                           Dt.Rows(i).Item("LAV_COD")))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo Caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Grsp_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	01/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Avversita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Veg_Cod As Integer,
                                ByVal Grsp_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim DtGrsp As DataTable
        Dim Array_VegCod As Integer() = Nothing
        Dim i, j As Integer
        Dim objGruColt As New AgronicaCoreMetaSchemaDAL.GruppoColturaleXSpecieVegetali_R
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.Avversita()"

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If (Veg_Cod = 0) AndAlso (Grsp_Cod = 0) Then

            StbSQL.Length = 0

            StbSQL.Append("SELECT Av_Cod, Av_Des_Vol FROM Avversita ")
            StbSQL.Append("ORDER BY Av_Des_Vol ")

        ElseIf ((Veg_Cod <> 0) AndAlso (Grsp_Cod <> 0)) OrElse
               ((Veg_Cod <> 0) AndAlso (Grsp_Cod = 0)) Then

            StbSQL.Length = 0

            StbSQL.Append(" SELECT Avversita.Av_Des_Vol, Avversita.Av_Cod ")
            StbSQL.Append(" FROM Avversita INNER JOIN SpecieVegetalixAvversita ON Avversita.Av_Cod = SpecieVegetalixAvversita.Av_Cod ")
            StbSQL.Append(" WHERE SpecieVegetalixAvversita.Veg_Cod = " & Veg_Cod)
            StbSQL.Append(" ORDER BY Av_Des_Vol ")

        ElseIf (Veg_Cod = 0) AndAlso (Grsp_Cod <> 0) Then

            objGruColt.VegCod_from_GrspCod(Grsp_Cod, Array_VegCod, objParametri)

            If Array_VegCod IsNot Nothing Then

                For i = 0 To Array_VegCod.Length - 1

                    Veg_Cod = Array_VegCod(i)

                    StbSQL.Length = 0

                    StbSQL.Append(" SELECT Avversita.Av_Des_Vol, Avversita.Av_Cod ")
                    StbSQL.Append(" FROM Avversita INNER JOIN SpecieVegetalixAvversita ON Avversita.Av_Cod = SpecieVegetalixAvversita.Av_Cod ")
                    StbSQL.Append(" WHERE SpecieVegetalixAvversita.Veg_Cod = " & Veg_Cod)
                    StbSQL.Append(" AND (Av_Des_Vol NOT LIKE '%non usare%') AND (Av_Des_Vol NOT LIKE '%(#)%') AND (Av_Des_Lat NOT LIKE '%non usare%') AND (Av_Des_Lat NOT LIKE '%(#)%') ")

                    'Recupero il datatable
                    DtGrsp = objDataProvider.EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)


                    If DtGrsp IsNot Nothing AndAlso DtGrsp.Rows.Count > 0 Then

                        'Inserisco i record trovati
                        For j = 0 To DtGrsp.Rows.Count - 1

                            Controllo.Items.Add(New ListItem(DtGrsp.Rows(j).Item("Av_Des_Vol"),
                                                       DtGrsp.Rows(j).Item("Av_Cod")))


                        Next

                    End If

                    'Elimino il datatable
                    DtGrsp = Nothing

                Next

            End If

        End If

        'Recupero il datatable
        Dt = objDataProvider.EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Elimino l'oggetto
        objGruColt = Nothing
        objDataProvider = Nothing

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è chiuso allora ...	
        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Av_Des_Vol"),
                                            Dt.Rows(i).Item("Av_Cod")))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub

    Public Shared Sub Infestanti(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )

        Dim DT As DataTable
        Dim objInfestAttive As New AgronicaCoreMetaSchemaDAL.InfestantiAttive_R
        DT = objInfestAttive.Leggi(0, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                            xFiltroAggiuntivo, xOrderBy, objParametri)

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        If DT.Rows.Count > 0 Then
            Controllo.Items.Clear()
            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Av_Des_Vol"),
                                           DT.Rows(i).Item("Av_Cod")))
            Next
        End If

        'Elimino il datatable
        DT = Nothing

    End Sub


    Public Shared Sub GruppoInfestanti(ByRef Controllo As ListControl,
                           ByVal PrimaRiga_Flag As Boolean,
                           ByVal PrimaRiga_Text As String,
                           ByVal PrimaRiga_Value As String,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           )

        Dim DT As DataTable
        Dim objGruppoAttive As New AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R

        DT = objGruppoAttive.Leggi(0, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                             xFiltroAggiuntivo, xOrderBy, objParametri)

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        If DT.Rows.Count > 0 Then
            Controllo.Items.Clear()
            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Av_Gru_Des"),
                                           DT.Rows(i).Item("Av_Gru")))
            Next
        End If

        'Elimino il datatable
        DT = Nothing

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	01/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub GruppoVarietale(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.GruppoVarietale()"

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim i As Integer

        StbSQL.Length = 0

        StbSQL.Append("SELECT Grva_Cod, Grva_Des FROM GruppoVarietale ")
        StbSQL.Append("ORDER BY Grva_Des ")

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = objDataProvider.EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Elimino l'oggetto
        objDataProvider = Nothing

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è vuoto allora...	

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Grva_Des"),
                                           Dt.Rows(i).Item("Grva_Cod")))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub

    Public Shared Sub GruppoVarietalexSpecie(ByRef Controllo As ListControl,
                             ByVal PrimaRiga_Flag As Boolean,
                             ByVal PrimaRiga_Text As String,
                             ByVal PrimaRiga_Value As String,
                             ByVal Veg_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.GruppoVarietalexSpecie()"

        Dim Dt As DataTable
        Dim i As Integer

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dim objGrva As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R

        Dt = objGrva.Leggi(Veg_Cod,
                           0, "",
                           enumSelezioneVariabile.Selezione_JoinCompleta,
                           "", "",
                           objParametri)

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è vuoto allora...	

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Grva_Des"),
                                           Dt.Rows(i).Item("Grva_Cod")))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	01/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Epoche(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.Epoche()"

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim i As Integer

        StbSQL.Length = 0

        StbSQL.Append("SELECT Ep_Cod, Descrizione FROM Epoche ")
        StbSQL.Append("ORDER BY Descrizione ")

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = objDataProvider.EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Elimino l'oggetto
        objDataProvider = Nothing

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è vuoto allora ...	

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Descrizione"),
                                            Dt.Rows(i).Item("Ep_Cod")))


            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub




    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub EpocheModalitaxSpecie(ByRef Controllo As ListControl,
                                ByVal Veg_Cod As Integer,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.EpocheModalitaxSpecie()"

        Dim Dt As DataTable

        Dim i As Integer

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dim objEpoche As New AgronicaCoreMetaSchemaDAL.EpocheModalita_R

        Dt = objEpoche.LeggixSpecie(0, 0,
                                    Veg_Cod,
                                    "", "",
                                    objParametri)

        'Se il datatable non è vuoto allora ...	

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("EM_Des"),
                                            Dt.Rows(i).Item("EM_Cod")))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub








    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	01/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub FasiFenologiche(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.FasiFenologiche()"

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim i As Integer

        StbSQL.Length = 0

        StbSQL.Append("SELECT FF_Cod, FF_Des FROM FasiFenologiche ")
        StbSQL.Append("ORDER BY FF_Des ")

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = objDataProvider.EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Elimino l'oggetto
        objDataProvider = Nothing

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è vuoto allora ...	

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("FF_Des"),
                                            Dt.Rows(i).Item("FF_Cod")))


            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub

    Public Shared Sub FormeGiuridiche(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.FormeGiuridiche()"


        Dim Dt As DataTable
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim i As Integer

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objFormeGiuridiche As New AgronicaCoreMetaSchemaDAL.FormeGiuridiche_R

        Dt = objFormeGiuridiche.Leggi(0, "",
                                      AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, xOrderBy,
                                      objParametri)

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è vuoto allora ...	

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("FG_Des"),
                                            Dt.Rows(i).Item("FG_Cod")))


            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	01/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub ModalitaImpiego(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.ModalitaImpiego()"

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim i As Integer

        StbSQL.Length = 0

        StbSQL.Append("SELECT Mdi_Cod, Mdi_Des FROM ModalitaImpiego ")
        StbSQL.Append("ORDER BY Mdi_Des ")

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = objDataProvider.EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Elimino l'oggetto
        objDataProvider = Nothing

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è vuoto allora ...	

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Mdi_Des"),
                                            Dt.Rows(i).Item("Mdi_Cod")))

            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	01/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub UdmFormulati(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.UdmFormulati()"

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim i As Integer

        StbSQL.Length = 0

        StbSQL.Append(" SELECT UnitaMisura.UDM_SIM, UnitaMisura.UDM_Cod ")
        StbSQL.Append(" FROM UnitaMisura INNER JOIN MisuraXFormulati ON UnitaMisura.UDM_COD = MisuraXFormulati.Udm_Cod ")
        StbSQL.Append(" ORDER BY UnitaMisura.UDM_SIM ")

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = objDataProvider.EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Elimino l'oggetto
        objDataProvider = Nothing

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Udm_sim"),
                                            Dt.Rows(i).Item("Udm_cod")))

            Next

        End If

        'Elimino l'oggetto
        objDataProvider = Nothing

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	01/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub UdmAvversita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "AgronicaCoreUtility.CaricaListControl.UdmAvversita()"

        Dim StbSQL As New StringBuilder
        Dim Dt As DataTable
        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim i As Integer

        StbSQL.Length = 0

        StbSQL.Append(" SELECT DISTINCT UnitaMisura.UDM_SIM, UnitaMisura.UDM_Cod ")
        StbSQL.Append(" FROM MisuraxAvversita INNER JOIN UnitaMisura ON MisuraxAvversita.UDM_COD = UnitaMisura.UDM_COD ")
        StbSQL.Append(" ORDER BY UnitaMisura.UDM_SIM ")

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il datatable
        Dt = objDataProvider.EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)

        'Elimino l'oggetto
        objDataProvider = Nothing

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Udm_sim"),
                                            Dt.Rows(i).Item("Udm_cod")))

            Next

        End If

        'Elimino l'oggetto
        objDataProvider = Nothing

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo caricacombo.
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	01/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------


    '############################################################################
    Public Shared Sub Zone(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim DT As DataTable

        Dim objZone As New AgronicaCoreAnagrafeDAL.Zone_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = objZone.Leggi(0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                            "", "", objParametri)
        If DT.Rows.Count > 0 Then

            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Descrizione"),
                                           DT.Rows(i).Item("Zona_Cod")))
            Next

        End If

    End Sub


    '############################################################################
    Public Shared Sub AreaGIAS(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String
                                )

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Banchedati_DPI",
                                           enum_AreaGIAS.Banchedati_DPI))

        Controllo.Items.Add(New ListItem("Biologico",
                                           enum_AreaGIAS.Biologico))

        Controllo.Items.Add(New ListItem("Cantine",
                                           enum_AreaGIAS.Cantine))

        Controllo.Items.Add(New ListItem("CheckList",
                                           enum_AreaGIAS.CheckList))

        Controllo.Items.Add(New ListItem("Conferimento",
                                           enum_AreaGIAS.Conferimento))

        Controllo.Items.Add(New ListItem("Contabilita",
                                           enum_AreaGIAS.Contabilita))

        Controllo.Items.Add(New ListItem("ControlloDiGestione",
                                           enum_AreaGIAS.ControlloDiGestione))

        Controllo.Items.Add(New ListItem("Documentale",
                                           enum_AreaGIAS.Documentale))

        Controllo.Items.Add(New ListItem("DSS",
                                           enum_AreaGIAS.DSS))

        Controllo.Items.Add(New ListItem("FatturazioneElettronica",
                                           enum_AreaGIAS.FatturazioneElettronica))

        Controllo.Items.Add(New ListItem("FF",
                                           enum_AreaGIAS.FF))

        Controllo.Items.Add(New ListItem("ImportazioneFascicoli",
                                           enum_AreaGIAS.ImportazioneFascicoli))

        Controllo.Items.Add(New ListItem("GiasAPP",
                                           enum_AreaGIAS.GiasAPP))

        Controllo.Items.Add(New ListItem("GiasToGias",
                                           enum_AreaGIAS.GiasToGias))

        Controllo.Items.Add(New ListItem("GIS",
                                           enum_AreaGIAS.GIS))

        Controllo.Items.Add(New ListItem("IntegrazioneconSistemiEsterni",
                                           enum_AreaGIAS.IntegrazioneconSistemiEsterni))

        Controllo.Items.Add(New ListItem("IrriFrame",
                                           enum_AreaGIAS.IrriFrame))

        Controllo.Items.Add(New ListItem("Magazzino",
                                           enum_AreaGIAS.Magazzino))

        Controllo.Items.Add(New ListItem("ManagerFramework",
                                           enum_AreaGIAS.ManagerFramework))

        Controllo.Items.Add(New ListItem("ManagerCliente",
                                           enum_AreaGIAS.ManagerCliente))

        Controllo.Items.Add(New ListItem("NonConformità",
                                           enum_AreaGIAS.NonConformita))

        Controllo.Items.Add(New ListItem("PathFinder",
                                           enum_AreaGIAS.PathFinder))

        Controllo.Items.Add(New ListItem("PianiDiCampionamento",
                                           enum_AreaGIAS.PianiDiCampionamento))

        Controllo.Items.Add(New ListItem("PianiDiConcimazione",
                                           enum_AreaGIAS.PianiDiConcimazione))

        Controllo.Items.Add(New ListItem("PianiSemina",
                                           enum_AreaGIAS.PianiSemina))

        Controllo.Items.Add(New ListItem("Planning",
                                           enum_AreaGIAS.Planning))

        Controllo.Items.Add(New ListItem("Pratiche",
                                           enum_AreaGIAS.Pratiche))

        Controllo.Items.Add(New ListItem("PUA",
                                           enum_AreaGIAS.PUA))

        Controllo.Items.Add(New ListItem("QdC",
                                           enum_AreaGIAS.QdC))

        Controllo.Items.Add(New ListItem("Qualita",
                                           enum_AreaGIAS.Qualita))

        Controllo.Items.Add(New ListItem("Sian",
                                           enum_AreaGIAS.Sian))

        Controllo.Items.Add(New ListItem("Statistiche",
                                           enum_AreaGIAS.Statistiche))

        Controllo.Items.Add(New ListItem("Tracciabilita",
                                           enum_AreaGIAS.Tracciabilita))

        Controllo.Items.Add(New ListItem("Utenti",
                                           enum_AreaGIAS.Utenti))

        Controllo.Items.Add(New ListItem("Vinificazione",
                                           enum_AreaGIAS.Vinificazione))

        Controllo.Items.Add(New ListItem("Visite",
                                           enum_AreaGIAS.Visite))

        Controllo.Items.Add(New ListItem("Zoo",
                                           enum_AreaGIAS.Zoo))

        Controllo.Items.Add(New ListItem("ALTRE",
                                           enum_AreaGIAS.ALTRE))

    End Sub


    Public Shared Sub GestoreAzienda(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String
                                )

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Coldiretti",
                                           enum_GestoreAzienda.Coldiretti))

        Controllo.Items.Add(New ListItem("Confagricoltura",
                                           enum_GestoreAzienda.Confagricoltura))

        Controllo.Items.Add(New ListItem("C.I.A.",
                                           enum_GestoreAzienda.CIA))

        Controllo.Items.Add(New ListItem("Ordine degli Agronomi",
                                           enum_GestoreAzienda.Ordine_degli_Agronomi))

        Controllo.Items.Add(New ListItem("Perito_Agrario",
                                           enum_GestoreAzienda.Perito_Agrario))

        Controllo.Items.Add(New ListItem("Servizi_Agricoli_Europei",
                                           enum_GestoreAzienda.Servizi_Agricoli_Europei))

        Controllo.Items.Add(New ListItem("SOL. ECO",
                                           enum_GestoreAzienda.SOLECO))

        Controllo.Items.Add(New ListItem("Liberi Agricoltori",
                                           enum_GestoreAzienda.Liberi_Agricoltori))

        Controllo.Items.Add(New ListItem("Privato",
                                           enum_GestoreAzienda.Privato))

        Controllo.Items.Add(New ListItem("COP.AGRI",
                                           enum_GestoreAzienda.COPAGRI))

        Controllo.Items.Add(New ListItem("CAF AGRI",
                                           enum_GestoreAzienda.CAF_AGRI))

        Controllo.Items.Add(New ListItem("UNICAA",
                                           enum_GestoreAzienda.UNICAA))

    End Sub


    '######################################################################
    Public Shared Sub Cultivar(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Veg_Cod As Integer,
                                ByVal Cul_Cod As Integer,
                                ByVal Cerca_CulDes As String,
                                ByVal Flag_FiltroUtente As Boolean,
                                ByVal Cul_Cod_daModificare As Integer,
                                ByVal VegCod_Rif_CulCod_daModificare As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer
        Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False

        Dim objMatrice As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If Flag_FiltroUtente Then

            Dt = objMatrice.GestioneFiltroUtente_Leggi(Cul_Cod,
                                                Veg_Cod,
                                                Cerca_CulDes,
                                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                xFiltroAggiuntivo,
                                                xOrderBy,
                                                objParametri_Utenti)



        Else

            Dt = objMatrice.Leggi(Cul_Cod,
                                        Veg_Cod,
                                        Cerca_CulDes,
                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        xFiltroAggiuntivo,
                                        xOrderBy,
                                        objParametri_Server)

        End If


        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                'se sono in info o modifica, e quindi ho passato il Cul_Cod_daModificare
                'faccio il controllo
                If Cul_Cod_daModificare <> 0 Then

                    'se all'interno delle varietà filtrate, trovo il cul_cod da modificare
                    If Dt.Rows(i).Item("Cul_Cod") = Cul_Cod_daModificare Then
                        Flag_VegCodTrovatoNelFiltroUtente = True
                    End If

                Else
                    'non è stato passato il cul_cod
                    'quindi imposto il flag, come se avessi trovato la varietà
                    'così non devo aggiungere niente
                    Flag_VegCodTrovatoNelFiltroUtente = True
                End If

                x_Cod = Dt.Rows(i).Item("Cul_Cod")

                x_Des = CStr(Dt.Rows(i).Item("Cul_Des"))

                If InStr(LCase(x_Des), "altre") <> 0 Then
                    If PrimaRiga_Flag Then
                        Controllo.Items.Insert(1, New ListItem(x_Des, x_Cod))
                    Else
                        Controllo.Items.Insert(0, New ListItem(x_Des, x_Cod))
                    End If

                Else
                    Controllo.Items.Add(New ListItem(x_Des, x_Cod))
                End If

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        'se sono in info o modifica, e quindi ho passato il Cul_Cod_daModificare
        If Cul_Cod_daModificare <> 0 Then

            'se all'interno delle varietà filtrate, non è stato trovato il cul_cod da modificare
            If Not Flag_VegCodTrovatoNelFiltroUtente Then

                x_Des = ""

                'se il Cul_Cod_daModificare è valorizzato, devo sapere anche il veg_cod
                '(se non l'ho passato me lo ricavo e intanto che ci sono ricavo la descrizione della varietà)
                'però solo nel caso di veg_cod <> 0
                If Veg_Cod <> 0 AndAlso VegCod_Rif_CulCod_daModificare = 0 Then

                    objCultivar.VegCod_VegDes_CulDes_from_CulCod(Cul_Cod_daModificare,
                                                       VegCod_Rif_CulCod_daModificare,
                                                       "",
                                                       x_Des,
                                                       objParametri_Server)
                End If

                'se il veg_cod della varietà da modificare è lo stesso di quello selezionato,
                'allora inserisco la varietà da modificare nel menù
                If Veg_Cod = VegCod_Rif_CulCod_daModificare Then

                    'se la descrizione non l'ho già ricavata sopra
                    If x_Des = "" Then
                        objCultivar.CulDes_from_CulCod(Cul_Cod_daModificare, objParametri_Server)
                        'x_Des = CulDes_from_CulCod(objServer, objSession, objPage, Cul_Cod_daModificare)
                    End If

                    Controllo.Items.Add(New ListItem(x_Des, Cul_Cod_daModificare))

                End If

            End If

        End If

    End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' NON USATO
    ''' Creato da
    ''' http://localhost/GiasOnline/AA_Script/Moduli/CaricaCombo.vb CaricaCombo_Cooperative
    ''' Controllare LeggixPermessi se ci vuole un orderby dato che il recordset veniva ordinato:  Rs.Sort = "rag_soc asc" 
    ''' Controllare enumSelezioneVariabile
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	14/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Cooperative(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )
        Dim objCOM As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim Rs As DataTable

        'Leggo le imprese associate al profilo selezionato			
        Rs = objCOM.LeggixPermessi("", 2, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          xFiltroAggiuntivo,
                          xOrderBy,
                          objParametri)

        'Elimino gli oggetti
        objCOM = Nothing

        '----- Riempio  con i dati

        'Pulisco il controllo
        Controllo.Items.Clear()

        'Inserisco una riga vuota
        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If
        'Rs.Sort = "rag_soc asc"
        If Not IsNothing(Rs) Then
            Dim u As New AgronicaCoreAnagrafeDAL.Imprese_Read

            If Rs.Rows.Count <> 0 Then
                Dim i As Integer
                For i = 0 To Rs.Rows.Count - 1

                    'Al momento ometto la radice dell'albero
                    If Rs.Rows(i).Item("padre") <> "" Then

                        Controllo.Items.Add(New ListItem(u.RagSoc_from_Piva(Rs.Rows(i).Item("figlio"), objParametri), Rs.Rows(i).Item("figlio")))
                    End If

                Next

            End If

        End If
        Rs = Nothing

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' NON USATO
    ''' Creato a partire da
    ''' http://localhost/GiasOnline/AA_Script/Moduli/CaricaCombo.vb
    ''' CaricaCombo_UdmAusiliari
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Aus_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	14/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub UdmAusiliari(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal Aus_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )

        Dim Rs As DataTable
        Dim objUdm As New AgronicaCoreMetaSchemaDAL.MisuraxAusiliari_R  'objServer.createobjxxx("Agro_DB_AD.MisuraxAusiliari_R")


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Recupero il recordset
        Rs = objUdm.Leggi(CInt(Aus_Cod),
                          0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          xFiltroAggiuntivo,
                          xOrderBy,
                          objParametri)

        'Elimino l'oggetto
        objUdm = Nothing

        If Not IsNothing(Rs) Then
            'Dim u As AgronicaCoreAnagrafeDAL.Imprese_Read

            If Rs.Rows.Count <> 0 Then
                Dim i As Integer
                For i = 0 To Rs.Rows.Count - 1
                    Controllo.Items.Add(New ListItem(Rs.Rows(i).Item("Udm_sim"),
                                        Rs.Rows(i).Item("Udm_cod")))
                Next

            Else
                Controllo.Items.Add(New ListItem("Indefinito", -1))
            End If

        Else
            Controllo.Items.Add(New ListItem("Indefinito", -1))
        End If

    End Sub


    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Contratto_Cod"></param>
    ''' <param name="Fase_Cod"></param>
    ''' <param name="Cod_RisUm"></param>
    ''' <param name="Cod_Contatto"></param>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="Pro_Cod"></param>
    ''' <param name="Mat_Cod"></param>
    ''' <param name="Progetto_Cod"></param>
    ''' <param name="Udm_Cod"></param>
    ''' <param name="Flag_Contatto"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	19/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub OrdiniFasi(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal Contratto_Cod As Integer,
                                ByVal Fase_Cod As Integer,
                                ByVal Cod_RisUm As Integer,
                                ByVal Cod_Contatto As String,
                                ByVal Elem_Cod As Integer,
                                ByVal Pro_Cod As Integer,
                                ByVal Mat_Cod As Integer,
                                ByVal Progetto_Cod As Integer,
                                ByVal Udm_Cod As Integer,
                                ByVal Flag_Contatto As Boolean,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable = Nothing
        Dim i As Integer
        Dim objFasi As New AgronicaCoreContabDAL.Imprese_Contratto_Fasi_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Select Case Flag_Contatto

            Case True

                'RsFasi = ListaOrdiniFasi(objServer, objSession, objPage, _
                'Piva, , , Cod_RisUm, Cod_Contatto, Elem_Cod, Pro_Cod, Mat_Cod)

                Dt = objFasi.Leggi(Piva,
                                         0,
                                         0,
                                         Cod_RisUm,
                                         Cod_Contatto,
                                         Elem_Cod,
                                         Pro_Cod,
                                         Mat_Cod,
                                         0,
                                         0,
                                         "",
                                         0,
                                         enumSelezioneVariabile.Selezione_JoinCompleta,
                                         xFiltroAggiuntivo,
                                         xOrderBy,
                                         objParametri
                                        )

            Case False

                'RsFasi = ListaOrdiniFasi(objServer, objSession, objPage, _
                'Piva, , , , , Elem_Cod, Pro_Cod, Mat_Cod)

                Dt = objFasi.Leggi(Piva,
                         0,
                         0,
                         0,
                         "",
                         Elem_Cod,
                         Pro_Cod,
                         Mat_Cod,
                         0,
                         0,
                         "",
                         0,
                         enumSelezioneVariabile.Selezione_JoinCompleta,
                         xFiltroAggiuntivo,
                         xOrderBy,
                         objParametri)

        End Select

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                'Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Rag_Soc"), _
                'Dt.Rows(i).Item("PIVA")))

                If IsNothing(Controllo.Items.FindByValue(Dt.Rows(i).Item("Fase_Cod"))) Then

                    Controllo.Items.Add(New ListItem("Ordine Numero " & Dt.Rows(i).Item("Contratto_Numero") & " " & Dt.Rows(i).Item("Rag_Soc"),
                                               Dt.Rows(i).Item("Fase_Cod")))

                End If

            Next

        End If

    End Sub

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Cod"> default inserire ""</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	19/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub CaricaServizi(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Cod As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim Categorie_R As New AgronicaCoreMetaSchemaDAL.Categorie_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim Padre As String = "S000029"
        Dt = Categorie_R.Leggi(Cod,
                                Padre,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Rag_Soc"),
                                                  Dt.Rows(i).Item("PIVA")))
                Controllo.Items.Add(New ListItem(
                                                CStr(Dt.Rows(i).Item("Descr")),
                                                CLng(Right(
                                                            Dt.Rows(i).Item("Cod"), Len(Dt.Rows(i).Item("Cod")) - 1)
                                                            ))
                                                )

                '=========================================================================
                'Seleziono il prodotto dagli archivi aziendali eventualmente passato come parametro
                If Cod <> "" Then

                    Controllo.SelectedIndex = Controllo.Items.IndexOf(Controllo.Items.FindByValue(Cod))

                End If
                '==========================================================================

            Next

        End If

    End Sub

    Public Function LeggiServizi(ByVal Cod As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Dim Dt As DataTable
        Dim Categorie_R As New AgronicaCoreMetaSchemaDAL.Categorie_R

        Dim Padre As String = "S000029"
        Dt = Categorie_R.Leggi(Cod,
                               Padre,
                               xFiltroAggiuntivo,
                               xOrderBy,
                               objParametri)

        Return Dt

    End Function

    '######################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	22/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Finanziamenti(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreMetaSchemaDAL.Finanziamenti_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = obj.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Fin_DES"),
                                                  Dt.Rows(i).Item("Fin_COD")))

            Next

        End If

    End Sub




    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="Id_Report"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	22/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Agro_Reportistica(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Id_Report As Integer,
                                            ByVal Tipo As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        Dim Dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreMetaSchemaDAL.Agro_Reportistica_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = obj.Leggi(CInt(Id_Report),
                        Tipo,
                        xFiltroAggiuntivo,
                        xOrderBy,
                        objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Caption_Label"),
                                                  Dt.Rows(i).Item("Id_Report")))

            Next

        End If

    End Sub


    '###################################################################################
    Public Shared Sub Agro_Reportistica_Tipi(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Tipo As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        Dim Dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreMetaSchemaDAL.Agro_Reportistica_Tipi_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = obj.Leggi(Tipo,
                        xFiltroAggiuntivo,
                        xOrderBy,
                        objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Tipo_Des"),
                                                  Dt.Rows(i).Item("Tipo")))

            Next

        End If

    End Sub


    '###################################################################################
    Public Shared Sub CAC_Codifica_ProdottiAziendali_Tipi(ByRef Controllo As ListControl,
                                                            ByVal PrimaRiga_Flag As Boolean,
                                                            ByVal PrimaRiga_Text As String,
                                                            ByVal PrimaRiga_Value As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            )

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Select Case objParametri.PivaSuperUser

            Case "01788291209" 'APOCONERPO
                Controllo.Items.Add(New ListItem("Non definito", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito))
                Controllo.Items.Add(New ListItem("Agrintesa", enum_Tipo_CAC_Codifica_ProdottiAziendali.Agrintesa))

            Case "01352530396" 'AGRISOL
                Controllo.Items.Add(New ListItem("Agrisol_Seled", enum_Tipo_CAC_Codifica_ProdottiAziendali.Agrisol_Seled))

            Case "03129920363" 'FRUIT MODENA
                Controllo.Items.Add(New ListItem("FruitModena_Seled", enum_Tipo_CAC_Codifica_ProdottiAziendali.FruitModena_Seled))

            Case "00554760397" 'FRANCESCONI
                Controllo.Items.Add(New ListItem("Francesconi", enum_Tipo_CAC_Codifica_ProdottiAziendali.Francesconi))

            Case "00069880391" 'TERREMERSE
                Controllo.Items.Add(New ListItem("Terremerse", enum_Tipo_CAC_Codifica_ProdottiAziendali.Terremerse))

            Case "03297010401" 'CEREALI ROMAGNA
                Controllo.Items.Add(New ListItem("Cons. Agr. Ravenna", enum_Tipo_CAC_Codifica_ProdottiAziendali.ConsAgrRavenna))
                Controllo.Items.Add(New ListItem("Non definito", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito))

            Case "05644051004" 'COLDIRETTI NAZIONALE
                Controllo.Items.Add(New ListItem("Non definito", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito))
                Controllo.Items.Add(New ListItem("Cons. Agr. Perugia", enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_ConsAgrPerugia))
                Controllo.Items.Add(New ListItem("Regione Umbria", enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_RegioneUmbria))

            Case Else
                Controllo.Items.Add(New ListItem("Non definito", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito))

        End Select

    End Sub






    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	22/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Sub Layers(ByRef Controllo As ListControl,
                             ByVal PrimaRiga_Flag As Boolean,
                             ByVal PrimaRiga_Text As String,
                             ByVal PrimaRiga_Value As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             )

        Dim Dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreGraficaDAL.Layers_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = obj.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Layer_Des"),
                                                  Dt.Rows(i).Item("Cod_Layer")))

            Next

        End If

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Controllo"></param>
    ''' <param name="PrimaRiga_Flag"></param>
    ''' <param name="PrimaRiga_Text"></param>
    ''' <param name="PrimaRiga_Value"></param>
    ''' <param name="TestoRicerca"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	22/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    'Public Shared Sub Carica_Formulati(ByRef Controllo As ListControl, _
    '                        ByVal PrimaRiga_Flag As Boolean, _
    '                        ByVal PrimaRiga_Text As String, _
    '                        ByVal PrimaRiga_Value As String, _
    '                        ByVal TestoRicerca As String, _
    '                        ByVal FOR_VEG_COD As Int32, _
    '                        ByVal FR_COD As Integer, _
    '                        ByVal VEG_COD As Integer, _
    '                        ByVal CUL_COD As Integer, _
    '                        ByVal GRSP_COD As Integer, _
    '                        ByVal GRVA_COD As Integer, _
    '                        ByVal TipoRichiesto As Integer, _
    '                        ByVal xFiltroAggiuntivo As String, _
    '                        ByVal xOrderBy As String, _
    '                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                     )
    '    Dim LastFr_Des As String = ""

    '    Dim Dt As DataTable
    '    Dim i As Integer
    '    Dim obj As New AgronicaCoreMetaSchemaDAL.FormulatixSpecie_R

    '    'Pulisco il controllo
    '    Controllo.Items.Clear()

    '    If PrimaRiga_Flag Then
    '        Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
    '    End If

    '    Dt = obj.Leggi(TestoRicerca,
    '                         FOR_VEG_COD,
    '                         FR_COD,
    '                         VEG_COD,
    '                         CUL_COD,
    '                         GRSP_COD,
    '                         GRVA_COD,
    '                         0, 0, TipoRichiesto,
    '                         xFiltroAggiuntivo,
    '                         xOrderBy,
    '                         objParametri
    '                    )

    '    If Not IsNothing(Dt) Then

    '        If Dt.Rows.Count <> 0 Then

    '            For i = 0 To Dt.Rows.Count - 1

    '                'controllo che nn ci siano doppioni relativi alla classificazione...
    '                If CStr(Dt.Rows(i).Item("fr_des")) <> LastFr_Des Then

    '                    Controllo.Items.Add(New ListItem((CStr(Dt.Rows(i).Item("fr_des")) & "  (" & CStr(Dt.Rows(i).Item("fr_cod")) & ")"), _
    '                                                                                    CInt(Dt.Rows(i).Item("fr_cod"))))

    '                    LastFr_Des = CStr(Dt.Rows(i).Item("fr_des"))

    '                End If


    '            Next

    '        End If

    '    End If

    'End Sub


    '###############################################################################
    '''Usato
    '''Versione NEW_COM tipo core ma non standard
    '''spostare nel core: AgronicaCoreUtility
    Public Shared Sub CaricaCombo_Tecnici(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Piva As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         )

        Dim NumTotale As Integer
        Dim i As Integer
        Dim Rag_Soc As String
        Dim dt As DataTable


        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
        dt = objContattiR.Contatti_Contatto_Leggi(CStr(Piva),
                                                  "",
                                                  0,
                                                  -6,
                                                  True,
                                                  False,
                                                  0,
                                                  0,
                                                  False,
                                                  0,
                                                  0,
                                                  0,
                                                  "", True, 0, 0, 0, 0, 0,
                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                  xFiltroAggiuntivo, xOrderBy, objParametri)


        If Not IsNothing(dt) Then

            NumTotale = dt.Rows.Count

            For i = 0 To dt.Rows.Count - 1

                Rag_Soc = CStr(dt.Rows(i).Item("Rag_Soc")) &
                          CStr(dt.Rows(i).Item("Cognome")) & " " &
                          CStr(dt.Rows(i).Item("Nome"))

                Controllo.Items.Add(New ListItem(Rag_Soc, dt.Rows(i).Item("Cod_Contatto")))

            Next

        Else
            NumTotale = 0
        End If

        dt = Nothing


    End Sub



    '###############################################################################
    '''Usato
    '''Versione NEW_COM tipo core ma non standard
    '''spostare nel core: AgronicaCoreUtility
    Public Shared Sub CaricaCombo_Portinnesto(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Veg_Cod As Integer,
                                            ByVal Port_COD As Integer,
                                            ByVal Cerca_PortDes As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         )



        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer


        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objPortinnesto As New AgronicaCoreMetaSchemaDAL.PortinnestixSpecieVegetali_R

        Dt = objPortinnesto.Leggi(Veg_Cod, Port_COD, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri)

        'Dt = NewCom_Portinnesti_Leggi(objServer, objSession, objPage, _
        '                                    Port_COD, _
        '                                    Veg_Cod, _
        '                                    Cerca_PortDes, _
        '                                    FinestraTemp_Inizio, _
        '                                    FinestraTemp_Fine, _
        '                                    FiltroAggiuntivo, _
        '                                    Ordinamento)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Port_COD")

                x_Des = CStr(Dt.Rows(i).Item("Port_DES"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing



    End Sub


    '###############################################################################
    '''Usato
    '''Versione NEW_COM tipo core ma non standard
    '''spostare nel core: AgronicaCoreUtility
    Public Shared Sub CaricaCombo_FormaAllevamento(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Veg_Cod As Integer,
                                            ByVal FORAL_COD As Integer,
                                            ByVal Cerca_ForalDes As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         )



        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer


        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objFormeAllevamento As New AgronicaCoreMetaSchemaDAL.FormeAllevamentoxSpecieVegetali_R

        Dt = objFormeAllevamento.Leggi(Veg_Cod, FORAL_COD, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri)
        'Dt = NewCom_FormeAllevamento_Leggi(objServer, objSession, objPage, _
        '                                            FORAL_COD, _
        '                                            Veg_Cod, _
        '                                            Cerca_ForalDes, _
        '                                            FinestraTemp_Inizio, _
        '                                            FinestraTemp_Fine, _
        '                                            FiltroAggiuntivo, _
        '                                            Ordinamento)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Foral_COD")

                x_Des = CStr(Dt.Rows(i).Item("Foral_DES"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing


    End Sub

    '###############################################################################
    '''Usato
    '''Versione NEW_COM tipo core ma non standard
    '''spostare nel core: AgronicaCoreUtility
    Public Shared Sub CaricaCombo_ImpIrrigazione(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByVal Imp_COD As Integer,
                                            ByVal Cerca_ImpDes As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         )


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer


        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objIrrigazioni As New AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R
        Dt = objIrrigazioni.Leggi(Imp_COD,
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  xFiltroAggiuntivo,
                                  xOrderBy, objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Imp_COD")
                x_Des = CStr(Dt.Rows(i).Item("Imp_DES"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing


    End Sub


    '###############################################################################
    '''Usato
    '''Versione NEW_COM tipo core ma non standard
    '''spostare nel core: AgronicaCoreUtility
    Public Shared Sub CaricaCombo_ProvenienzaSeme(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String)


        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Biologica", "1"))
        Controllo.Items.Add(New ListItem("Convenzionale", "2"))
        Controllo.Items.Add(New ListItem("Deroga", "3"))


    End Sub


    '###############################################################################
    '''Usato
    '''Versione NEW_COM tipo core ma non standard
    '''spostare nel core: AgronicaCoreUtility
    Public Shared Sub CaricaCombo_SeminaTrapianto(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String)


        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Seminato", "Seminato"))
        Controllo.Items.Add(New ListItem("Trapiantato", "Trapiantato"))


    End Sub



    '###############################################################
    Public Shared Sub Ist_Credito(ByRef Controllo As ListControl,
                                    ByVal PrimaRiga_Flag As Boolean,
                                    ByVal PrimaRiga_Text As String,
                                    ByVal PrimaRiga_Value As String,
                                    ByVal Piva As String,
                                    ByVal Cod_Contatto As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    )

        Dim Dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreContabDAL.Ist_Credito_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = obj.LeggiDistinctIstCredito_BYCodContatto(Piva,
                                                        -1,
                                                        0, 0,
                                                        Cod_Contatto,
                                                        xFiltroAggiuntivo,
                                                        xOrderBy,
                                                        objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Istituto_Des"),
                                                  Dt.Rows(i).Item("Cod_Istituto")))

            Next

        End If

    End Sub

    '###############################################################
    Public Shared Sub RisorseFinanziarie(ByRef Controllo As ListControl,
                                    ByVal PrimaRiga_Flag As Boolean,
                                    ByVal PrimaRiga_Text As String,
                                    ByVal PrimaRiga_Value As String,
                                    ByVal Piva As String,
                                    ByVal Cod_Contatto As String,
                                    ByVal Cau_Risorsa As enum_Liquidita_CauRisorsa,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    )

        Dim Dt As DataTable
        Dim i As Integer
        Dim obj As New AgronicaCoreContabDAL.Liquidita_R
        Dim Codice As Integer
        Dim Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'ricorda che 0 ha significato, è la cassa
        Dt = obj.LeggiRisFinanziarie_BYCodContatto(Piva,
                                                    0, 0, 0,
                                                    Cod_Contatto,
                                                    Cau_Risorsa,
                                                    xFiltroAggiuntivo,
                                                    xOrderBy,
                                                    objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Codice = Dt.Rows(i).Item("Cod_Liquidita")

                Des = Dt.Rows(i).Item("Istituto_Des")

                'If DT.Rows(0).Item("Nazione") <> "" AndAlso DT.Rows(0).Item("Nazione") <> "0" Then
                If Dt.Rows(i).Item("Abi") <> "" AndAlso Dt.Rows(i).Item("Cab") <> "" AndAlso
                   Dt.Rows(i).Item("Abi") <> "0" AndAlso Dt.Rows(i).Item("Cab") <> "0" Then

                    Des &= " " & Dt.Rows(i).Item("Nazione") & " " _
                                    & Dt.Rows(i).Item("Cifre_Controllo") & " " _
                                    & Dt.Rows(i).Item("Cin") & " " _
                                    & Dt.Rows(i).Item("Abi") & " " _
                                    & Dt.Rows(i).Item("Cab") & " " _
                                    & Dt.Rows(i).Item("Numero") & " "

                ElseIf Dt.Rows(i).Item("Numero") <> "" Then
                    'aggiunto in data 08/02/2015 per gestione multicassa
                    'nel campo numero viene salvato il nome della cassa
                    Des &= " " & Dt.Rows(i).Item("Numero")
                End If

                Des = Trim(Des)

                If Dt.Rows(i).Item("Bic") <> "" AndAlso Dt.Rows(i).Item("Bic") <> "0" Then
                    Des &= " Bic/Swift: " & Dt.Rows(i).Item("Bic")
                End If
                Des = Trim(Des)

                Controllo.Items.Add(New ListItem(Des, Codice))

            Next

        End If

    End Sub



    '######################################################################
    Public Shared Sub Lista_Tipi_Allevamenti(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Regolamento_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.Lista_Tipi_Allevamenti_R
        Dim DTRs As DataTable

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        DTRs = objCOM.Leggi(0, 0, 0,
                            Regolamento_Cod,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        objCOM = Nothing



        '----- Riempio la combo con i dati del recordset

        'Se il datatable non è chiuso allora ...	
        If DTRs.Rows.Count > 0 Then

            Dim i As Integer

            'Inserisco i record trovati
            For i = 0 To DTRs.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DTRs.Rows(i).Item("ALL_DES"),
                                           DTRs.Rows(i).Item("ALL_COD")))

            Next

        End If

    End Sub


    ''' -----------------------------------------------------------------------------



    '###############################################################################################
    Public Shared Sub Cantina_Caratteristiche(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Int32,
                                                ByVal Piano_Cod As Int32,
                                                ByVal Validita_Inizio As Date,
                                                ByVal Validita_Fine As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                )

        Dim i As Integer
        Dim x_Cod, x_Des As String
        Dim dt As DataTable
        Dim obj As New AgronicaCoreAnagrafeDAL.Cantina_Caratter_R

        dt = obj.Leggi(Piva,
                        Sa_Cod,
                        Piano_Cod,
                        Validita_Inizio,
                        Validita_Fine,
                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                        xFiltroAggiuntivo,
                        xOrderBy,
                        objParametri)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        For i = 0 To dt.Rows.Count - 1

            x_Cod = CStr(dt.Rows(i).Item("Piano_Cod"))
            x_Des = CStr(dt.Rows(i).Item("Piano_Des"))

            Controllo.Items.Add(New ListItem(x_Des, x_Cod))

        Next

    End Sub

    '###############################################################################################
    Public Shared Sub Cantina_Vasche(ByRef Controllo As ListControl,
                                    ByVal PrimaRiga_Flag As Boolean,
                                    ByVal PrimaRiga_Text As String,
                                    ByVal PrimaRiga_Value As String,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Int32,
                                    ByVal Piano_Cod As Int32,
                                    ByVal Vas_Cod As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    )

        Dim i As Integer
        Dim x_Cod, x_Des As String
        Dim dt As DataTable
        Dim obj As New AgronicaCoreAnagrafeDAL.Cantina_Vasche_R

        dt = obj.LeggiJoinUdm(Piva,
                              Sa_Cod,
                              Vas_Cod,
                              Piano_Cod,
                              xFiltroAggiuntivo,
                              xOrderBy,
                              objParametri)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            For i = 0 To dt.Rows.Count - 1

                x_Cod = CStr(dt.Rows(i).Item("Vas_Cod"))
                x_Des = "Num. " & CStr(dt.Rows(i).Item("Identificativo"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

    End Sub

#Region "PianoConcimazione"
    Public Shared Sub Piani_Concimazione(ByRef Controllo As ListControl,
                                         ByVal PrimaRiga_Flag As Boolean,
                                         ByVal PrimaRiga_Text As String,
                                         ByVal PrimaRiga_Value As String,
                                         ByVal Piva As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         )


        Dim DT As DataTable
        Dim i As Integer

        Dim objPCTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = objPCTestata.DistinctTestate_conFiltroPiva(Piva,
                                                        xFiltroAggiuntivo, "",
                                                        objParametri)
        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("PC_Testata_Des"), DT.Rows(i).Item("PC_Testata_Cod")))

            Next
        End If

    End Sub

    Public Shared Sub PUA(ByRef Controllo As ListControl,
                          ByVal PrimaRiga_Flag As Boolean,
                          ByVal PrimaRiga_Text As String,
                          ByVal PrimaRiga_Value As String,
                          ByVal Piva As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          )


        Dim DT As DataTable
        Dim i As Integer

        Dim objPuaTestata As New AgronicaCorePUA_DAL.PUA_Testata_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = objPuaTestata.Leggi(0, 0, Piva,
                                 objParametri.FinestraTemporaleInizio, objParametri.FinestraTemporaleFine,
                                 xFiltroAggiuntivo, "",
                                 objParametri)

        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem("PUA-" & DT.Rows(i).Item("PUA_Anno") & " " & CDate(DT.Rows(i).Item("Validita_Inizio")).ToShortDateString,
                                                 DT.Rows(i).Item("PUA_Cod")))

            Next
        End If

    End Sub

    Public Shared Sub PUA_Regolamento(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal AggiungiTipoAlCod As Boolean = False
                                )

        Dim Dt As New DataTable
        Dim i As Integer = 0
        Dim x_cod As String
        Dim x_des As String

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R

        Dt = objReg.Leggi(0,
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          xFiltroAggiuntivo, xOrderBy,
                          objParametri)

        If Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                x_cod = Dt.Rows(i).Item("Regolamento_Cod")
                x_des = Dt.Rows(i).Item("Regolamento_DES")
                If AggiungiTipoAlCod Then
                    x_cod &= "|" & Dt.Rows(i).Item("Tipo")
                End If

                Controllo.Items.Add(New ListItem(x_des, x_cod))
            Next

        End If

    End Sub


    Public Shared Sub PUA_RegolamentoBS(ByRef Controllo As ListItemCollection,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        'Pulisco la combo
        Controllo.Clear()

        If PrimaRiga_Flag Then
            Controllo.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R

        'Recupero il recordset
        Dt = objReg.Leggi(0,
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          xFiltroAggiuntivo, xOrderBy,
                          objParametri)

        If Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1
                Controllo.Add(New ListItem(Dt.Rows(i).Item("Regolamento_DES"),
                           Dt.Rows(i).Item("Regolamento_Cod")))
            Next

        End If

    End Sub

    Public Shared Sub PUA_RegolamentoxAgenda(ByRef Controllo As ListControl,
                                             ByVal PrimaRiga_Flag As Boolean,
                                             ByVal PrimaRiga_Text As String,
                                             ByVal PrimaRiga_Value As String,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        'NON Pulisco la combo perché ci sono già 'Per tipologia e nessuno'
        ' Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R

        'Recupero il recordset
        Dt = objReg.Leggi(0,
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          xFiltroAggiuntivo, xOrderBy,
                          objParametri)

        If Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Regolamento_DES"),
                                    Dt.Rows(i).Item("Regolamento_Cod")))
            Next

        End If

    End Sub

    Public Shared Sub Finalita_Rer(ByRef Controllo As ListControl,
                                   ByVal PrimaRiga_Flag As Boolean,
                                   ByVal PrimaRiga_Text As String,
                                   ByVal PrimaRiga_Value As String,
                                   ByVal Regolamento_Cod As Integer,
                                   ByVal Veg_Cod As Integer,
                                   ByVal Grfi_Cod As Integer,
                                   ByVal Cerca_GrfiDes As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_Rer_R

        Dt = objGruppoFinalita.Leggi(Regolamento_Cod,
                                     Grfi_Cod,
                                     Veg_Cod,
                                     Cerca_GrfiDes,
                                     xFiltroAggiuntivo,
                                     xOrderBy,
                                     objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Grfi_COD")

                x_Des = CStr(Dt.Rows(i).Item("Grfi_DES"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    Public Shared Sub FattoriCorrettivi(ByRef Controllo As ListControl,
                                        ByVal PrimaRiga_Flag As Boolean,
                                        ByVal PrimaRiga_Text As String,
                                        ByVal PrimaRiga_Value As String,
                                        ByVal Regolamento_Cod As Integer,
                                        ByVal Fattore_Cod As Integer,
                                        ByVal Tipo As String,
                                        ByVal Variazione As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objFattoriCorrettivi As New AgronicaCoreMetaSchemaDAL.FattoriCorrettivi_R

        Dt = objFattoriCorrettivi.Leggi(Regolamento_Cod,
                                        Fattore_Cod,
                                        Tipo,
                                        Variazione,
                                        xFiltroAggiuntivo,
                                        xOrderBy,
                                        objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Fattore_Cod")

                x_Des = CStr(Dt.Rows(i).Item("Fattore_Des"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    Public Shared Sub SpecieConcimazione(ByRef Controllo As ListControl,
                                           ByVal PrimaRiga_Flag As Boolean,
                                           ByVal PrimaRiga_Text As String,
                                           ByVal PrimaRiga_Value As String,
                                           ByVal Veg_Cod As Integer,
                                           ByVal Gru_Cod As Integer,
                                           ByVal LetteraIniziale As String,
                                           ByVal StringaCerca As String,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieConcimazione_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objSpecie.Leggi(Veg_Cod,
                             Gru_Cod,
                             LetteraIniziale,
                             StringaCerca,
                                xFiltroAggiuntivo,
                                xOrderBy,
                                objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Veg_Des"),
                                                 Dt.Rows(i).Item("Veg_Cod")))

            Next

        End If

    End Sub

    Public Shared Sub PC_PrecessioneColturale(ByRef Controllo As ListControl,
                                              ByVal PrimaRiga_Flag As Boolean,
                                              ByVal PrimaRiga_Text As String,
                                              ByVal PrimaRiga_Value As String,
                                              ByVal Regolamento_Cod As Integer,
                                              ByVal Pua_Tipo As Integer,
                                              ByVal Pre_Cod As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objPrec As New AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturale_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objPrec.Leggi(Regolamento_Cod, Pua_Tipo,
                           Pre_Cod,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Pre_Des"),
                                                 Dt.Rows(i).Item("Pre_Cod")))

            Next

        End If

    End Sub

    Public Shared Sub PC_Ubicazione(ByRef Controllo As ListControl,
                                    ByVal PrimaRiga_Flag As Boolean,
                                    ByVal PrimaRiga_Text As String,
                                    ByVal PrimaRiga_Value As String,
                                    ByVal Regolamento_Cod As Integer,
                                    ByVal Ubicazione_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objUbicazione As New AgronicaCoreMetaSchemaDAL.PC_Ubicazione_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objUbicazione.Leggi(Regolamento_Cod,
                                 Ubicazione_Cod,
                                 xFiltroAggiuntivo,
                                 xOrderBy,
                                 objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Ubicazione_Des"),
                                                 Dt.Rows(i).Item("Ubicazione_Cod")))

            Next

        End If

    End Sub

    Public Shared Sub PC_FasiCicloColturale(ByRef Controllo As ListControl,
                                           ByVal PrimaRiga_Flag As Boolean,
                                           ByVal PrimaRiga_Text As String,
                                           ByVal PrimaRiga_Value As String,
                                           ByVal Regolamento_Cod As Int32,
                                           ByVal Veg_Cod As Int32,
                                           ByVal Id_Ciclo As Int32,
                                           ByVal Gru_Cod As Int32,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objFasi As New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturalexGruppoVegetale_R

        Dt = objFasi.Leggi(Regolamento_Cod,
                           Veg_Cod,
                           Id_Ciclo,
                           Gru_Cod,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Id_Fase")

                x_Des = CStr(Dt.Rows(i).Item("Fase_Des"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    Public Shared Sub PC_DisponibilitaOssigeno(ByRef Controllo As ListControl,
                                               ByVal PrimaRiga_Flag As Boolean,
                                               ByVal PrimaRiga_Text As String,
                                               ByVal PrimaRiga_Value As String,
                                               ByVal Regolamento_Cod As Int32,
                                               ByVal Id_Disp As Int32,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objFasi As New AgronicaCoreMetaSchemaDAL.PC_DisponibilitaOssigeno_R

        Dt = objFasi.Leggi(Regolamento_Cod,
                           Id_Disp,
                            xFiltroAggiuntivo,
                            xOrderBy,
                            objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Id_Disp")

                x_Des = CStr(Dt.Rows(i).Item("Descrizione"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    Public Shared Sub PC_Frequenza(ByRef Controllo As ListControl,
                                   ByVal PrimaRiga_Flag As Boolean,
                                   ByVal PrimaRiga_Text As String,
                                   ByVal PrimaRiga_Value As String,
                                   ByVal Regolamento_Cod As Int32,
                                   ByVal Id_Fre As Int32,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objPC_Frequenza As New AgronicaCoreMetaSchemaDAL.PC_Frequenza_R

        Dt = objPC_Frequenza.Leggi(Regolamento_Cod,
                                   Id_Fre,
                                   xFiltroAggiuntivo,
                                   xOrderBy,
                                   objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Id_Fre")

                x_Des = CStr(Dt.Rows(i).Item("Frequenza_Des"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    Public Shared Sub PC_MatriciOrganiche(ByRef Controllo As ListControl,
                                          ByVal PrimaRiga_Flag As Boolean,
                                          ByVal PrimaRiga_Text As String,
                                          ByVal PrimaRiga_Value As String,
                                          ByVal Regolamento_Cod As Int32,
                                          ByVal Id_Mat_O As Int32,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objPC_Frequenza As New AgronicaCoreMetaSchemaDAL.PC_MatriciOrganiche_R

        Dt = objPC_Frequenza.Leggi(Regolamento_Cod,
                                   Id_Mat_O,
                                   xFiltroAggiuntivo,
                                   xOrderBy,
                                   objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Id_Mat_O")

                x_Des = CStr(Dt.Rows(i).Item("Mat_O_Des"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    Public Shared Sub FasiClicloColturale(ByRef Controllo As ListControl,
                                  ByVal PrimaRiga_Flag As Boolean,
                                  ByVal PrimaRiga_Text As String,
                                  ByVal PrimaRiga_Value As String,
                                  ByVal Veg_Cod As Integer,
                                  ByVal Grfi_Cod As Integer,
                                  ByVal Regolamento_Cod As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  )


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

        Dt = objGruppoFinalita.FasiCicloColturale_Leggi(Veg_Cod,
                                                        Grfi_Cod,
                                                        Regolamento_Cod,
                                                        xFiltroAggiuntivo,
                                                        xOrderBy,
                                                        objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Grfi_COD")
                x_Des = CStr(Dt.Rows(i).Item("Grfi_DES"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    Public Shared Sub FasiClicloColturale(ByRef Controllo As ListItemCollection,
                                  ByVal PrimaRiga_Flag As Boolean,
                                  ByVal PrimaRiga_Text As String,
                                  ByVal PrimaRiga_Value As String,
                                  ByVal Veg_Cod As Integer,
                                  ByVal Grfi_Cod As Integer,
                                  ByVal Regolamento_Cod As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  )


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Clear()

        If PrimaRiga_Flag Then
            Controllo.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

        Dt = objGruppoFinalita.FasiCicloColturale_Leggi(Veg_Cod,
                                                        Grfi_Cod,
                                                        Regolamento_Cod,
                                                        xFiltroAggiuntivo,
                                                        xOrderBy,
                                                        objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Grfi_COD")
                x_Des = CStr(Dt.Rows(i).Item("Grfi_DES"))

                Controllo.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

#End Region


    '##################################################################
    Public Shared Sub MultiModificaImpianti_Proprieta(ByRef Controllo As ListControl,
                                                        ByVal PrimaRiga_Flag As Boolean,
                                                        ByVal PrimaRiga_Text As String,
                                                        ByVal PrimaRiga_Value As String,
                                                        ByVal Flag_0Tutti_1ImpUtente As Integer,
                                                        ByVal Flag_VisualizzaRigheSeparatori As Boolean,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        )


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim ModificaMultiplaSupImpianto As Boolean = False

        Try

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            ModificaMultiplaSupImpianto = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                    5,
                                                                    enum_Security_Attivita.Anagrafica_MultiModificaSupImp,
                                                                    enum_Security_Operazione.Modifica,
                                                                    Date.Now,
                                                                    "",
                                                                    objParametri_Utenti)
        Catch ex As Exception

        End Try

        Dim Flag_CaricaTutti As Boolean = False

        If Flag_0Tutti_1ImpUtente = 1 Then

            '----------------------------------
            '--------- FILTRO UTENTE ----------
            '----------------------------------

            Dim Des, Cod As String
            Dim Dt As DataTable
            Dim i As Integer
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R

            Dt = objImpost.Leggi(enum_Impostazioni_Utenti.UTENTE_MultiModificaImpianti_FiltroProprieta,
                                0,
                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                  "", "",
                                  objParametri_Utenti)

            If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

                For i = 0 To Dt.Rows.Count - 1

                    Cod = Dt.Rows(i).Item("ID_0")
                    Des = MultiModificaImpiantiProprieta_Des_from_Cod(Cod)

                    Controllo.Items.Add(New ListItem(Des, Cod))
                Next

            Else
                'non c'è il filtro utente impostato -> carico tutto
                Flag_CaricaTutti = True
            End If

        ElseIf Flag_0Tutti_1ImpUtente = 0 Then
            Flag_CaricaTutti = True
        End If

        If Flag_CaricaTutti Then

            '----------------------------------
            '------------ TUTTI ---------------
            '----------------------------------

            Controllo.Items.Add(New ListItem("Finalità Produttiva", enum_MultiModificaImpianti_Proprieta.Finalita))

            If ModificaMultiplaSupImpianto Then
                Controllo.Items.Add(New ListItem("Superficie Impianto", enum_MultiModificaImpianti_Proprieta.Sup_Impianto))
            End If
            If Flag_VisualizzaRigheSeparatori Then
                Controllo.Items.Add(New ListItem("", "-1"))
            End If
            Controllo.Items.Add(New ListItem("Resa Prevista", enum_MultiModificaImpianti_Proprieta.ResaPrevista))
            Controllo.Items.Add(New ListItem("Data Semina Prevista", enum_MultiModificaImpianti_Proprieta.DataSeminaPrevista))
            Controllo.Items.Add(New ListItem("Data Fioritura Prevista", enum_MultiModificaImpianti_Proprieta.DataFiorituraPrevista))
            Controllo.Items.Add(New ListItem("Data Raccolta Prevista", enum_MultiModificaImpianti_Proprieta.DataRaccoltaPrevista))
            If Flag_VisualizzaRigheSeparatori Then
                Controllo.Items.Add(New ListItem("", "-1"))
            End If
            Controllo.Items.Add(New ListItem("Regolamento", enum_MultiModificaImpianti_Proprieta.Regolamento))
            Controllo.Items.Add(New ListItem("Disciplinare", enum_MultiModificaImpianti_Proprieta.Disciplinare))
            Controllo.Items.Add(New ListItem("Regolamento Fertilizzazioni", enum_MultiModificaImpianti_Proprieta.Regolamento_Fertilizzazioni))
            Controllo.Items.Add(New ListItem("Stato Impianto", enum_MultiModificaImpianti_Proprieta.Stato_Impianto))
            Controllo.Items.Add(New ListItem("Massimo Apporto Azoto (N) in kg/ha", enum_MultiModificaImpianti_Proprieta.Pro_N))
            Controllo.Items.Add(New ListItem("Massimo Apporto Anidride Fosforosa (P2O5) in kg/ha", enum_MultiModificaImpianti_Proprieta.Pro_P2O5))
            Controllo.Items.Add(New ListItem("Massimo Apporto Potassio (K2O) in kg/ha", enum_MultiModificaImpianti_Proprieta.Pro_K2O))
            Controllo.Items.Add(New ListItem("Massimo Apporto Ossido di Magnesio (MgO) in kg/ha", enum_MultiModificaImpianti_Proprieta.Pro_MgO))
            If Flag_VisualizzaRigheSeparatori Then
                Controllo.Items.Add(New ListItem("", "-1"))
            End If
            Controllo.Items.Add(New ListItem("Capitolato Privato", enum_MultiModificaImpianti_Proprieta.Capitolato_Privato))
            Controllo.Items.Add(New ListItem("Certificazione", enum_MultiModificaImpianti_Proprieta.Certificazione))
            Controllo.Items.Add(New ListItem("Organismo Referente", enum_MultiModificaImpianti_Proprieta.Organismo_Referente))
            Controllo.Items.Add(New ListItem("Magazzino di Conferimento", enum_MultiModificaImpianti_Proprieta.Magazzino_Conferimento))
            If Flag_VisualizzaRigheSeparatori Then
                Controllo.Items.Add(New ListItem("", "-1"))
            End If

        End If

    End Sub



    '##################################################################
    Public Shared Sub OrganismiReferenti(ByRef Controllo As ListControl,
                                                        ByVal PrimaRiga_Flag As Boolean,
                                                        ByVal PrimaRiga_Text As String,
                                                        ByVal PrimaRiga_Value As String,
                                                        ByVal Piva As String,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        )


        'Pulisco il controllo

        Dim Dt_Padri As DataTable
        Dim i As Integer
        Dim objOR As New AgronicaCoreAnagrafeDAL.OrganismoReferente_Read
        'Dt_Padri = objOR.Leggi(Piva, "", "", objParametri_Server)
        Dt_Padri = objOR.Leggi(True, Piva, "", "", "", objParametri_Server)

        'Pulisco la combo
        Controllo.Items.Clear()
        'Inserisco una riga vuota
        Controllo.Items.Add(New ListItem("", ""))

        For i = 0 To Dt_Padri.Rows.Count - 1
            Controllo.Items.Add(New ListItem(Dt_Padri.Rows(i).Item("piva_padre") & " - " & Dt_Padri.Rows(i).Item("ragsoc_padre"),
                                        Dt_Padri.Rows(i).Item("piva_padre")))

        Next

    End Sub


    '##################################################################
    Public Shared Sub Attivita(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Id_Attivita As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objAttivita.Leggi(Id_Attivita,
                               xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For Each dr In Dt.Rows
                Dim testo As String = If(Not IsDBNull(dr.Item("Sigla")) AndAlso dr.Item("Sigla").trim <> "", dr.Item("Sigla") & " - " & dr.Item("Desc"), dr.Item("Desc"))
                Controllo.Items.Add(New ListItem(testo, dr.Item("Id_Attivita")))
            Next

        End If

    End Sub

    Shared Sub AttivitaXLavCod(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Lav_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Filtro per LavCod
        'xFiltroAggiuntivo &= " (A.filtro_operazioni_dettagli like '%|0|%' OR A.filtro_operazioni_dettagli like '%|" & Lav_Cod & "|%' ) "
        xFiltroAggiuntivo &= " A.filtro_operazioni = 'C' "

        Dt = objAttivita.Leggi(0, xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For Each dr In Dt.Rows
                Dim testo As String = If(Not IsDBNull(dr.Item("Sigla")) AndAlso dr.Item("Sigla").trim <> "", dr.Item("Sigla") & " - " & dr.Item("Desc"), dr.Item("Desc"))
                Controllo.Items.Add(New ListItem(testo, dr.Item("Id_Attivita")))
            Next

        End If

    End Sub

    '##################################################################
    Public Shared Sub AttivitaNonAssociate(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Id_Attivita As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim dt As DataTable
        Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        dt = objAttivita.AttivitaNonAssociate(Id_Attivita,
                                              xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then

            For Each dr In dt.Rows
                Dim testo As String = If(Not IsDBNull(dr.Item("Sigla")) AndAlso dr.Item("Sigla").trim <> "", dr.Item("Sigla") & " - " & dr.Item("Desc"), dr.Item("Desc"))
                Controllo.Items.Add(New ListItem(testo, dr.Item("Id_Attivita")))
            Next

        End If

    End Sub


    '##################################################################
    Public Shared Sub Turni(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal Turno_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri)

        Dim dt As DataTable
        Dim i As Integer
        Dim objTurni As New AgronicaCoreContabDAL.Turni_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        dt = objTurni.Leggi(Turno_Cod,
                            xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then

            For i = 0 To dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(dt.Rows(i).Item("Turno_Cod") & "-" & dt.Rows(i).Item("Turno_Des"),
                                                 dt.Rows(i).Item("Turno_Cod")))

            Next

        End If

    End Sub

    '##################################################################
    Public Shared Sub Qualifica(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Qualifica_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objQualifiche As New AgronicaCoreContabDAL.Qualifiche_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objQualifiche.Leggi(Qualifica_Cod,
                            xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Qualifica_Cod") & "-" & Dt.Rows(i).Item("Qualifica_Des"),
                                                 Dt.Rows(i).Item("Qualifica_Cod")))

            Next

        End If

    End Sub



    '##################################################################
    Public Shared Sub Tariffa(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Tariffa_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objTariffe As New AgronicaCoreContabDAL.Tariffe_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objTariffe.Leggi(Tariffa_Cod,
                            xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Tariffa_Cod") & "-" & Dt.Rows(i).Item("Tariffa_Des"),
                                                 Dt.Rows(i).Item("Tariffa_Cod")))

            Next

        End If

    End Sub


    Public Shared Sub Semilavorati_LottoAccettazione(ByRef cmb As ListControl,
                                               ByRef NumTotale As Integer,
                                               ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Id_Destinazione As Integer,
                                               ByVal Mat_Cod As Integer,
                                               ByVal Cod_Progetto As Integer,
                                               ByVal Flag_PrimaRiga As Boolean,
                                               ByVal Testo_PrimaRiga As String,
                                               ByVal Cod_PrimaRiga As String,
                                               ByVal RicercaLotto As String,
                                               ByVal Data_Giacenza As Date,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametriServer As AgronicaCoreParametri,
                                               ByRef objParametriUtenti As AgronicaCoreParametri)


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Des As String

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        Dt = objG.SchedaGiacenzeMagazzino(Data_Giacenza,
                                            Piva,
                                            Sa_Cod,
                                            Id_Destinazione,
                                            SEMILAVORATI_VEGETALI,
                                            0,
                                            Mat_Cod,
                                            0,
                                            Cod_Progetto,
                                            0,
                                            0,
                                            LOTTO_NONDEFINITO,
                                            False,
                                            "",
                                            "", "", "", "", "", "", "", "", "", "",
                                            "",
                                            objParametriServer, objParametriUtenti)

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Des = Dt.Rows(i).Item("Lotto")

                If Not IsDBNull(x_Des) AndAlso IsNothing(cmb.Items.FindByValue(x_Des)) Then

                    NumTotale += 1

                    cmb.Items.Add(New ListItem(x_Des, x_Des))

                End If

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    Public Shared Sub Sementi_LottoAccettazione(ByRef cmb As ListControl,
                                           ByRef NumTotale As Integer,
                                           ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Id_Destinazione As Integer,
                                           ByVal Mat_Cod As Integer,
                                           ByVal Flag_PrimaRiga As Boolean,
                                           ByVal Testo_PrimaRiga As String,
                                           ByVal Cod_PrimaRiga As String,
                                           ByVal RicercaLotto As String,
                                           ByVal Data_Giacenza As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametriServer As AgronicaCoreParametri,
                                           ByRef objParametriUtenti As AgronicaCoreParametri)


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Des As String

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        Dt = objG.SchedaGiacenzeMagazzino(Data_Giacenza,
                                            Piva,
                                            Sa_Cod,
                                            Id_Destinazione,
                                            SEMENTI,
                                            0,
                                            Mat_Cod,
                                            0,
                                            0,
                                            0,
                                            0,
                                            LOTTO_NONDEFINITO,
                                            False,
                                            "",
                                            "", "", "", "", "", "", "", "", "", "",
                                            "",
                                            objParametriServer, objParametriUtenti)

        If Not IsNothing(Dt) Then

            'NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Des = Dt.Rows(i).Item("Lotto")

                If Not IsDBNull(x_Des) AndAlso
                   IsNothing(cmb.Items.FindByValue(x_Des)) Then

                    NumTotale += 1

                    cmb.Items.Add(New ListItem(x_Des, x_Des))

                End If

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing
    End Sub

    Public Shared Sub Trasformati_LottoAccettazione(ByRef cmb As ListControl,
                                           ByRef NumTotale As Integer,
                                           ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Id_Destinazione As Integer,
                                           ByVal Mat_Cod As Integer,
                                           ByVal Flag_PrimaRiga As Boolean,
                                           ByVal Testo_PrimaRiga As String,
                                           ByVal Cod_PrimaRiga As String,
                                           ByVal RicercaLotto As String,
                                           ByVal Data_Giacenza As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametriServer As AgronicaCoreParametri,
                                           ByRef objParametriUtenti As AgronicaCoreParametri)


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Des As String

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        Dt = objG.SchedaGiacenzeMagazzino(Data_Giacenza,
                                            Piva,
                                            Sa_Cod,
                                            Id_Destinazione,
                                            TRASFORMATI_VEGETALI,
                                            0,
                                            Mat_Cod,
                                            0,
                                            0,
                                            0,
                                            0,
                                            LOTTO_NONDEFINITO,
                                            False,
                                            "",
                                            "", "", "", "", "", "", "", "", "", "",
                                            "",
                                            objParametriServer, objParametriUtenti)

        If Not IsNothing(Dt) Then

            'NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Des = Dt.Rows(i).Item("Lotto")

                If Not IsDBNull(x_Des) AndAlso IsNothing(cmb.Items.FindByValue(x_Des)) Then

                    NumTotale += 1

                    cmb.Items.Add(New ListItem(x_Des, x_Des))

                End If

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    'questa vale per tutte le categorie
    Public Shared Sub MateriePrime_LottoAccettazione(ByRef cmb As ListControl,
                                               ByRef NumTotale As Integer,
                                               ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Id_Destinazione As Integer,
                                               ByVal Elem_Cod As Integer,
                                               ByVal Pro_Cod As Integer,
                                               ByVal Mat_Cod As Integer,
                                               ByVal Cod_Progetto As Integer,
                                               ByVal Flag_PrimaRiga As Boolean,
                                               ByVal Testo_PrimaRiga As String,
                                               ByVal Cod_PrimaRiga As String,
                                               ByVal RicercaLotto As String,
                                               ByVal Data_Giacenza As Date,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametriServer As AgronicaCoreParametri,
                                               ByRef objParametriUtenti As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Des As String

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        Dt = objG.SchedaGiacenzeMagazzino(Data_Giacenza,
                                            Piva,
                                            Sa_Cod,
                                            Id_Destinazione,
                                            Elem_Cod,
                                            Pro_Cod,
                                            Mat_Cod,
                                            0,
                                            CODPROGETTO_NONDEFINITO,
                                            0,
                                            0,
                                            LOTTO_NONDEFINITO,
                                            False,
                                            "",
                                            "", "", "", "", "", "", "", "", "", "",
                                            "",
                                            objParametriServer, objParametriUtenti)

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Des = Dt.Rows(i).Item("Lotto")

                If Not IsDBNull(x_Des) AndAlso IsNothing(cmb.Items.FindByValue(x_Des)) Then

                    NumTotale += 1

                    cmb.Items.Add(New ListItem(x_Des, x_Des))

                End If

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    Public Shared Sub Semilavorati_LottoInterno(ByRef cmb As ListControl,
                                      ByRef NumTotale As Integer,
                                      ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Id_Destinazione As Integer,
                                      ByVal Mat_Cod As Integer,
                                      ByVal Flag_PrimaRiga As Boolean,
                                      ByVal Testo_PrimaRiga As String,
                                      ByVal Cod_PrimaRiga As String,
                                      ByVal RicercaLotto As String,
                                      ByVal Data_Giacenza As Date,
                                      ByVal Cau_Mov As String,
                                      ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametriServer As AgronicaCoreParametri,
                                        ByRef objParametriUtenti As AgronicaCoreParametri)

        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String = ""

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Select Case Cau_Mov

            Case CAU_SCARICO

                Dim objG As New AgronicaCoreContabDAL.Giacenze_R
                Dim Dt_Giacenze As DataTable
                Dt_Giacenze = objG.SchedaGiacenzeMagazzino(Data_Giacenza,
                                                    Piva,
                                                    Sa_Cod,
                                                    Id_Destinazione,
                                                    SEMILAVORATI_VEGETALI,
                                                    0,
                                                    Mat_Cod,
                                                    0,
                                                    CODPROGETTO_NONDEFINITO,
                                                    0,
                                                    0,
                                                    LOTTO_NONDEFINITO,
                                                    False,
                                                    "",
                                                    "", "", "", "", "", "", "", "", "", "",
                                                    "",
                                                    objParametriServer, objParametriUtenti)


                If Not IsNothing(Dt_Giacenze) Then

                    For i = 0 To Dt_Giacenze.Rows.Count - 1

                        x_Cod = Dt_Giacenze.Rows(i).Item("Cod_Progetto")

                        If Not IsDBNull(x_Cod) AndAlso IsNothing(cmb.Items.FindByValue(x_Cod)) Then

                            NumTotale += 1

                            If x_Cod <> 0 Then
                                'caso di scarico ---> i semilavorati sono stati caricati con un'operazione di raccolta
                                'e hanno il cod_progetto valorizzato, ovvero l'impianto
                                'x_Des = Dt.Rows(i).Item("Progetto_Nome")
                                Dim ArrayTmp As String() = Split(Dt_Giacenze.Rows(i).Item("Descrizione_Prodotto"), " - ")
                                If ArrayTmp IsNot Nothing AndAlso ArrayTmp.Length > 0 Then
                                    x_Des = ArrayTmp(3)
                                End If
                                'x_Des = Dt_Giacenze.Rows(i).Item("Descrizione_Prodotto")
                                cmb.Items.Add(New ListItem(x_Des, x_Cod))
                            Else
                                'caso di carico ---> i semilavorati provengono da terzi
                                'hanno il cod_progetto =0, ovvero nessun impianto
                                cmb.Items.Add(New ListItem("Da Terzi", 0))
                            End If

                        End If

                    Next

                Else
                    NumTotale = 0
                End If

            Case CAU_CARICO

                'caso di carico ---> i semilavorati provengono da terzi
                'hanno il cod_progetto =0, ovvero nessun impianto
                cmb.Items.Add(New ListItem("Da Terzi", 0))
                NumTotale = 1

        End Select
        'Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        'Dim Dt_Giacenze As DataTable
        'Dt_Giacenze = objG.SchedaGiacenzeMagazzino(Data_Giacenza, _
        '                                    Piva, _
        '                                    Sa_Cod, _
        '                                    Id_Destinazione, _
        '                                    SEMILAVORATI_VEGETALI, _
        '                                    0, _
        '                                    Mat_Cod, _
        '                                    0, _
        '                                    CODPROGETTO_NONDEFINITO, _
        '                                    0, _
        '                                    0, _
        '                                    LOTTO_NONDEFINITO, _
        '                                    False, _
        '                                    "", _
        '                                    "", "", "", "", "", "", "", "", "", "", _
        '                                    "", _
        '                                    objParametriServer)


        'If Not IsNothing(Dt) Then

        '    'NumTotale = Dt.Rows.Count

        '    For i = 0 To Dt.Rows.Count - 1

        '        x_Cod = Dt.Rows(i).Item("Cod_Progetto")

        '        If Not IsDBNull(x_Cod) Then

        '            If IsNothing(cmb.Items.FindByValue(x_Cod)) Then

        '                NumTotale += 1

        '                If x_Cod <> 0 Then
        '                    'caso di scarico ---> i semilavorati sono stati caricati con un'operazione di raccolta
        '                    'e hanno il cod_progetto valorizzato, ovvero l'impianto
        '                    'x_Des = Dt.Rows(i).Item("Progetto_Nome")
        '                    x_Des = Dt.Rows(i).Item("Descrizione_Prodotto")
        '                    cmb.Items.Add(New ListItem(x_Des, x_Cod))
        '                Else
        '                    'caso di carico ---> i semilavorati provengono da terzi
        '                    'hanno il cod_progetto =0, ovvero nessun impianto
        '                    cmb.Items.Add(New ListItem("Da Terzi", 0))
        '                End If

        '            End If

        '        End If

        '    Next

        'Else
        '    NumTotale = 0
        'End If


        'Dt.Dispose()
        'Dt = Nothing



    End Sub

    Public Shared Sub Semilavorati_ParametriQualitativi(ByRef cmb As ListControl,
                                            ByRef NumTotale As Integer,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Id_Destinazione As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Cod_Progetto As Integer,
                                            ByVal Lotto As String,
                                            ByVal Flag_PrimaRiga As Boolean,
                                            ByVal Testo_PrimaRiga As String,
                                            ByVal Cod_PrimaRiga As String,
                                            ByVal Data_Giacenza As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametriServer As AgronicaCoreParametri,
                                            ByRef objParametriUtenti As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String = ""

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        'Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        ''mi serve Materie_Prime_Campionature
        'Dt = objMP.MateriePrime_Giacenze(Piva, _
        '                                       Sa_Cod, _
        '                                       SEMILAVORATI_VEGETALI, _
        '                                       0, _
        '                                       Mat_Cod, _
        '                                       0, _
        '                                       Id_Destinazione, _
        '                                       0, _
        '                                       Cod_Progetto, _
        '                                       0, _
        '                                       Lotto, _
        '                                       "", _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       "", _
        '                                        "", _
        '                                       "", 0, "", False, "", "", objParametriServer)


        Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        Dt = objG.SchedaGiacenzeMagazzino(Data_Giacenza,
                                            Piva,
                                            Sa_Cod,
                                            Id_Destinazione,
                                            SEMILAVORATI_VEGETALI,
                                            0,
                                            Mat_Cod,
                                            0,
                                            Cod_Progetto,
                                            0,
                                            0,
                                            Lotto,
                                            False,
                                            "",
                                            "", "", "", "", "", "", "", "", "", "",
                                            "",
                                            objParametriServer, objParametriUtenti)


        If Not IsNothing(Dt) Then

            'NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Cal_Cod")

                If Not IsDBNull(x_Cod) AndAlso
                   IsNothing(cmb.Items.FindByValue(x_Cod)) Then

                    NumTotale += 1

                    Select Case x_Cod

                        Case Is > 0
                            'x_Des = CalDes_from_CalCod(objServer, objSession, objPage, x_Cod)
                            'x_Des = Dt.Rows(i).Item("Cal_Des")
                            Dim ArrayTmp As String() = Split(Dt.Rows(i).Item("Descrizione_Prodotto"), " - ")
                            If ArrayTmp IsNot Nothing AndAlso ArrayTmp.Length > 0 Then
                                x_Des = ArrayTmp(2)
                            End If
                                'x_Des = Dt.Rows(i).Item("Descrizione_Prodotto")

                        Case Is < 0
                            'x_Des = Dt.Rows(i).Item("Descrizione")
                            Dim ArrayTmp As String() = Split(Dt.Rows(i).Item("Descrizione_Prodotto"), " - ")
                            If ArrayTmp IsNot Nothing AndAlso ArrayTmp.Length > 0 Then
                                x_Des = ArrayTmp(2)
                            End If
                                'x_Des = Dt.Rows(i).Item("Descrizione_Prodotto")
                        Case Is = 0
                            'caso di carico ---> i semilavorati provengono da terzi
                            'hanno il cod_progetto =0, ovvero nessun impianto
                            'e non hanno nessun calibro
                            x_Des = "Indefinito"

                    End Select

                    cmb.Items.Add(New ListItem(x_Des, x_Cod))

                End If

            Next

        Else
            NumTotale = 0
        End If


        'Dt.Dispose()
        Dt = Nothing


    End Sub

    Public Shared Sub Trasformati_ParametriQualitativi(ByRef cmb As ListControl,
                                        ByRef NumTotale As Integer,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Id_Destinazione As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Lotto As String,
                                        ByVal Flag_PrimaRiga As Boolean,
                                        ByVal Testo_PrimaRiga As String,
                                        ByVal Cod_PrimaRiga As String,
                                        ByVal Data_Giacenza As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametriServer As AgronicaCoreParametri,
                                        ByRef objParametriUtenti As AgronicaCoreParametri)

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String = ""

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        Dt = objG.SchedaGiacenzeMagazzino(Data_Giacenza,
                                            Piva,
                                            Sa_Cod,
                                            Id_Destinazione,
                                            TRASFORMATI_VEGETALI,
                                            0,
                                            Mat_Cod,
                                            0,
                                            0,
                                            0,
                                            0,
                                            Lotto,
                                            False,
                                            "",
                                            "", "", "", "", "", "", "", "", "", "",
                                            "",
                                            objParametriServer, objParametriUtenti)


        If Not IsNothing(Dt) Then

            'NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Cal_Cod")

                If Not IsDBNull(x_Cod) AndAlso IsNothing(cmb.Items.FindByValue(x_Cod)) Then

                    NumTotale += 1

                    Select Case x_Cod

                        Case Is > 0
                            'x_Des = CalDes_from_CalCod(objServer, objSession, objPage, x_Cod)
                            'x_Des = Dt.Rows(i).Item("Cal_Des")
                            Dim ArrayTmp As String() = Split(Dt.Rows(i).Item("Descrizione_Prodotto"), " - ")
                            If ArrayTmp IsNot Nothing AndAlso ArrayTmp.Length > 0 Then
                                x_Des = ArrayTmp(2)
                            End If
                                'x_Des = Dt.Rows(i).Item("Descrizione_Prodotto")

                        Case Is < 0
                            'x_Des = Dt.Rows(i).Item("Descrizione")
                            Dim ArrayTmp As String() = Split(Dt.Rows(i).Item("Descrizione_Prodotto"), " - ")
                            If ArrayTmp IsNot Nothing AndAlso ArrayTmp.Length > 0 Then
                                Select Case ArrayTmp.Length
                                    Case 2
                                        x_Des = ArrayTmp(1)
                                    Case 3
                                        x_Des = ArrayTmp(2)
                                    Case Else
                                        x_Des = ArrayTmp(1)
                                End Select

                            End If
                                'x_Des = Dt.Rows(i).Item("Descrizione_Prodotto")
                        Case Is = 0
                            'caso di carico ---> i semilavorati provengono da terzi
                            'hanno il cod_progetto =0, ovvero nessun impianto
                            'e non hanno nessun calibro
                            x_Des = "Indefinito"

                    End Select

                    cmb.Items.Add(New ListItem(x_Des, x_Cod))

                End If

            Next

        Else
            NumTotale = 0
        End If


        'Dt.Dispose()
        Dt = Nothing


    End Sub

    Public Shared Sub Semilavorati_Udm(ByRef cmb As ListControl,
                                       ByRef NumTotale As Integer,
                                       ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Id_Destinazione As Integer,
                                       ByVal Mat_Cod As Integer,
                                       ByVal Cod_Progetto As Integer,
                                       ByVal Lotto As String,
                                       ByVal Cal_Cod As Integer,
                                       ByVal Flag_SoloSimbolo As Boolean,
                                            ByVal Flag_PrimaRiga As Boolean,
                                            ByVal Testo_PrimaRiga As String,
                                            ByVal Cod_PrimaRiga As String,
                                            ByVal Data_Giacenza As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametriServer As AgronicaCoreParametri,
                                            ByRef objParametriUtenti As AgronicaCoreParametri)


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If


        '''mi serve l'unità di misura
        'Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        'Dt = objMP.MateriePrime_Giacenze(Piva, _
        '                                       Sa_Cod, _
        '                                       SEMILAVORATI_VEGETALI, _
        '                                       0, _
        '                                       Mat_Cod, _
        '                                       0, _
        '                                       Id_Destinazione, _
        '                                       Cal_Cod, _
        '                                       Cod_Progetto, _
        '                                       0, _
        '                                       Lotto, _
        '                                       "", _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       "", _
        '                                        "", _
        '                                       "", 0, "", False, "", "", objParametriServer)

        Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        Dt = objG.SchedaGiacenzeMagazzino(Data_Giacenza,
                                            Piva,
                                            Sa_Cod,
                                            Id_Destinazione,
                                            SEMILAVORATI_VEGETALI,
                                            0,
                                            Mat_Cod,
                                            Cal_Cod,
                                            Cod_Progetto,
                                            0,
                                            0,
                                            Lotto,
                                            False,
                                            "",
                                            "", "", "", "", "", "", "", "", "", "",
                                            "",
                                            objParametriServer, objParametriUtenti)
        If Not IsNothing(Dt) Then

            'NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Udm_Cod")

                If Flag_SoloSimbolo Then
                    x_Des = Dt.Rows(i).Item("Udm_Sim")
                Else
                    x_Des = Dt.Rows(i).Item("Udm_Des")
                End If

                If Not IsDBNull(x_Cod) AndAlso IsNothing(cmb.Items.FindByValue(x_Cod)) Then

                    NumTotale += 1

                    cmb.Items.Add(New ListItem(x_Des, x_Cod))

                End If

            Next

        Else
            NumTotale = 0
        End If


        'Dt.Dispose()
        Dt = Nothing


    End Sub








    Public Shared Sub Udm_Optimize(ByRef cmb As ListControl,
                                    ByRef NumTotale As Integer,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Id_Destinazione As Integer,
                                    ByVal Cau_Mov As String,
                                    ByVal Elem_Cod As Integer,
                                    ByVal Flag_ProCod_Negativo As Boolean,
                                    ByVal Pro_Cod As Integer,
                                    ByVal Mat_Cod As Integer,
                                    ByVal Flag_PrimaRiga As Boolean,
                                    ByVal Testo_PrimaRiga As String,
                                    ByVal Cod_PrimaRiga As String,
                                    ByVal RicercaTesto As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal isFreshAndFood As Boolean = False,
                                            Optional ByVal Flag_QtaNoZero As Boolean = False,
                                            Optional ByVal Flag_QtaMaggioreZero As Boolean = False)


        Dim Dt As DataTable = Nothing
        Dim i As Integer
        Dim x_ProCod As Integer
        Dim x_MatCod As Integer

        Dim x_Cod As Integer
        Dim x_Des As String

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Select Case Cau_Mov

            Case CAU_CARICO

                '#############################################
                '##############     CARICO    ################
                '#############################################

                'CategorieXUnitaMisura_R
                'mi serve l'unità di misura
                'Dim objMP As New AgronicaCoreAnagrafeDAL.cate
                Dt = New AgronicaCoreMetaSchemaDAL.CategorieXUnitaMisura_R().Leggi(
                                                                Elem_Cod,
                                                                0,
                                                                Cau_Mov,
                                                                False, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "",
                                                                objParametriServer)


            Case CAU_SCARICO

                '#############################################
                '##############     SCARICO    ###############
                '#############################################

                'leggo le unità di misura in magazzino

                Select Case Flag_ProCod_Negativo
                    Case True
                        If Pro_Cod < 0 Then
                            x_MatCod = -Pro_Cod
                            x_ProCod = 0
                        Else
                            x_ProCod = Pro_Cod
                            x_MatCod = 0
                        End If
                    Case False
                        x_ProCod = Pro_Cod
                        x_MatCod = Mat_Cod
                End Select


                Dt = New AgronicaCoreContabDAL.Giacenze_R().SchedaGiacenzeMagazzino(
                                            AGRODATAFINE,
                                            Piva,
                                            Sa_Cod,
                                            Id_Destinazione,
                                             Elem_Cod,
                                             x_ProCod,
                                             x_MatCod,
                                              0, 0, 0, 0,
                                             LOTTO_NONDEFINITO, Flag_QtaNoZero,
                                             "", "", "", "", "", "", "", "", "", "", "", "",
                                            objParametriServer, objParametriUtenti, isFreshAndFood:=isFreshAndFood, Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero)


        End Select


        If Not IsNothing(Dt) Then

            'NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Udm_Cod")
                x_Des = CStr(Dt.Rows(i).Item("Udm_Des"))

                If Not IsDBNull(x_Cod) AndAlso IsNothing(cmb.Items.FindByValue(x_Cod)) Then

                    NumTotale += 1

                    cmb.Items.Add(New ListItem(x_Des, x_Cod))

                End If


            Next

        Else
            NumTotale = 0
        End If


        'Dt.Dispose()
        Dt = Nothing


    End Sub

    '###################################################################
    '06/09/2018: uguale alla 1 ma tiene in considerazione il lotto nella lettura della giacenza
    'la 1 quindi restituisce le unità di misura della giacenza del prodotto indipendentemente dal lotto (errore)
    'se il lotto non si vuole filtrare si manda LOTTO_NONDEFINITO
    Public Shared Sub Udm_Optimize2(ByRef cmb As ListControl,
                                    ByRef NumTotale As Integer,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Id_Destinazione As Integer,
                                    ByVal Cau_Mov As String,
                                    ByVal Elem_Cod As Integer,
                                    ByVal Flag_ProCod_Negativo As Boolean,
                                    ByVal Pro_Cod As Integer,
                                    ByVal Mat_Cod As Integer,
                                    ByVal Lotto As String,
                                    ByVal Flag_PrimaRiga As Boolean,
                                    ByVal Testo_PrimaRiga As String,
                                    ByVal Cod_PrimaRiga As String,
                                    ByVal RicercaTesto As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal isFreshAndFood As Boolean = False,
                                            Optional ByVal Flag_QtaNoZero As Boolean = False,
                                            Optional ByVal Flag_QtaMaggioreZero As Boolean = False)


        Dim Dt As DataTable = Nothing
        Dim i As Integer
        Dim x_ProCod As Integer
        Dim x_MatCod As Integer

        Dim x_Cod As Integer
        Dim x_Des As String

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Select Case Cau_Mov

            Case CAU_CARICO

                '#############################################
                '##############     CARICO    ################
                '#############################################

                'CategorieXUnitaMisura_R
                'mi serve l'unità di misura
                'Dim objMP As New AgronicaCoreAnagrafeDAL.cate
                'Dt = objMP.NewCom_CategorieMagazzinoxUnitaMisura_Leggi(objServer, _
                '                                                objSession, _
                '                                                objPage, _
                '                                                Elem_Cod, _
                '                                                0, _
                '                                                Cau_Mov, _
                '                                                "", _
                '                                                False)

                Dt = New AgronicaCoreMetaSchemaDAL.CategorieXUnitaMisura_R().Leggi(
                                                                Elem_Cod,
                                                                0,
                                                                Cau_Mov,
                                                                False, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametriServer)


            Case CAU_SCARICO

                '#############################################
                '##############     SCARICO    ###############
                '#############################################

                'leggo le unità di misura in magazzino

                Select Case Flag_ProCod_Negativo
                    Case True
                        If Pro_Cod < 0 Then
                            x_MatCod = -Pro_Cod
                            x_ProCod = 0
                        Else
                            x_ProCod = Pro_Cod
                            x_MatCod = 0
                        End If
                    Case False
                        x_ProCod = Pro_Cod
                        x_MatCod = Mat_Cod
                End Select


                Dt = New AgronicaCoreContabDAL.Giacenze_R().SchedaGiacenzeMagazzino(
                                            AGRODATAFINE,
                                            Piva,
                                            Sa_Cod,
                                            Id_Destinazione,
                                             Elem_Cod,
                                             x_ProCod,
                                             x_MatCod,
                                              0, 0, 0, 0,
                                             Lotto, Flag_QtaNoZero,
                                             "", "", "", "", "", "", "", "", "", "", "", "",
                                            objParametriServer, objParametriUtenti, isFreshAndFood:=isFreshAndFood, Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero)


        End Select


        If Not IsNothing(Dt) Then

            'NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Udm_Cod")
                x_Des = CStr(Dt.Rows(i).Item("Udm_Des"))

                If Not IsDBNull(x_Cod) AndAlso IsNothing(cmb.Items.FindByValue(x_Cod)) Then

                    NumTotale += 1

                    cmb.Items.Add(New ListItem(x_Des, x_Cod))

                End If

            Next

        Else
            NumTotale = 0
        End If


        'Dt.Dispose()
        Dt = Nothing


    End Sub

    Public Shared Sub Udm_Optimize_Regolamento(ByRef cmb As ListControl,
                                              ByRef NumTotale As Integer,
                                              ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Id_Destinazione As Integer,
                                              ByVal Cau_Mov As String,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Flag_ProCod_Negativo As Boolean,
                                              ByVal Pro_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Flag_PrimaRiga As Boolean,
                                              ByVal Testo_PrimaRiga As String,
                                              ByVal Cod_PrimaRiga As String,
                                              ByVal RicercaTesto As String,
                                              ByVal Regolamento_Cod As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal isFreshAndFood As Boolean = False,
                                            Optional ByVal Flag_QtaNoZero As Boolean = False,
                                            Optional ByVal Flag_QtaMaggioreZero As Boolean = False)

        Dim Dt As DataTable = Nothing
        Dim i As Integer
        Dim x_ProCod As Integer
        Dim x_MatCod As Integer

        Dim x_Cod As Integer
        Dim x_Des As String

        NumTotale = 0

        cmb.Items.Clear()

        If Flag_PrimaRiga Then
            cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Select Case Cau_Mov

            Case CAU_CARICO

                '#############################################
                '##############     CARICO    ################
                '#############################################

                Dt = New AgronicaCoreMetaSchemaDAL.CategorieXUnitaMisura_R().Leggi(
                                                                Elem_Cod,
                                                                0,
                                                                Cau_Mov,
                                                                False, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametriServer)


            Case CAU_SCARICO

                '#############################################
                '##############     SCARICO    ###############
                '#############################################

                'leggo le unità di misura in magazzino
                ' il magazzino è obbligatorio

                Select Case Flag_ProCod_Negativo
                    Case True
                        If Pro_Cod < 0 Then
                            x_MatCod = -Pro_Cod
                            x_ProCod = 0
                        Else
                            x_ProCod = Pro_Cod
                            x_MatCod = 0
                        End If
                    Case False
                        x_ProCod = Pro_Cod
                        x_MatCod = Mat_Cod
                End Select

                Dim objGia As New AgronicaCoreStampeDAL.Magazzino

                Dt = objGia.SchedaGiacenzeMagazzino(objParametriServer.FinestraTemporaleInizio,
                                                    Piva,
                                                    Sa_Cod,
                                                    Id_Destinazione,
                                                    Elem_Cod,
                                                    x_ProCod,
                                                    x_MatCod,
                                                    0, 0, 0,
                                                    0,
                                                    LOTTO_NONDEFINITO,
                                                    Flag_QtaNoZero,
                                                    "", "", "", "", "", "", "", "", "", "", "",
                                                    "", "",
                                                    objParametriServer,
                                                    objParametriUtenti, isFreshAndFood:=isFreshAndFood, Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero)

                'Dt = New AgronicaCoreContabDAL.Giacenze_R().Giacenze_Leggi( _
                '                             Piva, _
                '                            Sa_Cod, _
                '                            Id_Destinazione, _
                '                            0, _
                '                            0, _
                '                            Elem_Cod, _
                '                            x_ProCod, _
                '                            x_MatCod, _
                '                            0, 0, 0, _
                '                            0, _
                '                            LOTTO_NONDEFINITO, _
                '                            0, 0, 0, AGRODATAFINE, _
                '                            "", "", objParametriServer)


        End Select


        If Not IsNothing(Dt) Then

            'NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Udm_Cod")
                x_Des = CStr(Dt.Rows(i).Item("Udm_Des"))

                If Not IsDBNull(x_Cod) AndAlso IsNothing(cmb.Items.FindByValue(x_Cod)) Then

                    NumTotale += 1

                    cmb.Items.Add(New ListItem(x_Des, x_Cod))

                End If


            Next

        Else
            NumTotale = 0
        End If


        'Dt.Dispose()
        Dt = Nothing

        If Regolamento_Cod > 1 Then

            Select Case Cau_Mov

                Case CAU_CARICO

                    cmb.Items.Add(New ListItem("metri cubi", enum_UnitaMisura.Metri_Cubi))
                    cmb.Items.Add(New ListItem("q", enum_UnitaMisura.Quintali))
                    cmb.Items.Add(New ListItem("t", enum_UnitaMisura.Tonnellate))

                Case CAU_SCARICO
                    'in questo caso se ho litri aggiungo metri cubi, se ho kg aggiungo quintali e tonnellate
                    If Not IsNothing(cmb.Items.FindByValue(enum_UnitaMisura.KG)) Then
                        cmb.Items.Add(New ListItem("q", enum_UnitaMisura.Quintali))
                        cmb.Items.Add(New ListItem("t", enum_UnitaMisura.Tonnellate))
                    End If

                    If Not IsNothing(cmb.Items.FindByValue(enum_UnitaMisura.Litri)) Then
                        cmb.Items.Add(New ListItem("metri cubi", enum_UnitaMisura.Metri_Cubi))
                    End If
            End Select

        End If



    End Sub


    '##################################################################
    Public Shared Sub Mesi(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String)


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Gennaio", "1"))
        Controllo.Items.Add(New ListItem("Febbraio", "2"))
        Controllo.Items.Add(New ListItem("Marzo", "3"))
        Controllo.Items.Add(New ListItem("Aprile", "4"))
        Controllo.Items.Add(New ListItem("Maggio", "5"))
        Controllo.Items.Add(New ListItem("Giugno", "6"))
        Controllo.Items.Add(New ListItem("Luglio", "7"))
        Controllo.Items.Add(New ListItem("Agosto", "8"))
        Controllo.Items.Add(New ListItem("Settembre", "9"))
        Controllo.Items.Add(New ListItem("Ottobre", "10"))
        Controllo.Items.Add(New ListItem("Novembre", "11"))
        Controllo.Items.Add(New ListItem("Dicembre", "12"))

    End Sub

    '##################################################################
    'il numero del mese è formattato a 2 cifre
    Public Shared Sub Mesi2(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String)


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Gennaio", "01"))
        Controllo.Items.Add(New ListItem("Febbraio", "02"))
        Controllo.Items.Add(New ListItem("Marzo", "03"))
        Controllo.Items.Add(New ListItem("Aprile", "04"))
        Controllo.Items.Add(New ListItem("Maggio", "05"))
        Controllo.Items.Add(New ListItem("Giugno", "06"))
        Controllo.Items.Add(New ListItem("Luglio", "07"))
        Controllo.Items.Add(New ListItem("Agosto", "08"))
        Controllo.Items.Add(New ListItem("Settembre", "09"))
        Controllo.Items.Add(New ListItem("Ottobre", "10"))
        Controllo.Items.Add(New ListItem("Novembre", "11"))
        Controllo.Items.Add(New ListItem("Dicembre", "12"))

    End Sub


    '##################################################################
    Public Shared Sub Anni(ByRef Controllo As ListControl,
                           ByVal PrimaRiga_Flag As Boolean,
                           ByVal PrimaRiga_Text As String,
                           ByVal PrimaRiga_Value As String,
                           ByVal PrimoAnno As Integer,
                           ByVal UltimoAnno As Integer)


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim i As Integer
        For i = PrimoAnno To UltimoAnno
            Controllo.Items.Add(New ListItem(i.ToString, i.ToString))
        Next

    End Sub

    '##################################################################
    Public Shared Sub Anno_Agenda(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal Lav_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim i As Integer
        Dim Dt As DataTable
        Dim x_Cod As Integer
        Dim x_Des As String

        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R

        Dt = objAgenda.Leggi_Distinct_Anno(Piva,
                                            Lav_Cod,
                                            xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Anno")

                x_Des = CStr(Dt.Rows(i).Item("Anno"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        End If

        Dt = Nothing

    End Sub


    ''' -----------------------------------------------------------------------------
    Public Shared Sub Operazioni_FiltroUtente(ByRef Controllo As ListControl,
                                              ByVal PrimaRiga_Flag As Boolean,
                                              ByVal PrimaRiga_Text As String,
                                              ByVal PrimaRiga_Value As String,
                                              ByVal Lav_Cod As Integer,
                                              ByVal P As Integer,
                                              ByVal Gru_Cod As Integer,
                                              ByVal Tipo As String,
                                              ByVal Att_Cod As Integer,
                                              ByVal Cerca_LavDes As String,
                                              ByVal Cerca_GruDes As String,
                                              ByVal Flag_OpColturali As Boolean,
                                              ByVal Flag_OpZoo As Boolean,
                                              ByVal Flag_OpMacchine As Boolean,
                                              ByVal Flag_OpContabili As Boolean,
                                              ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer

        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objOpFiltroUtente As New AgronicaCoreMetaSchemaDAL.OperazioniLavorazioni_GestioneFiltroUtente_R

        Dt = objOpFiltroUtente.Leggi(Lav_Cod, P, Gru_Cod, Tipo, Att_Cod, Cerca_LavDes, Cerca_GruDes,
                                     Flag_OpColturali, Flag_OpZoo, Flag_OpMacchine, Flag_OpContabili,
                                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                     xFiltroAggiuntivo, xOrderBy, objParametri)


        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Lav_Cod")

                x_Des = CStr(Dt.Rows(i).Item("Lav_Des"))

                Controllo.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub



    '##################################################################
    Public Shared Sub Tariffe(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Tariffa_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objTariffe As New AgronicaCoreContabDAL.Tariffe_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objTariffe.Leggi(Tariffa_Cod,
                               xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Tariffa_Des"),
                                                 Dt.Rows(i).Item("Tariffa_Cod")))

            Next

        End If

    End Sub

    '##################################################################
    Public Shared Sub TariffeXQualifica(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Qualifica_Cod As Int32,
                                ByVal DataDiValidita As DateTime,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objTariffe As New AgronicaCoreContabDAL.Tariffe_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objTariffe.LeggiTariffeXQualifica(Qualifica_Cod, DataDiValidita,
                               xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

            For i = 0 To Dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Tariffa_Des"),
                                                 Dt.Rows(i).Item("Tariffa_Cod")))

            Next

        End If

    End Sub

    Public Shared Sub Utenti_Tipologie(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Tipologia_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As New DataTable
        Dim i As Integer = 0

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objUT As New AgronicaCoreUtentiDAL.Utenti_Tipologie_R

        Dt = objUT.Leggi(Tipologia_Cod,
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          xFiltroAggiuntivo, xOrderBy,
                          objParametriUtenti)

        If Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Tipologia_Des"),
                           Dt.Rows(i).Item("Tipologia_Cod")))
            Next

        End If

    End Sub

    Public Shared Sub Servizi_xPratiche(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )


        Dim DT As DataTable

        Dim objServizi As New AgronicaCoreMetaSchemaDAL.Servizi_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = objServizi.Leggi(0, "", AGRODATAINIZIO, AGRODATAFINE,
                              "", "", objParametri)
        If DT.Rows.Count > 0 Then

            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Servizio_Des"),
                                           DT.Rows(i).Item("Servizio_Cod")))
            Next

        End If

    End Sub

    Public Shared Sub Servizi_Stati(ByRef Controllo As ListControl,
                        ByVal PrimaRiga_Flag As Boolean,
                        ByVal PrimaRiga_Text As String,
                        ByVal PrimaRiga_Value As String,
                        ByVal Servizio_Cod As Integer,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        )


        Dim DT As DataTable

        Dim objStati As New AgronicaCoreMetaSchemaDAL.Servizi_Stati_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = objStati.Leggi(0, "", Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE,
                              "", "", objParametri)

        If DT.Rows.Count > 0 Then

            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Stato_Des"),
                                           DT.Rows(i).Item("Stato_Cod")))
            Next

        End If

    End Sub

    Public Shared Sub Servizi_Stati_Distinct(ByRef Controllo As ListControl,
                        ByVal PrimaRiga_Flag As Boolean,
                        ByVal PrimaRiga_Text As String,
                        ByVal PrimaRiga_Value As String,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        )


        Dim DT As DataTable
        Dim i As Integer

        Dim objStati As New AgronicaCoreMetaSchemaDAL.Servizi_Stati_R
        Dim HashStati As New Hashtable

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = objStati.Leggi(0, "", 0, AGRODATAINIZIO, AGRODATAFINE,
                              "", "", objParametri)

        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1
                If Not HashStati.ContainsKey(CStr(DT.Rows(i).Item("Stato_Cod"))) Then
                    Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Stato_Des"),
                           DT.Rows(i).Item("Stato_Cod")))
                    HashStati.Add(CStr(DT.Rows(i).Item("Stato_Cod")), "")
                End If
            Next
        End If

    End Sub

    Public Shared Sub Servizi_Stati_WAnagraficaStati(ByRef Controllo As ListControl,
                    ByVal PrimaRiga_Flag As Boolean,
                    ByVal PrimaRiga_Text As String,
                    ByVal PrimaRiga_Value As String,
                        ByVal Servizio_Cod As Integer,
                    ByVal xFiltroAggiuntivo As String,
                    ByVal xOrderBy As String,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                    )


        Dim DT As DataTable
        Dim i As Integer

        Dim objPratiche_R As New AgronicaCoreProfilazioneDAL.Pratiche_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = objPratiche_R.Leggi_StatiServizio(Servizio_Cod, objParametri)

        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("WAnagraficaStati_Des"),
                       DT.Rows(i).Item("WAnagraficaStati_Cod")))
            Next
        End If

    End Sub

    Public Shared Sub Analisi_Terreno(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String,
                                      ByVal Piva As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      Optional ByVal sa_cod As Integer = 0)


        Dim DT As DataTable
        Dim i As Integer

        ' Dim objAnalisiTestata As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R
        Dim objAnalisiTestata As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'DT = objAnalisiTestata.Leggi(0, 0, _
        '                             Piva, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "",
        '                             enumSelezioneVariabile.Selezione_JoinCompleta, _
        '                             " Analisi_Testata.Analisi_Testata_Tipo = 1 ", "", _
        '                             objParametri)

        Dim Filtro As String
        If xFiltroAggiuntivo <> String.Empty Then
            Filtro = " Analisi_Testata.Analisi_Testata_Tipo = 1 AND " & xFiltroAggiuntivo
        Else
            Filtro = xFiltroAggiuntivo
        End If

        DT = objAnalisiTestata.LeggiConCertificati(Piva, sa_cod, 0, 0,
                                                   Filtro, xOrderBy,
                                                   objParametri)

        ' Dim strChiave As String

        Dim Data As String

        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1

                'strChiave = DT.Rows(i).Item("Analisi_Testata_Cod") & "|" & DT.Rows(i).Item("Piva") & "|" & DT.Rows(i).Item("Sa_Cod") & "|" & _
                '            DT.Rows(i).Item("Campo_Cod") & "|" & DT.Rows(i).Item("Appezza") & "|" & DT.Rows(i).Item("Id_Imp")

                Data = String.Empty
                If Not IsDBNull(DT.Rows(i).Item("Analisi_Testata_Data_Inizio")) Then
                    Data = "(" & CDate(DT.Rows(i).Item("Analisi_Testata_Data_Inizio")).ToShortDateString & ")"
                End If


                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Analisi_Testata_Des") & " - " & Data, DT.Rows(i).Item("Analisi_Testata_Cod")))

            Next
        End If

    End Sub

    Public Shared Sub Utenti(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim DT As DataTable
        Dim i As Integer

        Dim Utenti_Read As New AgronicaCoreUtentiDAL.Utenti_Read

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = Utenti_Read.Leggi2("", "",
                                   "", objParametri)

        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Cognome") & " " & DT.Rows(i).Item("Nome"), DT.Rows(i).Item("Username")))

            Next
        End If

    End Sub

    Public Shared Sub Gruppi_Utente(ByRef Controllo As ListControl,
                                    ByVal PrimaRiga_Flag As Boolean,
                                    ByVal PrimaRiga_Text As String,
                                    ByVal PrimaRiga_Value As String,
                                    ByVal Gruppi_Utente_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    )


        Dim DT As DataTable
        Dim i As Integer

        Dim objGruppiUtente As New AgronicaCoreUtentiDAL.Gruppi_Utente_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = objGruppiUtente.Leggi(Gruppi_Utente_Cod, xFiltroAggiuntivo,
                                   "", objParametri)

        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1
                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Gruppi_Utente_Des"), DT.Rows(i).Item("Gruppi_Utente_Cod")))
            Next
        End If

    End Sub


    '###############################################################################################
    Public Shared Sub ReportInsoluti_Ordinamento(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String
                                                )

        Dim x_Cod, x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        x_Cod = enum_ReportInsoluti_Ordinamento.Scadenza
        x_Des = "Scadenza, contatto, data, tipo documento, numero"

        Controllo.Items.Add(New ListItem(x_Des, x_Cod))

        x_Cod = enum_ReportInsoluti_Ordinamento.ClienteData
        x_Des = "Contatto, data, tipo documento, numero"

        Controllo.Items.Add(New ListItem(x_Des, x_Cod))

        x_Cod = enum_ReportInsoluti_Ordinamento.Data
        x_Des = "Data, tipo documento, numero"

        Controllo.Items.Add(New ListItem(x_Des, x_Cod))

        x_Cod = enum_ReportInsoluti_Ordinamento.Numero
        x_Des = "Tipo documento, numero"

        Controllo.Items.Add(New ListItem(x_Des, x_Cod))

        x_Cod = enum_ReportInsoluti_Ordinamento.ClienteScadenza
        x_Des = "Contatto, scadenza, data, tipo documento, numero"

        Controllo.Items.Add(New ListItem(x_Des, x_Cod))


    End Sub

    '###############################################################################################
    Public Shared Sub RegistriIva_Ordinamento(ByRef Controllo As ListControl,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String
                                                )

        Dim x_Cod, x_Des As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        x_Cod = enum_RegistriIva_Ordinamento.NumeroDoc
        x_Des = " Numero Doc., Data Movimento "

        Controllo.Items.Add(New ListItem(x_Des, x_Cod))

        x_Cod = enum_RegistriIva_Ordinamento.NumeroProtocollo
        x_Des = " Num. Protocollo, Data Registrazione "

        Controllo.Items.Add(New ListItem(x_Des, x_Cod))

    End Sub


    Public Shared Sub Analisi_Tipologia_Laboratori(ByRef Controllo As ListControl,
                                                   ByVal PrimaRiga_Flag As Boolean,
                                                   ByVal PrimaRiga_Text As String,
                                                   ByVal PrimaRiga_Value As String,
                                                   ByVal Analisi_Tipologia_Cod As Integer,
                                                   ByVal Cod_Risum As Integer,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   Optional ByVal soloStandard As Boolean = False)


        Dim DT As DataTable
        Dim i As Integer

        Dim objTipologiaLaboratori As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If soloStandard Then
            DT = objTipologiaLaboratori.LeggiconTipologiaDes(0, Cod_Risum,
                                                              "", "",
                                                              objParametri, True)
        Else
            DT = objTipologiaLaboratori.LeggiconTipologiaDes(0, Cod_Risum,
                                                              "", "",
                                                              objParametri, False)
        End If

        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Analisi_Tipologia_Des"), DT.Rows(i).Item("Analisi_Tipologia_Cod")))

            Next
        End If

    End Sub




    Public Shared Sub SpecieAgea(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               )


        Dim DT As DataTable
        Dim i As Integer

        Dim objSpecieAgea As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        DT = objSpecieAgea.SpecieDistinteAgea(xFiltroAggiuntivo, "",
                                              objParametri)

        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Veg_Des_Agea"),
                                                 DT.Rows(i).Item("Veg_Cod_Agea")))

            Next
        End If

    End Sub

    Shared Sub StampeReport(ByRef Controllo As ListControl,
                               ByVal PrimaRiga_Flag As Boolean,
                               ByVal PrimaRiga_Text As String,
                               ByVal PrimaRiga_Value As String,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               )

        Try

            Dim DT As DataTable
            Dim i As Integer

            Dim StampeReport As New AgronicaCoreMetaSchemaDAL.StampeReport

            'Pulisco il controllo
            Controllo.Items.Clear()

            If PrimaRiga_Flag Then
                Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
            End If

            DT = StampeReport.Leggi(0, 0, xFiltroAggiuntivo, "",
                                                  objParametri)

            If DT.Rows.Count > 0 Then
                For i = 0 To DT.Rows.Count - 1

                    Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Descrizione"),
                                                     DT.Rows(i).Item("Id_StampeReport")))

                Next
            End If

        Catch ex As Exception

        End Try



    End Sub

    Public Sub MacchineDitte(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                 ByVal sa_cod As Integer,
                                ByVal Validita As Date,
                                 ByVal objParametri_Server As AgronicaCoreParametri)
        Dim DT As DataTable
        Dim i As Integer
        Dim ParcoMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R
        'Dim filtro As String = " (Parco_Macchine.sa_cod= " & sa_cod & " or Parco_Macchine.sa_cod=0)  "
        DT = ParcoMacchine.LeggiConDitteCentro(Piva, sa_cod, True, AGRODATAINIZIO, AGRODATAFINE, "Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(Validita), " CLASS_DESC,Mac_Des, Ditta_Des, Modello  ", objParametri_Server)

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If DT.Rows.Count > 0 Then
            For i = 0 To DT.Rows.Count - 1

                Dim descr As String = DT.Rows(i).Item("CLASS_DESC")

                If Not IsDBNull(DT.Rows(i).Item("Mac_Des")) AndAlso DT.Rows(i).Item("Mac_Des") <> "" Then
                    descr = descr & "/" & DT.Rows(i).Item("Mac_Des")
                End If
                If Not IsDBNull(DT.Rows(i).Item("Ditta_Des")) AndAlso DT.Rows(i).Item("Ditta_Des") <> "" Then
                    descr = descr & "/" & DT.Rows(i).Item("Ditta_Des")
                End If
                If Not IsDBNull(DT.Rows(i).Item("Modello")) AndAlso DT.Rows(i).Item("Modello") <> "" Then
                    descr = descr & "/" & DT.Rows(i).Item("Modello")
                End If

                Controllo.Items.Add(New ListItem(descr, DT.Rows(i).Item("Mac_Cod")))

            Next
        End If


    End Sub


    '######################################################################
    Public Shared Sub Tipo_Fabbricato_Cod(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim objListaTipiFab As New AgronicaCoreAnagrafeDAL.Fabbricati_Tipi_R()
        Dim Dt As DataTable
        Dim i As Integer

        'Leggo le imprese associate al profilo selezionato			
        Dt = objListaTipiFab.Leggi(0,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "Tipo_Fabbricato_Des", objParametri)


        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Se il recordset non è chiuso allora ...	
        If Dt IsNot Nothing AndAlso Dt.Rows.Count - 1 Then

            'Inserisco i record trovati
            For i = 0 To Dt.Rows.Count - 1



                Controllo.Items.Add(New ListItem(
                                    Dt.Rows(i).Item("Tipo_Fabbricato_Des"),
                                    Dt.Rows(i).Item("Tipo_Fabbricato_Cod")))



            Next

        End If

        'Elimino il datatable
        Dt = Nothing

    End Sub

    Public Shared Sub ListaIcqrf(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Piva As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim Dt As DataTable
        Dim i As Integer
        Dim objCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dt = objCodici.Leggi(Piva, 0, 1109, "", "", enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri)


        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then
            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("Val_Cod").ToString.Replace("/", "").Replace("\", "").Replace("-", "").Replace(" ", "") & " " & Dt.Rows(i).Item("Sa_nome"), CStr(i + 1)))
            Next

        End If

    End Sub

    Public Shared Sub Fascicoli(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )


        Dim objCOM As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
        Dim DT As DataTable

        Dim Data_Validazione As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Leggo le imprese associate al profilo selezionato			
        DT = objCOM.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                          xFiltroAggiuntivo,
                          xOrderBy,
                          objParametri)

        objCOM = Nothing

        '----- Riempio la combo con i dati del datatable

        'Se il datatable non è chiuso allora ...	
        If DT.Rows.Count > 0 Then

            Dim i As Integer
            'Inserisco i record trovati
            For i = 0 To DT.Rows.Count - 1

                Data_Validazione = ""
                If Not IsDBNull(DT.Rows(i).Item("Validazione_Data")) AndAlso CDate(DT.Rows(i).Item("Validazione_Data")) <> AGRODATAINIZIO AndAlso CDate(DT.Rows(i).Item("Validazione_Data")) <> AGRODATAFINE Then
                    Data_Validazione = " Data Validazione " & CDate(DT.Rows(i).Item("Validazione_Data")).ToShortDateString
                End If

                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Allegati_Documenti_Numero") & Data_Validazione,
                                            DT.Rows(i).Item("Allegati_Documenti_Cod")))

            Next

        End If

    End Sub

    Public Shared Sub AspettoBeni(ByRef Controllo As ListControl,
                                  ByVal PrimaRiga_Flag As Boolean,
                                  ByVal PrimaRiga_Text As String,
                                  ByVal PrimaRiga_Value As String,
                                  ByVal moduloGias As enum_Omni_Modulo_Generazione,
                                  ByRef objParametri As AgronicaCoreParametri)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("VISIBILE", "0"))
        Controllo.Items.Add(New ListItem("CARTONI", "0"))
        Controllo.Items.Add(New ListItem("BANCALE", "0"))
        Controllo.Items.Add(New ListItem("CISTERNA", "0"))
        Controllo.Items.Add(New ListItem("PALLETS", "0"))
        Controllo.Items.Add(New ListItem("RIMORCHIO", "0"))

        If moduloGias = enum_Omni_Modulo_Generazione.Cantine Then
            Controllo.Items.Add(New ListItem("DAMIGIANE", "0"))
            Controllo.Items.Add(New ListItem("CASSE", "0"))
            Controllo.Items.Add(New ListItem("TANICHE", "0"))
        End If

    End Sub

    Public Sub CausaliTrasporto(ByRef Controllo As ListControl,
                                       ByVal PrimaRiga_Flag As Boolean,
                                       ByVal PrimaRiga_Text As String,
                                       ByVal PrimaRiga_Value As String,
                                       ByVal moduloGias As enum_Omni_Modulo_Generazione,
                                       ByVal lavCod As Integer,
                                       ByVal objParametri_Server As AgronicaCoreParametri)
        Dim dt As DataTable
        Dim i As Integer
        Dim objCausali As New AgronicaCoreContabDAL.Causali_Trasporto_R
        Dim filtro As String = ""

        Select Case lavCod

            Case 0  'Tutti i documenti

                filtro = " Tipo <> " & Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Invisibile) & " "

            Case LAVCOD_CONTRATTO_AFFITTO

                filtro = " Tipo = " & Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Contratti_Affitto) & " "

            Case LAVCOD_MVV_EMESSO

                filtro = " Tipo = " & Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.MVV_Elettronico) & " "

            Case Else

                Select Case moduloGias

                    Case enum_Omni_Modulo_Generazione.Nessuno   'Documenti Contabili Standard

                        Select Case Contabilita.Attivo_Passivo(lavCod)

                            Case "ATTIVO"

                                filtro = " Tipo IN (" &
                                         Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Tutti_Doc_Contabili) & " , " &
                                         Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Doc_Contabili_Attivi) & ") "

                            Case "PASSIVO"

                                filtro = " Tipo IN (" &
                                         Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Tutti_Doc_Contabili) & " , " &
                                         Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Doc_Contabili_Passivi) & ") "

                            Case Else 'Eccezione

                                filtro = " Tipo <> " & Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Invisibile) & " "

                        End Select

                    Case Else

                        Select Case lavCod

                            Case LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                                 LAVCOD_ACCETTAZIONE_DIVERSI,
                                 LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                                filtro = " Tipo IN (" &
                                         Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Tutti_Doc_Contabili) & " , " &
                                         Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Accettazione_Beni) & ") "

                            Case Else

                                Select Case Contabilita.Attivo_Passivo(lavCod)

                                    Case "ATTIVO"

                                        filtro = " Tipo IN (" &
                                                 Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Tutti_Doc_Contabili) & " , " &
                                                 Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Doc_Contabili_Attivi) & ") "

                                    Case "PASSIVO"

                                        filtro = " Tipo IN (" &
                                                 Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Tutti_Doc_Contabili) & " , " &
                                                 Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Doc_Contabili_Passivi) & ") "

                                    Case Else 'Eccezione

                                        filtro = " Tipo <> " & Agro_SQL_SaveNum(enum_Tipo_CausaliTrasporto.Invisibile) & " "

                                End Select

                        End Select

                End Select

        End Select

        dt = objCausali.Leggi(0,
                              "",
                              "",
                              -1,
                              enum_Tipo_CausaliTrasporto.Non_Impostato,
                              -1,
                              filtro,
                              " ChkDefault Desc, Causale_Trasporto_Cod  ",
                              objParametri_Server)

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If dt.Rows.Count > 0 Then
            For i = 0 To dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(dt.Rows(i).Item("Causale_Trasporto_Des"),
                                                 dt.Rows(i).Item("Causale_Trasporto_Cod")))

            Next
        End If

    End Sub



    Public Sub SigleAE(ByRef Controllo As ListControl,
                       ByVal PrimaRiga_Flag As Boolean,
                       ByVal PrimaRiga_Text As String,
                       ByVal PrimaRiga_Value As String,
                       ByVal lavCod As Integer,
                       ByVal objParametri_Server As AgronicaCoreParametri)
        Dim dt As DataTable
        Dim i As Integer
        'Dim objCausali As New AgronicaCoreContabDAL.Causali_Trasporto_R

        Dim objAcc As New AgronicaCoreContabDAL.SigleAE_R(objParametri_Server)

        Dim filtro As String = ""


        Select Case lavCod

            Case 0  'Tutti i documenti

                filtro = ""

            Case 1002, 1003 'Note di Credito

                filtro = "Codice = 4"

            Case Else

                filtro = "Codice <> 4"

        End Select

        dt = objAcc.LeggiSigleAE(
                              filtro,
                              "xDescrizione Asc",
                              objParametri_Server)

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If dt.Rows.Count > 0 Then
            For i = 0 To dt.Rows.Count - 1

                Controllo.Items.Add(New ListItem(dt.Rows(i).Item("xDescrizione"),
                                                 dt.Rows(i).Item("Codice")))

            Next
        End If

    End Sub


    Public Shared Sub GestioneVettore(ByRef Controllo As ListControl,
                                      ByVal PrimaRiga_Flag As Boolean,
                                      ByVal PrimaRiga_Text As String,
                                      ByVal PrimaRiga_Value As String,
                                      ByVal flagEstero As Boolean,
                                      ByRef objParametri As AgronicaCoreParametri)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Franco Partenza", "1"))
        Controllo.Items.Add(New ListItem("Franco Arrivo", "2"))

        If flagEstero Then
            Controllo.Items.Add(New ListItem("Franco Spedizioniere", "3"))
        End If

        Controllo.Items.Add(New ListItem("Franco lungo bordo", "4"))
        Controllo.Items.Add(New ListItem("Franco a bordo", "5"))
        Controllo.Items.Add(New ListItem("Costo e nolo", "6"))
        Controllo.Items.Add(New ListItem("Costo, assicurazione e nolo", "7"))
        Controllo.Items.Add(New ListItem("Trasporto pagato fino a", "8"))
        Controllo.Items.Add(New ListItem("Trasporto e assicurazione pagati fino a", "9"))
        Controllo.Items.Add(New ListItem("Reso frontiera", "10"))
        Controllo.Items.Add(New ListItem("Reso ex ship", "11"))
        Controllo.Items.Add(New ListItem("Reso banchina (sdoganato)", "12"))
        Controllo.Items.Add(New ListItem("Reso non sdoganato", "13"))
        Controllo.Items.Add(New ListItem("Reso sdoganato", "14"))

    End Sub

    'Shared Sub Carica_Da_Enumerativo(ByRef Controllo As ListControl, _
    '                                 enumType As System.Type)
    '    Dim names() As String = [Enum].GetNames(enumType)
    '    Array.Sort(names)
    '    For Each name In names
    '        Dim status As enum_RACCOLTA_TIPO = CType([Enum].Parse(enumType, name),  _
    '                                      enum_RACCOLTA_TIPO)
    '        Controllo.Items.Add(New ListItem( _
    '                   name, _
    '                   status))
    '    Next
    'End Sub

    '######################################################################
    Public Shared Sub WebService_Esterni(ByRef Controllo As ListControl,
                                          ByVal PrimaRiga_Flag As Boolean,
                                          ByVal PrimaRiga_Text As String,
                                          ByVal PrimaRiga_Value As String)

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Controllo.Items.Add(New ListItem("Piano Colturale Agrea", CStr(enum_WS_Esterni.WS_Fascicolo_Agrea)))
        Controllo.Items.Add(New ListItem("Anagrafe ER", CStr(enum_WS_Esterni.WS_Fascicolo_AnagrafeER)))
        Controllo.Items.Add(New ListItem("Agea Coordinamento", CStr(enum_WS_Esterni.WS_Fascicolo_Agea_Coordinamento)))
        Controllo.Items.Add(New ListItem("Agea Real Time", CStr(enum_WS_Esterni.WS_Fascicolo_Agea_RealTime)))
        Controllo.Items.Add(New ListItem("Piano Colturale Grafico", CStr(enum_WS_Esterni.WS_Fascicolo_Grafico_Artea)))
        Controllo.Items.Add(New ListItem("Agea Coordinamento BA", CStr(enum_WS_Esterni.WS_Fascicolo_BA)))


    End Sub

End Class
