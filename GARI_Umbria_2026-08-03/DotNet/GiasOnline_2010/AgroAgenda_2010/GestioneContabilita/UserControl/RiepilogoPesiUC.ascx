<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RiepilogoPesiUC.ascx.vb" Inherits="AgroAgenda_2010.RiepilogoPesiUC" %>

<div id="idFormRiepilogoPesi" class="panel-group">

    <ul id="panelbarRiepilogoPesi" style="margin-top: 10px;">
        <li class="k-state-active k-active">
            <span class="k-link k-state-selected k-selected">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RiepilogoPesi %>" runat="server"></asp:Localize>
            </span>
            <div class="row first-row">
                <div class="col-lg-5 col-md-5 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblPesoTotaleRiepilogo" for="inPesoTotaleRiepilogo">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PesoTotaleKg %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="inPesoTotaleRiepilogo" id="inPesoTotaleRiepilogo" class="form-control text-right" disabled="disabled" />
                    </div>

                    <ul class="list-group">
                        <li class="list-group-item">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblTaraVeicoloRiepilogo" for="inTaraVeicoloRiepilogo">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TaraVeicoloKg %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="inTaraVeicoloRiepilogo" id="inTaraVeicoloRiepilogo" class="form-control text-right" />
                            </div>
                        </li>
                        <li class="list-group-item">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblImballiVuotiRiepilogo" for="inImballiVuotiRiepilogo">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ImballiVuotiKg %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="inImballiVuotiRiepilogo" id="inImballiVuotiRiepilogo" class="form-control text-right" disabled="disabled" />
                            </div>
                        </li>
                        <li class="list-group-item">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblPesoLordoRiepilogo" for="inPesoLordoRiepilogo">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PesoLordoKg %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="inPesoLordoRiepilogo" id="inPesoLordoRiepilogo" class="form-control text-right" disabled="disabled" />
                            </div>
                            <ul class="riepilogo-pesi">
                                <li class="riepilogo-pesi fa-minus xonne-btn-primary">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="lblTaraImballiRiepilogo" for="inTaraImballiRiepilogo">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TaraImballiKg %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input name="inTaraImballiRiepilogo" id="inTaraImballiRiepilogo" class="form-control text-right" disabled="disabled" />
                                    </div>
                                </li>
                                <li class="riepilogo-pesi fa-arrow-right">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="lblPesoNettoRiepilogo" for="inPesoNettoRiepilogo">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PesoNettoKg %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input name="inPesoNettoRiepilogo" id="inPesoNettoRiepilogo" class="form-control text-right" disabled="disabled" />
                                    </div>

                                    <ul class="riepilogo-pesi" style="padding-inline-start: 40px !important;">
                                        <li class="riepilogo-pesi fa-minus"  id="idValoreDegradoRiepilogo">
                                            <div class="input-group">
                                                <label class="input-group-addon" id="lblValoreDegradoRiepilogo" for="inValoreDegradoRiepilogo">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ValoreDegradoKg %>" runat="server"></asp:Localize>:
                                                </label>
                                                <input name="inValoreDegradoRiepilogo" id="inValoreDegradoRiepilogo" class="form-control text-right" disabled="disabled" />
                                            </div>
                                        </li>
                                        <li class="riepilogo-pesi fa-arrow-right"  id="idPesoPagamentoRiepilogo">
                                            <div class="input-group">
                                                <label class="input-group-addon" id="lblPesoPagamentoRiepilogo" for="inPesoPagamentoRiepilogo">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PesoAPagamentoKg %>" runat="server"></asp:Localize>:
                                                </label>
                                                <input name="inPesoPagamentoRiepilogo" id="inPesoPagamentoRiepilogo" class="form-control text-right" disabled="disabled" />
                                            </div>

                                        </li>
                                    </ul>

                                </li>
                            </ul>
                        </li>
                    </ul>

                </div>
            </div>

        </li>
    </ul>

</div>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/RiepilogoPesiUC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/RiepilogoPesiUC_jQueryDocReady.js")) %>"></script>

<style type="text/css">
    ul.riepilogo-pesi {
        padding-inline-start: 5px;
    }

    li.riepilogo-pesi {
        list-style-type: none;
    }

        li.riepilogo-pesi:before {
            font-family: FontAwesome, sans-serif;
            font-size: 2.5em; /* fa-3x */
            margin: 0 8px 0 5px; /* fa */
            float: left;
            color: #052747; /*#428bca;*/
            /* fa */
            display: inline-block;
            /*font: normal normal normal 14px/1 FontAwesome;*/
            text-rendering: auto;
            -webkit-font-smoothing: antialiased;
            -moz-osx-font-smoothing: grayscale;
        }
</style>
