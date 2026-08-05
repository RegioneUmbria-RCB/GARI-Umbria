Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class EstrazioneDati_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function EstraiImbottigliamenti(ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreXMLUniversale.EstrazioneDati_R.EstraiImbottigliamenti"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT lp.piva, lp.Preparazione_Cod, lp.Preparazione_Des, lp.ChkConfezionamento, ")
            StrSQL.AppendLine("        a.PIVA, a.Sa_Cod, a.Id_Agenda, m.Id_Mov, md.Id_Mov_Det, a.Lav_Cod, a.Des_Lib, a.LINEA_COD, a.PREPARAZIONE_COD, a.ID_TRASFORMAZIONE, ")
            StrSQL.AppendLine("        m.Mov_Desc, m.Data_Movimento, m.Ora, ")
            StrSQL.AppendLine("        md.Id_Mov_Det, md.Mov_Det_Des, md.Elem_Cod, cm.NomeComune, md.Pro_Cod, md.Mat_Cod, mp.Mat_Des, md.Udm_Cod, um.UDM_DES,  ")
            StrSQL.AppendLine("        md.Qta, md.Data_Modifica, md.Lotto, md.Jolly_Int, md.QTA_EXTRA, md.UDM_COD_EXTRA, umExtra.UDM_DES, ")
            StrSQL.AppendLine("        mDest.Piva, mDest.Sa_Cod as Centro_Destinazione, ca.sa_nome, mDest.Id_Destinazione, mDest.Tipo_Destinazione,  ")
            StrSQL.AppendLine("        CASE mDest.Tipo_Destinazione ")
            StrSQL.AppendLine("            WHEN " & VASCA_ENOLOGICA & " THEN 'Vasca' ")
            StrSQL.AppendLine("            ELSE ft.Tipo_Fabbricato_Des ")
            StrSQL.AppendLine("        END AS Tipo_Destinazione, ")
            StrSQL.AppendLine("        COALESCE(f.Fabbricato_Des,'') as Fabbricato_Des, COALESCE(cv.Identificativo,'') as Vasca, mDest.Qta, ")

            StrSQL.AppendLine("        COALESCE( ")
            StrSQL.AppendLine("            (SELECT mdc.Qta ")
            StrSQL.AppendLine("             FROM Movimenti_Dettagli mdc ")
            StrSQL.AppendLine("             WHERE mdc.Elem_Cod = " & CALI_LAVORAZIONE & " ")
            StrSQL.AppendLine("             AND mdc.Piva = md.Piva ")
            StrSQL.AppendLine("             AND mdc.Id_Agenda = md.Id_Agenda ")
            StrSQL.AppendLine("             AND mdc.Id_Mov = md.Id_Mov ")
            StrSQL.AppendLine("            ) , 0) AS Calo ")

            StrSQL.AppendLine(" FROM Movimenti_Dettagli md ")
            StrSQL.AppendLine(" INNER JOIN Movimenti m ON m.Id_Mov = md.Id_Mov ")
            StrSQL.AppendLine(" INNER JOIN Agenda a ON a.Id_Agenda = m.Id_Agenda AND a.Id_Agenda = md.Id_Agenda ")
            StrSQL.AppendLine(" INNER JOIN Linee_Preparazioni lp ON lp.Preparazione_Cod = a.PREPARAZIONE_COD ")
            StrSQL.AppendLine(" INNER JOIN CategorieMagazzino cm ON cm.Elem_Cod = md.Elem_Cod ")
            StrSQL.AppendLine(" INNER JOIN Materie_Prime mp ON mp.Mat_Cod = md.Mat_Cod ")
            StrSQL.AppendLine(" INNER JOIN UnitaMisura um ON um.UDM_COD = md.Udm_Cod ")
            StrSQL.AppendLine(" INNER JOIN UnitaMisura umExtra ON umExtra.UDM_COD = md.UDM_COD_EXTRA ")
            StrSQL.AppendLine(" INNER JOIN Mov_Destinazioni mDest on mDest.Id_Agenda = a.Id_Agenda AND mDest.Id_Mov = m.Id_Mov AND mDest.Id_Mov_Det = md.Id_Mov_Det ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Fabbricati f on f.Piva = mDest.Piva AND f.Sa_Cod = mDest.Sa_Cod AND f.Fabbricato_Cod = mDest.Id_Destinazione ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Cantina_Vasche cv on cv.Piva = mDest.Piva AND cv.Sa_Cod = mDest.Sa_Cod AND cv.Vas_Cod = mDest.Id_Destinazione ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Fabbricati_Tipi ft on ft.Tipo_Fabbricato_Cod = mDest.Tipo_Destinazione ")
            StrSQL.AppendLine(" INNER JOIN Centri_Aziendali ca on ca.PIVA = mDest.Piva AND ca.sa_cod = mDest.Sa_Cod ")

            StrSQL.AppendLine(" WHERE m.Cau_Mov = '" & CAU_CARICO & "' ")
            StrSQL.AppendLine(" AND a.Lav_Cod = " & LAVCOD_TRASFORMAZIONI & " ")
            'StrSQL.AppendLine(" AND lp.ChkConfezionamento = 1 ")
            StrSQL.AppendLine(" AND lp.Tipo_Default = " & CInt(enum_Omni_TipoDefault_Preparazioni.Confezionamenti) & " ")
            StrSQL.AppendLine(" AND md.Elem_Cod <> " & CALI_LAVORAZIONE & " ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY m.Data_Movimento ASC, a.Id_Agenda ASC ")
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
    Public Function EstraiCarichiMagazzino(ByVal Lav_Cod As Integer,
                                           ByVal Piva_Dest As String,
                                           ByVal Sa_Cod_Dest As Integer,
                                           ByVal Id_DestinazioneString As String,
                                           ByVal Flag_EscludiGiaEstratti As Boolean,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreXMLUniversale.EstrazioneDati_R.EstraiCarichiMagazzino"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Append("SELECT  a.PIVA, a.Sa_Cod, a.Id_Agenda, m.Id_Mov, md.Id_Mov_Det, a.Lav_Cod, a.Des_Lib, a.LINEA_COD, a.PREPARAZIONE_COD, a.ID_TRASFORMAZIONE, " & vbCrLf)
            StrSQL.Append("        m.Mov_Desc, m.Data_Movimento, m.Ora, " & vbCrLf)
            StrSQL.Append("        md.Id_Mov_Det, md.Mov_Det_Des, md.Elem_Cod, cm.NomeComune, md.Pro_Cod, md.Mat_Cod, mp.Mat_Des, md.Udm_Cod, um.UDM_DES,  " & vbCrLf)
            StrSQL.Append("        md.Qta, md.Data_Modifica, md.Lotto, md.Jolly_Int, md.QTA_EXTRA, md.UDM_COD_EXTRA, umExtra.UDM_DES, " & vbCrLf)
            StrSQL.Append("        mDest.Piva, mDest.Sa_Cod as Centro_Destinazione, ca.sa_nome, mDest.Id_Destinazione, mDest.Tipo_Destinazione,  " & vbCrLf)
            StrSQL.Append("        CASE mDest.Tipo_Destinazione " & vbCrLf)
            StrSQL.Append("            WHEN " & VASCA_ENOLOGICA & " THEN 'Vasca' " & vbCrLf)
            StrSQL.Append("            ELSE ft.Tipo_Fabbricato_Des " & vbCrLf)
            StrSQL.Append("        END AS Tipo_Destinazione, " & vbCrLf)
            StrSQL.Append("        COALESCE(f.Fabbricato_Des,'') as Fabbricato_Des, COALESCE(cv.Identificativo,'') as Vasca, mDest.Qta ")

            'StrSQL.Append("        ,COALESCE( " & vbCrLf)
            'StrSQL.Append("            (SELECT mdc.Qta " & vbCrLf)
            'StrSQL.Append("             FROM Movimenti_Dettagli mdc " & vbCrLf)
            'StrSQL.Append("             WHERE mdc.Elem_Cod = " & Agro_SQL_SaveNum(CALI_LAVORAZIONE) & " " & vbCrLf)
            'StrSQL.Append("             AND mdc.Piva = md.Piva " & vbCrLf)
            'StrSQL.Append("             AND mdc.Id_Agenda = md.Id_Agenda " & vbCrLf)
            'StrSQL.Append("             AND mdc.Id_Mov = md.Id_Mov " & vbCrLf)
            'StrSQL.Append("            ) , 0) AS Calo ")

            'If Flag_EscludiGiaEstratti = True Then
            StrSQL.Append("        , x.Stato " & vbCrLf)
            'End If

            StrSQL.Append(" FROM Movimenti_Dettagli md " & vbCrLf)
            StrSQL.Append(" INNER JOIN Movimenti m ON m.Id_Mov = md.Id_Mov " & vbCrLf)
            StrSQL.Append(" INNER JOIN Agenda a ON a.Id_Agenda = m.Id_Agenda AND a.Id_Agenda = md.Id_Agenda " & vbCrLf)
            StrSQL.Append(" INNER JOIN Linee_Preparazioni lp ON lp.Preparazione_Cod = a.PREPARAZIONE_COD " & vbCrLf)
            StrSQL.Append(" INNER JOIN CategorieMagazzino cm ON cm.Elem_Cod = md.Elem_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN Materie_Prime mp ON mp.Mat_Cod = md.Mat_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN UnitaMisura um ON um.UDM_COD = md.Udm_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN UnitaMIsura umExtra ON umExtra.UDM_COD = md.UDM_COD_EXTRA " & vbCrLf)
            StrSQL.Append(" INNER JOIN Mov_Destinazioni mDest on mDest.Id_Agenda = a.Id_Agenda AND mDest.Id_Mov = m.Id_Mov AND mDest.Id_Mov_Det = md.Id_Mov_Det " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Fabbricati f on f.Piva = mDest.Piva AND f.Sa_Cod = mDest.Sa_Cod AND f.Fabbricato_Cod = mDest.Id_Destinazione " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Cantina_Vasche cv on cv.Piva = mDest.Piva AND cv.Sa_Cod = mDest.Sa_Cod AND cv.Vas_Cod = mDest.Id_Destinazione " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Fabbricati_Tipi ft on ft.Tipo_Fabbricato_Cod = mDest.Tipo_Destinazione " & vbCrLf)
            StrSQL.Append(" INNER JOIN Centri_Aziendali ca on ca.PIVA = mDest.Piva AND ca.sa_cod = mDest.Sa_Cod " & vbCrLf)

            'If Flag_EscludiGiaEstratti = True Then
            StrSQL.Append(" LEFT OUTER JOIN XML_Export_Mag_Attuali x ON x.Piva = mDest.Piva AND x.Id_Agenda = mdest.Id_Agenda AND x.Id_Mov_Det = mdest.Id_Mov_Det " & vbCrLf)
            'End If


            StrSQL.Append(" WHERE m.Cau_Mov = '" & Agro_SQL_SaveText(enum_Agenda_Causali.CARICO) & "' " & vbCrLf)
            'StrSQL.Append(" --AND lp.Tipo_Default = " & Agro_SQL_SaveNum(enum_Omni_TipoDefault_Preparazioni.Confezionamenti) & " " & vbCrLf)
            'StrSQL.Append(" --AND md.Elem_Cod <> " & Agro_SQL_SaveNum(CALI_LAVORAZIONE) & " " & vbCrLf)

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND a.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " " & vbCrLf)
            End If

            If Piva_Dest <> "" Then
                StrSQL.Append(" AND mDest.Piva = '" & Agro_SQL_SaveText(Piva_Dest) & "' " & vbCrLf)
            End If

            If Sa_Cod_Dest <> 0 AndAlso Id_DestinazioneString <> "" Then
                StrSQL.Append(" AND mDest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_Dest) & " " & vbCrLf)
                StrSQL.Append(" AND mDest.Id_Destinazione IN (" & Agro_SQL_Save_Clausola_IN(Id_DestinazioneString) & ") " & vbCrLf)
            End If

            'escludo quelli già estratti con stato 254
            If Flag_EscludiGiaEstratti Then
                StrSQL.AppendLine(" AND (x.Stato <> " & enum_WFlow_Export_XML_Universale.Operazione_Esportata & " OR x.Stato IS NULL) ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY m.Data_Movimento ASC, a.Id_Agenda ASC ")
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
    Public Function EstraiScarichiMagazzino(ByVal Lav_Cod As Integer,
                                            ByVal Piva_Dest As String,
                                            ByVal Sa_Cod_Dest As Integer,
                                            ByVal Id_DestinazioneString As String,
                                            ByVal Flag_EscludiGiaEstratti As Boolean,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByVal objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreXMLUniversale.EstrazioneDati_R.EstraiScarichiMagazzino"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Append("SELECT  a.PIVA, a.Sa_Cod, a.Id_Agenda, m.Id_Mov, md.Id_Mov_Det, a.Lav_Cod, a.Des_Lib, a.LINEA_COD, a.PREPARAZIONE_COD, a.ID_TRASFORMAZIONE, " & vbCrLf)
            StrSQL.Append("        m.Mov_Desc, m.Data_Movimento, m.Ora, " & vbCrLf)
            StrSQL.Append("        md.Id_Mov_Det, md.Mov_Det_Des, md.Elem_Cod, cm.NomeComune, md.Pro_Cod, md.Mat_Cod, mp.Mat_Des, md.Udm_Cod, um.UDM_DES,  " & vbCrLf)
            StrSQL.Append("        md.Qta, md.Data_Modifica, md.Lotto, md.Jolly_Int, md.QTA_EXTRA, md.UDM_COD_EXTRA, umExtra.UDM_DES, " & vbCrLf)
            StrSQL.Append("        mDest.Piva, mDest.Sa_Cod as Centro_Destinazione, ca.sa_nome, mDest.Id_Destinazione, mDest.Tipo_Destinazione,  " & vbCrLf)
            StrSQL.Append("        CASE mDest.Tipo_Destinazione " & vbCrLf)
            StrSQL.Append("            WHEN " & VASCA_ENOLOGICA & " THEN 'Vasca' " & vbCrLf)
            StrSQL.Append("            ELSE ft.Tipo_Fabbricato_Des " & vbCrLf)
            StrSQL.Append("        END AS Tipo_Destinazione, " & vbCrLf)
            StrSQL.Append("        COALESCE(f.Fabbricato_Des,'') as Fabbricato_Des, COALESCE(cv.Identificativo,'') as Vasca, mDest.Qta " & vbCrLf)

            'StrSQL.Append("        ,COALESCE( " & vbCrLf)
            'StrSQL.Append("            (SELECT mdc.Qta " & vbCrLf)
            'StrSQL.Append("             FROM Movimenti_Dettagli mdc " & vbCrLf)
            'StrSQL.Append("             WHERE mdc.Elem_Cod = " & Agro_SQL_SaveNum(CALI_LAVORAZIONE) & " " & vbCrLf)
            'StrSQL.Append("             AND mdc.Piva = md.Piva " & vbCrLf)
            'StrSQL.Append("             AND mdc.Id_Agenda = md.Id_Agenda " & vbCrLf)
            'StrSQL.Append("             AND mdc.Id_Mov = md.Id_Mov " & vbCrLf)
            'StrSQL.Append("            ) , 0) AS Calo ")

            'If Flag_EscludiGiaEstratti = True Then
            StrSQL.Append("        , x.Stato " & vbCrLf)
            'End If

            StrSQL.Append(" FROM Movimenti_Dettagli md " & vbCrLf)
            StrSQL.Append(" INNER JOIN Movimenti m ON m.Id_Mov = md.Id_Mov " & vbCrLf)
            StrSQL.Append(" INNER JOIN Agenda a ON a.Id_Agenda = m.Id_Agenda AND a.Id_Agenda = md.Id_Agenda " & vbCrLf)
            StrSQL.Append(" INNER JOIN Linee_Preparazioni lp ON lp.Preparazione_Cod = a.PREPARAZIONE_COD " & vbCrLf)
            StrSQL.Append(" INNER JOIN CategorieMagazzino cm ON cm.Elem_Cod = md.Elem_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN Materie_Prime mp ON mp.Mat_Cod = md.Mat_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN UnitaMisura um ON um.UDM_COD = md.Udm_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN UnitaMIsura umExtra ON umExtra.UDM_COD = md.UDM_COD_EXTRA " & vbCrLf)
            StrSQL.Append(" INNER JOIN Mov_Destinazioni mDest on mDest.Id_Agenda = a.Id_Agenda AND mDest.Id_Mov = m.Id_Mov AND mDest.Id_Mov_Det = md.Id_Mov_Det " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Fabbricati f on f.Piva = mDest.Piva AND f.Sa_Cod = mDest.Sa_Cod AND f.Fabbricato_Cod = mDest.Id_Destinazione " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Cantina_Vasche cv on cv.Piva = mDest.Piva AND cv.Sa_Cod = mDest.Sa_Cod AND cv.Vas_Cod = mDest.Id_Destinazione " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Fabbricati_Tipi ft on ft.Tipo_Fabbricato_Cod = mDest.Tipo_Destinazione " & vbCrLf)
            StrSQL.Append(" INNER JOIN Centri_Aziendali ca on ca.PIVA = mDest.Piva AND ca.sa_cod = mDest.Sa_Cod " & vbCrLf)

            'If Flag_EscludiGiaEstratti = True Then
            StrSQL.Append(" LEFT OUTER JOIN XML_Export_Mag_Attuali x ON x.Piva = mDest.Piva AND x.Id_Agenda = mdest.Id_Agenda AND x.Id_Mov_Det = mdest.Id_Mov_Det " & vbCrLf)
            'End If


            StrSQL.Append(" WHERE m.Cau_Mov = '" & Agro_SQL_SaveText(enum_Agenda_Causali.SCARICO) & "' " & vbCrLf)
            'StrSQL.Append(" --AND lp.Tipo_Default = " & Agro_SQL_SaveNum(enum_Omni_TipoDefault_Preparazioni.Confezionamenti) & " " & vbCrLf)
            'StrSQL.Append(" --AND md.Elem_Cod <> " & Agro_SQL_SaveNum(CALI_LAVORAZIONE) & " " & vbCrLf)

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND a.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " " & vbCrLf)
            End If

            If Piva_Dest <> "" Then
                StrSQL.Append(" AND mDest.Piva = '" & Agro_SQL_SaveText(Piva_Dest) & "' " & vbCrLf)
            End If

            If Sa_Cod_Dest <> 0 AndAlso Id_DestinazioneString <> "" Then
                StrSQL.Append(" AND mDest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_Dest) & " " & vbCrLf)
                StrSQL.Append(" AND mDest.Id_Destinazione IN (" & Agro_SQL_Save_Clausola_IN(Id_DestinazioneString) & ") " & vbCrLf)
            End If

            'escludo quelli già estratti con stato 254
            If Flag_EscludiGiaEstratti Then
                StrSQL.Append(" AND (x.Stato <> " & enum_WFlow_Export_XML_Universale.Operazione_Esportata & " OR x.Stato IS NULL) " & vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY m.Data_Movimento ASC, a.Id_Agenda ASC ")
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
    Public Function EstraiImbottigliamentiLINQ(ByVal piva As String,
                                               ByVal sa_cod As Integer,
                                               ByVal magazzino As Integer,
                                               ByVal objParametri As AgronicaCoreParametri
                                               ) As String

        Const nomeRoutine = "AgronicaCoreXMLUniversale.EstrazioneDati_R.EstraiImbottigliamentiLINQ"

        Dim messaggioErrore As String = ""
        Dim risposta As String = ""


        'Dim piva As String = "00860480375"

        'Dim _filtroSuVarieta As Boolean = False
        'If Not _varieta Is Nothing And _varieta.Length > 0 Then
        '    _filtroSuVarieta = True
        'End If

        'Dim dataMovDalDateTime As Nullable(Of DateTime)
        'dataMovDalDateTime = Nothing
        'If Not String.IsNullOrEmpty(_dataMovDal) Then
        '    dataMovDalDateTime = Convert.ToDateTime(_dataMovDal)
        'End If

        'Dim dataMovAlDateTime As Nullable(Of DateTime)
        'dataMovAlDateTime = Nothing
        'If Not String.IsNullOrEmpty(_dataMovAl) Then
        '    dataMovAlDateTime = Convert.ToDateTime(_dataMovAl)
        'End If


        Try
            
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim movimenti_dettagli As DbSet(Of Movimenti_dettagli) = GiasContext.Movimenti_dettagli
                Dim movimenti As DbSet(Of Movimenti) = GiasContext.Movimenti
                Dim agenda As DbSet(Of Agenda) = GiasContext.Agenda
                Dim linee_preparazioni As DbSet(Of Linee_Preparazioni) = GiasContext.Linee_Preparazioni
                Dim categorie_magazzino As DbSet(Of CategorieMagazzino) = GiasContext.CategorieMagazzino
                Dim matPrima As DbSet(Of Materie_Prime) = GiasContext.Materie_Prime
                Dim unita_misura As DbSet(Of UnitaMisura) = GiasContext.UnitaMisura
                Dim unita_misura_extra As DbSet(Of UnitaMisura) = GiasContext.UnitaMisura
                Dim mov_destinazioni As DbSet(Of Mov_Destinazioni) = GiasContext.Mov_Destinazioni
                Dim fabbricati As DbSet(Of Fabbricati) = GiasContext.Fabbricati
                Dim cantina_vasche As DbSet(Of Cantina_Vasche) = GiasContext.Cantina_Vasche
                Dim fabbricati_tipi As DbSet(Of Fabbricati_Tipi) = GiasContext.Fabbricati_Tipi
                Dim centri_aziendali As DbSet(Of Centri_Aziendali) = GiasContext.Centri_Aziendali

                'Dim lavCodConf As System.Nullable(Of Integer)() = {LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}

                Dim Imbottigliamenti =
                    From md In movimenti_dettagli
                    Join mov In movimenti
                        On
                         md.PIVA Equals mov.PIVA And
                         md.Id_Mov Equals mov.Id_Mov
                    Join ag In agenda
                        On ag.Id_Agenda Equals mov.Id_Agenda And
                           ag.Id_Agenda Equals md.Id_Agenda
                    Join lp In linee_preparazioni
                        On lp.Preparazione_Cod Equals ag.PREPARAZIONE_COD
                    Join cm In categorie_magazzino
                        On cm.Elem_Cod Equals md.Elem_Cod
                    Join mp In matPrima
                        On mp.Mat_Cod Equals md.Mat_Cod
                    Join um In unita_misura
                        On um.UDM_COD Equals md.Udm_Cod
                    Join umExtra In unita_misura_extra
                        On umExtra.UDM_COD Equals md.UDM_COD_EXTRA
                    Join mDest In mov_destinazioni
                        On mDest.Id_Agenda Equals ag.Id_Agenda And
                           mDest.Id_Mov Equals mov.Id_Mov And
                           mDest.Id_Mov_Det Equals md.Id_Mov_Det
                    Join ca In centri_aziendali
                        On ca.PIVA Equals mDest.Piva And
                           ca.sa_cod Equals mDest.Sa_Cod
                    Group Join fabbrmov_dest In fabbricati
                        On mDest.Piva Equals fabbrmov_dest.PIVA And
                           mDest.Sa_Cod Equals fabbrmov_dest.SA_COD And
                           mDest.Id_Destinazione Equals fabbrmov_dest.Fabbricato_Cod Into _fabbrmov_dest = Group
                    From _fd In _fabbrmov_dest.DefaultIfEmpty()
                    Group Join vaschemov_dest In cantina_vasche
                        On mDest.Piva Equals vaschemov_dest.Piva And
                           mDest.Sa_Cod Equals vaschemov_dest.Sa_Cod And
                           mDest.Id_Destinazione Equals vaschemov_dest.Vas_Cod Into _vaschemov_dest = Group
                    From _vd In _vaschemov_dest.DefaultIfEmpty()
                    Group Join fabbrtipimov_dest In fabbricati_tipi
                        On mDest.Tipo_Destinazione Equals fabbrtipimov_dest.Tipo_Fabbricato_Cod Into _fabbrtipimov_dest = Group
                    From _ftd In _fabbrtipimov_dest.DefaultIfEmpty()
                    Where
                        ag.PIVA = piva AndAlso
                        mov.Cau_Mov = CStr(enum_Agenda_Causali.CARICO) AndAlso
                        ag.Lav_Cod = LAVCOD_TRASFORMAZIONI AndAlso
                        lp.Tipo_Default = enum_Omni_TipoDefault_Preparazioni.Confezionamenti AndAlso
                        md.Elem_Cod <> CALI_LAVORAZIONE
                    Order By mov.Data_Movimento Ascending, ag.Id_Agenda Ascending
                    Select New With
                    {
                        .Piva_Preparazione = lp.Piva,
                        .Preparazione_Cod_Linee = lp.Preparazione_Cod,
                        .Preparazione_Des = lp.Preparazione_Des,
                        .ChkConfezionamento = lp.ChkConfezionamento,
                        .Piva_Agenda = ag.PIVA,
                        .Sa_Cod = ag.Sa_Cod,
                        .Id_Agenda = ag.Id_Agenda,
                        .Id_mov = mov.Id_Mov,
                        .Id_Mov_Det = md.Id_Mov_Det,
                        .Lav_Cod = ag.Lav_Cod,
                        .Des_Lib = ag.des_lib,
                        .Linea_Cod = ag.LINEA_COD,
                        .Preparazione_Cod_Agenda = ag.PREPARAZIONE_COD,
                        .Id_Trasformazione = ag.ID_TRASFORMAZIONE,
                        .Mov_Desc = mov.Mov_Desc,
                        .Data_Movimento = mov.Data_Movimento,
                        .Ora = mov.Ora,
                        .Mov_Det_Des = md.Mov_Det_Des,
                        .Elem_Cod = md.Elem_Cod,
                        .NomeComune = cm.NomeComune,
                        .Pro_Cod = md.Pro_Cod,
                        .Mat_Cod = md.Mat_Cod,
                        .Mat_Des = mp.Mat_Des,
                        .Udm_Cod = md.Udm_Cod,
                        .Udm_Des = um.UDM_DES,
                        .Qta = md.Qta,
                        .Data_Modifica_Det = md.Data_Modifica,
                        .Lotto = md.Lotto,
                        .Jolly_Int = md.Jolly_Int,
                        .Qta_Extra = md.QTA_EXTRA,
                        .Udm_Cod_Extra = md.UDM_COD_EXTRA,
                        .Udm_Des_Extra = umExtra.UDM_DES,
                        .Piva_Dest = mDest.Piva,
                        .Sa_Cod_Dest = mDest.Sa_Cod,
                        .Sa_Nome = ca.sa_nome,
                        .Id_Destinazione = mDest.Id_Destinazione,
                        .Tipo_Destinazione = mDest.Tipo_Destinazione,
                        .Tipo_Destinazione_Des = If(mDest.Tipo_Destinazione = 13, "Vasca", _ftd.Tipo_Fabbricato_Des),
                        .Fabbricato_Des = If(_fd Is Nothing, "", _fd.Fabbricato_Des),
                        .Vasca = If(_vd Is Nothing, "", _vd.Identificativo),
                        .Qta_Dest = mDest.Qta,
                        .Calo = (From mdc In movimenti_dettagli
                                 Where mdc.Elem_Cod = CALI_LAVORAZIONE AndAlso
                                 mdc.PIVA = md.PIVA AndAlso
                                 mdc.Id_Agenda = md.Id_Agenda AndAlso
                                 mdc.Id_Mov = md.Id_Mov
                                 Select mdc.Qta).FirstOrDefault
                    }

                If sa_cod <> 0 AndAlso magazzino <> 0 Then
                    Imbottigliamenti = Imbottigliamenti.Where(Function(x) x.Sa_Cod_Dest = sa_cod AndAlso x.Id_Destinazione = magazzino)
                End If

                Dim serializerSettings As New JsonSerializerSettings With {
                    .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }

                risposta = JsonConvert.SerializeObject(Imbottigliamenti.ToList(), Formatting.None, serializerSettings)

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risposta = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

End Class
