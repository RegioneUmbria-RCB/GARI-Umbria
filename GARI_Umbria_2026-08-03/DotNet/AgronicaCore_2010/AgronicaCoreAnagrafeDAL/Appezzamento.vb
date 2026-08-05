Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.Identity


Public Class Appezzamento_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal Appezza As Int32,
                           ByVal Sup_App As Decimal,
                           ByVal Data_App As Date,
                           ByVal Ep_Camp As Date,
                           ByVal X As Decimal,
                           ByVal Y As Decimal,
                           ByVal ZSLM As Decimal,
                           ByVal Esposiz As String,
                           ByVal Pende As Decimal,
                           ByVal Ubicazione As String,
                           ByVal Num_Del As Int32,
                           ByVal Clas As String,
                           ByVal sabbia As Decimal,
                           ByVal Limo As Decimal,
                           ByVal Argilla As Decimal,
                           ByVal pH As Decimal,
                           ByVal CalTot As Decimal,
                           ByVal CalAtt As Decimal,
                           ByVal SostOrg As Decimal,
                           ByVal K2OAss As Decimal,
                           ByVal P2O5Ass As Decimal,
                           ByVal Mg As Decimal,
                           ByVal Ntot As Decimal,
                           ByVal UM_S As Decimal,
                           ByVal CL_Dren As String,
                           ByVal Falda As Int32,
                           ByVal CSC As Decimal,
                           ByVal K2OAss_Data As Date,
                           ByVal MatOrg As Decimal,
                           ByVal MatOrg_Data As Date,
                           ByVal NOTot As Decimal,
                           ByVal NOTot_Data As Date,
                           ByVal P2O5Ass_Data As Date,
                           ByVal Suolo_CodAttri As String,
                           ByVal PivaSuperuser As String,
                           ByVal App_Nome As String,
                           ByVal Campo_Spia As Int16,
                           ByVal Campo_Spia_Area As Decimal,
                           ByVal CS_Sipi As String,
                           ByVal Campo_Cod As Int32,
                           ByVal Data_Inizio As Date,
                           ByVal Data_Fine As Date,
                           ByVal Prossimo As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal blk_flag As Integer = 0,
                           Optional ByVal blk_inizio_data As Date = #2/1/1900#,
                           Optional ByVal blk_inizio_username As String = "",
                           Optional ByVal blk_inizio_note As String = "",
                           Optional ByVal blk_fine_data As Date = #12/30/2100#,
                           Optional ByVal blk_fine_username As String = "",
                           Optional ByVal blk_fine_note As String = "",
                           Optional ByVal via_stringa As String = "",
                           Optional ByVal BZ_CorpiIdrici As Decimal = 0,
                           Optional ByVal BZ_AreeResPub As Decimal = 0,
                           Optional ByRef BZ_Allevamenti As Decimal = 0,
                           Optional ByRef BZ_VegNatNonColt As Decimal = 0,
                           Optional ByRef BZ_SupRiduzione As Decimal = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        '  Marco Grilli, 12/09/2014 11:29:17: Modificate anche le date di default per evitare che 
        'se un utente inserisce queste due date chiave, non vengano sostituite con quella odierna

        'If blk_inizio_data = #1/1/1900# Then
        If blk_inizio_data = #2/1/1900# Then
            blk_inizio_data = Now.Date
        End If
        'If blk_fine_data = #12/31/2100# Then
        If blk_fine_data = #12/30/2100# Then
            blk_fine_data = Now.Date
        End If

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Appezzamento ")
            StrSQL.AppendLine("                   ( Piva         ,Sa_Cod        ,Appezza       ,Sup_App          ")
            StrSQL.AppendLine("                    ,Data_App     ,Ep_Camp       ,X             ,Y                ")
            StrSQL.AppendLine("                    ,Zslm         ,Esposiz       ,Pende         ,Ubicazione       ")
            StrSQL.AppendLine("                    ,Num_Del      ,Clas          ,Sabbia        ,Limo             ")
            StrSQL.AppendLine("                    ,Argilla      ,pH            ,CalTot        ,CalAtt           ")
            StrSQL.AppendLine("                    ,SostOrg      ,K2OAss        ,P2O5Ass       ,Mg               ")
            StrSQL.AppendLine("                    ,Ntot         ,UM_S          ,CL_Dren       ,Falda            ")
            StrSQL.AppendLine("                    ,CSC          ,K2OAss_Data   ,MatOrg        ,MatOrg_Data      ")
            StrSQL.AppendLine("                    ,NOTot_Data   ,NOTot         ,P2O5Ass_Data  ,Suolo_CodAttri   ")
            StrSQL.AppendLine("                    ,[User]       ,App_Nome      ,Campo_Spia    ,Campo_Spia_Area  ")
            StrSQL.AppendLine("                    ,Cs_Sipi      ,Campo_Cod     ,Data_Inizio   ,Data_Fine        ")
            StrSQL.AppendLine("                    ,Prossimo     ")

            StrSQL.AppendLine("                    ,blk_flag     ,blk_inizio_data,blk_inizio_username, blk_inizio_note ")
            StrSQL.AppendLine("                    ,blk_fine_data     ,blk_fine_username,blk_fine_note   ")

            StrSQL.AppendLine("                    ,via_stringa     ,DistBZ_CorpiIdrici , DistBZ_AreeResPub  ")
            StrSQL.AppendLine("                    ,DistBZ_Allevamenti   ,DistBZ_VegNatNonColt , SupBZ_Riduzione  ")


            StrSQL.AppendLine("                    ,Inviato,            DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                   ) ")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sup_App) & "  ")
            StrSQL.AppendLine("         , " & If(Data_App = New Date, "Null", Agro_SQL_SaveDate(Data_App)) & "  ")
            StrSQL.AppendLine("         , " & If(Ep_Camp = New Date, "Null", Agro_SQL_SaveDate(Ep_Camp)) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(X) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Y) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ZSLM) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Esposiz) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Pende) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Ubicazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Num_Del) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Clas) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(sabbia) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Limo) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Argilla) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(pH) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(CalTot) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(CalAtt) & "  ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SostOrg) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(K2OAss) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(P2O5Ass) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Mg) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Ntot) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(UM_S) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(CL_Dren) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Falda) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(CSC) & "  ")
            StrSQL.AppendLine("         , " & If(K2OAss_Data = New Date, "Null", Agro_SQL_SaveDate(K2OAss_Data)) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MatOrg) & "  ")
            StrSQL.AppendLine("         , " & If(MatOrg_Data = New Date, "Null", Agro_SQL_SaveDate(MatOrg_Data)) & "  ")
            StrSQL.AppendLine("         , " & If(NOTot_Data = New Date, "Null", Agro_SQL_SaveDate(NOTot_Data)) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(NOTot) & "  ")
            StrSQL.AppendLine("         , " & If(P2O5Ass_Data = New Date, "Null", Agro_SQL_SaveDate(P2O5Ass_Data)) & "  ")

            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Suolo_CodAttri) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PivaSuperuser) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(App_Nome) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Campo_Spia) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Campo_Spia_Area) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(CS_Sipi) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.AppendLine("         , " & If(CStr(Data_Inizio) = New Date, "Null", Agro_SQL_SaveDate(Data_Inizio)) & "  ")
            StrSQL.AppendLine("         , " & If(CStr(Data_Fine) = New Date, "Null", Agro_SQL_SaveDate(Data_Fine)) & "  ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Prossimo) & "  ")



            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(blk_flag) & "  ")
            StrSQL.AppendLine("         ,  " & If(CStr(blk_inizio_data) = New Date, "Null", Agro_SQL_SaveDate(blk_inizio_data)) & "  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(blk_inizio_username) & "'  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(blk_inizio_note) & "'  ")

            StrSQL.AppendLine("         ,  " & If(CStr(blk_fine_data) = New Date, "Null", Agro_SQL_SaveDate(blk_fine_data)) & "  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(blk_fine_username) & "'  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(blk_fine_note) & "'  ")

            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(via_stringa) & "'  ")


            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(BZ_CorpiIdrici) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(BZ_AreeResPub) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(BZ_Allevamenti) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(BZ_VegNatNonColt) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(BZ_SupRiduzione) & "  ")


            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("		    , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("		    , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("		    ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("		    ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" )")

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

    '============================================================================
    Public Function Appezzamento_MarcaComeInviato(ByVal Piva As String,
                                                  ByVal Sa_Cod As Int32,
                                                  ByVal Appezza As Int32,
                                                  ByVal Data_invio As DateTime,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Appezzamento_MarcaComeInviato()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Appezzamento SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append(" AND     Sa_Cod   = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND     Appezza   = " & Agro_SQL_SaveNum(Appezza) & "  ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE UtentiXAppezzamenti SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append(" AND     Sa_Cod   = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND     Appezza   = " & Agro_SQL_SaveNum(Appezza) & "  ")

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

    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Appezza As Int32,
                             ByVal Sup_App As Decimal,
                             ByVal Data_App As Date,
                             ByVal Ep_Camp As Date,
                             ByVal X As Decimal,
                             ByVal Y As Decimal,
                             ByVal ZSLM As Decimal,
                             ByVal Esposiz As String,
                             ByVal Pende As Decimal,
                             ByVal Ubicazione As String,
                             ByVal Num_Del As Int32,
                             ByVal Clas As String,
                             ByVal Sabbia As Decimal,
                             ByVal Limo As Decimal,
                             ByVal Argilla As Decimal,
                             ByVal pH As Decimal,
                             ByVal CalTot As Decimal,
                             ByVal CalAtt As Decimal,
                             ByVal SostOrg As Decimal,
                             ByVal K2OAss As Decimal,
                             ByVal P2O5Ass As Decimal,
                             ByVal Mg As Decimal,
                             ByVal Ntot As Decimal,
                             ByVal UM_S As Decimal,
                             ByVal CL_Dren As String,
                             ByVal Falda As Int32,
                             ByVal CSC As Decimal,
                             ByVal K2OAss_Data As Date,
                             ByVal MatOrg As Decimal,
                             ByVal MatOrg_Data As Date,
                             ByVal NOTot As Decimal,
                             ByVal NOTot_Data As Date,
                             ByVal P2O5Ass_Data As Date,
                             ByVal Suolo_CodAttri As String,
                             ByVal App_Nome As String,
                             ByVal Campo_Spia As Int16,
                             ByVal Campo_Spia_Area As Decimal,
                             ByVal CS_Sipi As String,
                             ByVal Campo_Cod As Int32,
                             ByVal Data_Inizio As Date,
                             ByVal Data_Fine As Date,
                             ByVal Prossimo As Int32,
                             ByVal via_stringa As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
Optional ByVal BZ_CorpiIdrici As Decimal = -1,
Optional ByVal BZ_AreeResPub As Decimal = -1,
Optional ByVal BZ_Allevamenti As Decimal = -1,
Optional ByVal BZ_VegNatNonColt As Decimal = -1,
                             Optional ByVal BZ_SupRiduzione As Decimal = -1
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Appezzamento SET ")
            StrSQL.Append("    Sup_App           =  " & Agro_SQL_SaveNum(Sup_App) & " ")
            StrSQL.Append("   ,Data_App          =  " & If(Data_App = New Date, "Null", Agro_SQL_SaveDate(Data_App)) & "  ")
            StrSQL.Append("   ,Ep_Camp           =  " & If(Ep_Camp = New Date, "Null", Agro_SQL_SaveDate(Ep_Camp)) & "  ")
            StrSQL.Append("   ,X                 =  " & Agro_SQL_SaveNum(X) & " ")
            StrSQL.Append("   ,Y                 =  " & Agro_SQL_SaveNum(Y) & " ")
            StrSQL.Append("   ,Zslm              =  " & Agro_SQL_SaveNum(ZSLM) & " ")
            StrSQL.Append("   ,Esposiz           = '" & Agro_SQL_SaveText(Esposiz) & "'")
            StrSQL.Append("   ,Pende             =  " & Agro_SQL_SaveNum(Pende) & " ")
            StrSQL.Append("   ,Ubicazione        = '" & Agro_SQL_SaveText(Ubicazione) & "'")
            StrSQL.Append("   ,Num_Del           =  " & Agro_SQL_SaveNum(Num_Del) & " ")
            StrSQL.Append("   ,clas              = '" & Agro_SQL_SaveText(Clas) & "'")
            StrSQL.Append("   ,Sabbia            =  " & Agro_SQL_SaveNum(Sabbia) & " ")
            StrSQL.Append("   ,Limo              =  " & Agro_SQL_SaveNum(Limo) & " ")
            StrSQL.Append("   ,Argilla           =  " & Agro_SQL_SaveNum(Argilla) & " ")
            StrSQL.Append("   ,pH                =  " & Agro_SQL_SaveNum(pH) & " ")
            StrSQL.Append("   ,CalTot            =  " & Agro_SQL_SaveNum(CalTot) & " ")
            StrSQL.Append("   ,CalAtt            =  " & Agro_SQL_SaveNum(CalAtt) & " ")
            StrSQL.Append("   ,SostOrg           =  " & Agro_SQL_SaveNum(SostOrg) & " ")
            StrSQL.Append("   ,K2OAss            =  " & Agro_SQL_SaveNum(K2OAss) & " ")
            StrSQL.Append("   ,P2O5Ass           =  " & Agro_SQL_SaveNum(P2O5Ass) & " ")
            StrSQL.Append("   ,Mg                =  " & Agro_SQL_SaveNum(Mg) & " ")
            StrSQL.Append("   ,Ntot              =  " & Agro_SQL_SaveNum(Ntot) & " ")
            StrSQL.Append("   ,UM_S              =  " & Agro_SQL_SaveNum(UM_S) & " ")

            StrSQL.Append("   ,Cl_Dren           = '" & Agro_SQL_SaveText(CL_Dren) & "'")
            StrSQL.Append("   ,Falda             =  " & Agro_SQL_SaveNum(Falda) & " ")
            StrSQL.Append("   ,Csc               =  " & Agro_SQL_SaveNum(CSC) & " ")
            StrSQL.Append("   ,K2OAss_Data       =  " & If(K2OAss_Data = New Date, "Null", Agro_SQL_SaveDate(K2OAss_Data)) & "  ")
            StrSQL.Append("   ,MatOrg            =  " & Agro_SQL_SaveNum(MatOrg) & " ")
            StrSQL.Append("   ,MatOrg_Data       =  " & If(MatOrg_Data = New Date, "Null", Agro_SQL_SaveDate(MatOrg_Data)) & "  ")
            StrSQL.Append("   ,NOTot_Data        =  " & If(NOTot_Data = New Date, "Null", Agro_SQL_SaveDate(NOTot_Data)) & "  ")
            StrSQL.Append("   ,NOTot             =  " & Agro_SQL_SaveNum(NOTot) & " ")
            StrSQL.Append("   ,P2O5Ass_Data      =  " & If(P2O5Ass_Data = New Date, "Null", Agro_SQL_SaveDate(P2O5Ass_Data)) & "  ")
            StrSQL.Append("   ,Suolo_CodAttri    = '" & Agro_SQL_SaveText(Suolo_CodAttri) & "'")
            StrSQL.Append("   ,[User]            = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append("   ,App_Nome          = '" & Agro_SQL_SaveText(App_Nome) & "'")
            StrSQL.Append("   ,Campo_Spia        =  " & Agro_SQL_SaveNum(Campo_Spia) & " ")
            StrSQL.Append("   ,Campo_Spia_Area   =  " & Agro_SQL_SaveNum(Campo_Spia_Area) & " ")
            StrSQL.Append("   ,CS_Sipi           = '" & Agro_SQL_SaveText(CS_Sipi) & "'")
            StrSQL.Append("   ,Campo_Cod         =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            StrSQL.Append("   ,Data_Inizio       =  " & If(Data_Inizio = New Date, "Null", Agro_SQL_SaveDate(Data_Inizio)) & "  ")
            StrSQL.Append("   ,Data_Fine         =  " & If(Data_Fine = New Date, "Null", Agro_SQL_SaveDate(Data_Fine)) & "  ")

            StrSQL.Append("   ,via_stringa           = '" & Agro_SQL_SaveText(via_stringa) & "'")
            If BZ_CorpiIdrici <> -1 Then
                StrSQL.Append("   ,DistBZ_CorpiIdrici           = " & Agro_SQL_SaveNum(BZ_CorpiIdrici))
            End If
            If BZ_AreeResPub <> -1 Then
                StrSQL.Append("   ,DistBZ_AreeResPub           = " & Agro_SQL_SaveNum(BZ_AreeResPub))
            End If
            If BZ_Allevamenti <> -1 Then
                StrSQL.Append("   ,DistBZ_Allevamenti         = " & Agro_SQL_SaveNum(BZ_Allevamenti))
            End If
            If BZ_VegNatNonColt <> -1 Then
                StrSQL.Append("   ,DistBZ_VegNatNonColt           = " & Agro_SQL_SaveNum(BZ_VegNatNonColt))
            End If
            If BZ_SupRiduzione <> -1 Then
                StrSQL.Append("   ,SupBZ_Riduzione           = " & Agro_SQL_SaveNum(BZ_SupRiduzione))
            End If

            StrSQL.Append("   ,Prossimo          =  " & Agro_SQL_SaveNum(Prossimo) & " ")
            StrSQL.Append("   ,Inviato         =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            '----------------------------------------------------------------------
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


    Public Function Modifica_Parametrizzata(ByVal Piva As String,
                                            ByVal Sa_Cod As Int32,
                                            ByVal Appezza As Int32,
                                            ByVal Campo As String,
                                            ByVal Valore As Object,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Modifica_Parametrizzata()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim stringa As Type = GetType(System.String)
        Dim data As Type = GetType(System.DateTime)
        'Dim intero32 As Type = GetType(System.Int32)
        'Dim doubl As Type = GetType(System.Decimal)


        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            '---------------------------------------------

            Dim typeVal As Type = Valore.GetType()

            If typeVal.Equals(stringa) Then
                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "
            ElseIf typeVal.Equals(data) Then
                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "
            Else
                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "
            End If

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")

            StrSQL.Append(strAssegnamento)

            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            '----------------------------------------------------------------------
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

    Public Function Modifica_Chiave(ByVal Piva As String,
                                    ByVal Sa_Cod As Int32,
                                    ByVal Appezza As Int32,
                                    ByVal Piva_OLD As String,
                                    ByVal Sa_Cod_OLD As Int32,
                                    ByVal Appezza_OLD As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Modifica_Chiave()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")

            StrSQL.Append(" Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" ,Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" ,Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            StrSQL.Append(" ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append(" ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva_OLD)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_OLD) & " ")
            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza_OLD) & " ")

            '----------------------------------------------------------------------
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

    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Appezza As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si cancellano tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  si cancellano tutti gli appezzamenti del centro aziendale
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Appezzamento ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Appezzamento ")
                StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")

            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append("  AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
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


    Public Function AggiornaValiditaInizio(ByVal Piva As String,
                                           ByVal Sa_Cod As Long,
                                           ByVal Id_Campo As Long,
                                           ByVal Appezza As Long,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.AggiornaValiditaInizio()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  aggiorna tutti gli appezzamenti del centro aziendale
        '   Id_Campo = 0         =>  aggiorna tutti gli appezzamenti del centro aziendale
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Sa_Cod <> 0 Then
                StrSQL.Append("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND   Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            '---------------------------------------------
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

    Public Function AggiornaValiditaInizioForzata(ByVal Piva As String,
                                                  ByVal Sa_Cod As Long,
                                                  ByVal Id_Campo As Long,
                                                  ByVal Appezza As Long,
                                                  ByVal Validita_Inizio As Date,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.AggiornaValiditaInizioForzata()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  aggiorna tutti gli appezzamenti del centro aziendale
        '   Id_Campo = 0         =>  aggiorna tutti gli appezzamenti del centro aziendale
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                StrSQL.Append("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND   Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            '---------------------------------------------
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


    Public Function AggiornaValiditaFine(ByVal Piva As String,
                                         ByVal Sa_Cod As Long,
                                         ByVal Id_Campo As Long,
                                         ByVal Appezza As Long,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  aggiorna tutti gli appezzamenti del centro aziendale
        '   Id_Campo = 0         =>  aggiorna tutti gli appezzamenti del centro aziendale
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.Append("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND   Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            '---------------------------------------------
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


    Public Function AggiornaValiditaFineForzata(ByVal Piva As String,
                                                ByVal Sa_Cod As Long,
                                                ByVal Id_Campo As Long,
                                                ByVal Appezza As Long,
                                                ByVal Validita_Fine As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.AggiornaValiditaFineForzata()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  aggiorna tutti gli appezzamenti del centro aziendale
        '   Id_Campo = 0         =>  aggiorna tutti gli appezzamenti del centro aziendale
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")


            If Sa_Cod <> 0 Then
                StrSQL.Append("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND   Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            '---------------------------------------------
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

    Public Function Appezza_Sblocca(ByVal Piva As String,
                                    ByVal Sa_Cod As Long,
                                    ByVal Appezza As Long,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Appezza_Sblocca()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")
            StrSQL.Append("    Blk_Flag              =  " & Agro_SQL_SaveNum(0) & "  ")
            StrSQL.Append("   ,Blk_Fine_Data         =  " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")
            StrSQL.Append("   ,Blk_Fine_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.Append("   ,Blk_Fine_Note         = '" & Agro_SQL_SaveText("Sblocco manuale da menu anagrafica") & "' ")
            StrSQL.Append("   ,Blk_Inizio_Data         =  " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.Append("   ,Blk_Inizio_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.Append("   ,Blk_Inizio_Note         = '" & Agro_SQL_SaveText("Sblocco manuale da menu anagrafica") & "' ")

            StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND  Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            '---------------------------------------------
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

    Public Function Appezza_Blocca(ByVal Piva As String,
                                   ByVal Sa_Cod As Long,
                                   ByVal Appezza As Long,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Appezza_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("UPDATE Appezzamento SET ")
            strSql.AppendLine("    Blk_Flag              =  " & Agro_SQL_SaveNum(-1) & "  ")
            strSql.AppendLine("   ,Blk_Fine_Data         =  " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")
            strSql.AppendLine("   ,Blk_Fine_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            strSql.AppendLine("   ,Blk_Fine_Note         = '" & Agro_SQL_SaveText("Blocco manuale da menu anagrafica") & "' ")
            strSql.AppendLine("   ,Blk_Inizio_Data         =  " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            strSql.AppendLine("   ,Blk_Inizio_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            strSql.AppendLine("   ,Blk_Inizio_Note         = '" & Agro_SQL_SaveText("Blocco manuale da menu anagrafica") & "' ")

            strSql.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            strSql.AppendLine(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(" AND  Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            '---------------------------------------------
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

    Public Function Appezza_Blocco_Inizio(ByVal Piva As String,
                                          ByVal Sa_Cod As Long,
                                          ByVal Appezza As Long,
                                          ByVal Blk_Flag As Integer,
                                          ByVal Blk_Inizio_Data As Date,
                                          ByVal Blk_Inizio_UserName As String,
                                          ByVal Blk_Inizio_Note As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Appezza_Blocco_Inizio()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  aggiorna tutti gli appezzamenti di tutte le imprese
        '   Sa_Cod = 0           =>  aggiorna tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  aggiorna tutti gli appezzamenti del centro aziendale
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")
            StrSQL.Append("    Blk_Flag                =  " & Agro_SQL_SaveNum(Blk_Flag) & "  ")
            StrSQL.Append("   ,Blk_Inizio_Data         =  " & Agro_SQL_SaveDate(Blk_Inizio_Data) & "  ")
            StrSQL.Append("   ,Blk_Inizio_Username     = '" & Agro_SQL_SaveText(Blk_Inizio_UserName) & "'  ")
            StrSQL.Append("   ,Blk_Inizio_Note         = '" & Agro_SQL_SaveText(Blk_Inizio_Note) & "' ")

            If Piva <> "" Then

                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")

                If Sa_Cod <> 0 Then

                    StrSQL.Append(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND  Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                End If

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

    Public Function Appezza_Blocco_Fine(ByVal Piva As String,
                                        ByVal Sa_Cod As Long,
                                        ByVal Appezza As Long,
                                        ByVal Blk_Flag As Integer,
                                        ByVal Blk_Fine_Data As Date,
                                        ByVal Blk_Fine_UserName As String,
                                        ByVal Blk_Fine_Note As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Appezza_Blocco_Fine()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  aggiorna tutti gli appezzamenti di tutte le imprese
        '   Sa_Cod = 0           =>  aggiorna tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  aggiorna tutti gli appezzamenti del centro aziendale
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")
            StrSQL.Append("    Blk_Flag              =  " & Agro_SQL_SaveNum(Blk_Flag) & "  ")
            StrSQL.Append("   ,Blk_Fine_Data         =  " & Agro_SQL_SaveDate(Blk_Fine_Data) & "  ")
            StrSQL.Append("   ,Blk_Fine_Username     = '" & Agro_SQL_SaveText(Blk_Fine_UserName) & "'  ")
            StrSQL.Append("   ,Blk_Fine_Note         = '" & Agro_SQL_SaveText(Blk_Fine_Note) & "' ")

            If Piva <> "" Then

                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")

                If Sa_Cod <> 0 Then

                    StrSQL.Append(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND  Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                End If

            End If

            '---------------------------------------------
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

    Public Function Aggiorna_Validazione(ByVal Validazione As Integer,
                                         ByVal Piva As String,
                                         ByVal Sa_Cod As Long,
                                         ByVal Appezza As Long,
                                         ByVal Data_Validazione As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Aggiorna_Validazione()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Imprese SET ")
            StrSQL.Append("    Validazione          = " & Agro_SQL_SaveNum(Validazione))
            StrSQL.Append("   ,Username_Validazione = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Data_Validazione        = " & Agro_SQL_SaveDate(Data_Validazione))
            StrSQL.Append(" WHERE Piva='" & Agro_SQL_SaveText(Piva) & "'")

            If (Sa_Cod <> 0) Then
                StrSQL.Append(" AND SA_COD = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If (Appezza <> 0) Then
                StrSQL.Append(" AND APPEZZA = " & Agro_SQL_SaveNum(Appezza) & "  ")
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

    Public Function Superficie_ReImpostaDaRipartoCatasto(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Appezza As Integer,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Superficie_ReImpostaDaRipartoCatasto()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            '---------------------------------------------

            stb.Length = 0

            stb.AppendLine("  UPDATE app ")
            stb.AppendLine("  SET SUP_APP = catasto.area ")
            stb.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            stb.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            stb.AppendLine(" from ( ")
            stb.AppendLine("  select piva, Sa_Cod, Appezza, sum(area) as area ")
            stb.AppendLine("     From AppezzamentixParticelle ")
            stb.Append("      WHERE    Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            stb.Append("      AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            stb.Append("      AND      Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine("  group by PIVA, Sa_Cod, Appezza ")
            stb.AppendLine(" ) catasto ")
            stb.AppendLine("  inner Join Appezzamento app  ")
            stb.AppendLine("         On app.PIVA = catasto.PIVA  ")
            stb.AppendLine("      And app.SA_COD = catasto.SA_COD  ")
            stb.AppendLine("      And app.APPEZZA = catasto.appezza")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function ModificaSingolo_CampoNumericoDec(ByVal Piva As String,
                                                     ByVal Sa_Cod As Integer,
                                                     ByVal Appezza As Integer,
                                                     ByVal NomeCampo As String,
                                                     ByVal CampoValoreNumerico As Decimal,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.ModificaSingolo_CampoNumericoDec()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE   Appezzamento SET ")
            StrSQL.Append("         " & NomeCampo & "     =  " & Agro_SQL_SaveNum(CampoValoreNumerico) & " ")
            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND      Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

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

    Public Function ModificaSingolo_CampoNumerico(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Appezza As Integer,
                                                  ByVal NomeCampo As String,
                                                  ByVal CampoValoreNumerico As Integer,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.ModificaSingolo_CampoNumerico()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE   Appezzamento SET ")
            StrSQL.Append("         " & NomeCampo & "     =  " & IIf(Agro_SQL_SaveNum(CampoValoreNumerico, False) = -1, "Null", Agro_SQL_SaveNum(CampoValoreNumerico)) & " ")
            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND      Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

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

    Public Function BufferZone_Aggiorna(ByVal Piva As String,
                                        ByVal sa_cod As Integer,
                                        ByVal appezza As Integer,
                                        ByVal DistBZ_CorpiIdrici As Decimal,
                                        ByVal DistBZ_AreeResPub As Decimal,
                                        ByVal DistBZ_Allevamenti As Decimal,
                                        ByVal DistBZ_VegNatNonColt As Decimal,
                                        ByVal SupBZ_Riduzione As Decimal,
                                        ByVal objParametri_Server As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.BufferZone_Aggiorna()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento SET ")

            StrSQL.Append("     DistBZ_CorpiIdrici = " & Agro_SQL_SaveNum(DistBZ_CorpiIdrici) & vbCrLf)
            StrSQL.Append("   , DistBZ_AreeResPub = " & Agro_SQL_SaveNum(DistBZ_AreeResPub) & vbCrLf)
            StrSQL.Append("   , DistBZ_Allevamenti = " & Agro_SQL_SaveNum(DistBZ_Allevamenti) & vbCrLf)
            StrSQL.Append("   , DistBZ_VegNatNonColt = " & Agro_SQL_SaveNum(DistBZ_VegNatNonColt) & vbCrLf)
            StrSQL.Append("   , SupBZ_Riduzione = " & Agro_SQL_SaveNum(SupBZ_Riduzione) & vbCrLf)

            StrSQL.Append("   ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'")
            StrSQL.Append("   ,Data_Modifica = " & Agro_SQL_SaveDateTime(Now))

            StrSQL.Append(" WHERE Piva='" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND SA_COD = " & Agro_SQL_SaveNum(sa_cod) & "  ")
            StrSQL.Append(" AND APPEZZA = " & Agro_SQL_SaveNum(appezza) & "  ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function UpdateColonna_Massivo(listChiavi As List(Of (String, Integer, Integer)),
                                          nomeColonna As String,
                                          valore As String,
                                          tipoDato As String,
                                          timeStamp As Date,
                                          ByVal objParametri_Server As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.UpdateColonna_Massivo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim flagConnessione, flagTransazione As Boolean

        Try

            If listChiavi.Count = 0 Then
                Throw New Exception("Parametro non corretto nella query (listChiavi obbligatorio)")
            End If

            Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroAppezzamenti(listChiavi, nomeRoutine, objParametri_Server)

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE A ")
            StrSQL.AppendLine("SET ")

            StrSQL.Append($" {nomeColonna} = ")
            Select Case tipoDato
                Case "string"
                    StrSQL.Append($"'{Agro_SQL_SaveText(valore)}'")
                Case "date"
                    StrSQL.Append(Agro_SQL_SaveDate(valore))
                Case "number"
                    StrSQL.Append(Agro_SQL_SaveNum(valore))
            End Select

            StrSQL.AppendLine($" , Data_Modifica = {Agro_SQL_SaveDateTime(timeStamp)} ")

            StrSQL.AppendLine(" FROM Appezzamento A ")
            StrSQL.AppendLine(" JOIN #TempAppezzamento temp (NOLOCK) ON  ")
            StrSQL.AppendLine("     A.Piva = temp.Piva ")
            StrSQL.AppendLine(" AND A.Sa_Cod = temp.Sa_Cod ")
            StrSQL.AppendLine(" AND A.Appezza = temp.Appezza ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroAppezzamenti(nomeRoutine, objParametri_Server)
            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)

        Catch ex As Exception
            ' Rollback
            Utility.VerificaAnnullaTransazione(objParametri_Server, flagTransazione)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
        End Try

        Return xRisp

    End Function

    Private Function CreaTabellaTemp_FiltroChiavi() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempChiavi') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempChiavi ( ")
        stb.AppendLine("        Piva VARCHAR(50) NULL")
        stb.AppendLine("      , Sa_Cod INT NULL")
        stb.AppendLine("      , Appezza INT NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Private Function EliminaTabellaTemp_FiltroChiavi() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempChiavi') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempChiavi ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

#Region "PlotSlope"
    Public Function UpdateSlope(piva As String,
                                saCod As Int32,
                                appezza As Int32,
                                slope As Double,
                                objServer As AgronicaCoreParametri) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.UpdateSlope()"
        Dim StrSQL As New Text.StringBuilder

        Try
            StrSQL.AppendLine("    UPDATE Appezzamento")
            StrSQL.AppendLine("    SET")
            StrSQL.AppendLine($"         PENDE = {Agro_SQL_SaveNum(slope)}")
            StrSQL.AppendLine($"         , Data_Modifica = GETDATE() ")
            StrSQL.AppendLine($"         , Username_Modifica = '{Agro_SQL_SaveText(objServer.UtenteCodFiscale)}'")
            StrSQL.AppendLine("")
            StrSQL.AppendLine($"    WHERE Piva = '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        AND Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            StrSQL.AppendLine($"        AND Appezza = {Agro_SQL_SaveNum(appezza)}")

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function
#End Region

#Region "PlotWeaving"
    Public Function UpdateWeaving(piva As String,
                                  saCod As Int32,
                                  appezza As Int32,
                                  clay As Decimal,
                                  sand As Decimal,
                                  slit As Decimal,
                                  weavingClass As String,
                                  objServer As AgronicaCoreParametri) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.UpdateSlope()"
        Dim StrSQL As New Text.StringBuilder

        Try
            StrSQL.AppendLine("    UPDATE Appezzamento")
            StrSQL.AppendLine("    SET")
            StrSQL.AppendLine($"         ARGILLA = {Agro_SQL_SaveNum(clay)}")
            StrSQL.AppendLine($"         , SABBIA= {Agro_SQL_SaveNum(sand)}")
            StrSQL.AppendLine($"         , LIMO = {Agro_SQL_SaveNum(slit)}")
            StrSQL.AppendLine($"         , CLAS = '{Agro_SQL_SaveText(weavingClass)}'")
            StrSQL.AppendLine($"         , Data_Modifica = GETDATE() ")
            StrSQL.AppendLine($"         , Username_Modifica = '{Agro_SQL_SaveText(objServer.UtenteCodFiscale)}'")
            StrSQL.AppendLine("")
            StrSQL.AppendLine($"    WHERE Piva = '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        AND Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            StrSQL.AppendLine($"        AND Appezza = {Agro_SQL_SaveNum(appezza)}")

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function
#End Region

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Appezzamento_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Dato un appezzamento legge impianti, distinte per questo e per gli altri appezzamenti che hanno lo stesso riparto catastale di quello passato in ingresso
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>In uscita una tabella di distinte relative all'appezzamento passato come parametro e tutti gli appezzamenti associati alle stesse particelle intersecate dall'appezzamento passato come parametro, laddove la data di fine validità degli appezzamenti intersecati è minore della data di inizio validità per l'appezzamento in questione</returns>
    Public Function LeggiAppezzamentiConCatastoSovrapposto(ByVal Piva As String,
                                                           ByVal Sa_Cod As Int32,
                                                           ByVal Appezza As Int32,
                                                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByVal xOrderBy As String,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiAppezzamentiConCatastoSovrapposto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            Dim stb As New Text.StringBuilder

            stb.AppendLine(" declare @piva varchar(50) ")
            stb.AppendLine(" declare @sa_cod int ")
            stb.AppendLine(" declare @appezza int ")
            stb.AppendLine("  ")
            stb.AppendLine(" set @piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine(" set @sa_cod = 	 " & Agro_SQL_SaveNum(Sa_Cod))
            stb.AppendLine(" set @appezza =  " & Agro_SQL_SaveNum(Appezza))
            stb.AppendLine("  ")

            'alcuni esempi..:
            stb.AppendLine("----esempio su aboca, sam, appezzamento  ")
            stb.AppendLine(" --set @piva = '02969160544' ")
            stb.AppendLine(" --set @sa_cod = 130023425 ")
            stb.AppendLine(" --set @appezza = 130023461 ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine(" ----esempio su Genagricola, san giorgio ")
            stb.AppendLine(" --set @piva = '00570600320' ")
            stb.AppendLine(" --set @sa_cod = 114688005 ")
            stb.AppendLine(" --set @appezza = 114688003 ")
            stb.AppendLine("  ")
            stb.AppendLine(" ----esempio su groppi, coldiretti ")
            stb.AppendLine(" --set @piva = '01323380335' ")
            stb.AppendLine(" --set @sa_cod = 85327873  ")
            stb.AppendLine(" --set @appezza = 85328250 ")
            stb.AppendLine("  ")
            stb.AppendLine(" ----esempio su smartagri, biavati paolo ")
            stb.AppendLine(" --set @piva = '00171190549' ")
            stb.AppendLine(" --set @sa_cod = 131727361 ")
            stb.AppendLine(" --set @appezza = 131727369")

            'query
            stb.AppendLine(" select distinct   ")
            stb.AppendLine("       Risultato ")
            stb.AppendLine(" 	, case when  gru.Gru_Des is null then d.DestUso else v.Veg_Des + ' (' + gru.Gru_Des + ')' end as Utilizzo	 ")
            stb.AppendLine(" 	, anag1.PIVA ")
            stb.AppendLine(" 	, anag1.SA_COD ")
            stb.AppendLine(" 	, anag1.APPEZZA ")
            stb.AppendLine(" 	, anag1.id_reg ")
            stb.AppendLine(" 	--, j1.PROV ")
            stb.AppendLine(" 	--, j1.COM ")
            stb.AppendLine(" 	--, j1.SEZIONE ")
            stb.AppendLine(" 	--, j1.FOGLIO ")
            stb.AppendLine(" 	--, j1.NUMERO  ")
            stb.AppendLine(" 	--, j1.SUBALTERNO  ")
            stb.AppendLine(" 	, Progetto_Cod ")
            stb.AppendLine(" 	, Progetto_Des ")
            stb.AppendLine(" 	, Progetto_Nome ")
            stb.AppendLine(" 	, isnull(c.Cul_Cod, 0) as cul_cod ")
            stb.AppendLine(" 	, isnull(v.Veg_Cod, 0) as veg_cod ")
            stb.AppendLine(" 	, ISNULL(gru.gru_cod, 0) as gru_cod ")
            stb.AppendLine(" 	, isnull(d.codice, 0) as id_cod	 ")
            stb.AppendLine(" 	, APP_NOME ")
            stb.AppendLine(" 	, iValidita_inizio ")
            stb.AppendLine(" 	, iValidita_fine ")
            stb.AppendLine(" 	, aValiditaInizio ")
            stb.AppendLine(" 	, aValiditaFine ")
            stb.AppendLine(" 	, anag1.Validita_Inizio ")
            stb.AppendLine(" 	, anag1.Validita_Fine ")
            stb.AppendLine(" from ( ")
            stb.AppendLine("  ")
            stb.AppendLine(" --prima query: ricerca di appezzamenti 1 per ogni annata agraria (in base a catasto) ")
            stb.AppendLine(" select distinct  ")
            stb.AppendLine(" 	  'Altri appezzamenti' as [Risultato] ")
            stb.AppendLine(" 	, a.PIVA ")
            stb.AppendLine(" 	, a.SA_COD ")
            stb.AppendLine(" 	, a.APPEZZA ")
            stb.AppendLine(" 	, i.id_reg ")
            stb.AppendLine(" 	--, j1.PROV ")
            stb.AppendLine(" 	--, j1.COM ")
            stb.AppendLine(" 	--, j1.SEZIONE ")
            stb.AppendLine(" 	--, j1.FOGLIO ")
            stb.AppendLine(" 	--, j1.NUMERO  ")
            stb.AppendLine(" 	--, j1.SUBALTERNO  ")
            stb.AppendLine(" 	, ip.Progetto_Cod ")
            stb.AppendLine(" 	, ip.Progetto_Des ")
            stb.AppendLine(" 	, ip.Progetto_Nome	 ")
            stb.AppendLine(" 	, a.APP_NOME ")
            stb.AppendLine(" 	, i.CUL_COD ")
            stb.AppendLine(" 	, i.Validita_Inizio as iValidita_inizio ")
            stb.AppendLine(" 	, i.Validita_Fine as iValidita_fine ")
            stb.AppendLine(" 	, a.Validita_Inizio as aValiditaInizio ")
            stb.AppendLine(" 	, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	, ip.Validita_Inizio as Validita_Inizio ")
            stb.AppendLine(" 	, ip.Validita_Fine as Validita_Fine ")
            stb.AppendLine("  ")
            stb.AppendLine(" from ( ")
            stb.AppendLine(" 	select j1.*, a.Validita_Inizio as aValiditaInizio, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	from AppezzamentiXParticelle j1 ")
            stb.AppendLine(" 		inner join appezzamento a ")
            stb.AppendLine(" 		on  j1.PIVA = a.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = a.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" 	where j1.PIVA = @piva  ")
            stb.AppendLine(" 	and j1.SA_COD = @sa_cod ")
            stb.AppendLine(" 	and j1.APPEZZA = @appezza ")
            stb.AppendLine(" ) st ")
            stb.AppendLine(" inner join AppezzamentiXParticelle j1 ")
            stb.AppendLine(" 	on j1.prov = st.prov ")
            stb.AppendLine(" 	and j1.COM = st.COM ")
            stb.AppendLine(" 	and j1.SEZIONE = st.SEZIONE ")
            stb.AppendLine(" 	and j1.FOGLIO = st.FOGLIO ")
            stb.AppendLine(" 	and j1.NUMERO = st.NUMERO ")
            stb.AppendLine(" 	and j1.SUBALTERNO = st.SUBALTERNO ")
            stb.AppendLine(" 	--and ( ABS( j1.AREA - st.AREA ) < 0.001) ")
            stb.AppendLine(" 	and not ( ")
            stb.AppendLine(" 		    j1.PIVA = st.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = st.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = st.APPEZZA ")
            stb.AppendLine(" 	)		 ")
            stb.AppendLine(" inner join Appezzamento a ")
            stb.AppendLine(" 	on  ")
            stb.AppendLine(" 		    j1.PIVA = a.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = a.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = a.APPEZZA ")
            stb.AppendLine(" 		and a.Validita_Fine < st.aValiditaInizio ")
            stb.AppendLine("  ")
            stb.AppendLine(" --parte opzionale 1. (precessioni colturali) ")
            stb.AppendLine(" inner join Reg_Impianti i ")
            stb.AppendLine(" 	on i.piva = a.piva  ")
            stb.AppendLine(" 	and i.SA_COD = a.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese_Progetti ip ")
            stb.AppendLine(" 	on i.piva = ip.piva  ")
            stb.AppendLine(" 	and i.SA_COD = ip.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = ip.APPEZZA ")
            stb.AppendLine(" 	and i.ID_REG = ip.id_reg ")
            stb.AppendLine("  ")
            stb.AppendLine(" union 	 ")
            stb.AppendLine("  ")
            stb.AppendLine(" --seconda query: ricerca di impianti, distinte dell'appezzamento selezionato ")
            stb.AppendLine(" select distinct  ")
            stb.AppendLine(" 	 'Questo appezzamento' as [Risultato] ")
            stb.AppendLine(" 	, a.PIVA ")
            stb.AppendLine(" 	, a.SA_COD ")
            stb.AppendLine(" 	, a.APPEZZA ")
            stb.AppendLine(" 	, i.id_reg ")
            stb.AppendLine(" 	--, j1.PROV ")
            stb.AppendLine(" 	--, j1.COM ")
            stb.AppendLine(" 	--, j1.SEZIONE ")
            stb.AppendLine(" 	--, j1.FOGLIO ")
            stb.AppendLine(" 	--, j1.NUMERO  ")
            stb.AppendLine(" 	--, j1.SUBALTERNO  ")
            stb.AppendLine(" 	, ip.Progetto_Cod ")
            stb.AppendLine(" 	, ip.Progetto_Des ")
            stb.AppendLine(" 	, ip.Progetto_Nome	 ")
            stb.AppendLine(" 	, a.APP_NOME ")
            stb.AppendLine(" 	, i.CUL_COD  ")
            stb.AppendLine(" 	, i.Validita_Inizio as iValidita_inizio ")
            stb.AppendLine(" 	, i.Validita_Fine as iValidita_fine ")
            stb.AppendLine(" 	, a.Validita_Inizio as aValiditaInizio ")
            stb.AppendLine(" 	, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	, ip.Validita_Inizio as Validita_Inizio ")
            stb.AppendLine(" 	, ip.Validita_Fine as Validita_Fine ")
            stb.AppendLine("  ")
            stb.AppendLine(" from Appezzamento a ")
            stb.AppendLine(" 	 ")
            stb.AppendLine(" --parte opzionale 1. (precessioni colturali) ")
            stb.AppendLine(" inner join Reg_Impianti i ")
            stb.AppendLine(" 	on i.piva = a.piva  ")
            stb.AppendLine(" 	and i.SA_COD = a.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese_Progetti ip ")
            stb.AppendLine(" 	on i.piva = ip.piva  ")
            stb.AppendLine(" 	and i.SA_COD = ip.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = ip.APPEZZA ")
            stb.AppendLine(" 	and i.ID_REG = ip.id_reg ")
            stb.AppendLine(" 	 ")
            stb.AppendLine(" where a.PIVA = @piva  ")
            stb.AppendLine(" 	and a.SA_COD = @sa_cod ")
            stb.AppendLine(" 	and a.APPEZZA = @appezza ")
            stb.AppendLine("  ")
            stb.AppendLine(" ) Anag1 ")
            stb.AppendLine("  ")
            stb.AppendLine(" left join Cultivar c ")
            stb.AppendLine(" 	on c.Cul_Cod = Anag1.CUL_COD ")
            stb.AppendLine(" left join SpecieVegetali v ")
            stb.AppendLine(" 	on v.Veg_Cod = c.veg_Cod ")
            stb.AppendLine(" left join GruppoVegetale gru ")
            stb.AppendLine(" 	on gru.Gru_Cod = v.Gru_Cod ")
            stb.AppendLine("  ")
            stb.AppendLine(" left join ( ")
            stb.AppendLine(" 	select distinct piva, sa_cod, appezza, id_reg, a.codice, a.descrizione as DestUso ")
            stb.AppendLine(" 	from Reg_Impianti_Codici c ")
            stb.AppendLine(" 		inner join Codici_Anagrafe a ")
            stb.AppendLine(" 			on c.id_cod = a.codice ")
            stb.AppendLine(" 	where id_cod >= 3000 and id_cod < 4000 ")
            stb.AppendLine(" ) d ")
            stb.AppendLine(" on      Anag1.piva = d.piva  ")
            stb.AppendLine(" 	and Anag1.SA_COD = d.SA_COD ")
            stb.AppendLine(" 	and Anag1.APPEZZA = d.APPEZZA ")
            stb.AppendLine(" 	and Anag1.id_reg = d.id_reg")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" where " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiAppezzamentiConCatastoSovrapposto_VincoliAgronomici(
        ByVal Piva As String,
        ByVal Sa_Cod As Int32,
        ByVal Appezza As Int32,
        ByVal puaCod As String,
        ByVal regolamentoCod As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiAppezzamentiConCatastoSovrapposto_VincoliAgronomici()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            Dim stb As New Text.StringBuilder

            stb.AppendLine(" declare @piva varchar(50) ")
            stb.AppendLine(" declare @sa_cod int ")
            stb.AppendLine(" declare @appezza int ")
            stb.AppendLine("  ")
            stb.AppendLine(" set @piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine(" set @sa_cod = 	 " & Agro_SQL_SaveNum(Sa_Cod))
            stb.AppendLine(" set @appezza =  " & Agro_SQL_SaveNum(Appezza))
            stb.AppendLine("  ")

            'alcuni esempi..:
            stb.AppendLine("----esempio su aboca, sam, appezzamento  ")
            stb.AppendLine(" --set @piva = '02969160544' ")
            stb.AppendLine(" --set @sa_cod = 130023425 ")
            stb.AppendLine(" --set @appezza = 130023461 ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine(" ----esempio su Genagricola, san giorgio ")
            stb.AppendLine(" --set @piva = '00570600320' ")
            stb.AppendLine(" --set @sa_cod = 114688005 ")
            stb.AppendLine(" --set @appezza = 114688003 ")
            stb.AppendLine("  ")
            stb.AppendLine(" ----esempio su groppi, coldiretti ")
            stb.AppendLine(" --set @piva = '01323380335' ")
            stb.AppendLine(" --set @sa_cod = 85327873  ")
            stb.AppendLine(" --set @appezza = 85328250 ")
            stb.AppendLine("  ")
            stb.AppendLine(" ----esempio su smartagri, biavati paolo ")
            stb.AppendLine(" --set @piva = '00171190549' ")
            stb.AppendLine(" --set @sa_cod = 131727361 ")
            stb.AppendLine(" --set @appezza = 131727369")

            'query
            stb.AppendLine(" select distinct   ")
            stb.AppendLine("       Risultato ")
            stb.AppendLine(" 	, anag1.PIVA ")
            stb.AppendLine(" 	, anag1.SA_COD ")
            stb.AppendLine(" 	, anag1.APPEZZA ")
            stb.AppendLine(" 	, anag1.id_reg ")
            stb.AppendLine(" 	, anag1.Progetto_Cod ")
            stb.AppendLine(" 	, Progetto_Des ")
            stb.AppendLine(" 	, Progetto_Nome ")
            stb.AppendLine(" 	, APP_NOME ")
            stb.AppendLine(" 	, isnull(c.Cul_Cod, 0) as cul_cod ")
            stb.AppendLine(" 	, isnull(v.Veg_Cod, 0) as veg_cod ")
            stb.AppendLine(" 	, isnull(c.Cul_des, 0) as cul_des ")
            stb.AppendLine(" 	, isnull(v.Veg_des, 0) as veg_des ")
            stb.AppendLine(" 	, iValidita_inizio ")
            stb.AppendLine(" 	, iValidita_fine ")
            stb.AppendLine(" 	, aValiditaInizio ")
            stb.AppendLine(" 	, aValiditaFine ")
            stb.AppendLine(" 	, anag1.Validita_Inizio ")
            stb.AppendLine(" 	, anag1.Validita_Fine ")
            stb.AppendLine(" 	, av.Analisi_Testata_Cod ")
            stb.AppendLine(" 	, at.Analisi_Testata_Data_Fine ")
            stb.AppendLine(" 	, av.Ubicazione_Cod ")
            stb.AppendLine(" 	, av.TipoAcqua_Cod ")
            stb.AppendLine(" 	, av.N_FertilizzazioniPrecedenti ")
            stb.AppendLine(" 	, av.Validita_Inizio as av_Validita_Inizio ")
            stb.AppendLine(" 	, av.Validita_Fine as av_Validita_Fine ")
            stb.AppendLine(" 	, av.pua_cod ")

            stb.AppendLine(" from ( ")
            stb.AppendLine("  ")
            stb.AppendLine(" --prima query: ricerca di appezzamenti 1 per ogni annata agraria (in base a catasto) ")
            stb.AppendLine(" select distinct  ")
            stb.AppendLine(" 	  'Altri appezzamenti' as [Risultato] ")
            stb.AppendLine(" 	, a.PIVA ")
            stb.AppendLine(" 	, a.SA_COD ")
            stb.AppendLine(" 	, a.APPEZZA ")
            stb.AppendLine(" 	, i.id_reg ")
            stb.AppendLine(" 	, ip.Progetto_Cod ")
            stb.AppendLine(" 	, ip.Progetto_Des ")
            stb.AppendLine(" 	, ip.Progetto_Nome	 ")
            stb.AppendLine(" 	, a.APP_NOME ")
            stb.AppendLine(" 	, i.CUL_COD ")
            stb.AppendLine(" 	, i.Validita_Inizio as iValidita_inizio ")
            stb.AppendLine(" 	, i.Validita_Fine as iValidita_fine ")
            stb.AppendLine(" 	, a.Validita_Inizio as aValiditaInizio ")
            stb.AppendLine(" 	, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	, ip.Validita_Inizio as Validita_Inizio ")
            stb.AppendLine(" 	, ip.Validita_Fine as Validita_Fine ")
            stb.AppendLine("  ")
            stb.AppendLine(" from ( ")
            stb.AppendLine(" 	select j1.*, a.Validita_Inizio as aValiditaInizio, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	from AppezzamentiXParticelle j1 ")
            stb.AppendLine(" 		inner join appezzamento a ")
            stb.AppendLine(" 		on  j1.PIVA = a.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = a.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" 	where j1.PIVA = @piva  ")
            stb.AppendLine(" 	and j1.SA_COD = @sa_cod ")
            stb.AppendLine(" 	and j1.APPEZZA = @appezza ")
            stb.AppendLine(" ) st ")
            stb.AppendLine(" inner join AppezzamentiXParticelle j1 ")
            stb.AppendLine(" 	on j1.prov = st.prov ")
            stb.AppendLine(" 	and j1.COM = st.COM ")
            stb.AppendLine(" 	and j1.SEZIONE = st.SEZIONE ")
            stb.AppendLine(" 	and j1.FOGLIO = st.FOGLIO ")
            stb.AppendLine(" 	and j1.NUMERO = st.NUMERO ")
            stb.AppendLine(" 	and j1.SUBALTERNO = st.SUBALTERNO ")
            stb.AppendLine(" 	and not ( ")
            stb.AppendLine(" 		    j1.PIVA = st.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = st.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = st.APPEZZA ")
            stb.AppendLine(" 	)		 ")
            stb.AppendLine(" inner join Appezzamento a ")
            stb.AppendLine(" 	on  ")
            stb.AppendLine(" 		    j1.PIVA = a.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = a.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = a.APPEZZA ")
            stb.AppendLine(" 		and a.Validita_Fine < st.aValiditaInizio ")
            stb.AppendLine("  ")
            stb.AppendLine(" --parte opzionale 1. (precessioni colturali) ")
            stb.AppendLine(" inner join Reg_Impianti i ")
            stb.AppendLine(" 	on i.piva = a.piva  ")
            stb.AppendLine(" 	and i.SA_COD = a.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese_Progetti ip ")
            stb.AppendLine(" 	on i.piva = ip.piva  ")
            stb.AppendLine(" 	and i.SA_COD = ip.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = ip.APPEZZA ")
            stb.AppendLine(" 	and i.ID_REG = ip.id_reg ")
            stb.AppendLine("  ")
            stb.AppendLine(" union 	 ")
            stb.AppendLine("  ")
            stb.AppendLine(" --seconda query: ricerca di impianti, distinte dell'appezzamento selezionato ")
            stb.AppendLine(" select distinct  ")
            stb.AppendLine(" 	 'Questo appezzamento' as [Risultato] ")
            stb.AppendLine(" 	, a.PIVA ")
            stb.AppendLine(" 	, a.SA_COD ")
            stb.AppendLine(" 	, a.APPEZZA ")
            stb.AppendLine(" 	, i.id_reg ")
            stb.AppendLine(" 	, ip.Progetto_Cod ")
            stb.AppendLine(" 	, ip.Progetto_Des ")
            stb.AppendLine(" 	, ip.Progetto_Nome	 ")
            stb.AppendLine(" 	, a.APP_NOME ")
            stb.AppendLine(" 	, i.CUL_COD  ")
            stb.AppendLine(" 	, i.Validita_Inizio as iValidita_inizio ")
            stb.AppendLine(" 	, i.Validita_Fine as iValidita_fine ")
            stb.AppendLine(" 	, a.Validita_Inizio as aValiditaInizio ")
            stb.AppendLine(" 	, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	, ip.Validita_Inizio as Validita_Inizio ")
            stb.AppendLine(" 	, ip.Validita_Fine as Validita_Fine ")
            stb.AppendLine("  ")
            stb.AppendLine(" from Appezzamento a ")
            stb.AppendLine(" 	 ")
            stb.AppendLine(" --parte opzionale 1. (precessioni colturali) ")
            stb.AppendLine(" inner join Reg_Impianti i ")
            stb.AppendLine(" 	on i.piva = a.piva  ")
            stb.AppendLine(" 	and i.SA_COD = a.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese_Progetti ip ")
            stb.AppendLine(" 	on i.piva = ip.piva  ")
            stb.AppendLine(" 	and i.SA_COD = ip.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = ip.APPEZZA ")
            stb.AppendLine(" 	and i.ID_REG = ip.id_reg ")
            stb.AppendLine(" 	 ")
            stb.AppendLine(" where a.PIVA = @piva  ")
            stb.AppendLine(" 	and a.SA_COD = @sa_cod ")
            stb.AppendLine(" 	and a.APPEZZA = @appezza ")
            stb.AppendLine("  ")
            stb.AppendLine(" ) Anag1 ")


            stb.AppendLine("  ")
            stb.AppendLine(" left join Anagrafe_VincoliAgronomici av ")
            stb.AppendLine(" 	on av.Piva=Anag1.PIVA and av.Sa_Cod=Anag1.SA_COD and av.Appezza=Anag1.APPEZZA ")

            stb.AppendLine(" left join Cultivar c ")
            stb.AppendLine(" 	on c.Cul_Cod = Anag1.CUL_COD ")
            stb.AppendLine(" left join SpecieVegetali v ")
            stb.AppendLine(" 	on v.Veg_Cod = c.veg_Cod ")

            stb.AppendLine(" left join Analisi_Testata at ")
            stb.AppendLine(" 	on at.Analisi_Testata_Cod=av.Analisi_Testata_Cod ")

            If puaCod <> 0 Then
                stb.AppendLine(" where NOT (av.pua_cod =" & puaCod & " and av.regolamento_cod=" & regolamentoCod & ")")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine(" order by av.Validita_Inizio desc ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiAppezzamentiConCatastoSovrapposto_PUALetamazioniPrecedenti(
        ByVal Piva As String,
        ByVal Sa_Cod As Int32,
        ByVal Appezza As Int32,
        ByVal puaCod As String,
        ByVal regolamentoCod As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiAppezzamentiConCatastoSovrapposto_PUALetamazioniPrecedenti()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            Dim stb As New Text.StringBuilder

            stb.AppendLine(" declare @piva varchar(50) ")
            stb.AppendLine(" declare @sa_cod int ")
            stb.AppendLine(" declare @appezza int ")
            stb.AppendLine("  ")
            stb.AppendLine(" set @piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine(" set @sa_cod = 	 " & Agro_SQL_SaveNum(Sa_Cod))
            stb.AppendLine(" set @appezza =  " & Agro_SQL_SaveNum(Appezza))
            stb.AppendLine("  ")

            'alcuni esempi..:
            stb.AppendLine("----esempio su aboca, sam, appezzamento ")
            stb.AppendLine(" --set @piva = '02969160544' ")
            stb.AppendLine(" --set @sa_cod = 130023425 ")
            stb.AppendLine(" --set @appezza = 130023461 ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine(" ----esempio su Genagricola, san giorgio ")
            stb.AppendLine(" --set @piva = '00570600320' ")
            stb.AppendLine(" --set @sa_cod = 114688005 ")
            stb.AppendLine(" --set @appezza = 114688003 ")
            stb.AppendLine("  ")
            stb.AppendLine(" ----esempio su groppi, coldiretti ")
            stb.AppendLine(" --set @piva = '01323380335' ")
            stb.AppendLine(" --set @sa_cod = 85327873  ")
            stb.AppendLine(" --set @appezza = 85328250 ")
            stb.AppendLine("  ")
            stb.AppendLine(" ----esempio su smartagri, biavati paolo ")
            stb.AppendLine(" --set @piva = '00171190549' ")
            stb.AppendLine(" --set @sa_cod = 131727361 ")
            stb.AppendLine(" --set @appezza = 131727369")

            'query
            stb.AppendLine(" select distinct   ")
            stb.AppendLine("       Risultato ")
            stb.AppendLine(" 	, anag1.PIVA ")
            stb.AppendLine(" 	, anag1.SA_COD ")
            stb.AppendLine(" 	, anag1.APPEZZA ")
            stb.AppendLine(" 	, anag1.id_reg ")
            stb.AppendLine(" 	, anag1.Progetto_Cod ")
            stb.AppendLine(" 	, Progetto_Des ")
            stb.AppendLine(" 	, Progetto_Nome ")
            stb.AppendLine(" 	, APP_NOME ")
            stb.AppendLine(" 	, isnull(c.Cul_Cod, 0) as cul_cod ")
            stb.AppendLine(" 	, isnull(v.Veg_Cod, 0) as veg_cod ")
            stb.AppendLine(" 	, isnull(c.Cul_des, 0) as cul_des ")
            stb.AppendLine(" 	, isnull(v.Veg_des, 0) as veg_des ")
            stb.AppendLine(" 	, iValidita_inizio ")
            stb.AppendLine(" 	, iValidita_fine ")
            stb.AppendLine(" 	, aValiditaInizio ")
            stb.AppendLine(" 	, aValiditaFine ")
            stb.AppendLine(" 	, anag1.Validita_Inizio ")
            stb.AppendLine(" 	, anag1.Validita_Fine ")
            stb.AppendLine(" 	, av.Eff_Cod ")
            stb.AppendLine(" 	, av.Id_Fre ")
            stb.AppendLine(" 	, av.Udm_Cod ")
            stb.AppendLine(" 	, av.Qta ")
            stb.AppendLine(" 	, av.N_Titolo")
            stb.AppendLine(" 	, av.pua_cod ")

            stb.AppendLine(" from ( ")
            stb.AppendLine("  ")
            stb.AppendLine(" --prima query: ricerca di appezzamenti 1 per ogni annata agraria (in base a catasto) ")
            stb.AppendLine(" select distinct  ")
            stb.AppendLine(" 	  'Altri appezzamenti' as [Risultato] ")
            stb.AppendLine(" 	, a.PIVA ")
            stb.AppendLine(" 	, a.SA_COD ")
            stb.AppendLine(" 	, a.APPEZZA ")
            stb.AppendLine(" 	, i.id_reg ")
            stb.AppendLine(" 	, ip.Progetto_Cod ")
            stb.AppendLine(" 	, ip.Progetto_Des ")
            stb.AppendLine(" 	, ip.Progetto_Nome	 ")
            stb.AppendLine(" 	, a.APP_NOME ")
            stb.AppendLine(" 	, i.CUL_COD ")
            stb.AppendLine(" 	, i.Validita_Inizio as iValidita_inizio ")
            stb.AppendLine(" 	, i.Validita_Fine as iValidita_fine ")
            stb.AppendLine(" 	, a.Validita_Inizio as aValiditaInizio ")
            stb.AppendLine(" 	, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	, ip.Validita_Inizio as Validita_Inizio ")
            stb.AppendLine(" 	, ip.Validita_Fine as Validita_Fine ")
            stb.AppendLine("  ")
            stb.AppendLine(" from ( ")
            stb.AppendLine(" 	select j1.*, a.Validita_Inizio as aValiditaInizio, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	from AppezzamentiXParticelle j1 ")
            stb.AppendLine(" 		inner join appezzamento a ")
            stb.AppendLine(" 		on  j1.PIVA = a.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = a.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" 	where j1.PIVA = @piva  ")
            stb.AppendLine(" 	and j1.SA_COD = @sa_cod ")
            stb.AppendLine(" 	and j1.APPEZZA = @appezza ")
            stb.AppendLine(" ) st ")
            stb.AppendLine(" inner join AppezzamentiXParticelle j1 ")
            stb.AppendLine(" 	on j1.prov = st.prov ")
            stb.AppendLine(" 	and j1.COM = st.COM ")
            stb.AppendLine(" 	and j1.SEZIONE = st.SEZIONE ")
            stb.AppendLine(" 	and j1.FOGLIO = st.FOGLIO ")
            stb.AppendLine(" 	and j1.NUMERO = st.NUMERO ")
            stb.AppendLine(" 	and j1.SUBALTERNO = st.SUBALTERNO ")
            stb.AppendLine(" 	and not ( ")
            stb.AppendLine(" 		    j1.PIVA = st.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = st.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = st.APPEZZA ")
            stb.AppendLine(" 	)		 ")
            stb.AppendLine(" inner join Appezzamento a ")
            stb.AppendLine(" 	on  ")
            stb.AppendLine(" 		    j1.PIVA = a.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = a.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = a.APPEZZA ")
            stb.AppendLine(" 		and a.Validita_Fine < st.aValiditaInizio ")
            stb.AppendLine("  ")
            stb.AppendLine(" --parte opzionale 1. (precessioni colturali) ")
            stb.AppendLine(" inner join Reg_Impianti i ")
            stb.AppendLine(" 	on i.piva = a.piva  ")
            stb.AppendLine(" 	and i.SA_COD = a.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese_Progetti ip ")
            stb.AppendLine(" 	on i.piva = ip.piva  ")
            stb.AppendLine(" 	and i.SA_COD = ip.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = ip.APPEZZA ")
            stb.AppendLine(" 	and i.ID_REG = ip.id_reg ")
            stb.AppendLine("  ")
            stb.AppendLine(" union 	 ")
            stb.AppendLine("  ")
            stb.AppendLine(" --seconda query: ricerca di impianti, distinte dell'appezzamento selezionato ")
            stb.AppendLine(" select distinct  ")
            stb.AppendLine(" 	 'Questo appezzamento' as [Risultato] ")
            stb.AppendLine(" 	, a.PIVA ")
            stb.AppendLine(" 	, a.SA_COD ")
            stb.AppendLine(" 	, a.APPEZZA ")
            stb.AppendLine(" 	, i.id_reg ")
            stb.AppendLine(" 	, ip.Progetto_Cod ")
            stb.AppendLine(" 	, ip.Progetto_Des ")
            stb.AppendLine(" 	, ip.Progetto_Nome	 ")
            stb.AppendLine(" 	, a.APP_NOME ")
            stb.AppendLine(" 	, i.CUL_COD  ")
            stb.AppendLine(" 	, i.Validita_Inizio as iValidita_inizio ")
            stb.AppendLine(" 	, i.Validita_Fine as iValidita_fine ")
            stb.AppendLine(" 	, a.Validita_Inizio as aValiditaInizio ")
            stb.AppendLine(" 	, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	, ip.Validita_Inizio as Validita_Inizio ")
            stb.AppendLine(" 	, ip.Validita_Fine as Validita_Fine ")
            stb.AppendLine("  ")
            stb.AppendLine(" from Appezzamento a ")
            stb.AppendLine(" 	 ")
            stb.AppendLine(" --parte opzionale 1. (precessioni colturali) ")
            stb.AppendLine(" inner join Reg_Impianti i ")
            stb.AppendLine(" 	on i.piva = a.piva  ")
            stb.AppendLine(" 	and i.SA_COD = a.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese_Progetti ip ")
            stb.AppendLine(" 	on i.piva = ip.piva  ")
            stb.AppendLine(" 	and i.SA_COD = ip.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = ip.APPEZZA ")
            stb.AppendLine(" 	and i.ID_REG = ip.id_reg ")
            stb.AppendLine(" 	 ")
            stb.AppendLine(" where a.PIVA = @piva  ")
            stb.AppendLine(" 	and a.SA_COD = @sa_cod ")
            stb.AppendLine(" 	and a.APPEZZA = @appezza ")
            stb.AppendLine("  ")
            stb.AppendLine(" ) Anag1 ")


            stb.AppendLine("  ")
            stb.AppendLine(" left join PUA_LetamazioniPrecedenti av ")
            stb.AppendLine(" 	on av.Piva=Anag1.PIVA and av.Sa_Cod=Anag1.SA_COD and av.Appezza=Anag1.APPEZZA ")

            stb.AppendLine(" left join Cultivar c ")
            stb.AppendLine(" 	on c.Cul_Cod = Anag1.CUL_COD ")
            stb.AppendLine(" left join SpecieVegetali v ")
            stb.AppendLine(" 	on v.Veg_Cod = c.veg_Cod ")

            If puaCod <> 0 Then
                stb.AppendLine(" where NOT (av.pua_cod =" & puaCod & " and av.regolamento_cod=" & regolamentoCod & ")")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine(" order by av.eff_cod,  id_fre ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiAppezzamentiConCatastoSovrapposto_LetamazioniPrecedenti(
        ByVal Piva As String,
        ByVal Sa_Cod As Int32,
        ByVal Appezza As Int32,
        ByVal xFiltroAggiuntivo_Movimenti As String,
        ByVal xFiltroAggiuntivo_Movimenti_Dettagli As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiAppezzamentiConCatastoSovrapposto_LetamazioniPrecedenti()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            Dim stb As New Text.StringBuilder

            stb.AppendLine(" declare @piva varchar(50) ")
            stb.AppendLine(" declare @sa_cod int ")
            stb.AppendLine(" declare @appezza int ")
            stb.AppendLine("  ")
            stb.AppendLine(" set @piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine(" set @sa_cod = 	 " & Agro_SQL_SaveNum(Sa_Cod))
            stb.AppendLine(" set @appezza =  " & Agro_SQL_SaveNum(Appezza))
            stb.AppendLine("  ")

            ''alcuni esempi..:
            'stb.AppendLine("----esempio su aboca, sam, appezzamento ")
            'stb.AppendLine(" --set @piva = '02969160544' ")
            'stb.AppendLine(" --set @sa_cod = 130023425 ")
            'stb.AppendLine(" --set @appezza = 130023461 ")
            'stb.AppendLine("  ")
            'stb.AppendLine("  ")
            'stb.AppendLine(" ----esempio su Genagricola, san giorgio ")
            'stb.AppendLine(" --set @piva = '00570600320' ")
            'stb.AppendLine(" --set @sa_cod = 114688005 ")
            'stb.AppendLine(" --set @appezza = 114688003 ")
            'stb.AppendLine("  ")
            'stb.AppendLine(" ----esempio su groppi, coldiretti ")
            'stb.AppendLine(" --set @piva = '01323380335' ")
            'stb.AppendLine(" --set @sa_cod = 85327873  ")
            'stb.AppendLine(" --set @appezza = 85328250 ")
            'stb.AppendLine("  ")
            'stb.AppendLine(" ----esempio su smartagri, biavati paolo ")
            'stb.AppendLine(" --set @piva = '00171190549' ")
            'stb.AppendLine(" --set @sa_cod = 131727361 ")
            'stb.AppendLine(" --set @appezza = 131727369")

            'query
            stb.AppendLine(" select distinct   ")
            stb.AppendLine("       Risultato ")
            stb.AppendLine(" 	, anag1.PIVA ")
            stb.AppendLine(" 	, anag1.SA_COD ")
            stb.AppendLine(" 	, anag1.APPEZZA ")
            stb.AppendLine(" 	, anag1.id_reg ")
            stb.AppendLine(" 	, anag1.Progetto_Cod ")
            stb.AppendLine(" 	, Progetto_Des ")
            stb.AppendLine(" 	, Progetto_Nome ")
            stb.AppendLine(" 	, APP_NOME ")
            stb.AppendLine(" 	, case when  anag1.cul_cod=0 and d.DestUso is null ")
            stb.AppendLine(" 	           then 'Terreno Nudo' ")
            stb.AppendLine(" 	       when  anag1.cul_cod=0 and d.DestUso is not null ")
            stb.AppendLine(" 	           then d.DestUso ")
            stb.AppendLine(" 	       else v.Veg_Des ")
            stb.AppendLine(" 	  end as veg_des ")
            stb.AppendLine(" 	, anag1.cul_cod ")
            'stb.AppendLine(" 	, isnull(c.Cul_Cod, 0) as cul_cod ")
            stb.AppendLine(" 	, isnull(v.Veg_Cod, 0) as veg_cod ")
            stb.AppendLine(" 	, isnull(c.Cul_des, 0) as cul_des ")
            'stb.AppendLine(" 	, isnull(v.Veg_des, 0) as veg_des ")
            stb.AppendLine(" 	, iValidita_inizio ")
            stb.AppendLine(" 	, iValidita_fine ")
            stb.AppendLine(" 	, aValiditaInizio ")
            stb.AppendLine(" 	, aValiditaFine ")
            stb.AppendLine(" 	, anag1.Validita_Inizio ")
            stb.AppendLine(" 	, anag1.Validita_Fine ")

            stb.AppendLine(" 	, m.id_agenda ")
            stb.AppendLine(" 	, m.data_movimento ")
            stb.AppendLine(" 	, (mds.qta / mds.qta2) * mdt.n /100 as N_HA_App_Fert ")
            stb.AppendLine(" 	, mds.qta as qta_dest, mds.qta2 ")
            stb.AppendLine(" 	, md.pro_cod , md.qta, md.extra_int, md.udm_cod ")
            stb.AppendLine(" 	, mdt.n, mdt.efficienza")

            stb.AppendLine(" from ( ")
            stb.AppendLine("  ")
            stb.AppendLine(" --prima query: ricerca di appezzamenti 1 per ogni annata agraria (in base a catasto) ")
            stb.AppendLine(" select distinct  ")
            stb.AppendLine(" 	  'Altri appezzamenti' as [Risultato] ")
            stb.AppendLine(" 	, a.PIVA ")
            stb.AppendLine(" 	, a.SA_COD ")
            stb.AppendLine(" 	, a.APPEZZA ")
            stb.AppendLine(" 	, i.id_reg ")
            stb.AppendLine(" 	, ip.Progetto_Cod ")
            stb.AppendLine(" 	, ip.Progetto_Des ")
            stb.AppendLine(" 	, ip.Progetto_Nome	 ")
            stb.AppendLine(" 	, a.APP_NOME ")
            stb.AppendLine(" 	, i.CUL_COD ")
            stb.AppendLine(" 	, i.Validita_Inizio as iValidita_inizio ")
            stb.AppendLine(" 	, i.Validita_Fine as iValidita_fine ")
            stb.AppendLine(" 	, a.Validita_Inizio as aValiditaInizio ")
            stb.AppendLine(" 	, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	, ip.Validita_Inizio as Validita_Inizio ")
            stb.AppendLine(" 	, ip.Validita_Fine as Validita_Fine ")
            stb.AppendLine("  ")
            stb.AppendLine(" from ( ")
            stb.AppendLine(" 	select j1.*, a.Validita_Inizio as aValiditaInizio, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	from AppezzamentiXParticelle j1 ")
            stb.AppendLine(" 		inner join appezzamento a ")
            stb.AppendLine(" 		on  j1.PIVA = a.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = a.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" 	where j1.PIVA = @piva  ")
            stb.AppendLine(" 	and j1.SA_COD = @sa_cod ")
            stb.AppendLine(" 	and j1.APPEZZA = @appezza ")
            stb.AppendLine(" ) st ")
            stb.AppendLine(" inner join AppezzamentiXParticelle j1 ")
            stb.AppendLine(" 	on j1.prov = st.prov ")
            stb.AppendLine(" 	and j1.COM = st.COM ")
            stb.AppendLine(" 	and j1.SEZIONE = st.SEZIONE ")
            stb.AppendLine(" 	and j1.FOGLIO = st.FOGLIO ")
            stb.AppendLine(" 	and j1.NUMERO = st.NUMERO ")
            stb.AppendLine(" 	and j1.SUBALTERNO = st.SUBALTERNO ")
            stb.AppendLine(" 	and not ( ")
            stb.AppendLine(" 		    j1.PIVA = st.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = st.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = st.APPEZZA ")
            stb.AppendLine(" 	)		 ")
            stb.AppendLine(" inner join Appezzamento a ")
            stb.AppendLine(" 	on  ")
            stb.AppendLine(" 		    j1.PIVA = a.PIVA ")
            stb.AppendLine(" 		and j1.SA_COD = a.SA_COD ")
            stb.AppendLine(" 		and j1.APPEZZA = a.APPEZZA ")
            stb.AppendLine(" 		and a.Validita_Fine < st.aValiditaInizio ")
            stb.AppendLine("  ")
            stb.AppendLine(" --parte opzionale 1. (precessioni colturali) ")
            stb.AppendLine(" inner join Reg_Impianti i ")
            stb.AppendLine(" 	on i.piva = a.piva  ")
            stb.AppendLine(" 	and i.SA_COD = a.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese_Progetti ip ")
            stb.AppendLine(" 	on i.piva = ip.piva  ")
            stb.AppendLine(" 	and i.SA_COD = ip.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = ip.APPEZZA ")
            stb.AppendLine(" 	and i.ID_REG = ip.id_reg ")
            stb.AppendLine("  ")
            stb.AppendLine(" union 	 ")
            stb.AppendLine("  ")
            stb.AppendLine(" --seconda query: ricerca di impianti, distinte dell'appezzamento selezionato ")
            stb.AppendLine(" select distinct  ")
            stb.AppendLine(" 	 'Questo appezzamento' as [Risultato] ")
            stb.AppendLine(" 	, a.PIVA ")
            stb.AppendLine(" 	, a.SA_COD ")
            stb.AppendLine(" 	, a.APPEZZA ")
            stb.AppendLine(" 	, i.id_reg ")
            stb.AppendLine(" 	, ip.Progetto_Cod ")
            stb.AppendLine(" 	, ip.Progetto_Des ")
            stb.AppendLine(" 	, ip.Progetto_Nome	 ")
            stb.AppendLine(" 	, a.APP_NOME ")
            stb.AppendLine(" 	, i.CUL_COD  ")
            stb.AppendLine(" 	, i.Validita_Inizio as iValidita_inizio ")
            stb.AppendLine(" 	, i.Validita_Fine as iValidita_fine ")
            stb.AppendLine(" 	, a.Validita_Inizio as aValiditaInizio ")
            stb.AppendLine(" 	, a.Validita_Fine as aValiditaFine ")
            stb.AppendLine(" 	, ip.Validita_Inizio as Validita_Inizio ")
            stb.AppendLine(" 	, ip.Validita_Fine as Validita_Fine ")
            stb.AppendLine("  ")
            stb.AppendLine(" from Appezzamento a ")
            stb.AppendLine(" 	 ")
            stb.AppendLine(" --parte opzionale 1. (precessioni colturali) ")
            stb.AppendLine(" inner join Reg_Impianti i ")
            stb.AppendLine(" 	on i.piva = a.piva  ")
            stb.AppendLine(" 	and i.SA_COD = a.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = a.APPEZZA ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese_Progetti ip ")
            stb.AppendLine(" 	on i.piva = ip.piva  ")
            stb.AppendLine(" 	and i.SA_COD = ip.SA_COD ")
            stb.AppendLine(" 	and i.APPEZZA = ip.APPEZZA ")
            stb.AppendLine(" 	and i.ID_REG = ip.id_reg ")
            stb.AppendLine(" 	 ")
            stb.AppendLine(" where a.PIVA = @piva  ")
            stb.AppendLine(" 	and a.SA_COD = @sa_cod ")
            stb.AppendLine(" 	and a.APPEZZA = @appezza ")
            stb.AppendLine("  ")
            stb.AppendLine(" ) Anag1 ")


            stb.AppendLine("  ")
            stb.AppendLine(" inner join mov_destinazioni mds ")
            stb.AppendLine(" 	on mds.Piva=Anag1.PIVA and mds.Sa_Cod=Anag1.SA_COD and mds.Appezza=Anag1.APPEZZA  and mds.id_destinazione=Anag1.id_reg ")

            stb.AppendLine(" inner join movimenti_dettagli md  ")
            stb.AppendLine(" 	on  md.id_agenda=mds.id_agenda and md.id_mov=mds.id_mov  and md.id_mov_det=mds.id_mov_det ")

            If xFiltroAggiuntivo_Movimenti_Dettagli <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_Movimenti_Dettagli, , objParametri))
            End If

            stb.AppendLine(" inner join mov_dettaglio_tecnico mdt  ")
            stb.AppendLine(" 	on  md.id_agenda=mdt.id_agenda and md.id_mov=mdt.id_mov  and md.id_mov_det=mdt.id_mov_det ")

            stb.AppendLine(" inner join movimenti m ")
            stb.AppendLine(" 	on  md.id_agenda=m.id_agenda and md.id_mov=m.id_mov ")

            If xFiltroAggiuntivo_Movimenti <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_Movimenti, , objParametri))
            End If

            stb.AppendLine(" left join Cultivar c ")
            stb.AppendLine(" 	on c.Cul_Cod = Anag1.CUL_COD ")
            stb.AppendLine(" left join SpecieVegetali v ")
            stb.AppendLine(" 	on v.Veg_Cod = c.veg_Cod ")

            stb.AppendLine(" left join ( ")
            stb.AppendLine(" 	select distinct piva, sa_cod, appezza, id_reg, a.codice, a.descrizione as DestUso ")
            stb.AppendLine(" 	from Reg_Impianti_Codici c ")
            stb.AppendLine(" 		inner join Codici_Anagrafe a ")
            stb.AppendLine(" 			on c.id_cod = a.codice ")
            stb.AppendLine(" 	where id_cod >= 3000 and id_cod < 4000 ")
            stb.AppendLine(" ) d ")
            stb.AppendLine(" on      Anag1.piva = d.piva  ")
            stb.AppendLine(" 	and Anag1.SA_COD = d.SA_COD ")
            stb.AppendLine(" 	and Anag1.APPEZZA = d.APPEZZA ")
            stb.AppendLine(" 	and Anag1.id_reg = d.id_reg")



            'filtri spostati dentro la query
            'stb.AppendLine(" where m.cau_mov ='" & CAU_LAVORAZIONE & "' ")

            'If xFiltroAggiuntivo <> "" Then
            '    stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.Append(" ORDER BY m.data_movimento desc")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiPerTerritorio(ByRef stb As Text.StringBuilder,
                                       ByVal Piva As String,
                                       ByVal Sa_Cod As Int32,
                                       ByVal Appezza As Int32,
                                       ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiPerTerritorio()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            Dim standAlone As Boolean = False

            If stb Is Nothing Then
                stb = New Text.StringBuilder
                standAlone = True
            End If


            stb.AppendLine("     Select  ")
            stb.AppendLine("         Case when Not cat.PIVA Is null then cat.PIVA else  ")
            stb.AppendLine("          Case when Not ind.PIVA Is null then ind.PIVA else  ")
            stb.AppendLine("              Case when Not indC.PIVA Is null then indC.PIVA  ")
            stb.AppendLine("              End ")
            stb.AppendLine("                     End ")
            stb.AppendLine("                     End As Piva ")
            stb.AppendLine("      , case when Not cat.SA_COD  Is null then cat.SA_COD else  ")
            stb.AppendLine("          Case when Not ind.SA_COD Is null then ind.SA_COD else  ")
            stb.AppendLine("              Case when Not indC.SA_COD Is null then indC.SA_COD  ")
            stb.AppendLine("              End ")
            stb.AppendLine("                     End ")
            stb.AppendLine("                     End As sa_cod  ")
            stb.AppendLine("      , case when Not cat.APPEZZA Is null then cat.APPEZZA else  ")
            stb.AppendLine("          Case when Not ind.APPEZZA Is null then ind.APPEZZA else  ")
            stb.AppendLine("              Case when Not indC.APPEZZA Is null then indC.APPEZZA  ")
            stb.AppendLine("              End ")
            stb.AppendLine("                     End ")
            stb.AppendLine("                     End As Appezza ")
            stb.AppendLine("      , case when Not cat.PROV Is null then cat.PROV else  ")
            stb.AppendLine("          Case when Not ind.APPEZZA Is null then ind.PROV else  ")
            stb.AppendLine("              Case when Not indC.PROV Is null then indC.PROV  ")
            stb.AppendLine("              End ")
            stb.AppendLine("                     End ")
            stb.AppendLine("                     End As PROV ")
            stb.AppendLine("      , case when Not cat.COM Is null then cat.COM else  ")
            stb.AppendLine("          Case when Not ind.COM Is null then ind.COM else  ")
            stb.AppendLine("              Case when Not indC.COM Is null then indC.COM  ")
            stb.AppendLine("              End ")
            stb.AppendLine("                     End ")
            stb.AppendLine("                     End As COM ")
            stb.AppendLine("      , case when Not cat.AREA Is null then cat.AREA else  ")
            stb.AppendLine("          Case when Not ind.AREA Is null then ind.AREA else  ")
            stb.AppendLine("              Case when Not indC.AREA Is null then indC.AREA  ")
            stb.AppendLine("              End ")
            stb.AppendLine("                     End ")
            stb.AppendLine("                     End As AREA ")
            stb.AppendLine("  ")
            stb.AppendLine("  From Appezzamento app ")
            stb.AppendLine("  ")
            stb.AppendLine("  Left Join( ")
            stb.AppendLine("  ")
            stb.AppendLine("     Select ")
            stb.AppendLine("         Piva, SA_COD, APPEZZA, ")
            stb.AppendLine("         PROV, COM, ")
            stb.AppendLine("         SUM(AREA) AS AREA  ")
            stb.AppendLine("     From AppezzamentiXParticelle ")
            stb.AppendLine("     group by  ")
            stb.AppendLine("         Piva, SA_COD, APPEZZA, ")
            stb.AppendLine("         PROV, COM ")
            stb.AppendLine("  ) cat ")
            stb.AppendLine("  ")
            stb.AppendLine("  On cat.PIVA = app.PIVA  ")
            stb.AppendLine("  And cat.SA_COD = app.SA_COD  ")
            stb.AppendLine("  And cat.APPEZZA = app.APPEZZA  ")
            stb.AppendLine("  ")
            stb.AppendLine("  Left Join( ")
            stb.AppendLine("  ")
            stb.AppendLine("     Select ")
            stb.AppendLine("     a.PIVA, a.SA_COD, a.APPEZZA, ")
            stb.AppendLine("     ind.pro_cod_istat As prov, ind.com_cod_istat As com, ")
            stb.AppendLine("     0 as AREA ")
            stb.AppendLine("  From Appezzamento a  ")
            stb.AppendLine("       ")

            stb.AppendLine(" inner Join ( ")
            stb.AppendLine("    select PIVA, sa_cod, appezza, MIN(cod_indirizzo) as cod_indirizzo ")
            stb.AppendLine("    From AppezzamentixIndirizzi ")
            stb.AppendLine("    Group By Piva, sa_cod, appezza  ")
            stb.AppendLine(" )  ai")

            stb.AppendLine("              On ai.PIVA = a.PIVA  ")
            stb.AppendLine("          And ai.SA_COD = a.SA_COD ")
            stb.AppendLine("          And ai.APPEZZA = a.APPEZZA ")
            stb.AppendLine("      inner Join Indirizzi ind ")
            stb.AppendLine("             On ind.cod_indirizzo = ai.cod_indirizzo ")
            stb.AppendLine("  ) ind     ")
            stb.AppendLine("  ")
            stb.AppendLine("   ")
            stb.AppendLine("  On ind.PIVA = app.PIVA  ")
            stb.AppendLine("  And ind.SA_COD = app.SA_COD  ")
            stb.AppendLine("  And ind.APPEZZA = app.APPEZZA  ")
            stb.AppendLine("  ")
            stb.AppendLine("  Left Join( ")
            stb.AppendLine("  ")
            stb.AppendLine("     Select ")
            stb.AppendLine("     a.PIVA, a.SA_COD, a.APPEZZA, ")
            stb.AppendLine("     ind.pro_cod_istat As prov, ind.com_cod_istat As com, ")
            stb.AppendLine("     0 as AREA ")
            stb.AppendLine("  From Appezzamento a  ")
            stb.AppendLine("           ")

            stb.AppendLine("   inner Join( ")
            stb.AppendLine("          select PIVA, sa_cod, MIN(cod_indirizzo) as cod_indirizzo ")
            stb.AppendLine("             From CentrixIndirizzi ")
            stb.AppendLine("             Group By Piva, sa_cod  ")
            stb.AppendLine("      )   ai")


            stb.AppendLine("              On ai.PIVA = a.PIVA  ")
            stb.AppendLine("          And ai.SA_COD = a.SA_COD ")
            stb.AppendLine("       ")
            stb.AppendLine("      inner Join Indirizzi ind ")
            stb.AppendLine("             On ind.cod_indirizzo = ai.cod_indirizzo ")
            stb.AppendLine("  ) indC    ")
            stb.AppendLine("   ")
            stb.AppendLine("  On indC.PIVA = app.PIVA  ")
            stb.AppendLine("  And indC.SA_COD = app.SA_COD  ")
            stb.AppendLine("  And indC.APPEZZA = app.APPEZZA  ")

            If standAlone Then
                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            Else
                dt = Nothing
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiConCodiceCliente(ByVal Piva As String,
                                          ByVal Sa_Cod As Int32,
                                          ByVal Appezza As Int32,
                                          ByVal id_cod_cliente_2000 As Integer,
                                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiConCodiceCliente()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim dt As DataTable

        If id_cod_cliente_2000 < 2000 OrElse id_cod_cliente_2000 > 2999 Then
            Throw New Exception("codice cliente deve essere compreso fra 2000 e 2999")
        End If

        Try
            stb.AppendLine("  Select ")
            stb.AppendLine("       a.piva + '-' + cast(a.sa_cod as varchar(50)) + '-' + cast(a.Appezza as varchar(50)) as chiave ")
            stb.AppendLine("     , a.piva ")
            stb.AppendLine("     , a.sa_cod ")
            stb.AppendLine("     , a.appezza ")
            stb.AppendLine("     , app_nome ")
            stb.AppendLine("     , sup_App ")
            stb.AppendLine("     , isNull(rif.val_cod, '') as RiferimentoAlfanumerico ")
            stb.AppendLine("     , isNull(ac.val_Cod, '') as Val_Cod ")
            stb.AppendLine("     , a.data_modifica ")
            stb.AppendLine(" From Appezzamento a ")
            stb.AppendLine("     left Join Appezzamento_Codici ac ")
            stb.AppendLine("         On a.piva = ac.piva ")
            stb.AppendLine("         And a.SA_COD = ac.sa_cod ")
            stb.AppendLine("         And a.APPEZZA = ac.appezza ")
            stb.AppendLine("         And ac.id_cod = " & Agro_SQL_SaveNum(id_cod_cliente_2000))
            stb.AppendLine("     Left Join Appezzamento_Codici rif ")
            stb.AppendLine("         On a.piva = rif.piva ")
            stb.AppendLine("         And a.SA_COD = rif.sa_cod ")
            stb.AppendLine("         And a.APPEZZA = rif.appezza ")
            stb.AppendLine("         And rif.id_cod = 1104")

            stb.AppendLine("   where a.piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                stb.AppendLine("   and a.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                stb.AppendLine("   and a.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
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
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Appezza As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal Leggi_Cartografia As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Piva, Sa_Cod, Appezza, Campo_Cod, App_Nome, Sup_App, Validita_Inizio, Validita_Fine ")
                    StrSQL.Append(" FROM  Appezzamento ")
                    StrSQL.Append(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Appezzamento.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Appezzamento.* ")

                    If Leggi_Cartografia Then
                        StrSQL.AppendLine("     , ISNULL (g.Poligono_GeoEntity.STAsText(), '') as cartografia")
                        StrSQL.AppendLine("     , COALESCE(g.Poligono_GeoEntity.STArea()/10000, 0) as SuperficieGis")
                    End If

                    StrSQL.AppendLine(" FROM  Appezzamento ")
                    StrSQL.AppendLine("")

                    If Leggi_Cartografia Then
                        StrSQL.AppendLine(" LEFT JOIN gis_entita e ON Appezzamento.piva = e.piva ")
                        StrSQL.AppendLine("     AND Appezzamento.sa_cod = e.sa_cod ")
                        StrSQL.AppendLine("     AND Appezzamento.appezza = e.appezza ")
                        StrSQL.AppendLine("     AND e.TipoEntita_Cod = " & enum_GIS2012_TipoEntita.APPEZZAMENTI & " ")
                        StrSQL.AppendLine("     AND e.Id_Imp = 0 ")
                        StrSQL.AppendLine("")
                        StrSQL.AppendLine(" LEFT JOIN gis_elementigrafici g ON e.PivaSuperUser = g.PivaSuperUser ")
                        StrSQL.AppendLine("     AND e.entita_cod = g.Entita_Cod ")
                        StrSQL.AppendLine("     AND g.LayerElementiGrafici_Cod = 1")
                        StrSQL.AppendLine("")
                    End If

                    StrSQL.AppendLine(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("     AND Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.AppendLine("     AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine("     AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine("     AND Appezzamento.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine("     AND Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine("     AND Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.AppendLine("SELECT Imprese.Rag_Soc, Centri_Aziendali.Sa_Nome, Appezzamento.*")

                    StrSQL.AppendLine("FROM Imprese INNER JOIN")
                    StrSQL.AppendLine("Centri_Aziendali ON Imprese.Piva = Centri_Aziendali.Piva ")
                    StrSQL.AppendLine("INNER JOIN Appezzamento ON Appezzamento.PIVA = Centri_Aziendali.PIVA")
                    StrSQL.AppendLine("AND Appezzamento.sa_cod = Centri_Aziendali.sa_cod")

                    StrSQL.AppendLine(" WHERE Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND    Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND     Appezzamento.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND     Appezzamento.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc, Sa_Nome, App_nome ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" select app.* ")
                    StrSQL.AppendLine(" , ( ")
                    StrSQL.AppendLine("     Select * from(select StaticMap as '*') Tbl ")
                    StrSQL.AppendLine("     For Xml path('') ")
                    StrSQL.AppendLine(" ) StaticMapBase64String")
                    StrSQL.AppendLine(", ISNULL (g.Poligono_GeoEntity.STAsText(), '') as cartografia")


                    StrSQL.AppendLine(" From Appezzamento app ")
                    StrSQL.AppendLine(" Left Join gis_entita e ")
                    StrSQL.AppendLine("     On  app.piva = e.piva ")
                    StrSQL.AppendLine("     And app.sa_cod = e.sa_cod ")
                    StrSQL.AppendLine("     And app.appezza = e.appezza    ")
                    StrSQL.AppendLine("     AND e.TipoEntita_Cod = 1    ")
                    StrSQL.AppendLine("     AND e.Id_Imp = 0    ")
                    StrSQL.AppendLine(" Left Join gis_elementigrafici g ")
                    StrSQL.AppendLine("     On   e.PivaSuperUser = g.PivaSuperUser ")
                    StrSQL.AppendLine("     And  e.entita_cod = g.Entita_Cod ")
                    StrSQL.AppendLine("     And g.LayerElementiGrafici_Cod = 1")

                    StrSQL.AppendLine(" WHERE App.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   App.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND App.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND App.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND App.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   App.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   App.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

            End Select

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

    Public Function Leggi_x_GUID(ByVal GUID As String,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Piva, sa_Cod, Appezza, Via_Stringa  ")

            StrSQL.AppendLine(" FROM  Appezzamento ")
            StrSQL.AppendLine("")

            StrSQL.AppendLine(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("     AND Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If GUID <> "" Then
                StrSQL.AppendLine("     AND Appezzamento.Via_Stringa = '" & Agro_SQL_SaveText(GUID) & "' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine("     AND Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine("     AND Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_x_Sincronizzatore_WCF(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Veg_Cod As String,
                                                ByVal Cul_Cod As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi_x_Sincronizzatore_WCF()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT        Appezzamento.SUP_APP, Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, Imprese_Progetti.Progetto_Cod, ")
            StrSQL.Append("        Appezzamento.APP_NOME, Cultivar.Cul_Des, SpecieVegetali.Veg_Des, Imprese_Progetti.Cod_Contratto, Imprese_Progetti.Progetto_Nome, ")
            StrSQL.Append("        Imprese_Progetti.Progetto_Des, ")
            StrSQL.Append("        (SELECT        val_cod FROM Appezzamento_Codici ")
            StrSQL.Append("              WHERE        PIVA = Appezzamento.PIVA AND sa_cod = Appezzamento.SA_COD AND appezza = Appezzamento.APPEZZA AND id_cod = 1073) AS Codice_Contratto, ")
            StrSQL.Append("         Imprese_Progetti.Validita_Inizio  as Validita_Inizio_Distinta , Imprese_Progetti.Validita_Fine as Validita_Fine_Distinta , Reg_Impianti.Sup_Imp , Reg_Impianti.validita_inizio as Data_Inizio_Impianto ")

            StrSQL.Append(" FROM            Cultivar INNER JOIN ")
            StrSQL.Append("        SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            StrSQL.Append("        Appezzamento INNER JOIN ")
            StrSQL.Append("        Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
            StrSQL.Append("        Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN ")
            StrSQL.Append("        Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND ")
            StrSQL.Append("        Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")

            StrSQL.Append(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND   Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Appezzamento.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND SpecieVegetali.veg_cod  = " & Veg_Cod & " ")
            End If

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti.Cul_Cod  = " & Cul_Cod & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_x_Sincronizzatore_WCF(ByVal Piva As String,
                                                ByVal sa_cod As Integer,
                                                ByVal Appezza As Integer,
                                                ByVal Id_reg As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi_x_Sincronizzatore_WCF()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT        Appezzamento.SUP_APP, Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, Imprese_Progetti.Progetto_Cod, ")
            StrSQL.Append("        Appezzamento.APP_NOME, Cultivar.Cul_Des, SpecieVegetali.Veg_Des, Imprese_Progetti.Cod_Contratto, Imprese_Progetti.Progetto_Nome, ")
            StrSQL.Append("        Imprese_Progetti.Progetto_Des, ")
            StrSQL.Append("        (SELECT        val_cod FROM Appezzamento_Codici ")
            StrSQL.Append("              WHERE        PIVA = Appezzamento.PIVA AND sa_cod = Appezzamento.SA_COD AND appezza = Appezzamento.APPEZZA AND id_cod = 1073) AS Codice_Contratto, ")
            StrSQL.Append("         Imprese_Progetti.Validita_Inizio  as Validita_Inizio_Distinta , Imprese_Progetti.Validita_Fine as Validita_Fine_Distinta , Reg_Impianti.Sup_Imp , Reg_Impianti.validita_inizio as Data_Inizio_Impianto ")

            StrSQL.Append("         , (select reg_des from regolamenti where reg_cod = regolamento_cod) as reg_des ")
            StrSQL.Append("         , isnull((select nomeesteso from DPI_Regolamenti d where d.flag_privato_pubblico = Disciplinare_PubblicoPrivato and d.cod_regolamento = Disciplinare_Cod),'Nessun Disciplinare') as disciplinare ")


            StrSQL.Append(" FROM            Cultivar INNER JOIN ")
            StrSQL.Append("        SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            StrSQL.Append("        Appezzamento INNER JOIN ")
            StrSQL.Append("        Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
            StrSQL.Append("        Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN ")
            StrSQL.Append("        Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND ")
            StrSQL.Append("        Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")

            StrSQL.Append(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND   Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If sa_cod <> 0 Then
                StrSQL.Append(" AND Appezzamento.sa_cod   = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Appezzamento.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti.Id_reg   = " & Agro_SQL_SaveNum(Id_reg) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################################################
    Public Function Leggi_x_anagrafica(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Campo_Cod As Integer,
                                       ByVal Appezza As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi_x_anagrafica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" Select   Appezzamento.PIVA + '_' + CAST(Appezzamento.SA_COD AS nvarchar(10)) + '_' + CAST(Appezzamento.APPEZZA AS nvarchar(10)) AS chiave,Appezzamento.Validazione,  ")
            StrSQL.AppendLine(" Appezzamento.PIVA as Piva, Appezzamento.SA_COD AS Sa_Cod, Appezzamento.APPEZZA As Appezza,  ")
            StrSQL.AppendLine("     Appezzamento.APP_NOME, Appezzamento.SUP_APP, Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine, Centri_Aziendali.sa_nome, ")
            StrSQL.AppendLine("   Appezzamento.Blk_Flag ,(select [User] from utenti where CODICE_FISCALE = Appezzamento.Username_Modifica ) as utente_modifica , Appezzamento.Data_Modifica, ")
            StrSQL.AppendLine("   (select [User] from utenti where CODICE_FISCALE = Appezzamento.Username_Creazione ) as utente_creazione , Appezzamento.Data_Creazione, ")
            StrSQL.AppendLine("   Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome, ")
            StrSQL.AppendLine("   Campi.Campo_Cod, Campi.Campo_Des ")

            StrSQL.AppendLine(" , ISNULL(rif_app.val_cod, '') AS rif_alfanumerico ")
            StrSQL.AppendLine(" , ISNULL(appBio.val_cod, '') AS cod_biologico ")
            StrSQL.AppendLine(" , ISNULL(isola.val_cod, '') AS isola ")
            StrSQL.AppendLine(" , ISNULL(metodoProd.val_cod, '1') AS MetodoProduzione_Cod ")
            StrSQL.AppendLine(" , CASE WHEN metodoProd.val_cod IS NULL THEN 'Integrato'  ")
            StrSQL.AppendLine("        WHEN metodoProd.val_cod = '1' THEN 'Integrato' ")
            StrSQL.AppendLine("        WHEN metodoProd.val_cod = '2' THEN 'In Conversione' ")
            StrSQL.AppendLine("        WHEN metodoProd.val_cod = '3' THEN 'Biologico' ")
            StrSQL.AppendLine("        ELSE 'Integrato' END AS MetodoProduzione_Des ")

            StrSQL.AppendLine(" FROM Appezzamento   ")
            StrSQL.AppendLine(" INNER JOIN Centri_Aziendali ON Appezzamento.PIVA = Centri_Aziendali.PIVA AND Appezzamento.SA_COD = Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine(" LEFT JOIN Campi ON Appezzamento.PIVA = Campi.PIVA AND Appezzamento.SA_COD = Campi.sa_cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici rif_app ON Appezzamento.PIVA = rif_app.PIVA AND Appezzamento.SA_COD = rif_app.sa_cod AND Appezzamento.APPEZZA = rif_app.appezza AND rif_app.id_cod = 1104 ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici appBio ON Appezzamento.PIVA = appBio.PIVA AND Appezzamento.SA_COD = appBio.sa_cod AND Appezzamento.APPEZZA = appBio.appezza AND appBio.id_cod = 1085 ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici isola ON Appezzamento.PIVA = isola.PIVA AND Appezzamento.SA_COD = isola.sa_cod AND Appezzamento.APPEZZA = isola.appezza AND isola.id_cod = 1320 ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici metodoProd ON Appezzamento.PIVA = metodoProd.PIVA AND Appezzamento.SA_COD = metodoProd.sa_cod AND Appezzamento.APPEZZA = metodoProd.appezza AND metodoProd.id_cod = 1018 ")

            StrSQL.AppendLine(" where Appezzamento.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" and Appezzamento.validita_fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim filtroCentri As String = ""
                Dim dtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Piva & "'", "", objParametri)
                If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To dtCentriVisibili.Rows.Count - 1
                        filtroCentri &= dtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If filtroCentri <> "" Then
                        StrSQL.AppendLine(" AND Appezzamento.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(filtroCentri, filtroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Appezzamento.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '-------------------------------------------------------------------------- 
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '-------------------------------------------------------------------------- 
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_x_anagraficaNG(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Campo_Cod As Integer,
                                         ByVal Appezza As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi_x_anagraficaNG()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" Select   Appezzamento.PIVA + '_' + CAST(Appezzamento.SA_COD AS nvarchar(10)) + '_' + CAST(Appezzamento.APPEZZA AS nvarchar(10)) AS chiave,  ")
            StrSQL.AppendLine(" Appezzamento.PIVA as Piva, Appezzamento.SA_COD AS Sa_Cod, Appezzamento.APPEZZA As Appezza,  ")
            StrSQL.AppendLine("     Appezzamento.APP_NOME, Appezzamento.SUP_APP, Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine, Centri_Aziendali.sa_nome, ")
            StrSQL.AppendLine("   Appezzamento.Blk_Flag ,(select [User] from utenti where CODICE_FISCALE = Appezzamento.Username_Modifica ) as utente_modifica , Appezzamento.Data_Modifica, ")
            StrSQL.AppendLine("   (select [User] from utenti where CODICE_FISCALE = Appezzamento.Username_Creazione ) as utente_creazione , Appezzamento.Data_Creazione, ")
            StrSQL.AppendLine("   Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome, ")
            StrSQL.AppendLine("   Campi.Campo_Cod, Campi.Campo_Des ")

            StrSQL.AppendLine(" , ISNULL(rif_app.val_cod, '') AS rif_alfanumerico ")
            StrSQL.AppendLine(" , ISNULL(appBio.val_cod, '') AS cod_biologico ")
            StrSQL.AppendLine(" , ISNULL(isola.val_cod, '') AS isola ")
            StrSQL.AppendLine(" , ISNULL(metodoProd.val_cod, '1') AS MetodoProduzione_Cod ")
            StrSQL.AppendLine(" , CASE WHEN metodoProd.val_cod IS NULL THEN 'Integrato'  ")
            StrSQL.AppendLine("        WHEN metodoProd.val_cod = '1' THEN 'Integrato' ")
            StrSQL.AppendLine("        WHEN metodoProd.val_cod = '2' THEN 'In Conversione' ")
            StrSQL.AppendLine("        WHEN metodoProd.val_cod = '3' THEN 'Biologico' ")
            StrSQL.AppendLine("        ELSE 'Integrato' END AS MetodoProduzione_Des ")

            StrSQL.AppendLine(" FROM Appezzamento   ")
            StrSQL.AppendLine(" INNER JOIN Centri_Aziendali ON Appezzamento.PIVA = Centri_Aziendali.PIVA AND Appezzamento.SA_COD = Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine(" LEFT JOIN Campi ON Appezzamento.PIVA = Campi.PIVA AND Appezzamento.SA_COD = Campi.sa_cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici rif_app ON Appezzamento.PIVA = rif_app.PIVA AND Appezzamento.SA_COD = rif_app.sa_cod AND Appezzamento.APPEZZA = rif_app.appezza AND rif_app.id_cod = 1104 ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici appBio ON Appezzamento.PIVA = appBio.PIVA AND Appezzamento.SA_COD = appBio.sa_cod AND Appezzamento.APPEZZA = appBio.appezza AND appBio.id_cod = 1085 ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici isola ON Appezzamento.PIVA = isola.PIVA AND Appezzamento.SA_COD = isola.sa_cod AND Appezzamento.APPEZZA = isola.appezza AND isola.id_cod = 1320 ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici metodoProd ON Appezzamento.PIVA = metodoProd.PIVA AND Appezzamento.SA_COD = metodoProd.sa_cod AND Appezzamento.APPEZZA = metodoProd.appezza AND metodoProd.id_cod = 1018 ")

            StrSQL.AppendLine(" where Appezzamento.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" and Appezzamento.validita_fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim filtroCentri As String = ""
                Dim dtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Piva & "'", "", objParametri)
                If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To dtCentriVisibili.Rows.Count - 1
                        filtroCentri &= dtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If filtroCentri <> "" Then
                        StrSQL.AppendLine(" AND Appezzamento.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(filtroCentri, filtroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Appezzamento.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '-------------------------------------------------------------------------- 
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '-------------------------------------------------------------------------- 
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################################################
    Public Function Leggi_x_anagrafica_smart(ByVal Piva As String,
                                             ByVal Sa_Cod As Int32,
                                             ByVal Codice_Fiscale_Tecnico As String,
                                             ByVal join_centri_aziendali As Boolean,
                                             ByVal id_cod As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi_x_anagrafica_smart()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT     Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APPEZZA, reg_impianti.Sup_imp, Appezzamento.APP_NOME, Appezzamento.Campo_Cod, ")
            StrSQL.Append("   Reg_Impianti.Validita_Fine as Reg_Impianti_Validita_Fine, Reg_Impianti.Validita_Inizio as Reg_Impianti_Validita_Inizio, Reg_Impianti.ID_REG, Reg_Impianti.CUL_COD, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, SpecieVegetali.Veg_cod, Cultivar.Cul_cod, 0 as id_cod, Appezzamento.Blk_Flag, Imprese_Progetti.Validita_Inizio, Imprese_Progetti.Validita_Fine  ")

            If join_centri_aziendali Then
                StrSQL.Append("   ,Centri_Aziendali.sa_nome")
            End If

            StrSQL.Append(" ,ISNULL ((SELECT     TOP 1 Reg_Impianti_Codici.id_cod ")
            StrSQL.Append(" FROM Reg_Impianti_Codici ")
            StrSQL.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) ")
            StrSQL.Append(" AND (Reg_Impianti_Codici.sa_cod = Reg_Impianti.sa_cod) ")
            StrSQL.Append(" AND (Reg_Impianti_Codici.appezza = Reg_Impianti.appezza) ")
            StrSQL.Append(" AND (Reg_Impianti_Codici.ID_REG = Reg_Impianti.Id_Reg) ")
            StrSQL.Append(" AND (Reg_Impianti_Codici.id_cod >=3000 and Reg_Impianti_Codici.id_cod<4000) ")
            StrSQL.Append(" ), 0) AS Codice_id ")

            StrSQL.Append(" ,ISNULL ((SELECT     TOP 1 Codici_Anagrafe.descrizione ")
            StrSQL.Append(" FROM Codici_Anagrafe  ")
            StrSQL.Append(" RIGHT OUTER JOIN Reg_Impianti_Codici ON  Codici_Anagrafe.codice  = Reg_Impianti_Codici.id_cod ")
            StrSQL.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA)  AND (Reg_Impianti_Codici.sa_cod = Reg_Impianti.sa_cod)  ")
            StrSQL.Append(" AND (Reg_Impianti_Codici.appezza = Reg_Impianti.appezza)  ")
            StrSQL.Append(" AND (Reg_Impianti_Codici.ID_REG = Reg_Impianti.Id_Reg)  ")
            StrSQL.Append(" AND (Reg_Impianti_Codici.id_cod >=3000 and Reg_Impianti_Codici.id_cod<4000)  ), 0) ")
            StrSQL.Append(" AS Codici_Anagrafe_descrizione  ")

            StrSQL.Append(" FROM         Imprese_Progetti INNER JOIN  ")
            StrSQL.Append(" Appezzamento INNER JOIN  ")
            StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND   ")
            StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Imprese_Progetti.Piva = Reg_Impianti.PIVA AND Imprese_Progetti.Sa_Cod = Reg_Impianti.SA_COD AND   ")
            StrSQL.Append(" Imprese_Progetti.Appezza = Reg_Impianti.APPEZZA AND Imprese_Progetti.Id_Reg = Reg_Impianti.ID_REG   ")

            StrSQL.Append(" LEFT OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod   ")
            StrSQL.Append(" LEFT OUTER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  ")

            If id_cod > 0 Then
                StrSQL.Append(" LEFT OUTER JOIN   Reg_Impianti_Codici ON (Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA)  AND (Reg_Impianti_Codici.sa_cod = Reg_Impianti.sa_cod)  AND (Reg_Impianti_Codici.appezza = Reg_Impianti.appezza)  AND (Reg_Impianti_Codici.ID_REG = Reg_Impianti.Id_Reg) ")
            End If

            If join_centri_aziendali Then
                StrSQL.Append(" INNER JOIN Centri_Aziendali ON Appezzamento.PIVA = Centri_Aziendali.PIVA AND Appezzamento.SA_COD = Centri_Aziendali.sa_cod   ")
            End If


            StrSQL.Append(" where Imprese_Progetti.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" and imprese_progetti.validita_fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If id_cod > 0 Then
                StrSQL.Append(" AND Reg_Impianti_Codici.id_cod = " & id_cod & " ")
            End If

            '@Paolo: Caso in cui il terreno nudo non ha funzionalità
            If id_cod = 0 Then

                StrSQL.Length = 0

                StrSQL.Append(" select distinct * ")
                StrSQL.Append(" from ( ")

                StrSQL.Append(" SELECT     Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APPEZZA, reg_impianti.Sup_imp, Appezzamento.APP_NOME, Appezzamento.Campo_Cod, ")
                StrSQL.Append("   Reg_Impianti.Validita_Fine as Reg_Impianti_Validita_Fine, Reg_Impianti.Validita_Inizio as Reg_Impianti_Validita_Inizio, Reg_Impianti.ID_REG, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, SpecieVegetali.Veg_cod, Cultivar.Cul_cod, 0 as id_cod, Appezzamento.Blk_Flag, Imprese_Progetti.Validita_Inizio, Imprese_Progetti.Validita_Fine  ")

                If join_centri_aziendali Then
                    StrSQL.Append("   ,Centri_Aziendali.sa_nome")
                End If

                StrSQL.Append(" ,ISNULL ((SELECT     TOP 1 Reg_Impianti_Codici.id_cod ")
                StrSQL.Append(" FROM Reg_Impianti_Codici ")
                StrSQL.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA) ")
                StrSQL.Append(" AND (Reg_Impianti_Codici.sa_cod = Reg_Impianti.sa_cod) ")
                StrSQL.Append(" AND (Reg_Impianti_Codici.appezza = Reg_Impianti.appezza) ")
                StrSQL.Append(" AND (Reg_Impianti_Codici.ID_REG = Reg_Impianti.Id_Reg) ")
                StrSQL.Append(" AND (Reg_Impianti_Codici.id_cod >=3000 and Reg_Impianti_Codici.id_cod<4000) ")
                StrSQL.Append(" ), 0) AS Codice_id ")

                StrSQL.Append(" ,ISNULL ((SELECT     TOP 1 Codici_Anagrafe.descrizione ")
                StrSQL.Append(" FROM Codici_Anagrafe  ")
                StrSQL.Append(" RIGHT OUTER JOIN Reg_Impianti_Codici ON  Codici_Anagrafe.codice  = Reg_Impianti_Codici.id_cod ")
                StrSQL.Append(" WHERE(Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA)  AND (Reg_Impianti_Codici.sa_cod = Reg_Impianti.sa_cod)  ")
                StrSQL.Append(" AND (Reg_Impianti_Codici.appezza = Reg_Impianti.appezza)  ")
                StrSQL.Append(" AND (Reg_Impianti_Codici.ID_REG = Reg_Impianti.Id_Reg)  ")
                StrSQL.Append(" AND (Reg_Impianti_Codici.id_cod >=3000 and Reg_Impianti_Codici.id_cod<4000)  ), 0) ")
                StrSQL.Append(" AS Codici_Anagrafe_descrizione  ")

                StrSQL.Append(" FROM         Imprese_Progetti INNER JOIN  ")
                StrSQL.Append(" Appezzamento INNER JOIN  ")
                StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND   ")
                StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Imprese_Progetti.Piva = Reg_Impianti.PIVA AND Imprese_Progetti.Sa_Cod = Reg_Impianti.SA_COD AND   ")
                StrSQL.Append(" Imprese_Progetti.Appezza = Reg_Impianti.APPEZZA AND Imprese_Progetti.Id_Reg = Reg_Impianti.ID_REG   ")

                StrSQL.Append(" LEFT OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod   ")
                StrSQL.Append(" LEFT OUTER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  ")

                If join_centri_aziendali Then
                    StrSQL.Append(" INNER JOIN Centri_Aziendali ON Appezzamento.PIVA = Centri_Aziendali.PIVA AND Appezzamento.SA_COD = Centri_Aziendali.sa_cod   ")
                End If

                StrSQL.Append(" where Imprese_Progetti.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                StrSQL.Append(" and imprese_progetti.validita_fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                If Piva <> "" Then
                    StrSQL.Append(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                End If

            End If

            'StrSQL.Append(" SELECT     Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APPEZZA, reg_impianti.Sup_imp, Appezzamento.APP_NOME, Appezzamento.Campo_Cod, ")
            'StrSQL.Append("   Reg_Impianti.Validita_Fine, Reg_Impianti.Validita_Inizio, Reg_Impianti.ID_REG, Reg_Impianti.CUL_COD, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, SpecieVegetali.Veg_cod, Cultivar.Cul_cod,   ")
            'StrSQL.Append("   CASE WHEN Reg_Impianti.CUL_COD = 0 THEN ")
            'StrSQL.Append("     (SELECT     Codici_Anagrafe.descrizione FROM          Codici_Anagrafe RIGHT OUTER JOIN Reg_Impianti_Codici ON Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod WHERE      Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg AND (Reg_Impianti_Codici.id_cod >= 3000) AND (Reg_Impianti_Codici.id_cod <= 4000)) ")
            'StrSQL.Append("   ELSE '' END AS 'Codici_Anagrafe_descrizione'")
            'If join_centri_aziendali = True Then
            '    StrSQL.Append("   ,Centri_Aziendali.sa_nome")
            'End If

            ''If id_cod > 0 Then
            'StrSQL.Append("   ,Reg_Impianti_Codici.id_cod")
            ''End If

            'StrSQL.Append("   ,blk_flag , Imprese_Progetti.Validita_Inizio , Imprese_Progetti.Validita_Fine")

            'StrSQL.Append(" FROM         Imprese_Progetti INNER JOIN  ")
            'StrSQL.Append(" Appezzamento INNER JOIN  ")
            'StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND   ")
            'StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Imprese_Progetti.Piva = Reg_Impianti.PIVA AND Imprese_Progetti.Sa_Cod = Reg_Impianti.SA_COD AND   ")
            'StrSQL.Append(" Imprese_Progetti.Appezza = Reg_Impianti.APPEZZA AND Imprese_Progetti.Id_Reg = Reg_Impianti.ID_REG   ")

            'If join_centri_aziendali = True Then
            '    StrSQL.Append(" INNER JOIN Centri_Aziendali ON Appezzamento.PIVA = Centri_Aziendali.PIVA AND Appezzamento.SA_COD = Centri_Aziendali.sa_cod   ")
            'End If

            ''If id_cod > 0 Then
            'StrSQL.Append(" INNER JOIN Reg_Impianti_Codici ON Appezzamento.PIVA = Reg_Impianti_Codici.PIVA AND Appezzamento.SA_COD = Reg_Impianti_Codici.sa_cod   ")
            ''StrSQL.Append(" INNER JOIN Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod  AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza  ")
            ''End If

            'StrSQL.Append(" LEFT OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod   ")
            'StrSQL.Append(" LEFT OUTER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  ")


            'StrSQL.Append(" where Imprese_Progetti.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            'StrSQL.Append(" and imprese_progetti.validita_fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            'If Piva <> "" Then
            '    StrSQL.Append(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            'End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            'If Codice_Fiscale_Tecnico <> "" Then
            '    StrSQL.Append(" AND reg_impianti.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ")
            'End If


            ''@Paolo: aggiunta per il terreno nudo
            'StrSQL.Append(" AND (Reg_Impianti_Codici.id_cod >= 3000) AND (Reg_Impianti_Codici.id_cod <= 4000)")
            'If id_cod > 0 Then
            '    StrSQL.Append(" AND Reg_Impianti_Codici.id_cod = '" & id_cod & "' ")
            'End If

            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '-------------------------------------------------------------------------- 
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '-------------------------------------------------------------------------- 
            If id_cod <> 0 Then
                If xOrderBy <> "" Then
                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                End If
            End If

            '@Paolo: Caso in cui il terreno nudo non ha funzionalità
            If id_cod = 0 Then

                StrSQL.Append(" ) a ")

                StrSQL.Append(" where Codice_id = 0 ")
                StrSQL.Append(" ORDER BY  a.APP_NOME ")

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

    Public Function LeggiconFiltroSementieri(ByVal Piva As String,
                                             ByVal Sa_Cod As Int32,
                                             ByVal CampoCod As Int32,
                                             ByVal FiltroSementieri As String,
                                             ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal IDTestataTemp As Integer = 0
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiconFiltroSementieri()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   CampoCod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    stb.Length = 0
                    stb.AppendLine(" SELECT distinct  Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APPEZZA,  ")
                    stb.AppendLine(" Appezzamento.Campo_Cod,  Appezzamento.APP_NOME, Appezzamento.SUP_APP,  ")
                    stb.AppendLine("  Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine,  Appezzamento.Validazione, ")
                    stb.AppendLine("  Appezzamento.Blk_Flag ,  T.InizioImpianto, ISNULL( C.val_cod, '') as RiferimentoAlfanumerico ")

                    stb.AppendLine(" FROM  Appezzamento ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine(" On f.piva = Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)
                    End If

                    stb.AppendLine(" inner join  ")

                    stb.AppendLine(" (Select max(Validita_Inizio) as InizioImpianto , PIVA,SA_COD,APPEZZA ")
                    stb.AppendLine(" from dbo.Reg_Impianti ")
                    stb.AppendLine(" group by PIVA,SA_COD,APPEZZA ")
                    stb.AppendLine(" ) T on T.PIVA = Appezzamento.piva AND T.SA_COD = Appezzamento.SA_COD AND T.APPEZZA = Appezzamento.APPEZZA  ")

                    stb.AppendLine(" inner join (Select distinct PIVA,SA_COD,APPEZZA, codice_fiscale_tecnico")
                    stb.AppendLine(" from dbo.Reg_Impianti ")
                    stb.AppendLine(" group by PIVA,SA_COD,APPEZZA, codice_fiscale_tecnico ")
                    stb.AppendLine(" ) T1 on T1.PIVA = Appezzamento.piva AND T1.SA_COD = Appezzamento.SA_COD AND T1.APPEZZA = Appezzamento.APPEZZA  ")
                    stb.AppendLine(FiltroSementieri)

                    stb.AppendLine("  left join Appezzamento_Codici C on C.PIVA = Appezzamento.piva AND C.SA_COD = Appezzamento.SA_COD AND C.APPEZZA = Appezzamento.APPEZZA AND C.id_cod= 1104 ")

                    stb.AppendLine(" WHERE Appezzamento.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(CampoCod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY App_Nome ASC ")
                    End If

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiconCampo(ByVal Piva As String,
                                  ByVal Sa_Cod As Int32,
                                  ByVal CampoCod As Int32,
                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri,
                                  Optional ByVal IDTestataTemp As Integer = 0
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiconCampo()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   CampoCod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Appezzamento.PIVA,  Appezzamento.SA_COD,  Appezzamento.APPEZZA,  Appezzamento.Campo_Cod,   Appezzamento.APP_NOME,  Appezzamento.SUP_APP,  Appezzamento.Validita_Inizio,  Appezzamento.Validita_Fine,  Appezzamento.Validazione,   Appezzamento.Blk_Flag  ")
                    stb.AppendLine(" FROM  Appezzamento ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)
                    End If

                    stb.AppendLine(" WHERE Appezzamento.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(CampoCod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY App_Nome ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Appezzamento.PIVA,  Appezzamento.SA_COD,  Appezzamento.APPEZZA,  Appezzamento.Campo_Cod,   Appezzamento.APP_NOME,  Appezzamento.SUP_APP,  Appezzamento.Validita_Inizio,  Appezzamento.Validita_Fine,  Appezzamento.Validazione,   Appezzamento.Blk_Flag , ISNULL( C.val_cod, '') as RiferimentoAlfanumerico ")

                    stb.AppendLine(" FROM  Appezzamento ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)
                    End If

                    stb.AppendLine("  left join Appezzamento_Codici C on C.PIVA = Appezzamento.piva AND C.SA_COD = Appezzamento.SA_COD AND C.APPEZZA = Appezzamento.APPEZZA AND C.id_cod= 1104 ")

                    stb.AppendLine(" WHERE Appezzamento.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(CampoCod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY  Appezzamento.App_Nome ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APPEZZA,  ")
                    stb.AppendLine(" Appezzamento.Campo_Cod,  Appezzamento.APP_NOME, Appezzamento.SUP_APP,  ")
                    stb.AppendLine("  Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine,  Appezzamento.Validazione, ")
                    stb.AppendLine("  Appezzamento.Blk_Flag ,  T.InizioImpianto, ISNULL( C.val_cod, '') as RiferimentoAlfanumerico ")

                    stb.AppendLine(" FROM  Appezzamento ")


                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)
                    End If

                    stb.AppendLine(" inner join  ")

                    stb.AppendLine(" (Select max(Validita_Inizio) as InizioImpianto , PIVA,SA_COD,APPEZZA ")
                    stb.AppendLine(" from dbo.Reg_Impianti ")
                    stb.AppendLine(" group by PIVA,SA_COD,APPEZZA ")
                    stb.AppendLine(" ) T on T.PIVA = Appezzamento.piva AND T.SA_COD = Appezzamento.SA_COD AND T.APPEZZA = Appezzamento.APPEZZA  ")

                    stb.AppendLine("  left join Appezzamento_Codici C on C.PIVA = Appezzamento.piva AND C.SA_COD = Appezzamento.SA_COD AND C.APPEZZA = Appezzamento.APPEZZA AND C.id_cod= 1104 ")

                    stb.AppendLine(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(CampoCod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY App_Nome ASC ")
                    End If

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiconCampo_eVegCod(ByVal Piva As String,
                                          ByVal Sa_Cod As Int32,
                                          ByVal CampoCod As Int32,
                                          ByVal Veg_Cod As Int32,
                                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal IDTestataTemp As Integer = 0
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiconCampo_eVegCod()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   CampoCod = 0
        '   Veg_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Appezzamento.PIVA,  Appezzamento.SA_COD,  Appezzamento.APPEZZA,  Appezzamento.Campo_Cod,   Appezzamento.APP_NOME,  Appezzamento.SUP_APP,  Appezzamento.Validita_Inizio,  Appezzamento.Validita_Fine,  Appezzamento.Validazione,   Appezzamento.Blk_Flag  ")
                    stb.AppendLine(" FROM  Appezzamento ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)
                    End If

                    stb.AppendLine(" INNER JOIN Reg_Impianti ON (Appezzamento.Piva = Reg_Impianti.Piva and Appezzamento.Sa_Cod = Reg_Impianti.Sa_Cod and Appezzamento.Appezza = Reg_Impianti.Appezza) ")
                    stb.AppendLine(" INNER JOIN Cultivar ON (Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod) ")
                    stb.AppendLine(" INNER JOIN SpecieVegetali on (Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod)")

                    stb.AppendLine(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(CampoCod) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        stb.AppendLine(" AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY App_Nome ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    'L'appezzamento viene recuperato solo se il Codice_Fiscale_Tecnico di uno dei suoi impianti e' valido
    Public Function LeggiconCampo_ConFiltroCFT(ByVal Piva As String,
                                               ByVal Sa_Cod As Int32,
                                               ByVal CampoCod As Int32,
                                               ByVal Codice_Fiscale_Tecnico As String,
                                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               Optional ByVal IDTestataTemp As Integer = 0
                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiconCampo_ConFiltroCFT()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Appezzamento.PIVA,  Appezzamento.SA_COD,  Appezzamento.APPEZZA,  Appezzamento.Campo_Cod,   Appezzamento.APP_NOME,  Appezzamento.SUP_APP,  Appezzamento.Validita_Inizio,  Appezzamento.Validita_Fine,  Appezzamento.Validazione,   Appezzamento.Blk_Flag  ")
                    stb.AppendLine(" FROM  Appezzamento ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)
                    End If

                    stb.AppendLine(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(CampoCod) & " ")
                    End If

                    stb.AppendLine(" AND exists")
                    stb.AppendLine("         ( ")
                    stb.AppendLine("         SELECT * FROM [Reg_Impianti] ")
                    stb.AppendLine("         WHERE   [Reg_Impianti].Piva = Appezzamento.Piva ")
                    stb.AppendLine("         AND     [Reg_Impianti].sa_Cod = Appezzamento.sa_Cod   ")
                    stb.AppendLine("         AND     [Reg_Impianti].appezza = Appezzamento.appezza ")
                    stb.AppendLine("         AND     [Reg_Impianti].[CODICE_FISCALE_TECNICO] = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "'  ")
                    stb.AppendLine("         ) ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY App_Nome ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Appezzamento.PIVA,  Appezzamento.SA_COD,  Appezzamento.APPEZZA,  Appezzamento.Campo_Cod,   Appezzamento.APP_NOME,  Appezzamento.SUP_APP,  Appezzamento.Validita_Inizio,  Appezzamento.Validita_Fine,  Appezzamento.Validazione,   Appezzamento.Blk_Flag , ISNULL( C.val_cod, '') as RiferimentoAlfanumerico ")
                    stb.AppendLine(" FROM  Appezzamento ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)
                    End If

                    stb.AppendLine("  left join Appezzamento_Codici C on C.PIVA = Appezzamento.piva AND C.SA_COD = Appezzamento.SA_COD AND C.APPEZZA = Appezzamento.APPEZZA AND C.id_cod= 1104 ")

                    stb.AppendLine(" WHERE Appezzamento.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(CampoCod) & " ")
                    End If

                    stb.AppendLine(" AND exists")
                    stb.AppendLine("         ( ")
                    stb.AppendLine("         SELECT * FROM [Reg_Impianti] ")
                    stb.AppendLine("         WHERE   [Reg_Impianti].Piva = Appezzamento.Piva ")
                    stb.AppendLine("         AND     [Reg_Impianti].sa_Cod = Appezzamento.sa_Cod   ")
                    stb.AppendLine("         AND     [Reg_Impianti].appezza = Appezzamento.appezza ")
                    stb.AppendLine("         AND     [Reg_Impianti].[CODICE_FISCALE_TECNICO] = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "'  ")
                    stb.AppendLine("         ) ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY  Appezzamento.App_Nome ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APPEZZA,  ")
                    stb.AppendLine(" Appezzamento.Campo_Cod,  Appezzamento.APP_NOME, Appezzamento.SUP_APP,  ")
                    stb.AppendLine("  Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine,  Appezzamento.Validazione, ")
                    stb.AppendLine("  Appezzamento.Blk_Flag ,  T.InizioImpianto, ISNULL( C.val_cod, '') as RiferimentoAlfanumerico ")

                    stb.AppendLine(" FROM  Appezzamento ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)
                    End If

                    stb.AppendLine(" inner join  ")

                    stb.AppendLine(" (Select max(Validita_Inizio) as InizioImpianto , PIVA,SA_COD,APPEZZA ")
                    stb.AppendLine(" from dbo.Reg_Impianti ")
                    stb.AppendLine(" group by PIVA,SA_COD,APPEZZA ")
                    stb.AppendLine(" ) T on T.PIVA = Appezzamento.piva AND T.SA_COD = Appezzamento.SA_COD AND T.APPEZZA = Appezzamento.APPEZZA  ")

                    stb.AppendLine("  left join Appezzamento_Codici C on C.PIVA = Appezzamento.piva AND C.SA_COD = Appezzamento.SA_COD AND C.APPEZZA = Appezzamento.APPEZZA AND C.id_cod= 1104 ")

                    stb.AppendLine(" WHERE Appezzamento.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(CampoCod) & " ")
                    End If

                    stb.AppendLine(" AND exists")
                    stb.AppendLine("         ( ")
                    stb.AppendLine("         SELECT * FROM [Reg_Impianti] ")
                    stb.AppendLine("         WHERE   [Reg_Impianti].Piva = Appezzamento.Piva ")
                    stb.AppendLine("         AND     [Reg_Impianti].sa_Cod = Appezzamento.sa_Cod   ")
                    stb.AppendLine("         AND     [Reg_Impianti].appezza = Appezzamento.appezza ")
                    stb.AppendLine("         AND     [Reg_Impianti].[CODICE_FISCALE_TECNICO] = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "'  ")
                    stb.AppendLine("         ) ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY App_Nome ASC ")
                    End If

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
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
    Public Function Recupera_Appezzamenti_per_Campi(ByVal Piva As String,
                                                    ByVal Sa_cod As Integer,
                                                    ByVal Campo_Cod As Integer,
                                                    ByVal FlagIncludiAppezzamentiLiberi As Boolean,
                                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Recupera_Appezzamenti_per_Campi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Campo_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    appezzamento ")
                    StrSQL.Append(" WHERE   ")
                    StrSQL.Append("         (")
                    StrSQL.Append("         piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND     sa_cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
                    StrSQL.Append(" AND     campo_cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                    StrSQL.Append("         ) ")

                    If FlagIncludiAppezzamentiLiberi Then
                        StrSQL.Append(" OR ")
                        StrSQL.Append("         (")
                        StrSQL.Append("         piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                        StrSQL.Append(" AND     sa_cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
                        StrSQL.Append(" AND     campo_cod = " & 0 & " ")
                        StrSQL.Append("         )")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY appezza ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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

    '##############################################################################################
    Public Function Recupera_Superfici_Campo(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Campo_Cod As Integer,
                                             ByRef SAU_Totale As Decimal,
                                             ByRef SAU_Biologico As Decimal,
                                             ByRef SAU_Conversione As Decimal,
                                             ByRef SAU_Convenzionale As Decimal,
                                             ByRef SAU_Catastale As Decimal,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Recupera_Superfici_Campo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim tipoAgricoltura As enum_TipoAgricoltura

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  Appezzamento.PIVA, ")
            StrSQL.Append("         Appezzamento.SA_COD,  ")
            StrSQL.Append("         Appezzamento.Campo_Cod,  ")
            StrSQL.Append("         Appezzamento.APPEZZA,  ")
            StrSQL.Append("         Appezzamento.APP_NOME,  ")
            StrSQL.Append("         Appezzamento.SUP_APP,  ")
            StrSQL.Append("         Appezzamento_Codici.val_cod,  ")
            StrSQL.Append("         Appezzamento.Validita_Inizio,  ")
            StrSQL.Append("         Appezzamento.Validita_Fine, ")
            StrSQL.Append("         ISNULL(Appezzamento_Codici.id_cod, 1018) AS Id_Cod  ")

            StrSQL.Append(" FROM    Appezzamento LEFT OUTER JOIN ")
            StrSQL.Append("         Appezzamento_Codici ON Appezzamento.PIVA = Appezzamento_Codici.PIVA   ")
            StrSQL.Append("         AND Appezzamento.SA_COD = Appezzamento_Codici.sa_cod ")
            StrSQL.Append("         AND Appezzamento.APPEZZA = Appezzamento_Codici.appezza ")
            StrSQL.Append("         AND Appezzamento_Codici.id_cod = 1018")

            StrSQL.Append(" WHERE   (Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
            StrSQL.Append(" AND     (Appezzamento.SA_COD = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
            StrSQL.Append(" AND     (Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ") ")
            StrSQL.Append(" AND     (Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND     (Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

        If dt.Rows.Count > 0 Then

            'Inizializzo
            SAU_Totale = 0
            SAU_Biologico = 0
            SAU_Conversione = 0
            SAU_Convenzionale = 0

            'Ciclo sugli elementi selezionati
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1

                'Aggiorno il totale
                SAU_Totale += CDbl(dt.Rows(i).Item("sup_app"))

                'Verifico il tipo di agricoltura
                If IsDBNull(dt.Rows(i).Item("val_cod")) Then

                    'NOTA
                    'Se non è impostato il tipo di agricoltura,
                    'considero come default quella Convenzionale
                    tipoAgricoltura = enum_TipoAgricoltura.Convenzionale
                Else
                    tipoAgricoltura = CInt(dt.Rows(i).Item("val_cod"))
                End If


                'Aggiorno il contatore giusto
                Select Case tipoAgricoltura

                    Case enum_TipoAgricoltura.Convenzionale
                        SAU_Convenzionale += CDbl(dt.Rows(i).Item("sup_app"))

                    Case enum_TipoAgricoltura.InConversione
                        SAU_Conversione += CDbl(dt.Rows(i).Item("sup_app"))

                    Case enum_TipoAgricoltura.Biologica
                        SAU_Biologico += CDbl(dt.Rows(i).Item("sup_app"))

                End Select

                'Prossimo record
            Next

        Else
            SAU_Totale = 0
            SAU_Biologico = 0
            SAU_Conversione = 0
            SAU_Convenzionale = 0
        End If


        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")

            StrSQL.Append(" FROM    CampiXParticelle ")
            StrSQL.Append(" WHERE   (CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
            StrSQL.Append(" AND     (CampiXParticelle.SA_COD = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
            StrSQL.Append(" AND     (CampiXParticelle.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ") ")
            StrSQL.Append(" AND     (CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND     (CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   CampiXParticelle.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   CampiXParticelle.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

        If dt.Rows.Count > 0 Then

            'Inizializzo
            SAU_Catastale = 0

            'Ciclo sugli elementi selezionati
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1

                'Aggiorno il totale
                SAU_Catastale += CDbl(dt.Rows(i).Item("AREA"))

                'Prossimo record
            Next

        Else
            SAU_Catastale = 0
        End If

    End Function

    '################################################################################
    'questa funzione equivale a:
    'AgronicaCoreDataProvider.Numeri_from_StringaAlfaNumerica che restituisce un Decimal (il numero restituito può superare un integer)
    Public Function AppezzamentoNumero_from_AppezzamentoNome(ByVal App_Nome As String) As Integer

        Dim i As Integer
        Dim array As Char()
        Dim numero As String

        array = App_Nome.ToCharArray()

        For i = 0 To array.Length - 1
            If IsNumeric(array(i)) Then
                numero += array(i)
            End If
        Next

        Return CInt(numero)

    End Function

    Public Function Numero_Appezzamento(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Campo_Des As String,
                                        ByVal Appezza As Integer,
                                        ByVal TipoCodiceAppezzamento As enum_TipoCodiceAppezzamento_SchedaCampagna,
                                        ByVal App_Nome As String,
                                        ByVal App_Nome_Breve As String,
                                        ByVal N_Appezza As Integer,
                                        ByVal BaseCode As Integer,
                                        ByVal HTCentri As Hashtable,
                                        ByVal flag_nascondiCampo As Boolean
                                        ) As String

        Dim strCodiceAppezzamento As String = ""
        Dim numAppezzamento As Long

        If TipoCodiceAppezzamento = enum_TipoCodiceAppezzamento_SchedaCampagna.RiferimentoAlfanumerico Then
            strCodiceAppezzamento = App_Nome_Breve
        End If

        If TipoCodiceAppezzamento = enum_TipoCodiceAppezzamento_SchedaCampagna.NumeriInAppNome Then
            Dim objNum As New AgronicaCoreDataProvider.UtilityProvider
            numAppezzamento = objNum.Numero_from_Stringa(App_Nome)
            If numAppezzamento = 0 Then
                strCodiceAppezzamento = ""
            Else
                strCodiceAppezzamento = CStr(numAppezzamento)
            End If
        End If

        If TipoCodiceAppezzamento = enum_TipoCodiceAppezzamento_SchedaCampagna.AppezzaMenoBasecode Then
            ' in molti ds non viene calcolato N_Appezza quindi lo calcolo qui
            If N_Appezza < 0 Then
                N_Appezza = Appezza - BaseCode
            End If
            'se è un valore alto lo tronco alle ultime 3 cifre..
            If CStr(N_Appezza).Length > 3 Then
                strCodiceAppezzamento = CInt(Right(CStr(N_Appezza), 3))
            Else
                strCodiceAppezzamento = N_Appezza
            End If
        End If

        If TipoCodiceAppezzamento = enum_TipoCodiceAppezzamento_SchedaCampagna.NomeAppezzamento Then
            strCodiceAppezzamento = App_Nome
        End If


        If strCodiceAppezzamento = "" Then

            If App_Nome_Breve <> "" Then
                strCodiceAppezzamento = App_Nome_Breve
            Else
                strCodiceAppezzamento = App_Nome
                'Dim objNum As New AgronicaCoreDataProvider.UtilityProvider
                'numAppezzamento = objNum.Numero_from_Stringa(App_Nome)
                'If numAppezzamento <> 0 Then
                '    strCodiceAppezzamento = CStr(numAppezzamento)
                'End If
            End If

            If strCodiceAppezzamento = "" Then
                ' in molti ds non viene calcolato N_Appezza quindi lo calcolo qui
                If N_Appezza < 0 Then
                    N_Appezza = Appezza - BaseCode
                End If
                'se è un valore alto lo tronco alle ultime 3 cifre..
                If CStr(N_Appezza).Length > 3 Then
                    strCodiceAppezzamento = CInt(Right(CStr(N_Appezza), 3))
                Else
                    strCodiceAppezzamento = N_Appezza
                End If
            End If

        End If

        ' se è la stampa multicentro concateno centro e numero appezzamento
        If HTCentri IsNot Nothing Then
            If flag_nascondiCampo Then
                strCodiceAppezzamento = String.Format("{0}/{1}", HTCentri.Item(String.Format("{0}{1}", Piva, Sa_Cod)), strCodiceAppezzamento)
            Else
                If Campo_Des IsNot Nothing AndAlso Campo_Des.Trim <> "" Then
                    strCodiceAppezzamento = String.Format("{0}/{1}/{2}", HTCentri.Item(String.Format("{0}{1}", Piva, Sa_Cod)), Campo_Des, strCodiceAppezzamento)
                Else
                    strCodiceAppezzamento = String.Format("{0}/{1}", HTCentri.Item(String.Format("{0}{1}", Piva, Sa_Cod)), strCodiceAppezzamento)
                End If
            End If
        Else
            If Not flag_nascondiCampo Then
                If Campo_Des IsNot Nothing AndAlso Campo_Des.Trim <> "" Then
                    strCodiceAppezzamento = String.Format("{0}/{1}", Campo_Des, strCodiceAppezzamento)
                End If
            End If
        End If

        Return strCodiceAppezzamento

    End Function

    '##########################################################################################
    Public Function AppezzamentoInizio_from_Appezza(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As Date

        Dim dt As DataTable
        dt = Leggi(Piva, Sa_Cod, Appezza,
                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                   "", "", objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Validita_Inizio")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    '##########################################################################################
    Public Function AppezzamentoNome_from_Appezza(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Appezza As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Dim dt As DataTable
        dt = Leggi(Piva, Sa_Cod, Appezza,
                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                   "", "", objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("App_Nome")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    '##########################################################################################
    Public Function Recupera_Appezzamenti_Colture_del_Campo(
                                        ByVal Piva As String,
                                        ByVal Sa_cod As Integer,
                                        ByVal Campo_Cod As Integer,
                                        ByVal DataValiditaInizio As Date,
                                        ByVal DataValiditaFine As Date,
                                        ByVal FlagIncludiAppezzamentiLiberi As Boolean,
                                        ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal IDTestataTemp As Integer = 0
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Recupera_Appezzamenti_Colture_del_Campo()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Campo_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    stb.Length = 0

                    stb.AppendLine(" SELECT  Appezzamento.PIVA,  ")
                    stb.AppendLine("         Appezzamento.SA_COD, Appezzamento.APPEZZA, Appezzamento.Campo_Cod, Appezzamento.SUP_APP,  ")
                    stb.AppendLine("         Appezzamento.APP_NOME, Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine, ISNULL(Reg_Impianti.ID_REG,0) AS ID_REG,  ")
                    stb.AppendLine("         Reg_Impianti.Validita_Inizio AS Impianto_Validita_Inizio, Reg_Impianti.Validita_Fine AS Impianto_Validita_Fine, Reg_Impianti.CUL_COD,  ")
                    stb.AppendLine("         Cultivar.Cul_Des, SpecieVegetali.Veg_Des ")

                    stb.AppendLine(" FROM    Cultivar INNER JOIN ")
                    stb.AppendLine("         SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod RIGHT OUTER JOIN ")
                    stb.AppendLine("         Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD RIGHT OUTER JOIN ")
                    stb.AppendLine("         Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  ")
                    stb.AppendLine("         Reg_Impianti.APPEZZA = Appezzamento.APPEZZA ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)
                    End If

                    stb.AppendLine(" WHERE   (Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
                    stb.AppendLine(" AND     (Appezzamento.SA_COD = " & Agro_SQL_SaveNum(Sa_cod) & ")  ")

                    If Not FlagIncludiAppezzamentiLiberi Then
                        stb.AppendLine(" AND     (Appezzamento.campo_cod = " & Agro_SQL_SaveNum(Campo_Cod) & ")  ")
                    Else
                        stb.AppendLine(" AND     (Appezzamento.campo_cod = " & Agro_SQL_SaveNum(Campo_Cod) & " OR Appezzamento.campo_cod =0 )  ")
                    End If

                    stb.AppendLine(" AND     (Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(DataValiditaFine) & ")  ")
                    stb.AppendLine(" AND     (Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(DataValiditaInizio) & ")  ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY  Appezzamento.APP_NOME ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
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
    Public Function LeggiAppezzamentiDaVegCod(ByVal Piva As String,
                                              ByVal Veg_Cod As Int32,
                                              ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiAppezzamentiDaVegCod()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Appezzamento.Piva, Appezzamento.Sa_Cod, Appezzamento.Appezza, Appezzamento.App_Nome, ")
                    StrSQL.AppendLine(" Appezzamento.Sup_App, Appezzamento.Validita_Inizio as Validita_Inizio_Appezzamento,Appezzamento.Validita_Fine as Validita_Fine_Appezzamento, ")
                    StrSQL.AppendLine(" Reg_Impianti.Id_Reg ,  Reg_Impianti.Sup_Imp ,Reg_Impianti.Validita_Inizio as Validita_Inizio_Impianti,Reg_Impianti.Validita_Fine as Validita_Fine_Impianti,  Cultivar.Cul_Des, SpecieVegetali.Veg_Des   FROM Appezzamento ")
                    StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON (Appezzamento.Piva = Reg_Impianti.Piva and Appezzamento.Sa_Cod = Reg_Impianti.Sa_Cod and Appezzamento.Appezza = Reg_Impianti.Appezza) ")
                    StrSQL.AppendLine(" INNER JOIN Cultivar ON (Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod) ")
                    StrSQL.AppendLine(" INNER JOIN SpecieVegetali on (Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod)")

                    StrSQL.AppendLine(" WHERE   Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND     Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND     Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY  Veg_des, Cul_Des, Validita_Inizio_Impianti ")
                    End If

            End Select

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

    Public Function Leggi_EntitaBloccate(ByVal Validita_Inizio As Date,
                                         ByVal Validita_Fine As Date,
                                         ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi_EntitaBloccate()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append("SELECT Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.Campo_Cod, Appezzamento.APPEZZA, Appezzamento.Blk_Inizio_Data,   " &
                                    " Appezzamento.Blk_Inizio_UserName , Appezzamento.Blk_Inizio_Note, Appezzamento.Sup_App, Appezzamento.App_Nome,  Imprese.Rag_Soc, Centri_Aziendali.Sa_Nome " &
                                    "FROM   (Centri_Aziendali INNER JOIN Appezzamento ON Centri_Aziendali.Sa_Cod = Appezzamento.Sa_Cod and Centri_Aziendali.Piva = Appezzamento.Piva) INNER JOIN Imprese ON Imprese.PIVA = Appezzamento.PIVA " &
                                    "WHERE  (Imprese.PIVA IN " &
                                    "       (SELECT     Imprese.piva" &
                                    "        FROM       UtentixImprese, Imprese" &
                                    "        WHERE      UtentixImprese.PIVA = Imprese.PIVA AND UtentixImprese.Inviato >= 0 AND Imprese.Inviato >= 0 " &
                                    "        AND        Imprese.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    "        AND        Imprese.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                    "        AND        UtentixImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')) " &
                                    "        AND        Appezzamento.Blk_Flag = -1  ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                        'Else
                        'StrSQL.Append(" ORDER BY ....")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Append("SELECT Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.Campo_Cod, Appezzamento.APPEZZA, Appezzamento.Blk_Inizio_Data,   " &
                                                       " Appezzamento.Blk_Inizio_UserName , Appezzamento.Blk_Inizio_Note, Appezzamento.Sup_App, Appezzamento.App_Nome,  Imprese.Rag_Soc, Centri_Aziendali.Sa_Nome , SpecieVegetali.Veg_Des" &
                                                       " FROM         Centri_Aziendali INNER JOIN Appezzamento ON Centri_Aziendali.sa_cod = Appezzamento.SA_COD AND Centri_Aziendali.PIVA = Appezzamento.PIVA INNER JOIN Imprese ON Imprese.PIVA = Appezzamento.PIVA INNER JOIN Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND  Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod " &
                                                       " WHERE  (Imprese.PIVA IN " &
                                                       "       (SELECT     Imprese.piva" &
                                                       "        FROM       UtentixImprese, Imprese" &
                                                       "        WHERE      UtentixImprese.PIVA = Imprese.PIVA AND UtentixImprese.Inviato >= 0 AND Imprese.Inviato >= 0 " &
                                                       "        AND        Imprese.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                                       "        AND        Imprese.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                                       "        AND        UtentixImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')) " &
                                                       "        AND        Appezzamento.Blk_Flag = -1  ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                        'Else
                        'StrSQL.Append(" ORDER BY ....")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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

    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_TabellaDatiMinimi
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	08/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiSuperfici(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Appezza As Integer,
                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiSuperfici()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT sum(Sup_App) as Superficie ")
                    StrSQL.Append(" FROM  Appezzamento ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then

                        StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")

                        If Sa_Cod <> 0 Then

                            StrSQL.Append(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                            If Appezza <> 0 Then
                                StrSQL.Append(" AND  Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                            End If

                        End If

                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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

    '#################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Nuova versione senza COM+ e con objParametri.
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="SaCod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	16/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Superficie_from_PivaSaCodAppezza(ByVal Piva As String,
                                                     ByVal SaCod As Integer,
                                                     ByVal Appezza As Integer,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As Decimal

        Dim dtAppezza As DataTable
        dtAppezza = LeggiSuperfici(CStr(Piva),
                                   CInt(SaCod),
                                   CInt(Appezza),
                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                   "", "", objParametri)

        If dtAppezza IsNot Nothing AndAlso dtAppezza.Rows.Count > 0 Then
            Return dtAppezza.Rows(0).Item("superficie")
        Else
            Return 0
        End If

    End Function

    '#################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>

    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Numero_Appezza_Bio"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>
    ''' Somma delle superfici degli appezzamenti che hanno lo stesso numero appezzamento bio
    ''' </returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[gunelli]	10/10/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Superficie_from_NumeroAppezzaBio(ByVal Piva As String,
                                                     ByVal Numero_Appezza_Bio As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As Decimal

        Dim dtAppezza As DataTable
        dtAppezza = LeggiSuperficieAppezzamentoBio(CStr(Piva),
                                                   Numero_Appezza_Bio,
                                                   "", "", objParametri)

        If dtAppezza IsNot Nothing AndAlso dtAppezza.Rows.Count > 0 Then
            Return dtAppezza.Rows(0).Item("superficie")
        Else
            Return 0
        End If

    End Function

    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Numero_Appezza_Bio"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[gunelli]	10/10/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiSuperficieAppezzamentoBio(ByVal Piva As String,
                                                   ByVal Numero_Appezza_Bio As String,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiSuperficieAppezzamentoBio()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT ISNULL(SUM(Appezzamento.Sup_App), 0) as Superficie ")
            StrSQL.Append(" FROM  Appezzamento ")
            StrSQL.Append(" INNER JOIN  Appezzamento_Codici ON Appezzamento_Codici.Piva = Appezzamento.Piva AND Appezzamento_Codici.Sa_Cod = Appezzamento.Sa_Cod AND Appezzamento_Codici.Appezza = Appezzamento.Appezza ")
            StrSQL.Append(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   Appezzamento_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Codice_Appezza_Biologico) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND  Appezzamento.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            End If

            If Numero_Appezza_Bio <> "" Then
                StrSQL.Append(" AND  Val_Cod = '" & Agro_SQL_SaveText(Trim(Numero_Appezza_Bio)) & "'")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function VerificaAppezzamentoBloccato(ByVal piva As String,
                                                 ByVal sa_cod As Integer,
                                                 ByVal appezza As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.VerificaAppezzamentoBloccato()"

        Dim messaggioErrore As String = ""
        Dim strQuery As New Text.StringBuilder
        Dim dt As DataTable

        Dim risp As Boolean

        Try

            risp = True

            strQuery.Length = 0

            'Preparo la query
            strQuery.Append(" SELECT COUNT(*) ")
            strQuery.Append(" FROM Appezzamento ")
            strQuery.Append(" WHERE Blk_Flag = -1 ")
            strQuery.Append(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strQuery.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            strQuery.Append(" AND Appezza = " & Agro_SQL_SaveNum(appezza) & " ")

            dt = EseguiQuery_Lettura(objParametri, strQuery.ToString, nomeRoutine)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                If CInt(dt.Rows(0).Item(0)) > 0 Then
                    risp = True 'Se il numero di record è >0 allora l'appezzamento è bloccato
                Else
                    risp = False
                End If

            Else
                risp = False
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risp

    End Function

    '####################################################################
    Function Numero_Appezzamenti_X_Specie(ByVal piva As String,
                                          ByVal sa_cod As Integer,
                                          ByVal Veg_Cod As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Numero_Appezzamenti_X_Specie()"

        Dim messaggioErrore As String = ""
        Dim strQuery As New Text.StringBuilder
        Dim dt As DataTable
        Dim numAppezza As Integer = 0

        Try

            strQuery.Length = 0
            strQuery.Append(" SELECT  COUNT(*) AS NUM ")
            strQuery.Append(" FROM    Appezzamento ")
            strQuery.Append(" INNER JOIN Reg_Impianti ON (Appezzamento.Piva = Reg_Impianti.Piva and Appezzamento.Sa_Cod = Reg_Impianti.Sa_Cod and Appezzamento.Appezza = Reg_Impianti.Appezza) ")
            strQuery.Append(" INNER JOIN Cultivar ON (Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod) ")
            strQuery.Append(" INNER JOIN SpecieVegetali on (Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod)")
            strQuery.Append(" WHERE  ")
            strQuery.Append(" Appezzamento.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If sa_cod <> 0 Then
                strQuery.Append(" AND Appezzamento.Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If Veg_Cod <> 0 Then
                strQuery.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            dt = EseguiQuery_Lettura(objParametri, strQuery.ToString, nomeRoutine)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numAppezza = CInt(dt.Rows(0).Item("NUM"))
            Else
                numAppezza = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numAppezza

    End Function

    '####################################################################
    Function Numero_Appezzamenti(ByVal piva As String,
                                 ByVal sa_cod As Integer,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Numero_Appezzamenti()"

        Dim messaggioErrore As String = ""
        Dim strQuery As New Text.StringBuilder
        Dim dt As DataTable
        Dim numAppezza As Integer = 0

        Try

            strQuery.Length = 0
            strQuery.Append(" SELECT  COUNT(*) AS NUM ")
            strQuery.Append(" FROM    Appezzamento ")
            strQuery.Append(" WHERE  ")
            strQuery.Append(" Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If sa_cod <> 0 Then
                strQuery.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            dt = EseguiQuery_Lettura(objParametri, strQuery.ToString, nomeRoutine)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                numAppezza = CInt(dt.Rows(0).Item("NUM"))
            Else
                numAppezza = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numAppezza

    End Function

    'Leggo le specie Vegetali con le relative sup [Tabella di Join di appoggio]
    '##############################################################################################
    Public Function Leggi_Veg_Sup_x_piva(ByVal xFiltroAggiuntivo As String,
                                         ByVal Data_Inizio As Date,
                                         ByVal Data_Fine As Date,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi_Veg_Sup_x_piva()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT     reg_impianti.PIVA, sum(reg_impianti.SUP_imp) as SUP_APP, SpecieVegetali.Veg_Des, SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" FROM        Reg_Impianti ")
            StrSQL.Append("         INNER Join Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            StrSQL.Append("         INNER Join SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            'StrSQL.Append("     INNER JOIN " & TabellaAppoggio & "  ON Appezzamento.Piva = " & TabellaAppoggio & ".Piva ")
            StrSQL.Append("     Where 1 = 1  ")
            StrSQL.Append("     AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine))
            StrSQL.Append("     AND Reg_Impianti.Validita_FINE >= " & Agro_SQL_SaveDate(Data_Inizio))

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            StrSQL.Append("     group by reg_impianti.PIVA, SpecieVegetali.Veg_Des, SpecieVegetali.Veg_Cod ")
            StrSQL.Append("     order by reg_impianti.PIVA, SpecieVegetali.Veg_Des, SpecieVegetali.Veg_Cod ")

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

    'legge dati appezzamenti (con catasto), impianti, distinte
    Public Function LeggiDaCampoconCatasto(ByVal Piva As String,
                                           ByVal Sa_Cod As Int32,
                                           ByVal CampoCod As Int32,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiDaCampoconCatasto()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   CampoCod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT     Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.DATA_APP, Appezzamento.APP_NOME, Appezzamento.Campo_Cod, Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine,   ")
            StrSQL.Append(" AppezzamentiXParticelle.PROV, AppezzamentiXParticelle.COM, AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, AppezzamentiXParticelle.NUMERO, AppezzamentiXParticelle.SUBALTERNO, AppezzamentiXParticelle.AREA,    ")
            StrSQL.Append(" Reg_Impianti.ID_REG, Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto, Reg_Impianti.Sup_Imp, ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod, ISNULL(Cultivar.Cul_Des, '') AS Cul_Des, ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des,    ")
            StrSQL.Append(" ISNULL(Imprese_Progetti.Progetto_Cod,0) AS Progetto_Cod, ISNULL(Imprese_Progetti.Progetto_Nome,'') AS Progetto_Nome, Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta,     ")
            StrSQL.Append(" Centri_Aziendali.sa_nome, Imprese.rag_soc   ")

            StrSQL.Append(" FROM  Appezzamento INNER JOIN ")
            StrSQL.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND  ")
            StrSQL.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN ")
            StrSQL.Append(" AppezzamentiXParticelle ON Appezzamento.PIVA = AppezzamentiXParticelle.PIVA AND Appezzamento.SA_COD = AppezzamentiXParticelle.SA_COD AND  ")
            StrSQL.Append(" Appezzamento.APPEZZA = AppezzamentiXParticelle.APPEZZA INNER JOIN ")
            StrSQL.Append(" Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND  ")
            StrSQL.Append(" Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg INNER JOIN ")
            StrSQL.Append(" Centri_Aziendali ON Appezzamento.PIVA = Centri_Aziendali.PIVA AND Appezzamento.SA_COD = Centri_Aziendali.sa_cod INNER JOIN ")
            StrSQL.Append(" Imprese ON Appezzamento.PIVA = Imprese.PIVA LEFT OUTER JOIN ")
            StrSQL.Append(" SpecieVegetali INNER JOIN ")
            StrSQL.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            StrSQL.Append(" WHERE Appezzamento.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Appezzamento.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If CampoCod <> 0 Then
                StrSQL.Append(" AND Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(CampoCod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY App_Nome ASC ")
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

    '################################################################################
    Public Function Recupera_Dati_Appezzamento(ByVal Piva As String,
                                               ByVal Sa_Cod As Int32,
                                               ByVal Appezza As Int32,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Recupera_Dati_Appezzamento()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Imprese.rag_soc, Centri_Aziendali.sa_nome, ISNULL(Campi.Campo_Des,'') AS Campo_Des, ISNULL(Campi.Campo_Cod,0) AS Campo_Cod, ")
            StrSQL.Append(" Appezzamento.SUP_APP, Appezzamento.APP_NOME, Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine ")

            StrSQL.Append(" FROM  Imprese INNER JOIN ")
            StrSQL.Append(" Centri_Aziendali ON Imprese.PIVA = Centri_Aziendali.PIVA INNER JOIN ")
            StrSQL.Append(" Appezzamento ON Centri_Aziendali.sa_cod = Appezzamento.SA_COD AND Centri_Aziendali.PIVA = Appezzamento.PIVA LEFT OUTER JOIN ")
            StrSQL.Append(" Campi ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod ")

            StrSQL.Append(" WHERE Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Appezzamento.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "")
            StrSQL.Append(" AND Appezzamento.Appezza = " & Agro_SQL_SaveNum(Appezza) & "")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                'Else
                '    StrSQL.Append(" ORDER BY App_Nome ASC ")
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

    Public Function Leggi_Max_DataModifica(ByVal Piva As String,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri
                                           ) As DateTime

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.Leggi_Max_DataModifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim dataModifica = AGRODATAINIZIO

        Try

            If Piva = "" Then
                Throw New Exception("Piva obbligatoria")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT MAX(Data_Modifica) ")
            StrSQL.AppendLine(" FROM Appezzamento ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso
                dt.Rows.Count > 0 AndAlso
                IsDate(dt.Rows(0)(0)) Then
                dataModifica = dt.Rows(0)(0)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dataModifica = AGRODATAINIZIO
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dataModifica

    End Function

    Public Function LeggixReportPianoColturale(ByVal Padre As String,
                                               ByVal Id_Budget As Integer,
                                               ByVal includiAziendeFiglie As Integer,
                                               ByVal Validita_Inizio As Date,
                                               ByVal Validita_Fine As Date,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Const nomeRoutine = "AnagrafeDAL.Reg_Impianti_Read.LeggixReportPianoColturale()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim dtAziendePadre As DataTable
        Dim filtroPivaPadre As String = ""
        Dim tabellaPrefisso As String = ""

        Try

            If Id_Budget <> 0 Then
                tabellaPrefisso = "Budget_"
            End If

            If Padre <> "" Then

                'Comprendo anche l'azienda padre
                filtroPivaPadre = "'" & Agro_SQL_SaveText(Padre) & "'"

                If includiAziendeFiglie = 1 Then

                    StrSQL.AppendLine(" select figlio from GerarchiaImprese where padre = '" & Agro_SQL_SaveText(Padre) & "'")
                    '-------------------------------------------------------------------------------
                    dtAziendePadre = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
                    '-------------------------------------------------------------------------------

                    If dtAziendePadre IsNot Nothing AndAlso dtAziendePadre.Rows.Count > 0 Then
                        For Each r In dtAziendePadre.Rows
                            filtroPivaPadre &= ",'" & r.Item("figlio") & "'"
                        Next
                    End If

                End If

            End If


            StrSQL.Length = 0
            StrSQL.AppendLine(" Select Distinct gi.Padre, i.rag_soc, a.PIVA, a.SA_COD, a.APPEZZA ")
            StrSQL.AppendLine(" , a.APP_NOME, a.Campo_Cod, ca.Campo_Des ")
            StrSQL.AppendLine(" , a.SUP_APP, ri.Id_Reg, ISNULL(RC.Val_Cod, '') as Codice_Impianto ")

            StrSQL.AppendLine(" From " & tabellaPrefisso & "Reg_Impianti ri ")
            StrSQL.AppendLine(" inner Join Imprese i on ri.piva = i.piva ")
            StrSQL.AppendLine(" inner Join " & tabellaPrefisso & "Appezzamento a on ri.PIVA = a.Piva And ri.SA_COD = a.Sa_Cod And ri.APPEZZA = a.Appezza ")
            StrSQL.AppendLine(" Left Join " & tabellaPrefisso & "Campi ca ON a.PIVA = ca.Piva And a.SA_COD = ca.Sa_Cod And a.Campo_Cod = ca.Campo_Cod ")

            StrSQL.AppendLine(" Left Join GerarchiaImprese gi on i.piva = gi.Figlio ")
            StrSQL.AppendLine(" Inner Join " & tabellaPrefisso & "Imprese_Progetti ip on ri.PIVA = ip.Piva And ri.SA_COD = ip.Sa_Cod And ri.APPEZZA = ip.Appezza And ri.ID_REG = ip.Id_Reg ")

            StrSQL.AppendLine(" LEFT JOIN " & tabellaPrefisso & "Reg_Impianti_Codici RC ")
            StrSQL.AppendLine("   ON ip.piva = RC.piva ")
            StrSQL.AppendLine("   AND ip.SA_COD = RC.sa_cod ")
            StrSQL.AppendLine("   AND ip.APPEZZA = RC.appezza ")
            StrSQL.AppendLine("   AND ip.ID_REG = RC.Id_Reg ")
            StrSQL.AppendLine("   AND RC.id_cod = 1300 ")


            StrSQL.AppendLine(" WHERE ip.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND ip.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine(" AND ri.Cul_Cod <> 0 ")

            If Trim(filtroPivaPadre) <> "" Then
                'Costruzione stringa con apici
                StrSQL.AppendLine(" And ri.piva In (" & Agro_SQL_Save_Clausola_IN(filtroPivaPadre, True) & ") ")
            End If

            If Id_Budget <> 0 Then
                StrSQL.Append(" And " & tabellaPrefisso & "Reg_impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            StrSQL.AppendLine(" ORDER BY Padre, Piva, App_Nome, Sa_Cod, Appezza ")

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

    Public Function ReadAppezzamentoCampoCod(piva As String,
                                             saCod As Integer,
                                             appezza As Integer,
                                             objParametriServer As AgronicaCoreParametri) As DataTable

        Const routineName = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.ReadAppezzamentoCampoCod()"

        Dim strSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            strSQL.Length = 0
            strSQL.AppendLine("SELECT *")
            strSQL.AppendLine("FROM Appezzamento")
            strSQL.AppendLine(String.Format("WHERE PIVA = '{0}'", piva))
            strSQL.AppendLine(String.Format("    AND SA_COD = {0}", saCod))
            strSQL.AppendLine(String.Format("    AND APPEZZA = {0}", appezza))

            dt = EseguiQuery_Lettura(objParametriServer, strSQL.ToString, routineName)
        Catch ex As Exception
            Scrivi_LOG(objParametriServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Public Function LeggiPerAggiornamentoPendenza(ByVal Piva As String,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  Optional ByVal FiltroAziendePriorita As Boolean = False,
                                                  Optional ByVal ElencoProvince As List(Of String) = Nothing) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiPerAggiornamentoPendenza"

        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Dim TipoEntitaAppezzamenti As Integer = enum_GIS2012_TipoEntita.APPEZZAMENTI
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Appezzamento.Piva, Appezzamento.Sa_Cod, Appezzamento.Appezza,Appezzamento.Sup_App, e.Entita_Cod ")
            StrSQL.AppendLine(" FROM  Appezzamento ")
            StrSQL.AppendLine(" INNER JOIN gis_entita e ON Appezzamento.piva = e.piva ")
            StrSQL.AppendLine("     AND Appezzamento.sa_cod = e.sa_cod ")
            StrSQL.AppendLine("     AND Appezzamento.appezza = e.appezza ")
            StrSQL.AppendLine("     AND e.TipoEntita_Cod = " & Agro_SQL_SaveNum(TipoEntitaAppezzamenti) & " ")
            StrSQL.AppendLine("     AND e.Id_Imp = 0 ")
            StrSQL.AppendLine(" LEFT JOIN AppezzamentiXParticelle axp ON Appezzamento.piva = axp.piva ")
            StrSQL.AppendLine("     AND Appezzamento.sa_cod = axp.sa_cod ")
            StrSQL.AppendLine("     AND Appezzamento.appezza = axp.appezza ")
            If ElencoProvince IsNot Nothing AndAlso ElencoProvince.Count > 0 Then
                StrSQL.AppendLine(" LEFT JOIN AppezzamentixIndirizzi axi (NOLOCK) ON Appezzamento.piva = axi.piva ")
                StrSQL.AppendLine("     And Appezzamento.sa_cod = axi.sa_cod ")
                StrSQL.AppendLine("     And Appezzamento.appezza = axi.appezza ")
                StrSQL.AppendLine(" LEFT JOIN Indirizzi ind (NOLOCK) ON axi.cod_indirizzo = ind.cod_indirizzo ")
            End If
            'Lavez - 20/10/2025 - porcata galattica per sopperire alle frigne contigue di regione umbria
            If FiltroAziendePriorita Then
                StrSQL.AppendLine(" INNER JOIN _Filtro_Pive_Catasto fp ON Appezzamento.piva = fp.piva ")
            End If
            StrSQL.AppendLine(" WHERE 1=1 ")
            'StrSQL.AppendLine(" AND Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(Date.Now) & " ")
            'StrSQL.AppendLine(" AND Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Date.Now) & " ")
            StrSQL.AppendLine(" AND Agea_identificativoAppezzamento is not null ")
            StrSQL.AppendLine(" AND Agea_identificativoAppezzamento != '' ")
            StrSQL.AppendLine(" AND axp.APPEZZA is null ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            'Lavez - 03/11/2025 - Filtro per provincia
            If ElencoProvince IsNot Nothing AndAlso ElencoProvince.Count > 0 Then
                Dim elencoProvinceSQL As String = ""
                For Each p In ElencoProvince
                    elencoProvinceSQL += "'" & Agro_SQL_SaveText(p) & "'" + ","
                Next
                If elencoProvinceSQL <> "" Then
                    elencoProvinceSQL = elencoProvinceSQL.Substring(0, elencoProvinceSQL.Length - 1)
                End If
                StrSQL.AppendLine(" AND ind.pro_cod_istat IN (" & elencoProvinceSQL & ") ")
            End If

            Scrivi_LOG(objParametri, NomeRoutine, "Query: " & vbCrLf & StrSQL.ToString())

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString(), NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function
End Class
