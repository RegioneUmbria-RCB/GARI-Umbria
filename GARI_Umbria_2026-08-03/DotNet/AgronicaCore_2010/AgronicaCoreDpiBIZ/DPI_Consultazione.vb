Imports System.Data
Imports System.Data.Common
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class DPI_Consultazione

    Inherits AgronicaCoreDataProvider.LogProvider

    '============================================================================
    Public Function DPI_Consultazione_Difesa(ByVal Disciplinare_Cod As Int32,
                                             ByVal Id_RcDpi As Int32,
                                             ByVal Id_GaDpi As Int32,
                                             ByVal Av_Gru As Int32,
                                             ByVal Av_Cod As Int32,
                                             ByVal Gru_Pa_Cod As Int32,
                                             ByVal Pa_Cod As Int32,
                                             ByVal Modulo As Int32,
                                             ByRef objParametri_Disciplinari As AgronicaCoreParametri,
                                             ByRef objParametri_Matrice As AgronicaCoreParametri
                                             ) As String

        Const nomeRoutine = "DpiBIZ.DPI_Consultazione.DPI_Consultazione_Difesa()"

        Dim messaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione_Disciplinari As DbConnection
        Dim xConnessione_Matrice As DbConnection
        Dim xConnectionState_Disciplinari As ConnectionState = ConnectionState.Closed
        Dim xConnectionState_Matrice As ConnectionState = ConnectionState.Closed

        Dim i As Int32

        Dim risultatoFunzione As String = String.Empty

        '======

        Dim riga As Int32

        Dim Av_Gru_Des As String
        Dim Av_Cod_Des As String
        Dim Ga_Dpi_Des As String

        Dim strCriteri As String
        Dim strPA As String
        Dim strID_PAA As String
        Dim strLimitazioni As String
        Dim strNoteXTestata As String
        Dim strNoteXRighe As String
        Dim strNoteXEpoche As String
        Dim strPostilla As String
        Dim strLastLimitazione As String

        Dim Criteri(,) As String
        Dim Limitazioni(,) As String
        Dim Avversita As String()
        Dim NotexRighe As String()
        Dim NotexTestata As String()

        Dim bTrovato As Boolean

        Dim Limitazione As String
        Dim bDuplicato As Boolean
        Dim Avv_Des As String
        Dim Modulo_Des As String
        Dim LastModulo_Des As String
        Dim TipoTestata As Int32

        Dim XmlDoc As XmlDocument
        Dim XmlDatiRisultati As XmlElement
        Dim XmlDatiDettaglio As XmlElement

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione 
            If IsNothing(objParametri_Disciplinari.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione_Disciplinari = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri_Disciplinari.StringaConnessione)
            Else
                'Utilizzo quella passata come parametro
                xConnessione_Disciplinari = objParametri_Disciplinari.objConnessione
                xConnectionState_Disciplinari = objParametri_Disciplinari.objConnessione.State
            End If
            If IsNothing(objParametri_Matrice.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione_Matrice = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri_Matrice.StringaConnessione)
            Else
                'Utilizzo quella passata come parametro
                xConnessione_Matrice = objParametri_Matrice.objConnessione
                xConnectionState_Matrice = objParametri_Matrice.objConnessione.State
            End If
            '------------------------------

            'XmlDoc = CreateObject("MSXML2.DOMDocument.4.0")
            'XmlDoc.async = False
            'XmlDatiRisultati = XmlDoc.createElement("DatiRisultati")
            XmlDoc = New XmlDocument
            XmlDatiRisultati = XmlDoc.CreateElement("DatiRisultati")

            'Controllo parametri validi
            If Disciplinare_Cod <> 0 AndAlso Id_RcDpi <> 0 Then

                'Parametri settati
                TipoTestata = 0 'Difesa

                Dim ObjDPI As New AgronicaCoreDpiDAL.Dpi_R

                Dim ObjPA As New AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R
                Dim ObjGruppiAvversita As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
                Dim ObjAvversita As New AgronicaCoreMetaSchemaDAL.SpecieVegetalixAvversita_R

                Dim DtDPI As DataTable
                Dim DtDPI_PA As DataTable
                Dim DtGPA As DataTable
                Dim DtNote As DataTable
                Dim DtLimitazioni As DataTable
                Dim DtCriteri As DataTable

                Dim DtAvversita As DataTable
                Dim DtPA As DataTable

                'Lettura delle Infestanti
                DtDPI = ObjDPI.Leggi_Infestanti(Id_RcDpi,
                                                Disciplinare_Cod,
                                                TipoTestata,
                                                Id_GaDpi,
                                                Av_Gru,
                                                Av_Cod,
                                                Pa_Cod,
                                                Gru_Pa_Cod,
                                                0,
                                                "",
                                                Modulo,
                                                0,
                                                "",
                                                "",
                                                objParametri_Disciplinari)

                '========================================================================

                If DtDPI.Rows.Count > 0 Then

                    ReDim Avversita(0)

                    'Reset Filtro
                    'RsDPI.Filter = ""

                    Dim iDPI As Int32
                    For iDPI = 0 To DtDPI.Rows.Count - 1

                        riga = 0

                        ReDim Criteri(1, 0)
                        ReDim Limitazioni(1, 0)
                        ReDim NotexRighe(0)
                        ReDim NotexTestata(0)

                        strPA = ""
                        strLimitazioni = ""
                        strLastLimitazione = ""

                        If Modulo <> 0 Then
                            Modulo_Des = DtDPI.Rows(iDPI).Item("DescrizionePeriodoDa")
                        Else
                            Modulo_Des = ""
                        End If

                        '====================================================================================================================
                        ' AV_GRU
                        '--------------------------------------------------------------------------------------------------------------------
                        Av_Gru_Des = ""

                        If CInt(DtDPI.Rows(iDPI).Item("Av_Gru")) <> 0 Then

                            'Lettura Av_Gru
                            DtAvversita = ObjGruppiAvversita.Leggi(CInt(DtDPI.Rows(iDPI).Item("Av_Gru")),
                                                                   0,
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   "", "",
                                                                   objParametri_Matrice)

                            If DtAvversita.Rows.Count > 0 Then

                                Av_Gru_Des = UCase(DtAvversita.Rows(0).Item("Av_Gru_Des")) & Chr(13)
                                riga = riga + 1

                            End If

                            DtAvversita = Nothing

                            Avv_Des = Av_Gru_Des

                        End If


                        '====================================================================================================================
                        ' ID_GADPI
                        '--------------------------------------------------------------------------------------------------------------------
                        Ga_Dpi_Des = ""
                        If Agro_SQL_SaveNum(DtDPI.Rows(iDPI).Item("Id_GaDpi")) <> 0 Then

                            'Lettura Ga_Dpi
                            DtAvversita = ObjDPI.Leggi_GruppiAvversitaDPI(CInt(DtDPI.Rows(iDPI).Item("Id_GaDpi")),
                                                                          "", "",
                                                                          objParametri_Disciplinari)

                            If DtAvversita.Rows.Count > 0 Then

                                Ga_Dpi_Des = DtAvversita.Rows(0).Item("Descrizione") & Chr(13)
                                riga = riga + 1

                            End If

                            Avv_Des = Ga_Dpi_Des

                        End If


                        '====================================================================================================================
                        ' AV_COD
                        '--------------------------------------------------------------------------------------------------------------------
                        Av_Cod_Des = ""
                        If DtDPI.Rows(iDPI).Item("Av_Cod") <> 0 Then

                            'Lettura Av_Cod

                            Select Case TipoTestata

                                Case 0 'Difesa

                                    DtAvversita = ObjAvversita.Leggi(0, 0,
                                                                     CInt(DtDPI.Rows(0).Item("Av_Cod")),
                                                                     AGRODATAINIZIO, AGRODATAFINE,
                                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                     "", "", objParametri_Matrice)

                            End Select

                            If DtAvversita.Rows.Count > 0 Then

                                Av_Cod_Des = "(" & LCase(DtAvversita.Rows(0).Item("Av_Des_Lat")) & ")" & Chr(13)

                                riga = riga + 1

                            End If

                            Avv_Des = Av_Cod_Des

                        End If

                        '================================================================================================================

                        strCriteri = ""

                        'Controllo duplicati
                        bDuplicato = False

                        For i = 0 To UBound(Avversita, 1) - 1
                            If Avv_Des = Avversita(i) AndAlso LastModulo_Des = Modulo_Des Then
                                bDuplicato = True
                                Exit For
                            End If
                        Next

                        If Not bDuplicato Then

                            If riga <> 0 Then

                                ReDim Preserve Avversita(UBound(Avversita, 1) + 1)
                                Avversita(UBound(Avversita, 1) - 1) = Avv_Des

                                DtCriteri = ObjDPI.Leggi_CriteriIntervento(0,
                                                                           CInt(DtDPI.Rows(iDPI).Item("DFT_Cod")),
                                                                           CInt(DtDPI.Rows(iDPI).Item("DFR_Cod")),
                                                                           0,
                                                                           "",
                                                                           "",
                                                                           objParametri_Disciplinari)

                                If DtCriteri.Rows.Count > 0 Then

                                    Dim iCrit As Integer
                                    For iCrit = 0 To DtCriteri.Rows.Count - 1

                                        bTrovato = False
                                        For i = 0 To UBound(Criteri, 1) - 1

                                            If UCase(Criteri(i, 0)) = UCase(DtCriteri.Rows(iCrit).Item("TCI_Descrizione")) Then
                                                If IsNumeric(DtCriteri.Rows(iCrit).Item("IdVincolo")) Then
                                                    'Vincolo Presente
                                                    Criteri(1, i) = Criteri(1, i) & "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & CStr(DtCriteri.Rows(iCrit).Item("Descrizione")) & "</FONT>"
                                                Else
                                                    'Vincolo Assente
                                                    Criteri(1, i) = Criteri(1, i) & "<BR>" & " - " & CStr(DtCriteri.Rows(iCrit).Item("Descrizione"))
                                                End If

                                                bTrovato = True

                                                Exit For
                                            End If
                                        Next

                                        If Not bTrovato Then
                                            ReDim Preserve Criteri(1, UBound(Criteri, 2) + 1)

                                            If IsNumeric(DtCriteri.Rows(iCrit).Item("IdVincolo")) Then
                                                'Vincolo Presente
                                                Criteri(1, UBound(Criteri, 2) - 1) = "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & CStr(DtCriteri.Rows(iCrit).Item("Descrizione")) & "</FONT>"

                                            Else
                                                'Vincolo Assente
                                                Criteri(1, UBound(Criteri, 2) - 1) = " - " & CStr(DtCriteri.Rows(iCrit).Item("Descrizione"))
                                            End If

                                            Criteri(0, UBound(Criteri, 2) - 1) = CStr(DtCriteri.Rows(iCrit).Item("TCI_Descrizione"))

                                        End If

                                    Next

                                End If


                                '============================================================================================================================================================
                                'Lettura dei Principi Attivi
                                '------------------------------------------------------------------------------------------------------------------------------------------------------------
                                strPA = ""
                                Dim strOrderBy As String = " PA_Ausiliari.Posizione "

                                DtDPI_PA = ObjDPI.Leggi_PrincipiAttivi_codDescrizioni(CInt(DtDPI.Rows(iDPI).Item("DFT_Cod")),
                                                                                      CInt(DtDPI.Rows(iDPI).Item("DFR_Cod")),
                                                                                      0,
                                                                                      "",
                                                                                      strOrderBy,
                                                                                      objParametri_Disciplinari)

                                If DtDPI_PA.Rows.Count > 0 Then

                                    'Dim bLimitazioni As Boolean
                                    Dim Miscela As Boolean
                                    Dim ID_PAA_Misc As Integer

                                    Dim iDPI_PA As Int32
                                    For iDPI_PA = 0 To DtDPI_PA.Rows.Count - 1

                                        If (Not IsDBNull(DtDPI_PA.Rows(iDPI_PA).Item("Pa_Cod")) AndAlso DtDPI_PA.Rows(iDPI_PA).Item("Pa_Cod") <> 0) Or
                                           (Not IsDBNull(DtDPI_PA.Rows(iDPI_PA).Item("GRU_PA_Cod")) AndAlso DtDPI_PA.Rows(iDPI_PA).Item("GRU_PA_Cod") <> 0) Then

                                            'bLimitazioni = True
                                            Miscela = False
                                            ID_PAA_Misc = 0

                                        Else

                                            'bLimitazioni = False
                                            Miscela = True
                                            ID_PAA_Misc = DtDPI_PA.Rows(iDPI_PA).Item("ID_PAA")

                                        End If


                                        '============================================================================================
                                        'LIMITAZIONI

                                        strPostilla = ""

                                        'If bLimitazioni Then

                                        'Lettura limitazioni
                                        strID_PAA = "( " & CInt(DtDPI_PA.Rows(iDPI_PA).Item("ID_PAA")) & " )"

                                        Dim strFiltro As String = " Flag_Controllo=0 "

                                        DtLimitazioni = ObjDPI.Leggi_LimitazioniUso(0, strID_PAA,
                                                                                    strFiltro,
                                                                                    "",
                                                                                    objParametri_Disciplinari)

                                        If DtLimitazioni.Rows.Count <> 0 Then

                                            Dim iLim As Integer
                                            For iLim = 0 To DtLimitazioni.Rows.Count - 1

                                                Limitazione = ""

                                                If Not IsDBNull(DtLimitazioni.Rows(iLim).Item("Note")) Then

                                                    'Controllo che la postilla esista
                                                    If Not IsDBNull(DtLimitazioni.Rows(iLim).Item("SimboloPostilla")) Then

                                                        strPostilla = DtLimitazioni.Rows(iLim).Item("SimboloPostilla")
                                                        strPostilla = Replace(strPostilla, "§", "")
                                                        strPostilla = Replace(strPostilla, "%", "")

                                                        'Controllo che la limitazione non sia duplicata
                                                        'strPostilla = strPostilla & DtLimitazioni.Rows(iLim).Item("SimboloPostilla")

                                                        'Limitazione = If(CInt(DtLimitazioni.Rows(iLim).Item("IDVincolo")) <> 0, "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>", "") & If(UCase(strLastLimitazione) <> UCase(DtLimitazioni.Rows(iLim).Item("Note")), DtLimitazioni.Rows(iLim).Item("SimboloPostilla") & DtLimitazioni.Rows(iLim).Item("Note"), "") & If(Agro_SQL_SaveNum(DtLimitazioni.Rows(iLim).Item("IDVincolo")) <> 0, "</FONT>", "") & "<BR><BR>"
                                                        Limitazione = If(CInt(DtLimitazioni.Rows(iLim).Item("IDVincolo")) <> 0, "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>", "") & If(UCase(strLastLimitazione) <> UCase(DtLimitazioni.Rows(iLim).Item("Note")), strPostilla & DtLimitazioni.Rows(iLim).Item("Note"), "") & If(Agro_SQL_SaveNum(DtLimitazioni.Rows(iLim).Item("IDVincolo")) <> 0, "</FONT>", "") & "<BR><BR>"
                                                    Else
                                                        Limitazione = If(CInt(DtLimitazioni.Rows(iLim).Item("IDVincolo")) <> 0, "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>", "") & If(UCase(strLastLimitazione) <> UCase(DtLimitazioni.Rows(iLim).Item("Note")), DtLimitazioni.Rows(iLim).Item("Note"), "") & If(Agro_SQL_SaveNum(DtLimitazioni.Rows(iLim).Item("IDVincolo")) <> 0, "</FONT>", "") & "<BR><BR>"
                                                    End If

                                                    strLastLimitazione = DtLimitazioni.Rows(iLim).Item("Note")

                                                End If


                                                'Verifica Elasticità
                                                If Not IsDBNull(DtLimitazioni.Rows(iLim).Item("FlagElasticita")) Then
                                                    If CBool(DtLimitazioni.Rows(iLim).Item("FlagElasticita")) Then

                                                        strPostilla = strPostilla & " (E)"
                                                        Limitazione = "<FONT STYLE='BACKGROUND-COLOR: YELLOW'> (E) Note Transitorie (Elasticità): </FONT><BR>Limitatamente ai programmi applicativi del Reg.CEE 2000/96 e delle leggi regionali n.28/98 e 28/99 <BR><BR>"

                                                    End If

                                                End If


                                                If Trim(Limitazione) <> "" Then

                                                    strPostilla = Replace(strPostilla, "§", "")
                                                    Limitazione = Replace(Limitazione, "§", "")
                                                    strPostilla = Replace(strPostilla, "%", "")
                                                    Limitazione = Replace(Limitazione, "%", "")

                                                    'Verifico che la limitazione d'uso non sia duplicata
                                                    bTrovato = False
                                                    For i = 0 To UBound(Limitazioni, 2) - 1
                                                        If UCase(Limitazioni(1, i)) = UCase(Limitazione) Then
                                                            bTrovato = True
                                                            Exit For
                                                        End If
                                                    Next


                                                    If Not bTrovato Then

                                                        'If Miscela = False Then

                                                        ReDim Preserve Limitazioni(1, UBound(Limitazioni, 2) + 1)

                                                        Limitazioni(1, UBound(Limitazioni, 2) - 1) = Limitazione
                                                        Limitazioni(0, UBound(Limitazioni, 2) - 1) = strPostilla
                                                        'Else

                                                        'End If

                                                    End If

                                                End If
                                                '------------------------------------------------------------------------

                                            Next

                                        End If


                                        'End If


                                        '================================================================================================
                                        '     PRINCIPI ATTIVI
                                        '------------------------------------------------------------------------------------------------
                                        If Not IsDBNull(DtDPI_PA.Rows(iDPI_PA).Item("Pa_Cod")) AndAlso DtDPI_PA.Rows(iDPI_PA).Item("Pa_Cod") <> 0 Then

                                            'Lettura del principio attivo gias
                                            strPA = strPA & "<I>" & LCase(DtDPI_PA.Rows(iDPI_PA).Item("Pa_Des")) & " " & strPostilla & "</I><BR><BR>"

                                            ''Lettura del principio attivo gias
                                            'DtPA = ObjPA.Leggi(CInt(DtDPI_PA.Rows(iDPI_PA).Item("Pa_Cod")), AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Matrice)

                                            'If DtPA.Rows.Count <> 0 Then

                                            '    strPA = strPA & "<I>" & LCase(DtPA.Rows(0).Item("Pa_Des")) & " " & strPostilla & "</I><BR><BR>"

                                            'End If


                                        ElseIf Not IsDBNull(DtDPI_PA.Rows(iDPI_PA).Item("GRU_PA_Cod")) AndAlso DtDPI_PA.Rows(iDPI_PA).Item("GRU_PA_Cod") <> 0 Then

                                            'Lettura del principio attivo gias
                                            strPA = strPA & "<I>" & LCase(DtDPI_PA.Rows(iDPI_PA).Item("GPA_DES")) & " " & strPostilla & "</I><BR><BR>"

                                            'DtGPA = ObjDPI.Leggi_GruppiPrincipiAttivi(CInt(DtDPI_PA.Rows(iDPI_PA).Item("GRU_PA_Cod")), "", "", objParametri_Disciplinari)

                                            'If DtGPA.Rows.Count <> 0 Then

                                            '    strPA = strPA & "<I>" & LCase(DtGPA.Rows(0).Item("GPA_Des")) & " " & strPostilla & "</I><BR><BR>"

                                            'End If

                                        Else

                                            Dim DrMisc As DataRow()
                                            Dim iMisc As Int32
                                            DrMisc = DtDPI_PA.Select("ID_PAA_Misc=" & ID_PAA_Misc.ToString)
                                            Dim strMisc As String = ""

                                            If Not IsNothing(DrMisc) AndAlso DrMisc.Length > 0 Then
                                                For iMisc = 0 To DrMisc.Length - 1
                                                    strMisc &= DrMisc(iMisc).Item("PA_DES") & " + "
                                                Next
                                            End If
                                            If strMisc <> "" Then
                                                strMisc = Left(strMisc, strMisc.Length - 3)
                                            End If
                                            strPA = strPA & "<I>" & LCase(strMisc) & " " & strPostilla & "</I><BR><BR>"


                                        End If
                                        '================================================================================================

                                    Next

                                End If
                                '============================================================================================================================================================


                            End If
                            '
                            'Costruzione Criteri
                            strCriteri = ""
                            For i = 0 To UBound(Criteri, 2) - 1

                                strCriteri = strCriteri & "<U>" & Criteri(0, i) & "</U><BR>" & Criteri(1, i) & "<BR>"

                            Next


                            '=========================================================================================================================================================
                            'Lettura delle NotexTestata
                            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                            strNoteXTestata = ""

                            DtNote = ObjDPI.Leggi_NotexTestataxVincoli(CInt(DtDPI.Rows(iDPI).Item("DFT_Cod")), 0, "", "", objParametri_Disciplinari)

                            If DtNote.Rows.Count <> 0 Then

                                Dim iNote As Integer
                                For iNote = 0 To DtNote.Rows.Count - 1

                                    'Check Duplicati
                                    bTrovato = False
                                    For i = 0 To UBound(NotexTestata, 1) - 1

                                        If CLng(NotexTestata(i)) = CInt(DtNote.Rows(iNote).Item("Id_Nota")) Then

                                            bTrovato = True
                                            Exit For

                                        End If

                                    Next i

                                    If Not bTrovato Then

                                        ReDim Preserve NotexTestata(UBound(NotexTestata, 1) + 1)
                                        NotexTestata(UBound(NotexTestata, 1) - 1) = DtNote.Rows(iNote).Item("Id_Nota")

                                        If IsNumeric(DtNote.Rows(iNote).Item("IDVincolo")) Then

                                            'Nota Vincolata
                                            strNoteXTestata = strNoteXTestata & "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & DtNote.Rows(iNote).Item("Nota") & "</FONT><BR>"

                                        Else

                                            'Nota NON Vincolata
                                            strNoteXTestata = strNoteXTestata & DtNote.Rows(iNote).Item("Nota") & "<BR>"

                                        End If

                                    End If

                                Next

                            End If
                            '=========================================================================================================================================================



                            '=========================================================================================================================================================
                            'Lettura delle NotexRighe
                            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                            strNoteXRighe = ""

                            DtNote = ObjDPI.Leggi_NotexRighexVincoli(CInt(DtDPI.Rows(iDPI).Item("DFR_Cod")), 0, "", "", objParametri_Disciplinari)

                            If DtNote.Rows.Count <> 0 Then

                                Dim iNote As Integer
                                For iNote = 0 To DtNote.Rows.Count - 1

                                    'Check Duplicati
                                    bTrovato = False
                                    For i = 0 To UBound(NotexRighe, 1) - 1

                                        If CInt(NotexRighe(i)) = CInt(DtNote.Rows(iNote).Item("Id_Nota")) Then

                                            bTrovato = True
                                            Exit For

                                        End If

                                    Next i

                                    If Not bTrovato Then

                                        ReDim Preserve NotexRighe(UBound(NotexRighe, 1) + 1)
                                        NotexRighe(UBound(NotexRighe, 1) - 1) = DtNote.Rows(iNote).Item("Id_Nota")

                                        If IsNumeric(DtNote.Rows(iNote).Item("IDVincolo")) Then

                                            'Nota Vincolata
                                            strNoteXRighe = strNoteXRighe & "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & DtNote.Rows(iNote).Item("Nota") & "</FONT><BR>"

                                        Else

                                            'Nota NON Vincolata
                                            strNoteXRighe = strNoteXRighe & DtNote.Rows(iNote).Item("Nota") & "<BR>"

                                        End If

                                    End If

                                Next

                            End If
                            '=========================================================================================================================================================


                            '=========================================================================================================================================================
                            'Lettura delle NotexEpoche
                            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                            strNoteXEpoche = ""

                            DtNote = ObjDPI.Leggi_NotexEpochexRighe(CInt(DtDPI.Rows(iDPI).Item("DFT_Cod")), CInt(DtDPI.Rows(iDPI).Item("DFR_Cod")), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Disciplinari)

                            If DtNote.Rows.Count <> 0 Then

                                Dim iNote As Integer
                                For iNote = 0 To DtNote.Rows.Count - 1

                                    If IsNumeric(DtNote.Rows(iNote).Item("IDVincolo")) Then

                                        'Nota Vincolata
                                        strNoteXEpoche = strNoteXEpoche & "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & DtNote.Rows(iNote).Item("Nota") & "</FONT><BR>"

                                    Else

                                        'Nota NON Vincolata
                                        strNoteXEpoche = strNoteXEpoche & DtNote.Rows(iNote).Item("Nota") & "<BR>"

                                    End If

                                Next

                            End If
                            '=========================================================================================================================================================


                            ''Ordinamento Vettore 'Limitazioni

                            'Sw = False

                            'Do While Not Sw
                            '    Sw = True
                            '    i = 0
                            '    Do While i < UBound(Limitazioni, 2)

                            '        If Limitazioni(0, i) > Limitazioni(0, i + 1) Then
                            '            'Scambio
                            '            ReDim Scambio(1)
                            '            Scambio(0) = Limitazioni(0, i)
                            '            Scambio(1) = Limitazioni(1, i)
                            '            Limitazioni(0, i) = Limitazioni(0, i + 1)
                            '            Limitazioni(1, i) = Limitazioni(1, i + 1)
                            '            Limitazioni(0, i + 1) = Scambio(0)
                            '            Limitazioni(1, i + 1) = Scambio(1)
                            '            Sw = False
                            '        End If

                            '        i = i + 1

                            '    Loop

                            'Loop


                            strLimitazioni = ""

                            'Ottenimento stringa strLimitazioni
                            For i = 1 To UBound(Limitazioni, 2)
                                strLimitazioni = strLimitazioni & Limitazioni(1, i)
                            Next


                            '=========================================================================================================================================================
                            'Inserimento dettaglio in stringa xml
                            XmlDatiDettaglio = XmlDoc.CreateElement("Dettaglio")

                            XmlDatiDettaglio.SetAttribute("av_gru_des", Av_Gru_Des)
                            XmlDatiDettaglio.SetAttribute("ga_dpi_Des", Ga_Dpi_Des)
                            XmlDatiDettaglio.SetAttribute("av_cod_des", Av_Cod_Des)
                            XmlDatiDettaglio.SetAttribute("criteri", strCriteri)
                            XmlDatiDettaglio.SetAttribute("principi_attivi", strPA)
                            XmlDatiDettaglio.SetAttribute("limitazioni", strLimitazioni)
                            XmlDatiDettaglio.SetAttribute("notextestata", strNoteXTestata)
                            XmlDatiDettaglio.SetAttribute("notexrighe", strNoteXRighe)
                            XmlDatiDettaglio.SetAttribute("notexepoche", strNoteXEpoche)
                            XmlDatiDettaglio.SetAttribute("modulo_des", Modulo_Des)

                            XmlDatiRisultati.AppendChild(XmlDatiDettaglio)

                            XmlDatiDettaglio = Nothing
                            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                        End If 'Fine Check Duplicati

                        LastModulo_Des = Modulo_Des


                    Next

                End If



                '========================================================================

                XmlDoc.AppendChild(XmlDatiRisultati)

                Return XmlDoc.OuterXml

            Else

                Return "-1"

            End If
            '-----------------------------------------------------------------------------------------------------------------


        Catch ex As Exception

            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Disciplinari, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

            Return "-1"

        Finally

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione_Disciplinari) Then
                    xConnessione_Disciplinari.Close()
                    xConnessione_Disciplinari.Dispose()
                End If
                If Not IsNothing(xConnessione_Matrice) Then
                    xConnessione_Matrice.Close()
                    xConnessione_Matrice.Dispose()
                End If
            Else
                If xConnectionState_Disciplinari = ConnectionState.Closed Then
                    xConnessione_Disciplinari.Close()
                End If
                If xConnectionState_Matrice = ConnectionState.Closed Then
                    xConnessione_Matrice.Close()
                End If
            End If

        End Try


    End Function

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################


    Public Function DPI_Consultazione_Diserbo(ByVal Disciplinare_Cod As Int32,
                                              ByVal Gru_Cod As Int32,
                                              ByVal Id_RcDpi As Int32,
                                              ByVal Id_GaDpi As Int32,
                                              ByVal Av_Gru As Int32,
                                              ByVal Av_Cod As Int32,
                                              ByVal Gru_Pa_Cod As Int32,
                                              ByVal Pa_Cod As Int32,
                                              ByVal Epoca As Int32,
                                              ByRef objParametri_Disciplinari As AgronicaCoreParametri,
                                              ByRef objParametri_Matrice As AgronicaCoreParametri
                                              ) As String

        Const nomeRoutine = "DpiBIZ.DPI_Consultazione.DPI_Consultazione_Diserbo()"

        Dim messaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione_Disciplinari As DbConnection
        Dim xConnessione_Matrice As DbConnection
        Dim xConnectionState_Disciplinari As ConnectionState = ConnectionState.Closed
        Dim xConnectionState_Matrice As ConnectionState = ConnectionState.Closed

        Dim i As Int32

        Dim risultatoFunzione As String = String.Empty

        Dim riga As Int32
        Dim Av_Gru_Des As String
        Dim Av_Cod_Des As String
        Dim Ga_Dpi_Des As String
        Dim strCriteri As String
        Dim strPA As String
        Dim strLimitazioni As String
        Dim strNoteXTestata As String
        Dim strNoteXRighe As String
        Dim strNoteXEpoche As String
        Dim strPostilla As String

        Dim Criteri(,) As String
        Dim Limitazioni(,) As String
        Dim Avversita As String()

        Dim bTrovato As Boolean

        Dim bDuplicato As Boolean
        Dim Avv_Des As String
        Dim TipoTestata As Int32
        Dim Ep_Cod As Int32
        Dim Epoca_Des As String
        Dim LastEp_Cod As Int32
        Dim strPercentuale As String
        Dim Dose_min As Decimal
        Dim Dose_Max As Decimal
        Dim strQta As String


        Dim XmlDoc As XmlDocument
        Dim XmlDatiRisultati As XmlElement
        Dim XmlDatiDettaglio As XmlElement

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione 
            If IsNothing(objParametri_Disciplinari.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione_Disciplinari = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri_Disciplinari.StringaConnessione)
            Else
                'Utilizzo quella passata come parametro
                xConnessione_Disciplinari = objParametri_Disciplinari.objConnessione
                xConnectionState_Disciplinari = objParametri_Disciplinari.objConnessione.State
            End If
            If IsNothing(objParametri_Matrice.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione_Matrice = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri_Matrice.StringaConnessione)
            Else
                'Utilizzo quella passata come parametro
                xConnessione_Matrice = objParametri_Matrice.objConnessione
                xConnectionState_Matrice = objParametri_Matrice.objConnessione.State
            End If
            '------------------------------


            'XmlDoc = CreateObject("MSXML2.DOMDocument.4.0")
            'XmlDoc.async = False
            'XmlDatiRisultati = XmlDoc.createElement("DatiRisultati")
            XmlDoc = New XmlDocument
            XmlDatiRisultati = XmlDoc.CreateElement("DatiRisultati")


            'Controllo parametri validi
            If Disciplinare_Cod <> 0 AndAlso Id_RcDpi <> 0 Then

                'Parametri settati
                TipoTestata = 1 'Diserbo

                'Parametri settati

                Dim ObjDpi As New AgronicaCoreDpiDAL.Dpi_R

                Dim ObjPA As New AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R
                Dim ObjGruppiAvversita As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
                Dim ObjAvversita As New AgronicaCoreMetaSchemaDAL.SpecieVegetalixAvversita_R
                Dim ObjEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R

                Dim DtDPI As DataTable
                Dim DtDPI_PA As DataTable
                Dim DtGPA As DataTable
                Dim DtNote As DataTable

                Dim DtLimitazioni As DataTable
                Dim DtPA As DataTable
                Dim DtGruppiAvversita As DataTable
                Dim DtGruppiInfestanti As DataTable
                Dim DtAvversita As DataTable
                Dim DtCriteri As DataTable
                Dim DtEpoca As DataTable


                Ep_Cod = Epoca


                'Lettura delle Infestanti
                DtDPI = ObjDpi.Leggi_Infestanti(Id_RcDpi, Disciplinare_Cod, TipoTestata, Id_GaDpi, Av_Gru, Av_Cod, Pa_Cod, Gru_Pa_Cod, 0, "", 0, Epoca, "", "", objParametri_Disciplinari)

                '========================================================================

                If DtDPI.Rows.Count <> 0 Then

                    ReDim Avversita(0)

                    Dim iDPI As Int32
                    For iDPI = 0 To DtDPI.Rows.Count - 1

                        riga = 0

                        ReDim Criteri(1, 0)
                        ReDim Limitazioni(1, 0)
                        strPA = ""
                        strLimitazioni = ""

                        LastEp_Cod = Ep_Cod

                        Ep_Cod = Agro_SQL_SaveNum(DtDPI.Rows(iDPI).Item("Da_Ep_Cod"))

                        'Epoca_Des
                        Epoca_Des = ""
                        If Ep_Cod <> 0 Then
                            DtEpoca = ObjEpoca.Leggi(Ep_Cod, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Matrice)
                            If DtEpoca.Rows.Count <> 0 Then
                                Epoca_Des = DtEpoca.Rows(iDPI).Item("Descrizione")
                            End If

                        End If

                        '====================================================================================================================
                        ' AV_GRU
                        '--------------------------------------------------------------------------------------------------------------------
                        Av_Gru_Des = ""
                        If Agro_SQL_SaveNum(DtDPI.Rows(iDPI).Item("Av_Gru")) <> 0 Then

                            'Lettura Av_Gru

                            DtAvversita = ObjGruppiAvversita.Leggi(DtDPI.Rows(iDPI).Item("Av_Gru"), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Matrice)

                            If DtAvversita.Rows.Count <> 0 Then

                                Av_Gru_Des = UCase(DtAvversita.Rows(0).Item("Av_Gru_Des")) & Chr(13)
                                riga = riga + 1

                            End If

                            Avv_Des = Av_Gru_Des

                        End If


                        '====================================================================================================================
                        ' ID_GADPI
                        '--------------------------------------------------------------------------------------------------------------------
                        Ga_Dpi_Des = ""
                        If Agro_SQL_SaveNum(DtDPI.Rows(iDPI).Item("Id_GaDpi")) <> 0 Then

                            'Lettura Ga_Dpi
                            DtAvversita = ObjDpi.Leggi_GruppiAvversitaDPI(DtDPI.Rows(iDPI).Item("Id_GaDpi"), "", "", objParametri_Disciplinari)

                            If DtAvversita.Rows.Count <> 0 Then

                                Ga_Dpi_Des = DtAvversita.Rows(0).Item("Descrizione") & Chr(13)
                                riga = riga + 1

                            End If

                            Avv_Des = Ga_Dpi_Des

                        End If


                        '====================================================================================================================
                        ' AV_COD
                        '--------------------------------------------------------------------------------------------------------------------
                        Av_Cod_Des = ""
                        If Agro_SQL_SaveNum(DtDPI.Rows(iDPI).Item("Av_Cod")) <> 0 Then

                            'Lettura Av_Cod

                            Select Case TipoTestata

                                Case 0 'Difesa

                                    DtAvversita = ObjAvversita.Leggi(0, 0, Agro_SQL_SaveNum(DtDPI.Rows(0).Item("Av_Cod")), AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Matrice)

                            End Select

                            If DtAvversita.Rows.Count <> 0 Then

                                Av_Cod_Des = "(" & LCase(DtAvversita.Rows(0).Item("Av_Des_Lat")) & ")" & Chr(13)

                                riga = riga + 1

                            End If

                            Avv_Des = Av_Cod_Des


                        End If

                        '================================================================================================================

                        'Controllo duplicati
                        bDuplicato = False

                        If Not bDuplicato Then

                            If riga <> 0 Then

                                ReDim Preserve Avversita(UBound(Avversita, 1) + 1)
                                Avversita(UBound(Avversita, 1) - 1) = Avv_Des & Epoca_Des

                                DtCriteri = ObjDpi.Leggi_CriteriIntervento(0, CInt(DtDPI.Rows(iDPI).Item("DFT_Cod")), CInt(DtDPI.Rows(iDPI).Item("DFR_Cod")), 0, "", "", objParametri_Disciplinari)

                                If DtCriteri.Rows.Count <> 0 Then

                                    Dim iCrit As Integer
                                    For iCrit = 0 To DtCriteri.Rows.Count - 1

                                        bTrovato = False
                                        For i = 0 To UBound(Criteri, 1) - 1

                                            If UCase(Criteri(i, 0)) = UCase(DtCriteri.Rows(iCrit).Item("TCI_Descrizione")) Then
                                                If IsNumeric(DtCriteri.Rows(iCrit).Item("IdVincolo")) Then
                                                    'Vincolo Presente
                                                    Criteri(1, i) = Criteri(1, i) & "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & CStr(DtCriteri.Rows(iCrit).Item("Descrizione")) & "</FONT>"
                                                Else
                                                    'Vincolo Assente
                                                    Criteri(1, i) = Criteri(1, i) & "<BR>" & " - " & CStr(DtCriteri.Rows(iCrit).Item("Descrizione"))
                                                End If

                                                bTrovato = True

                                                Exit For
                                            End If

                                        Next i

                                        If Not bTrovato Then
                                            ReDim Preserve Criteri(1, UBound(Criteri, 2) + 1)

                                            If IsNumeric(DtCriteri.Rows(iCrit).Item("IdVincolo")) Then
                                                'Vincolo Presente
                                                Criteri(1, UBound(Criteri, 2) - 1) = "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & CStr(DtCriteri.Rows(iCrit).Item("Descrizione")) & "</FONT>"

                                            Else
                                                'Vincolo Assente
                                                Criteri(1, UBound(Criteri, 2) - 1) = " - " & CStr(DtCriteri.Rows(iCrit).Item("Descrizione"))
                                            End If

                                            Criteri(0, UBound(Criteri, 2) - 1) = CStr(DtCriteri.Rows(iCrit).Item("TCI_Descrizione"))

                                        End If

                                    Next iCrit

                                End If




                                '============================================================================================================================================================
                                'Lettura dei Principi Attivi
                                '------------------------------------------------------------------------------------------------------------------------------------------------------------
                                strPA = ""
                                strPercentuale = ""
                                strQta = ""

                                '================================================================================================
                                '     PRINCIPI ATTIVI
                                '------------------------------------------------------------------------------------------------
                                If DtDPI.Rows(iDPI).Item("Pa_Cod") <> 0 Then

                                    'Lettura del principio attivo gias
                                    DtPA = ObjPA.Leggi(CInt(DtDPI.Rows(iDPI).Item("Pa_Cod")), AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Matrice)

                                    If DtPA.Rows.Count <> 0 Then

                                        strPA = strPA & "<I>" & LCase(DtPA.Rows(0).Item("Pa_Des")) & " " & strPostilla & "</I><BR><BR>"

                                    End If


                                ElseIf DtDPI.Rows(iDPI).Item("GRU_PA_Cod") <> 0 Then

                                    'Lettura del principio attivo gias
                                    DtGPA = ObjDpi.Leggi_GruppiPrincipiAttivi(CInt(DtDPI.Rows(iDPI).Item("GRU_PA_Cod")),
                                                                              "",
                                                                              "",
                                                                              objParametri_Disciplinari)

                                    If DtGPA.Rows.Count <> 0 Then

                                        strPA = strPA & "<I>" & LCase(DtGPA.Rows(0).Item("GPA_Des")) & " " & strPostilla & "</I><BR><BR>"

                                    End If

                                End If
                                '================================================================================================



                                'Lettura dei parametri associati al principio attivo
                                If IsNumeric(DtDPI.Rows(iDPI).Item("PercPa")) Then
                                    strPercentuale = DtDPI.Rows(iDPI).Item("PercPa")
                                End If


                                Select Case Gru_Cod

                                    Case 1 'Arboree

                                        If IsDBNull(DtDPI.Rows(iDPI).Item("Dose_Max_Anno")) Then
                                            'Eccezione
                                            Dose_Max = 0
                                        Else
                                            Dose_Max = DtDPI.Rows(iDPI).Item("Dose_Max_Anno") / 2
                                        End If

                                        strQta = CStr(Dose_Max)

                                    Case Else 'Erbacee, Orticole

                                        If IsDBNull(DtDPI.Rows(iDPI).Item("DoseMin")) Then
                                            'Eccezione
                                            Dose_Max = 0
                                        Else
                                            Dose_Max = DtDPI.Rows(iDPI).Item("DoseMin")
                                        End If

                                        If IsDBNull(DtDPI.Rows(iDPI).Item("DoseMax")) Then
                                            'Eccezione
                                            Dose_Max = 0
                                        Else
                                            Dose_Max = DtDPI.Rows(iDPI).Item("DoseMax")
                                        End If

                                        strQta = CStr(Dose_min) & "-" & CStr(Dose_Max)

                                End Select

                            End If
                            '============================================================================================================================================================

                            'Costruzione Criteri
                            strCriteri = ""
                            For i = 0 To UBound(Criteri, 2) - 1

                                strCriteri = strCriteri & "<U>" & Criteri(0, i) & "</U><BR>" & Criteri(1, i) & "<BR>"

                            Next i

                            '=========================================================================================================================================================
                            'Lettura delle NotexTestata
                            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                            strNoteXTestata = ""

                            DtNote = ObjDpi.Leggi_NotexTestataxVincoli(CInt(DtDPI.Rows(iDPI).Item("DFT_Cod")), 0, "", "", objParametri_Disciplinari)

                            If DtNote.Rows.Count <> 0 Then

                                Dim iNote As Integer
                                For iNote = 0 To DtNote.Rows.Count - 1

                                    If IsNumeric(DtNote.Rows(iNote).Item("IDVincolo")) Then

                                        'Nota Vincolata
                                        strNoteXTestata = strNoteXTestata & "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & DtNote.Rows(iNote).Item("Nota") & "</FONT><BR>"

                                    Else

                                        'Nota NON Vincolata
                                        strNoteXTestata = strNoteXTestata & DtNote.Rows(iNote).Item("Nota") & "<BR>"

                                    End If

                                Next iNote

                            End If
                            '=========================================================================================================================================================



                            '=========================================================================================================================================================
                            'Lettura delle NotexRighe
                            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                            strNoteXRighe = ""

                            DtNote = ObjDpi.Leggi_NotexRighexVincoli(CInt(DtDPI.Rows(iDPI).Item("DFR_Cod")), 0, "", "", objParametri_Disciplinari)

                            If DtNote.Rows.Count <> 0 Then

                                Dim iNote As Integer
                                For iNote = 0 To DtNote.Rows.Count - 1

                                    If IsNumeric(DtNote.Rows(iNote).Item("IDVincolo")) Then

                                        'Nota Vincolata
                                        strNoteXRighe = strNoteXRighe & "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & DtNote.Rows(iNote).Item("Nota") & "</FONT><BR>"

                                    Else

                                        'Nota NON Vincolata
                                        strNoteXRighe = strNoteXRighe & DtNote.Rows(iNote).Item("Nota") & "<BR>"

                                    End If

                                Next iNote

                            End If
                            '=========================================================================================================================================================


                            '=========================================================================================================================================================
                            'Lettura delle NotexEpoche
                            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                            strNoteXEpoche = ""

                            DtNote = ObjDpi.Leggi_NotexEpochexRighe(CInt(DtDPI.Rows(iDPI).Item("DFT_Cod")), CInt(DtDPI.Rows(iDPI).Item("DFR_Cod")), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Disciplinari)

                            If DtNote.Rows.Count <> 0 Then

                                Dim iNote As Integer
                                For iNote = 0 To DtNote.Rows.Count - 1

                                    If IsNumeric(DtNote.Rows(iNote).Item("IDVincolo")) Then

                                        'Nota Vincolata
                                        strNoteXEpoche = strNoteXEpoche & "<FONT STYLE='BACKGROUND-COLOR: LIGHTGREY'>" & DtNote.Rows(iNote).Item("Nota") & "</FONT><BR>"

                                    Else

                                        'Nota NON Vincolata
                                        strNoteXEpoche = strNoteXEpoche & DtNote.Rows(iNote).Item("Nota") & "<BR>"

                                    End If

                                Next iNote

                            End If
                            '=========================================================================================================================================================




                            '=========================================================================================================================================================
                            'Inserimento dettaglio in stringa xml
                            XmlDatiDettaglio = XmlDoc.CreateElement("Dettaglio")

                            XmlDatiDettaglio.SetAttribute("av_gru_des", Av_Gru_Des)
                            XmlDatiDettaglio.SetAttribute("ga_dpi_Des", Ga_Dpi_Des)
                            XmlDatiDettaglio.SetAttribute("av_cod_des", Av_Cod_Des)
                            XmlDatiDettaglio.SetAttribute("criteri", strCriteri)
                            XmlDatiDettaglio.SetAttribute("principi_attivi", strPA)
                            XmlDatiDettaglio.SetAttribute("limitazioni", strLimitazioni)
                            XmlDatiDettaglio.SetAttribute("notextestata", strNoteXTestata)
                            XmlDatiDettaglio.SetAttribute("notexrighe", strNoteXRighe)
                            XmlDatiDettaglio.SetAttribute("notexepoche", strNoteXEpoche)
                            XmlDatiDettaglio.SetAttribute("epoca_des", Epoca_Des)
                            XmlDatiDettaglio.SetAttribute("percentuale", strPercentuale)
                            XmlDatiDettaglio.SetAttribute("qta", strQta)

                            XmlDatiRisultati.AppendChild(XmlDatiDettaglio)

                            XmlDatiDettaglio = Nothing
                            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


                        End If 'Fine Check Duplicati

                        LastEp_Cod = Ep_Cod

                    Next iDPI


                End If

                XmlDoc.AppendChild(XmlDatiRisultati)

                Return XmlDoc.OuterXml

            Else

                Return "-1"

            End If

        Catch ex As Exception

            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Disciplinari, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

            'Restituisco un valore Dummy
            Return "-1"

        Finally

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione_Disciplinari) Then
                    xConnessione_Disciplinari.Close()
                    xConnessione_Disciplinari.Dispose()
                End If
                If Not IsNothing(xConnessione_Matrice) Then
                    xConnessione_Matrice.Close()
                    xConnessione_Matrice.Dispose()
                End If
            Else
                If xConnectionState_Disciplinari = ConnectionState.Closed Then
                    xConnessione_Disciplinari.Close()
                End If
                If xConnectionState_Matrice = ConnectionState.Closed Then
                    xConnessione_Matrice.Close()
                End If
            End If

        End Try

    End Function

End Class
