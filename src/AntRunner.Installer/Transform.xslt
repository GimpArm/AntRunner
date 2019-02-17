<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
                xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">
  <xsl:output method="xml" indent="yes"/>
  <xsl:key
    name="Exclude"
    use="@Id"
    match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 3 ) = '.pdb' ] | 
          wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 9 ) = '.deps.json' ] |
          wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 18 ) = 'Newtonsoft.Json.xml' ] |
          wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 18 ) = 'System.Waf.Core.xml' ] |
          wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 17 ) = 'System.Waf.Wpf.xml' ]" />

  <!-- copy everything verbatim -->
  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>

  <!-- except "Component" nodes -->
  <xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'Exclude', @Id ) ]" />
</xsl:stylesheet>