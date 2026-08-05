<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes" encoding="utf-8" />
  
    <xsl:template match="/">
      <root>
      <xsl:apply-templates select="Impresa"/>
      </root>
  </xsl:template>

  <xsl:template match="Esercizio">
    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />
    <xsl:param name = "appezza" />
    <xsl:param name = "id_reg" />

    <Esercizio
     codice_esercizio="{$piva}-{$sa_cod}-{$appezza}-{$id_reg}-{@codice_progetto}"   
      lotto       = "{@lotto}"
      descrizione       = "{@descrizione_progetto}"   
      regolamento ="{@regolamento_des}" 
      capitolato_cod  = "{@capitolato_privato_cod}"
      capitolato_des="{@capitolato_privato_des}" 
      organismo_ref_cod  = "{@org_referente_cod}"
      organismo_ref_des ="{@org_referente_des}" 
      piante_ha  = "{@piante_per_ha}"     
      resa_kg_ha  = "{@resa_prevista}"
     data_inizio="{@validita_inizio}"
     data_fine="{@validita_fine}"     
     >
<!--
      regolamento_cod  = "{@codice_regolamento}"
-->
    </Esercizio>
  </xsl:template>

  <xsl:template match="Impianto">        
    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />
    <xsl:param name = "appezza" />

    <Impianto
     codice_impianto="{$piva}-{$sa_cod}-{$appezza}-{@codice_impianto}"
     specie   = "{@descrizione_specie_gias}"   
     varieta   = "{@descrizione_varieta_gias}"
     tipologia_varietale="{@desc_tipologia_varieta_gias}"
     finalita="{@desc_finalita_gias}"
     portinnesto="{@desc_portinnesto_gias}"
     forma_allevamento ="{@desc_forma_allevamento_gias}"
     impianto_irrigazione = "{@desc_impianto_irrigazione_gias}"    
     copertura = "{@desc_copertura_gias}"
     distanza_su_fila= "{@distanza_su_fila}"
     distanza_tra_fila= "{@distanza_tra_fila}"
     interbina= "{@interbina}"  
     germinabilita_perc="{@germinabilita}"
     superficie="{@sup_imp}"
     data_inizio="{@validita_inizio}"
     data_fine="{@validita_fine}"
     
     >

      <xsl:apply-templates select="Esercizio">
        <xsl:with-param name="piva" select = "$piva" />
        <xsl:with-param name="sa_cod" select = "$sa_cod" />
        <xsl:with-param name="appezza" select = "$appezza" />
        <xsl:with-param name="id_reg" select = "@codice_impianto" />
      </xsl:apply-templates>
      
      <!--
      specie_cod     = "{@codice_specie_gias}"
     specie_des   = "{@descrizione_specie_gias}"   
     varieta_cod   = "{@codice_varieta_gias}"
     varieta_des   = "{@descrizione_varieta_gias}"
     tipologia_varieta_cod   = "{@codice_tipologia_varieta_gias}"
     tipologia_varietale_des="" 
     finalita_cod     = "{@codice_finalita_gias}"
     finalita_des=""
     portinnesto_cod       = "{@codice_portinnesto}"
     portinnesto_des=""
     impianto_irrigazione_cod   = "{@codice_impianto_irrigazione}"
     impianto_irrigazione_des  = ""
     forma_allevamento_cod   = "{@codice_forma_allevamento}"
     forma_allevamento_des =""
     copertura_cod   = "{@codice_copertura}" 
     copertura_des =""
      -->
      
     <!--
             = "{@codice_dettaglio_specie_personalizzato}"
        = "{@codice_specie_cliente}"
        = "{@codice_finalita_cliente}"
        = "{@codice_varieta_cliente}"
        = "{@cod_portinnesto_cliente}"
        = "{@cod_forma_allevamento_cliente}"
        = "{@cod_copertura_cliente}"
        = "{@cod_impianto_irriguo_cliente}"
     -->

    </Impianto>
    
  </xsl:template>

  <xsl:template match="Appezzamento_Particella">
    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />
    <xsl:param name = "appezza" />

    <Appezzamento_Particella
      codice_appezzamento="{$piva}-{$sa_cod}-{$appezza}"
     codice_istat_comune= "{@istat_comune}"
      codice_istat_provincia= "{@istat_provincia}"
      sezione = "{@sezione}"
      foglio  = "{@foglio}"
      numero  = "{@numero}"
      subalterno  = "{@subalterno}"
      sup_intersezione="{@superficie}"
      data_inizio="{@validita_inizio}"
      data_fine="{@validita_fine}"
      />
  </xsl:template>
  
  
  <xsl:template match="Appezzamento">
    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />
    <xsl:param name = "campo_cod" />

    <!--<xsl:variable name = "codice_campo">
      <xsl:choose>
        <xsl:when test="$campo_cod != 0">     
          <xsl:value-of select="$piva - $sa_cod - $campo_cod" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="0" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>-->

    <Appezzamento
      codice_appezzamento="{$piva}-{$sa_cod}-{@codice_appezzamento}"       
      denominazione="{@appezzamento_denominazione}"
      numero_appezzamento_bio = "{@numero_appezzamento_bio}"
      rif_qdc_app="{@rif_qdc_app}"
      metodo_produzione = "{@metodo_produzione_des}"
      sup_appezzamento="{@sup_appezzamento}"
      data_inizio="{@validita_inizio}"
      data_fine="{@validita_fine}"
      >

      <!--codice_campo="{$codice_campo}"-->

      <xsl:apply-templates select="Appezzamento_Particella">
        <xsl:with-param name="piva" select = "$piva" />
        <xsl:with-param name="sa_cod" select = "$sa_cod" />
        <xsl:with-param name="appezza" select = "@codice_appezzamento" />
      </xsl:apply-templates>
      <xsl:apply-templates select="Impianto">
        <xsl:with-param name="piva" select = "$piva" />
        <xsl:with-param name="sa_cod" select = "$sa_cod" />
        <xsl:with-param name="appezza" select = "@codice_appezzamento" />
      </xsl:apply-templates>

    </Appezzamento>
  </xsl:template>

  <xsl:template match="Campo_Particella">
    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />
    <xsl:param name = "campo_cod" />

    <Campo_Particella
      codice_campo="{$piva}-{$sa_cod}-{$campo_cod}"
     codice_istat_comune= "{@istat_comune}"
      codice_istat_provincia= "{@istat_provincia}"
      sezione = "{@sezione}"
      foglio  = "{@foglio}"
      numero  = "{@numero}"
      subalterno  = "{@subalterno}"
      sup_intersezione="{@superficie}"   
   />
  </xsl:template>

  <xsl:template match="Campo">
    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />
    <Campo
      codice_campo = "{$piva}-{$sa_cod}-{@codice_campo}"
      denominazione= "{@campo_denominazione}"
      campo_tipo= "{@campo_tipo_des}"
      data_inizio="{@validita_inizio}"
      data_fine="{@validita_fine}"  
      >
      <xsl:apply-templates select="Campo_Particella">
        <xsl:with-param name="piva" select = "$piva" />
        <xsl:with-param name="sa_cod" select = "$sa_cod" />
        <xsl:with-param name="campo_cod" select = "@codice_campo" />
      </xsl:apply-templates>
      <xsl:apply-templates select="Appezzamento">
        <xsl:with-param name="piva" select = "$piva" />
        <xsl:with-param name="sa_cod" select = "$sa_cod" />
        <xsl:with-param name="campo_cod" select = "@codice_campo" />
      </xsl:apply-templates>
    </Campo>
  </xsl:template>

  <xsl:template match="Particella">
    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />
    <Particella
      codice_centro="{$piva}-{$sa_cod}"
      codice_istat_comune= "{@p_codice_istat_comune}"
      codice_istat_provincia= "{@p_codice_istat_provincia}"
      sezione = "{@sezione}"
      foglio  = "{@foglio}"
      numero  = "{@numero}"
      subalterno  = "{@subalterno}"
      ettari  = "{@ettari}"
      are  = "{@are}"
      centiare  = "{@centiare}"
      superficie_condotta  = "{@sup_condotta}"
      titolo_possesso  = "{@titolo_possesso_des}"
      data_inizio_possesso  = "{@validita_inizio_possesso}"
      data_fine_possesso  = "{@validita_fine_possesso}"
      >
      <!--
      titolo_possesso_cod  = "{@titolo_possesso}"
      titolo_possesso_des  = "{@titolo_possesso_des}"
      -->
    </Particella>
  </xsl:template>
            
  <xsl:template match="CentroAziendale">
    <xsl:param name = "piva" />
      <CentroAziendale
        codice_centro = "{$piva}-{@codice_centro}"
        denominazione= "{@nome_centro}"
        indirizzo= "{@c_indirizzo}"
        frazione= "{@c_frazione}"
        cap= "{@c_cap}"
        comune= "{@c_comune}"
        provincia= "{@c_provincia}"
        stato= "{@c_stato}"
        codice_istat_comune= "{@c_codice_istat_comune}"
        codice_istat_provincia= "{@c_codice_istat_provincia}"       
      >
        <xsl:apply-templates select="Particella">
          <xsl:with-param name="piva" select = "$piva" />
          <xsl:with-param name="sa_cod" select = "@codice_centro" />
        </xsl:apply-templates>
        <xsl:apply-templates select="Appezzamento">
          <xsl:with-param name="piva" select = "$piva" />
          <xsl:with-param name="sa_cod" select = "@codice_centro" />
          <xsl:with-param name="campo_cod" select = "0" />        
        </xsl:apply-templates>
        <xsl:apply-templates select="Campo">
          <xsl:with-param name="piva" select = "$piva" />
          <xsl:with-param name="sa_cod" select = "@codice_centro" />
        </xsl:apply-templates>
      </CentroAziendale>  
  </xsl:template>
  
  <xsl:template match="Impresa">
    <Impresa
            codice_impresa="{@partita_iva}"
            partita_iva="{@partita_iva}"
            cuaa="{@codice_cuaa}"
            codice_fiscale="{@codice_fiscale}"
            codice_socio="{@codice_socio}"
            ggn="{@codice_ggn}"
            ragione_sociale="{@ragione_sociale}"
            data_iscrizione_libro_soci = "{@data_iscrizione_libro_soci}"
            indirizzo= "{@i_indirizzo}"
            frazione= "{@i_frazione}"
            cap= "{@i_cap}"
            comune= "{@i_comune}"
            provincia= "{@i_provincia}"
            stato= "{@i_stato}"
            codice_istat_comune= "{@i_codice_istat_comune}"
            codice_istat_provincia= "{@i_codice_istat_provincia}"           
      >      
      <xsl:apply-templates select="CentroAziendale">
        <xsl:with-param name="piva" select = "@partita_iva" />
      </xsl:apply-templates>
    </Impresa>
  </xsl:template>

</xsl:stylesheet>
