<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="ListsNotifications.Layouts.ERListsSettings.NotificationsSettingsPage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <script type="text/javascript" src="<%= Page.ResolveUrl("Notifications.js") %>"></script>
    <script type="text/javascript">
        window.onload = disableTextBoxes;
    </script>

    <SharePoint:SPGridView ID="AdditionalParamsTable" runat="server" AutoGenerateColumns="false">
        <RowStyle BackColor="#f6f7f8" Height="30px" HorizontalAlign="Left" />
        <AlternatingRowStyle BackColor="White" ForeColor="#000" Height="30px" HorizontalAlign="Left" />
        <HeaderStyle Font-Bold="true" HorizontalAlign="Left" CssClass="ms-viewheadertr" />
        <HeaderStyle />
        <Columns>
            <asp:TemplateField HeaderText="Parameter" HeaderStyle-Width="100px">
                <ItemTemplate>
                    <asp:Label ID="ParameterLabel" runat="server" Text='<%# Eval("Parameter") %>' Width="100"></asp:Label>
                </ItemTemplate> 
            </asp:TemplateField> 
            <asp:TemplateField HeaderText="Value">
                <ItemTemplate>
                    <asp:TextBox ID="ValueTextBox" runat="server" Text='<%# Eval("Value") %>' Width="500" Visible='<%# Eval("LinkVisible").ToString() != "True" %>'></asp:TextBox>
                    <asp:HyperLink ID="MailTemplatesList" runat="server" Text='<%# Eval("Value") %>' 
                        Visible='<%# Eval("LinkVisible") %>' NavigateUrl='<%# Eval("LinkValue") %>'/>
                </ItemTemplate> 
            </asp:TemplateField> 
        </Columns>
    </SharePoint:SPGridView>
    <br/>
    <SharePoint:SPGridView ID="FieldsTable" runat="server" AutoGenerateColumns="false">
        <RowStyle BackColor="#f6f7f8" Height="30px" HorizontalAlign="Left" />
        <AlternatingRowStyle BackColor="White" ForeColor="#000" Height="30px" HorizontalAlign="Left" />
        <HeaderStyle Font-Bold="true" HorizontalAlign="Left" CssClass="ms-viewheadertr" />
        <HeaderStyle />
        <Columns>
            <asp:TemplateField HeaderText="Field" HeaderStyle-Width="250px">
                <ItemTemplate>
                    <asp:Label ID="FieldLabel" runat="server" Text='<%# Eval("FieldName") %>'></asp:Label>
                </ItemTemplate> 
            </asp:TemplateField> 
            
            <%--IDs of elements should be same as props in ERConfNotifications class--%>
            <asp:TemplateField HeaderText="TrackUpdating" ControlStyle-Width="100">
                <ItemTemplate>
                    <asp:CheckBox ID="ItemUpdatingTrackFields" runat="server" AutoPostBack="false" Checked='<%# Eval("ItemUpdatingTrackFields") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="TrackAdded" ControlStyle-Width="100">
                <ItemTemplate>
                    <asp:CheckBox ID="ItemAddedTrackFields" runat="server" AutoPostBack="false" Checked='<%# Eval("ItemAddedTrackFields") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="SeparateMail" ControlStyle-Width="50">
                <ItemTemplate>
                    <asp:CheckBox ID="TrackFieldsSingleMail" runat="server" OnCheckedChanged="TrackSingleMail_EventHandler" 
                        Checked='<%# Eval("TrackFieldsSingleMail") %>' AutoPostBack="true"/>
                    <asp:HyperLink ID="MailTemplatesUrl" runat="server" 
                        Text="Configuration" NavigateUrl='<%# Eval("MailTemplatesUrl") %>'
                        Visible='<%# Eval("TrackFieldsSingleMail") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Notify" ControlStyle-Width="100">
                <ItemTemplate>
                    <asp:CheckBox ID="to" runat="server" AutoPostBack="false" Checked='<%# Eval("to") %>' Visible='<%# Eval("UserField") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="NotifyManagers" ControlStyle-Width="100">
                <ItemTemplate>
                    <asp:CheckBox  ID="toManagers" runat="server" AutoPostBack="false" Checked='<%# Eval("toManagers") %>' Visible='<%# Eval("UserField") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="FixedUpdating" ControlStyle-Width="100">
                <ItemTemplate>
                    <asp:CheckBox ID="ItemUpdatingFixedFields" runat="server" AutoPostBack="false" Checked='<%# Eval("ItemUpdatingFixedFields") %>'/>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </SharePoint:SPGridView>
    <asp:Button ID="ButtonOK" runat="server" Text="OK" OnClick="ButtonOK_EventHandler"/>
    <asp:Button ID="ButtonCANCEL" runat="server" Text="Cancel" OnClick="ButtonCANCEL_EventHandler"/>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
ER Lists Settings
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
ER Lists Settings
</asp:Content>
