<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="xml" indent="yes"  encoding="utf-8" />

  <xsl:template match="/DatiImprese">
    <root>
      <xsl:apply-templates select="Impresa"/>
    </root>
  </xsl:template>

  <xsl:template match="Impresa">

    <xsl:variable name ="cuaa" select="Codice[@id_cod='1010']/@val_cod">
    </xsl:variable>
    
    <xsl:variable name ="codice_socio" select="Codice[@id_cod='1033']/@val_cod">
    </xsl:variable>

    <Impresa
      codice_impresa="{@piva}"
      partita_iva="{@piva}"
      ragione_sociale="{@rag_soc}"
      cuaa="{$cuaa}"
      codice_socio="{$codice_socio}"
      >
    </Impresa>
     
  </xsl:template>
  
</xsl:stylesheet>
