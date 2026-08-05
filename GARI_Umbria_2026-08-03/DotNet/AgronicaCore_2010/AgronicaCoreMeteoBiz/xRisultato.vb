
Imports System.Data


Public Class xRisultato



    '####################################################################################
    Public Function Errore(
                                ByVal ErrCod As Integer,
                                ByVal ErrMsg As String) _
                                As String

        Dim Apici As String = Chr(34)

        Dim XmlRisultato = "<RISULTATO " &
                            " errcod = " & Apici & ErrCod.ToString & Apici & " " &
                            " errmsg = " & Apici & ErrMsg & Apici & " " &
                            " />"

        Return XmlRisultato

    End Function



    '####################################################################################
    Public Function Dati_Precipitazioni(
                                ByRef DT As DataTable,
                                ByVal Flag_Dettagli As String,
                                ByVal Flag_Aggrega As String,
                                ByVal Flag_Spazio As String,
                                ByVal ID_Quadrante As Integer,
                                ByVal Soglia As Single) _
                                As String

        Dim NumeroRecords As Integer = 0
        Dim NumeroRecordsReale As Integer = 0

        Dim XmlRisultato As String = ""
        Dim XmlRisultatoInizio As String = ""
        Dim XmlRisultatoFine As String = ""
        Dim XmlDato As String = ""
        Dim XmlDati As String = ""

        Dim i As Integer = 0

        Dim Data As Date
        Dim Ora As Integer
        Dim Precipitazione As Single
        Dim OP As String

        Dim SommaPrecipitazioni As Single = 0

        '-----------------------------------------

        NumeroRecords = DT.Rows.Count

        Select Case Flag_Aggrega

            Case "H" '////////////////////////////////////////////////////////////////
                XmlRisultatoInizio =
                                "<RISULTATO " &
                                "idq=^" & ID_Quadrante & "^ " &
                                "num=^" & NumeroRecords & "^ " &
                                "errcod=^0^ " &
                                "errmsg=^^ " &
                                "> "

                For i = 0 To DT.Rows.Count - 1

                    Data = DT.Rows(i).Item("Data").ToString
                    Ora = DT.Rows(i).Item("Ora").ToString
                    Precipitazione = DT.Rows(i).Item("Precipitazione").ToString
                    OP = DT.Rows(i).Item("OP").ToString

                    XmlDato =
                                "<DATO " &
                                "gg=^" & Data & "^ " &
                                "hh=^" & Ora & "^ " &
                                "mm=^" & Precipitazione & "^ "

                    If Flag_Dettagli = "H" Then
                        XmlDato += "op=^" & OP & "^ "
                    End If

                    XmlDato += "/>"
                    XmlDati += XmlDato

                Next

                XmlRisultatoFine = "</RISULTATO>"

            Case "G" '////////////////////////////////////////////////////////////////

                NumeroRecordsReale = 0

                For i = 0 To DT.Rows.Count - 1

                    Data = DT.Rows(i).Item("Data").ToString
                    Precipitazione = DT.Rows(i).Item("Precipitazione24ore").ToString

                    If CSng(Precipitazione) >= Soglia Then

                        XmlDato =
                                "<DATO " &
                                "gg=^" & Data & "^ " &
                                "mm=^" & Precipitazione & "^ "

                        XmlDato += "/>"
                        XmlDati += XmlDato

                        NumeroRecordsReale += 1

                    End If

                Next

                XmlRisultatoInizio =
                                "<RISULTATO " &
                                "idq=^" & ID_Quadrante & "^ " &
                                "num=^" & NumeroRecordsReale & "^ " &
                                "errcod=^0^ " &
                                "errmsg=^^ " &
                                "> "

                XmlRisultatoFine = "</RISULTATO>"


            Case "I" '////////////////////////////////////////////////////////////////
                XmlRisultatoInizio =
                                "<RISULTATO " &
                                "idq=^" & ID_Quadrante & "^ " &
                                "num=^1^ " &
                                "errcod=^0^ " &
                                "errmsg=^^ " &
                                "> "

                SommaPrecipitazioni = 0

                For i = 0 To DT.Rows.Count - 1
                    Precipitazione = CSng(DT.Rows(i).Item("Precipitazione24ore").ToString)
                    SommaPrecipitazioni += Precipitazione
                Next

                XmlDato = "<DATO mm=^" & SommaPrecipitazioni & "^ />"

                XmlDati = XmlDato

                XmlRisultatoFine = "</RISULTATO>"

        End Select

        '-----------------------------------------

        XmlRisultato = (XmlRisultatoInizio & XmlDati & XmlRisultatoFine).Replace("^", Chr(34))

        Return XmlRisultato

    End Function

    Public Function Dati_Completi(
                                ByRef DT As DataTable,
                                ByVal Flag_Dettagli As String,
                                ByVal Flag_Aggrega As String,
                                ByVal Flag_Spazio As String,
                                ByVal ID_Quadrante As Integer,
                                ByVal Soglia As Single,
                                ByVal flag_tabella_leggi As String) _
                                As String

        Dim NumeroRecords As Integer = 0
        Dim NumeroRecordsReale As Integer = 0

        Dim XmlRisultato As String = ""
        Dim XmlRisultatoInizio As String = ""
        Dim XmlRisultatoFine As String = ""
        Dim XmlDato As String = ""
        Dim XmlDati As String = ""

        Dim i As Integer = 0

        Dim Data As Date
        Dim Ora As Integer
        Dim Precipitazione As Single
        Dim Temperatura As Single
        Dim Temp_min As Single
        Dim Temp_max As Single
        Dim Vento_int As Single
        Dim Vento_dir As Integer
        Dim EvapoTraspirazione As Double
        Dim Bagnatura As Single
        Dim Umidita As Single
        Dim OP As String

        Dim SommaPrecipitazioni As Single = 0

        '-----------------------------------------

        NumeroRecords = DT.Rows.Count


        XmlRisultatoInizio =
            "<RISULTATO " &
            "idq=^" & ID_Quadrante & "^ " &
            "num=^" & NumeroRecords & "^ " &
            "errcod=^0^ " &
            "errmsg=^^ " &
            "> "

        For i = 0 To DT.Rows.Count - 1

            Data = DT.Rows(i).Item("Data").ToString
            Ora = DT.Rows(i).Item("Ora").ToString
            OP = DT.Rows(i).Item("OP").ToString
            Temperatura = DT.Rows(i)("Temp_Media").ToString

            If flag_tabella_leggi = "H" Then
                Umidita = DT.Rows(i)("UmiditaRelativa").ToString
                Bagnatura = DT.Rows(i)("Bagnatura").ToString
                Precipitazione = DT.Rows(i).Item("Precipitazione").ToString

            ElseIf flag_tabella_leggi = "M" Then 'GABRIELE da verificare

                Umidita = DT.Rows(i)("UmiditaRelativa").ToString
                Precipitazione = DT.Rows(i).Item("Precipitazione").ToString
                Temp_min = DT.Rows(i)("Temp_Min").ToString
                Temp_max = DT.Rows(i)("Temp_Max").ToString
                Vento_dir = DT.Rows(i)("Vento_dir").ToString
                Vento_int = DT.Rows(i)("Vento_int").ToString
                EvapoTraspirazione = DT.Rows(i)("EvapoTraspirazione").ToString

            Else
                Temp_min = DT.Rows(i)("Temp_Min").ToString
                Temp_max = DT.Rows(i)("Temp_Max").ToString
                Vento_dir = DT.Rows(i)("Vento_dir").ToString
                Vento_int = DT.Rows(i)("Vento_int").ToString
                EvapoTraspirazione = DT.Rows(i)("EvapoTraspirazione").ToString

            End If

            XmlDato =
                "<DATO " &
                "gg=^" & Data & "^ " &
                "hh=^" & Ora & "^ " &
                "mm=^" & Precipitazione & "^ " &
                "t=^" & Temperatura & "^ " &
                "t_min=^" & Temp_min & "^ " &
                "t_max=^" & Temp_max & "^ " &
                "v_dir=^" & Vento_dir & "^ " &
                "v_int=^" & Vento_int & "^ " &
                "et=^" & EvapoTraspirazione & "^ " &
                "u=^" & Umidita & "^ " &
                "b=^" & Bagnatura & "^ "

            If Flag_Dettagli = "H" Then
                XmlDato += "op=^" & OP & "^ "
            End If

            XmlDato += "/>"
            XmlDati += XmlDato

        Next

        XmlRisultatoFine = "</RISULTATO>"





        '-----------------------------------------

        XmlRisultato = (XmlRisultatoInizio & XmlDati & XmlRisultatoFine).Replace("^", Chr(34))

        Return XmlRisultato

    End Function



    '####################################################################################    
    Public Function Dati_ListaStazioniDistanza(
                                ByRef DT As DataTable) _
                                As String

        Dim NumeroRecords As Integer = 0
        Dim NumeroRecordsReale As Integer = 0

        Dim XmlRisultato As String = ""
        Dim XmlRisultatoInizio As String = ""
        Dim XmlRisultatoFine As String = ""
        Dim XmlDato As String = ""
        Dim XmlDati As String = ""

        Dim i As Integer = 0

        Dim nome, descrizione As String
        Dim distanza, f_longitude, f_latitude As Double
        Dim data_ultimo_agg As Date
        Dim stazione_cod_fornitore As String


        '-----------------------------------------

        NumeroRecords = DT.Rows.Count



        For i = 0 To DT.Rows.Count - 1

            distanza = "-1"
            f_longitude = "-1"
            f_latitude = "-1"

            nome = DT.Rows(i).Item("nome").ToString
            descrizione = DT.Rows(i).Item("descrizione").ToString
            stazione_cod_fornitore = DT.Rows(i).Item("stazione_cod_fornitore").ToString

            If Not IsDBNull(DT.Rows(i).Item("distanza")) Then
                distanza = CDbl(DT.Rows(i).Item("distanza"))
            End If

            If Not IsDBNull(DT.Rows(i).Item("f_longitude")) Then
                Dim val As String = DT.Rows(i).Item("f_longitude").ToString.Replace("N", "").Replace("E", "")
                If IsNumeric(val) Then
                    f_longitude = CDbl(val)
                End If

            End If

            If Not IsDBNull(DT.Rows(i).Item("f_latitude")) Then
                Dim val As String = CDbl(DT.Rows(i).Item("f_latitude").ToString.Replace("N", "").Replace("E", ""))
                If IsNumeric(val) Then
                    f_latitude = CDbl(val)
                End If

            End If

            If Not IsDBNull(DT.Rows(i).Item("data_ultimo_agg")) Then
                data_ultimo_agg = DT.Rows(i).Item("data_ultimo_agg")
            End If



            XmlDato =
                    "<DATO " &
                    "nome=^" & nome & "^ " &
                    "stazione_cod_fornitore=^" & stazione_cod_fornitore & "^ " &
                    "descrizione=^" & descrizione & "^ " &
                    "distanza=^" & distanza & "^ " &
                    "f_longitude=^" & f_longitude & "^ " &
                    "f_latitude=^" & f_latitude & "^ " &
                    "data_ultimo_agg=^" & data_ultimo_agg.Date & "^ "

            XmlDato += "/>"
            XmlDati += XmlDato

            NumeroRecordsReale += 1



        Next

        XmlRisultatoInizio = "<RISULTATO> "

        XmlRisultatoFine = "</RISULTATO>"

        '-----------------------------------------

        XmlRisultato = (XmlRisultatoInizio & XmlDati & XmlRisultatoFine).Replace("^", Chr(34))

        Return XmlRisultato

    End Function






End Class
