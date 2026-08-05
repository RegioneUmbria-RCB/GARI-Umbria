Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Xml
Imports AgronicaCoreVarieBIZ
Imports System.Text

Public Class CureRaccolte

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim ASG_ProgressivoGIAS As Integer

    Sub New(ByVal objparametriserver As AgronicaCoreParametri, ByVal ASGProgressivoGIAS As Integer)
        objParametri_Server = objparametriserver
        ASG_ProgressivoGIAS = ASGProgressivoGIAS
    End Sub


    'filtro per aziende da non considerare
    'NB: I CUAA sono da escludere perché sono di Deltafina e quindi non vengono gestiti.
    Dim FiltroTabella As String = "  cancellato not like '%s%' AND CUUA not in ('01478300542' )"

    Public Sub ImportaCure(ByVal annoDiImportazione As String, ByRef messaggio As String, importa As Boolean)
        Dim dp As New AgronicaCoreDataProvider.DataProvider
        Dim Imprese_Codici_Read As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read()
        Dim Imprese_Codici_Write As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write()
        Dim CentriAziendali_Read As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()
        Dim Fabbricati_R As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
        Dim Imprese_Read As New AgronicaCoreAnagrafeDAL.Imprese_Read()
        Dim Materie_Prime_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim objAgendaScrivi As New Agenda_Operazione_Helper
        Dim BaseCode, TopCode As Integer
        Call Calcola_BaseCode_TopCode(BaseCode,
              TopCode,
              ASG_ProgressivoGIAS)

        'attenzione, elimino prima i dati da non importare
        'metto un notprima di FiltroTabella
        Dim eliminaDaNonImportare As Boolean = True
        If eliminaDaNonImportare Then
            Dim strsql As String = "DELETE FROM [__ImportazioneCure] where NOT ( " & FiltroTabella & " ) "
            dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")
        End If

        Dim Verifica As Boolean = True
        '-------------------------------------------------------------------------------------------------------
        'Verifico coerenza piva, ciaa, codice socio(inserisco se non c'è)
        If Verifica Then
            '1) Leggo e assegno i codici socio se non presenti
            'Il codice socio è: le prime 4 cifre del chiveid, le ultime 4 di codcoltiv 14brxxxx 
            Dim strsql As String = " Select distinct [CHIAVEID], [CODCOLTIV], [CUUA] from [__ImportazioneCure] where " & FiltroTabella & " order by CODCOLTIV "
            Dim dtSoci As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            If Not IsNumeric(annoDiImportazione) OrElse annoDiImportazione > 2100 OrElse annoDiImportazione < 2000 Then
                Throw New Exception("L' anno di importazione deve essere compreso tra 2000 e 2100")
            End If
            'estraggo le ultime due cifre dell'anno
            annoDiImportazione = annoDiImportazione - 2000

            For Each dr As DataRow In dtSoci.Rows
                Dim anno As Integer = dr.Item("CODCOLTIV").ToString.Trim.Substring(0, 2)
                If dr.Item("CODCOLTIV").ToString.Trim.Substring(2, 2) <> "BR" Then
                    Throw New Exception(" dr.Item(CODCOLTIV).ToString.Trim.Substring(2, 2) <> BR  " & dr.Item("CODCOLTIV").ToString)
                End If

                If anno <> annoDiImportazione Then
                    Throw New Exception(" anno = " & anno)
                End If
                Dim csoc1 As String = dr.Item("CHIAVEID").ToString.Trim.Substring(0, 4)
                Dim csoc2 As String = dr.Item("CODCOLTIV").ToString.Trim.Substring(4, 4)
                Dim Cuaa As String = dr.Item("CUUA").ToString.Trim
                If csoc1 <> csoc2 Then
                    Throw New Exception(dr.Item("CHIAVEID") & " CHIAVEID <> CODCOLTIV " & dr.Item("CODCOLTIV"))
                End If
                If csoc1 = "" Then
                    Throw New Exception(" csoc1 = '' ")
                End If
                If Cuaa = "" Then
                    Throw New Exception(" Cuaa = '' ")
                End If

                'cerco la piva
                Dim Piva1 As String = Imprese_Read.Piva_From_CUAA(Cuaa, objParametri_Server)
                If Piva1 = "" Then
                    Throw New Exception(" al Cuaa " & Cuaa & " non corrisponde nessuna piva")
                End If

                'il problema è che lo stesso cuaa può avere piu codici socio nei dati di rampi
                'ma per fortuna
                'la query fa il like, quindi anche se concateno piu codici mi prende la piva
                Dim Piva2 As String = Imprese_Codici_Read.Piva_from_Codice_Like(csoc1, enum_CodiciAnagrafe.Codice_Fornitore, objParametri_Server)
                If Piva2 = "" Then
                    'devo aggiungere  il codice socio per la piva
                    Dim elencocodicisocio As String = Imprese_Codici_Read.Leggi_Codice_from_Imprese_Codici(Piva1, enum_CodiciAnagrafe.Codice_Fornitore, objParametri_Server)
                    If elencocodicisocio <> "" AndAlso Not elencocodicisocio.Contains(csoc1) Then
                        ' il problema è che lo stesso cuaa può avere piu codici socio nei dati di rampi
                        'quindi lo aggiungo in coda
                        'Throw New Exception(" la piva " & Piva1 & " con codice socio " & csoc1 & " e cuaa " & Cuaa & " nha nel sistema un altro codice socio " & csoc3 & "  ")
                        Imprese_Codici_Write.Cancella(Piva1, enum_CodiciAnagrafe.Codice_Fornitore, "", objParametri_Server)
                        Imprese_Codici_Write.Scrivi(Piva1, enum_CodiciAnagrafe.Codice_Fornitore,
                                                    elencocodicisocio & ";" & csoc1, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                    End If
                    If elencocodicisocio = "" Then
                        Imprese_Codici_Write.Cancella(Piva1, enum_CodiciAnagrafe.Codice_Fornitore, "", objParametri_Server)
                        Imprese_Codici_Write.Scrivi(Piva1, enum_CodiciAnagrafe.Codice_Fornitore, csoc1, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                    End If

                End If

                Piva2 = Imprese_Codici_Read.Piva_from_Codice_Like(csoc1, enum_CodiciAnagrafe.Codice_Fornitore, objParametri_Server)
                If Piva1 <> Piva2 Then
                    Throw New Exception(" la piva " & Piva1 & " del socio " & csoc1 & " e cuaa " & Cuaa & " non corrisponde alla Piva2 " & Piva2 & " presente nel sistema con quel codice socio ")
                End If

            Next

        End If
        '-------------------------------------------------------------------------------------------------------

        '-------------------------------------------------------------------------------------------------------
        'Verifico coerenza uds e forno
        If Verifica Then
            Dim strsql As String = " SELECT distinct uds, locale FROM [__ImportazioneCure] where " & FiltroTabella & "  order by uds "
            Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            For Each dr As DataRow In dt.Rows

                Dim Piva As String = ""
                Dim Sa_Cod As Integer = 0
                Dim Fabbricato_Cod As Integer = 0

                leggiPivaSaCodFabbricatoCodUds(dr, Piva, Sa_Cod, Fabbricato_Cod)

                If Piva = "" Then
                    Throw (New Exception("Piva = '' "))
                End If

                If Sa_Cod = 0 Then
                    Throw (New Exception("Sa_Cod = '' "))
                End If

                If Fabbricato_Cod = 0 Then
                    Throw (New Exception("Fabbricato_Cod = '' "))
                End If

            Next

        End If
        '-------------------------------------------------------------------------------------------------------

        '-------------------------------------------------------------------------------------------------------
        'Verifico coerenza numeri cartoni
        If Verifica Then
            Dim strsql As String = " SELECT distinct CARTNO  FROM [__ImportazioneCure] where " & FiltroTabella & "  order by CARTNO "
            Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            For Each dr As DataRow In dt.Rows

                If Not IsNumeric(dr.Item("CARTNO")) Then

                End If
                Dim CARTNO As Integer = CInt(dr.Item("CARTNO"))
                If CARTNO <= 0 Or CARTNO > 99999 Then
                    Throw New Exception("numero cartone fuori range " & CARTNO)
                End If
            Next
        End If
        '-------------------------------------------------------------------------------------------------------


        '-------------------------------------------------------------------------------------------------------
        'Verifico righe doppie
        If Verifica Then
            Dim strsql As String = " SELECT count(*) from ( select distinct *  FROM [__ImportazioneCure] where " & FiltroTabella & "  )t "
            Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            strsql = " SELECT count(*) FROM [__ImportazioneCure] where " & FiltroTabella & "   "
            Dim d2t As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            If dt.Rows(0).Item(0) <> d2t.Rows(0).Item(0) Then
                Throw New Exception("ci sono righe doppie")
            End If

        End If
        '-------------------------------------------------------------------------------------------------------


        '-------------------------------------------------------------------------------------------------------
        'Verifico date
        If Verifica Then
            Dim strsql As String = " SELECT distinct inizio,fine FROM [__ImportazioneCure] where " & FiltroTabella & "   "
            Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            For Each dr As DataRow In dt.Rows
                Dim INIZIO As String = dr.Item("INIZIO")
                Dim FINE As String = dr.Item("FINE")
                If INIZIO.Count <> 8 Then
                    Throw New Exception("lunghezza INIZIO" & INIZIO & " non corretta")
                End If
                If FINE.Count <> 8 Then
                    Throw New Exception("lunghezza FINE" & FINE & " non corretta")
                End If
                Try
                    Dim data As Date = New Date(INIZIO.Substring(0, 4), INIZIO.Substring(4, 2), INIZIO.Substring(6, 2))
                    Dim data2 As Date = New Date(FINE.Substring(0, 4), FINE.Substring(4, 2), FINE.Substring(6, 2))
                Catch ex As Exception
                    Throw New Exception("cast non riuscito delle date INIZIO FINE " & ex.Message)
                End Try
            Next

        End If
        '-------------------------------------------------------------------------------------------------------

        '-------------------------------------------------------------------------------------------------------
        'Verifico corona
        If Verifica Then
            Dim strsql As String = " SELECT distinct corona FROM [__ImportazioneCure]  where " & FiltroTabella & "  "
            Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            For Each dr As DataRow In dt.Rows
                Dim corona As String = dr.Item("corona")
                If corona <> 1 And corona <> 2 And corona <> 3 And corona <> 4 Then
                    Throw New Exception("corona " & corona & " non comporesa tra 1-4")
                End If
            Next

        End If
        '-------------------------------------------------------------------------------------------------------

        '-------------------------------------------------------------------------------------------------------
        'Verifico numinizio e numfine per soci, 
        'da NUMINIZ, NUMFINE, CODCOLTIV, appezzamento creo il gruppo per la raccolta e cura
        If Verifica Then
            Dim strsql As String = " SELECT distinct NUMINIZ, NUMFINE, CARTNO, CODCOLTIV  FROM [__ImportazioneCure] where " & FiltroTabella & "   "
            Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            For Each dr As DataRow In dt.Rows
                Dim csoc2 As String = dr.Item("CODCOLTIV").ToString.Trim.Substring(4, 4)
                Dim NUMINIZ As Integer = dr.Item("NUMINIZ")
                Dim NUMFINE As Integer = dr.Item("NUMFINE")
                Dim CARTNO As Integer = dr.Item("CARTNO")
                If CARTNO < NUMINIZ Or CARTNO > NUMFINE Then
                    Throw New Exception("CARTNO < NUMINIZ Or CARTNO > NUMFINE " & CARTNO & "  " & NUMINIZ & "  " & NUMFINE & " ")
                End If
            Next

        End If
        '-------------------------------------------------------------------------------------------------------

        '-------------------------------------------------------------------------------------------------------
        'Verifico linea e appezza ci metto inizio per controllare appezza
        Dim stbListaErrori As New StringBuilder
        Dim esitoVerificaLineaAppezza As New RispostaStandard
        Dim iConteggioVerifiche As Integer = 0
        Dim iConteggioVerificheInErrore As Integer = 0

        If Verifica Then
            Dim strsql As String = " SELECT distinct linea, appezzamento, CODCOLTIV, INIZIO FROM [__ImportazioneCure] where " & FiltroTabella & "   "
            Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            esitoVerificaLineaAppezza.RispostaOK = True

            For Each dr As DataRow In dt.Rows
                Dim csoc2 As String = dr.Item("CODCOLTIV").ToString.Trim.Substring(4, 4)
                'il problema è che lo stesso cuaa può avere piu codici socio nei dati di rampi
                'ma per fortuna
                'la query fa il like, quindi anche se concateno piu codici mi prende la piva
                Dim Piva2 As String = Imprese_Codici_Read.Piva_from_Codice_Like(csoc2, enum_CodiciAnagrafe.Codice_Fornitore, objParametri_Server)
                Dim appezzamento As Integer
                If IsNumeric(dr.Item("appezzamento").ToString) Then
                    appezzamento = CInt(dr.Item("appezzamento").ToString)
                Else
                    appezzamento = -1
                End If

                Dim INIZIO As String = dr.Item("INIZIO").ToString.Trim
                Dim dataInizio As Date = New Date(INIZIO.Substring(0, 4), INIZIO.Substring(4, 2), INIZIO.Substring(6, 2))
                Dim Azianda_Appezza As Integer
                Dim Azianda_Id_Reg As Integer
                Dim Azianda_Progetto_Cod As Integer
                Dim sacod As Integer = 0
                Dim rvalSaCodAppezzaAzienda As RispostaStandard =
                    leggiSaCodAppezzaAzienda(Piva2, sacod, appezzamento, dataInizio, Azianda_Appezza, Azianda_Id_Reg, Azianda_Progetto_Cod)
                If Azianda_Appezza = 0 Or Azianda_Id_Reg = 0 Or sacod = 0 Or Not rvalSaCodAppezzaAzienda.RispostaOK Then
                    esitoVerificaLineaAppezza.RispostaOK = False
                    iConteggioVerificheInErrore += 1
                    stbListaErrori.Append("appezzamento = 0 Or Azianda_Id_Reg = 0 Or sacod = 0: ")
                    stbListaErrori.Append(rvalSaCodAppezzaAzienda.Errore)
                    stbListaErrori.AppendLine(";")
                End If

                iConteggioVerifiche += 1

            Next
            ' p = p

            If Not esitoVerificaLineaAppezza.RispostaOK Then
                esitoVerificaLineaAppezza.Errore = "Totale verifiche in errore: " & iConteggioVerificheInErrore & " su " & iConteggioVerifiche & " effettuate." & vbCrLf
                esitoVerificaLineaAppezza.Errore &= stbListaErrori.ToString()
                Throw New Exception(esitoVerificaLineaAppezza.Errore)
            End If

        End If
        '-------------------------------------------------------------------------------------------------------

        '-------------------------------------------------------------------------------------------------------
        'carico dati, 
        'uds, locale, NUMINIZ, NUMFINE, CODCOLTIV, linea, appezzamento sono id gruppo per la raccolta e cura
        'anche se sono univoci anche uds, locale, NUMINIZ, NUMFINE, CODCOLTIV
        If importa Then
            Dim strsql As String = "SELECT distinct  UDS , LOCALE , INIZIO , FINE , CODCOLTIV, CUUA, NUMINIZ , NUMFINE ,  APPEZZAMENTO , CORONA, PESO   FROM [__ImportazioneCure] where " & FiltroTabella & "  "
            Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            strsql = " SELECT distinct  UDS , LOCALE ,  CODCOLTIV, NUMINIZ , NUMFINE  FROM [__ImportazioneCure]  where " & FiltroTabella & "  "
            Dim d2t As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

            If dt.Rows.Count <> d2t.Rows.Count Then
                Throw New Exception("ci sono UDS, LOCALE, CODCOLTIV, NUMINIZ, NUMFINE CON APPEZZAMENTO, CORONA, INIZIO, FINE, peso diversi")
            End If

            Dim Mat_Cod_Prod_InCura As Integer = Materie_Prime_R.Mat_Cod_Leggi("03261930543", "-1", TRASFORMATI_VEGETALI, "INCURA/335/05010411", objParametri_Server)
            Dim Mat_Cod_Prod_Curato As Integer = Materie_Prime_R.Mat_Cod_Leggi("03261930543", "-1", TRASFORMATI_VEGETALI, "CURATO/335/05010411", objParametri_Server)

            'ino per ciascuna raccolta e infornatura
            Dim c As Integer = 0
            For Each dr As DataRow In dt.Rows
                c = c + 1

                Try

                    Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni
                    ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

                    'non modificarli che dopo li uso nella select
                    Dim UDS As String = dr.Item("UDS")
                    Dim LOCALE As String = dr.Item("LOCALE")
                    Dim NUMINIZ As String = dr.Item("NUMINIZ")
                    Dim NUMFINE As String = dr.Item("NUMFINE")
                    Dim CODCOLTIV As String = dr.Item("CODCOLTIV")
                    'Dim LINEA As String = dr.Item("LINEA")
                    Dim CORONA As String = dr.Item("CORONA")
                    Dim INIZIO As String = dr.Item("INIZIO")
                    Dim FINE As String = dr.Item("FINE")
                    Dim APPEZZAMENTO As Integer = dr.Item("APPEZZAMENTO")

                    Dim IDInfornaturaImportata As String = UDS.Trim & "/" & LOCALE.Trim & "/" & INIZIO.Trim & "/" & FINE.Trim & "/" & CODCOLTIV.Trim & "/" & NUMINIZ.Trim & "/" & NUMFINE.Trim & "/" & CStr(APPEZZAMENTO).Trim & "/" & CORONA.Trim & "/"

                    Dim anno As Integer = dr.Item("CODCOLTIV").ToString.Trim.Substring(0, 2)
                    Dim Socio As String = dr.Item("CODCOLTIV").ToString.Trim.Substring(4, 4)
                    Dim Cuaa As String = dr.Item("CUUA").ToString.Trim
                    Dim peso As Double = dr.Item("peso")
                    Dim Piva As String = Imprese_Read.Piva_From_CUAA(Cuaa, objParametri_Server)
                    Dim dataInizio As Date = New Date(INIZIO.Substring(0, 4), INIZIO.Substring(4, 2), INIZIO.Substring(6, 2))
                    Dim dataFine As Date = New Date(FINE.Substring(0, 4), FINE.Substring(4, 2), FINE.Substring(6, 2))

                    Dim UDS_Piva As String = ""
                    Dim UDS_Sa_Cod As Integer = 0
                    Dim UDS_Fabbricato_Cod As Integer = 0
                    leggiPivaSaCodFabbricatoCodUds(dr, UDS_Piva, UDS_Sa_Cod, UDS_Fabbricato_Cod)

                    Dim Azianda_Sa_Cod As Integer = 0
                    Dim Azianda_Appezza As Integer = 0
                    Dim Azianda_Id_Reg As Integer = 0
                    Dim Azianda_Progetto_Cod As Integer = 0
                    Dim Azienda_Magazzino_Sa_Cod As Integer = 0
                    Dim Azienda_Magazzino_Fabbricato_Cod As Integer
                    leggiSaCodAppezzaAzienda(Piva, Azianda_Sa_Cod, CInt(APPEZZAMENTO), dataInizio, Azianda_Appezza, Azianda_Id_Reg, Azianda_Progetto_Cod)

                    Azienda_Magazzino_Fabbricato_Cod = Fabbricati_R.LeggiMagazzino(Piva, Azianda_Sa_Cod, objParametri_Server)
                    If Azienda_Magazzino_Fabbricato_Cod = 0 Then
                        Azienda_Magazzino_Fabbricato_Cod = Fabbricati_R.LeggiMagazzino2_passopivasolo(Piva, Azienda_Magazzino_Sa_Cod, objParametri_Server)
                        If Azienda_Magazzino_Fabbricato_Cod = 0 Then
                            Throw New Exception("non c'e magazzino azienda " & Piva)
                        End If
                    Else
                        Azienda_Magazzino_Sa_Cod = Azianda_Sa_Cod
                    End If

                    Dim LottoRaccolta As String = GeneraIdLottoRaccolta(Piva)
                    Dim Elem_Cod As Integer = TRASFORMATI_VEGETALI
                    Dim PRodottoraccolto As String = "RACC/335/" & CInt(CORONA) & "CF"
                    Dim Mat_Cod As Integer = Materie_Prime_R.Mat_Cod_Leggi("03261930543", "-1", Elem_Cod, PRodottoraccolto, objParametri_Server)
                    Dim Cal_Cod_Raccolto As Integer = GeneraCampionatura()


                    'elimino raccolta e cura con quell'id IDInfornatura
                    'recupero i due id agenda della rcaccolta e cura con IDInfornaturaImportata in blocco_username
                    Dim filtrodacanc As String = " blocco_username = '" & IDInfornaturaImportata & "' "
                    Dim dtopagelim As DataTable = New AgronicaCoreContabDAL.Agenda_R().Leggi("", 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, filtrodacanc, "", objParametri_Server)
                    'o 0 o 2, altrimenti eccezione
                    If dtopagelim.Rows.Count = 2 Then
                        Dim Totaleagenda As Integer = New AgronicaCoreContabDAL.Agenda_R().NumetoTotale(objParametri_Server)
                        For i = 0 To dtopagelim.Rows.Count - 1
                            Dim objParametriAgenda As New ParametriAgenda
                            objParametriAgenda.Piva = dtopagelim.Rows(i).Item("Piva")
                            objParametriAgenda.Sa_Cod = dtopagelim.Rows(i).Item("Sa_Cod")
                            objParametriAgenda.Id_Agenda = dtopagelim.Rows(i).Item("Id_Agenda")
                            objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Cancellazione
                            objParametriAgenda.Lav_Cod = dtopagelim.Rows(i).Item("Lav_Cod")

                            Dim res As Boolean = AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda_Utility.Cancella_Operazione_E_Collegate(
                                objParametriAgenda,
                                objParametri_Server, messaggio, True)

                            If res = False Then
                                Throw New Exception("non cancellata op = " & dtopagelim.Rows(i).Item("Id_Agenda"))
                            End If

                        Next

                        Dim TotaleagendaDopo As Integer = New AgronicaCoreContabDAL.Agenda_R().NumetoTotale(objParametri_Server)
                        '6 operazioni, ci sono le 4 collegate
                        If TotaleagendaDopo <> (Totaleagenda - 6) Then
                            Throw New Exception("TotaleagendaDopo <> (Totaleagenda - 2)  " & TotaleagendaDopo & " " & Totaleagenda)
                        End If

                    ElseIf dtopagelim.Rows.Count = 0 Then
                        ' ok
                    Else
                        Throw New Exception("dtopagelim.Rows.Count = " & dtopagelim.Rows.Count)
                    End If

                    'salvo nuova
                    CreaSalvaRaccolta(objAgendaScrivi, BaseCode, TopCode, UDS, LOCALE, IDInfornaturaImportata, Piva,
                              dataInizio, UDS_Piva, UDS_Sa_Cod, Azianda_Sa_Cod, Azianda_Appezza,
                              Azianda_Id_Reg, Azianda_Progetto_Cod, Azienda_Magazzino_Sa_Cod,
                              Azienda_Magazzino_Fabbricato_Cod, LottoRaccolta, Elem_Cod, Mat_Cod, Cal_Cod_Raccolto, peso)

                    'quelli in maiuscolo li uso nella query, devono essere uguali
                    CreaSalvaCura(IDInfornaturaImportata, dp, Fabbricati_R, objAgendaScrivi, Mat_Cod_Prod_InCura, Mat_Cod_Prod_Curato,
                                  UDS, LOCALE, INIZIO, FINE, CODCOLTIV, NUMINIZ, NUMFINE, APPEZZAMENTO, CORONA,
                                  dataInizio, dataFine,
                                  UDS_Piva, UDS_Sa_Cod, UDS_Fabbricato_Cod,
                                  Piva, Azianda_Sa_Cod, Azienda_Magazzino_Sa_Cod, Azienda_Magazzino_Fabbricato_Cod,
                                  LottoRaccolta, Mat_Cod, Cal_Cod_Raccolto, peso)


                    'elimino righe importate
                    strsql = "DELETE FROM [__ImportazioneCure]  where UDS=" & UDS & " AND LOCALE=" & LOCALE & " AND INIZIO='" & Agro_SQL_SaveText(INIZIO) & "' AND FINE='" & Agro_SQL_SaveText(FINE) & "' AND CODCOLTIV='" & Agro_SQL_SaveText(CODCOLTIV) & "'  AND NUMINIZ=" & NUMINIZ & " AND NUMFINE=" & NUMFINE & " AND CORONA='" & Agro_SQL_SaveText(CORONA) & "' AND " & FiltroTabella & "  "
                    dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")



                    ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                Catch ex As Exception
                    Dim msg As String = ex.Message
                    Try
                        ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    Catch ex2 As Exception
                        msg = msg & " ; " & ex2.Message
                    End Try
                    messaggio = messaggio & "<br /> Errore non importata la riga " & c & ": " & msg
                Finally
                    ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
                End Try


            Next

        End If
        '-------------------------------------------------------------------------------------------------------

    End Sub

    Private Sub leggiPivaSaCodFabbricatoCodUds(ByVal dr As DataRow, ByRef piva As String, ByRef Sa_Cod As Integer, ByRef Fabbricato_Cod As Integer)
        Dim CentriAziendali_Read As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim Fabbricati_R As New AgronicaCoreAnagrafeDAL.Fabbricati_R

        Dim uds As Integer = dr.Item("uds")
        Dim locale As Integer = dr.Item("locale")

        Dim udsstr As String = uds.ToString.PadLeft(3, "0") & " - "
        Dim localestr As String = locale.ToString.PadLeft(3, "0") & " - "

        'cerco sotto FAT
        piva = "00165600545"

        Dim filtro As String = "Sa_Nome like '" & udsstr & "%'  "
        Dim dtc As DataTable = CentriAziendali_Read.Leggi(piva, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, "", objParametri_Server)
        If dtc.Rows.Count = 1 Then
            Sa_Cod = dtc.Rows(0).Item("Sa_Cod")
        Else
            Throw New Exception("centri: " & dtc.Rows.Count & " per la piva uds " & piva & " e numero uds " & uds)
        End If

        filtro = "Fabbricato_Des like '" & localestr & "%'  "
        Dim dtf As DataTable = Fabbricati_R.Leggi(piva, Sa_Cod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, "", objParametri_Server)
        If dtf.Rows.Count = 1 Then
            Fabbricato_Cod = dtf.Rows(0).Item("Fabbricato_Cod")
        Else
            Throw New Exception("fabbricati: " & dtf.Rows.Count & " per la piva uds " & piva & " sacod " & Sa_Cod & " e numero forno " & locale)
        End If

        If Sa_Cod = 0 Then
            Throw New Exception("sacod 0")
        End If

        If Fabbricato_Cod = 0 Then
            Throw New Exception("Fabbricato_Cod 0")
        End If
    End Sub

    ' Private p As String = ""

    Private Function leggiSaCodAppezzaAzienda(
        ByVal Piva As String,
        ByRef Azianda_Sa_Cod As Integer,
        ByVal appezzamento As Integer,
        ByVal dataInizio As Date,
        ByRef Azianda_Appezza As Integer,
        ByRef Azianda_Id_Reg As Integer,
        ByRef Azianda_Progetto_Cod As Integer
    ) As RispostaStandard

        Dim rval As New RispostaStandard

        rval.RispostaOK = True
        rval.Errore = ""

        Dim Appcod As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R()
        '21/11/2019 Aggiunto il not like per escludere le serre (ci mettono la S)
        Dim dt As DataTable = Appcod.Leggi(Piva,
                                0,
                                0,
                                enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                appezzamento,
                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                " Appezzamento_Codici.Val_Cod not Like '%s%' ",
                                "",
                                objParametri_Server)

        If dt.Rows.Count = 0 Then
            rval.RispostaOK = False
            rval.Errore &= " Per la Piva " & Piva & " non è stato trovato nessun appezzamento con codice " & appezzamento
            Return rval
        ElseIf dt.Rows.Count > 1 Then
            rval.RispostaOK = False
            rval.Errore &= " Per la Piva " & Piva & " sono stati trovati " & dt.Rows.Count & " appezzamenti con codice " & appezzamento
            Return rval
        ElseIf dt.Rows.Count <> 1 Then
            rval.RispostaOK = False
            rval.Errore &= " dt.Rows.Count <> 1 " & dt.Rows.Count
            Return rval
            'If Not p.Contains(Piva) Then
            '    p = p & ";" & Piva
            'End If
            'Azianda_Appezza = -1
            'Azianda_Id_Reg = -1
            'Azianda_Sa_Cod = -1
            'Exit Sub
        End If

        Azianda_Sa_Cod = dt.Rows(0).Item("Sa_Cod")
        Azianda_Appezza = dt.Rows(0).Item("Appezza")


        Dim regimp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        dt = regimp.Leggi(Piva, Azianda_Sa_Cod, Azianda_Appezza, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " validita_inizio < " & Agro_SQL_SaveDate(dataInizio) & " and validita_fine > " & Agro_SQL_SaveDate(dataInizio) & " ", "", objParametri_Server)
        If dt.Rows.Count <> 1 Then
            rval.RispostaOK = False
            rval.Errore &= " dt.Rows.Count <> 1 " & dt.Rows.Count
            Return rval
        End If
        Azianda_Id_Reg = dt.Rows(0).Item("Id_Reg")

        Return rval

    End Function




#Region "Raccolta"

    Private Sub CreaSalvaRaccolta(ByVal objAgendaScrivi As Agenda_Operazione_Helper, ByRef BaseCode As Integer,
                                  ByRef TopCode As Integer, ByVal UDS As Integer, ByVal LOCALE As Integer,
                                  ByVal IDInfornaturaImportata As String, ByRef Piva As String, ByVal dataInizio As Date,
                                  ByVal UDS_Piva As String, ByVal UDS_Sa_Cod As Integer, ByVal Azianda_Sa_Cod As Integer,
                                  ByVal Azianda_Appezza As Integer, ByVal Azianda_Id_Reg As Integer,
                                  ByVal Azianda_Progetto_Cod As Integer, ByVal Azienda_Magazzino_Sa_Cod As Integer,
                                  ByVal Azienda_Magazzino_Fabbricato_Cod As Integer, ByVal Lotto As String,
                                  ByVal Elem_Cod As Integer, ByVal Mat_Cod As Integer, ByVal ProgrCalCod As Integer, ByVal peso As Double)

        Dim Agenda As Operazione_Agenda
        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = dataInizio
        Agenda.Piva = Piva
        Agenda.Sa_Cod = Azianda_Sa_Cod
        Agenda.Lav_Cod = LAVCOD_RACCOLTA
        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode

        Agenda.Blocco_Flag = 1
        Agenda.Blocco_Username = IDInfornaturaImportata
        Agenda.Blocco_Data = Date.Now

        Agenda.Des_Lib = "Raccolta Tabacco Importato -  lotto: " & Lotto & " - ID: " & IDInfornaturaImportata


        Dim ListaImp As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        Dim imp As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
        imp.Piva = Piva
        imp.Sa_Cod = Azianda_Sa_Cod
        imp.Appezza = Azianda_Appezza
        imp.ID_Reg = Azianda_Id_Reg
        imp.Progetto_Cod = Azianda_Progetto_Cod
        imp.Qta = peso
        imp.Qta2 = 0
        ListaImp.Add(imp)

        Dim messaggio_errore As String
        If Not Crea_Agenda_Movimento_Raccolta(Agenda, ListaImp, messaggio_errore, Lotto, Elem_Cod, Mat_Cod) Then
            Throw New Exception("Crea_Agenda_Movimento_Raccolta" & messaggio_errore)
        End If
        Dim Movimento_Carico_Creato As Movimento
        If Not Crea_Agenda_Movimento_Carico(Agenda, ListaImp, messaggio_errore, Lotto, Movimento_Carico_Creato, Azienda_Magazzino_Sa_Cod, Azienda_Magazzino_Fabbricato_Cod, ProgrCalCod) Then
            Throw New Exception("Crea_Agenda_Movimento_Carico" & messaggio_errore)
        End If
        Dim Agenda_Scarico As New Operazione_Agenda
        Dim Agenda_Carico_Uds As New Operazione_Agenda
        Dim IdAgedaScaricoOld As Integer = 0
        Dim IdAgedaCaricoOld As Integer = 0
        CreaOggettoAgendaScaricoProdottoPerInvioUDS(Agenda, Movimento_Carico_Creato, Agenda_Scarico, IdAgedaScaricoOld, UDS_Piva, UDS, LOCALE)
        CreaOggettoAgendaCaricoPressoUDS(Agenda, Movimento_Carico_Creato, Agenda_Carico_Uds, IdAgedaCaricoOld, UDS_Piva, UDS_Sa_Cod, Piva)

        Dim Id_Agenda As Integer
        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)



        'Scrivo le op colegate e i riferimenti
        Dim ListaOpAgendaCollegate As New List(Of Operazione_Agenda)
        ListaOpAgendaCollegate.Add(Agenda_Scarico)
        ListaOpAgendaCollegate.Add(Agenda_Carico_Uds)
        For Each opcollegata As Operazione_Agenda In ListaOpAgendaCollegate
            Dim Id_Agenda_Rif As Integer
            ' Dim Id_Mov_Rif As Integer = opcollegata.Movimenti(0).Id_Mov

            'dovrei cambiare gli id_agenda
            Dim newid As Integer = 0
            'solo per modifica, ma quin non modifico
            'vedi op raccolta

            'scrivo le op linkate
            Id_Agenda_Rif = objAgendaScrivi.Scrivi(opcollegata, objParametri_Server)

            'scrivo i riferimenti
            Dim RifAg As New Movimento_Dettaglio_Riferimento
            RifAg.Piva_Rif = Agenda.Piva
            RifAg.Sa_Cod_Rif = Agenda.Sa_Cod
            RifAg.Id_Agenda_Rif = Id_Agenda
            RifAg.Id_Mov_Rif = -1
            RifAg.Id_Mov_Det_Rif = -1
            RifAg.Lav_Cod_Rif = Agenda.Lav_Cod
            RifAg.Piva = opcollegata.Piva
            RifAg.Sa_Cod = opcollegata.Sa_Cod
            RifAg.Id_Agenda = Id_Agenda_Rif
            RifAg.Id_Mov = -1
            RifAg.Id_Mov_Det = -1
            RifAg.Lav_Cod = opcollegata.Lav_Cod
            Dim objRifScrivi As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
            objRifScrivi.Scrivi(RifAg, objParametri_Server)
        Next


    End Sub

    Private Function Crea_Agenda_Movimento_Raccolta(ByRef Agenda As Operazione_Agenda, ByRef ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef messaggio_errore As String, ByVal Lotto As String, ByVal Elem_Cod As Integer, ByVal Mat_Cod As Integer) As Boolean


        Dim Udm As Integer = enum_UnitaMisura.KG

        Dim Movimento_OperazioneColturale As New Movimento

        Movimento_OperazioneColturale.Id_Agenda = Agenda.Id_Agenda
        Movimento_OperazioneColturale.Piva = Agenda.Piva
        Movimento_OperazioneColturale.Sa_Cod = Agenda.Sa_Cod
        Movimento_OperazioneColturale.Lav_Cod = LAVCOD_RACCOLTA
        Movimento_OperazioneColturale.Cau_Mov = enum_Agenda_Causali.RILIEVO_RACCOLTA
        Movimento_OperazioneColturale.Mov_Desc = "Operazione Importata"
        Movimento_OperazioneColturale.Data = Agenda.Data
        Movimento_OperazioneColturale.BaseCode = Agenda.BaseCode
        Movimento_OperazioneColturale.TopCode = Agenda.TopCode

        Movimento_OperazioneColturale.Modalita = 0

        Movimento_OperazioneColturale.Extra_Int = enum_RACCOLTA_TIPO.Raccolta_e_Cura 'salvo il tipo raccolta, puo essere utile

        '----------------------------------------------------------
        '----- MOVIMENTI DETTAGLI , DET.TECNICI, DESTINAZIONI -----
        '----------------------------------------------------------
        Dim Movimento_Dettaglio As New Movimento_Dettaglio
        Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
        Movimento_Dettaglio.Piva = Agenda.Piva
        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod

        Movimento_Dettaglio.Mov_Det_Des = "Dettagli Prodotti Aziendali Ottenuti da Raccolta"

        Movimento_Dettaglio.Elem_Cod = Elem_Cod
        Movimento_Dettaglio.Pro_Cod = 0
        Movimento_Dettaglio.Mat_Cod = Mat_Cod
        Movimento_Dettaglio.Udm_Cod = Udm
        'dopo  Movimento_Dettaglio.Qta = QtaTot
        Movimento_Dettaglio.Contabilizzato = NONCONTABILE
        Movimento_Dettaglio.Pendente = enum_Pendenza.MovESENTE
        Movimento_Dettaglio.Cal_Cod = 12 'Materie_Prime_Calibri cal_cod=12 indefinito
        Movimento_Dettaglio.Lotto = Lotto
        Movimento_Dettaglio.Data = Agenda.Data
        Movimento_Dettaglio.Anno = 1900
        Movimento_Dettaglio.BaseCode = Agenda.BaseCode
        Movimento_Dettaglio.TopCode = Agenda.TopCode

        Dim tot As Double = 0
        For i = 0 To ListaImp.Count - 1

            Dim Movimento_Destinazione As New Movimento_Destinazione

            Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
            Movimento_Destinazione.Piva = ListaImp(i).Piva
            Movimento_Destinazione.Sa_Cod = ListaImp(i).Sa_Cod
            Movimento_Destinazione.Appezza = ListaImp(i).Appezza
            Movimento_Destinazione.Id_Destinazione = ListaImp(i).ID_Reg
            Movimento_Destinazione.Qta = ListaImp(i).Qta
            tot = tot + ListaImp(i).Qta
            Movimento_Destinazione.BaseCode = Agenda.BaseCode
            Movimento_Destinazione.TopCode = Agenda.TopCode


            'salvo qui progetto cod per metytere poi nel dettaglio della destinazione nel caso di semilavorati
            Movimento_Destinazione.parametroGenerico = ListaImp(i).Progetto_Cod

            Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)
        Next

        Movimento_Dettaglio.Qta = tot

        Movimento_OperazioneColturale.Movimenti_Dettagli.Add(Movimento_Dettaglio)
        'se ho almeno un movimento dettaglio nell'operazione la aggiungo all'agenda
        If Movimento_OperazioneColturale.Movimenti_Dettagli.Count > 0 Then
            'aggiungo il movimento rilievo in campo-installazione trappola all'operazione agenda
            Agenda.Movimenti.Add(Movimento_OperazioneColturale)
        Else
            messaggio_errore = messaggio_errore & " NessunDatoSalvato"
            Return False
        End If



        Return True
    End Function

    Private Function Crea_Agenda_Movimento_Carico(ByRef Agenda As Operazione_Agenda,
                                                  ByRef ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef messaggio_errore As String, ByVal Lotto As String, ByRef Movimento_Carico As Movimento, ByVal Azienda_Magazzino_Sa_Cod As Integer,
                                                  ByVal Azienda_Magazzino_Fabbricato_Cod As Integer,
                                                  ByVal ProgrCalCod As Integer) As Boolean


        Dim Movimento_Raccolta As Movimento = Agenda.Movimenti(Agenda.Movimenti.Count - 1)


        Dim Sa_Cod_magazzino As String = ""
        Dim fabbricatox_Cod As String = ""

        Dim magazzinoEsterno As Boolean = True

        'se magazzino è quello dell'azienda
        magazzinoEsterno = False
        Sa_Cod_magazzino = Azienda_Magazzino_Sa_Cod
        fabbricatox_Cod = Azienda_Magazzino_Fabbricato_Cod



        If False Then 'CAU_CONFERIMENTO Then
            Sa_Cod_magazzino = -1
            fabbricatox_Cod = -1
            '...
        End If

        Dim generalizzata As Boolean = False
        If Movimento_Raccolta.Movimenti_Dettagli(0).Elem_Cod = TRASFORMATI_VEGETALI Then
            generalizzata = True
        ElseIf Movimento_Raccolta.Movimenti_Dettagli(0).Elem_Cod = SEMILAVORATI_VEGETALI Then
            generalizzata = False
        Else
            Throw New Exception()
        End If



        Movimento_Carico = New Movimento
        Movimento_Carico.Id_Agenda = Agenda.Id_Agenda
        Movimento_Carico.Piva = Agenda.Piva
        Movimento_Carico.Sa_Cod = Sa_Cod_magazzino 'Agenda.Sa_Cod è sbagliato se ho magazzino in altro centro se multicentro è sbagliat ousare Split(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue, "|")(1)
        Movimento_Carico.Data = CDate(Agenda.Data)
        Movimento_Carico.Ora = CDate(Agenda.Data)
        Movimento_Carico.Lav_Cod = Agenda.Lav_Cod
        Movimento_Carico.Cau_Mov = CAU_CARICO
        Movimento_Carico.Mov_Desc = "Carico di Magazzino Da Raccolta"
        Movimento_Carico.BaseCode = Agenda.BaseCode
        Movimento_Carico.TopCode = Agenda.TopCode

        If generalizzata Then
            'un movimento dettaglio e una destinazione (trasformati)
            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI CARICO 
            '------------------------------------------------

            Dim Movimento_Dettaglio_Carico_Trappola As New Movimento_Dettaglio

            Movimento_Dettaglio_Carico_Trappola.Id_Agenda = Agenda.Id_Agenda
            Movimento_Dettaglio_Carico_Trappola.Piva = Agenda.Piva
            Movimento_Dettaglio_Carico_Trappola.Sa_Cod = Sa_Cod_magazzino 'ok per multicentro
            Movimento_Dettaglio_Carico_Trappola.Data = Agenda.Data
            Movimento_Dettaglio_Carico_Trappola.Elem_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Elem_Cod
            Movimento_Dettaglio_Carico_Trappola.Pro_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Pro_Cod
            Movimento_Dettaglio_Carico_Trappola.Mat_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Mat_Cod
            Movimento_Dettaglio_Carico_Trappola.Mov_Det_Des = "Carico di Trasformati Aziendali da Raccolta"
            Movimento_Dettaglio_Carico_Trappola.Udm_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Udm_Cod
            Movimento_Dettaglio_Carico_Trappola.Qta = Movimento_Raccolta.Movimenti_Dettagli(0).Qta
            Movimento_Dettaglio_Carico_Trappola.Cal_Cod = ProgrCalCod 'negativo, puntatore
            Movimento_Dettaglio_Carico_Trappola.Lotto = Movimento_Raccolta.Movimenti_Dettagli(0).Lotto
            Movimento_Dettaglio_Carico_Trappola.Contabilizzato = NONCONTABILE
            Movimento_Dettaglio_Carico_Trappola.Pendente = enum_Pendenza.MovGiustificato
            Movimento_Dettaglio_Carico_Trappola.Lav_Cod = Agenda.Lav_Cod
            Movimento_Dettaglio_Carico_Trappola.Cau_Mov = Movimento_Carico.Cau_Mov
            Movimento_Dettaglio_Carico_Trappola.Anno = 1900

            'Movimento_Dettaglio_Carico_Trappola.Raccolto_Campionatura = XML_GeneraBlocco_RaccoltoCampionatura()
            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI SCARICO 
            '------------------------------------------------
            'per generalizzata uno altrimenti per ciasdcun Appezza
            Dim Movimento_Destinazione_Carico_Trappola As New Movimento_Destinazione
            Movimento_Destinazione_Carico_Trappola.Id_Agenda = Agenda.Id_Agenda
            Movimento_Destinazione_Carico_Trappola.Data = Agenda.Data
            Movimento_Destinazione_Carico_Trappola.Piva = Agenda.Piva
            Movimento_Destinazione_Carico_Trappola.Sa_Cod = Sa_Cod_magazzino
            Movimento_Destinazione_Carico_Trappola.Id_Destinazione = fabbricatox_Cod
            Movimento_Destinazione_Carico_Trappola.Tipo = MAGAZZINO
            Movimento_Destinazione_Carico_Trappola.Qta = Movimento_Raccolta.Movimenti_Dettagli(0).Qta
            Movimento_Destinazione_Carico_Trappola.BaseCode = Agenda.BaseCode
            Movimento_Destinazione_Carico_Trappola.TopCode = Agenda.TopCode


            Movimento_Dettaglio_Carico_Trappola.Movimenti_Destinazioni.Add(Movimento_Destinazione_Carico_Trappola)
            Movimento_Carico.Movimenti_Dettagli.Add(Movimento_Dettaglio_Carico_Trappola)


            '-------------- aggiungo il movimento scarico all'agenda
            Agenda.Movimenti.Add(Movimento_Carico)
        Else
            '
            'un movimento dettaglio e una destinazione per ciascun impianto (semilavorati)

            For Each destinazione In Movimento_Raccolta.Movimenti_Dettagli(0).Movimenti_Destinazioni
                '------------------------------------------------
                '----- MOVIMENTI DETTAGLI SCARICO 
                '------------------------------------------------

                Dim Movimento_Dettaglio_Carico_Trappola As New Movimento_Dettaglio

                Movimento_Dettaglio_Carico_Trappola.Id_Agenda = Agenda.Id_Agenda
                Movimento_Dettaglio_Carico_Trappola.Piva = Agenda.Piva
                Movimento_Dettaglio_Carico_Trappola.Sa_Cod = Sa_Cod_magazzino 'ok per multicentro
                Movimento_Dettaglio_Carico_Trappola.Data = Agenda.Data
                Movimento_Dettaglio_Carico_Trappola.Elem_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Elem_Cod
                Movimento_Dettaglio_Carico_Trappola.Pro_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Pro_Cod
                Movimento_Dettaglio_Carico_Trappola.Mat_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Mat_Cod
                Movimento_Dettaglio_Carico_Trappola.Mov_Det_Des = "Carico di Semiavorati Aziendali da Raccolta"
                Movimento_Dettaglio_Carico_Trappola.Udm_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Udm_Cod
                Movimento_Dettaglio_Carico_Trappola.Qta = destinazione.Qta 'DIFFERENZA - SEMILAVORATO
                Dim Progr As Integer = GeneraCampionatura()
                Movimento_Dettaglio_Carico_Trappola.Cal_Cod = Progr 'negativo, puntatore
                Movimento_Dettaglio_Carico_Trappola.Lotto = Movimento_Raccolta.Movimenti_Dettagli(0).Lotto
                Movimento_Dettaglio_Carico_Trappola.Contabilizzato = NONCONTABILE
                Movimento_Dettaglio_Carico_Trappola.Pendente = enum_Pendenza.MovESENTE
                Movimento_Dettaglio_Carico_Trappola.Lav_Cod = Agenda.Lav_Cod
                Movimento_Dettaglio_Carico_Trappola.Cau_Mov = Movimento_Carico.Cau_Mov
                Movimento_Dettaglio_Carico_Trappola.Anno = 1900


                Movimento_Dettaglio_Carico_Trappola.Cod_Progetto = destinazione.parametroGenerico 'DIFFERENZA - SEMILAVORATO

                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI SCARICO 
                '------------------------------------------------
                'per generalizzata uno altrimenti per ciasdcun Appezza
                Dim Movimento_Destinazione_Carico_Trappola As New Movimento_Destinazione
                Movimento_Destinazione_Carico_Trappola.Id_Agenda = Agenda.Id_Agenda
                Movimento_Destinazione_Carico_Trappola.Data = Agenda.Data
                Movimento_Destinazione_Carico_Trappola.Piva = Agenda.Piva
                Movimento_Destinazione_Carico_Trappola.Sa_Cod = Sa_Cod_magazzino
                Movimento_Destinazione_Carico_Trappola.Appezza = 0 'destinazione.Appezza noo 'DIFFERENZA - SEMILAVORATO
                Movimento_Destinazione_Carico_Trappola.Id_Destinazione = fabbricatox_Cod
                Movimento_Destinazione_Carico_Trappola.Tipo = MAGAZZINO
                Movimento_Destinazione_Carico_Trappola.Qta = destinazione.Qta 'DIFFERENZA - SEMILAVORATO
                Movimento_Destinazione_Carico_Trappola.BaseCode = Agenda.BaseCode
                Movimento_Destinazione_Carico_Trappola.TopCode = Agenda.TopCode


                Movimento_Dettaglio_Carico_Trappola.Movimenti_Destinazioni.Add(Movimento_Destinazione_Carico_Trappola)
                Movimento_Carico.Movimenti_Dettagli.Add(Movimento_Dettaglio_Carico_Trappola)
            Next


            '-------------- aggiungo il movimento scarico all'agenda
            Agenda.Movimenti.Add(Movimento_Carico)

        End If


        Return True


    End Function

    Private Sub CreaOggettoAgendaCaricoPressoUDS(ByVal Agenda As Operazione_Agenda, ByVal Movimento_Carico_Creato As Movimento, ByVal Agenda_Scarico As Operazione_Agenda, ByVal Id_AgendaCaricoUDS_OLD As Integer, ByVal UDS_Piva As String, ByVal UDS_Sa_Cod As Integer, ByVal PivaAz As String)

        Dim Az_Dest As String = UDS_Piva
        Dim Centro_Dest As Integer = UDS_Sa_Cod

        Agenda_Scarico.Tipo_Operazione = Agenda.Tipo_Operazione
        Agenda_Scarico.Id_Agenda = Id_AgendaCaricoUDS_OLD
        Agenda_Scarico.Data = Agenda.Data
        Agenda_Scarico.Piva = Az_Dest
        Agenda_Scarico.Sa_Cod = Centro_Dest
        Agenda_Scarico.Lav_Cod = LAVCOD_CARICO
        Agenda_Scarico.Des_Lib = "Carico Raccolta per Lavorazione presso UDS (Az: " & PivaAz & ")"
        Agenda_Scarico.BaseCode = Agenda.BaseCode
        Agenda_Scarico.TopCode = Agenda.TopCode

        Dim Movimento_Scarico As New Movimento
        Movimento_Scarico.Id_Agenda = Agenda_Scarico.Id_Agenda
        Movimento_Scarico.Piva = Az_Dest
        Movimento_Scarico.Sa_Cod = Centro_Dest
        Movimento_Scarico.Data = Movimento_Carico_Creato.Data
        Movimento_Scarico.Ora = Movimento_Carico_Creato.Ora
        Movimento_Scarico.Lav_Cod = Agenda_Scarico.Lav_Cod
        Movimento_Scarico.Cau_Mov = CAU_CARICO
        Movimento_Scarico.Mov_Desc = "Carico Raccolta per Lavorazione presso UDS (Az: " & PivaAz & ")"
        Movimento_Scarico.BaseCode = Agenda_Scarico.BaseCode
        Movimento_Scarico.TopCode = Agenda_Scarico.TopCode

        For Each MovimentoDettaglioCarico As Movimento_Dettaglio In Movimento_Carico_Creato.Movimenti_Dettagli

            Dim MovimentoDettaglioScarico As New Movimento_Dettaglio
            MovimentoDettaglioScarico.Id_Agenda = Agenda_Scarico.Id_Agenda
            MovimentoDettaglioScarico.Piva = Az_Dest
            MovimentoDettaglioScarico.Sa_Cod = Centro_Dest
            MovimentoDettaglioScarico.Data = MovimentoDettaglioCarico.Data
            MovimentoDettaglioScarico.Elem_Cod = MovimentoDettaglioCarico.Elem_Cod
            MovimentoDettaglioScarico.Pro_Cod = MovimentoDettaglioCarico.Pro_Cod
            MovimentoDettaglioScarico.Cod_Progetto = MovimentoDettaglioCarico.Cod_Progetto
            MovimentoDettaglioScarico.Mat_Cod = MovimentoDettaglioCarico.Mat_Cod
            MovimentoDettaglioScarico.Mov_Det_Des = "Carico Raccolta per Lavorazione presso UDS (Az: " & PivaAz & ")"
            MovimentoDettaglioScarico.Udm_Cod = MovimentoDettaglioCarico.Udm_Cod
            MovimentoDettaglioScarico.Qta = MovimentoDettaglioCarico.Qta
            MovimentoDettaglioScarico.Cal_Cod = MovimentoDettaglioCarico.Cal_Cod
            MovimentoDettaglioScarico.Lotto = MovimentoDettaglioCarico.Lotto
            MovimentoDettaglioScarico.Contabilizzato = MovimentoDettaglioCarico.Contabilizzato
            MovimentoDettaglioScarico.Pendente = MovimentoDettaglioCarico.Pendente
            MovimentoDettaglioScarico.Lav_Cod = Agenda_Scarico.Lav_Cod
            MovimentoDettaglioScarico.Cau_Mov = CAU_SCARICO
            MovimentoDettaglioScarico.Anno = 1900

            For Each MovimentoDestinazioneCarico As Movimento_Destinazione In MovimentoDettaglioCarico.Movimenti_Destinazioni

                Dim MovimentoDestinazioneScarico As New Movimento_Destinazione
                MovimentoDestinazioneScarico.Id_Agenda = Agenda_Scarico.Id_Agenda
                MovimentoDestinazioneScarico.Data = MovimentoDestinazioneCarico.Data
                MovimentoDestinazioneScarico.Piva = Az_Dest
                MovimentoDestinazioneScarico.Sa_Cod = Centro_Dest
                Dim prg As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
                Dim Id_Destinazione As Integer = prg.LeggiMagazzino(Az_Dest, Centro_Dest, objParametri_Server)
                If Id_Destinazione = 0 Then
                    If Id_Destinazione = 0 Then
                        Id_Destinazione = CreaMagazzino(Agenda.BaseCode, Agenda.TopCode, Az_Dest, Centro_Dest)
                        If Id_Destinazione = 0 Then
                            Throw New Exception("Non c'è nessun magazzino nel dentro dell'UDS selezionata, creare prima un magazzino e ritentare.")
                        End If
                    End If
                End If
                MovimentoDestinazioneScarico.Id_Destinazione = Id_Destinazione ' devo trovare il magazzino
                MovimentoDestinazioneScarico.Tipo = MAGAZZINO
                MovimentoDestinazioneScarico.Qta = MovimentoDestinazioneCarico.Qta
                MovimentoDestinazioneScarico.BaseCode = Agenda_Scarico.BaseCode
                MovimentoDestinazioneScarico.TopCode = Agenda_Scarico.TopCode

                MovimentoDettaglioScarico.Movimenti_Destinazioni.Add(MovimentoDestinazioneScarico)
            Next
            Movimento_Scarico.Movimenti_Dettagli.Add(MovimentoDettaglioScarico)
        Next

        Agenda_Scarico.Movimenti.Add(Movimento_Scarico)
    End Sub


    Private Function CreaMagazzino(ByVal BaseCode As Integer, ByVal TopCode As Integer, ByRef Piva_Magazzino As String, ByRef Centro_Magazzino As Integer) As Integer
        Dim Ind_Des As String = ""
        Dim Frz_Des As String = ""
        Dim CAP As String = ""
        Dim Stato As String = ""
        Dim Comune As String = ""
        Dim Provincia As String = ""
        Dim Sigla_Prov As String = ""
        Dim pro_cod_istat As String = ""
        Dim com_cod_istat As String = ""
        Dim Cod_Regione As String = ""

        Dim x As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
        x.Indirizzo_from_PivaSaCod(Piva_Magazzino, Centro_Magazzino, Ind_Des, Frz_Des,
                                 CAP,
                                 Stato,
                                 Comune,
                                 Provincia,
                                 Sigla_Prov,
                                 pro_cod_istat,
                                 com_cod_istat,
                                 Cod_Regione,
                                 objParametri_Server
                                )

        Dim XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
        Dim XmlDoc2 As New XmlDocument
        Dim log2 As String
        Dim Fabbricato_Des = "Magazzino Cure"
        Dim XMLFAbbricato As XmlElement = XML_Anagrafe.XML_2_Fabbricati(log2,
                                                XmlDoc2,
                                                BaseCode,
                                                TopCode,
                                                enum_TipoOperazioneDB.Scrittura,
                                                Piva_Magazzino,
                                                Centro_Magazzino,
                                                0,
                                                Fabbricato_Des,
                                                TipiEnumerativi.enum_FabbricatiTipi.MagazzinoAziendale,
                                                1,
                                                pro_cod_istat,
                                                com_cod_istat,
                                                0,
                                                Ind_Des,
                                                "",
                                                CAP,
                                                "IT",
                                                "",
                                                Nothing,
                                                Nothing,
                                                Nothing,
                                                Nothing,
                                                Nothing,
                                                , , , , , , , , , ,
                                                TipiEnumerativi.enum_TitoloPossesso.Proprieta,
                                                , , , , , , , , , , , , , ,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                , , , , , , , )


        Dim Fabbricato_W As New AgronicaCoreAnagrafeBIZ.Fabbricato_W
        Dim fabbricato_cod As Integer
        Fabbricato_W.Fabbricato_Scrivi(XMLFAbbricato.OuterXml, Piva_Magazzino, Centro_Magazzino, fabbricato_cod, objParametri_Server)
        Return fabbricato_cod

    End Function

    Private Sub CreaOggettoAgendaScaricoProdottoPerInvioUDS(ByVal Agenda As Operazione_Agenda, ByVal Movimento_Carico_Creato As Movimento, ByVal Agenda_Scarico As Operazione_Agenda, ByVal Id_AgendaScaricoPErUDS_OLD As Integer, ByVal UDS_Piva As String, ByVal UDSnum As String, ByVal LOCALE As String)
        Agenda_Scarico.Tipo_Operazione = Agenda.Tipo_Operazione
        Agenda_Scarico.Id_Agenda = Id_AgendaScaricoPErUDS_OLD
        Agenda_Scarico.Data = Agenda.Data
        Agenda_Scarico.Piva = Agenda.Piva
        Agenda_Scarico.Sa_Cod = Movimento_Carico_Creato.Sa_Cod
        Agenda_Scarico.Lav_Cod = LAVCOD_SCARICO
        Agenda_Scarico.Des_Lib = "Scarico Raccolta per Lavorazione presso UDS " & UDS_Piva & " - " & UDSnum & " - " & LOCALE & ""
        Agenda_Scarico.BaseCode = Agenda.BaseCode
        Agenda_Scarico.TopCode = Agenda.TopCode

        Dim Movimento_Scarico As New Movimento
        Movimento_Scarico.Id_Agenda = Agenda_Scarico.Id_Agenda
        Movimento_Scarico.Piva = Movimento_Carico_Creato.Piva
        Movimento_Scarico.Sa_Cod = Movimento_Carico_Creato.Sa_Cod
        Movimento_Scarico.Data = Movimento_Carico_Creato.Data
        Movimento_Scarico.Ora = Movimento_Carico_Creato.Ora
        Movimento_Scarico.Lav_Cod = Agenda_Scarico.Lav_Cod
        Movimento_Scarico.Cau_Mov = CAU_SCARICO
        Movimento_Scarico.Mov_Desc = "Scarico Raccolta per Lavorazione presso UDS " & UDS_Piva & " - " & UDSnum & " - " & LOCALE & ""
        Movimento_Scarico.BaseCode = Agenda_Scarico.BaseCode
        Movimento_Scarico.TopCode = Agenda_Scarico.TopCode

        For Each MovimentoDettaglioCarico As Movimento_Dettaglio In Movimento_Carico_Creato.Movimenti_Dettagli

            Dim MovimentoDettaglioScarico As New Movimento_Dettaglio
            MovimentoDettaglioScarico.Id_Agenda = Agenda_Scarico.Id_Agenda
            MovimentoDettaglioScarico.Piva = MovimentoDettaglioCarico.Piva
            MovimentoDettaglioScarico.Sa_Cod = MovimentoDettaglioCarico.Sa_Cod
            MovimentoDettaglioScarico.Data = MovimentoDettaglioCarico.Data
            MovimentoDettaglioScarico.Elem_Cod = MovimentoDettaglioCarico.Elem_Cod
            MovimentoDettaglioScarico.Pro_Cod = MovimentoDettaglioCarico.Pro_Cod
            MovimentoDettaglioScarico.Cod_Progetto = MovimentoDettaglioCarico.Cod_Progetto
            MovimentoDettaglioScarico.Mat_Cod = MovimentoDettaglioCarico.Mat_Cod
            MovimentoDettaglioScarico.Mov_Det_Des = "Scarico Raccolta per Lavorazione presso UDS " & UDS_Piva & " - " & UDSnum & " - " & LOCALE & ""
            MovimentoDettaglioScarico.Udm_Cod = MovimentoDettaglioCarico.Udm_Cod
            MovimentoDettaglioScarico.Qta = MovimentoDettaglioCarico.Qta
            MovimentoDettaglioScarico.Cal_Cod = MovimentoDettaglioCarico.Cal_Cod
            MovimentoDettaglioScarico.Lotto = MovimentoDettaglioCarico.Lotto
            MovimentoDettaglioScarico.Contabilizzato = MovimentoDettaglioCarico.Contabilizzato
            MovimentoDettaglioScarico.Pendente = MovimentoDettaglioCarico.Pendente
            MovimentoDettaglioScarico.Lav_Cod = Agenda_Scarico.Lav_Cod
            MovimentoDettaglioScarico.Cau_Mov = CAU_SCARICO
            MovimentoDettaglioScarico.Anno = 1900

            For Each MovimentoDestinazioneCarico As Movimento_Destinazione In MovimentoDettaglioCarico.Movimenti_Destinazioni

                Dim MovimentoDestinazioneScarico As New Movimento_Destinazione
                MovimentoDestinazioneScarico.Id_Agenda = Agenda_Scarico.Id_Agenda
                MovimentoDestinazioneScarico.Data = MovimentoDestinazioneCarico.Data
                MovimentoDestinazioneScarico.Piva = MovimentoDestinazioneCarico.Piva
                MovimentoDestinazioneScarico.Sa_Cod = MovimentoDestinazioneCarico.Sa_Cod
                MovimentoDestinazioneScarico.Id_Destinazione = MovimentoDestinazioneCarico.Id_Destinazione
                MovimentoDestinazioneScarico.Tipo = MAGAZZINO
                MovimentoDestinazioneScarico.Qta = MovimentoDestinazioneCarico.Qta
                MovimentoDestinazioneScarico.BaseCode = Agenda_Scarico.BaseCode
                MovimentoDestinazioneScarico.TopCode = Agenda_Scarico.TopCode

                MovimentoDettaglioScarico.Movimenti_Destinazioni.Add(MovimentoDestinazioneScarico)
            Next
            Movimento_Scarico.Movimenti_Dettagli.Add(MovimentoDettaglioScarico)
        Next

        Agenda_Scarico.Movimenti.Add(Movimento_Scarico)
    End Sub

    Private Function GeneraIdLottoRaccolta(ByVal piva As String) As String
        'copiato da pagina raccolta
        Dim seq As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim id As Integer = seq.NuovoId_Tabella("GeneraIdLottoRaccolta", 0, 2000000000, objParametri_Server)
        Dim risp As String = CStr(id).PadLeft(10, "0") '10 caratteri per id
        risp = risp & "/" & piva
        risp = "LR/" & risp 'Lotto Raccolta
        Return risp
    End Function

    Private Function GeneraCampionatura() As Integer
        Dim Progr As Integer
        Dim Materie_Prime_Campionature As New AgronicaCoreContabDAL.Materie_Prime_Campion_W
        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Progr = ObjSequenze.NuovoId_Tabella( _
                    "Materie_Prime_Campionature", _
                    0, _
                    2000000000, _
                    objParametri_Server)
        Progr = -Math.Abs(Progr)
        Materie_Prime_Campionature.Scrivi( _
                                                Progr, _
                                                "calibro", _
                                                12, _
                                                0, _
                                                0, _
                                                "Indefinito", _
                                                0, _
                                                0, _
                                                "", _
                                                AGRODATAINIZIO, _
                                                AGRODATAFINE, _
                                                objParametri_Server)
        Return Progr
    End Function


#End Region

#Region "Cura"

    Private Function CreaSalvaCura(ByVal IDInfornaturaImportata As String, _
                                   ByVal dp As AgronicaCoreDataProvider.DataProvider, ByVal Fabbricati_R As AgronicaCoreAnagrafeDAL.Fabbricati_R, ByVal objAgendaScrivi As Agenda_Operazione_Helper, _
                                   ByVal Mat_Cod_Prod_InCura As Integer, ByVal Mat_Cod_Prod_Curato As Integer, _
                                   ByVal UDS As Integer, ByVal LOCALE As Integer, ByVal INIZIO As String, ByVal FINE As String, _
                                   ByVal CODCOLTIV As String, ByVal NUMINIZ As Integer, ByVal NUMFINE As Integer, _
                                   ByVal APPEZZAMENTO As Integer, ByVal CORONA As Integer, _
                                   ByVal dataInizio As Date, ByVal dataFine As Date, _
                                   ByVal UDS_Piva As String, ByVal UDS_Sa_Cod As Integer, ByVal UDS_Fabbricato_Cod As Integer, _
                                   ByVal Azienda_Piva As String, ByVal Azianda_Sa_Cod As Integer, ByVal Azienda_Magazzino_Sa_Cod As Integer, ByVal Azienda_Magazzino_Fabbricato_Cod As Integer, _
                                   ByVal LottoRaccolta As String, ByVal Mat_Cod_Raccolto As Integer, ByVal Cal_Cod_Raccolto As Integer, ByVal Peso_Raccolto As Double) As String
        Dim strsql As String
        strsql = "SELECT *   FROM [__ImportazioneCure]  where UDS=" & UDS & " AND LOCALE=" & LOCALE & " AND INIZIO='" & Agro_SQL_SaveText(INIZIO) & "' AND FINE='" & Agro_SQL_SaveText(FINE) & "' AND CODCOLTIV='" & Agro_SQL_SaveText(CODCOLTIV) & "'  AND NUMINIZ=" & NUMINIZ & " AND NUMFINE=" & NUMFINE & " AND CORONA='" & Agro_SQL_SaveText(CORONA) & "' AND " & FiltroTabella & "  order by cartno  "
        Dim dt3 As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

        'Infornature
        Dim DtInfornature As DataTable = InizializzaDtInfornature()
        Dim drinf As DataRow = DtInfornature.NewRow
        drinf.Item("Id_Infornatura") = generaIdInfronatura()
        drinf.Item("Piva") = UDS_Piva
        drinf.Item("Rag_Soc") = "FAT"
        drinf.Item("Sa_Cod") = UDS_Sa_Cod
        drinf.Item("Sa_nome") = UDS
        drinf.Item("Fabbricato_Cod") = UDS_Fabbricato_Cod
        drinf.Item("Fabbricato_Des") = LOCALE
        drinf.Item("Cassoni") = "0"
        drinf.Item("Data_Inizio_Cura") = dataInizio
        drinf.Item("Ora_Inizio_Cura") = dataInizio
        'Dati Magazzino
        Dim Id_Destinazione As Integer = Fabbricati_R.LeggiMagazzino(UDS_Piva, UDS_Sa_Cod, objParametri_Server)
        If Id_Destinazione = 0 Then
            If Id_Destinazione = 0 Then
                Throw New Exception("Non c'è nessun magazzino nel dentro dell'UDS selezionata, creare prima un magazzino e ritentare.")
            End If
        End If
        drinf.Item("Magazzino_Sa_Cod") = UDS_Sa_Cod
        drinf.Item("Magazzino_Sa_Nome") = UDS
        drinf.Item("Magazzino_Fabbricato_Cod") = Id_Destinazione
        drinf.Item("Magazzino_Fabbricato_Des") = ""
        drinf.Item("Magazzino_Cat_Cod") = "0"
        drinf.Item("Magazzino_Cat_Des") = ""
        drinf.Item("Magazzino_Pro_Cod") = "0"
        drinf.Item("Magazzino_Pro_Des") = ""
        drinf.Item("Magazzino_Mat_Cod") = Mat_Cod_Raccolto
        drinf.Item("Magazzino_Lotto_Int") = ""
        drinf.Item("Magazzino_Lotto_Acc") = LottoRaccolta
        drinf.Item("Magazzino_Cod_Progetto") = "0"
        drinf.Item("Magazzino_Param_Des") = ""
        drinf.Item("Magazzino_Cal_Cod") = Cal_Cod_Raccolto
        drinf.Item("Magazzino_Cal_Des") = ""
        drinf.Item("Magazzino_Udm_Cod") = CInt(enum_UnitaMisura.KG)
        drinf.Item("Magazzino_Udm_Des") = "Kilogrammi"
        drinf.Item("Magazzino_Qta") = Peso_Raccolto
        DtInfornature.Rows.Add(drinf)

        'sfornature
        Dim DtSfornature As DataTable = InizializzaDtSfornature()

        Dim pesoparziale As Integer = 0
        Dim i As Integer = 0
        For Each dr3 As DataRow In dt3.Rows
            Dim drsf As DataRow = DtSfornature.NewRow

            Dim Id_Lotto As String = dr3.Item("ChiaveId").trim 'generaIdCollo(dataFineCura)
            Id_Lotto = "02" & Id_Lotto '2 caratteri per importati metto 02
            Id_Lotto = "LN/" & dataInizio.Year.ToString.Substring(2, 2) & Id_Lotto '2 caratteri per anno

            Dim Id_Collo As String = "LU/" & dataInizio.Year.ToString.Substring(2, 2) & "/02/" & Azienda_Piva & "/" & CStr(CInt(dr3.Item("CARTNO"))).PadLeft(5, "0") & ""

            'controllo se esiste lotto

            drsf.Item("Id_Infornatura") = drinf.Item("Id_Infornatura")
            drsf.Item("Piva") = UDS_Piva

            drsf.Item("Cassoni") = "0"

            drsf.Item("Data_Fine_Cura") = dataFine
            drsf.Item("Ora_Fine_Cura") = dataFine
            drsf.Item("Id_Lotto") = Id_Lotto
            drsf.Item("ColloTemp") = Id_Collo

            drsf.Item("Magazzino_Sa_Cod") = UDS_Sa_Cod
            drsf.Item("Magazzino_Sa_Nome") = ""
            drsf.Item("Magazzino_Fabbricato_Cod") = Id_Destinazione
            drsf.Item("Magazzino_Fabbricato_Des") = ""

            'usti solo per il ripristino valori da agenda e per la creazione dell'oggetto da salvare
            drsf.Item("Collo") = Id_Collo

            pesoparziale = dr3.Item("Peso") / dt3.Rows.Count
            If i = dt3.Rows.Count - 1 Then
                'all'ultimo uniformo i pesi
                pesoparziale = dr3.Item("Peso") - (pesoparziale * (dt3.Rows.Count - 1))
            End If
            drsf.Item("Peso") = pesoparziale
            DtSfornature.Rows.Add(drsf)
            i = i + 1
        Next

        Dim messaggio_errore As String = ""
        Dim Agenda As Operazione_Agenda = creaOggettiAgendaLavorazione(messaggio_errore, LottoRaccolta, DtInfornature, DtSfornature, Mat_Cod_Prod_Curato, "Tabacco Curato", Mat_Cod_Prod_InCura, "Tabacco in cura")
        If IsNothing(Agenda) Then
            Throw New Exception("NonÈStatoPossibileCreareLOperazione")
        End If

        Agenda.Blocco_Flag = 1
        Agenda.Blocco_Username = IDInfornaturaImportata

        Dim Id_Agenda_Carico As Integer = 0
        Dim Id_Agenda_Scarico As Integer = 0
        Dim Id_Agenda As Integer
        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)
        Dim MovimentiCure As New List(Of Movimento)
        For Each Movimento As Movimento In Agenda.Movimenti
            If Movimento.Cau_Mov = "7300" Then
                If Movimento.Movimenti_Dettagli.Count > 0 Then
                    If Movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0 Then
                        If Movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Tipo = MAGAZZINO Then
                            MovimentiCure.Add(Movimento)
                        End If
                    End If
                End If
            End If
        Next

        '-----------------------------------------------------------------------------------
        'creo l'agenda con mov scarico dei prodotti caricati
        If MovimentiCure.Count > 0 Then



            '--------------------------------------------------------------------------------------------------------------------------------
            '--------------SALVO-MODIFICO SCARICO PER AZ--------------------------------------------------------------------------
            '--------------------------------------------------------------------------------------------------------------------------------

            Dim Tipo_Operazione As Integer = enum_TipoOperazioneDB.Scrittura
            Dim Provenienza As String = Azienda_Piva
            Dim ProvenienzaDesc = Provenienza & " " & New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(Provenienza, objParametri_Server)
            Dim agendaScarico As Operazione_Agenda = AgronicaCoreModello.OperazioneDiCuraClasse.CreaOggettoAgendaScaricoProdottoPerInvioAziendaOrigine(MovimentiCure, _
                                                                   Id_Agenda_Scarico, _
                                                                   Tipo_Operazione, ProvenienzaDesc)
            Id_Agenda_Scarico = objAgendaScrivi.Scrivi(agendaScarico, objParametri_Server)

            '--------------------------------------------------------------------------------------------------------------------------------
            '--------------SALVO-MODIFICO SCARICO AZ--------------------------------------------------------------------------
            '--------------------------------------------------------------------------------------------------------------------------------
            '-----------------------------------------------------------------------------------

            Dim ProvenienzaSa_Cod_magazzino As Integer = Azienda_Magazzino_Sa_Cod
            Dim ProvenienzaFabbricato_Cod As Integer = Azienda_Magazzino_Fabbricato_Cod

            Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
            Dim udsDescr As String = New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(MovimentiCure(0).Piva, objParametri_Server)
            udsDescr = udsDescr & " - " & New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(MovimentiCure(0).Piva, MovimentiCure(0).Sa_Cod, objParametri_Server)
            Dim agendaCarico As Operazione_Agenda = AgronicaCoreModello.OperazioneDiCuraClasse.CreaOggettoAgendaCaricoPressoAziendaOrigine(MovimentiCure, _
                                                        Provenienza, _
                                                        ProvenienzaSa_Cod_magazzino, _
                                                        ProvenienzaFabbricato_Cod, _
                                                        Id_Agenda_Carico, Tipo_Operazione, udsDescr)

            Id_Agenda_Carico = objAgendaScrivi.Scrivi(agendaCarico, objParametri_Server)


            '--------------------------------------------------------------------------------------------------------------------------------
            '--------------SALVO-MODIFICO RIFERIMENTI--------------------------------------------------------------------------
            '--------------------------------------------------------------------------------------------------------------------------------
            Dim RifSc As New Movimento_Dettaglio_Riferimento
            Dim RifCa As New Movimento_Dettaglio_Riferimento
            RifSc.Piva_Rif = Agenda.Piva
            RifCa.Piva_Rif = Agenda.Piva
            RifSc.Sa_Cod_Rif = Agenda.Sa_Cod
            RifCa.Sa_Cod_Rif = Agenda.Sa_Cod
            RifSc.Id_Agenda_Rif = Id_Agenda
            RifCa.Id_Agenda_Rif = Id_Agenda
            RifSc.Id_Mov_Rif = -1
            RifCa.Id_Mov_Rif = -1
            RifSc.Id_Mov_Det_Rif = -1
            RifCa.Id_Mov_Det_Rif = -1
            RifSc.Lav_Cod_Rif = Agenda.Lav_Cod
            RifCa.Lav_Cod_Rif = Agenda.Lav_Cod
            RifSc.Piva = agendaScarico.Piva
            RifCa.Piva = agendaCarico.Piva
            RifSc.Sa_Cod = agendaScarico.Sa_Cod
            RifCa.Sa_Cod = agendaCarico.Sa_Cod
            RifSc.Id_Agenda = Id_Agenda_Scarico
            RifCa.Id_Agenda = Id_Agenda_Carico
            RifSc.Id_Mov = -1
            RifCa.Id_Mov = -1
            RifSc.Id_Mov_Det = -1
            RifCa.Id_Mov_Det = -1
            RifSc.Lav_Cod = LAVCOD_SCARICO
            RifCa.Lav_Cod = LAVCOD_CARICO
            Dim objRifScrivi As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
            'scriv i riferimenti
            objRifScrivi.Scrivi(RifSc, objParametri_Server)
            objRifScrivi.Scrivi(RifCa, objParametri_Server)


        End If
        Return strsql
    End Function

    Private Shared Function InizializzaDtInfornature() As DataTable
        Dim Dt As New DataTable("Infornature")

        'dato infornatura (prodottto in combo)
        Dt.Columns.Add("Id_Infornatura", GetType(String))
        Dt.Columns.Add("Piva", GetType(String))
        Dt.Columns.Add("Rag_Soc", GetType(String))
        Dt.Columns.Add("Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Sa_nome", GetType(String))
        Dt.Columns.Add("Fabbricato_Cod", GetType(Integer))
        Dt.Columns.Add("Fabbricato_Des", GetType(String))
        Dt.Columns.Add("Cassoni", GetType(Integer))
        Dt.Columns.Add("Data_Inizio_Cura", GetType(String))
        Dt.Columns.Add("Ora_Inizio_Cura", GetType(String))

        'Dati Magazzino
        Dt.Columns.Add(New DataColumn("Magazzino_Sa_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Sa_Nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Fabbricato_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Fabbricato_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Cat_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Cat_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Pro_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Pro_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Mat_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Lotto_Int", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Lotto_Acc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Cod_Progetto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Param_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Cal_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Cal_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Udm_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Udm_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Magazzino_Qta", GetType(String)))
        Return Dt
    End Function

    Private Shared Function InizializzaDtSfornature() As DataTable
        Dim Dt As New DataTable("Cure")

        Dt.Columns.Add("Id_Infornatura", GetType(String))
        Dt.Columns.Add("Piva", GetType(String))

        Dt.Columns.Add("Cassoni", GetType(Integer))

        Dt.Columns.Add("Data_Fine_Cura", GetType(String))
        Dt.Columns.Add("Ora_Fine_Cura", GetType(String))
        Dt.Columns.Add("Id_Lotto", GetType(String))
        Dt.Columns.Add("ColloTemp", GetType(String))

        Dt.Columns.Add("Magazzino_Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Magazzino_Sa_Nome", GetType(String))
        Dt.Columns.Add("Magazzino_Fabbricato_Cod", GetType(Integer))
        Dt.Columns.Add("Magazzino_Fabbricato_Des", GetType(String))

        'usti solo per il ripristino valori da agenda e per la creazione dell'oggetto da salvare
        Dt.Columns.Add("Collo", GetType(String))
        Dt.Columns.Add("Peso", GetType(String))

        Return Dt
    End Function

    Private Function creaOggettiAgendaLavorazione(ByRef messaggioerrore As String, ByRef AgendaLottoRacc As String, DtInfornature As DataTable, DtSfornature As DataTable, ProdottoTabaccoCurato As Integer, NomeProdottoTabaccoCurato As String, ProdottoTabaccoInCura As Integer, NomeProdottoTabaccoInCura As String) As Operazione_Agenda


        Dim veg_cod As Integer = 335 'tabacco

        If DtInfornature.Rows.Count = 0 Then
            Throw New Exception("Ci deve essere almeno una infornatura")
        End If
        If DtSfornature.Rows.Count = 0 Then
            Throw New Exception("Ci deve essere almeno una sfornatura.")
        End If


        Dim note As String = "Operazione di cura in essiccatoio"
        Dim Agenda As Operazione_Agenda
        Agenda = AgronicaCoreModello.OperazioneDiCuraClasse.CreaOggettoAgenda_CURATABACCO(0, objParametri_Server, _
                                        enum_TipoOperazioneDB.Scrittura, _
                                        ASG_ProgressivoGIAS, _
                                        DtInfornature, DtSfornature, _
                                        ProdottoTabaccoInCura, _
                                        ProdottoTabaccoCurato, _
                                         note, "", veg_cod, NomeProdottoTabaccoCurato, NomeProdottoTabaccoInCura, _
                                        enum_UnitaMisura.KG, AgendaLottoRacc)



        Return Agenda


    End Function

    Private Function generaIdInfronatura() As String
        Dim seq As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim id As Integer = seq.NuovoId_Tabella("generaIdInfronatura", 0, 2000000000, objParametri_Server)
        Dim risp As String = CStr(id).PadLeft(10, "0") '10 caratteri per id
        risp = "LC/" & risp
        Return risp
    End Function

#End Region

End Class
