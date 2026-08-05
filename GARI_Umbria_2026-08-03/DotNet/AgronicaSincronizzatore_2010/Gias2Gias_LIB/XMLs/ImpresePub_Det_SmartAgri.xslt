<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes" encoding="utf-8" />

  <xsl:template match="/DatiImprese">
    <utente username="" password="" codice="">
      <xsl:apply-templates select="Impresa"/>
    </utente>
  </xsl:template>

  <xsl:template match="Impianto">


    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />
    <xsl:param name = "appezza" />

    <Impianto
     tipo_operazione="0"
     codice_impianto="{$piva}-{$sa_cod}-{$appezza}-{@codice_impianto}"
     codice_specie ="{@descrizione_specie_gias}"
     codice_varieta ="{@descrizione_varieta_gias}"
     validita_inizio="{@validita_inizio}"
     validita_fine="{@validita_fine}"
     sup_imp="{@sup_imp}" />

  </xsl:template>

  <xsl:template match="Appezzamento">

    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />

    <Appezzamento
      tipo_operazione="0"
      codice_appezzamento="{$piva}-{$sa_cod}-{@codice_appezzamento}"
      appezzamento_denominazione="{@appezzamento_denominazione}"
      coltura_precedente_1 ="{@coltura_precedente_1}"
      coltura_precedente_2 ="{@coltura_precedente_2}"
      coltura_precedente_3 ="{@coltura_precedente_3}"
      coltura_precedente_4 ="{@coltura_precedente_4}"
      coltura_precedente_5 ="X"      
      gis="{@gis}"
      gis_epsg="3857"
      sup_appezzamento="{@sup_appezzamento}"
      validita_inizio="{@validita_inizio}"
      validita_fine="{@validita_fine}">

      <xsl:apply-templates select="Impianto">
        <xsl:with-param name="piva" select = "$piva" />
        <xsl:with-param name="sa_cod" select = "$sa_cod" />
        <xsl:with-param name="appezza" select = "@codice_appezzamento" />
      </xsl:apply-templates>

    </Appezzamento>
  </xsl:template>

  
  <xsl:template match="Campo">
    <xsl:param name = "piva" />
    <xsl:param name = "sa_cod" />
      
    <!--il template dei campi non richiede di creare alcun elemento se non chiamare il template appezzamenti-->
      <xsl:apply-templates select="Appezzamento">
        <xsl:with-param name="piva" select = "$piva" />
        <xsl:with-param name="sa_cod" select = "$sa_cod" />        
      </xsl:apply-templates>
    
  </xsl:template>
        
  <xsl:template match="CentroAziendale">
    <xsl:param name = "piva" />
    <CentroAziendale>      
      
      <xsl:apply-templates select="Appezzamento">
        <xsl:with-param name="piva" select = "$piva" />
        <xsl:with-param name="sa_cod" select = "@codice_centro" />
      </xsl:apply-templates>
    
      <!--applica il template dei campi per leggere gli appezzamenti figli di campo-->
      <xsl:apply-templates select="Campo">
          <xsl:with-param name="piva" select = "$piva" />
          <xsl:with-param name="sa_cod" select = "@codice_centro" />
      </xsl:apply-templates>
    </CentroAziendale>
  </xsl:template>

  <xsl:template match="Impresa">

    <!--il primo attributo "gis" valorizzato nei centri figli-->
    <xsl:variable name = "gisPrimoCentro">
      <xsl:value-of select="CentroAziendale[string-length(@gis) > 0]/@gis" />
    </xsl:variable>
    
    <Impresa
      tipo_operazione="0"
      Codice="{@codice_cuaa}"
      partita_iva="{@partita_iva}"
      ragione_sociale="{@ragione_sociale}"
      codice_cuaa="{@codice_cuaa}"
      codice_fiscale=""
      gis="{$gisPrimoCentro}"
      gis_epsg="3857">
      <xsl:apply-templates select="CentroAziendale">
        <xsl:with-param name="piva" select = "@partita_iva" />
      </xsl:apply-templates>
    </Impresa>


  </xsl:template>

</xsl:stylesheet>
