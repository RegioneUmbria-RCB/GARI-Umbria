Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class BIO_Notifica_SezF_Appezzamenti_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Notifica_ID As Int32, _
                            ByVal Progressivo_Appezzamento As Int32, _
                                ByVal Progressivo_Particelle As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                Optional ByVal leftJoin As Boolean = False _
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim innerLeft As String
        If leftJoin Then
            innerLeft = " left "
        Else
            innerLeft = " inner "
        End If

        Try

            strSQL.Length = 0
            strSQL.Append(" SELECT  BIO_Notifica_SezF_Appezzamenti.*, MetodoProduzione_Des, TipologiaColtura_Des ")
            strSQL.Append(" FROM BIO_Notifica_SezF_Appezzamenti  ")
            strSQL.Append(innerLeft & " JOIN  BIO_Dati_MetodoProduzione ON BIO_Notifica_SezF_Appezzamenti.MetodoProduzione_Cod = BIO_Dati_MetodoProduzione.MetodoProduzione_Cod ")
            strSQL.Append(innerLeft & " JOIN  BIO_Dati_TipologiaColtura ON BIO_Notifica_SezF_Appezzamenti.TipologiaColtura_Cod = BIO_Dati_TipologiaColtura.TipologiaColtura_Cod ")

            '----- Condizioni
            strSQL.Append(" WHERE BIO_Notifica_SezF_Appezzamenti.Notifica_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")

            If Notifica_ID <> 0 Then
                strSQL.Append(" AND BIO_Notifica_SezF_Appezzamenti.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  ")
            End If

            If Progressivo_Appezzamento <> 0 Then
                strSQL.Append(" AND Progressivo_Appezzamento = " & Agro_SQL_SaveNum(Progressivo_Appezzamento) & "  ")
            End If

            If Progressivo_Particelle <> 0 Then
                strSQL.Append(" AND Progressivo_Particelle = " & Agro_SQL_SaveNum(Progressivo_Particelle) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSQL.Append(" ORDER BY Progressivo_Appezzamento, CodIstat_Provincia, CodIstat_Comune, Sezione, Foglio, Numero, Subalterno ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function LeggiJoinCompleta(ByVal Notifica_ID As Int32,
                                        ByVal Progressivo_Appezzamento As Int32,
                                        ByVal Progressivo_Particelle As Int32,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_R.LeggiJoinCompleta()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSQL.Length = 0
            strSQL.Append(" SELECT  BIO_Notifica_SezF_Appezzamenti.* ")
            strSQL.Append(" FROM BIO_Notifica_SezF_Appezzamenti  ")

            '----- Condizioni
            strSQL.Append(" WHERE BIO_Notifica_SezF_Appezzamenti.Notifica_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")

            If Notifica_ID <> 0 Then
                strSQL.Append(" AND BIO_Notifica_SezF_Appezzamenti.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  ")
            End If

            If Progressivo_Appezzamento <> 0 Then
                strSQL.Append(" AND Progressivo_Appezzamento = " & Agro_SQL_SaveNum(Progressivo_Appezzamento) & "  ")
            End If

            If Progressivo_Particelle <> 0 Then
                strSQL.Append(" AND Progressivo_Particelle = " & Agro_SQL_SaveNum(Progressivo_Particelle) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                'Else
                'strSQL.Append(" ORDER BY " )
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function LeggiJoinPlanning(ByVal Notifica_ID As Int32,
                                        ByVal Progressivo_Appezzamento As Int32,
                                        ByVal Progressivo_Particelle As Int32,
                                        ByVal Programmazione_Entita_Cod As Int32,
                                        ByVal Programmazione_Cod As Int32,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_R.LeggiJoinPlanning()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSQL.Length = 0
            strSQL.Append(" SELECT  BIO_Notifica_SezF_Appezzamenti.*,  " & vbCrLf)
            strSQL.Append(" Notifica_Fotografia_Data, Notifica_Ricevimento_Protocollo, Notifica_Ricevimento_Data, " & vbCrLf)
            strSQL.Append(" Programmazione_Entita.Programmazione_Entita_Cod, Programmazione_Entita.Programmazione_Cod, Entita_Des, Programmazione_Des  " & vbCrLf)

            strSQL.Append(" FROM BIO_Notifica_SezF_Appezzamenti  " & vbCrLf)

            strSQL.Append(" INNER JOIN BIO_Notifica_Frontespizio ON BIO_Notifica_Frontespizio.Notifica_SuperUser = BIO_Notifica_SezF_Appezzamenti.Notifica_SuperUser   " & vbCrLf)
            strSQL.Append(" AND BIO_Notifica_Frontespizio.Notifica_ID = BIO_Notifica_SezF_Appezzamenti.Notifica_ID " & vbCrLf)

            strSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Entita.Piva_SuperUser = BIO_Notifica_SezF_Appezzamenti.Notifica_SuperUser   " & vbCrLf)
            strSQL.Append(" AND Programmazione_Entita.Programmazione_Entita_Cod = BIO_Notifica_SezF_Appezzamenti.Programmazione_Entita_Cod " & vbCrLf)

            strSQL.Append(" INNER JOIN Programmazione_Testata ON Programmazione_Entita.Piva_SuperUser = Programmazione_Testata.Piva_SuperUser   " & vbCrLf)
            strSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod " & vbCrLf)

            '----- Condizioni
            strSQL.Append(" WHERE BIO_Notifica_SezF_Appezzamenti.Notifica_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' " & vbCrLf)

            If Notifica_ID <> 0 Then
                strSQL.Append(" AND BIO_Notifica_SezF_Appezzamenti.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  " & vbCrLf)
            End If

            If Progressivo_Appezzamento <> 0 Then
                strSQL.Append(" AND Progressivo_Appezzamento = " & Agro_SQL_SaveNum(Progressivo_Appezzamento) & "  " & vbCrLf)
            End If

            If Progressivo_Particelle <> 0 Then
                strSQL.Append(" AND Progressivo_Particelle = " & Agro_SQL_SaveNum(Progressivo_Particelle) & "  " & vbCrLf)
            End If

            If Programmazione_Entita_Cod <> 0 Then
                strSQL.Append(" AND BIO_Notifica_SezF_Appezzamenti.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  " & vbCrLf)
            End If

            If Programmazione_Cod <> 0 Then
                strSQL.Append(" AND Programmazione_Testata.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                'Else
                'strSQL.Append(" ORDER BY " )
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    'filtrare in base al codice necessario
    Public Function Esiste_NotificaConPlanning(ByVal Notifica_ID As Integer,
                                                ByVal Progressivo_Appezzamento As Int32,
                                                ByVal Programmazione_Entita_Cod As Int32,
                                                ByVal Programmazione_Cod As Int32,
                                                ByVal Flag_RitornaInfoNotifica As Boolean,
                                                ByRef InfoNotifica As String,
                                                ByVal Flag_RitornaInfoPlanning As Boolean,
                                                ByRef InfoPlanning As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim dt As DataTable
        Dim Flag_Esiste As Boolean = False

        InfoNotifica = ""
        InfoPlanning = ""

        dt = LeggiJoinPlanning(Notifica_ID,
                                Progressivo_Appezzamento,
                                0,
                                Programmazione_Entita_Cod,
                                Programmazione_Cod,
                                xFiltroAggiuntivo,
                                "",
                                objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Flag_Esiste = True

            If Flag_RitornaInfoNotifica = True Or Flag_RitornaInfoPlanning = True Then

                'no ciclo, perché ci sono tante righe quante le righe della sezione F
                'Dim i As Integer
                'For i = 0 To DT.Rows.Count - 1

                'questo flag ha senso se si sta filtrando dalle chiavi del planning e si vuole sapere qual è la notifica collegata
                If Flag_RitornaInfoNotifica = True Then
                    InfoNotifica = "Notifica del " & dt.Rows(0).Item("Notifica_Fotografia_Data")
                End If

                'questo flag ha senso se si sta filtrando dalla chiave della notifica e si vuole sapere qual è il planning collegato
                If Flag_RitornaInfoPlanning = True Then
                    InfoPlanning = dt.Rows(0).Item("Programmazione_Des")
                End If

                ' Next
            End If
        End If

        Return Flag_Esiste

    End Function

    '##################################################################################
    Public Function TipologiaColturaCod_from_CampoCodData(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Campo_Cod As Integer,
                                                        ByVal Data As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer


        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_R.TipologiaColturaCod_from_CampoCod()"

        Dim TipologiaColtura_Cod As Integer = 0
        Dim objcampi As New AgronicaCoreAnagrafeDAL.Campi_R
        Dim dt As DataTable
        Dim i As Integer
        Dim VegCodPrec As Integer = 0
        Dim GruCodPrec As Integer = 0
        Dim GruCod As Integer
        Dim Flag_Consociata As Boolean = False

        Try

            dt = objcampi.LeggiDatiImpianto_from_CampoCodData(Piva, Sa_Cod, Campo_Cod, Data,
                                                                "", " SpecieVegetali.gru_cod, SpecieVegetali.VEG_COD ",
                                                                objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                For i = 0 To dt.Rows.Count - 1

                    Select Case dt.Rows(i).Item("gru_cod")
                        Case 2, 3
                            GruCod = 3
                        Case Else
                            GruCod = 1
                    End Select

                    If i <> 0 Then

                        If GruCod <> GruCodPrec Then
                            'GRUPPI COLTURALI DIVERSI -> PROMISCUA
                            TipologiaColtura_Cod = 5    'promiscua = arborea ed erbacea
                            Exit For
                        ElseIf dt.Rows(i).Item("Veg_Cod") = VegCodPrec Then
                            'se nel ciclo non sono mai passato da consociata
                            If Flag_Consociata = False Then
                                'GRUPPI COLTURALI UGUALI, SPECIE UGUALI -> PURA
                                Select Case dt.Rows(i).Item("gru_cod")
                                    Case 2, 3
                                        TipologiaColtura_Cod = 3    'Erbacea pura
                                    Case Else
                                        TipologiaColtura_Cod = 1  'Arborea pura
                                End Select
                            Else
                                'DEVO LASCIARE CONSOCIATO, NON PUO' DIVENTARE PURO
                                Select Case dt.Rows(i).Item("gru_cod")
                                    Case 2, 3
                                        TipologiaColtura_Cod = 4    'Erbacea consociata
                                    Case Else
                                        TipologiaColtura_Cod = 2  'Arborea consociata
                                End Select
                            End If
                        Else
                            'GRUPPI COLTURALI UGUALI, SPECIE DIVERSE -> CONSOCIATA
                            Flag_Consociata = True
                            Select Case dt.Rows(i).Item("gru_cod")
                                Case 2, 3
                                    TipologiaColtura_Cod = 4    'Erbacea consociata
                                Case Else
                                    TipologiaColtura_Cod = 2  'Arborea consociata
                            End Select
                        End If
                    Else
                        TipologiaColtura_Cod = GruCod
                    End If

                    VegCodPrec = dt.Rows(i).Item("Veg_Cod")
                    GruCodPrec = dt.Rows(i).Item("gru_cod")

                Next

            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return TipologiaColtura_Cod


    End Function


    '##################################################################################
    'Attenzione, questa è la versione del Quadro P senza consociazione, quella ufficiale è in
    'AgronicaCoreStampeDAL.AnagraficaAziendale.Quadro_P
    'Questa versione inoltre è dedicata solo alla Sezione F della notifica bio.
    Public Function QuadroP_xSezioneF(ByVal Piva As String,
                                    ByVal DataNotifica As Date,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_R.QuadroP_xSezioneF()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0


            '------------------------------------------------
            'PARTICELLE ASSOCIATE AD APPEZZAMENTI (non separo impianti normali da consociati, come nel QuadroP)
            '------------------------------------------------
            StrSQL.Append(" ( " & vbCrLf)
            StrSQL.Append(" SELECT DISTINCT " & vbCrLf)
            StrSQL.Append(" dbo.Reg_Impianti.PIVA, dbo.Reg_Impianti.SA_COD, 0 AS Campo_Cod, dbo.Reg_Impianti.APPEZZA AS Appezza, dbo.Reg_Impianti.ID_REG AS Id_Reg, 	 " & vbCrLf)
            StrSQL.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.PART_COD, dbo.ParticelleCatastali.PROV, dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, 	 " & vbCrLf)
            StrSQL.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, 	 " & vbCrLf)
            StrSQL.Append(" dbo.ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, 	 " & vbCrLf)
            StrSQL.Append(" dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, dbo.AppezzamentiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, 	 " & vbCrLf)
            StrSQL.Append(" dbo.AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, 	 " & vbCrLf)
            StrSQL.Append(" dbo.Appezzamento.Validita_Inizio, dbo.Appezzamento.Validita_Fine, dbo.SpecieVegetali.Veg_Des, dbo.Cultivar.Cul_Des, ImpresexParticelle.TitoloPossesso  " & vbCrLf)
            'metodo di produzione
            StrSQL.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  ")
            StrSQL.Append("           FROM Appezzamento_Codici ")
            StrSQL.Append("           WHERE Appezzamento_Codici.piva = '" & Agro_SQL_SaveText(Piva) & "' AND  Appezzamento_Codici.sa_cod = Appezzamento.sa_cod AND Appezzamento_Codici.appezza = Appezzamento.appezza  ")
            StrSQL.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & "  ), '1') AS MetodoProduzione_Cod ")
            'num appezza bio
            StrSQL.Append(" , ISNULL((SELECT top 1 Appezzamento_Codici.Val_Cod FROM Appezzamento_Codici " & vbCrLf)
            StrSQL.Append("         WHERE Appezzamento_Codici.piva = '" & Agro_SQL_SaveText(Piva) & "' and  Appezzamento_Codici.sa_cod = Appezzamento.sa_cod and Appezzamento_Codici.appezza = Appezzamento.appezza  " & vbCrLf)
            StrSQL.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.Codice_Appezza_Biologico) & "  ), '-1') AS Num_Appezzamento_Str " & vbCrLf)
            'veg_cod
            StrSQL.Append(" , Cultivar.Veg_Cod  " & vbCrLf)
            'orientamento produttivo
            StrSQL.Append(" , ISNULL((SELECT top 1 Appezzamento_Codici.Val_Cod FROM Appezzamento_Codici " & vbCrLf)
            StrSQL.Append("         WHERE Appezzamento_Codici.piva = '" & Agro_SQL_SaveText(Piva) & "' and  Appezzamento_Codici.sa_cod = Appezzamento.sa_cod and Appezzamento_Codici.appezza = Appezzamento.appezza  " & vbCrLf)
            StrSQL.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.OrientamentoProduttivo) & "  ), '-1') AS OrientamentoProduttivo_Cod_1 " & vbCrLf)
            'Gru_cod
            StrSQL.Append(" , SpecieVegetali.gru_cod " & vbCrLf)
            'Data_FineImpiegoProdottiNonConformi
            StrSQL.Append(" , ISNULL((SELECT top 1 Appezzamento_Codici.Val_Cod FROM Appezzamento_Codici " & vbCrLf)
            StrSQL.Append("         WHERE Appezzamento_Codici.piva = '" & Agro_SQL_SaveText(Piva) & "' and  Appezzamento_Codici.sa_cod = Appezzamento.sa_cod and Appezzamento_Codici.appezza = Appezzamento.appezza  " & vbCrLf)
            StrSQL.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.DataFineImpiegoPNC) & "  ), '') AS Data_FineImpiegoProdottiNonConformi " & vbCrLf)
            'App_Nome
            StrSQL.Append(" , Appezzamento.App_Nome, Reg_Impianti.Id_Consociazione  " & vbCrLf)

            StrSQL.Append(" FROM dbo.SpecieVegetali " & vbCrLf)

            StrSQL.Append(" INNER JOIN " & vbCrLf)
            StrSQL.Append(" dbo.Cultivar ON dbo.SpecieVegetali.Veg_Cod = dbo.Cultivar.Veg_Cod  " & vbCrLf)

            StrSQL.Append(" RIGHT OUTER JOIN " & vbCrLf)
            StrSQL.Append(" dbo.ParticelleCatastali " & vbCrLf)

            StrSQL.Append(" INNER JOIN " & vbCrLf)
            StrSQL.Append(" 	dbo.AppezzamentiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV AND 	 " & vbCrLf)
            StrSQL.Append(" 	dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ParticelleCatastali.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE AND  " & vbCrLf)
            StrSQL.Append(" 	dbo.ParticelleCatastali.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND  " & vbCrLf)
            StrSQL.Append(" 	dbo.ParticelleCatastali.NUMERO = dbo.AppezzamentiXParticelle.NUMERO AND  " & vbCrLf)
            StrSQL.Append(" 	dbo.ParticelleCatastali.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO INNER JOIN	 " & vbCrLf)
            StrSQL.Append(" 	dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM INNER JOIN " & vbCrLf)
            StrSQL.Append(" 	dbo.Appezzamento ON dbo.AppezzamentiXParticelle.PIVA = dbo.Appezzamento.PIVA AND  " & vbCrLf)
            StrSQL.Append(" 	dbo.AppezzamentiXParticelle.SA_COD = dbo.Appezzamento.SA_COD AND  " & vbCrLf)
            StrSQL.Append(" 	dbo.AppezzamentiXParticelle.APPEZZA = dbo.Appezzamento.APPEZZA INNER JOIN " & vbCrLf)
            StrSQL.Append(" 	dbo.Reg_Impianti ON dbo.Appezzamento.PIVA = dbo.Reg_Impianti.PIVA AND dbo.Appezzamento.SA_COD = dbo.Reg_Impianti.SA_COD AND " & vbCrLf)
            StrSQL.Append(" 	dbo.Appezzamento.APPEZZA = dbo.Reg_Impianti.APPEZZA ON dbo.Cultivar.Cul_Cod = dbo.Reg_Impianti.CUL_COD	" & vbCrLf)

            StrSQL.Append(" INNER JOIN " & vbCrLf)
            StrSQL.Append(" dbo.ImpresexParticelle " & vbCrLf)
            StrSQL.Append(" ON dbo.ImpreseXParticelle.PIVA = dbo.AppezzamentiXParticelle.PIVA AND dbo.ImpreseXParticelle.PROV = dbo.AppezzamentiXParticelle.PROV AND  " & vbCrLf)
            StrSQL.Append(" dbo.ImpreseXParticelle.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ImpreseXParticelle.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE AND   " & vbCrLf)
            StrSQL.Append(" dbo.ImpreseXParticelle.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND dbo.ImpreseXParticelle.NUMERO = dbo.AppezzamentiXParticelle.NUMERO AND   " & vbCrLf)
            StrSQL.Append(" dbo.ImpreseXParticelle.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO " & vbCrLf)

            StrSQL.Append(" WHERE dbo.AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StrSQL.Append(" AND dbo.AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataNotifica))
            StrSQL.Append(" AND dbo.AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(DataNotifica))

            StrSQL.Append(" AND dbo.Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(DataNotifica))
            StrSQL.Append(" AND dbo.Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(DataNotifica))

            StrSQL.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataNotifica))
            StrSQL.Append(" AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(DataNotifica))

            StrSQL.Append(" ) " & vbCrLf)

            '--- 

            '------------------------------------------------
            'PARTICELLE ASSOCIATE A CAMPI 
            '------------------------------------------------
            StrSQL.Append(" UNION ALL " & vbCrLf)

            StrSQL.Append(" ( " & vbCrLf)
            StrSQL.Append(" SELECT DISTINCT dbo.CampiXParticelle.PIVA as PIVA, dbo.CampiXParticelle.SA_COD as Sa_Cod, dbo.CampiXParticelle.Campo_Cod as Campo_Cod, 0 as Appezza, 0 as Id_Reg," & vbCrLf)
            StrSQL.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StrSQL.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StrSQL.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)
            StrSQL.Append(" dbo.ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)
            StrSQL.Append(" dbo.CampiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, dbo.CampiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.CampiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)
            StrSQL.Append(" dbo.Campi.Validita_Inizio as Validita_Inizio, dbo.Campi.Validita_Fine as Validita_Fine, " & vbCrLf)
            StrSQL.Append(" '' as Veg_Des, '' as Cul_Des, ImpresexParticelle.TitoloPossesso " & vbCrLf)

            'prende il metodo produzione del primo appezzamento sotto al campo
            StrSQL.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  " & vbCrLf)
            StrSQL.Append("           FROM Appezzamento_Codici " & vbCrLf)
            StrSQL.Append("           INNER JOIN Appezzamento ON Appezzamento.Piva = Appezzamento_Codici.Piva AND Appezzamento.Sa_Cod = Appezzamento_Codici.Sa_Cod AND Appezzamento.Appezza= Appezzamento_Codici.Appezza " & vbCrLf)
            StrSQL.Append("           WHERE Appezzamento.piva = Campi.Piva AND Appezzamento.sa_cod = Campi.sa_cod AND Appezzamento.campo_cod = Campi.campo_cod " & vbCrLf)
            StrSQL.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & "  ), '1') AS MetodoProduzione_Cod " & vbCrLf)

            'prende il NUMERO APPEZZAMENTO BIO del primo appezzamento sotto al campo
            StrSQL.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  " & vbCrLf)
            StrSQL.Append("           FROM Appezzamento_Codici " & vbCrLf)
            StrSQL.Append("           INNER JOIN Appezzamento ON Appezzamento.Piva = Appezzamento_Codici.Piva AND Appezzamento.Sa_Cod = Appezzamento_Codici.Sa_Cod AND Appezzamento.Appezza= Appezzamento_Codici.Appezza " & vbCrLf)
            StrSQL.Append("           WHERE Appezzamento.piva = Campi.Piva AND Appezzamento.sa_cod = Campi.sa_cod AND Appezzamento.campo_cod = Campi.campo_cod " & vbCrLf)
            StrSQL.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.Codice_Appezza_Biologico) & "  ), '999') AS Num_Appezzamento_Str " & vbCrLf)

            'default veg_cod =0
            StrSQL.Append("     , 0 AS veg_cod " & vbCrLf)
            ''prende il veg_cod del primo appezzamento sotto al campo
            'StrSQL.Append(" , ISNULL((SELECT TOP 1 Cultivar.Veg_Cod  " & vbCrLf)
            'StrSQL.Append("           FROM Appezzamento " & vbCrLf)
            'StrSQL.Append("           INNER JOIN Reg_Impianti ON Appezzamento.Piva = Reg_Impianti.Piva AND Appezzamento.Sa_Cod = Reg_Impianti.Sa_Cod AND Appezzamento.Appezza= Reg_Impianti.Appezza " & vbCrLf)
            'StrSQL.Append("           INNER JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod " & vbCrLf)
            'StrSQL.Append("           WHERE Appezzamento.piva = Campi.Piva AND Appezzamento.sa_cod = Campi.sa_cod AND Appezzamento.campo_cod = Campi.campo_cod " & vbCrLf)
            'StrSQL.Append("            ), '-1') AS veg_cod " & vbCrLf)

            'prende gli orientamenti produttivi di tutti gli appezzamenti sotto al campo
            StrSQL.Append(" , dbo.ElencoOrientamentiProduttivi_from_CampoData(Campi.Piva, Campi.sa_cod, Campi.Campo_Cod, " & Agro_SQL_SaveDate(DataNotifica) & ") AS OrientamentoProduttivo_Cod_1 " & vbCrLf)

            'StrSQL.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  " & vbCrLf)
            'StrSQL.Append("           FROM Appezzamento_Codici " & vbCrLf)
            'StrSQL.Append("           INNER JOIN Appezzamento ON Appezzamento.Piva = Appezzamento_Codici.Piva AND Appezzamento.Sa_Cod = Appezzamento_Codici.Sa_Cod AND Appezzamento.Appezza= Appezzamento_Codici.Appezza " & vbCrLf)
            'StrSQL.Append("           WHERE Appezzamento.piva = Campi.Piva AND Appezzamento.sa_cod = Campi.sa_cod AND Appezzamento.campo_cod = Campi.campo_cod " & vbCrLf)
            'StrSQL.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.OrientamentoProduttivo) & "  ), '-1') AS OrientamentoProduttivo_Cod_1 " & vbCrLf)

            'default gru_cod =0
            StrSQL.Append("     , 0 AS gru_cod " & vbCrLf)
            ''prende il Gru_cod del primo appezzamento sotto al campo
            'StrSQL.Append(" , ISNULL((SELECT TOP 1 SpecieVegetali.gru_cod " & vbCrLf)
            'StrSQL.Append("           FROM Appezzamento " & vbCrLf)
            'StrSQL.Append("           INNER JOIN Reg_Impianti ON Appezzamento.Piva = Reg_Impianti.Piva AND Appezzamento.Sa_Cod = Reg_Impianti.Sa_Cod AND Appezzamento.Appezza= Reg_Impianti.Appezza " & vbCrLf)
            'StrSQL.Append("           INNER JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod " & vbCrLf)
            'StrSQL.Append("           INNER JOIN SpecieVegetali ON Cultivar.Veg_cod = SpecieVegetali.Veg_cod " & vbCrLf)
            'StrSQL.Append("           WHERE Appezzamento.piva = Campi.Piva AND Appezzamento.sa_cod = Campi.sa_cod AND Appezzamento.campo_cod = Campi.campo_cod " & vbCrLf)
            'StrSQL.Append("            ), '-1') AS gru_cod " & vbCrLf)

            'prende Data_FineImpiegoProdottiNonConformi del primo appezzamento sotto al campo
            StrSQL.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  " & vbCrLf)
            StrSQL.Append("           FROM Appezzamento_Codici " & vbCrLf)
            StrSQL.Append("           INNER JOIN Appezzamento ON Appezzamento.Piva = Appezzamento_Codici.Piva AND Appezzamento.Sa_Cod = Appezzamento_Codici.Sa_Cod AND Appezzamento.Appezza= Appezzamento_Codici.Appezza " & vbCrLf)
            StrSQL.Append("           WHERE Appezzamento.piva = Campi.Piva AND Appezzamento.sa_cod = Campi.sa_cod AND Appezzamento.campo_cod = Campi.campo_cod " & vbCrLf)
            StrSQL.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.DataFineImpiegoPNC) & "  ), '') AS Data_FineImpiegoProdottiNonConformi " & vbCrLf)

            StrSQL.Append(" , Campi.Campo_Des AS App_Nome, 0 AS Id_Consociazione  " & vbCrLf)

            'StrSQL.Append("  '-1', '-1', '-1', '-1', '-1', '', '', 0 AS Id_Consociazione " & vbCrlF )

            StrSQL.Append(" FROM dbo.ParticelleCatastali " & vbCrLf)

            StrSQL.Append(" INNER JOIN dbo.CampiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.CampiXParticelle.PROV " & vbCrLf)
            StrSQL.Append(" AND dbo.ParticelleCatastali.COM = dbo.CampiXParticelle.COM " & vbCrLf)
            StrSQL.Append(" AND dbo.ParticelleCatastali.sezione = dbo.CampiXParticelle.sezione " & vbCrLf)
            StrSQL.Append(" AND dbo.ParticelleCatastali.foglio = dbo.CampiXParticelle.foglio " & vbCrLf)
            StrSQL.Append(" AND dbo.ParticelleCatastali.numero = dbo.CampiXParticelle.numero " & vbCrLf)
            StrSQL.Append(" AND dbo.ParticelleCatastali.subalterno = dbo.CampiXParticelle.subalterno " & vbCrLf)

            StrSQL.Append(" INNER JOIN dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV " & vbCrLf)
            StrSQL.Append(" AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM " & vbCrLf)

            StrSQL.Append(" INNER JOIN dbo.Campi ON dbo.CampiXParticelle.PIVA = dbo.Campi.PIVA " & vbCrLf)
            StrSQL.Append(" AND dbo.CampiXParticelle.SA_COD = dbo.Campi.SA_COD " & vbCrLf)
            StrSQL.Append(" AND dbo.CampiXParticelle.Campo_Cod = dbo.Campi.Campo_Cod " & vbCrLf)

            StrSQL.Append(" INNER JOIN  dbo.ImpresexParticelle " & vbCrLf)
            StrSQL.Append(" ON dbo.ImpreseXParticelle.PIVA = dbo.CampiXParticelle.PIVA AND dbo.ImpreseXParticelle.PROV = dbo.CampiXParticelle.PROV AND  " & vbCrLf)
            StrSQL.Append(" dbo.ImpreseXParticelle.COM = dbo.CampiXParticelle.COM AND dbo.ImpreseXParticelle.SEZIONE = dbo.CampiXParticelle.SEZIONE AND   " & vbCrLf)
            StrSQL.Append(" dbo.ImpreseXParticelle.FOGLIO = dbo.CampiXParticelle.FOGLIO AND dbo.ImpreseXParticelle.NUMERO = dbo.CampiXParticelle.NUMERO AND   " & vbCrLf)
            StrSQL.Append(" dbo.ImpreseXParticelle.SUBALTERNO = dbo.CampiXParticelle.SUBALTERNO " & vbCrLf)

            StrSQL.Append(" WHERE dbo.CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StrSQL.Append(" AND dbo.CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataNotifica))
            StrSQL.Append(" AND dbo.CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(DataNotifica))
            'StrSQL.Append(" And Campi.Veg_Cod <> 0 " & vbCrLf )

            StrSQL.Append(" AND Dbo.CampiXParticelle.Campo_Cod NOT IN " & vbCrLf)
            StrSQL.Append("                                     (SELECT DISTINCT dbo.Appezzamento.Campo_Cod " & vbCrLf)
            StrSQL.Append("                                         FROM  dbo.AppezzamentiXParticelle INNER JOIN " & vbCrLf)
            StrSQL.Append("                                         dbo.Appezzamento ON dbo.AppezzamentiXParticelle.PIVA = dbo.Appezzamento.PIVA AND " & vbCrLf)
            StrSQL.Append("                                         dbo.AppezzamentiXParticelle.SA_COD = dbo.Appezzamento.SA_COD AND " & vbCrLf)
            StrSQL.Append("                                         dbo.AppezzamentiXParticelle.APPEZZA = dbo.Appezzamento.APPEZZA " & vbCrLf)
            StrSQL.Append("                                         WHERE AppezzamentiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StrSQL.Append("                                         AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataNotifica))
            StrSQL.Append("                                         AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(DataNotifica))
            StrSQL.Append("                                     ) " & vbCrLf)

            StrSQL.Append(" ) " & vbCrLf)


            'NON SERVONO NELLA SEZIONE F
            '''''------------------------------------------------
            '''''PARTICELLE NON ASSOCIATE 
            '''''------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.Append(" ORDER BY Reg_Impianti.PIVA, dbo.Reg_Impianti.SA_COD, num_appezzamento_str, ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio, ParticelleCatastali.numero, ParticelleCatastali.subalterno " & vbCrLf)
                StrSQL.Append(" ORDER BY Num_Appezzamento_Str, App_Nome, ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio, ParticelleCatastali.numero, ParticelleCatastali.subalterno " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    '##################################################################################
    Public Function Leggi_Raggruppamento(ByVal Notifica_ID As Int32,
                           ByVal Progressivo_Appezzamento As Int32,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSQL.Length = 0
            strSQL.Append(" SELECT Num_Appezzamento_Str, CodIstat_Comune, CodIstat_Provincia, Sezione, Foglio, Numero, Subalterno,")
            strSQL.Append(" OrientamentoProduttivo_Cod_1, Data_FineImpiegoProdottiNonConformi, MetodoProduzione_Cod, TipologiaColtura_Cod, SUM(Sup_Intersezione*10000) AS Sup_Intersezione ")
            'aggiunta del 20/10/2011
            strSQL.Append(" , Validita_Inizio, Validita_Fine, Sup_Appezzamento   ")

            strSQL.Append(" FROM BIO_Notifica_SezF_Appezzamenti  ")

            '----- Condizioni
            strSQL.Append(" WHERE BIO_Notifica_SezF_Appezzamenti.Notifica_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")

            If Notifica_ID <> 0 Then
                strSQL.Append(" AND BIO_Notifica_SezF_Appezzamenti.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  ")
            End If

            If Progressivo_Appezzamento <> 0 Then
                strSQL.Append(" AND Progressivo_Appezzamento = " & Agro_SQL_SaveNum(Progressivo_Appezzamento) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            strSQL.Append(" GROUP BY Num_Appezzamento_Str, CodIstat_Comune, CodIstat_Provincia, Sezione, Foglio, Numero, Subalterno, ")
            strSQL.Append(" MetodoProduzione_Cod, TipologiaColtura_Cod, Data_FineImpiegoProdottiNonConformi, OrientamentoProduttivo_Cod_1 ")
            'aggiunta del 20/10/2011
            strSQL.Append(" , Validita_Inizio, Validita_Fine, Sup_Appezzamento   ")


            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '#################################################################################################
    Public Function SupAppezzamento_from_Num_Appezzamento_Str(ByVal Notifica_ID As Int32,
                                                                ByVal Num_Appezzamento_Str As String,
                                                                    ByVal xFiltroAggiuntivo As String,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                    ) As Decimal

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_R.SupAppezzamento_from_Num_Appezzamento_Str()"

        Dim messaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim Sup_App As Decimal = 0

        Try

            strSQL.Length = 0
            strSQL.Append(" SELECT  SUM(Sup_Intersezione) AS Sup_App ")
            strSQL.Append(" FROM    BIO_Notifica_SezF_Appezzamenti  ")

            '----- Condizioni
            strSQL.Append(" WHERE BIO_Notifica_SezF_Appezzamenti.Notifica_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
            strSQL.Append(" AND BIO_Notifica_SezF_Appezzamenti.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  ")
            strSQL.Append(" AND Num_Appezzamento_Str = '" & Agro_SQL_SaveText(Num_Appezzamento_Str) & "'  ")

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   BIO_Notifica_SezF_Appezzamenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                Sup_App = dt.Rows(0).Item("Sup_App")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Sup_App

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class BIO_Notifica_SezF_Appezzamenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##########################################################################
    'Data_Creazione e username_creazione sono passati come parametri per tenere traccia 
    'dei dati originali di creazione
    '(la modifica avviene con cancella e scrivi)
    Public Function Scrivi(
                    ByVal Notifica_ID As Int32,
                    ByVal Progressivo_Appezzamento As Int32,
                    ByVal Progressivo_Particelle As Int32,
                    ByVal Flag_Appezzamento1_Particella2 As Int32,
                    ByVal Piva As String,
                    ByVal Sa_Cod As Int32,
                    ByVal Appezza As Int32,
                    ByVal Progressivo_UnitaProduttiva As Int32,
                    ByVal Sup_Appezzamento As Decimal,
                    ByVal Data_FineImpiegoProdottiNonConformi As Date,
                    ByVal MetodoProduzione_Cod As Int32,
                    ByVal TipologiaColtura_Cod As Int32,
                    ByVal OrientamentoProduttivo_Cod_1 As Int32,
                    ByVal OrientamentoProduttivo_Cod_2 As Int32,
                    ByVal OrientamentoProduttivo_Cod_3 As Int32,
                    ByVal OrientamentoProduttivo_Cod_4 As Int32,
                    ByVal OrientamentoProduttivo_Cod_5 As Int32,
                    ByVal CodIstat_Provincia As String,
                    ByVal CodIstat_Comune As String,
                    ByVal Sezione As String,
                    ByVal Foglio As Int32,
                    ByVal Numero As Int32,
                    ByVal Subalterno As String,
                    ByVal Sup_Intersezione As Decimal,
                    ByVal TitoloPossesso As Int32,
                    ByVal Num_Appezzamento_Str As String,
                    ByVal Veg_cod As Int32,
                    ByVal Programmazione_Entita_Cod As Int32,
                    ByVal Username_Creazione As String,
                    ByVal Data_Creazione As Date,
                        ByVal Validita_Inizio As Date,
                        ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            , Optional ByVal Data_modifica As Date = #2/1/1900# _
            , Optional ByVal username_modifica As String = ""
                            ) As Boolean

        Const nomeRoutine = "AnagrafeCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti.Scrivi()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO BIO_Notifica_SezF_Appezzamenti ")
            StrSQL.Append("             (Notifica_SuperUser,")
            StrSQL.Append("             Notifica_ID,")
            StrSQL.Append("             Progressivo_Appezzamento,")
            StrSQL.Append("             Progressivo_Particelle,")
            StrSQL.Append("             Flag_Appezzamento1_Particella2,")
            StrSQL.Append("             Piva,")
            StrSQL.Append("             Sa_Cod,")
            StrSQL.Append("             Appezza,")
            StrSQL.Append("             Progressivo_UnitaProduttiva,")
            StrSQL.Append("             Sup_Appezzamento,")
            StrSQL.Append("             Data_FineImpiegoProdottiNonConformi,")
            StrSQL.Append("             MetodoProduzione_Cod,")
            StrSQL.Append("             TipologiaColtura_Cod,")
            StrSQL.Append("             OrientamentoProduttivo_Cod_1,")
            StrSQL.Append("             OrientamentoProduttivo_Cod_2,")
            StrSQL.Append("             OrientamentoProduttivo_Cod_3,")
            StrSQL.Append("             OrientamentoProduttivo_Cod_4,")
            StrSQL.Append("             OrientamentoProduttivo_Cod_5,")
            StrSQL.Append("             CodIstat_Provincia,")
            StrSQL.Append("             CodIstat_Comune,")
            StrSQL.Append("             Sezione,")
            StrSQL.Append("             Foglio,")
            StrSQL.Append("             Numero,")
            StrSQL.Append("             Subalterno,")
            StrSQL.Append("             Sup_Intersezione,")
            StrSQL.Append("             TitoloPossesso,")
            StrSQL.Append("             Num_Appezzamento_Str,")
            StrSQL.Append("             Veg_cod,")
            StrSQL.Append("             Programmazione_Entita_Cod,")
            '----- Info Standard
            StrSQL.Append("             Inviato,")
            StrSQL.Append("             DataInvio,")
            StrSQL.Append("             Data_Creazione,")
            StrSQL.Append("             Data_Modifica,")
            StrSQL.Append("             Username_Creazione,")
            StrSQL.Append("             Username_Modifica,")
            StrSQL.Append("             Validita_Inizio,")
            StrSQL.Append("             Validita_Fine,")
            StrSQL.Append("             DataLock) ")
            StrSQL.Append("  VALUES(              ")

            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Notifica_ID)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Progressivo_Appezzamento)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Progressivo_Particelle)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Flag_Appezzamento1_Particella2)) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Piva)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Sa_Cod)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Appezza)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Progressivo_UnitaProduttiva)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Sup_Appezzamento)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveDate(Data_FineImpiegoProdottiNonConformi) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(MetodoProduzione_Cod)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(TipologiaColtura_Cod)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(OrientamentoProduttivo_Cod_1)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(OrientamentoProduttivo_Cod_2)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(OrientamentoProduttivo_Cod_3)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(OrientamentoProduttivo_Cod_4)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(OrientamentoProduttivo_Cod_5)) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(CodIstat_Provincia)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(CodIstat_Comune)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Sezione)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Foglio)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Numero)) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Subalterno)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Sup_Intersezione)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(TitoloPossesso)) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Num_Appezzamento_Str)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Veg_cod)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Programmazione_Entita_Cod)) & " ")

            '----- Info Standard
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(0) & ") ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal Notifica_ID As Int32,
                                ByVal Progressivo_Appezzamento As Int32,
                                ByVal Progressivo_Particelle As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Notifica_ID = 0 Then
                Throw New Exception("Parametro non corretto nella query (Notifica_ID obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE BIO_Notifica_SezF_Appezzamenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM BIO_Notifica_SezF_Appezzamenti ")
                StrSQL.Append(" WHERE Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  ")
            End If
            '---------------------------------------------

            If Progressivo_Appezzamento <> 0 Then
                StrSQL.Append(" AND Progressivo_Appezzamento = " & Agro_SQL_SaveNum(Progressivo_Appezzamento) & "  ")
            End If

            If Progressivo_Particelle <> 0 Then
                StrSQL.Append(" AND Progressivo_Particelle = " & Agro_SQL_SaveNum(Progressivo_Particelle) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
