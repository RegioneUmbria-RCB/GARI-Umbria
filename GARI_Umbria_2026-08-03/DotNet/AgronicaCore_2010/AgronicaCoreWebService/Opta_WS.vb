Imports System.ServiceModel
Imports System.Web.Services.Protocols
Imports System.Net
Imports System.Net.WebRequestMethods.Http
Imports System.ServiceModel.Channels
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Opta_Ws


    'https://tti.ttitalia.it:92/wstracc.asmx


#Region "Lavorazione"



    Public Shared Function Importa_TestNoImport_Lavorazione(
                                         ByRef msg As String,
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Lavorazioni(User, passw, startID, limit, False, True, False, msg, objParametri_Server)
    End Function

    Public Shared Function Importa_Test_Lavorazione( _
                                         ByRef msg As String, _
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Lavorazioni(User, passw, startID, limit, False, False, True, msg, objParametri_Server)
    End Function

    Public Shared Function Importa_Tutti_Lavorazione( _
                                         ByRef msg As String, _
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Lavorazioni(User, passw, startID, limit, True, False, True, msg, objParametri_Server)
    End Function

    Public Shared Function Importa_Ultimi_Lavorazione( _
                                         ByRef msg As String, _
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Lavorazioni(User, passw, startID, limit, True, True, True, msg, objParametri_Server)
    End Function

#End Region

#Region "Prodotto Finito"


    Public Shared Function Importa_Test_ProdottoFinito( _
                                         ByRef msg As String, _
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Prodotto(User, passw, startID, limit, False, False, True, msg, objParametri_Server)
    End Function


    Public Shared Function Importa_Tutti_ProdottoFinito( _
                                         ByRef msg As String, _
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Prodotto(User, passw, startID, limit, True, False, True, msg, objParametri_Server)
    End Function

    Public Shared Function Importa_Ultimi_ProdottoFinito( _
                                     ByRef msg As String, _
                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Prodotto(User, passw, startID, limit, True, True, True, msg, objParametri_Server)
    End Function


#End Region

#Region "Carichi"


    Public Shared Function Importa_TestNoImport_Carichi(
                                         ByRef msg As String,
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Carichi(User, passw, startID, limit, False, True, False, msg, objParametri_Server)
    End Function

    Public Shared Function Importa_Tutti_Carichi( _
                                         ByRef msg As String, _
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Carichi(User, passw, startID, limit, True, False, True, msg, objParametri_Server)
    End Function


    Public Shared Function Importa_Ultimi_Carichi( _
                                     ByRef msg As String, _
                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim User As String = "wsTracc"
        Dim passw As String = "ext-2013$.ws"
        Dim startID As Integer = 140000000
        Dim limit As Integer = 500
        Return Leggi_Importa_Carichi(User, passw, startID, limit, True, True, True, msg, objParametri_Server)
    End Function

#End Region



    Public Shared Function Leggi_Importa_Lavorazioni(ByVal user As String, _
                                    ByVal passw As String, _
                                    ByVal startID As Integer, _
                                    ByVal limit As Integer, _
                                    ByVal iteratutto As Boolean, _
                                    ByVal importaDaUltimoCaricati As Boolean, _
                                    ByVal Importa As Boolean, _
                                    ByRef msg As String, _
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        'DataTable = Nothing

        msg = ""

        If user = "" Then
            user = "wsTracc"
        End If

        If passw = "" Then
            passw = "ext-2013$.ws"
        End If

        Dim sDebugInfo As String = ""


        Try


            'ATTENZIONE, bisogna creare il wsclient 
            'Conn aggiungi riferimento al servizio e AVANZATE,
            'in questo modo lo aggiunge sotto Web References
            'altrimenti se lo aggiungo senza avanzate me lo mette in Service References
            'che è in WCF e mi ibbliga ad usare la funzione GetwstraccSoapClient per mettere a mano bind e url non avendo 
            'appconfig disponibile, ma soprattutto non ho l'oggetto CookieContainer
            'e mettendo binding.AllowCookies = True non va

            'SoapClient.ClientCredentials.UserName.UserName = user
            'SoapClient.ClientCredentials.UserName.Password = passw
            ''Dim cookieJar As CookieContainer
            ''cookieJar = New CookieContainer()
            ''  Dim s = System.Web.HttpContext.Current.Response.Cookies
            'Dim loCookie = System.Web.HttpContext.Current.Request.Cookies("Session")

            'Dim responseMessageProperty As HttpResponseMessageProperty = OperationContext.Current.IncomingMessageProperties("")


            Dim ritorno As Boolean = True

            Dim SoapClient As New wstracc.wstracc
            Dim cc As New CookieContainer
            SoapClient.CookieContainer = cc
            'SoapClient = GetwstraccSoapClient()

            sDebugInfo = SoapClient.Url



            Dim autentica As Boolean = SoapClient.Authenticate(user, passw)
            If Not autentica Then
                Throw New Exception("Autenticazione fallita")
            End If

            autentica = SoapClient.isAuthenticate()
            If Not autentica Then
                Throw New Exception("non è autenticato")
            End If

            Dim Dt As New DataTable
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            Dim strsql As String = ""

            If importaDaUltimoCaricati Then
                strsql = " SELECT ISNULL(MAX(ID) + 1 , " & startID & ") as ID FROM [dbo].[__Rintraccio_Lavorazione]"
                Dt = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")
                If Dt.Rows.Count > 0 Then
                    startID = CInt(Dt.Rows(0).Item("ID"))                
                End If
            End If

            Dim count As Integer = 0
            Do


                Dt = LeggiLavorazione(startID, limit, Importa, msg, objParametri_Server, ritorno, SoapClient, dp, strsql)

                count += Dt.Rows.Count

            Loop Until (Not iteratutto Or Dt.Rows.Count = 0)

            msg = "Letti " & count & " record da ws."


            Return ritorno

        Catch ex As Exception
            msg = "Eccezione: " & "[ws dedug info: " & sDebugInfo & "]" & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return False
        End Try


    End Function


    Public Shared Function Leggi_Importa_Prodotto(ByVal user As String, _
                                    ByVal passw As String, _
                                    ByVal startID As Integer, _
                                    ByVal limit As Integer, _
                                    ByVal iteratutto As Boolean, _
                                    ByVal importaDaUltimoCaricati As Boolean, _
                                    ByVal Importa As Boolean, _
                                    ByRef msg As String, _
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        'DataTable = Nothing

        msg = ""

        If user = "" Then
            user = "wsTracc"
        End If

        If passw = "" Then
            passw = "ext-2013$.ws"
        End If

        Dim sDebugInfo As String = ""

        Try


            'ATTENZIONE, bisogna creare il wsclient 
            'Conn aggiungi riferimento al servizio e AVANZATE,
            'in questo modo lo aggiunge sotto Web References
            'altrimenti se lo aggiungo senza avanzate me lo mette in Service References
            'che è in WCF e mi ibbliga ad usare la funzione GetwstraccSoapClient per mettere a mano bind e url non avendo 
            'appconfig disponibile, ma soprattutto non ho l'oggetto CookieContainer
            'e mettendo binding.AllowCookies = True non va

            'SoapClient.ClientCredentials.UserName.UserName = user
            'SoapClient.ClientCredentials.UserName.Password = passw
            ''Dim cookieJar As CookieContainer
            ''cookieJar = New CookieContainer()
            ''  Dim s = System.Web.HttpContext.Current.Response.Cookies
            'Dim loCookie = System.Web.HttpContext.Current.Request.Cookies("Session")

            'Dim responseMessageProperty As HttpResponseMessageProperty = OperationContext.Current.IncomingMessageProperties("")


            Dim ritorno As Boolean = True

            Dim SoapClient As New wstracc.wstracc
            Dim cc As New CookieContainer
            SoapClient.CookieContainer = cc
            'SoapClient = GetwstraccSoapClient()

            sDebugInfo = SoapClient.Url


            Dim autentica As Boolean = SoapClient.Authenticate(user, passw)
            If Not autentica Then
                Throw New Exception("Autenticazione fallita")
            End If

            autentica = SoapClient.isAuthenticate()
            If Not autentica Then
                Throw New Exception("non è autenticato")
            End If

            Dim Dt As New DataTable
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            Dim strsql As String = ""

            If importaDaUltimoCaricati Then
                strsql = " SELECT ISNULL(MAX(ID) + 1 , " & startID & ") as ID FROM [dbo].[__Rintraccio_ProdottoFinito]"
                Dt = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")
                If Dt.Rows.Count > 0 Then
                    startID = CInt(Dt.Rows(0).Item("ID"))
                End If
            End If

            Dim count As Integer = 0
            Do


                Dt = LeggiProdottoFinito(startID, limit, Importa, msg, objParametri_Server, ritorno, SoapClient, dp, strsql)

                count += Dt.Rows.Count

            Loop Until (Not iteratutto Or Dt.Rows.Count = 0)

            msg = "Letti " & count & " record da ws."


            Return ritorno

        Catch ex As Exception
            msg = "Eccezione: " & "[ws dedug info: " & sDebugInfo & "]" & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return False
        End Try


    End Function


    Public Shared Function Leggi_Importa_Carichi(ByVal user As String, _
                                    ByVal passw As String, _
                                    ByVal startID As Integer, _
                                    ByVal limit As Integer, _
                                    ByVal iteratutto As Boolean, _
                                    ByVal importaDaUltimoCaricati As Boolean, _
                                    ByVal Importa As Boolean, _
                                    ByRef msg As String, _
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        'DataTable = Nothing

        msg = ""

        If user = "" Then
            user = "wsTracc"
        End If

        If passw = "" Then
            passw = "ext-2013$.ws"
        End If

        Dim sDebugInfo As String = ""

        Try


            'ATTENZIONE, bisogna creare il wsclient 
            'Conn aggiungi riferimento al servizio e AVANZATE,
            'in questo modo lo aggiunge sotto Web References
            'altrimenti se lo aggiungo senza avanzate me lo mette in Service References
            'che è in WCF e mi ibbliga ad usare la funzione GetwstraccSoapClient per mettere a mano bind e url non avendo 
            'appconfig disponibile, ma soprattutto non ho l'oggetto CookieContainer
            'e mettendo binding.AllowCookies = True non va

            'SoapClient.ClientCredentials.UserName.UserName = user
            'SoapClient.ClientCredentials.UserName.Password = passw
            ''Dim cookieJar As CookieContainer
            ''cookieJar = New CookieContainer()
            ''  Dim s = System.Web.HttpContext.Current.Response.Cookies
            'Dim loCookie = System.Web.HttpContext.Current.Request.Cookies("Session")

            'Dim responseMessageProperty As HttpResponseMessageProperty = OperationContext.Current.IncomingMessageProperties("")


            Dim ritorno As Boolean = True

            Dim SoapClient As New wstracc.wstracc
            Dim cc As New CookieContainer
            SoapClient.CookieContainer = cc
            'SoapClient = GetwstraccSoapClient()

            sDebugInfo = SoapClient.Url


            Dim autentica As Boolean = SoapClient.Authenticate(user, passw)
            If Not autentica Then
                Throw New Exception("Autenticazione fallita")
            End If

            autentica = SoapClient.isAuthenticate()
            If Not autentica Then
                Throw New Exception("non è autenticato")
            End If

            Dim Dt As New DataTable
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            Dim strsql As String = ""

            If importaDaUltimoCaricati Then
                strsql = " SELECT MAX(ID) as ID FROM [dbo].[__Rintraccio_Ritiro]"
                Dt = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")
                If Dt.Rows.Count > 0 Then
                    startID = CInt(Dt.Rows(0).Item("ID")) + 1
                Else
                    startID = 0
                End If
            End If

            Dim count As Integer = 0
            Do


                Dt = LeggiRicevimenti(startID, limit, Importa, msg, objParametri_Server, ritorno, SoapClient, dp, strsql)

                count += Dt.Rows.Count

            Loop Until (Not iteratutto Or Dt.Rows.Count = 0)

            msg = "Letti " & count & " record da ws."


            Return ritorno

        Catch ex As Exception
            msg = "Eccezione: " & "[ws dedug info: " & sDebugInfo & "]" & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return False
        End Try


    End Function


    'Scorre dall'id minimo importato al massimo e se l'id non è presente
    'importa 100 record partendo da dall'id mancante meno 10 
    'salta poi il contatore
    Public Shared Function Importa_Carichi_Mancanti(
                                                         ByRef msg As String, _
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        'DataTable = Nothing

        msg = ""
        Dim user = "wsTracc"
        Dim passw = "ext-2013$.ws"

        Dim sDebugInfo As String = ""

        Try


            Dim ritorno As Boolean = True

            Dim SoapClient As New wstracc.wstracc
            Dim cc As New CookieContainer
            SoapClient.CookieContainer = cc
            'SoapClient = GetwstraccSoapClient()

            sDebugInfo = SoapClient.Url

            Dim autentica As Boolean = SoapClient.Authenticate(user, passw)
            If Not autentica Then
                Throw New Exception("Autenticazione fallita")
            End If

            autentica = SoapClient.isAuthenticate()
            If Not autentica Then
                Throw New Exception("non è autenticato")
            End If

            Dim Dt As New DataTable
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            Dim strsql As String = ""

            Dim minID As Integer = 0
            strsql = " SELECT min(ID) as ID FROM [dbo].[__Rintraccio_Ritiro]"
            Dt = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")
            If Dt.Rows.Count > 0 Then
                minID = CInt(Dt.Rows(0).Item("ID")) + 1
            Else
                minID = 0
            End If


            Dim finID As Integer = 0
            strsql = " SELECT max(ID) as ID FROM [dbo].[__Rintraccio_Ritiro]"
            Dt = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")
            If Dt.Rows.Count > 0 Then
                finID = CInt(Dt.Rows(0).Item("ID"))
            Else
                finID = 0
            End If

            For i = minID To finID

                Dim currID As Integer = 0
                strsql = " SELECT ID FROM [dbo].[__Rintraccio_Ritiro] WHERE ID = " & i
                Dt = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")
                If Dt.Rows.Count = 0 Then
                    'non c'è, imprto i 500 successivi
                    Dim limit As Integer = 100
                    Dt = LeggiRicevimenti(i - 10, limit, True, msg, objParametri_Server, ritorno, SoapClient, dp, strsql)
                    i = i - 10 + limit - 1
                Else
                    'c'è
                End If

            Next



            Return ritorno

        Catch ex As Exception
            msg = "Eccezione: " & "[ws dedug info: " & sDebugInfo & "]" & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return False
        End Try


    End Function

    Private Shared Function LeggiRicevimenti(ByRef startID As Integer, ByVal limit As Integer, ByVal Importa As Boolean, ByRef msg As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef ritorno As Boolean, ByVal SoapClient As wstracc.wstracc, ByVal dp As AgronicaCoreDataProvider.DataProvider, ByRef strsql As String) As DataTable
        Dim Dt As DataTable
        Dim ds As DataSet = SoapClient.GetCarichi(startID, limit)
        Dt = ds.Tables(0)

        If Importa Then

            For Each dr As DataRow In Dt.Rows

                If CInt(dr.Item("Anno")) >= 2014 Then

                    Dim Collo As Integer = -1
                    Dim strcol As String = dr.Item("Collo")
                    If IsNumeric(strcol) AndAlso strcol.Count < 8 AndAlso CDbl(strcol) = CInt(strcol) AndAlso CInt(strcol) > 0 Then
                        Collo = CInt(strcol)
                    End If

                    strsql = " DELETE FROM [dbo].[__Rintraccio_Ritiro] WHERE ID = " & dr.Item("ID")
                    dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")

                    strsql &= " INSERT INTO [dbo].[__Rintraccio_Ritiro]"
                    strsql &= "            ([ID]"
                    strsql &= "            ,[Anno]"
                    strsql &= "            ,[Centro]"
                    strsql &= "            ,[ID_Partita]"
                    strsql &= "            ,[Prog]"
                    strsql &= "            ,[Data]"
                    strsql &= "            ,[Grado]"
                    strsql &= "            ,[Tara]"
                    strsql &= "            ,[Kg]"
                    strsql &= "            ,[Colli]"
                    strsql &= "            ,[Collo]"
                    strsql &= "            ,[QSup]"
                    strsql &= "            ,[Sostanza_E1]"
                    strsql &= "            ,[Sostanza_E2]"
                    strsql &= "            ,[Sostanza_E3]"
                    strsql &= "            ,[Sostanza_E4]"
                    strsql &= "            ,[Sostanza_E5]"
                    strsql &= "            ,[Sostanza_E6]"
                    strsql &= "            ,[Caratteristica_1]"
                    strsql &= "            ,[Caratteristica_2]"
                    strsql &= "            ,[Barcode]"
                    strsql &= "            ,[ID_Gruppo_Varietale]"
                    strsql &= "            ,[Prog_Partita]"
                    strsql &= "            ,[CODICE_CUAA]"
                    strsql &= "            ,[QI])"
                    strsql &= "      VALUES"
                    strsql &= "            (" & dr.Item("ID") & " "
                    strsql &= "            ," & dr.Item("Anno") & " "
                    strsql &= "            ," & dr.Item("Centro") & " "
                    strsql &= "            ," & dr.Item("ID_Partita") & " "
                    strsql &= "            ," & dr.Item("Prog") & " "
                    strsql &= "            ," & Agro_SQL_SaveDate(dr.Item("Data")) & " "
                    strsql &= "            ,'" & Agro_SQL_SaveText(dr.Item("Grado")) & "' "
                    strsql &= "            ," & Agro_SQL_SaveNum(dr.Item("Tara")) & " "
                    strsql &= "            ," & Agro_SQL_SaveNum(dr.Item("Kg")) & " "
                    strsql &= "            ," & dr.Item("Colli") & " "
                    ' strsql &= "            ," & IIf(Not IsNumeric(dr.Item("Collo") Or dr.Item("Collo").ToString.Count > 8), "NULL", dr.Item("Collo")) & " "
                    strsql &= "            ," & Collo & " "
                    strsql &= "            ," & IIf(Agro_SQL_SaveNum(dr.Item("QSup")) = "", "NULL", Agro_SQL_SaveNum(dr.Item("QSup"))) & " "
                    strsql &= "            ," & IIf(IsDBNull(dr.Item("Sostanza_E1")), "NULL", "'" & Agro_SQL_SaveText(dr.Item("Sostanza_E1")) & "'") & " "
                    strsql &= "            ," & IIf(IsDBNull(dr.Item("Sostanza_E2")), "NULL", "'" & Agro_SQL_SaveText(dr.Item("Sostanza_E2")) & "'") & " "
                    strsql &= "            ," & IIf(IsDBNull(dr.Item("Sostanza_E3")), "NULL", "'" & Agro_SQL_SaveText(dr.Item("Sostanza_E3")) & "'") & " "
                    strsql &= "            ," & IIf(IsDBNull(dr.Item("Sostanza_E4")), "NULL", "'" & Agro_SQL_SaveText(dr.Item("Sostanza_E4")) & "'") & " "
                    strsql &= "            ," & IIf(IsDBNull(dr.Item("Sostanza_E5")), "NULL", "'" & Agro_SQL_SaveText(dr.Item("Sostanza_E5")) & "'") & " "
                    strsql &= "            ," & IIf(IsDBNull(dr.Item("Sostanza_E6")), "NULL", "'" & Agro_SQL_SaveText(dr.Item("Sostanza_E6")) & "'") & " "
                    strsql &= "            ," & IIf(IsDBNull(dr.Item("Caratteristica_1")), "NULL", "'" & Agro_SQL_SaveText(dr.Item("Caratteristica_1")) & "'") & " "
                    strsql &= "            ," & IIf(IsDBNull(dr.Item("Caratteristica_2")), "NULL", "'" & Agro_SQL_SaveText(dr.Item("Caratteristica_2")) & "'") & " "
                    strsql &= "            ,'" & Agro_SQL_SaveText(dr.Item("Barcode")) & "' "
                    strsql &= "            ," & dr.Item("ID_Gruppo_Varietale") & " "
                    strsql &= "            ," & Agro_SQL_SaveNum(dr.Item("Prog_Partita")) & " "
                    strsql &= "            ,'" & Agro_SQL_SaveText(dr.Item("CODICE_CUAA")) & "' "
                    strsql &= "            ," & Agro_SQL_SaveNum(dr.Item("QI")) & " )"

                    Try
                        Dim res2 As Boolean = dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")
                        If Not res2 Then
                            msg &= "---------errore scrittura in __Rintraccio_Ritiro ID  " & dr.Item("ID") & "  " & vbCrLf
                        End If
                    Catch ex As Exception
                        msg &= "--------errore scrittura in __Rintraccio_Ritiro ID  " & dr.Item("ID") & " : " & ex.Message & vbCrLf

                        ritorno = False

                    End Try


                    startID = dr.Item("ID") + 1

                End If

            Next
        Else
            startID += Dt.Rows.Count
        End If
        Return Dt
    End Function


    Private Shared Function LeggiLavorazione(ByRef startID As Integer, ByVal limit As Integer, ByVal Importa As Boolean, ByRef msg As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef ritorno As Boolean, ByVal SoapClient As wstracc.wstracc, ByVal dp As AgronicaCoreDataProvider.DataProvider, ByRef strsql As String) As DataTable
        Dim Dt As DataTable
        Dim ds As DataSet = SoapClient.GetLavorazione(startID, limit)


        Dt = ds.Tables(0)


        If Importa Then

            For Each dr As DataRow In Dt.Rows

                If CInt(dr.Item("Anno")) >= 2014 Then

                    strsql = " DELETE FROM [dbo].[__Rintraccio_Lavorazione] WHERE ID = " & dr.Item("ID")
                    dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")

                    strsql = " INSERT INTO [dbo].[__Rintraccio_Lavorazione]"
                    strsql &= "            ([ID]"
                    strsql &= "            ,[Anno]"
                    strsql &= "            ,[Centro]"
                    strsql &= "            ,[Prog]"
                    strsql &= "            ,[Data]"
                    strsql &= "            ,[Tipo_Mov]"
                    strsql &= "            ,[Qta]"
                    strsql &= "            ,[Centro_In]"
                    strsql &= "            ,[Grado_In]"
                    strsql &= "            ,[Anno_In]"
                    strsql &= "            ,[ID_Gruppo]"
                    strsql &= "            ,[Barcode]"
                    strsql &= "            ,[Grado_Out]"
                    strsql &= "     ) VALUES"
                    strsql &= "            (" & dr.Item("ID") & " "
                    strsql &= "            ," & dr.Item("Anno") & " "
                    strsql &= "            ," & dr.Item("Centro") & " "
                    strsql &= "            ," & dr.Item("Prog") & " "
                    strsql &= "            ," & Agro_SQL_SaveDateTime_NULL(dr.Item("Data")) & " "
                    strsql &= "            ," & Agro_SQL_SaveText_NULL(dr.Item("Tipo_Mov")) & " "
                    strsql &= "            ," & Agro_SQL_SaveNum_NULL(dr.Item("Qta")) & " "
                    strsql &= "            ," & Agro_SQL_SaveNum_NULL(dr.Item("Centro_In")) & " "
                    strsql &= "            ," & Agro_SQL_SaveText_NULL(dr.Item("Grado_In")) & " "
                    strsql &= "            ," & Agro_SQL_SaveNum_NULL(dr.Item("Anno_In")) & " "
                    strsql &= "            ," & Agro_SQL_SaveNum_NULL(dr.Item("ID_Gruppo")) & " "
                    strsql &= "            ," & Agro_SQL_SaveText_NULL(dr.Item("Barcode")) & " "
                    strsql &= "            ," & Agro_SQL_SaveText_NULL(dr.Item("Grado_Out")) & " "
                    strsql &= " )"

                    Try
                        Dim res2 As Boolean = dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport Lavorazione")
                        If Not res2 Then
                            msg &= "---------errore scrittura in __Rintraccio_Lavorazione ID  " & dr.Item("ID") & "  " & vbCrLf
                        End If
                    Catch ex As Exception
                        msg &= "--------errore scrittura in __Rintraccio_Lavorazione ID  " & dr.Item("ID") & " : " & ex.Message & vbCrLf

                        ritorno = False

                    End Try


                    startID = dr.Item("ID") + 1

                End If

            Next

        End If
        Return Dt
    End Function


    Private Shared Function LeggiProdottoFinito(ByRef startID As Integer, ByVal limit As Integer, ByVal Importa As Boolean, ByRef msg As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef ritorno As Boolean, ByVal SoapClient As wstracc.wstracc, ByVal dp As AgronicaCoreDataProvider.DataProvider, ByRef strsql As String) As DataTable
        Dim Dt As DataTable
        Dim ds As DataSet = SoapClient.GetProdFinito(startID, limit)
        Dt = ds.Tables(0)

        If Importa Then

            For Each dr As DataRow In Dt.Rows

                If CInt(dr.Item("Anno")) >= 2014 Then

                    strsql = " DELETE FROM [dbo].[__Rintraccio_ProdottoFinito] WHERE ID = " & dr.Item("ID")
                    dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")

                    strsql = " INSERT INTO [dbo].[__Rintraccio_ProdottoFinito]"
                    strsql &= "            ([ID]"
                    strsql &= "            ,[Anno]"
                    strsql &= "            ,[Centro]"
                    strsql &= "            ,[Data]"
                    strsql &= "            ,[Grado_Out]"
                    strsql &= "            ,[Grado]"
                    strsql &= "            ,[Prog]"
                    strsql &= "            ,[Qta]"
                    strsql &= "            ,[CS]"
                    strsql &= "            ,[Tara]"
                    strsql &= "            ,[Barcode]"
                    strsql &= "            ,[Rimanenza]"
                    strsql &= "            ,[Tipo]"
                    strsql &= "    )  VALUES"
                    strsql &= "            (" & dr.Item("ID") & " "
                    strsql &= "            ," & dr.Item("Anno") & " "
                    strsql &= "            ," & dr.Item("Centro") & " "
                    strsql &= "            ," & Agro_SQL_SaveDateTime_NULL(dr.Item("Data")) & " "
                    strsql &= "            ," & Agro_SQL_SaveText_NULL(dr.Item("Grado_Out")) & " "
                    strsql &= "            ," & Agro_SQL_SaveText_NULL(dr.Item("Grado")) & " "
                    strsql &= "            ," & Agro_SQL_SaveNum_NULL(dr.Item("Prog")) & " "
                    strsql &= "            ," & Agro_SQL_SaveNum_NULL(dr.Item("Qta")) & " "
                    strsql &= "            ," & Agro_SQL_SaveText_NULL(dr.Item("CS")) & " "
                    strsql &= "            ," & Agro_SQL_SaveNum_NULL(dr.Item("Tara")) & " "
                    strsql &= "            ," & Agro_SQL_SaveText_NULL(dr.Item("Barcode")) & " "
                    strsql &= "            ," & Agro_SQL_SaveBoolStrToInt_NULL(dr.Item("Rimanenza")) & " "
                    strsql &= "            ," & Agro_SQL_SaveText_NULL(dr.Item("Tipo")) & " "
                    strsql &= "  )"

                    Try
                        Dim res2 As Boolean = dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")
                        If Not res2 Then
                            msg &= "---------errore scrittura in __Rintraccio_Lavorazione ID  " & dr.Item("ID") & "  " & vbCrLf
                        End If
                    Catch ex As Exception
                        msg &= "--------errore scrittura in __Rintraccio_Lavorazione ID  " & dr.Item("ID") & " : " & ex.Message & vbCrLf

                        ritorno = False

                    End Try


                    startID = dr.Item("ID") + 1

                End If

            Next

        End If
        Return Dt
    End Function

    'Private Shared Function GetwstraccSoapClient() As WS_Opta_Tracciabilita.wstraccSoapClient


    '    ' <endpoint address="https://tti.ttitalia.it:92/wstracc.asmx"
    '    '       binding="basicHttpBinding" bindingConfiguration="wstraccSoap"
    '    '       contract="WS_Opta_Tracciabilita.wstraccSoap" name="wstraccSoap" />
    '    '           <binding name="wstraccSoap">
    '    '        <security mode="Transport" />
    '    '</binding>
    '    Dim url As String = "https://tti.ttitalia.it:92/wstracc.asmx"
    '    Dim binding As New System.ServiceModel.BasicHttpBinding()

    '    Dim EndpointAddress As New System.ServiceModel.EndpointAddress(url)


    '    binding.Security.Mode = System.ServiceModel.SecurityMode.Transport
    '    binding.Name = "wstraccSoap"
    '    binding.AllowCookies = True

    '    Dim wstraccSoapClient As New WS_Opta_Tracciabilita.wstraccSoapClient(binding, EndpointAddress)


    '    Return wstraccSoapClient

    'End Function

End Class