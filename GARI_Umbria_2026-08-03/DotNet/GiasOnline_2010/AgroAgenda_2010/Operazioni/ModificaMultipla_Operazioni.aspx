<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master"
    CodeBehind="ModificaMultipla_Operazioni.aspx.vb" Inherits="AgroAgenda_2010.ModificaMultipla_Operazioni" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">

<script language="javascript" type="text/javascript">
   

        function SelezionaDeselezionaTutti() {
            if ($('#chkSelezionaTutte').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaOperazione').each(function () {
                    $(this).children('input').prop('checked', 'checked');
                });
            }
            else {
                //seleziono tutto
                $('.ChkSelezionaOperazione').each(function () {
                    $(this).children('input').removeProp('checked');
                });
            }
        }

        function SelezionaDeselezionaTuttiContatti() {
            if ($('#chkSelezionaTuttiContatti').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaContatto').each(function () {
                    $(this).children('input').prop('checked', 'checked');
                });
            }
            else {
                //seleziono tutto
                $('.ChkSelezionaContatto').each(function () {
                    $(this).children('input').removeProp('checked');
                });
            }
        }


        function SelezionaDeselezionaTutteMacchine() {
            if ($('#chkSelezionaTutteMacchine').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaMacchina').each(function () {
                    $(this).children('input').prop('checked', 'checked');
                });
            }
            else {
                //seleziono tutto
                $('.ChkSelezionaMacchina').each(function () {
                    $(this).children('input').removeProp('checked');
                });
            }
        }

        $(document).ready(function () {
            $(document).on('click', '#chkSelezionaTutte', function () {
                SelezionaDeselezionaTutti();
            });
            $(document).on('click', '#chkSelezionaTuttiContatti', function () {
                SelezionaDeselezionaTuttiContatti();
            });
            $(document).on('click', '#chkSelezionaTutteMacchine', function () {
                SelezionaDeselezionaTutteMacchine();
            });
            $('.bottone').button();
        }); 

        
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">

         <asp:UpdatePanel ID="UpdatePanel_Ricerca" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
    <table aria-hidden="true">
        <tr>
            <td>
                <table aria-hidden="true">
                    <tr>
                        <td>
                            <b>Modifica :</b>
                        </td>
                        <td>
                            <asp:DropDownList ID="Combo_Modifiche" runat="server" AutoPostBack="true" CssClass="">
                                <asp:ListItem Text="" Value="0"> </asp:ListItem>
                                <asp:ListItem Text="Modifica operazioni a seguito di Modifica Sup. Impianti in anagrafica, mantenendo Qta Prodotto Totale" Value="-5"> </asp:ListItem>
                                <asp:ListItem Text="Modifica operazioni a seguito di Modifica Sup. Impianti in anagrafica, mantenendo Qta Ha Prodotto" Value="-6"> </asp:ListItem>
<%--                                <asp:ListItem Text="Elimina Impianti Doppi Mantenendo Qta Prodotto Totale" Value="-4"> </asp:ListItem>
--%>                            
                                <asp:ListItem Value="-3">Assegna Magazzino</asp:ListItem>
                                <asp:ListItem Value="-1">Aggiungi Macchine / Attrezzature</asp:ListItem>
                                <asp:ListItem Value="-7">Elimina Macchine / Attrezzature</asp:ListItem>
                                <asp:ListItem Value="-2">Aggiungi Manodopera / Terzista / Tecnico Responsabile</asp:ListItem>
                                <asp:ListItem Value="-8">Elimina Manodopera / Terzista / Tecnico Responsabile</asp:ListItem>
                            </asp:DropDownList>
                           
                           
                        </td>
                        <td>
                            <asp:Button ID="Btn_Salva" runat="server" Text="SALVA" Style="margin-left: 5px; margin-right: 5px;"
                                CssClass="bottone" />
                        </td>
                    </tr>
                    <tr id="Riga_Magazzino" runat="server">
                        <td>
                            <b>Magazzino :</b>
                        </td>
                        <td colspan="2">
                            <cc1:ComboMagazzini ID="ComboMagazzini" runat="server" Fabbricato_Cod="0" Flag_CodCentroFabbricato="True"
                                Flag_GestioneMagazziniImpresaPadre="False" meta:resourcekey="ComboMagazziniResource1"
                                Sa_Cod="0" TipoMagazzino="20" />
                        </td>
                        
                    </tr>

                    <tr id="Riga_Macchine" runat="server">
                    
                    <td colspan="3"> <table aria-hidden="true"><tr><td>
                    <div style=" background-color:#f0e68c; width:70px; display: inline-block;">&nbsp;    </div>
                    <div style=" margin-left:10px; display: inline-block;"> Pubbliche </div>
                    <div style=" background-color:#98fb98; width:70px ; margin-left:15px; display: inline-block;">&nbsp; </div>
                    <div style=" margin-left:10px; margin-right:10px; display: inline-block;"> Aziendali</div>
                    <div style=" margin-left:10px;  display: inline-block;"> <asp:CheckBox id="chk_solo_privati_macchine" runat="server"  AutoPostBack="True"
							 Text="Solo Aziendali" ></asp:CheckBox></div>
                    <div style=" margin-left:10px;  display: inline-block;"> <asp:CheckBox id="chk_elimina_precedenti_macchine" runat="server"  
							 Text="Elimina macchine/attrezzature eventualmente già associate" ></asp:CheckBox></div>
                    </td></tr>
                    <tr><td>
                                                                    <asp:GridView ID="GridView_Macchine" runat="server" AutoGenerateColumns="False"
                                CellPadding="5" CellSpacing="0" CssClass="ui-widget-content" AllowPaging="false">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <input type="checkbox" id="chkSelezionaTutteMacchine" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="ChkSelezionaMacchina" runat="server" CssClass="ChkSelezionaMacchina" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="20px" />
                                        <ItemStyle Width="20px" />
                                        <FooterStyle Width="20px" />
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Mac_Cod" HeaderText="Mac_Cod" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                  
                                    <asp:BoundField DataField="CLASS_DESC" HeaderText="Classe" HtmlEncode="false" SortExpression="Classe">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Mac_Des" HeaderText="Macchina" HtmlEncode="false" >
                                                                       </asp:BoundField>
                                    
                                    <asp:BoundField DataField="Modello" HeaderText="Modello" HtmlEncode="false" SortExpression="Modello">
                                    </asp:BoundField>
                                    
                                                 <asp:BoundField DataField="Targa" HeaderText="Targa" HtmlEncode="false" SortExpression="Targa">
                                    </asp:BoundField>

                                                 <asp:BoundField DataField="Prezzo_Unitario" HeaderText="Prezzo Unitario" HtmlEncode="false" >
                                    </asp:BoundField>
                                                 <asp:BoundField DataField="Unita_Misura" HeaderText="Unita Misura" HtmlEncode="false" >
                                    </asp:BoundField>
                                                 <asp:BoundField DataField="Costo_Inizio" HeaderText="Validita Inizio" HtmlEncode="false">
                                    </asp:BoundField>
                                                 <asp:BoundField DataField="Costo_Fine" HeaderText="Validita Fine" HtmlEncode="false">
                                    </asp:BoundField>
                                                 <asp:BoundField DataField="Impresa" HeaderText="Impresa Referente" HtmlEncode="false" SortExpression="Impresa">
                                    </asp:BoundField>

                                </Columns>
                                <HeaderStyle CssClass="ui-widget-header" />
                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                            </asp:GridView>

</td></tr>
                    </table>

                    
                    </td></tr>

                                        <tr id="Riga_Contatti" runat="server">
                    
                    <td colspan="3"> <table aria-hidden="true"><tr><td>
                    <div style=" background-color:#f0e68c; width:70px; display: inline-block;">&nbsp;    </div>
                    <div style=" margin-left:10px; display: inline-block;"> Pubblici </div>
                    <div style=" background-color:#98fb98; width:70px ; margin-left:15px; display: inline-block;">&nbsp; </div>
                    <div style=" margin-left:10px; margin-right:10px; display: inline-block;"> Aziendali</div>
                    <div style=" margin-left:10px;  display: inline-block;"> <asp:CheckBox id="chk_solo_privati_contatti" runat="server"  AutoPostBack="True"
							 Text="Solo Aziendali" ></asp:CheckBox></div>
                                     <div style=" margin-left:10px;  display: inline-block;"> <asp:CheckBox id="chk_elimina_precedenti_contatti" runat="server"  
							 Text="Elimina contatti eventualmente già associati" ></asp:CheckBox></div>

                    </td></tr>
                    <tr><td>
                                                                    <asp:GridView ID="GridView_Contatti" runat="server" AutoGenerateColumns="False"
                                CellPadding="5" CellSpacing="0" CssClass="ui-widget-content" AllowPaging="false">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <input type="checkbox" id="chkSelezionaTuttiContatti" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="ChkSelezionaContatto" runat="server" CssClass="ChkSelezionaContatto" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="20px" />
                                        <ItemStyle Width="20px" />
                                        <FooterStyle Width="20px" />
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Cod_Contatto" HeaderText="Cod_Contatto" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                  
                                    <asp:BoundField DataField="Rag_Soc_Nome_Cognome" HeaderText="Ragione Sociale / Nome Cognome" HtmlEncode="false" SortExpression="Rag_Soc_Nome_Cognome">
                                    </asp:BoundField>

                                        <asp:BoundField DataField="Cod_Rapporto" HeaderText="Cod_Rapporto" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Rapporto_Des" HeaderText="Rapporto Contabile" HtmlEncode="false" >
                                                                       </asp:BoundField>
                                    
                                  
                                                 <asp:BoundField DataField="Prezzo_Unitario" HeaderText="Prezzo Unitario" HtmlEncode="false" >
                                    </asp:BoundField>
                                                 <asp:BoundField DataField="Unita_Misura" HeaderText="Unita Misura" HtmlEncode="false" >
                                    </asp:BoundField>

                                          <asp:BoundField DataField="Mezzo" HeaderText="Mezzo" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                          <asp:BoundField DataField="Cod_RisUm" HeaderText="Cod_RisUm" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                                 <asp:BoundField DataField="Costo_Inizio" HeaderText="Validita Inizio" HtmlEncode="false">
                                    </asp:BoundField>
                                                 <asp:BoundField DataField="Costo_Fine" HeaderText="Validita Fine" HtmlEncode="false">
                                    </asp:BoundField>
                                                 <asp:BoundField DataField="Impresa" HeaderText="Impresa Referente" HtmlEncode="false" SortExpression="Impresa">
                                    </asp:BoundField>
                                         <asp:BoundField DataField="Dipendente" HeaderText="FlagManodopera" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                         <asp:BoundField DataField="Terzista" HeaderText="FlagTerzista" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                </Columns>
                                <HeaderStyle CssClass="ui-widget-header" />
                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                            </asp:GridView>

</td></tr>
                    </table>

                    
                    </td></tr>


                </table>
            </td>
        </tr>
        <tr>
        <td>
          <asp:UpdateProgress ID="UpdateProgress_Ricerca" runat="server" AssociatedUpdatePanelID="UpdatePanel_Ricerca"
                                        DisplayAfter="0">
                                        <ProgressTemplate>
                                            <img src="../AB_Immagini/Messaggi/pleasewait.gif" style="margin-top: 3px; margin-left: 20px;
                                                margin-right: 20px" alt="Ricerca in corso..." />
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
        </td>
        </tr>
        <tr>
            <td>
                <table aria-hidden="true">
                    <tr>
                        <td>
                            <asp:GridView ID="GridView_Operazioni" runat="server" AutoGenerateColumns="False"
                                CellPadding="5" CellSpacing="0" CssClass="ui-widget-content" AllowPaging="false">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <input type="checkbox" id="chkSelezionaTutte" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="ChkSelezionaOperazione" runat="server" CssClass="ChkSelezionaOperazione" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="20px" />
                                        <ItemStyle Width="20px" />
                                        <FooterStyle Width="20px" />
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id_Agenda" HeaderText="Id_Agenda" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Lav_Cod" HeaderText="Lav_Cod" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>                                  
                                    <asp:BoundField DataField="Data_Movimento" HeaderText="Data" HtmlEncode="false" SortExpression="Data">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="rag_soc" HeaderText="Ragione Sociale" HtmlEncode="false">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    
                                    <asp:BoundField DataField="des_lib" HeaderText="Operazione" HtmlEncode="false" SortExpression="Data">
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Utilizzo">
                                        <ItemTemplate>
                                            <asp:TextBox ID="ha_utilizzo" runat="server" CssClass="txtUI" Style="width: 80px"
                                                Text="0" />
                                        </ItemTemplate>
<%--                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />--%>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="ui-widget-header" />
                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

    </table>
            </ContentTemplate>
        </asp:UpdatePanel>
</asp:Content>
