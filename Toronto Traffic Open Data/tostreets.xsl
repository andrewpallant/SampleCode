<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>
  <xsl:template match="/TrafficData">
    <h1 style="font-weight:bold; font-size:1.4em;">TO Streets</h1>
    Data Source [<a href="http://www.toronto.ca/trafficimages/RescuData/UtcsData.xml  ">XML</a>]
    <hr></hr>
    <table>
      <xsl:apply-templates select="Division" />
    </table>
  </xsl:template>

  <xsl:template match="Division">
    <tr>
      <td>
        <xsl:value-of select="@LongName"/>
        <xsl:apply-templates select="Cameras/Camera" />
      </td>
    </tr>
  </xsl:template>
  <xsl:template match="Cameras/Camera">
    <tr>
      <td>
        <table style="border: inset 1px #eeeeee; background-color:#dddddd;width:800px;">
          <tr>
            <td>
              <img src="{ImageUrl}"></img>
              <br></br>
              <xsl:apply-templates select="RefUrl" />
            </td>
            <td valign="top">
              <b>Main Road: </b><xsl:value-of select="MainRoad"/><br></br>
              <b>Cross Road: </b><xsl:value-of select="CrossRoad"/>          
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </xsl:template>
  <xsl:template match="RefUrl">
    <img src="{.}" style="display:inline;"></img>
  </xsl:template>
</xsl:stylesheet>
