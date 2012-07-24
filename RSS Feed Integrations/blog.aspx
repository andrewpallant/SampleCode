<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="blog.aspx.cs" Inherits="new_blog" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="grid_12">
    <div class="grid_5 alpha" style="padding-left:10px;padding-top:10px;">
        <h3 class="title">Software Developer In London</h3>
    </div>
    <div class="grid_12">&nbsp;
    </div>
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div class="grid_3 alpha">
                    &nbsp;&nbsp;&nbsp;Search Postings   <asp:TextBox runat="server" Width="150px" ID="txtSearch" style="margin-right:5px;margin-left:5px;"></asp:TextBox><asp:Button Text="Search" style="padding:1px;padding-right:3px;padding-left:3px;" runat="server" ID="btnSearch" OnClick="btnSearch_Click" /><asp:Button Text="Reset" style="padding:1px;padding-right:3px;padding-left:3px;" runat="server" ID="btnReset" OnClick="btnReset_Click" />
                </div>
                <div class="grid_9 omega">
                    &nbsp;&nbsp;&nbsp;Subscribe to Posts [<a href="http://madprogrammer76.wordpress.com/feed/">Atom</a>]
                </div>
                <div class="grid_12 alpha" style="padding:5px;">
                    <asp:GridView runat="server" ID="gridView1" AutoGenerateColumns="false" CellSpacing="10" CellPadding="4" ShowHeader="true" Width="100%" BorderStyle="None">
                        <AlternatingRowStyle backcolor="#232323" />
                        <HeaderStyle BackColor="Black" ForeColor="white" Font-Bold="true" BorderStyle="Outset" BorderColor="whiteSmoke" />
                        <RowStyle BackColor="#2b2b2b" />
                        <Columns>
                            <asp:TemplateField ItemStyle-BorderStyle="None" HeaderText="Title">
                                <ItemTemplate>
                                    <li style="padding:4px; font-size: 1em;">
                                        <%# getURL(Eval("title").ToString())%>
                                    </li>
                                    <!--Key Words: &nbsp;&nbsp;<%# Eval("Category")%><br /><br /-->
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField ItemStyle-BorderStyle="None" HeaderText="Published">
                                <ItemTemplate>
                                    <%# getDate(Eval("published").ToString()) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField ItemStyle-BorderStyle="None" ItemStyle-Wrap="true" ItemStyle-Width="400" HeaderText="Keywords">
                                <ItemTemplate> 
                                    <%# Eval("Category").ToString()%>
                                </ItemTemplate>    
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>            
            </ContentTemplate>
        </asp:UpdatePanel>
    
</div>
</asp:Content>

