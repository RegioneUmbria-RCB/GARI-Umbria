Imports System.Text
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json

Public Class Lavorazioni_CriteriAggrega_R : Inherits DataProvider

    ''' <summary>
    ''' I filtri su preparazioneCod e preparazioneCodLinea vengono applicati 
    ''' solo se i valori passati sono diversi da Nothing
    ''' </summary>
    Public Function Leggi(
        piva As String,
        preparazioneCod As Integer?,
        preparazioneCodLinea As Integer?,
        xFiltroAggiuntivo As String,
        xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_R.Leggi()"
        Dim dt As DataTable = Nothing
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String = String.Empty

        Try

            strSql.AppendLine(" select * from Lavorazioni_CriteriAggregazione ")
            strSql.AppendLine(" where piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If Not IsNothing(preparazioneCod) Then
                strSql.AppendLine(" and preparazione_cod = " & Agro_SQL_SaveNum(preparazioneCod) & " ")
            End If

            If Not IsNothing(preparazioneCodLinea) Then
                strSql.AppendLine(" and preparazione_cod_linea = " & Agro_SQL_SaveNum(preparazioneCodLinea) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, objParametri:=objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY preparazione_cod ")
            End If

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    ''' <summary>
    ''' I filtri su preparazioneCod e preparazioneCodLinea vengono applicati 
    ''' solo se i valori passati sono diversi da Nothing
    ''' </summary>
    Public Function LeggiComeLista(
        piva As String,
        preparazioneCod As Integer?,
        preparazioneCodLinea As Integer?,
        xFiltroAggiuntivo As String,
        xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
        ) As List(Of CriteriAggregazioneLavorazioni)

        Dim dtCriteri = Leggi(piva,
                              preparazioneCod,
                              preparazioneCodLinea,
                              xFiltroAggiuntivo,
                              xOrderBy,
                              objParametri)

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_R.LeggiComeLista()"

        Dim messaggioErrore As String = String.Empty

        Dim listaCriteri = New List(Of CriteriAggregazioneLavorazioni)

        Try

            If Not IsNothing(dtCriteri) And dtCriteri.Rows.Count > 0 Then

                For Each rigaCriterio As DataRow In dtCriteri.Rows

                    Dim elementoLista = New CriteriAggregazioneLavorazioni

                    elementoLista.Piva = rigaCriterio.Item("Piva")
                    elementoLista.PreparazioneCod = rigaCriterio.Item("Preparazione_Cod")
                    elementoLista.PreparazioneCodLinea = rigaCriterio.Item("Preparazione_Cod_Linea")
                    elementoLista.AggregaFornitore = rigaCriterio.Item("AggregaFornitore")
                    elementoLista.AggregaSpecie = rigaCriterio.Item("AggregaSpecie")
                    elementoLista.AggregaVarieta = rigaCriterio.Item("AggregaVarieta")
                    elementoLista.AggregaRegolamento = rigaCriterio.Item("AggregaRegolamento")
                    elementoLista.AggregaLotto = rigaCriterio.Item("AggregaLotto")
                    elementoLista.LottoModificabileInUscita = rigaCriterio.Item("LottoModificabileInUscita")
                    elementoLista.AggregaProdotto = rigaCriterio.Item("AggregaProdotto")
                    elementoLista.ProdottoModificabileInUscita = rigaCriterio.Item("ProdottoModificabileInUscita")
                    elementoLista.AggregaUdm = rigaCriterio.Item("AggregaUdm")
                    elementoLista.AggregaCella = rigaCriterio.Item("AggregaCella")
                    elementoLista.CellaModificabileInUscita = rigaCriterio.Item("CellaModificabileInUscita")

                    elementoLista.CriteriParamQual = JsonConvert.DeserializeObject(Of List(Of CriteriParametriQualitativi))(rigaCriterio.Item("CriteriParamQual"))

                    listaCriteri.Add(elementoLista)

                Next

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            listaCriteri = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return listaCriteri

    End Function

    ''' <summary>
    ''' I filtri su preparazioneCod e preparazioneCodLinea vengono applicati 
    ''' solo se i valori passati sono diversi da Nothing
    ''' </summary>
    Public Function LeggiComeJson(
        piva As String,
        preparazioneCod As Integer?,
        preparazioneCodLinea As Integer?,
        xFiltroAggiuntivo As String,
        xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
        ) As String

        Dim listaCriteri = LeggiComeLista(piva,
                                          preparazioneCod,
                                          preparazioneCodLinea,
                                          xFiltroAggiuntivo,
                                          xOrderBy,
                                          objParametri)

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_R.LeggiComeJson()"

        Dim messaggioErrore As String = String.Empty

        Dim jsonCriteri = ""

        Try

            Dim serializerSettings As New JsonSerializerSettings()

            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            jsonCriteri = JsonConvert.SerializeObject(listaCriteri, Formatting.None, serializerSettings)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            listaCriteri = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return jsonCriteri

    End Function

    Public Function LeggiPerGriglia(ByVal piva As String, xFiltroAggiuntivo As String, xOrderBy As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_R.LeggiPerGriglia()"
        Dim dt As DataTable = Nothing
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String = String.Empty

        Try

            strSql.Length = 0

            strSql.AppendLine(" select ROW_NUMBER() OVER(ORDER BY l.PIVA, l.preparazione_cod ASC) as key_criteri_aggregazione, ")
            strSql.AppendLine("l.piva as Piva, ")
            strSql.AppendLine("i.rag_soc as Rag_Soc, ")
            strSql.AppendLine("lp.Preparazione_Cod as Tipologia_Lavorazione_Cod, ")
            strSql.AppendLine("lp.Preparazione_Des as Tipologia_Lavorazione, ")
            strSql.AppendLine("l.Preparazione_Cod_Linea as Tipologia_Lavorazione_Linea_Cod, ")
            strSql.AppendLine("lp.Preparazione_Des + ' - ' + lprod.Linea_Des as Tipologia_Lavorazione_Linea, ")
            strSql.AppendLine("l.AggregaFornitore As Aggrega_Fornitore_Cod, ")
            strSql.AppendLine("'' As Aggrega_Fornitore, ")
            strSql.AppendLine("l.AggregaSpecie As Aggrega_Specie_Cod, ")
            strSql.AppendLine("'' As Aggrega_Specie, ")
            strSql.AppendLine("l.AggregaVarieta As Aggrega_Varieta_Cod, ")
            strSql.AppendLine("'' As Aggrega_Varieta, ")
            strSql.AppendLine("l.AggregaRegolamento As Aggrega_Regolamento_Cod, ")
            strSql.AppendLine("'' As Aggrega_Regolamento, ")
            strSql.AppendLine("l.AggregaLotto As Aggrega_Lotto_Cod, ")
            strSql.AppendLine("'' As Aggrega_Lotto, ")
            strSql.AppendLine("l.LottoModificabileInUscita As Lotto_Modifica_Uscita_Cod, ")
            strSql.AppendLine("'' As Lotto_Modifica_Uscita, ")
            strSql.AppendLine("l.ProdottoModificabileInUscita As Prodotto_Modifica_Uscita_Cod, ")
            strSql.AppendLine("'' As Prodotto_Modifica_Uscita, ")
            strSql.AppendLine("l.AggregaProdotto As Aggrega_Prodotto_Cod, ")
            strSql.AppendLine("'' As Aggrega_Prodotto, ")
            strSql.AppendLine("l.AggregaCella As Aggrega_Cella_Cod, ")
            strSql.AppendLine("'' As Aggrega_Cella, ")
            strSql.AppendLine("l.CellaModificabileInUscita As Cella_Modifica_Uscita_Cod, ")
            strSql.AppendLine("'' As Cella_Modifica_Uscita, ")
            strSql.AppendLine("l.AggregaUdm As Aggrega_Unita_Misura_Cod, ")
            strSql.AppendLine("'' As Aggrega_Unita_Misura, ")
            strSql.AppendLine("l.CriteriParamQual As Criteri_Aggiuntivi, ")
            strSql.AppendLine("l.CriteriParamQual As Criteri_Aggiuntivi_JSON ")

            strSql.AppendLine(" from Lavorazioni_CriteriAggregazione l ")

            strSql.AppendLine(" JOIN Imprese i on i.piva = l.piva ")
            strSql.AppendLine(" JOIN Linee_Preparazioni lp on lp.preparazione_cod = l.Preparazione_Cod ")
            strSql.AppendLine(" LEFT JOIN Linee_ProduzionixPreparazioni lpxp on lpxp.preparazione_cod = l.Preparazione_Cod_Linea AND lpxp.Piva = l.Piva ")
            strSql.AppendLine(" LEFT JOIN Linee_Produzioni lprod on lprod.linea_cod = lpxp.linea_cod AND lprod.Piva = lpxp.Piva ")

            strSql.AppendLine(" where 1=1 ")

            If piva <> "" Then
                strSql.AppendLine(" AND l.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, objParametri:=objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY l.preparazione_cod ")
            End If

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function
End Class

Public Class Lavorazioni_CriteriAggrega_W : Inherits DataProvider

    Public Function Nuovo_CriteriAggregazione(ByVal piva As String, ByVal tipoLav As Integer, ByVal tipoLavLinea As Integer, ByVal aggregaFornitore As Integer,
                                              ByVal aggregaSpecie As Integer, ByVal aggregaVarieta As Integer, ByVal aggregaRegolamento As Integer,
                                              ByVal aggregaLotto As Integer, ByVal aggregaLottoUscita As Integer, ByVal aggregaProdotto As Integer, ByVal aggregaProdottoUscita As Integer,
                                              ByVal aggregaCella As Integer, ByVal aggregaCellaUscita As Integer, ByVal aggregaUnitaMisura As Integer,
                                              ByVal criteriAggiuntivi As String, ByRef ObjParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_W.Nuovo_CriteriAggregazione()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim Data_creazione = Date.Now

            Dim Data_modifica = Date.Now

            Dim username_creazione = ObjParametriServer.UsernameOperazione
            Dim username_modifica = ObjParametriServer.UsernameOperazione

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" INSERT INTO Lavorazioni_CriteriAggregazione ")
            strSql.AppendLine("             (Piva,    Preparazione_Cod,    Preparazione_Cod_Linea, ")
            strSql.AppendLine("              AggregaFornitore, AggregaSpecie, AggregaVarieta, ")
            strSql.AppendLine("              AggregaRegolamento,  AggregaLotto, LottoModificabileInUscita, ")
            strSql.AppendLine("              AggregaProdotto,  ProdottoModificabileInUscita, AggregaCella, CellaModificabileInUscita, AggregaUdm, CriteriParamQual, ")

            strSql.AppendLine("              Inviato,            DataInvio, ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica ")
            strSql.AppendLine("              ) ")

            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(tipoLav) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(tipoLavLinea) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaFornitore) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaSpecie) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaVarieta) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaRegolamento) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaLotto) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaLottoUscita) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaProdotto) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaProdottoUscita) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaCella) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaCellaUscita) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(aggregaUnitaMisura) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(criteriAggiuntivi) & "' ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica_CriteriAggregazione(ByVal piva As String, ByVal tipoLav As Integer, ByVal tipoLavLinea As Integer, ByVal aggregaFornitore As Integer,
                                              ByVal aggregaSpecie As Integer, ByVal aggregaVarieta As Integer, ByVal aggregaRegolamento As Integer,
                                              ByVal aggregaLotto As Integer, ByVal aggregaLottoUscita As Integer, ByVal aggregaProdotto As Integer,
                                              ByVal aggregaProdottoUscita As Integer, ByVal aggregaCella As Integer, ByVal aggregaCellaUscita As Integer,
                                              ByVal aggregaUnitaMisura As Integer, ByVal criteriAggiuntivi As String, ByRef ObjParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_W.Modifica_CriteriAggregazione()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim Data_creazione = Date.Now

            Dim Data_modifica = Date.Now

            Dim username_creazione = ObjParametriServer.UsernameOperazione
            Dim username_modifica = ObjParametriServer.UsernameOperazione

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Lavorazioni_CriteriAggregazione ")
            strSql.AppendLine(" SET AggregaFornitore = " & Agro_SQL_SaveNum(aggregaFornitore) & " ,")
            strSql.AppendLine(" AggregaSpecie = " & Agro_SQL_SaveNum(aggregaSpecie) & " ,")
            strSql.AppendLine(" aggregaVarieta =  " & Agro_SQL_SaveNum(aggregaVarieta) & " ,")
            strSql.AppendLine(" aggregaRegolamento =  " & Agro_SQL_SaveNum(aggregaRegolamento) & " ,")
            strSql.AppendLine(" aggregaLotto =  " & Agro_SQL_SaveNum(aggregaLotto) & " ,")
            strSql.AppendLine(" LottoModificabileInUscita =  " & Agro_SQL_SaveNum(aggregaLottoUscita) & " ,")
            strSql.AppendLine(" aggregaProdotto =  " & Agro_SQL_SaveNum(aggregaProdotto) & " ,")
            strSql.AppendLine(" ProdottoModificabileInUscita =  " & Agro_SQL_SaveNum(aggregaProdottoUscita) & " ,")
            strSql.AppendLine(" aggregaCella =  " & Agro_SQL_SaveNum(aggregaCella) & " ,")
            strSql.AppendLine(" CellaModificabileInUscita =  " & Agro_SQL_SaveNum(aggregaCellaUscita) & " ,")
            strSql.AppendLine(" AggregaUdm =  " & Agro_SQL_SaveNum(aggregaUnitaMisura) & " ,")
            strSql.AppendLine(" CriteriParamQual =  '" & Agro_SQL_SaveText(criteriAggiuntivi) & "' ,")
            strSql.AppendLine(" Data_modifica = " & Agro_SQL_SaveDate(Date.Now) & ",")
            strSql.AppendLine(" username_modifica = '" & Agro_SQL_SaveText(ObjParametriServer.UtenteUsername) & "' ")

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' AND ")
            strSql.AppendLine(" Preparazione_Cod = " & Agro_SQL_SaveNum(tipoLav) & " AND ")
            strSql.AppendLine(" Preparazione_Cod_Linea = " & Agro_SQL_SaveNum(tipoLavLinea) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function CancellaCriterioAggregazione(ByVal piva As String, ByVal codLavorazione As Integer, ByVal codLinea As Integer, ByVal xFiltroAggiuntivo As String,
                                                 ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_W.Nuovo_CriteriAggregazione()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim Data_creazione = Date.Now

            Dim Data_modifica = Date.Now

            Dim username_creazione = objParametriServer.UsernameOperazione
            Dim username_modifica = objParametriServer.UsernameOperazione

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" DELETE FROM Lavorazioni_CriteriAggregazione ")
            strSql.AppendLine(" WHERE ")

            strSql.AppendLine(" PIVA LIKE '" & Agro_SQL_SaveText(piva) & "' AND ")
            strSql.AppendLine(" Preparazione_Cod = " & Agro_SQL_SaveNum(codLavorazione) & " AND ")
            strSql.AppendLine(" Preparazione_Cod_Linea = " & Agro_SQL_SaveNum(codLinea) & "  ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class

Public Class CriteriAggregazioneLavorazioni

    Public Piva As String
    Public PreparazioneCod As Integer
    Public PreparazioneCodLinea As Integer
    Public AggregaFornitore As TipoAggregazione
    Public AggregaSpecie As TipoAggregazione
    Public AggregaVarieta As TipoAggregazione
    Public AggregaRegolamento As TipoAggregazione
    Public AggregaLotto As TipoAggregazione
    Public LottoModificabileInUscita As Boolean
    Public AggregaProdotto As TipoAggregazione
    Public ProdottoModificabileInUscita As Boolean
    Public AggregaUdm As TipoAggregazione
    Public AggregaCella As TipoAggregazione
    Public CellaModificabileInUscita As Boolean

    ''' <summary>
    ''' Elenco dei soli parametri qualitativi che prevedono qualche tipo di aggregazione
    ''' </summary>
    Public CriteriParamQual As List(Of CriteriParametriQualitativi)

End Class

Public Class CriteriParametriQualitativi

    ''' <summary>
    ''' Codice parametro qualitativo (Tabella_Key)
    ''' </summary>
    Public CodParam As String

    ''' <summary>
    ''' Tipo di aggregazione:
    ''' 1 = Solo uguali;
    ''' 10 = Media (solo per parametri numerici);
    ''' 11 = Minimo (solo per parametri numerici);
    ''' 12 = Massimo (solo per parametri numerici).
    ''' </summary>
    Public AggregaParam As TipoAggregazioneParam

    ''' <summary>
    ''' Indica se il parametro è modificabile in uscita lavorazione
    ''' </summary>
    Public ModifUscita As Boolean

End Class

Public Enum TipoAggregazione
    Tutti = 0
    SoloUguali = 1
End Enum

Public Enum TipoAggregazioneParam
    SoloUguali = 1
    Media = 10
    Minimo = 11
    Massimo = 12
End Enum
