Imports System.Collections.ObjectModel
Imports AgronicaCoreDataProvider
Imports AgronicaControlliGIS
Imports AgronicaSHPWrapper

Public Class EzGuideToGias2012


    Private _ListaAziende As New List(Of ezAziende)
    Public Property ListaAziende() As List(Of ezAziende)
        Get
            Return _ListaAziende
        End Get
        Set(value As List(Of ezAziende))
            _ListaAziende = value
        End Set
    End Property

    Private Sub AggiungiAzienda(ByVal a As ezAziende)
        _ListaAziende.Add(a)
    End Sub


    Private Sub settaPunti(ByVal v As String(), ByVal lat As String, ByVal Lon As String, ByRef pos As String(), ByVal ForzaLetturaPrimaCoordinata As Boolean)

        If ForzaLetturaPrimaCoordinata Then
            pos(0) = v(0)
            pos(1) = v(1)
            Exit Sub
        End If

        For Each cD In v

            If dStartWith(cD, lat) Then
                pos(0) = cD
            End If

            If dStartWith(cD, Lon) Then
                pos(1) = cD
            End If

        Next
    End Sub


    Private Sub settaLaLb(ByVal v As String(), ByVal pos As String(), ByRef hiddenPunti_A As String, ByRef hiddenPunti_B As String)
        hiddenPunti_A = "(" & pos(0) & ", " & pos(1) & ")"
        hiddenPunti_B = "(" & ridammiVet(v, " ").Replace(pos(0) & " " & pos(1), "").Trim(" ").Replace(" ", ", ") & ")"
    End Sub


    Private Sub EstraiPuntiDaSegmentoNonAssociatiAPunti(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal Codice_Fiscale_Tecnico As String)

        Dim leggi As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim X As String
        Dim streerore As String = "Ok"
        Dim risp As Boolean
        Dim planning1Impianti2 As Integer = 2

        For Each ppp In _FilePost

            Dim lPppSplit As String() = ppp.Split("|")
            Dim appGias As String() = lPppSplit(1).Split("\")


            Dim piva As String = appGias(0)
            Dim sa_cod, campo_cod, appezza, reg_impianto, programmazione_cod As Integer
            sa_cod = appGias(1)
            campo_cod = 0
            appezza = appGias(2)
            reg_impianto = appGias(3)
            programmazione_cod = appGias(4)

            If programmazione_cod <> 0 Then
                planning1Impianti2 = 1
            End If

            X = leggi.LeggiXML(objParametri.PivaSuperUser, 0, 55, "", piva, sa_cod, appezza, campo_cod, reg_impianto, "", "", "-1", -1, -1, "-1", 0, 0, programmazione_cod, 0, "-1", Codice_Fiscale_Tecnico, "", Nothing, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri, objParametri_Utenti)

            Dim xDoc As XDocument = XDocument.Parse(X)


            Dim v As String() = AgronicaConversioneCartografiaGias.FormatsConverter.GML.DammiArrayCoordinateDatoXml(X)

            Dim pos(2) As String


            Dim lat As String = ""
            Dim Lon As String = ""
            Dim hiddenPunti_A As String = ""
            Dim hiddenPunti_B As String = ""

            latlon(lPppSplit(0), lat, Lon)


            settaPunti(v, lat, Lon, pos, False)
            settaLaLb(v, pos, hiddenPunti_A, hiddenPunti_B)

            If Not (hiddenPunti_A.Split(",").Count = 2 AndAlso hiddenPunti_B.Split(",").Count = 2) Then
                settaPunti(v, lat, Lon, pos, True)
                settaLaLb(v, pos, hiddenPunti_A, hiddenPunti_B)
            End If

            Dim objImpianto As New AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo
            objImpianto.Piva = piva
            objImpianto.Sa_Cod = sa_cod
            objImpianto.Appezza = appezza
            objImpianto.Id_reg = reg_impianto

            'salvare! todo, verificare agenda
            streerore = V_M.SalvaGrafica(New InSalvaGraficaModel(piva, sa_cod, 0, hiddenPunti_A, programmazione_cod, 0, 55, objImpianto, "A", 55, 0, 0, 0), risp, objParametri, TipiEnumerativi.enum_TipoOperazioneDB.Scrittura)
            streerore &= V_M.SalvaGrafica(New InSalvaGraficaModel(piva, sa_cod, 0, hiddenPunti_B, programmazione_cod, 0, 55, objImpianto, "B", 55, 0, 0, 0), risp, objParametri, TipiEnumerativi.enum_TipoOperazioneDB.Scrittura)


        Next



        Dim aPF As New AgronicaCoreGisDAL.PrecisionFarming
        aPF.RimuoviSegmentiInutilizzati(planning1Impianti2, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

    End Sub

    Private Function dStartWith(ByVal cD As String, ByVal lon As String) As Boolean
        Return Math.Round(AgronicaGIS2012.Commons.xyz.myCDBL(cD), 5).ToString.Replace(",", ".").StartsWith(lon)
    End Function

    Private Function ridammiVet(ByVal v As String(), ByVal trC As String) As String
        Dim rv As String = ""
        For Each c In v
            rv &= c & trC
        Next

        Return rv.TrimEnd(trC)

    End Function

    Private Sub latlon(ByVal pos As String, ByRef lat As String, ByRef lon As String)

        '12.15377E44.49391N41H
        '12.15377,44.49391,41

        pos = pos.Replace("N", ",").Replace("E", ",").Replace("H", "")
        Dim pp As String() = pos.Split(",")

        lat = pp(1)
        lon = pp(0)


    End Sub

    Public Function Importa(ByVal CriteriMatch As String, ByVal folder As String, ByVal progressivoGias As Integer,
                            ByVal objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal objparametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Codice_Fiscale_Tecnico As String, ByVal ckDeduciDaposizione As Boolean) As String

        'esempio criteri match

        'codice ez ^ codice gias, piva, sa_cod, campo_cod, appezza, id_reg ^ 001 | ecc. | ecc.
        'dove 001 sta per i flag di cosa importare ...

        'AGRISFERA°CARLINA°11A_EZ56072^00085770394°68026494°68026373°app°id_reg|AGRISFERA°CARLINA°11A_EZ56072^00085770394°68026494°68026373°app°id_reg

        CriteriMatch = CriteriMatch.Replace("°", "\")

        folder &= "\AgGPS\Data"

        Dim files As ReadOnlyCollection(Of String)
        files = My.Computer.FileSystem.GetFiles(folder, FileIO.SearchOption.SearchAllSubDirectories, "*.shp")

        For Each f In files
            CaricaShp(folder, f, CriteriMatch, objparametri_Server, objparametri_Utenti, progressivoGias, Codice_Fiscale_Tecnico)
        Next

        EstraiPuntiDaSegmentoNonAssociatiAPunti(objparametri_Server, objparametri_Utenti, Codice_Fiscale_Tecnico)

        If ckDeduciDaposizione Then

            Try
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objparametri_Server)
                Dim AssegnaOperazioni As New AgronicaCoreGisDAL.GIS_Entita_W
                AssegnaOperazioni.AssegnaOperazioniPF_Impianto(
                    objparametri_Server.PivaSuperUser,
                    "",
                    0,
                    0,
                    "",
                    "",
                    objparametri_Server
                )
                G2G_Chiusura_Transazione(1, objparametri_Server)
            Catch ex As Exception
                G2G_Chiusura_Transazione(2, objparametri_Server)
            End Try

        End If

        Return "Ok"
    End Function


    Private Sub G2G_Chiusura_Transazione( _
                                ByVal Flag_Commit1_Rollback2 As Integer _
                                , ByVal objParametri_Server As AgronicaCoreParametri
                                )

        Dim NomeRoutine As String = "G2G_Chiusura_Transazione"

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(Flag_Commit1_Rollback2, objParametri_Server)

        Catch ex As Exception
        End Try

    End Sub

    Private _FilePost As New List(Of String)

    Private Sub CaricaShp(ByVal basepath As String, ByVal f As String, ByVal criteriMatch As String, ByVal objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objparametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal progressivoGias As Integer, ByVal Codice_Fiscale_Tecnico As String)


        Dim layercod As Integer = 55
        Dim lTipoEntita_Cod As String
        Dim bool As Boolean = False
        Dim tipo_importazione As Integer
        Dim sistemaRiferimento As Integer

        'trimble

        bool = False
        tipo_importazione = 2
        sistemaRiferimento = 1
        lTipoEntita_Cod = "1"
        layercod = 1


        Dim isSwats As Boolean = f.ToLower.EndsWith("swaths.shp")
        Dim isBoundary As Boolean = f.ToLower.EndsWith("boundary.shp")

        If isSwats Then
            lTipoEntita_Cod = "55"
            layercod = 55
        End If

        If isBoundary Then
            'il tipo di entità deve essere cooerente con l'albero..
            lTipoEntita_Cod = "19"
            layercod = 19
        End If

        Dim isCoverage As Boolean = Not (isSwats Or isBoundary)
        If isCoverage Then
            lTipoEntita_Cod = "50"
            layercod = 50
        End If

        Dim totalMatch As String() = criteriMatch.Split("|")

        Dim soloFile As String() = f.Split("\")
        Dim sFileToCheck As String = soloFile(soloFile.Length - 1).ToLower

        Dim Partecartella As String = dammiCartella(f)
        Dim iTotal As String = trovaIndice(totalMatch, Replace(Partecartella, basepath, "").TrimStart("\"), isCoverage)

        If iTotal = -1 Then
            Exit Sub
        End If

        Dim lTotalMatchSplit As String() = totalMatch(iTotal).Split("^")
        Dim parteEZ As String = lTotalMatchSplit(0)
        Dim parteGias As String = lTotalMatchSplit(1)

        'esempio: importa sì no (linee guida ab) importa sì no (confini) £  importa sì no (operazione) ! Operatore ! Macc ! Operazioni ? importa sì no (operazione) ! Operatore ! Macc ! Operazioni ? importa sì no (operazione) ! Operatore ! Macc ! Operazioni 
        'esempio: 01£1!DISERBO_EZ56072!12!9!3?1!Event_031612_0001_EZ5607!13!8!5
        Dim cosaImportare As String = lTotalMatchSplit(2)

        Dim cod_Risum As String = 0
        Dim mac_cod As String = 0
        Dim lav_cod As String = 0
        Dim lav_des As String = ""

        Dim importa As Boolean = False

        If sFileToCheck.ToLower.EndsWith(".shp") Then
            Select Case sFileToCheck
                Case "swaths.shp"
                    importa = CBool(-CInt(cosaImportare(0).ToString))
                    Exit Select
                Case "boundary.shp"
                    importa = CBool(-CInt(cosaImportare(1).ToString))
                    Exit Select
                Case "coverage.shp"
                    importa = getImportaSuOperazioni(cosaImportare, soloFile(soloFile.Length - 2), lav_cod, cod_Risum, mac_cod, lav_des)
                    Exit Select
                Case Else
                    importa = getImportaSuOperazioni(cosaImportare, soloFile(soloFile.Length - 2), lav_cod, cod_Risum, mac_cod, lav_des)
                    Exit Select
            End Select
        End If

        If Not importa Then
            Exit Sub
        End If

        Dim objimport As New AgronicaSHPWrapper.ShapeFileToAgronicaGis2012



        Dim files As ReadOnlyCollection(Of String)
        files = My.Computer.FileSystem.GetFiles(Partecartella, FileIO.SearchOption.SearchTopLevelOnly, "*.pos")



        If files.Count > 0 Then
            _FilePost.Add(files(0).Split("\")(files(0).Split("\").Length - 1) & "|" & parteGias)
        End If

        Dim appGias As String() = parteGias.Split("\")


        Dim piva As String = appGias(0)
        Dim sa_cod, campo_cod, appezza, reg_impianto, programmazione_Cod As Integer
        sa_cod = appGias(1)
        campo_cod = 0
        appezza = appGias(2)
        reg_impianto = appGias(3)
        programmazione_Cod = appGias(4)

        Dim GestioneRiportoDatiInGias As InterpretaDatiDBF.Tipo_Importazione.Tipo_GestioneRiportoDatiInGias = InterpretaDatiDBF.Tipo_Importazione.Tipo_GestioneRiportoDatiInGias.RiportoAutomaticoDeiDati
        Dim CategoriaDocumento As TipiEnumerativi.enum_CategorieDocumenti

        Dim inData As New ConfigurazioneImportazione(
            f,
            objparametri_Server.PivaSuperUser,
            piva,
            sa_cod,
            campo_cod,
            appezza,
            reg_impianto,
            programmazione_Cod,
            lav_cod,
            lav_des,
            cod_Risum,
            mac_cod,
            layercod,
            tipo_importazione,
            2011,
            "",
            "",
            progressivoGias,
            bool,
            sistemaRiferimento,
            lTipoEntita_Cod,
            Codice_Fiscale_Tecnico,
            GestioneRiportoDatiInGias
        )

        ' VAnni: 8/3/2021: il layer ora viene memorizzato sugli allegati, tutti gi altri vengono memorizzati in strutture dati GIS
        If layercod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Dettagli_Precision_Farming Then
            GestioneRiportoDatiInGias = InterpretaDatiDBF.Tipo_Importazione.Tipo_GestioneRiportoDatiInGias.RiportoSuAllegati
            CategoriaDocumento = TipiEnumerativi.enum_CategorieDocumenti.PrecisionFarming_MappaProduzione
            inData.CategoriaDocumento = CategoriaDocumento
            inData.GestioneRiportoDatiInGias = GestioneRiportoDatiInGias
        End If
        objimport.convert(inData, objparametri_Server, objparametri_Utenti)

    End Sub

    Private Shared Function getImportaSuOperazioni(ByVal cosaImportare As String, ByVal nomeCartella As String, ByRef lav_cod As Integer, ByRef cod_Risum As Integer, ByRef mac_cod As Integer, ByRef lav_des As String) As Boolean
        Dim importa As Boolean = False

        Dim cosaImportareSplitted As String() = cosaImportare.Split("£")(1).Split("?")
        For Each cur In cosaImportareSplitted

            Dim CC As String() = cur.Split("!")
            If CC(1).ToLower = nomeCartella.ToLower Then
                importa = CBool(-CInt(CC(0).ToString))
                lav_cod = CC(4)
                cod_Risum = CC(2)
                mac_cod = CC(3)
                lav_des = CC(5) & "-" & CC(1)
            End If

        Next

        Return importa
    End Function
    Private Function dammiCartella(ByVal f As String) As String
        Dim rval As String = ""
        Dim a As String() = f.Split("\")
        For i As Integer = 0 To a.Length - 2
            rval &= a(i) & "\"
        Next

        Return rval.TrimEnd("\")

    End Function


    Private Function trovaIndice(ByVal a As String(), ByVal trova As String, ByVal AppendLastDir As Boolean) As Integer

        If AppendLastDir Then
            trova = dammiCartella(trova)
        End If

        For i As Integer = 0 To a.Length - 1
            If a(i).StartsWith(trova) Then
                Return i
            End If
        Next
        Return -1
    End Function

    Public Function AnalizzaFileZip(ByVal FileZip As String) As String

        Dim folder As String = FileZip.Replace(".zip", "") & "\AgGPS\Data\"
        Dim files As ReadOnlyCollection(Of String)
        files = My.Computer.FileSystem.GetDirectories(folder, FileIO.SearchOption.SearchAllSubDirectories, "*")

        For Each f In files
            AnalizzaSingoloFile(folder, f)
        Next


        Return "Ok"

    End Function

    Private Sub AnalizzaSingoloFile(ByVal folder As String, ByVal f As String)
        Dim livello As Integer = 0
        Dim nomi As String() = Nothing

        LivelloNomeDataCartella(folder, f, livello, nomi)

        Dim myAzienda As String = nomi(0)
        Dim az As ezAziende = _
                           (From myAz In _ListaAziende _
                           Where myAz.NomeAzienda = myAzienda).FirstOrDefault
        If az Is Nothing Then
            az = New ezAziende With {.NomeAzienda = myAzienda}
            AggiungiAzienda(az)
        End If

        If nomi.Count = 1 Then
            Exit Sub
        End If

        Dim myCentro As String = nomi(1)
        Dim ce As ezCentri = _
            (From myc In az.ListaCentri _
             Where myc.NomeCentro = myCentro).FirstOrDefault

        If ce Is Nothing Then
            ce = New ezCentri With {.NomeCentro = myCentro}
            az.AggiungiCentro(ce)
        End If

        If nomi.Count = 2 Then
            Exit Sub
        End If

        Dim myi As String = nomi(2)
        Dim i As ezImpianti = _
            (From ii In ce.ListaImpianti _
             Where ii.NomeImpianto = myi).FirstOrDefault
        If i Is Nothing Then
            i = New ezImpianti With {.NomeImpianto = myi}
            ce.AggiungiImpianto(i)
        End If

        If nomi.Count = 3 Then
            Exit Sub
        End If

        Dim mOpera As String = nomi(3)
        Dim op As ezEventiOperazioni = _
            (From oo In i.ListaOperazioni _
             Where oo.NomeOperazione = mOpera).FirstOrDefault
        If op Is Nothing Then
            op = New ezEventiOperazioni With {.NomeOperazione = mOpera}
            i.AggiungiEventoOperazione(op)
        End If
    End Sub


    Private Sub LivelloNomeDataCartella(ByVal CartellaPadre As String, ByVal cartella As String, ByRef livello As Integer, ByRef appNomi As String())
        cartella = cartella.Replace("\\", "\")
        CartellaPadre = CartellaPadre.Replace("\\", "\")

        CartellaPadre = addslash(CartellaPadre)
        appNomi = cartella.Replace(CartellaPadre, "").Split("\")

        livello = appNomi.Count

    End Sub

    Private Function addslash(ByVal f As String) As String
        If Not f.EndsWith("\") Then
            f = f & "\"
        End If

        Return f
    End Function




End Class
