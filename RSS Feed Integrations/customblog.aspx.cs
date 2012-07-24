using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.IO;
using System.Xml.XPath;
using System.Xml;
using System.Text.RegularExpressions;

public partial class customblog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get RSS Feed
        WebRequest r = HttpWebRequest.Create("http://madprogrammer76.wordpress.com/feed/");
        WebResponse re = r.GetResponse();
        XmlDocument doc = new XmlDocument();
        using (StreamReader sr = new StreamReader(re.GetResponseStream()))
        {
            doc.LoadXml(sr.ReadToEnd());
        }

        // Translate Page Name To Node To Fetch
        String pageName = Request.RawUrl.Replace("blog_","");
        pageName = pageName.Replace("_", " ").Replace(".aspx", "").Replace("/","");
        pageName = pageName.Replace("unlatched.com", "");

        // Build Table
        XmlNodeList entries = doc.DocumentElement.GetElementsByTagName("item");

        // Set Up XML Document / Manager
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
        nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
        XmlNodeList nodes = doc.DocumentElement.SelectNodes(".");
        XmlNode node = nodes[0].ParentNode;

        // Get Parent Node
        XmlNode n = nodes[0].SelectSingleNode("channel");

        // Find Requested Node
        nodes = n.SelectNodes("item[title='" + pageName + "']", nsmgr);

        String title = "";
        String published = "";
        String updated = "";
        String content = "";
        String category = "";
        String alternate = "";

        // Parse Nodes Into Usable Variables
        XmlNode entry = nodes[0];
        int count = 0;
        foreach (XmlNode tag in entry.ChildNodes)
        {
            if (tag.Name.Equals("title"))
            {
                title = tag.InnerText;
            }
            if (tag.Name.Equals("pubDate"))
            {
                published = tag.InnerText;
            }
            if (tag.Name.Equals("updated"))
            {
                updated = tag.InnerText;
            }
            if (tag.Name.Equals("content:encoded"))
            {
                content = getValue(tag);

                if (content.Length == 0)
                {
                    content = tag.InnerText;
                }
            }
            if (tag.Name.Equals("content"))
            {
                content = getValue(tag);
            }
            if (tag.Name.Equals("category"))
            {
                category += getValue(tag);
            }

            if (tag.Name.Equals("link"))
            {
                alternate = tag.InnerText;
                
            }
        }

        // Build Page
        HtmlGenericControl gen = new HtmlGenericControl("div");
        gen.InnerHtml = content;

        HtmlGenericControl h2 = new HtmlGenericControl("h2");
        h2.InnerHtml = title;
        
        HtmlGenericControl h2Published = new HtmlGenericControl();
        h2Published.InnerHtml = "Published: " + formatDate(published) + "<br>";
        
        HtmlGenericControl h2Updated = new HtmlGenericControl();
        h2Updated.InnerHtml = "Updated: " + formatDate(updated) + "<br>";

        HtmlGenericControl physical = new HtmlGenericControl();
        physical.InnerHtml = "Physical Link: <a href=\"" + alternate + "\">" + title + "</a><br><br>";

        pnlContent.Controls.Add(h2);
        pnlContent.Controls.Add(h2Published);
        pnlContent.Controls.Add(physical);
        pnlContent.Controls.Add(new HtmlGenericControl("HR"));
        pnlContent.Controls.Add(gen);
        
        // Add Class Attribute To Body Tag
        ((HtmlGenericControl)Master.FindControl("BodyTag")).Attributes.Add("class", "Links");

        // Build Title/Description/KeyWords MetaTags
        ((HtmlTitle)Master.FindControl("TitleTag")).Text = title + " - by Andrew Pallant (#ldnont)";
        ((HtmlMeta)Master.FindControl("MetaDescription")).Content = title;
        ((HtmlMeta)Master.FindControl("MetaKeywords")).Content = category + "," + title;

    }

    /// <summary>
    /// Get Value From XML Tag
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    private static String getValue(XmlNode tag)
    {
        String content = "";
        RegexOptions options = RegexOptions.None;
        Regex regex = new Regex(@"\<\!\[CDATA\[(?<text>[^\]]*)\]\]\>", options);
        string input = tag.InnerXml;

        // Check for match
        bool isMatch = regex.IsMatch(input);
        if (isMatch)
        {
            Match match = regex.Match(input);
            string HTMLtext = match.Groups["text"].Value;

            content = HTMLtext;
        }
        return content;
    }

    /// <summary>
    /// Format Date
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public String formatDate(object dt)
    {
        //2011-02-08T18:34:00.003-08:00
        String strDt = (String)dt;
        if (strDt.Length == 0) return "";
        char[] ch = {' '};
        String[] dtArray = ((String)dt).Split(ch);
        String year = dtArray[3];
        String month = dtArray[2];
        String day = dtArray[1];
        String time = dtArray[4];
        return month + " " + day + ", " + year + " " + time;
    }


    
}
