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
    <div class="listSettings" style="">
    </div>
    <asp:Panel ID="SettingsPanel" runat="server"></asp:Panel>

    <SharePoint:SPGridView ID="AdditionalParamsTable" runat="server" AutoGenerateColumns="false" Width="80%">
        <RowStyle BackColor="#f6f7f8" Height="30px" HorizontalAlign="Left" />
        <AlternatingRowStyle BackColor="White" ForeColor="#000" Height="30px" HorizontalAlign="Left" />
        <HeaderStyle Font-Bold="true" HorizontalAlign="Left" CssClass="ms-viewheadertr" />
        <HeaderStyle />
        <Columns>
            <%--<asp:BoundField DataField="Variable" HeaderText="Variable" ItemStyle-Width = "200" />--%>
            <asp:TemplateField HeaderText="Parameter" HeaderStyle-Width="100px">
                <ItemTemplate>
                    <asp:Label ID="ParameterLabel" runat="server" Text='<%# Eval("Parameter") %>' Width="100"></asp:Label>
                </ItemTemplate> 
            </asp:TemplateField> 
            <asp:TemplateField HeaderText="Value">
                <ItemTemplate>
                    <asp:TextBox ID="ValueTextBox" runat="server" Text='<%# Eval("Value") %>' Width="500"></asp:TextBox>
                </ItemTemplate> 
            </asp:TemplateField> 
        </Columns>
    </SharePoint:SPGridView>
    <br/>
    <SharePoint:SPGridView ID="FieldsTable" runat="server" AutoGenerateColumns="false" Width="80%">
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

            <asp:TemplateField HeaderText="TrackUpdating">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="false" Checked='<%# Eval("TrackUpdating").ToString()=="1" ? true : false %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="TrackAdded">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="false" Checked='<%# Eval("TrackAdded").ToString()=="1" ? true : false %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="SeparateMail">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox3" runat="server" AutoPostBack="false" Checked='<%# Eval("SeparateMail").ToString()=="1" ? true : false %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Notify">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox4" runat="server" AutoPostBack="false" Checked='<%# Eval("Notify").ToString()=="1" ? true : false %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="NotifyManagers">
                <ItemTemplate>
                    <asp:CheckBox  ID="CheckBox5" runat="server" AutoPostBack="false" Checked='<%# Eval("NotifyManagers").ToString()=="1" ? true : false %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="FixedUpdating">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox6" runat="server" AutoPostBack="false" Checked='<%# Eval("FixedUpdating").ToString()=="1" ? true : false %>'/>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </SharePoint:SPGridView>
    <asp:Button ID="ButtonOK" runat="server" Text="OK" OnClick="ButtonOK_EventHandler" AutoPostback="false"/>
    <asp:Button ID="ButtonCANCEL" runat="server" Text="Cancel" OnClick="ButtonCANCEL_EventHandler"/>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
ER Lists Settings
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
ER Lists Settings
</asp:Content>
