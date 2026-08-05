

'Imports <xmlns="http://grafica_old">
Imports <xmlns="http://grafica_new">

Imports AgronicaConversioneCartografiaGIAS.FormatsConverter
Imports AgronicaConversioneCartografiaGIAS.Agronica
Imports System.Configuration.ConfigurationManager
Imports AgronicaGIS2012.Commons
Imports AgronicaGIS2012.Commons.DataOraHelper
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Gias2Gias_LIB

Imports System.Web

Imports System.Text

Public Class ConvertiVecchioNuovo

    Sub New()

    End Sub

    ''' <summary>
    ''' Importazione della grafica di 1 centro aziendale.
    ''' </summary>
    ''' <param name="oldXmlGrafica"></param>
    ''' <param name="pivasuperuser"></param>
    ''' <param name="PivaPadre"></param>
    ''' <param name="piva"></param>
    ''' <param name="sa_cod"></param>
    ''' <param name="objOpzioni"></param>
    ''' <param name="tipoEntita">Tipo di entità nel vecchio formato (I = impianti, ecc..)</param>
    ''' <param name="swapLatLong">scambia i valori di lat e long (X,Y se ED50)</param>
    ''' <param name="ImportaLayerSpecieDaImpianto"></param>
    ''' <param name="GEORiferimento_COD"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConvertiXml(ByVal oldXmlGrafica As String, ByVal pivasuperuser As String, ByVal PivaPadre As String, piva As String, sa_cod As Integer, objOpzioni As clsOpzioni, ByVal LayertipoEntita As String, ByVal swapLatLong As Boolean, ByVal ImportaLayerSpecieDaImpianto As Boolean, ByVal GEORiferimento_COD As Integer, ByVal ParametriCartografici As ParametriCoordinateConverter, ByVal objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim cconverter As New Agronica.CoordinateConverter
        Dim ltipoEntita_vecchio As String = LayertipoEntita.Split("|")(0)
        Dim lLayer_nuovo As String = LayertipoEntita.Split("|")(1)

        Dim rval As String = ""

        Dim listaOggetti As New List(Of EntitaGias)


        Dim xDocOld As XDocument = ReadXmlFromString(oldXmlGrafica)


        Dim entitaEsistente As EntitaGias
        Dim id As String = ""

        Dim RecuperatoCodiceGias As Boolean = False
        Dim veg_cod As String = ""
        Dim gru_cod As Integer = 0


        Dim appezza As Long = 0
        Dim regImpianto As Long = 0
        Dim campo_cod As Integer = 0

        Dim Programmazione_Cod As Integer = 0
        Dim Programmazione_Entita_Cod As Integer = 0


        Dim PROV As String = ""
        Dim COM As String = ""
        Dim SEZIONE As String = ""
        Dim FOGLIO As String = ""
        Dim NUMERO As String = ""
        Dim SUBALTERNO As String = ""



        Dim oLeggiDatiImpianto As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R

        Dim baseCode As Integer = GetBasecod(sa_cod)



        'trasformazione


        Dim conteggio As Integer = 0
        Dim totale As Integer =
            (From OldentitaCorrente In xDocOld.Elements("DatiEntita").Elements("Entita")
             Where OldentitaCorrente.@id.StartsWith(ltipoEntita_vecchio) AndAlso
                   OldentitaCorrente.@descr.StartsWith("VERTEX")
             Order By CInt(OldentitaCorrente.@descr.Replace("VERTEX    ", "")) Descending
             Select OldentitaCorrente).ToList.Count


        'entita poligoni
        For Each entita In
            (From OldentitaCorrente In xDocOld.Elements("DatiEntita").Elements("Entita")
             Where OldentitaCorrente.@id.StartsWith(ltipoEntita_vecchio) AndAlso
                   OldentitaCorrente.@descr.StartsWith("VERTEX")
             Order By CInt(OldentitaCorrente.@descr.Replace("VERTEX    ", "")) Descending
             Select OldentitaCorrente).ToList


            conteggio += 1
            System.Web.HttpContext.Current.Session("conteggio") = conteggio & " di " & totale


            id = entita.@id
            entitaEsistente = Nothing

            RecuperatoCodiceGias = False
            veg_cod = ""


            Dim lPrefissoAccessorio As String
            lPrefissoAccessorio = AgronicaGIS2012.Commons.VecchioFormatoHelper.lDammiPrefissoAccessorio(ltipoEntita_vecchio)

            Dim DatiTestata = (
             From o In xDocOld.Elements("DatiEntita").Elements("Entita")
             Where o.@id.StartsWith(lPrefissoAccessorio) AndAlso
                   o.@id.Replace(lPrefissoAccessorio, ltipoEntita_vecchio) = id
             Select o).FirstOrDefault




            entitaEsistente =
                (From o In listaOggetti
                 Where o.CodiceGias = id
                 ).FirstOrDefault


            If entitaEsistente Is Nothing Then
                resettachiavi(appezza, regImpianto, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO)


                Select Case ltipoEntita_vecchio

                    Case "A"
                        'appezzamenti (scritti in formato A00000001, con base code incorporato
                        appezza = Long.Parse(id.Substring(1, 8), System.Globalization.NumberStyles.HexNumber)

                    Case "I" 'impianti 
                        '(scritti in formato I00010001, dove i primi 4 numeri sono appezza, gli altri 4 impianti, bisogna aggiungere il base cod)
                        appezza = Long.Parse(id.Substring(1, 4), System.Globalization.NumberStyles.HexNumber)
                        regImpianto = Long.Parse(id.Substring(5, 4), System.Globalization.NumberStyles.HexNumber)
                        appezza += baseCode
                        regImpianto += baseCode

                    Case "D"
                        'Particelle catastali
                        ParticelleCatastali(PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, entita)
                    Case "G"

                        Programmazione_Entita_Cod = Long.Parse(id.Substring(1, 8), System.Globalization.NumberStyles.HexNumber)

                        Dim xLeggiProgrammazione_cod As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
                        Dim dtLeggiProgrammazione_cod As DataTable = xLeggiProgrammazione_cod.Leggi_PlanningAppartenenza_Distinct(Programmazione_Entita_Cod, False, 1, "", objParametri_server)

                        If dtLeggiProgrammazione_cod.Rows.Count = 0 Then
                            Throw New Exception("Errore in fase di importazione dati, a fronte dell'entità con codice" & Programmazione_Cod & " non è stato trovato il planning di appartentenza.")
                        Else
                            Programmazione_Cod = dtLeggiProgrammazione_cod.Rows(0)("Programmazione_Cod")
                        End If

                    Case "E"
                        'testi associati ad appezzamenti o liberi

                    Case Else

                End Select




                If Not RecuperatoCodiceGias AndAlso ltipoEntita_vecchio = "I" AndAlso ImportaLayerSpecieDaImpianto Then

                    getDatiImpianto(piva, sa_cod, objOpzioni, veg_cod, gru_cod, appezza, regImpianto)
                    RecuperatoCodiceGias = True

                End If


                Dim lNuovoTipoEntita_Cod As String = "0"

                DammiNuovoTipoEntita(ltipoEntita_vecchio, lLayer_nuovo, lNuovoTipoEntita_Cod, gru_cod)

                entitaEsistente = New EntitaGias With {
                    .CodiceGias = id,
                    .Text = If(DatiTestata Is Nothing, "", DatiTestata.@text),
                    .Descr = If(DatiTestata Is Nothing, "", DatiTestata.@descr),
                    .Layer = If(DatiTestata Is Nothing, entita.@layer, DatiTestata.@layer),
                    .Data_Creazione = entita.@data_creazione,
                    .Data_Modifica = entita.@data_modifica,
                    .Entita_Cod = entita.@entita_cod,
                    .ElementoGrafico_cod = entita.@elementografico_cod,
                    .TipoOperazioneDB = entita.@TipoOperazioneDB,
                    .Analisi_campione_cod = entita.@analisi_campione_cod,
                    .Id_agenda = entita.@id_agenda,
                    .Ricetta_Operazione_Cod = entita.@ricetta_operazione_cod,
                    .Appezza = appezza,
                    .Campo_cod = campo_cod,
                    .RegImpianto = regImpianto,
                    .PROV = PROV,
                    .COM = COM,
                    .SEZIONE = SEZIONE,
                    .FOGLIO = FOGLIO,
                    .NUMERO = NUMERO,
                    .SUBALTERNO = SUBALTERNO,
                    .NuovoTipoEntita = lNuovoTipoEntita_Cod,
                    .Programmazione_Entita_Cod = Programmazione_Entita_Cod,
                    .Programmazione_cod = Programmazione_Cod
                }

            End If 'entitaEsistente is nothing




            If entita.@lat <> "" AndAlso entita.@lat <> "0" Then
                entitaEsistente.DatiCartograficiOriginali_WGS84.Add(New xyz With {
                        .X = CDbl(entita.@lon),
                        .Y = CDbl(entita.@lat)
                    })
                entitaEsistente.DatoConvertito = 1
            Else
                entitaEsistente.DatiCartograficiED50.Add(New xyz With {
                        .X = CDbl(entita.@vx1),
                        .Y = CDbl(entita.@vy1)
                    })
                entitaEsistente.DatoConvertito = 2

            End If


            Dim OggettoAggiuntoInLista As Boolean = False
            OggettoAggiuntoInLista = (
                From e In listaOggetti
                Where e.CodiceGias = id).ToList.Count > 0


            If Not OggettoAggiuntoInLista Then
                listaOggetti.Add(entitaEsistente)
            End If


        Next 'fine lettura vecchie entità poligoni



        'lettura entita punti
        For Each entita In
            (From OldentitaCorrente In xDocOld.Elements("DatiEntita").Elements("Entita")
             Where OldentitaCorrente.@id.StartsWith(ltipoEntita_vecchio) _
             AndAlso (
                 OldentitaCorrente.@descr.StartsWith("TEXT") OrElse
                 OldentitaCorrente.@descr.StartsWith("CIRCLE")
                 )
             Select OldentitaCorrente).ToList


            conteggio += 1
            System.Web.HttpContext.Current.Session("conteggio") = conteggio & " di " & totale


            id = entita.@id
            entitaEsistente = Nothing

            RecuperatoCodiceGias = False
            veg_cod = ""


            Dim lPrefissoAccessorio As String
            lPrefissoAccessorio = AgronicaGIS2012.Commons.VecchioFormatoHelper.lDammiPrefissoAccessorio(ltipoEntita_vecchio)


            entitaEsistente =
                (From o In listaOggetti
                 Where o.CodiceGias = id
                 ).FirstOrDefault


            If entitaEsistente Is Nothing Then
                resettachiavi(appezza, regImpianto, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO)


                Select Case ltipoEntita_vecchio

                    Case "T"
                        'appezzamenti (scritti in formato X00000001, con base code incorporato
                        appezza = Long.Parse(id.Substring(1, 8), System.Globalization.NumberStyles.HexNumber)

                    Case Else

                End Select



                Dim lNuovoTipoEntita_Cod As String = "0"

                DammiNuovoTipoEntita(ltipoEntita_vecchio, lLayer_nuovo, lNuovoTipoEntita_Cod, gru_cod)

                entitaEsistente = New EntitaGias With {
                    .CodiceGias = id,
                    .Text = entita.@text,
                    .Descr = entita.@descr,
                    .Layer = entita.@layer,
                    .Data_Creazione = entita.@data_creazione,
                    .Data_Modifica = entita.@data_modifica,
                    .Entita_Cod = entita.@entita_cod,
                    .ElementoGrafico_cod = entita.@elementografico_cod,
                    .TipoOperazioneDB = entita.@TipoOperazioneDB,
                    .Analisi_campione_cod = entita.@analisi_campione_cod,
                    .Id_agenda = entita.@id_agenda,
                    .Ricetta_Operazione_Cod = entita.@ricetta_operazione_cod,
                    .Appezza = appezza,
                    .Campo_cod = campo_cod,
                    .RegImpianto = regImpianto,
                    .PROV = PROV,
                    .COM = COM,
                    .SEZIONE = SEZIONE,
                    .FOGLIO = FOGLIO,
                    .NUMERO = NUMERO,
                    .SUBALTERNO = SUBALTERNO,
                    .NuovoTipoEntita = lNuovoTipoEntita_Cod,
                    .Programmazione_Entita_Cod = Programmazione_Entita_Cod,
                    .Programmazione_cod = Programmazione_Cod
                }

            End If 'entitaEsistente is nothing




            If entita.@lat <> "" AndAlso entita.@lat <> "0" Then
                entitaEsistente.DatiCartograficiOriginali_WGS84.Add(New xyz With {
                        .X = CDbl(entita.@lon),
                        .Y = CDbl(entita.@lat)
                    })
                entitaEsistente.DatoConvertito = 1
            Else
                entitaEsistente.DatiCartograficiED50.Add(New xyz With {
                        .X = CDbl(entita.@vx1),
                        .Y = CDbl(entita.@vy1)
                    })
                entitaEsistente.DatoConvertito = 2

            End If


            Dim OggettoAggiuntoInLista As Boolean = False
            OggettoAggiuntoInLista = (
                From e In listaOggetti
                Where e.CodiceGias = id).ToList.Count > 0


            If Not OggettoAggiuntoInLista Then
                listaOggetti.Add(entitaEsistente)
            End If


        Next 'fine lettura vecchie entità (punti)



        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml



        Dim ElemFinalXdoc As XElement = <DatiEntita xmlns="http://www.agronica.it/grafica/"
                                            xmlns:gml="http://www.opengis.net/gml"></DatiEntita>
        Dim FinalDoc As New XDocument
        FinalDoc.Add(ElemFinalXdoc)


        For Each EntitaDaTrasformare In listaOggetti
            Dim sNodeDoc As XDocument

            If EntitaDaTrasformare.DatiCartograficiOriginali_WGS84.Count > 0 Then
                sNodeDoc = ReadXmlFromString(
                wktToGeoML.Trasforma(
                    wktHelp.CreaPoligonoDaCoordinate(
                        EntitaDaTrasformare.DatiCartograficiOriginali_WGS84,
                        (EntitaDaTrasformare.DatiCartograficiOriginali_WGS84.Count = 1)
                    ),
                    swapLatLong,
                    False,
                    True,
                    EntitaDaTrasformare.ElementoGrafico_cod)
            )
                sNodeDoc.Root.@Flag_GPS = 1
            Else

                sNodeDoc = ReadXmlFromString(
                wktToGeoML.Trasforma(
                    cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wktHelp.CreaPoligonoDaCoordinate(EntitaDaTrasformare.DatiCartograficiED50), Not swapLatLong, ParametriCartografici),
                    swapLatLong,
                    False,
                    True,
                    EntitaDaTrasformare.ElementoGrafico_cod)
            )

                sNodeDoc.Root.@Flag_GPS = 0
            End If





            Dim newEntitaElement = <Entita TipoOperazioneDB=<%= EntitaDaTrasformare.TipoOperazioneDB %> recno="" deleted="" section="E" id=<%= EntitaDaTrasformare.CodiceGias %> descr=<%= EntitaDaTrasformare.Descr %> ecolor="256" eline="256" rad="15" text=<%= EntitaDaTrasformare.Text %> gps="1" lat="0" lon="0" pdop="0" validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="">
                                       <layers>
                                           <layer tipologia_layer="1"><%= lLayer_nuovo %></layer>
                                           <layer tipologia_layer="10"><%= EntitaDaTrasformare.DatoConvertito %></layer>
                                           <layer tipologia_layer="100"><%= PivaPadre %></layer>
                                           <layer tipologia_layer="5"><%= veg_cod %></layer>
                                       </layers>
                                       <EntitaGIAS>
                                           <DatoGias>
                                               <PivaSuperUser><%= pivasuperuser %></PivaSuperUser>
                                               <Entita_Cod><%= EntitaDaTrasformare.Entita_Cod %></Entita_Cod>
                                               <TipoEntita_Cod><%= EntitaDaTrasformare.NuovoTipoEntita %></TipoEntita_Cod>
                                               <Piva><%= piva %></Piva>
                                               <Sa_Cod><%= sa_cod %></Sa_Cod>
                                               <Appezza><%= EntitaDaTrasformare.Appezza %></Appezza>
                                               <Campo_Cod><%= EntitaDaTrasformare.Campo_cod %></Campo_Cod>
                                               <Id_Imp><%= EntitaDaTrasformare.RegImpianto %></Id_Imp>
                                               <PROV><%= EntitaDaTrasformare.PROV %></PROV>
                                               <COM><%= EntitaDaTrasformare.COM %></COM>
                                               <SEZIONE><%= EntitaDaTrasformare.SEZIONE %></SEZIONE>
                                               <FOGLIO><%= EntitaDaTrasformare.FOGLIO %></FOGLIO>
                                               <NUMERO><%= EntitaDaTrasformare.NUMERO %></NUMERO>
                                               <SUBALTERNO><%= EntitaDaTrasformare.SUBALTERNO %></SUBALTERNO>
                                               <Programmazione_Entita_Cod><%= EntitaDaTrasformare.Programmazione_Entita_Cod %></Programmazione_Entita_Cod>
                                               <Programmazione_Cod><%= EntitaDaTrasformare.Programmazione_cod %></Programmazione_Cod>
                                               <Id_Agenda><%= EntitaDaTrasformare.Id_agenda %></Id_Agenda>
                                               <Ricetta_Operazione_Cod><%= EntitaDaTrasformare.Ricetta_Operazione_Cod %></Ricetta_Operazione_Cod>
                                               <OLDGrafica_ID><%= EntitaDaTrasformare.CodiceGias %></OLDGrafica_ID>
                                               <analisi_campione_cod><%= EntitaDaTrasformare.Analisi_campione_cod %></analisi_campione_cod>
                                               <inviato>0</inviato>
                                               <Data_Creazione><%= GetData_Creazione(EntitaDaTrasformare.Data_Modifica) %></Data_Creazione>
                                               <Data_Modifica><%= GetData_Creazione(EntitaDaTrasformare.Data_Modifica) %></Data_Modifica>
                                               <Username_Creazione>agronica</Username_Creazione>
                                               <Username_Modifica>agronica</Username_Modifica>
                                               <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                               <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                           </DatoGias>
                                       </EntitaGIAS>
                                   </Entita>

            newEntitaElement.Add(sNodeDoc.FirstNode)
            FinalDoc.Root.Add(newEntitaElement)

        Next 'fine creazione nuovo oggetto grafico

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")

        rval = xmlHelper.RemoveNamespace(FinalDoc, ListaNS).ToString.Replace("xmlns=""""", "")
        Return rval

    End Function


    Private Shared Sub getDatiCampioniAnalisi(ByVal piva As String, ByVal sa_cod As Integer, ByVal objOpzioni As clsOpzioni, ByRef veg_cod As String, ByRef gru_cod As Integer, ByVal appezza As Long, ByVal regImpianto As Long)
        Dim stb As New StringBuilder

        stb.Append("select top 1 veg_cod, coalesce(Gru_Cod, 0) as Gru_Cod " & vbCrLf)
        stb.Append(" from specievegetali  " & vbCrLf)
        stb.Append(" where veg_cod in ( " & vbCrLf)
        stb.Append("    select Veg_Cod  " & vbCrLf)
        stb.Append("    from Cultivar  " & vbCrLf)
        stb.Append("    where Cul_Cod in ( " & vbCrLf)
        stb.Append("        select cul_cod " & vbCrLf)
        stb.Append("        from reg_impianti " & vbCrLf)
        stb.Append("        where 1=1 " & vbCrLf)
        stb.Append("        AND     Piva = '" & piva & "'   " & vbCrLf)
        stb.Append("        AND Sa_Cod = " & sa_cod & "   " & vbCrLf)
        stb.Append("        AND Appezza = " & appezza & "   " & vbCrLf)
        stb.Append("        AND Id_Reg = " & regImpianto & "    " & vbCrLf)
        stb.Append("    ) " & vbCrLf)
        stb.Append(" ) " & vbCrLf)

        Dim appRLeggi As New AgronicaCoreDataProvider.DataProvider
        Dim dt As New DataTable
        dt = appRLeggi.EseguiQuery_Lettura(objOpzioni.objParametri_Server_GIAS_ORIGINE, _
                stb.ToString, _
                "" _
            )

        If dt.Rows.Count > 0 Then
            veg_cod = dt(0)(0)
            gru_cod = dt(0)(1)
        Else
            veg_cod = "-1"
            gru_cod = 0
        End If
    End Sub


    Private Shared Sub getDatiImpianto(ByVal piva As String, ByVal sa_cod As Integer, ByVal objOpzioni As clsOpzioni, ByRef veg_cod As String, ByRef gru_cod As Integer, ByVal appezza As Long, ByVal regImpianto As Long)
        Dim stb As New StringBuilder

        stb.Append("select top 1 veg_cod, coalesce(Gru_Cod, 0) as Gru_Cod " & vbCrLf)
        stb.Append(" from specievegetali  " & vbCrLf)
        stb.Append(" where veg_cod in ( " & vbCrLf)
        stb.Append("    select Veg_Cod  " & vbCrLf)
        stb.Append("    from Cultivar  " & vbCrLf)
        stb.Append("    where Cul_Cod in ( " & vbCrLf)
        stb.Append("        select cul_cod " & vbCrLf)
        stb.Append("        from reg_impianti " & vbCrLf)
        stb.Append("        where 1=1 " & vbCrLf)
        stb.Append("        AND     Piva = '" & piva & "'   " & vbCrLf)
        stb.Append("        AND Sa_Cod = " & sa_cod & "   " & vbCrLf)
        stb.Append("        AND Appezza = " & appezza & "   " & vbCrLf)
        stb.Append("        AND Id_Reg = " & regImpianto & "    " & vbCrLf)
        stb.Append("    ) " & vbCrLf)
        stb.Append(" ) " & vbCrLf)

        Dim appRLeggi As New AgronicaCoreDataProvider.DataProvider
        Dim dt As New DataTable
        dt = appRLeggi.EseguiQuery_Lettura(objOpzioni.objParametri_Server_GIAS_ORIGINE, _
                stb.ToString, _
                "" _
            )

        If dt.Rows.Count > 0 Then
            veg_cod = dt(0)(0)
            gru_cod = dt(0)(1)
        Else
            veg_cod = "-1"
            gru_cod = 0
        End If
    End Sub
    Private Shared Sub resettachiavi(ByRef appezza As Long, ByRef regImpianto As Long, ByRef PROV As String, ByRef COM As String, ByRef SEZIONE As String, ByRef FOGLIO As String, ByRef NUMERO As String, ByRef SUBALTERNO As String)

        appezza = 0
        regImpianto = 0
        PROV = "0"
        COM = "0"
        SEZIONE = "0"
        FOGLIO = "0"
        NUMERO = "0"
        SUBALTERNO = "0"
    End Sub
    Private Shared Function GetData_Creazione(ByVal dataCreazione As DateTime) As String
        Return CreateISO8601DateTimeFromSystemDateTime(dataCreazione)
    End Function




    Private Shared Sub ParticelleCatastali(ByRef PROV As String, ByRef COM As String, ByRef SEZIONE As String, ByRef FOGLIO As String, ByRef NUMERO As String, ByRef SUBALTERNO As String, ByVal entita As XElement)


        PROV = entita.@prov
        COM = entita.@com
        SEZIONE = entita.@sezione
        FOGLIO = entita.@foglio
        NUMERO = entita.@numero
        SUBALTERNO = entita.@subalterno

        If PROV = "" Then
            PROV = "0"
        End If

        If COM = "" Then
            COM = "0"
        End If

        If SEZIONE = "" Then
            SEZIONE = "0"
        End If

        If FOGLIO = "" Then
            FOGLIO = "0"
        End If

        If NUMERO = "" Then
            NUMERO = "0"
        End If

        If SUBALTERNO = "" Then
            SUBALTERNO = "0"
        End If
    End Sub
    Private Shared Sub DammiNuovoTipoEntita(ByVal ltipoEntita_vecchio As String, ByVal tipoEntita As String, ByRef lTipoEntita_Cod As String, ByVal gru_cod As Integer)

        If ltipoEntita_vecchio = "G" Then
            lTipoEntita_Cod = 53
            Exit Sub
        End If

        If ltipoEntita_vecchio <> "I" Then
            lTipoEntita_Cod = tipoEntita
            Exit Sub
        End If


        Select Case gru_cod
            Case 0
                lTipoEntita_Cod = 21
            Case 1
                lTipoEntita_Cod = 22
            Case 2
                lTipoEntita_Cod = 20
            Case 3
                lTipoEntita_Cod = 19
        End Select
    End Sub
    Private Shared Function GetBasecod(ByVal Sa_cod As Integer) As Integer

        Return (Sa_cod \ (2 ^ 17)) * (2 ^ 17)

    End Function
    Private Function ReadXmlFromString(ByVal stringaXml As String) As XDocument
        Return XDocument.Parse(stringaXml)
    End Function

End Class
