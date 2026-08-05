
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json.Linq

Public Class AgeaCodifiche

    Public Function ReadAgeaMachineCodes(
                                        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal AGEA_Cod As String = "",
                                        Optional ByVal AGEA_Des As String = "",
                                        Optional ByVal Class_Cod As String = ""
                                        ) As DataTable

        Dim objCodificaMacchine As New Codifica_Macchine_Agea
        Dim dt = objCodificaMacchine.leggi(
            objParametri_Server,
            AGEA_Cod,
            AGEA_Des,
            Class_Cod
            )

        Return dt
    End Function

    ''' <summary>
    ''' Imposta su un datatable le colonne corrispondenti a codici specie, cultivar Agea 2015-2020 partendo dalla chiave dell'impianto (possono essere anche ripetuti sul datatable) 
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="DtDatiConImpianti">Datatable contenente i dati fra cui le colonne sa_cod, appezza, id_reg, anche ripetuti</param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="NomeColonnaCodiceSpecie"></param>
    ''' <param name="NomeColonnaCodiceCultivar"></param>
    ''' <param name="NomeColonnaDescrizioneSpecie"></param>
    ''' <param name="NomeColonnaDescrizioneCultivar"></param>
    Public Sub ImpostaSpecieCultivarAgeaSuDatatableImpianti(
        piva As String,
        DtDatiConImpianti As DataTable,
        ByVal objParametri_Server As AgronicaCoreParametri,
        Optional ByVal NomeColonnaCodiceSpecie As String = "COD_PRODOTTO",
        Optional ByVal NomeColonnaCodiceCultivar As String = "COD_USO_VARIETA",
        Optional ByVal NomeColonnaDescrizioneSpecie As String = "DES_PRODOTTO",
        Optional ByVal NomeColonnaDescrizioneCultivar As String = "DES_USO_VARIETA"
    )

        Dim DT_AGEA_bycodiciGias, DT_AGEA_byQuintuplaAgea As DataTable

        Dim strFiltroImpianti As String =
            RicavaFiltroImpianti(piva, DtDatiConImpianti)



        '---------------------------------------------------------------------------
        AGEA_LeggiDT_DatiAgea(piva, strFiltroImpianti, objParametri_Server, DT_AGEA_byQuintuplaAgea, DT_AGEA_bycodiciGias)
        '---------------------------------------------------------------------------

        Dim HT_OccupazioneDes As New Hashtable

        For Each dRowImp As DataRow In DtDatiConImpianti.Rows



            Dim Dati As String = AGEA_Ricava_Dati(DT_AGEA_byQuintuplaAgea, DT_AGEA_bycodiciGias,
                                                             dRowImp("sa_cod"),
                                                             dRowImp("appezza"),
                                                             dRowImp("id_reg"),
                                                              HT_OccupazioneDes)

            If Dati <> "" Then

                Dim jO As JObject = JObject.Parse(Dati)

                'imposta specie, cultivar su tabella.
                dRowImp(NomeColonnaCodiceSpecie) = jO(NomeColonnaCodiceSpecie)
                dRowImp(NomeColonnaCodiceCultivar) = jO(NomeColonnaCodiceCultivar)
                dRowImp(NomeColonnaDescrizioneSpecie) = jO(NomeColonnaDescrizioneSpecie)
                dRowImp(NomeColonnaDescrizioneCultivar) = jO(NomeColonnaDescrizioneCultivar)

            End If

        Next

    End Sub

    Private Function RicavaFiltroImpianti(piva As String, dtImpianti As DataTable) As String


        Dim listaFiltro As New List(Of String)

        For Each dRowImp As DataRow In dtImpianti.Rows

            Dim s1 As String = " (Reg_Impianti.PIVA='" & piva & "' " &
                                                    " AND Reg_Impianti.SA_COD=" & dRowImp("sa_cod") &
                                                        " AND Reg_Impianti.APPEZZA=" & dRowImp("Appezza") &
                                                        " AND Reg_Impianti.ID_REG=" & dRowImp("ID_REG") &
                                                        " ) "

            If Not listaFiltro.Contains(s1) Then

                listaFiltro.Add(s1)

            End If

        Next

        Dim rval As String = " AND " &
            String.Join(" OR ", listaFiltro.ToArray)

        Return rval

    End Function



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strFiltroImpianti"></param>
    ''' <param name="DT_AGEA_byQuintuplaAgea"></param>
    ''' <param name="DT_AGEA_bycodiciGias"></param>
    Private Sub AGEA_LeggiDT_DatiAgea(
        ByVal piva As String,
        ByVal strFiltroImpianti As String,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByRef DT_AGEA_byQuintuplaAgea As DataTable, ByRef DT_AGEA_bycodiciGias As DataTable)

        Try

            Dim objRIP As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R

            DT_AGEA_byQuintuplaAgea = objRIP.LeggiOccupazioneDes_su_quintuplaAGEA(piva, 0, 0, 0, "", "", "", "", "",
                                                                   strFiltroImpianti,
                                                                   "", "", objParametri_Server)

            DT_AGEA_bycodiciGias = objRIP.LeggiOccupazioneDes_su_VegCodCulCodIdCod(piva, 0, 0, 0, 0, 0, 0,
                                                                   strFiltroImpianti,
                                                                   "", "", objParametri_Server)



        Catch ex As Exception

            Throw New Exception("Eccezione in: AGEA_LeggiDT_OccupazioneDes: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True), ex)

        End Try

    End Sub

    Private Function AGEA_Ricava_Dati_jSon(ByVal drow As DataRow) As String



        Dim stb As New StringBuilder
        stb.AppendLine("{")
        stb.Append(" ""COD_PRODOTTO"": """ & drow("veg_cod_agea").ToString & """")
        stb.Append(" ""DES_PRODOTTO"": """ & drow("veg_des_agea").ToString & """")
        stb.Append(" ""COD_SPECIE_VARIETA"": """ & drow("cul_cod_Agea").ToString & """")
        stb.Append(" ""DES_SPECIE_VARIETA"": """ & drow("Cul_Des_Agea").ToString & """")
        stb.AppendLine(",")
        stb.AppendLine("}")

        Return stb.ToString
    End Function

    '#########################################################################################
    Private Function AGEA_Ricava_Dati(ByVal DT_AGEA_byQuintuplaAgea As DataTable, ByVal DT_AGEA_bycodiciGias As DataTable,
                                           ByVal sa_cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer,
                                           ByRef HT_OccupazioneDes As Hashtable) As String

        Dim DatiJson As String = ""

        Try

            Dim flag_ricercaXcodiciGias As Boolean = False

            Dim chiave_Impianto As String = sa_cod & "|" & Appezza & "|" & Id_Reg

            If Not HT_OccupazioneDes.ContainsKey(chiave_Impianto) Then

                If Not IsNothing(DT_AGEA_byQuintuplaAgea) AndAlso DT_AGEA_byQuintuplaAgea.Rows.Count > 0 Then
                    'RICERCA SULLA QUINTUPLA AGEA

                    Dim dr() As DataRow

                    dr = DT_AGEA_byQuintuplaAgea.Select(" Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " AND appezza = " & Agro_SQL_SaveNum(Appezza) & " AND id_reg = " & Agro_SQL_SaveNum(Id_Reg))

                    If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                        DatiJson = AGEA_Ricava_Dati_jSon(dr(0))
                        HT_OccupazioneDes.Add(chiave_Impianto, DatiJson)
                    Else
                        flag_ricercaXcodiciGias = True
                    End If

                Else
                    flag_ricercaXcodiciGias = True
                End If

                If flag_ricercaXcodiciGias = True AndAlso Not IsNothing(DT_AGEA_bycodiciGias) AndAlso DT_AGEA_bycodiciGias.Rows.Count > 0 Then
                    'RICERCA SUI CODICI GIAS
                    Dim dr() As DataRow

                    dr = DT_AGEA_bycodiciGias.Select(" Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " AND appezza = " & Agro_SQL_SaveNum(Appezza) & " AND id_reg = " & Agro_SQL_SaveNum(Id_Reg))

                    If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                        DatiJson = AGEA_Ricava_Dati_jSon(dr(0))
                        HT_OccupazioneDes.Add(chiave_Impianto, DatiJson)
                    End If

                End If

            Else
                DatiJson = HT_OccupazioneDes(chiave_Impianto)
            End If 'ht impianti

        Catch ex As Exception


            Throw New Exception("Eccezione in: AGEA_Ricava_OccupazioneDes: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True), ex)

        End Try

        Return DatiJson

    End Function


End Class
