Imports System.Data.OleDb
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Stampe_OP
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Sub DatiIntestazione_Impresa_Legale(
        ByVal Piva As String,
        ByRef Vet_Intestazione() As String,
        ByRef log As String,
        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        'TODO: Vanni, Roberta ...Verificare se va bene fare qui la funzione (cioè nel Impresa_R, può essere fuorviante? viene usato solo per letture XML?) se va spostato, dove?

        Dim cod_contatto, rappr_legale, data_nascita, Codice_Fiscale As String
        Dim ind_residenza, cap_residenza, com_residenza, prov_residenza, cap_nascita, com_nascita, prov_nascita As String

        Dim rag_soc, ind_impresa, cap_impresa, com_impresa, prov_impresa As String
        Dim codice_fiscale_impresa, Cuaa, Codice_ICQ As String

        Dim xLettura As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim messaggio As String = ""
        Dim DT As DataTable = Nothing

        Try

            DT = xLettura.DatiIntestazione_Impresa_Legale(Piva, "", "", objParametri_Server)
        Catch ex As Exception
            messaggio = ex.Message
        End Try



        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            cod_contatto = CStr(DT.Rows(0).Item("Cod_Contatto"))
            Codice_Fiscale = CStr(DT.Rows(0).Item("Codice_Fiscale"))
            rappr_legale = Trim(CStr(DT.Rows(0).Item("Rappr_Legale")))
            data_nascita = DataNascita_from_CodFisc(cod_contatto)

            com_nascita = CStr(DT.Rows(0).Item("com_nascita"))
            prov_nascita = CStr(DT.Rows(0).Item("prov_nascita"))

            If com_nascita = "" Or prov_nascita = "0" Or prov_nascita = "00" Then
                prov_nascita = ""
                com_nascita = ""
                log += CStr(Date.Now) + "Non è stato inserito il luogo di nascita del legale rappresentante!" & vbCrLf & vbCrLf
            End If

            ind_residenza = CStr(DT.Rows(0).Item("ind_residenza"))
            com_residenza = CStr(DT.Rows(0).Item("com_residenza"))
            prov_residenza = CStr(DT.Rows(0).Item("prov_residenza"))

            If com_residenza = "" Or prov_residenza = "0" Or prov_residenza = "00" Then
                com_residenza = ""
                prov_residenza = ""
                log += CStr(Date.Now) + "Non è stato inserito l'indirizzo e il luogo di residenza del legale rappresentante!" & vbCrLf & vbCrLf
            End If

            rag_soc = CStr(DT.Rows(0).Item("rag_soc"))
            ind_impresa = CStr(DT.Rows(0).Item("ind_impresa"))
            com_impresa = CStr(DT.Rows(0).Item("com_impresa"))
            prov_impresa = CStr(DT.Rows(0).Item("prov_impresa"))

            If com_impresa = "" Or prov_impresa = "0" Then
                log += CStr(Date.Now) + "Non è stato inserito l'indirizzo dell'impresa!" & vbCrLf & vbCrLf
            End If

            Cuaa = CStr(DT.Rows(0).Item("cuaa"))
            Codice_ICQ = CStr(DT.Rows(0).Item("Codice_ICQ"))
            codice_fiscale_impresa = CStr(DT.Rows(0).Item("codice_fiscale_impresa"))



            Vet_Intestazione(0) = cod_contatto
            Vet_Intestazione(1) = Codice_Fiscale
            Vet_Intestazione(2) = rappr_legale
            Vet_Intestazione(3) = data_nascita
            Vet_Intestazione(4) = cap_nascita
            Vet_Intestazione(5) = com_nascita
            Vet_Intestazione(6) = prov_nascita
            Vet_Intestazione(7) = ind_residenza
            Vet_Intestazione(8) = cap_residenza
            Vet_Intestazione(9) = com_residenza
            Vet_Intestazione(10) = prov_residenza

            Vet_Intestazione(11) = Piva
            Vet_Intestazione(12) = rag_soc
            Vet_Intestazione(13) = ind_impresa
            Vet_Intestazione(14) = cap_impresa
            Vet_Intestazione(15) = com_impresa
            Vet_Intestazione(16) = prov_impresa
            Vet_Intestazione(17) = codice_fiscale_impresa
            Vet_Intestazione(18) = Cuaa
            Vet_Intestazione(19) = Codice_ICQ

        Else
            log += CStr(Date.Now) + "   Non sono stati trovati dati per l'INTESTAZIONE:  " + messaggio & vbCrLf & vbCrLf
            For i = 0 To Vet_Intestazione.Length - 1
                Vet_Intestazione(i) = ""
            Next
        End If


    End Sub


    Public Function DatiIntestazioneImpresa(
        ByVal Piva As String,
        ByRef Vet_Intestazione() As String,
        ByRef log As String,
        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        'TODO: Vanni, Roberta ...Verificare se va bene fare qui la funzione (cioè nel Impresa_R, può essere fuorviante? viene usato solo per letture XML?) se va spostato, dove?

        Dim xLettura As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim messaggio As String = ""
        Dim DT As DataTable = Nothing

        Try

            DT = xLettura.DatiIntestazioneImpresa(Piva, "", "", objParametri_Server)
        Catch ex As Exception
            messaggio = ex.Message
        End Try


        If messaggio = "" Then
        Else

            DT = New DataTable
            log += CStr(Date.Now) + "   Errore nella query INTESTAZIONE:  " + messaggio & vbCrLf & vbCrLf

        End If

        Return DT

    End Function

    Public Function Carica_ImpresePerStampaMassiva(ByRef DSImprese As DataSet, ByVal strFiltroImprese As String, ByVal tipo_Selezione As String,
                                        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim Log As String = ""
        Imprese_StampaMassiva(DSImprese, DSImprese.Tables(0).TableName, strFiltroImprese, tipo_Selezione, objParametri_Server)

        Dim listaIntestazioni As Dictionary(Of String, String()) = DSImprese.Tables(0).AsEnumerable().AsParallel.
            ToDictionary(
                Function(row) row.Field(Of String)("Piva"),
                Function(row) New String() {}
            )

        Dim listaPive As List(Of String) = DSImprese.Tables(0).AsEnumerable().AsParallel().
                Where(Function(row) Not row.IsNull("Piva")).
                Select(Function(row) row.Field(Of String)("Piva")).
                ToList()

        Dim ImpreseCodici_Read As New Imprese_Codici_Read
        DatiIntestazioneSoci(listaIntestazioni, listaPive, Log, objParametri_Server, True, True, True, objParametri_Server.PivaSuperUser, True, True, strFiltroImprese, tipo_Selezione)
        Dim CUAA_GGN_Produttore_Socio_DT = ImpreseCodici_Read.CUAA_GGN_Produttore_Socio_from_Piva_Massivo(listaPive, objParametri_Server)

        For Each row In DSImprese.Tables(0).Rows

            'prelevo i dati dell'intestazione
            Dim Vet_Intestazione As String() = listaIntestazioni.Item(row.Item("Piva"))
            row(0) = listaIntestazioni.Item(row.Item("Piva")).GetValue(9)

            'prelevo dati aggiuntivi
            Dim datiAggiuntivi = If(CUAA_GGN_Produttore_Socio_DT.Select($"Piva = '{ row.Item("Piva") }'").Any(),
                        CUAA_GGN_Produttore_Socio_DT.Select($"Piva = '{ row.Item("Piva") }'").CopyToDataTable(),
                        CUAA_GGN_Produttore_Socio_DT.Clone())

            Dim CUAA = ""
            Dim GGN = ""
            Dim Produttore = ""
            Dim Socio = ""

            If Not IsNothing(datiAggiuntivi) AndAlso datiAggiuntivi.Rows.Count > 0 Then
                For i = 0 To datiAggiuntivi.Rows.Count - 1
                    Select Case datiAggiuntivi.Rows(i).Item("id_cod")
                        Case enum_CodiciAnagrafe.CodiceCUAA
                            CUAA = datiAggiuntivi.Rows(i).Item("val_cod")
                        Case enum_CodiciAnagrafe.Codice_GlobalGap
                            GGN = datiAggiuntivi.Rows(i).Item("val_cod")
                        Case enum_CodiciAnagrafe.CodiceProduttore
                            Produttore = datiAggiuntivi.Rows(i).Item("val_cod")
                        Case enum_CodiciAnagrafe.Codice_Socio
                            Socio = datiAggiuntivi.Rows(i).Item("val_cod")
                    End Select
                Next
            End If

            Dim ind_impresa = If(Vet_Intestazione(11) <> "", ", " + Vet_Intestazione(11), "")
            Dim com_impresa = Vet_Intestazione(12)
            Dim prov_impresa = If(Vet_Intestazione(13) <> "", " (" + Vet_Intestazione(13) + ")", "")
            Dim cap_impresa = If(Vet_Intestazione(26) <> "", ", " + Vet_Intestazione(26), "")

            Dim com_coop = Vet_Intestazione(18)
            Dim prov_coop = If(Vet_Intestazione(19) <> "", " (" + Vet_Intestazione(19) + ")", "")
            Dim ind_coop = If(Vet_Intestazione(16) <> "", ", " + Vet_Intestazione(16), "")
            Dim cap_coop = If(Vet_Intestazione(17) <> "", ", " + Vet_Intestazione(17), "")

            Dim prov_residenza = If(Vet_Intestazione(7) <> "", " (" + Vet_Intestazione(7) + ")", "")
            Dim ind_residenza = If(Vet_Intestazione(5) <> "", ", " + Vet_Intestazione(5), "")
            Dim com_residenza = Vet_Intestazione(6)

            Dim com_nascita = Vet_Intestazione(2)
            Dim prov_nascita = If(Vet_Intestazione(3) <> "", " (" + Vet_Intestazione(3) + ")", "")
            Dim data_nascita = Vet_Intestazione(4)

            row.Rappr_Legale = Vet_Intestazione(1).ToUpper
            row.cooperativa = If(Vet_Intestazione(15) <> "", Vet_Intestazione(15), String.Concat(Enumerable.Repeat("&nbsp;", 40)))
            row.codice_libro_soci = Vet_Intestazione(20)
            row.data_libro_soci = Vet_Intestazione(21)
            row.codice_fiscale = Vet_Intestazione(24)
            row.Cuaa = Vet_Intestazione(25)
            row.cap_impresa = Vet_Intestazione(26)
            row.nome_ref_coop = Vet_Intestazione(27)
            row.recapito_tel = Vet_Intestazione(28)
            row.codice_BP = Vet_Intestazione(29)
            row.piva_padre = Vet_Intestazione(14)
            row.indirizzo_rappr = com_residenza + prov_residenza + ind_residenza
            row.indirizzo_coop = If(Vet_Intestazione(30) <> "", Vet_Intestazione(30), String.Concat(Enumerable.Repeat("&nbsp;", 40)))
            row.indirizzo_impresa = com_impresa + prov_impresa + cap_impresa + ind_impresa
            row.indirizzo_nascita = com_nascita + prov_nascita
            row.data_nascita = data_nascita
            row.GGN = GGN
            row.cod_socio = Socio
            row.num_lavoratori = ""
        Next

    End Function

    Public Function Imprese_StampaMassiva(ByRef DSImprese As DataSet,
                                        ByVal tableName As String,
                                        ByVal strFiltroImprese As String,
                                        ByVal tipo_Selezione As String,
                                        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim strErr As String
        Dim sSql As New System.Text.StringBuilder

        sSql.Length = 0
        Select Case tipo_Selezione
            Case "impianto"
                sSql.AppendLine(" SELECT DISTINCT Imprese.Piva, Imprese.Rag_Soc ")
                sSql.AppendLine(" FROM Reg_Impianti ")
                sSql.AppendLine(" LEFT JOIN Imprese ON Imprese.PIVA = Reg_Impianti.PIVA ")
            Case "azienda"
                sSql.AppendLine(" SELECT DISTINCT Piva, Rag_Soc ")
                sSql.AppendLine(" FROM Imprese ")
        End Select

        sSql.AppendLine(" WHERE 1 = 1 ")
        sSql.AppendLine(strFiltroImprese)

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            objSQL.EseguiQuery_Lettura(objParametri_Server, sSql.ToString, "Scheda_OP_Tipo_1.CaricaDS_Imprese", DSImprese, tableName)
        Catch ex As Exception
            strErr = ex.Message
        End Try

    End Function

    Public Sub DatiIntestazioneSocio(
       ByVal Piva As String,
       ByVal impresa As String,
       ByRef Vet_Intestazione() As String,
       ByRef log As String,
       ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
       ByVal Optional carica_cap As Boolean = False,
       ByVal Optional carica_nome_ref_coop As Boolean = False,
       ByVal Optional carica_coop_solo_se_singola As Boolean = False,
       ByVal Optional pivaPadre_preferito As String = "",
       ByVal Optional carica_recapito_tel As Boolean = False,
       ByVal Optional carica_codice_BP As Boolean = False,
       ByVal Optional filtro_impianti_coop_multiple As String = ""
   )

        'TODO: Vanni, Roberta ...Verificare se va bene fare qui la funzione (cioè nel Impresa_R, può essere fuorviante? viene usato solo per letture XML?) se va spostato, dove?

        Dim cod_contatto, rappr_legale, data_nascita, Codice_Fiscale As String
        Dim ind_residenza, com_residenza, prov_residenza, com_nascita, prov_nascita As String
        Dim rag_soc As String
        Dim ind_impresa, com_impresa, prov_impresa, cap_impresa, recapito_tel As String
        Dim piva_padre, cooperativa As String
        Dim codice_libro_soci, data_libro_soci, Cuaa As String
        Dim ind_coop, com_coop, prov_coop, cap_coop, nome_ref_coop, codice_BP As String

        Dim xLettura As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim xLetturaImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        Dim messaggio As String = ""
        Dim DT As DataTable = Nothing
        Dim DTInfo As DataTable = Nothing
        Dim DTcooperativeEsercizi As DataTable = Nothing

        Try

            DT = xLettura.DatiIntestazioneSocio_New(Piva, "", "", objParametri_Server)

            DTInfo = xLettura.RecuperaDatiImpresa_From_Piva(Piva, "", Nothing,
                                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                              "", "", objParametri_Server)
            'If filtro_impianti_coop_multiple <> "" Then
            '    DTcooperativeEsercizi = xLetturaImpianti.Leggi_CoopPadri_Impianti(Piva, filtro_impianti_coop_multiple, "", objParametri_Server)
            'End If

        Catch ex As Exception
            messaggio = ex.Message
        End Try


        If messaggio = "" Then
            If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

                cod_contatto = CStr(DT.Rows(0).Item("Cod_Contatto"))
                rappr_legale = CStr(DT.Rows(0).Item("Rappr_Legale"))

                '28/01/2019: gestita la lettura dal db
                data_nascita = CStr(DT.Rows(0).Item("data_nascita"))
                If data_nascita = "01/01/1900" Then
                    data_nascita = DataNascita_from_CodFisc(cod_contatto)
                End If

                Codice_Fiscale = CStr(DT.Rows(0).Item("Codice_Fiscale"))
                Cuaa = CStr(DT.Rows(0).Item("cuaa"))

                com_nascita = If(CStr(DT.Rows(0).Item("com_nascita")) = "Non Definita", "", CStr(DT.Rows(0).Item("com_nascita")))
                prov_nascita = If(CStr(DT.Rows(0).Item("prov_nascita")) = "00", "", CStr(DT.Rows(0).Item("prov_nascita")))

                If com_nascita = "" Or prov_nascita = "0" Then
                    log += CStr(Date.Now) + "   Impresa: " + impresa + ".   Non è stato inserito il luogo di nascita del legale rappresentante!" & vbCrLf & vbCrLf
                End If

                ind_residenza = If(CStr(DT.Rows(0).Item("ind_residenza")) = "&nbsp;", "", CStr(DT.Rows(0).Item("ind_residenza")))
                com_residenza = If(CStr(DT.Rows(0).Item("com_residenza")) = "Non Definita", "", CStr(DT.Rows(0).Item("com_residenza")))
                prov_residenza = If(CStr(DT.Rows(0).Item("prov_residenza")) = "00", "", CStr(DT.Rows(0).Item("prov_residenza")))

                If com_residenza = "" Or prov_residenza = "0" Then
                    log += CStr(Date.Now) + "   Impresa: " + impresa + ".   Non è stato inserito l'indirizzo e il luogo di residenza del legale rappresentante!" & vbCrLf & vbCrLf
                End If

                rag_soc = CStr(DT.Rows(0).Item("rag_soc"))
                ind_impresa = If(CStr(DT.Rows(0).Item("ind_impresa")) = "&nbsp;", "", CStr(DT.Rows(0).Item("ind_impresa")))
                com_impresa = If(CStr(DT.Rows(0).Item("com_impresa")) = "Non Definita", "", CStr(DT.Rows(0).Item("com_impresa")))
                prov_impresa = If(CStr(DT.Rows(0).Item("prov_impresa")) = "00", "", CStr(DT.Rows(0).Item("prov_impresa")))
                If (carica_cap) Then
                    cap_impresa = If(CStr(DT.Rows(0).Item("cap_impresa")) = "&nbsp;", "", CStr(DT.Rows(0).Item("cap_impresa")))
                End If
                If (carica_recapito_tel) Then
                    recapito_tel = CStr(DTInfo.Rows(0).Item("Telefono"))
                End If
                If (carica_codice_BP) Then
                    codice_BP = CStr(DT.Rows(0).Item("codice_bp"))
                End If

                If com_impresa = "" Or prov_impresa = "0" Then
                    log += CStr(Date.Now) + "   Impresa: " + impresa + ".   Non è stato inserito l'indirizzo dell'impresa!" & vbCrLf & vbCrLf
                End If

                Dim padreRow As DataRow = Nothing

                If (carica_coop_solo_se_singola And CInt(DT.Rows(0).Item("num_padri")) > 1) Then
                    piva_padre = ""
                    cooperativa = ""
                    ind_coop = ""
                    com_coop = ""
                    prov_coop = ""
                    cap_coop = ""
                    nome_ref_coop = ""
                    codice_libro_soci = ""
                    data_libro_soci = "01/01/1900"
                ElseIf (pivaPadre_preferito <> "") Then
                    For Each r In DT.Rows()
                        If r.Item("piva_padre") = pivaPadre_preferito Then
                            padreRow = r
                        End If
                    Next
                    If Not IsNothing(padreRow) Then
                        piva_padre = CStr(padreRow.Item("piva_padre"))
                        cooperativa = CStr(padreRow.Item("cooperativa"))
                        ind_coop = CStr(padreRow.Item("ind_coop"))
                        com_coop = If(CStr(DT.Rows(0).Item("com_coop")) = "Non Definita", "", CStr(DT.Rows(0).Item("com_coop")))
                        prov_coop = If(CStr(DT.Rows(0).Item("prov_coop")) = "00", "", CStr(DT.Rows(0).Item("prov_coop")))
                        cap_coop = CStr(padreRow.Item("cap_coop"))
                        codice_libro_soci = CStr(padreRow.Item("codice_libro_soci"))
                        data_libro_soci = CStr(padreRow.Item("data_libro_soci"))
                    Else
                        piva_padre = ""
                        cooperativa = ""
                        ind_coop = ""
                        com_coop = ""
                        prov_coop = ""
                        cap_coop = ""
                        nome_ref_coop = ""
                        codice_libro_soci = ""
                        data_libro_soci = "01/01/1900"
                    End If
                Else
                    piva_padre = CStr(DT.Rows(0).Item("piva_padre"))
                    cooperativa = CStr(DT.Rows(0).Item("cooperativa"))
                    ind_coop = CStr(DT.Rows(0).Item("ind_coop"))
                    com_coop = CStr(DT.Rows(0).Item("com_coop"))
                    prov_coop = CStr(DT.Rows(0).Item("prov_coop"))
                    cap_coop = CStr(DT.Rows(0).Item("cap_coop"))
                    codice_libro_soci = CStr(DT.Rows(0).Item("codice_libro_soci"))
                    data_libro_soci = CStr(DT.Rows(0).Item("data_libro_soci"))
                End If

                If Not IsNothing(DTcooperativeEsercizi) AndAlso DTcooperativeEsercizi.Rows().Count > 0 Then
                    piva_padre = String.Join(", ", DTcooperativeEsercizi.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("piva_padre"))) _
                                .Select(Function(row) row.Field(Of String)("piva_padre")))
                    cooperativa = String.Join(", ", DTcooperativeEsercizi.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("cooperativa"))) _
                                .Select(Function(row) row.Field(Of String)("cooperativa")))
                    ind_coop = String.Join(", ", DTcooperativeEsercizi.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("ind_coop"))) _
                                .Select(Function(row) row.Field(Of String)("ind_coop")))
                    com_coop = String.Join(", ", DTcooperativeEsercizi.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("com_coop")) _
                                And Not row.Field(Of String)("com_coop") = "Non Definita") _
                                .Select(Function(row) row.Field(Of String)("com_coop")))
                    prov_coop = String.Join(", ", DTcooperativeEsercizi.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("prov_coop")) _
                                And Not row.Field(Of String)("prov_coop") = "00") _
                                .Select(Function(row) row.Field(Of String)("prov_coop")))
                    cap_coop = String.Join(", ", DTcooperativeEsercizi.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("cap_coop"))) _
                                .Select(Function(row) row.Field(Of String)("cap_coop")))
                    codice_libro_soci = String.Join(", ", DTcooperativeEsercizi.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("codice_libro_soci"))) _
                                .Select(Function(row) row.Field(Of String)("codice_libro_soci")))
                    data_libro_soci = String.Join(", ", DTcooperativeEsercizi.AsEnumerable() _
                                .Where(Function(row) Not IsNothing(row.Field(Of Date)("data_libro_soci")) _
                                And Not row.Field(Of Date)("data_libro_soci").ToString("dd/MM/yyyy") = "01/01/1900") _
                                .Select(Function(row) row.Field(Of Date)("data_libro_soci").ToString("dd/MM/yyyy")))
                End If

                If com_coop = "" Or prov_coop = "0" Then
                    log += CStr(Date.Now) + "   Impresa: " + impresa + ".   Non è stato inserito l'indirizzo della cooperativa a cui l'impresa appartiene!" & vbCrLf & vbCrLf
                End If

                If codice_libro_soci = " " Then
                    log += CStr(Date.Now) + "   Impresa: " + impresa + ".   Non è stato inserito il codice libro soci!" & vbCrLf & vbCrLf
                End If

                If data_libro_soci = "01/01/1900" Then
                    log += CStr(Date.Now) + "   Impresa: " + impresa + ".   Non è stata inserita la data di iscrizione al libro soci!" & vbCrLf & vbCrLf
                    data_libro_soci = ""
                End If

                Vet_Intestazione(0) = cod_contatto
                Vet_Intestazione(1) = rappr_legale
                Vet_Intestazione(2) = com_nascita
                Vet_Intestazione(3) = prov_nascita
                Vet_Intestazione(4) = data_nascita
                Vet_Intestazione(5) = ind_residenza
                Vet_Intestazione(6) = com_residenza
                Vet_Intestazione(7) = prov_residenza
                Vet_Intestazione(8) = "legale rappresentante"
                Vet_Intestazione(9) = Piva
                Vet_Intestazione(10) = rag_soc
                Vet_Intestazione(11) = ind_impresa
                Vet_Intestazione(12) = com_impresa
                Vet_Intestazione(13) = prov_impresa
                Vet_Intestazione(14) = piva_padre
                Vet_Intestazione(15) = cooperativa
                Vet_Intestazione(16) = ind_coop
                Vet_Intestazione(17) = cap_coop
                Vet_Intestazione(18) = com_coop
                Vet_Intestazione(19) = prov_coop
                Vet_Intestazione(20) = codice_libro_soci
                Vet_Intestazione(21) = data_libro_soci
                Vet_Intestazione(22) = objParametri_Server.PivaSuperUser 'CStr(objSession("ASG_SuperUser_CodFiscale"))
                Vet_Intestazione(23) = xLettura.RagSoc_from_Piva(objParametri_Server.PivaSuperUser, objParametri_Server) 'CStr(objSession("ASG_SuperUser_Username"))
                Vet_Intestazione(24) = Codice_Fiscale
                Vet_Intestazione(25) = Cuaa
                If (carica_cap) Then
                    Vet_Intestazione(26) = cap_impresa
                End If
                If (carica_nome_ref_coop) Then
                    Vet_Intestazione(27) = nome_ref_coop
                End If
                If (carica_recapito_tel) Then
                    Vet_Intestazione(28) = recapito_tel
                End If
                If (carica_codice_BP) Then
                    Vet_Intestazione(29) = codice_BP
                End If
            Else
                log += CStr(Date.Now) + "   Non sono stati trovati dati per l'INTESTAZIONE:  " + messaggio & vbCrLf & vbCrLf
                For i = 0 To Vet_Intestazione.Length - 1
                    Vet_Intestazione(i) = ""
                Next
            End If
        End If

    End Sub

    Public Sub DatiIntestazioneSoci(ByRef listaSoci As Dictionary(Of String, String()),
                                   ByRef listaPive As List(Of String),
                                   ByRef log As String,
                                   ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByVal Optional carica_cap As Boolean = False,
                                   ByVal Optional carica_nome_ref_coop As Boolean = False,
                                   ByVal Optional carica_coop_solo_se_singola As Boolean = False,
                                   ByVal Optional pivaPadre_preferito As String = "",
                                   ByVal Optional carica_recapito_tel As Boolean = False,
                                   ByVal Optional carica_codice_BP As Boolean = False,
                                   ByVal Optional filtro_impianti_coop_multiple As String = "",
                                   ByVal Optional tipo_selezione As String = ""
                               )

        Dim cod_contatto, rappr_legale, data_nascita, Codice_Fiscale As String
        Dim ind_residenza, com_residenza, prov_residenza, com_nascita, prov_nascita As String
        Dim rag_soc As String
        Dim ind_impresa, com_impresa, prov_impresa, cap_impresa, recapito_tel As String
        Dim piva_padre, cooperativa As String
        Dim codice_libro_soci, data_libro_soci, Cuaa As String
        Dim ind_coop, com_coop, prov_coop, cap_coop, nome_ref_coop, codice_BP, indirizzo_coop_multiple As String
        Dim xLettura As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim xLetturaImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        Dim messaggio As String = ""
        Dim DT As DataTable = Nothing
        Dim DTInfo As DataTable = Nothing
        Dim DTcooperativeEsercizi As DataTable = Nothing
        Dim Vet_Intestazione As String()

        Try

            DT = xLettura.DatiIntestazioneSoci_New(listaSoci.Keys.ToList(), "", "", objParametri_Server)

            DTInfo = xLettura.RecuperaDatiImpresa_From_Pive(listaSoci.Keys.ToList(), "", Nothing,
                                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                              "", "", objParametri_Server)

            If filtro_impianti_coop_multiple <> "" And tipo_selezione <> "" Then
                DTcooperativeEsercizi = xLetturaImpianti.Leggi_CoopPadri_Impianti(listaPive, filtro_impianti_coop_multiple, tipo_selezione, "", objParametri_Server)
            End If

        Catch ex As Exception
            messaggio = ex.Message
        End Try


        If messaggio = "" Then
            For Each piva In listaSoci.Keys.ToList()

                Dim pivaReale As String = piva

                Vet_Intestazione = New String(30) {}

                Dim current = If(DT.Select($"Piva = '{ piva }'").Any(),
                    DT.Select($"Piva = '{ piva }'").CopyToDataTable(),
                    DT.Clone())

                Dim currentInfo = If(DTInfo.Select($"Piva = '{ piva }'").Any(),
                    DTInfo.Select($"Piva = '{ piva }'").CopyToDataTable(),
                    DTInfo.Clone())

                carica_recapito_tel = (Not IsNothing(currentInfo)) AndAlso (currentInfo.Rows.Count > 0)

                Dim currentCooperative = If(DTcooperativeEsercizi.Select($"piva_padre = '{ piva }'").Any(),
                    DTcooperativeEsercizi.Select($"piva_padre = '{ piva }'").CopyToDataTable(),
                    DTcooperativeEsercizi.Clone())

                If current IsNot Nothing AndAlso current.Rows.Count <> 0 Then

                    pivaReale = CStr(current.Rows(0).Item("PivaReale"))

                    cod_contatto = CStr(current.Rows(0).Item("Cod_Contatto"))

                    rappr_legale = CStr(current.Rows(0).Item("Rappr_Legale"))

                    '28/01/2019: gestita la lettura dal db
                    data_nascita = CStr(current.Rows(0).Item("data_nascita"))

                    If data_nascita = "01/01/1900" Then
                        data_nascita = DataNascita_from_CodFisc(cod_contatto)
                    End If

                    Codice_Fiscale = CStr(current.Rows(0).Item("Codice_Fiscale"))

                    Cuaa = CStr(current.Rows(0).Item("cuaa"))

                    com_nascita = If(CStr(current.Rows(0).Item("com_nascita")) = "Non Definita", "", CStr(current.Rows(0).Item("com_nascita")))

                    Dim temp_prov_nascita = CStr(current.Rows(0).Item("prov_nascita"))

                    prov_nascita = If(temp_prov_nascita = "00", "", temp_prov_nascita)

                    If com_nascita = "" Or prov_nascita = "0" Then
                        log += CStr(Date.Now) + "   Non è stato inserito il luogo di nascita del legale rappresentante!" & vbCrLf & vbCrLf
                    End If

                    Dim temp_ind_nascita = CStr(current.Rows(0).Item("ind_residenza"))

                    ind_residenza = If(temp_ind_nascita = "&nbsp;", "", temp_ind_nascita)

                    Dim temp_com_residenza = CStr(current.Rows(0).Item("com_residenza"))

                    com_residenza = If(temp_com_residenza = "Non Definita", "", temp_com_residenza)

                    Dim temp_prov_residenza = CStr(current.Rows(0).Item("prov_residenza"))

                    prov_residenza = If(temp_prov_residenza = "00", "", temp_prov_residenza)

                    If com_residenza = "" Or prov_residenza = "0" Then
                        log += CStr(Date.Now) + "   Non è stato inserito l'indirizzo e il luogo di residenza del legale rappresentante!" & vbCrLf & vbCrLf
                    End If

                    rag_soc = CStr(current.Rows(0).Item("rag_soc"))

                    Dim temp_ind_impresa = CStr(current.Rows(0).Item("ind_impresa"))

                    ind_impresa = If(temp_ind_impresa = "&nbsp;", "", temp_ind_impresa)

                    Dim temp_com_impresa = CStr(current.Rows(0).Item("com_impresa"))

                    com_impresa = If(temp_com_impresa = "Non Definita", "", temp_com_impresa)

                    Dim temp_prov_impresa = CStr(current.Rows(0).Item("prov_impresa"))


                    prov_impresa = If(temp_prov_impresa = "00", "", temp_prov_impresa)
                    If (carica_cap) Then

                        Dim temp_cap_impresa = CStr(current.Rows(0).Item("cap_impresa"))


                        cap_impresa = If(temp_cap_impresa = "&nbsp;", "", temp_cap_impresa)
                    End If
                    If (carica_recapito_tel) Then
                        recapito_tel = CStr(currentInfo.Rows(0).Item("Telefono"))

                    End If
                    If (carica_codice_BP) Then
                        codice_BP = CStr(current.Rows(0).Item("codice_bp"))

                    End If

                    If com_impresa = "" Or prov_impresa = "0" Then
                        log += CStr(Date.Now) + "   Non è stato inserito l'indirizzo dell'impresa!" & vbCrLf & vbCrLf
                    End If

                    Dim padreRow As DataRow = Nothing
                    Dim numPadri = CInt(current.Rows(0).Item("num_padri"))


                    If (carica_coop_solo_se_singola And numPadri > 1) Then
                        piva_padre = ""
                        cooperativa = ""
                        ind_coop = ""
                        com_coop = ""
                        prov_coop = ""
                        cap_coop = ""
                        nome_ref_coop = ""
                        codice_libro_soci = ""
                        data_libro_soci = "01/01/1900"
                        indirizzo_coop_multiple = ""
                    ElseIf (pivaPadre_preferito <> "") Then
                        For Each r In current.Rows()
                            If r.Item("piva_padre") = pivaPadre_preferito Then
                                padreRow = r
                            End If
                        Next
                        If Not IsNothing(padreRow) Then
                            piva_padre = CStr(padreRow.Item("piva_padre"))
                            cooperativa = CStr(padreRow.Item("cooperativa"))
                            ind_coop = CStr(padreRow.Item("ind_coop"))
                            com_coop = If(CStr(current.Rows(0).Item("com_coop")) = "Non Definita", "", CStr(current.Rows(0).Item("com_coop")))
                            prov_coop = If(CStr(current.Rows(0).Item("prov_coop")) = "00", "", CStr(current.Rows(0).Item("prov_coop")))
                            cap_coop = CStr(padreRow.Item("cap_coop"))
                            codice_libro_soci = CStr(padreRow.Item("codice_libro_soci"))
                            data_libro_soci = CStr(padreRow.Item("data_libro_soci"))
                        Else
                            piva_padre = ""
                            cooperativa = ""
                            ind_coop = ""
                            com_coop = ""
                            prov_coop = ""
                            cap_coop = ""
                            nome_ref_coop = ""
                            codice_libro_soci = ""
                            data_libro_soci = "01/01/1900"
                            indirizzo_coop_multiple = ""
                        End If
                    Else
                        piva_padre = CStr(current.Rows(0).Item("piva_padre"))

                        cooperativa = CStr(current.Rows(0).Item("cooperativa"))

                        ind_coop = CStr(current.Rows(0).Item("ind_coop"))

                        com_coop = CStr(current.Rows(0).Item("com_coop"))

                        prov_coop = CStr(current.Rows(0).Item("prov_coop"))

                        cap_coop = CStr(current.Rows(0).Item("cap_coop"))

                        codice_libro_soci = CStr(current.Rows(0).Item("codice_libro_soci"))

                        data_libro_soci = CStr(current.Rows(0).Item("data_libro_soci"))

                    End If

                    'Fix per ticket 197276 07/11/25 AndAlso currentCooperative.Rows().Count > 1
                    If Not IsNothing(currentCooperative) Then

                        piva_padre = ""
                        cooperativa = ""
                        ind_coop = ""
                        com_coop = ""
                        prov_coop = ""
                        cap_coop = ""
                        nome_ref_coop = ""
                        codice_libro_soci = ""
                        data_libro_soci = "01/01/1900"
                        indirizzo_coop_multiple = ""

                        piva_padre = String.Join(", ", currentCooperative.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("piva_padre"))) _
                                .Select(Function(row) row.Field(Of String)("piva_padre")))
                        cooperativa = String.Join(", ", currentCooperative.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("rag_soc_padre"))) _
                                .Select(Function(row) row.Field(Of String)("rag_soc_padre")))
                        ind_coop = String.Join(", ", currentCooperative.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("ind_coop"))) _
                                .Select(Function(row) row.Field(Of String)("ind_coop")))
                        com_coop = String.Join(", ", currentCooperative.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("com_coop")) _
                                And Not row.Field(Of String)("com_coop") = "Non Definita") _
                                .Select(Function(row) row.Field(Of String)("com_coop")))
                        prov_coop = String.Join(", ", currentCooperative.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("prov_coop")) _
                                And Not row.Field(Of String)("prov_coop") = "00") _
                                .Select(Function(row) row.Field(Of String)("prov_coop")))
                        cap_coop = String.Join(", ", currentCooperative.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("cap_coop"))) _
                                .Select(Function(row) row.Field(Of String)("cap_coop")))
                        codice_libro_soci = String.Join(", ", currentCooperative.AsEnumerable() _
                                .Where(Function(row) Not String.IsNullOrEmpty(row.Field(Of String)("codice_libro_soci"))) _
                                .Select(Function(row) row.Field(Of String)("codice_libro_soci")))
                        data_libro_soci = String.Join(", ", currentCooperative.AsEnumerable() _
                                .Where(Function(row) Not IsNothing(row.Field(Of Date)("data_libro_soci")) _
                                And Not row.Field(Of Date)("data_libro_soci").ToString("dd/MM/yyyy") = "01/01/1900") _
                                .Select(Function(row) row.Field(Of Date)("data_libro_soci").ToString("dd/MM/yyyy")))

                        indirizzo_coop_multiple = String.Join(", ",
                            currentCooperative.AsEnumerable().
                            Select(Function(row)
                                       Dim coop_indirizzi = New List(Of String) From {
                                            If(Not IsDBNull(row("com_coop")) AndAlso Not String.IsNullOrEmpty(row.Field(Of String)("com_coop")) AndAlso row.Field(Of String)("com_coop") <> "Non Definita", row.Field(Of String)("com_coop"), Nothing),
                                            If(Not IsDBNull(row("prov_coop")) AndAlso Not String.IsNullOrEmpty(row.Field(Of String)("prov_coop")) AndAlso row.Field(Of String)("prov_coop") <> "00", " (" + row.Field(Of String)("prov_coop") + ")", Nothing),
                                            If(Not IsDBNull(row("ind_coop")) AndAlso Not String.IsNullOrEmpty(row.Field(Of String)("ind_coop")), ", " + row.Field(Of String)("ind_coop"), Nothing),
                                            If(Not IsDBNull(row("cap_coop")) AndAlso Not String.IsNullOrEmpty(row.Field(Of String)("cap_coop")), ", " + row.Field(Of String)("cap_coop"), Nothing)
                                       }
                                       Return String.Join("", coop_indirizzi.Where(Function(coop_indirizzo) coop_indirizzo IsNot Nothing))
                                   End Function))

                    End If

                    If com_coop = "" Or prov_coop = "0" Then
                        log += CStr(Date.Now) + "   Non è stato inserito l'indirizzo della cooperativa a cui l'impresa appartiene!" & vbCrLf & vbCrLf
                    End If

                    If codice_libro_soci = " " Then
                        log += CStr(Date.Now) + "   Non è stato inserito il codice libro soci!" & vbCrLf & vbCrLf
                    End If

                    If data_libro_soci = "01/01/1900" Then
                        log += CStr(Date.Now) + "   Non è stata inserita la data di iscrizione al libro soci!" & vbCrLf & vbCrLf
                        data_libro_soci = ""
                    End If

                    Vet_Intestazione(0) = cod_contatto
                    Vet_Intestazione(1) = rappr_legale
                    Vet_Intestazione(2) = com_nascita
                    Vet_Intestazione(3) = prov_nascita
                    Vet_Intestazione(4) = data_nascita
                    Vet_Intestazione(5) = ind_residenza
                    Vet_Intestazione(6) = com_residenza
                    Vet_Intestazione(7) = prov_residenza
                    Vet_Intestazione(8) = "legale rappresentante"
                    Vet_Intestazione(9) = pivaReale
                    Vet_Intestazione(10) = rag_soc
                    Vet_Intestazione(11) = ind_impresa
                    Vet_Intestazione(12) = com_impresa
                    Vet_Intestazione(13) = prov_impresa
                    Vet_Intestazione(14) = piva_padre
                    Vet_Intestazione(15) = cooperativa
                    Vet_Intestazione(16) = ind_coop
                    Vet_Intestazione(17) = cap_coop
                    Vet_Intestazione(18) = com_coop
                    Vet_Intestazione(19) = prov_coop
                    Vet_Intestazione(20) = codice_libro_soci
                    Vet_Intestazione(21) = data_libro_soci
                    Vet_Intestazione(22) = objParametri_Server.PivaSuperUser 'CStr(objSessioncurrent.Rows(0).Item("ASG_SuperUser_CodFiscale")))
                    Vet_Intestazione(23) = xLettura.RagSoc_from_Piva(objParametri_Server.PivaSuperUser, objParametri_Server) 'CStr(objSessioncurrent.Rows(0).Item("ASG_SuperUser_Username")))
                    Vet_Intestazione(24) = Codice_Fiscale
                    Vet_Intestazione(25) = Cuaa
                    If (carica_cap) Then
                        Vet_Intestazione(26) = cap_impresa
                    End If
                    If (carica_nome_ref_coop) Then
                        Vet_Intestazione(27) = nome_ref_coop
                    End If
                    If (carica_recapito_tel) Then
                        Vet_Intestazione(28) = recapito_tel
                    End If
                    If (carica_codice_BP) Then
                        Vet_Intestazione(29) = codice_BP
                    End If
                    Vet_Intestazione(30) = indirizzo_coop_multiple

                Else
                    log += CStr(Date.Now) + "   Non sono stati trovati dati per l'INTESTAZIONE:  " + messaggio & vbCrLf & vbCrLf
                    For i = 0 To Vet_Intestazione.Length - 1
                        Vet_Intestazione(i) = ""
                    Next
                End If

                listaSoci.Item(piva) = Vet_Intestazione

            Next
        End If

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <param name="piva"></param>
    ''' <param name="tipoIntervalloTemporale">0 = Impianti attivi nell'anno selezionato; 1 = impianti nati nell'anno selezionato</param>
    ''' <param name="validitaInizio"></param>
    ''' <param name="validitaFine"></param>
    ''' <returns></returns>
    Public Function SuperficieSpecieConsuntivo(ByRef objParametri As AgronicaCoreParametri, ByVal piva As String, ByVal tipoIntervalloTemporale As Integer, ByVal validitaInizio As Date, ByVal validitaFine As Date) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Stampe_OP.SuperficieSpecieConsuntivo()"
        Dim stbQ As New Text.StringBuilder
        Dim dt As DataTable

        Try
            stbQ.AppendLine(" SELECT        SpecieVegetali.Veg_Des, SUM(Reg_Impianti.Sup_Imp) AS Sup_Specie, CONVERT(varchar, SUM(Reg_Impianti.Sup_Imp), 0) AS Str_Sup_Specie ")
            stbQ.AppendLine(" FROM          SpecieVegetali INNER JOIN ")
            stbQ.AppendLine("               Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod INNER JOIN ")
            stbQ.AppendLine("               Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
            stbQ.AppendLine(" WHERE         Reg_Impianti.PIVA = '" + UtilityProvider.Agro_SQL_SaveText(piva) + "' ")

            Select Case tipoIntervalloTemporale

                Case 0 'impianti attivi nell'anno selezionato
                    'Impianti con Data Inizio <= 31/12/Anno e Data Fine >= Validità Inizio
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Fine >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))

                Case 1 'impianti nati nell'anno selezionato
                    'Impianti con Data Inizio >= Validità Inizio e Data Inizio <= 31/12/Anno
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))

            End Select

            stbQ.AppendLine(" GROUP BY      SpecieVegetali.Veg_Des ")
            stbQ.AppendLine(" ORDER BY      SpecieVegetali.Veg_Des ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <param name="piva"></param>
    ''' <param name="filtroImpianti"></param>
    ''' <param name="tipoIntervalloTemporale">0 = Impianti attivi nell'anno selezionato; 1 = impianti nati nell'anno selezionato</param>
    ''' <param name="validitaInizio"></param>
    ''' <param name="validitaFine"></param>
    ''' <returns></returns>
    Public Function SuperficieSpecieConsuntivoFiltroImpianti(ByRef objParametri As AgronicaCoreParametri, ByVal piva As String, ByVal strFiltroImpianti As String, ByVal tipoIntervalloTemporale As Integer, ByVal validitaInizio As Date, ByVal validitaFine As Date) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Stampe_OP.SuperficieSpecieConsuntivoFiltroImpianti()"
        Dim stbQ As New Text.StringBuilder
        Dim dt As DataTable

        Try
            stbQ.AppendLine(" SELECT        CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Reg_Impianti.PIVA ELSE Imprese.partitaIvaReale END AS Piva,")
            stbQ.AppendLine("               SpecieVegetali.Veg_Des, SUM(Reg_Impianti.Sup_Imp) AS Sup_Specie, CONVERT(varchar, SUM(Reg_Impianti.Sup_Imp), 0) AS Str_Sup_Specie ")
            stbQ.AppendLine(" FROM          SpecieVegetali INNER JOIN ")
            stbQ.AppendLine("               Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")
            stbQ.AppendLine(" INNER JOIN    Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
            stbQ.AppendLine(" INNER JOIN    Imprese ON Reg_Impianti.PIVA = Imprese.PIVA ")
            stbQ.AppendLine(" WHERE 1 = 1")
            If strFiltroImpianti IsNot Nothing Then
                stbQ.AppendLine(strFiltroImpianti)
            Else
                stbQ.AppendLine("And Reg_Impianti.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            Select Case tipoIntervalloTemporale

                Case 0 'impianti attivi nell'anno selezionato
                    'Impianti con Data Inizio <= 31/12/Anno e Data Fine >= Validità Inizio
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Fine >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))

                Case 1 'impianti nati nell'anno selezionato
                    'Impianti con Data Inizio >= Validità Inizio e Data Inizio <= 31/12/Anno
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))

            End Select

            stbQ.AppendLine(" GROUP BY      SpecieVegetali.Veg_Des, Reg_Impianti.PIVA, Imprese.partitaIvaReale ")
            stbQ.AppendLine(" ORDER BY      SpecieVegetali.Veg_Des ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <param name="piva"></param>
    ''' <param name="tipoIntervalloTemporale">0 = Impianti attivi nell'anno selezionato; 1 = impianti nati nell'anno selezionato</param>
    ''' <param name="validitaInizio"></param>
    ''' <param name="validitaFine"></param>
    ''' <returns></returns>
    Public Function SuperficieSpeciePrevisionale(ByRef objParametri As AgronicaCoreParametri, ByVal piva As String, ByVal tipoIntervalloTemporale As Integer, ByVal validitaInizio As Date, ByVal validitaFine As Date) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Stampe_OP.SuperficieSpecieConsuntivo()"
        Dim stbQ As New Text.StringBuilder
        Dim dt As DataTable

        Try

            ' AND ((Campo_Cod<>0 AND Appezza=0) OR (Campo_Cod=0 AND Appezza<>0))
            ' (Campo_Cod<>0 AND Appezza=0) è per le pianificazioni quindicinali e serve per conteggiare solo la superficie del campo
            ' (Campo_Cod=0 AND Appezza<>0) è per le pianificazioni annuali, dove non esiste il campo e bisogna conteggiare le sup degli appezzamenti

            ' la pianificazione mensile ancora non è gestita, ma ricadrebbe nel primo caso (Campo_Cod<>0 AND Appezza=0)

            stbQ.AppendLine(" ")
            stbQ.AppendLine(" SELECT        SpecieVegetali.Veg_Des, SUM(Programmazione_Entita.Superficie) AS Sup_Specie, CONVERT(varchar, SUM(Programmazione_Entita.Superficie), 0) AS Str_Sup_Specie ")
            stbQ.AppendLine(" FROM          SpecieVegetali ")
            stbQ.AppendLine("               INNER JOIN Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            stbQ.AppendLine("               INNER JOIN Programmazione_Entita ON Cultivar.Cul_Cod = Programmazione_Entita.CUL_COD ")
            stbQ.AppendLine("               INNER JOIN Programmazione_Testata ON  Programmazione_Entita.Programmazione_Cod=Programmazione_Testata.Programmazione_Cod ")
            stbQ.AppendLine("               AND Programmazione_Entita.Piva_SuperUser=Programmazione_Testata.Piva_SuperUser ")
            stbQ.AppendLine(" WHERE         Programmazione_Entita.PIVA = '" + UtilityProvider.Agro_SQL_SaveText(piva) + "' ")

            Select Case tipoIntervalloTemporale

                Case 0 'impianti attivi nell'anno selezionato
                    'Impianti con Data Inizio <= 31/12/Anno e Data Fine >= Validità Inizio
                    stbQ.AppendLine(" AND           Programmazione_Entita.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))
                    stbQ.AppendLine(" AND           Programmazione_Entita.Validita_Fine  >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))

                Case 1 'impianti nati nell'anno selezionato
                    'Impianti con Data Inizio >= Validità Inizio e Data Inizio <= 31/12/Anno
                    stbQ.AppendLine(" AND           Programmazione_Entita.Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))
                    stbQ.AppendLine(" AND           Programmazione_Entita.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))

            End Select

            stbQ.AppendLine("               AND ((Campo_Cod<>0 AND Appezza=0) OR (Campo_Cod=0 AND Appezza<>0)) ")   ' Vedi commento sopra
            stbQ.AppendLine(" GROUP BY      SpecieVegetali.Veg_Des ")
            stbQ.AppendLine(" ORDER BY      SpecieVegetali.Veg_Des ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function SuperficieSpecieConsuntivoxCentri(ByRef objParametri As AgronicaCoreParametri, ByVal strParametri As String, ByVal piva As String, ByVal tipoIntervalloTemporale As Integer, ByVal validitaInizio As Date, ByVal validitaFine As Date, Optional Tipo_Selezione As String = "") As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Stampe_OP.SuperficieSpecieConsuntivo()"
        Dim stbQ As New Text.StringBuilder
        Dim dt As DataTable
        Dim filtroImpianti = strParametri

        Select Case Tipo_Selezione
            Case "azienda"
                'Formatto il filtro per la query SQL in base alle aziende fornite
                filtroImpianti = filtroImpianti.Replace("Imprese", "Reg_Impianti")
        End Select

        Try
            stbQ.AppendLine(" SELECT        Reg_Impianti.Sa_Cod, ")
            stbQ.AppendLine("               Reg_Impianti.PIVA, ")
            stbQ.AppendLine("               SUM(Reg_Impianti.Sup_Imp) As Sup_Specie, ")
            stbQ.AppendLine("               Convert(varchar, SUM(Reg_Impianti.Sup_Imp), 0) As Str_Sup_Specie, ")
            stbQ.AppendLine("               ISNULL(SpecieVegetali.Veg_Des, ISNULL(cod.descrizione, '')) AS Veg_Des ")
            stbQ.AppendLine(" FROM Reg_Impianti LEFT Join ")
            stbQ.AppendLine("               Cultivar On Reg_Impianti.CUL_COD = Cultivar.Cul_Cod LEFT Join ")
            stbQ.AppendLine("               SpecieVegetali On Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            stbQ.AppendLine("               Left Join Reg_Impianti_Codici ric ON ")
            stbQ.AppendLine("               ric.PIVA = Reg_Impianti.PIVA ")
            stbQ.AppendLine("               And ric.sa_cod = Reg_Impianti.SA_COD ")
            stbQ.AppendLine("               And ric.appezza = Reg_Impianti.APPEZZA ")
            stbQ.AppendLine("               And ric.Id_Reg = Reg_Impianti.ID_REG ")
            stbQ.AppendLine("               And ric.Progetto_Cod = 0 ")
            stbQ.AppendLine("               And ric.id_cod BETWEEN 3000 And 4000 ")
            stbQ.AppendLine("               Left Join Codici_Anagrafe cod ON ")
            stbQ.AppendLine("               cod.codice = ric.id_cod")
            stbQ.AppendLine(" WHERE       1 = 1")
            If filtroImpianti IsNot Nothing And filtroImpianti <> "" Then
                stbQ.AppendLine(filtroImpianti)
                stbQ.AppendLine(" AND         Reg_Impianti.PIVA = '" + UtilityProvider.Agro_SQL_SaveText(piva) + "' ")
            Else
                stbQ.AppendLine(" AND         Reg_Impianti.PIVA = '" + UtilityProvider.Agro_SQL_SaveText(piva) + "' ")
            End If

            Select Case tipoIntervalloTemporale

                Case 0 'impianti attivi nell'anno selezionato
                    'Impianti con Data Inizio <= 31/12/Anno e Data Fine >= Validità Inizio
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Fine >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))

                Case 1 'impianti nati nell'anno selezionato
                    'Impianti con Data Inizio >= Validità Inizio e Data Inizio <= 31/12/Anno
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))

            End Select

            stbQ.AppendLine(" GROUP BY      Reg_Impianti.Sa_Cod, Reg_Impianti.PIVA, SpecieVegetali.Veg_Des, cod.descrizione ")
            stbQ.AppendLine(" ORDER BY      SpecieVegetali.Veg_Des ")



            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function SuperficieSpecieConsuntivoxCentriMultiPiva(ByRef objParametri As AgronicaCoreParametri, ByVal strParametri As String, ByVal listaPiva As List(Of String), ByVal tipoIntervalloTemporale As Integer, ByVal validitaInizio As Date, ByVal validitaFine As Date, Optional Tipo_Selezione As String = "") As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Stampe_OP.SuperficieSpecieConsuntivo()"
        Dim stbQ As New Text.StringBuilder
        Dim dt As DataTable

        Try
            If listaPiva Is Nothing OrElse listaPiva.Count = 0 Then
                Throw New Exception("La lista di PIVA è vuota.")
            End If

            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroPiva(listaPiva, NomeRoutine, objParametri)

            stbQ.AppendLine(" SELECT CASE ")
            stbQ.AppendLine("               WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Reg_Impianti.PIVA ")
            stbQ.AppendLine("               ELSE Imprese.partitaIvaReale ")
            stbQ.AppendLine("               END PivaReale, ")
            stbQ.AppendLine("               Reg_Impianti.Sa_Cod, ")
            stbQ.AppendLine("               Reg_Impianti.PIVA, ")
            stbQ.AppendLine("               SUM(Reg_Impianti.Sup_Imp) As Sup_Specie, ")
            stbQ.AppendLine("               Convert(varchar, SUM(Reg_Impianti.Sup_Imp), 0) As Str_Sup_Specie, ")
            stbQ.AppendLine("               ISNULL(SpecieVegetali.Veg_Des, ISNULL(cod.descrizione, '')) AS Veg_Des ")
            stbQ.AppendLine(" FROM Reg_Impianti LEFT Join ")
            stbQ.AppendLine("               Imprese On Reg_Impianti.Piva = Imprese.Piva LEFT Join ")
            stbQ.AppendLine("               Cultivar On Reg_Impianti.CUL_COD = Cultivar.Cul_Cod LEFT Join ")
            stbQ.AppendLine("               SpecieVegetali On Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            stbQ.AppendLine("               INNER JOIN #TempPiva tmp ON tmp.Piva = Reg_Impianti.Piva COLLATE DATABASE_DEFAULT ")
            stbQ.AppendLine("               Left Join Reg_Impianti_Codici ric ON ")
            stbQ.AppendLine("               ric.PIVA = Reg_Impianti.PIVA ")
            stbQ.AppendLine("               And ric.sa_cod = Reg_Impianti.SA_COD ")
            stbQ.AppendLine("               And ric.appezza = Reg_Impianti.APPEZZA ")
            stbQ.AppendLine("               And ric.Id_Reg = Reg_Impianti.ID_REG ")
            stbQ.AppendLine("               And ric.Progetto_Cod = 0 ")
            stbQ.AppendLine("               And ric.id_cod BETWEEN 3000 And 4000 ")
            stbQ.AppendLine("               Left Join Codici_Anagrafe cod ON ")
            stbQ.AppendLine("               cod.codice = ric.id_cod")
            stbQ.AppendLine(" WHERE       1 = 1")

            Select Case tipoIntervalloTemporale
                Case 0 ' Impianti attivi nell'anno selezionato
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Fine >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))
                Case 1 ' Impianti nati nell'anno selezionato
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(validitaInizio))
                    stbQ.AppendLine("               AND Reg_Impianti.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine))
            End Select

            stbQ.AppendLine(" GROUP BY      Reg_Impianti.Sa_Cod, Reg_Impianti.PIVA, SpecieVegetali.Veg_Des, cod.descrizione, Imprese.partitaIvaReale  ")
            stbQ.AppendLine(" ORDER BY      SpecieVegetali.Veg_Des ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroPiva(NomeRoutine, objParametri)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            dt = Nothing
            'rollback transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return dt

    End Function

End Class
