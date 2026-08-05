<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"  encoding="utf-8" />

  <xsl:template match="/DatiImprese">
    <utente username="" password="" codice="">
      <xsl:apply-templates select="Impresa"/>
    </utente>
  </xsl:template>

  <xsl:template match="Impresa">

    <xsl:variable name ="cuaa" select="Codice[@id_cod='1010']/@val_cod">
    </xsl:variable>

    <Impresa
      tipo_operazione="0"
      Codice="{$cuaa}"
      partita_iva="{@piva}"
      ragione_sociale="{@rag_soc}"
      codice_cuaa="{$cuaa}"
      codice_fiscale="">
    </Impresa>


  </xsl:template>

</xsl:stylesheet>
