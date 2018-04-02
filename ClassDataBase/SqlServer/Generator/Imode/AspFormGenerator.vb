Imports System.Management
Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml


Namespace SqlServer.IMode

    Public Class AspFormGenerator

        Public Shared Sub CreateInterfaceCodeBehind(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_LibraryName As TextBox)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomWebform & ".aspx.vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)
            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")
            header &= ""
            Dim content As String = "Partial Class " & nomWebform & Chr(13) _
                                     & " Inherits BasePage"

            _end = "End Class" & Chr(13)
            ' Delete the file if it exists.
            If File.Exists(path) Then
                File.Delete(path)
            End If

            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If
            ' Create the file.
            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()



            Dim objWriter As New System.IO.StreamWriter(path, True)
            'objWriter.WriteLine(header)
            If ListBox_NameSpace.Items.Count > 0 Then
                For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                    objWriter.WriteLine(ListBox_NameSpace.Items(i))
                Next
            End If
            Dim libraryname As String = "Imports " & txt_LibraryName.Text
            objWriter.WriteLine(libraryname)
            'objWriter.WriteLine("Imports System.Data")
            'objWriter.WriteLine("Imports SolutionsHT")
            'objWriter.WriteLine("Imports SolutionsHT.DataAccessLayer")
            objWriter.WriteLine()

            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            objWriter.WriteLine(content)
            objWriter.WriteLine()


            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = _table.ListofColumn.Count


            Id_table = _table.ListofColumn.Item(0).Name

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                ListofIndex.Insert(countindex, _index.ListofColumn.Item(0).Name)
                countindex = countindex + 1
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}


            For Each _foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreignkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next


            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next


            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")


            objWriter.WriteLine("Dim _out As Boolean = False")
            objWriter.WriteLine("Dim _tmpEditState As Boolean = False")

            objWriter.WriteLine()

            objWriter.WriteLine(" Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load")
            objWriter.WriteLine("If txt_Code" & nomSimple & "_Hid.Text <> ""0"" Then " & Chr(13) _
                                & Chr(9) & Chr(9) & "_tmpEditState = True" & Chr(13) _
                                & "Else" & Chr(13) _
                                & Chr(9) & Chr(9) & "_tmpEditState = False" & Chr(13) _
                                & "End If")

            objWriter.WriteLine("   If _message = """" Then" & Chr(13) _
                              & "  lbl_Error.Text = """" " & Chr(13) _
                              & Chr(13) _
                              & "If Not IsPostBack Then")

            objWriter.WriteLine(" txt_Code" & nomSimple & "_Hid.Text = CStr(TypeSafeConversion.NullSafeLong(Request.QueryString(""id""), 0)) ")
            objWriter.WriteLine("If txt_Code" & nomSimple & "_Hid.Text <> ""0"" Then " & Chr(13) _
                              & Chr(9) & Chr(9) & "_tmpEditState = True" & Chr(13) _
                              & "Else" & Chr(13) _
                              & Chr(9) & Chr(9) & "_tmpEditState = False" & Chr(13) _
                              & "End If")
            objWriter.WriteLine("  btnCancel.Attributes.Add(""onclick"", ""javascript:void(closeWindow());"")" & Chr(13) _
                                & "Setup()" & Chr(13) _
                                & "End If")

            objWriter.WriteLine(" Else " & Chr(13) _
                                & "MessageToShow(_message)" & Chr(13) _
                                & "pn_Infos" & nomSimple & ".Enabled = False" & Chr(13) _
                                & "End If")
            objWriter.WriteLine("End Sub")

            objWriter.WriteLine()

            objWriter.WriteLine(" Private Sub EnableEditInfos" & nomSimple & "(ByVal modify As Boolean)")

            objWriter.WriteLine("If modify Then " & Chr(13) _
                                & "btnSave.Text = ""Sauvegarder""" & Chr(13) _
                                & "Else" & Chr(13) _
                                & "btnSave.Text = ""Editer""" & Chr(13) _
                                & "End If" & Chr(13) _
                                & "btnSave.CausesValidation = modify")

            For i As Int32 = 1 To cols.Count - 3
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " .Enabled =   modify ")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    objWriter.WriteLine("rdp" & cols(i) & ".Enabled =   modify ")
                ElseIf types(i) = "Boolean" And cols(i) <> "isdirty" Then
                    'objWriter.WriteLine(" radio_yes" & cols(i) & ".Enabled =   modify ")
                    'objWriter.WriteLine("radio_no" & cols(i) & ".Enabled =   modify ")
                    objWriter.WriteLine("chk" & cols(i) & ".Enabled =   modify ")
                Else
                    objWriter.WriteLine("rtxt" & cols(i) & ".Enabled =   modify ")
                End If
            Next

            objWriter.WriteLine("End Sub")

            objWriter.WriteLine()

            objWriter.WriteLine("Private Sub Setup()")


            For Each foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                Dim attributUsed As String = foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                Dim ClassName As String = "Cls_" & foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                objWriter.WriteLine("FillCombo" & attributUsed & "()")
            Next

            objWriter.WriteLine("  If _tmpEditState Then" & Chr(13) _
                                & "Dim obj as New " & nomClasse & "( CLng(txt_Code" & nomSimple & "_Hid.Text))" & Chr(13) _
                                & "With obj")

            For i As Int32 = 1 To cols.Count - 3
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " .SelectedIndex =  rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & ".FindItemIndexByValue(." & cols(i).Substring(1, cols(i).Length - 1) & ")")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    objWriter.WriteLine("rdp" & cols(i) & ".SelectedDate = ." & cols(i).Substring(1, cols(i).Length - 1))
                ElseIf types(i) = "Boolean" Then
                    'objWriter.WriteLine("  If ." & cols(i).Substring(1, cols(i).Length - 1) & " = True Then")
                    'objWriter.WriteLine(" radio_yes" & cols(i) & ".Checked = True ")
                    'objWriter.WriteLine("Else")
                    'objWriter.WriteLine("radio_no" & cols(i) & ".Checked = True")
                    'objWriter.WriteLine("End If")
                    objWriter.WriteLine("chk" & cols(i) & ".Checked = ." & cols(i).Substring(1, cols(i).Length - 1))
                Else
                    objWriter.WriteLine("rtxt" & cols(i) & ".Text = ." & cols(i).Substring(1, cols(i).Length - 1))
                End If
            Next
            objWriter.WriteLine("End With")
            objWriter.WriteLine("Else")
            For i As Int32 = 1 To cols.Count - 3
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " .SelectedIndex = 0")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    objWriter.WriteLine("rdp" & cols(i) & ".SelectedDate = Now")
                ElseIf (types(i) = "Boolean") Then
                    '  objWriter.WriteLine("radio_no" & cols(i) & ".Checked = True")
                    'objWriter.WriteLine("radio_no" & cols(i) & ".Checked = False")
                    objWriter.WriteLine("chk" & cols(i) & ".Checked = False")
                Else
                    objWriter.WriteLine("rtxt" & cols(i) & ".Text = """"")
                End If
            Next
            objWriter.WriteLine("End If")
            objWriter.WriteLine("End Sub")
            objWriter.WriteLine()
            objWriter.WriteLine("Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click" & Chr(13) _
                                & "Select Case btnSave.Text" & Chr(13) _
                                & " Case ""Editer""" & Chr(13) _
                                & "EnableEditInfos" & nomSimple & "(True)" & Chr(13) _
                                & "Case ""Sauvegarder""" & Chr(13) _
            & "Dim _id As Long = CLng(txt_Code" & nomSimple & "_Hid.Text)" & Chr(13) _
            & "Dim _obj As New " & nomClasse & "(_id)" & Chr(13) _
            & "Try" & Chr(13) _
            & "With _obj")

            For i As Int32 = 1 To cols.Count - 3
                Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("." & columnToUse & "  =   rcmb_" & columnToUse.Substring(SqlServerHelper.ForeinKeyPrefix.Length, columnToUse.Length - (SqlServerHelper.ForeinKeyPrefix.Length)) & " .SelectedValue ")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    objWriter.WriteLine("." & columnToUse & " = rdp_" & columnToUse & " .SelectedDate ")
                ElseIf types(i) = "Boolean" Then
                    ' objWriter.WriteLine("." & columnToUse & " =  radio_yes_" & columnToUse & " .Checked")
                    objWriter.WriteLine("." & columnToUse & " =  chk_" & columnToUse & " .Checked")
                Else
                    objWriter.WriteLine("." & columnToUse & " = rtxt_" & columnToUse & ".Text ")
                End If
            Next

            objWriter.WriteLine("End With")

            objWriter.WriteLine("_obj.Save(""Admin"")")

            objWriter.WriteLine("_message = ""Sauvegarde Effectuée""")
            objWriter.WriteLine("MessageToShow(_message, ""S"")")
            objWriter.WriteLine("RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe" & nomSimple & "();"")")
            objWriter.WriteLine(" Catch ex As Exception")
            objWriter.WriteLine(" _message = ex.Message & Chr(13) & ""La sauvegarde a échouée!""")
            objWriter.WriteLine("MessageToShow(_message)")
            objWriter.WriteLine("End Try")
            objWriter.WriteLine("  End Select" & Chr(13) & _
                                "End Sub")
            objWriter.WriteLine()


            objWriter.WriteLine("Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"")")
            objWriter.WriteLine("   Panel_Msg.Visible = True")
            objWriter.WriteLine(" GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
            objWriter.WriteLine(" Label_Msg.Text = _message")
            objWriter.WriteLine(" If E_or_S = ""S"" Then")
            objWriter.WriteLine(" Label_Msg.ForeColor = Drawing.Color.Green")
            objWriter.WriteLine(" Else")
            objWriter.WriteLine(" Label_Msg.ForeColor = Drawing.Color.Red")
            objWriter.WriteLine(" End If")
            objWriter.WriteLine(" RadAjaxManager1.ResponseScripts.Add(""alert('"" & _message.Replace(""'"", ""\'"") & ""');"")")
            objWriter.WriteLine(" End Sub")
            objWriter.WriteLine()
            For Each foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                Dim textForcombo As String = ""
                Dim attributUsed As String = foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))

                Dim ClassName As String = "Cls_" & foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                ClassName = foreignkey.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
                Dim nomforeign As String = foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                objWriter.WriteLine("Private Sub FillCombo" & attributUsed & "()")
                objWriter.WriteLine("Try")
                objWriter.WriteLine("Dim objs1 As List(of " & ClassName & ") = " & ClassName & ".SearchAll ")

                For Each column As Cls_Column In foreignkey.RefTable.ListofColumn
                    If column.Type.VbName = "String" Then
                        textForcombo = column.Name
                        Exit For
                    End If
                Next

                objWriter.WriteLine("With rcmb_" & nomforeign)
                objWriter.WriteLine(".Datasource = objs1" & Chr(13) _
                                    & ".DataValueField = ""ID""" & Chr(13) _
                                    & ".DataTextField = """ & textForcombo & """" & Chr(13) _
                                    & ".DataBind()" & Chr(13) _
                                    & ".SelectedIndex = 0" & Chr(13) _
                                    & "End With")
                objWriter.WriteLine("Catch ex As Exception" & Chr(13) _
                                    & "_message = ex.Message" & Chr(13) _
                                    & "MessageToShow(_message)")

                objWriter.WriteLine("End Try")
                objWriter.WriteLine("End Sub")
                objWriter.WriteLine()
            Next

            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterfaceCodeAsp(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & ".aspx"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            If File.Exists(path) Then
                File.Delete(path)
            End If
            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If

            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()
            Dim objWriter As New System.IO.StreamWriter(path, True)

            objWriter.WriteLine()

            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer
            cap = _table.ListofColumn.Count
            Id_table = _table.ListofColumn.Item(0).Name
            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}
            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")

            objWriter.WriteLine("<%@ Page Title="""" Language=""VB"" MasterPageFile=""~/Pages/MasterPage/MasterPagePopup.master""" & _
           "AutoEventWireup=""false"" CodeFile=""wbfrm_" & nomSimple & ".aspx.vb"" Inherits=""wbfrm_" & nomSimple & """ %>")

            objWriter.WriteLine("<asp:Content ID=""Content3"" ContentPlaceHolderID=""SheetContentPlaceHolder"" runat=""Server"">")

            objWriter.WriteLine("<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("<script type=""text/javascript"">")
            objWriter.WriteLine("function closeWindow() {")
            objWriter.WriteLine(" GetRadWindow().BrowserWindow.refreshMe();")
            objWriter.WriteLine("  GetRadWindow().close();")
            objWriter.WriteLine("}")



            objWriter.WriteLine("function CloseAndRefreshListe" & nomSimple & "() {")
            objWriter.WriteLine(" GetRadWindow().BrowserWindow.refreshMe();")
            objWriter.WriteLine("  GetRadWindow().close();")
            objWriter.WriteLine(" }")

            objWriter.WriteLine(" function GetRadWindow() {")
            objWriter.WriteLine("  var oWindow = null;")
            objWriter.WriteLine("  if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog")
            objWriter.WriteLine("  else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well)")

            objWriter.WriteLine("   return oWindow;")
            objWriter.WriteLine("  }")


            objWriter.WriteLine("</script>")
            objWriter.WriteLine("</telerik:RadCodeBlock>")

            objWriter.WriteLine(" <telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server"">")
            objWriter.WriteLine("<Scripts>")
            objWriter.WriteLine("</Scripts>")
            objWriter.WriteLine("</telerik:RadScriptManager>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("<AjaxSettings>")
            objWriter.WriteLine("<telerik:AjaxSetting AjaxControlID=""btnSave"">")
            objWriter.WriteLine("<UpdatedControls>")
            objWriter.WriteLine(" <telerik:AjaxUpdatedControl ControlID=""Panel_Msg"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine(" <telerik:AjaxUpdatedControl ControlID=""pn_Infos" & nomSimple & """ LoadingPanelID=""RadAjaxLoadingPanel1"" />")

            objWriter.WriteLine("</UpdatedControls>")
            objWriter.WriteLine("</telerik:AjaxSetting>")
            objWriter.WriteLine("</AjaxSettings>")
            objWriter.WriteLine("</telerik:RadAjaxManager>")

            objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7"">")
            objWriter.WriteLine("</telerik:RadSkinManager>")

            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")
            objWriter.WriteLine("<center>")
            objWriter.WriteLine(" <asp:Panel runat=""server"" ID=""pnCompletePage"">")
            objWriter.WriteLine(" <table id=""global"" width=""800px"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #F7F7F7;  border: 3px solid #66A6CC"">")
            objWriter.WriteLine(" <tr valign=""top"">")
            objWriter.WriteLine(" <td valign=""top"" style=""width: 100%; height: 100%;"">")
            objWriter.WriteLine(" <table cellspacing=""0"" cellpadding=""0"" style=""width: 100%; height: 100%;"">")
            objWriter.WriteLine(" <tr valign=""top"" style=""background-color: #FFFFFF; height: 100px; width: 100%; border-bottom-color: #66A6CC;border-bottom-style : solid"">")
            objWriter.WriteLine(" <td align=""left"" style=""width: 10%"">")
            objWriter.WriteLine(" <asp:Image ID=""imgHeader"" runat=""server"" ImageUrl=""~/images/img_default.png"" />")
            objWriter.WriteLine("</td>")
            objWriter.WriteLine(" <td align=""left"" valign=""top"" style=""width: 90%"">")
            objWriter.WriteLine(" <h1 Class=""popupFormTitleLabel"" >")
            objWriter.WriteLine(" <asp:Label ID=""lbl_title"" CssClass=""popupFormTitleLabel""   Text=""Gestion des " & nomSimple & """   runat=""server""></asp:Label>")
            objWriter.WriteLine("</h1>")
            objWriter.WriteLine("</td>")
            objWriter.WriteLine("</tr>")

            objWriter.WriteLine("<tr valign=""top"">")
            objWriter.WriteLine("<td style=""width: 100%;"" colspan=""2"" valign=""top"">")
            objWriter.WriteLine("<hr class=""hrHeaderAfterPhoto"" />")
            objWriter.WriteLine("</td>")
            objWriter.WriteLine("</tr>")

            objWriter.WriteLine("<tr valign=""top"">")
            objWriter.WriteLine("<td style=""width: 100%;""  align=""center"" colspan=""2"" valign=""top"">")
            objWriter.WriteLine("<asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
            objWriter.WriteLine("<div class=""Classe_MsgErreur"">")
            objWriter.WriteLine(" <asp:Image ID=""Image_Msg"" runat=""server"" Style=""vertical-align: middle;"" />")
            objWriter.WriteLine(" <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
            objWriter.WriteLine(" </div>")
            objWriter.WriteLine("</asp:Panel>")
            objWriter.WriteLine("</td>")
            objWriter.WriteLine("</tr>")

            objWriter.WriteLine(" <tr valign=""top"">")
            objWriter.WriteLine(" <td valign=""top"" align=""center"" style=""width: 100%;"" colspan=""2"">")
            objWriter.WriteLine(" <asp:Label ID=""lbl_Error"" runat=""server"" CssClass=""LabelBold"" Font-Bold=""True"" Font-Size=""Small"" ForeColor=""Red"" Font-Italic=""True""></asp:Label> ")


            objWriter.WriteLine(" </td>")
            objWriter.WriteLine("  </tr>")
            objWriter.WriteLine("<tr valign=""top"">")
            objWriter.WriteLine("<td valign=""top"" style=""width: 100%;"" colspan=""2"">")
            objWriter.WriteLine(" <asp:Panel ID=""pn_Infos" & nomSimple & """ runat=""server"">")



            objWriter.WriteLine("  <asp:Panel ID=""pn_entete"" runat=""server"" Width=""100%"">")
            objWriter.WriteLine("<table border=""0"" class=""form_Enregistrement"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")

            Dim countColumn As Integer = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then

                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ CssClass=""popupFormInputLabel"" Text=""" & columnName & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadComboBox ID=""rcmb_" & columnName & """  Filter=""Contains""  AutoPostBack=""true""  Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadComboBox> ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & columnName & """ runat=""server"" ControlToValidate=""rcmb_" & columnName & """  ")
                        objWriter.WriteLine(" InitialValue=""--Choisir un " & columnName & "--"" ErrorMessage=""Le " & columnName & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("</tr>")
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType <> "bit" Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadTextBox ID=""rtxt_" & column.Name & """ Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadTextBox> ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & column.Name & """ runat=""server"" ControlToValidate=""rtxt_" & column.Name & """  ")
                        objWriter.WriteLine("  ErrorMessage=""Le " & column.Name & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("</tr>")
                    ElseIf (column.TrueSqlServerType = "date" Or column.TrueSqlServerType = "datetime") And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadDatePicker ID=""rdp_" & column.Name & """ Width=""50%"" runat=""server""" & _
                                         "DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01""  ToolTip=""Cliquer sur le bouton pour choisir une date"" > " & Chr(13) & " </telerik:RadDatePicker>  ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & column.Name & """ runat=""server"" ControlToValidate=""rdp_" & column.Name & """  ")
                        objWriter.WriteLine("  ErrorMessage=""La " & column.Name & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("</tr>")

                    ElseIf column.TrueSqlServerType = "bit" Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<asp:CheckBox ID=""chk_" & column.Name & """ Width=""50%"" runat=""server"">" & _
                                          " </asp:CheckBox>  ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("</tr>")
                    End If
                End If
                countColumn = countColumn + 1

            Next

            objWriter.WriteLine("<tr>")
            objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
            objWriter.WriteLine("</td>")
            objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
            objWriter.WriteLine("<telerik:RadButton ID=""btnSave"" runat=""server"" Skin=""Windows7""  Text=""Sauvegarder"" ValidationGroup=""" & nomSimple & "Group""> ")
            objWriter.WriteLine("<Icon PrimaryIconCssClass=""rbSave"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
            objWriter.WriteLine("</telerik:RadButton>")
            objWriter.WriteLine("&nbsp;")
            objWriter.WriteLine("<telerik:RadButton ID=""btnCancel"" runat=""server"" CausesValidation=""False"" Skin=""Windows7""  Text = ""Abandonner"" >")
            objWriter.WriteLine("<Icon PrimaryIconCssClass=""rbCancel"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
            objWriter.WriteLine("</telerik:RadButton>")

            objWriter.WriteLine("</td>")

            objWriter.WriteLine("</tr>")
            objWriter.WriteLine(" </table>")
            objWriter.WriteLine("</asp:Panel>")
            objWriter.WriteLine(" </asp:Panel>")
            objWriter.WriteLine("  </td>")
            objWriter.WriteLine("</tr>")
            objWriter.WriteLine("</table>")
            objWriter.WriteLine("</td>")
            objWriter.WriteLine("</tr>")
            objWriter.WriteLine("</table>")
            objWriter.WriteLine("</asp:Panel>")

            objWriter.WriteLine("<br />")
            objWriter.WriteLine("</center>")
            objWriter.WriteLine("<asp:TextBox ID=""txt_Code" & nomSimple & "_Hid"" runat=""server"" Text=""0"" Visible=""False"" Width=""1px""></asp:TextBox>")
            objWriter.WriteLine("<asp:ValidationSummary ID=""ValidationSummary1"" runat=""server"" ShowMessageBox=""true""" & _
            " ShowSummary=""false"" DisplayMode=""list"" ValidationGroup=""" & nomSimple & "Group"" />")
            objWriter.WriteLine("</asp:Content>")
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterfaceCodeAsp_DynamicControl(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & ".aspx"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            If File.Exists(path) Then
                File.Delete(path)
            End If
            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If

            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()
            Dim objWriter As New System.IO.StreamWriter(path, True)

            objWriter.WriteLine()

            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer
            cap = _table.ListofColumn.Count
            Id_table = _table.ListofColumn.Item(0).Name
            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}
            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")

            objWriter.WriteLine("<%@ Page Title="""" Language=""VB"" MasterPageFile=""~/Pages/MasterPage/MasterPagePopup.master""" & _
           "AutoEventWireup=""false"" CodeFile=""wbfrm_" & nomSimple & ".aspx.vb"" Inherits=""wbfrm_" & nomSimple & """ %>")

            objWriter.WriteLine("<asp:Content ID=""Content3"" ContentPlaceHolderID=""SheetContentPlaceHolder"" runat=""Server"">")

            objWriter.WriteLine("<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("<script type=""text/javascript"">")
            objWriter.WriteLine("function closeWindow() {")
            objWriter.WriteLine(" GetRadWindow().BrowserWindow.refreshMe();")
            objWriter.WriteLine("  GetRadWindow().close();")
            objWriter.WriteLine("}")



            objWriter.WriteLine("function CloseAndRefreshListe" & nomSimple & "() {")
            objWriter.WriteLine(" GetRadWindow().BrowserWindow.refreshMe();")
            objWriter.WriteLine("  GetRadWindow().close();")
            objWriter.WriteLine(" }")

            objWriter.WriteLine(" function GetRadWindow() {")
            objWriter.WriteLine("  var oWindow = null;")
            objWriter.WriteLine("  if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog")
            objWriter.WriteLine("  else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well)")

            objWriter.WriteLine("   return oWindow;")
            objWriter.WriteLine("  }")


            objWriter.WriteLine("</script>")
            objWriter.WriteLine("</telerik:RadCodeBlock>")

            objWriter.WriteLine(" <telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server"">")
            objWriter.WriteLine("<Scripts>")
            objWriter.WriteLine("</Scripts>")
            objWriter.WriteLine("</telerik:RadScriptManager>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("<AjaxSettings>")
            objWriter.WriteLine("<telerik:AjaxSetting AjaxControlID=""btnSave"">")
            objWriter.WriteLine("<UpdatedControls>")
            objWriter.WriteLine(" <telerik:AjaxUpdatedControl ControlID=""Panel_Msg"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine(" <telerik:AjaxUpdatedControl ControlID=""pn_Infos" & nomSimple & """ LoadingPanelID=""RadAjaxLoadingPanel1"" />")

            objWriter.WriteLine("</UpdatedControls>")
            objWriter.WriteLine("</telerik:AjaxSetting>")
            objWriter.WriteLine("</AjaxSettings>")
            objWriter.WriteLine("</telerik:RadAjaxManager>")

            objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7"">")
            objWriter.WriteLine("</telerik:RadSkinManager>")

            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")
            objWriter.WriteLine("<center>")
            objWriter.WriteLine("<div id=""divFormContainer"" class=""divFormContainer"">")
            objWriter.WriteLine("     <div  class=""divHeaderFormPrime"">")
            objWriter.WriteLine("<asp:Label runat=""server"" ID=""lbl_Title"" Text=""Nouvel Achat""  Font-Size=""18"" ></asp:Label>")
            objWriter.WriteLine("</div>")
            objWriter.WriteLine("<div runat=""server"" id=""pnCompletePage"" class=""divFormContent"">")
            objWriter.WriteLine("<asp:Panel ID=""pn_InfosAchat"" runat=""server"">")
            objWriter.WriteLine("<table border=""0"" class=""form_Enregistrement"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")
            objWriter.WriteLine("<tr valign=""top"">")
            objWriter.WriteLine("<td style=""width: 100%;"" align=""center"" colspan=""2"" valign=""top"">")
            objWriter.WriteLine("<asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
            objWriter.WriteLine("<div class=""Classe_MsgErreur"">")
            objWriter.WriteLine("<asp:Image ID=""Image_Msg"" runat=""server"" Style=""vertical-align: middle;"" />")
            objWriter.WriteLine("<asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
            objWriter.WriteLine("</div>")
            objWriter.WriteLine("</asp:Panel>")
            objWriter.WriteLine("</td>")
            objWriter.WriteLine("</tr>")
            objWriter.WriteLine("<tr valign=""top"">")
            objWriter.WriteLine("<td style=""width: 100%"" colspan=""4"">")
            objWriter.WriteLine("  <asp:Panel ID=""pn_entete"" runat=""server"" Width=""100%"">")
            objWriter.WriteLine("<table border=""0"" class=""form_Enregistrement"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")

            Dim countColumn As Integer = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then

                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ CssClass=""popupFormInputLabel"" Text=""" & columnName & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadComboBox ID=""rcmb_" & columnName & """  Filter=""Contains""  AutoPostBack=""true""  Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadComboBox> ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & columnName & """ runat=""server"" ControlToValidate=""rcmb_" & columnName & """  ")
                        objWriter.WriteLine(" InitialValue=""--Choisir un " & columnName & "--"" ErrorMessage=""Le " & columnName & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("</tr>")
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType <> "bit" Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadTextBox ID=""rtxt_" & column.Name & """ Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadTextBox> ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & column.Name & """ runat=""server"" ControlToValidate=""rtxt_" & column.Name & """  ")
                        objWriter.WriteLine("  ErrorMessage=""Le " & column.Name & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("</tr>")
                    ElseIf (column.TrueSqlServerType = "date" Or column.TrueSqlServerType = "datetime") And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadDatePicker ID=""rdp_" & column.Name & """ Width=""50%"" runat=""server""" & _
                                         "DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01""  ToolTip=""Cliquer sur le bouton pour choisir une date"" > " & Chr(13) & " </telerik:RadDatePicker>  ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & column.Name & """ runat=""server"" ControlToValidate=""rdp_" & column.Name & """  ")
                        objWriter.WriteLine("  ErrorMessage=""La " & column.Name & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("</tr>")

                    ElseIf column.TrueSqlServerType = "bit" Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<asp:CheckBox ID=""chk_" & column.Name & """ Width=""50%"" runat=""server"">" & _
                                          " </asp:CheckBox>  ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("</tr>")
                    End If
                End If
                countColumn = countColumn + 1

            Next

            objWriter.WriteLine("<tr>")
            objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
            objWriter.WriteLine("</td>")
            objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
            objWriter.WriteLine("<telerik:RadButton ID=""btnSave"" runat=""server"" Skin=""Windows7""  Text=""Sauvegarder"" ValidationGroup=""" & nomSimple & "Group""> ")
            objWriter.WriteLine("<Icon PrimaryIconCssClass=""rbSave"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
            objWriter.WriteLine("</telerik:RadButton>")
            objWriter.WriteLine("&nbsp;")
            objWriter.WriteLine("<telerik:RadButton ID=""btnCancel"" runat=""server"" CausesValidation=""False"" Skin=""Windows7""  Text = ""Abandonner"" >")
            objWriter.WriteLine("<Icon PrimaryIconCssClass=""rbCancel"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
            objWriter.WriteLine("</telerik:RadButton>")

            objWriter.WriteLine("</td>")

            objWriter.WriteLine("</tr>")
            objWriter.WriteLine(" </table>")
            objWriter.WriteLine("</asp:Panel>")
            objWriter.WriteLine("</td>")
            objWriter.WriteLine("                    </tr>")

            objWriter.WriteLine("</table>")

            objWriter.WriteLine("</asp:Panel>")
            objWriter.WriteLine("</div>")
            objWriter.WriteLine("<br />")
            objWriter.WriteLine("</div>")
            objWriter.WriteLine("</center>")
            objWriter.WriteLine("<asp:TextBox ID=""txt_Code" & nomSimple & "_Hid"" runat=""server"" Text=""0"" Visible=""False"" Width=""1px""></asp:TextBox>")
            objWriter.WriteLine("<asp:ValidationSummary ID=""ValidationSummary1"" runat=""server"" ShowMessageBox=""true""" & _
            " ShowSummary=""false"" DisplayMode=""list"" ValidationGroup=""" & nomSimple & "Group"" />")
            objWriter.WriteLine("</asp:Content>")
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateListingCodeBehind(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_libraryname As TextBox)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomWebform & "Listing.aspx.vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)
            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")
            header &= ""
            Dim content As String = "Partial Class " & nomWebform & "Listing" & Chr(13) _
                                     & " Inherits BasePage"

            _end = "End Class" & Chr(13)
            ' Delete the file if it exists.
            If File.Exists(path) Then
                File.Delete(path)
            End If

            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If
            ' Create the file.
            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()



            Dim objWriter As New System.IO.StreamWriter(path, True)
            'objWriter.WriteLine(header)
            If ListBox_NameSpace.Items.Count > 0 Then
                For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                    objWriter.WriteLine(ListBox_NameSpace.Items(i))
                Next
            End If
            Dim libraryname As String = "Imports " & txt_libraryname.Text
            objWriter.WriteLine("Imports Telerik.Web.UI")
            objWriter.WriteLine(libraryname)
            'objWriter.WriteLine("Imports System.Data")
            'objWriter.WriteLine("Imports SolutionsHT")
            'objWriter.WriteLine("Imports SolutionsHT.DataAccessLayer")
            objWriter.WriteLine()

            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            objWriter.WriteLine(content)
            objWriter.WriteLine()


            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = _table.ListofColumn.Count


            Id_table = _table.ListofColumn.Item(0).Name

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                ListofIndex.Insert(countindex, _index.ListofColumn.Item(0).Name)
                countindex = countindex + 1
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}


            For Each _foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreignkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next


            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next


            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")


            objWriter.WriteLine("Dim _out As Boolean = False")
            objWriter.WriteLine("Dim _tmpEditState As Boolean = False")

            objWriter.WriteLine()

            With objWriter
                .WriteLine(" Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load")


                .WriteLine("   If _message = """" Then" & Chr(13) _
                                    & " ' lbl_Error.Text = """" " & Chr(13) _
                                    & Chr(13) _
                                    & "If Not IsPostBack Then")

                .WriteLine("  rbtnAdd" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Parametres/wbfrm_" & nomSimple & ".aspx', 550, 285)); return false;"")" & Chr(13) _
                                    & "BindGrid()" & Chr(13) _
                                    & "End If")

                .WriteLine(" Else " & Chr(13) _
                                    & "MessageToShow(_message)" & Chr(13) _
                                    & "pn_List.Visible = False" & Chr(13) _
                                    & "End If")
                .WriteLine("End Sub")

                .WriteLine()
            End With

            objWriter.WriteLine("Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"")")
            objWriter.WriteLine("   Panel_Msg.Visible = True")
            objWriter.WriteLine(" GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
            objWriter.WriteLine(" Label_Msg.Text = _message")
            objWriter.WriteLine(" If E_or_S = ""S"" Then")
            objWriter.WriteLine(" Label_Msg.ForeColor = Drawing.Color.Green")
            objWriter.WriteLine(" Else")
            objWriter.WriteLine(" Label_Msg.ForeColor = Drawing.Color.Red")
            objWriter.WriteLine(" End If")
            objWriter.WriteLine(" RadAjaxManager1.ResponseScripts.Add(""alert('"" & _message.Replace(""'"", ""\'"") & ""');"")")
            objWriter.WriteLine(" End Sub")
            objWriter.WriteLine()


            With objWriter
                .WriteLine("Private Function BindGrid(Optional ByVal _refresh As Boolean = True ) As Long")
                .WriteLine("Dim objs As List(of Cls_" & nomSimple & ")")
                .WriteLine("Dim _ret As Long = 0")
                .WriteLine("Try")
                .WriteLine("objs = Cls_" & nomSimple & ".SearchAll")
                .WriteLine("rdg" & nomSimple & ".DataSource = objs")
                .WriteLine("If _refresh Then")
                .WriteLine("rdg" & nomSimple & ".DataBind()")
                .WriteLine("End If")
                .WriteLine(" _ret = objs.Count")
                .WriteLine("Catch ex As Exception")
                .WriteLine("_ret = 0")
                .WriteLine("End Try")
                .WriteLine("Return _ret")
                objWriter.WriteLine(" End Function")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rdg" & nomSimple & ".ItemDataBound")
                .WriteLine("Dim gridDataItem = TryCast(e.Item, GridDataItem)")
                .WriteLine(" If e.Item.ItemType = GridItemType.Item Or e.Item.ItemType = GridItemType.AlternatingItem Then")
                .WriteLine(" 'Dim _lnk As HyperLink = DirectCast(gridDataItem.FindControl(""hlk""), HyperLink)")
                .WriteLine("  'Dim _lbl_ID As Label = DirectCast(gridDataItem.FindControl(""lbl_ID""), Label)")
                .WriteLine("'_lnk.Attributes.Clear()")
                .WriteLine("'_lnk.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Parametres/wbfrm_" & nomSimple & "?id="" & CLng(_lbl_ID.Text) & ""', 550, 285));"")")
                .WriteLine("End If")

                .WriteLine("If (gridDataItem IsNot Nothing) Then")
                .WriteLine("Dim item As GridDataItem = gridDataItem")
                .WriteLine("Dim imagedelete As ImageButton = CType(item(""delete"").Controls(0), ImageButton)")
                .WriteLine("Dim imageediter As ImageButton = CType(item(""editer"").Controls(0), ImageButton)")
                .WriteLine("imagedelete.ToolTip = ""Effacer ce " & nomSimple.ToLower & """")
                .WriteLine("imageediter.ToolTip = ""Editer ce " & nomSimple.ToLower & """")
                .WriteLine("imagedelete.CommandArgument = CType(DataBinder.Eval(e.Item.DataItem, ""ID""), String)")
                .WriteLine("imageediter.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Parametres/wbfrm_" & nomSimple & ".aspx?id="" & CType(DataBinder.Eval(e.Item.DataItem, ""ID""), Long) & ""',600,400));"")")
                .WriteLine("End If")

                objWriter.WriteLine(" End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles rdg" & nomSimple & ".NeedDataSource")
                .WriteLine("If IsPostBack Then")
                .WriteLine("BindGrid(False)")
                .WriteLine("End If")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rbtnClearFilters_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnClearFilters.Click")
                .WriteLine("For Each column As GridColumn In rdg" & nomSimple & ".MasterTableView.Columns")
                .WriteLine("column.CurrentFilterFunction = GridKnownFunction.NoFilter")
                .WriteLine("column.CurrentFilterValue = String.Empty")
                .WriteLine("Next")
                .WriteLine("rdg" & nomSimple & ".MasterTableView.FilterExpression = String.Empty")
                .WriteLine(" rdg" & nomSimple & ".MasterTableView.Rebind()")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub RadAjaxManager1_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs) Handles RadAjaxManager1.AjaxRequest")
                .WriteLine(" Try")
                .WriteLine("Select Case e.Argument")
                .WriteLine("Case ""Reload""")
                .WriteLine("BindGrid(True)")
                .WriteLine(" End Select")
                .WriteLine("Catch ex As Exception")
                .WriteLine("_message = ex.Message + "": RadAjaxManager1_AjaxRequest""")
                .WriteLine("MessageToShow(_message)")
                .WriteLine("End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemCommand(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs)")
                .WriteLine("If e.CommandName = Telerik.Web.UI.RadGrid.ExportToExcelCommandName Then")
                .WriteLine("rdg" & nomSimple & ".ExportSettings.ExportOnlyData = True")

                .WriteLine("rdg" & nomSimple & ".GridLines = GridLines.Both")
                .WriteLine("rdg" & nomSimple & ".ExportSettings.IgnorePaging = True")
                .WriteLine("rdg" & nomSimple & ".ExportSettings.OpenInNewWindow = False")
                .WriteLine("rdg" & nomSimple & ".ExportSettings.FileName = ""Liste des " & nomSimple & """")
                .WriteLine("rdg" & nomSimple & ".MasterTableView.Columns(0).Visible = False")
                .WriteLine("rdg" & nomSimple & ".MasterTableView.ExportToExcel()")
                .WriteLine("End If")
                .WriteLine("")
                .WriteLine("Select Case e.CommandName")
                .WriteLine("Case ""delete""")
                .WriteLine("Dim obj As New Cls_" & nomSimple & "(TypeSafeConversion.NullSafeLong(e.CommandArgument))")
                .WriteLine("obj.Delete()")
                .WriteLine("rdg" & nomSimple & ".Rebind()")
                .WriteLine("End Select")
                .WriteLine()

                .WriteLine("End Sub")
                .WriteLine()
            End With

            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateListingCodeAsp(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & "Listing.aspx"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            If File.Exists(path) Then
                File.Delete(path)
            End If
            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If

            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()
            Dim objWriter As New System.IO.StreamWriter(path, True)

            objWriter.WriteLine()

            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer
            cap = _table.ListofColumn.Count
            Id_table = _table.ListofColumn.Item(0).Name
            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}
            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")

            objWriter.WriteLine("<%@ Page Title=""" & nomSimple & """ Language=""VB"" MasterPageFile=""~/Pages/MasterPage/MasterPageCommon.master""" & _
           "AutoEventWireup=""false"" CodeFile=""wbfrm_" & nomSimple & "Listing.aspx.vb"" Inherits=""wbfrm_" & nomSimple & "Listing"" %>")

            objWriter.WriteLine("<asp:Content ID=""Content3"" ContentPlaceHolderID=""SheetContentPlaceHolder"" runat=""Server"">")


            objWriter.WriteLine("<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("<script type=""text/javascript"">")
            objWriter.WriteLine()
            objWriter.WriteLine("function onRequestStart(sender, args) {")
            objWriter.WriteLine(" if (args.get_eventTarget().indexOf(""ExportToExcelButton"") >= 0) {")
            objWriter.WriteLine("args.set_enableAjax(false);")
            objWriter.WriteLine(" }")
            objWriter.WriteLine(" }")
            objWriter.WriteLine()

            With objWriter
                .WriteLine(" function ShowAddUpdateForm(strPage, tmpW, tmpH) {")
                .WriteLine("var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
                .WriteLine("oWindow.set_autoSize(true);")
                .WriteLine("document.getElementById(""txtWindowPage"").value = strPage;")
                .WriteLine(" if (oWindow) {")
                .WriteLine("if (!oWindow.isClosed()) {")
                .WriteLine("oWindow.center();")
                .WriteLine("var bounds = oWindow.getWindowBounds();")
                .WriteLine("oWindow.moveTo(bounds.x + 'px', ""50px"");")
                .WriteLine("}")
                .WriteLine("}")
                .WriteLine("  return false;")
                .WriteLine("}")
                .WriteLine()
            End With

            objWriter.WriteLine("    function ShowAddUpdateFormAutoSize(strPage, tmpW, tmpH) {")
            objWriter.WriteLine("                  var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
            objWriter.WriteLine("                  oWindow.set_autoSize(true);")
            objWriter.WriteLine("                  document.getElementById(""txtWindowPage"").value = strPage;")
            objWriter.WriteLine("                  if (oWindow) {")
            objWriter.WriteLine("                      if (!oWindow.isClosed()) {")
            objWriter.WriteLine("                          oWindow.center();")
            objWriter.WriteLine("                          var bounds = oWindow.getWindowBounds();")
            objWriter.WriteLine("                          oWindow.moveTo(bounds.x + 'px', ""50px"");")
            objWriter.WriteLine("                      }")
            objWriter.WriteLine("                  }")
            objWriter.WriteLine("                  return false;")
            objWriter.WriteLine("              }")



            objWriter.WriteLine(" function ShowAddUpdateFormMaximize(strPage, tmpW, tmpH) {")
            objWriter.WriteLine("var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
            objWriter.WriteLine("oWindow.SetSize(tmpW, tmpH);")
            objWriter.WriteLine("document.getElementById(""txtWindowPage"").value = strPage;")
            objWriter.WriteLine(" if (oWindow) {")
            objWriter.WriteLine("if (!oWindow.isClosed()) {")
            objWriter.WriteLine("oWindow.center();")
            objWriter.WriteLine("var bounds = oWindow.getWindowBounds();")
            objWriter.WriteLine("oWindow.moveTo(bounds.x + 'px', ""50px"");")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("oWindow.maximize();")
            objWriter.WriteLine("  return false;")
            objWriter.WriteLine("}")
            objWriter.WriteLine()



            With objWriter
                .WriteLine("function RadWindowClosing() {")
                .WriteLine(" $find(""<%= RadAjaxManager1.ClientID %>"").ajaxRequest(""Reload"");")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function RadWindowClientResizeEnd() {")
                .WriteLine("var manager = GetRadWindowManager();")
                .WriteLine("var window1 = manager.getActiveWindow();")
                .WriteLine(" window1.center();")
                .WriteLine("var bounds = window1.getWindowBounds();")
                .WriteLine("window1.moveTo(bounds.x + 'px', ""50px"");")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function refreshMe() {")
                .WriteLine("$find(""<%= RadAjaxManager1.ClientID %>"").ajaxRequest(""Reload"");")
                .WriteLine("}")
                .WriteLine()
            End With


            objWriter.WriteLine("</script>")
            objWriter.WriteLine("</telerik:RadCodeBlock>")

            objWriter.WriteLine(" <telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server"">")
            objWriter.WriteLine("<Scripts>")
            objWriter.WriteLine("</Scripts>")
            objWriter.WriteLine("</telerik:RadScriptManager>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("<AjaxSettings>")

            With objWriter
                .WriteLine("<telerik:AjaxSetting AjaxControlID=""RadAjaxManager1"">")
                .WriteLine("<UpdatedControls>")
                .WriteLine(" <telerik:AjaxUpdatedControl ControlID=""pn_List"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
                .WriteLine("</UpdatedControls>")
                .WriteLine("</telerik:AjaxSetting>")
            End With

            With objWriter
                .WriteLine("<telerik:AjaxSetting AjaxControlID=""rdg" & nomSimple & """>")
                .WriteLine("<UpdatedControls>")
                .WriteLine(" <telerik:AjaxUpdatedControl ControlID=""Panel_Msg"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
                .WriteLine(" <telerik:AjaxUpdatedControl ControlID=""rdg" & nomSimple & """ LoadingPanelID=""RadAjaxLoadingPanel1"" />")
                .WriteLine("</UpdatedControls>")
                .WriteLine("</telerik:AjaxSetting>")
            End With

            With objWriter
                .WriteLine("<telerik:AjaxSetting AjaxControlID=""rbtnClearFilters"">")
                .WriteLine("<UpdatedControls>")
                .WriteLine(" <telerik:AjaxUpdatedControl ControlID=""rdg" & nomSimple & """ LoadingPanelID=""RadAjaxLoadingPanel1"" />")
                .WriteLine("</UpdatedControls>")
                .WriteLine("</telerik:AjaxSetting>")
            End With

            objWriter.WriteLine("</AjaxSettings>")
            objWriter.WriteLine("</telerik:RadAjaxManager>")

            objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7"">")
            objWriter.WriteLine("</telerik:RadSkinManager>")

            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")

            objWriter.WriteLine("<center>")


            With objWriter
                .WriteLine("<table style=""width: 100%; height: 100%;"">")
                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td valign=""top"" style=""width: 100%; height: 100%;"">")
                .WriteLine("<table cellspacing=""0"" cellpadding=""0"" style=""width: 100%; height: 100%;"">")
                
                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td style=""width: 100%;""  align=""center"" colspan=""2"" valign=""top"">")
                .WriteLine("<asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
                .WriteLine("<div class=""Classe_MsgErreur"">")
                .WriteLine(" <asp:Image ID=""Image_Msg"" runat=""server"" Style=""vertical-align: middle;"" />")
                .WriteLine(" <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
                .WriteLine(" </div>")
                .WriteLine("</asp:Panel>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td style=""width: 100%;""  align=""center"" colspan=""2"" valign=""top"">")
                .WriteLine("<asp:Panel ID=""pn_List"" runat=""server"" Width=""100%"">")

                .WriteLine("<table style=""width: 100%;"">")
                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td valign=""top"" style=""width: 100%;"">")
                .WriteLine("<table style=""width: 100%;"">")

                .WriteLine(" <tr valign=""top"">")
                .WriteLine("   <td align=""right"" valign=""top"" style=""padding-right: 10px;"">")
                .WriteLine("<div style=""display: block; width: 100%;"">")
                .WriteLine("<div style=""display: block; float: right; padding-right: 10px;"">")
                .WriteLine("<telerik:RadButton ID=""rbtnClearFilters""  Visible=""false"" runat=""server"" Text=""Vider filtres"">")
                .WriteLine("</telerik:RadButton>")
                .WriteLine("</div>")
               
                .WriteLine("<br clear=""all"" />")
                .WriteLine("</div>")
                .WriteLine("</td>")
                .WriteLine(" </tr>")


                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td colspan=""2"">")
                .WriteLine("<div class=""divHeaderEncadrementGridPrime"">")
                .WriteLine("<asp:Label ID=""lbHeaderTitle"" runat=""server"" CssClass=""LabelBold"" Font-Bold=""True""")
                .WriteLine("Font-Size=""Large"" ForeColor=""Black"">")
                .WriteLine("</asp:Label>")
                .WriteLine("</div>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td colspan=""2"">")
                .WriteLine("<div class=""divMenuEncadrementPrime"">")
                .WriteLine("<div>")
                .WriteLine("<div style=""float: left"">")
                .WriteLine("<button runat=""server"" id=""rbtnAdd" & nomSimple & """ type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgAdd""></span><span class=""buttonPrimeSpan"">Nouveau </span>")
                .WriteLine("</button>")
                .WriteLine("<button runat=""server"" visible=""false"" id=""rbtnEdit" & nomSimple & """ type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgEdit""></span><span class=""buttonPrimeSpan"">Editer </span>")
                .WriteLine("</button>")
                .WriteLine("<button runat=""server"" visible=""false"" id=""rbtnDelete" & nomSimple & """ type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgDelete""></span><span class=""buttonPrimeSpan"">Effacer")
                .WriteLine("</span>")
                .WriteLine("</button>")
                .WriteLine("<button runat=""server"" id=""rbtnPayAchat"" type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgTransaction""></span><span class=""buttonPrimeSpan"">Payer")
                .WriteLine("</span>")
                .WriteLine("</button>")
                .WriteLine("<button runat=""server"" id=""rbtnRepot"" type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgPrint""></span><span class=""buttonPrimeSpan"">Imprimer")
                .WriteLine("</span>")
                .WriteLine("</button>")
                .WriteLine("</div>")
                .WriteLine("<div style=""float: left; margin-left: 5px; margin-top: 3px;"">")
                .WriteLine("<asp:Label ID=""lbl_Periode"" Font-Size=""12"" runat=""server"" Text=""Période :"">")
                .WriteLine("</asp:Label>")
                .WriteLine("<div style=""float: right; margin-left: 5px;"">")
                .WriteLine("    <telerik:RadDatePicker Width=""100px"" ID=""rdp_DebutPeriode"" runat=""server"">")
                .WriteLine("</telerik:RadDatePicker>")
                .WriteLine("<asp:Label ID=""Label2"" runat=""server"" Text=""au"">")
                .WriteLine("</asp:Label>")
                .WriteLine("<telerik:RadDatePicker Width=""100px"" ID=""rdp_FinPeriode"" runat=""server"">")
                .WriteLine("</telerik:RadDatePicker>")
                .WriteLine("</div>")
                .WriteLine("</div>")
                .WriteLine("<div style=""float: left; margin-left: 5px; margin-top: 3px;"">")
                .WriteLine("<asp:Label ID=""Label3"" Font-Size=""12"" runat=""server"" Text=""Type :"">")
                .WriteLine("</asp:Label>")
                .WriteLine("<div style=""float: right; margin-left: 5px;"">")
                .WriteLine("<telerik:RadComboBox AutoPostBack=""true"" ID=""rcmb_TypePaiement"" Width=""60px"" runat=""server"">")
                .WriteLine("<Items>")
                .WriteLine("<telerik:RadComboBoxItem runat=""server"" Value=""0"" Text=""Tous"" />")
                .WriteLine("<telerik:RadComboBoxItem runat=""server"" Value=""1"" Text=""Cash"" />")
                .WriteLine("<telerik:RadComboBoxItem runat=""server"" Value=""2"" Text=""Crédit"" />")
                .WriteLine("</Items>")
                .WriteLine("</telerik:RadComboBox>")
                .WriteLine("</div>")
                .WriteLine("</div>")
                .WriteLine("<div style=""float: right; margin-top: 3px;"">")
                .WriteLine("<asp:Label ID=""Label1"" Style=""clear: both;"" Font-Size=""12"" runat=""server"" Text=""Rechercher :""></asp:Label>")
                .WriteLine("<div style=""float: right"">")
                .WriteLine("<telerik:RadTextBox ID=""rtxt_searchString"" runat=""server"">")
                .WriteLine("</telerik:RadTextBox>")
                .WriteLine("<asp:ImageButton ID=""btnSearch"" Style=""clear: both;"" runat=""server"" ImageUrl=""~/img/iconSearch.png"" />")
                .WriteLine("</div>")
                .WriteLine("</div>")
                .WriteLine("</div>")
                .WriteLine("</div>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine(" <tr valign=""top"">")
                .WriteLine("<td align=""left"" valign=""top"">")


                .WriteLine(" <telerik:RadGrid ID=""rdg" & nomSimple & """ AllowPaging=""True"" AllowSorting=""True"" PageSize=""20""")
                .WriteLine(" runat=""server"" AutoGenerateColumns=""False"" GridLines=""None"" AllowFilteringByColumn=""true"" ")

                .WriteLine(" EnableViewState=""true"" AllowMultiRowSelection=""false"" OnItemCommand=""rdg" & nomSimple & "_ItemCommand"" GroupingSettings-CaseSensitive=""false"">")
                .WriteLine(" <ExportSettings HideStructureColumns=""true"" />")
                .WriteLine("  <MasterTableView CommandItemDisplay=""Top"" GridLines=""None"" DataKeyNames=""ID"" NoDetailRecordsText=""Pas d'enregistrement""")
                .WriteLine("NoMasterRecordsText=""Pas d'enregistrement"">")
                .WriteLine(" <CommandItemSettings ShowAddNewRecordButton=""false"" ShowRefreshButton=""false"" ShowExportToExcelButton=""false"" ")
                .WriteLine("  ExportToExcelText=""Exporter en excel"" />")
                .WriteLine(" <Columns>")

                .WriteLine("<telerik:GridBoundColumn DataField=""ID"" UniqueName=""ID"" Display=""false"">")
                .WriteLine("<ItemStyle Width=""1px"" />")
                .WriteLine("</telerik:GridBoundColumn>")

                Dim countColumn As Integer = 0
                Dim pourcentagevalue As Decimal = 100 / (_table.ListofColumn.Count - 4)
                Dim pourcentage As String = pourcentagevalue.ToString + "%"
                For Each column As Cls_Column In _table.ListofColumn
                    Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                    Dim primary As String = _table.ListofColumn(0).Name
                    If countColumn < _table.ListofColumn.Count - 4 Then

                        If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "date" And column.Name <> primary Then
                            Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                            Dim textForcombo As String = ""
                            Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                            Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                            Dim countcolumnref As Long
                            For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                                If column_fore.Type.VbName = "String" Then
                                    textForcombo = column_fore.Name
                                    Exit For
                                End If

                            Next
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & reftablename & "_" & textForcombo & """ UniqueName=""" & reftablename & "_" & textForcombo & """ HeaderText=""" & reftablename & "_" & textForcombo & """")
                            .WriteLine(" FilterControlAltText=""Filter " & reftablename & "_" & textForcombo & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")

                            '.WriteLine("                   <ItemTemplate>")
                            '.WriteLine("<asp:HyperLink ID=""hlk"" runat=""server"" Font-Underline=""true"" ToolTip=""Cliquer pour visualiser"" Text='<%# Bind(""" & nomSimple & "_" & textForcombo & """) %>' NavigateUrl=""#""></asp:HyperLink>")
                            '.WriteLine("<asp:Label ID=""lbl_ID"" runat=""server"" Visible=""false"" Text='<%# Bind(""ID"") %>'></asp:Label>")
                            '.WriteLine("</ItemTemplate>")
                            .WriteLine("</telerik:GridBoundColumn>")
                        ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name Then
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                            .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                            .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" DataFormatString=""{0:##,###,##0.00}"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        ElseIf column.TrueSqlServerType = "date" And column.Name <> _table.ListofColumn(0).Name Then
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                            .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" DataFormatString=""{0:dd/MM/yyyy}"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        End If
                    End If
                    countColumn = countColumn + 1

                Next

                .WriteLine(" <telerik:GridButtonColumn ButtonType=""ImageButton"" CommandArgument=""ID"" CommandName=""editer""")
                .WriteLine("        DataTextField=""ID"" HeaderStyle-HorizontalAlign=""Right"" ImageUrl=""~/images/_edit.png""")
                .WriteLine("        ItemStyle-HorizontalAlign=""Right"" UniqueName=""editer"">")
                .WriteLine("        <HeaderStyle HorizontalAlign=""Right"" Width=""1%"" />")
                .WriteLine("        <ItemStyle HorizontalAlign=""Right"" Width=""1%"" />")
                .WriteLine("    </telerik:GridButtonColumn>")

                .WriteLine("<telerik:GridButtonColumn ButtonType=""ImageButton"" CommandName=""delete"" DataTextField=""ID""")
                .WriteLine("HeaderStyle-HorizontalAlign=""Right"" ImageUrl=""~/images/delete.png"" ItemStyle-HorizontalAlign=""Right""")
                .WriteLine("UniqueName=""delete"" ConfirmDialogType=""RadWindow"" ConfirmText=""Voulez-vous vraiment supprimer ce " & nomSimple & "?""")
                .WriteLine("ConfirmTitle=""Attention!"">")
                .WriteLine("<HeaderStyle HorizontalAlign=""Right"" Width=""1%"" />")
                .WriteLine("<ItemStyle HorizontalAlign=""Right"" Width=""1%"" />")
                .WriteLine("</telerik:GridButtonColumn>")

                .WriteLine("  </Columns>")
                .WriteLine("<RowIndicatorColumn FilterControlAltText=""Filter RowIndicator column"">")
                .WriteLine("</RowIndicatorColumn>")
                .WriteLine("<ExpandCollapseColumn FilterControlAltText=""Filter ExpandColumn column"">")
                .WriteLine("</ExpandCollapseColumn>")
                .WriteLine("</MasterTableView>")

                .WriteLine("<ClientSettings>")
                .WriteLine("<ClientEvents ></ClientEvents>")
                .WriteLine("<Selecting AllowRowSelect=""true"" />")
                .WriteLine("</ClientSettings>")
                .WriteLine("<HeaderContextMenu />")
                .WriteLine("<FilterMenu EnableImageSprites=""False"">")
                .WriteLine("</FilterMenu>")
                .WriteLine("</telerik:RadGrid>")
                .WriteLine("<telerik:RadWindowManager ID=""RadWindowManager1"" runat=""server"" VisibleStatusbar=""false""")
                .WriteLine("EnableViewState=""false"">")
                .WriteLine("<Windows>")
                .WriteLine("<telerik:RadWindow ID=""AddUpdateDialog"" runat=""server"" Title="""" Left=""75px"" ReloadOnShow=""true""")
                .WriteLine(" ShowContentDuringLoad=""false"" Modal=""true"" OnClientClose=""RadWindowClosing"" Behaviors=""Minimize, Move, Resize, Maximize, Close""")
                .WriteLine("EnableShadow=""true"" OnClientResizeEnd=""RadWindowClientResizeEnd"" />")
                .WriteLine("</Windows>")
                .WriteLine("</telerik:RadWindowManager>")


                .WriteLine("<telerik:RadContextMenu ID=""ContextMenu"" runat=""server"" OnClientItemClicked=""""")
                .WriteLine("EnableRoundedCorners=""true"" EnableShadows=""true"">")
                .WriteLine("<Items>")
                .WriteLine("<telerik:RadMenuItem Text=""Editer"" />")
                .WriteLine("</Items>")
                .WriteLine("</telerik:RadContextMenu>")


                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("</table>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("</table>")


                .WriteLine("</asp:Panel>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("</table>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("</table>")
            End With



            objWriter.WriteLine("<br />")
            objWriter.WriteLine("<input id=""txtWindowPage"" type=""hidden"" />")
            objWriter.WriteLine("</center>")
            objWriter.WriteLine("</asp:Content>")

            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateTableInXMLFormat(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\", Application.StartupPath & "\SCRIPT\GENERIC_12\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & ".xml"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)
            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomClasse & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")
            header &= ""
            Dim content As String = "Public Class " & nomClasse & Chr(13) _
                                     & "Implements IGeneral"

            _end = "End Class" & Chr(13)
            ' Delete the file if it exists.
            If File.Exists(path) Then
                File.Delete(path)
            End If
            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If
            ' Create the file.
            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()

            Dim objWriter As New System.IO.StreamWriter(path, True)
            objWriter.WriteLine(header)
            If ListBox_NameSpace.Items.Count > 0 Then
                For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                    objWriter.WriteLine(ListBox_NameSpace.Items(i))
                Next
            End If
            objWriter.WriteLine()
            objWriter.WriteLine(content)
            objWriter.WriteLine()
            Dim _table As New Cls_Table()
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = _table.ListofColumn.Count
            Id_table = _table.ListofColumn.Item(0).Name

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}
            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next


            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next

            Dim settings As XmlWriterSettings = New XmlWriterSettings()
            settings.Indent = True
            Using writer As XmlWriter = XmlWriter.Create(txt_PathGenerate_Script & nomSimple.Substring(1, nomSimple.Length - 1) & ".xml", settings)
                writer.WriteStartDocument()
                writer.WriteStartElement(nomSimple)
                Dim count2 As Integer = 0
                For Each _column As Cls_Column In _table.ListofColumn
                    If count2 < cap - 4 Then
                        writer.WriteStartElement("Colonne")
                        writer.WriteElementString("name", _column.Name)
                        writer.WriteElementString("type", _column.Type.SqlServerName)
                        writer.WriteElementString("longueur", _column.Length)
                        writer.WriteEndElement()
                        count2 += 1
                    Else
                        Exit For
                    End If
                Next
                writer.WriteEndElement()
                'writer.WriteEndDocument()
            End Using

        End Sub


        Public Shared Sub CreateSendRemoveChildInParentAsp(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal group As Cls_GroupTable)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & "Listing.aspx"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            If File.Exists(path) Then
                File.Delete(path)
            End If
            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If

            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()
            Dim objWriter As New System.IO.StreamWriter(path, True)

            objWriter.WriteLine()

            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer
            cap = _table.ListofColumn.Count
            Id_table = _table.ListofColumn.Item(0).Name
            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}
            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")


            objWriter.WriteLine("  <%@ Page Language=""VB"" AutoEventWireup=""false"" CodeFile=""" & group.LiaisonTable.NameWithWbfrm_ & ".aspx.vb"" Inherits=""" & group.LiaisonTable.NameWithWbfrm_ & """ MasterPageFile=""~/PL/Common/MasterPageCommon.master"" %>")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  <%@ Register TagPrefix=""telerik"" Namespace=""Telerik.Web.UI"" Assembly=""Telerik.Web.UI"" %>")
            objWriter.WriteLine("  <asp:Content ID=""Content1"" ContentPlaceHolderID=""SheetContentPlaceHolder"" runat=""server"">")
            objWriter.WriteLine("   ")
            objWriter.WriteLine("  	<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("          <script type=""text/javascript"">")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              function closeWindow() {")
            objWriter.WriteLine("                  window.close();")
            objWriter.WriteLine("              }")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              function CloseAndRefreshListeTypeIdentification() {")
            objWriter.WriteLine("                  GetRadWindow().BrowserWindow.refreshMe();")
            objWriter.WriteLine("                  GetRadWindow().close();")
            objWriter.WriteLine("              }")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              function GetRadWindow() {")
            objWriter.WriteLine("                  var oWindow = null;")
            objWriter.WriteLine("                  if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog")
            objWriter.WriteLine("                  else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well)")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  return oWindow;")
            objWriter.WriteLine("              }")
            objWriter.WriteLine("          </script>")
            objWriter.WriteLine("      </telerik:RadCodeBlock>")
            objWriter.WriteLine("        <telerik:RadStyleSheetManager ID=""RadStyleSheetManager1"" runat=""server"" />")
            objWriter.WriteLine("      <telerik:RadScriptManager ID=""RadScriptManager1"" runat=""server"">")
            objWriter.WriteLine("          <Scripts>")
            objWriter.WriteLine("          </Scripts>")
            objWriter.WriteLine("      </telerik:RadScriptManager>")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("         <telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7"">")
            objWriter.WriteLine("      </telerik:RadSkinManager>")
            objWriter.WriteLine("      <telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  	<input type=""hidden"" id=""radGridClickedRowIndex"" name=""radGridClickedRowIndex"" />")
            objWriter.WriteLine("      ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      <table style=""width: 100%; height: 100%;"">")
            objWriter.WriteLine("  		<tr valign=""top"">")
            objWriter.WriteLine("  			<td valign=""top"" style=""width: 100%; height: 100%;"">")
            objWriter.WriteLine("  				<asp:TextBox id=""txt_Height"" runat=""server"" BackColor=""White"" ForeColor=""White"" Height=""1px"" Visible=""false"" BorderStyle=""None"" BorderColor=""White"" Width=""1px"">500</asp:TextBox>")
            objWriter.WriteLine("                  <table style=""width: 100%; height: 100%;"">")
            objWriter.WriteLine("  					<tr>")
            objWriter.WriteLine("                          <td valign=""middle"" align=""center"" style=""width: 100%;"" colspan=""2"">")
            objWriter.WriteLine("                              <asp:Label ID=""lbl_Error"" runat=""server"" CssClass=""LabelBold"" ForeColor=""Red"" />")
            objWriter.WriteLine("                          </td>")
            objWriter.WriteLine("                      </tr>")
            objWriter.WriteLine("  					<tr valign=""top"" style=""height: 100%;"">")
            objWriter.WriteLine("  						<td valign=""middle"" align=""right"" style=""width: 100%;"" colspan=""2"">")
            objWriter.WriteLine("  									")
            objWriter.WriteLine("  							<asp:Label id=""lbHeaderTitle"" runat=""server"" CssClass=""LabelBold"" Font-Bold=""True"" Font-Size=""Large"" ForeColor=""Black"">Les " & group.ChildTable.NameSansTbl_ & " par " & group.ParentTable.NameSansTbl_ & ".</asp:Label>")
            objWriter.WriteLine("                              <asp:Panel ID=""pnAddUpdate"" runat=""server"">")
            objWriter.WriteLine("  							<table style=""width: 100%; height: 100%;"">")
            objWriter.WriteLine("  								<tr valign=""top"" style=""height: 30px;"">")
            objWriter.WriteLine("  									<td valign=""middle"" align=""right"" style=""width: 45%;"">&nbsp;</td>")
            objWriter.WriteLine("  									<td valign=""middle"" align=""center"" style=""width: 10%;"">&nbsp;</td>")
            objWriter.WriteLine("  									<td valign=""middle"" align=""left"" style=""width: 45%;"">")
            objWriter.WriteLine("  										<asp:Label id=""Label1"" CssClass=""LabelBold"" ForeColor=""Black"" Runat=""server"">Groupes d'Utilisateur</asp:Label>")
            objWriter.WriteLine("  										<telerik:RadComboBox  id=""ddl_" & group.ParentTable.NameSansTbl_ & """ runat=""server"" CssClass=""TextBoxBold"" Width=""95%"" AutoPostBack=""True""></telerik:RadComboBox></td>")
            objWriter.WriteLine("  								</tr>")
            objWriter.WriteLine("  								<tr valign=""top"" style=""height: 100%;"">")
            objWriter.WriteLine("  									<td valign=""top"" align=""right"" style=""width: 45%;"">")
            objWriter.WriteLine("  										<asp:Label id=""Label3"" CssClass=""LabelBold"" ForeColor=""Black"" Runat=""server""> " & group.ChildTable.NameSansTbl_ & " Disponibles</asp:Label>")
            objWriter.WriteLine("  										<telerik:RadListBox id=""LstAvail" & group.ChildTable.NameSansTbl_ & """  tabIndex=""2"" runat=""server"" CssClass=""TextBoxBold"" BackColor=""#FFFFFF""")
            objWriter.WriteLine("  											ForeColor=""Black"" Height=""200px"" BorderStyle=""solid"" Width=""95%"" BorderWidth=""1"" SelectionMode=""Multiple""></telerik:RadListBox></td>")
            objWriter.WriteLine("  									<td valign=""middle"" align=""center"" style=""width: 10%;"">")
            objWriter.WriteLine("  										<table width=""100%"" border=""0"">")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr valign=""middle"">")
            objWriter.WriteLine("  												<td valign=""middle"" align=""center"">")
            objWriter.WriteLine("  													<telerik:RadButton id=""btnSendOne"" runat=""server"" CssClass=""ButtonBold"" Width=""25px"" Text="">""></telerik:RadButton></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td valign=""middle"" align=""center"">")
            objWriter.WriteLine("  													<telerik:RadButton id=""btnSendAll"" runat=""server"" CssClass=""ButtonBold"" Width=""25px"" Text="">>""></telerik:RadButton></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td valign=""middle"" align=""center"">")
            objWriter.WriteLine("  													<telerik:RadButton id=""btnRemoveAll"" runat=""server"" CssClass=""ButtonBold"" Width=""25px"" Text=""<<""></telerik:RadButton></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  											<tr>")
            objWriter.WriteLine("  												<td valign=""middle"" align=""center"">")
            objWriter.WriteLine("  													<telerik:RadButton id=""btnRemoveOne"" runat=""server"" CssClass=""ButtonBold"" Width=""25px"" Text=""<""></telerik:RadButton></td>")
            objWriter.WriteLine("  											</tr>")
            objWriter.WriteLine("  										</table>")
            objWriter.WriteLine("  									</td>")
            objWriter.WriteLine("  									<td valign=""top"" align=""left"" style=""width: 45%;"">")
            objWriter.WriteLine("  										<asp:Label id=""Label4"" CssClass=""LabelBold"" ForeColor=""Black"" Runat=""server"">" & group.ChildTable.NameSansTbl_ & " Assign�(s) au " & group.ParentTable.NameSansTbl_ & "</asp:Label>")
            objWriter.WriteLine("  										<telerik:RadListBox  id=""lstAffect" & group.ChildTable.NameSansTbl_ & """ style=""text-align:right"" tabIndex=""3"" runat=""server"" CssClass=""TextBoxBold"" BackColor=""#FFFFFF""")
            objWriter.WriteLine("  											ForeColor=""Black"" Height=""200px"" BorderStyle=""solid"" Width=""95%"" BorderWidth=""1"" SelectionMode=""Multiple""></telerik:RadListBox ></td>")
            objWriter.WriteLine("  								</tr>")
            objWriter.WriteLine("  							</table>                            ")
            objWriter.WriteLine("                              </asp:Panel>")
            objWriter.WriteLine("  						</td>")
            objWriter.WriteLine("  					</tr>")
            objWriter.WriteLine("  				</table>")
            objWriter.WriteLine("  			</td>")
            objWriter.WriteLine("  		</tr>")
            objWriter.WriteLine("  	</table>")
            objWriter.WriteLine("  </asp:Content>")



        End Sub

        Public Shared Sub CreateSendRemoveChildInParentBehind(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal group As Cls_GroupTable, ByVal libraryname As String)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & "Listing.aspx"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            If File.Exists(path) Then
                File.Delete(path)
            End If
            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If

            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()
            Dim objWriter As New System.IO.StreamWriter(path, True)

            objWriter.WriteLine()

            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer
            cap = _table.ListofColumn.Count
            Id_table = _table.ListofColumn.Item(0).Name
            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}
            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")

            objWriter.WriteLine("  Imports SolutionsHT.Security")
            objWriter.WriteLine("  Imports SolutionsHT.Security.Collections")
            objWriter.WriteLine("  Imports SolutionsHT")
            objWriter.WriteLine("  Imports CaissePopulaireLibrary")
            objWriter.WriteLine("  'Imports CaissePopulaireLibrary.Security")
            objWriter.WriteLine("  Imports System.Collections.Generic")
            objWriter.WriteLine("  Imports Telerik.Web.UI")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  Partial Class wbfrm_" & group.LiaisonTable.NameSansTbl_ & "")
            objWriter.WriteLine("      Inherits BasePage")
            objWriter.WriteLine("      Dim _out As Boolean = False")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load")
            objWriter.WriteLine("          Response.Cache.SetCacheability(HttpCacheability.NoCache)")
            objWriter.WriteLine("          Response.Expires = -1")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          lbl_Error.Text = """"")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          'If Session(""Current_user"") Is Nothing Then")
            objWriter.WriteLine("          '    _message = ""Session expir�e.""")
            objWriter.WriteLine("          '    _out = True")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          '    Response.Redirect(""../Security/wbfrm_Login.aspx?returnURL="" & Request.Url.AbsoluteUri)")
            objWriter.WriteLine("          'Else")
            objWriter.WriteLine("          '    Try")
            objWriter.WriteLine("          '        _currentUser = CType(Session(""Current_user""), Cls_UserAIC)")
            objWriter.WriteLine("          '        If Not (GlobalFunctions.IsUserStillConnected(_currentUser) And GlobalFunctions.IsUserStillActive(_currentUser)) Then")
            objWriter.WriteLine("          '            _currentUser.SetConnectedStatus(False)")
            objWriter.WriteLine("          '            _currentUser.LogActivityUser(""Forced Log Off"", ""Forced to Log Off"", Request.UserHostAddress)")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          '            Session.RemoveAll()")
            objWriter.WriteLine("          '            _message = ""Session expir�e.""")
            objWriter.WriteLine("          '            _out = True")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          '            Response.Redirect(""../Security/wbfrm_Login.aspx?returnURL="" & Request.Url.AbsoluteUri)")
            objWriter.WriteLine("          '        End If")
            objWriter.WriteLine("          '    Catch ex As Exception")
            objWriter.WriteLine("          '        _message = ""Session expir�e.""")
            objWriter.WriteLine("          '        _out = True")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          '        Response.Redirect(""../Security/wbfrm_Login.aspx?returnURL="" & Request.Url.AbsoluteUri)")
            objWriter.WriteLine("          '    End Try")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          '    If _message = """" Then")
            objWriter.WriteLine("          '        If Not IsPostBack Then")
            objWriter.WriteLine("          '            If Privilege.Verify" & group.ChildTable.NameSansTbl_ & "OnObject(_formName, _currentUser.ID_" & group.ParentTable.NameSansTbl_ & ") Then")
            objWriter.WriteLine("          '                CType(Page.Master.FindControl(""lbl_User""), Label).Text = ""| Utilisateur : "" & _currentUser.Login & "" ("" & _currentUser.NomComplet & "") | "" & _currentUser." & group.ParentTable.NameSansTbl_ & "U.Nom" & group.ParentTable.NameSansTbl_ & "e & "" | Derni�re Connection :"" & CStr(_currentUser.LastLogged)")
            objWriter.WriteLine("          '            Else")
            objWriter.WriteLine("          '                _message = ""Pas de Droit � la page d'Association des T�ches aux " & group.ParentTable.NameSansTbl_ & "es.""")
            objWriter.WriteLine("          '                pnAddUpdate.Visible = False")
            objWriter.WriteLine("          '            End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          '        End If")
            objWriter.WriteLine("          '    End If")
            objWriter.WriteLine("          'End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          If _message = """" Then")
            objWriter.WriteLine("              If Not IsPostBack Then")
            objWriter.WriteLine("                  Me.ViewState.Add(""sortfield"", ""ID"")")
            objWriter.WriteLine("                  Me.ViewState.Add(""sortdirection"", ""ASC"")")
            objWriter.WriteLine("                  SetUpPageTitles()")
            objWriter.WriteLine("                  LikeFirstTime(True)")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  lstAffect" & group.ChildTable.NameSansTbl_ & ".Height = Unit.Pixel(CInt(txt_Height.Text) - 200) 'Unit.Pixel(CInt(Request(""h"")) - 200)")
            objWriter.WriteLine("                  LstAvail" & group.ChildTable.NameSansTbl_ & ".Height = Unit.Pixel(CInt(txt_Height.Text) - 200) 'Unit.Pixel(CInt(Request(""h"")) - 200)")
            objWriter.WriteLine("              Else")
            objWriter.WriteLine("                  lstAffect" & group.ChildTable.NameSansTbl_ & ".Height = Unit.Pixel(CInt(txt_Height.Text) - 200)")
            objWriter.WriteLine("                  LstAvail" & group.ChildTable.NameSansTbl_ & ".Height = Unit.Pixel(CInt(txt_Height.Text) - 200)")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          Else")
            objWriter.WriteLine("              RemoveHandler btnSendOne.Click, AddressOf btnSendOne_Click")
            objWriter.WriteLine("              RemoveHandler btnSendAll.Click, AddressOf btnSendAll_Click")
            objWriter.WriteLine("              RemoveHandler btnRemoveOne.Click, AddressOf btnRemoveOne_Click")
            objWriter.WriteLine("              RemoveHandler btnRemoveAll.Click, AddressOf btnRemoveAll_Click")
            objWriter.WriteLine("              RemoveHandler ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndexChanged, AddressOf ddl_" & group.ParentTable.NameSansTbl_ & "_SelectedIndexChanged")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              MessageToShow()")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub MessageToShow()")
            objWriter.WriteLine("          If _message <> """" Then")
            objWriter.WriteLine("              lbl_Error.Text = _message")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub LikeFirstTime(ByVal withchanges As Boolean)")
            objWriter.WriteLine("          Dim tmp As " & group.ParentTable.NameSansTbl_ & "sCollections")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("              tmp = (New " & group.ParentTable.NameWithCls_ & ").Search()")
            objWriter.WriteLine("              If tmp.Count > 0 Then")
            objWriter.WriteLine("                  ddl_" & group.ParentTable.NameSansTbl_ & ".DataSource = tmp")
            objWriter.WriteLine("                  ddl_" & group.ParentTable.NameSansTbl_ & ".DataTextField = ""Nom" & group.ParentTable.NameSansTbl_ & "e""")
            objWriter.WriteLine("                  ddl_" & group.ParentTable.NameSansTbl_ & ".DataValueField = ""ID""")
            objWriter.WriteLine("                  ddl_" & group.ParentTable.NameSansTbl_ & ".DataBind()")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex = 0")
            objWriter.WriteLine("                  FillListes(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("              Else")
            objWriter.WriteLine("                  _message = ""Pas de " & group.ParentTable.NameSansTbl_ & "e d�j� cr��.""")
            objWriter.WriteLine("                  MessageToShow()")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _message = ex.Message")
            objWriter.WriteLine("              MessageToShow()")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub FillListes(ByVal idcmd As Long)")
            objWriter.WriteLine("          Dim tmp As " & group.ChildTable.NameSansTbl_ & "sCollections")
            objWriter.WriteLine("          Dim tmp1 As " & group.ChildTable.NameSansTbl_ & "sCollections")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("              tmp = " & group.ChildTable.NameSansTbl_ & "s.Search" & group.ChildTable.NameSansTbl_ & "sBy" & group.ParentTable.NameSansTbl_ & "(idcmd)")
            objWriter.WriteLine("              If tmp.Count > 0 Then")
            objWriter.WriteLine("                  lstAffect" & group.ChildTable.NameSansTbl_ & ".DataSource = tmp")
            objWriter.WriteLine("                  lstAffect" & group.ChildTable.NameSansTbl_ & ".DataTextField = ""Nom" & group.ChildTable.NameSansTbl_ & """")
            objWriter.WriteLine("                  lstAffect" & group.ChildTable.NameSansTbl_ & ".DataValueField = ""ID""")
            objWriter.WriteLine("                  lstAffect" & group.ChildTable.NameSansTbl_ & ".DataBind()")
            objWriter.WriteLine("              Else")
            objWriter.WriteLine("                  lstAffect" & group.ChildTable.NameSansTbl_ & ".Items.Clear()")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              tmp1 = " & group.ChildTable.NameSansTbl_ & "s.Search" & group.ChildTable.NameSansTbl_ & "sNotAssignTo" & group.ParentTable.NameSansTbl_ & "(idcmd)")
            objWriter.WriteLine("              If tmp1.Count > 0 Then")
            objWriter.WriteLine("                  LstAvail" & group.ChildTable.NameSansTbl_ & ".DataSource = tmp1")
            objWriter.WriteLine("                  LstAvail" & group.ChildTable.NameSansTbl_ & ".DataTextField = ""Nom" & group.ChildTable.NameSansTbl_ & """")
            objWriter.WriteLine("                  LstAvail" & group.ChildTable.NameSansTbl_ & ".DataValueField = ""ID""")
            objWriter.WriteLine("                  LstAvail" & group.ChildTable.NameSansTbl_ & ".DataBind()")
            objWriter.WriteLine("              Else")
            objWriter.WriteLine("                  LstAvail" & group.ChildTable.NameSansTbl_ & ".Items.Clear()")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _message = ex.Message")
            objWriter.WriteLine("              MessageToShow()")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub ddl_" & group.ParentTable.NameSansTbl_ & "_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndexChanged")
            objWriter.WriteLine("          FillListes(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub btnSendOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSendOne.Click")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("              Dim _item As RadListBoxItem")
            objWriter.WriteLine("              Dim obj As " & group.ParentTable.NameSansTbl_ & "s")
            objWriter.WriteLine("              Dim obj1 As " & group.ChildTable.NameSansTbl_ & "s")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              If LstAvail" & group.ChildTable.NameSansTbl_ & ".SelectedIndex >= 0 And ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex >= 0 Then")
            objWriter.WriteLine("                  obj = New " & group.ParentTable.NameSansTbl_ & "s(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("                  For Each _item In LstAvail" & group.ChildTable.NameSansTbl_ & ".Items")
            objWriter.WriteLine("                      If _item.Selected Then")
            objWriter.WriteLine("                          obj1 = New " & group.ChildTable.NameSansTbl_ & "s(CLng(_item.Value))")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                          obj.add" & group.ChildTable.NameSansTbl_ & "s(obj1, _currentUser.Login)")
            objWriter.WriteLine("                      End If")
            objWriter.WriteLine("                  Next")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  FillListes(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("              Else")
            objWriter.WriteLine("                  _message = ""Pas de T�che s�lectionn�e pour affectation OU pas de " & group.ParentTable.NameSansTbl_ & "e.""")
            objWriter.WriteLine("                  MessageToShow()")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _message = ex.Message")
            objWriter.WriteLine("              MessageToShow()")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub btnSendAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSendAll.Click")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("              If LstAvail" & group.ChildTable.NameSansTbl_ & ".Items.Count > 0 And ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex >= 0 Then")
            objWriter.WriteLine("                  Dim i As Long")
            objWriter.WriteLine("                  Dim obj As New " & group.ParentTable.NameSansTbl_ & "s(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("                  Dim obj1 As " & group.ChildTable.NameSansTbl_ & "s")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  For i = 0 To (LstAvail" & group.ChildTable.NameSansTbl_ & ".Items.Count - 1)")
            objWriter.WriteLine("                      obj1 = New " & group.ChildTable.NameSansTbl_ & "s(CLng(LstAvail" & group.ChildTable.NameSansTbl_ & ".Items(i).Value))")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                      obj.add" & group.ChildTable.NameSansTbl_ & "s(obj1, _currentUser.Login)")
            objWriter.WriteLine("                  Next")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  FillListes(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("              Else")
            objWriter.WriteLine("                  _message = ""Pas de T�che disponible pour affectation OU pas de " & group.ParentTable.NameSansTbl_ & "e.""")
            objWriter.WriteLine("                  MessageToShow()")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _message = ex.Message")
            objWriter.WriteLine("              MessageToShow()")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub btnRemoveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveAll.Click")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("              If lstAffect" & group.ChildTable.NameSansTbl_ & ".Items.Count > 0 And ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex >= 0 Then")
            objWriter.WriteLine("                  Dim i As Long")
            objWriter.WriteLine("                  Dim obj As New " & group.ParentTable.NameSansTbl_ & "s(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("                  Dim obj1 As " & group.ChildTable.NameSansTbl_ & "s")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  For i = 0 To (lstAffect" & group.ChildTable.NameSansTbl_ & ".Items.Count - 1)")
            objWriter.WriteLine("                      obj1 = New " & group.ChildTable.NameSansTbl_ & "s(CLng(lstAffect" & group.ChildTable.NameSansTbl_ & ".Items(i).Value))")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                      obj.remove" & group.ChildTable.NameSansTbl_ & "s(obj1)")
            objWriter.WriteLine("                  Next")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  FillListes(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("              Else")
            objWriter.WriteLine("                  _message = ""Pas de T�che disponible pour suppression OU pas de " & group.ParentTable.NameSansTbl_ & "e.""")
            objWriter.WriteLine("                  MessageToShow()")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _message = ex.Message")
            objWriter.WriteLine("              MessageToShow()")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub btnRemoveOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveOne.Click")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("              Dim _item As RadListBoxItem")
            objWriter.WriteLine("              Dim obj As " & group.ParentTable.NameSansTbl_ & "s")
            objWriter.WriteLine("              Dim obj1 As " & group.ChildTable.NameSansTbl_ & "s")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              If lstAffect" & group.ChildTable.NameSansTbl_ & ".SelectedIndex >= 0 And ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex >= 0 Then")
            objWriter.WriteLine("                  obj = New " & group.ParentTable.NameSansTbl_ & "s(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("                  For Each _item In lstAffect" & group.ChildTable.NameSansTbl_ & ".Items")
            objWriter.WriteLine("                      If _item.Selected Then")
            objWriter.WriteLine("                          obj1 = New " & group.ChildTable.NameSansTbl_ & "s(CLng(_item.Value))")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                          obj.remove" & group.ChildTable.NameSansTbl_ & "s(obj1)")
            objWriter.WriteLine("                      End If")
            objWriter.WriteLine("                  Next")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  FillListes(CLng(ddl_" & group.ParentTable.NameSansTbl_ & ".Items(ddl_" & group.ParentTable.NameSansTbl_ & ".SelectedIndex).Value))")
            objWriter.WriteLine("              Else")
            objWriter.WriteLine("                  _message = ""Pas de T�che s�lectionn�e pour suppression OU pas de " & group.ParentTable.NameSansTbl_ & "e.""")
            objWriter.WriteLine("                  MessageToShow()")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _message = ex.Message")
            objWriter.WriteLine("              MessageToShow()")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub SetUpPageTitles()")
            objWriter.WriteLine("          Dim _rpbOperations As RadPanelBar = CType(Page.Master.FindControl(""rpbOperations""), RadPanelBar)")
            objWriter.WriteLine("          _rpbOperations.Visible = False")
            objWriter.WriteLine("          Dim _rpbCompteClients As RadPanelBar = CType(Page.Master.FindControl(""rpbGestionComptes""), RadPanelBar)")
            objWriter.WriteLine("          _rpbCompteClients.Visible = False")
            objWriter.WriteLine("          Dim _rpbdashboard As RadPanelBar = CType(Page.Master.FindControl(""rpbRapports""), RadPanelBar)")
            objWriter.WriteLine("          _rpbdashboard.Visible = False")
            objWriter.WriteLine("          Dim _rpbParametres As RadPanelBar = CType(Page.Master.FindControl(""rpbParametres""), RadPanelBar)")
            objWriter.WriteLine("          _rpbParametres.Visible = False")
            objWriter.WriteLine("          Dim _rpbSecurite As RadPanelBar = CType(Page.Master.FindControl(""rpbSecurite""), RadPanelBar)")
            objWriter.WriteLine("          _rpbSecurite.Visible = True")
            objWriter.WriteLine("          Dim _menuTitle As Label = CType(Page.Master.FindControl(""lbl_menuTitle""), Label)")
            objWriter.WriteLine("          _menuTitle.Text = _rpbSecurite.CookieName")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          Dim item As RadPanelItem = _rpbSecurite.FindItemByValue(""SECURITE"")")
            objWriter.WriteLine("          item.Expanded = True")
            objWriter.WriteLine("          Dim _selectedChild As RadPanelItem = item.Items.FindItemByValue(""SecurAll" & group.LiaisonTable.NameSansTbl_ & """)")
            objWriter.WriteLine("          _selectedChild.Selected = True")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          lbHeaderTitle.Text = ""T�ches assign�es aux groupes""")
            objWriter.WriteLine("          Me.Title = lbHeaderTitle.Text '& "" -  AIC""")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      'Private Sub SetUpPageTitles()")
            objWriter.WriteLine("      '    Dim _rpbSecurite As RadPanelBar = CType(Page.Master.FindControl(""rpbSecurite""), RadPanelBar)")
            objWriter.WriteLine("      '    _rpbSecurite.Visible = True")
            objWriter.WriteLine("      '    Dim _rsp_Workflow As RadSlidingPane = CType(Page.Master.FindControl(""rsp_Workflow""), RadSlidingPane)")
            objWriter.WriteLine("      '    _rsp_Workflow.Title = _rpbSecurite.CookieName")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      '    Dim item As RadPanelItem = _rpbSecurite.FindItemByValue(""GESTIONUTILISATEURS"")")
            objWriter.WriteLine("      '    Dim _selectedChild As RadPanelItem")
            objWriter.WriteLine("      '    lbHeaderTitle.Text = ""Liste des t�ches syst�me""")
            objWriter.WriteLine("      '    _selectedChild = item.Items.FindItemByValue(""Security" & group.ParentTable.NameSansTbl_ & "Tasks"")")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      '    Me.Title = lbHeaderTitle.Text & "" -  AIC""")
            objWriter.WriteLine("      '    item.Expanded = True")
            objWriter.WriteLine("      '    _selectedChild.Selected = True")
            objWriter.WriteLine("      'End Sub")
            objWriter.WriteLine("  End Class")




        End Sub

        Public Shared Sub CreateParentChildInterfaceAsp(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal group As Cls_GroupTable, ByVal libraryname As String)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = group.ParentTable.Name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = group.ParentTable.Name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & group.ParentTable.NameWithWbfrm_ & ".aspx"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            If File.Exists(path) Then
                File.Delete(path)
            End If
            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If

            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()
            Dim objWriter As New System.IO.StreamWriter(path, True)

            objWriter.WriteLine()

            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer
            cap = _table.ListofColumn.Count
            Id_table = _table.ListofColumn.Item(0).Name
            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}
            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")


            objWriter.WriteLine("  <%@ Page Title="""" Language=""VB"" MasterPageFile=""~/Pages/MasterPage/MasterPagePopup.master""")
            objWriter.WriteLine("      AutoEventWireup=""false"" CodeFile=""wbfrm_" & group.ParentTable.NameSansTbl_ & ".aspx.vb"" Inherits=""wbfrm_" & group.ParentTable.NameSansTbl_ & """ %>")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  <asp:Content ID=""Content3"" ContentPlaceHolderID=""SheetContentPlaceHolder"" runat=""Server"">")
            objWriter.WriteLine("      <telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("          <script type=""text/javascript"">")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              function ShowAddUpdateForm(strPage, tmpW, tmpH) {")
            objWriter.WriteLine("                  var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
            objWriter.WriteLine("                  oWindow.set_autoSize(true);")
            objWriter.WriteLine("                  document.getElementById(""txtWindowPage"").value = strPage;")
            objWriter.WriteLine("                  if (oWindow) {")
            objWriter.WriteLine("                      if (!oWindow.isClosed()) {")
            objWriter.WriteLine("                          oWindow.center();")
            objWriter.WriteLine("                          var bounds = oWindow.getWindowBounds();")
            objWriter.WriteLine("                          oWindow.moveTo(bounds.x + 'px', ""50px"");")
            objWriter.WriteLine("                      }")
            objWriter.WriteLine("                  }")
            objWriter.WriteLine("                  return false;")
            objWriter.WriteLine("              }")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              function RadWindowClosing() {")
            objWriter.WriteLine("                  $find(""<%= RadAjaxManager1.ClientID %>"").ajaxRequest(""Reload"");")
            objWriter.WriteLine("              }")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              function RadWindowClientResizeEnd() {")
            objWriter.WriteLine("                  var manager = GetRadWindowManager();")
            objWriter.WriteLine("                  var window1 = manager.getActiveWindow();")
            objWriter.WriteLine("                  window1.center();")
            objWriter.WriteLine("                  var bounds = window1.getWindowBounds();")
            objWriter.WriteLine("                  window1.moveTo(bounds.x + 'px', ""50px"");")
            objWriter.WriteLine("              }")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              function closeWindow() {")
            objWriter.WriteLine("                  GetRadWindow().BrowserWindow.refreshMe();")
            objWriter.WriteLine("                  GetRadWindow().close();")
            objWriter.WriteLine("              }")
            objWriter.WriteLine("              function CloseAndRefreshListe" & group.ParentTable.NameSansTbl_ & "() {")
            objWriter.WriteLine("                  GetRadWindow().BrowserWindow.refreshMe();")
            objWriter.WriteLine("                  GetRadWindow().close();")
            objWriter.WriteLine("              }")
            objWriter.WriteLine("              function GetRadWindow() {")
            objWriter.WriteLine("                  var oWindow = null;")
            objWriter.WriteLine("                  if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog")
            objWriter.WriteLine("                  else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well)")
            objWriter.WriteLine("                  return oWindow;")
            objWriter.WriteLine("              }")
            objWriter.WriteLine("          </script>")
            objWriter.WriteLine("      </telerik:RadCodeBlock>")
            objWriter.WriteLine("      <telerik:RadScriptManager ID=""RadScriptManager1"" runat=""server"">")
            objWriter.WriteLine("          <Scripts>")
            objWriter.WriteLine("          </Scripts>")
            objWriter.WriteLine("      </telerik:RadScriptManager>")
            objWriter.WriteLine("      <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("          <AjaxSettings>")
            objWriter.WriteLine("          </AjaxSettings>")
            objWriter.WriteLine("      </telerik:RadAjaxManager>")
            objWriter.WriteLine("      <%--<telerik:AjaxSetting AjaxControlID=""btnSave"">")
            objWriter.WriteLine("                  <UpdatedControls>")
            objWriter.WriteLine("                      <telerik:AjaxUpdatedControl ControlID=""Panel_Msg"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("                      <telerik:AjaxUpdatedControl ControlID=""pn_Infos" & group.ParentTable.NameSansTbl_ & """ LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("                  </UpdatedControls>")
            objWriter.WriteLine("              </telerik:AjaxSetting>")
            objWriter.WriteLine("              <telerik:AjaxSetting AjaxControlID=""btnSave" & group.ChildTable.NameSansTbl_ & """>")
            objWriter.WriteLine("                  <UpdatedControls>")
            objWriter.WriteLine("                      <telerik:AjaxUpdatedControl ControlID=""Panel_Msg"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("                      <telerik:AjaxUpdatedControl ControlID=""pn_Infos" & group.ParentTable.NameSansTbl_ & """ LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("                  </UpdatedControls>")
            objWriter.WriteLine("              </telerik:AjaxSetting>--%>")
            objWriter.WriteLine("      <telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7"">")
            objWriter.WriteLine("      </telerik:RadSkinManager>")
            objWriter.WriteLine("      <telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")
            objWriter.WriteLine("      <center>")
            objWriter.WriteLine("          <div id=""divFormContainer"" class=""divFormContainer"">")
            objWriter.WriteLine("              <div  class=""divHeaderFormPrime"">")
            objWriter.WriteLine("                  <asp:Label runat=""server"" ID=""lbl_Title"" Text=""Nouvel " & group.ParentTable.NameSansTbl_ & """  Font-Size=""18"" ></asp:Label>")
            objWriter.WriteLine("              </div>")
            objWriter.WriteLine("              <div runat=""server"" id=""pnCompletePage"" class=""divFormContent"">")
            objWriter.WriteLine("                  <asp:Panel ID=""pn_Infos" & group.ParentTable.NameSansTbl_ & """ runat=""server"">")
            objWriter.WriteLine("                          <table border=""0"" class=""form_Enregistrement"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")
            objWriter.WriteLine("                              <tr valign=""top"">")
            objWriter.WriteLine("                                  <td style=""width: 100%;"" align=""center"" colspan=""2"" valign=""top"">")
            objWriter.WriteLine("                                      <asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
            objWriter.WriteLine("                                          <div class=""Classe_MsgErreur"">")
            objWriter.WriteLine("                                              <asp:Image ID=""Image_Msg"" runat=""server"" Style=""vertical-align: middle;"" />")
            objWriter.WriteLine("                                              <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
            objWriter.WriteLine("                                          </div>")
            objWriter.WriteLine("                                      </asp:Panel>")
            objWriter.WriteLine("                                  </td>")
            objWriter.WriteLine("                              </tr>")
            objWriter.WriteLine("                              <tr valign=""top"">")
            objWriter.WriteLine("                                  <td style=""width: 100%"" colspan=""4"">")
            objWriter.WriteLine("                                   ")
            objWriter.WriteLine("                                      <asp:Panel runat=""server"" ID=""pnl_" & group.ParentTable.NameSansTbl_ & """ Visible=""true"" GroupingText="""">")
            objWriter.WriteLine("                                          <fieldset runat=""server"" id=""Fieldset1"" class=""contentBoot"">")
            objWriter.WriteLine("                                              <legend  class=""legendPrime"">En-Tete</legend>")
            objWriter.WriteLine("                                              <table border=""0"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")
            Dim countColumn As Integer = 0
            For Each column As Cls_Column In group.ParentTable.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then

                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ CssClass=""popupFormInputLabel"" Text=""" & columnName & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadComboBox ID=""rcmb_" & columnName & """  Filter=""Contains""  AutoPostBack=""true""  Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadComboBox> ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & columnName & """ runat=""server"" ControlToValidate=""rcmb_" & columnName & """  ")
                        objWriter.WriteLine(" InitialValue=""--Choisir un " & columnName & "--"" ErrorMessage=""Le " & columnName & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("</tr>")
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType <> "bit" Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadTextBox ID=""rtxt_" & column.Name & """ Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadTextBox> ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & column.Name & """ runat=""server"" ControlToValidate=""rtxt_" & column.Name & """  ")
                        objWriter.WriteLine("  ErrorMessage=""Le " & column.Name & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("</tr>")
                    ElseIf (column.TrueSqlServerType = "date" Or column.TrueSqlServerType = "datetime") And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadDatePicker ID=""rdp_" & column.Name & """ Width=""50%"" runat=""server""" & _
                                         "DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01""  ToolTip=""Cliquer sur le bouton pour choisir une date"" > " & Chr(13) & " </telerik:RadDatePicker>  ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & column.Name & """ runat=""server"" ControlToValidate=""rdp_" & column.Name & """  ")
                        objWriter.WriteLine("  ErrorMessage=""La " & column.Name & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("</tr>")

                    ElseIf column.TrueSqlServerType = "bit" Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<asp:CheckBox ID=""chk_" & column.Name & """ Width=""50%"" runat=""server"">" & _
                                          " </asp:CheckBox>  ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("</tr>")
                    End If
                End If
                countColumn = countColumn + 1

            Next
            objWriter.WriteLine("                                                  <tr>")
            objWriter.WriteLine("                                                      <td style=""width: 30%;"" align=""right"">")
            objWriter.WriteLine("                                                      </td>")
            objWriter.WriteLine("                                                      <td style=""width: 70%;"" align=""left"">")
            objWriter.WriteLine("                                                          <telerik:RadButton ID=""btnSave"" runat=""server"" Skin=""Windows7"" Text=""Valider"" ValidationGroup=""" & group.ParentTable.NameSansTbl_ & "Group"">")
            objWriter.WriteLine("                                                              <Icon PrimaryIconCssClass=""rbSave"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
            objWriter.WriteLine("                                                          </telerik:RadButton>")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                                                          &nbsp;")
            objWriter.WriteLine("                                                          <telerik:RadButton ID=""btnCancel"" runat=""server"" CausesValidation=""False"" Skin=""Windows7""")
            objWriter.WriteLine("                                                              Text=""Abandonner"">")
            objWriter.WriteLine("                                                              <Icon PrimaryIconCssClass=""rbCancel"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
            objWriter.WriteLine("                                                          </telerik:RadButton>")
            objWriter.WriteLine("                                                      </td>")
            objWriter.WriteLine("                                                  </tr>")
            objWriter.WriteLine("                                              </table>")
            objWriter.WriteLine("                                          </fieldset>")
            objWriter.WriteLine("                                      </asp:Panel>")
            objWriter.WriteLine("                                   ")
            objWriter.WriteLine("                                  </td>")
            objWriter.WriteLine("                              </tr>")
            objWriter.WriteLine("                              <tr valign=""top"">")
            objWriter.WriteLine("                                  <td style=""width: 100%"" colspan=""4"">")
            objWriter.WriteLine("                                      <asp:Panel runat=""server"" ID=""pnl_" & group.ChildTable.NameSansTbl_ & """ Visible=""true"" GroupingText="""">")
            objWriter.WriteLine("                                          <fieldset runat=""server"" id=""pnl_Specialisations2"" class=""contentBoot"">")
            objWriter.WriteLine("                                              <legend class=""legendPrime"" >Ligne de commande</legend>")
            objWriter.WriteLine("                                              <table style=""width: 100%"">")
            objWriter.WriteLine("                                                  <tr>")
            objWriter.WriteLine("                                                      <td style=""width: 100%"">")
            objWriter.WriteLine("                                                          <asp:Panel runat=""server"" ID=""pnlAdd" & group.ChildTable.NameSansTbl_ & """ BorderColor=""Black"">")
            objWriter.WriteLine("                                                              <table style=""width: 100%;"">")

            countColumn = 0
            For Each column As Cls_Column In group.ChildTable.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then

                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ CssClass=""popupFormInputLabel"" Text=""" & columnName & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadComboBox ID=""rcmb_" & columnName & """  Filter=""Contains""  AutoPostBack=""true""  Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadComboBox> ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & columnName & """ runat=""server"" ControlToValidate=""rcmb_" & columnName & """  ")
                        objWriter.WriteLine(" InitialValue=""--Choisir un " & columnName & "--"" ErrorMessage=""Le " & columnName & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("</tr>")
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType <> "bit" Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadTextBox ID=""rtxt_" & column.Name & """ Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadTextBox> ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & column.Name & """ runat=""server"" ControlToValidate=""rtxt_" & column.Name & """  ")
                        objWriter.WriteLine("  ErrorMessage=""Le " & column.Name & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("</tr>")
                    ElseIf (column.TrueSqlServerType = "date" Or column.TrueSqlServerType = "datetime") And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<telerik:RadDatePicker ID=""rdp_" & column.Name & """ Width=""50%"" runat=""server""" & _
                                         "DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01""  ToolTip=""Cliquer sur le bouton pour choisir une date"" > " & Chr(13) & " </telerik:RadDatePicker>  ")
                        objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & column.Name & """ runat=""server"" ControlToValidate=""rdp_" & column.Name & """  ")
                        objWriter.WriteLine("  ErrorMessage=""La " & column.Name & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("</tr>")

                    ElseIf column.TrueSqlServerType = "bit" Then
                        objWriter.WriteLine("<tr>")
                        objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                        objWriter.WriteLine("<asp:Label ID=""lbl_" & column.Name & """ CssClass=""popupFormInputLabel"" Text=""" & column.Name & " : "" runat=""server""></asp:Label> ")
                        objWriter.WriteLine("</td>")
                        objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                        objWriter.WriteLine("<asp:CheckBox ID=""chk_" & column.Name & """ Width=""50%"" runat=""server"">" & _
                                          " </asp:CheckBox>  ")
                        objWriter.WriteLine("</td>")

                        objWriter.WriteLine("</tr>")
                    End If
                End If
                countColumn = countColumn + 1

            Next
            objWriter.WriteLine("                                                                  <tr>")
            objWriter.WriteLine("                                                                      <td style=""width: 30%;"" align=""right"">")
            objWriter.WriteLine("                                                                      </td>")
            objWriter.WriteLine("                                                                      <td style=""width: 70%;"" align=""left"">")
            objWriter.WriteLine("                                                                          <telerik:RadButton ID=""btnSave" & group.ChildTable.NameSansTbl_ & """ runat=""server"" Skin=""Windows7"" Text=""Sauvegarder"">")
            objWriter.WriteLine("                                                                              <Icon PrimaryIconCssClass=""rbSave"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
            objWriter.WriteLine("                                                                          </telerik:RadButton>")
            objWriter.WriteLine("                                                                          &nbsp;")
            objWriter.WriteLine("                                                                          <telerik:RadButton ID=""btnCancel" & group.ChildTable.NameSansTbl_ & """ runat=""server"" CausesValidation=""False""")
            objWriter.WriteLine("                                                                              Skin=""Windows7"" Text=""Abandonner"">")
            objWriter.WriteLine("                                                                              <Icon PrimaryIconCssClass=""rbCancel"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
            objWriter.WriteLine("                                                                          </telerik:RadButton>")
            objWriter.WriteLine("                                                                      </td>")
            objWriter.WriteLine("                                                                  </tr>")
            objWriter.WriteLine("                                                              </table>")
            objWriter.WriteLine("                                                          </asp:Panel>")
            objWriter.WriteLine("                                                      </td>")
            objWriter.WriteLine("                                                  </tr>")
            objWriter.WriteLine("                                                  <tr id=""tr_addRemove" & group.ChildTable.NameSansTbl_ & """ runat=""server"" visible=""true"">")
            objWriter.WriteLine("                                                      <td style=""width: 100%"" align=""right"">")
            objWriter.WriteLine("                                                          <telerik:RadButton ID=""btnFinish"" Visible=""false"" runat=""server"" Skin=""Windows7""")
            objWriter.WriteLine("                                                              Text=""Terminer"">")
            objWriter.WriteLine("                                                          </telerik:RadButton>")
            objWriter.WriteLine("                                                          &nbsp&nbsp")
            objWriter.WriteLine("                                                          <telerik:RadButton ID=""btnAddNew" & group.ChildTable.NameSansTbl_ & """ runat=""server"" CssClass=""button"" Text=""Ajouter nouvelle ligne de commande""")
            objWriter.WriteLine("                                                              ValidationGroup=""SpecialisationGroup"">")
            objWriter.WriteLine("                                                              <Icon PrimaryIconCssClass=""rbAdd"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
            objWriter.WriteLine("                                                          </telerik:RadButton>")
            objWriter.WriteLine("                                                      </td>")
            objWriter.WriteLine("                                                  </tr>")
            objWriter.WriteLine("                                                  <tr>")
            objWriter.WriteLine("                                                      <td style=""width: 100%"">")
            objWriter.WriteLine("                                                          <telerik:RadGrid ID=""rdg" & group.ChildTable.NameSansTbl_ & """ AllowPaging=""True"" AllowSorting=""True"" PageSize=""20""")
            objWriter.WriteLine("                                                              runat=""server"" AutoGenerateColumns=""False"" GridLines=""None"" AllowFilteringByColumn=""true""")
            objWriter.WriteLine("                                                              EnableViewState=""true"" AllowMultiRowSelection=""false"" OnItemCommand=""rdg" & group.ChildTable.NameSansTbl_ & "_ItemCommand""")
            objWriter.WriteLine("                                                              GroupingSettings-CaseSensitive=""false"">")
            objWriter.WriteLine("                                                              <ExportSettings HideStructureColumns=""true"" />")
            objWriter.WriteLine("                                                              <MasterTableView CommandItemDisplay=""Top"" GridLines=""None"" DataKeyNames=""ID"" NoDetailRecordsText=""Pas d'enregistrement""")
            objWriter.WriteLine("                                                                  NoMasterRecordsText=""Pas d'enregistrement"">")
            objWriter.WriteLine("                                                                  <CommandItemSettings ShowAddNewRecordButton=""false"" ShowRefreshButton=""false"" ShowExportToExcelButton=""false""")
            objWriter.WriteLine("                                                                      ExportToExcelText=""Exporter en excel"" />")
            objWriter.WriteLine("                                                                  <Columns>")
            objWriter.WriteLine("                                                                      <telerik:GridBoundColumn DataField=""ID"" UniqueName=""ID"" Display=""false"">")
            objWriter.WriteLine("                                                                          <ItemStyle Width=""1px"" />")
            objWriter.WriteLine("                                                                      </telerik:GridBoundColumn>")
            countColumn = 0
            Dim pourcentagevalue As Decimal = 100 / (_table.ListofColumn.Count - 4)
            Dim pourcentage As String = pourcentagevalue.ToString + "%"
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "date" And column.Name <> primary Then
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                        Dim textForcombo As String = ""
                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                        Dim countcolumnref As Long
                        For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                            If column_fore.Type.VbName = "String" Then
                                textForcombo = column_fore.Name
                                Exit For
                            End If

                        Next
                        objWriter.WriteLine("<telerik:GridBoundColumn DataField=""" & reftablename & "_" & textForcombo & """ UniqueName=""" & reftablename & "_" & textForcombo & """ HeaderText=""" & reftablename & "_" & textForcombo & """")
                        objWriter.WriteLine(" FilterControlAltText=""Filter " & reftablename & "_" & textForcombo & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                        objWriter.WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        objWriter.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")

                        '.WriteLine("                   <ItemTemplate>")
                        '.WriteLine("<asp:HyperLink ID=""hlk"" runat=""server"" Font-Underline=""true"" ToolTip=""Cliquer pour visualiser"" Text='<%# Bind(""" & nomSimple & "_" & textForcombo & """) %>' NavigateUrl=""#""></asp:HyperLink>")
                        '.WriteLine("<asp:Label ID=""lbl_ID"" runat=""server"" Visible=""false"" Text='<%# Bind(""ID"") %>'></asp:Label>")
                        '.WriteLine("</ItemTemplate>")
                        objWriter.WriteLine("</telerik:GridBoundColumn>")
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                        objWriter.WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                        objWriter.WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        objWriter.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                        objWriter.WriteLine("</telerik:GridBoundColumn>")
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then
                        objWriter.WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                        objWriter.WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" DataFormatString=""{0:##,###,##0.00}"" ShowFilterIcon=""false""")
                        objWriter.WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        objWriter.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                        objWriter.WriteLine("</telerik:GridBoundColumn>")
                    ElseIf column.TrueSqlServerType = "date" And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                        objWriter.WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" DataFormatString=""{0:##,###,##0.00}"" ShowFilterIcon=""false""")
                        objWriter.WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        objWriter.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                        objWriter.WriteLine("</telerik:GridBoundColumn>")
                    End If
                End If
                countColumn = countColumn + 1

            Next
            objWriter.WriteLine("                                                                      <telerik:GridButtonColumn ButtonType=""ImageButton"" CommandArgument=""ID"" CommandName=""editer""")
            objWriter.WriteLine("                                                                          DataTextField=""ID"" HeaderStyle-HorizontalAlign=""Right"" ImageUrl=""~/images/_edit.png""")
            objWriter.WriteLine("                                                                          ItemStyle-HorizontalAlign=""Right"" UniqueName=""editer"">")
            objWriter.WriteLine("                                                                          <HeaderStyle HorizontalAlign=""Right"" Width=""1%"" />")
            objWriter.WriteLine("                                                                          <ItemStyle HorizontalAlign=""Right"" Width=""1%"" />")
            objWriter.WriteLine("                                                                      </telerik:GridButtonColumn>")
            objWriter.WriteLine("                                                                      <telerik:GridButtonColumn ButtonType=""ImageButton"" CommandName=""delete"" DataTextField=""ID""")
            objWriter.WriteLine("                                                                          HeaderStyle-HorizontalAlign=""Right"" ImageUrl=""~/images/delete.png"" ItemStyle-HorizontalAlign=""Right""")
            objWriter.WriteLine("                                                                          UniqueName=""delete"" ConfirmDialogType=""RadWindow"" ConfirmText=""Voulez-vous vraiment supprimer cet enregistrement?""")
            objWriter.WriteLine("                                                                          ConfirmTitle=""Attention!"">")
            objWriter.WriteLine("                                                                          <HeaderStyle HorizontalAlign=""Right"" Width=""1%"" />")
            objWriter.WriteLine("                                                                          <ItemStyle HorizontalAlign=""Right"" Width=""1%"" />")
            objWriter.WriteLine("                                                                      </telerik:GridButtonColumn>")
            objWriter.WriteLine("                                                                  </Columns>")
            objWriter.WriteLine("                                                                  <RowIndicatorColumn FilterControlAltText=""Filter RowIndicator column"">")
            objWriter.WriteLine("                                                                  </RowIndicatorColumn>")
            objWriter.WriteLine("                                                                  <ExpandCollapseColumn FilterControlAltText=""Filter ExpandColumn column"">")
            objWriter.WriteLine("                                                                  </ExpandCollapseColumn>")
            objWriter.WriteLine("                                                              </MasterTableView>")
            objWriter.WriteLine("                                                              <ClientSettings>")
            objWriter.WriteLine("                                                                  <ClientEvents></ClientEvents>")
            objWriter.WriteLine("                                                                  <Selecting AllowRowSelect=""true"" />")
            objWriter.WriteLine("                                                              </ClientSettings>")
            objWriter.WriteLine("                                                              <HeaderContextMenu />")
            objWriter.WriteLine("                                                              <FilterMenu EnableImageSprites=""False"">")
            objWriter.WriteLine("                                                              </FilterMenu>")
            objWriter.WriteLine("                                                          </telerik:RadGrid>")
            objWriter.WriteLine("                                                          <telerik:RadWindowManager ID=""RadWindowManager1"" runat=""server"" VisibleStatusbar=""false""")
            objWriter.WriteLine("                                                              EnableViewState=""false"">")
            objWriter.WriteLine("                                                              <Windows>")
            objWriter.WriteLine("                                                                  <telerik:RadWindow ID=""AddUpdateDialog"" runat=""server"" Title="""" Left=""75px"" ReloadOnShow=""true""")
            objWriter.WriteLine("                                                                      ShowContentDuringLoad=""false"" Modal=""true"" OnClientClose=""RadWindowClosing"" Behaviors=""Minimize, Move, Resize, Maximize, Close""")
            objWriter.WriteLine("                                                                      EnableShadow=""true"" OnClientResizeEnd=""RadWindowClientResizeEnd"" />")
            objWriter.WriteLine("                                                              </Windows>")
            objWriter.WriteLine("                                                          </telerik:RadWindowManager>")
            objWriter.WriteLine("                                                          <telerik:RadContextMenu ID=""ContextMenu"" runat=""server"" OnClientItemClicked="""" EnableRoundedCorners=""true""")
            objWriter.WriteLine("                                                              EnableShadows=""true"">")
            objWriter.WriteLine("                                                              <Items>")
            objWriter.WriteLine("                                                                  <telerik:RadMenuItem Text=""Editer"" />")
            objWriter.WriteLine("                                                              </Items>")
            objWriter.WriteLine("                                                          </telerik:RadContextMenu>")
            objWriter.WriteLine("                                                      </td>")
            objWriter.WriteLine("                                                  </tr>")
            objWriter.WriteLine("                                              </table>")
            objWriter.WriteLine("                                          </fieldset>")
            objWriter.WriteLine("                                      </asp:Panel>")
            objWriter.WriteLine("                                  </td>")
            objWriter.WriteLine("                              </tr>")
            objWriter.WriteLine("                          </table>")
            objWriter.WriteLine("                     ")
            objWriter.WriteLine("                  </asp:Panel>")
            objWriter.WriteLine("              </div>")
            objWriter.WriteLine("              <br />")
            objWriter.WriteLine("          </div>")
            objWriter.WriteLine("      </center>")
            objWriter.WriteLine("      <input id=""txtWindowPage"" type=""hidden"" />")
            objWriter.WriteLine("      <asp:TextBox ID=""txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid"" runat=""server"" Text=""0"" Visible=""False"" Width=""1px""></asp:TextBox>")
            objWriter.WriteLine("      <asp:TextBox ID=""txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid"" runat=""server"" Text=""-1"" Visible=""False""")
            objWriter.WriteLine("          Width=""1px""></asp:TextBox>")
            objWriter.WriteLine("      <asp:TextBox ID=""txt_CodeRolePersonne_Hid"" runat=""server"" Text=""1"" Visible=""False""")
            objWriter.WriteLine("          Width=""1px""></asp:TextBox>")
            objWriter.WriteLine("      <asp:ValidationSummary ID=""ValidationSummary1"" runat=""server"" ShowMessageBox=""true""")
            objWriter.WriteLine("          ShowSummary=""false"" DisplayMode=""list"" ValidationGroup=""" & group.ParentTable.NameSansTbl_ & "Group"" />")
            objWriter.WriteLine("  </asp:Content>")

        End Sub

        Public Shared Sub CreateParentChildInterfaceCodeBehind(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal group As Cls_GroupTable, ByVal libraryname As String)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = group.ParentTable.Name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = group.ParentTable.Name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & "Listing.aspx"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            If File.Exists(path) Then
                File.Delete(path)
            End If
            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If

            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()
            Dim objWriter As New System.IO.StreamWriter(path, True)

            objWriter.WriteLine()

            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer
            cap = _table.ListofColumn.Count
            Id_table = _table.ListofColumn.Item(0).Name
            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}
            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")

            objWriter.WriteLine("  Imports Microsoft")
            objWriter.WriteLine("  Imports System.Data")
            objWriter.WriteLine("  Imports System.Collections.Generic")
            objWriter.WriteLine("  Imports AskesisLibrary")
            objWriter.WriteLine("  Imports AskesisLibrary.DataAccessLayer")
            objWriter.WriteLine("  Imports " & libraryname & "")
            objWriter.WriteLine("  Imports Telerik.Web.UI")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  Partial Class wbfrm_" & group.ParentTable.NameSansTbl_ & "")
            objWriter.WriteLine("      Inherits BasePage")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Dim _out As Boolean = False")
            objWriter.WriteLine("      Dim _tmpEditState As Boolean = False")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load")
            objWriter.WriteLine("          If txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text <> ""0"" Then")
            objWriter.WriteLine("              _tmpEditState = True")
            objWriter.WriteLine("          Else")
            objWriter.WriteLine("              _tmpEditState = False")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("          If _message = """" Then")
            objWriter.WriteLine("              'lbl_Error.Text = """"")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              If Not IsPostBack Then")
            objWriter.WriteLine("                  txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text = CStr(AskesisLibrary.TypeSafeConversion.NullSafeLong(Request.QueryString(""id""), 0))")
            objWriter.WriteLine("                  If txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text <> ""0"" Then")
            objWriter.WriteLine("                      _tmpEditState = True")
            objWriter.WriteLine("                  Else")
            objWriter.WriteLine("                      _tmpEditState = False")
            objWriter.WriteLine("                  End If")
            objWriter.WriteLine("                  rbtnAddPersonne.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Personne/wbfrm_Personne.aspx?rolpers="" & CLng(txt_CodeRolePersonne_Hid.Text) & ""', 550, 285)); return false;"")")
            objWriter.WriteLine("                  btnCancel.Attributes.Add(""onclick"", ""javascript:void(closeWindow());"")")
            objWriter.WriteLine("                  Setup()")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          Else")
            objWriter.WriteLine("              MessageToShow(_message)")
            objWriter.WriteLine("              pn_Infos" & group.ParentTable.NameSansTbl_ & ".Enabled = False")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub EnableEditInfos" & group.ParentTable.NameSansTbl_ & "(ByVal modify As Boolean)")
            objWriter.WriteLine("          If modify Then")
            objWriter.WriteLine("              btnSave.Text = ""Sauvegarder""")
            objWriter.WriteLine("          Else")
            objWriter.WriteLine("              btnSave.Text = ""Editer""")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("          btnSave.CausesValidation = modify")
            For i As Int32 = 1 To cols.Count - 3
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " .Enabled =   modify ")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    objWriter.WriteLine("rdp" & cols(i) & ".Enabled =   modify ")
                ElseIf types(i) = "Boolean" And cols(i) <> "isdirty" Then
                    'objWriter.WriteLine(" radio_yes" & cols(i) & ".Enabled =   modify ")
                    'objWriter.WriteLine("radio_no" & cols(i) & ".Enabled =   modify ")
                    objWriter.WriteLine("chk" & cols(i) & ".Enabled =   modify ")
                Else
                    objWriter.WriteLine("rtxt" & cols(i) & ".Enabled =   modify ")
                End If
            Next
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub Setup()")

            For Each foreignkey As Cls_ForeignKey In group.ParentTable.ListofForeinKey
                Dim attributUsed As String = foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                Dim ClassName As String = "Cls_" & foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                objWriter.WriteLine("FillCombo" & attributUsed & "()")
            Next

            objWriter.WriteLine("          SetupHeader" & group.ChildTable.NameSansTbl_ & "()")
            objWriter.WriteLine("          rtxt_Taxe.Visible = chk_Taxe.Checked")
            objWriter.WriteLine("          If _tmpEditState Then")
            objWriter.WriteLine("              Dim obj As New Cls_" & group.ParentTable.NameSansTbl_ & "(CLng(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text))")
            objWriter.WriteLine("              With obj")
            For i As Int32 = 1 To cols.Count - 3
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " .SelectedIndex =  rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & ".FindItemIndexByValue(." & cols(i).Substring(1, cols(i).Length - 1) & ")")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    objWriter.WriteLine("rdp" & cols(i) & ".SelectedDate = ." & cols(i).Substring(1, cols(i).Length - 1))
                ElseIf types(i) = "Boolean" Then
                    'objWriter.WriteLine("  If ." & cols(i).Substring(1, cols(i).Length - 1) & " = True Then")
                    'objWriter.WriteLine(" radio_yes" & cols(i) & ".Checked = True ")
                    'objWriter.WriteLine("Else")
                    'objWriter.WriteLine("radio_no" & cols(i) & ".Checked = True")
                    'objWriter.WriteLine("End If")
                    objWriter.WriteLine("chk" & cols(i) & ".Checked = ." & cols(i).Substring(1, cols(i).Length - 1))
                Else
                    objWriter.WriteLine("rtxt" & cols(i) & ".Text = ." & cols(i).Substring(1, cols(i).Length - 1))
                End If
            Next
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              End With")
            objWriter.WriteLine("          Else")
            For i As Int32 = 1 To cols.Count - 3
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " .SelectedIndex = 0")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    objWriter.WriteLine("rdp" & cols(i) & ".SelectedDate = Now")
                ElseIf (types(i) = "Boolean") Then
                    '  objWriter.WriteLine("radio_no" & cols(i) & ".Checked = True")
                    'objWriter.WriteLine("radio_no" & cols(i) & ".Checked = False")
                    objWriter.WriteLine("chk" & cols(i) & ".Checked = False")
                Else
                    objWriter.WriteLine("rtxt" & cols(i) & ".Text = """"")
                End If
            Next
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click")
            objWriter.WriteLine("          Select Case btnSave.Text")
            objWriter.WriteLine("              Case ""Editer""")
            objWriter.WriteLine("                  EnableEditInfos" & group.ParentTable.NameSansTbl_ & "(True)")
            objWriter.WriteLine("              Case ""Valider""")
            objWriter.WriteLine("                  Dim _id As Long = CLng(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text)")
            objWriter.WriteLine("                  Dim _obj As New Cls_" & group.ParentTable.NameSansTbl_ & "(_id)")
            objWriter.WriteLine("                  Try")
            objWriter.WriteLine("                      With _obj")
            For i As Int32 = 1 To cols.Count - 3
                Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("." & columnToUse & "  =   rcmb_" & columnToUse.Substring(SqlServerHelper.ForeinKeyPrefix.Length, columnToUse.Length - (SqlServerHelper.ForeinKeyPrefix.Length)) & " .SelectedValue ")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    objWriter.WriteLine("." & columnToUse & " = rdp_" & columnToUse & " .SelectedDate ")
                ElseIf types(i) = "Boolean" Then
                    ' objWriter.WriteLine("." & columnToUse & " =  radio_yes_" & columnToUse & " .Checked")
                    objWriter.WriteLine("." & columnToUse & " =  chk_" & columnToUse & " .Checked")
                Else
                    objWriter.WriteLine("." & columnToUse & " = rtxt_" & columnToUse & ".Text ")
                End If
            Next
            objWriter.WriteLine("                          End If")
            objWriter.WriteLine("                      End With")
            objWriter.WriteLine("                      _obj.Save(""Admin"")")
            objWriter.WriteLine("                      txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text = _obj.ID")
            objWriter.WriteLine("                      txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text = ""0""")
            objWriter.WriteLine("                      '_message = ""Sauvegarde Effectu�e""")
            objWriter.WriteLine("                      'MessageToShow(_message, ""S"")")
            objWriter.WriteLine("                      SetupHeader" & group.ChildTable.NameSansTbl_ & "()")
            objWriter.WriteLine("                      btnSave.Visible = False")
            objWriter.WriteLine("                      btnCancel.Visible = False")
            objWriter.WriteLine("                      'RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe" & group.ParentTable.NameSansTbl_ & "();"")")
            objWriter.WriteLine("                  Catch ex As Exception")
            objWriter.WriteLine("                      _message = ex.Message & Chr(13) & ""La sauvegarde a �chou�e!""")
            objWriter.WriteLine("                      MessageToShow(_message)")
            objWriter.WriteLine("                  End Try")
            objWriter.WriteLine("          End Select")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub btnFinish_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFinish.Click")
            objWriter.WriteLine("          If rcmb_TypePaiement.SelectedValue = Enums.TypePaiement.Credit Then")
            objWriter.WriteLine("              _message = """ & group.ParentTable.NameSansTbl_ & " enregistr�""")
            objWriter.WriteLine("              MessageToShow(_message)")
            objWriter.WriteLine("              RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe" & group.ParentTable.NameSansTbl_ & "();"")")
            objWriter.WriteLine("          Else")
            objWriter.WriteLine("              SaveTransaction()")
            objWriter.WriteLine("              _message = """ & group.ParentTable.NameSansTbl_ & " enregistr�""")
            objWriter.WriteLine("              MessageToShow(_message)")
            objWriter.WriteLine("              RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe" & group.ParentTable.NameSansTbl_ & "();"")")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("          ")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"")")
            objWriter.WriteLine("          Panel_Msg.Visible = True")
            objWriter.WriteLine("          'GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
            objWriter.WriteLine("          Label_Msg.Text = _message")
            objWriter.WriteLine("          If E_or_S = ""S"" Then")
            objWriter.WriteLine("              Label_Msg.ForeColor = Drawing.Color.Green")
            objWriter.WriteLine("          Else")
            objWriter.WriteLine("              Label_Msg.ForeColor = Drawing.Color.Red")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("          RadAjaxManager1.ResponseScripts.Add(""alert('"" & _message.Replace(""'"", ""\'"") & ""');"")")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine()
            For Each foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                Dim textForcombo As String = ""
                Dim attributUsed As String = foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))

                Dim ClassName As String = "Cls_" & foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                ClassName = foreignkey.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
                Dim nomforeign As String = foreignkey.Column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                objWriter.WriteLine("Private Sub FillCombo" & attributUsed & "()")
                objWriter.WriteLine("Try")
                objWriter.WriteLine("Dim objs1 As List(of " & ClassName & ") = " & ClassName & ".SearchAll ")

                For Each column As Cls_Column In foreignkey.RefTable.ListofColumn
                    If column.Type.VbName = "String" Then
                        textForcombo = column.Name
                        Exit For
                    End If
                Next

                objWriter.WriteLine("With rcmb_" & nomforeign)
                objWriter.WriteLine(".Datasource = objs1" & Chr(13) _
                                    & ".DataValueField = ""ID""" & Chr(13) _
                                    & ".DataTextField = """ & textForcombo & """" & Chr(13) _
                                    & ".DataBind()" & Chr(13) _
                                    & ".SelectedIndex = 0" & Chr(13) _
                                    & "End With")
                objWriter.WriteLine("Catch ex As Exception" & Chr(13) _
                                    & "_message = ex.Message" & Chr(13) _
                                    & "MessageToShow(_message)")

                objWriter.WriteLine("End Try")
                objWriter.WriteLine("End Sub")
                objWriter.WriteLine()
            Next
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub rcmb_Personne_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles rcmb_Personne.SelectedIndexChanged")
            objWriter.WriteLine("          ' rtxt_Code" & group.ParentTable.NameSansTbl_ & ".Text = rcmb_Personne.SelectedItem.Text.Substring(0, 3) + ""-"" + Now.Date.Year.ToString + IIf(Now.Date.Month < 10, ""0"" + Now.Date.Month.ToString, Now.Date.Month) + Now.Date.Day.ToString + ""-"" + Now.Hour.ToString + Now.Minute.ToString")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub rcmb_Monnaie_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles rcmb_Monnaie.SelectedIndexChanged")
            objWriter.WriteLine("          Dim obj As New Cls_Monnaie(rcmb_Monnaie.SelectedValue)")
            objWriter.WriteLine("          rtxt_Taux.Text = obj.Taux")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub chk_Taxe_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk_Taxe.CheckedChanged")
            objWriter.WriteLine("          rtxt_Taxe.Visible = chk_Taxe.Checked")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  #Region ""Header " & group.ParentTable.NameSansTbl_ & " Detail""")
            objWriter.WriteLine("    ")
            objWriter.WriteLine("      Private Sub FillComboArticle()")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              rcmb_Article.Items.Clear()")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              Dim objs1 As List(Of Cls_Article) = Cls_Article.SearchAll")
            objWriter.WriteLine("              Dim objstrue As New List(Of Cls_Article)")
            objWriter.WriteLine("              ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              If CLng(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text) > 0 Then")
            objWriter.WriteLine("                  Dim listidArticle As New List(Of Integer)")
            objWriter.WriteLine("                  Dim _achat As New Cls_" & group.ParentTable.NameSansTbl_ & "(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text)")
            objWriter.WriteLine("                  For Each det In _achat." & group.ChildTable.NameSansTbl_ & "s")
            objWriter.WriteLine("                      listidArticle.Add(det.Id_Article)")
            objWriter.WriteLine("                  Next")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  For Each obj In objs1")
            objWriter.WriteLine("                      If listidArticle.Contains(obj.ID) Then")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                      Else")
            objWriter.WriteLine("                          objstrue.Add(obj)")
            objWriter.WriteLine("                      End If")
            objWriter.WriteLine("                  Next")
            objWriter.WriteLine("                  rcmb_Article.DataSource = objstrue")
            objWriter.WriteLine("              Else")
            objWriter.WriteLine("                  rcmb_Article.DataSource = objs1")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              With rcmb_Article")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  .DataValueField = ""ID""")
            objWriter.WriteLine("                  .DataTextField = ""NomArticle""")
            objWriter.WriteLine("                  .DataBind()")
            objWriter.WriteLine("                  .SelectedIndex = 0")
            objWriter.WriteLine("              End With")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _message = ex.Message")
            objWriter.WriteLine("              MessageToShow(_message)")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub SetupHeader" & group.ChildTable.NameSansTbl_ & "()")
            objWriter.WriteLine("          FillComboArticle()")
            objWriter.WriteLine("          If txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text = ""0"" Then")
            objWriter.WriteLine("              pnl_" & group.ChildTable.NameSansTbl_ & ".Enabled = False")
            objWriter.WriteLine("          Else")
            objWriter.WriteLine("              pnl_" & group.ChildTable.NameSansTbl_ & ".Enabled = True")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          If txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text = ""0"" Then")
            objWriter.WriteLine("              pnlAdd" & group.ChildTable.NameSansTbl_ & ".Visible = True")
            objWriter.WriteLine("              btnAddNew" & group.ChildTable.NameSansTbl_ & ".Visible = False")
            objWriter.WriteLine("              '")
            objWriter.WriteLine("              rcmb_Article.SelectedIndex = 0")
            objWriter.WriteLine("              rtxt_Quantite.Text = """"")
            objWriter.WriteLine("              rtxt_PrixUnitaire.Text = """"")

            objWriter.WriteLine("          ElseIf txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text = ""-1"" Then")
            objWriter.WriteLine("              'SetupTypeUnite(""Else"", 0)")
            objWriter.WriteLine("              'SetupTypeListePreremplie(""Else"", 0)")
            objWriter.WriteLine("              pnlAdd" & group.ChildTable.NameSansTbl_ & ".Visible = False")
            objWriter.WriteLine("              btnAddNew" & group.ChildTable.NameSansTbl_ & ".Visible = True")
            objWriter.WriteLine("              BindGrid()")

            objWriter.WriteLine("          ElseIf CLng(txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text) > 0 Then")
            objWriter.WriteLine("              Dim obj As New Cls_" & group.ChildTable.NameSansTbl_ & "(CLng(txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text))")
            objWriter.WriteLine("              With obj")
            objWriter.WriteLine("                  rcmb_Article.SelectedIndex = rcmb_Article.FindItemIndexByValue(.Id_Article)")
            objWriter.WriteLine("                  rtxt_Quantite.Text = .Quantite")
            objWriter.WriteLine("                  rtxt_PrixUnitaire.Text = .PrixUnitaire")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              End With")
            objWriter.WriteLine("              pnlAdd" & group.ChildTable.NameSansTbl_ & ".Visible = True")
            objWriter.WriteLine("              btnAddNew" & group.ChildTable.NameSansTbl_ & ".Visible = False")
            objWriter.WriteLine("              BindGrid()")
            objWriter.WriteLine("          Else")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")


            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub btnAddNew" & group.ChildTable.NameSansTbl_ & "_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNew" & group.ChildTable.NameSansTbl_ & ".Click")
            objWriter.WriteLine("          txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text = ""0""")
            objWriter.WriteLine("          SetupHeader" & group.ChildTable.NameSansTbl_ & "()")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub btnSave" & group.ChildTable.NameSansTbl_ & "_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave" & group.ChildTable.NameSansTbl_ & ".Click")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("              Save" & group.ChildTable.NameSansTbl_ & "()")
            objWriter.WriteLine("              Update" & group.ParentTable.NameSansTbl_ & "WithDetail()")
            objWriter.WriteLine("              UpdateStockArticle()")
            objWriter.WriteLine("              txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text = ""-1""")
            objWriter.WriteLine("              SetupHeader" & group.ChildTable.NameSansTbl_ & "()")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _message = ex.Message & Chr(13) & ""La sauvegarde a �chou�e!""")
            objWriter.WriteLine("              MessageToShow(_message)")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub Save" & group.ChildTable.NameSansTbl_ & "()")
            objWriter.WriteLine("          Dim _id As Long = CLng(txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text)")
            objWriter.WriteLine("          Dim _obj As New Cls_" & group.ChildTable.NameSansTbl_ & "(_id)")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("         With _obj")
            objWriter.WriteLine("              .Id_" & group.ParentTable.NameSansTbl_ & " = CLng(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text)")
            objWriter.WriteLine("              .Id_Article = rcmb_Article.SelectedValue")
            objWriter.WriteLine("              .Quantite = rtxt_Quantite.Text")
            objWriter.WriteLine("              .PrixUnitaire = rtxt_PrixUnitaire.Text")
            objWriter.WriteLine("          End With")
            objWriter.WriteLine("          _obj.Save(""Admin"")")
            objWriter.WriteLine("          txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text = _obj.ID")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub Update" & group.ParentTable.NameSansTbl_ & "WithDetail()")
            objWriter.WriteLine("          Dim _details" & group.ParentTable.NameSansTbl_ & "s As New List(Of Cls_" & group.ChildTable.NameSansTbl_ & ")")
            objWriter.WriteLine("          Dim _achat As New Cls_" & group.ParentTable.NameSansTbl_ & "(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text)")
            objWriter.WriteLine("          _details" & group.ParentTable.NameSansTbl_ & "s = Cls_" & group.ChildTable.NameSansTbl_ & ".SearchAllBy" & group.ParentTable.NameSansTbl_ & "(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text)")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          If _tmpEditState Then")
            objWriter.WriteLine("              _achat.Balance_Saved = 0")
            objWriter.WriteLine("              If _achat.Id_TypePaiement = Enums.TypePaiement.Credit Then")
            objWriter.WriteLine("                  For Each _detail In _details" & group.ParentTable.NameSansTbl_ & "s")
            objWriter.WriteLine("                      If _achat.Balance_Saved = 0 Then")
            objWriter.WriteLine("                          _achat.Balance_Saved = _achat.Balance_Saved + _detail.PrixUnitaire * _detail.Quantite")
            objWriter.WriteLine("                      Else")
            objWriter.WriteLine("                          _achat.Balance_Saved = _achat.Balance_Saved + _detail.PrixUnitaire * _detail.Quantite")
            objWriter.WriteLine("                      End If")
            objWriter.WriteLine("                  Next")
            objWriter.WriteLine("                  If chk_Taxe.Checked Then")
            objWriter.WriteLine("                      _achat.Balance_Saved = _achat.Balance_Saved + _achat.Balance_Saved * _achat.Taxe")
            objWriter.WriteLine("                  End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          Else")
            objWriter.WriteLine("              If _achat.Id_TypePaiement = Enums.TypePaiement.Credit Then")
            objWriter.WriteLine("                  For Each _detail In _details" & group.ParentTable.NameSansTbl_ & "s")
            objWriter.WriteLine("                      If _achat.Balance = 0 Then")
            objWriter.WriteLine("                          _achat.Balance_Saved = _achat.Balance_Saved + _detail.PrixUnitaire * _detail.Quantite")
            objWriter.WriteLine("                      Else")
            objWriter.WriteLine("                          _achat.Balance_Saved = _achat.Balance_Saved + _detail.PrixUnitaire * _detail.Quantite")
            objWriter.WriteLine("                      End If")
            objWriter.WriteLine("                  Next")
            objWriter.WriteLine("                  If chk_Taxe.Checked Then")
            objWriter.WriteLine("                      _achat.Balance_Saved = _achat.Balance_Saved + _achat.Balance_Saved * _achat.Taxe")
            objWriter.WriteLine("                  End If")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          With _achat")
            objWriter.WriteLine("              .IsValidated = True")
            objWriter.WriteLine("              .Save(""Admin"")")
            objWriter.WriteLine("          End With")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          btnFinish.Visible = True")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub SaveTransaction()")
            objWriter.WriteLine("          Dim _achat As New Cls_" & group.ParentTable.NameSansTbl_ & "(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text)")
            objWriter.WriteLine("          If _achat.Id_TypePaiement = Enums.TypePaiement.Cash Then")
            objWriter.WriteLine("              Dim _transaction As New Cls_Transaction")
            objWriter.WriteLine("              With _transaction")
            objWriter.WriteLine("                  .DateTransaction = Now")
            objWriter.WriteLine("                  .NumeroTransaction = ""000""")
            objWriter.WriteLine("                  .Montant = _achat.PrixTotal")
            objWriter.WriteLine("                  .NumeroCheque = ""00""")
            objWriter.WriteLine("                  .Id_Banque = 0")
            objWriter.WriteLine("                  .Id_Compte = Enums.Compte.Fournisseur")
            objWriter.WriteLine("                  .Id_TypeTransaction = Enums.TypeTransaction.Depot")
            objWriter.WriteLine("                  .Description = """"")
            objWriter.WriteLine("                  .Id_Personne = 0")
            objWriter.WriteLine("                  .Taux = _achat.Taux")
            objWriter.WriteLine("                  .Id_Monnaie = _achat.Id_Monnaie")
            objWriter.WriteLine("                  .Save(""Admin"")")
            objWriter.WriteLine("              End With")
            objWriter.WriteLine("              Save" & group.ParentTable.NameSansTbl_ & "Transaction(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text, _transaction.ID)")
            objWriter.WriteLine("              ")
            objWriter.WriteLine("          ElseIf _achat.Id_TypePaiement = Enums.TypePaiement.Cheque Then")
            objWriter.WriteLine("              'Dim _transaction As New Cls_Transaction")
            objWriter.WriteLine("              'With _transaction")
            objWriter.WriteLine("              '    .DateTransaction = Now")
            objWriter.WriteLine("              '    .NumeroTransaction = ""000""")
            objWriter.WriteLine("              '    .Montant = _achat.PrixTotal")
            objWriter.WriteLine("              '    .NumeroCheque = ""00""")
            objWriter.WriteLine("              '    .Id_Banque = 0")
            objWriter.WriteLine("              '    .Id_Compte = Enums.Compte.Fournisseur")
            objWriter.WriteLine("              '    .Id_TypeTransaction = Enums.TypeTransaction.Depot")
            objWriter.WriteLine("              '    .Description = """"s")
            objWriter.WriteLine("              '    .Id_Personne = 0")
            objWriter.WriteLine("              '    .Taux = _achat.Taux")
            objWriter.WriteLine("              '    .Id_Monnaie = _achat.Id_Monnaie")
            objWriter.WriteLine("              '    .Save(""Admin"")")
            objWriter.WriteLine("              'End With")
            objWriter.WriteLine("              'Save" & group.ParentTable.NameSansTbl_ & "Transaction(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text, _transaction.ID)")
            objWriter.WriteLine("          ElseIf _achat.Id_TypePaiement = Enums.TypePaiement.Credit Then")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub Save" & group.ParentTable.NameSansTbl_ & "Transaction(ByVal idachat As Long, ByVal idtransaction As Long)")
            objWriter.WriteLine("          Dim achatTransaction As New Cls_" & group.ParentTable.NameSansTbl_ & "Transaction")
            objWriter.WriteLine("          With achatTransaction")
            objWriter.WriteLine("              .Id_" & group.ParentTable.NameSansTbl_ & " = idachat")
            objWriter.WriteLine("              .Id_Transaction = idtransaction")
            objWriter.WriteLine("              .Save(""Admin"")")
            objWriter.WriteLine("          End With")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Private Sub UpdateStockArticle()")
            objWriter.WriteLine("          Dim _achat As New Cls_" & group.ParentTable.NameSansTbl_ & "(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text)")
            objWriter.WriteLine("          For Each det In _achat." & group.ChildTable.NameSansTbl_ & "s")
            objWriter.WriteLine("              Dim _article As New Cls_Article(det.Id_Article)")
            objWriter.WriteLine("              _article.QuantiteEnStock = _article.QuantiteEnStock + det.Quantite")
            objWriter.WriteLine("              _article.Save(""Admin"")")
            objWriter.WriteLine("          Next")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub btnCancel" & group.ChildTable.NameSansTbl_ & "_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel" & group.ChildTable.NameSansTbl_ & ".Click")
            objWriter.WriteLine("          'Dim obj As New Cls_" & group.ChildTable.NameSansTbl_ & "(CLng(txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text))")
            objWriter.WriteLine("          'If obj.IsValid = False Then")
            objWriter.WriteLine("          '    obj.Delete()")
            objWriter.WriteLine("          'End If")
            objWriter.WriteLine("          'txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text = ""-1""")
            objWriter.WriteLine("          'SetupHeader" & group.ChildTable.NameSansTbl_ & "()")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  #End Region")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  #Region ""Grid " & group.ParentTable.NameSansTbl_ & " Detail """)
            objWriter.WriteLine("      Private Function BindGrid(Optional ByVal _refresh As Boolean = True) As Long")
            objWriter.WriteLine("          Dim objs As List(Of Cls_" & group.ChildTable.NameSansTbl_ & ")")
            objWriter.WriteLine("          Dim _ret As Long = 0")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("              objs = Cls_" & group.ChildTable.NameSansTbl_ & ".SearchAllBy" & group.ParentTable.NameSansTbl_ & "(CLng(txt_Code" & group.ParentTable.NameSansTbl_ & "_Hid.Text))")
            objWriter.WriteLine("              rdg" & group.ChildTable.NameSansTbl_ & ".DataSource = objs")
            objWriter.WriteLine("              If _refresh Then")
            objWriter.WriteLine("                  rdg" & group.ChildTable.NameSansTbl_ & ".DataBind()")
            objWriter.WriteLine("              End If")
            objWriter.WriteLine("              _ret = objs.Count")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _ret = 0")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("          Return _ret")
            objWriter.WriteLine("      End Function")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub rdg" & group.ChildTable.NameSansTbl_ & "_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rdg" & group.ChildTable.NameSansTbl_ & ".ItemDataBound")
            objWriter.WriteLine("          Dim gridDataItem = TryCast(e.Item, GridDataItem)")
            objWriter.WriteLine("          If e.Item.ItemType = GridItemType.Item Or e.Item.ItemType = GridItemType.AlternatingItem Then")
            objWriter.WriteLine("              'Dim _lnk As HyperLink = DirectCast(gridDataItem.FindControl(""hlk""), HyperLink)")
            objWriter.WriteLine("              'Dim _lbl_ID As Label = DirectCast(gridDataItem.FindControl(""lbl_ID""), Label)")
            objWriter.WriteLine("              '_lnk.Attributes.Clear()")
            objWriter.WriteLine("              '_lnk.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Parametres/wbfrm_" & group.ChildTable.NameSansTbl_ & "?id="" & CLng(_lbl_ID.Text) & ""', 550, 285));"")")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("          If (gridDataItem IsNot Nothing) Then")
            objWriter.WriteLine("              Dim item As GridDataItem = gridDataItem")
            objWriter.WriteLine("              Dim imagedelete As ImageButton = CType(item(""delete"").Controls(0), ImageButton)")
            objWriter.WriteLine("              Dim imageediter As ImageButton = CType(item(""editer"").Controls(0), ImageButton)")
            objWriter.WriteLine("              imagedelete.ToolTip = ""Effacer ce champ""")
            objWriter.WriteLine("              imageediter.ToolTip = ""Editer ce champ""")
            objWriter.WriteLine("              imagedelete.CommandArgument = CType(DataBinder.Eval(e.Item.DataItem, ""ID""), String)")
            objWriter.WriteLine("              imageediter.CommandArgument = CType(DataBinder.Eval(e.Item.DataItem, ""ID""), String)")
            objWriter.WriteLine("              ' imageediter.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Parametres/wbfrm_" & group.ChildTable.NameSansTbl_ & ".aspx?id="" & CType(DataBinder.Eval(e.Item.DataItem, ""ID""), Long) & ""',600,400));"")")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub rdg" & group.ChildTable.NameSansTbl_ & "_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles rdg" & group.ChildTable.NameSansTbl_ & ".NeedDataSource")
            objWriter.WriteLine("          If IsPostBack Then")
            objWriter.WriteLine("              BindGrid(False)")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      'Protected Sub rbtnClearFilters_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnClearFilters.Click")
            objWriter.WriteLine("      '    For Each column As GridColumn In rdg" & group.ChildTable.NameSansTbl_ & ".MasterTableView.Columns")
            objWriter.WriteLine("      '        column.CurrentFilterFunction = GridKnownFunction.NoFilter")
            objWriter.WriteLine("      '        column.CurrentFilterValue = String.Empty")
            objWriter.WriteLine("      '    Next")
            objWriter.WriteLine("      '    rdg" & group.ChildTable.NameSansTbl_ & ".MasterTableView.FilterExpression = String.Empty")
            objWriter.WriteLine("      '    rdg" & group.ChildTable.NameSansTbl_ & ".MasterTableView.Rebind()")
            objWriter.WriteLine("      'End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub RadAjaxManager1_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs) Handles RadAjaxManager1.AjaxRequest")
            objWriter.WriteLine("          Try")
            objWriter.WriteLine("              Select Case e.Argument")
            objWriter.WriteLine("                  Case ""Reload""")
            objWriter.WriteLine("                      BindGrid(True)")
            objWriter.WriteLine("              End Select")
            objWriter.WriteLine("          Catch ex As Exception")
            objWriter.WriteLine("              _message = ex.Message + "": RadAjaxManager1_AjaxRequest""")
            objWriter.WriteLine("              MessageToShow(_message)")
            objWriter.WriteLine("          End Try")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("      Protected Sub rdg" & group.ChildTable.NameSansTbl_ & "_ItemCommand(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs)")
            objWriter.WriteLine("          If e.CommandName = Telerik.Web.UI.RadGrid.ExportToExcelCommandName Then")
            objWriter.WriteLine("              rdg" & group.ChildTable.NameSansTbl_ & ".ExportSettings.ExportOnlyData = True")
            objWriter.WriteLine("              rdg" & group.ChildTable.NameSansTbl_ & ".GridLines = GridLines.Both")
            objWriter.WriteLine("              rdg" & group.ChildTable.NameSansTbl_ & ".ExportSettings.IgnorePaging = True")
            objWriter.WriteLine("              rdg" & group.ChildTable.NameSansTbl_ & ".ExportSettings.OpenInNewWindow = False")
            objWriter.WriteLine("              rdg" & group.ChildTable.NameSansTbl_ & ".ExportSettings.FileName = ""Liste des champs""")
            objWriter.WriteLine("              rdg" & group.ChildTable.NameSansTbl_ & ".MasterTableView.Columns(0).Visible = False")
            objWriter.WriteLine("              rdg" & group.ChildTable.NameSansTbl_ & ".MasterTableView.ExportToExcel()")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          Select Case e.CommandName")
            objWriter.WriteLine("              Case ""delete""")
            objWriter.WriteLine("                  Dim obj As New Cls_" & group.ChildTable.NameSansTbl_ & "(TypeSafeConversion.NullSafeLong(e.CommandArgument))")
            objWriter.WriteLine("                  obj.Delete()")
            objWriter.WriteLine("                  rdg" & group.ChildTable.NameSansTbl_ & ".Rebind()")
            objWriter.WriteLine("              Case ""editer""")
            objWriter.WriteLine("                  Dim obj As New Cls_" & group.ChildTable.NameSansTbl_ & "(TypeSafeConversion.NullSafeLong(e.CommandArgument))")
            objWriter.WriteLine("                  txt_Code" & group.ChildTable.NameSansTbl_ & "_Hid.Text = obj.ID")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("                  SetupHeader" & group.ChildTable.NameSansTbl_ & "()")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("          End Select")
            objWriter.WriteLine("      End Sub")
            objWriter.WriteLine("  #End Region")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("     ")
            objWriter.WriteLine("  End Class")
            objWriter.WriteLine("  ")
            objWriter.WriteLine("  ")


        End Sub

        Public Shared Sub CreateParentListingCodeBehind(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_libraryname As TextBox, ByVal group As Cls_GroupTable)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomClasse As String = group.ParentTable.Name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = group.ParentTable.Name.Substring(4, name.Length - 4)
            Dim nomSimpleChild As String = group.ChildTable.Name.Substring(4, group.ChildTable.Name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomWebform & "Listing.aspx.vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)
            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")
            header &= ""
            Dim content As String = "Partial Class " & nomWebform & "Listing" & Chr(13) _
                                     & " Inherits BasePage"

            _end = "End Class" & Chr(13)
            ' Delete the file if it exists.
            If File.Exists(path) Then
                File.Delete(path)
            End If

            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If
            ' Create the file.
            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()



            Dim objWriter As New System.IO.StreamWriter(path, True)
            'objWriter.WriteLine(header)
            If ListBox_NameSpace.Items.Count > 0 Then
                For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                    objWriter.WriteLine(ListBox_NameSpace.Items(i))
                Next
            End If
            Dim libraryname As String = "Imports " & txt_libraryname.Text
            objWriter.WriteLine("Imports Telerik.Web.UI")
            objWriter.WriteLine(libraryname)
            'objWriter.WriteLine("Imports System.Data")
            'objWriter.WriteLine("Imports SolutionsHT")
            'objWriter.WriteLine("Imports SolutionsHT.DataAccessLayer")
            objWriter.WriteLine()

            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            objWriter.WriteLine(content)
            objWriter.WriteLine()


            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = _table.ListofColumn.Count


            Id_table = _table.ListofColumn.Item(0).Name

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                ListofIndex.Insert(countindex, _index.ListofColumn.Item(0).Name)
                countindex = countindex + 1
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}


            For Each _foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreignkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next


            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next


            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")


            objWriter.WriteLine("Dim _out As Boolean = False")
            objWriter.WriteLine("Dim _tmpEditState As Boolean = False")

            objWriter.WriteLine()

            With objWriter
                .WriteLine(" Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load")


                .WriteLine("   If _message = """" Then" & Chr(13) _
                                    & " ' lbl_Error.Text = """" " & Chr(13) _
                                    & Chr(13) _
                                    & "If Not IsPostBack Then")

                .WriteLine("  rbtnAdd" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Parametres/wbfrm_" & nomSimple & ".aspx', 550, 285)); return false;"")" & Chr(13) _
                                    & "BindGrid()" & Chr(13) _
                                    & "End If")

                .WriteLine(" Else " & Chr(13) _
                                    & "MessageToShow(_message)" & Chr(13) _
                                    & "pn_List.Visible = False" & Chr(13) _
                                    & "End If")
                .WriteLine("End Sub")

                .WriteLine()
            End With

            objWriter.WriteLine("Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"")")
            objWriter.WriteLine("   Panel_Msg.Visible = True")
            objWriter.WriteLine(" GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
            objWriter.WriteLine(" Label_Msg.Text = _message")
            objWriter.WriteLine(" If E_or_S = ""S"" Then")
            objWriter.WriteLine(" Label_Msg.ForeColor = Drawing.Color.Green")
            objWriter.WriteLine(" Else")
            objWriter.WriteLine(" Label_Msg.ForeColor = Drawing.Color.Red")
            objWriter.WriteLine(" End If")
            objWriter.WriteLine(" RadAjaxManager1.ResponseScripts.Add(""alert('"" & _message.Replace(""'"", ""\'"") & ""');"")")
            objWriter.WriteLine(" End Sub")
            objWriter.WriteLine()


            With objWriter
                .WriteLine("Private Function BindGrid(Optional ByVal _refresh As Boolean = True ) As Long")
                .WriteLine("Dim objs As List(of Cls_" & nomSimple & ")")
                .WriteLine("Dim _ret As Long = 0")
                .WriteLine("Try")
                .WriteLine("objs = Cls_" & nomSimple & ".SearchAll")
                .WriteLine("rdg" & nomSimple & ".DataSource = objs")
                .WriteLine("If _refresh Then")
                .WriteLine("rdg" & nomSimple & ".DataBind()")
                .WriteLine("End If")
                .WriteLine(" _ret = objs.Count")
                .WriteLine("Catch ex As Exception")
                .WriteLine("_ret = 0")
                .WriteLine("End Try")
                .WriteLine("Return _ret")
                objWriter.WriteLine(" End Function")
                .WriteLine()
            End With

            objWriter.WriteLine("      Protected Sub rdg" & nomSimple & "_ItemCreated(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rdg" & nomSimple & ".ItemCreated")
            objWriter.WriteLine("          If TypeOf e.Item Is GridNestedViewItem Then")
            objWriter.WriteLine("              e.Item.FindControl(""InnerContainer"").Visible = (DirectCast(e.Item, GridNestedViewItem)).ParentItem.Expanded")
            objWriter.WriteLine("          End If")
            objWriter.WriteLine("      End Sub")

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rdg" & nomSimple & ".ItemDataBound")
                .WriteLine("Dim gridDataItem = TryCast(e.Item, GridDataItem)")
                .WriteLine(" If e.Item.ItemType = GridItemType.Item Or e.Item.ItemType = GridItemType.AlternatingItem Then")
                .WriteLine(" 'Dim _lnk As HyperLink = DirectCast(gridDataItem.FindControl(""hlk""), HyperLink)")
                .WriteLine("  'Dim _lbl_ID As Label = DirectCast(gridDataItem.FindControl(""lbl_ID""), Label)")
                .WriteLine("'_lnk.Attributes.Clear()")
                .WriteLine("'_lnk.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Parametres/wbfrm_" & nomSimple & "?id="" & CLng(_lbl_ID.Text) & ""', 550, 285));"")")
                .WriteLine("End If")


                .WriteLine("   If e.Item.ItemType = GridItemType.NestedView Then")
                .WriteLine("              Dim rdg" & nomSimpleChild & " As RadGrid = CType(e.Item.FindControl(""rdg" & nomSimpleChild & """), RadGrid)")
                .WriteLine("  ")
                .WriteLine("   Dim _obj As New Cls_" & nomSimple & "(CType(DataBinder.Eval(e.Item.DataItem, ""ID""), Long))")
                .WriteLine("              rdg" & nomSimpleChild & ".DataSource = _obj." & nomSimpleChild & "s")
                .WriteLine("              rdg" & nomSimpleChild & ".DataBind()")
                .WriteLine("          End If")


                .WriteLine("If (gridDataItem IsNot Nothing) Then")
                .WriteLine("Dim item As GridDataItem = gridDataItem")
                .WriteLine("Dim imagedelete As ImageButton = CType(item(""delete"").Controls(0), ImageButton)")
                .WriteLine("Dim imageediter As ImageButton = CType(item(""editer"").Controls(0), ImageButton)")
                .WriteLine("imagedelete.ToolTip = ""Effacer ce " & nomSimple.ToLower & """")
                .WriteLine("imageediter.ToolTip = ""Editer ce " & nomSimple.ToLower & """")
                .WriteLine("imagedelete.CommandArgument = CType(DataBinder.Eval(e.Item.DataItem, ""ID""), String)")
                .WriteLine("imageediter.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Parametres/wbfrm_" & nomSimple & ".aspx?id="" & CType(DataBinder.Eval(e.Item.DataItem, ""ID""), Long) & ""',600,400));"")")
                .WriteLine("End If")

                objWriter.WriteLine(" End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles rdg" & nomSimple & ".NeedDataSource")
                .WriteLine("If IsPostBack Then")
                .WriteLine("BindGrid(False)")
                .WriteLine("End If")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("    Protected Sub rdg" & nomSimpleChild & "_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs)")
                .WriteLine("          Dim rdg" & nomSimpleChild & " As RadGrid = CType(sender, RadGrid)")
                .WriteLine("          Dim nestedItem As GridNestedViewItem = CType(rdg" & nomSimpleChild & ".NamingContainer, GridNestedViewItem)")
                .WriteLine("          Dim ShiftID As Long = CType(nestedItem.ParentItem, GridDataItem).GetDataKeyValue(""ID"")")
                .WriteLine("          Dim _obj As New Cls_" & nomSimple & "(ShiftID)")
                .WriteLine("          rdg" & nomSimpleChild & ".DataSource = _obj." & nomSimpleChild & "s")
                .WriteLine("          rdg" & nomSimpleChild & ".DataBind()")
                .WriteLine("      End Sub")
            End With

            With objWriter
                .WriteLine("Protected Sub rbtnClearFilters_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnClearFilters.Click")
                .WriteLine("For Each column As GridColumn In rdg" & nomSimple & ".MasterTableView.Columns")
                .WriteLine("column.CurrentFilterFunction = GridKnownFunction.NoFilter")
                .WriteLine("column.CurrentFilterValue = String.Empty")
                .WriteLine("Next")
                .WriteLine("rdg" & nomSimple & ".MasterTableView.FilterExpression = String.Empty")
                .WriteLine(" rdg" & nomSimple & ".MasterTableView.Rebind()")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub RadAjaxManager1_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs) Handles RadAjaxManager1.AjaxRequest")
                .WriteLine(" Try")
                .WriteLine("Select Case e.Argument")
                .WriteLine("Case ""Reload""")
                .WriteLine("BindGrid(True)")
                .WriteLine(" End Select")
                .WriteLine("Catch ex As Exception")
                .WriteLine("_message = ex.Message + "": RadAjaxManager1_AjaxRequest""")
                .WriteLine("MessageToShow(_message)")
                .WriteLine("End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemCommand(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs)")
                .WriteLine("If e.CommandName = Telerik.Web.UI.RadGrid.ExportToExcelCommandName Then")
                .WriteLine("rdg" & nomSimple & ".ExportSettings.ExportOnlyData = True")

                .WriteLine("rdg" & nomSimple & ".GridLines = GridLines.Both")
                .WriteLine("rdg" & nomSimple & ".ExportSettings.IgnorePaging = True")
                .WriteLine("rdg" & nomSimple & ".ExportSettings.OpenInNewWindow = False")
                .WriteLine("rdg" & nomSimple & ".ExportSettings.FileName = ""Liste des " & nomSimple & """")
                .WriteLine("rdg" & nomSimple & ".MasterTableView.Columns(0).Visible = False")
                .WriteLine("rdg" & nomSimple & ".MasterTableView.ExportToExcel()")
                .WriteLine("End If")
                .WriteLine("")

                .WriteLine("          If e.CommandName = RadGrid.ExpandCollapseCommandName And TypeOf e.Item Is GridDataItem Then")
                .WriteLine("              DirectCast(e.Item, GridDataItem).ChildItem.FindControl(""InnerContainer"").Visible = Not e.Item.Expanded")
                .WriteLine("          End If")


                .WriteLine("Select Case e.CommandName")
                .WriteLine("Case ""delete""")
                .WriteLine("Dim obj As New Cls_" & nomSimple & "(TypeSafeConversion.NullSafeLong(e.CommandArgument))")
                .WriteLine("obj.Delete()")
                .WriteLine("rdg" & nomSimple & ".Rebind()")
                .WriteLine("End Select")
                .WriteLine()

                .WriteLine("End Sub")
                .WriteLine()
            End With

            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateParentListingCodeAsp(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal group As Cls_GroupTable)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = group.ParentTable.Name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = group.ParentTable.Name.Substring(4, name.Length - 4)

            Dim nomSimpleChild As String = group.ChildTable.Name.Substring(4, group.ChildTable.Name.Length - 4)

            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\ASPWebForm\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & "Listing.aspx"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            If File.Exists(path) Then
                File.Delete(path)
            End If
            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If

            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()
            Dim objWriter As New System.IO.StreamWriter(path, True)

            objWriter.WriteLine()

            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer
            cap = _table.ListofColumn.Count
            Id_table = _table.ListofColumn.Item(0).Name
            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}
            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")

            objWriter.WriteLine("<%@ Page Title=""" & nomSimple & """ Language=""VB"" MasterPageFile=""~/Pages/MasterPage/MasterPageCommon.master""" & _
           "AutoEventWireup=""false"" CodeFile=""wbfrm_" & nomSimple & "Listing.aspx.vb"" Inherits=""wbfrm_" & nomSimple & "Listing"" %>")

            objWriter.WriteLine("<asp:Content ID=""Content3"" ContentPlaceHolderID=""SheetContentPlaceHolder"" runat=""Server"">")


            objWriter.WriteLine("<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("<script type=""text/javascript"">")
            objWriter.WriteLine()
            objWriter.WriteLine("function onRequestStart(sender, args) {")
            objWriter.WriteLine(" if (args.get_eventTarget().indexOf(""ExportToExcelButton"") >= 0) {")
            objWriter.WriteLine("args.set_enableAjax(false);")
            objWriter.WriteLine(" }")
            objWriter.WriteLine(" }")
            objWriter.WriteLine()

            With objWriter
                .WriteLine(" function ShowAddUpdateForm(strPage, tmpW, tmpH) {")
                .WriteLine("var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
                .WriteLine("oWindow.set_autoSize(true);")
                .WriteLine("document.getElementById(""txtWindowPage"").value = strPage;")
                .WriteLine(" if (oWindow) {")
                .WriteLine("if (!oWindow.isClosed()) {")
                .WriteLine("oWindow.center();")
                .WriteLine("var bounds = oWindow.getWindowBounds();")
                .WriteLine("oWindow.moveTo(bounds.x + 'px', ""50px"");")
                .WriteLine("}")
                .WriteLine("}")
                .WriteLine("  return false;")
                .WriteLine("}")
                .WriteLine()
            End With

            objWriter.WriteLine("    function ShowAddUpdateFormAutoSize(strPage, tmpW, tmpH) {")
            objWriter.WriteLine("                  var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
            objWriter.WriteLine("                  oWindow.set_autoSize(true);")
            objWriter.WriteLine("                  document.getElementById(""txtWindowPage"").value = strPage;")
            objWriter.WriteLine("                  if (oWindow) {")
            objWriter.WriteLine("                      if (!oWindow.isClosed()) {")
            objWriter.WriteLine("                          oWindow.center();")
            objWriter.WriteLine("                          var bounds = oWindow.getWindowBounds();")
            objWriter.WriteLine("                          oWindow.moveTo(bounds.x + 'px', ""50px"");")
            objWriter.WriteLine("                      }")
            objWriter.WriteLine("                  }")
            objWriter.WriteLine("                  return false;")
            objWriter.WriteLine("              }")



            objWriter.WriteLine(" function ShowAddUpdateFormMaximize(strPage, tmpW, tmpH) {")
            objWriter.WriteLine("var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
            objWriter.WriteLine("oWindow.SetSize(tmpW, tmpH);")
            objWriter.WriteLine("document.getElementById(""txtWindowPage"").value = strPage;")
            objWriter.WriteLine(" if (oWindow) {")
            objWriter.WriteLine("if (!oWindow.isClosed()) {")
            objWriter.WriteLine("oWindow.center();")
            objWriter.WriteLine("var bounds = oWindow.getWindowBounds();")
            objWriter.WriteLine("oWindow.moveTo(bounds.x + 'px', ""50px"");")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("oWindow.maximize();")
            objWriter.WriteLine("  return false;")
            objWriter.WriteLine("}")
            objWriter.WriteLine()



            With objWriter
                .WriteLine("function RadWindowClosing() {")
                .WriteLine(" $find(""<%= RadAjaxManager1.ClientID %>"").ajaxRequest(""Reload"");")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function RadWindowClientResizeEnd() {")
                .WriteLine("var manager = GetRadWindowManager();")
                .WriteLine("var window1 = manager.getActiveWindow();")
                .WriteLine(" window1.center();")
                .WriteLine("var bounds = window1.getWindowBounds();")
                .WriteLine("window1.moveTo(bounds.x + 'px', ""50px"");")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function refreshMe() {")
                .WriteLine("$find(""<%= RadAjaxManager1.ClientID %>"").ajaxRequest(""Reload"");")
                .WriteLine("}")
                .WriteLine()
            End With


            objWriter.WriteLine("</script>")
            objWriter.WriteLine("</telerik:RadCodeBlock>")

            objWriter.WriteLine(" <telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server"">")
            objWriter.WriteLine("<Scripts>")
            objWriter.WriteLine("</Scripts>")
            objWriter.WriteLine("</telerik:RadScriptManager>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("<AjaxSettings>")

            With objWriter
                .WriteLine("<telerik:AjaxSetting AjaxControlID=""RadAjaxManager1"">")
                .WriteLine("<UpdatedControls>")
                .WriteLine(" <telerik:AjaxUpdatedControl ControlID=""pn_List"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
                .WriteLine("</UpdatedControls>")
                .WriteLine("</telerik:AjaxSetting>")
            End With

            With objWriter
                .WriteLine("<telerik:AjaxSetting AjaxControlID=""rdg" & nomSimple & """>")
                .WriteLine("<UpdatedControls>")
                .WriteLine(" <telerik:AjaxUpdatedControl ControlID=""Panel_Msg"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
                .WriteLine(" <telerik:AjaxUpdatedControl ControlID=""rdg" & nomSimple & """ LoadingPanelID=""RadAjaxLoadingPanel1"" />")
                .WriteLine("</UpdatedControls>")
                .WriteLine("</telerik:AjaxSetting>")
            End With

            With objWriter
                .WriteLine("<telerik:AjaxSetting AjaxControlID=""rbtnClearFilters"">")
                .WriteLine("<UpdatedControls>")
                .WriteLine(" <telerik:AjaxUpdatedControl ControlID=""rdg" & nomSimple & """ LoadingPanelID=""RadAjaxLoadingPanel1"" />")
                .WriteLine("</UpdatedControls>")
                .WriteLine("</telerik:AjaxSetting>")
            End With

            objWriter.WriteLine("</AjaxSettings>")
            objWriter.WriteLine("</telerik:RadAjaxManager>")

            objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7"">")
            objWriter.WriteLine("</telerik:RadSkinManager>")

            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")

            objWriter.WriteLine("<center>")


            With objWriter
                .WriteLine("<table style=""width: 100%; height: 100%;"">")
                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td valign=""top"" style=""width: 100%; height: 100%;"">")
                .WriteLine("<table cellspacing=""0"" cellpadding=""0"" style=""width: 100%; height: 100%;"">")

                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td style=""width: 100%;""  align=""center"" colspan=""2"" valign=""top"">")
                .WriteLine("<asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
                .WriteLine("<div class=""Classe_MsgErreur"">")
                .WriteLine(" <asp:Image ID=""Image_Msg"" runat=""server"" Style=""vertical-align: middle;"" />")
                .WriteLine(" <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
                .WriteLine(" </div>")
                .WriteLine("</asp:Panel>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td style=""width: 100%;""  align=""center"" colspan=""2"" valign=""top"">")
                .WriteLine("<asp:Panel ID=""pn_List"" runat=""server"" Width=""100%"">")

                .WriteLine("<table style=""width: 100%;"">")
                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td valign=""top"" style=""width: 100%;"">")
                .WriteLine("<table style=""width: 100%;"">")

                .WriteLine(" <tr valign=""top"">")
                .WriteLine("   <td align=""right"" valign=""top"" style=""padding-right: 10px;"">")
                .WriteLine("<div style=""display: block; width: 100%;"">")
                .WriteLine("<div style=""display: block; float: right; padding-right: 10px;"">")
                .WriteLine("<telerik:RadButton ID=""rbtnClearFilters""  Visible=""false"" runat=""server"" Text=""Vider filtres"">")
                .WriteLine("</telerik:RadButton>")
                .WriteLine("</div>")

                .WriteLine("<br clear=""all"" />")
                .WriteLine("</div>")
                .WriteLine("</td>")
                .WriteLine(" </tr>")


                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td colspan=""2"">")
                .WriteLine("<div class=""divHeaderEncadrementGridPrime"">")
                .WriteLine("<asp:Label ID=""lbHeaderTitle"" runat=""server"" CssClass=""LabelBold"" Font-Bold=""True""")
                .WriteLine("Font-Size=""Large"" ForeColor=""Black"">")
                .WriteLine("</asp:Label>")
                .WriteLine("</div>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("<tr valign=""top"">")
                .WriteLine("<td colspan=""2"">")
                .WriteLine("<div class=""divMenuEncadrementPrime"">")
                .WriteLine("<div>")
                .WriteLine("<div style=""float: left"">")
                .WriteLine("<button runat=""server"" id=""rbtnAdd" & nomSimple & """ type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgAdd""></span><span class=""buttonPrimeSpan"">Nouveau </span>")
                .WriteLine("</button>")
                .WriteLine("<button runat=""server"" visible=""false"" id=""rbtnEdit" & nomSimple & """ type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgEdit""></span><span class=""buttonPrimeSpan"">Editer </span>")
                .WriteLine("</button>")
                .WriteLine("<button runat=""server"" visible=""false"" id=""rbtnDelete" & nomSimple & """ type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgDelete""></span><span class=""buttonPrimeSpan"">Effacer")
                .WriteLine("</span>")
                .WriteLine("</button>")
                .WriteLine("<button runat=""server"" id=""rbtnPayAchat"" type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgTransaction""></span><span class=""buttonPrimeSpan"">Payer")
                .WriteLine("</span>")
                .WriteLine("</button>")
                .WriteLine("<button runat=""server"" id=""rbtnRepot"" type=""submit"" class=""buttonPrime"">")
                .WriteLine("<span class=""buttonPrimeImgPrint""></span><span class=""buttonPrimeSpan"">Imprimer")
                .WriteLine("</span>")
                .WriteLine("</button>")
                .WriteLine("</div>")
                .WriteLine("<div style=""float: left; margin-left: 5px; margin-top: 3px;"">")
                .WriteLine("<asp:Label ID=""lbl_Periode"" Font-Size=""12"" runat=""server"" Text=""Période :"">")
                .WriteLine("</asp:Label>")

                .WriteLine("    <telerik:RadDatePicker Width=""100px"" ID=""rdp_DebutPeriode"" runat=""server"">")
                .WriteLine("</telerik:RadDatePicker>")
                .WriteLine("<asp:Label ID=""Label2"" runat=""server"" Text=""au"">")
                .WriteLine("</asp:Label>")
                .WriteLine("<telerik:RadDatePicker Width=""100px"" ID=""rdp_FinPeriode"" runat=""server"">")
                .WriteLine("</telerik:RadDatePicker>")

                .WriteLine("</div>")
                .WriteLine("<div style=""float: left; margin-left: 5px; margin-top: 3px;"">")
                .WriteLine("<asp:Label ID=""Label3"" Font-Size=""12"" runat=""server"" Text=""Type :"">")
                .WriteLine("</asp:Label>")

                .WriteLine("<telerik:RadComboBox AutoPostBack=""true"" ID=""rcmb_TypePaiement"" Width=""60px"" runat=""server"">")
                .WriteLine("<Items>")
                .WriteLine("<telerik:RadComboBoxItem runat=""server"" Value=""0"" Text=""Tous"" />")
                .WriteLine("<telerik:RadComboBoxItem runat=""server"" Value=""1"" Text=""Cash"" />")
                .WriteLine("<telerik:RadComboBoxItem runat=""server"" Value=""2"" Text=""Crédit"" />")
                .WriteLine("</Items>")
                .WriteLine("</telerik:RadComboBox>")
                .WriteLine("</div>")
                .WriteLine("<div style=""float: right; margin-top: 3px;"">")
                .WriteLine("<asp:Label ID=""Label1"" Style=""clear: both;"" Font-Size=""12"" runat=""server"" Text=""Rechercher :""></asp:Label>")
                .WriteLine("<telerik:RadTextBox ID=""rtxt_searchString"" runat=""server"">")
                .WriteLine("</telerik:RadTextBox>")
                .WriteLine("<asp:ImageButton ID=""btnSearch"" Style=""clear: both;"" runat=""server"" ImageUrl=""~/img/iconSearch.png"" />")
                .WriteLine("</div>")
                .WriteLine("</div>")
                .WriteLine("</div>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine(" <tr valign=""top"">")
                .WriteLine("<td align=""left"" valign=""top"">")
                .WriteLine(" <telerik:RadGrid ID=""rdg" & nomSimple & """ AllowPaging=""True"" AllowSorting=""True"" PageSize=""20""")
                .WriteLine(" runat=""server"" AutoGenerateColumns=""False"" GridLines=""None"" AllowFilteringByColumn=""true"" ")
                .WriteLine(" EnableViewState=""true"" AllowMultiRowSelection=""false"" OnItemCommand=""rdg" & nomSimple & "_ItemCommand"" GroupingSettings-CaseSensitive=""false"">")
                .WriteLine(" <ExportSettings HideStructureColumns=""true"" />")
                .WriteLine("  <MasterTableView CommandItemDisplay=""Top"" GridLines=""None"" DataKeyNames=""ID"" NoDetailRecordsText=""Pas d'enregistrement""")
                .WriteLine("NoMasterRecordsText=""Pas d'enregistrement"">")
                .WriteLine(" <CommandItemSettings ShowAddNewRecordButton=""false"" ShowRefreshButton=""false"" ShowExportToExcelButton=""false"" ")
                .WriteLine("  ExportToExcelText=""Exporter en excel"" />")


                objWriter.WriteLine("  <NestedViewTemplate>")
                objWriter.WriteLine("                                                                          <asp:Panel runat=""server"" ID=""InnerContainer"" CssClass=""viewWrap"" Visible=""true"">")
                objWriter.WriteLine("                                                                              <telerik:RadTabStrip runat=""server"" ID=""TabStip1"" MultiPageID=""Multipage1"" SelectedIndex=""0"">")
                objWriter.WriteLine("                                                                                  <Tabs>")
                objWriter.WriteLine("                                                                                      <telerik:RadTab runat=""server"" Text=""" & nomSimpleChild & """ PageViewID=""PageView" & nomSimpleChild & """>")
                objWriter.WriteLine("                                                                                      </telerik:RadTab>")
                
                objWriter.WriteLine("                                                                                  </Tabs>")
                objWriter.WriteLine("                                                                              </telerik:RadTabStrip>")
                objWriter.WriteLine("                                                                              <telerik:RadMultiPage runat=""server"" ID=""Multipage1"" SelectedIndex=""0"" RenderSelectedPageOnly=""false"">")
                objWriter.WriteLine("                                                                                  <telerik:RadPageView runat=""server"" ID=""PageView" & nomSimpleChild & """>")
                objWriter.WriteLine("                                                                                      <%-- <asp:Label ID=""Label1"" Font-Bold=""true"" Font-Italic=""true"" Text='<%# Eval(""ID"") %>'")
                objWriter.WriteLine("                                                                                          Visible=""false"" runat=""server""></asp:Label>--%>")
                objWriter.WriteLine("                                                                                      <telerik:RadGrid runat=""server"" ID=""rdg" & nomSimpleChild & """ OnNeedDataSource=""" & nomSimpleChild & "_NeedDataSource"" ShowFooter=""true"" AllowSorting=""true""")
                objWriter.WriteLine("                                                                                          EnableLinqExpressions=""false"">")
                objWriter.WriteLine("                                                                                          <MasterTableView ShowHeader=""true"" AutoGenerateColumns=""False"" AllowPaging=""true""")
                objWriter.WriteLine("                                                                                              DataKeyNames=""ID"" PageSize=""7"">")
                objWriter.WriteLine("                                                                                              <Columns>")


                .WriteLine("<telerik:GridBoundColumn DataField=""ID"" UniqueName=""ID"" Display=""false"">")
                .WriteLine("<ItemStyle Width=""1px"" />")
                .WriteLine("</telerik:GridBoundColumn>")

                Dim countColumn As Integer = 0
                Dim pourcentagevalue As Decimal = 100 / (group.ChildTable.ListofColumn.Count - 4)
                Dim pourcentage As String = pourcentagevalue.ToString + "%"
                For Each column As Cls_Column In group.ChildTable.ListofColumn
                    Dim listforeignkey As List(Of Cls_Column) = group.ChildTable.ListofColumnForeignKey
                    Dim primary As String = group.ChildTable.ListofColumn(0).Name
                    If countColumn < group.ChildTable.ListofColumn.Count - 4 Then

                        If group.ChildTable.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "date" And column.Name <> primary Then
                            Dim foreignkey As New Cls_ForeignKey(group.ChildTable.ID, column.ID)
                            Dim textForcombo As String = ""
                            Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                            Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                            Dim countcolumnref As Long
                            For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                                If column_fore.Type.VbName = "String" Then
                                    textForcombo = column_fore.Name
                                    Exit For
                                End If

                            Next
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & reftablename & "_" & textForcombo & """ UniqueName=""" & reftablename & "_" & textForcombo & """ HeaderText=""" & reftablename & "_" & textForcombo & """")
                            .WriteLine(" FilterControlAltText=""Filter " & reftablename & "_" & textForcombo & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")

                            '.WriteLine("                   <ItemTemplate>")
                            '.WriteLine("<asp:HyperLink ID=""hlk"" runat=""server"" Font-Underline=""true"" ToolTip=""Cliquer pour visualiser"" Text='<%# Bind(""" & nomSimple & "_" & textForcombo & """) %>' NavigateUrl=""#""></asp:HyperLink>")
                            '.WriteLine("<asp:Label ID=""lbl_ID"" runat=""server"" Visible=""false"" Text='<%# Bind(""ID"") %>'></asp:Label>")
                            '.WriteLine("</ItemTemplate>")
                            .WriteLine("</telerik:GridBoundColumn>")
                        ElseIf Not group.ChildTable.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> group.ChildTable.ListofColumn(0).Name Then
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                            .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        ElseIf Not group.ChildTable.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> group.ChildTable.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                            .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" DataFormatString=""{0:##,###,##0.00}"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        ElseIf column.TrueSqlServerType = "date" And column.Name <> group.ChildTable.ListofColumn(0).Name Then
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                            .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" DataFormatString=""{0:dd/MM/yyyy}"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        End If
                    End If
                    countColumn = countColumn + 1

                Next


                objWriter.WriteLine("                                                                                              </Columns>")
                objWriter.WriteLine("                                                                                          </MasterTableView>")
                objWriter.WriteLine("                                                                                      </telerik:RadGrid>")
                objWriter.WriteLine("                                                                                  </telerik:RadPageView>")
                objWriter.WriteLine("                                                                              </telerik:RadMultiPage>")
                objWriter.WriteLine("                                                                          </asp:Panel>")
                objWriter.WriteLine("                                                                      </NestedViewTemplate>")




                .WriteLine(" <Columns>")

                .WriteLine("<telerik:GridBoundColumn DataField=""ID"" UniqueName=""ID"" Display=""false"">")
                .WriteLine("<ItemStyle Width=""1px"" />")
                .WriteLine("</telerik:GridBoundColumn>")

                countColumn = 0
                pourcentagevalue = 100 / (_table.ListofColumn.Count - 4)
                pourcentage = pourcentagevalue.ToString + "%"
                For Each column As Cls_Column In _table.ListofColumn
                    Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                    Dim primary As String = _table.ListofColumn(0).Name
                    If countColumn < _table.ListofColumn.Count - 4 Then

                        If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "date" And column.Name <> primary Then
                            Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                            Dim textForcombo As String = ""
                            Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                            Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                            Dim countcolumnref As Long
                            For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                                If column_fore.Type.VbName = "String" Then
                                    textForcombo = column_fore.Name
                                    Exit For
                                End If

                            Next
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & reftablename & "_" & textForcombo & """ UniqueName=""" & reftablename & "_" & textForcombo & """ HeaderText=""" & reftablename & "_" & textForcombo & """")
                            .WriteLine(" FilterControlAltText=""Filter " & reftablename & "_" & textForcombo & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")

                            '.WriteLine("                   <ItemTemplate>")
                            '.WriteLine("<asp:HyperLink ID=""hlk"" runat=""server"" Font-Underline=""true"" ToolTip=""Cliquer pour visualiser"" Text='<%# Bind(""" & nomSimple & "_" & textForcombo & """) %>' NavigateUrl=""#""></asp:HyperLink>")
                            '.WriteLine("<asp:Label ID=""lbl_ID"" runat=""server"" Visible=""false"" Text='<%# Bind(""ID"") %>'></asp:Label>")
                            '.WriteLine("</ItemTemplate>")
                            .WriteLine("</telerik:GridBoundColumn>")
                        ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name Then
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                            .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                            .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" DataFormatString=""{0:##,###,##0.00}"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        ElseIf column.TrueSqlServerType = "date" And column.Name <> _table.ListofColumn(0).Name Then
                            .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                            .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" DataFormatString=""{0:dd/MM/yyyy}"" ShowFilterIcon=""false""")
                            .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        End If
                    End If
                    countColumn = countColumn + 1

                Next

                .WriteLine(" <telerik:GridButtonColumn ButtonType=""ImageButton"" CommandArgument=""ID"" CommandName=""editer""")
                .WriteLine("        DataTextField=""ID"" HeaderStyle-HorizontalAlign=""Right"" ImageUrl=""~/images/_edit.png""")
                .WriteLine("        ItemStyle-HorizontalAlign=""Right"" UniqueName=""editer"">")
                .WriteLine("        <HeaderStyle HorizontalAlign=""Right"" Width=""1%"" />")
                .WriteLine("        <ItemStyle HorizontalAlign=""Right"" Width=""1%"" />")
                .WriteLine("    </telerik:GridButtonColumn>")

                .WriteLine("<telerik:GridButtonColumn ButtonType=""ImageButton"" CommandName=""delete"" DataTextField=""ID""")
                .WriteLine("HeaderStyle-HorizontalAlign=""Right"" ImageUrl=""~/images/delete.png"" ItemStyle-HorizontalAlign=""Right""")
                .WriteLine("UniqueName=""delete"" ConfirmDialogType=""RadWindow"" ConfirmText=""Voulez-vous vraiment supprimer ce " & nomSimple & "?""")
                .WriteLine("ConfirmTitle=""Attention!"">")
                .WriteLine("<HeaderStyle HorizontalAlign=""Right"" Width=""1%"" />")
                .WriteLine("<ItemStyle HorizontalAlign=""Right"" Width=""1%"" />")
                .WriteLine("</telerik:GridButtonColumn>")

                .WriteLine("  </Columns>")
                .WriteLine("<RowIndicatorColumn FilterControlAltText=""Filter RowIndicator column"">")
                .WriteLine("</RowIndicatorColumn>")
                .WriteLine("<ExpandCollapseColumn FilterControlAltText=""Filter ExpandColumn column"">")
                .WriteLine("</ExpandCollapseColumn>")
                .WriteLine("</MasterTableView>")

                .WriteLine("<ClientSettings>")
                .WriteLine("<ClientEvents ></ClientEvents>")
                .WriteLine("<Selecting AllowRowSelect=""true"" />")
                .WriteLine("</ClientSettings>")
                .WriteLine("<HeaderContextMenu />")
                .WriteLine("<FilterMenu EnableImageSprites=""False"">")
                .WriteLine("</FilterMenu>")
                .WriteLine("</telerik:RadGrid>")
                .WriteLine("<telerik:RadWindowManager ID=""RadWindowManager1"" runat=""server"" VisibleStatusbar=""false""")
                .WriteLine("EnableViewState=""false"">")
                .WriteLine("<Windows>")
                .WriteLine("<telerik:RadWindow ID=""AddUpdateDialog"" runat=""server"" Title="""" Left=""75px"" ReloadOnShow=""true""")
                .WriteLine(" ShowContentDuringLoad=""false"" Modal=""true"" OnClientClose=""RadWindowClosing"" Behaviors=""Minimize, Move, Resize, Maximize, Close""")
                .WriteLine("EnableShadow=""true"" OnClientResizeEnd=""RadWindowClientResizeEnd"" />")
                .WriteLine("</Windows>")
                .WriteLine("</telerik:RadWindowManager>")


                .WriteLine("<telerik:RadContextMenu ID=""ContextMenu"" runat=""server"" OnClientItemClicked=""""")
                .WriteLine("EnableRoundedCorners=""true"" EnableShadows=""true"">")
                .WriteLine("<Items>")
                .WriteLine("<telerik:RadMenuItem Text=""Editer"" />")
                .WriteLine("</Items>")
                .WriteLine("</telerik:RadContextMenu>")


                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("</table>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("</table>")


                .WriteLine("</asp:Panel>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("</table>")
                .WriteLine("</td>")
                .WriteLine("</tr>")
                .WriteLine("</table>")
            End With



            objWriter.WriteLine("<br />")
            objWriter.WriteLine("<input id=""txtWindowPage"" type=""hidden"" />")
            objWriter.WriteLine("</center>")
            objWriter.WriteLine("</asp:Content>")

            objWriter.WriteLine()
            objWriter.Close()
        End Sub


    End Class

End Namespace
