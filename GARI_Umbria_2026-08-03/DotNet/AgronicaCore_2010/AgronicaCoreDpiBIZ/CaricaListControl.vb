Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports System.Web.UI.WebControls

Public Class CaricaListControl
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################################
    ' CaricaCombo_RcDpi 
    '###############################################################################
    Public Sub RcDpi(ByRef Controllo As ListControl, _
                                ByVal PrimaRiga_Flag As Boolean, _
                                ByVal PrimaRiga_Text As String, _
                                ByVal PrimaRiga_Value As String, _
                                ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                 ByVal Id_RcDpi As Integer, _
                                 ByVal Veg_Cod As Integer, _
                                 ByVal Grfi_Cod As Integer, _
                                 ByVal Disciplinare_Cod As Integer, _
                                 ByVal Flag_Protetto As Integer, _
                                 ByVal Tipo_Testata As Integer)


        Dim strErr As String

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

            '##############################  Chiama WS - "Leggi_RaggruppamentiDPI" ################################
            Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
            Dim Dati As String

            Try

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs, _
                                System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString(), _
                                objParametri_Utenti)


                Dati = ObjDownloadWs.Leggi_RaggruppamentiDPI2(CInt(Id_RcDpi), _
                                                            CInt(Veg_Cod), _
                                                            CInt(Grfi_Cod), _
                                                            CInt(Disciplinare_Cod), _
                                                            CInt(Flag_Protetto), _
                                                            CInt(Tipo_Testata), _
                                                            CStr(objSession("ASG_Utente_Username_Crypt").ToString), _
                                                            CStr(objSession("ASG_Utente_Password_Crypt").ToString), _
                                                            strErr)


                If strErr = "" Then

                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                        Dim XmlDocumento As New System.Xml.XmlDocument
                        Dim XmlNodo As System.Xml.XmlNodeList
                        Dim XmlElemento As System.Xml.XmlElement
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")





                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo

                                Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nome")), _
                                                CInt(XmlElemento.GetAttribute("id_rcdpi"))))

                            Next
                        End If
                    End If

                End If

            Catch ex As Exception

                'Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!"

            End Try

    End Sub




    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' nuova versione 
    ''' default -->
    ''' Includi_Nessuno As Boolean = True
    ''' Includi_Biologico As Boolean = True
    ''' Consultazione As Boolean = False
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub Disciplinari(ByRef Controllo As ListControl, _
                            ByVal PrimaRiga_Flag As Boolean, _
                            ByVal PrimaRiga_Text As String, _
                            ByVal PrimaRiga_Value As String, _
                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Reg_Cod As Integer, _
                                        ByVal Disciplinare_Cod As Integer, _
                                        ByVal VEG_COD As Integer, _
                                        ByVal Id_RcDpi As Integer, _
                                        ByVal Flag_Privato_Pubblico As Integer, _
                                        ByVal Includi_Nessuno As Boolean, _
                                        ByVal Includi_Biologico As Boolean, _
                                        ByVal Consultazione As Boolean)


        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement


        'Pulisco la combo
        Controllo.Items.Clear()

        'Aggiungo la voce nulla
        If Includi_Nessuno Then
            Controllo.Items.Add(New ListItem("Nessun Disciplinare", "0"))
        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico And VEG_COD <> 0 Then
            Controllo.Items.Add(New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio))
        End If


        'Verifico la possibilit? di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then


        Try

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            objWs.NewWS(ObjDownloadWs, _
                            objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari, _
                            objParametri_Utenti)


            'Richiamo il disciplinare pubblico
            Dati = ObjDownloadWs.Leggi_Disciplinari2(CInt(Disciplinare_Cod), _
                                                    CInt(0), _
                                                    CInt(VEG_COD), _
                                                    CInt(Reg_Cod), _
                                                    CInt(Flag_Privato_Pubblico), _
                                                    objParametri_Server.FinestraTemporaleInizio, _
                                                    objParametri_Server.FinestraTemporaleFine, _
                                                    CStr(objSession("ASG_Utente_Username_Crypt").ToString), _
                                                    CStr(objSession("ASG_Utente_Password_Crypt").ToString), _
                                                    strErr)


            If strErr = "" Then
                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                    If Not XmlNodo Is Nothing Then
                        For Each XmlElemento In XmlNodo
                            Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nomeesteso")), _
                                                                   CInt(XmlElemento.GetAttribute("cod_regolamento"))))
                        Next
                    End If
                End If
            End If

        Catch ex As Exception

            'Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) & _
            '       ex.Message

        End Try


    End Sub









    '###############################################################################
    ' CaricaCombo_Disciplinari 
    '###############################################################################
    ' default 
    'FinestraTemp_Inizio agrodatainizio
    'FinestraTemp_Fine agrodatafine
    'Includi_Nessuno true
    'Includi_Biologico true
    'Consultazione false
    Public Sub Disciplinari_2(ByRef Controllo As ListControl, _
                            ByVal PrimaRiga_Flag As Boolean, _
                            ByVal PrimaRiga_Text As String, _
                            ByVal PrimaRiga_Value As String, _
                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Reg_Cod As Integer, _
                                        ByVal Disciplinare_Cod As Integer, _
                                        ByVal VEG_COD As Integer, _
                                        ByVal Id_RcDpi As Integer, _
                                        ByVal Flag_Privato_Pubblico As Integer, _
                                        ByVal FinestraTemp_Inizio As Date, _
                                        ByVal FinestraTemp_Fine As Date, _
                                        ByVal Includi_Nessuno As Boolean, _
                                        ByVal Includi_Biologico As Boolean, _
                                        ByVal Consultazione As Boolean, _
                                        ByVal Tipo_Testata As Integer)


        Dim Testo As String
        Dim Valore As String

        Dim Dpi_Cod As Integer
        Dim Dpi_Des As String

        Dim IdRcdpi As Integer
        Dim Grfi_Cod As Integer
        Dim Flag_Protetto As Integer

        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim DatiRcdpi As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement


        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        'Aggiungo la voce nulla
        If Includi_Nessuno Then
            Controllo.Items.Add(New ListItem("Nessun Disciplinare", "0"))
        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico And VEG_COD <> 0 Then
            Controllo.Items.Add(New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio))
        End If


        'Verifico la possibilit? di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then

        Try


            Dim agroWs As String
            If IsNothing(System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                'creo l'agrowebconfig
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            Else
                agroWs = System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
            End If

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs, _
                            agroWs, _
                            objParametri_Utenti)


            'Richiamo il disciplinare pubblico
            Dati = ObjDownloadWs.Leggi_Disciplinari_xTestata(CInt(Disciplinare_Cod), _
                                                            CInt(0), _
                                                            CInt(VEG_COD), _
                                                            CInt(Reg_Cod), _
                                                            CInt(Flag_Privato_Pubblico), _
                                                            CInt(Tipo_Testata), _
                                                            CDate(FinestraTemp_Inizio), _
                                                            CDate(FinestraTemp_Fine), _
                                                            CStr(objSession("ASG_Utente_Username_Crypt").ToString), _
                                                            CStr(objSession("ASG_Utente_Password_Crypt").ToString), _
                                                            strErr)
            ObjDownloadWs.Dispose()

            If strErr = "" Then

                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                    If Not XmlNodo Is Nothing Then
                        For Each XmlElemento In XmlNodo

                            Dpi_Cod = CInt(XmlElemento.GetAttribute("cod_regolamento"))
                            Dpi_Des = CStr(XmlElemento.GetAttribute("nomeesteso"))

                            IdRcdpi = CInt(XmlElemento.GetAttribute("id_rcdpi"))

                            If Not IsDBNull(XmlElemento.GetAttribute("grfi_cod")) Then
                                Grfi_Cod = CInt(XmlElemento.GetAttribute("grfi_cod"))
                            Else
                                Grfi_Cod = 0
                            End If

                            If Not IsDBNull(XmlElemento.GetAttribute("flag_protetto")) Then
                                Flag_Protetto = CInt(XmlElemento.GetAttribute("flag_protetto"))
                                'se Flag_Protetto = 2 (= non applicabile) 
                                'lo imposto a 0 x uniformare
                                'e riuscire a distinguere poi..
                                'If Flag_Protetto = 2 Then
                                '    Flag_Protetto = 0
                                'End If
                            Else
                                Flag_Protetto = 0
                            End If

                            Controllo.Items.Add(New ListItem(Dpi_Des & "-" & CStr(XmlElemento.GetAttribute("nome")), _
                                                                Dpi_Cod & "/" & IdRcdpi & _
                                                                "/" & Grfi_Cod & "/" & Flag_Protetto))

                        Next
                    End If
                End If
            End If

        Catch ex As Exception

            'Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) & _
            '       ex.Message

        End Try

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' nuova versione 
    ''' default -->
    ''' Flag_Privato_Pubblico As integer  Pubblico=1; Privato=2 Tutti=0
    ''' Includi_Nessuno As Boolean = True
    ''' Includi_Biologico As Boolean = True
    ''' Consultazione As Boolean = False
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub Disciplinari_Elenco(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Reg_Cod As Integer,
                                        ByVal VEG_COD As Integer,
                                        ByVal Id_RcDpi As Integer,
                                        ByVal Flag_Privato_Pubblico As Integer,
                                        ByVal Includi_Nessuno As Boolean,
                                        ByVal Includi_Biologico As Boolean,
                                        ByVal Consultazione As Boolean,
                                        ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                   Optional ByVal IncludiPUA_Regolamento_Cod As Boolean = False)


        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))

        'Pulisco la combo
        Controllo.Items.Clear()


        ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)


        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)


                If objWeb.Flag_DisciplinarePrivato = False Then
                    Flag_Privato_Pubblico = 1
                Else
                    Flag_Privato_Pubblico = 0
                End If

                'If objSession("permessoDPIPrivati") = False Then
                '    Flag_Privato_Pubblico = 1
                'Else
                '    Flag_Privato_Pubblico = 0
                'End If

                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        CStr(objSession("ASG_SuperUser_CodFiscale").ToString),
                                                        CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                        CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")
                                Dt.Rows.Add(Dr)
                                'Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nomeesteso")), _
                                '                                       CInt(XmlElemento.GetAttribute("cod_regolamento"))))
                            Next
                        End If
                    End If
                End If

                If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

                    Dim Dv As New DataView
                    Dim i As Integer

                    Dt.TableName = "Dpi"
                    Dv.Table = Dt
                    Dv.Sort = "nomeEsteso ASC"

                    For i = 0 To Dv.Count - 1

                        If IncludiPUA_Regolamento_Cod = False Then
                            Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1) & "/" & Dv.Item(i).Item(2)))
                        Else
                            Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1) & "/" & Dv.Item(i).Item(2) & "/" & Dv.Item(i).Item(3)))
                        End If
                        'If i >= 1 AndAlso Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
                        '    Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1) & "/" & Dv.Item(i).Item(2)))
                        'Else
                        '    Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1) & "/" & Dv.Item(i).Item(2) & "/" & Dv.Item(i).Item(3)))
                        'End If
                    Next

                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

            End Try

        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico Then
            Controllo.Items.Insert(0, New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio))
        End If

        'Aggiungo la voce nulla
        If Includi_Nessuno Then
            Controllo.Items.Insert(0, New ListItem("Nessun Disciplinare", "0"))
        End If

    End Sub

    Public Sub Disciplinari_Elenco_Ente(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Reg_Cod As Integer,
                                        ByVal VEG_COD As Integer,
                                        ByVal Id_RcDpi As Integer,
                                        ByVal Flag_Privato_Pubblico As Integer,
                                        ByVal Includi_Nessuno As Boolean,
                                        ByVal Includi_Biologico As Boolean,
                                        ByVal Consultazione As Boolean,
                                        ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                   Optional ByVal IncludiPUA_Regolamento_Cod As Boolean = False,
                                   Optional ByVal seleziona_Nome_e_codRegolamento As Boolean = False)


        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ente_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))

        'Pulisco la combo
        Controllo.Items.Clear()


        ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)


        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)


                If objWeb.Flag_DisciplinarePrivato = False Then
                    Flag_Privato_Pubblico = 1
                Else
                    Flag_Privato_Pubblico = 0
                End If

                'If objSession("permessoDPIPrivati") = False Then
                '    Flag_Privato_Pubblico = 1
                'Else
                '    Flag_Privato_Pubblico = 0
                'End If

                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        CStr(objSession("ASG_SuperUser_CodFiscale").ToString),
                                                        CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                        CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("Ente_Cod") = XmlElemento.GetAttribute("idente")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")
                                Dt.Rows.Add(Dr)
                                'Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nomeesteso")), _
                                '                                       CInt(XmlElemento.GetAttribute("cod_regolamento"))))
                            Next
                        End If
                    End If
                End If

                If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

                    Dim Dv As New DataView
                    Dim i As Integer

                    Dt.TableName = "Dpi"
                    Dv.Table = Dt
                    Dv.Sort = "nomeEsteso ASC"

                    Dim anno = Now.Date.Year

                    For i = 0 To Dv.Count - 1
                        Dim name As String
                        If seleziona_Nome_e_codRegolamento Then
                            name = Dv.Item(i).Item(0)
                        Else
                            name = Dv.Item(i).Item(0).ToString.Replace(anno, "")
                        End If

                        Dim cod_regolamento = Dv.Item(i).Item(1)
                        If IncludiPUA_Regolamento_Cod = False Then
                            Controllo.Items.Add(New ListItem(name, "e:" & Dv.Item(i).Item(4) & "/" & "fp:" & Dv.Item(i).Item(2)))
                        ElseIf seleziona_Nome_e_codRegolamento = True Then
                            Controllo.Items.Add(New ListItem(name, cod_regolamento))
                        Else
                            Controllo.Items.Add(New ListItem(name, Dv.Item(i).Item(1) & "/" & Dv.Item(i).Item(2) & "/" & Dv.Item(i).Item(3) & "/" & Dv.Item(i).Item(4)))
                        End If
                        'If i >= 1 AndAlso Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
                        '    Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1) & "/" & Dv.Item(i).Item(2)))
                        'Else
                        '    Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1) & "/" & Dv.Item(i).Item(2) & "/" & Dv.Item(i).Item(3)))
                        'End If
                    Next

                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

            End Try

        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico Then
            Controllo.Items.Insert(0, New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio))
        End If

        'Aggiungo la voce nulla
        If Includi_Nessuno Then
            Controllo.Items.Insert(0, New ListItem("Nessun Disciplinare", "0"))
        End If

    End Sub

    Public Sub Disciplinari_Elenco_TuttigliElemInChiave(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Reg_Cod As Integer,
                                        ByVal VEG_COD As Integer,
                                        ByVal Id_RcDpi As Integer,
                                        ByVal Flag_Privato_Pubblico As Integer,
                                        ByVal Includi_Nessuno As Boolean,
                                        ByVal Includi_Biologico As Boolean,
                                        ByVal Consultazione As Boolean,
                                        ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                   Optional ByVal IncludiPUA_Regolamento_Cod As Boolean = False)

        Dim messaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreDpiBIZ.Disciplinari_Elenco_TuttigliElemInChiave()"
        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))

        'Pulisco la combo
        Controllo.Items.Clear()


        ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)


        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)


                If objWeb.Flag_DisciplinarePrivato = False Then
                    Flag_Privato_Pubblico = 1
                Else
                    Flag_Privato_Pubblico = 0
                End If

                'If objSession("permessoDPIPrivati") = False Then
                '    Flag_Privato_Pubblico = 1
                'Else
                '    Flag_Privato_Pubblico = 0
                'End If

                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        CStr(objSession("ASG_SuperUser_CodFiscale").ToString),
                                                        CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                        CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")
                                Dt.Rows.Add(Dr)
                            Next
                        End If
                    End If
                End If

                If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

                    For i = 0 To Dt.Rows.Count - 1
                        Controllo.Items.Add(New ListItem(Dt.Rows(i).Item(0), Dt.Rows(i).Item(1) & "/" & Dt.Rows(i).Item(2) & "/" & Dt.Rows(i).Item(3) & "/" & Dt.Rows(i).Item(4)))
                    Next

                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

                messaggioErrore = Dati

                Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
                Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

            End Try

        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico Then
            Controllo.Items.Insert(0, New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio))
        End If

        'Aggiungo la voce nulla
        If Includi_Nessuno Then
            Controllo.Items.Insert(0, New ListItem("Nessun Disciplinare", "0"))
        End If

    End Sub


    Public Sub Trova_Ente_Disciplinare(valore_disciplinare_old As String,
                                       ByVal PrimaRiga_Flag As Boolean,
                                       ByVal PrimaRiga_Text As String,
                                       ByVal PrimaRiga_Value As String,
                                       ByRef objSession As System.Web.SessionState.HttpSessionState,
                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal Reg_Cod As Integer,
                                       ByVal VEG_COD As Integer,
                                       ByVal Id_RcDpi As Integer,
                                       ByVal Flag_Privato_Pubblico As Integer,
                                       ByVal Includi_Nessuno As Boolean,
                                       ByVal Includi_Biologico As Boolean,
                                       ByVal Consultazione As Boolean,
                                       ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                       ByRef Ente_Cod As String,
                                       ByRef pubblicoPrivato As String,
                                       Optional ByVal ASG_SuperUser_CodFiscale As String = "",
                                       Optional ByVal ASG_Utente_Username_Crypt As String = "",
                                       Optional ByVal ASG_Utente_Password_Crypt As String = "")


        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ente_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))

        Dim SuperUser_CodFiscale As String = ""

        Dim Utente_Username_Crypt As String = ""

        Dim Utente_Password_Crypt As String = ""

        ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)


        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If

                If String.IsNullOrEmpty(agroWs) Then
                    Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)


                If objWeb.Flag_DisciplinarePrivato = False Then
                    Flag_Privato_Pubblico = 1
                Else
                    Flag_Privato_Pubblico = 0
                End If

                'If objSession("permessoDPIPrivati") = False Then
                '    Flag_Privato_Pubblico = 1
                'Else
                '    Flag_Privato_Pubblico = 0
                'End If

                If IsNothing(objSession) AndAlso
                    Not String.IsNullOrEmpty(ASG_SuperUser_CodFiscale) AndAlso Not String.IsNullOrEmpty(ASG_Utente_Username_Crypt) AndAlso Not String.IsNullOrEmpty(ASG_Utente_Password_Crypt) Then
                    SuperUser_CodFiscale = ASG_SuperUser_CodFiscale
                    Utente_Username_Crypt = ASG_Utente_Username_Crypt
                    Utente_Password_Crypt = ASG_Utente_Password_Crypt
                Else
                    SuperUser_CodFiscale = CStr(objSession("ASG_SuperUser_CodFiscale").ToString)
                    Utente_Username_Crypt = CStr(objSession("ASG_Utente_Username_Crypt").ToString)
                    Utente_Password_Crypt = CStr(objSession("ASG_Utente_Password_Crypt").ToString)
                End If

                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        SuperUser_CodFiscale,
                                                        Utente_Username_Crypt,
                                                        Utente_Password_Crypt,
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("Ente_Cod") = XmlElemento.GetAttribute("idente")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")
                                Dim valore_combo = XmlElemento.GetAttribute("cod_regolamento") & "/" & XmlElemento.GetAttribute("pubblicoprivato")
                                If valore_combo = valore_disciplinare_old Then
                                    Ente_Cod = CStr(XmlElemento.GetAttribute("idente"))
                                    pubblicoPrivato = CStr(XmlElemento.GetAttribute("pubblicoprivato"))
                                    Exit Sub
                                End If

                                Dt.Rows.Add(Dr)
                                'Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nomeesteso")), _
                                '                                       CInt(XmlElemento.GetAttribute("cod_regolamento"))))
                            Next
                        End If
                    End If
                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

            End Try

        End If

    End Sub

    Public Sub Trova_Ente_Disciplinare_NoSession(valore_disciplinare_old As String,
                                       ASG_SuperUser_CodFiscale As String,
                                       ASG_Utente_Username_Crypt As String,
                                       ASG_Utente_Password_Crypt As String,
                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal Reg_Cod As Integer,
                                       ByVal VEG_COD As Integer,
                                       ByVal Id_RcDpi As Integer,
                                       ByVal Flag_Privato_Pubblico As Integer,
                                       ByVal Includi_Nessuno As Boolean,
                                       ByVal Includi_Biologico As Boolean,
                                       ByVal Consultazione As Boolean,
                                       ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                       ByRef Ente_Cod As String,
                                       ByRef pubblicoPrivato As String)


        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ente_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))

        ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)


        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)


                If objWeb.Flag_DisciplinarePrivato = False Then
                    Flag_Privato_Pubblico = 1
                Else
                    Flag_Privato_Pubblico = 0
                End If

                'If objSession("permessoDPIPrivati") = False Then
                '    Flag_Privato_Pubblico = 1
                'Else
                '    Flag_Privato_Pubblico = 0
                'End If

                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        ASG_SuperUser_CodFiscale,
                                                        ASG_Utente_Username_Crypt,
                                                        ASG_Utente_Password_Crypt,
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("Ente_Cod") = XmlElemento.GetAttribute("idente")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")
                                Dim valore_combo = XmlElemento.GetAttribute("cod_regolamento") & "/" & XmlElemento.GetAttribute("pubblicoprivato")
                                If valore_combo = valore_disciplinare_old Then
                                    Ente_Cod = CStr(XmlElemento.GetAttribute("idente"))
                                    pubblicoPrivato = CStr(XmlElemento.GetAttribute("pubblicoprivato"))
                                    Exit Sub
                                End If

                                Dt.Rows.Add(Dr)
                                'Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nomeesteso")), _
                                '                                       CInt(XmlElemento.GetAttribute("cod_regolamento"))))
                            Next
                        End If
                    End If
                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori In fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

            End Try

        End If

    End Sub

    Public Sub Trova_Disciplinare_Ente(valore_ente_old As String,
                                       valore_fp_old As String,
                                       ByVal PrimaRiga_Flag As Boolean,
                                       ByVal PrimaRiga_Text As String,
                                       ByVal PrimaRiga_Value As String,
                                       ByRef objSession As System.Web.SessionState.HttpSessionState,
                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal Reg_Cod As Integer,
                                       ByVal VEG_COD As Integer,
                                       ByVal Id_RcDpi As Integer,
                                       ByVal Flag_Privato_Pubblico As Integer,
                                       ByVal Includi_Nessuno As Boolean,
                                       ByVal Includi_Biologico As Boolean,
                                       ByVal Consultazione As Boolean,
                                       ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                       ByRef Disciplinare_Cod As String,
                                       ByRef pubblicoPrivato As String,
                                       Optional ByVal ASG_SuperUser_CodFiscale As String = "",
                                       Optional ByVal ASG_Utente_Username_Crypt As String = "",
                                       Optional ByVal ASG_Utente_Password_Crypt As String = "")


        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ente_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))

        Dim SuperUser_CodFiscale As String = ""

        Dim Utente_Username_Crypt As String = ""

        Dim Utente_Password_Crypt As String = ""

        ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)


        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If

                If String.IsNullOrEmpty(agroWs) Then
                    Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)


                If objWeb.Flag_DisciplinarePrivato = False Then
                    Flag_Privato_Pubblico = 1
                Else
                    Flag_Privato_Pubblico = 0
                End If

                'If objSession("permessoDPIPrivati") = False Then
                '    Flag_Privato_Pubblico = 1
                'Else
                '    Flag_Privato_Pubblico = 0
                'End If

                If IsNothing(objSession) AndAlso
                    Not String.IsNullOrEmpty(ASG_SuperUser_CodFiscale) AndAlso Not String.IsNullOrEmpty(ASG_Utente_Username_Crypt) AndAlso Not String.IsNullOrEmpty(ASG_Utente_Password_Crypt) Then
                    SuperUser_CodFiscale = ASG_SuperUser_CodFiscale
                    Utente_Username_Crypt = ASG_Utente_Username_Crypt
                    Utente_Password_Crypt = ASG_Utente_Password_Crypt
                Else
                    SuperUser_CodFiscale = CStr(objSession("ASG_SuperUser_CodFiscale").ToString)
                    Utente_Username_Crypt = CStr(objSession("ASG_Utente_Username_Crypt").ToString)
                    Utente_Password_Crypt = CStr(objSession("ASG_Utente_Password_Crypt").ToString)
                End If

                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        SuperUser_CodFiscale,
                                                        Utente_Username_Crypt,
                                                        Utente_Password_Crypt,
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("Ente_Cod") = XmlElemento.GetAttribute("idente")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")
                                Dim valore_combo = XmlElemento.GetAttribute("cod_regolamento") & "/" & XmlElemento.GetAttribute("pubblicoprivato")
                                If CStr(XmlElemento.GetAttribute("idente")) = valore_ente_old AndAlso CStr(XmlElemento.GetAttribute("pubblicoprivato")) = valore_fp_old Then
                                    Disciplinare_Cod = CStr(XmlElemento.GetAttribute("cod_regolamento"))
                                    pubblicoPrivato = CStr(XmlElemento.GetAttribute("pubblicoprivato"))
                                    Exit Sub
                                End If

                                Dt.Rows.Add(Dr)
                                'Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nomeesteso")), _
                                '                                       CInt(XmlElemento.GetAttribute("cod_regolamento"))))
                            Next
                        End If
                    End If
                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori In fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

            End Try

        End If

    End Sub

    Public Sub Trova_DisciplinareCompleto_Ente(valore_ente_old As String,
                                       valore_fp_old As String,
                                       ASG_SuperUser_CodFiscale As String,
                                       ASG_Utente_Username_Crypt As String,
                                       ASG_Utente_Password_Crypt As String,
                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal Reg_Cod As Integer,
                                       ByVal VEG_COD As Integer,
                                       ByVal Id_RcDpi As Integer,
                                       ByVal Flag_Privato_Pubblico As Integer,
                                       ByVal Includi_Nessuno As Boolean,
                                       ByVal Includi_Biologico As Boolean,
                                       ByVal Consultazione As Boolean,
                                       ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                       ByRef Disciplinare As String,
                                       ByRef Dpi_Cod As String,
                                       ByRef Reg_Cod_out As String,
                                       ByRef Regolamento_Concimazione_Cod As String,
                                       ByRef Flag_PubblicoPrivato As String,
                                       ByRef id_tr As String,
                                       ByRef MetodoProduzione_Cod As Integer,
                                       ByRef MetodoProduzione_Des As String)


        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ente_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))

        ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)


        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)


                If objWeb.Flag_DisciplinarePrivato = False Then
                    Flag_Privato_Pubblico = 1
                Else
                    Flag_Privato_Pubblico = 0
                End If

                'If objSession("permessoDPIPrivati") = False Then
                '    Flag_Privato_Pubblico = 1
                'Else
                '    Flag_Privato_Pubblico = 0
                'End If

                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        ASG_SuperUser_CodFiscale,
                                                        ASG_Utente_Username_Crypt,
                                                        ASG_Utente_Password_Crypt,
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("Ente_Cod") = XmlElemento.GetAttribute("idente")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")
                                Dim valore_combo = XmlElemento.GetAttribute("cod_regolamento") & "/" & XmlElemento.GetAttribute("pubblicoprivato")
                                If CStr(XmlElemento.GetAttribute("idente")) = valore_ente_old AndAlso CStr(XmlElemento.GetAttribute("pubblicoprivato")) = valore_fp_old Then
                                    Disciplinare = CStr(XmlElemento.GetAttribute("nomeesteso"))
                                    Dpi_Cod = CStr(XmlElemento.GetAttribute("cod_regolamento"))
                                    Reg_Cod_out = "1"
                                    Regolamento_Concimazione_Cod = CStr(XmlElemento.GetAttribute("pua_regolamento_cod"))
                                    Flag_PubblicoPrivato = CStr(XmlElemento.GetAttribute("pubblicoprivato"))
                                    id_tr = CStr(XmlElemento.GetAttribute("id_tr"))
                                    MetodoProduzione_Cod = 1
                                    MetodoProduzione_Des = "Integrato"
                                    Exit Sub
                                End If

                                Dt.Rows.Add(Dr)
                                'Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nomeesteso")), _
                                '                                       CInt(XmlElemento.GetAttribute("cod_regolamento"))))
                            Next
                        End If
                    End If
                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori In fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

            End Try

        End If

    End Sub

    Public Sub Trova_Regolamento_Ente(valore_ente_old As String,
                                       valore_fp_old As String,
                                       ByVal PrimaRiga_Flag As Boolean,
                                       ByVal PrimaRiga_Text As String,
                                       ByVal PrimaRiga_Value As String,
                                       ByRef objSession As System.Web.SessionState.HttpSessionState,
                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal Reg_Cod As Integer,
                                       ByVal VEG_COD As Integer,
                                       ByVal Id_RcDpi As Integer,
                                       ByVal Flag_Privato_Pubblico As Integer,
                                       ByVal Includi_Nessuno As Boolean,
                                       ByVal Includi_Biologico As Boolean,
                                       ByVal Consultazione As Boolean,
                                       ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                       ByRef Regolamento_Cod As String)


        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ente_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))

        ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)


        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)


                If objWeb.Flag_DisciplinarePrivato = False Then
                    Flag_Privato_Pubblico = 1
                Else
                    Flag_Privato_Pubblico = 0
                End If

                'If objSession("permessoDPIPrivati") = False Then
                '    Flag_Privato_Pubblico = 1
                'Else
                '    Flag_Privato_Pubblico = 0
                'End If

                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        CStr(objSession("ASG_SuperUser_CodFiscale").ToString),
                                                        CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                        CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("Ente_Cod") = XmlElemento.GetAttribute("idente")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")
                                Dim valore_combo = XmlElemento.GetAttribute("cod_regolamento") & "/" & XmlElemento.GetAttribute("pubblicoprivato")
                                If CStr(XmlElemento.GetAttribute("idente")) = valore_ente_old AndAlso CStr(XmlElemento.GetAttribute("pubblicoprivato")) = valore_fp_old Then
                                    Regolamento_Cod = CStr(XmlElemento.GetAttribute("pua_regolamento_cod"))
                                    Exit Sub
                                End If

                                Dt.Rows.Add(Dr)
                                'Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nomeesteso")), _
                                '                                       CInt(XmlElemento.GetAttribute("cod_regolamento"))))
                            Next
                        End If
                    End If
                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori In fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

            End Try

        End If

    End Sub

    'A differenza della precedente i DPI vengono letti nel metaschema locale
    'NON DEVE ESSERE USATA se si ha necessità di filtrare sulle specie
    Public Sub Disciplinari_Elenco_Metaschema(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Flag_Privato_Pubblico As Integer,
                                        ByVal Includi_Nessuno As Boolean,
                                        ByVal Includi_Biologico As Boolean,
                                        ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig)

        Dim Dt As DataTable
        Dim i As Integer
        Dim objDpi As New AgronicaCoreDpiDAL.Dpi_R
        Dim strFiltro As String = ""

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        'Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)

        If objWeb.Flag_DisciplinarePrivato = False Then
            strFiltro = " (Flag_Privato_Pubblico = 1) "
        Else
            If Flag_Privato_Pubblico <> 0 Then
                strFiltro = " (Flag_Privato_Pubblico = " & Flag_Privato_Pubblico.ToString & ") "
            Else
                strFiltro = " (Flag_Privato_Pubblico = 1 Or Flag_Privato_Pubblico = 2) "
            End If
        End If


        'If objSession("permessoDPIPrivati") = False Then
        '    strFiltro = " (Flag_Privato_Pubblico = 1) "
        'Else
        '    If Flag_Privato_Pubblico <> 0 Then
        '        strFiltro = " (Flag_Privato_Pubblico = " & Flag_Privato_Pubblico.ToString & ") "
        '    Else
        '        strFiltro = " (Flag_Privato_Pubblico = 1 Or Flag_Privato_Pubblico = 2) "
        '    End If
        'End If

        Dt = objDpi.Leggi_DPI_Regolamenti(-1,
                                          0,
                                          3,
                                          objParametri_Server.FinestraTemporaleInizio,
                                          objParametri_Server.FinestraTemporaleFine,
                                          strFiltro,
                                          " NomeEsteso ASC ",
                                          objParametri_Server)

        If Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1
                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("nomeEsteso"),
                                                Dt.Rows(i).Item("COD_REGOLAMENTO") & "/" & Dt.Rows(i).Item("Flag_Privato_Pubblico")))
            Next
        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico Then
            Controllo.Items.Insert(0, New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio))
        End If

        'Aggiungo la voce nulla
        If Includi_Nessuno Then
            Controllo.Items.Insert(0, New ListItem("Nessun Disciplinare", "0"))
        End If

    End Sub







    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' nuova versione 
    ''' default -->
    ''' Flag_Privato_Pubblico As integer  Pubblico=1; Privato=2 Tutti=0
    ''' Includi_Nessuno As Boolean = True
    ''' Includi_Biologico As Boolean = True
    ''' Consultazione As Boolean = False
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub Disciplinari_Elenco_SensaSession(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Reg_Cod As Integer,
                                        ByVal VEG_COD As Integer,
                                        ByVal Id_RcDpi As Integer,
                                        ByVal Flag_Privato_Pubblico As Integer,
                                        ByVal Includi_Nessuno As Boolean,
                                        ByVal Includi_Biologico As Boolean,
                                        ByVal Consultazione As Boolean,
                                        ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig)
        ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)

        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))

        'Pulisco la combo
        Controllo.Items.Clear()


        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(objWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = objWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)

                If objWeb.Flag_DisciplinarePrivato = False Then
                    Flag_Privato_Pubblico = 1
                Else
                    Flag_Privato_Pubblico = 0
                End If


                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
                Dim dt_utenti As DataTable
                dt_utenti = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_Server.UtenteUsername,
                                                                 "", Now.Today, 0, 0, objParametri_Utenti)


                'Dim asg_utente_username_cr As String = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile( _
                '                    objParametri_Server.UtenteUsername, _
                '                    AgroKey_EncoderDecoder)

                'Dim asg_utente_Password_Cr As String = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile( _
                '                    dt_utenti.Rows(0).Item("Password"), _
                '                    AgroKey_EncoderDecoder)

                Dim asg_SuperUser_username_cr As String = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(
                                    dt_utenti.Rows(0).Item("Username_SuperUser"),
                                    AgroKey_EncoderDecoder)

                Dim asg_SuperUser_Password_Cr As String = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(
                                    dt_utenti.Rows(0).Item("Password_SuperUser"),
                                    AgroKey_EncoderDecoder)

                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        objParametri_Server.PivaSuperUser,
                                                        asg_SuperUser_username_cr,
                                                        asg_SuperUser_Password_Cr,
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dt.Rows.Add(Dr)
                                'Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("nomeesteso")), _
                                '                                       CInt(XmlElemento.GetAttribute("cod_regolamento"))))
                            Next
                        End If
                    End If
                End If

                If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

                    Dim Dv As New DataView
                    Dim i As Integer

                    Dt.TableName = "Dpi"
                    Dv.Table = Dt
                    Dv.Sort = "nomeEsteso ASC"

                    For i = 0 To Dv.Count - 1
                        If i = 0 OrElse Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
                            'Aggiungo l'elemento al primo giro oppure quando un elemento differisce dal precedente
                            Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1) & "/" & Dv.Item(i).Item(2)))
                        End If
                    Next

                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori In fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

            End Try

        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico Then
            Controllo.Items.Insert(0, New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio))
        End If

        'Aggiungo la voce nulla
        If Includi_Nessuno Then

            'richiesta compatibilità con framework 1.1

            Dim ass As String = "AgronicaCoreDpiBiz.resources"

            'Controllo.Items.Insert(0, New ListItem(my.Resources.AgronicaCoreDPIBiz_Resx.NessunDisciplinare, "0"))
            Dim rm As System.Resources.ResourceManager
            rm = New System.Resources.ResourceManager(ass, Me.GetType().Assembly)
            Controllo.Items.Insert(0, New ListItem(rm.GetString("NessunDisciplinare")))
        End If

    End Sub














    Public Sub Disciplinari_ElencoxTestata(ByRef Controllo As ListControl,
                        ByVal PrimaRiga_Flag As Boolean,
                        ByVal PrimaRiga_Text As String,
                        ByVal PrimaRiga_Value As String,
                        ByRef objSession As System.Web.SessionState.HttpSessionState,
                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal Reg_Cod As Integer,
                                    ByVal VEG_COD As Integer,
                                    ByVal Id_RcDpi As Integer,
                                    ByVal Flag_Privato_Pubblico As Integer,
                                    ByVal FinestraTemp_Inizio As Date,
                                    ByVal FinestraTemp_Fine As Date,
                                    ByVal Includi_Nessuno As Boolean, ByVal TestoNessuno As String,
                                    ByVal Includi_Biologico As Boolean, ByVal TestoRegolamentoBio As String,
                                    ByVal Consultazione As Boolean,
                                    ByVal Tipo_Testata As Integer)



        If TestoNessuno = "" Then
            TestoNessuno = "Nessun Disciplinare"
        End If
        If TestoRegolamentoBio = "" Then
            TestoRegolamentoBio = Descrizione_Regolamento_Bio
        End If

        Dim Dpi_Cod As Integer
        Dim Dpi_Des As String

        Dim IdRcdpi As Integer
        Dim Grfi_Cod As Integer
        Dim Flag_Protetto As Integer
        Dim PubblicoPrivato As Integer

        Dim strErr As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        'Aggiungo la voce nulla
        If Includi_Nessuno = True Then
            Controllo.Items.Add(New ListItem(TestoNessuno, "0"))
        End If

        'Aggiungo la voce BIOLOGICO
        If Includi_Biologico = True And VEG_COD <> 0 Then
            Controllo.Items.Add(New ListItem(TestoRegolamentoBio, "-2"))
        End If


        Try


            Dim agroWs As String
            If IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                'creo l'agrowebconfig
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            Else
                agroWs = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
            End If

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            agroWs,
                            objParametri_Utenti)


            'Richiamo il disciplinare pubblico
            Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco_xTestata(CInt(0),
                                                            CInt(VEG_COD),
                                                            CInt(Reg_Cod),
                                                            CInt(Flag_Privato_Pubblico),
                                                            CInt(Tipo_Testata),
                                                            CDate(FinestraTemp_Inizio),
                                                            CDate(FinestraTemp_Fine),
                                                            CStr(objSession("ASG_SuperUser_CodFiscale").ToString),
                                                            CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                            CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                            strErr)
            ObjDownloadWs.Dispose()

            If strErr = "" Then

                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                    If Not XmlNodo Is Nothing Then
                        For Each XmlElemento In XmlNodo

                            Dpi_Cod = CInt(XmlElemento.GetAttribute("cod_regolamento"))
                            Dpi_Des = CStr(XmlElemento.GetAttribute("nomeesteso"))

                            IdRcdpi = CInt(XmlElemento.GetAttribute("id_rcdpi"))

                            If Not IsDBNull(XmlElemento.GetAttribute("grfi_cod")) Then
                                Grfi_Cod = CInt(XmlElemento.GetAttribute("grfi_cod"))
                            Else
                                Grfi_Cod = 0
                            End If

                            If Not IsDBNull(XmlElemento.GetAttribute("flag_protetto")) Then
                                Flag_Protetto = CInt(XmlElemento.GetAttribute("flag_protetto"))
                                'se Flag_Protetto = 2 (= non applicabile) 
                                'lo imposto a 0 x uniformare
                                'e riuscire a distinguere poi..
                                'If Flag_Protetto = 2 Then
                                '    Flag_Protetto = 0
                                'End If
                            Else
                                Flag_Protetto = 0
                            End If

                            PubblicoPrivato = XmlElemento.GetAttribute("pubblicoprivato")

                            Controllo.Items.Add(New ListItem(Dpi_Des & "-" & CStr(XmlElemento.GetAttribute("nome")),
                                                                Dpi_Cod & "/" & IdRcdpi &
                                                                "/" & Grfi_Cod & "/" & Flag_Protetto & "/" & PubblicoPrivato))

                        Next
                    End If
                End If
            End If

        Catch ex As Exception

            'Dati = "Si sono verificati errori In fase di chiamata al WebService Discliplinari!" & Chr(13) & _
            '       ex.Message

        End Try

    End Sub





    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' nuova versione 
    ''' default -->
    ''' Includi_Nessuno As Boolean = True
    ''' Includi_Biologico As Boolean = True
    ''' Consultazione As Boolean = False
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub DisciplinariElenco_prova(ByRef Controllo As ListControl,
                                    ByVal PrimaRiga_Flag As Boolean,
                                    ByVal PrimaRiga_Text As String,
                                    ByVal PrimaRiga_Value As String,
                                        ByVal Reg_Cod As Integer,
                                        ByVal Disciplinare_Cod As Integer,
                                        ByVal VEG_COD As Integer,
                                        ByVal Id_RcDpi As Integer,
                                        ByVal Flag_AnchePrivati As Boolean,
                                        ByVal Includi_Nessuno As Boolean,
                                        ByVal Includi_Biologico As Boolean,
                                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig)

        Dim ObjDownloadWsDpi As WS_Disciplinari.AgroWS_Disciplinari
        Dim ObjDownloadWsCapitolato As WS_CapitolatoCliente.AgroWS_CapitolatoCliente
        Dim Risultati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlElementi As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement
        Dim XmlRoot As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))

        'Pulisco la combo
        Controllo.Items.Clear()

        'Verifico la possibilit? di gestione dei disciplinari
        Try

            ObjDownloadWsCapitolato = New WS_CapitolatoCliente.AgroWS_CapitolatoCliente

            'aggiungo quelli Pubblici
            Select Case Flag_AnchePrivati
                Case 1

                    Risultati = ObjDownloadWsCapitolato.get_elenco_dpi_pubblici(objParametri_Server.PivaSuperUser)
                    If Risultati <> "" Then
                        Risultati = Risultati.Replace(" xmlns=""http://ws_CapitolatoCliente_VerificaAnalisi""", "")
                        XmlDocumento.LoadXml(Risultati)
                        XmlRoot = XmlDocumento.SelectSingleNode("root")
                        If Not XmlRoot Is Nothing Then
                            XmlElementi = XmlRoot.GetElementsByTagName("dpi")
                            If Not XmlElementi Is Nothing Then
                                For Each XmlElemento In XmlElementi
                                    Dr = Dt.NewRow
                                    Dr.Item("nomeEsteso") = XmlElemento.SelectSingleNode("nomeEsteso").InnerText
                                    Dr.Item("codRegolamento") = XmlElemento.SelectSingleNode("codRegolamento").InnerText
                                    Dr.Item("PubblicoPrivato") = 1
                                    Dt.Rows.Add(Dr)
                                Next
                            Else
                                'se non ritorna dpi pubblici leggo nel vecchio modo
                                ObjDownloadWsDpi = New WS_Disciplinari.AgroWS_Disciplinari
                                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                                Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                                objWs.NewWS(ObjDownloadWsDpi,
                                                objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari,
                                                objParametri_Utenti)

                                Risultati = ObjDownloadWsDpi.Leggi_Disciplinari2(CInt(0),
                                                                        CInt(0),
                                                                        CInt(0),
                                                                        CInt(0),
                                                                        CInt(0),
                                                                        objParametri_Server.FinestraTemporaleInizio,
                                                                        objParametri_Server.FinestraTemporaleFine,
                                                                        CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                                        CStr(objSession("ASG_Utente_Password_Crypt").ToString),
                                                                        "")
                                If Risultati = "" Then
                                    If Not IsNothing(Risultati) AndAlso Risultati.ToLower.IndexOf("errore") < 0 Then
                                        XmlDocumento.LoadXml(Risultati)
                                        XmlElementi = XmlDocumento.GetElementsByTagName("Record")
                                        If Not XmlElementi Is Nothing Then
                                            For Each XmlElemento In XmlElementi
                                                Dr = Dt.NewRow
                                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                                Dr.Item("PubblicoPrivato") = 1
                                                Dt.Rows.Add(Dr)
                                            Next
                                        End If
                                    End If
                                End If


                            End If
                        End If

                    End If

                    ' Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)


                    If objWeb.Flag_DisciplinarePrivato = True Then

                        Risultati = ObjDownloadWsCapitolato.get_elenco_dpi_privati(objParametri_Server.PivaSuperUser)
                        If Risultati <> "" Then
                            Risultati = Risultati.Replace(" xmlns=""http://ws_CapitolatoCliente_VerificaAnalisi""", "")
                            XmlDocumento.LoadXml(Risultati)
                            XmlRoot = XmlDocumento.SelectSingleNode("root")
                            If Not XmlRoot Is Nothing Then
                                XmlElementi = XmlRoot.GetElementsByTagName("dpi")
                                If Not XmlElementi Is Nothing Then
                                    For Each XmlElemento In XmlElementi
                                        Dr = Dt.NewRow
                                        Dr.Item("nomeEsteso") = XmlElemento.SelectSingleNode("nomeEsteso").InnerText
                                        Dr.Item("codRegolamento") = XmlElemento.SelectSingleNode("codRegolamento").InnerText
                                        Dr.Item("PubblicoPrivato") = 2
                                        Dt.Rows.Add(Dr)
                                    Next
                                End If
                            End If
                        End If

                    End If

                Case Else
                    Risultati = ObjDownloadWsCapitolato.get_elenco_dpi_pubblici(objParametri_Server.PivaSuperUser)
                    If Risultati <> "" Then
                        Risultati = Risultati.Replace(" xmlns=""http://ws_CapitolatoCliente_VerificaAnalisi""", "")
                        XmlDocumento.LoadXml(Risultati)
                        XmlRoot = XmlDocumento.SelectSingleNode("root")
                        If Not XmlRoot Is Nothing Then
                            XmlElementi = XmlRoot.GetElementsByTagName("dpi")
                            If Not XmlElementi Is Nothing Then
                                For Each XmlElemento In XmlElementi
                                    Dr = Dt.NewRow
                                    Dr.Item("nomeEsteso") = XmlElemento.SelectSingleNode("nomeEsteso").InnerText
                                    Dr.Item("codRegolamento") = XmlElemento.SelectSingleNode("codRegolamento").InnerText
                                    Dr.Item("PubblicoPrivato") = 1
                                    Dt.Rows.Add(Dr)
                                Next
                            End If
                        End If
                    End If
            End Select

            If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

                Dim Dv As New DataView
                Dim i As Integer

                Dt.TableName = "Dpi"
                Dv.Table = Dt
                Dv.Sort = "nomeEsteso ASC"

                For i = 0 To Dv.Count - 1
                    If i = 0 OrElse Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
                        'Aggiungo l'elemento al primo giro oppure quando un elemento differisce dal precedente
                        Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1) & "|" & Dv.Item(i).Item(2)))
                    End If
                Next

            End If

            'Aggiungo la voce BIOLOGICO
            If Includi_Biologico Then
                Controllo.Items.Insert(0, New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio))
            End If

            'Aggiungo la voce nulla
            If Includi_Nessuno Then
                Controllo.Items.Insert(0, New ListItem("Nessun Disciplinare", "0"))
            End If

        Catch ex As Exception

            Risultati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) & _
                   ex.Message

        End Try


    End Sub










    '###############################################################################
    ' CaricaCombo_Epoche 
    '###############################################################################
    Public Sub Epoche_DPI(ByRef Controllo As ListControl, _
                            ByVal PrimaRiga_Flag As Boolean, _
                            ByVal PrimaRiga_Text As String, _
                            ByVal PrimaRiga_Value As String, _
                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                      ByVal Tipo_Testata As Integer, _
                                      ByVal Id_RcDpi As Integer, _
                                      ByVal Disciplinare_Cod As Integer, _
                                      ByVal Modulo As Integer, _
                                      Optional ByVal Includi_Tutte As Boolean = False)


        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

        Dim agroWs As String
        If IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
            'creo l'agrowebconfig
            Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
            agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
        Else
            agroWs = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
        End If

        ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
        objWs.NewWS(ObjDownloadWs, _
                        agroWs, _
                        objParametri_Utenti)

        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If Includi_Tutte Then
            Controllo.Items.Add(New ListItem("Tutte le epoche", "0"))
        End If

        Select Case Tipo_Testata

            Case 0 'difesa

                '##############################  Chiama WS - "Leggi_EpocheDifesa" ################################

                Try

                    Dati = ObjDownloadWs.Leggi_EpocheDifesa(CInt(Id_RcDpi), _
                                                            CInt(Disciplinare_Cod), _
                                                            CInt(Modulo), _
                                                            CStr(objSession("ASG_Utente_Username_Crypt").ToString), _
                                                            CStr(objSession("ASG_Utente_Password_Crypt").ToString))


                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                        Dim XmlDocumento As New System.Xml.XmlDocument
                        Dim XmlNodo As System.Xml.XmlNodeList
                        Dim XmlElemento As System.Xml.XmlElement
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo

                                Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("descrizioneperiododa")), _
                                                CInt(XmlElemento.GetAttribute("modulo"))))


                            Next
                        End If

                    End If


                Catch ex As Exception

                    'Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!"

                End Try

            Case 1 'diserbo

                '##############################  Chiama WS - "Leggi_EpocheDiserbo" ################################

                Try

                    Dati = ObjDownloadWs.Leggi_EpocheDiserbo(CInt(Id_RcDpi), _
                                                            CInt(Disciplinare_Cod), _
                                                            CStr(objSession("ASG_Utente_Username_Crypt").ToString), _
                                                            CStr(objSession("ASG_Utente_Password_Crypt").ToString))


                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                        Dim XmlDocumento As New System.Xml.XmlDocument
                        Dim XmlNodo As System.Xml.XmlNodeList
                        Dim XmlElemento As System.Xml.XmlElement
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo

                                Controllo.Items.Add(New ListItem(CStr(XmlElemento.GetAttribute("descrizione")), _
                                                CInt(XmlElemento.GetAttribute("ep_cod"))))


                            Next
                        End If

                    End If

                Catch ex As Exception

                    'Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!"

                End Try

        End Select

    End Sub






    '###############################################################################
    'SOLO in caso di TRATTAMENTO!!! ---> TipoTestata = 0
    'A differenza della precedente la funzione del WebService filtra i formulati 
    'in base alla presenza dell'etichetta nella banca dati col seguente criterio:
    'SINGOLA/SINGOLE AVVERSITA' -->Verifico che il prodotto sia registrato su :
    'ciascuna singola avversit? + (eventualmente) gruppo avversit? a cui la singola appartiene
    'GRUPPO/GRUPPI AVVERSITA' -->Verifico che il prodotto sia registrato su :
    'ciascun gruppo + singole avversit? che appartengono ai gruppi


    '''
    ''' default
    ''' ByVal flag_classeTossicologica As Boolean = False, _
    ''' ByVal flag_principiAttivi As Boolean = False, _
    ''' ByVal StrAvversita As String = "", _
    ''' ByVal Opt_Singola_Gruppo As Integer = 0 , _
    Public Sub Formulati_DPI( _
                                    ByRef Controllo As ListControl, _
                                    ByVal PrimaRiga_Flag As Boolean, _
                                    ByVal PrimaRiga_Text As String, _
                                    ByVal PrimaRiga_Value As String, _
                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal TestoRicerca As String, _
                                ByVal TipoRichiesto As Integer, _
                                ByVal Id_RcDpi As Integer, _
                                ByVal Disciplinare_cod As Integer, _
                                ByVal TipoTestata As Integer, _
                                ByVal Id_GaDPI As Integer, _
                                ByVal Av_Gru() As Integer, _
                                ByVal Av_Cod() As Integer, _
                                ByVal Veg_cod As Integer, _
                                ByVal Ep_cod As Integer, _
                                ByVal Modulo As Integer, _
                                ByVal Data_Intervento As Date, _
                                ByVal flag_classeTossicologica As Boolean, _
                                ByVal flag_principiAttivi As Boolean, _
                                ByVal StrAvversita As String, _
                                ByVal Opt_Singola_Gruppo As Integer)


        'NOTA 
        'Il TipoRichiesto consente di selezionare solo i formulati specifici
        'per la particolare applicazione
        '
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, Bagnanti, Antischiuma
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti

        'UTILIZZO

        Dim i As Integer

        Dim Fr_Des As String
        Dim Fr_Cod As String

        Dim descrizione_ClasseTossicologica As String
        Dim descrizione_PA As String

        Dim strValue As String

        Dim strErr As String = ""
        Dim N_Formulati As Integer = 0
        Dim strFormulati As String = ""

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlRisultati As System.Xml.XmlElement
        Dim XmlDatiFormulati As System.Xml.XmlElement
        Dim XML_Formulato As System.Xml.XmlElement
        Dim XMLs_Formulati As System.Xml.XmlNodeList

        Dim Stringa_Pa_Validi As String = ""
        Dim Stringa_Pa_Validi_Totali As String = ""

        Dim Array_PA_Validi(0) As String
        Dim Array_PA_Validi_Temp(0) As String
        Dim Array_PA_Validi_Temp1(0) As String
        Dim Array_PA_Validi_Temp2(0) As String


        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        Dim ObjDownloadWs As New WS_Disciplinari.AgroWS_Disciplinari
        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
        objWs.NewWS(ObjDownloadWs, _
                        Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString(), _
                        objParametri_Utenti)





        Try

            'Select Case TipoTestata

            '    Case 0 'SOLO IN CASO DI TRATTAMENTO!!!

            '########################################################################################################################
            ' Leggo i Formulati
            '########################################################################################################################

            strFormulati = ObjDownloadWs.Leggi_Formulati_DPI(objSession("ASG_Utente_Username_Crypt").ToString, _
                                                            objSession("ASG_Utente_Password_Crypt").ToString, _
                                                            objSession("ASG_ProgressivoGIAS").ToString, _
                                                            CInt(Disciplinare_cod), _
                                                            CInt(Veg_cod), _
                                                            CInt(Id_RcDpi), _
                                                            CInt(TipoTestata), _
                                                            CInt(Opt_Singola_Gruppo), _
                                                            Av_Gru, _
                                                            Av_Cod, _
                                                            StrAvversita, _
                                                            CInt(Ep_cod), _
                                                            CInt(Modulo), _
                                                            TestoRicerca, _
                                                            TipoRichiesto, _
                                                            CDate(Data_Intervento))

            ObjDownloadWs.Dispose()

            '===========================================================================================
            '   <RISULTATI errore="..." numero_formulati="..." >
            '       <DATIFORMULATI>
            '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..." tempocarenza="..." 
            '                  pa_cod="..." pa_des="..." titolo="..." class_cod="..." class_des="..."
            '                  dose_min="..." dose_max="..." udm_cod="..." udm_sim="..."
            '   </RISULTATI>
            '===========================================================================================

            If Not strFormulati Is Nothing AndAlso strFormulati <> "" Then

                'Carico la stringa nel documento XML
                XmlDoc.LoadXml(strFormulati)

                '----- Tag RISULTATI

                XmlRisultati = XmlDoc.SelectSingleNode("RISULTATI")

                strErr = CStr(XmlRisultati.GetAttribute("errore"))
                N_Formulati = CInt(XmlRisultati.GetAttribute("numero_formulati"))

                If strErr = "" Then

                    If XmlRisultati.HasChildNodes Then

                        XmlDatiFormulati = XmlRisultati.SelectSingleNode("DATIFORMULATI")

                        If XmlDatiFormulati.HasChildNodes Then

                            XMLs_Formulati = XmlDatiFormulati.GetElementsByTagName("FORMULATO")

                            If Not XMLs_Formulati Is Nothing Then

                                For i = 0 To XMLs_Formulati.Count - 1

                                    XML_Formulato = XMLs_Formulati.Item(i)

                                    Fr_Des = XML_Formulato.GetAttribute("fr_des")
                                    Fr_Cod = XML_Formulato.GetAttribute("fr_cod")

                                    '---------------------------------------------------------------------
                                    'se c'? aggiungo la classe tossicologica
                                    If flag_classeTossicologica Then
                                        descrizione_ClasseTossicologica = ""
                                        'If Not IsDBNull(Dt.Rows(i).Item("NewCLTOSS_Cod")) And CStr(Dt.Rows(i).Item("NewCLTOSS_Cod")) <> "" Then
                                        '    descrizione_ClasseTossicologica = " --- (" + CStr(Dt.Rows(i).Item("NewCLTOSS_Cod")) + ")"
                                        'End If
                                    End If

                                    If flag_principiAttivi Then
                                        descrizione_PA = XML_Formulato.GetAttribute("pa_des")
                                        If descrizione_PA <> "" Then
                                            descrizione_PA = " --- <" + descrizione_PA + ">"
                                        End If
                                    End If

                                    strValue = Fr_Cod & "£" & _
                                            XML_Formulato.GetAttribute("tempocarenza") & "£" & _
                                            XML_Formulato.GetAttribute("dose_min") & "£" & _
                                            XML_Formulato.GetAttribute("dose_max") & "£" & _
                                            XML_Formulato.GetAttribute("udm_cod") & "£" & _
                                            XML_Formulato.GetAttribute("udm_sim")

                                    Controllo.Items.Add(New ListItem((Fr_Des + "  (" + CStr(Fr_Cod) + ")" _
                                                                + descrizione_ClasseTossicologica _
                                                                + descrizione_PA), _
                                                                strValue))


                                Next

                            End If

                        End If

                    End If

                End If

            End If

            'Case 1 'DISERBO


            '    '########################################################################################################################
            '    ' Recupero i Principi Attivi validi x OGNI Avversit?/Gruppi scelti
            '    '########################################################################################################################

            '    If StrAvversita = "" Then

            '        For i = 0 To UBound(Av_Cod)

            '            Stringa_Pa_Validi = ObjDownloadWs.Stringa_PA_Validi(CInt(Id_RcDpi), _
            '                                                CInt(Disciplinare_cod), _
            '                                                CInt(TipoTestata), _
            '                                                CInt(Av_Gru(i)), _
            '                                                CInt(Av_Cod(i)), _
            '                                                CInt(Ep_cod), _
            '                                                CInt(Modulo), _
            '                                                CStr(StrAvversita))



            '            Stringa_Pa_Validi = Replace(Stringa_Pa_Validi, " ", "")

            '            Stringa_Pa_Validi = Replace(Stringa_Pa_Validi, "(", "")
            '            Stringa_Pa_Validi = Replace(Stringa_Pa_Validi, ")", "")


            '            Array_PA_Validi_Temp1 = Nothing

            '            Array_PA_Validi_Temp1 = Split(Stringa_Pa_Validi, ",")

            '            If i = 0 Then
            '                If Av_Cod.Length = 1 Then
            '                    Array_PA_Validi = Array_PA_Validi_Temp1
            '                Else
            '                    Array_PA_Validi_Temp2 = Array_PA_Validi_Temp1
            '                End If
            '            Else
            '                ReDim Array_PA_Validi(0)
            '                Interseca_Vettori(Array_PA_Validi_Temp1, Array_PA_Validi_Temp2, Array_PA_Validi)
            '                Array_PA_Validi_Temp2 = Array_PA_Validi
            '            End If

            '        Next

            '        For i = 0 To UBound(Array_PA_Validi)
            '            If Not IsNothing(Array_PA_Validi(i)) Then
            '                Stringa_Pa_Validi_Totali = Stringa_Pa_Validi_Totali & Array_PA_Validi(i) & ","
            '            End If
            '        Next

            '        If Stringa_Pa_Validi_Totali <> "" Then
            '            Stringa_Pa_Validi_Totali = Left(Stringa_Pa_Validi_Totali, Stringa_Pa_Validi_Totali.Length - 1)
            '            Stringa_Pa_Validi_Totali = "(" & Stringa_Pa_Validi_Totali & ")"
            '        End If

            '    Else

            '        Stringa_Pa_Validi = ObjDownloadWs.Stringa_PA_Validi(CInt(Id_RcDpi), _
            '                            CInt(Disciplinare_cod), _
            '                            CInt(TipoTestata), _
            '                            CInt(0), _
            '                            CInt(0), _
            '                            CInt(Ep_cod), _
            '                            CInt(Modulo), _
            '                            CStr(StrAvversita))

            '        Stringa_Pa_Validi_Totali = Stringa_Pa_Validi

            '    End If


            '    If Stringa_Pa_Validi_Totali <> "" Then

            '        '########################################################################################################################
            '        ' Leggo i Formulati
            '        '########################################################################################################################

            '        Dim Dt As New DataTable
            '        Dim strFiltro As String
            '        Dim LastFr_Des As String = ""


            '        'Filtro i Non Coadiuvanti, Bagnanti, Antischiuma, Etc...
            '        strFiltro = " (FormulatixClassificazioni.Class_Cod < 500 OR FormulatixClassificazioni.Class_Cod > 506) "
            '        Dim objMetaschema As New AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R
            '        Dt = objMetaschema.LeggiXClassificazione_PA(CStr(TestoRicerca), _
            '                                             CInt(TipoRichiesto), _
            '                                             CInt(Veg_cod), _
            '                                             CStr(Stringa_Pa_Validi_Totali), _
            '                                             AGRODATAINIZIO, CDate(Data_Intervento), _
            '                                             enumSelezioneVariabile.Selezione_TabellaCompleta, _
            '                                             strFiltro, "", _
            '                                             objParametri_Server _
            '                                             )

            '        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            '            For i = 0 To Dt.Rows.Count - 1

            '                'controllo che nn ci siano doppioni relativi alla classificazione...
            '                If CStr(Dt.Rows(i).Item("fr_des")) <> LastFr_Des Then

            '                    'se c'? aggiungo la classe tossicologica
            '                    If flag_classeTossicologica Then
            '                        descrizione_ClasseTossicologica = ""
            '                        If Not IsDBNull(Dt.Rows(i).Item("NewCLTOSS_Cod")) And CStr(Dt.Rows(i).Item("NewCLTOSS_Cod")) <> "" Then
            '                            descrizione_ClasseTossicologica = " --- (" + CStr(Dt.Rows(i).Item("NewCLTOSS_Cod")) + ")"
            '                        End If
            '                    End If

            '                    If flag_principiAttivi Then
            '                        descrizione_PA = ""
            '                        If Not IsDBNull(Dt.Rows(i).Item("pa_des")) And CStr(Dt.Rows(i).Item("pa_des")) <> "" Then
            '                            descrizione_PA = " --- <" + (CStr(Dt.Rows(i).Item("pa_des"))) + ">"
            '                        End If
            '                    End If

            '                    Controllo.Items.Add(New ListItem((CStr(Dt.Rows(i).Item("fr_des")) + "  (" + CStr(Dt.Rows(i).Item("fr_cod")) + ")" + descrizione_ClasseTossicologica + descrizione_PA), _
            '                                                                                    CInt(Dt.Rows(i).Item("fr_cod"))))

            '                    LastFr_Des = CStr(Dt.Rows(i).Item("fr_des"))

            '                End If

            '            Next

            '        End If


            '    End If


            'End Select


        Catch ex As Exception

            'AgroMsgBox("Si sono verificati errori: " & ex.Message, objPage)
            'Exit Sub

        End Try


    End Sub



    ''###############################################################################
    '' CaricaCombo_Prodotti in Magazzino filtrando sul Disciplinare
    ''###############################################################################
    'Public Sub Formulati_Magazzino_DPI( _
    '                                ByRef Controllo As ListControl, _
    '                                ByVal PrimaRiga_Flag As Boolean, _
    '                                ByVal PrimaRiga_Text As String, _
    '                                ByVal PrimaRiga_Value As String, _
    '                                ByRef objSession As System.Web.SessionState.HttpSessionState, _
    '                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                    ByVal Piva As String, _
    '                                    ByVal Sa_Cod As Integer, _
    '                                    ByVal Destinazione As Integer, _
    '                                    ByVal Elem_Cod As Integer, _
    '                                    Optional ByVal Id_rcdpi As Integer = 0, _
    '                                    Optional ByVal Disciplinare_Cod As Integer = 0, _
    '                                    Optional ByVal TipoTestata As Integer = 0, _
    '                                    Optional ByVal Av_Gru() As Integer = Nothing, _
    '                                    Optional ByVal Av_Cod() As Integer = Nothing, _
    '                                    Optional ByVal Ep_Cod As Integer = 0, _
    '                                    Optional ByVal Modulo As Integer = 0, _
    '                                    Optional ByVal strAvversita As String = "", _
    '                                    Optional ByVal TipoRichiesto As Integer = 0, _
    '                                    Optional ByVal TestoRicerca As String = "", _
    '                                    Optional ByVal Flag_ClasseTossicologica As Boolean = False, _
    '                                    Optional ByVal Flag_PrincipiAttivi As Boolean = False, _
    '                                    Optional ByVal Veg_Cod As Integer = 0, _
    '                                    Optional ByVal Opt_Singola_Gruppo As Integer = 0, _
    '                                    Optional ByVal Data As Date = #12/31/2100#)

    '    Dim Stringa_FrCod_Validi As String
    '    Dim Stringa_Pa_Validi As String = ""
    '    Dim Stringa_Pa_Validi_Temp As String = ""
    '    Dim Stringa_Pa_Validi_Totali As String

    '    Dim Array_PA_Validi(0) As String
    '    Dim Array_PA_Validi_Temp(0) As String
    '    Dim Array_PA_Validi_Temp1(0) As String
    '    Dim Array_PA_Validi_Temp2(0) As String

    '    Dim Classe_Tossicologica As String
    '    Dim Principi_Attivi As String

    '    Dim filtro_formulati As String = " "
    '    Dim Fr_Cod As Integer

    '    Dim strFiltro As String = ""

    '    Dim Dt As New DataTable
    '    Dim Dr As DataRow
    '    Dim Dt_Formulati As New DataTable
    '    Dim Dr_Formulati() As DataRow
    '    Dim Dr_Formulato As DataRow
    '    Dim Dt_Giacenze As New DataTable

    '    'Dim objSQL As New Codex_Utility.Sql

    '    Dim strErr As String

    '    Dim Last_ProCod As Integer = 0
    '    Dim strFormulati As String = ""
    '    Dim strValue As String = ""

    '    Dim i As Integer = 0
    '    Dim N_Formulati As Integer = 0

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XmlRisultati As System.Xml.XmlElement
    '    Dim XmlDatiFormulati As System.Xml.XmlElement
    '    Dim XML_Formulato As System.Xml.XmlElement
    '    Dim XMLs_Formulati As System.Xml.XmlNodeList

    '    '----- Definisco la struttura del DataTable

    '    Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Valore", GetType(String)))


    '    'Pulisco la combo
    '    Controllo.Items.Clear()

    '    If PrimaRiga_Flag = True Then
    '        Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
    '    End If

    '    Try

    '        If Disciplinare_Cod <> 0 Then

    '            Dim ObjDownloadWs As New WS_Disciplinari.AgroWS_Disciplinari
    '            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
    '            objWs.NewWS(ObjDownloadWs, _
    '                            System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString(), _
    '                            objParametri_Utenti)


    '            Select Case TipoTestata

    '                Case 0 'SOLO IN CASO DI TRATTAMENTO!!!

    '                    '########################################################################################################################
    '                    ' Leggo i Formulati
    '                    '########################################################################################################################

    '                    strFormulati = ObjDownloadWs.Leggi_Formulati_DPI(objSession("ASG_Utente_Username_Crypt").ToString, _
    '                                                                    objSession("ASG_Utente_Password_Crypt").ToString, _
    '                                                                    objSession("ASG_ProgressivoGIAS").ToString, _
    '                                                                    CInt(Disciplinare_Cod), _
    '                                                                    CInt(Veg_Cod), _
    '                                                                    CInt(Id_rcdpi), _
    '                                                                    CInt(TipoTestata), _
    '                                                                    CInt(Opt_Singola_Gruppo), _
    '                                                                    Av_Gru, _
    '                                                                    Av_Cod, _
    '                                                                    strAvversita, _
    '                                                                    CInt(Ep_Cod), _
    '                                                                    CInt(Modulo), _
    '                                                                    TestoRicerca, _
    '                                                                    TipoRichiesto, _
    '                                                                    CDate(Data))

    '                    ObjDownloadWs.Dispose()

    '                    '===========================================================================================
    '                    '   <RISULTATI errore="..." numero_formulati="..." >
    '                    '       <DATIFORMULATI>
    '                    '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..." tempocarenza="..." 
    '                    '                  pa_cod="..." pa_des="..." titolo="..." class_cod="..." class_des="..."
    '                    '                  dose_min="..." dose_max="..." udm_cod="..." udm_sim="..."
    '                    '   </RISULTATI>
    '                    '===========================================================================================

    '                    If Not strFormulati Is Nothing AndAlso strFormulati <> "" Then

    '                        '---------------------------
    '                        'Creo il Dt
    '                        Dt_Formulati.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
    '                        Dt_Formulati.Columns.Add(New DataColumn("Fr_cod", GetType(String)))
    '                        Dt_Formulati.Columns.Add(New DataColumn("Pa_cod", GetType(String)))
    '                        Dt_Formulati.Columns.Add(New DataColumn("Pa_Des", GetType(String)))
    '                        Dt_Formulati.Columns.Add(New DataColumn("TempoCarenza", GetType(String)))
    '                        Dt_Formulati.Columns.Add(New DataColumn("Dose_Min", GetType(String)))
    '                        Dt_Formulati.Columns.Add(New DataColumn("Dose_Max", GetType(String)))
    '                        Dt_Formulati.Columns.Add(New DataColumn("Udm_Cod", GetType(String)))
    '                        Dt_Formulati.Columns.Add(New DataColumn("Udm_Sim", GetType(String)))

    '                        '---------------------------
    '                        'Carico la stringa nel documento XML
    '                        XmlDoc.LoadXml(strFormulati)

    '                        '----- Tag RISULTATI

    '                        XmlRisultati = XmlDoc.SelectSingleNode("RISULTATI")

    '                        strErr = CStr(XmlRisultati.GetAttribute("errore"))
    '                        N_Formulati = CInt(XmlRisultati.GetAttribute("numero_formulati"))

    '                        If strErr = "" Then

    '                            If XmlRisultati.HasChildNodes Then

    '                                XmlDatiFormulati = XmlRisultati.SelectSingleNode("DATIFORMULATI")

    '                                If XmlDatiFormulati.HasChildNodes Then

    '                                    XMLs_Formulati = XmlDatiFormulati.GetElementsByTagName("FORMULATO")

    '                                    If Not XMLs_Formulati Is Nothing Then

    '                                        For i = 0 To XMLs_Formulati.Count - 1

    '                                            XML_Formulato = XMLs_Formulati.Item(i)

    '                                            'dt_formulati.
    '                                            Fr_Cod = XML_Formulato.GetAttribute("fr_cod")

    '                                            If filtro_formulati = " " Then
    '                                                filtro_formulati += " Pro_Cod = " + CStr(Fr_Cod)
    '                                            Else
    '                                                filtro_formulati += " or Pro_Cod = " + CStr(Fr_Cod)
    '                                            End If

    '                                            'Creo una nuova riga
    '                                            Dr_Formulato = Dt_Formulati.NewRow

    '                                            'Definisco i valori
    '                                            Dr_Formulato.Item("Fr_cod") = Fr_Cod
    '                                            Dr_Formulato.Item("Fr_Des") = XML_Formulato.GetAttribute("fr_des")
    '                                            Dr_Formulato.Item("Pa_Cod") = XML_Formulato.GetAttribute("pa_cod")
    '                                            Dr_Formulato.Item("Pa_Des") = XML_Formulato.GetAttribute("pa_des")
    '                                            Dr_Formulato.Item("TempoCarenza") = XML_Formulato.GetAttribute("tempocarenza")
    '                                            Dr_Formulato.Item("dose_min") = XML_Formulato.GetAttribute("dose_min")
    '                                            Dr_Formulato.Item("dose_max") = XML_Formulato.GetAttribute("dose_max")
    '                                            Dr_Formulato.Item("udm_cod") = XML_Formulato.GetAttribute("udm_cod")
    '                                            Dr_Formulato.Item("udm_sim") = XML_Formulato.GetAttribute("udm_sim")

    '                                            'Associo alla tabella la nuova riga creata
    '                                            Dt_Formulati.Rows.Add(Dr_Formulato)

    '                                        Next

    '                                    End If

    '                                End If

    '                            End If

    '                        End If


    '                    End If


    '                    '----------------------------------------------------------
    '                    '----------------------------------------------------------
    '                    '----------------------------------------------------------

    '                Case 1 'DISERBO

    '                    If Not IsNothing(Av_Cod) Then

    '                        If strAvversita = "" Then

    '                            For i = 0 To UBound(Av_Cod)

    '                                Stringa_Pa_Validi = ObjDownloadWs.Stringa_PA_Validi(CInt(Id_rcdpi), _
    '                                                                                    CInt(Disciplinare_Cod), _
    '                                                                                    CInt(TipoTestata), _
    '                                                                                    CInt(Av_Gru(i)), _
    '                                                                                    CInt(Av_Cod(i)), _
    '                                                                                    CInt(Ep_Cod), _
    '                                                                                    CInt(Modulo), _
    '                                                                                    CStr(strAvversita))

    '                                Stringa_Pa_Validi = Replace(Stringa_Pa_Validi, " ", "")
    '                                Stringa_Pa_Validi = Replace(Stringa_Pa_Validi, "(", "")
    '                                Stringa_Pa_Validi = Replace(Stringa_Pa_Validi, ")", "")


    '                                Array_PA_Validi_Temp1 = Nothing
    '                                Array_PA_Validi_Temp1 = Split(Stringa_Pa_Validi, ",")

    '                                If i = 0 Then
    '                                    If Av_Cod.Length = 1 Then
    '                                        Array_PA_Validi = Array_PA_Validi_Temp1
    '                                    Else
    '                                        Array_PA_Validi_Temp2 = Array_PA_Validi_Temp1
    '                                    End If
    '                                Else
    '                                    ReDim Array_PA_Validi(0)
    '                                    Dim objDPIVarie As New AgronicaCoreDpiBIZ.CaricaListControl
    '                                    objDPIVarie.Interseca_Vettori(Array_PA_Validi_Temp1, Array_PA_Validi_Temp2, Array_PA_Validi)
    '                                    Array_PA_Validi_Temp2 = Array_PA_Validi
    '                                End If


    '                            Next

    '                            For i = 0 To UBound(Array_PA_Validi)
    '                                If Not IsNothing(Array_PA_Validi(i)) Then
    '                                    Stringa_Pa_Validi_Totali = Stringa_Pa_Validi_Totali & Array_PA_Validi(i) & ","
    '                                End If
    '                            Next

    '                            If Stringa_Pa_Validi_Totali <> "" Then
    '                                Stringa_Pa_Validi_Totali = Left(Stringa_Pa_Validi_Totali, Stringa_Pa_Validi_Totali.Length - 1)
    '                                Stringa_Pa_Validi_Totali = "(" & Stringa_Pa_Validi_Totali & ")"
    '                            End If

    '                        Else

    '                            Stringa_Pa_Validi = ObjDownloadWs.Stringa_PA_Validi(CInt(Id_rcdpi), _
    '                                                                                CInt(Disciplinare_Cod), _
    '                                                                                CInt(TipoTestata), _
    '                                                                                CInt(0), _
    '                                                                                CInt(0), _
    '                                                                                CInt(Ep_Cod), _
    '                                                                                CInt(Modulo), _
    '                                                                                CStr(strAvversita))

    '                            Stringa_Pa_Validi_Totali = Stringa_Pa_Validi

    '                        End If

    '                        If Stringa_Pa_Validi_Totali <> "" Then

    '                            '########################################################################################################################
    '                            ' Leggo i Formulati
    '                            '########################################################################################################################

    '                            'Filtro i Non Coadiuvanti, Bagnanti, Antischiuma, Etc...
    '                            strFiltro = " (FormulatixClassificazioni.Class_Cod < 500 OR FormulatixClassificazioni.Class_Cod > 506) "

    '                            'Dt_Formulati = NewCom_LeggiXClassificazione_PA(objSession, _
    '                            '                                    CStr(TestoRicerca), _
    '                            '                                    CInt(TipoRichiesto), _
    '                            '                                    CInt(Veg_Cod), _
    '                            '                                    CStr(Stringa_Pa_Validi_Totali), _
    '                            '                                    CDate(Data), _
    '                            '                                    strFiltro, _
    '                            '                                    )


    '                            Dim objMetaschema As New AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R
    '                            Dt_Formulati = objMetaschema.LeggiXClassificazione_PA(CStr(TestoRicerca), _
    '                                                                           CInt(TipoRichiesto), _
    '                                                                           CInt(Veg_Cod), _
    '                                                                           CStr(Stringa_Pa_Validi_Totali), _
    '                                                                           AGRODATAINIZIO, _
    '                                                                           CDate(Data), _
    '                                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                                           strFiltro, _
    '                                                                           "", _
    '                                                                           objParametri_Server _
    '                                                                           )



    '                            If Not Dt_Formulati Is Nothing AndAlso Dt_Formulati.Rows.Count > 0 Then

    '                                For i = 0 To Dt_Formulati.Rows.Count - 1

    '                                    Fr_Cod = Dt_Formulati.Rows(i).Item("Fr_Cod")

    '                                    If filtro_formulati = " " Then
    '                                        filtro_formulati += " Pro_Cod = " + CStr(Fr_Cod)
    '                                    Else
    '                                        filtro_formulati += " or Pro_Cod = " + CStr(Fr_Cod)
    '                                    End If

    '                                Next

    '                            End If

    '                        End If

    '                    End If

    '            End Select

    '            ObjDownloadWs.Dispose()

    '            'Filtro nelle giacenze i Formulati validi x il DPI
    '            If filtro_formulati <> " " Then

    '                filtro_formulati = " AND ( " + filtro_formulati & " ) "

    '                ''legge le giacenze 
    '                'Dt_Giacenze = NewCom_Leggi_Giacenze(objServer, objSession, objPage, _
    '                '                                    Piva, _
    '                '                                    Sa_Cod, _
    '                '                                    , , , _
    '                '                                    Destinazione, _
    '                '                                    MAGAZZINO, _
    '                '                                    , , _
    '                '                                    Elem_Cod, _
    '                '                                    , , , , , , , , , , _
    '                '                                    filtro_formulati, _
    '                '                                    " ORDER BY Pro_Cod ")

    '                'legge le giacenze 
    '                'passo il filtro eventuale sulla classificazione
    '                Dim objG As New AgronicaCoreContabDAL.Giacenze_R

    '                Dt_Giacenze = objG.SchedaGiacenzeMagazzino(Data, _
    '                                     Piva, _
    '                                     Sa_Cod, _
    '                                     Destinazione, _
    '                                     FORMULATI, _
    '                                     0, _
    '                                     0, 0, 0, 0, 0, _
    '                                     LOTTO_NONDEFINITO, _
    '                                     True, _
    '                                     filtro_formulati, _
    '                                     "", "", "", "", "", "", "", "", "", "", _
    '                                     " Pro_Cod ", _
    '                                     objParametri_Server)
    '                objG = Nothing


    '                If Not Dt_Giacenze Is Nothing Then

    '                    For i = 0 To Dt_Giacenze.Rows.Count - 1

    '                        If Last_ProCod <> CInt(Dt_Giacenze.Rows(i).Item("pro_cod")) Then

    '                            Dr = Dt.NewRow

    '                            Select Case TipoTestata

    '                                Case 0 'SOLO IN CASO DI TRATTAMENTO!!!

    '                                    Principi_Attivi = ""

    '                                    Dr_Formulati = Dt_Formulati.Select("fr_cod='" & Dt_Giacenze.Rows(i).Item("pro_cod").ToString & "'")

    '                                    If Not Dr_Formulati Is Nothing AndAlso Dr_Formulati.Length > 0 Then

    '                                        strValue = Dr_Formulati(0).Item("fr_cod") & "£" & _
    '                                                Dr_Formulati(0).Item("tempocarenza") & "£" & _
    '                                                Dr_Formulati(0).Item("dose_min") & "£" & _
    '                                                Dr_Formulati(0).Item("dose_max") & "£" & _
    '                                                Dr_Formulati(0).Item("udm_cod") & "£" & _
    '                                                Dr_Formulati(0).Item("udm_sim")

    '                                        If Flag_PrincipiAttivi Then

    '                                            Principi_Attivi = Dr_Formulati(0).Item("pa_des")

    '                                            If Principi_Attivi <> "" Then
    '                                                Principi_Attivi = " --- <" + Principi_Attivi + ">"
    '                                            End If

    '                                        End If

    '                                    End If

    '                                    Dr.Item("Descrizione") = Dt_Giacenze.Rows(i).Item("Descrizione_Prodotto") + "  (" + CStr(Dt_Giacenze.Rows(i).Item("pro_cod")) + ")" + Principi_Attivi

    '                                    Dr.Item("Valore") = strValue

    '                                Case Else

    '                                    If Flag_PrincipiAttivi Then

    '                                        Principi_Attivi = ""
    '                                        Dim objMetDal As New AgronicaCoreMetaSchemaDAL.FormulatixPrincipiAttivi_R
    '                                        objMetDal.Leggi(CInt(Dt_Giacenze.Rows(i).Item("pro_cod")), _
    '                                                        0, _
    '                                                        AGRODATAINIZIO, _
    '                                                        AGRODATAFINE, _
    '                                                         enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                         "", _
    '                                                         "", _
    '                                                         objParametri_Server)

    '                                        If Principi_Attivi <> "" Then
    '                                            Principi_Attivi = " --- <" + Principi_Attivi + ">"
    '                                        End If

    '                                    End If

    '                                    Dr.Item("Descrizione") = Dt_Giacenze.Rows(i).Item("Descrizione_Prodotto") + "  (" + CStr(Dt_Giacenze.Rows(i).Item("pro_cod")) + ")" + Principi_Attivi

    '                                    Dr.Item("Valore") = CInt(Dt_Giacenze.Rows(i).Item("pro_cod"))

    '                            End Select

    '                            Dt.Rows.Add(Dr)

    '                            Last_ProCod = CInt(Dt_Giacenze.Rows(i).Item("pro_cod"))

    '                        End If

    '                    Next

    '                End If

    '            End If

    '        End If


    '        '------------------------------------------------------------
    '        '--------  ORDINAMENTO
    '        '------------------------------------------------------------

    '        Select Case Dt.Rows.Count

    '            Case 1
    '                Controllo.Items.Add(New ListItem(Dt.Rows(0).Item("Descrizione"), Dt.Rows(0).Item("Valore")))

    '            Case Is > 1
    '                'uso il dataview per ordinare 
    '                Dim Dv As New DataView

    '                Dt.TableName = "Prodotti"
    '                Dv.Table = Dt
    '                Dv.Sort = "Descrizione ASC"

    '                For i = 0 To Dv.Count - 1
    '                    If i >= 1 AndAlso Dv.Item(i).Item(0) <> Dv.Item(i - 1).Item(0) Then
    '                        Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
    '                    Else
    '                        Controllo.Items.Add(New ListItem(Dv.Item(i).Item(0), Dv.Item(i).Item(1)))
    '                    End If
    '                Next


    '        End Select


    '    Catch ex As Exception

    '        'AgroMsgBox("Si sono verificati errori :" & ex.Message, objPage)

    '        'Exit Sub

    '    End Try


    'End Sub




    '###############################################################################
    ' CaricaCombo_Interventi 
    '###############################################################################
    Public Sub Interventi_DPI(ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        Cmb.Items.Clear()
        Cmb.Items.Add(New ListItem("Difesa", "0"))
        Cmb.Items.Add(New ListItem("Diserbo", "1"))
        Cmb.Items.Add(New ListItem("Piano Concimazione", "2"))
        Cmb.Items.Add(New ListItem("Irrigazione", "3"))

    End Sub

    '###############################################################################
    Public Function Interseca_Vettori(ByVal Array1() As String, _
                                      ByVal Array2() As String, _
                                      ByRef Array3() As String) As Array

        Dim i, j, n, p As Integer
        Dim Array1_New(1, 0) As String
        Dim Array2_New(1, 0) As String
        Dim Array3_New(0) As String
        Dim Trovato As Boolean

        For i = 0 To UBound(Array1)
            ReDim Preserve Array1_New(1, i)
            Array1_New(0, i) = Array1(i)
            Array1_New(1, i) = 0
        Next

        For i = 0 To UBound(Array2)
            ReDim Preserve Array2_New(1, i)
            Array2_New(0, i) = Array2(i)
            Array2_New(1, i) = 0
        Next

        'se un elemento ? in entrambi i vettori metto 1 nella seconda riga..
        For i = 0 To UBound(Array1_New, 2)
            For j = 0 To UBound(Array2_New, 2)
                If Array1_New(0, i) = Array2_New(0, j) Then
                    Array1_New(1, i) = 1
                    Array2_New(1, j) = 1
                End If
            Next
        Next

        'metto tutti gli elementi di entrambi i vettori con 1 in un nuovo vettore..
        For i = 0 To UBound(Array1_New, 2)
            If Array1_New(1, i) = 1 Then
                ReDim Preserve Array3_New(n)
                Array3_New(n) = Array1_New(0, i)
                n += 1
            End If
        Next
        For i = 0 To UBound(Array2_New, 2)
            If Array2_New(1, i) = 1 Then
                ReDim Preserve Array3_New(n)
                Array3_New(n) = Array2_New(0, i)
                n += 1
            End If
        Next

        'metto gli elementi in un nuovo vettore controllando che nn ci siano doppioni..
        For i = 0 To UBound(Array3_New)
            Trovato = False
            For j = 0 To UBound(Array3)
                If Array3_New(i) = Array3(j) Then
                    Trovato = True
                    Exit For
                End If
            Next
            If Trovato = False Then
                ReDim Preserve Array3(p)
                Array3(p) = Array3_New(i)
                p += 1
            End If
        Next

    End Function


    '###############################################################################
    '''Usato
    '''Versione NewCom tipo core ma non standard
    '''Spostare nei core a standardizzare
    '####################################################################################
    '####################################################################################
    ' Nuova versione che carica i FORMULATI con filtro CLASSIFICAZIONE 
    ' validi tra una DATA INIZIO e una DATA FINE
    '               - pu? essere aggiunta alla combo la descrizione di:
    '               1) classeTossicologica
    '               2) principiAttivi
    Public Sub Formulati_Magazzino_ConDosi(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByRef objSession As System.Web.SessionState.HttpSessionState,
                                ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByVal Causale As enum_Agenda_Causali,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Destinazione As Integer,
                                ByVal FinestraTemp_Inizio As Date,
                                ByVal FinestraTemp_fine As Date,
                                Optional ByVal TestoRicerca As String = "",
                                Optional ByVal Veg_Cod As Integer = 0,
                                Optional ByVal strPA As String = "",
                                Optional ByVal Flag_PrincipiAttivi As Boolean = False,
                                Optional ByVal TipoRichiesto As Integer = 0,
                                Optional ByVal strFiltro As String = "",
                                Optional ByVal Opt_Avversita_Infestanti As Integer = 0,
                                Optional ByVal Opt_Singola_Gruppo As Integer = 0,
                                Optional ByVal Av_Gru() As Integer = Nothing,
                                Optional ByVal Av_Cod() As Integer = Nothing,
                                Optional ByVal grfi_cod As Integer = 0,
                                           Optional ByVal stato_cod As String = "IT"
                            )


        'NOTA
        'Il TipoRichiesto consente di selezionare solo i formulati specifici
        'per la particolare applicazione:
        '
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, Bagnanti, Antischiuma
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        '   ecc...

        Dim XmlDoc As System.Xml.XmlDocument
        Dim XmlDocRis As System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim XmlRisultati As System.Xml.XmlElement
        Dim XmlFormulati As System.Xml.XmlNodeList
        Dim XmlFormulato As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim Parametri As String
        Dim Risultati As String

        Dim Dt As New DataTable
        Dim Dt_Formulati As New DataTable
        Dim Dt_Giacenze As New DataTable


        Dim strErr As String


        Dim LastFr_Des As String = ""
        Dim i As Integer

        Dim strPA_Des As String
        Dim strFiltroClassificazione As String = ""
        Dim strFiltroFormulati As String = ""

        Dim strValue As String = ""

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If


        'Se ho scelto una CLASSIFICAZIONE filtro prima i formulati in base a quella
        If TipoRichiesto <> 0 Then


            Dim objMetaschema As New AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R
            Dt_Formulati = objMetaschema.LeggiXClassificazione_PA(CStr(TestoRicerca),
                                                           CInt(TipoRichiesto),
                                                           CInt(Veg_Cod),
                                                           CStr(strPA),
                                                           AGRODATAINIZIO,
                                                           CDate(FinestraTemp_fine),
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                           strFiltro,
                                                           "",
                                                           objParametri_Server
                                                           )



            If Not Dt_Formulati Is Nothing AndAlso Dt_Formulati.Rows.Count > 0 Then

                For i = 0 To Dt_Formulati.Rows.Count - 1
                    strFiltroClassificazione = strFiltroClassificazione & Dt_Formulati.Rows(i).Item("fr_cod") & ","
                Next

                Dt_Formulati = Nothing

            End If

            If strFiltroClassificazione <> "" Then
                strFiltroClassificazione = Left(strFiltroClassificazione, strFiltroClassificazione.Length - 1)
                strFiltroClassificazione = " AND pro_cod IN (" & strFiltroClassificazione & ")"
            End If

        End If

        Select Case Causale

            Case enum_Agenda_Causali.CARICO

            Case enum_Agenda_Causali.SCARICO

                'legge le giacenze 
                'passo il filtro eventuale sulla classificazione
                Dim objG As New AgronicaCoreContabDAL.Giacenze_R

                Dt_Giacenze = objG.SchedaGiacenzeMagazzino(FinestraTemp_fine,
                                     Piva,
                                     Sa_Cod,
                                     Destinazione,
                                     FORMULATI,
                                     0,
                                     0, 0, 0, 0, 0,
                                     LOTTO_NONDEFINITO,
                                     True,
                                     strFiltroClassificazione,
                                     "", "", "", "", "", "", "", "", "", "",
                                     "",
                                     objParametri_Server, objParametri_Utenti)
                objG = Nothing


                If Not Dt_Giacenze Is Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then

                    For i = 0 To Dt_Giacenze.Rows.Count - 1
                        strFiltroFormulati = strFiltroFormulati & Dt_Giacenze.Rows(i).Item("pro_cod") & ","
                    Next

                    Dt_Giacenze = Nothing

                    If strFiltroFormulati <> "" Then

                        strFiltroFormulati = Left(strFiltroFormulati, strFiltroFormulati.Length - 1)
                        strFiltroFormulati = " AND Formulati.FR_COD IN ( " & strFiltroFormulati & " )"


                        '########################################################################################################################
                        ' Leggo i Formulati
                        '########################################################################################################################

                        Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
                        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                        objWs.NewWS(ObjDownloadWs,
                                        System.Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString(),
                                        objParametri_Utenti)

                        Try

                            XmlDoc = New System.Xml.XmlDocument

                            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                    StrCredenziali,
                                                    objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                                    objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                                    objSession("ASG_ProgressivoGIAS"),
                                                    objSession("ASG_SuperUser_Username").ToString,
                                                    objSession("ASG_SuperUser_Password").ToString)

                            XmlDoc.LoadXml(StrCredenziali)

                            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                            objAgroWS.AgroWS_XML_Parametri_Formulati_ConDosi(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                                   StrParametri,
                                                                   Veg_Cod,
                                                                   Opt_Avversita_Infestanti,
                                                                   Opt_Singola_Gruppo,
                                                                    Av_Gru,
                                                                    Av_Cod,
                                                                    strPA,
                                                                    TipoRichiesto,
                                                                    TestoRicerca,
                                                                    FinestraTemp_fine,
                                                                    strFiltroFormulati,
                                                                    "",
                                                                    strErr,
                                                                    grfi_cod,
                                                                    0,
                                                                    stato_cod)

                            XML_Credenziali.InnerXml = StrParametri

                            Parametri = XmlDoc.OuterXml

                            Parametri = objAgroWS.AWS_Codifica_P(Parametri)

                            Risultati = ObjDownloadWs.Leggi_Formulati_ConDosi(Parametri)
                            'Risultati = ObjDownloadWs.Leggi_Formulati_ConDosi_Multiple(Parametri)

                            Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
                            Risultati = Risultati.Replace(">", ">" & vbCrLf)

                            ObjDownloadWs.Dispose()

                            'Carico la stringa XML nel documento
                            XmlDocRis = New System.Xml.XmlDocument

                            XmlDocRis.LoadXml(Risultati)

                            'Prelevo il nodo XmlParametri
                            XmlRisultati = XmlDocRis.SelectSingleNode("//RISULTATI")

                            strErr = CStr(XmlRisultati.GetAttribute("errore"))

                            If strErr = "" Then

                                XmlFormulati = XmlRisultati.GetElementsByTagName("FORMULATO")

                                For i = 0 To XmlFormulati.Count - 1

                                    XmlFormulato = XmlFormulati.Item(i)

                                    'controllo che nn ci siano doppioni relativi alla classificazione...
                                    If CStr(XmlFormulato.GetAttribute("fr_des")) <> LastFr_Des Then

                                        'se ? richiesto aggiungo il principio attivo principale
                                        If Flag_PrincipiAttivi Then
                                            strPA_Des = ""
                                            If Not IsDBNull(XmlFormulato.GetAttribute("pa_des")) And CStr(XmlFormulato.GetAttribute("pa_des")) <> "" Then
                                                strPA_Des = " --- <" + (CStr(XmlFormulato.GetAttribute("pa_des"))) + ">"
                                            End If
                                        End If

                                        strValue = XmlFormulato.GetAttribute("fr_cod").ToString & "£" &
                                                    XmlFormulato.GetAttribute("tempocarenza").ToString & "£" &
                                                    XmlFormulato.GetAttribute("dose_min").ToString & "£" &
                                                    XmlFormulato.GetAttribute("dose_max").ToString & "£" &
                                                    XmlFormulato.GetAttribute("udm_cod").ToString & "£" &
                                                    XmlFormulato.GetAttribute("udm_sim").ToString

                                        Controllo.Items.Add(New ListItem((CStr(XmlFormulato.GetAttribute("fr_des")) +
                                                                    "  (" + CStr(XmlFormulato.GetAttribute("fr_cod")) + ")" +
                                                                    strPA_Des),
                                                                    strValue))

                                        LastFr_Des = CStr(XmlFormulato.GetAttribute("fr_des"))

                                    End If

                                Next

                            End If

                            'Distruggo gli oggetti
                            XmlFormulato = Nothing
                            XmlFormulati = Nothing
                            XmlRisultati = Nothing
                            XmlDocRis = Nothing

                        Catch ex As Exception

                        End Try

                    End If

                End If

        End Select


    End Sub

    '####################################################################################
    ' Nuova versione che carica i FORMULATI con filtro CLASSIFICAZIONE 
    ' validi tra una DATA INIZIO e una DATA FINE
    '               - pu? essere aggiunta alla combo la descrizione di:
    '               1) classeTossicologica
    '               2) principiAttivi
    Public Sub Formulati_ConDosi(ByRef Controllo As ListControl,
                                            ByVal PrimaRiga_Flag As Boolean,
                                            ByVal PrimaRiga_Text As String,
                                            ByVal PrimaRiga_Value As String,
                                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                                            ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByVal FinestraTemp_Inizio As Date,
                                            ByVal FinestraTemp_fine As Date,
                                            Optional ByVal TestoRicerca As String = "",
                                            Optional ByVal Veg_Cod As Integer = 0,
                                            Optional ByVal strPA As String = "",
                                            Optional ByVal Flag_PrincipiAttivi As Boolean = False,
                                            Optional ByVal TipoRichiesto As Integer = 0,
                                            Optional ByVal strFiltro As String = "",
                                            Optional ByVal Opt_Avversita_Infestanti As Integer = 0,
                                            Optional ByVal Opt_Singola_Gruppo As Integer = 0,
                                            Optional ByVal Av_Gru() As Integer = Nothing,
                                            Optional ByVal Av_Cod() As Integer = Nothing,
                                            Optional ByVal grfi_cod As Integer = 0,
                                                Optional ByVal stato_cod As String = "IT"
                                        )

        'NOTA
        'Il TipoRichiesto consente di selezionare solo i formulati specifici
        'per la particolare applicazione:
        '
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, Bagnanti, Antischiuma
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        '   ecc...

        Dim XmlDoc As System.Xml.XmlDocument
        Dim XmlDocRis As System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim XmlRisultati As System.Xml.XmlElement
        Dim XmlFormulati As System.Xml.XmlNodeList
        Dim XmlFormulato As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim Parametri As String
        Dim Risultati As String

        Dim Dt As New DataTable

        Dim strErr As String


        Dim LastFr_Des As String = ""
        Dim i As Integer

        Dim strPA_Des As String
        Dim strFiltroClassificazione As String

        Dim strValue As String = ""

        'Pulisco la combo
        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        '########################################################################################################################
        ' Leggo i Formulati
        '########################################################################################################################


        Try

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString(),
                            objParametri_Utenti)

            Dim objAgroWS As New AgronicaCoreWebService.AgroWs

            XmlDoc = New System.Xml.XmlDocument

            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                    StrCredenziali,
                                    objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                    objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                    objSession("ASG_ProgressivoGIAS"),
                                    objSession("ASG_SuperUser_Username").ToString,
                                    objSession("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulati_ConDosi(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                   StrParametri,
                                                   Veg_Cod,
                                                   Opt_Avversita_Infestanti,
                                                   Opt_Singola_Gruppo,
                                                    Av_Gru,
                                                    Av_Cod,
                                                    strPA,
                                                    TipoRichiesto,
                                                    TestoRicerca,
                                                    FinestraTemp_fine,
                                                    strFiltro,
                                                    "",
                                                    strErr,
                                                    grfi_cod,
                                                    0,
                                                    stato_cod)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objAgroWS.AWS_Codifica_P(Parametri)

            'Risultati = ObjDownloadWs.Leggi_Formulati_ConDosi_Multiple(Parametri)
            Risultati = ObjDownloadWs.Leggi_Formulati_ConDosi(Parametri)

            Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            ObjDownloadWs.Dispose()

            'Carico la stringa XML nel documento
            XmlDocRis = New System.Xml.XmlDocument

            XmlDocRis.LoadXml(Risultati)

            'Prelevo il nodo XmlParametri
            XmlRisultati = XmlDocRis.SelectSingleNode("//RISULTATI")

            strErr = CStr(XmlRisultati.GetAttribute("errore"))

            If strErr = "" Then

                XmlFormulati = XmlRisultati.GetElementsByTagName("FORMULATO")

                For i = 0 To XmlFormulati.Count - 1

                    XmlFormulato = XmlFormulati.Item(i)

                    'controllo che nn ci siano doppioni relativi alla classificazione...
                    If CStr(XmlFormulato.GetAttribute("fr_des")) <> LastFr_Des Then

                        'se ? richiesto aggiungo il principio attivo principale
                        If Flag_PrincipiAttivi Then
                            strPA_Des = ""
                            If Not IsDBNull(XmlFormulato.GetAttribute("pa_des")) And CStr(XmlFormulato.GetAttribute("pa_des")) <> "" Then
                                strPA_Des = " --- <" + (CStr(XmlFormulato.GetAttribute("pa_des"))) + ">"
                            End If
                        End If

                        strValue = XmlFormulato.GetAttribute("fr_cod").ToString & "£" &
                                    XmlFormulato.GetAttribute("tempocarenza").ToString & "£" &
                                    XmlFormulato.GetAttribute("dose_min").ToString & "£" &
                                    XmlFormulato.GetAttribute("dose_max").ToString & "£" &
                                    XmlFormulato.GetAttribute("udm_cod").ToString & "£" &
                                    XmlFormulato.GetAttribute("udm_sim").ToString

                        Controllo.Items.Add(New ListItem((CStr(XmlFormulato.GetAttribute("fr_des")) +
                                                    "  (" + CStr(XmlFormulato.GetAttribute("fr_cod")) + ")" +
                                                    strPA_Des),
                                                    strValue))

                        LastFr_Des = CStr(XmlFormulato.GetAttribute("fr_des"))

                    End If

                Next

            End If

            'Distruggo gli oggetti
            XmlFormulato = Nothing
            XmlFormulati = Nothing
            XmlRisultati = Nothing
            XmlDocRis = Nothing


        Catch ex As Exception


        End Try


    End Sub


    Public Sub IAF_Elenco(ByRef Controllo As ListControl,
                            ByVal PrimaRiga_Flag As Boolean,
                            ByVal PrimaRiga_Text As String,
                            ByVal PrimaRiga_Value As String,
                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Disciplinare_Cod As Integer,
                            ByVal VEG_COD As Integer,
                            ByRef objWeb As AgronicaCoreGestioneRichieste.AgroWebConfig)


        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("IAF_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("IAF_Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("IAF_Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("IAF_Descrizione_Metodo", GetType(String)))

        'Pulisco la combo
        Controllo.Items.Clear()

        'Verifico la possibilit di gestione dei disciplinari
        'If permessoDPI And (VEG_COD <> 0 Or Consultazione) Then
        If objWeb.Flag_DisciplinareAttivo = True Then

            Try

                Dim agroWs As String
                If IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    'creo l'agrowebconfig
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                Else
                    agroWs = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs, agroWs, objParametri_Utenti)

                Dati = ObjDownloadWs.Leggi_IAF(Disciplinare_Cod,
                                                VEG_COD,
                                                objParametri_Server.FinestraTemporaleInizio,
                                                objParametri_Server.FinestraTemporaleFine,
                                                CStr(objSession("ASG_Utente_Username_Crypt").ToString),
                                                CStr(objSession("ASG_Utente_Password_Crypt").ToString))


                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                    If Not XmlNodo Is Nothing Then
                        For Each XmlElemento In XmlNodo
                            Dr = Dt.NewRow
                            Dr.Item("IAF_Cod") = XmlElemento.GetAttribute("iaf_cod")
                            Dr.Item("IAF_Numero") = XmlElemento.GetAttribute("iaf_numero")
                            Dr.Item("IAF_Descrizione") = XmlElemento.GetAttribute("iaf_descrizione")
                            Dr.Item("IAF_Descrizione_Metodo") = XmlElemento.GetAttribute("iaf_descrizione_metodo")
                            Dt.Rows.Add(Dr)
                        Next
                    End If
                End If

                If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

                    Dim Dv As New DataView
                    Dt.TableName = "IAF"
                    Dv.Table = Dt
                    Dv.Sort = "IAF_Numero, IAF_Descrizione, IAF_Descrizione_Metodo"

                    For i As Integer = 0 To Dv.Count - 1
                        Controllo.Items.Add(New ListItem(Dv.Item(i).Item("IAF_Numero") & " " & Dv.Item(i).Item("IAF_Descrizione") & " - " & Dv.Item(i).Item("IAF_Descrizione_Metodo"), Dv.Item(i).Item("IAF_Cod")))
                    Next

                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) & ex.Message

            End Try

        End If

    End Sub




End Class
