<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoogleOAuth.aspx.cs" Inherits="Skybrud.Social.Umbraco.App_Plugins.Skybrud.Social.Dialogs.GoogleOAuth" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%=Title %></title>
    <link href="http://fonts.googleapis.com/css?family=Open+Sans:400,700" rel="stylesheet" type="text/css">
    <style>
        body {
            margin: 0;
            font-family: 'Open Sans', 'Helvetica Neue', Helvetica, Arial, sans-serif;
            font-size: 12px;
        }
        .umb-panel-header {
            height: 99px;
            background: #f8f8f8;
            border-bottom: 1px solid #d9d9d9;
            padding: 0 20px;
            line-height: 99px;
        }
        h1 {
            margin: 0;
            line-height: 99px;
            font-size: 18px;
            font-weight: normal;
        }
        .content {
            padding: 20px;
        }
        .selectItems {
            padding: 0;
            margin: 15px 0 0;
            border: solid 1px;
            border-bottom: 0;
        }
        .selectItems li {
            position: relative;
            padding: 10px 15px;
            border-bottom: solid 1px;
            cursor: pointer;
            background: #bbb;
        }
        .selectItems li:hover {
            background: #eee;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="umb-panel-header">
            <h1><%=Title %></h1>
        </div>
        <div class="content">
            <asp:Literal runat="server" ID="Content" />
            <ul class="selectItems">            
            <asp:Repeater runat="server" ID="rptSelectItems" EnableViewState="false">
                <ItemTemplate>
                <li onclick="onSelectItem('<%#Eval("Url") %>', this)">
                    <%#Eval("Name") %><br />
                    <i><small><%# Eval("Address.AddressLines[0]") %></small></i>
                </li>
                </ItemTemplate>
            </asp:Repeater>
            </ul>
        </div>
    </div>
    </form>
    <script type="text/javascript">
        function onSelectItem(itemId, liElement) {
            if (typeof oautData !== 'undefined') {
                oautData.locationUrl = itemId;
                oautData.locationName = liElement.innerText;
                self.opener.<%= Callback %>(oautData);
                window.close();
            }
        }
    </script>
</body>
</html>
