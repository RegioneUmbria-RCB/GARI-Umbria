Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class DPI_Leggi

    Public Function ImpegniAggiuntiviFacoltativi_LeggiDT(ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                         ByRef Log_Errori As String,
                                                         ByVal IAF_Cod As String,
                                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                                         ByRef objParametri_Utenti As AgronicaCoreParametri
                                                         ) As DataTable

        Dim DT As New DataTable

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari

        Try

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari

            Dim agroWs As String
            If IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            Else
                agroWs = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
            End If

            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs, agroWs, objParametri_Utenti)

            DT = ObjDownloadWs.Leggi_ImpegniAggiuntiviFacoltativi_DT(IAF_Cod,
                                                    CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                    CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                    Log_Errori)

        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante la chiamata al ws dei Disciplinari:" & Chr(13) &
                   ex.Message
        End Try

        Return DT

    End Function

    Public Function IAF_x_Dpi_Specie_Leggi(ByRef objSession As System.Web.SessionState.HttpSessionState,
                                           ByRef Log_Errori As String,
                                           ByVal Dpi_Cod As String,
                                           ByVal Veg_Cod As String,
                                           ByVal strFiltro As String,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri
                                           ) As DataTable

        Dim DT As New DataTable

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari

        Try

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim agroWs As String
            If IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            Else
                agroWs = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
            End If

            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs, agroWs, objParametri_Utenti)

            DT = ObjDownloadWs.Leggi_IAF_DT(Dpi_Cod, Veg_Cod,
                                         AGRODATAINIZIO, AGRODATAFINE,
                                         CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                         CStr(objSession("ASG_Utente_Password_Crypt").ToString), strFiltro, "", Log_Errori)

        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante la chiamata al ws dei Discliplinari:" & Chr(13) &
               ex.Message
        End Try

        Return DT

    End Function

    Public Function IAF_x_Dpi_Specie_Leggi_Controlli(ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                     ByRef Log_Errori As String,
                                                     ByVal Dpi_Cod As String, ByVal Bio As Boolean,
                                                     ByVal Veg_Cod As String,
                                                     ByVal strFiltro As String,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri
                                                     ) As DataTable

        Dim DT As New DataTable

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari

        Try

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim agroWs As String
            If IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            Else
                agroWs = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
            End If

            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs, agroWs, objParametri_Utenti)

            DT = ObjDownloadWs.Leggi_IAF_DT_Controlli(Dpi_Cod, Bio, Veg_Cod,
                                         AGRODATAINIZIO, AGRODATAFINE,
                                         CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                         CStr(objSession("ASG_Utente_Password_Crypt").ToString), strFiltro, "", Log_Errori)

        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante la chiamata al ws dei Disciplinari:" & Chr(13) &
               ex.Message
        End Try

        Return DT

    End Function

    '###############################################################################
    ' Veg_Cod from ID_Rcdpi  
    '###############################################################################
    Public Function VegCod_from_IdRcDpi(ByRef objServer As System.Web.HttpServerUtility,
                                        ByRef objSession As System.Web.SessionState.HttpSessionState,
                                        ByRef objPage As System.Web.UI.Page,
                                        ByVal Id_RcDpi As Integer,
                                        ByVal Veg_Cod As Integer,
                                        ByVal Grfi_Cod As Integer,
                                        ByVal Disciplinare_Cod As Integer,
                                        ByVal Flag_Protetto As Integer,
                                        ByVal Tipo_Testata As Integer) As Integer


        Dim strErr As String = ""
        Dim VegCod As Integer = 0

        '##############################  Chiama WS - "Leggi_RaggruppamentiDPI" ################################
        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String


        Try
            Dim objParametri_Utenti As AgronicaCoreParametri
            objParametri_Utenti = New AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString(),
                            objParametri_Utenti)



            Dati = ObjDownloadWs.Leggi_RaggruppamentiDPI2(CInt(Id_RcDpi),
                                                          CInt(Veg_Cod),
                                                          CInt(Grfi_Cod),
                                                          CInt(Disciplinare_Cod),
                                                          CInt(Flag_Protetto),
                                                          CInt(Tipo_Testata),
                                                          CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                          CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                          strErr)


            If strErr = "" Then

                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                    Dim XmlDocumento As New System.Xml.XmlDocument
                    Dim XmlNodo As System.Xml.XmlNodeList
                    Dim XmlElemento As System.Xml.XmlElement
                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                    If XmlNodo IsNot Nothing Then
                        For Each XmlElemento In XmlNodo
                            VegCod = CInt(XmlElemento.GetAttribute("veg_cod"))
                        Next
                    End If
                End If

            End If

        Catch ex As Exception

            'Dati = "Si sono verificati errori in fase di chiamata al WebService Disciplinari!"

        End Try

        Return VegCod


    End Function


    '###############################################################################
    Public Function Nome_Disciplinare(
        ByVal Reg_Cod As Integer,
        ByVal Disciplinare_Cod As Integer,
        ByVal VEG_COD As Integer,
        ByVal Id_RcDpi As Integer,
        ByVal Flag_Privato_Pubblico As Integer,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        ByVal ASG_Utente_Username_Crypt As String,
        ByVal ASG_Utente_Password_Crypt As String,
        Optional ByVal FinestraTemp_Inizio As Date = #1/1/1900#,
        Optional ByVal FinestraTemp_Fine As Date = #12/31/2100#
    ) As String

        Dim strErr As String = ""

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Try

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString(),
                            objParametri_Utenti)



            'Richiamo il disciplinare pubblico
            Dati = ObjDownloadWs.Leggi_Disciplinari2(CInt(Disciplinare_Cod),
                                                    CInt(Id_RcDpi),
                                                    CInt(VEG_COD),
                                                    CInt(Reg_Cod),
                                                    CInt(Flag_Privato_Pubblico),
                                                    CDate(FinestraTemp_Inizio),
                                                    CDate(FinestraTemp_Fine),
                                                    ASG_Utente_Username_Crypt,
                                                    ASG_Utente_Password_Crypt,
                                                    strErr)

            If strErr = "" Then

                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                    If XmlNodo IsNot Nothing AndAlso XmlNodo.Count > 0 Then
                        XmlElemento = XmlNodo.Item(0)
                        Return XmlElemento.GetAttribute("nomeesteso")
                    Else
                        Return ""
                    End If
                Else
                    Return ""
                End If
            Else
                Return ""
            End If


        Catch ex As Exception

            ' Dati = "Si sono verificati errori in fase di chiamata al WebService Disciplinari!"

        End Try


    End Function



    '###############################################################################
    Public Function Nome_Disciplinare(ByRef objServer As System.Web.HttpServerUtility,
                                    ByRef objSession As System.Web.SessionState.HttpSessionState,
                                    ByRef objPage As System.Web.UI.Page,
                                    ByVal Reg_Cod As Integer,
                                    ByVal Disciplinare_Cod As Integer,
                                    ByVal VEG_COD As Integer,
                                    ByVal Id_RcDpi As Integer,
                                    ByVal Flag_Privato_Pubblico As Integer,
                                    Optional ByVal FinestraTemp_Inizio As Date = #1/1/1900#,
                                    Optional ByVal FinestraTemp_Fine As Date = #12/31/2100#) As String

        Dim strErr As String = ""

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        'Verifico la possibilità di gestione dei disciplinari
        Try
            Dim objParametri_Utenti As AgronicaCoreParametri
            objParametri_Utenti = New AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))
            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString(),
                            objParametri_Utenti)



            'Richiamo il disciplinare pubblico
            Dati = ObjDownloadWs.Leggi_Disciplinari2(CInt(Disciplinare_Cod),
                                                    CInt(Id_RcDpi),
                                                    CInt(VEG_COD),
                                                    CInt(Reg_Cod),
                                                    CInt(Flag_Privato_Pubblico),
                                                    CDate(FinestraTemp_Inizio),
                                                    CDate(FinestraTemp_Fine),
                                                    CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                    CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                    strErr)

            If strErr = "" Then

                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                    If XmlNodo IsNot Nothing AndAlso XmlNodo.Count > 0 Then
                        XmlElemento = XmlNodo.Item(0)
                        Return XmlElemento.GetAttribute("nomeesteso")
                    Else
                        Return ""
                    End If
                Else
                    Return ""
                End If
            Else
                Return ""
            End If


        Catch ex As Exception

            ' Dati = "Si sono verificati errori in fase di chiamata al WebService Disciplinari!"

        End Try


    End Function


    '''' -----------------------------------------------------------------------------
    '''' <summary>
    '''' anziche caricare direttamente la combo, si carica in un dt il risultato
    '''' default -->
    '''' Includi_Nessuno As Boolean = True
    '''' Includi_Biologico As Boolean = True
    '''' Consultazione As Boolean = False
    '''' </summary>
    '''' -----------------------------------------------------------------------------
    Public Function Disciplinari_LeggiDT(ByRef objSession As System.Web.SessionState.HttpSessionState,
                                        ByRef Log_Errori As String,
                                        ByVal Reg_Cod As Integer,
                                        ByVal Disciplinare_Cod As Integer,
                                        ByVal VEG_COD As Integer,
                                        ByVal Id_RcDpi As Integer,
                                        ByVal Flag_Privato_Pubblico As Integer,
                                        ByVal Includi_Nessuno As Boolean,
                                        ByVal Includi_Biologico As Boolean,
                                        ByVal Consultazione As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri
                                            ) As DataTable

        Dim DT As New DataTable
        Dim Dr As DataRow

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        DT.Columns.Add(New DataColumn("Disciplinare_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Disciplinare_Cod", GetType(Integer)))

        'Aggiungo la voce nulla
        If Includi_Nessuno Then
            Dr = DT.NewRow
            Dr.Item("Disciplinare_Des") = "Nessun Disciplinare"
            Dr.Item("Disciplinare_Cod") = 0
            DT.Rows.Add(Dr)
        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico AndAlso VEG_COD <> 0 Then
            Dr = DT.NewRow
            Dr.Item("Disciplinare_Des") = "Reg 2092/91"
            Dr.Item("Disciplinare_Cod") = -1
            DT.Rows.Add(Dr)
        End If


        'Verifico la possibilità di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        Try

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari

            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            objWs.NewWS(ObjDownloadWs,
                            Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString(),
                            objParametri_Utenti)

            'Richiamo il disciplinare pubblico
            Dati = ObjDownloadWs.Leggi_Disciplinari2(CInt(Disciplinare_Cod),
                                                    CInt(0),
                                                    CInt(VEG_COD),
                                                    CInt(Reg_Cod),
                                                    CInt(Flag_Privato_Pubblico),
                                                    objParametri_Server.FinestraTemporaleInizio,
                                                    objParametri_Server.FinestraTemporaleFine,
                                                    CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                    CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                    Log_Errori)

        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante la chiamata al ws dei Disciplinari:" & Chr(13) &
                   ex.Message
        End Try

        If Log_Errori = "" Then

            If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                XmlDocumento.LoadXml(Dati)
                XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                If XmlNodo IsNot Nothing Then
                    For Each XmlElemento In XmlNodo
                        Dr = DT.NewRow
                        Dr.Item("Disciplinare_Des") = CStr(XmlElemento.GetAttribute("nomeesteso"))
                        Dr.Item("Disciplinare_Cod") = CInt(XmlElemento.GetAttribute("cod_regolamento"))
                        DT.Rows.Add(Dr)
                    Next
                Else
                    Log_Errori = "Nessun dato di risposta dal WS dei Disciplinari."
                End If
            Else
                Log_Errori = "Nessun dato di risposta dal WS dei Disciplinari."
            End If
        End If

        Return DT


    End Function

    'A differenza della precedente i DPI vengono letti nel metaschema locale
    'NON DEVE ESSERE USATA se si ha necessità di filtrare sulle specie
    Public Function Disciplinari_LeggiDT_Metaschema(ByRef objSession As System.Web.SessionState.HttpSessionState,
                                        ByVal Disciplinare_Cod As Integer,
                                        ByVal Flag_Privato_Pubblico As Integer,
                                        ByVal Includi_Nessuno As Boolean,
                                        ByVal Includi_Biologico As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig) As DataTable

        Dim DT As New DataTable
        Dim Dr As DataRow

        Dim strFiltro As String = ""

        DT.Columns.Add(New DataColumn("Disciplinare_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Disciplinare_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Flag_Privato_Pubblico", GetType(Integer)))

        'Aggiungo la voce nulla
        If Includi_Nessuno Then
            Dr = DT.NewRow
            Dr.Item("Disciplinare_Des") = "Nessun Disciplinare"
            Dr.Item("Disciplinare_Cod") = 0
            Dr.Item("Flag_Privato_Pubblico") = 0
            DT.Rows.Add(Dr)
        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico Then
            Dr = DT.NewRow
            Dr.Item("Disciplinare_Des") = Descrizione_Regolamento_Bio
            Dr.Item("Disciplinare_Cod") = -2
            Dr.Item("Flag_Privato_Pubblico") = 0
            DT.Rows.Add(Dr)
        End If

        'Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)

        If objWeb.Flag_DisciplinarePrivato = False Then
            strFiltro = " (Flag_Privato_Pubblico = 1) "
        Else
            If Flag_Privato_Pubblico <> 0 Then
                strFiltro = " (Flag_Privato_Pubblico = " & Flag_Privato_Pubblico.ToString & ") "
            Else
                strFiltro = " (Flag_Privato_Pubblico = 1 OR Flag_Privato_Pubblico = 2) "
            End If
        End If

        Dim DTDpi As DataTable
        Dim objDpi As New AgronicaCoreDpiDAL.Dpi_R
        Dim i As Integer
        DTDpi = objDpi.Leggi_DPI_Regolamenti(-1,
                                          Disciplinare_Cod,
                                          3,
                                          objParametri_Server.FinestraTemporaleInizio,
                                          objParametri_Server.FinestraTemporaleFine,
                                          strFiltro,
                                          " NomeEsteso ASC ",
                                          objParametri_Server)

        For i = 0 To DTDpi.Rows.Count - 1
            Dr = DT.NewRow
            Dr.Item("Disciplinare_Des") = DTDpi.Rows(i).Item("nomeEsteso")
            Dr.Item("Disciplinare_Cod") = DTDpi.Rows(i).Item("COD_REGOLAMENTO")
            Dr.Item("Flag_Privato_Pubblico") = DTDpi.Rows(i).Item("Flag_Privato_Pubblico")
            DT.Rows.Add(Dr)
        Next

        Return DT

    End Function

End Class
