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
using System.Xml;
using System.Text.RegularExpressions;

public partial class new_blog : System.Web.UI.Page
{
    protected DataTable dt = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup Meta / Title Tags
        ((HtmlGenericControl)Master.FindControl("BodyTag")).Attributes.Add("class", "Blog");
        ((HtmlTitle)Master.FindControl("TitleTag")).Text = "Andrew Pallant - Software and Web Developer In London Ontario (#ldnont) - Blog Entries";
        ((HtmlMeta)Master.FindControl("MetaDescription")).Content = "Software / Web Developer Resume of Andrew Pallant - London Ontario - Blog";
        ((HtmlMeta)Master.FindControl("MetaKeywords")).Content = "Andrew, Pallant, Blog";

        if (!IsPostBack)
        {
            // Load Grid
            DefaultLoadGrid();
        }
    }

    /// <summary>
    /// Default Load Grid
    /// </summary>
    private void DefaultLoadGrid()
    {
        // Get Blog RSS
        WebRequest r = HttpWebRequest.Create("http://madprogrammer76.wordpress.com/feed/");
        WebResponse re = r.GetResponse();
        XmlDocument doc = new XmlDocument();
        using (StreamReader sr = new StreamReader(re.GetResponseStream()))
        {

            doc.LoadXml(sr.ReadToEnd());
        }

        // Build Data Table
        dt.Columns.Add("Published");
        dt.Columns.Add("Updated");
        dt.Columns.Add("Title");
        dt.Columns.Add("Content");
        dt.Columns.Add("Category");
        XmlNodeList entries = doc.DocumentElement.GetElementsByTagName("item");

        // Parse XML Nodes
        foreach (XmlNode entry in entries)
        {
            String title = "";
            String published = "";
            String updated = "";
            String content = "";
            String categegory = "";

            // Process Each Node Into Local Variables
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
                if (tag.Name.Equals("content"))
                {
                    content = getValue(tag);
                }
                if (tag.Name.Equals("category"))
                {
                    categegory += getValue(tag);
                }
            }

            // Build String Array Of Local Variables
            String[] str = new String[5];
            str[0] = published;
            str[1] = updated;
            str[2] = title;
            str[3] = content;
            str[4] = categegory;

            // Add To Database Table
            dt.Rows.Add(str);
        }

        gridView1.DataSource = dt;
        gridView1.DataBind();
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
        char[] ch = { ' ' };
        String[] dtArray = ((String)dt).Split(ch);
        String year = dtArray[3];
        String month = dtArray[2];
        String day = dtArray[1];
        String time = dtArray[4];
        return month + " " + day + ", " + year + " " + time;
    }

    /// <summary>
    /// Get URL
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string getURL(String str)
    {
        return "<a href=\"blog_" + str.Replace(" ", "_") + ".aspx\" title=\"" + str + "\">" + str + "</a>";
    }

    /// <summary>
    /// Get Date
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public string getDate(String dt)
    {
        return formatDate(dt);
    }

    /// <summary>
    /// Search For String Entered By User
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DataTable dt1 = new DataTable();
        WebRequest r = HttpWebRequest.Create("http://madprogrammer76.wordpress.com/feed/");
        WebResponse re = r.GetResponse();
        XmlDocument doc = new XmlDocument();
        using (StreamReader sr = new StreamReader(re.GetResponseStream()))
        {

            doc.LoadXml(sr.ReadToEnd());
        }

        dt.Columns.Add("Published");
        dt.Columns.Add("Updated");
        dt.Columns.Add("Title");
        dt.Columns.Add("Content");
        dt.Columns.Add("Category");

        dt1.Columns.Add("Published");
        dt1.Columns.Add("Updated");
        dt1.Columns.Add("Title");
        dt1.Columns.Add("Content");
        dt1.Columns.Add("Category");

        XmlNodeList entries = doc.DocumentElement.GetElementsByTagName("item");
        
        // Process Each Node Into Local Variables
        foreach (XmlNode entry in entries)
        {
            String title = "";
            String published = "";
            String updated = "";
            String content = "";
            String categegory = "";
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
                if (tag.Name.Equals("content"))
                {
                    content = getValue(tag);
                }
                if (tag.Name.Equals("category"))
                {
                    categegory += getValue(tag);
                }
            }

            // Build String Array Of Local Variables
            String[] str = new String[5];
            str[0] = published;
            str[1] = updated;
            str[2] = title;
            str[3] = content;
            str[4] = categegory;

            // Add To Database Table
            dt.Rows.Add(str);
        }

        // Filter Rows based on User Selections
        DataRow[] rows = dt.Select("Content like '%" + txtSearch.Text + "%' or Title like '%" + txtSearch.Text + "%' or Category like '%" + txtSearch.Text + "%'", "Published desc");

        // Build New Database Table
        foreach (DataRow row in rows)
        {
            dt1.Rows.Add(row.ItemArray);
        }

        // Bind To Data Grid
        gridView1.DataSource = dt1;
        gridView1.DataBind();
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
    /// Reset To Default View
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        DefaultLoadGrid();
    }
}
