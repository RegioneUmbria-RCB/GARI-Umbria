Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Listini_Prezzi_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiPrezzi(ByVal Piva As String,
                                ByVal Elem_Cod As Integer,
                                ByVal Pro_Cod As Integer,
                                ByVal Mat_Cod As Integer,
                                ByVal Cal_Cod As Integer,
                                ByVal Udm_Cod As Integer,
                                ByVal Data_Validita As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Listini_Prezzi_Dettagli_R.LeggiPrezzi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0


            StbSQL.AppendLine(" SELECT * ")
            StbSQL.AppendLine(" FROM Listini_Prezzi_Dettagli LPD ")

            StbSQL.AppendLine(" INNER JOIN Listini_Prezzi LP ON LPD.Piva_SuperUser= LP.Piva_SuperUser ")
            StbSQL.AppendLine(" AND LPD.Piva= LP.Piva AND LPD.Listino_Cod= LP.Listino_Cod ")


            StbSQL.AppendLine(" WHERE LPD.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Elem_Cod <> 0 Then
                StbSQL.AppendLine(" AND LPD.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Pro_Cod <> 0 Then
                StbSQL.AppendLine(" AND LPD.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
            End If

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND LPD.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            End If

            If Udm_Cod <> 0 Then
                StbSQL.AppendLine(" AND LPD.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ")
            End If

            If Cal_Cod <> 0 Then
                StbSQL.AppendLine(" AND LPD.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & " ")
            End If

            If Data_Validita <> "" AndAlso IsDate(Data_Validita) Then
                StbSQL.AppendLine(" AND LPD.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Validita) & " ")
                StbSQL.AppendLine(" AND LPD.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Validita) & " ")

                StbSQL.AppendLine(" AND LP.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Validita) & " ")
                StbSQL.AppendLine(" AND LP.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Validita) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '################################
            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StbSQL.AppendLine(" ORDER BY Descrizione ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
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
    Public Function Leggi_Listino_Prezzi(ByVal piva As String,
                                         ByVal KeyListinoProdotto As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim keys As String() = KeyListinoProdotto.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Elem_Cod As Integer = Integer.Parse(keys(2))
        Dim key_Pro_Cod As Integer = Integer.Parse(keys(3))
        Dim key_Mat_Cod As Integer = Integer.Parse(keys(4))

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_Dettagli_R.Leggi_Listino_Prezzi()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ListinoPrezzi =
               From d In GiasContext.Listini_Prezzi_Dettagli
               Join l In GiasContext.Listini_Prezzi On l.Piva_SuperUser Equals d.Piva_SuperUser And l.Piva Equals d.Piva And l.Listino_Cod Equals d.Listino_Cod
               Join u In GiasContext.UnitaMisura On u.UDM_COD Equals d.Udm_Cod
               Group Join pc In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 1 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                   On pc.Tabella_Par_Cod Equals d.Cal_Cod Into cg = Group From c In cg.DefaultIfEmpty()
               Group Join pq In GiasContext.OTabelle_Parametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                   On pq.Tabella_Par_Cod Equals d.Qualita_Cod Into qg = Group From q In qg.DefaultIfEmpty()
               Group Join rc In GiasContext.Rapporti_Contabili On rc.Cod_Rapporto Equals d.Cod_Rapporto Into _rc = Group From r In _rc.DefaultIfEmpty()
               Group Join ru In GiasContext.Risorse_Umane On ru.Cod_RisUm Equals d.Cod_RisUm Into _ru = Group From rr In _ru.DefaultIfEmpty()
               Group Join cn In GiasContext.Contatti On cn.Cod_Contatto Equals rr.Cod_Contatto Into _cn = Group From cc In _cn.DefaultIfEmpty()
               Where d.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                   d.Piva.Equals(piva) AndAlso
                   d.Listino_Cod = key_Listino_Cod AndAlso
                   d.Elem_Cod = key_Elem_Cod AndAlso
                   d.Pro_Cod = key_Pro_Cod AndAlso
                   d.Mat_Cod = key_Mat_Cod
               Order By r.Rapporto_Des, cc.Rag_Soc, d.Cal_Cod, d.Qualita_Cod, d.Udm_Cod, d.Validita_Inizio, d.Validita_Fine
               Select New With {
                   .KeyListinoPrezzi = d.ID,
                   .Validita_Inizio = If(d.Validita_Inizio = AGRODATAINIZIO, l.Validita_Inizio, d.Validita_Inizio),
                   .Validita_Fine = If(d.Validita_Fine = AGRODATAFINE, l.Validita_Fine, d.Validita_Fine),
                   d.Udm_Cod,
                   .Unita_Misura = u.UDM_DES,
                   d.Prezzo_Livello,
                   d.Qta,
                   d.Prezzo,
                   d.Sconto,
                   d.Sconto_Add1,
                   d.Sconto_Condizione1,
                   d.Sconto_Condizione_Valore1,
                   d.Sconto_Add2,
                   d.Sconto_Condizione2,
                   d.Sconto_Condizione_Valore2,
                   d.Sconto_Add3,
                   d.Sconto_Condizione3,
                   d.Sconto_Condizione_Valore3,
                   d.Cod_Iva,
                   d.Tipo_Iva_Det,
                   d.Cod_Conto_Det_Default,
                   d.Cal_Cod,
                   .Calibro_Des = c.Descrizione,
                   d.Qualita_Cod,
                   .Qualita_Des = q.Descrizione,
                   d.Cod_Rapporto,
                   r.Rapporto_Des,
                   d.Cod_RisUm,
                   cc.Rag_Soc
               }

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(ListinoPrezzi, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Listini_Prezzi_Dettaglio(ByVal piva As String,
                                                   ByVal ID As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Listini_Prezzi_Dettagli

        Dim TestataElem As Listini_Prezzi_Dettagli = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_Dettagli_R.Leggi_Listini_Prezzi_Dettaglio()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            TestataElem = (From d In GiasContext.Listini_Prezzi_Dettagli Where d.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso d.Piva.Equals(piva) AndAlso d.ID = ID Select d).FirstOrDefault()
        End Using

        Return TestataElem

    End Function

    '################################################################################
    Public Function LeggiListiniPrezziValidi(ByVal Piva As String,
                                             ByVal Tipo_Classe As enum_TipoListinoClasse,
                                             ByVal Listino_Cod As Integer,
                                             ByVal Elem_Cod As Integer,
                                             ByVal Pro_Cod As Integer,
                                             ByVal Mat_Cod As Integer,
                                             ByVal Cal_Cod As Integer,
                                             ByVal Qualita_Cod As Integer,
                                             ByVal Lotto_Cod1 As Integer,
                                             ByVal Lotto_Val1 As String,
                                             ByVal Lotto_Cod2 As Integer,
                                             ByVal Lotto_Val2 As String,
                                             ByVal Lav_Cod As Integer,
                                             ByVal ChkVettore As Integer,
                                             ByVal Cod_Rapporto As Integer,
                                             ByVal Cod_RisUm As Integer,
                                             ByVal ChkSemina As Integer,
                                             ByVal Livello_Prezzo As Integer,
                                             ByVal Udm_Cod As Integer,
                                             ByVal Udm_Cod_Extra As Integer,
                                             ByVal ChkApplicabilita As Integer,
                                             ByVal Data As Date,
                                             ByVal strFiltro As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable


        Const nomeRoutine = "AgronicaCoreContabDAL.Listini_Prezzi_Dettagli_R.LeggiListiniPrezziValidi()"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim strSort_Cal_Cod As String
        Dim strSort_Qualita_Cod As String
        Dim strSort_Lotto_Cod1 As String
        Dim strSort_Lotto_Cod2 As String
        Dim strSort_Lav_Cod As String
        Dim strSort_Cod_Rapporto As String
        Dim strSort_Cod_RisUm As String
        'Dim strSort_Mezzo As String
        Dim strSort_Udm_Cod As String

        Try

            'Mat_Cod = 590
            'Livello_Prezzo = 0
            'Piva = "00040710295"
            'Tipo_Classe = 2
            'Elem_Cod = 210
            'Cod_RisUm = 10043

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT Listini_Prezzi.Listino_Des, Listini_Prezzi_Dettagli.*, ")
            stbSql.AppendLine(" IsNull(Rapporti_Contabili.Rapporto_Des, '') as Rapporto_Des, IsNull(Contatti.Rag_Soc, '') as Rag_Soc, IsNull(IVA_Aliquote.Aliquota, '') as Aliquota, IsNull(UnitaMisura.Udm_Sim, '') as Udm_Des, ")
            stbSql.AppendLine(" IsNull(Contatti2.Provvigione, 0) as Provvigione,  IsNull(Contatti2.provvigione_capoarea, 0) as Provvigione_Capoarea ")

            stbSql.AppendLine(" FROM   Listini_Prezzi, Listini_Classi_Prezzi, Listini_PrezzixRisorse, Listini_Prezzi_Dettagli ")

            stbSql.AppendLine("LEFT OUTER JOIN Rapporti_Contabili On Rapporti_Contabili.Cod_Rapporto = Listini_Prezzi_Dettagli.Cod_Rapporto ")
            stbSql.AppendLine("LEFT OUTER JOIN Risorse_Umane On Risorse_Umane.Cod_Risum = Listini_Prezzi_Dettagli.Cod_Risum ")
            stbSql.AppendLine("LEFT OUTER JOIN Contatti On Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
            stbSql.AppendLine("LEFT OUTER JOIN IVA_Aliquote On IVA_Aliquote.Codice = Listini_Prezzi_Dettagli.Cod_Iva ")
            stbSql.AppendLine("LEFT OUTER JOIN UnitaMisura On UnitaMisura.Udm_Cod = Listini_Prezzi_Dettagli.Udm_Cod  ")
            stbSql.AppendLine("LEFT OUTER JOIN Risorse_Umane as Risorse_Umane2 On (Risorse_Umane2.Cod_Risum = " & Agro_SQL_SaveNum(Cod_RisUm) & ")")
            stbSql.AppendLine("LEFT OUTER JOIN Contatti as Contatti2 On Contatti2.Cod_Contatto = Risorse_Umane2.Cod_Contatto ")


            stbSql.AppendLine(" WHERE  Listini_Prezzi_Dettagli.Piva_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Data) & " ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            stbSql.AppendLine(" AND    Listini_Prezzi.Validita_inizio <= " & Agro_SQL_SaveDate(Data) & " ")
            stbSql.AppendLine(" AND    Listini_Prezzi.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            stbSql.AppendLine(" AND    Listini_Prezzi.Piva_SuperUser = Listini_Prezzi_Dettagli.Piva_SuperUser ")
            stbSql.AppendLine(" AND    Listini_Prezzi.Piva = Listini_Prezzi_Dettagli.Piva ")
            stbSql.AppendLine(" AND    Listini_Prezzi.Listino_Cod = Listini_Prezzi_Dettagli.Listino_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi.Listino_Classe_Cod = Listini_Classi_Prezzi.Listino_Classe_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Piva_SuperUser = Listini_PrezzixRisorse.Piva_SuperUser    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Piva = Listini_PrezzixRisorse.Piva    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Listino_Cod = Listini_PrezzixRisorse.Listino_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Elem_Cod = Listini_PrezzixRisorse.Elem_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Pro_Cod = Listini_PrezzixRisorse.Pro_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Mat_Cod = Listini_PrezzixRisorse.Mat_Cod    ")


            'Considero la tipologia di listino (acquisto/vendita)
            stbSql.AppendLine(" AND Listini_Classi_Prezzi.Tipo_Classe = " & Agro_SQL_SaveNum(CInt(Tipo_Classe)) & "   ")

            'Escludo i Prezzi Nulli
            stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Prezzo <> 0 ")

            '------------------------------------------------------------------------------------------------------------------------------
            'CAMPI OPZIONALI

            If Elem_Cod <> -1 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Listino_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Listino_Cod = " & Agro_SQL_SaveNum(Listino_Cod) & "   ")
            End If

            If Cal_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Cal_Cod In (" & Agro_SQL_Save_Clausola_IN(Cal_Cod) & ",0)   ")
            End If

            If Qualita_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Qualita_Cod In (" & Agro_SQL_Save_Clausola_IN(Qualita_Cod) & ",0)   ")
            End If

            If Lotto_Cod1 <> 0 Then
                stbSql.AppendLine(" AND (Listini_Prezzi_Dettagli.Lotto_Cod1 In (" & Agro_SQL_Save_Clausola_IN(Lotto_Cod1) & " ,0) And Listini_Prezzi_Dettagli.Lotto_Val1 In (" & Agro_SQL_Save_Clausola_IN("'" & Trim(Lotto_Val1) & "'", True) & ", '') Or Listini_Prezzi_Dettagli.Lotto_Cod1 In (" & Agro_SQL_Save_Clausola_IN(Lotto_Cod2) & ",0) And Listini_Prezzi_Dettagli.Lotto_Val1 In (" & Agro_SQL_Save_Clausola_IN("'" & Trim(Lotto_Val2) & "'", True) & ", '') )")
            End If

            If Lotto_Cod2 <> 0 Then
                stbSql.AppendLine(" AND (Listini_Prezzi_Dettagli.Lotto_Cod2 In (" & Agro_SQL_Save_Clausola_IN(Lotto_Cod1) & " ,0) And Listini_Prezzi_Dettagli.Lotto_Val2 In (" & Agro_SQL_Save_Clausola_IN("'" & Trim(Lotto_Val1) & "'", True) & ", '') Or Listini_Prezzi_Dettagli.Lotto_Cod2 In (" & Agro_SQL_Save_Clausola_IN(Lotto_Cod2) & ",0) And Listini_Prezzi_Dettagli.Lotto_Val2 In (" & Agro_SQL_Save_Clausola_IN("'" & Trim(Lotto_Val2) & "'", True) & ", '') )")
            End If

            If Lav_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Lav_Cod In (" & Agro_SQL_Save_Clausola_IN(Lav_Cod, False) & ",0)   ")
            End If

            If ChkVettore <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.ChkVettore = " & Agro_SQL_SaveNum(ChkVettore) & "   ")
            End If

            If Cod_Rapporto <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Cod_Rapporto In (" & Agro_SQL_Save_Clausola_IN(Cod_Rapporto, False) & ",0)   ")
            End If

            If Cod_RisUm <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Cod_RisUm In (" & Agro_SQL_Save_Clausola_IN(Cod_RisUm, False) & ",0)   ")
            End If

            If ChkSemina <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.ChkSemina = " & Agro_SQL_SaveNum(ChkSemina) & "   ")
            End If


            Select Case Livello_Prezzo
                Case 0

                    If Udm_Cod <> 0 Then
                        If Udm_Cod_Extra <> 0 Then
                            stbSql.AppendLine(" AND (Listini_Prezzi_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " Or Listini_Prezzi_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod_Extra) & ") ")
                        Else
                            stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ")
                        End If

                    End If

                Case Else

                    stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Prezzo_Livello = " & Agro_SQL_SaveNum(Livello_Prezzo) & "   ")

            End Select


            If ChkApplicabilita <> -1 Then

                Select Case ChkApplicabilita

                    Case 0 'Tutti i listini applicabili

                        stbSql.AppendLine(" AND Listini_Prezzi.ChkApplicabilita > 0 ")

                    Case Else

                        stbSql.AppendLine(" AND Listini_Prezzi.ChkApplicabilita = " & Agro_SQL_SaveNum(ChkApplicabilita) & "   ")

                End Select

            End If

            If Trim(strFiltro) <> "" Then
                stbSql.AppendLine(" AND (" & strFiltro & ")")
            End If


            'Impostazione Criteri di Ordinamento
            strSort_Cal_Cod = IIf(Cal_Cod <> 0, "Desc", "Asc")
            strSort_Qualita_Cod = IIf(Qualita_Cod <> 0, "Desc", "Asc")
            strSort_Lotto_Cod1 = "Desc"
            strSort_Lotto_Cod2 = "Desc"
            strSort_Lav_Cod = If(Lav_Cod <> 0, "Desc", "Asc")
            strSort_Cod_Rapporto = If(Cod_Rapporto > 0, "Desc", "Asc")  'nota: I codici rapporto di default sono in negativo e l'ordinamento non funzionerebbe CON <>
            strSort_Cod_RisUm = If(Cod_RisUm <> 0, "Desc", "Asc")
            strSort_Udm_Cod = If(Udm_Cod <> 0 OrElse Udm_Cod_Extra <> 0, "Desc", "Asc")

            stbSql.AppendLine(" ORDER BY Listini_Prezzi.Piva_SuperUser Asc, ")
            stbSql.AppendLine("          Listini_Prezzi.Piva Asc, ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Cal_Cod " & strSort_Cal_Cod & ", ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Qualita_Cod " & strSort_Qualita_Cod & ", ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Lotto_Cod1 " & strSort_Lotto_Cod1 & ", ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Lotto_Val1 " & strSort_Lotto_Cod1 & ", ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Lotto_Cod2 " & strSort_Lotto_Cod2 & ", ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Lotto_Val2 " & strSort_Lotto_Cod2 & ", ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Lav_Cod " & strSort_Lav_Cod & ", ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Cod_Rapporto " & strSort_Cod_Rapporto & ", ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Cod_RisUm " & strSort_Cod_RisUm & ", ")
            stbSql.AppendLine("          Listini_Prezzi_Dettagli.Udm_Cod " & strSort_Udm_Cod & ",  ")
            stbSql.AppendLine("          Listini_Prezzi.Listino_Des Asc ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '################################################################################
    Public Function GetListiniConSpiegazione(ByRef dt As DataTable,
                                             ByVal data As String,
                                             ByVal lavCod As Integer,
                                             ByVal piva As String,
                                             ByVal tipoClasse As enum_TipoListinoClasse,
                                             ByVal elem_cod As Integer, ByVal pro_cod As Integer, ByVal mat_cod As Integer,
                                             ByVal cal_cod As Integer, ByVal qualita_cod As Integer,
                                             ByVal lotto_cod1 As Integer, ByVal lotto_val1 As String,
                                             ByVal lotto_cod2 As Integer, ByVal lotto_val2 As String,
                                             ByVal chkVettore As Integer,
                                             ByVal cod_rapporto As Integer, ByVal cod_risum As Integer,
                                             ByVal chkSemina As Integer, ByVal livello_prezzo As Integer,
                                             ByVal udm_cod As Integer, ByVal udm_cod_extra As Integer,
                                             ByRef objParametriServer As AgronicaCoreParametri,
                                             ByVal listino_cod_default As Integer,
                                             ByVal chkContatto As Boolean,
                                             Optional ByVal rimuoviCampiInutili As Boolean = False
                                             ) As Boolean

        Const nomeRoutine = "GetListiniConSpiegazione"
        Dim bOk As Boolean = False

        Try

            Dim strFiltro As String
            Dim strListinoInfo As String
            Dim drListinoDefault As EnumerableRowCollection(Of DataRow)

            'Controllo coerenza data
            If Not IsDate(data) Then
                data = CDate(Now)
            End If

            Select Case lavCod

                Case LAVCOD_VENDITA 'Corrispettivi

                    strFiltro = "ChkApplicabilita = 2"

                Case Else

                    strFiltro = "" 'ChkApplicabilita <> 2"

            End Select


            'Lettura dei prezzi validi
            dt = LeggiListiniPrezziValidi(piva, tipoClasse, 0, elem_cod, pro_cod, mat_cod, cal_cod, qualita_cod,
                                          lotto_cod1, lotto_val1, lotto_cod2, lotto_val2,
                                          lavCod, chkVettore, cod_rapporto, cod_risum, chkSemina,
                                          livello_prezzo, udm_cod, udm_cod_extra, 0,
                                          data, strFiltro, objParametriServer)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                dt.Columns.Add(New DataColumn("Nota_Listino", GetType(String)))
                dt.Columns.Add(New DataColumn("ChkAutomatico", GetType(Integer)))

                For Each dr As DataRow In dt.Rows

                    '----------------------------------------------------------------------------------------------------------------------
                    'Costruzione Nota
                    '----------------------------------------------------------------------------------------------------------------------

                    strListinoInfo = ""

                    'Udm
                    If dr.Item("Udm_Cod") <> 0 Then
                        strListinoInfo &= " " & dr.Item("Prezzo") & " €/" & dr.Item("Udm_Des") & "."
                    End If

                    'Contatto
                    If dr.Item("Cod_Risum") <> 0 Then
                        strListinoInfo &= " Applicato al contatto " & dr.Item("Rag_Soc") & "."
                    ElseIf dr.Item("Cod_Rapporto") <> 0 Then
                        strListinoInfo &= " Applicato al rapporto contabile " & dr.Item("Rapporto_Des") & "."
                    Else
                        strListinoInfo &= " Prezzo per generico contatto."
                    End If


                    If CInt(dr.Item("Cod_Iva")) > 0 Then

                        'Tipo Iva                   
                        Select Case CInt(dr.Item("Tipo_Iva_Det"))

                            Case enum_TipoIVAListino.IVA_Sconto_Inclusi, enum_TipoIVAListino.IVA_Inclusa

                                If Math.Abs(dr.Item("Sconto")) <> 0 Then
                                    strListinoInfo &= " Incluse iva del " & dr.Item("Aliquota") & " e sconto max del " & Math.Abs(dr.Item("Sconto")) & " %."
                                Else
                                    strListinoInfo &= " Inclusa iva del " & dr.Item("Aliquota") & "."
                                End If

                            Case enum_TipoIVAListino.IVA_Esclusa

                                If Math.Abs(dr.Item("Sconto")) <> 0 Then
                                    strListinoInfo &= " Iva esclusa del " & dr.Item("Aliquota") & " e sconto max del " & Math.Abs(dr.Item("Sconto")) & " %."
                                Else
                                    strListinoInfo &= " Iva esclusa del " & dr.Item("Aliquota") & "."
                                End If

                        End Select

                    End If

                    dr.Item("Nota_Listino") = strListinoInfo
                    dr.Item("ChkAutomatico") = 0 'Inizializzazione

                    '----------------------------------------------------------------------------------------------------------------------
                    'Costruzione Provvigione
                    '----------------------------------------------------------------------------------------------------------------------
                    Select Case dr.Item("Tipo_Provvigione_Det")

                        Case 0 'Anagrafica Contatto

                        Case 2  'Listino
                            dr.Item("Provvigione") = dr.Item("Provvigione_Listino")
                    End Select

                Next


                Select Case lavCod

                    Case LAVCOD_VENDITA 'Corrispettivi

                        dt.Rows(0).Item("ChkAutomatico") = 1
                        bOk = True

                    Case Else

                        drListinoDefault = dt.AsEnumerable().Where(Function(x) x.Item("Listino_Cod") = listino_cod_default)

                        If drListinoDefault Is Nothing OrElse drListinoDefault.Count() = 0 Then
                            'Nessuno Valido

                            'Controllo Ricerca anche tra i listini non associati in anagrafica
                            If chkContatto Then

                                bOk = True

                                If dt.Rows.Count = 1 Then
                                    dt.Rows(0).Item("ChkAutomatico") = 1
                                Else
                                    'Tutti i Listini senza preselezione
                                End If

                            End If

                        Else

                            'Scelgo quello più giusto
                            drListinoDefault(0).Item("ChkAutomatico") = 1
                            bOk = True

                        End If

                End Select

            End If

            'Rimozioni Campi Inutili   
            If bOk = True AndAlso rimuoviCampiInutili = True Then

                Dim col As Integer = 0
                Do While col < dt.Columns.Count

                    Select Case UCase(dt.Columns(col).ColumnName)

                        Case "LISTINO_DES", "LISTINO_COD", "CHKAUTOMATICO", "NOTA_LISTINO", "PREZZO",
                            "UDM_COD", "PREZZO_LIVELLO", "PROVVIGIONE", "SCONTO",
                            "SCONTO_ADD1", "SCONTO_ADD2", "SCONTO_ADD3",
                            "SCONTO_CONDIZIONE1", "SCONTO_CONDIZIONE2", "SCONTO_CONDIZIONE3",
                            "SCONTO_CONDIZIONE_VALORE1", "SCONTO_CONDIZIONE_VALORE2", "SCONTO_CONDIZIONE_VALORE3"

                            col += 1

                        Case Else

                            dt.Columns.Remove(dt.Columns(col).ColumnName)

                    End Select

                Loop

            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return bOk

    End Function

    '################################################################################
    Public Function LeggiListinoContatto(ByVal Piva As String,
                                        ByVal Tipo_Classe As enum_TipoListinoClasse,
                                        ByVal Cod_RisUm As Integer,
                                        ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Listini_Prezzi_Dettagli_R.LeggiListinoContatto()"

        Dim listinoAssociato As Integer = 0
        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.AppendLine(" SELECT ru.Cod_RisUm, ru.Cod_Rapporto, c.Nome, c.Cognome, c.Rag_Soc, c.Piva, c.Sa_Cod, cc.Id_cod, cc.Val_cod ")
            stbSql.AppendLine(" FROM Risorse_Umane ru ")
            stbSql.AppendLine(" INNER JOIN Contatti c on c.piva=ru.piva and c.cod_contatto=ru.cod_contatto ")
            stbSql.AppendLine(" LEFT JOIN Contatti_Codici cc on cc.piva=c.piva and cc.cod_contatto=c.cod_contatto ")

            stbSql.AppendLine(" WHERE ru.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            stbSql.AppendLine(" AND ru.Cod_RisUm =" & Agro_SQL_SaveNum(Cod_RisUm) & " ")

            If Tipo_Classe = enum_TipoListinoClasse.ListinoAcquisto Then
                stbSql.AppendLine(" AND cc.Id_Cod =" & Agro_SQL_SaveNum(enum_CodiciAnagrafe.ListinoPrezziAcquistoDefault) & " ")
            ElseIf Tipo_Classe = enum_TipoListinoClasse.ListinoVendita Then
                stbSql.AppendLine(" AND cc.Id_Cod =" & Agro_SQL_SaveNum(enum_CodiciAnagrafe.ListinoPrezziVenditaDefault) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            listinoAssociato = If(dt.Rows.Count > 0, CInt(dt.Rows(0).Item("Val_Cod")), 0)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return listinoAssociato

    End Function


    Public Function LeggiModuliAttivi(ByVal Piva As String, ByRef objParametri As AgronicaCoreParametri) As String

        'Controllo se F&F / Cantine / Tabacco / Zoo
        Dim elencoModuli = ""
        Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
        Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(Piva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)

        If dtAnagrafeLog IsNot Nothing AndAlso dtAnagrafeLog.Rows.Count > 0 Then
            For Each rAnagrafeLog In dtAnagrafeLog.Rows
                If elencoModuli <> "" Then
                    elencoModuli &= "|"
                End If
                elencoModuli &= CStr(rAnagrafeLog.Item("Modulo_Generazione"))
            Next
        End If

        Return elencoModuli

    End Function

    '################################################################################
    Public Function LeggiPrezziListini(ByVal Piva As String,
                                             ByVal Tipo_Classe As enum_TipoListinoClasse,
                                             ByVal Listino_Cod As Integer,
                                             ByVal Data As Date,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Cod_Rapporto As Integer,
                                             ByVal Cod_Contatto As String,
                                             ByVal Cod_RisUm As Integer,
                                             ByVal Elem_Cod As Integer,
                                             ByVal UdM_Cod As Integer,
                                             ByVal Pro_Cod As Integer,
                                             ByVal Mat_Cod As Integer,
                                             ByVal strFiltro As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable


        Const nomeRoutine = "AgronicaCoreContabDAL.Listini_Prezzi_Dettagli_R.LeggiPrezziListini()"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            ' controllo prezzo livello se modulo FF/Zoo/Tabacco e trasformati vegetali/animali
            'Dim checkPrezzoLivello As Boolean = False
            'If Elem_Cod = TRASFORMATI_VEGETALI OrElse Elem_Cod = TRASFORMATI_ANIMALI Then
            '    Dim moduli = LeggiModuliAttivi(Piva, objParametri).Split("|")
            '    checkPrezzoLivello = moduli.Contains(enum_Omni_Modulo_Generazione.FreshFood)
            '    checkPrezzoLivello = checkPrezzoLivello OrElse moduli.Contains(enum_Omni_Modulo_Generazione.Tabacco)
            '    checkPrezzoLivello = checkPrezzoLivello OrElse moduli.Contains(enum_Omni_Modulo_Generazione.Zoo)
            'End If

            stbSql.AppendLine(" SELECT Listini_Classi_Prezzi.Listino_Classe_Des, Listini_Classi_Prezzi.Tipo_Classe, ")
            stbSql.AppendLine(" Listini_Prezzi.Listino_Cod_Des, Listini_Prezzi.Listino_Des, Listini_Prezzi.ChkApplicabilita, ")
            stbSql.AppendLine(" Listini_Prezzi.Validita_Inizio as Validita_Inizio_Listino, Listini_Prezzi.Validita_Fine as Validita_Fine_Listino, Listini_Prezzi_Dettagli.*, ")
            stbSql.AppendLine(" IsNull(Centri_Aziendali.sa_nome, '') as Sa_Nome, IsNull(Rapporti_Contabili.Rapporto_Des, '') as Rapporto_Des, ")
            stbSql.AppendLine(" IsNull(Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome, '') as Rag_Soc, ")
            stbSql.AppendLine(" IsNull(IVA_Aliquote.Aliquota, '') as Aliquota, IsNull(UnitaMisura.Udm_Sim, '') as Udm_Des, ")
            stbSql.AppendLine(" CategorieMagazzino.NomeComune as Categoria, Materie_Prime.Mat_Des As Prodotto, Materie_Prime.Cod_Articolo, OCalibro.Descrizione as Calibro, OQualita.Descrizione as Qualita, ")
            'stbSql.AppendLine(" CASE WHEN Listini_Prezzi_Dettagli.Elem_Cod = 191 THEN Formulati.Fr_Des ELSE Materie_Prime.Mat_Des END As Prodotto, ")
            stbSql.AppendLine(" (CASE WHEN IsNull(Listini_PrezzixContatti.Sa_Cod, 0) <> 0 THEN 1 ELSE 0 END + ")
            stbSql.AppendLine(" CASE WHEN IsNull(Listini_PrezzixContatti.Cod_Rapporto, 0) <> 0 THEN 2 ELSE 0 END + ")
            stbSql.AppendLine(" CASE WHEN IsNull(Listini_PrezzixContatti.Cod_Contatto, '') <> '' THEN 4 ELSE 0 END) AS Livello ")
            stbSql.AppendLine(" FROM   Listini_Prezzi, Listini_Classi_Prezzi, Listini_PrezzixRisorse, Listini_Prezzi_Dettagli ")

            stbSql.AppendLine("LEFT JOIN Listini_PrezzixContatti On Listini_PrezzixContatti.Piva_SuperUser = Listini_Prezzi_Dettagli.Piva_SuperUser And Listini_PrezzixContatti.Piva = Listini_Prezzi_Dettagli.Piva And Listini_PrezzixContatti.Listino_Cod = Listini_Prezzi_Dettagli.Listino_Cod ")
            stbSql.AppendLine("LEFT JOIN Centri_Aziendali On Centri_Aziendali.Piva = Listini_PrezzixContatti.Piva And Centri_Aziendali.sa_cod = Listini_PrezzixContatti.Sa_Cod ")
            stbSql.AppendLine("LEFT JOIN Rapporti_Contabili On Rapporti_Contabili.Cod_Rapporto = Listini_PrezzixContatti.Cod_Rapporto ")
            stbSql.AppendLine("LEFT JOIN Contatti On Contatti.Cod_Contatto = Listini_PrezzixContatti.Cod_Contatto And (Contatti.Sa_Cod = -1 OR Contatti.Piva = Listini_PrezzixContatti.Piva) ")
            'stbSql.AppendLine("LEFT JOIN Rapporti_Contabili On Rapporti_Contabili.Cod_Rapporto = Listini_Prezzi_Dettagli.Cod_Rapporto ")
            'stbSql.AppendLine("LEFT JOIN Risorse_Umane On Risorse_Umane.Cod_Risum = Listini_Prezzi_Dettagli.Cod_Risum ")
            'stbSql.AppendLine("LEFT JOIN Contatti On Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
            stbSql.AppendLine("LEFT JOIN IVA_Aliquote On IVA_Aliquote.Codice = Listini_Prezzi_Dettagli.Cod_Iva ")
            stbSql.AppendLine("LEFT JOIN UnitaMisura On UnitaMisura.Udm_Cod = Listini_Prezzi_Dettagli.Udm_Cod  ")
            stbSql.AppendLine("LEFT JOIN CategorieMagazzino On CategorieMagazzino.Elem_Cod = Listini_Prezzi_Dettagli.Elem_Cod ")
            stbSql.AppendLine("LEFT JOIN Materie_Prime On Materie_Prime.Elem_Cod = Listini_Prezzi_Dettagli.Elem_Cod And Materie_Prime.Mat_Cod = Listini_Prezzi_Dettagli.Mat_Cod ")
            'stbSql.AppendLine("LEFT JOIN Formulati On Listini_Prezzi_Dettagli.Elem_Cod = 191 And Formulati.Fr_Cod = Listini_Prezzi_Dettagli.Pro_Cod ")
            stbSql.AppendLine("LEFT JOIN (SELECT * FROM OTabelle_Parametri WHERE Tabella_Cod = 1 And Modulo_Generazione = 2) OCalibro On OCalibro.Tabella_Par_Cod = Listini_Prezzi_Dettagli.Cal_Cod And (OCalibro.Piva = Listini_Prezzi_Dettagli.Piva Or OCalibro.Piva = 'AAAAAAAAAAA') ")
            stbSql.AppendLine("LEFT JOIN (SELECT * FROM OTabelle_Parametri WHERE Tabella_Cod = 3 And Modulo_Generazione = 2) OQualita On OQualita.Tabella_Par_Cod = Listini_Prezzi_Dettagli.Qualita_Cod And (OQualita.Piva = Listini_Prezzi_Dettagli.Piva Or OQualita.Piva = 'AAAAAAAAAAA') ")

            stbSql.AppendLine(" WHERE  Listini_Prezzi_Dettagli.Piva_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            If Data <> AGRODATAINIZIO Then
                stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Data) & " ")
                stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
                stbSql.AppendLine(" AND    Listini_Prezzi.Validita_inizio <= " & Agro_SQL_SaveDate(Data) & " ")
                stbSql.AppendLine(" AND    Listini_Prezzi.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            End If

            stbSql.AppendLine(" AND    Listini_Prezzi.Piva_SuperUser = Listini_Prezzi_Dettagli.Piva_SuperUser ")
            stbSql.AppendLine(" AND    Listini_Prezzi.Piva = Listini_Prezzi_Dettagli.Piva ")
            stbSql.AppendLine(" AND    Listini_Prezzi.Listino_Cod = Listini_Prezzi_Dettagli.Listino_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi.Listino_Classe_Cod = Listini_Classi_Prezzi.Listino_Classe_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Piva_SuperUser = Listini_PrezzixRisorse.Piva_SuperUser    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Piva = Listini_PrezzixRisorse.Piva    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Listino_Cod = Listini_PrezzixRisorse.Listino_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Elem_Cod = Listini_PrezzixRisorse.Elem_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Pro_Cod = Listini_PrezzixRisorse.Pro_Cod    ")
            stbSql.AppendLine(" AND    Listini_Prezzi_Dettagli.Mat_Cod = Listini_PrezzixRisorse.Mat_Cod    ")

            'Considero la tipologia di listino (acquisto/vendita)
            stbSql.AppendLine(" AND Listini_Classi_Prezzi.Tipo_Classe = " & Agro_SQL_SaveNum(CInt(Tipo_Classe)) & "   ")

            'Escludo i Prezzi Nulli
            stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Prezzo <> 0 ")

            'Solo Listini applicabili
            'stbSql.AppendLine(" AND Listini_Prezzi.ChkApplicabilita > 0 ")

            '------------------------------------------------------------------------------------------------------------------------------
            'CAMPI OPZIONALI

            If Elem_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If UdM_Cod <> 0 Then
                Dim filtro_udm As String = "Listini_Prezzi_Dettagli.UdM_Cod = " & Agro_SQL_SaveNum(UdM_Cod)
                'If checkPrezzoLivello AndAlso UdM_Cod = 38 Then
                '    filtro_udm = "(" & filtro_udm & " OR Listini_Prezzi_Dettagli.Prezzo_Livello = 5)"
                'End If
                stbSql.AppendLine(" AND " & filtro_udm & "   ")
            End If

            If Listino_Cod <> 0 Then
                stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Listino_Cod = " & Agro_SQL_SaveNum(Listino_Cod) & "   ")
            End If

            If Sa_Cod <> 0 Then
                stbSql.AppendLine(" AND (ISNULL(Listini_PrezzixContatti.Sa_Cod,0) = 0 OR Listini_PrezzixContatti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")   ")
            End If

            If Cod_Rapporto <> 0 Then
                stbSql.AppendLine(" AND (ISNULL(Listini_PrezzixContatti.Cod_Rapporto,0) = 0 OR Listini_PrezzixContatti.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & ")   ")
                'stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Cod_Rapporto In (" & Agro_SQL_SaveNum(Cod_Rapporto) & ",0)   ")
            End If

            If Cod_Contatto <> "" Then
                stbSql.AppendLine(" AND (ISNULL(Listini_PrezzixContatti.Cod_Contatto,'') = '' OR Listini_PrezzixContatti.Cod_Contatto ='" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "')   ")
            End If

            If Cod_RisUm <> 0 Then
                'stbSql.AppendLine(" AND Listini_Prezzi_Dettagli.Cod_RisUm In (" & Agro_SQL_SaveNum(Cod_RisUm) & ",0)   ")
            End If

            If Trim(strFiltro) <> "" Then
                stbSql.AppendLine(" AND (" & strFiltro & ")")
            End If

            stbSql.AppendLine(" ORDER BY Prodotto, Udm_Des, Livello DESC")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class

Public Class Listini_Prezzi_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "Costruttori"

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As Globalization.CultureInfo
    Public Shadows Property Provider() As Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property


    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property

    Public Function Aggiorna_Listino_Prezzi(
            ByVal piva As String,
            ByVal KeyListinoProdotto As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim keys As String() = KeyListinoProdotto.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Elem_Cod As Integer = Integer.Parse(keys(2))
        Dim key_Pro_Cod As Integer = Integer.Parse(keys(3))
        Dim key_Mat_Cod As Integer = Integer.Parse(keys(4))

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_Dettagli_W.Aggiorna_Listino_Prezzi()"

        Try

            Dim leggi As New Listini_Prezzi_Dettagli_R
            Dim listino_prezzi As New Listini_Prezzi_Dettagli

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim EFArrayRigheProdottiToDelete As New ArrayList

            For Each obj As JObject In righeInseriteArray

                Dim Prezzo_Livello As Integer = If(obj("Prezzo_Livello"), 0)
                Dim Udm_Cod As Integer = If(Prezzo_Livello = 5, 38, If(obj("Udm_Cod"), 2))

                listino_prezzi = New Listini_Prezzi_Dettagli
                listino_prezzi.Piva_SuperUser = Piva_SuperUser
                listino_prezzi.Piva = piva
                listino_prezzi.Listino_Cod = key_Listino_Cod
                listino_prezzi.Elem_Cod = key_Elem_Cod
                listino_prezzi.Pro_Cod = key_Pro_Cod
                listino_prezzi.Mat_Cod = key_Mat_Cod
                listino_prezzi.Mezzo = If(obj("Mezzo"), -1)
                listino_prezzi.Udm_Cod = Udm_Cod
                listino_prezzi.Qta = If(obj("Qta"), 1)
                listino_prezzi.Prezzo = If(obj("Prezzo"), 0)
                listino_prezzi.Sconto_Add1 = If(obj("Sconto_Add1"), 0)
                listino_prezzi.Sconto_Condizione_Valore1 = If(obj("Sconto_Condizione_Valore1"), 0)
                listino_prezzi.Sconto_Condizione1 = If(listino_prezzi.Sconto_Condizione_Valore1 > 0, 1, 0)
                listino_prezzi.Sconto_Add2 = If(obj("Sconto_Add2"), 0)
                listino_prezzi.Sconto_Condizione_Valore2 = If(obj("Sconto_Condizione_Valore2"), 0)
                listino_prezzi.Sconto_Condizione2 = If(listino_prezzi.Sconto_Condizione_Valore2 > 0, 1, 0)
                listino_prezzi.Sconto_Add3 = If(obj("Sconto_Add3"), 0)
                listino_prezzi.Sconto_Condizione_Valore3 = If(obj("Sconto_Condizione_Valore3"), 0)
                listino_prezzi.Sconto_Condizione3 = If(listino_prezzi.Sconto_Condizione_Valore3 > 0, 1, 0)
                listino_prezzi.Sconto = Math.Round((1 - (1 - listino_prezzi.Sconto_Add1 / 100) * (1 - listino_prezzi.Sconto_Add2 / 100) * (1 - listino_prezzi.Sconto_Add3 / 100)) * -100, 2)
                listino_prezzi.Cod_Iva = If(obj("Cod_Iva"), -1)
                listino_prezzi.Cod_Rapporto = If(obj("Cod_Rapporto"), 0)
                listino_prezzi.Cod_RisUm = If(obj("Cod_RisUm"), 0)
                listino_prezzi.Cal_Cod = If(obj("Cal_Cod"), 0)
                listino_prezzi.Qualita_Cod = If(obj("Qualita_Cod"), 0)
                listino_prezzi.Prezzo_Livello = Prezzo_Livello
                listino_prezzi.Lotto_Cod1 = If(obj("Lotto_Cod1"), 0)
                listino_prezzi.Lotto_Val1 = If(obj("Lotto_Val1"), "")
                listino_prezzi.Lotto_Cod2 = If(obj("Lotto_Cod2"), 0)
                listino_prezzi.Lotto_Val2 = If(obj("Lotto_Val2"), "")
                listino_prezzi.Cod_Conto_Det_Default = If(obj("Cod_Conto_Det_Default"), -1)

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    listino_prezzi.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    listino_prezzi.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                End If
                listino_prezzi.Data_Creazione = Date.Now
                listino_prezzi.Username_Creazione = objParametri.UsernameOperazione
                listino_prezzi.Data_Modifica = Date.Now
                listino_prezzi.Username_Modifica = objParametri.UsernameOperazione
                listino_prezzi.inviato = 0

                ' Controllo che non ci siano periodi sovrapposti già registrati
                'MessaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, True, False, True)

                EFArrayToInsert.Add(listino_prezzi)

            Next

            For Each obj As JObject In righeModificateArray

                listino_prezzi = leggi.Leggi_Listini_Prezzi_Dettaglio(piva, obj("KeyListinoPrezzi"), objParametri)

                If listino_prezzi Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare non trovata"
                Else

                    Dim Prezzo_Livello As Integer = If(obj("Prezzo_Livello"), 0)
                    Dim Udm_Cod As Integer = If(Prezzo_Livello = 5, 38, If(obj("Udm_Cod"), 2))

                    listino_prezzi.Udm_Cod = Udm_Cod
                    listino_prezzi.Qta = If(obj("Qta"), 1)
                    listino_prezzi.Prezzo = If(obj("Prezzo"), 0)
                    listino_prezzi.Sconto_Add1 = If(obj("Sconto_Add1"), 0)
                    listino_prezzi.Sconto_Condizione_Valore1 = If(obj("Sconto_Condizione_Valore1"), 0)
                    listino_prezzi.Sconto_Condizione1 = If(listino_prezzi.Sconto_Condizione_Valore1 > 0, 1, 0)
                    listino_prezzi.Sconto_Add2 = If(obj("Sconto_Add2"), 0)
                    listino_prezzi.Sconto_Condizione_Valore2 = If(obj("Sconto_Condizione_Valore2"), 0)
                    listino_prezzi.Sconto_Condizione2 = If(listino_prezzi.Sconto_Condizione_Valore2 > 0, 1, 0)
                    listino_prezzi.Sconto_Add3 = If(obj("Sconto_Add3"), 0)
                    listino_prezzi.Sconto_Condizione_Valore3 = If(obj("Sconto_Condizione_Valore3"), 0)
                    listino_prezzi.Sconto_Condizione3 = If(listino_prezzi.Sconto_Condizione_Valore3 > 0, 1, 0)
                    listino_prezzi.Sconto = Math.Round((1 - (1 - listino_prezzi.Sconto_Add1 / 100) * (1 - listino_prezzi.Sconto_Add2 / 100) * (1 - listino_prezzi.Sconto_Add3 / 100)) * -100, 2)
                    listino_prezzi.Cod_Iva = If(obj("Cod_Iva"), -1)
                    listino_prezzi.Cod_Rapporto = If(obj("Cod_Rapporto"), 0)
                    listino_prezzi.Cod_RisUm = If(obj("Cod_RisUm"), 0)
                    listino_prezzi.Cal_Cod = If(obj("Cal_Cod"), 0)
                    listino_prezzi.Qualita_Cod = If(obj("Qualita_Cod"), 0)
                    listino_prezzi.Prezzo_Livello = Prezzo_Livello
                    listino_prezzi.Data_Modifica = Date.Now
                    listino_prezzi.Username_Modifica = objParametri.UsernameOperazione

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        listino_prezzi.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    End If

                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        listino_prezzi.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    End If

                    ' Controllo che non si possano fare modifiche se ci sono movimenti collegati e che non ci siano periodi sovrapposti già registrati
                    ' MessaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, False, True, True)

                    EFArrayToUpdate.Add(listino_prezzi)

                End If
            Next

            For Each obj As JObject In righeCancellateArray

                listino_prezzi = leggi.Leggi_Listini_Prezzi_Dettaglio(piva, obj("KeyListinoPrezzi"), objParametri)

                ' Controllo che non si possano fare modifiche se ci sono movimenti collegati
                ' MessaggioErrore += CheckListiniProdotti(listini_campConf_prodotti, objParametri, False, True, False)

                EFArrayToDelete.Add(listino_prezzi)

            Next

            ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
            If False AndAlso String.IsNullOrEmpty(MessaggioErrore) Then

                Dim listino_prezziElem As Listini_Prezzi_Dettagli = Nothing
                Dim listino_prezziToCompare As Listini_Prezzi_Dettagli = Nothing
                Dim trovatoErrore As Boolean = False

                ' Righe nuove
                For i As Integer = 0 To EFArrayToInsert.Count - 1
                    listino_prezziElem = EFArrayToInsert(i)
                    For x As Integer = 0 To EFArrayToInsert.Count - 1
                        listino_prezziToCompare = EFArrayToInsert(x)
                        If i <> x Then
                            If ((listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Inizio AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Inizio) OrElse
                               (listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Fine AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Fine)) Then
                                trovatoErrore = True
                            End If
                        End If
                    Next
                    For x As Integer = 0 To EFArrayToUpdate.Count - 1
                        listino_prezziToCompare = EFArrayToUpdate(x)
                        If ((listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Inizio AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Inizio) OrElse
                               (listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Fine AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Fine)) Then
                            trovatoErrore = True
                        End If
                    Next
                Next

                If Not trovatoErrore Then
                    ' Righe modificate
                    For i As Integer = 0 To EFArrayToUpdate.Count - 1
                        listino_prezziElem = EFArrayToUpdate(i)
                        For x As Integer = 0 To EFArrayToUpdate.Count - 1
                            listino_prezziToCompare = EFArrayToUpdate(x)
                            If i <> x Then
                                If ((listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Inizio AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Inizio) OrElse
                               (listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Fine AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Fine)) Then
                                    trovatoErrore = True
                                End If
                            End If
                        Next
                        For x As Integer = 0 To EFArrayToInsert.Count - 1
                            listino_prezziToCompare = EFArrayToInsert(x)
                            If ((listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Inizio AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Inizio) OrElse
                               (listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Fine AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Fine)) Then
                                trovatoErrore = True
                            End If
                        Next
                    Next
                End If

                If trovatoErrore Then
                    MessaggioErrore &= "Esistono righe in periodi sovrapposti al periodo " & listino_prezziElem.Validita_Inizio.ToShortDateString & " - " & listino_prezziElem.Validita_Fine.ToShortDateString & "<br/>"
                End If

            End If

            If String.IsNullOrEmpty(MessaggioErrore) Then

                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each prezzi_dettaglio As Listini_Prezzi_Dettagli In EFArrayToInsert
                        GiasContext.Listini_Prezzi_Dettagli.Add(prezzi_dettaglio)
                    Next

                    For Each prezzi_dettaglio As Listini_Prezzi_Dettagli In EFArrayToUpdate
                        GiasContext.Listini_Prezzi_Dettagli.Attach(prezzi_dettaglio)
                        GiasContext.Entry(prezzi_dettaglio).State = EntityState.Modified
                    Next

                    For Each prezzi_dettaglio As Listini_Prezzi_Dettagli In EFArrayToDelete
                        GiasContext.Listini_Prezzi_Dettagli.Attach(prezzi_dettaglio)
                        GiasContext.Listini_Prezzi_Dettagli.Remove(prezzi_dettaglio)
                    Next

                    GiasContext.SaveChanges()

                End Using

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function





End Class