Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Namespace OperazioneAgenda_Temp

    Public Class GHG_Registrazione

        Sub New()

            PivaSuperUser = ""
            Piva = ""
            Id_GHG_Registrazioni = 0
            Id_Agenda_GHG = 0
            Id_Mov_Det = 0
            Direttiva_Cod = 0
            Origine_Imputazione = 0
            Tipologia = 0
            Elem_Cod = 0
            Pro_Cod = 0
            Mat_Cod = 0
            Mac_Cod = 0
            Lotto = ""
            Udm_Trasporto_Attuale = 0
            Qty_Trasporto_Attuale = 0
            ETD_Trasporto_Attuale = 0
            EEC_Intero = 0
            GHG_Total = 0
            Dettaglio_JSON = ""
            Chain_Custody = 0
            GHG_Method_Calculation = 0
            Waste = 0
            Art29_Compliant = 0
            Iscc_Red_Compliant = 0
            Sust_Decl_Date = #2/1/1900#
            Sust_Decl_Number = ""

            'Valori Per Collegamento con Parametri_Indice
            ID_Indice = 0
            Valore_Des = ""
            Nome_Campo = ""
            TipoCampo = 0
            TipoDato = ""
            Elenco_Val = ""
            ID_Indice_Det = 0


            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub

        Public Property PivaSuperUser As String
        Public Property Piva As String

        Public Property Id_GHG_Registrazioni As Integer
        Public Property Id_Agenda_GHG As Integer

        Public Property Id_Mov_Det As Integer

        Public Property Direttiva_Cod As Integer

        Public Property Origine_Imputazione As Integer

        Public Property Tipologia As Integer
        Public Property Elem_Cod As Integer
        Public Property Mat_Cod As Integer
        Public Property Pro_Cod As Integer
        Public Property Mac_Cod As Integer
        Public Property Lotto As String
        Public Property Udm_Trasporto_Attuale As Integer
        Public Property ETD_Trasporto_Attuale As Decimal
        Public Property Qty_Trasporto_Attuale As Decimal
        Public Property EEC_Intero As Decimal
        Public Property GHG_Total As Decimal
        Public Property Dettaglio_JSON As String
        Public Property Chain_Custody As Integer
        Public Property GHG_Method_Calculation As Integer
        Public Property Waste As Integer
        Public Property Art29_Compliant As Integer
        Public Property Iscc_Red_Compliant As Integer
        Public Property Sust_Decl_Date As DateTime
        Public Property Sust_Decl_Number As String

        'Campi Per Mappatura Tabella Parametri_Indici
        Public Property ID_Indice As Integer
        Public Property Valore_Des As String
        Public Property Nome_Campo As String
        Public Property TipoCampo As Integer
        Public Property TipoDato As String
        Public Property Elenco_Val As String
        Public Property ID_Indice_Det As Integer

        'Campi Standard
        Public Property Validita_Inizio As Date

        Public Property Validita_Fine As Date

        Public Property Data_Creazione As DateTime

        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String
        Public Property BaseCode As Integer
        Public Property TopCode As Integer


    End Class

    Public Class Agenda_GHG_Registrazione_Helper

        Public Function Scrivi(ByVal objGHG_Registrazione As GHG_Registrazione,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Integer


            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Dim Id_GHG_Registrazioni As Integer

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Id_GHG_Registrazioni = objGHG_Registrazione.Id_GHG_Registrazioni

                If Id_GHG_Registrazioni = 0 Then

                    Dim objSequenze As New Agro_Sequenze

                    Id_GHG_Registrazioni = objSequenze.NuovoId_Tabella("GHG_Registrazioni",
                                                              objGHG_Registrazione.BaseCode,
                                                              objGHG_Registrazione.TopCode,
                                                              objParametri)

                    objSequenze = Nothing

                End If


                '------------------------------
                'GHG REGISTRAZIONI
                '------------------------------
                Dim objGHG_Registrazioni = New AgronicaCoreContabDAL.GHG_Registrazioni_W

                objGHG_Registrazioni.Scrivi(objGHG_Registrazione.Piva,
                                            Id_GHG_Registrazioni,
                                            objGHG_Registrazione.Id_Agenda_GHG,
                                            objGHG_Registrazione.Id_Mov_Det,
                                            objGHG_Registrazione.Direttiva_Cod,
                                            objGHG_Registrazione.Origine_Imputazione,
                                            objGHG_Registrazione.Tipologia,
                                            objGHG_Registrazione.Elem_Cod,
                                            objGHG_Registrazione.Pro_Cod,
                                            objGHG_Registrazione.Mat_Cod,
                                            objGHG_Registrazione.Mac_Cod,
                                            objGHG_Registrazione.Lotto,
                                            objGHG_Registrazione.Udm_Trasporto_Attuale,
                                            objGHG_Registrazione.Qty_Trasporto_Attuale,
                                            objGHG_Registrazione.ETD_Trasporto_Attuale,
                                            objGHG_Registrazione.EEC_Intero,
                                            objGHG_Registrazione.GHG_Total,
                                            objGHG_Registrazione.Dettaglio_JSON,
                                            objGHG_Registrazione.Chain_Custody,
                                            objGHG_Registrazione.GHG_Method_Calculation,
                                            objGHG_Registrazione.Waste,
                                            objGHG_Registrazione.Art29_Compliant,
                                            objGHG_Registrazione.Iscc_Red_Compliant,
                                            objGHG_Registrazione.Sust_Decl_Date,
                                            objGHG_Registrazione.Sust_Decl_Number,
                                            AGRODATAINIZIO,
                                            AGRODATAFINE,
                                            objParametri)

                objGHG_Registrazioni = Nothing



                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_GHG_Registrazione_Helper.Scrivi() ] : " & ex.Message)
            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return Id_GHG_Registrazioni

        End Function

        Public Function ModificaProdotto(ByVal Piva As String,
                                         ByVal ID_Agenda_GHG As Integer,
                                         ByVal Id_GHG_Registrazioni As Integer,
                                         ByVal Elem_Cod As Integer,
                                         ByVal Pro_Cod As Integer,
                                         ByVal Mat_Cod As Integer,
                                         ByVal Lotto As String,
                                         ByVal objParametri As AgronicaCoreParametri
                                         ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim ObjGHG_Registrazione As New AgronicaCoreContabDAL.GHG_Registrazioni_W

                xRisp = ObjGHG_Registrazione.ModificaProdotto(Piva, ID_Agenda_GHG, Id_GHG_Registrazioni, Elem_Cod, Pro_Cod, Mat_Cod, Lotto, objParametri)

                ObjGHG_Registrazione = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_GHG_Registrazione_Helper.ModificaProdotto() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function



        Public Function AggiornaGHGTotal(ByVal Piva As String,
                                         ByVal ID_Agenda_GHG As Integer,
                                         ByVal objParametri As AgronicaCoreParametri
                                         ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim ObjGHG_Registrazione As New AgronicaCoreContabDAL.GHG_Registrazioni_W

                xRisp = ObjGHG_Registrazione.AggiornaGHGTotalPQ(Piva, ID_Agenda_GHG, objParametri)

                ObjGHG_Registrazione = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_GHG_Registrazione_Helper.AggiornaGHGTotal() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function



        Public Function ModificaPuntuale(ByVal Piva As String,
                                         ByVal ID_Agenda_GHG As Integer,
                                         ByVal Id_GHG_Registrazioni As Integer,
                                         ByVal TipoCampo As Integer,
                                         ByVal TipoDato As String,
                                         ByVal Nome_Campo As String,
                                         ByVal Valore As String,
                                         ByVal ID_Indice_Det As String,
                                         ByVal Elenco_Val As String,
                                         ByVal objParametri As AgronicaCoreParametri
                                         ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim ObjGHG_Registrazione As New AgronicaCoreContabDAL.GHG_Registrazioni_W

                Select Case TipoCampo

                    Case 0 'Libera Imputazione

                        Select Case LCase(TipoDato)

                            Case "string", "boolean"

                                xRisp = ObjGHG_Registrazione.ModificaPuntualeStringa(Piva, Id_GHG_Registrazioni, ID_Agenda_GHG, Nome_Campo, Valore, objParametri)

                            Case "numeric"

                                If Not IsNumeric(Valore) Then
                                    Valore = 0
                                End If

                                xRisp = ObjGHG_Registrazione.ModificaPuntualeNumerico(Piva, Id_GHG_Registrazioni, ID_Agenda_GHG, Nome_Campo, Valore, objParametri)

                            Case "date"

                                If Not IsDate(Valore) Then
                                    Valore = AGRODATAINIZIO
                                End If

                                xRisp = ObjGHG_Registrazione.ModificaPuntualeData(Piva, Id_GHG_Registrazioni, ID_Agenda_GHG, Nome_Campo, Valore, objParametri)

                        End Select

                    Case 1 'Parametri Dettagli

                        If Not IsNumeric(ID_Indice_Det) Then
                            ID_Indice_Det = 0
                        End If


                        xRisp = ObjGHG_Registrazione.ModificaPuntualeNumerico(Piva, Id_GHG_Registrazioni, ID_Agenda_GHG, Nome_Campo, ID_Indice_Det, objParametri)


                    Case 2 'Elenco_Val

                        If Not IsNumeric(Elenco_Val) Then
                            Elenco_Val = 0
                        End If


                        xRisp = ObjGHG_Registrazione.ModificaPuntualeNumerico(Piva, Id_GHG_Registrazioni, ID_Agenda_GHG, Nome_Campo, Elenco_Val, objParametri)


                End Select

                ObjGHG_Registrazione = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_GHG_Registrazione_Helper.ModificaPuntuale() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function



    End Class

End Namespace
