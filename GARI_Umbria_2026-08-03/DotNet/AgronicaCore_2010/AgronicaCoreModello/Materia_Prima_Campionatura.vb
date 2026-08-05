Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Namespace OperazioneAgenda_Temp

    Public Class Materia_Prima_Campionatura

        Sub New(ByVal tipoInput As String, ByVal dataMovimento As Date)

            Progressivo = 0
            Tipo = tipoInput
            Tipo_Cod = 0
            Udm_Cod = 0

            Val_Cod = "0"
            Descrizione = ""
            Validita_Inizio = dataMovimento
            Validita_Fine = AGRODATAFINE
            Progressivo_Origine = 0
            Piva_SuperUser_Origine = ""
            Peso_Campione = 0
            ChkStima = 0
            ChkTara_Campionatura = 0
            Tara_Campionatura = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub

        Sub New()

            Progressivo = 0
            Tipo = ""
            Tipo_Cod = 0
            Udm_Cod = 0

            Val_Cod = "0"
            Descrizione = ""
            Validita_Inizio = AGRODATAINIZIO
            Validita_Fine = AGRODATAFINE
            Progressivo_Origine = 0
            Piva_SuperUser_Origine = ""
            Peso_Campione = 0
            ChkStima = 0
            ChkTara_Campionatura = 0
            Tara_Campionatura = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub

        Public Property Progressivo As Integer

        Public Property Tipo As String

        Public Property Tipo_Cod As Integer

        Public Property Udm_Cod As Integer

        Public Property Val_Cod As String

        Public Property Descrizione As String

        Public Property Validita_Inizio As Date

        Public Property Validita_Fine As Date

        Public Property Progressivo_Origine As Integer

        Public Property Piva_SuperUser_Origine As String

        Public Property Peso_Campione As Decimal

        Public Property ChkStima As Integer

        Public Property ChkTara_Campionatura As Integer

        Public Property Tara_Campionatura As Decimal

        Public Property Data_Creazione As DateTime
        
        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

        Public Property TopCode As Integer

        Public Property BaseCode As Integer
        
    End Class

    Public Class Agenda_Materie_Prime_Campionature_Helper

        Public Function Scrivi(ByVal objMateriaPrimaCampionatura As Materia_Prima_Campionatura,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean


            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Dim progressivo As Integer

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                progressivo = objMateriaPrimaCampionatura.Progressivo

                If progressivo = 0 Then

                    Dim objSequenze As New Agro_Sequenze

                    progressivo = objSequenze.NuovoId_Tabella("Materie_Prime_Campionature",
                                                              objMateriaPrimaCampionatura.BaseCode,
                                                              objMateriaPrimaCampionatura.TopCode,
                                                              objParametri)

                    progressivo = -Math.Abs(progressivo) 'valore negativo

                    objSequenze = Nothing

                End If

                Dim objMateriePrimeCampionature As New AgronicaCoreContabDAL.Materie_Prime_Campion_W

                objMateriePrimeCampionature.Scrivi(progressivo,
                                                   objMateriaPrimaCampionatura.Tipo,
                                                   objMateriaPrimaCampionatura.Tipo_Cod,
                                                   objMateriaPrimaCampionatura.Udm_Cod,
                                                   objMateriaPrimaCampionatura.Val_Cod,
                                                   objMateriaPrimaCampionatura.Descrizione,
                                                   objMateriaPrimaCampionatura.Peso_Campione,
                                                   objMateriaPrimaCampionatura.Progressivo_Origine,
                                                   objMateriaPrimaCampionatura.Piva_SuperUser_Origine,
                                                   objMateriaPrimaCampionatura.Validita_Inizio,
                                                   objMateriaPrimaCampionatura.Validita_Fine,
                                                   objParametri,
                                                   objMateriaPrimaCampionatura.ChkStima,
                                                   objMateriaPrimaCampionatura.ChkTara_Campionatura,
                                                   objMateriaPrimaCampionatura.Tara_Campionatura,
                                                   Data_creazione:=objMateriaPrimaCampionatura.Data_Creazione,
                                                   username_creazione:=objMateriaPrimaCampionatura.Username_Creazione)

                 objMateriePrimeCampionature = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Materie_Prime_Campionature_Helper.Scrivi() ] : " & ex.Message)
            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function Cancella(ByVal progressivo As Integer,
                                 ByVal tipo As String,
                                 ByVal tipoCod As Integer,
                                 ByVal udmCod As Integer,
                                 ByVal flagSoloOrfani As Boolean,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False
            
            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


                Dim objMateriePrimeCampionature As New AgronicaCoreContabDAL.Materie_Prime_Campion_W

                xRisp = objMateriePrimeCampionature.Cancella(progressivo, tipo, tipoCod, udmCod, flagSoloOrfani, "", objParametri)

                objMateriePrimeCampionature = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Materie_Prime_Campionature_Helper.Cancella() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function

        Public Function Modifica(ByVal progressivo As Integer,
                                 ByVal tipo As String,
                                 ByVal tipoCod As Integer,
                                 ByVal udmCod As Integer,
                                 ByVal objParametri As AgronicaCoreParametri,
                                 Optional ByVal valCod As String = Nothing,
                                 Optional ByVal descrizione As String = Nothing,
                                 Optional ByVal validitaInizio As Date? = Nothing,
                                 Optional ByVal validitaFine As Date? = Nothing,
                                 Optional ByVal progressivoOrigine As Integer? = Nothing,
                                 Optional ByVal pivaSuperUserOrigine As String = Nothing,
                                 Optional ByVal pesoCampione As Decimal? = Nothing,
                                 Optional ByVal chkStima As Integer? = Nothing,
                                 Optional ByVal chkTaraCampionatura As integer? = Nothing,
                                 Optional ByVal taraCampionatura as Decimal? = Nothing,
                                 Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                 Optional ByVal usernameModifica As String = ""
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


                Dim objMateriePrimeCampionature As New AgronicaCoreContabDAL.Materie_Prime_Campion_W

                xRisp = objMateriePrimeCampionature.Modifica(progressivo, tipo, tipoCod, udmCod, objParametri,
                                                             Val_Cod:=valCod,
                                                             Descrizione:=descrizione,
                                                             Validita_Inizio:=validitaInizio,
                                                             Validita_Fine:=validitaFine,
                                                             Progressivo_Origine:=progressivoOrigine,
                                                             Piva_SuperUser_Origine:=pivaSuperUserOrigine,
                                                             Peso_Campione:=pesoCampione,
                                                             ChkStima:=chkStima,
                                                             ChkTara_Campionatura:=chkTaraCampionatura,
                                                             Tara_Campionatura:=taraCampionatura,
                                                             Data_modifica:=dataModifica,
                                                             username_modifica:=usernameModifica)

                objMateriePrimeCampionature = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Materie_Prime_Campionature_Helper.Modifica() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function

        ''' <summary>
        ''' Cancella e riscrive la voce, alterando i parametri passati; utile quando si devono modificare anche le colonne PK
        ''' </summary>
        Public Function CancellaRiscriviMod(ByVal progressivo As Integer,
                                            ByVal tipo As String,
                                            ByVal tipoCod As Integer,
                                            ByVal udmCod As Integer,
                                            ByVal objParametri As AgronicaCoreParametri,
                                            Optional ByVal flagVerificaPreventiva As Boolean = True,
                                            Optional ByVal listMatPrime As List(Of Materia_Prima_Campionatura) = Nothing,
                                            Optional ByVal progressivoNew As Integer? = Nothing,
                                            Optional ByVal tipoNew As String = Nothing,
                                            Optional ByVal tipoCodNew As Integer? = Nothing,
                                            Optional ByVal udmCodNew As Integer? = Nothing,
                                            Optional ByVal valCod As String = Nothing,
                                            Optional ByVal descrizione As String = Nothing,
                                            Optional ByVal validitaInizio As Date? = Nothing,
                                            Optional ByVal validitaFine As Date? = Nothing,
                                            Optional ByVal progressivoOrigine As Integer? = Nothing,
                                            Optional ByVal pivaSuperUserOrigine As String = Nothing,
                                            Optional ByVal pesoCampione As Decimal? = Nothing,
                                            Optional ByVal chkStima As Integer? = Nothing,
                                            Optional ByVal chkTaraCampionatura As integer? = Nothing,
                                            Optional ByVal taraCampionatura as Decimal? = Nothing,
                                            Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                            Optional ByVal usernameModifica As String = ""
                                            ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False
            Dim eseguiCancScrivi As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                'se non mi è già arrivata da fuori devo leggere la riga
                If IsNothing(listMatPrime) Then
                    'Lettura preventiva della riga
                    listMatPrime = Leggi(progressivo, tipo, tipoCod, udmCod, objParametri)
                    
                End If
                

                If Not IsNothing(listMatPrime) Then
                    
                    If listMatPrime.Count > 1 Then
                        Throw New Exception("Impossibile cancellare e riscrivere, sono presenti più righe.")
                        
                    ElseIf listMatPrime.Count = 1 Then
                        
                        Dim objMatPrimNew As Materia_Prima_Campionatura = listMatPrime(0)
                        
                        'Meglio verificare se effettivamente li devo modficare?!?
                        If flagVerificaPreventiva = True Then
                            'per evitare di fare operazioni inutili verifico se i nuovi valori chiave sono effettivamente diversi da quelli vecchi
                            'se non lo sono è inutile fare cancellazione e riscrittura, posso direttamente lanciare l'update
                            
                            If eseguiCancScrivi = False AndAlso Not IsNothing(progressivoNew) AndAlso objMatPrimNew.Progressivo <> progressivoNew Then
                                eseguiCancScrivi = True
                            End If
                    
                            If eseguiCancScrivi = False AndAlso Not IsNothing(tipoNew) AndAlso objMatPrimNew.Tipo <> tipoNew Then
                                eseguiCancScrivi = True
                            End If
                    
                            If eseguiCancScrivi = False AndAlso Not IsNothing(tipoCodNew) AndAlso objMatPrimNew.Tipo_Cod <> tipoCodNew Then
                                eseguiCancScrivi = True
                            End If

                            If eseguiCancScrivi = False AndAlso Not IsNothing(udmCodNew) AndAlso objMatPrimNew.Udm_Cod <> udmCodNew Then
                                eseguiCancScrivi = True
                            End If

                            If eseguiCancScrivi = False AndAlso Not IsNothing(valCod) AndAlso objMatPrimNew.Val_Cod <> valCod Then
                                eseguiCancScrivi = True
                            End If
                        Else
                            eseguiCancScrivi = True
                        End If
                        
                        If eseguiCancScrivi = True Then
                            
                            'Cancellazione della riga
                            'FlagSoloOrfani = False, perchè questo è altamente probabile che sia usato da qualche parte, ma lo devo eliminare cmq
                            xRisp = Cancella(objMatPrimNew.Progressivo,
                                             objMatPrimNew.Tipo,
                                             objMatPrimNew.Tipo_Cod,
                                             objMatPrimNew.Udm_Cod,
                                             False, objParametri)
                    
                            If xRisp = True Then
                    
                                'Nuove chiavi
                                If Not IsNothing(progressivoNew) Then
                                    objMatPrimNew.Progressivo = progressivoNew
                                End If
                    
                                If Not IsNothing(tipoNew) Then
                                    objMatPrimNew.Tipo = tipoNew
                                End If
                    
                                If Not IsNothing(tipoCodNew) Then
                                    objMatPrimNew.Tipo_Cod = tipoCodNew
                                End If
                    
                                If Not IsNothing(udmCodNew) Then
                                    objMatPrimNew.Udm_Cod = udmCodNew
                                End If
                    
                                'Altre colonne
                                If Not IsNothing(valCod) Then
                                    objMatPrimNew.Val_Cod = valCod
                                End If
                    
                                If Not IsNothing(descrizione) Then
                                    objMatPrimNew.Descrizione = descrizione
                                End If
                    
                                If Not IsNothing(validitaInizio) Then
                                    objMatPrimNew.Validita_Inizio = validitaInizio
                                End If
                    
                                If Not IsNothing(validitaFine) Then
                                    objMatPrimNew.Validita_Fine = validitaFine
                                End If
                    
                                If Not IsNothing(progressivoOrigine) Then
                                    objMatPrimNew.Progressivo_Origine = progressivoOrigine
                                End If
                    
                                If Not IsNothing(pivaSuperUserOrigine) Then
                                    objMatPrimNew.Piva_SuperUser_Origine = pivaSuperUserOrigine
                                End If
                    
                                If Not IsNothing(pesoCampione) Then
                                    objMatPrimNew.Peso_Campione = pesoCampione
                                End If
                    
                                If Not IsNothing(chkStima) Then
                                    objMatPrimNew.ChkStima = chkStima
                                End If
                    
                                If Not IsNothing(chkTaraCampionatura) Then
                                    objMatPrimNew.ChkTara_Campionatura = chkTaraCampionatura
                                End If
                    
                                If Not IsNothing(taraCampionatura) Then
                                    objMatPrimNew.Tara_Campionatura = taraCampionatura
                                End If
                    
                                'Scrittura della riga
                                xRisp = Scrivi(objMatPrimNew, objParametri)
                            End If
                        Else
                            'Eseguo direttamente la modifica, tanto non sono cambiate le colonne chiave
                            xRisp = Modifica(objMatPrimNew.Progressivo,
                                             objMatPrimNew.Tipo, objMatPrimNew.Tipo_Cod,
                                             objMatPrimNew.Udm_Cod, objParametri,
                                             valCod, descrizione, validitaInizio, validitaFine,
                                             progressivoOrigine,pivaSuperUserOrigine,
                                             pesoCampione, chkStima, chkTaraCampionatura, taraCampionatura,
                                             dataModifica, usernameModifica)
    
                        End If
                        
                    End If
                    
                End If               

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Materie_Prime_Campionature_Helper.CancellaRiscriviMod() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function
        
        Public Function Leggi(ByVal progressivo As Integer,
                              ByVal tipo As String,
                              ByVal tipoCod As Integer,
                              ByVal udmCod As Integer,
                              ByVal objParametri As AgronicaCoreParametri
                              ) As List(Of Materia_Prima_Campionatura)

            Dim flagConnessione As Boolean = False

            Dim listaMateriePrimeCampionature As New List(Of Materia_Prima_Campionatura)
            Dim objMateriaPrimaCampionatura As Materia_Prima_Campionatura

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '--------------------------------------------------------
                '-------- MATERIE_PRIME_CAMPIONATURE --------------------
                '--------------------------------------------------------
                Dim materiePrimeCampionatureR = New AgronicaCoreContabDAL.Materie_Prime_Campionature_R
                Dim dtMateriePrimeCamp As DataTable

                dtMateriePrimeCamp = materiePrimeCampionatureR.Leggi(progressivo,
                                                                     tipo,
                                                                     tipoCod,
                                                                     udmCod,
                                                                     Progressivo_Origine:=0,
                                                                     Piva_SuperUser_Origine:="",
                                                                     Flag_AncheImportatati:=True,
                                                                     xSelezioneVariabile:=enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                     xFiltroAggiuntivo:="",
                                                                     xOrderBy:="",
                                                                     objParametri:=objParametri)

                materiePrimeCampionatureR = Nothing

                If dtMateriePrimeCamp.Rows.Count > 0 Then

                    For j = 0 To dtMateriePrimeCamp.Rows.Count - 1

                        objMateriaPrimaCampionatura = New Materia_Prima_Campionatura With {
                            .Progressivo = dtMateriePrimeCamp.Rows(j).Item("Progressivo"),
                            .Tipo = dtMateriePrimeCamp.Rows(j).Item("Tipo"),
                            .Tipo_Cod = dtMateriePrimeCamp.Rows(j).Item("Tipo_Cod"),
                            .Udm_Cod = dtMateriePrimeCamp.Rows(j).Item("Udm_Cod"),
                            .Val_Cod = dtMateriePrimeCamp.Rows(j).Item("Val_Cod"),
                            .Descrizione = dtMateriePrimeCamp.Rows(j).Item("Descrizione"),
                            .Validita_Inizio = dtMateriePrimeCamp.Rows(j).Item("Validita_Inizio"),
                            .Validita_Fine = dtMateriePrimeCamp.Rows(j).Item("Validita_Fine"),
                            .Progressivo_Origine = dtMateriePrimeCamp.Rows(j).Item("Progressivo_Origine"),
                            .Piva_SuperUser_Origine = dtMateriePrimeCamp.Rows(j).Item("Piva_SuperUser_Origine"),
                            .Peso_Campione = dtMateriePrimeCamp.Rows(j).Item("Peso_Campione"),
                            .ChkStima = dtMateriePrimeCamp.Rows(j).Item("ChkStima"),
                            .ChkTara_Campionatura = dtMateriePrimeCamp.Rows(j).Item("ChkTara_Campionatura"),
                            .Tara_Campionatura = dtMateriePrimeCamp.Rows(j).Item("Tara_Campionatura"),
                            .Data_Creazione = CDate(dtMateriePrimeCamp.Rows(j).Item("Data_Creazione")),
                            .Data_Modifica = CDate(dtMateriePrimeCamp.Rows(j).Item("Data_Modifica")),
                            .Username_Creazione = dtMateriePrimeCamp.Rows(j).Item("Username_Creazione"),
                            .Username_Modifica = dtMateriePrimeCamp.Rows(j).Item("Username_Modifica")
                        }

                        listaMateriePrimeCampionature.Add(objMateriaPrimaCampionatura)

                    Next

                End If

            Catch ex As Exception

                Throw New Exception("[ Agenda_Materie_Prime_Campionature_Helper.Leggi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaMateriePrimeCampionature

        End Function

        Public Function RiempiListaConDefault(ByVal piva As String, ByVal listaReali As List(Of Materia_Prima_Campionatura),
                                              ByVal objParametri As AgronicaCoreParametri
                                              ) As List(Of Materia_Prima_Campionatura)

            Dim listaTipiPossibili As New List(Of String)
            Dim listaCompleta As New List(Of Materia_Prima_Campionatura)
            Dim flagTipoCalibroPresente As Boolean = False

            Try
                'creo lista di tutti i possibili
                listaTipiPossibili = CreaListaTipiPossibili(piva, objParametri)

                If Not IsNothing(listaReali) AndAlso listaReali.Count > 0 Then
                    'scorro lista reali
                    For Each item As Materia_Prima_Campionatura In listaReali
                        listaTipiPossibili.Remove(item.Tipo)
                        If item.Tipo = "calibro" Then
                            flagTipoCalibroPresente = True
                        End If
                    Next

                    'duplico la nuova lista
                    listaCompleta.AddRange(listaReali)
                End If



                'scorro la lista dei possibili rimasti
                For Each tipoPossibile In listaTipiPossibili
                    'creo oggetto Materia_Prima_Campionatura con valori default e quel tipo
                    Dim objTemp As New Materia_Prima_Campionatura With {.Tipo = tipoPossibile}

                    'aggiungo l'oggetto alla nuova lista
                    listaCompleta.Add(objTemp)
                Next

                'devo anche inserire la voce "calibro" con tipo_cod = 12
                If flagTipoCalibroPresente = False Then
                    listaCompleta.Add(New Materia_Prima_Campionatura With {.Tipo = "calibro", .Tipo_Cod = 12})
                End If

            Catch ex As Exception
                Throw New Exception("[ Agenda_Materie_Prime_Campionature_Helper.RiempiListaConDefault() ] : " & ex.Message)
            End Try

            Return listaCompleta

        End Function

        Private Function CreaListaTipiPossibili(ByVal piva As String, ByVal objParametri As AgronicaCoreParametri) As List(Of String)
            Dim listaPossibili As New List(Of String)

            Try
                Dim objConfigDettagli = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
                Dim dtParamQual As DataTable = objConfigDettagli.Leggi(piva, 0, False, "Tipo = 1", "", objParametri)

                For Each row As DataRow In dtParamQual.Rows
                    listaPossibili.Add("o" & row.Item("Tabella_Key"))
                Next

            Catch ex As Exception
                Throw New Exception("[ Agenda_Materie_Prime_Campionature_Helper.CreaListaTipiPossibili() ] : " & ex.Message)
            End Try

            Return listaPossibili

        End Function

    End Class

End Namespace
