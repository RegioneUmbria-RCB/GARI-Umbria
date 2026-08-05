Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class MenuBS_Lavorazioni
    Public Shared Function Carica_Operazioni_Zootecniche(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri
        ) As DataTable

        Dim Dt As New DataTable
        Dim DtAgenda As New DataTable
        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim DtOperazione As New DataTable
        Dim DtApp As New DataTable
        Dim DtProdotti As New DataTable
        Dim DtAvversita As New DataTable
        Dim DtAvversitaGru As New DataTable
        Dim DtProdotti1 As New DataTable
        Dim DtSpecie As New DataTable

        Dim i, j As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim AppezzamentoNome As String
        Dim strAppezzamenti As String
        Dim strCulDes As String
        Dim strProdotti As String
        Dim strAvversita As String
        Dim strSpecie As String
        Dim strCentro As String
        Dim strSpecieVarieta As String
        Dim strDettaglioTecnico As String
        Dim strCentroCampo As String
        Dim strLottiProduzione As String
        Dim strLottiImpianto As String
        Dim strNote As String
        Dim strCosti_Operatori As String
        Dim strCosti_Macchine As String
        Dim Prodotto As String
        Dim Ricetta As String
        Dim Sup_TrattataTot As Decimal

        Dim strId_Agenda() As String

        Dim Testo As String
        Dim strDettagli As String
        Dim Bloccato As String

        Dim ht_Permessi As New Hashtable

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

        Dim Icona_INFO As String = "<img src='../AB_Immagini/Icone16/cI.ico' border='0'>"

        Validita_Inizio = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Validita_Fine = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Info", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettagli", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Operazione_DES", GetType(String)))
        Dt.Columns.Add(New DataColumn("gru_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo_colore", GetType(String)))
        'Dt.Columns.Add(New DataColumn("cul_des", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Specie_Varieta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Centro_Campo", GetType(String)))
        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("LottiProduzione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Operatori", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Macchine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Trattata", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("LottiImpianto", GetType(String)))
        Dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prodotti", GetType(String)))


        Dim filtro As String = "|"

        Dim filtrolavorazioni = filtro.Split("|")(0)


        Try

            Dim objOperazioni As New AgronicaCoreContabDAL.Agenda_R
            Dt = objOperazioni.Carica_Operazioni_ZooTecnichexAgenda(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                xOrderBy,
                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                HttpContext.Current.Session("ASG_objParametri_Server")
                )


        Catch ex As Exception

            Return Nothing

        End Try




        'If DtAgenda.Rows.Count > 0 Then

        '    '----------------------------------
        '    'Leggo tutti i principi attivi
        '    Dim HtProdPA As New Hashtable()
        '    'Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(DtAgenda, HtProdPA, objparametri_Server)

        '    Dim DtCosti As New DataTable


        '    strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "id_agenda")

        '    If Not strId_Agenda Is Nothing Then
        '        For i = 0 To strId_Agenda.Length - 1
        '            'AZZERO LE STRINGHE AD OGNI GIRO
        '            strAppezzamenti = ""
        '            strCulDes = ""
        '            strProdotti = ""
        '            strAvversita = ""
        '            strNote = ""
        '            strCosti_Operatori = ""
        '            strCosti_Macchine = ""
        '            Sup_TrattataTot = 0


        '            DrAgenda = DtAgenda.Select("id_agenda=" & strId_Agenda(i))

        '            DtOperazione = DtAgenda.Clone

        '            For j = 0 To DrAgenda.Length - 1
        '                DtOperazione.ImportRow(DrAgenda(j))
        '            Next


        '            If DrAgenda.Length > 0 Then

        '                Dr = Dt.NewRow
        '                '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
        '                Dr.Item("Piva") = DrAgenda(0).Item("Piva")
        '                Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
        '                Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")
        '                Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
        '                Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
        '                Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))
        '                Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
        '                Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
        '                Dr.Item("Id_Mov_Det") = DrAgenda(0).Item("Id_Mov_Det")
        '                Dr.Item("Blocco_Flag") = 0
        '                Dr.Item("Info") = ""
        '                Dr.Item("Dettagli") = ""
        '                Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
        '                Dr.Item("Operazione_DES") = DrAgenda(0).Item("lav_des")
        '                Dr.Item("tipo") = DrAgenda(0).Item("tipo")

        '                'Se è un altre lavorazioni aggiungo il dettaglio
        '                If DrAgenda(0).Item("attivitaDesc") <> "" Then
        '                    'Dr.Item("Lav_Des") &= " (" & String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s))) & ")"
        '                    Dr.Item("Lav_Des") = String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))
        '                End If

        '                strCentro = ""
        '                Prodotto = ""
        '                strDettaglioTecnico = ""
        '                strLottiProduzione = ""


        '                strDettagli = ""


        '                Dim lc As Integer = CInt(DrAgenda(0).Item("Lav_Cod"))


        '                Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")

        '                Bloccato = If(DrAgenda(0).Item("Blocco_Flag") = 1, "Si", "No")

        '                'Testo = "<a " &
        '                '        "title='" & "ID: " & DrAgenda(0).Item("id_agenda") & vbCrLf &
        '                '        Resources.AgronicaAgenda_2010.CreatoreIntervento & DrAgenda(0).Item("Tecnico") & vbCrLf &
        '                '        Resources.AgronicaAgenda_2010.InterventoBloccato & Bloccato &
        '                '        "' " &
        '                '        ">" &
        '                '        Icona_INFO &
        '                '        "</a>"

        '                Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
        '                Dim riga2 As String = strSpecieVarieta & If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
        '                Dim riga3 As String = strCentroCampo & If(String.IsNullOrEmpty(strAppezzamenti), "", " <i>" & strAppezzamenti & "</i>")
        '                Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim(), riga3.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

        '                Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
        '                Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
        '                'Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

        '                If strCentro <> "" Then
        '                    strDettagli &= "<b>" & "Centro Aziendale" & "</b> " & strCentro & "<br>"
        '                End If

        '                Dr.Item("Info") = Testo
        '                Dr.Item("Dettagli") = strDettagli

        '                Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
        '                Dr.Item("Note") = strNote

        '                strProdotti = estraiListaProdotti(DtOperazione)


        '                Dr.Item("Prodotti") = strProdotti

        '                Dt.Rows.Add(Dr)

        '            End If

        '        Next

        '    End If

        'End If


        'Elimino l'oggetto
        objSQL = Nothing
        objSqlDis = Nothing

        Dt.TableName = "Movimenti"

        'uso il dataview per Riordinare 
        Dim Dv As New DataView(Dt)


        Dv.Sort = " Data2 DESC, Ora DESC, Id_Agenda DESC"

        Dim dtOrd As DataTable = Dv.ToTable

        Return dtOrd

    End Function
    Private Shared Function estraiListaProdotti(ByRef DtOperazione As DataTable) As String

        'Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility
        'Dim DtProdotti As DataTable
        'Dim strProdotti As String = ""

        Dim listaProd As New List(Of String)
        For Each dr As DataRow In DtOperazione.Rows
            Select Case dr.Item("elem_cod")
                Case FERTILIZZANTI
                    If dr.Item("Pro_Cod") <> 0 AndAlso Not listaProd.Contains(dr.Item("Fer_Des")) Then
                        listaProd.Add(dr.Item("Fer_Des"))
                    End If

                    If dr.Item("Mat_Cod") <> 0 AndAlso Not listaProd.Contains(dr.Item("Mat_Des")) Then
                        listaProd.Add(dr.Item("Mat_Des"))
                    End If

                Case FORMULATI
                    If dr.Item("Fr_Des") <> "" AndAlso Not listaProd.Contains(dr.Item("Fr_Des")) Then
                        listaProd.Add(dr.Item("Fr_Des"))
                    End If

                Case TRAPPOLE
                    If dr.Item("Trap_Des") <> "" AndAlso Not listaProd.Contains(dr.Item("Trap_Des")) Then
                        listaProd.Add(dr.Item("Trap_Des"))
                    End If

                Case Else
                    If dr.Item("Mat_Des") <> "" Then
                        Dim codart As String = If(dr.Item("Cod_Articolo") = "", "", "Articolo: " & dr.Item("Cod_Articolo"))
                        'Dim lotto As String = If(dr.Item("LottoProduzione") = "", "", "Lotto: " & dr.Item("LottoProduzione"))
                        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Dim prod As String = dr.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                        If Not listaProd.Contains(prod) Then
                            listaProd.Add(prod)
                        End If

                    End If

            End Select
        Next

        Dim strProdotti As String = String.Join(", ", listaProd)

        Return strProdotti

    End Function

End Class
