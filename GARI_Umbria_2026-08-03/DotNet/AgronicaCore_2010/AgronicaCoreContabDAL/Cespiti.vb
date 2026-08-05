

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Cespiti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                      ByVal PIVA_SuperUser As String, _
                      ByVal PIVA As String, _
                      Optional ByVal Sa_Cod As Long = 0, _
                      Optional ByVal IdCodCespite As Long = 0, _
                      Optional ByVal BeneTipo As Integer = 0, _
                      Optional ByVal xFiltroAggiuntivo As String = "", _
                      Optional ByVal xOrderBy As String = "", _
                      Optional ByVal ChiamataDaGiasLan As Boolean = False, _
                      Optional ByRef strSQLOutput As String = "") As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Cespiti_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT Cespiti.* " & vbCrLf)
            Stb.Append(" FROM  Cespiti  " & vbCrLf)

            Stb.Append(" WHERE 1=1 " & vbCrLf)

            If PIVA_SuperUser <> "" Then
                Stb.Append(" AND   Cespiti.Piva_SuperUser = '" & Agro_SQL_SaveText(PIVA_SuperUser) & "' " & vbCrLf)
            End If
            If PIVA <> "" Then
                Stb.Append(" AND   Cespiti.Piva = '" & Agro_SQL_SaveText(PIVA) & "' " & vbCrLf)
            End If
            If Sa_Cod <> 0 Then
                Stb.Append(" AND Cespiti_Anagrafiche_W.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            If IdCodCespite <> 0 Then
                Stb.Append(" AND Cespiti.id_cod_cespite = " & Agro_SQL_SaveNum(IdCodCespite) & "   " & vbCrLf)
            End If

            If BeneTipo <> 0 Then
                Stb.Append(" AND Cespiti.Bene_tipo = " & Agro_SQL_SaveNum(BeneTipo) & "   " & vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY Cespiti.Piva, Cespiti.id_cod_cespite ")
            End If

            If ChiamataDaGiasLan Then
                strSQLOutput = Stb.ToString
            Else
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function ListaCategorieBeniAmmortizzabili(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                  Optional ByVal BeneCod As Integer = 0, _
                  Optional ByVal BeneTipo As Integer = 0, _
                  Optional ByVal xFiltroAggiuntivo As String = "", _
                  Optional ByVal xOrderBy As String = "", _
                  Optional ByVal ChiamataDaGiasLan As Boolean = False, _
                  Optional ByRef strSQLOutput As String = "") As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.LeggiBeniAmmortizzabili()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append(" SELECT Elem_Cod as Bene_Cod,Tipo_Produzione as Bene_tipo,Tabella,NomeComune,Tabella_Cod,Tabella_Des ,ammortizzabile  " & vbCrLf)
            Stb.Append(" FROM CategorieMagazzino " & vbCrLf)
            Stb.Append(" WHERE CategorieMagazzino.ammortizzabile = 1 " & vbCrLf)

            Stb.Append(" UNION " & vbCrLf)

            Stb.Append(" SELECT Cod as Bene_Cod,Tipo as Bene_tipo ,Tabella,NomeComune,Tabella_Cod,Tabella_Des ,ammortizzabile " & vbCrLf)
            Stb.Append(" FROM CategorieAmmortizzabili " & vbCrLf)
            Stb.Append(" WHERE CategorieAmmortizzabili.ammortizzabile = 1 " & vbCrLf)



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------

            'If xOrderBy <> "" Then
            '    Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            '    Stb.Append(" ORDER BY Cespiti.Piva, Cespiti.id_cod_cespite ")
            'End If

            If ChiamataDaGiasLan Then
                strSQLOutput = Stb.ToString
            Else
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    Public Function LeggiAnagraficheCollegateACespiti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                      ByVal PIVA_SuperUser As String, _
                                                      ByVal PIVA As String, _
                                                      Optional ByVal Sa_Cod As Long = 0, _
                                                      Optional ByVal IdCodCespite As Long = 0, _
                                                      Optional ByVal BeneTipo As Integer = 0, _
                                                    Optional ByVal Elem_Cod As Long = 0, _
                                                    Optional ByVal NomeTabella As String = "", _
                                                    Optional ByVal Tabella_Cod As String = "", _
                                                    Optional ByVal Pro_Cod As Long = 0, _
                                                      Optional ByVal xFiltroAggiuntivo As String = "", _
                                                      Optional ByVal xOrderBy As String = "", _
                                                      Optional ByVal ChiamataDaGiasLan As Boolean = False, _
                                                      Optional ByRef strSQLOutput As String = ""
                                                      ) As DataTable



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Cespiti_R.LeggiAnagraficheCollegateACespiti()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable


        Try

            Stb.Length = 0

            Select Case NomeTabella
                Case "Parco_Macchine"

                    'Elenco macchine non ancora nell'archivio cespiti
                    Stb.Append(" SELECT Parco_Macchine.*  " & vbCrLf)
                    Stb.Append(" FROM Parco_Macchine " & vbCrLf)
                    Stb.Append(" LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva " & vbCrLf)
                    Stb.Append(" WHERE NOT EXISTS (SELECT id_cod_cespite FROM Cespiti WHERE Cespiti.Piva=Parco_Macchine.Piva " & vbCrLf)
                    Stb.Append("	AND Cespiti.Sa_Cod=Parco_Macchine.Sa_Cod " & vbCrLf)
                    Stb.Append("	AND Parco_Macchine.Mac_Cod=Cespiti.cesp_cod ) " & vbCrLf)


                Case "Zoo_Animali"

                    Stb.Append(" SELECT DISTINCT Zoo_Animali.Matricola,Lista_Categorie_Animali.CAT_DES, " & vbCrLf)
                    Stb.Append("  --Lista_Razze_Animali.RAZ_DES, " & vbCrLf)
                    Stb.Append("  Lista_IndirizziProd_Animali.IPRO_DES , " & vbCrLf)
                    Stb.Append("  Zoo_Animali.Sesso, " & vbCrLf)
                    Stb.Append("  Centri_Aziendali.SA_cod,Centri_Aziendali.SA_NOME ,Fabbricati.Fabbricato_DES, " & vbCrLf)
                    Stb.Append("  Zoo_Animali.Cod_Progetto,  " & vbCrLf)
                    Stb.Append("  Mov_Destinazioni.Id_Destinazione, " & vbCrLf)
                    Stb.Append("  Fabbricati.fabbricato_cod,Fabbricati.tipo_fabbricato_cod " & vbCrLf)

                    Stb.Append(" FROM Zoo_Animali " & vbCrLf)
                    Stb.Append("  INNER JOIN Lista_Categorie_Animali ON (Zoo_Animali.GEN_COD = Lista_Categorie_Animali.GEN_COD AND Zoo_Animali.SPE_COD = Lista_Categorie_Animali.SPE_COD AND Zoo_Animali.IPRO_COD = Lista_Categorie_Animali.IPRO_COD AND Zoo_Animali.CAT_COD = Lista_Categorie_Animali.CAT_COD )          " & vbCrLf)
                    Stb.Append("  --INNER JOIN Lista_Razze_Animali   ON (Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD AND Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD AND Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD )                                  " & vbCrLf)
                    Stb.Append("  INNER JOIN Lista_IndirizziProd_Animali ON (Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD AND Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD AND Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD )  " & vbCrLf)
                    Stb.Append("  INNER JOIN Movimenti_dettagli ON (Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto) " & vbCrLf)
                    Stb.Append("  INNER JOIN Mov_Destinazioni ON (Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND  Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  Movimenti_dettagli.Id_Mov=Mov_Destinazioni.Id_Mov and Movimenti_dettagli.Id_Mov_Det=Mov_Destinazioni.Id_Mov_Det)  " & vbCrLf)
                    Stb.Append("  LEFT OUTER JOIN Centri_Aziendali ON (Mov_Destinazioni.Piva=Centri_Aziendali.Piva and Mov_Destinazioni.SA_COD=Centri_Aziendali.sa_cod) " & vbCrLf)
                    Stb.Append("  INNER JOIN Agenda on (Movimenti_dettagli.Id_Agenda =Agenda.Id_Agenda) " & vbCrLf)
                    Stb.Append("  INNER JOIN Fabbricati on (Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod) " & vbCrLf)
                    Stb.Append("  WHERE Lav_Cod=3001 " & vbCrLf)
                    Stb.Append("       AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti WHERE Cespiti.Piva=Zoo_Animali.Piva  " & vbCrLf)
                    Stb.Append("       AND Cespiti.Sa_Cod=Mov_Destinazioni.Sa_Cod  " & vbCrLf)
                    Stb.Append("       AND Zoo_Animali.Cod_Progetto=Cespiti.cesp_cod )")


                Case "Materie_Prime", "TipologieSementi"

                    'Elem_cod=500 --> Altri beni ammortizzabili
                    If Elem_Cod = 500 Then

                        'Beni caricati da Doc Fattura : hanno la corrispondenza in Movimenti_Imputazioni  
                        Stb.Append(" SELECT  Movimenti_dettagli.Sa_Cod,Centri_Aziendali.SA_NOME,'' as Fabbricato_DES, " & vbCrLf)
                        Stb.Append("         Materie_Prime.Elem_Cod,CategorieMagazzino.NomeComune, " & vbCrLf)
                        Stb.Append("         Materie_Prime.Mat_Cod, Materie_Prime.Mat_Des, " & vbCrLf)
                        Stb.Append("         Movimenti_dettagli.Qta,Movimenti_dettagli.Udm_cod, UnitaMisura.UDM_SIM,  " & vbCrLf)
                        Stb.Append("         Agenda.des_lib,  " & vbCrLf)
                        Stb.Append("         Mov_Destinazioni.Id_Destinazione, Fabbricati.fabbricato_cod,Fabbricati.tipo_fabbricato_cod  " & vbCrLf)


                        Stb.Append("  FROM Materie_Prime " & vbCrLf)
                        Stb.Append("    INNER JOIN Movimenti_Imputazioni on (Movimenti_Imputazioni.Imputazione_Cod=Materie_Prime.Mat_Cod) " & vbCrLf)
                        Stb.Append("    INNER JOIN Movimenti_dettagli on (Movimenti_dettagli.piva=movimenti_imputazioni.Piva and movimenti_imputazioni.Id_Agenda=Movimenti_dettagli.Id_Agenda and movimenti_imputazioni.Id_Mov=Movimenti_dettagli.Id_Mov and movimenti_imputazioni.Id_Mov_det=Movimenti_dettagli.Id_Mov_det)    " & vbCrLf)
                        Stb.Append("    LEFT OUTER JOIN Mov_Destinazioni ON (Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND  Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  Movimenti_dettagli.Id_Mov=Mov_Destinazioni.Id_Mov and Movimenti_dettagli.Id_Mov_Det=Mov_Destinazioni.Id_Mov_Det)    " & vbCrLf)
                        Stb.Append("    LEFT OUTER JOIN Centri_Aziendali ON (Movimenti_dettagli.Piva=Centri_Aziendali.Piva and Movimenti_dettagli.SA_COD=Centri_Aziendali.sa_cod)   " & vbCrLf)
                        Stb.Append("    INNER JOIN Agenda ON (Movimenti_dettagli.Id_Agenda =Agenda.Id_Agenda)   " & vbCrLf)
                        Stb.Append("    LEFT OUTER JOIN Fabbricati ON (Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod)   " & vbCrLf)
                        Stb.Append("    INNER JOIN UnitaMisura ON (UnitaMisura.Udm_cod=Movimenti_dettagli.Udm_cod)  " & vbCrLf)
                        Stb.Append("    INNER JOIN CategorieMagazzino ON (CategorieMagazzino.Elem_Cod = Materie_Prime.Elem_Cod)  " & vbCrLf)
                        Stb.Append("    WHERE Materie_Prime.Elem_cod=500  --and Lav_Cod = 1000   --(1000 ACQ , 1001 VEN , 1022)  " & vbCrLf)
                        Stb.Append("         AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti WHERE Cespiti.Piva=Materie_Prime.Piva    " & vbCrLf)
                        Stb.Append("         AND Cespiti.Sa_Cod=Movimenti_dettagli.Sa_Cod    " & vbCrLf)
                        Stb.Append("         AND Materie_Prime.Elem_Cod=Cespiti.Bene_Cod   " & vbCrLf)
                        Stb.Append("         AND Materie_Prime.Mat_Cod=Cespiti.cesp_cod    " & vbCrLf)
                        Stb.Append("        AND Materie_Prime.Elem_cod =500 )    " & vbCrLf)


                        'Beni caricati da Doc Risorse : NON hanno la corrispondenza in Movimenti_Imputazioni
                        Stb.Append(" UNION ALL " & vbCrLf)

                        Stb.Append(" SELECT 0 as Sa_Cod,'' as SA_NOME,'' as Fabbricato_DES, " & vbCrLf)
                        Stb.Append(" Materie_Prime.Elem_Cod,CategorieMagazzino.NomeComune, " & vbCrLf)
                        Stb.Append(" Materie_Prime.Mat_Cod, Materie_Prime.Mat_Des, " & vbCrLf)
                        Stb.Append(" 0 as Qta,0 as Udm_cod, '' as UDM_SIM, " & vbCrLf)
                        Stb.Append("                                 '' as des_lib , " & vbCrLf)
                        Stb.Append(" 0 as Id_Destinazione, 0 as fabbricato_cod,0 as tipo_fabbricato_cod  " & vbCrLf)
                        Stb.Append(" FROM Materie_Prime " & vbCrLf)
                        Stb.Append(" LEFT OUTER JOIN Movimenti_Imputazioni on (Movimenti_Imputazioni.Imputazione_Cod=Materie_Prime.Mat_Cod) " & vbCrLf)
                        Stb.Append(" INNER JOIN CategorieMagazzino ON (CategorieMagazzino.Elem_Cod = Materie_Prime.Elem_Cod)  " & vbCrLf)
                        Stb.Append(" WHERE Materie_Prime.Elem_cod=500  --and Lav_Cod = 1000   --(1000 ACQ , 1001 VEN , 1022)  " & vbCrLf)
                        Stb.Append("         AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti WHERE Cespiti.Piva=Materie_Prime.Piva    " & vbCrLf)
                        Stb.Append("         --AND Cespiti.Sa_Cod=Movimenti_dettagli.Sa_Cod    " & vbCrLf)
                        Stb.Append("         AND Materie_Prime.Elem_Cod=Cespiti.Bene_Cod   " & vbCrLf)
                        Stb.Append("         AND Materie_Prime.Mat_Cod=Cespiti.cesp_cod    " & vbCrLf)
                        Stb.Append("        AND Materie_Prime.Elem_cod =500 )   " & vbCrLf)
                        Stb.Append(" AND Movimenti_Imputazioni.Imputazione_Cod is null  " & vbCrLf)

                        xOrderBy = "Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod ASC "


                    Else

                        Stb.Append(" SELECT  " & vbCrLf)
                        Stb.Append("   Centri_Aziendali.SA_cod,Centri_Aziendali.SA_NOME ,Fabbricati.Fabbricato_DES,  " & vbCrLf)
                        Stb.Append("   Materie_Prime.Elem_Cod,CategorieMagazzino.NomeComune, Materie_Prime.Mat_Cod, Materie_Prime.Mat_Des,Movimenti_dettagli.Qta,Movimenti_dettagli.Udm_cod, UnitaMisura.UDM_SIM, " & vbCrLf)
                        Stb.Append("   Agenda.des_lib, " & vbCrLf)
                        Stb.Append("   Mov_Destinazioni.Id_Destinazione,  " & vbCrLf)
                        Stb.Append("   Fabbricati.fabbricato_cod,Fabbricati.tipo_fabbricato_cod , " & vbCrLf)
                        Stb.Append("   * " & vbCrLf)
                        Stb.Append(" FROM Materie_Prime   " & vbCrLf)
                        Stb.Append("   INNER JOIN Movimenti_dettagli ON (Movimenti_dettagli.Elem_Cod = " & Elem_Cod & " AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod)  " & vbCrLf)
                        Stb.Append("   INNER JOIN Mov_Destinazioni ON (Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND  Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  Movimenti_dettagli.Id_Mov=Mov_Destinazioni.Id_Mov and Movimenti_dettagli.Id_Mov_Det=Mov_Destinazioni.Id_Mov_Det)   " & vbCrLf)
                        Stb.Append("   LEFT OUTER JOIN Centri_Aziendali ON (Mov_Destinazioni.Piva=Centri_Aziendali.Piva and Mov_Destinazioni.SA_COD=Centri_Aziendali.sa_cod)  " & vbCrLf)
                        Stb.Append("   INNER JOIN Agenda ON (Movimenti_dettagli.Id_Agenda =Agenda.Id_Agenda)  " & vbCrLf)
                        Stb.Append("   INNER JOIN Fabbricati ON (Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod)  " & vbCrLf)
                        Stb.Append("   INNER JOIN UnitaMisura ON (UnitaMisura.Udm_cod=Movimenti_dettagli.Udm_cod) " & vbCrLf)
                        Stb.Append("   INNER JOIN CategorieMagazzino ON (CategorieMagazzino.Elem_Cod = Materie_Prime.Elem_Cod) " & vbCrLf)
                        Stb.Append("   WHERE (Lav_Cod = 1022 )  --(1000 ACQ , 1001 VEN , 1022) " & vbCrLf)
                        Stb.Append("        AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti WHERE Cespiti.Piva=Materie_Prime.Piva   " & vbCrLf)
                        Stb.Append("        AND Cespiti.Sa_Cod=Mov_Destinazioni.Sa_Cod   " & vbCrLf)
                        Stb.Append("        AND Materie_Prime.Elem_Cod=Cespiti.Bene_Cod  " & vbCrLf)
                        Stb.Append("        AND Materie_Prime.Mat_Cod=Cespiti.cesp_cod   " & vbCrLf)


                        If Elem_Cod <> 0 Then
                            Stb.Append("        AND Materie_Prime.Elem_cod =" & Elem_Cod & " )   " & vbCrLf)
                        End If

                        xOrderBy = " Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Agenda.Lav_Cod ASC "

                    End If

                Case "Formulati"
                    Stb.Append("   select * from Formulati " & vbCrLf)
                    Stb.Append("    INNER JOIN Movimenti_dettagli ON (Movimenti_dettagli.Elem_Cod = 191 AND Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Mov_Destinazioni ON (Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND  Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  Movimenti_dettagli.Id_Mov=Mov_Destinazioni.Id_Mov and Movimenti_dettagli.Id_Mov_Det=Mov_Destinazioni.Id_Mov_Det)    " & vbCrLf)
                    Stb.Append("    LEFT OUTER JOIN Centri_Aziendali ON (Mov_Destinazioni.Piva=Centri_Aziendali.Piva and Mov_Destinazioni.SA_COD=Centri_Aziendali.sa_cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Agenda ON (Movimenti_dettagli.Id_Agenda =Agenda.Id_Agenda)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Fabbricati ON (Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN UnitaMisura ON (UnitaMisura.Udm_cod=Movimenti_dettagli.Udm_cod)  " & vbCrLf)
                    Stb.Append("    --INNER JOIN CategorieMagazzino ON (CategorieMagazzino.Elem_Cod = Materie_Prime.Elem_Cod)  " & vbCrLf)
                    Stb.Append("    WHERE (Lav_Cod = 1022)  --(1000 ACQ , 1001 VEN , 1022)  " & vbCrLf)
                    Stb.Append("         AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti   " & vbCrLf)
                    Stb.Append("         WHERE Cespiti.Sa_Cod=Mov_Destinazioni.Sa_Cod    " & vbCrLf)
                    Stb.Append("         AND Cespiti.Bene_Cod =191  " & vbCrLf)
                    Stb.Append("         AND Formulati.Fr_Cod=Cespiti.cesp_cod)   ")



                Case "Fertilizzanti"

                    Stb.Append("   select * from Fertilizzanti " & vbCrLf)
                    Stb.Append("    INNER JOIN Movimenti_dettagli ON (Movimenti_dettagli.Elem_Cod = 3 AND Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Mov_Destinazioni ON (Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND  Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  Movimenti_dettagli.Id_Mov=Mov_Destinazioni.Id_Mov and Movimenti_dettagli.Id_Mov_Det=Mov_Destinazioni.Id_Mov_Det)    " & vbCrLf)
                    Stb.Append("    LEFT OUTER JOIN Centri_Aziendali ON (Mov_Destinazioni.Piva=Centri_Aziendali.Piva and Mov_Destinazioni.SA_COD=Centri_Aziendali.sa_cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Agenda ON (Movimenti_dettagli.Id_Agenda =Agenda.Id_Agenda)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Fabbricati ON (Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN UnitaMisura ON (UnitaMisura.Udm_cod=Movimenti_dettagli.Udm_cod)  " & vbCrLf)
                    Stb.Append("    --INNER JOIN CategorieMagazzino ON (CategorieMagazzino.Elem_Cod = Materie_Prime.Elem_Cod)  " & vbCrLf)
                    Stb.Append("    WHERE (Lav_Cod = 1022 )  --(1000 ACQ , 1001 VEN , 1022)  " & vbCrLf)
                    Stb.Append("        AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti   " & vbCrLf)
                    Stb.Append("        WHERE Cespiti.Sa_Cod=Mov_Destinazioni.Sa_Cod    " & vbCrLf)
                    Stb.Append("        AND Cespiti.Bene_Cod =3  " & vbCrLf)
                    Stb.Append("        AND Fertilizzanti.Fer_Cod=Cespiti.cesp_cod)    " & vbCrLf)

                Case "Formulati"

                    Stb.Append("                select * from Formulati " & vbCrLf)
                    Stb.Append("                     INNER JOIN Movimenti_dettagli ON (Movimenti_dettagli.Elem_Cod = 191 AND Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod)   " & vbCrLf)
                    Stb.Append("                        INNER JOIN Mov_Destinazioni ON (Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND  Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  Movimenti_dettagli.Id_Mov=Mov_Destinazioni.Id_Mov and Movimenti_dettagli.Id_Mov_Det=Mov_Destinazioni.Id_Mov_Det)    " & vbCrLf)
                    Stb.Append("                        LEFT OUTER JOIN Centri_Aziendali ON (Mov_Destinazioni.Piva=Centri_Aziendali.Piva and Mov_Destinazioni.SA_COD=Centri_Aziendali.sa_cod)   " & vbCrLf)
                    Stb.Append("                        INNER JOIN Agenda ON (Movimenti_dettagli.Id_Agenda =Agenda.Id_Agenda)   " & vbCrLf)
                    Stb.Append("                        INNER JOIN Fabbricati ON (Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod)   " & vbCrLf)
                    Stb.Append("                        INNER JOIN UnitaMisura ON (UnitaMisura.Udm_cod=Movimenti_dettagli.Udm_cod)  " & vbCrLf)
                    Stb.Append("                        --INNER JOIN CategorieMagazzino ON (CategorieMagazzino.Elem_Cod = Materie_Prime.Elem_Cod)  " & vbCrLf)
                    Stb.Append("                        WHERE (Lav_Cod = 1022 )  --(1000 ACQ , 1001 VEN , 1022)  " & vbCrLf)
                    Stb.Append("                             AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti   " & vbCrLf)
                    Stb.Append("                             WHERE Cespiti.Sa_Cod=Mov_Destinazioni.Sa_Cod    " & vbCrLf)
                    Stb.Append("                             AND Cespiti.Bene_Cod =191  " & vbCrLf)
                    Stb.Append("                             AND Formulati.Fr_Cod=Cespiti.cesp_cod) ")


                Case "Coadiuvante"

                    Stb.Append("    select * from Coadiuvante " & vbCrLf)
                    Stb.Append("    INNER JOIN Movimenti_dettagli ON (Movimenti_dettagli.Elem_Cod = 195 AND Movimenti_dettagli.Pro_Cod = Coadiuvante.Coad_Cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Mov_Destinazioni ON (Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND  Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  Movimenti_dettagli.Id_Mov=Mov_Destinazioni.Id_Mov and Movimenti_dettagli.Id_Mov_Det=Mov_Destinazioni.Id_Mov_Det)    " & vbCrLf)
                    Stb.Append("    LEFT OUTER JOIN Centri_Aziendali ON (Mov_Destinazioni.Piva=Centri_Aziendali.Piva and Mov_Destinazioni.SA_COD=Centri_Aziendali.sa_cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Agenda ON (Movimenti_dettagli.Id_Agenda =Agenda.Id_Agenda)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Fabbricati ON (Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN UnitaMisura ON (UnitaMisura.Udm_cod=Movimenti_dettagli.Udm_cod)  " & vbCrLf)
                    Stb.Append("    --INNER JOIN CategorieMagazzino ON (CategorieMagazzino.Elem_Cod = Materie_Prime.Elem_Cod)  " & vbCrLf)
                    Stb.Append("    WHERE (Lav_Cod = 1022 )  --(1000 ACQ , 1001 VEN , 1022)  " & vbCrLf)
                    Stb.Append("    AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti   " & vbCrLf)
                    Stb.Append("        WHERE Cespiti.Sa_Cod=Mov_Destinazioni.Sa_Cod    " & vbCrLf)
                    Stb.Append("        AND Cespiti.Bene_Cod =195  " & vbCrLf)
                    Stb.Append("        AND Coadiuvante.Coad_Cod=Cespiti.cesp_cod)   ")

                Case "Trappole"
                    Stb.Append("    select * from Trappole " & vbCrLf)
                    Stb.Append("    INNER JOIN Movimenti_dettagli ON (Movimenti_dettagli.Elem_Cod = 197 AND Movimenti_dettagli.Pro_Cod = Trappole.Trap_Cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Mov_Destinazioni ON (Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND  Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  Movimenti_dettagli.Id_Mov=Mov_Destinazioni.Id_Mov and Movimenti_dettagli.Id_Mov_Det=Mov_Destinazioni.Id_Mov_Det)    " & vbCrLf)
                    Stb.Append("    LEFT OUTER JOIN Centri_Aziendali ON (Mov_Destinazioni.Piva=Centri_Aziendali.Piva and Mov_Destinazioni.SA_COD=Centri_Aziendali.sa_cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Agenda ON (Movimenti_dettagli.Id_Agenda =Agenda.Id_Agenda)   " & vbCrLf)
                    Stb.Append("    INNER JOIN Fabbricati ON (Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod)   " & vbCrLf)
                    Stb.Append("    INNER JOIN UnitaMisura ON (UnitaMisura.Udm_cod=Movimenti_dettagli.Udm_cod)  " & vbCrLf)
                    Stb.Append("    --INNER JOIN CategorieMagazzino ON (CategorieMagazzino.Elem_Cod = Materie_Prime.Elem_Cod)  " & vbCrLf)
                    Stb.Append("    WHERE (Lav_Cod = 1022 )  --(1000 ACQ , 1001 VEN , 1022)  " & vbCrLf)
                    Stb.Append("    AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti   " & vbCrLf)
                    Stb.Append("            WHERE Cespiti.Sa_Cod=Mov_Destinazioni.Sa_Cod    " & vbCrLf)
                    Stb.Append("            AND Cespiti.Bene_Cod =197  " & vbCrLf)
                    Stb.Append("            AND Trappole.Trap_Cod=Cespiti.cesp_cod)  ")


                Case "Fabbricati"

                    'Elenco fabbicati non ancora nell'archivio cespiti
                    Stb.Append(" SELECT Fabbricati.* " & vbCrLf)
                    Stb.Append(" FROM Fabbricati " & vbCrLf)
                    Stb.Append(" WHERE NOT EXISTS (SELECT id_cod_cespite FROM Cespiti WHERE Cespiti.Piva=Fabbricati.Piva  " & vbCrLf)
                    Stb.Append("    AND Cespiti.Sa_Cod=Fabbricati.Sa_Cod " & vbCrLf)
                    Stb.Append("    AND Fabbricati.Fabbricato_Cod=Cespiti.cesp_cod ) " & vbCrLf)

                Case "Reg_Impianti"

                    'Elenco impianti colturali non ancora nell'archivio cespiti
                    Stb.Append("                    select Reg_Impianti.PIVA,  " & vbCrLf)
                    Stb.Append("                         Reg_Impianti.SA_COD, " & vbCrLf)
                    Stb.Append("                         Centri_Aziendali.sa_nome , " & vbCrLf)
                    Stb.Append("                         Reg_Impianti.APPEZZA,Campi.Campo_Des, " & vbCrLf)
                    Stb.Append("                         Appezzamento.APP_NOME,  " & vbCrLf)
                    Stb.Append("                         Reg_Impianti.ID_REG,Imprese_Progetti.Progetto_Cod, " & vbCrLf)
                    Stb.Append("                         (Imprese_Progetti.P_HA * Reg_Impianti.Sup_Imp ) as NumPiante, " & vbCrLf)
                    Stb.Append("                         Cultivar.Cul_Des,SpecieVegetali.Veg_des,SpecieVegetali.Gru_Cod,  " & vbCrLf)
                    Stb.Append("                         Imprese_Progetti.Data_Inizio_Prevista,Imprese_Progetti.Data_Fioritura_Prevista,Imprese_Progetti.Validita_Inizio")

                    Stb.Append("                     from Reg_Impianti " & vbCrLf)
                    Stb.Append("                     inner join Appezzamento on (Reg_Impianti.Piva = Appezzamento.Piva AND Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod and Reg_Impianti.APPEZZA = Appezzamento.APPEZZA) " & vbCrLf)
                    Stb.Append("                     inner join Imprese_Progetti on (Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
                    Stb.Append("                     inner join Cultivar on (Reg_Impianti.CUL_COD=Cultivar.CUL_COD) " & vbCrLf)
                    Stb.Append("                     inner join SpecieVegetali on (Cultivar.Veg_Cod= SpecieVegetali.Veg_Cod) " & vbCrLf)
                    Stb.Append("                     inner join Centri_Aziendali ON (Reg_Impianti.Piva=Centri_Aziendali.Piva and Reg_Impianti.SA_COD=Centri_Aziendali.sa_cod) " & vbCrLf)
                    Stb.Append("                     inner join Campi on (Campi.Piva = Appezzamento.Piva AND Campi.Sa_Cod = Appezzamento.Sa_Cod AND Campi.Campo_Cod = Appezzamento.Campo_Cod ) " & vbCrLf)
                    Stb.Append("  " & vbCrLf)
                    Stb.Append("                     where SpecieVegetali.Gru_Cod =1 --(1-erboree, 2-orticole, 3-erbacee) " & vbCrLf)
                    Stb.Append("                     and Reg_Impianti.CUL_COD > 0 -- 0 = terreno nudo  " & vbCrLf)

                    Stb.Append("     AND NOT EXISTS (SELECT id_cod_cespite FROM Cespiti WHERE Cespiti.Piva=Reg_Impianti.Piva  " & vbCrLf)
                    Stb.Append("                        AND Cespiti.Sa_Cod=Reg_Impianti.Sa_Cod  " & vbCrLf)
                    Stb.Append("                        AND Imprese_Progetti.Progetto_Cod=Cespiti.cesp_cod )")


                    xOrderBy = " Centri_Aziendali.sa_nome"


                Case "Cantina_Insiemi"

                    '-- celle frigorifere (solo F&F dove sono contenute le stive/bancali)
                    Stb.Append("SELECT Piva, Sa_cod,Insieme_Cod,Insieme_Des " & vbCrLf)
                    Stb.Append(" FROM Cantina_Insiemi " & vbCrLf)
                    Stb.Append(" WHERE NOT EXISTS (SELECT id_cod_cespite FROM Cespiti WHERE Cespiti.Piva=Cantina_Insiemi.Piva    " & vbCrLf)
                    Stb.Append("                     AND Cespiti.Sa_Cod=Cantina_Insiemi.Sa_Cod    " & vbCrLf)
                    Stb.Append("                     AND Cantina_Insiemi.Insieme_Cod=Cespiti.cesp_cod) ")


                Case "Cantina_Vasche"

                    '-- vasche (cantine) stive (F&F)
                    Stb.Append("SELECT Piva, Sa_cod,Vas_Cod ,Identificativo  " & vbCrLf)
                    Stb.Append(" FROM Cantina_Vasche " & vbCrLf)
                    Stb.Append(" WHERE NOT EXISTS (SELECT id_cod_cespite FROM Cespiti WHERE Cespiti.Piva=Cantina_Vasche.Piva    " & vbCrLf)
                    Stb.Append("                     AND Cespiti.Sa_Cod=Cantina_Vasche.Sa_Cod    " & vbCrLf)
                    Stb.Append("                     AND Cantina_Vasche.Vas_Cod=Cespiti.cesp_cod)  ")




                Case Else
            End Select



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   " & NomeTabella & ".Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   " & NomeTabella & ".Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------



            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

            End If


            If ChiamataDaGiasLan Then
                strSQLOutput = Stb.ToString
            Else
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Cespiti_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                       ByVal Piva_SuperUser As String, _
                       ByVal Piva As String, _
                       ByVal Sa_Cod As Long, _
                       ByVal IdCodCespite As Long, _
                       ByVal DesCespite As String, _
                       Optional ByVal Bene_cod As Long = 0, _
                       Optional ByVal Bene_tipo As Integer = 0, _
                       Optional ByVal Cesp_Cod As Long = 0, _
                       Optional ByVal IdCodCategoria As Long = 0, _
                       Optional ByVal PrcAmmort As Double = 0, _
                       Optional ByVal BeneTipo As Integer = 0, _
                       Optional ByVal NaturaBene As Integer = 0, _
                       Optional ByVal DimezzaPrimoAnno As Integer = 0, _
                       Optional ByVal DatIniUtilizzo As Date = AGRODATAINIZIO, _
                       Optional ByVal DatChiusura As Date = AGRODATAFINE, _
                       Optional ByVal NumAnniDurata As Integer = 0, _
                       Optional ByVal TipoUbicazione As Integer = 0, _
                       Optional ByVal CodUbicazione As Long = 0, _
                       Optional ByVal IdCodCespitePadre As Long = 0, _
                       Optional ByVal Fis_PrcAmmort As Double = 0, _
                       Optional ByVal Fis_DimezzaPrimoAnno As Integer = 0, _
                       Optional ByVal Fis_LimMaxDed As Double = 0,
                       Optional ByVal Fis_PrcDed As Double = 0,
                             Optional ByVal Validita_Inizio As Date = #2/1/1900#, _
                             Optional ByVal Validita_Fine As Date = #2/1/1900#, _
                             Optional ByVal Data_creazione As Date = #2/1/1900#, _
                             Optional ByVal Data_modifica As Date = #2/1/1900#, _
                             Optional ByVal username_creazione As String = "", _
                             Optional ByVal username_modifica As String = "") As Boolean



        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If




            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" INSERT INTO Cespiti                ( " & vbCrLf)
            Stb.Append("        Piva_SuperUser,               " & vbCrLf)
            Stb.Append("        Piva,                         " & vbCrLf)
            Stb.Append("        Sa_Cod,                       " & vbCrLf)
            Stb.Append("        id_cod_cespite ,              " & vbCrLf)
            Stb.Append("        des_cespite ,                 " & vbCrLf)
            Stb.Append("        Bene_cod ,                    " & vbCrLf)
            Stb.Append("        Bene_tipo ,                    " & vbCrLf)
            Stb.Append("        Cesp_Cod ,                    " & vbCrLf)
            Stb.Append("        id_cod_categoria,             " & vbCrLf)
            Stb.Append("        prc_ammor,                    " & vbCrLf)
            Stb.Append("        ind_tipo_bene,                " & vbCrLf)
            Stb.Append("        ind_natura_bene,                " & vbCrLf)
            Stb.Append("        flg_dimezza_primo_anno,         " & vbCrLf)
            Stb.Append("        dat_ini_utilizzo,             " & vbCrLf)
            Stb.Append("        dat_chiusura,                 " & vbCrLf)
            Stb.Append("        num_anni_durata,              " & vbCrLf)
            Stb.Append("        ind_tipo_ubicazione,          " & vbCrLf)
            Stb.Append("        cod_ubicazione,               " & vbCrLf)
            Stb.Append("        id_cod_cespite_padre,         " & vbCrLf)
            Stb.Append("        fis_prc_ammor ,				  " & vbCrLf)
            Stb.Append("        fis_flg_dimezza_primo_anno 	, " & vbCrLf)
            Stb.Append("        fis_lim_max_ded ,			  " & vbCrLf)
            Stb.Append("        fis_perc_ded ,                " & vbCrLf)

            Stb.Append(" Inviato,            DataInvio, " & vbCrLf)
            Stb.Append(" Data_Creazione,     Data_Modifica, " & vbCrLf)
            Stb.Append(" UserName_Creazione, UserName_Modifica, " & vbCrLf)
            Stb.Append(" Validita_Inizio,    Validita_Fine " & vbCrLf)

            Stb.Append(" )" & vbCrLf)


            Stb.Append("  VALUES ( " & vbCrLf)
            Stb.Append("         '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  " & vbCrLf)
            Stb.Append("         , '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(IdCodCespite) & " " & vbCrLf)
            Stb.Append("         ,'" & Agro_SQL_SaveText(DesCespite) & "' " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(Bene_cod) & " " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(Bene_tipo) & " " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(Cesp_Cod) & " " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(IdCodCategoria) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(PrcAmmort) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(BeneTipo) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(NaturaBene) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(DimezzaPrimoAnno) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveDate(DatIniUtilizzo) & " " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveDate(DatChiusura) & "" & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(NumAnniDurata) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(TipoUbicazione) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(CodUbicazione) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(IdCodCespitePadre) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(Fis_PrcAmmort) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(Fis_DimezzaPrimoAnno) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(Fis_LimMaxDed) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(Fis_PrcDed) & "  " & vbCrLf)

            Stb.Append("         , 0  ")
            Stb.Append("         , Null  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            Stb.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            Stb.Append(" ) " & vbCrLf)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    'Ini - Modifica
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                           ByVal Sa_Cod As Long, _
                           ByVal IdCodCespite As Long, _
                           ByVal DesCespite As String, _
                           ByVal BeneCod As Integer, _
                           ByVal IdCodCategoria As Long, _
                           ByVal PrcAmmort As Double, _
                           ByVal TipoBene As Integer, _
                           ByVal NaturaBene As Integer, _
                           ByVal DimezzaPrimoAnno As Integer, _
                           ByVal DatIniUtilizzo As Date, _
                           ByVal DatChiusura As Date, _
                           ByVal NumAnniDurata As Integer, _
                           ByVal TipoUbicazione As Integer, _
                           ByVal CodUbicazione As Long, _
                           ByVal IdCodCespitePadre As Long, _
                        ByVal Fis_PrcAmmort As Double, _
                        ByVal Fis_DimezzaPrimoAnno As Integer, _
                        ByVal Fis_LimMaxDed As Double,
                        ByVal Fis_PrcDed As Double,
                         Optional ByVal Validita_Inizio As Date = #2/1/1900#, _
                         Optional ByVal Validita_Fine As Date = #2/1/1900#, _
                         Optional ByVal Data_creazione As Date = #2/1/1900#, _
                         Optional ByVal Data_modifica As Date = #2/1/1900#, _
                         Optional ByVal username_creazione As String = "", _
                         Optional ByVal username_modifica As String = "") As Boolean



        Dim NomeRoutine As String = "Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0

            Stb.Append(" UPDATE Cespiti SET " & vbCrLf)
            Stb.Append(" Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ",  " & vbCrLf)
            Stb.Append(" des_cespite = '" & Agro_SQL_SaveText(DesCespite) & "',  " & vbCrLf)
            'Il bene_tipo e il bene_cod non devono essere modificati   
            'Stb.Append(" bene_cod = " & Agro_SQL_SaveNum(BeneCod) & ",  " & vbCrLf)
            Stb.Append(" id_cod_categoria = " & Agro_SQL_SaveNum(IdCodCategoria) & ",  " & vbCrLf)
            Stb.Append(" prc_ammor = " & Agro_SQL_SaveNum(PrcAmmort) & ",  " & vbCrLf)
            Stb.Append(" ind_tipo_bene = " & Agro_SQL_SaveNum(TipoBene) & ",  " & vbCrLf)
            Stb.Append(" ind_natura_bene = " & Agro_SQL_SaveNum(NaturaBene) & ",  " & vbCrLf)
            Stb.Append(" flg_dimezza_primo_anno = " & Agro_SQL_SaveNum(DimezzaPrimoAnno) & ",  " & vbCrLf)
            Stb.Append(" dat_ini_utilizzo = " & Agro_SQL_SaveDate(DatIniUtilizzo) & " ,  " & vbCrLf)
            Stb.Append(" dat_chiusura = " & Agro_SQL_SaveDate(DatChiusura) & " ,   " & vbCrLf)
            Stb.Append(" num_anni_durata = " & Agro_SQL_SaveNum(NumAnniDurata) & ",  " & vbCrLf)
            Stb.Append(" ind_tipo_ubicazione = " & Agro_SQL_SaveNum(TipoUbicazione) & ",  " & vbCrLf)
            Stb.Append(" cod_ubicazione = " & Agro_SQL_SaveNum(CodUbicazione) & ",  " & vbCrLf)
            Stb.Append(" id_cod_cespite_padre = " & Agro_SQL_SaveNum(IdCodCespitePadre) & ",  " & vbCrLf)

            Stb.Append(" fis_prc_ammor = " & Agro_SQL_SaveNum(Fis_PrcAmmort) & ",  " & vbCrLf)
            Stb.Append(" fis_flg_dimezza_primo_anno = " & Agro_SQL_SaveNum(Fis_DimezzaPrimoAnno) & ",  " & vbCrLf)
            Stb.Append(" fis_lim_max_ded = " & Agro_SQL_SaveNum(Fis_LimMaxDed) & ",  " & vbCrLf)
            Stb.Append(" fis_perc_ded = " & Agro_SQL_SaveNum(Fis_PrcDed) & "  " & vbCrLf)

            Stb.Append("   ,Inviato           = 0 ")
            Stb.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now) & "  ")
            Stb.Append("   ,DataInvio         = Null ")

            Stb.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            Stb.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            Stb.Append(" WHERE id_cod_cespite =  " & Agro_SQL_SaveNum(IdCodCespite) & "    ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    'End - Modifica





    '#################################################################
    Public Function Cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                             ByVal IdCodCespite As Long, _
                             Optional ByVal xFiltroAggiuntivo As String = "" _
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE Cespiti ")
                Stb.Append(" SET ... ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM Cespiti ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If IdCodCespite <> 0 Then
                Stb.Append(" AND id_cod_cespite = " & Agro_SQL_SaveNum(IdCodCespite) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class





