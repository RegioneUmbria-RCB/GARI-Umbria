Imports System.Data.Entity
Imports System.Security.Cryptography
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Parco_Macchine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Parco_Macchine(ByVal macCod As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Parco_Macchine

        Dim Parco_MacchineElem As Parco_Macchine = Nothing

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Parco_MacchineElem = (From m In GiasContext.Parco_Macchine
                                  Where m.Mac_Cod = macCod
                                  Select m).FirstOrDefault()

        End Using

        Return Parco_MacchineElem


    End Function

    Public Function Leggi_Caratteristica(ByVal Mac_Car_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As String
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi_Caratteristica()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT * FROM Macchine_Caratteristiche ")
            strSql.AppendLine(" WHERE Mac_Car_Cod = '" & Agro_SQL_SaveText(CStr(Mac_Car_Cod)) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt.Rows(0).Item("Mac_Car_Des")
    End Function

    '########################################################################
    Public Function Leggi_Caratteristiche(ByVal ClassCode As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi_Caratteristiche()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT UdM.UDM_SIM, M_C.* FROM Macchine_Caratteristiche as M_C ")
            strSql.AppendLine(" join MacchinexCaratteristiche as MxC on  M_C.Mac_Car_Cod = MxC.Mac_Car_Cod ")
            strSql.AppendLine(" LEFT JOIN UnitaMisura UdM On M_C.Mac_Car_Udm_Cod = UdM.UDM_COD")
            strSql.AppendLine(" where MxC.CLASS_CODE = '" & Agro_SQL_SaveText(ClassCode) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function
    '########################################################################
    Public Function Leggi_MacchinaXChiaveAPI(ByVal Codice As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String


        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" select  ")
            strSql.AppendLine(" a.Mac_Cod,b.Ditta_Des as Fornitore ,a.Modello,a.N_Immatricolazione as Matricola,  ")
            strSql.AppendLine(" case ")
            strSql.AppendLine(" 	when a.Alimentazione_Cod=0  then 'NonDefinita' ")
            strSql.AppendLine(" 	when a.Alimentazione_Cod=1  then 'Benzina' ")
            strSql.AppendLine(" 	when a.Alimentazione_Cod=2  then 'Gasolio' ")
            strSql.AppendLine(" 	when a.Alimentazione_Cod=3  then 'Metano' ")
            strSql.AppendLine(" 	when a.Alimentazione_Cod=4  then 'Gpl' ")
            strSql.AppendLine(" 	when a.Alimentazione_Cod=5  then 'Elettricità' ")
            strSql.AppendLine(" 	when a.Alimentazione_Cod=6  then 'OlioCombustibile' ")
            strSql.AppendLine(" 	when a.Alimentazione_Cod=7  then 'Petrolio' ")
            strSql.AppendLine(" end as Alimentazione")
            strSql.AppendLine(" from Parco_Macchine a left join Ditte b on (a.Ditta_Cod=b.Ditta_Cod) ")
            strSql.AppendLine(" where ExternalAPIKey='" & Agro_SQL_SaveText(Codice) & "'  ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Macchina_Image_da_ChiaveAPI(ByVal Codice As String,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String


        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" select ")
            strSql.AppendLine("     Img_Thumbnail,Img_Thumbnail_FileName, Img_Thumbnail_Extension, ")
            strSql.AppendLine("     Img_Large,Img_Large_Filename, Img_Large_Extension ")
            strSql.AppendLine(" from Parco_Macchine ")
            strSql.AppendLine(" where ExternalAPIKey='" & Agro_SQL_SaveText(Codice) & "' ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    '########################################################################
    Public Function leggi_x_anagrafica(ByVal Piva As String,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal AliasChiaveMacchina As String = "chiave"
                                       ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("SELECT")
            strSql.AppendLine("    Parco_Macchine.piva +'_'+ cast(Parco_Macchine.sa_cod as nvarchar(50)) + '_' + cast(Parco_Macchine.mac_cod as nvarchar(50))  as " & AliasChiaveMacchina)
            strSql.AppendLine("    , Parco_Macchine.Piva")
            strSql.AppendLine("    , Imprese.rag_soc")
            strSql.AppendLine("    , Parco_Macchine.Sa_Cod")
            strSql.AppendLine("    , Parco_Macchine.Mac_Cod")
            strSql.AppendLine("    , Contatti.Cod_Contatto")
            strSql.AppendLine("    , Case Contatti.Rag_Soc WHEN '' then  Contatti.Cognome + ' ' +  Contatti.Nome ")
            strSql.AppendLine("      Else Contatti.Rag_Soc End As Contatto_Des")
            strSql.AppendLine("    , Parco_Macchine.Codice")
            strSql.AppendLine("    , mac_des as macchina")
            strSql.AppendLine("    , class_desc as tipologia")

            strSql.AppendLine("    , Parco_Macchine.CLASS_CODE")
            strSql.AppendLine("    , Parco_Macchine.Agea_Cod")
            strSql.AppendLine("    , Codifica_Macchine_Agea.AGEA_Des")

            strSql.AppendLine("    , Ditte.Ditta_Des")
            strSql.AppendLine("    , Ditte.Ditta_Cod")
            strSql.AppendLine("    , Parco_Macchine.Modello")
            strSql.AppendLine("    , Parco_Macchine.Targa")
            strSql.AppendLine("    , Parco_Macchine.Telaio")

            ' aggiunti campi per macchine irrigazione
            strSql.AppendLine("    , Parco_Macchine.Portata")
            strSql.AppendLine("    , Parco_Macchine.Efficienza")
            strSql.AppendLine("    , Parco_Macchine.IMP_COD")

            strSql.AppendLine("    , ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Parco_Macchine.Username_Creazione), Parco_Macchine.Username_Creazione) AS Utente_Creazione")
            strSql.AppendLine("    , ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Parco_Macchine.Username_Modifica), Parco_Macchine.Username_Modifica) AS Utente_Modifica")

            strSql.AppendLine("    , Parco_Macchine.Data_Creazione")
            strSql.AppendLine("    , Parco_Macchine.Data_Modifica")

            strSql.AppendLine("    , Parco_Macchine.Ultima_Manutenzione")
            strSql.AppendLine("    , Parco_Macchine.Ultima_Revisione")

            'strSql.AppendLine("    , Parco_Macchine.VIN")

            strSql.AppendLine("    , Parco_Macchine.CUAA_Proprietario")
            strSql.AppendLine("    , Parco_Macchine.Denominazione_Proprietario")
            strSql.AppendLine("    , Parco_Macchine.TitoloPossesso")

            strSql.AppendLine("    , CASE Parco_Macchine.TitoloPossesso")
            strSql.AppendLine("        WHEN 0 THEN 'Altro'")
            strSql.AppendLine("        WHEN 1 THEN 'Proprietà'")
            strSql.AppendLine("        WHEN 2 THEN 'Comodato d''uso'")
            strSql.AppendLine("        WHEN 3 THEN 'Affitto con contratto'")
            strSql.AppendLine("        WHEN 4 THEN 'Affitto senza contratto'")
            strSql.AppendLine("        WHEN 5 THEN 'In conto terzi'")
            strSql.AppendLine("        WHEN 6 THEN 'In convenzione'")
            strSql.AppendLine("        WHEN 7 THEN 'In compartecipazione'")
            strSql.AppendLine("        ELSE 'Non Definito'")
            strSql.AppendLine("    END   as TitoloPossesso_Des")

            strSql.AppendLine("    , Parco_Macchine.Validita_Inizio")
            strSql.AppendLine("    , Parco_Macchine.Validita_Fine")

            strSql.AppendLine("    , Centri_Aziendali.sa_nome")
            strSql.AppendLine("    , CASE WHEN Parco_Macchine.sa_cod = 0 THEN 'Macchina Privata'")
            strSql.AppendLine("      WHEN Parco_Macchine.sa_cod = -1 THEN 'Macchina Pubblica'")
            strSql.AppendLine("      Else Centri_Aziendali.sa_nome End As [Visibilita]")

            strSql.AppendLine("    , CASE WHEN Parco_Macchine.Validita_Inizio < GETDATE() AND Parco_Macchine.validita_fine > GETDATE() THEN 1 ELSE 0 END as Attivo")
            strSql.AppendLine("")
            strSql.AppendLine("FROM Parco_Macchine")
            strSql.AppendLine("    INNER JOIN UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA")
            strSql.AppendLine("")
            strSql.AppendLine("    LEFT JOIN Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE")
            strSql.AppendLine("")
            strSql.AppendLine("    LEFT JOIN Codifica_Macchine_Agea ON Parco_Macchine.Agea_Cod= Codifica_Macchine_Agea.AGEA_Cod")
            strSql.AppendLine("")
            strSql.AppendLine("    INNER JOIN Imprese ON Imprese.Piva = Parco_Macchine.Piva")
            strSql.AppendLine("")
            strSql.AppendLine("    LEFT JOIN Ditte ON Ditte.Ditta_Cod = Parco_Macchine.Ditta_Cod")
            strSql.AppendLine("")
            strSql.AppendLine("    LEFT JOIN Contatti on Contatti.Piva = Parco_Macchine.Piva")
            strSql.AppendLine("        AND Contatti.cod_Contatto = Parco_Macchine.Cod_Contatto")
            strSql.AppendLine("")
            strSql.AppendLine("    LEFT JOIN Centri_aziendali ON Parco_Macchine.Piva = Centri_Aziendali.Piva AND Centri_aziendali.sa_cod = Parco_Macchine.Sa_Cod")
            strSql.AppendLine("")

            strSql.AppendLine("WHERE 1 = 1")
            'strSql.AppendLine(" AND UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            ' aggiunto filtro su finestra temporale
            strSql.AppendLine("    AND Parco_Macchine.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            strSql.AppendLine("    AND Parco_Macchine.validita_fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Piva <> "" Then
                strSql.AppendLine("    AND ( ( Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim FiltroCentri As String = ""
            Dim DtCentriVisibili As DataTable
            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                FiltroCentri = "0,"
                For i = 0 To DtCentriVisibili.Rows.Count - 1
                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                Next
                If FiltroCentri <> "" Then
                    strSql.AppendLine("    AND Parco_Macchine.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                End If
            End If

            strSql.AppendLine("    ) OR Parco_Macchine.Sa_Cod = -1)")

            '--------------------------------------------------------------------------
            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                strSql.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine("    AND   Parco_Macchine.Inviato >=0")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine("    AND   Parco_Macchine.Inviato =-1")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine("ORDER BY Parco_Macchine.Mac_Des  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '########################################################################
    'legge la tabella Parco_Macchine e Macchine
    'per maggiori dettagli (tipo i costi, udm, ecc) usare Parco_Macchine_Leggi
    Public Function Leggi(ByVal Piva As String,
                          ByVal Mac_Cod As Integer,
                          ByVal FlagPubblico As Boolean,
                          ByVal Class_Code As String,
                          ByVal Targa As String,
                          ByVal Telaio As String,
                          ByVal Modello As String,
                          ByVal Potenza As String,
                          ByVal Ditta_Cod As Integer,
                          ByVal Ordinamento As String,
                          ByVal Flag_Costi As Boolean,
                          ByVal Mac_Cod_Origine As Long,
                          ByVal Piva_SuperUser_Origine As String,
                          ByVal Flag_AncheImportati As Boolean,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal N_Immatricolazione As String = ""
                          ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT")
            strSql.AppendLine("    Parco_Macchine.*")
            strSql.AppendLine("    , CASE")
            strSql.AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 0 THEN 'Non Definito'")
            strSql.AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 1 THEN 'JohnDeere'")
            strSql.AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 2 THEN 'Agrirouter'")
            strSql.AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 3 THEN 'AGCO_Trimble'")
            strSql.AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 4 THEN 'CNH1'")
            strSql.AppendLine("    END AS HubIot_PlatformDestinationDes")
            strSql.AppendLine("    , agea.AGEA_Des")
            strSql.AppendLine("    , Macchine.CLASS_DESC")
            strSql.AppendLine("    , Imprese.Rag_Soc AS Impresa")
            strSql.AppendLine("    , ISNULL((select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 2) and Len(Class_Code)=2),'') as tipo_desc")
            strSql.AppendLine("    , ISNULL((select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 5) and Len(Class_Code)=5),'') as dettaglio_1_desc")
            strSql.AppendLine("    , ISNULL((select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 7) and Len(Class_Code)=7 ),'') as dettaglio_2_desc")

            strSql.AppendLine("    , coalesce(D.Ditta_Des, '') as Ditta_Des")
            strSql.AppendLine("    , COALESCE(pcm.val_cod, '') AS numero_certificato")

            strSql.AppendLine("FROM Parco_Macchine WITH(NOLOCK)")
            strSql.AppendLine("")
            strSql.AppendLine("    INNER JOIN  UtentiXImprese WITH(NOLOCK) ON Parco_Macchine.Piva = UtentiXImprese.PIVA")
            strSql.AppendLine("")
            strSql.AppendLine("    LEFT JOIN  Macchine WITH(NOLOCK) ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE")
            strSql.AppendLine("")
            strSql.AppendLine("    INNER JOIN Imprese WITH(NOLOCK) ON Imprese.Piva = Parco_Macchine.Piva")
            strSql.AppendLine("")
            strSql.AppendLine("    LEFT JOIN Ditte D WITH(NOLOCK) ON D.Ditta_cod = Parco_Macchine.Ditta_Cod")
            strSql.AppendLine("")
            strSql.AppendLine("    LEFT JOIN Codifica_Macchine_Agea agea WITH(NOLOCK) ON Parco_Macchine.Agea_Cod = agea.AGEA_Cod")

            strSql.AppendLine("")
            strSql.AppendLine("    LEFT JOIN Parco_Macchine_Codici pcm WITH(NOLOCK) ON Parco_Macchine.Mac_Cod = pcm.Mac_cod")
            strSql.AppendLine("        AND Parco_Macchine.Sa_Cod = pcm.sa_cod")
            strSql.AppendLine("        AND Parco_Macchine.Piva = pcm.PIVA")
            strSql.AppendLine(String.Format("        AND pcm.id_cod = {0}", CInt(enum_CodiciAnagrafe.MacchinaCertificazioneTaratura)))

            strSql.AppendLine("")
            strSql.AppendLine("WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If Not FlagPubblico Then
                If Piva <> "" Then
                    strSql.AppendLine("    AND (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')")
                End If
            Else
                If Piva <> "" Then
                    strSql.AppendLine("    AND (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Parco_Macchine.Sa_Cod = -1 )")
                End If
            End If

            If Class_Code <> "" Then
                strSql.AppendLine("    AND (Parco_Macchine.Class_Code = '" & Agro_SQL_SaveText(Class_Code) & "')")
            End If

            If Targa <> "" Then
                strSql.AppendLine("    AND (Parco_Macchine.Targa = '" & Agro_SQL_SaveText(Targa) & "')")
            End If

            If Modello <> "" Then
                strSql.AppendLine("    AND (Parco_Macchine.Modello = '" & Agro_SQL_SaveText(Modello) & "')")
            End If

            If Telaio <> "" Then
                strSql.AppendLine("    AND (Parco_Macchine.Telaio = '" & Agro_SQL_SaveText(Telaio) & "')")
            End If

            If Potenza <> "" Then
                strSql.AppendLine("    AND (Parco_Macchine.Potenza = '" & Agro_SQL_SaveText(Potenza) & "')")
            End If

            If Ditta_Cod <> 0 Then
                strSql.AppendLine("    AND (Parco_Macchine.Ditta_Cod = " & Agro_SQL_SaveNum(Ditta_Cod.ToString) & ")")
            End If

            If N_Immatricolazione <> "" Then
                strSql.AppendLine("    AND (Parco_Macchine.N_Immatricolazione = " & Agro_SQL_SaveNum(N_Immatricolazione) & ")")
            End If

            If Mac_Cod <> 0 Then
                strSql.AppendLine("    AND (Parco_Macchine.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod.ToString) & ")")
            End If

            'gias2gias
            If Not Flag_AncheImportati Then
                strSql.AppendLine("    AND Parco_Macchine.Mac_Cod_Origine = 0")
            Else
                If Mac_Cod_Origine <> 0 Then
                    strSql.AppendLine("    AND Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine.ToString))
                End If
                If Piva_SuperUser_Origine <> "" Then
                    strSql.AppendLine("    AND Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                End If
            End If
            'fine gias2gias

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine("    AND Parco_Macchine.Inviato >= 0")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine("    AND Parco_Macchine.Inviato = -1")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine("ORDER BY Parco_Macchine.Mac_Des")

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '########################################################################

    Public Function NewCom_ParcoMacchine_Leggi(ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               Optional ByVal Piva As String = "",
                                               Optional ByVal Mac_Cod As Integer = 0,
                                               Optional ByVal FlagPubblico As Boolean = False,
                                               Optional ByVal Class_Code As String = "",
                                               Optional ByVal Targa As String = "",
                                               Optional ByVal Telaio As String = "",
                                               Optional ByVal Modello As String = "",
                                               Optional ByVal Potenza As String = "",
                                               Optional ByVal Ditta_Cod As Integer = 0,
                                               Optional ByVal Ordinamento As String = "",
                                               Optional ByVal Flag_Costi As Boolean = False
                                               ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT  Imprese.Rag_Soc AS Impresa,    Parco_Macchine.Class_Code, Macchine.CLASS_DESC, Parco_Macchine.Piva, Parco_Macchine.Sa_Cod, Parco_Macchine.Mac_Cod,   ")
            strSql.AppendLine("  Parco_Macchine.Mac_Des, Parco_Macchine.Targa, Parco_Macchine.Modello, Parco_Macchine.Telaio, Parco_Macchine.Potenza, Parco_Macchine.Tipo,  ")
            strSql.AppendLine("  Parco_Macchine.Costo_Acquisto, Parco_Macchine.Ammortamento, Parco_Macchine.Ammortizzato,  ")
            strSql.AppendLine("  Parco_Macchine.Data_Immatricolazione, Parco_Macchine.Ultima_Manutenzione, Parco_Macchine.Ultima_Revisione, Parco_Macchine.Stato_Utilizzo, ")
            strSql.AppendLine("  Parco_Macchine.Note, UtentiXImprese.[USER], Parco_Macchine.Ditta_Cod, ISNULL(Ditte.Ditta_Des, '') AS Ditta_Des, ")
            strSql.AppendLine("  Parco_Macchine.validita_inizio AS Data_Inizio_Utilizzo, Parco_Macchine.validita_fine AS Data_Dismissione,  ")

            strSql.AppendLine(" N_Immatricolazione, N_Immatricolazione_Rimorchio, N_Autorizzazione_Trasporto, Data_Rilascio_Autorizzazione, Peso ")

            If Flag_Costi Then
                strSql.AppendLine(" , ISNULL(Prodotti_Costi.Elem_Cod, 1) AS Elem_Cod, ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo,  ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario, ")
                strSql.AppendLine("  ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine  ")
            End If

            strSql.AppendLine(" FROM Parco_Macchine ")
            strSql.AppendLine(" INNER JOIN UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" INNER JOIN Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE  ")
            strSql.AppendLine(" LEFT OUTER JOIN  Ditte ON Parco_Macchine.Ditta_Cod = Ditte.Ditta_Cod ")

            'JOIN IMPRESE 
            strSql.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Parco_Macchine.Piva ")

            If Flag_Costi Then
                strSql.AppendLine(" LEFT OUTER JOIN Prodotti_Costi ON Parco_Macchine.Mac_Cod = Prodotti_Costi.Mat_Cod And Prodotti_Costi.Id_Budget = 0  ")
            End If

            strSql.AppendLine("   ")

            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")

            If Not FlagPubblico Then
                If Piva <> "" Then
                    strSql.AppendLine(" AND    (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
                End If
                If Mac_Cod <> 0 Then
                    strSql.AppendLine(" AND    (Parco_Macchine.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & ")   ")
                End If
            Else
                If Piva <> "" Then
                    strSql.AppendLine(" AND    (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Parco_Macchine.Sa_Cod = -1 )  ")
                End If
            End If

            If Class_Code <> "" Then
                strSql.AppendLine(" AND    (Parco_Macchine.Class_Code = '" & Agro_SQL_SaveText(Class_Code) & "')   ")
            End If

            If Targa <> "" Then
                strSql.AppendLine(" AND    (Parco_Macchine.Targa = '" & Agro_SQL_SaveText(Targa) & "')   ")
            End If

            If Modello <> "" Then
                strSql.AppendLine(" AND    (Parco_Macchine.Modello = '" & Agro_SQL_SaveText(Modello) & "')   ")
            End If

            If Telaio <> "" Then
                strSql.AppendLine(" AND    (Parco_Macchine.Telaio = '" & Agro_SQL_SaveText(Telaio) & "')   ")
            End If

            If Potenza <> "" Then
                strSql.AppendLine(" AND    (Parco_Macchine.Potenza = '" & Agro_SQL_SaveText(Potenza) & "')   ")
            End If

            If Ditta_Cod <> 0 Then
                strSql.AppendLine(" AND    (Parco_Macchine.Ditta_Cod = " & Agro_SQL_SaveNum(Ditta_Cod) & ")   ")
            End If

            strSql.AppendLine("   ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy = "" Then
                strSql.AppendLine(" ORDER BY Parco_Macchine.Mac_Des  ")
            Else
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiMacchineContatto(ByVal piva As String,
                                          ByVal codContatto As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal validitaInizio As Date = AGRODATAINIZIO,
                                          Optional ByVal validitaFine As Date = AGRODATAFINE
                                          ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiMacchineContatto()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT  Parco_Macchine.* , ISNULL(Ditte.Ditta_Des,'') AS Ditta_Des ")
            strSql.AppendLine(" FROM Parco_Macchine   ")
            strSql.AppendLine(" INNER JOIN  UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Parco_Macchine.Piva ")
            strSql.AppendLine(" LEFT JOIN Ditte ON Parco_Macchine.Ditta_Cod = Ditte.Ditta_Cod ")

            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ")

            strSql.AppendLine(" AND  Parco_Macchine.Validita_Inizio >= " & Agro_SQL_SaveDate(validitaInizio) & " ")
            strSql.AppendLine(" AND  Parco_Macchine.Validita_Fine <= " & Agro_SQL_SaveDate(validitaFine) & " ")

            If piva <> "" Then
                strSql.AppendLine(" AND  Parco_Macchine.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If codContatto <> "" Then
                strSql.AppendLine(" AND  Parco_Macchine.Cod_Contatto = '" & Agro_SQL_SaveText(codContatto) & "' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Parco_Macchine.Mac_Des  ")

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiConDitteCentro(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal LeggiAncheAziendali As Boolean,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiConDitteCentro()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT  Parco_Macchine.*, Macchine.CLASS_DESC, ")
            strSql.AppendLine(" Imprese.Rag_Soc AS Impresa, ditte.Ditta_Des  ")

            strSql.AppendLine(" FROM Parco_Macchine   ")
            strSql.AppendLine(" INNER JOIN  UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" INNER JOIN  Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE  ")
            strSql.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Parco_Macchine.Piva ")
            strSql.AppendLine(" left JOIN ditte ON Parco_Macchine.ditta_cod = ditte.ditta_cod ")

            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ")

            If Piva <> "" Then
                strSql.AppendLine(" AND  Parco_Macchine.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If LeggiAncheAziendali Then
                strSql.AppendLine(" AND  (Parco_Macchine.sa_cod= " & Sa_Cod & " or Parco_Macchine.sa_cod=0) ")
            Else
                strSql.AppendLine(" AND  (Parco_Macchine.sa_cod= " & Sa_Cod & " ) ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Parco_Macchine.Mac_Des  ")

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Tutte_Piva_Che_Hanno_Macchine(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi_Tutte_Piva_Che_Hanno_Macchine()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT  distinct piva  FROM Parco_Macchine   ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '###################################################################################
    Public Function Esiste_MacCod(ByVal Mac_Cod As Integer,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Esiste_MacCod()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        Try

            dt = Leggi2("",
                        Mac_Cod,
                        SACOD_NOFILTRO,
                         0, "", True,
                        "", "",
                        objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                flagEsiste = True
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function

    '########################################################################
    'legge solo la tabella Parco_Macchine 
    'per non filtrare il sa_cod, passare il valore SACOD_NOFILTRO
    'perchè 0 è sigificativo
    Public Function Leggi2(ByVal Piva As String,
                           ByVal Mac_Cod As Integer,
                           ByVal Sa_Cod As Integer,
                           ByVal Mac_Cod_Origine As Long,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal Flag_AncheImportati As Boolean,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi2()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT  Parco_Macchine.* ")

            strSql.AppendLine(" FROM Parco_Macchine   ")
            strSql.AppendLine(" INNER JOIN  UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND    (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
            End If

            If Mac_Cod <> 0 Then
                strSql.AppendLine(" AND    (Parco_Macchine.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod.ToString) & ")   ")
            End If

            If Sa_Cod <> SACOD_NOFILTRO Then
                strSql.AppendLine(" AND    Parco_Macchine.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            'gias2gias
            If Not Flag_AncheImportati Then
                strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ")
            Else
                If Mac_Cod_Origine <> 0 Then
                    strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine.ToString) & "   ")
                End If
                If Piva_SuperUser_Origine <> "" Then
                    strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                End If
            End If
            'fine gias2gias

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Parco_Macchine.Mac_Des  ")

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '########################################################################
    'union di due parti:
    'prima parte macchine senza costi
    'seconda parte macchine con costi
    'USATA DA GESTIONE PRODOTTI E RISORSE DEL GIASONLINE
    Public Function Leggi3(ByVal Piva As String,
                           ByVal Mac_Cod As Integer,
                           ByVal Sa_Cod As Integer,
                           ByVal TestoRicerca As String,
                           ByVal Mac_Cod_Origine As Long,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal Flag_AncheImportati As Boolean,
                           ByVal xFiltroAggiuntivo1 As String,
                           ByVal xFiltroAggiuntivo2 As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi3()"

        Dim dt As New DataTable
        Dim i As Integer = 0
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            strSql.Length = 0

            '----------------------------------------------------
            '--- MACCHINE SENZA COSTI ---------------------------
            '----------------------------------------------------
            strSql.AppendLine(" SELECT * FROM ( ")
            strSql.AppendLine(" ( ")
            strSql.AppendLine("  SELECT   Parco_Macchine.Mac_Cod, Parco_Macchine.Mac_Des, CLASS_DESC, Rag_Soc, ")
            strSql.AppendLine("  Parco_Macchine.Class_Code,    Parco_Macchine.Targa, Parco_Macchine.Modello, Parco_Macchine.Piva, Parco_Macchine.Sa_Cod ,")
            strSql.AppendLine("   0 AS Prezzo_Unitario, '01/01/1900' as Validita_Inizio, '31/12/2100' as Validita_Fine, -1 as Mezzo ")
            strSql.AppendLine("   , Parco_Macchine.ditta_cod, isnull(Ditte.ditta_des,'') as ditta_des ")

            strSql.AppendLine(" FROM Parco_Macchine   ")
            strSql.AppendLine(" INNER JOIN Macchine ON  Parco_Macchine.Class_Code = Macchine.Class_Code ")
            strSql.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Parco_Macchine.Piva ")
            strSql.AppendLine(" INNER JOIN  UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" LEFT outer JOIN Ditte ON  Parco_Macchine.ditta_cod = Ditte.ditta_cod ")

            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            strSql.AppendLine(" AND NOT EXISTS ( ")
            strSql.AppendLine("                 SELECT 1 ")
            strSql.AppendLine("                 FROM Prodotti_Costi ")
            strSql.AppendLine("                  WHERE Elem_Cod=1 And Prodotti_Costi.Id_Budget = 0  ")
            strSql.AppendLine("                 AND Parco_Macchine.Mac_Cod = Prodotti_Costi.Mat_Cod )")

            strSql.AppendLine(" AND ( Mac_Des Like '%" & TestoRicerca & "%'")
            strSql.AppendLine("     OR  Targa Like '%" & TestoRicerca & "%'")
            strSql.AppendLine("     OR  telaio Like '%" & TestoRicerca & "%'")
            strSql.AppendLine("     OR  modello Like '%" & TestoRicerca & "%'")
            strSql.AppendLine("     ) ")

            'commentato in data 09/12/2014:
            'la piva deve esserci sempre altrimenti il filtro sulla visibilità (Impresa, centro, pubblico) non funziona
            'If Piva <> "" Then
            '    strSql.AppendLine(" AND    (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
            'End If

            If Mac_Cod <> 0 Then
                strSql.AppendLine(" AND    (Parco_Macchine.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod.ToString) & ")   ")
            End If

            If Sa_Cod <> SACOD_NOFILTRO Then
                strSql.AppendLine(" AND    Parco_Macchine.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            Else

                strSql.AppendLine(" AND    ( (Parco_Macchine.Sa_Cod = -1)")

                'visibilità centro
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= " (Parco_Macchine.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Parco_Macchine.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                    Next
                    If FiltroCentri <> "" Then
                        strSql.AppendLine(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Parco_Macchine.Sa_Cod = 0) ) ")
                    End If
                End If

                'aziendali
                If FiltroCentri = "" Then
                    strSql.AppendLine("    OR  (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                End If

                strSql.AppendLine("        ) ")

            End If

            'gias2gias
            If Not Flag_AncheImportati Then
                strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ")
            Else
                If Mac_Cod_Origine <> 0 Then
                    strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine.ToString) & "   ")
                End If
                If Piva_SuperUser_Origine <> "" Then
                    strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                End If
            End If
            'fine gias2gias

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo1 <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            strSql.AppendLine(" ) ")

            '----------------------------------------------------
            '--- UNION ALL ---------------------------
            '----------------------------------------------------
            strSql.AppendLine(" UNION ALL ")


            '----------------------------------------------------
            '--- MACCHINE CON COSTI ---------------------------
            '----------------------------------------------------
            strSql.AppendLine(" ( ")
            strSql.AppendLine("  SELECT   Parco_Macchine.Mac_Cod, Parco_Macchine.Mac_Des, CLASS_DESC,Rag_Soc, ")
            strSql.AppendLine("  Parco_Macchine.Class_Code,    Parco_Macchine.Targa, Parco_Macchine.Modello, Parco_Macchine.Piva, Parco_Macchine.Sa_Cod ,")
            strSql.AppendLine("  Prodotti_Costi.Prezzo_Unitario,  Prodotti_Costi.Validita_Inizio ,  Prodotti_Costi.Validita_Fine ,  Prodotti_Costi.Mezzo ")
            strSql.AppendLine("   , Parco_Macchine.ditta_cod, isnull(Ditte.ditta_des,'') as ditta_des ")

            strSql.AppendLine(" FROM Parco_Macchine   ")
            strSql.AppendLine(" INNER JOIN Macchine ON  Parco_Macchine.Class_Code = Macchine.Class_Code ")
            strSql.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Parco_Macchine.Piva ")
            strSql.AppendLine(" INNER JOIN  UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" INNER JOIN Prodotti_Costi on Parco_Macchine.Mac_Cod = Prodotti_Costi.Mat_Cod And Prodotti_Costi.Id_Budget = 0  ")
            strSql.AppendLine(" LEFT outer JOIN Ditte ON  Parco_Macchine.ditta_cod = Ditte.ditta_cod ")

            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            strSql.AppendLine(" and Elem_Cod=1 ")
            strSql.AppendLine(" AND ( Mac_Des Like '%" & TestoRicerca & "%'")
            strSql.AppendLine("     OR  Targa Like '%" & TestoRicerca & "%'")
            strSql.AppendLine("     OR  telaio Like '%" & TestoRicerca & "%'")
            strSql.AppendLine("     OR  modello Like '%" & TestoRicerca & "%'")
            strSql.AppendLine("     ) ")

            'commentato in data 09/12/2014:
            'la piva deve esserci sempre altrimenti il filtro sulla visibilità (Impresa, centro, pubblico) non funziona
            'If Piva <> "" Then
            '    strSql.AppendLine(" AND    (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
            'End If

            If Mac_Cod <> 0 Then
                strSql.AppendLine(" AND    (Parco_Macchine.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod.ToString) & ")   ")
            End If

            If Sa_Cod <> SACOD_NOFILTRO Then
                strSql.AppendLine(" AND    Parco_Macchine.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            Else

                strSql.AppendLine(" AND    ( (Parco_Macchine.Sa_Cod = -1) ")

                'visibilità centro
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= " (Parco_Macchine.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Parco_Macchine.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                    Next
                    If FiltroCentri <> "" Then
                        strSql.AppendLine(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Parco_Macchine.Sa_Cod = 0) ) ")
                    End If
                End If

                'aziendali
                If FiltroCentri = "" Then
                    strSql.AppendLine("    OR  (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                End If

                strSql.AppendLine("        ) ")

            End If

            'gias2gias
            If Not Flag_AncheImportati Then
                strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ")
            Else
                If Mac_Cod_Origine <> 0 Then
                    strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine.ToString) & "   ")
                End If
                If Piva_SuperUser_Origine <> "" Then
                    strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                End If
            End If
            'fine gias2gias

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo2 <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine(" ) ")


            strSql.AppendLine(" ) AS Macchine ")
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Class_Desc, Mac_Des ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiMacchinaDaOrigine(ByVal Mac_Cod_Origine As Integer,
                                           ByVal Piva_SuperUser_Origine As String,
                                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiMacchinaDaOrigine()"

        '====================================================================================
        'Parametri opzionali :
        '   Mac_Cod_Origine = 0  
        '   Piva_SuperUser_Origine = ""  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    'Nota: E' necessario l'outer join per ricavare il proprietario della macchina poichè
                    'in versione standalone l'impresa referente potrebbe non essere stata importata.

                    strSql.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                    strSql.AppendLine(" AND    Parco_Macchine.Cod_Contatto = ''  ")

                    If Mac_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    'Nota: E' necessario l'outer join per ricavare il proprietario della macchina poichè
                    'in versione standalone l'impresa referente potrebbe non essere stata importata.

                    strSql.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                    strSql.AppendLine(" AND    Parco_Macchine.Cod_Contatto = ''  ")

                    If Mac_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    'legge la tabella Parco_Macchine e Macchine
    'per maggiori dettagli (tipo i costi, udm, ecc) usare Parco_Macchine_Leggi
    Public Function Leggi_daMacCod(ByVal Piva As String,
                                   ByVal Mac_Cod As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT  Parco_Macchine.*, Macchine.CLASS_DESC, ISNULL(Ditte.Ditta_Des,'') AS Ditta_Des, ")
            strSql.AppendLine(" Imprese.Rag_Soc AS Impresa  , ")
            strSql.AppendLine(" (select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 2) and Len(Class_Code)=2) as tipo_desc ,   ")
            strSql.AppendLine(" (select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 5) and Len(Class_Code)=5) as dettaglio_1_desc ,   ")
            strSql.AppendLine(" (select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 7) and Len(Class_Code)=7 ) as dettaglio_2_desc ")

            strSql.AppendLine(" FROM Parco_Macchine   ")
            strSql.AppendLine(" INNER JOIN  UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" INNER JOIN  Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE  ")
            strSql.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Parco_Macchine.Piva  ")

            strSql.AppendLine(" LEFT JOIN Ditte ON Parco_Macchine.Ditta_Cod = Ditte.Ditta_Cod")

            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND    (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
            End If
            If Mac_Cod <> 0 Then
                strSql.AppendLine(" AND    (Parco_Macchine.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod.ToString) & ")   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Parco_Macchine.Mac_Des  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_DefaultAcqua(ByVal Mac_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi_DefaultAcqua()"

        '====================================================================================
        'Parametri opzionali :
        '   Mac_Cod_Origine = 0  
        '   Piva_SuperUser_Origine = ""  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" SELECT Taratura_Ugello ")
            strSql.AppendLine(" FROM   Parco_Macchine ")
            strSql.AppendLine(" WHERE  1 = 1 ")
            strSql.AppendLine(" AND    Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.AppendLine(" ORDER BY Parco_Macchine.Mac_Des ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiMacchine_xCostiAccessori(ByVal Piva As String,
                                                  ByVal Qs_Data As Date,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  Optional visualizzaMacchineImportate As Boolean = False
                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiMacchine_xCostiAccessori()"

        '====================================================================================
        'Parametri opzionali :
        '   Mac_Cod_Origine = 0  
        '   Piva_SuperUser_Origine = ""  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT M.Class_Desc AS Col_0, PM.Mac_Des AS Col_1, ")
            strSql.AppendLine(" PM.Mac_Cod AS Col_8, PM.Codice as Col_13, ")
            strSql.AppendLine(" ISNULL(d.Ditta_des,'') AS Ditta_des, ISNULL(d.Ditta_cod,0) AS Ditta_cod, ISNULL(PM.Modello,'') AS Modello, ISNULL(PM.Taratura_Ugello,0) AS Taratura_Ugello, ")
            strSql.AppendLine(" PM.Piva, PM.Validita_Taratura_Fine, PM.Class_Code, PM.Targa, PM.Validita_Inizio, PM.Validita_Fine ")

            strSql.AppendLine(" FROM Parco_Macchine PM (NOLOCK) INNER JOIN Macchine M (NOLOCK) ON PM.Class_Code = M.CLASS_CODE ")
            strSql.AppendLine(" INNER JOIN UtentiXImprese (NOLOCK) ON PM.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" LEFT JOIN Ditte d (NOLOCK) ON d.ditta_cod = pm.ditta_cod ")
            'StrSQL.AppendLine(" WHERE  ((PM.Piva = '" & Agro_SQL_SaveText(Piva) & "' ) OR (PM.Sa_Cod = -1)) ")
            'StrSQL.AppendLine(" AND    PM.Class_Code = M.Class_Code ")
            'StrSQL.AppendLine(" AND    PM.Validita_Fine > " & Agro_SQL_SaveDate(Qs_Data) & " ")
            'StrSQL.AppendLine(" AND    PM.Mac_Cod_Origine = 0 ")
            'StrSQL.AppendLine(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            strSql.AppendLine(" WHERE  PM.Class_Code = M.Class_Code ")
            strSql.AppendLine(" AND    PM.Validita_Fine >= " & Agro_SQL_SaveDate(Qs_Data) & " ")

            If Not visualizzaMacchineImportate Then
                strSql.AppendLine(" AND    PM.Mac_Cod_Origine = 0 ")
            End If
            strSql.AppendLine(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'pubbliche
            strSql.AppendLine(" AND    ( (PM.Sa_Cod = -1) ")

            'visibilità centro
            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim FiltroCentri As String = ""
            Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To DtCentriVisibili.Rows.Count - 1
                    FiltroCentri &= " (PM.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND PM.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                Next
                If FiltroCentri <> "" Then
                    strSql.AppendLine(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (PM.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND PM.Sa_Cod = 0) ) ")
                End If
            End If

            'aziendali
            If FiltroCentri = "" Then
                strSql.AppendLine("          OR  (PM.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
            End If

            strSql.AppendLine("        ) ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   PM.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   PM.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.AppendLine(" ORDER BY Parco_Macchine.Mac_Des ASC ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiMacchine_xCostiAccessori_Terzisti(ByVal strPive As String,
                                                           ByVal Qs_Data As Date,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByVal xOrderBy As String,
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiMacchine_xCostiAccessori()"

        '====================================================================================
        'Parametri opzionali :
        '   Mac_Cod_Origine = 0  
        '   Piva_SuperUser_Origine = ""  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT M.Class_Desc AS Col_0, PM.Mac_Des AS Col_1, ")
            strSql.AppendLine(" PM.Mac_Cod AS Col_8, PM.Piva ")
            strSql.AppendLine(" FROM   Parco_Macchine PM INNER JOIN Macchine M ON ")
            strSql.AppendLine(" PM.Class_Code = M.CLASS_CODE ")
            strSql.AppendLine(" INNER JOIN UtentiXImprese ON PM.Piva = UtentiXImprese.PIVA ")

            strSql.AppendLine(" WHERE  PM.Class_Code = M.Class_Code ")
            strSql.AppendLine(" AND    PM.Validita_Fine > " & Agro_SQL_SaveDate(Qs_Data) & " ")
            strSql.AppendLine(" AND    PM.Mac_Cod_Origine = 0 ")
            strSql.AppendLine(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            ''pubbliche
            'StrSQL.AppendLine(" AND    ( (PM.Sa_Cod = -1) ")
            strSql.AppendLine(" AND    (  ")

            'visibilità centro
            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim FiltroCentri As String = ""
            Dim DtCentriVisibili As DataTable
            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva IN (" & strPive & ")", "", objParametri)
            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To DtCentriVisibili.Rows.Count - 1
                    FiltroCentri &= " (PM.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND PM.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                Next
                If FiltroCentri <> "" Then
                    strSql.AppendLine(" (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (PM.Piva = (" & strPive & ") AND PM.Sa_Cod = 0) ) ")
                End If
            End If

            'aziendali
            If FiltroCentri = "" Then
                strSql.AppendLine("            (PM.Piva IN (" & Agro_SQL_Save_Clausola_IN(strPive, True) & ") )  ")
            End If

            strSql.AppendLine("        ) ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   PM.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   PM.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.AppendLine(" ORDER BY Parco_Macchine.Mac_Des ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function MacchinaDes(ByVal Piva As String,
                                ByVal DataLavorazione As Date,
                                ByVal Mac_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.MacchinaDes()"

        '====================================================================================
        'Parametri opzionali :
        '   Mac_Cod_Origine = 0  
        '   Piva_SuperUser_Origine = ""  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT M.CLASS_DESC, PM.Mac_Des, PM.Targa, PM.Mac_Cod, PM.Ammortamento, PM.Taratura_Ugello, Ditte.Ditta_Des, PM.Modello ")
            strSql.AppendLine(" FROM   Parco_Macchine AS PM INNER JOIN ")
            strSql.AppendLine("       Macchine AS M ON PM.Class_Code = M.CLASS_CODE INNER JOIN ")
            strSql.AppendLine("       UtentiXImprese ON PM.Piva = UtentiXImprese.PIVA LEFT JOIN ")
            strSql.AppendLine("       Ditte ON PM.Ditta_Cod = Ditte.Ditta_Cod")
            strSql.AppendLine(" WHERE  ((PM.Piva = '" & Agro_SQL_SaveText(Piva) & "' ) OR (PM.Sa_Cod = -1)) ")
            strSql.AppendLine(" AND    PM.Class_Code = M.Class_Code ")
            strSql.AppendLine(" AND    PM.Validita_Fine > " & Agro_SQL_SaveDate(DataLavorazione) & " ")
            strSql.AppendLine(" AND    PM.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            strSql.AppendLine(" AND    PM.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   PM.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   PM.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.AppendLine(" ORDER BY Parco_Macchine.Mac_Des ASC ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '########################################################################
    'I costi sono in LEFT OUTER JOIN E SONO FACOLTATIVI (SI DECIDE IN BASE AL FLAG)
    'RICORDARSI POI A LIVELLO DI CODICE DI FILTRARE ELEM_COD = 1
    Public Function ParcoMacchine_Leggi(ByVal Piva As String,
                                        ByVal Mac_Cod As Integer,
                                        ByVal FlagPubblico As Boolean,
                                        ByVal Class_Code As String,
                                        ByVal Targa As String,
                                        ByVal Telaio As String,
                                        ByVal Modello As String,
                                        ByVal Potenza As String,
                                        ByVal Ditta_Cod As Integer,
                                        ByVal Ordinamento As String,
                                        ByVal Flag_Costi As Boolean,
                                        ByVal Mac_Cod_Origine As Long,
                                        ByVal Piva_SuperUser_Origine As String,
                                        ByVal Flag_AncheImportati As Boolean,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable


        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.ParcoMacchine_Leggi()"

        Dim dt As New DataTable
        Dim i As Integer = 0
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    strSql.AppendLine("SELECT * FROM Parco_Macchine")
                    strSql.AppendLine($"WHERE Mac_Cod = {Mac_Cod}")

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '----------------------------------------------------
                    '--- Preparo la Query SQL ---------------------------
                    '----------------------------------------------------
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT  Parco_Macchine.*, Macchine.CLASS_DESC, ")
                    strSql.AppendLine(" Imprese.Rag_Soc AS Impresa, UtentiXImprese.[USER], ISNULL(Ditte.Ditta_Des, '') AS Ditta_Des,  ")
                    strSql.AppendLine(" Parco_Macchine.validita_inizio AS Data_Inizio_Utilizzo, Parco_Macchine.validita_fine AS Data_Dismissione, ")
                    strSql.AppendLine(" ISNULL(DitteXMotore.Ditta_Des, '') As Motore_Ditta_Des, ")
                    strSql.AppendLine(" CASE Parco_Macchine.TitoloPossesso ")
                    strSql.AppendLine("     WHEN 0 THEN 'Altro' ")
                    strSql.AppendLine("     WHEN 1 THEN 'Proprietà' ")
                    strSql.AppendLine("     WHEN 2 THEN 'Comodato d''uso' ")
                    strSql.AppendLine("     WHEN 3 THEN 'Affitto con contratto' ")
                    strSql.AppendLine("     WHEN 4 THEN 'Affitto senza contratto' ")
                    strSql.AppendLine("     WHEN 5 THEN 'In conto terzi' ")
                    strSql.AppendLine("     WHEN 6 THEN 'In convenzione' ")
                    strSql.AppendLine("     WHEN 7 THEN 'In compartecipazione' ")
                    strSql.AppendLine("     ELSE 'Non Definito' ")
                    strSql.AppendLine(" END   as TitoloPossesso_Des, ")
                    strSql.AppendLine(" Carburanti.Car_Des, ")
                    strSql.AppendLine(" ISNULL(UnitaMisura.UDM_SIM,'') AS Potenza_Udm_Des, ")

                    strSql.AppendLine("ISNULL((SELECT TOP 1 Parco_Macchine_Codici.Val_Cod FROM Parco_Macchine_Codici ")
                    strSql.AppendLine("        WHERE Parco_Macchine.mac_cod = Parco_Macchine_Codici.mac_cod ")
                    strSql.AppendLine("	       AND Parco_Macchine_Codici.Id_Cod = 1016 ")
                    strSql.AppendLine("	       ORDER BY Parco_Macchine_Codici.Validita_Inizio DESC ), '0') AS Titolo_Possesso, ")

                    strSql.AppendLine("ISNULL((SELECT TOP 1 Parco_Macchine_Codici.Validita_Inizio FROM Parco_Macchine_Codici ")
                    strSql.AppendLine("        WHERE Parco_Macchine.mac_cod = Parco_Macchine_Codici.mac_cod ")
                    strSql.AppendLine("	       AND Parco_Macchine_Codici.Id_Cod = 1016 ")
                    strSql.AppendLine("	       ORDER BY Parco_Macchine_Codici.Validita_Inizio DESC ), '01/01/1900') AS Inizio_Possesso, ")

                    strSql.AppendLine("ISNULL((SELECT TOP 1 Parco_Macchine_Codici.Validita_Fine FROM Parco_Macchine_Codici ")
                    strSql.AppendLine("        WHERE Parco_Macchine.mac_cod = Parco_Macchine_Codici.mac_cod ")
                    strSql.AppendLine("	       AND Parco_Macchine_Codici.Id_Cod = 1016 ")
                    strSql.AppendLine("	       ORDER BY Parco_Macchine_Codici.Validita_Inizio DESC ), '31/12/2100') AS Fine_Possesso, ")

                    strSql.AppendLine("ISNULL((SELECT TOP 1 Parco_Macchine_Codici.Val_Cod FROM Parco_Macchine_Codici ")
                    strSql.AppendLine("        WHERE Parco_Macchine.mac_cod = Parco_Macchine_Codici.mac_cod ")
                    strSql.AppendLine("	       AND Parco_Macchine_Codici.Id_Cod = " & enum_DatiAnagrafici_CodiciAnagrafe.Macchine & " ), '0') AS Modifica ")

                    If Flag_Costi Then

                        strSql.AppendLine(" , ISNULL(Prodotti_Costi.Elem_Cod, 1) AS Elem_Cod, ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo,  ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario,  ")
                        strSql.AppendLine(" ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine  ")

                    End If

                    strSql.AppendLine(" FROM Parco_Macchine INNER JOIN   ")
                    strSql.AppendLine(" UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA INNER JOIN  ")
                    strSql.AppendLine(" Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE  ")
                    strSql.AppendLine(" LEFT OUTER JOIN  Ditte ON Parco_Macchine.Ditta_Cod = Ditte.Ditta_Cod ")
                    strSql.AppendLine(" LEFT OUTER JOIN  Ditte DitteXMotore ON Parco_Macchine.Ditta_Cod_Motore = DitteXMotore.Ditta_Cod ")
                    strSql.AppendLine(" LEFT OUTER JOIN  Carburanti ON Parco_Macchine.Alimentazione_Cod = Carburanti.Car_Cod ")
                    strSql.AppendLine(" LEFT OUTER JOIN  UnitaMisura ON Parco_Macchine.Potenza_Udm_Cod = UnitaMisura.UDM_COD ")

                    'JOIN IMPRESE 
                    strSql.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Parco_Macchine.Piva ")

                    ''LEFT JOIN Macchine_Codici
                    'StrSQL.AppendLine(" LEFT OUTER JOIN Parco_Macchine_Codici ON Parco_Macchine.Piva = Parco_Macchine_Codici.Piva, Parco_Macchine.Sa_Cod = Parco_Macchine_Codici.Sa_Cod, Parco_Macchine.Mac_Cod = Parco_Macchine_Codici.Mac_Cod  ")

                    If Flag_Costi Then
                        strSql.AppendLine(" LEFT OUTER JOIN Prodotti_Costi ON Parco_Macchine.Mac_Cod = Prodotti_Costi.Mat_Cod And Prodotti_Costi.Id_Budget = 0  ")
                    End If


                    strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If Not FlagPubblico Then

                        If Piva <> "" Then

                            strSql.AppendLine(" AND    (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")

                            'pubbliche
                            strSql.AppendLine(" AND    ( (Parco_Macchine.Sa_Cod = -1) ")

                            'visibilità centro
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim filtroCentri As String = ""
                            Dim DtCentriVisibili As DataTable
                            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    filtroCentri &= " (Parco_Macchine.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Parco_Macchine.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                                Next
                                If filtroCentri <> "" Then
                                    strSql.AppendLine(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Parco_Macchine.Sa_Cod = 0) ) ")
                                End If
                            End If

                            'aziendali
                            If filtroCentri = "" Then
                                strSql.AppendLine("          OR  (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                            End If

                            strSql.AppendLine("        ) ")

                        End If

                        If Mac_Cod <> 0 Then
                            strSql.AppendLine(" AND    (Parco_Macchine.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod.ToString) & ")   ")
                        End If
                    Else

                        If Piva <> "" Then

                            'StrSQL.AppendLine(" AND    (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Parco_Macchine.Sa_Cod = -1 )  ")

                            'pubbliche
                            strSql.AppendLine(" AND    ( (Parco_Macchine.Sa_Cod = -1) ")

                            'visibilità centro
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim filtroCentri As String = ""
                            Dim DtCentriVisibili As DataTable
                            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    filtroCentri &= " (Parco_Macchine.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Parco_Macchine.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                                Next
                                If filtroCentri <> "" Then
                                    strSql.AppendLine(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Parco_Macchine.Sa_Cod = 0) ) ")
                                End If
                            End If

                            'aziendali
                            If filtroCentri = "" Then
                                strSql.AppendLine("          OR  (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                            End If

                            strSql.AppendLine("        ) ")

                        End If
                    End If

                    If Class_Code <> "" Then
                        strSql.AppendLine(" AND    (Parco_Macchine.Class_Code = '" & Agro_SQL_SaveText(Class_Code) & "')   ")
                    End If

                    If Targa <> "" Then
                        strSql.AppendLine(" AND    (Parco_Macchine.Targa = '" & Agro_SQL_SaveText(Targa) & "')   ")
                    End If

                    If Modello <> "" Then
                        strSql.AppendLine(" AND    (Parco_Macchine.Modello = '" & Agro_SQL_SaveText(Modello) & "')   ")
                    End If

                    If Telaio <> "" Then
                        strSql.AppendLine(" AND    (Parco_Macchine.Telaio = '" & Agro_SQL_SaveText(Telaio) & "')   ")
                    End If

                    If Potenza <> "" Then
                        strSql.AppendLine(" AND    (Parco_Macchine.Potenza = '" & Agro_SQL_SaveText(Potenza) & "')   ")
                    End If

                    If Ditta_Cod <> 0 Then
                        strSql.AppendLine(" AND    (Parco_Macchine.Ditta_Cod = " & Agro_SQL_SaveNum(Ditta_Cod.ToString) & ")   ")
                    End If

                    'If Flag_Costi = True Then
                    '    StrSQL.AppendLine(" AND Prodotti_Costi.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    '    StrSQL.AppendLine(" AND Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
                    'End If


                    'gias2gias
                    If Mac_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine.ToString) & "   ")
                    End If
                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If
                    If Not Flag_AncheImportati Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ")
                    End If
                    'fine gias2gias


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Mac_Des  ")

                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
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
    Public Function LeggiParcoMacchinexSuperUser(ByVal PIVA As String,
                                                 ByVal Mac_Cod As Long,
                                                 ByVal Mac_Cod_Origine As Long,
                                                 ByVal Piva_SuperUser_Origine As String,
                                                 ByVal Flag_AncheImportatati As Boolean,
                                                 ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiParcoMacchinexSuperUser()"
        Dim messaggioErrore As String
        Dim dt As New DataTable
        Dim i As Integer = 0
        Dim strSql As New StringBuilder

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '----------------------------------------------------
                    '--- Preparo la Query SQL ---------------------------
                    '----------------------------------------------------
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ")
                    strSql.AppendLine(" INNER  JOIN UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND    UtentiXImprese.[USER] = '" & Trim(objParametri.PivaSuperUser) & "'")
                    'strSql.AppendLine(" AND    Parco_Macchine.Cod_Contatto = ''  ")

                    Select Case Mac_Cod

                        Case 0 'Lettura delle Macchine Visibili dall'Impresa

                            'StrSQL.AppendLine(" AND ( Parco_Macchine.Piva = '" & Agro_Sql_SaveText(Trim(PIVA)) & "'  OR Parco_Macchine.Sa_Cod = -1 )   ")

                            'pubbliche
                            strSql.AppendLine(" AND    ( (Parco_Macchine.Sa_Cod = -1) ")

                            'visibilità centro
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim filtroCentri As String = ""
                            Dim DtCentriVisibili As DataTable
                            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(PIVA) & "'", "", objParametri)
                            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    filtroCentri &= " (Parco_Macchine.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Parco_Macchine.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                                Next
                                If filtroCentri <> "" Then
                                    strSql.AppendLine(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(PIVA) & "' AND Parco_Macchine.Sa_Cod = 0) ) ")
                                End If
                            End If

                            'aziendali
                            If filtroCentri = "" Then
                                strSql.AppendLine("          OR  (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(PIVA) & "')  ")
                            End If

                            strSql.AppendLine("        ) ")



                        Case Else 'Lettura Mirata

                            strSql.AppendLine(" AND Parco_Macchine.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & "   ")

                    End Select

                    If Mac_Cod_Origine <> 0 Then

                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   ")

                    End If

                    If Piva_SuperUser_Origine <> "" Then

                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")

                    End If

                    If Not Flag_AncheImportatati Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ")

                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente, Ditte.Ditta_Des, Macchine.* ")

                    'StrSQL.AppendLine(" FROM    Parco_Macchine LEFT OUTER JOIN ")
                    'StrSQL.AppendLine("       Imprese ON Parco_Macchine.Piva = Imprese.PIVA INNER JOIN ")
                    'StrSQL.AppendLine("       UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA INNER JOIN ")
                    'StrSQL.AppendLine("       Ditte ON Parco_Macchine.Ditta_Cod = Ditte.Ditta_Cod ")

                    strSql.AppendLine(", ISNULL((select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 2) and Len(Class_Code)=2),'') as tipo_desc   ")
                    strSql.AppendLine(", ISNULL((select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 5) and Len(Class_Code)=5),'') as dettaglio_1_desc  ")
                    strSql.AppendLine(", ISNULL((select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 7) and Len(Class_Code)=7 ),'') as dettaglio_2_desc ")



                    strSql.AppendLine(" FROM Parco_Macchine (NOLOCK) LEFT OUTER JOIN ")
                    strSql.AppendLine("   Imprese (NOLOCK) ON Parco_Macchine.Piva = Imprese.PIVA LEFT OUTER JOIN ")
                    strSql.AppendLine("   Ditte (NOLOCK) ON Parco_Macchine.Ditta_Cod = Ditte.Ditta_Cod LEFT OUTER JOIN ")
                    strSql.AppendLine("   UtentiXImprese (NOLOCK) ON Parco_Macchine.Piva = UtentiXImprese.PIVA INNER JOIN ")
                    strSql.AppendLine("   Macchine (NOLOCK) ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")




                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND    UtentiXImprese.[USER] = '" & Trim(objParametri.PivaSuperUser) & "'")
                    strSql.AppendLine(" AND    Parco_Macchine.Cod_Contatto = ''  ")

                    Select Case Mac_Cod

                        Case 0 'Lettura delle Macchine Visibili dall'Impresa

                            'StrSQL.AppendLine(" AND ( Parco_Macchine.Piva = '" & Agro_Sql_SaveText(Trim(PIVA)) & "'  OR Parco_Macchine.Sa_Cod = -1 )   ")

                            'pubbliche
                            strSql.AppendLine(" AND    ( (Parco_Macchine.Sa_Cod = -1) ")

                            'visibilità centro
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim filtroCentri As String = ""
                            Dim DtCentriVisibili As DataTable
                            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(PIVA) & "'", "", objParametri)
                            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    filtroCentri &= " (Parco_Macchine.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Parco_Macchine.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                                Next
                                If filtroCentri <> "" Then
                                    strSql.AppendLine(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(PIVA) & "' AND Parco_Macchine.Sa_Cod = 0) ) ")
                                End If
                            End If

                            'aziendali
                            If filtroCentri = "" Then
                                strSql.AppendLine("          OR  (Parco_Macchine.Piva = '" & Agro_SQL_SaveText(PIVA) & "')  ")
                            End If

                            strSql.AppendLine("        ) ")




                        Case Else 'Lettura Mirata

                            strSql.AppendLine(" AND Parco_Macchine.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & "   ")

                    End Select

                    If Mac_Cod_Origine <> 0 Then

                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   ")

                    End If

                    If Piva_SuperUser_Origine <> "" Then

                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")

                    End If

                    If Not Flag_AncheImportatati Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ")

                    End If
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
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
    Public Function MacchineTipoTargaDes_from_MacchineTipoTarga(ByVal Tipo_Targa_Cod As Integer) As String

        Dim des As String

        Select Case Tipo_Targa_Cod

            Case 0
                des = "Non Definito"
            Case 1
                des = "Senza Targa"
            Case 2
                des = "Stradale"
            Case 3
                des = "Rimorchio"
            Case 4
                des = "Triangolare"
            Case Else
                des = "Non Definito"

        End Select

        'Restituisco il risultato
        Return des

    End Function

    '#######################################################################
    Public Function MacchinaDes_from_MacchinaCod(ByVal Piva As String,
                                                 ByVal Mac_Cod As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As String

        Dim dt As DataTable

        dt = LeggiParcoMacchinexSuperUser(CStr(Piva),
                                          CInt(Mac_Cod),
                                          0,
                                          "",
                                          True,
                                          enumSelezioneVariabile.Selezione_JoinCompleta,
                                          "",
                                          "",
                                          objParametri)


        'Se il recordset non è chiuso allora ...
        If dt.Rows.Count > 0 Then
            If dt.Rows(0).Item("Mac_Des").Equals("") Then
                Return dt.Rows(0).Item("Ditta_Des") & " " & dt.Rows(0).Item("Modello")
            Else
                Return dt.Rows(0).Item("Mac_Des")
            End If
        End If


    End Function

    '################################################################################
    Public Function MacchineAlimentazioneDes_from_MacchineAlimentazioneCod(ByVal MacchineAlimentazioneCod As Integer) As String

        Dim des As String

        Select Case MacchineAlimentazioneCod

            Case 0
                des = "Non Definita"
            Case 1
                des = "Benzina"
            Case 2
                des = "Gasolio"
            Case 3
                des = "Metano"
            Case 4
                des = "Gpl"
            Case 5
                des = "Elettricita"
            Case 6
                des = "Olio Combustibile"
            Case 7
                des = "Petrolio"

            Case Else
                des = "Non Definita"

        End Select

        'Restituisco il risultato
        Return des

    End Function

    Public Function LeggiMacchinexClassificazione(ByVal Mac_Cod_Origine As Integer,
                                                  ByVal Class_Code As String,
                                                  ByVal Piva_SuperUser_Origine As String,
                                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiMacchinexClassificazione()"

        '====================================================================================
        'Parametri opzionali :
        '   Mac_Cod_Origine = 0  
        '   Piva_SuperUser_Origine = ""  
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente, Left(Class_Code + '00', 2) as CLASS_CODE_ROOT  ")
                    strSql.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))


                    If Mac_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   ")
                    End If

                    If Class_Code <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Class_Code LIKE '" & Agro_SQL_SaveText(Class_Code) & "%' ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))


                    If Mac_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   ")
                    End If

                    If Class_Code <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Class_Code LIKE '" & Agro_SQL_SaveText(Class_Code) & "%' ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                    strSql.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente, Isnull(Macchine.Class_Code, '0') as CLASS_CODE_ROOT, IsNull(Macchine.Class_Desc, '') as CLASS_DESC, IsNull(Centri_Aziendali.Sa_Nome, '') as Sa_Nome  ")
                    strSql.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ")
                    strSql.AppendLine(" Left OUTER JOIN Macchine ON Left(Parco_Macchine.Class_Code + '00', 2) = Macchine.Class_Code ")
                    strSql.AppendLine(" Left OUTER JOIN Centri_Aziendali ON (Centri_Aziendali.Piva = Parco_Macchine.Piva and  Centri_Aziendali.Sa_Cod = Parco_Macchine.Sa_Cod) ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" And    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))


                    If Mac_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   ")
                    End If

                    If Class_Code <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Class_Code LIKE '" & Agro_SQL_SaveText(Class_Code) & "%' ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return dt

    End Function

    Public Function LeggiMacchinexEsclusioneClassificazione(ByVal Mac_Cod_Origine As Integer,
                                                            ByVal Class_Code As String,
                                                            ByVal Piva_SuperUser_Origine As String,
                                                            ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiMacchinexEsclusioneClassificazione()"

        '====================================================================================
        'Parametri opzionali :
        '   Mac_Cod_Origine = 0  
        '   Piva_SuperUser_Origine = ""  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    'Nota: E' necessario l'outer join per ricavare il proprietario della macchina poichè
                    'in versione standalone l'impresa referente potrebbe non essere stata importata.

                    strSql.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                    strSql.AppendLine(" AND    Parco_Macchine.Cod_Contatto = ''  ")

                    If Mac_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   ")
                    End If

                    If Class_Code <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Class_Code NOT LIKE '" & Agro_SQL_SaveText(Class_Code) & "%' ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    'Nota: E' necessario l'outer join per ricavare il proprietario della macchina poichè
                    'in versione standalone l'impresa referente potrebbe non essere stata importata.

                    strSql.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                    strSql.AppendLine(" AND    Parco_Macchine.Cod_Contatto = ''  ")

                    If Mac_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   ")
                    End If

                    If Class_Code <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Class_Code NOT LIKE '" & Agro_SQL_SaveText(Class_Code) & "%' ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiMacchinaDaDescrizione(ByVal Descrizione As String,
                                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiMacchinaDaDescrizione()"

        '====================================================================================
        'Parametri opzionali :
        '   Descrizione = ""  
        '   Piva_SuperUser_Origine = ""  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    strSql.Length = 0
                    'Nota: E' necessario l'outer join per ricavare il proprietario della macchina poichè
                    'in versione standalone l'impresa referente potrebbe non essere stata importata.

                    strSql.AppendLine(" SELECT Parco_Macchine.* ")
                    strSql.AppendLine(" FROM   Parco_Macchine ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                    strSql.AppendLine(" AND    Parco_Macchine.Cod_Contatto = ''  ")

                    If Descrizione <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Des = '" & Agro_SQL_SaveText(Descrizione) & "'   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0
                    'Nota: E' necessario l'outer join per ricavare il proprietario della macchina poichè
                    'in versione standalone l'impresa referente potrebbe non essere stata importata.

                    strSql.AppendLine(" SELECT Parco_Macchine.* ")
                    strSql.AppendLine(" FROM   Parco_Macchine ")
                    strSql.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Parco_Macchine.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                    strSql.AppendLine(" AND    Parco_Macchine.Cod_Contatto = ''  ")

                    If Descrizione <> "" Then
                        strSql.AppendLine(" AND  Parco_Macchine.Mac_Des = '" & Agro_SQL_SaveText(Descrizione) & "'   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Parco_Macchine.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return dt

    End Function

#Region "Movimenti Macchina"

    Public Function LeggiMovimentiMacchina(ByVal Piva As String,
                                           ByVal Mac_Cod As Integer,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional checkAll As Boolean = False,
                                           Optional Validita_Inizio As Date = AGRODATAINIZIO,
                                           Optional Validita_Fine As Date = AGRODATAFINE) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.isMacchinaMovimentata()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try
            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0
            If checkAll Then
                strSql.AppendLine("SELECT Agenda.*")
            Else
                strSql.AppendLine("SELECT TOP 1 Agenda.*")
            End If
            strSql.AppendLine("FROM Agenda")
            strSql.AppendLine("    JOIN Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda")
            strSql.AppendLine("    JOIN Movimenti_dettagli ON Agenda.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov")
            strSql.AppendLine($"WHERE Movimenti.CAU_MOV = '{CAU_IMPUTAZIONE_PARCOMACCHINE}' ")
            strSql.AppendLine($"    AND Movimenti_Dettagli.Elem_Cod = {CInt(CostantiPersonalizzate.MACCHINE)} ")
            strSql.AppendLine($"    AND Movimenti_dettagli.Mat_Cod = {Agro_SQL_SaveNum(Mac_Cod)} ")

            'If Piva <> "" Then
            '    strSql.AppendLine(" AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            'End If

            If Validita_Inizio <> AGRODATAINIZIO OrElse Validita_Fine <> AGRODATAFINE Then
                strSql.AppendLine(" AND ( Movimenti.Data_Movimento > " & Agro_SQL_SaveDate(Validita_Fine, False) & " OR Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function CheckMacchinaRicette(ByVal Piva As String,
                                         ByVal Mac_Cod As Integer,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         Optional checkAll As Boolean = False,
                                         Optional Validita_Inizio As Date = AGRODATAINIZIO,
                                         Optional Validita_Fine As Date = AGRODATAFINE) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.CheckMacchinaRicette()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try
            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT Ricette.*")
            strSql.AppendLine(" FROM Ricette")
            strSql.AppendLine(" JOIN Ricette_Operazioni ON Ricette_Operazioni.Ricetta_Cod = Ricette.Ricetta_Cod")
            strSql.AppendLine(" JOIN Ricette_Dettagli ON  Ricette_Operazioni.Ricetta_Cod=Ricette_Dettagli.Ricetta_Cod AND Ricette_Operazioni.Ricetta_Operazione_Cod=Ricette_Dettagli.Ricetta_Operazione_Cod")
            strSql.AppendLine($" WHERE Ricette_Dettagli.CAU_MOV = '{CAU_IMPUTAZIONE_PARCOMACCHINE}'")
            strSql.AppendLine($" AND Ricette_Dettagli.Elem_Cod = {CInt(CostantiPersonalizzate.MACCHINE)}")
            strSql.AppendLine($" AND Ricette_Dettagli.Mat_Cod = {Agro_SQL_SaveNum(Mac_Cod)}")
            'strSql.AppendLine(String.Format("    AND Ricette.Piva = '{0}'", Agro_SQL_SaveText(Piva)))

            If Validita_Inizio <> AGRODATAINIZIO OrElse Validita_Fine <> AGRODATAFINE Then
                strSql.AppendLine(" AND ( Ricette_Operazioni.Validita_inizio > " & Agro_SQL_SaveDate(Validita_Fine, False) & " OR Ricette_Operazioni.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function RecipesCentres(ByVal Mac_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.HasRecipesOnlyWithinSameCentre()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try
            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("SELECT DISTINCT rdes.Sa_Cod")
            strSql.AppendLine("FROM Ricette r")
            strSql.AppendLine("    JOIN Ricette_Destinazioni rdes ON r.Ricetta_SuperUser = rdes.Ricetta_SuperUser")
            strSql.AppendLine("        AND r.Ricetta_Cod = rdes.Ricetta_Cod")
            strSql.AppendLine("WHERE EXISTS (")
            strSql.AppendLine("    SELECT *")
            strSql.AppendLine("    FROM Ricette_Dettagli rdet")
            strSql.AppendLine("    WHERE 1 = 1")
            strSql.AppendLine("        AND rdet.Ricetta_SuperUser = r.Ricetta_SuperUser")
            strSql.AppendLine("        AND rdet.Ricetta_Cod = r.Ricetta_Cod")
            strSql.AppendLine($"        AND rdet.CAU_MOV = '{CAU_IMPUTAZIONE_PARCOMACCHINE}' ")
            strSql.AppendLine($"        AND rdet.Elem_Cod = {CInt(CostantiPersonalizzate.MACCHINE)} ")
            strSql.AppendLine($"        AND rdet.Mat_Cod = {Agro_SQL_SaveNum(Mac_Cod)} ")
            strSql.AppendLine("    )")
            strSql.AppendLine("    AND rdes.Tipo_Destinazione = 0")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function CheckMacchinaCosti(ByVal Piva As String,
                                       ByVal Mac_Cod As Integer,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional checkAll As Boolean = False,
                                       Optional Validita_Inizio As Date = AGRODATAINIZIO,
                                       Optional Validita_Fine As Date = AGRODATAFINE) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.CheckMacchinaCosti()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try
            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            If checkAll Then
                strSql.AppendLine("SELECT Piva_SuperUser, Piva, Mac_Cod, Id_CDG, Id_CDG_Dettagli = 0")
            Else
                strSql.AppendLine("SELECT TOP 1 Piva_SuperUser, Piva, Mac_Cod, Id_CDG, Id_CDG_Dettagli = 0")
            End If
            strSql.AppendLine("FROM CDG_Testata")
            strSql.AppendLine(String.Format("where CDG_Testata.Piva_SuperUser = '{0}'", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            strSql.AppendLine(String.Format("AND CDG_Testata.Mac_Cod = {0}", Agro_SQL_SaveNum(Mac_Cod)))
            'strSql.AppendLine(String.Format("AND CDG_Testata.Piva = '{0}'", Agro_SQL_SaveText(Piva)))

            strSql.AppendLine("UNION")

            If checkAll Then
                strSql.AppendLine("SELECT Piva_SuperUser, Piva, Macchine_Cod AS Mac_Cod, Id_CDG, Id_CDG_Dettagli")
            Else
                strSql.AppendLine("SELECT TOP 1 Piva_SuperUser, Piva, Macchine_Cod AS Mac_Cod, Id_CDG, Id_CDG_Dettagli")
            End If
            strSql.AppendLine("FROM CDG_Dettagli")
            strSql.AppendLine($"where CDG_Dettagli.Piva_SuperUser = '{Agro_SQL_SaveText(objParametri.PivaSuperUser)}'")
            strSql.AppendLine($"AND CDG_Dettagli.Macchine_Cod = {Agro_SQL_SaveNum(Mac_Cod)}")
            'strSql.AppendLine(String.Format("AND CDG_Dettagli.Piva = '{0}'", Agro_SQL_SaveText(Piva)))

            If Validita_Inizio <> AGRODATAINIZIO OrElse Validita_Fine <> AGRODATAFINE Then
                strSql.AppendLine(" AND ( CDG_Dettagli.Validita_Inizio > " & Agro_SQL_SaveDate(Validita_Fine, False) & " ) ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function HasCostsOnlyByOwner(ByVal mac As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.CheckMacchinaCosti()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try
            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("SELECT TOP 1 Piva_SuperUser, Piva, Mac_Cod, Id_CDG, Id_CDG_Dettagli = 0")
            strSql.AppendLine("FROM CDG_Testata")
            strSql.AppendLine(String.Format("where CDG_Testata.Piva_SuperUser = '{0}'", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            strSql.AppendLine(String.Format("AND CDG_Testata.Mac_Cod = {0}", Agro_SQL_SaveNum(mac.codice)))
            strSql.AppendLine(String.Format("AND CDG_Testata.Piva <> '{0}'", Agro_SQL_SaveText(mac.partitaIva)))

            strSql.AppendLine("UNION")

            strSql.AppendLine("SELECT TOP 1 Piva_SuperUser, Piva, Macchine_Cod AS Mac_Cod, Id_CDG, Id_CDG_Dettagli")
            strSql.AppendLine("FROM CDG_Dettagli")
            strSql.AppendLine(String.Format("where CDG_Dettagli.Piva_SuperUser = '{0}'", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            strSql.AppendLine(String.Format("AND CDG_Dettagli.Macchine_Cod = {0}", Agro_SQL_SaveNum(mac.codice)))
            strSql.AppendLine(String.Format("AND CDG_Dettagli.Piva <> '{0}'", Agro_SQL_SaveText(mac.partitaIva)))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt.Rows.Count = 0
    End Function

#End Region

    Public Function LeggiAssociazioneBTM(ByVal Mac_Cod As Integer, ByVal VIN As String, ByVal BTM_Serial As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.LeggiAssociazioneBTM()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            strSql.AppendLine(" SELECT Mac_Cod, VIN, BTM_Serial  ")
            strSql.AppendLine(" FROM Parco_Macchine ")
            strSql.AppendLine(" WHERE 1=1 ")

            If Mac_Cod <> 0 Then
                strSql.AppendLine(" AND Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            End If

            If VIN <> "" Then
                strSql.AppendLine(" AND VIN = '" & Agro_SQL_SaveText(VIN) & "' ")
            End If

            If BTM_Serial <> "" Then
                strSql.AppendLine(" AND BTM_Serial = '" & Agro_SQL_SaveText(BTM_Serial) & "'   ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function CaricaListaMacchine(operazioni As List(Of AgronicaCoreModelsSTD.attivita.Attivita_xModificaMultipla),
                                        Solo_Aziendali As Boolean,
                                        objParametri_Server As AgronicaCoreParametri)

        Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim DT As DataTable

        Dim Piva As String = ""
        Dim PivaSingola As Boolean = True
        Dim MostraPubblici As Boolean = True

        If operazioni.Select(Function(a) a.Piva).Distinct().Count() = 1 Then
            Piva = operazioni.ElementAt(0).Piva
        Else
            Piva = "----"
            PivaSingola = False
        End If

        If PivaSingola Then
            If Solo_Aziendali Then
                MostraPubblici = False
            Else
                MostraPubblici = True
            End If
        Else
            MostraPubblici = True
        End If

        DT = objMacchine.ParcoMacchine_Leggi(Piva, 0, MostraPubblici,
                                             "", "", "", "", "", 0, "", True, 0, "", True, AGRODATAINIZIO, AGRODATAFINE,
                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "", " Mac_Des", objParametri_Server)


        Dim Dt_Macchine As New DataTable
        ''----- Definisco la struttura del DataTable
        ''1
        Dt_Macchine.Columns.Add(New DataColumn("Piva", GetType(String)))
        ''2
        Dt_Macchine.Columns.Add(New DataColumn("sa_cod", GetType(String)))
        ''3
        Dt_Macchine.Columns.Add(New DataColumn("Mac_Cod", GetType(String)))
        ''4
        Dt_Macchine.Columns.Add(New DataColumn("CLASS_DESC", GetType(String)))
        ''5
        Dt_Macchine.Columns.Add(New DataColumn("Mac_Des", GetType(String)))
        ''6
        Dt_Macchine.Columns.Add(New DataColumn("Modello", GetType(String)))
        ''7
        Dt_Macchine.Columns.Add(New DataColumn("Targa", GetType(String)))
        ''8
        Dt_Macchine.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
        ''9
        Dt_Macchine.Columns.Add(New DataColumn("Unita_Misura", GetType(String)))
        ''10
        Dt_Macchine.Columns.Add(New DataColumn("Impresa", GetType(String)))
        '11
        Dt_Macchine.Columns.Add(New DataColumn("Costo_Inizio", GetType(String)))
        '12
        Dt_Macchine.Columns.Add(New DataColumn("Costo_Fine", GetType(String)))


        Dim DR As DataRow
        Dim Costo_Inizio, Costo_Fine As String

        For i = 0 To DT.Rows.Count - 1
            DR = Dt_Macchine.NewRow()
            DR.Item("Piva") = DT.Rows(i).Item("Piva")
            DR.Item("sa_cod") = DT.Rows(i).Item("sa_cod")
            DR.Item("Mac_Cod") = DT.Rows(i).Item("Mac_Cod")
            DR.Item("CLASS_DESC") = DT.Rows(i).Item("CLASS_DESC")
            DR.Item("Mac_Des") = DT.Rows(i).Item("Mac_Des")
            DR.Item("Modello") = DT.Rows(i).Item("Modello")

            If DT.Rows(i).Item("Mezzo") = 1 Then
                DR.Item("Unita_Misura") = "Ha"
            ElseIf DT.Rows(i).Item("Mezzo") = 2 Then
                DR.Item("Unita_Misura") = "Ora"
            End If

            If DT.Rows(i).Item("Elem_Cod") = "1" Then
                DR.Item("Prezzo_Unitario") = DT.Rows(i).Item("Prezzo_Unitario")

                Costo_Inizio = DT.Rows(i).Item("Costo_Inizio")
                If Costo_Inizio.Length > 0 Then
                    Costo_Inizio = Costo_Inizio.Substring(0, 10)
                    If Costo_Inizio = "01/01/1900" Then
                        Costo_Inizio = "---"
                    End If
                Else
                    Costo_Inizio = "---"
                End If
                DR.Item("Costo_Inizio") = Costo_Inizio

                Costo_Fine = DT.Rows(i).Item("Costo_Fine")
                If Costo_Fine.Length > 0 Then
                    Costo_Fine = Costo_Fine.Substring(0, 10)
                    If Costo_Fine = "31/12/2100" Then
                        Costo_Fine = "---"
                    End If
                Else
                    Costo_Fine = "---"
                End If
                DR.Item("Costo_Fine") = Costo_Fine
            Else
                DR.Item("Prezzo_Unitario") = "0"
                Costo_Inizio = "---"
                Costo_Fine = "---"
                DR.Item("Costo_Fine") = Costo_Fine
                DR.Item("Costo_Inizio") = Costo_Inizio
            End If

            DR.Item("Impresa") = DT.Rows(i).Item("Impresa")
            DR.Item("Targa") = DT.Rows(i).Item("Targa")

            Dt_Macchine.Rows.Add(DR)
        Next

        Return Dt_Macchine
    End Function

    Public Function Leggi_Possesso_x_Targa(ByVal targhe As List(Of String),
                                           ByVal piva_escludi As String,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi_Possesso_x_Targa()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT")
            strSql.AppendLine("         REPLACE(Targa,' ','') Targa")
            strSql.AppendLine("         , ISNULL(STUFF ((SELECT DISTINCT")
            strSql.AppendLine("                             ', ' + i.rag_soc + ISNULL(' (' + ic.val_cod + ')' , '') + ' Titolo Possesso: ' + ")
            strSql.AppendLine("                             CASE pm.TitoloPossesso ")
            strSql.AppendLine("                                 WHEN 0 THEN 'Altro' ")
            strSql.AppendLine("                                 WHEN 1 THEN 'Proprietà' ")
            strSql.AppendLine("                                 WHEN 2 THEN 'Comodato d''uso' ")
            strSql.AppendLine("                                 WHEN 3 THEN 'Affitto con contratto' ")
            strSql.AppendLine("                                 WHEN 4 THEN 'Affitto senza contratto' ")
            strSql.AppendLine("                                 WHEN 5 THEN 'In conto terzi' ")
            strSql.AppendLine("                                 WHEN 6 THEN 'In convenzione' ")
            strSql.AppendLine("                                 WHEN 7 THEN 'In compartecipazione' ")
            strSql.AppendLine("                                 ELSE 'Non Definito' ")
            strSql.AppendLine("                             END + ")
            strSql.AppendLine("                             ' dal ' + CONVERT(VARCHAR, pm.validita_inizio, 103) + ' al ' + CONVERT(VARCHAR, pm.validita_fine, 103)")
            strSql.AppendLine("                          FROM Parco_Macchine pm")
            strSql.AppendLine("                          INNER JOIN Imprese i ON i.piva = pm.PIVA ")
            strSql.AppendLine("                          LEFT JOIN Imprese_Codici ic ON  ic.PIVA = i.piva AND id_cod = 1010 ")
            strSql.AppendLine("                          WHERE REPLACE(Targa,' ','') = REPLACE(p.Targa,' ','') ")
            strSql.AppendLine("                          AND pm.Piva <> '" & Agro_SQL_SaveText(piva_escludi) & "' ")
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("                          AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            strSql.AppendLine("                          FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 2, '') , '') AS Possessi ")
            strSql.AppendLine(" FROM Parco_Macchine p")
            strSql.AppendLine(" WHERE 1 = 1 ")

            If targhe IsNot Nothing AndAlso targhe.Count > 0 Then
                strSql.AppendLine(" AND REPLACE(p.Targa,' ','') IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", targhe), True) & ") ")
            End If

            strSql.AppendLine(" AND p.Piva <> '" & Agro_SQL_SaveText(piva_escludi) & "' ")

            strSql.AppendLine(" GROUP BY p.Targa ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_RateiTempo_Macchina(ByVal Mac_Cod As Integer,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi_Possesso_x_Targa()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT rt.* ")
            strSql.AppendLine(" FROM RateiTempo rt ")
            strSql.AppendLine(" INNER JOIN Parco_MacchinexRateiTempo xrt ON rt.Rateo_Cod = xrt.Rateo_Cod ")
            strSql.AppendLine(" WHERE xrt.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Leggi_RateiTempo(ByVal rateoTempo As AgronicaCoreModelsSTD.anagrafiche.RateoTempo,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_R.Leggi_RateiTempo()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM RateiTempo ")
            strSql.AppendLine(" WHERE 1 = 1 ")

            'se RateoTempo è valorizzato, potrebbe già esistere nella tabella e mi serve il Rateo_Cod
            If rateoTempo IsNot Nothing Then

                If rateoTempo.DataInizio.HasValue Then
                    strSql.AppendLine(" AND DAY(DataInizio) = DAY(" & Agro_SQL_SaveDateTime(rateoTempo.DataInizio) & ") AND MONTH(DataInizio) = MONTH(" & Agro_SQL_SaveDateTime(rateoTempo.DataInizio) & ") ")
                End If

                If rateoTempo.DataFine.HasValue Then
                    strSql.AppendLine(" AND DAY(DataFine) = DAY(" & Agro_SQL_SaveDateTime(rateoTempo.DataFine) & ") AND MONTH(DataFine) = MONTH(" & Agro_SQL_SaveDateTime(rateoTempo.DataFine) & ") ")
                End If

                If rateoTempo.OraInizio.HasValue Then
                    strSql.AppendLine(" AND DATEPART(HOUR, OraInizio) = DATEPART(HOUR, " & Agro_SQL_SaveDateTime(rateoTempo.OraInizio) & ") AND DATEPART(MINUTE, OraInizio) = DATEPART(MINUTE, " & Agro_SQL_SaveDateTime(rateoTempo.OraInizio) & ") ")
                End If

                If rateoTempo.OraFine.HasValue Then
                    strSql.AppendLine(" AND DATEPART(HOUR, OraFine) = DATEPART(HOUR, " & Agro_SQL_SaveDateTime(rateoTempo.OraFine) & ") AND DATEPART(MINUTE, OraFine) = DATEPART(MINUTE, " & Agro_SQL_SaveDateTime(rateoTempo.OraFine) & ") ")
                End If

                If rateoTempo.Rotazione <> 0 Then
                    strSql.AppendLine(" AND Rotazione = " & Agro_SQL_SaveNum(rateoTempo.Rotazione) & " ")
                End If

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Parco_Macchine_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Aggiorna_Parco_Macchine(ByRef EFArrayToInsert As ArrayList,
                                            ByRef EFArrayToUpdate As ArrayList,
                                            ByRef EFArrayToDelete As ArrayList,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As String

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreParco_Macchine.Aggiorna_Parco_Macchine()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curParco_Macchine As Parco_Macchine In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                'Richiedo un nuovo id sequenza
                                idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "Parco_Macchine", 0, 2000000000, objParametri)
                                curParco_Macchine.Mac_Cod = idSeq

                                GiasContext.Parco_Macchine.Add(curParco_Macchine)
                                GiasContext.SaveChanges()
                                success = True

                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each listParco_Macchine As Parco_Macchine In EFArrayToUpdate
                            GiasContext.Parco_Macchine.Attach(listParco_Macchine)
                            GiasContext.Entry(listParco_Macchine).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listlistParco_Macchine As Parco_Macchine In EFArrayToDelete
                            GiasContext.Parco_Macchine.Attach(listlistParco_Macchine)
                            GiasContext.Parco_Macchine.Remove(listlistParco_Macchine)
                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using
            End Using


        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return messaggioErrore

    End Function


    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Mac_Cod As Integer,
                           ByVal Cod_Contatto As String,
                           ByVal Class_Code As String,
                           ByVal Tipo As Integer,
                           ByVal Mac_Des As String,
                           ByVal Costo_Acquisto As Decimal,
                           ByVal Targa As String,
                           ByVal Telaio As String,
                           ByVal Ditta_Cod As Integer,
                           ByVal Modello As String,
                           ByVal Potenza As String,
                           ByVal Ammortamento As Decimal,
                           ByVal Data_Immatricolazione As Date,
                           ByVal Ultima_Manutenzione As Date,
                           ByVal Ultima_Revisione As Date,
                           ByVal Stato_Utilizzo As String,
                           ByVal Note As String,
                           ByVal N_Immatricolazione As String,
                           ByVal N_Immatricolazione_Rimorchio As String,
                           ByVal N_Autorizzazione_Trasporto As String,
                           ByVal Data_Rilascio_Autorizzazione As Date,
                           ByVal Peso As Decimal,
                           ByVal Portata_Max As Decimal,
                           ByVal ChkDefault As Integer,
                           ByVal Alimentazione_Cod As Integer,
                           ByVal Potenza_Udm_Cod As Integer,
                           ByVal Mac_Cod_Origine As Integer,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal CUAA_Proprietario As String,
                           ByVal Denominazione_Proprietario As String,
                           ByVal Tipo_Targa_Cod As Integer,
                           ByVal Tipo_Trazione_Cod As Integer,
                           ByVal N_Omologazione As String,
                           ByVal Ditta_Cod_Motore As Integer,
                           ByVal Tipo_Motore As String,
                           ByVal Matricola_Motore As String,
                           ByVal Data_Reimmatricolazione As Date,
                           ByVal Data_Carico As Date,
                           ByVal Data_Scarico As Date,
                           ByVal TitoloPossesso As Integer,
                           ByVal Flag_Attrezzatura_Macchina As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Taratura_Ugello As Decimal = 0,
                           Optional ByVal Data_Taratura_Inizio As Date = AGRODATAINIZIO,
                           Optional ByVal Data_Taratura_Fine As Date = AGRODATAFINE,
                           Optional ByVal Visibile_ctrl_gestione As Integer = 1,
                           Optional ByVal Codice As String = ""
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_W.Scrivi()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" INSERT INTO Parco_Macchine ")
            strSql.AppendLine("             ( Piva, Sa_Cod, Mac_Cod, Cod_Contatto, Class_Code, Tipo, Mac_Des, ")
            strSql.AppendLine("               Costo_Acquisto, Targa, Telaio, Ditta_Cod, ")
            strSql.AppendLine("               Modello, Potenza, Ammortamento, Data_Immatricolazione, ")
            strSql.AppendLine("               Ultima_Manutenzione, Ultima_Revisione, Stato_Utilizzo, Note,  ")
            strSql.AppendLine("               N_Immatricolazione, N_Immatricolazione_Rimorchio, N_Autorizzazione_Trasporto, ")
            strSql.AppendLine("               Data_Rilascio_Autorizzazione, Peso, Portata_Max, ChkDefault,  ")
            strSql.AppendLine("               Alimentazione_Cod, Potenza_Udm_Cod,  ")
            strSql.AppendLine("               Mac_Cod_Origine,    Piva_SuperUser_Origine, ")

            strSql.AppendLine("               CUAA_Proprietario, ")
            strSql.AppendLine("               Denominazione_Proprietario, ")
            strSql.AppendLine("               Tipo_Targa_Cod, ")
            strSql.AppendLine("               Tipo_Trazione_Cod, ")
            strSql.AppendLine("               N_Omologazione, ")
            strSql.AppendLine("               Ditta_Cod_Motore, ")
            strSql.AppendLine("               Tipo_Motore, ")
            strSql.AppendLine("               Matricola_Motore, ")
            strSql.AppendLine("               Data_Reimmatricolazione, ")
            strSql.AppendLine("               Data_Carico, ")
            strSql.AppendLine("               Data_Scarico, ")
            strSql.AppendLine("               TitoloPossesso, ")
            strSql.AppendLine("               Flag_Attrezzatura_Macchina, ")

            strSql.AppendLine("               Inviato,            DataInvio, ")
            strSql.AppendLine("               Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("               UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("               Validita_Inizio,    Validita_Fine, ")
            strSql.AppendLine("               Taratura_Ugello, Validita_Taratura_Inizio, Validita_Taratura_Fine, ")
            strSql.AppendLine("               Visibile_ctrl_gestione, Codice ")
            strSql.AppendLine("               ) ")

            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mac_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cod_Contatto) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Class_Code) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Mac_Des) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Costo_Acquisto) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Targa) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Telaio) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Modello) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Potenza) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ammortamento) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Immatricolazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Ultima_Manutenzione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Ultima_Revisione) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Stato_Utilizzo) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(N_Immatricolazione) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(N_Immatricolazione_Rimorchio) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(N_Autorizzazione_Trasporto) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Rilascio_Autorizzazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Peso) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Portata_Max) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkDefault) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Alimentazione_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Potenza_Udm_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "' ")

            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(CUAA_Proprietario) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Denominazione_Proprietario) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Targa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Trazione_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(N_Omologazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ditta_Cod_Motore) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Tipo_Motore) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Matricola_Motore) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Reimmatricolazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Carico) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Scarico) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(TitoloPossesso) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Flag_Attrezzatura_Macchina) & "' ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Taratura_Ugello) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Taratura_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Taratura_Fine) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Visibile_ctrl_gestione) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Codice) & "' ")
            strSql.AppendLine(") ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '============================================================================
    Public Function Modifica_x_Smart(ByVal Mac_Cod As Integer,
                                     ByVal Class_Code As String,
                                     ByVal Tipo As Integer,
                                     ByVal Mac_Des As String,
                                     ByVal Ditta_Cod As Integer,
                                     ByVal Modello As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Validita_Fine As Date,
                                     ByVal Stato_Utilizzo As String,
                                     ByVal Data_Ultima_Manutenzione As Date
                                     ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Mac_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mac_Cod obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            'Query per l'inserimento dei dati                 ' #### CLASSE ####
            strSql.AppendLine(" UPDATE Parco_Macchine SET ")
            strSql.AppendLine("   Class_Code               = '" & Agro_SQL_SaveText(Class_Code) & "'  ")
            strSql.AppendLine("   ,Tipo                     =  " & Agro_SQL_SaveNum(Tipo) & "  ")
            strSql.AppendLine("   ,Mac_Des                  = '" & Agro_SQL_SaveText(Mac_Des) & "'  ")
            strSql.AppendLine("   ,Ditta_Cod                =  " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.AppendLine("   ,Modello                  = '" & Agro_SQL_SaveText(Modello) & "'  ")

            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine("   ,Stato_Utilizzo    = '" & Agro_SQL_SaveText(Stato_Utilizzo) & "'  ")

            strSql.AppendLine("   ,Sa_Cod                     =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

            strSql.AppendLine("   ,Ultima_Manutenzione     =  " & Agro_SQL_SaveDateTime(Data_Ultima_Manutenzione))

            strSql.AppendLine(" WHERE   Mac_Cod    =  " & Agro_SQL_SaveNum(Mac_Cod) & "   ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function

    Public Function Modifica_DataTaratura(ByVal Mac_Cod As Integer,
                                          ByVal Ultima_Revisione As Date,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        'ByVal Piva As String, ByVal Sa_Cod As Integer,

        Dim nomeroutine As String = "Modifica_DataTaratura"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Mac_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mac_Cod obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            'Query per l'inserimento dei dati                 ' #### CLASSE ####
            strSql.AppendLine(" UPDATE Parco_Macchine SET ")
            strSql.AppendLine("   Ultima_Revisione         =  " & Agro_SQL_SaveDate(Ultima_Revisione) & "  ")


            strSql.AppendLine(" WHERE   Mac_Cod    =  " & Agro_SQL_SaveNum(Mac_Cod) & "   ")

            'StrSQL.AppendLine(" and   piva    =  " & Agro_SQL_SaveText(Piva) & "   ")
            'StrSQL.AppendLine(" and   sa_cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            'StrSQL.AppendLine(" and   Class_Code    =  '06.02.01'   ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeroutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeroutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeroutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica_Data_Ultima_Manutenzione(ByVal Mac_Cod As Integer,
                                                      ByVal Ultima_Manutenzione As Date,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                      ) As Boolean

        'ByVal Piva As String, ByVal Sa_Cod As Integer, 


        Dim nomeroutine As String = "Modifica_Data_Ultima_Manutenzione"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Mac_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mac_Cod obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            'Query per l'inserimento dei dati                 ' #### CLASSE ####
            strSql.AppendLine(" UPDATE Parco_Macchine SET ")
            strSql.AppendLine("   Ultima_Manutenzione         =  " & Agro_SQL_SaveDate(Ultima_Manutenzione) & "  ")


            strSql.AppendLine(" WHERE   Mac_Cod    =  " & Agro_SQL_SaveNum(Mac_Cod) & "   ")

            'StrSQL.AppendLine(" and   piva    =  " & Agro_SQL_SaveText(Piva) & "   ")
            'StrSQL.AppendLine(" and   sa_cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            'StrSQL.AppendLine(" and   Class_Code    =  '06.02.01'   ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeroutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeroutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeroutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica(ByVal New_Piva As String,
                             ByVal New_Sa_Cod As Integer,
                             ByVal Mac_Cod As Integer,
                             ByVal Cod_Contatto As String,
                             ByVal Class_Code As String,
                             ByVal Tipo As Integer,
                             ByVal Mac_Des As String,
                             ByVal Costo_Acquisto As Decimal,
                             ByVal Targa As String,
                             ByVal Telaio As String,
                             ByVal Ditta_Cod As Integer,
                             ByVal Modello As String,
                             ByVal Potenza As String,
                             ByVal Ammortamento As Decimal,
                             ByVal Data_Immatricolazione As Date,
                             ByVal Ultima_Manutenzione As Date,
                             ByVal Ultima_Revisione As Date,
                             ByVal Stato_Utilizzo As String,
                             ByVal Note As String,
                             ByVal N_Immatricolazione As String,
                             ByVal N_Immatricolazione_Rimorchio As String,
                             ByVal N_Autorizzazione_Trasporto As String,
                             ByVal Data_Rilascio_Autorizzazione As Date,
                             ByVal Peso As Decimal,
                             ByVal Portata_Max As Decimal,
                             ByVal ChkDefault As Integer,
                             ByVal Alimentazione_Cod As Integer,
                             ByVal Potenza_Udm_Cod As Integer,
                             ByVal CUAA_Proprietario As String,
                             ByVal Denominazione_Proprietario As String,
                             ByVal Tipo_Targa_Cod As Integer,
                             ByVal Tipo_Trazione_Cod As Integer,
                             ByVal N_Omologazione As String,
                             ByVal Ditta_Cod_Motore As Integer,
                             ByVal Tipo_Motore As String,
                             ByVal Matricola_Motore As String,
                             ByVal Data_Reimmatricolazione As Date,
                             ByVal Data_Carico As Date,
                             ByVal Data_Scarico As Date,
                             ByVal TitoloPossesso As Integer,
                             ByVal Flag_Attrezzatura_Macchina As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Taratura_Ugello As Decimal = 0,
                             Optional ByVal Data_Taratura_Inizio As Date = AGRODATAINIZIO,
                             Optional ByVal Data_Taratura_Fine As Date = AGRODATAFINE,
                             Optional ByVal Visibile_ctrl_gestione As Integer = 0,
                             Optional ByVal Codice As String = ""
                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_W.Modifica()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Mac_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mac_Cod obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            'Query per l'inserimento dei dati                 ' #### CLASSE ####
            strSql.AppendLine(" UPDATE Parco_Macchine SET ")
            strSql.AppendLine("    Piva                     = '" & Agro_SQL_SaveText(New_Piva) & "'  ")
            strSql.AppendLine("   ,Sa_Cod                   =  " & Agro_SQL_SaveNum(New_Sa_Cod) & "  ")
            strSql.AppendLine("   ,Cod_Contatto             = '" & Agro_SQL_SaveText(Cod_Contatto) & "'  ")
            strSql.AppendLine("   ,Class_Code               = '" & Agro_SQL_SaveText(Class_Code) & "'  ")
            strSql.AppendLine("   ,Tipo                     =  " & Agro_SQL_SaveNum(Tipo) & "  ")
            strSql.AppendLine("   ,Mac_Des                  = '" & Agro_SQL_SaveText(Mac_Des) & "'  ")
            strSql.AppendLine("   ,Costo_Acquisto           =  " & Agro_SQL_SaveNum(Costo_Acquisto) & "  ")
            strSql.AppendLine("   ,Targa                    = '" & Agro_SQL_SaveText(Targa) & "'  ")
            strSql.AppendLine("   ,Telaio                   = '" & Agro_SQL_SaveText(Telaio) & "'  ")
            strSql.AppendLine("   ,Ditta_Cod                =  " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.AppendLine("   ,Modello                  = '" & Agro_SQL_SaveText(Modello) & "'  ")
            strSql.AppendLine("   ,Potenza                  = '" & Agro_SQL_SaveText(Potenza) & "'  ")
            strSql.AppendLine("   ,Ammortamento             =  " & Agro_SQL_SaveNum(Ammortamento) & "  ")
            strSql.AppendLine("   ,Data_Immatricolazione    =  " & Agro_SQL_SaveDate(Data_Immatricolazione) & "  ")
            strSql.AppendLine("   ,Ultima_Manutenzione      =  " & Agro_SQL_SaveDate(Ultima_Manutenzione) & "  ")
            strSql.AppendLine("   ,Ultima_Revisione         =  " & Agro_SQL_SaveDate(Ultima_Revisione) & "  ")
            strSql.AppendLine("   ,Stato_Utilizzo           = '" & Agro_SQL_SaveText(Stato_Utilizzo) & "'  ")
            strSql.AppendLine("   ,Note                     = '" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.AppendLine("   ,N_Immatricolazione           = '" & Agro_SQL_SaveText(N_Immatricolazione) & "'  ")
            strSql.AppendLine("   ,N_Immatricolazione_Rimorchio = '" & Agro_SQL_SaveText(N_Immatricolazione_Rimorchio) & "'  ")
            strSql.AppendLine("   ,N_Autorizzazione_Trasporto   = '" & Agro_SQL_SaveText(N_Autorizzazione_Trasporto) & "'  ")
            strSql.AppendLine("   ,Data_Rilascio_Autorizzazione =  " & Agro_SQL_SaveDate(Data_Rilascio_Autorizzazione) & "  ")
            strSql.AppendLine("   ,Peso                         =  " & Agro_SQL_SaveNum(Peso) & "  ")
            strSql.AppendLine("   ,Portata_Max                  =  " & Agro_SQL_SaveNum(Portata_Max) & "  ")
            strSql.AppendLine("   ,ChkDefault                   =  " & Agro_SQL_SaveNum(ChkDefault) & "  ")
            strSql.AppendLine("   ,Alimentazione_Cod            =  " & Agro_SQL_SaveNum(Alimentazione_Cod) & "  ")
            strSql.AppendLine("   ,Potenza_Udm_Cod              =  " & Agro_SQL_SaveNum(Potenza_Udm_Cod) & "  ")

            strSql.AppendLine("   ,CUAA_Proprietario            = '" & Agro_SQL_SaveText(CUAA_Proprietario) & "'  ")
            strSql.AppendLine("   ,Denominazione_Proprietario   = '" & Agro_SQL_SaveText(Denominazione_Proprietario) & "'  ")
            strSql.AppendLine("   ,Tipo_Targa_Cod               =  " & Agro_SQL_SaveNum(Tipo_Targa_Cod) & "  ")
            strSql.AppendLine("   ,Tipo_Trazione_Cod            =  " & Agro_SQL_SaveNum(Tipo_Trazione_Cod) & "  ")
            strSql.AppendLine("   ,N_Omologazione               = '" & Agro_SQL_SaveText(N_Omologazione) & "'  ")
            strSql.AppendLine("   ,Ditta_Cod_Motore             =  " & Agro_SQL_SaveNum(Ditta_Cod_Motore) & "  ")
            strSql.AppendLine("   ,Tipo_Motore                  = '" & Agro_SQL_SaveText(Tipo_Motore) & "'  ")
            strSql.AppendLine("   ,Matricola_Motore             = '" & Agro_SQL_SaveText(Matricola_Motore) & "'  ")
            strSql.AppendLine("   ,Data_Reimmatricolazione      =  " & Agro_SQL_SaveDate(Data_Reimmatricolazione) & "  ")
            strSql.AppendLine("   ,Data_Carico                  =  " & Agro_SQL_SaveDate(Data_Carico) & "  ")
            strSql.AppendLine("   ,Data_Scarico                 =  " & Agro_SQL_SaveDate(Data_Scarico) & "  ")
            strSql.AppendLine("   ,TitoloPossesso               =  " & Agro_SQL_SaveNum(TitoloPossesso) & "  ")
            strSql.AppendLine("   ,Flag_Attrezzatura_Macchina   = '" & Agro_SQL_SaveText(Flag_Attrezzatura_Macchina) & "'  ")
            strSql.AppendLine("   ,Taratura_Ugello              =  " & Agro_SQL_SaveNum(Taratura_Ugello) & "  ")
            strSql.AppendLine("   ,Validita_Taratura_Inizio     =  " & Agro_SQL_SaveDate(Data_Taratura_Inizio) & "  ")
            strSql.AppendLine("   ,Validita_Taratura_Fine       =  " & Agro_SQL_SaveDate(Data_Taratura_Fine) & "  ")

            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine("   ,Visibile_ctrl_gestione =  " & Agro_SQL_SaveNum(Visibile_ctrl_gestione))
            strSql.AppendLine("   ,Codice = '" & Agro_SQL_SaveText(Codice) & "'")

            strSql.AppendLine(" WHERE   Mac_Cod    =  " & Agro_SQL_SaveNum(Mac_Cod) & "   ")


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function


    '============================================================================
    'La nuova versione di questa funzione cancella solo la macchina
    'E' NECESSARIO ricordarsi di cancellare anche l'operazione di agenda correlata
    '============================================================================
    Public Function Cancella(ByVal Piva As String,
                             ByVal Mac_Cod As Integer,
                             ByVal Cod_Contatto As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_W.Cancella()"

        'Nota: La cancellazione di un'impresa comporterebbe la cancellazione del suo
        'parco macchine. Controllare da codice che la macchina/attrezzatura 
        'non sia stata impiegata da altri prima di cancellarla!!.

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Mac_Cod = 0
        '   Cod_Contatto = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False





        Try

            '
            '
            '
            '
            '
            ' MANCA IL PEZZO CHE CANCELLA 
            '   L'OPERAZIONE DI AGENDA CORRELATA
            '
            '
            '
            '
            '
            '




            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Parco_Macchine ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")
            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Parco_Macchine ")
                strSql.AppendLine(" WHERE  1=1 ")
            End If

            If Piva <> String.Empty Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Mac_Cod <> 0 Then
                strSql.AppendLine(" AND Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & "   ")
            End If

            If Cod_Contatto <> "" Then
                strSql.AppendLine(" AND  Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "'  ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ScriviAssociazioneBTM(ByVal Mac_Cod As Integer, ByVal VIN As String, ByVal BTM_Serial As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_W.ScriviAssociazioneBTM()"

        Dim xRisp As Boolean = False
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            strSql.AppendLine(" UPDATE Parco_Macchine SET ")
            strSql.AppendLine(" VIN = '" & Agro_SQL_SaveText(VIN) & "' ")
            strSql.AppendLine(",BTM_Serial = '" & Agro_SQL_SaveText(BTM_Serial) & "' ")
            strSql.AppendLine(",Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            strSql.AppendLine(",Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine(" WHERE  Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Scrivi_RateoTempo(ByVal rateoTempo As AgronicaCoreModelsSTD.anagrafiche.RateoTempo,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_W.Scrivi_RateoTempo()"

        Dim xRisp As Boolean = False
        Dim idRateo As Integer = 0
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try
            If rateoTempo IsNot Nothing Then
                Dim seq As New Agro_Sequenze()
                rateoTempo.Rateo_Cod = seq.NuovoId_Tabella("RateiTempo", 0, 2000000000, objParametri)

                strSql.AppendLine(" INSERT INTO RateiTempo(Rateo_Cod, DataInizio, DataFine, OraInizio, OraFine, Rotazione, inviato, datainvio, Data_Creazione, ")
                strSql.AppendLine("  Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")
                strSql.AppendLine(" VALUES(")
                strSql.AppendLine("  " & Agro_SQL_SaveNum(rateoTempo.Rateo_Cod) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(rateoTempo.DataInizio) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(rateoTempo.DataFine) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(rateoTempo.OraInizio) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(rateoTempo.OraFine) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveNum(rateoTempo.Rotazione) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveNum(0) & " ")
                strSql.AppendLine(" , Null ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine(" , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine(" , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(rateoTempo.ValiditaInizio) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(rateoTempo.ValiditaFine) & " ")
                strSql.AppendLine(") ")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

                If xRisp Then
                    idRateo = rateoTempo.Rateo_Cod
                End If

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idRateo

    End Function

    Public Function Scrivi_RateiTempo_Macchina(ByVal rateoCod As Integer,
                                               ByVal macCod As Integer,
                                               ByVal ValiditaInizio As DateTime,
                                               ByVal ValiditaFine As DateTime,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_W.Scrivi_RateiTempo_Macchina()"

        Dim xRisp As Boolean = False
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            If rateoCod <> 0 AndAlso macCod <> 0 Then

                strSql.AppendLine(" INSERT INTO Parco_MacchinexRateiTempo(Mac_Cod, Rateo_Cod, inviato, datainvio, Data_Creazione, ")
                strSql.AppendLine("  Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")
                strSql.AppendLine(" VALUES(")
                strSql.AppendLine("  " & Agro_SQL_SaveNum(macCod) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveNum(rateoCod) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveNum(0) & " ")
                strSql.AppendLine(" , Null ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine(" , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine(" , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(ValiditaInizio) & " ")
                strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(ValiditaFine) & " ")
                strSql.AppendLine(") ")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella_RateiTempo_Macchina(ByVal macCod As Integer,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_Macchine_W.Cancella_RateiTempo_Macchina()"

        Dim xRisp As Boolean = False
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            If macCod <> 0 Then

                strSql.AppendLine(" DELETE FROM Parco_MacchinexRateiTempo ")
                strSql.AppendLine(" WHERE Mac_Cod = " & Agro_SQL_SaveNum(macCod) & " ")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
