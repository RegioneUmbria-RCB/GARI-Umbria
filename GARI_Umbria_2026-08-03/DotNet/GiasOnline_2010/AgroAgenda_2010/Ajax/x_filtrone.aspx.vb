Imports System.Web.Services
Imports System.Web.Caching
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class x_filtrone
    Inherits System.Web.UI.Page



#Region "ricerca territoriale"
    <Script.Services.ScriptMethod()> _
  <WebMethod(EnableSession:=True)> _
    Public Shared Function GetRegioni() As String
        If IsNothing(System.Web.HttpContext.Current.Cache("GetRegioni")) Then
            Dim objParametri_server As AgronicaCoreParametri
            objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objR As New AgronicaCoreMetaSchemaDAL.Lista_Regioni_R
            Dim dt As DataTable = objR.Leggi("", "", "", objParametri_server)

            Dim strRisp As String = "["
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1
                If i > 0 Then
                    strRisp = strRisp & ","
                End If
                strRisp = strRisp & "{""des"":""" & dt.Rows(i).Item("regione_des") & """, ""val"":""" & dt.Rows(i).Item("reg") & """}"
            Next
            strRisp = strRisp & "]"
            strRisp = strRisp.Replace("'", "'")
            System.Web.HttpContext.Current.Cache("ListaRegioni") = strRisp

            Return strRisp
        Else
            Return System.Web.HttpContext.Current.Cache("ListaRegioni")
        End If

        Return True
    End Function


    <Script.Services.ScriptMethod()> _
  <WebMethod(EnableSession:=True)> _
    Public Shared Function GetProvincie(ByVal valori_regioni As String) As String
        Dim strP As String = "GetProvincie_" & valori_regioni
        If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then
            Dim objParametri_server As AgronicaCoreParametri
            objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objR As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
            Dim dt As DataTable = objR.Leggi("", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                             " Lista_Province.REG in(" & valori_regioni & ")", "", objParametri_server)


            Dim strRisp As String = "["
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1
                If i > 0 Then
                    strRisp = strRisp & ","
                End If
                strRisp = strRisp & "{""regione_des"":""" & dt.Rows(i).Item("regione_des") & """, ""provincia"":""" & dt.Rows(i).Item("provincia") & """, ""prov"":""" & dt.Rows(i).Item("prov") & """}"
            Next
            strRisp = strRisp & "]"

            strRisp = strRisp.Replace("'", "'")
            System.Web.HttpContext.Current.Cache(strP) = strRisp
            Return strRisp
        Else
            Return System.Web.HttpContext.Current.Cache(strP)
        End If





        Return True
    End Function


    <Script.Services.ScriptMethod()> _
  <WebMethod(EnableSession:=True)> _
    Public Shared Function GetComuni(ByVal valori_provincia As String) As String

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objR As New AgronicaCoreMetaSchemaDAL.ISTAT_Comuni_R
        Dim dt As DataTable = objR.Leggi2(" Lista_Province.prov in(" & valori_provincia & ")", "", objParametri_server)


        Dim strRisp As String = "["
        Dim i As Integer
        strRisp = strRisp & "{""provincia_des"":""" & "nessuno" & """, ""descrizione"":""" & "nessuno" & """, ""cod_istat"":""" & "" & """}"
        For i = 0 To dt.Rows.Count - 1
            'If i > 0 Then
            strRisp = strRisp & ","
            'End If
            strRisp = strRisp & "{""provincia_des"":""" & dt.Rows(i).Item("provincia_des") & """, ""descrizione"":""" & dt.Rows(i).Item("descrizione") & """, ""cod_istat"":""" & dt.Rows(i).Item("cod_istat") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        Return strRisp


        Return True
    End Function

#End Region



#Region "dettagli"

    <Script.Services.ScriptMethod()> _
 <WebMethod(EnableSession:=True)> _
    Public Shared Function CaricaCombo_Codici(ByVal tipo As String) As String

        'Dim strP As String = "CaricaCombo_Codici_" & tipo
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then



        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim strFiltro

        If tipo = "azienda" Then
            'CaricaCombo_Codici(Me.Cmb_CodiceAzienda, StrCodiciAzienda, 0, "", True, "", "0", objParametri_Server)
            Dim objCodiceAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            strFiltro = objCodiceAn.Filtro_Codici_Anagrafe(1, 3, 2, objParametri_server)
            strFiltro = Replace(strFiltro, " Or Codice = 1016", "")
            strFiltro = Replace(strFiltro, "Codice = 1016 Or ", "")
            strFiltro = Replace(strFiltro, " Or Codice = 1088", "")
            strFiltro = Replace(strFiltro, " Codice = 1088", "")
        End If

        If tipo = "appezzamento" Then
            'CaricaCombo_Codici(Me.Cmb_CodiceAzienda, StrCodiciAzienda, 0, "", True, "", "0", objParametri_server)
            Dim objCodiceAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            strFiltro = objCodiceAn.Filtro_Codici_Anagrafe(3, 3, 2, objParametri_server)

        End If

        If tipo = "impianto" Then
            'CaricaCombo_Codici(Me.Cmb_CodiceImpianto, StrCodiciImpianto, 0, "", True, "", "0", objParametri_Server)
            Dim objCodiceAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            strFiltro = objCodiceAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_server)

            strFiltro = Replace(strFiltro, " Or Codice = 1016", "")
            strFiltro = Replace(strFiltro, "Codice = 1016 Or ", "")
            strFiltro = Replace(strFiltro, " Or Codice = 1018", "")
            strFiltro = Replace(strFiltro, "Codice = 1018 Or ", "")
            strFiltro = Replace(strFiltro, " Or Codice = 1108", "")
            strFiltro = Replace(strFiltro, "Codice = 1108 Or ", "")
        End If




        Dim DT As DataTable
        Dim i As Integer
        Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        DT = objCodiceAnagrafe.Leggi(0, _
                                      "", _
                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                      strFiltro, _
                                      "", _
                                      objParametri_server)

        Dim strRisp As String = "["
        For i = 0 To DT.Rows.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & DT.Rows(i).Item("codice") & """, ""descrizione"":""" & DT.Rows(i).Item("descrizione") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If

    End Function




    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Carica_Zone() As String
        'Dim strP As String = "Carica_Zone"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")


        Dim DT As DataTable

        Dim objZone As New AgronicaCoreAnagrafeDAL.Zone_R
        DT = objZone.Leggi(0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                            "", "", objParametri_server)


        Dim strRisp As String = "["
        For i = 0 To DT.Rows.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""descrizione"":""" & DT.Rows(i).Item("Descrizione") & """, ""zona_cod"":""" & DT.Rows(i).Item("Zona_Cod") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If

    End Function


    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True )> _
    Public Shared Function GetMacrouso() As String
        Dim strP As String = "GetMacrouso"
        If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

            Dim objParametri_server As AgronicaCoreParametri
            objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")


            Dim DT As DataTable

            Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
            DT = objMacrousi.Leggi("", "", _
                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                "", "", objParametri_server)





            Dim strRisp As String = "["
            For i = 0 To DT.Rows.Count - 1
                If i > 0 Then
                    strRisp = strRisp & ","
                End If
                strRisp = strRisp & "{""descrizione"":""" & DT.Rows(i).Item("Macrouso_Des") & """, ""macrouso_cod"":""" & DT.Rows(i).Item("Macrouso_Cod") & """}"
            Next
            strRisp = strRisp & "]"
            strRisp = strRisp.Replace("'", "'")
            System.Web.HttpContext.Current.Cache(strP) = strRisp
            Return strRisp
        Else
            Return System.Web.HttpContext.Current.Cache(strP)
        End If
    End Function


#End Region



#Region "Utenti (azienda)"

    <Script.Services.ScriptMethod()> _
 <WebMethod(EnableSession:=True, CacheDuration:=43200)> _
    Public Shared Function Carica_TecnicoRiferimento() As String
        Dim strP As String = "Carica_TecnicoRiferimento"
        If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then


            Dim objParametri_server As AgronicaCoreParametri
            objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")


            Dim i As Integer
            Dim Rag_Soc As String
            Dim Dt_Contatti As DataTable

            Dt_Contatti = New DataTable

            Dim objContattoCon As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dt_Contatti = objContattoCon.Contatti_Contatto_Leggi( _
                                                        objParametri_server.PivaSuperUser, _
                                                        "", _
                                                        0, _
                                                        -6, _
                                                        True, _
                                                        False, _
                                                        0, _
                                                        0, _
                                                        False, _
                                                        0, _
                                                        -99, _
                                                         0, "", False, 0, 0, 0, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                         "", "", objParametri_server)

            Dim strRisp As String = "["
            For i = 0 To Dt_Contatti.Rows.Count - 1
                If i > 0 Then
                    strRisp = strRisp & ","
                End If

                Rag_Soc = CStr(Dt_Contatti.Rows(i).Item("Rag_Soc")) + _
                            CStr(Dt_Contatti.Rows(i).Item("Cognome")) + " " + _
                                CStr(Dt_Contatti.Rows(i).Item("Nome"))

                strRisp = strRisp & "{""val"":""" & Dt_Contatti.Rows(i).Item("Cod_Contatto") & """, ""descrizione"":""" & _
                                            Rag_Soc & """}"
            Next
            strRisp = strRisp & "]"
            strRisp = strRisp.Replace("'", "'")
            System.Web.HttpContext.Current.Cache(strP) = strRisp
            Return strRisp
        Else
            Return System.Web.HttpContext.Current.Cache(strP)
        End If
    End Function


    <Script.Services.ScriptMethod()> _
 <WebMethod(EnableSession:=True, CacheDuration:=43200)> _
    Public Shared Function Carica_Utenti() As String
        'Dim strP As String = "Carica_Utenti"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then


        Dim objParametri_utenti As AgronicaCoreParametri
        objParametri_utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")


        Dim i As Integer


        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim DT As DataTable
        DT = objUtenti.Leggi("", _
                            5, _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                            "", "", objParametri_utenti)


        Dim strRisp As String = "["
        For i = 0 To DT.Rows.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If

            strRisp = strRisp & "{""user"":""" & DT.Rows(i).Item("UserName") & """, ""cod_fisc"":""" & _
                                        DT.Rows(i).Item("CodFisc") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function




#End Region


#Region "Parole Chiave "

    <Script.Services.ScriptMethod()> _
 <WebMethod(EnableSession:=True, CacheDuration:=43200)> _
    Public Shared Function Carica_ParolaChiave() As String
        'Dim strP As String = "Carica_ParolaChiave"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")


        Dim i As Integer

        Dim DT As New DataTable

        Dim objPChiaveImpresa As New AgronicaCoreAnagrafeDAL.ParoleChiave_R
        ' stefania
        DT = objPChiaveImpresa.Leggi("", _
                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                    "", "", objParametri_server)



        Dim strRisp As String = "["
        For i = 0 To DT.Rows.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If

            strRisp = strRisp & "{""parola"":""" & DT.Rows(i).Item("ParolaChiave") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If

    End Function




#End Region









    <Script.Services.ScriptMethod()> _
 <WebMethod(EnableSession:=True)> _
    Public Shared Function GetTipoCentro() As String
        'Dim strP As String = "GetTipoCentro"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")


        Dim i As Integer


        Dim objCOM As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R   'Agro_Anagrafe_AD.Codici_Anagrafe_Read
        Dim DT As DataTable

        'Leggo le imprese associate al profilo selezionato			
        DT = objCOM.Leggi(0, "", _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                            "creatore = 'CSA' and gruppo = 'TIPO_CA'", _
                            "", _
                            objParametri_Server)


        Dim strRisp As String = "["
        strRisp = strRisp & "{""stringa"":""" & "" & """, ""valore"":""" & _
                                        "0" & """}"
        For i = 0 To DT.Rows.Count - 1
            strRisp = strRisp & ","

            strRisp = strRisp & "{""stringa"":""" & DT.Rows(i).Item("descrizione") & """, ""valore"":""" & _
                                        DT.Rows(i).Item("codice") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function


    <Script.Services.ScriptMethod()> _
 <WebMethod(EnableSession:=True)> _
    Public Shared Function GetOTE() As String
        'Dim strP As String = "GetOTE"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then


        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")


        Dim i As Integer

        Dim DT As DataTable
        'Creo gli oggetti COM
        Dim objCOM As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R
        'Leggo le imprese associate al profilo selezionato	
        DT = objCOM.Leggi("", _
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                             "", "OTE_ordine ASC", objParametri_Server)




        Dim strRisp As String = "["
        strRisp = strRisp & "{""stringa"":""" & " " & """, ""valore"":""" & _
                                        " " & """}"
        For i = 0 To DT.Rows.Count - 1
            strRisp = strRisp & ","
            strRisp = strRisp & "{""stringa"":""" & DT.Rows(i).Item("OTE_DES") & """, ""valore"":""" & _
                                        DT.Rows(i).Item("OTE_COD") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If

    End Function



    <Script.Services.ScriptMethod()> _
 <WebMethod(EnableSession:=True)> _
    Public Shared Function GetCodiceStruttura() As String
        'Dim strP As String = "GetCodiceStruttura"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then


        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim i As Integer

        Dim obj As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R    'Agro_Anagrafe_AD.Codici_Anagrafe_Read
        Dim DT As DataTable
        Dim Testo As String

        'Leggo le imprese associate al profilo selezionato			
        DT = obj.Leggi(0, "", _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                           "  UPPER(gruppo) = 'CENTRO' OR creatore = 'CRPA'  ", _
                            "", _
                            objParametri_Server)




        Dim strRisp As String = "["
        strRisp = strRisp & "{""stringa"":""" & "" & """, ""valore"":""" & _
                                        "0" & """}"
        For i = 0 To DT.Rows.Count - 1
            strRisp = strRisp & ","
            strRisp = strRisp & "{""stringa"":""" & DT.Rows(i).Item("descrizione") & """, ""valore"":""" & _
                                        DT.Rows(i).Item("codice") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If

    End Function
     
    <Script.Services.ScriptMethod()> _
 <WebMethod(EnableSession:=True)> _
    Public Shared Function GetOrganismoControllo() As String
        'Dim strP As String = "GetOrganismoControllo"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim i As Integer

        Dim obj As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim DT As DataTable

        DT = obj.Leggi(0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                            "creatore = 'AGRONICA' and gruppo = 'ORGANISMO'", _
                            "", _
                            objParametri_Server)




        Dim strRisp As String = "["
        strRisp = strRisp & "{""stringa"":""" & "" & """, ""valore"":""" & _
                                        "0" & """}"
        For i = 0 To DT.Rows.Count - 1
            strRisp = strRisp & ","
            strRisp = strRisp & "{""stringa"":""" & DT.Rows(i).Item("descrizione") & """, ""valore"":""" & _
                                        DT.Rows(i).Item("codice") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function
     
    '   <Script.Services.ScriptMethod()> _
    '<WebMethod(EnableSession:=True)> _
    '   Public Shared Function GetTitoloPossessoStruttura() As String
    '       Dim strP As String = "GetTitoloPossessoStruttura"
    '       If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

    '           Dim strRisp As String = "["
    '           strRisp = strRisp & "{""stringa"":""" & "Altro" & """, ""valore"":""" & _
    '                                           "0" & """}"
    '           strRisp = strRisp & ","

    '           strRisp = strRisp & "{""stringa"":""" & "Proprietà" & """, ""valore"":""" & _
    '                                           "1" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Comodato d'uso" & """, ""valore"":""" & _
    '                                           "2" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Affitto con contratto" & """, ""valore"":""" & _
    '                                           "3" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Affitto senza contratto" & """, ""valore"":""" & _
    '                                           "4" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "In conto terzi" & """, ""valore"":""" & _
    '                                           "5" & """}"

    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "In  convenzione" & """, ""valore"":""" & _
    '                                           "6" & """}"

    '           strRisp = strRisp & "]"
    '           strRisp = strRisp.Replace("'", "'")
    '           System.Web.HttpContext.Current.Cache(strP) = strRisp
    '           Return strRisp
    '       Else
    '           Return System.Web.HttpContext.Current.Cache(strP)
    '       End If
    '   End Function

    '   <Script.Services.ScriptMethod()> _
    '<WebMethod(EnableSession:=True)> _
    '   Public Shared Function GetTipoAttivita() As String
    '       Dim strP As String = "GetTipoAttivita"
    '       If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

    '           Dim strRisp As String = "["
    '           strRisp = strRisp & "{""stringa"":""" & " " & """, ""valore"":""" & _
    '                                           " " & """}"
    '           strRisp = strRisp & ","

    '           strRisp = strRisp & "{""stringa"":""" & "Produzione vegetale" & """, ""valore"":""" & _
    '                                           "PV" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Produzione zootecnica" & """, ""valore"":""" & _
    '                                           "PZ" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Produzione vegetale e zootecnica" & """, ""valore"":""" & _
    '                                           "PVZ" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Preparazione vegetale" & """, ""valore"":""" & _
    '                                           "TPV" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Preparazione zootecnica" & """, ""valore"":""" & _
    '                                           "TPZ" & """}"

    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Preparazione vegetale e zootecnica" & """, ""valore"":""" & _
    '                                           "TPVZ" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Importazione" & """, ""valore"":""" & _
    '                                           "I" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Raccolta spontanea" & """, ""valore"":""" & _
    '                                           "RS" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Produzione / Preparazione" & """, ""valore"":""" & _
    '                                           "P/TP" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Preparazione / Importazione" & """, ""valore"":""" & _
    '                                           "TP/I" & """}"
    '           strRisp = strRisp & ","
    '           strRisp = strRisp & "{""stringa"":""" & "Altro" & """, ""valore"":""" & _
    '                                           "@" & """}"


    '           strRisp = strRisp & "]"
    '           strRisp = strRisp.Replace("'", "'")
    '           System.Web.HttpContext.Current.Cache(strP) = strRisp
    '           Return strRisp
    '       Else
    '           Return System.Web.HttpContext.Current.Cache(strP)
    '       End If
    '   End Function
      
    '    <Script.Services.ScriptMethod()> _
    '<WebMethod(EnableSession:=True)> _
    '    Public Shared Function GetTitoloPossessoAppezzamento() As String
    '        Dim strP As String = "GetTitoloPossessoAppezzamento"
    '        If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then


    '            Dim i As Integer

    '            Dim strRisp As String = "["

    '            strRisp = strRisp & "{""stringa"":""" & "Altro" & """, ""valore"":""" & _
    '                                            "0" & """}"
    '            strRisp = strRisp & ","
    '            strRisp = strRisp & "{""stringa"":""" & "Proprietà" & """, ""valore"":""" & _
    '                                            "1" & """}"
    '            strRisp = strRisp & ","
    '            strRisp = strRisp & "{""stringa"":""" & "Comodato d'uso" & """, ""valore"":""" & _
    '                                            "2" & """}"
    '            strRisp = strRisp & ","
    '            strRisp = strRisp & "{""stringa"":""" & "Affitto con contratto" & """, ""valore"":""" & _
    '                                            "3" & """}"
    '            strRisp = strRisp & ","
    '            strRisp = strRisp & "{""stringa"":""" & "Affitto senza contratto" & """, ""valore"":""" & _
    '                                            "4" & """}"
    '            strRisp = strRisp & ","
    '            strRisp = strRisp & "{""stringa"":""" & "In conto terzi" & """, ""valore"":""" & _
    '                                            "5" & """}"
    '            strRisp = strRisp & ","
    '            strRisp = strRisp & "{""stringa"":""" & "In convenzione" & """, ""valore"":""" & _
    '                                            "6" & """}"
    '            strRisp = strRisp & "]"
    '            strRisp = strRisp.Replace("'", "'")
    '            System.Web.HttpContext.Current.Cache(strP) = strRisp
    '            Return strRisp
    '        Else
    '            Return System.Web.HttpContext.Current.Cache(strP)
    '        End If
    '    End Function





    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetSpecie(ByVal Gru_Cod As String) As String
        'Dim strP As String = "GetSpecie" & Gru_Cod
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")



        Dim str_filtoro As String = " SpecieVegetali.Gru_Cod in (" & Gru_Cod & ") "
        If Gru_Cod = "0" Then
            str_filtoro = ""
        End If

        Dim cblSpecie As New DropDownList
        Dim i As Integer



        CaricaCheckBoxList_SpecieVegetale_Optimize(cblSpecie, 0, True, str_filtoro, "", 0, 0, objParametri_server, objParametri_Utenti)


        Dim strRisp As String = "["
        For i = 0 To cblSpecie.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & cblSpecie.Items(i).Value & """, ""descrizione"":""" & cblSpecie.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function


    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetCultivar(ByVal Veg_Cod As String) As String
        'Dim strP As String = "GetCultivar" & Veg_Cod
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")



        Dim CBL_Cultivar As New DropDownList
        Dim i As Integer
        Cultivar(CBL_Cultivar, False, "", "", Veg_Cod, 0, "", True, 0, 0, "", "", objParametri_server, objParametri_Utenti)


        Dim strRisp As String = "["
        For i = 0 To CBL_Cultivar.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL_Cultivar.Items(i).Value & """, ""descrizione"":""" & CBL_Cultivar.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function



    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetTipologiaVarietale(ByVal Veg_Cod As String) As String
        'Dim strP As String = "GetTipologiaVarietale" & Veg_Cod
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_GruppoVarietale As New DropDownList
        Dim i As Integer
        GruppoVarietalexSpecie(CBL_GruppoVarietale, False, "", "", Veg_Cod, "", "", objParametri_server)

        Dim strRisp As String = "["
        For i = 0 To CBL_GruppoVarietale.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL_GruppoVarietale.Items(i).Value & """, ""descrizione"":""" & CBL_GruppoVarietale.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function

    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetFinalitaProduttive(ByVal Veg_Cod As String) As String
        'Dim strP As String = "GetFinalitaProduttive" & Veg_Cod
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_Finalita As New DropDownList
        Dim i As Integer
        Finalita(CBL_Finalita, False, "", "", Veg_Cod, 0, "", "", "", objParametri_server)

        Dim strRisp As String = "["


        strRisp = strRisp & "{""codice"":"""", ""descrizione"":""""}"

        For i = 0 To CBL_Finalita.Items.Count - 1
            strRisp = strRisp & ","
            strRisp = strRisp & "{""codice"":""" & CBL_Finalita.Items(i).Value & """, ""descrizione"":""" & CBL_Finalita.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function


    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetRegolamento() As String
        'Dim strP As String = "GetRegolamento"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_Regolamento As New DropDownList
        Dim i As Integer
        Regolamento(CBL_Regolamento, True, "", "", "", "", objParametri_server)

        Dim strRisp As String = "["
        For i = 0 To CBL_Regolamento.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL_Regolamento.Items(i).Value & """, ""descrizione"":""" & CBL_Regolamento.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function

    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetDisciplinare() As String
        'Dim strP As String = "GetDisciplinare"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim Cmb_Disciplinare As New DropDownList
        Dim i As Integer

        Dim objCaricaCombo As New AgronicaCoreDpiBIZ.CaricaListControl
        objCaricaCombo.Disciplinari_Elenco_Metaschema(Cmb_Disciplinare, _
                                       True, "", "", _
                                       objParametri_server, _
                                       0, _
                                       False, _
                                       True, _
                                            New AgronicaCoreGestioneRichieste.AgroWebConfig)


        Dim strRisp As String = "["
        For i = 0 To Cmb_Disciplinare.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & Cmb_Disciplinare.Items(i).Value & """, ""descrizione"":""" & Cmb_Disciplinare.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function


    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetCapitolato() As String
        'Dim strP As String = "GetCapitolato"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_Capitolato As New DropDownList
        Dim i As Integer
        Capitolato_Privato(CBL_Capitolato, False, "", "", "", 0, 2, "", "", objParametri_server)

        Dim strRisp As String = "["
        For i = 0 To CBL_Capitolato.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL_Capitolato.Items(i).Value & """, ""descrizione"":""" & CBL_Capitolato.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function

    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetStatoImpianto() As String
        'Dim strP As String = "GetStatoImpianto"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_StatoImpianto As New DropDownList
        Dim i As Integer
        StatoImpianto(CBL_StatoImpianto, True, "", "", objParametri_server)

        Dim strRisp As String = "["
        For i = 0 To CBL_StatoImpianto.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL_StatoImpianto.Items(i).Value & """, ""descrizione"":""" & CBL_StatoImpianto.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If

    End Function

    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetCopertura(ByVal Gru_Cod As String) As String
        'Dim strP As String = "GetCopertura" & Gru_Cod
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_Copertura As New DropDownList
        Dim i As Integer
        Copertura(CBL_Copertura, True, "", "", Gru_Cod, 0, "", "", "", objParametri_server)

        Dim strRisp As String = "["
        For i = 0 To CBL_Copertura.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL_Copertura.Items(i).Value & """, ""descrizione"":""" & CBL_Copertura.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp

        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function



    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetDettagliVarietaPersonalizzato() As String
        'Dim strP As String = "GetDettagliVarietaPersonalizzato"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_Dettagli As New DropDownList
        Dim i As Integer
        DettaglioPersonalizzato_Impianto(CBL_Dettagli, True, "", "0", "", "", objParametri_server)

        Dim strRisp As String = "["
        For i = 0 To CBL_Dettagli.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL_Dettagli.Items(i).Value & """, ""descrizione"":""" & CBL_Dettagli.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function








    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetGruppoOperazioni() As String
        'Dim strP As String = "GetGruppoOperazioni"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")


        Dim Dt As DataTable
        Dim i As Integer
        Dim FiltroAggiuntivo As String


        'Cbl.Items.Clear()

        FiltroAggiuntivo = "(GOper.GRU_COD = 1 OR GOper.GRU_COD = 2 OR GOper.GRU_COD = 3 OR GOper.GRU_COD = 4 ) "

        Dim objGrOp As New AgronicaCoreMetaSchemaDAL.GruppoOperazioni_R

        Dt = objGrOp.Leggi(0, _
                            "", _
                            0, "", True, False, False, False, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                FiltroAggiuntivo, "", objParametri_server)


        Dim strRisp As String = "["
        For i = 0 To Dt.Rows.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & Dt.Rows(i).Item("Gru_Cod") & """, ""descrizione"":""" & Dt.Rows(i).Item("Gru_des") & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function





    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetOperazioni(ByVal Gruppo_Operazioni As String) As String
        'Dim strP As String = "GetOperazioni" & Gruppo_Operazioni
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then
        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim ddl As New DropDownList

        Dim i As Integer
        Dim objOperazioni As New AgronicaCoreMetaSchemaDAL.GruppoOperazioni_R


        Dim strRisp As String = "["

        For i = 0 To Gruppo_Operazioni.Split(",").Length - 1
            Dim gru_cod As String = Gruppo_Operazioni.Split(",")(i)

            Dim gru_des = objOperazioni.GruOper_Des_from_GruOper_Cod(gru_cod, objParametri_server)
            Operazioni(ddl, False, "", "", 0, 0, gru_cod, "", "", objParametri_server)

            If i > 0 Then
                strRisp = strRisp & ","
            End If

            Dim j As Integer
            For j = 0 To ddl.Items.Count - 1
                If j > 0 Then
                    strRisp = strRisp & ","
                End If
                strRisp = strRisp & "{""gruppo"":""" & gru_des & """, ""codice"":""" & ddl.Items(j).Value & """, ""descrizione"":""" & ddl.Items(j).Text & """}"
            Next
        Next

        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If



    End Function



    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetCategoriaProdotto() As String
        'Dim strP As String = "GetCategoriaProdotto"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then
        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_prodotto As New DropDownList
        Dim i As Integer
        CategorieMagazzino(CBL_prodotto, True, "", "", 0, CAU_SCARICO, 0, False, False, "", "", objParametri_server)

        Dim strRisp As String = "["
        For i = 0 To CBL_prodotto.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL_prodotto.Items(i).Value & """, ""descrizione"":""" & CBL_prodotto.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function




    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetRicercaProdotti(ByVal tipologia As String, ByVal str_ricerca As String) As String
        


        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_prodotto As New DropDownList
        Dim i As Integer


        ProdottiAnagrafica(CBL_prodotto, _
                                        False, "", "", _
                                        AGRODATAFINE, _
                                        tipologia, _
                                        str_ricerca, _
                                        False, _
                                        False, _
                                        "", _
                                        objParametri_server)

        Dim strRisp As String = "["
        For i = 0 To CBL_prodotto.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL_prodotto.Items(i).Value & """, ""descrizione"":""" & CBL_prodotto.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        Return strRisp
    End Function



    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetInfestantiAvversita(ByVal veg_cod As String, ByVal lav_cod As String, ByVal singolo_gruppo As String) As String
        'Dim strP As String = "GetInfestantiAvversita" & veg_cod & "_" & lav_cod & "_" & singolo_gruppo
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL As New DropDownList
        Dim i As Integer
        Dim clc = New AgronicaCoreUtility.CaricaListControl

        If lav_cod = 74 Then
            clc.Avversita(CBL,
                        False, "", "",
                        veg_cod,
                        0,
                        "",
                        "",
                        objParametri_server)
        End If
        If lav_cod = 18 Then
            If singolo_gruppo = 0 Then
                Infestanti(CBL, False, "", "",
                        "", "", objParametri_server)
            Else
                GruppoInfestanti(CBL, False, "", "", _
                        "", "", objParametri_server)
            End If
        End If


        Dim strRisp As String = "["
        For i = 0 To CBL.Items.Count - 1
            If i > 0 Then
                strRisp = strRisp & ","
            End If
            strRisp = strRisp & "{""codice"":""" & CBL.Items(i).Value & """, ""descrizione"":""" & CBL.Items(i).Text & """}"
        Next
        strRisp = strRisp & "]"
        strRisp = strRisp.Replace("'", "'")
        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function


End Class