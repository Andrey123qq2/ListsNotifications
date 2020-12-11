<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MailTemplates.aspx.cs" Inherits="ListsNotifications.Layouts.ERListsSettings.MailTemplatesPage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <script type="text/javascript" src="<%= Page.ResolveUrl("MailTemplates.js") %>"></script>
    <script type="text/javascript">
        window.onload = linksForMailTemplates;
    </script>
    <asp:Label ID="FieldNameLabel" runat="server" Text="Mail Templates for {0}"></asp:Label>
    <br /><br />

    <SharePoint:SPGridView ID="MailVariablesTable" runat="server" AutoGenerateColumns="false" Width="80%">
        <RowStyle BackColor="#f6f7f8" Height="30px" HorizontalAlign="Left" />
        <AlternatingRowStyle BackColor="White" ForeColor="#000" Height="30px" HorizontalAlign="Left" />
        <HeaderStyle Font-Bold="true" HorizontalAlign="Left" CssClass="ms-viewheadertr" />
        <HeaderStyle />
        <Columns>
            <%--<asp:BoundField DataField="Variable" HeaderText="Variable" ItemStyle-Width = "200" />--%>
            <asp:TemplateField HeaderText="Variable" HeaderStyle-Width="250px">
                <ItemTemplate>
                    <asp:Label ID="TextBoxLabel" runat="server" Text='<%# Eval("Variable") %>' Width="100"></asp:Label>
                </ItemTemplate> 
            </asp:TemplateField> 
            <asp:TemplateField HeaderText="Value">
                <ItemTemplate>
                    <asp:TextBox ID="TextBox" runat="server" Text='<%# Eval("Value") %>' Visible="true" Width="400" 
                        TextMode='<%# Eval("TextMode") %>' Height='<%# Eval("Value").ToString().Length > 100 ? 500 : 17 %>'></asp:TextBox>
                </ItemTemplate> 
            </asp:TemplateField> 
        </Columns>
    </SharePoint:SPGridView>

    <div>Variables {N} in MAIL_BODY_TEMPLATE :></div>
    <div>0 - MAIL_FIELDS_TEMPLATE_* (where 0 - field name, 1 - previous value, 2 - new value)></div>
    <div>1 - MAIL_URL_TEMPLATE (where 0 - url, 1 - title)></div>
    <div>2 - MAIL_*_BY_TEMPLATE (where 0 - user display name)></div>
    <br />
    <asp:Button ID="ButtonOK" runat="server" Text="OK" OnClick="ButtonOK_EventHandler" AutoPostback="false"/>
    <asp:Button ID="ButtonCANCEL" runat="server" Text="Cancel" OnClick="ButtonCANCEL_EventHandler"/>
</asp:Content>


<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Mail Templates
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Mail Templates
</asp:Content>
