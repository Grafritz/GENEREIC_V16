Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.ComponentModel
Imports System.Text.RegularExpressions

Namespace SqlServer.Fast

    Public Class AspFormGenerator
        Public Shared Sub CreateInterfaceCodeBehind(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_LibraryName As TextBox, ByVal databasename As String)
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim _PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
            Dim path As String = _PathGenerate_Script & nomWebform & "ADD.aspx.vb"

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
            Dim content As String = "Partial Class " & nomWebform & "ADD" & Chr(13) _
                                     & " Inherits Cls_BasePage ' LA CLASSE DE LA PAGE HERITE DE CETTE CLASSE DANS LE CAS OU NOUS AVONS UNE APPLICATION WEB multilingue"

            _end = "End Class" & Chr(13)
            ' Delete the file if it exists.
            If File.Exists(path) Then
                File.Delete(path)
            End If

            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(_PathGenerate_Script) Then
                Directory.CreateDirectory(_PathGenerate_Script)
            End If
            ' Create the file.
            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()



            Dim objWriter As New System.IO.StreamWriter(path, True)
            ' objWriter.WriteLine(header)
            If ListBox_NameSpace.Items.Count > 0 Then
                For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                    objWriter.WriteLine(ListBox_NameSpace.Items(i))
                Next
            End If
            Dim libraryname As String = "Imports " & txt_LibraryName.Text
            objWriter.WriteLine("Imports Telerik.Web.UI")
            objWriter.WriteLine(libraryname)
            objWriter.WriteLine()

            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            objWriter.WriteLine(content)
            objWriter.WriteLine()

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
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

            Dim _tmpEditState As Boolean = False
            'objWriter.WriteLine("Dim _out As Boolean = False")
            'objWriter.WriteLine("Dim _tmpEditState As Boolean = False")

            With objWriter
                .WriteLine("")
                .WriteLine("#Region ""ATTRIBUTS"" ")
                .WriteLine("    Private _message As String  ' VARIABLE SERVANT A LA RECUPERATION DE TOUS LES MESSAGES D'ECHECS OU DE SUCCES")
                .WriteLine("")
                .WriteLine("    REM DEFINITION ET INITIALISATION DES CONSTANTE POUR LA SECURITE")
                .WriteLine("    Private Const Nom_page As String = ""PAGE-FORMULAIRE-DOSSIER-ENFANT""  ' POUR LA PAGE")
                .WriteLine("    Private Const Btn_Save As String = ""Bouton-SAVE-DOSSIER-ENFANT""       ' POUR LE BOUTON D'ENREGISTREMENT")
                .WriteLine("    Private Const Btn_Edit As String = ""Bouton-EDIT-DOSSIER-ENFANT""       ' POUR LE BOUTON DE MODIFICATION")
                .WriteLine("    Private Const Btn_Delete As String = ""Bouton-DELETE-DOSSIER-ENFANT""   ' POUR LE BOUTON DE SUPPRESSION")
                .WriteLine("")
                .WriteLine("    Dim User_Connected As Cls_User          ' INSTANCE DE LA CLASSE UTILISATEUR - UTILISER POUR L'UTILISATEUR EN SESSION ")
                .WriteLine("    Dim Is_Acces_Page As Boolean = True     ' LA VARIABLE SERVANT DE TEST POUR DONNEER L'ACCES A LA PAGE")
                .WriteLine("    Dim GetOut As Boolean = False           ' LA VARIABLE SERVANT DE TEST POUR REDIRIGER L'UTILISATEUR VERS LA PAGE DE CONNEXION")
                .WriteLine("    Private IS_SendMail As Boolean = True   ' PAS TROP IMPORTANT...")
                .WriteLine("#End Region")
                .WriteLine("")
            End With

            With objWriter
                .WriteLine("Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load ")
                .WriteLine("    Response.Cache.SetCacheability(HttpCacheability.NoCache) ")
                .WriteLine("    Response.Expires = -1 ")
                .WriteLine("    Panel_Msg.Visible = False ")
                .WriteLine(" ")
                .WriteLine("    SYSTEME_SECURITE()  ' APPEL A LA METHODE SERVANT A TESTER LES COMPOSANTS DE LA PAGE Y COMPRIS LA PAGE ELLE MEME ")
                .WriteLine("")
                .WriteLine("    '--- Si l'utilisateur n'a Access a la page les informations ne sont pas charger dans la Page_Load ")
                .WriteLine("    If Is_Acces_Page Then ")
                .WriteLine("        If Not IsPostBack Then ")
                .WriteLine("            btnCancel.Attributes.Add(""onclick"", ""javascript:void(closeWindow());"")")
                .WriteLine("            'rbtnAdd" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx', 550, 400)); return false;"")") '& Chr(13) _
                .WriteLine("            'BtnADDNew.Attributes.Add(""onclick"", ""javascript:Open_Window('Frm_" & nomSimple & "ADD.aspx', '_self',500,400); return false;"") ")
                .WriteLine("            Load_ALL_DATA() ")
                .WriteLine("        End If ")
                .WriteLine("    End If ")
                .WriteLine("End Sub ")
                .WriteLine(" ")
            End With

            With objWriter
                .WriteLine("#Region ""SECURITE""")
                .WriteLine("    Public Sub SYSTEME_SECURITE()")
                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liGROUPE_PARAMETRES""), HtmlControl).Attributes.Add(""class"", ""active treeview"")")
                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liCentreDeDetentionListe""), HtmlControl).Attributes.Add(""class"", ""active"")")
                .WriteLine("")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) IsNot Nothing Then")
                .WriteLine("            User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)")
                .WriteLine("            If Not Cls_Privilege.VerifyRightOnObject(Nom_page, User_Connected.IdGroupeuser) Then    ' VERIFICATION SI L'UTILISATEUR N'A PAS ACCES A LA PAGE")
                .WriteLine("                _message = [Global].NO_ACCES_PAGE")
                .WriteLine("                MessageToShow(_message)")
                .WriteLine("                Is_Acces_Page = False")
                .WriteLine(" ")
                .WriteLine("                Panel_First.Visible = False")
                .WriteLine("            Else    ' SI L'UTILISATEUR A ACCES A LA PAGE ON VERIFIE POUR LES BOUTONS ET LES LIENS")
                .WriteLine("                '---  Okey vous avez acces a la page ---'")
                .WriteLine("                btnSave.Text=""Enregistrer""")
                .WriteLine("                btnSave.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Save, User_Connected.IdGroupeuser)")
                .WriteLine("                If Request.QueryString(""ID"") IsNot Nothing Then")
                .WriteLine("                    btnSave.Text=""Modifier""")
                .WriteLine("                    btnSave.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Edit, User_Connected.IdGroupeuser)")
                .WriteLine("                End If")
                .WriteLine("            End If")
                .WriteLine("        End If")
                .WriteLine(" ")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) Is Nothing Then")
                .WriteLine("            '-- Session expirée --'")
                .WriteLine("            GetOut = True")
                .WriteLine("        Else")
                .WriteLine("            Try")
                .WriteLine("                User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)  ' ON VERIFIE SI LÚTILISATEUR A ETE FORCE DE SE CONNECTER 'PAR L'ADM")
                .WriteLine("                If Not (GlobalFunctions.IsUserStillConnected(User_Connected) And GlobalFunctions.IsUserStillActive(User_Connected)) Then")
                .WriteLine("                    User_Connected.Set_Status_ConnectedUser(False)")
                .WriteLine("                    User_Connected.Activite_Utilisateur_InRezo(""Forced Log Off"", ""Forced to Log Off"", Request.UserHostAddress)")
                .WriteLine("")
                .WriteLine("                    GetOut = True")
                .WriteLine("                    Session.RemoveAll()")
                .WriteLine("                    _message = ""Session expirée.""")
                .WriteLine("                    MessageToShow(_message)")
                .WriteLine("                    Is_Acces_Page = True")
                .WriteLine("                End If")
                .WriteLine("            Catch ex As Exception")
                .WriteLine("                GetOut = True")
                .WriteLine("                _message = ""Session expirée.""")
                .WriteLine("                MessageToShow(_message)")
                .WriteLine("            End Try")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("    If GetOut Then ' REDIRECTIONNEMENT DE L'UTILISATUER OU PAS.")
                .WriteLine("Response.Redirect([Global].PAGE_LOGIN)")
                .WriteLine("End If")
                .WriteLine("End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""Other Method - MessageToShow""")
                .WriteLine("    Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"")")
                .WriteLine("        Panel_Msg.Visible = True")
                .WriteLine("        GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
                .WriteLine("        Label_Msg.Text = _message")
                .WriteLine("        RadAjaxManager1.ResponseScripts.Add(""alert('"" & _message.Replace(""'"", ""\'"") & ""');"")")
                .WriteLine("        If E_or_S = ""S"" Then")
                .WriteLine("            Label_Msg.ForeColor = Drawing.Color.Green")
                .WriteLine("        Else")
                .WriteLine("            Label_Msg.ForeColor = Drawing.Color.Red")
                .WriteLine("        End If")
                .WriteLine("    End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""Load DATA""")
                .WriteLine("Private Sub LOAD_ALL_DATA()")
                ''---------------------------------''
                For Each fk In ListofForeignKey
                    Dim NameFileCombo1 As String = "FillCombo" & fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                    .WriteLine("" & NameFileCombo1 & "()")
                Next
                ''---------------------------------''
                .WriteLine("LOAD_" & nomSimple.ToUpper & "()")
                .WriteLine("End Sub")
                .WriteLine()

                .WriteLine("Private Sub LOAD_" & nomSimple.ToUpper & "()")
                .WriteLine("Try")
                .WriteLine("    If Request.QueryString(""ID"") IsNot Nothing Then")
                .WriteLine("        txt_Code" & nomSimple & "_Hid.Text = TypeSafeConversion.NullSafeLong(Request.QueryString(""ID""))")
                .WriteLine("        Dim obj as New " & nomClasse & "( TypeSafeConversion.NullSafeLong(txt_Code" & nomSimple & "_Hid.Text))")
                .WriteLine("        If obj.ID > 0 Then")
                .WriteLine("            btnSave.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Edit, User_Connected.IdGroupeuser)")
                .WriteLine("            With obj")
                For i As Int32 = 1 To cols.Count - 3
                    If ListofForeignKey.Contains(cols(i)) Then
                        .WriteLine("rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " .SelectedIndex =  rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & ".FindItemIndexByValue(." & cols(i).Substring(1, cols(i).Length - 1) & ")")
                    ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                        .WriteLine("rdp" & cols(i) & ".SelectedDate = ." & cols(i).Substring(1, cols(i).Length - 1))
                    ElseIf types(i) = "Boolean" Then
                        .WriteLine("chk" & cols(i) & ".Checked = ." & cols(i).Substring(1, cols(i).Length - 1))
                    Else
                        .WriteLine("rtxt" & cols(i) & ".Text = ." & cols(i).Substring(1, cols(i).Length - 1))
                    End If
                Next
                .WriteLine("            End With")
                .WriteLine("        End If")
                .WriteLine("    Else")
                .WriteLine()
                .WriteLine("    End If")
                .WriteLine("Catch ex As Exception")
                .WriteLine("    _message = ex.Message")
                .WriteLine("    MessageToShow(_message)")
                .WriteLine("    _message = "" Methode -> [ LOAD_" & nomSimple.ToUpper & " ] :"" & ex.Message")
                .WriteLine("    [Global].WriteError(_message, User_Connected)")
                .WriteLine("End Try")
                .WriteLine("End Sub")

                ''-------------------------------------------------''
                objWriter.WriteLine()

                For Each fk In ListofForeignKey
                    'objWriter.WriteLine("FillCombo" & fk & "()")

                    Dim textForcombo As String = ""
                    Dim attributUsed As String = fk.Substring(SqlServerHelper.ForeinKeyPrefix.Length, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                    Dim nomforeign As String = fk.Substring(SqlServerHelper.ForeinKeyPrefix.Length, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                    Dim ClassName As String = "Cls" & fk.Substring(SqlServerHelper.ForeinKeyPrefix.Length, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length))

                    Dim NameFileCombo As String = "FillCombo" & fk.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

                    textForcombo = nomforeign.Substring(1, nomforeign.Length - 1)
                    .WriteLine("Private Sub " & NameFileCombo & "()")
                    .WriteLine("Try")
                    .WriteLine("Dim objs1 As List(of " & ClassName & ") = " & ClassName & ".SearchAll ")

                    .WriteLine("With rcmb" & nomforeign)
                    .WriteLine("    .Datasource = objs1")
                    .WriteLine("    .DataValueField = ""ID""")
                    .WriteLine("    .DataTextField = """ & textForcombo & """")
                    .WriteLine("    .DataBind()")
                    .WriteLine("    .Items.Sort()")
                    .WriteLine("    .Items.Insert(0, New RadComboBoxItem("" - Choisir -"", """"))")
                    .WriteLine("    .SelectedIndex = 0")
                    .WriteLine("    .EmptyMessage = ""- Choisir -""")
                    .WriteLine("End With")
                    .WriteLine("Catch ex As Exception")
                    .WriteLine("    _message = ex.Message")
                    .WriteLine("    MessageToShow(_message)")
                    .WriteLine("    _message = "" Methode -> [ " & NameFileCombo & " ] :"" & ex.Message")
                    .WriteLine("    [Global].WriteError(_message, User_Connected)")
                    .WriteLine("End Try")
                    .WriteLine("End Sub")
                    .WriteLine()
                Next
                ''-------------------------------------------------''
                .WriteLine("#End Region")
            End With

            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""METHODES - SAVE""")
            With objWriter
                .WriteLine("Private Sub SAVE_" & nomSimple.ToUpper & "()")
                .WriteLine("Try")
                .WriteLine("Dim _id As Long = TypeSafeConversion.NullSafeLong(txt_Code" & nomSimple & "_Hid.Text)")
                .WriteLine("Dim obj As New " & nomClasse & "(_id)")
                .WriteLine("With obj")
                For i As Int32 = 1 To cols.Count - 3
                    Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                    If ListofForeignKey.Contains(cols(i)) Then
                        .WriteLine("." & columnToUse & "  =   rcmb_" & columnToUse.Substring(SqlServerHelper.ForeinKeyPrefix.Length, columnToUse.Length - (SqlServerHelper.ForeinKeyPrefix.Length)) & " .SelectedValue ")
                    ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                        .WriteLine("." & columnToUse & " = rdp_" & columnToUse & " .SelectedDate ")
                    ElseIf types(i) = "Boolean" Then
                        ' .WriteLine("." & columnToUse & " =  radio_yes_" & columnToUse & " .Checked")
                        .WriteLine("." & columnToUse & " =  chk_" & columnToUse & " .Checked")
                    Else
                        .WriteLine("." & columnToUse & " = rtxt_" & columnToUse & ".Text ")
                    End If
                Next
                .WriteLine("End With")
                .WriteLine("obj.Save(User_Connected.Username)")
                .WriteLine("REM TRACE UTILUSATEUR / Trace Transaction")
                .WriteLine("User_Connected.Activite_Utilisateur_InRezo(IIf(TypeSafeConversion.NullSafeLong(_id) > 0, ""ADD"", ""EDIT"") & "" " & nomSimple & """, "" ID:"" & obj.ID , Request.UserHostAddress)")
                .WriteLine("txt_Code" & nomSimple & "_Hid.Text=obj.ID")

                .WriteLine("    _message = ""Sauvegarde Effectuée""")
                .WriteLine("    MessageToShow(_message, ""S"")")
                .WriteLine("    RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe" & nomSimple & "();"")")
                .WriteLine("Catch ex As Exception")
                .WriteLine("    _message = ex.Message & Chr(13) & "" La sauvegarde a échouée!""")
                .WriteLine("    MessageToShow(_message)")
                .WriteLine("    _message = "" Methode -> [ SAVE_" & nomSimple.ToUpper & " ] :"" & ex.Message")
                .WriteLine("    [Global].WriteError(_message, User_Connected)")
                .WriteLine("End Try")
                '.WriteLine("  End Select")
                .WriteLine("End Sub")
            End With
            objWriter.WriteLine("#End Region")

            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""EVENTS BUTTON""")
            With objWriter
                .WriteLine("Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click")
                .WriteLine("SAVE_" & nomSimple.ToUpper & "()")
                .WriteLine("End Sub")
            End With
            objWriter.WriteLine("#End Region")

            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterfaceCodeAsp(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
            '  Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            'Dim RepertoiresName As String = Cls_Enumeration.PATH_GENERIC_FOLDER_DEFAULT
            Dim _PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
            Dim path As String = _PathGenerate_Script & nomClasse & "ADD.aspx"

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
            If Not Directory.Exists(_PathGenerate_Script) Then
                Directory.CreateDirectory(_PathGenerate_Script)
            End If

            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()
            Dim objWriter As New System.IO.StreamWriter(path, True)

            'objWriter.WriteLine()

            'objWriter.WriteLine()

            Dim _table As New Cls_Table()
            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            '  _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next


            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If

            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next

            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then ' - 4
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
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

            objWriter.WriteLine("<%@ Page Title=""" & nomSimple & """ Language=""VB"" MasterPageFile=""~/MasterPage/DashboardMasterPage.master"" " & _
           " AutoEventWireup=""false"" CodeFile=""Frm_" & nomSimple & "ADD.aspx.vb"" Inherits=""Frm_" & nomSimple & "ADD""  MaintainScrollPositionOnPostback=""true"" %>")

            objWriter.WriteLine("<asp:Content ID=""Content1"" ContentPlaceHolderID=""ContentPlaceHolder1"" runat=""Server"">")

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

            objWriter.WriteLine("<%--<telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server""></telerik:RadScriptManager>--%>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("<AjaxSettings>")
            objWriter.WriteLine("<telerik:AjaxSetting AjaxControlID=""btnSave"">")
            objWriter.WriteLine("<UpdatedControls>")
            objWriter.WriteLine(" <telerik:AjaxUpdatedControl ControlID=""Panel_Msg"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("           <telerik:AjaxUpdatedControl ControlID=""Panel_First"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("</UpdatedControls>")
            objWriter.WriteLine("</telerik:AjaxSetting>")
            objWriter.WriteLine("</AjaxSettings>")
            objWriter.WriteLine("</telerik:RadAjaxManager>")
            'objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7""> </telerik:RadSkinManager>")
            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")

            With objWriter
                .WriteLine("   <section class=""content-header""> ")
                .WriteLine("       <h1> ")
                .WriteLine("           <asp:Label ID=""LabelTitre"" runat=""server"" Text=""" & nomSimple & """ /> ")
                .WriteLine("           <small> <asp:Label ID=""LabelSousTitre"" runat=""server"" /></small> ")
                .WriteLine("       </h1> ")
                .WriteLine("       <%--<ol class=""breadcrumb""> ")
                .WriteLine("           <li><a href=""#""><i class=""fa fa-dashboard""></i>Accueil</a></li>")
                .WriteLine("           <li class=""active"">" & nomSimple & "</li>")
                .WriteLine("           <asp:SiteMapPath ID=""sitmape111"" runat=""server"" ></asp:SiteMapPath>")
                .WriteLine("       </ol>  --%>")
                .WriteLine("   </section> ")
            End With

            With objWriter
                .WriteLine("   <asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false""> ")
                .WriteLine("       <div class=""alert alert-warning""> ")
                .WriteLine("           <asp:Image ID=""Image_Msg"" runat=""server"" /> ")
                .WriteLine("           <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label> ")
                .WriteLine("       </div> ")
                .WriteLine("   </asp:Panel> ")
            End With

            objWriter.WriteLine("<asp:ValidationSummary ID=""ValidationSummary1"" runat=""server"" CssClass=""alert alert-warning"" ShowMessageBox=""true"" ShowSummary=""true"" />")

            objWriter.WriteLine("  ")
            objWriter.WriteLine("<section class=""content"" >")
            objWriter.WriteLine("   <asp:Panel ID=""Panel_First"" runat=""server"" CssClass=""panel panel-default panel-body"">")

            objWriter.WriteLine("<table border=""0"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")
            Dim countColumn As Integer = 0

            For i As Int32 = 1 To cols.Count '- 3
                If ListofForeignKey.Contains(cols(i)) Then
                    Dim columnName As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ Text=""" & columnNameToShow & " : "" runat=""server"" /> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadComboBox ID=""rcmb_" & columnName & """  Width=""80%"" runat=""server""> " & Chr(13) & " </telerik:RadComboBox> ")
                    objWriter.WriteLine(" <asp:RequiredFieldValidator  ID=""RequiredFieldValidator_" & columnName & """ runat=""server"" ControlToValidate=""rcmb_" & columnName & """  ")
                    objWriter.WriteLine("            ErrorMessage=""Le " & columnName & " est obligatoire !"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" /> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ Text=""" & columnNameToShow & " : "" runat=""server"" /> ")
                    objWriter.WriteLine("</td>")

                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadDatePicker ID=""rdp_" & columnName & """ Width=""50%"" runat=""server""" &
                                     "DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01""  ToolTip=""Cliquer sur le bouton pour choisir une date"" > " & Chr(13) & " </telerik:RadDatePicker>  ")
                    objWriter.WriteLine(" <asp:RequiredFieldValidator  ID=""RequiredFieldValidator_" & columnName & """ runat=""server"" ControlToValidate=""rdp_" & columnName & """  ")
                    objWriter.WriteLine("           ErrorMessage=""La " & columnNameToShow & " est obligatoire !"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" /> ")
                    objWriter.WriteLine("</td>")

                    objWriter.WriteLine("</tr>")
                ElseIf types(i) = "Boolean" And cols(i) <> "isdirty" Then
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ Text=""" & columnNameToShow & " : "" runat=""server"" /> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("       <asp:CheckBox ID=""chk_" & columnName & """ runat=""server"" text=""" & columnName & """ />  ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                Else
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])_?([A-Z])", "$1 $2")
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ Text=""" & columnNameToShow & " : "" runat=""server"" /> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadTextBox ID=""rtxt_" & columnName & """ Width=""80%"" runat=""server""> " & Chr(13) & " </telerik:RadTextBox> ")
                    objWriter.WriteLine(" <asp:RequiredFieldValidator ID=""RequiredFieldValidator_" & columnName & """ runat=""server"" ControlToValidate=""rtxt_" & columnName & """  ")
                    objWriter.WriteLine("           ErrorMessage=""Le " & columnNameToShow & " est obligatoire !""  SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" /> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                End If
            Next

            With objWriter
                .WriteLine("<tr>")
                .WriteLine("   <td style=""width: 30%;"" align=""right"">")
                .WriteLine("   </td>")
                .WriteLine("   <td style=""width: 70%;"" align=""left"">")
                .WriteLine("       <telerik:RadButton ID=""btnSave"" runat=""server""  Text=""Sauvegarder"" > ")
                .WriteLine("           <Icon PrimaryIconCssClass=""rbSave"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
                .WriteLine("       </telerik:RadButton>")
                .WriteLine("       &nbsp;")
                .WriteLine("       <telerik:RadButton ID=""btnCancel"" runat=""server"" CausesValidation=""False""   Text = ""Annuler"" >")
                .WriteLine("           <Icon PrimaryIconCssClass=""rbCancel"" PrimaryIconLeft=""4"" PrimaryIconTop=""4""></Icon>")
                .WriteLine("       </telerik:RadButton>")
                .WriteLine("   </td>")
                .WriteLine("</tr>")
            End With
            objWriter.WriteLine(" </table>")

            objWriter.WriteLine("   </asp:Panel>") 'FIN PANEL
            objWriter.WriteLine("   <input id=""txtWindowPage"" type=""hidden"" />")
            objWriter.WriteLine("<asp:TextBox ID=""txt_Code" & nomSimple & "_Hid"" runat=""server"" Text=""0"" Visible=""False"" Width=""1px""></asp:TextBox>")
            objWriter.WriteLine("</section>") ' FIN Section Content
            objWriter.WriteLine("</asp:Content>")
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterfaceCodeAsp(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String, ByVal idTypeControl As Long)
            '  Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            'Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
            'Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & databasename & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & databasename & "\ASPWebForm\")
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
            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            '  _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next


            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If

            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next

            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
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

            objWriter.WriteLine(" <table border=""0"" class=""form_Enregistrement"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")
            objWriter.WriteLine("<tr valign=""top"">")
            objWriter.WriteLine(" <td style=""width: 100%;"">")

            objWriter.WriteLine("  <asp:Panel ID=""pn_entete"" runat=""server"" Width=""100%"">")
            objWriter.WriteLine("<table border=""0"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")

            Dim countColumn As Integer = 0

            For i As Int32 = 1 To cols.Count - 3
                If ListofForeignKey.Contains(cols(i)) Then
                    Dim columnName As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
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
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ CssClass=""popupFormInputLabel"" Text=""" & columnName & " : "" runat=""server""></asp:Label> ")
                    objWriter.WriteLine("</td>")

                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadDatePicker ID=""rdp_" & columnName & """ Width=""50%"" runat=""server""" & _
                                     "DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01""  ToolTip=""Cliquer sur le bouton pour choisir une date"" > " & Chr(13) & " </telerik:RadDatePicker>  ")
                    objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & columnName & """ runat=""server"" ControlToValidate=""rdp_" & columnName & """  ")
                    objWriter.WriteLine("  ErrorMessage=""La " & columnName & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                    objWriter.WriteLine("</td>")

                    objWriter.WriteLine("</tr>")
                ElseIf types(i) = "Boolean" And cols(i) <> "isdirty" Then
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ CssClass=""popupFormInputLabel"" Text=""" & columnName & " : "" runat=""server""></asp:Label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<asp:CheckBox ID=""chk_" & columnName & """ Width=""50%"" runat=""server"">" & _
                                      " </asp:CheckBox>  ")
                    objWriter.WriteLine("</td>")

                    objWriter.WriteLine("</tr>")
                Else
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<asp:Label ID=""lbl_" & columnName & """ CssClass=""popupFormInputLabel"" Text=""" & columnName & " : "" runat=""server""></asp:Label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadTextBox ID=""rtxt_" & columnName & """ Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadTextBox> ")
                    objWriter.WriteLine(" <asp:RequiredFieldValidator CssClass=""error_color"" ID=""RequiredFieldValidator_" & columnName & """ runat=""server"" ControlToValidate=""rtxt_" & columnName & """  ")
                    objWriter.WriteLine("  ErrorMessage=""Le " & columnName & " est obligatoire !"" ValidationGroup=""" & nomSimple & "Group"">*</asp:RequiredFieldValidator> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                End If
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
            objWriter.WriteLine(" </td>")
            objWriter.WriteLine("</tr>")
            objWriter.WriteLine("</table>")
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

        Public Shared Sub CreateListingCodeBehind(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_libraryname As TextBox, ByVal databasename As String)
            ' Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
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
                                     & " Inherits Cls_BasePage ' LA CLASSE DE LA PAGE HERITE DE CETTE CLASSE DANS LE CAS OU NOUS AVONS UNE APPLICATION WEB multilingue"

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
            objWriter.WriteLine()

            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            objWriter.WriteLine(content)
            objWriter.WriteLine()


            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
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

            'objWriter.WriteLine("Dim _out As Boolean = False")
            'objWriter.WriteLine("Dim _tmpEditState As Boolean = False")

            With objWriter
                .WriteLine("")
                .WriteLine("#Region ""ATTRIBUTS"" ")
                .WriteLine("    Private _message As String  ' VARIABLE SERVANT A LA RECUPERATION DE TOUS LES MESSAGES D'ECHECS OU DE SUCCES")
                .WriteLine("")
                .WriteLine("    REM DEFINITION ET INITIALISATION DES CONSTANTE POUR LA SECURITE")
                .WriteLine("    Private Const Nom_page As String = ""PAGE-LISTING-DOSSIER-ENFANT""  ' POUR LA PAGE")
                .WriteLine("    Private Const Btn_Save As String = ""Bouton-SAVE-DOSSIER-ENFANT""       ' POUR LE BOUTON D'ENREGISTREMENT")
                .WriteLine("    Private Const Btn_Edit As String = ""Bouton-EDIT-DOSSIER-ENFANT""       ' POUR LE BOUTON DE MODIFICATION")
                .WriteLine("    Private Const Btn_Delete As String = ""Bouton-DELETE-DOSSIER-ENFANT""   ' POUR LE BOUTON DE SUPPRESSION")
                .WriteLine("")
                .WriteLine("    Dim User_Connected As Cls_User          ' INSTANCE DE LA CLASSE UTILISATEUR - UTILISER POUR L'UTILISATEUR EN SESSION ")
                .WriteLine("    Dim Is_Acces_Page As Boolean = True     ' LA VARIABLE SERVANT DE TEST POUR DONNEER L'ACCES A LA PAGE")
                .WriteLine("    Dim GetOut As Boolean = False           ' LA VARIABLE SERVANT DE TEST POUR REDIRIGER L'UTILISATEUR VERS LA PAGE DE CONNEXION")
                .WriteLine("    Private IS_SendMail As Boolean = True   ' PAS TROP IMPORTANT...")
                .WriteLine("#End Region")
            End With
            objWriter.WriteLine("")

            With objWriter
                .WriteLine(" Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load")
                .WriteLine("    Response.Cache.SetCacheability(HttpCacheability.NoCache) ")
                .WriteLine("    Response.Expires = -1 ")
                .WriteLine("    Panel_Msg.Visible = False ")
                .WriteLine(" ")
                .WriteLine("    SYSTEME_SECURITE()  ' APPEL A LA METHODE SERVANT A TESTER LES COMPOSANTS DE LA PAGE Y COMPRIS LA PAGE ELLE MEME ")
                .WriteLine("")
                .WriteLine("    '--- Si l'utilisateur n'a Access a la page les informations ne sont pas charger dans la Page_Load ")
                .WriteLine("    If Is_Acces_Page Then ")
                .WriteLine("        If Not IsPostBack Then ")
                .WriteLine("            rbtnAdd" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx', 750, 400)); return false;"")") '& Chr(13) _
                .WriteLine("            'BtnADDNew.Attributes.Add(""onclick"", ""javascript:Open_Window('Frm_" & nomSimple & "ADD.aspx', '_self',500,400); return false;"") ")
                .WriteLine("            BindGrid() ")
                .WriteLine("        End If ")
                .WriteLine("    End If ")
                .WriteLine("End Sub")
                .WriteLine(" ")
                .WriteLine(" ")
            End With

            With objWriter
                .WriteLine("#Region ""SECURITE""")
                .WriteLine("    Public Sub SYSTEME_SECURITE()")
                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liGROUPE_PARAMETRES""), HtmlControl).Attributes.Add(""class"", ""active treeview"")")
                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liCentreDeDetentionListe""), HtmlControl).Attributes.Add(""class"", ""active"")")
                .WriteLine("")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) IsNot Nothing Then")
                .WriteLine("            User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)")
                .WriteLine("            If Not Cls_Privilege.VerifyRightOnObject(Nom_page, User_Connected.IdGroupeuser) Then    ' VERIFICATION SI L'UTILISATEUR N'A PAS ACCES A LA PAGE")
                .WriteLine("                _message = [Global].NO_ACCES_PAGE")
                .WriteLine("                MessageToShow(_message)")
                .WriteLine("                Is_Acces_Page = False")
                .WriteLine(" ")
                .WriteLine("                Panel_First.Visible = False")
                .WriteLine("            Else    ' SI L'UTILISATEUR A ACCES A LA PAGE ON VERIFIE POUR LES BOUTONS ET LES LIENS")
                .WriteLine("                '---  Okey vous avez acces a la page ---'")
                .WriteLine("                'rbtnAdd" & nomSimple & ".Visible = Cls_Privilege.VerifyRightOnObject(Btn_Save, User_Connected.IdGroupeuser)")
                .WriteLine("            End If")
                .WriteLine("        End If")
                .WriteLine(" ")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) Is Nothing Then")
                .WriteLine("            '-- Session expirée --'")
                .WriteLine("            GetOut = True")
                .WriteLine("        Else")
                .WriteLine("            Try")
                .WriteLine("                User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)  ' ON VERIFIE SI LÚTILISATEUR A ETE FORCE DE SE CONNECTER 'PAR L'ADM")
                .WriteLine("                If Not (GlobalFunctions.IsUserStillConnected(User_Connected) And GlobalFunctions.IsUserStillActive(User_Connected)) Then")
                .WriteLine("                    User_Connected.Set_Status_ConnectedUser(False)")
                .WriteLine("                    User_Connected.Activite_Utilisateur_InRezo(""Forced Log Off"", ""Forced to Log Off"", Request.UserHostAddress)")
                .WriteLine("")
                .WriteLine("                    GetOut = True")
                .WriteLine("                    Session.RemoveAll()")
                .WriteLine("                    _message = ""Session expirée.""")
                .WriteLine("                    MessageToShow(_message)")
                .WriteLine("                    Is_Acces_Page = True")
                .WriteLine("                End If")
                .WriteLine("            Catch ex As Exception")
                .WriteLine("                GetOut = True")
                .WriteLine("                _message = ""Session expirée.""")
                .WriteLine("                MessageToShow(_message)")
                .WriteLine("            End Try")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("    If GetOut Then ' REDIRECTIONNEMENT DE L'UTILISATUER OU PAS.")
                .WriteLine("Response.Redirect([Global].PAGE_LOGIN)")
                .WriteLine("End If")
                .WriteLine("End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine()
                .WriteLine("#Region ""Other Method""")
                .WriteLine("    Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"")")
                .WriteLine("        Panel_Msg.Visible = True")
                .WriteLine("        GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
                .WriteLine("        Label_Msg.Text = _message")
                .WriteLine("        RadAjaxManager1.ResponseScripts.Add(""alert('"" & _message.Replace(""'"", ""\'"") & ""');"")")
                .WriteLine("        If E_or_S = ""S"" Then")
                .WriteLine("            Label_Msg.ForeColor = Drawing.Color.Green")
                .WriteLine("        Else")
                .WriteLine("            Label_Msg.ForeColor = Drawing.Color.Red")
                .WriteLine("        End If")
                .WriteLine("    End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With


            With objWriter
                .WriteLine()
                .WriteLine("#Region ""Load DATA""")
                .WriteLine("    Private Sub BindGrid(Optional ByVal _refresh As Boolean = True )")
                .WriteLine("Dim objs As List(of Cls_" & nomSimple & ")")
                .WriteLine("Dim _ret As Long = 0")
                .WriteLine("Try")
                .WriteLine("objs = Cls_" & nomSimple & ".SearchAll")
                .WriteLine("rdg" & nomSimple & ".DataSource = objs")
                .WriteLine("If _refresh Then")
                .WriteLine("rdg" & nomSimple & ".DataBind()")
                .WriteLine("End If")
                .WriteLine(" _ret = objs.Count")
                .WriteLine("            LabelTitre.Text = """ & nomSimple & " <small class=""""badge  bg-yellow"""">"" & _ret & ""</small>""")
                .WriteLine("Catch ex As Exception")
                .WriteLine("            _message = ex.Message")
                .WriteLine("            MessageToShow(_message)")
                .WriteLine("            _message = "" Methode -> [ BindGrid ] :"" & ex.Message")
                .WriteLine("            [Global].WriteError(_message, User_Connected)")
                .WriteLine("End Try")
                .WriteLine("    End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine()
                .WriteLine("#Region ""RADGRID EVENTS""")
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemCommand(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs)")
                .WriteLine("    Try")
                .WriteLine("        If e.CommandName = Telerik.Web.UI.RadGrid.ExportToExcelCommandName Then")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.ExportOnlyData = True")
                .WriteLine("            rdg" & nomSimple & ".GridLines = GridLines.Both")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.IgnorePaging = True")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.OpenInNewWindow = False")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.FileName = ""Liste des " & nomSimple & """")
                .WriteLine("            rdg" & nomSimple & ".MasterTableView.Columns(0).Visible = False")
                .WriteLine("            rdg" & nomSimple & ".MasterTableView.ExportToExcel()")
                .WriteLine("        End If")
                .WriteLine()
                .WriteLine("        Select Case e.CommandName")
                .WriteLine("            Case ""delete""")
                .WriteLine("                Dim obj As New Cls_" & nomSimple & "(TypeSafeConversion.NullSafeLong(e.CommandArgument))")
                .WriteLine("                obj.Delete()")
                .WriteLine("                'User_Connected.Activite_Utilisateur_InRezo(""DELETE " & nomSimple & " "", obj.ID & "" - Code:"" & obj.Titrerapport & "" Prop:"", Request.UserHostAddress)")
                .WriteLine("                MessageToShow(""" & nomSimple & "  supprim&eacute;(e)"", ""S"")")
                .WriteLine("                rdg" & nomSimple & ".Rebind()")
                .WriteLine("        End Select")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        _message = ex.Message")
                .WriteLine("        MessageToShow(_message)")
                .WriteLine("        _message = "" Methode -> [ rdg" & nomSimple & "_ItemCommand ] :"" & ex.Message")
                .WriteLine("        [Global].WriteError(_message, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rdg" & nomSimple & ".ItemDataBound")
                .WriteLine("    Try")
                .WriteLine("Dim gridDataItem = TryCast(e.Item, GridDataItem)")
                .WriteLine(" If e.Item.ItemType = GridItemType.Item Or e.Item.ItemType = GridItemType.AlternatingItem Then")
                .WriteLine(" 'Dim _lnk As HyperLink = DirectCast(gridDataItem.FindControl(""hlk""), HyperLink)")
                .WriteLine("  'Dim _lbl_ID As Label = DirectCast(gridDataItem.FindControl(""lbl_ID""), Label)")
                .WriteLine("'_lnk.Attributes.Clear()")
                .WriteLine("            '_lnk.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID="" & CLng(_lbl_ID.Text) & ""', 750, 400));"")")
                .WriteLine("End If")
                .WriteLine()
                .WriteLine("If (gridDataItem IsNot Nothing) Then")
                .WriteLine("Dim item As GridDataItem = gridDataItem")
                .WriteLine("CType(item.FindControl(""lbOrder""), Label).Text = rdg" & nomSimple & ".PageSize * rdg" & nomSimple & ".CurrentPageIndex + (item.RowIndex / 2)")
                .WriteLine("")
                .WriteLine("Dim imagedelete As ImageButton = CType(item(""delete"").Controls(0), ImageButton)")
                .WriteLine("Dim imageediter As ImageButton = CType(item(""editer"").Controls(0), ImageButton)")
                .WriteLine("imagedelete.ToolTip = ""Effacer ce " & nomSimple.ToLower & """")
                .WriteLine("imageediter.ToolTip = ""Editer ce " & nomSimple.ToLower & """")
                .WriteLine("imagedelete.CommandArgument = CType(DataBinder.Eval(e.Item.DataItem, ""ID""), String)")
                .WriteLine("            imageediter.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID="" & CType(DataBinder.Eval(e.Item.DataItem, ""ID""), Long) & ""',800,550));"")")
                .WriteLine("            REM Privilege")
                .WriteLine("            imageediter.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Save, User_Connected.IdGroupeuser)")
                .WriteLine("            imagedelete.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Delete, User_Connected.IdGroupeuser)")
                .WriteLine("End If")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        _message = ex.Message")
                .WriteLine("        MessageToShow(_message)")
                .WriteLine("        _message = "" Methode -> [ rdg" & nomSimple & "_ItemDataBound ] :"" & ex.Message")
                .WriteLine("        [Global].WriteError(_message, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
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
                .WriteLine("    Try")
                .WriteLine("For Each column As GridColumn In rdg" & nomSimple & ".MasterTableView.Columns")
                .WriteLine("column.CurrentFilterFunction = GridKnownFunction.NoFilter")
                .WriteLine("column.CurrentFilterValue = String.Empty")
                .WriteLine("Next")
                .WriteLine("rdg" & nomSimple & ".MasterTableView.FilterExpression = String.Empty")
                .WriteLine(" rdg" & nomSimple & ".MasterTableView.Rebind()")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        _message = ex.Message")
                .WriteLine("        MessageToShow(_message)")
                .WriteLine("        _message = "" Methode -> [ rbtnClearFilters_Click ] :"" & ex.Message")
                .WriteLine("        [Global].WriteError(_message, User_Connected)")
                .WriteLine("    End Try")
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
                .WriteLine("        _message = ex.Message")
                .WriteLine("MessageToShow(_message)")
                .WriteLine("        _message = "" Methode -> [ RadAjaxManager1_AjaxRequest ] :"" & ex.Message")
                .WriteLine("        [Global].WriteError(_message, User_Connected)")
                .WriteLine("End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#End Region")
                .WriteLine()
            End With

            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateListingCodeAsp(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
            '  Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
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

            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            Dim _table As New Cls_Table()

            ' _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
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

            objWriter.WriteLine("<%@ Page Title=""" & nomSimple & """ Language=""VB"" MasterPageFile=""~/MasterPage/DashboardMasterPage.master"" " & _
           "  AutoEventWireup=""false""  MaintainScrollPositionOnPostback=""true"" CodeFile=""Frm_" & nomSimple & "Listing.aspx.vb"" Inherits=""Frm_" & nomSimple & "Listing"" %>")

            objWriter.WriteLine("<asp:Content ID=""Content1"" ContentPlaceHolderID=""ContentPlaceHolder1"" runat=""Server"">")
            objWriter.WriteLine("<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("<script type=""text/javascript"">")
            objWriter.WriteLine(" ")
            objWriter.WriteLine("function onRequestStart(sender, args) {")
            objWriter.WriteLine(" if (args.get_eventTarget().indexOf(""ExportToExcelButton"") >= 0) {")
            objWriter.WriteLine("args.set_enableAjax(false);")
            objWriter.WriteLine(" }")
            objWriter.WriteLine(" }")
            objWriter.WriteLine()

            With objWriter
                .WriteLine(" function ShowAddUpdateForm(strPage, tmpW, tmpH) {")
                .WriteLine("var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
                .WriteLine("        //oWindow.set_autoSize(true);")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("        document.getElementById(""txtWindowPage"").value = strPage;")
                .WriteLine("        if (oWindow) {")
                .WriteLine("            if (!oWindow.isClosed()) {")
                .WriteLine("                oWindow.center();")
                .WriteLine("                var bounds = oWindow.getWindowBounds();")
                .WriteLine("                oWindow.moveTo(bounds.x + 'px', ""50px"");")
                .WriteLine("            }")
                .WriteLine("        }")
                .WriteLine("        return false;")
                .WriteLine("    }")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function ShowAddUpdateFormMaximized(strPage, tmpW, tmpH) { ")
                .WriteLine("    var oWindow = window.radopen(strPage, ""AddUpdateDialog""); ")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("    document.getElementById(""txtWindowPage"").value = strPage; ")
                .WriteLine("    if (oWindow) { ")
                .WriteLine("                       if (!oWindow.isClosed()) { ")
                .WriteLine("                           oWindow.center(); ")
                .WriteLine("                           var bounds = oWindow.getWindowBounds(); ")
                .WriteLine("                           oWindow.moveTo(bounds.x + 'px', ""50px""); ")
                .WriteLine("                       } ")
                .WriteLine("                   } ")
                .WriteLine("                   oWindow.maximize(); ")
                .WriteLine("                   return false; ")
                .WriteLine("               } // ")
            End With
            objWriter.WriteLine()

            With objWriter
                .WriteLine("    function ShowAddUpdateFormAutoSize(strPage, tmpW, tmpH) {")
                .WriteLine("                  var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
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
            End With

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

            objWriter.WriteLine(" <%--<telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server""> </telerik:RadScriptManager>--%>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("<AjaxSettings>")
            With objWriter
                .WriteLine("<telerik:AjaxSetting AjaxControlID=""RadAjaxManager1"">")
                .WriteLine("<UpdatedControls>")
                .WriteLine("        <telerik:AjaxUpdatedControl ControlID=""Panel_First"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
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
            'objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7""> </telerik:RadSkinManager>")
            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")

            With objWriter
                .WriteLine("   <section class=""content-header""> ")
                .WriteLine("<h1>")
                .WriteLine("           <asp:Label ID=""LabelTitre"" runat=""server"" Text=""" & nomSimple & """ /> ")
                .WriteLine("           <small> <asp:Label ID=""LabelSousTitre"" runat=""server"" /></small> ")
                .WriteLine("</h1>")
                .WriteLine("       <%--<ol class=""breadcrumb""> ")
                .WriteLine("           <li><a href=""#""><i class=""fa fa-dashboard""></i>Accueil</a></li>")
                .WriteLine("           <li class=""active"">" & nomSimple & "</li>")
                .WriteLine("           <asp:SiteMapPath ID=""sitmape111"" runat=""server"" ></asp:SiteMapPath> ")
                .WriteLine("       </ol> --%>")
                .WriteLine("   </section> ")
            End With

            With objWriter
                .WriteLine("<asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
                .WriteLine("       <div class=""alert alert-warning""> ")
                .WriteLine("           <asp:Image ID=""Image_Msg"" runat=""server"" /> ")
                .WriteLine(" <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
                .WriteLine(" </div>")
                .WriteLine("</asp:Panel>")
            End With
            objWriter.WriteLine("  ")
            objWriter.WriteLine("<section class=""content"" >")
            objWriter.WriteLine("   <asp:Panel ID=""Panel_First"" runat=""server"" CssClass=""panel panel-default panel-body"">")

            With objWriter
                .WriteLine("<table style=""width: 100%;"">")
                .WriteLine("    <tr>")
                .WriteLine("       <td>")
                .WriteLine("            <%--<telerik:RadButton ID=""rbtnAdd" & nomSimple & """  runat=""server"" AutoPostBack=""False"" Text=""Enregistrer un " & nomSimple.ToLower & """ ")
                .WriteLine("             Icon-PrimaryIconUrl=""~/images/_add.png"" > </telerik:RadButton>--%>")
                .WriteLine("            <asp:LinkButton ID=""rbtnAdd" & nomSimple & """ runat=""server"" CssClass=""btn btn-primary"" style=""margin-bottom:5px;"">")
                .WriteLine("                <i class=""fa fa-plus-square-o""></i> Enregistrer " & nomSimple.ToLower & " </asp:LinkButton> ")
                .WriteLine("            <telerik:RadButton ID=""rbtnClearFilters""  Visible=""false"" runat=""server"" Text=""Vider filtres""> </telerik:RadButton>")
                .WriteLine("        </td>")
                .WriteLine("        <td style=""width:30%;"">")
                .WriteLine("            <div class=""input-group input-group-sm"">")
                .WriteLine("                <asp:TextBox ID=""txt_Search"" runat=""server"" CssClass=""form-control"" placeholder=""Recherche...""  />")
                .WriteLine("                <span class=""input-group-btn"">")
                .WriteLine("                    <asp:LinkButton ID=""LinkButton_Search"" runat=""server"" CausesValidation=""false"" CssClass=""btn btn-info btn-flat"">")
                .WriteLine("                        <i class=""fa fa-search""></i> </asp:LinkButton>")
                .WriteLine("                </span>")
                .WriteLine("</div>")
                .WriteLine("</td>")
                .WriteLine(" </tr>")
                .WriteLine("</table>")
            End With

            With objWriter
                .WriteLine(" <telerik:RadGrid ID=""rdg" & nomSimple & """ AllowPaging=""True"" AllowSorting=""True"" PageSize=""20""")
                .WriteLine(" runat=""server"" AutoGenerateColumns=""False"" GridLines=""None"" AllowFilteringByColumn=""true"" ")

                .WriteLine(" EnableViewState=""true"" AllowMultiRowSelection=""false"" OnItemCommand=""rdg" & nomSimple & "_ItemCommand"" GroupingSettings-CaseSensitive=""false"">")
                .WriteLine(" <ExportSettings HideStructureColumns=""true"" />")
                .WriteLine("  <MasterTableView CommandItemDisplay=""Top"" GridLines=""None"" DataKeyNames=""ID"" NoDetailRecordsText=""Pas d'enregistrement""")
                .WriteLine("NoMasterRecordsText=""Pas d'enregistrement"">")
                .WriteLine(" <CommandItemSettings ShowAddNewRecordButton=""false"" ShowRefreshButton=""false"" ShowExportToExcelButton=""true"" ")
                .WriteLine("  ExportToExcelText=""Exporter en excel"" />")
                .WriteLine(" <Columns>")

                .WriteLine("<telerik:GridBoundColumn DataField=""ID"" UniqueName=""ID"" Display=""false"">")
                .WriteLine("<ItemStyle Width=""1px"" />")
                .WriteLine("</telerik:GridBoundColumn>")

                .WriteLine("<telerik:GridTemplateColumn HeaderText=""#"" UniqueName=""Compteur"">")
                .WriteLine("           <ItemTemplate>")
                .WriteLine("               <asp:Label Visible=""true"" ID=""lbOrder"" runat=""server"" />")
                .WriteLine("           </ItemTemplate>")
                .WriteLine("           <HeaderStyle HorizontalAlign=""Center"" Width=""16px"" />")
                .WriteLine("           <ItemStyle HorizontalAlign=""Center"" Width=""16px"" />")
                .WriteLine("       </telerik:GridTemplateColumn>")

                Dim countColumn As Integer = 0
                'Dim pourcentagevalue As Decimal = 100 / (_table.ListofColumn.Count - 4)
                Dim pourcentagevalue As Decimal = 100 / cols.Count - 3
                Dim pourcentage As String = pourcentagevalue.ToString + "%"


                For i As Int32 = 1 To cols.Count - 3
                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                        Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")

                        .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                        .WriteLine(" FilterControlAltText=""Filter " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                        .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        '.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                        .WriteLine("</telerik:GridBoundColumn>")


                    ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                        Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                        Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                        .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                        .WriteLine(" FilterControlAltText=""Filter " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                        .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                        .WriteLine("</telerik:GridBoundColumn>")
                    ElseIf types(i) = "Boolean" And cols(i) <> "isdirty" Then
                        Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                        Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                        .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                        .WriteLine(" FilterControlAltText=""Filter " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                        .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        '.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                        .WriteLine("</telerik:GridBoundColumn>")
                    Else
                        Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                        Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                        .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                        .WriteLine(" FilterControlAltText=""Filter " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                        .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        '.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                        .WriteLine("</telerik:GridBoundColumn>")
                    End If
                Next

                .WriteLine(" <telerik:GridButtonColumn ButtonType=""ImageButton"" CommandArgument=""ID"" CommandName=""editer""")
                .WriteLine("        DataTextField=""ID"" ImageUrl=""~/images/_edit.png""")
                .WriteLine("          HeaderText="""" UniqueName=""editer"">")
                .WriteLine("        <HeaderStyle HorizontalAlign=""Center"" Width=""16px"" />")
                .WriteLine("        <ItemStyle  HorizontalAlign=""Center"" Width=""16px"" />")
                .WriteLine("    </telerik:GridButtonColumn>")

                .WriteLine("<telerik:GridButtonColumn ButtonType=""ImageButton"" CommandName=""delete"" DataTextField=""ID""")
                .WriteLine(" ImageUrl=""~/images/delete.png"" ")
                .WriteLine("UniqueName=""delete"" HeaderText="""" ConfirmDialogType=""RadWindow"" ConfirmText=""Voulez-vous vraiment supprimer ce " & nomSimple & "?""")
                .WriteLine("ConfirmTitle=""Attention!"">")
                .WriteLine("<HeaderStyle  HorizontalAlign=""Center"" Width=""16px""  />")
                .WriteLine("<ItemStyle  HorizontalAlign=""Center"" Width=""16px""  />")
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
                .WriteLine(" ShowContentDuringLoad=""false"" Modal=""true"" OnClientClose=""RadWindowClosing"" Behaviors=""Reload, Pin, Minimize, Move, Resize, Maximize, Close""")
                .WriteLine("EnableShadow=""true"" OnClientResizeEnd=""RadWindowClientResizeEnd"" />")
                .WriteLine("</Windows>")
                .WriteLine("</telerik:RadWindowManager>")

                .WriteLine("<telerik:RadContextMenu ID=""ContextMenu"" runat=""server"" OnClientItemClicked=""""")
                .WriteLine("EnableRoundedCorners=""true"" EnableShadows=""true"">")
                .WriteLine("<Items>")
                .WriteLine("<telerik:RadMenuItem Text=""Editer"" />")
                .WriteLine("</Items>")
                .WriteLine("</telerik:RadContextMenu>")
            End With

            objWriter.WriteLine("   </asp:Panel>") 'FIN PANEL
            objWriter.WriteLine("<input id=""txtWindowPage"" type=""hidden"" />")
            objWriter.WriteLine("</section>") ' FIN Section Content

            objWriter.WriteLine("</asp:Content>")

            objWriter.WriteLine()
            objWriter.Close()
        End Sub


#Region "TEMPLATE AdminLTE"
        Public Shared Sub CreateInterface_AdminLTE_Tableau_Formulaire_Design(ByVal name As String _
                                                                         , ByRef txt_PathGenerate_ScriptFile As TextBox _
                                                                         , ByRef ListBox_NameSpace As ListBox _
                                                                         , ByVal databasename As String)
            '  Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
            Dim path As String = txt_PathGenerate_Script & nomClasse & "ADD.aspx"

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

            'objWriter.WriteLine()

            'objWriter.WriteLine()

            Dim _table As New Cls_Table()
            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            '  _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next


            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If

            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next

            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
                    count += 1
                Else
                    Exit For
                End If
            Next

            'cols.Add("_isdirty")
            'cols.Add("_LogData")
            'types.Add("Boolean")
            'types.Add("String")
            'initialtypes.Add("Byte")
            'initialtypes.Add("nvarchar")

            Dim nomSimpleToShow As String = Regex.Replace(nomSimple, "([a-z])?([A-Z])", "$1 $2")

            objWriter.WriteLine("<%@ Page Title=""" & nomSimpleToShow & """ Language=""VB"" MasterPageFile=""~/MasterPage/DashboardMasterPage.master"" " &
           " AutoEventWireup=""false"" CodeFile=""Frm_" & nomSimple & "ADD.aspx.vb"" Inherits=""Frm_" & nomSimple & "ADD""  MaintainScrollPositionOnPostback=""true"" %>")

            objWriter.WriteLine("<asp:Content ID=""Content1"" ContentPlaceHolderID=""ContentPlaceHolder1"" runat=""Server"">")

            objWriter.WriteLine("<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("<script type=""text/javascript"">")


            With objWriter
                .WriteLine(" function ShowAddUpdateForm(strPage, tmpW, tmpH) {")
                .WriteLine("var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
                .WriteLine("        //oWindow.set_autoSize(true);")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("        document.getElementById(""txtWindowPage"").value = strPage;")
                .WriteLine("        if (oWindow) {")
                .WriteLine("            if (!oWindow.isClosed()) {")
                .WriteLine("                oWindow.center();")
                .WriteLine("                var bounds = oWindow.getWindowBounds();")
                .WriteLine("                oWindow.moveTo(bounds.x + 'px', ""50px"");")
                .WriteLine("            }")
                .WriteLine("        }")
                .WriteLine("        return false;")
                .WriteLine("    }")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function ShowAddUpdateFormMaximized(strPage, tmpW, tmpH) { ")
                .WriteLine("    var oWindow = window.radopen(strPage, ""AddUpdateDialog""); ")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("    document.getElementById(""txtWindowPage"").value = strPage; ")
                .WriteLine("    if (oWindow) { ")
                .WriteLine("                       if (!oWindow.isClosed()) { ")
                .WriteLine("                           oWindow.center(); ")
                .WriteLine("                           var bounds = oWindow.getWindowBounds(); ")
                .WriteLine("                           oWindow.moveTo(bounds.x + 'px', ""50px""); ")
                .WriteLine("            } ")
                .WriteLine("    } ")
                .WriteLine("                   oWindow.maximize(); ")
                .WriteLine("                   return false; ")
                .WriteLine("} // ")
            End With
            objWriter.WriteLine()

            With objWriter
                .WriteLine("function ShowAddUpdateFormAutoSize(strPage, tmpW, tmpH) {")
                .WriteLine("                  var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
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
            End With

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

            objWriter.WriteLine("function CloseAndRefreshListe() {")
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

            objWriter.WriteLine("<%--<telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server""></telerik:RadScriptManager>--%>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("   <AjaxSettings>")
            objWriter.WriteLine("       <%--<telerik:AjaxSetting AjaxControlID=""Btn_SaveInfo"">")
            objWriter.WriteLine("           <UpdatedControls>")
            objWriter.WriteLine("               <telerik:AjaxUpdatedControl ControlID=""Panel_Msg"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("               <telerik:AjaxUpdatedControl ControlID=""Panel_First"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("           </UpdatedControls>")
            objWriter.WriteLine("       </telerik:AjaxSetting>--%>")
            objWriter.WriteLine("   </AjaxSettings>")
            objWriter.WriteLine("</telerik:RadAjaxManager>")
            'objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7""> </telerik:RadSkinManager>")
            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")


            'objWriter.WriteLine("<div class=""container-fluid"" id=""pcont"">")
            'objWriter.WriteLine("<div class=""mail-inbox"">")

            With objWriter
                .WriteLine("<section class=""content-header"" ID=""PageHeader"" runat=""server"" >")
                .WriteLine("<h1>")
                .WriteLine("    <i class=""fa fa-dashboard""></i>")
                .WriteLine("    <asp:Label ID=""Label_Titre"" runat=""server"" Text=""" & nomSimple & """ /> ")
                .WriteLine("    <small id=""OL_SeeAllData"" runat=""server"">")
                .WriteLine("        <asp:Label ID=""Label_SousTitre"" runat=""server"" />")
                .WriteLine("    </small>")
                .WriteLine("</h1>")
                .WriteLine("<!--<ol class=""breadcrumb""> ")
                .WriteLine("    <li><a href=""#""><i class=""fa fa-dashboard""></i>Accueil</a></li>")
                .WriteLine("    <li class=""active"">" & nomSimple & "</li>")
                .WriteLine("</ol> -->")
                .WriteLine("</section> ") 'end section 1
            End With

            objWriter.WriteLine("<section class=""content"">")

            With objWriter
                .WriteLine("    <Msg:msgBox ID=""Dialogue"" runat=""server"" />")
                .WriteLine("    <asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
                .WriteLine("        <div id=""DIV_Msg"" runat=""server"" class=""alert alert-warning alert-dismissable"">")
                .WriteLine("            <i id=""Icon_Msg"" runat=""server"" class=""fa fa-warning""></i>")
                .WriteLine("            <button type=""button"" class=""close"" data-dismiss=""alert"" aria-hidden=""true"">×</button>")
                .WriteLine("            <asp:Image ID=""Image_Msg"" runat=""server"" />")
                .WriteLine("            <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
                .WriteLine("        </div>")
                .WriteLine("    </asp:Panel>")
                .WriteLine("")
            End With

            objWriter.WriteLine("<asp:ValidationSummary ID=""ValidationSummary1"" ValidationGroup=""GPSave""  runat=""server"" CssClass=""alert alert-danger alert-dismissable"" ShowMessageBox=""true"" ShowSummary=""true"" />")

            objWriter.WriteLine("")
            objWriter.WriteLine("<asp:Panel ID=""Panel_First"" runat=""server"" CssClass=""panel panel-default panel-body"" Style=""margin: 5px;"">")

            REM DEBUT TABLEAU
            objWriter.WriteLine("<div id=""DIV_Panel"" class=""form-horizontal group-border-dashed1"" style=""border-radius: 0px;"">")
            Dim countColumn As Integer = 0

            For i As Int32 = 1 To cols.Count - 1 '3
                objWriter.WriteLine("   <div class=""form-group"">")

                If ListofForeignKey.Contains(cols(i)) Then
                    Dim columnName As String = cols(i) '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                    If Not columnName.Equals("CreatedBy") Then

                    End If

                    objWriter.WriteLine("<label class=""col-sm-3 control-label"">" & columnNameToShow & "")
                    objWriter.WriteLine("   <asp:RequiredFieldValidator  ID=""RFV" & columnName & """ runat=""server"" ControlToValidate=""DDL" & columnName & """  ")
                    objWriter.WriteLine("       ErrorMessage=""" & columnNameToShow & " Obligatoire !"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" ")
                    objWriter.WriteLine("       ValidationGroup=""GPSave"" CssClass=""text-danger"" /> ")
                    objWriter.WriteLine("   <asp:RequiredFieldValidator  ID=""RFV1" & columnName & """ runat=""server"" ControlToValidate=""DDL" & columnName & """  ")
                    objWriter.WriteLine("       ErrorMessage=""" & columnNameToShow & " Obligatoire !"" InitialValue=""0"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" ")
                    objWriter.WriteLine("       ValidationGroup=""GPSave"" CssClass=""text-danger"" /> ")
                    objWriter.WriteLine("</label>")
                    objWriter.WriteLine("<div class=""col-sm-6"">")
                    objWriter.WriteLine("   <asp:DropDownList ID=""DDL" & columnName & """ CssClass=""select2"" Width=""100%"" runat=""server"" >")
                    objWriter.WriteLine("   </asp:DropDownList>")
                    objWriter.WriteLine("</div>")
                    objWriter.WriteLine("")

                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                    columnNameToShow = Regex.Replace(columnNameToShow, "_", " ")

                    objWriter.WriteLine("<label class=""col-sm-3 control-label"">" & columnNameToShow & "")
                    objWriter.WriteLine("   <asp:RequiredFieldValidator  ID=""RFV_" & columnName & """ runat=""server"" ControlToValidate=""rdp_" & columnName & """  ")
                    objWriter.WriteLine("       ErrorMessage=""" & columnNameToShow & " Obligatoire !"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" ")
                    objWriter.WriteLine("       ValidationGroup=""GPSave"" CssClass=""text-danger"" /> ")
                    objWriter.WriteLine("</label>")
                    objWriter.WriteLine("<div class=""col-sm-6"">")
                    objWriter.WriteLine("   <telerik:RadDatePicker ID=""rdp_" & columnName & """ placeholder=""" & columnNameToShow & """ Width=""100%""")
                    objWriter.WriteLine("       runat=""server"" DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01"" ToolTip=""Cliquer sur le bouton pour choisir une date""")
                    objWriter.WriteLine("       Skin=""MetroTouch"">")
                    objWriter.WriteLine("       <Calendar runat=""server"" Skin=""MetroTouch"" UseColumnHeadersAsSelectors=""False"" UseRowHeadersAsSelectors=""False"" CultureInfo=""fr-FR""></Calendar>")
                    objWriter.WriteLine("       <DateInput runat=""server"" DateFormat=""dd-MMM-yyyy"" DisplayDateFormat=""dd MMM yyyy"" LabelWidth=""40%"" Culture=""fr-FR""></DateInput>")
                    objWriter.WriteLine("   </telerik:RadDatePicker>")
                    objWriter.WriteLine("</div>")
                    objWriter.WriteLine("")

                ElseIf types(i) = "Boolean" And cols(i) <> "isdirty" Then
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                    columnNameToShow = Regex.Replace(columnNameToShow, "_", " ")

                    objWriter.WriteLine("<label class=""col-sm-3 control-label"">" & columnNameToShow & "")
                    objWriter.WriteLine("</label>")
                    objWriter.WriteLine("<div class=""col-sm-6"">")
                    objWriter.WriteLine("   <label><asp:CheckBox ID=""CB_" & columnName & """ runat=""server"" text=""" & columnName & """ /></label>  ")
                    objWriter.WriteLine("</div>")
                    objWriter.WriteLine("")
                Else
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])_?([A-Z])", "$1 $2")
                    columnNameToShow = Regex.Replace(columnNameToShow, "_", " ")

                    objWriter.WriteLine("<label class=""col-sm-3 control-label"">" & columnNameToShow & "")
                    objWriter.WriteLine("   <asp:RequiredFieldValidator  ID=""RFV_" & columnName & """ runat=""server"" ControlToValidate=""txt_" & columnName & """  ")
                    objWriter.WriteLine("       ErrorMessage=""" & columnNameToShow & " Obligatoire !"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" ")
                    objWriter.WriteLine("       ValidationGroup=""GPSave"" CssClass=""text-danger"" /> ")
                    objWriter.WriteLine("</label>")
                    objWriter.WriteLine("<div class=""col-sm-6"">")
                    objWriter.WriteLine("   <asp:TextBox ID=""txt_" & columnName & """ CssClass=""form-control"" Width=""100%"" runat=""server"" placeholder=""" & columnNameToShow & "...""></asp:TextBox>")
                    objWriter.WriteLine("</div>")
                    objWriter.WriteLine("")
                End If
                objWriter.WriteLine(" </div>") 'FIN DIV form-group
            Next

            With objWriter
                .WriteLine("<div id=""DIV_SaveInfo"" runat=""server"" style=""margin: 5px 0px; text-align: left;"">")
                .WriteLine("    <span id=""span_SaveInfo"" runat=""server"">")
                .WriteLine("        <asp:LinkButton ID=""Btn_SaveInfo"" runat=""server"" CssClass=""btn btn-primary"" ValidationGroup=""GPSave"">")
                .WriteLine("            <i class=""fa fa-save"" ></i> Enregistrer")
                .WriteLine("        </asp:LinkButton>")
                .WriteLine("    </span>")
                .WriteLine("    ")
                .WriteLine("&nbsp;")
                .WriteLine("    <asp:LinkButton ID=""Btn_Annuler"" CausesValidation=""false"" runat=""server"" CssClass=""btn btn-danger"">")
                .WriteLine("        <i class=""fa  fa-reply-all"" ></i> Annuler")
                .WriteLine("    </asp:LinkButton>")
                .WriteLine("</div>")
            End With
            objWriter.WriteLine(" </div>") 'FIN DIV TABLEAU

            objWriter.WriteLine("   </asp:Panel>") 'FIN PANEL

            With objWriter
                .WriteLine("<!-- FORM LOGIN -->")
                .WriteLine("<BRAIN:CULogin2 runat=""server"" ID=""LoginWUC"" Visible=""false"" />")
                .WriteLine("<div class=""md-overlay""></div>")
            End With

            objWriter.WriteLine("<asp:TextBox ID=""txt_Code" & nomSimple & "_Hid"" runat=""server"" Text=""0"" Visible=""False"" Width=""1px""></asp:TextBox>")

            objWriter.WriteLine("</section>") ' FIN Section Content
            objWriter.WriteLine("")
            objWriter.WriteLine("<asp:Literal runat=""server"" ID=""LiteralStyleCSS""></asp:Literal>")

            'objWriter.WriteLine("</div>") 'END DIV mail-inbox
            'objWriter.WriteLine("</div>") 'END DIV pcont

            objWriter.WriteLine("<telerik:RadWindowManager ID=""RadWindowManager1"" runat=""server"" VisibleStatusbar=""false"" EnableViewState=""false"">")
            objWriter.WriteLine("   <Windows>")
            objWriter.WriteLine("       <telerik:RadWindow ID=""AddUpdateDialog"" runat=""server"" Title="""" IconUrl=""~/Images/favicon.ico"" Left=""75px"" ReloadOnShow=""true""")
            objWriter.WriteLine("       ShowContentDuringLoad=""false"" Modal=""true"" OnClientClose=""RadWindowClosing"" Behaviors=""Reload, Move, Resize, Maximize, Close""")
            objWriter.WriteLine("       EnableShadow=""false"" OnClientResizeEnd=""RadWindowClientResizeEnd"" />")
            objWriter.WriteLine("   </Windows>")
            objWriter.WriteLine("</telerik:RadWindowManager>")

            objWriter.WriteLine("<input id=""txtWindowPage"" type=""hidden"" />")

            objWriter.WriteLine("</asp:Content>")
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterface_AdminLTE_Tableau_Formulaire_CodeBehind(ByVal name As String _
                                                                        , ByRef txt_PathGenerate_ScriptFile As TextBox _
                                                                        , ByRef ListBox_NameSpace As ListBox _
                                                                        , ByRef txt_LibraryName As TextBox _
                                                                        , ByVal databasename As String)
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            'Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & databasename & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & databasename & "\ASPWebForm\")
            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
            Dim path As String = txt_PathGenerate_Script & nomWebform & "ADD.aspx.vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            Dim cult As Globalization.CultureInfo = New Globalization.CultureInfo("en-EN")
            Threading.Thread.CurrentThread.CurrentCulture = cult

            Dim header As String = "REM Generate By [GENERIC 16] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now '.ToString("dd-MMM-yyyy hh:mm tt")
            'header &= ""
            Dim content As String = "Partial Class " & nomWebform & "ADD" & Chr(13) _
                                     & " Inherits Cls_BasePage ' LA CLASSE DE LA PAGE HERITE DE CETTE CLASSE DANS LE CAS OU NOUS AVONS UNE APPLICATION WEB multilingue"

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
            Dim libraryname As String = "Imports " & txt_LibraryName.Text
            objWriter.WriteLine("Imports Telerik.Web.UI")
            objWriter.WriteLine(libraryname)
            objWriter.WriteLine()

            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            objWriter.WriteLine(content)
            objWriter.WriteLine()

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
                    count += 1
                Else
                    Exit For
                End If
            Next
            'cols.Add("_isdirty")
            'cols.Add("_LogData")
            'types.Add("Boolean")
            'types.Add("String")
            'initialtypes.Add("Byte")
            'initialtypes.Add("nvarchar")

            Dim _tmpEditState As Boolean = False
            'objWriter.WriteLine("Dim _out As Boolean = False")
            'objWriter.WriteLine("Dim _tmpEditState As Boolean = False")

            With objWriter
                .WriteLine("")
                .WriteLine("#Region ""ATTRIBUTS"" ")
                .WriteLine("    Private _message As String  ' VARIABLE SERVANT A LA RECUPERATION DE TOUS LES MESSAGES D'ECHECS OU DE SUCCES")
                .WriteLine("")
                .WriteLine("    REM DEFINITION ET INITIALISATION DES CONSTANTE POUR LA SECURITE")
                .WriteLine("    Private Const Nom_page As String = ""PAGE-FORMULAIRE-" & nomSimple.ToUpper & """  ' POUR LA PAGE")
                .WriteLine("    Private Const Btn_Save As String = ""Bouton-SAVE-" & nomSimple.ToUpper & """       ' POUR LE BOUTON D'ENREGISTREMENT")
                .WriteLine("    Private Const Btn_Edit As String = ""Bouton-EDIT-" & nomSimple.ToUpper & """       ' POUR LE BOUTON DE MODIFICATION")
                .WriteLine("    Private Const Btn_Delete As String = ""Bouton-DELETE-" & nomSimple.ToUpper & """   ' POUR LE BOUTON DE SUPPRESSION")
                .WriteLine("")
                .WriteLine("    Dim User_Connected As Cls_User          ' INSTANCE DE LA CLASSE UTILISATEUR - UTILISER POUR L'UTILISATEUR EN SESSION ")
                .WriteLine("    Dim Is_Acces_Page As Boolean = True     ' LA VARIABLE SERVANT DE TEST POUR DONNEER L'ACCES A LA PAGE")
                .WriteLine("    Dim GetOut As Boolean = False           ' LA VARIABLE SERVANT DE TEST POUR REDIRIGER L'UTILISATEUR VERS LA PAGE DE CONNEXION")
                .WriteLine("    Dim PAGE_MERE As Long = 0' PAS TROP IMPORTANT...")
                .WriteLine("    Dim PAGE_TITLE As String = """" ")
                .WriteLine("#End Region")
                .WriteLine("")
            End With

            With objWriter
                .WriteLine("Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load ")
                .WriteLine("    Response.Cache.SetCacheability(HttpCacheability.NoCache) ")
                .WriteLine("    Response.Expires = -1 ")
                .WriteLine("    Panel_Msg.Visible = False ")

                Dim nomSimpleToShow As String = Regex.Replace(nomSimple, "([a-z])?([A-Z])", "$1 $2")

                .WriteLine("    PAGE_TITLE = """ & nomSimpleToShow & """")
                .WriteLine("    Page.Title = [Global].Global_APP_NAME_SIGLE & "" | "" & PAGE_TITLE")
                .WriteLine(" ")
                .WriteLine("    SYSTEME_SECURITE()  ' APPEL A LA METHODE SERVANT A TESTER LES COMPOSANTS DE LA PAGE Y COMPRIS LA PAGE ELLE MEME ")
                .WriteLine("")
                .WriteLine("    '--- Si l'utilisateur n'a Access a la page les informations ne sont pas charger dans la Page_Load ")
                .WriteLine("    If Is_Acces_Page Then ")
                .WriteLine("        If Not IsPostBack Then ")
                .WriteLine("            Label_Titre.Text = PAGE_TITLE")
                .WriteLine("            'btnCancel.Attributes.Add(""onclick"", ""javascript:void(closeWindow());"")")
                .WriteLine("            'rbtnAdd" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx', 950, 650)); return false;"")") '& Chr(13) _
                .WriteLine("            'BtnADDNew.Attributes.Add(""onclick"", ""javascript:Open_Window('Frm_" & nomSimple & "ADD.aspx', '_self',500,400); return false;"") ")
                .WriteLine("            Load_ALL_DATA() ")
                .WriteLine("        End If ")
                .WriteLine("    End If ")
                .WriteLine("End Sub ")
                .WriteLine(" ")
            End With


            With objWriter
                .WriteLine("#Region ""SECURITE""")
                .WriteLine("Public Sub SYSTEME_SECURITE()")
                .WriteLine("    Try")
                .WriteLine("        User_Connected = [Global].KeepUserContinuesToWork(User_Connected)")
                .WriteLine("")
                .WriteLine("        'CType(Page.Master.FindControl(""li_" & nomSimple & """), HtmlControl).Attributes.Add(""class"", ""active "")")
                .WriteLine("        'CType(Page.Master.FindControl(""i_" & nomSimple & """), HtmlControl).Attributes.Add(""class"", ""fa fa-folder-open fa-lg "")")

                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liGROUPE_PARAMETRES""), HtmlControl).Attributes.Add(""class"", ""active treeview"")")
                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liCentreDeDetentionListe""), HtmlControl).Attributes.Add(""class"", ""active"")")

                .WriteLine("        LiteralStyleCSS.Text = """" ")
                .WriteLine("        If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("            Select Case Request.QueryString([Global].ACTION)")
                .WriteLine("                Case [Global].HideMenuHeader")
                .WriteLine("                    CType(Page.Master.FindControl([Global].Head_Nav_Menu), HtmlControl).Visible = False")
                .WriteLine("                    CType(Page.Master.FindControl([Global].Head_Nav_MenuVertical), HtmlControl).Visible = False")
                .WriteLine("                    Dim StyleCss As String = ""<style type=""""text/css""""> #cl-wrapper { padding-top: 0px; } </style>""")
                .WriteLine("                    LiteralStyleCSS.Text = StyleCss")
                .WriteLine("                Case Else")
                .WriteLine("                    'span_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("                    'Btn_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("                End Select")
                .WriteLine("        Else")
                .WriteLine("            'span_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("            'Btn_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) IsNot Nothing Then")
                .WriteLine("            User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)")
                .WriteLine("            If Not Cls_Privilege.VerifyRightOnObject(Nom_page, User_Connected.IdGroupeuser) Then    ' VERIFICATION SI L'UTILISATEUR N'A PAS ACCES A LA PAGE")
                .WriteLine("                _message = [Global].NO_ACCES_PAGE")
                .WriteLine("                MessageToShow(_message)")
                .WriteLine("                Is_Acces_Page = False")
                .WriteLine(" ")
                .WriteLine("                Panel_First.Visible = False")
                .WriteLine("            Else    ' SI L'UTILISATEUR A ACCES A LA PAGE ON VERIFIE POUR LES BOUTONS ET LES LIENS")
                .WriteLine("                '---  Okey vous avez acces a la page ---'")
                .WriteLine("                Dim _check As Boolean = Cls_Privilege.VerifyRightOnObject(Btn_Save, User_Connected.IdGroupeuser)")
                .WriteLine("                'Btn_ADD_" & nomSimple & ".Visible = _check")
                .WriteLine("                Btn_SaveInfo.Visible = _check")
                .WriteLine("                'rdg" & nomSimple & ".MasterTableView.Columns.FindByUniqueNameSafe(""editer"").Visible = _check")

                .WriteLine("                If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("                    If Request.QueryString([Global].ACTION).Equals([Global].HideMenuHeader) Then")
                .WriteLine("                        Btn_SaveInfo.Visible = _check")
                .WriteLine("                    End If")
                .WriteLine("                End If")
                .WriteLine("            End If")
                .WriteLine("        End If")
                .WriteLine(" ")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) Is Nothing Then")
                .WriteLine("            '-- Session expirée --'")
                .WriteLine("            GetOut = True")
                .WriteLine("        Else")
                .WriteLine("            Try")
                .WriteLine("                User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)  ' ON VERIFIE SI LÚTILISATEUR A ETE FORCE DE SE CONNECTER 'PAR L'ADM")
                .WriteLine("                If Not (GlobalFunctions.IsUserStillConnected(User_Connected) And GlobalFunctions.IsUserStillActive(User_Connected)) Then")
                .WriteLine("                    User_Connected.Set_Status_ConnectedUser(False)")
                .WriteLine("                    User_Connected.Activite_Utilisateur_InRezo(""Forced Log Off"", ""Forced to Log Off"", Request.UserHostAddress)")
                .WriteLine("")
                .WriteLine("                    GetOut = True")
                .WriteLine("                    Session.RemoveAll()")
                .WriteLine("                    '_message = ""Session expirée.""")
                .WriteLine("                    'MessageToShow(_message)")
                .WriteLine("                    Is_Acces_Page = True")
                .WriteLine("                End If")
                .WriteLine("            Catch ex As Exception")
                .WriteLine("                GetOut = True")
                .WriteLine("                '_message = ""Session expirée.""")
                .WriteLine("                'MessageToShow(_message)")
                .WriteLine("            End Try")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("    If GetOut Then ' REDIRECTIONNEMENT DE L'UTILISATUER OU PAS.")
                .WriteLine("        CType(Page.Master.FindControl([Global].htmlMasterPage), HtmlControl).Attributes.Add(""class"", ""lockscreen"")")
                .WriteLine("        CType(Page.Master.FindControl([Global].bodyMasterPage), HtmlControl).Attributes.Add(""class"", ""texture"")")
                .WriteLine("        CType(Page.Master.FindControl([Global].Head_Nav_Menu), HtmlControl).Visible = False")
                .WriteLine("        CType(Page.Master.FindControl([Global].Head_Nav_MenuVertical), HtmlControl).Visible = False")
                .WriteLine("        ")
                .WriteLine("        Is_Acces_Page = False")
                .WriteLine("        PageHeader.Attributes.Add(""style"", ""visibility:hidden;"")")
                .WriteLine("        Panel_First.Visible = False")
                .WriteLine("        LoginWUC.Visible = True")
                .WriteLine("        Session([Global].GLOBAL_PAGENAME) = System.Web.HttpContext.Current.Request.Url.ToString()")
                .WriteLine("        'Response.Redirect([Global].PAGE_LOGIN)")
                .WriteLine("    End If")

                .WriteLine("")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        Is_Acces_Page = False")
                .WriteLine("        Panel_First.Visible = False")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""Other Method - MessageToShow""")
                .WriteLine("    Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"", Optional ByVal ShowPopUp As Boolean = True)")
                .WriteLine("        Panel_Msg.Visible = True")
                .WriteLine("        GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
                .WriteLine("        Label_Msg.Text = _message")
                .WriteLine("        If ShowPopUp Then")
                .WriteLine("            RadAjaxManager1.ResponseScripts.Add(""alert('"" & [Global].GetTextFromHtml(_message).Replace(""'"", ""\'"") & ""');"")")
                .WriteLine("            'Dialogue.alert([Global].GetTextFromHtml(_message))")
                .WriteLine("        End If")
                .WriteLine("        If E_or_S = ""S"" Then")
                .WriteLine("            Style_Division(DIV_Msg, ""alert alert-success alert-dismissable"")")
                .WriteLine("            Style_Division(Icon_Msg, ""fa  fa-thumbs-up"")")
                .WriteLine("        Else")
                .WriteLine("            Style_Division(DIV_Msg, ""alert alert-danger alert-dismissable"")")
                .WriteLine("            Style_Division(Icon_Msg, ""fa  fa-thumbs-down"")")
                .WriteLine("        End If")
                .WriteLine("    End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            objWriter.WriteLine("#Region ""Load DATA""")
            With objWriter
                .WriteLine("Private Sub LOAD_ALL_DATA()")
                ''---------------------------------''
                For Each fk In ListofForeignKey
                    Dim NameFileCombo1 As String = "FillCombo" & fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                    .WriteLine("" & NameFileCombo1 & "()")
                Next
                ''---------------------------------''
                .WriteLine("LOAD_" & nomSimple.ToUpper & "()")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Private Sub LOAD_" & nomSimple.ToUpper & "()")
                .WriteLine("    Try")
                .WriteLine("        If Request.QueryString(""ID"") IsNot Nothing Then")
                .WriteLine("            dim _id as long = TypeSafeConversion.NullSafeLong(Request.QueryString(""ID""))")
                .WriteLine("            txt_Code" & nomSimple & "_Hid.Text = _id")
                .WriteLine("            Dim obj as New " & nomClasse & "( _id )")
                .WriteLine("            If obj.ID > 0 Then")
                .WriteLine("                Btn_SaveInfo.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Edit, User_Connected.IdGroupeuser)")
                .WriteLine("                With obj")

                For i As Int32 = 1 To cols.Count - 1 '3
                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim nom_DDL As String = "DDL" & cols(i) '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

                        .WriteLine(nom_DDL & " .SelectedIndex =  " & nom_DDL & ".Items.IndexOf(" & nom_DDL & ".Items.FindByValue(." & cols(i).Substring(1, cols(i).Length - 1) & "))")
                        '.WriteLine("rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " .SelectedIndex =  rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & ".FindItemIndexByValue(." & cols(i).Substring(1, cols(i).Length - 1) & ")")
                        'ddl_Sexe.SelectedIndex = ddl_Sexe.Items.IndexOf(ddl_Sexe.Items.FindByValue(obj.Sexe))
                    ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                        .WriteLine("rdp" & cols(i) & ".SelectedDate = ." & cols(i).Substring(1, cols(i).Length - 1))

                    ElseIf types(i) = "Boolean" Then
                        .WriteLine("CB" & cols(i) & ".Checked = ." & cols(i).Substring(1, cols(i).Length - 1))

                    Else
                        .WriteLine("txt" & cols(i) & ".Text = ." & cols(i).Substring(1, cols(i).Length - 1))
                    End If
                Next
                .WriteLine("            End With")
                .WriteLine("        End If")
                .WriteLine("    Else")
                .WriteLine()
                .WriteLine("    End If")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")

                ''-------------------------------------------------''
                objWriter.WriteLine()

                For Each fk In ListofForeignKey
                    'objWriter.WriteLine("FillCombo" & fk & "()")

                    Dim textForcombo As String = ""
                    Dim attributUsed As String = fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                    Dim nomforeign As String = fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                    Dim ClassName As String = "Cls" & fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length))

                    Dim NameFileCombo As String = "FillCombo" & fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

                    textForcombo = nomforeign.Substring(1, nomforeign.Length - 1)
                    .WriteLine("Private Sub " & NameFileCombo & "()")
                    .WriteLine("Try")
                    .WriteLine("Dim objs1 As List(of " & ClassName & ") = " & ClassName & ".SearchAll ")

                    .WriteLine("With DDL" & nomforeign)
                    .WriteLine("    .Datasource = objs1")
                    .WriteLine("    .DataValueField = ""ID""")
                    .WriteLine("    .DataTextField = """ & textForcombo & """")
                    .WriteLine("    .DataBind()")

                    .WriteLine("    .Items.Insert(0, New ListItem("" - Choisir("" & objs1.Count & "") - "", 0))")
                    .WriteLine("    .SelectedIndex = -1")

                    .WriteLine("    '.Items.Sort()")
                    .WriteLine("    '.Items.Insert(0, New RadComboBoxItem("" - Choisir -"", """"))")
                    .WriteLine("    '.SelectedIndex = 0")
                    .WriteLine("    '.EmptyMessage = ""- Choisir -""")
                    .WriteLine("End With")
                    .WriteLine("    Catch ex As Threading.ThreadAbortException")
                    .WriteLine("    Catch ex As Rezo509Exception")
                    .WriteLine("        MessageToShow(ex.Message)")
                    .WriteLine("    Catch ex As Exception")
                    .WriteLine("        MessageToShow(ex.Message)")
                    .WriteLine("        [Global].WriteError(ex, User_Connected)")
                    .WriteLine("    End Try")
                    .WriteLine("End Sub")
                    .WriteLine()
                Next
                ''-------------------------------------------------''
                .WriteLine("#End Region")
            End With

            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""METHODES - SAVE""")
            With objWriter
                .WriteLine("Private Sub SAVE_" & nomSimple.ToUpper & "()")
                .WriteLine("    Try")
                .WriteLine("        Dim _id As Long = TypeSafeConversion.NullSafeLong(txt_Code" & nomSimple & "_Hid.Text)")
                .WriteLine("        Dim obj As New " & nomClasse & "(_id)")
                .WriteLine("        With obj")
                For i As Int32 = 1 To cols.Count - 1 '3
                    Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim ForeinKeyPrefix As String = columnToUse '.Substring(SqlServerHelper.ForeinKeyPrefix.Length, columnToUse.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        .WriteLine("." & columnToUse & "  =   DDL_" & ForeinKeyPrefix & " .SelectedValue ")

                    ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                        .WriteLine("." & columnToUse & " = rdp_" & columnToUse & " .SelectedDate ")
                    ElseIf types(i) = "Boolean" Then
                        ' .WriteLine("." & columnToUse & " =  radio_yes_" & columnToUse & " .Checked")
                        .WriteLine("." & columnToUse & " =  CB_" & columnToUse & " .Checked")
                    Else
                        .WriteLine("." & columnToUse & " = txt_" & columnToUse & ".Text ")
                    End If
                Next
                .WriteLine("End With")
                .WriteLine("obj.Save(User_Connected.Username)")
                .WriteLine("REM TRACE UTILUSATEUR / Trace Transaction")
                .WriteLine("User_Connected.Activite_Utilisateur_InRezo(IIf(_id <= 0, ""ADD "", ""EDIT "") & "" " & nomSimple & """, obj.LogData(obj) , Request.UserHostAddress)")
                .WriteLine("txt_Code" & nomSimple & "_Hid.Text=obj.ID")

                .WriteLine("    '_message = ""Sauvegarde Effectuée""")
                .WriteLine("    MessageToShow([Global].Msg_Enregistrement_Effectue, ""S"", False)")
                .WriteLine("    'RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe" & nomSimple & "();"")")
                .WriteLine("    RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe();"")")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                '.WriteLine("  End Select")
                .WriteLine("End Sub")
            End With
            objWriter.WriteLine("#End Region")

            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""EVENTS BUTTON""")
            With objWriter
                .WriteLine("Protected Sub Btn_SaveInfo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Btn_SaveInfo.Click")
                .WriteLine("SAVE_" & nomSimple.ToUpper & "()")
                .WriteLine("End Sub")
            End With

            With objWriter
                .WriteLine("Protected Sub Btn_Annuler_Click(sender As Object, e As EventArgs) Handles Btn_Annuler.Click")
                .WriteLine("    PAGE_MERE = TypeSafeConversion.NullSafeLong(Request.QueryString([Global].PAGE_MERE))")
                .WriteLine("    If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("        Select Case Request.QueryString([Global].ACTION)")
                .WriteLine("            Case [Global].HideMenuHeader")
                .WriteLine("                 RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe();"")")
                .WriteLine("            Case Else")
                .WriteLine("                Response.Redirect([Global].GetPath_PageMere(PAGE_MERE))")
                .WriteLine("            End Select")
                .WriteLine("    Else")
                .WriteLine("        Response.Redirect([Global].GetPath_PageMere(PAGE_MERE))")
                .WriteLine("    End If")
                .WriteLine("End Sub")
            End With
            objWriter.WriteLine("#End Region")

            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterface_AdminLTE_Listing_Design(ByVal name As String _
                                                                   , ByRef txt_PathGenerate_ScriptFile As TextBox _
                                                                   , ByRef ListBox_NameSpace As ListBox _
                                                                   , ByVal databasename As String)
            '  Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
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

            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            Dim _table As New Cls_Table()

            ' _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
                    count += 1
                Else
                    Exit For
                End If
            Next
            'cols.Add("_isdirty")
            'cols.Add("_LogData")
            'types.Add("Boolean")
            'types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")

            Dim nomSimpleToShow As String = Regex.Replace(nomSimple, "([a-z])?([A-Z])", "$1 $2")
            nomSimpleToShow = Regex.Replace(nomSimpleToShow, "_", " ")

            objWriter.WriteLine("<%@ Page Title=""" & nomSimpleToShow & """ Language=""VB"" MasterPageFile=""~/MasterPage/DashboardMasterPage.master"" " &
           "  AutoEventWireup=""false""  MaintainScrollPositionOnPostback=""true"" CodeFile=""Frm_" & nomSimple & "Listing.aspx.vb"" Inherits=""Frm_" & nomSimple & "Listing"" %>")

            objWriter.WriteLine("<asp:Content ID=""Content1"" ContentPlaceHolderID=""ContentPlaceHolder1"" runat=""Server"">")

            objWriter.WriteLine("<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("<script type=""text/javascript"">")

            'objWriter.WriteLine("function onRequestStart(sender, args) {")
            'objWriter.WriteLine(" if (args.get_eventTarget().indexOf(""ExportToExcelButton"") >= 0) {")
            'objWriter.WriteLine("args.set_enableAjax(false);")
            'objWriter.WriteLine(" }")
            'objWriter.WriteLine(" }")
            'objWriter.WriteLine()

            With objWriter
                .WriteLine(" function ShowAddUpdateForm(strPage, tmpW, tmpH) {")
                .WriteLine("var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
                .WriteLine("        //oWindow.set_autoSize(true);")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("        document.getElementById(""txtWindowPage"").value = strPage;")
                .WriteLine("        if (oWindow) {")
                .WriteLine("            if (!oWindow.isClosed()) {")
                .WriteLine("                oWindow.center();")
                .WriteLine("                var bounds = oWindow.getWindowBounds();")
                .WriteLine("                oWindow.moveTo(bounds.x + 'px', ""50px"");")
                .WriteLine("            }")
                .WriteLine("        }")
                .WriteLine("        return false;")
                .WriteLine("    }")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function ShowAddUpdateFormMaximized(strPage, tmpW, tmpH) { ")
                .WriteLine("    var oWindow = window.radopen(strPage, ""AddUpdateDialog""); ")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("    document.getElementById(""txtWindowPage"").value = strPage; ")
                .WriteLine("    if (oWindow) { ")
                .WriteLine("                       if (!oWindow.isClosed()) { ")
                .WriteLine("                           oWindow.center(); ")
                .WriteLine("                           var bounds = oWindow.getWindowBounds(); ")
                .WriteLine("                           oWindow.moveTo(bounds.x + 'px', ""50px""); ")
                .WriteLine("            } ")
                .WriteLine("    } ")
                .WriteLine("                   oWindow.maximize(); ")
                .WriteLine("                   return false; ")
                .WriteLine("} // ")
            End With
            objWriter.WriteLine()

            With objWriter
                .WriteLine("function ShowAddUpdateFormAutoSize(strPage, tmpW, tmpH) {")
                .WriteLine("                  var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
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
            End With

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
                .WriteLine("var listItemIndex = null;")

                .WriteLine("function MenuItemClicked(sender, eventArgs) {")
                .WriteLine("     var clickedItemValue = eventArgs.get_item().get_value();")
                .WriteLine("     var rdGrid = $find(""<%=rdg" & nomSimple & ".ClientID %>"");")
                .WriteLine("    var _id = rdGrid.get_masterTableView().get_dataItems()[listItemIndex].get_element().cells[0].innerHTML")
                .WriteLine("    switch (clickedItemValue) {")
                .WriteLine("        case ""Editer"":")
                .WriteLine("            ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID=' + _id + '&ACTION=HideMenuHeader', 950, 550); break;")
                .WriteLine("        case ""Delete"":")
                .WriteLine("            ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID=' + _id + '&ACTION=HideMenuHeader', 950, 550); break;")
                .WriteLine("        default:")
                .WriteLine("            break;")
                .WriteLine("    }")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function RowContextMenu(sender, eventArgs) {")
                .WriteLine("    var menu = $find(""<%= ContextMenu.ClientID %>"");")
                .WriteLine("    var evt = eventArgs.get_domEvent();")
                .WriteLine("    if (evt.target.tagName == ""INPUT"" || evt.target.tagName == ""A"") { return; }")
                .WriteLine("     var index = eventArgs.get_itemIndexHierarchical();")
                .WriteLine("    document.getElementById(""radGridClickedRowIndex"").value = index;")
                .WriteLine("    listItemIndex = index;")
                .WriteLine("    sender.get_masterTableView().selectItem(sender.get_masterTableView().get_dataItems()[index].get_element(), true);")
                .WriteLine("    menu.show(evt);")
                .WriteLine("    evt.cancelBubble = true;")
                .WriteLine("    evt.returnValue = false;")
                .WriteLine("    if (evt.stopPropagation) {")
                .WriteLine("        evt.stopPropagation();")
                .WriteLine("        evt.preventDefault();")
                .WriteLine("    }")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function RowDblClick(sender, eventArgs) {")
                .WriteLine("    var index = eventArgs.get_itemIndexHierarchical();")
                .WriteLine("    document.getElementById(""radGridClickedRowIndex"").value = index;")
                .WriteLine("    listItemIndex = index;")
                .WriteLine("    var rdGrid = $find(""<%=rdg" & nomSimple & ".ClientID %>"");")
                .WriteLine("    var _id = rdGrid.get_masterTableView().get_dataItems()[listItemIndex].get_element().cells[0].innerHTML")
                .WriteLine("    ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID=' + _id + '&ACTION=HideMenuHeader', 950, 550);")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function refreshMe() {")
                .WriteLine("$find(""<%= RadAjaxManager1.ClientID %>"").ajaxRequest(""Reload"");")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function closeWindow() {")
                .WriteLine(" GetRadWindow().BrowserWindow.refreshMe();")
                .WriteLine(" GetRadWindow().close();")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function CloseAndRefreshListe() {")
                .WriteLine("    GetRadWindow().BrowserWindow.refreshMe();")
                .WriteLine("    GetRadWindow().close();")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function GetRadWindow() {")
                .WriteLine("    var oWindow = null;")
                .WriteLine("    if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog")
                .WriteLine("   else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well)")
                .WriteLine("   return oWindow;")
                .WriteLine("}")
                .WriteLine()
            End With

            objWriter.WriteLine("</script>")
            objWriter.WriteLine("</telerik:RadCodeBlock>")

            objWriter.WriteLine(" <%--<telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server""> </telerik:RadScriptManager>--%>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("<AjaxSettings>")
            With objWriter
                .WriteLine("<telerik:AjaxSetting AjaxControlID=""RadAjaxManager1"">")
                .WriteLine("<UpdatedControls>")
                .WriteLine("        <telerik:AjaxUpdatedControl ControlID=""Panel_First"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
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
            'objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7""> </telerik:RadSkinManager>")
            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")
            objWriter.WriteLine("<input type='hidden' id='radGridClickedRowIndex' name='radGridClickedRowIndex' />")


            'objWriter.WriteLine("<div class=""container-fluid"" id=""pcont"">")
            'objWriter.WriteLine("<div class=""mail-inbox"">")

            With objWriter
                .WriteLine("<section class=""content-header"" ID=""PageHeader"" runat=""server"" >")
                .WriteLine("<h1>")
                .WriteLine("    <i class=""fa fa-dashboard""></i>")
                .WriteLine("    <asp:Label ID=""Label_Titre"" runat=""server"" Text=""" & nomSimpleToShow & """ /> ")
                .WriteLine("    <small id=""OL_SeeAllData"" runat=""server"">")
                .WriteLine("        <asp:Label ID=""Label_SousTitre"" runat=""server"" />")
                .WriteLine("    </small>")
                .WriteLine("</h1>")
                .WriteLine("<!--<ol class=""breadcrumb""> ")
                .WriteLine("    <li><a href=""#""><i class=""fa fa-dashboard""></i>Accueil</a></li>")
                .WriteLine("    <li class=""active"">" & nomSimpleToShow & "</li>")
                .WriteLine("</ol> -->")
                .WriteLine("</section> ")
            End With

            objWriter.WriteLine("<section class=""content"">")

            With objWriter
                .WriteLine("    <Msg:msgBox ID=""Dialogue"" runat=""server"" />")
                .WriteLine("    <asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
                .WriteLine("        <div id=""DIV_Msg"" runat=""server"" class=""alert alert-warning alert-dismissable"">")
                .WriteLine("            <i id=""Icon_Msg"" runat=""server"" class=""fa fa-warning""></i>")
                .WriteLine("            <button type=""button"" class=""close"" data-dismiss=""alert"" aria-hidden=""true"">×</button>")
                .WriteLine("            <asp:Image ID=""Image_Msg"" runat=""server"" />")
                .WriteLine("            <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
                .WriteLine("        </div>")
                .WriteLine("    </asp:Panel>")
                .WriteLine("")
            End With

            With objWriter
                .WriteLine("<asp:Panel runat=""server"" ID=""Panel_First"" style=""margin: 5px;"">")

                'Button ADD
                .WriteLine("<asp:LinkButton ID=""Btn_ADD_" & nomSimple & """ runat=""server"" CssClass=""btn btn-primary"" CausesValidation=""false"">")
                .WriteLine("    <i class=""fa fa-plus-circle"" ></i>  Ajouter " & nomSimpleToShow & "")
                .WriteLine("</asp:LinkButton>")

                'Button Clear
                .WriteLine("<span class=""pull-right box-tools"">")
                .WriteLine("    <asp:LinkButton ID=""rbtnClearFilters"" runat=""server"" CssClass=""btn btn-sm btn-default"" CausesValidation=""false""> ")
                .WriteLine("        <i class=""fa fa-ban on fa-filter"" ></i> Clear Filters")
                .WriteLine("    </asp:LinkButton>")
                .WriteLine("</span>")

                .WriteLine("")
                .WriteLine("")
                With objWriter
                    .WriteLine(" <telerik:RadGrid ID=""rdg" & nomSimple & """ AllowPaging=""True"" AllowSorting=""True"" PageSize=""20""")
                    .WriteLine(" runat=""server"" AutoGenerateColumns=""False"" GridLines=""None"" AllowFilteringByColumn=""true"" ")
                    .WriteLine("  Culture=""fr-FR"" ShowGroupPanel=""True"" ")
                    .WriteLine(" EnableViewState=""true"" AllowMultiRowSelection=""false"" GroupingSettings-CaseSensitive=""false"">")
                    .WriteLine(" <ExportSettings HideStructureColumns=""true"" />")

                    .WriteLine("  <MasterTableView CommandItemDisplay=""Top"" GridLines=""None"" DataKeyNames=""ID"" NoDetailRecordsText=""Pas d'enregistrement""")
                    .WriteLine(" NoMasterRecordsText=""Pas d'enregistrement"">")

                    .WriteLine(" <CommandItemSettings ShowAddNewRecordButton=""false"" ShowRefreshButton=""false"" ShowExportToExcelButton=""true"" ")
                    .WriteLine("  ExportToExcelText=""Exporter en excel"" />")
                    .WriteLine(" <PagerStyle Mode=""NextPrevAndNumeric""></PagerStyle>")

                    .WriteLine(" <Columns>")

                    .WriteLine("<telerik:GridBoundColumn DataField=""ID"" UniqueName=""ID"" Display=""false"" />")

                    .WriteLine("<telerik:GridTemplateColumn Visible=""true"" ShowFilterIcon=""false"" AllowFiltering=""false""  HeaderText=""#"" UniqueName=""Compteur"">")
                    .WriteLine("    <ItemTemplate>")
                    .WriteLine("        <asp:Label Visible=""true"" ID=""lbOrder"" runat=""server"" />")
                    .WriteLine("    </ItemTemplate>")
                    .WriteLine("    <HeaderStyle HorizontalAlign=""Center"" Width=""16px"" />")
                    .WriteLine("    <ItemStyle HorizontalAlign=""Center"" Width=""16px"" />")
                    .WriteLine("</telerik:GridTemplateColumn>")

                    Dim countColumn As Integer = 0
                    'Dim pourcentagevalue As Decimal = 100 / (_table.ListofColumn.Count - 4)
                    Dim pourcentagevalue As Decimal = 100 / cols.Count - 1 '- 3
                    Dim pourcentage As String = pourcentagevalue.ToString + "%"


                    For i As Int32 = 1 To cols.Count - 1 '- 3
                        If ListofForeignKey.Contains(cols(i)) Then
                            Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                            Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                            columnNameToShow = Regex.Replace(columnNameToShow, "_", "")

                            .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName.Replace("ID_", "") & "STR"" UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                            .WriteLine(" FilterControlAltText=""Recherche par " & columnNameToShow & """ FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine("  AllowFiltering=""true"" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            '.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")


                        ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                            Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                            Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                            columnNameToShow = Regex.Replace(columnNameToShow, "_", "")

                            .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                            .WriteLine(" FilterControlAltText=""Recherche par " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AllowFiltering=""true"" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")

                        ElseIf types(i) = "Boolean" And cols(i) <> "isdirty" Then
                            Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                            Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                            columnNameToShow = Regex.Replace(columnNameToShow, "_", "")

                            .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                            .WriteLine(" FilterControlAltText=""Filter " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AllowFiltering=""false"" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            '.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        Else
                            Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                            Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                            columnNameToShow = Regex.Replace(columnNameToShow, "_", "")

                            .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                            .WriteLine(" FilterControlAltText=""Filter " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AllowFiltering=""true"" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            '.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        End If
                    Next

                    .WriteLine(" <telerik:GridButtonColumn ButtonType=""ImageButton"" CommandArgument=""ID"" CommandName=""editer""")
                    .WriteLine("        DataTextField=""ID"" ImageUrl=""~/images/_edit.png""")
                    .WriteLine("          HeaderText="""" UniqueName=""editer"">")
                    .WriteLine("        <HeaderStyle HorizontalAlign=""Center"" Width=""16px"" />")
                    .WriteLine("        <ItemStyle  HorizontalAlign=""Center"" Width=""16px"" />")
                    .WriteLine("    </telerik:GridButtonColumn>")

                    .WriteLine("<telerik:GridButtonColumn ButtonType=""ImageButton"" CommandName=""delete"" DataTextField=""ID""")
                    .WriteLine(" ImageUrl=""~/images/delete.png"" ")
                    .WriteLine("UniqueName=""delete"" HeaderText="""" ConfirmDialogType=""RadWindow"" ConfirmText=""Voulez-vous vraiment supprimer cette information ?""")
                    .WriteLine("ConfirmTitle=""Attention!"">")
                    .WriteLine("<HeaderStyle  HorizontalAlign=""Center"" Width=""16px""  />")
                    .WriteLine("<ItemStyle  HorizontalAlign=""Center"" Width=""16px""  />")
                    .WriteLine("</telerik:GridButtonColumn>")

                    .WriteLine("</Columns>")
                    .WriteLine("<RowIndicatorColumn FilterControlAltText=""Filter RowIndicator column""></RowIndicatorColumn>")
                    .WriteLine("<ExpandCollapseColumn FilterControlAltText=""Filter ExpandColumn column""></ExpandCollapseColumn>")
                    .WriteLine("</MasterTableView>")

                    .WriteLine("<GroupingSettings CaseSensitive=""False"" />")

                    .WriteLine("<ClientSettings  AllowDragToGroup=""True"" AllowColumnsReorder=""True"">")
                    .WriteLine("<ClientEvents OnRowContextMenu=""RowContextMenu"" OnRowDblClick=""RowDblClick"" />")
                    .WriteLine("<Selecting AllowRowSelect=""true"" />")
                    .WriteLine("</ClientSettings>")

                    .WriteLine("<HeaderContextMenu CssClass=""GridContextMenu GridContextMenu_Default"" />")

                    .WriteLine("<PagerStyle PageSizeControlType=""RadComboBox"" />")

                    .WriteLine("<FilterMenu EnableImageSprites=""False""></FilterMenu>")
                    .WriteLine("</telerik:RadGrid>")
                End With



                .WriteLine("</asp:Panel>") 'FIN PANEL

                .WriteLine("<!-- FORM LOGIN -->")
                .WriteLine("<BRAIN:CULogin2 runat=""server"" ID=""LoginWUC"" Visible=""false"" />")
                .WriteLine("<div class=""md-overlay""></div>")
            End With
            objWriter.WriteLine("</section>") ' FIN Section Content
            objWriter.WriteLine("")
            objWriter.WriteLine("<asp:Literal runat=""server"" ID=""LiteralStyleCSS""></asp:Literal>")

            'objWriter.WriteLine("</div>") 'END DIV mail-inbox
            'objWriter.WriteLine("</div>") 'END DIV pcont

            objWriter.WriteLine("<telerik:RadWindowManager ID=""RadWindowManager1"" runat=""server"" VisibleStatusbar=""false"" EnableViewState=""false"">")
            objWriter.WriteLine("   <Windows>")
            objWriter.WriteLine("       <telerik:RadWindow ID=""AddUpdateDialog"" runat=""server"" Title="""" IconUrl=""~/Images/favicon.ico"" Left=""75px"" ReloadOnShow=""true""")
            objWriter.WriteLine("       ShowContentDuringLoad=""false"" Modal=""true"" OnClientClose=""RadWindowClosing"" Behaviors=""Reload, Move, Resize, Maximize, Close""")
            objWriter.WriteLine("       EnableShadow=""false"" OnClientResizeEnd=""RadWindowClientResizeEnd"" />")
            objWriter.WriteLine("   </Windows>")
            objWriter.WriteLine("</telerik:RadWindowManager>")

            objWriter.WriteLine("<telerik:RadContextMenu ID=""ContextMenu"" runat=""server"" OnClientItemClicked=""MenuItemClicked"" EnableRoundedCorners=""true"" EnableShadows=""true"">")
            objWriter.WriteLine("   <Items>")
            objWriter.WriteLine("       <telerik:RadMenuItem Visible=""true"" Value=""Editer"" Text=""Editer"" ImageUrl=""~/images/_edit.png"" HoveredImageUrl=""~/images/_edit.png"" />")
            objWriter.WriteLine("       <telerik:RadMenuItem Visible=""true"" Value=""Delete"" Text=""Supprimer"" ImageUrl=""~/images/delete.png"" HoveredImageUrl=""~/images/delete.png"" />")
            objWriter.WriteLine("   </Items>")
            objWriter.WriteLine("</telerik:RadContextMenu>")

            objWriter.WriteLine("<input id=""txtWindowPage"" type=""hidden"" />")

            objWriter.WriteLine("</asp:Content>")
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterface_AdminLTE_Listing_CodeBehind(ByVal name As String _
                                                                       , ByRef txt_PathGenerate_ScriptFile As TextBox _
                                                                       , ByRef ListBox_NameSpace As ListBox _
                                                                       , ByRef txt_libraryname As TextBox _
                                                                       , ByVal databasename As String)
            ' Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
            Dim path As String = txt_PathGenerate_Script & nomWebform & "Listing.aspx.vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            Dim cult As Globalization.CultureInfo = New Globalization.CultureInfo("en-EN")
            Threading.Thread.CurrentThread.CurrentCulture = cult

            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy hh:mm tt")
            'header &= ""
            Dim content As String = "Partial Class " & nomWebform & "Listing" & Chr(13) _
                                     & " Inherits Cls_BasePage ' LA CLASSE DE LA PAGE HERITE DE CETTE CLASSE DANS LE CAS OU NOUS AVONS UNE APPLICATION WEB multilingue"

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
            Dim libraryname As String = "Imports " & txt_libraryname.Text
            objWriter.WriteLine("Imports Telerik.Web.UI")
            objWriter.WriteLine(libraryname)
            objWriter.WriteLine()

            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            objWriter.WriteLine(content)
            objWriter.WriteLine()


            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
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

            'objWriter.WriteLine("Dim _out As Boolean = False")
            'objWriter.WriteLine("Dim _tmpEditState As Boolean = False")

            With objWriter
                .WriteLine("")
                .WriteLine("#Region ""ATTRIBUTS"" ")
                .WriteLine("    Private _message As String  ' VARIABLE SERVANT A LA RECUPERATION DE TOUS LES MESSAGES D'ECHECS OU DE SUCCES")
                .WriteLine("")
                .WriteLine("    REM DEFINITION ET INITIALISATION DES CONSTANTE POUR LA SECURITE")
                .WriteLine("    Private Const Nom_page As String = ""PAGE-LISTING-" & nomSimple.ToUpper & """  ' POUR LA PAGE")
                .WriteLine("    Private Const Btn_Save As String = ""Bouton-SAVE-" & nomSimple.ToUpper & """       ' POUR LE BOUTON D'ENREGISTREMENT")
                .WriteLine("    Private Const Btn_Edit As String = ""Bouton-EDIT-" & nomSimple.ToUpper & """       ' POUR LE BOUTON DE MODIFICATION")
                .WriteLine("    Private Const Btn_Delete As String = ""Bouton-DELETE-" & nomSimple.ToUpper & """   ' POUR LE BOUTON DE SUPPRESSION")
                .WriteLine("")
                .WriteLine("    Dim User_Connected As Cls_User          ' INSTANCE DE LA CLASSE UTILISATEUR - UTILISER POUR L'UTILISATEUR EN SESSION ")
                .WriteLine("    Dim Is_Acces_Page As Boolean = True     ' LA VARIABLE SERVANT DE TEST POUR DONNEER L'ACCES A LA PAGE")
                .WriteLine("    Dim GetOut As Boolean = False           ' LA VARIABLE SERVANT DE TEST POUR REDIRIGER L'UTILISATEUR VERS LA PAGE DE CONNEXION")
                .WriteLine("    Dim PAGE_MERE As Long = 0' PAS TROP IMPORTANT...")
                .WriteLine("    Dim PAGE_TITLE As String = """" ")
                .WriteLine("#End Region")
            End With
            objWriter.WriteLine("")

            With objWriter
                .WriteLine(" Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load")
                .WriteLine("    Response.Cache.SetCacheability(HttpCacheability.NoCache) ")
                .WriteLine("    Response.Expires = -1 ")
                .WriteLine("    Panel_Msg.Visible = False ")

                Dim nomSimpleToShow As String = Regex.Replace(nomSimple, "([a-z])?([A-Z])", "$1 $2")

                .WriteLine("    PAGE_TITLE = """ & nomSimpleToShow & """")
                .WriteLine("    Page.Title = [Global].Global_APP_NAME_SIGLE & "" | "" & PAGE_TITLE")
                .WriteLine(" ")
                .WriteLine("    SYSTEME_SECURITE()  ' APPEL A LA METHODE SERVANT A TESTER LES COMPOSANTS DE LA PAGE Y COMPRIS LA PAGE ELLE MEME ")
                .WriteLine("")
                .WriteLine("    '--- Si l'utilisateur n'a Access a la page les informations ne sont pas charger dans la Page_Load ")
                .WriteLine("    If Is_Acces_Page Then ")
                .WriteLine("        If Not IsPostBack Then ")
                .WriteLine("            Label_Titre.Text = PAGE_TITLE")
                .WriteLine("            Btn_ADD_" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?"" & [Global].ACTION & ""="" & [Global].HideMenuHeader & ""', 950, 650)); return false;"")") '& Chr(13) _
                .WriteLine("            'Btn_ADD_" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:Open_Window('Frm_" & nomSimple & "ADD.aspx', '_self',500,400); return false;"") ")
                .WriteLine("            BindGrid() ")
                .WriteLine("        End If ")
                .WriteLine("    End If ")
                .WriteLine("End Sub")
                .WriteLine(" ")
                .WriteLine(" ")
            End With

            With objWriter
                .WriteLine("#Region ""SECURITE""")
                .WriteLine("Public Sub SYSTEME_SECURITE()")
                .WriteLine("    Try")
                .WriteLine("        User_Connected = [Global].KeepUserContinuesToWork(User_Connected)")
                .WriteLine("")
                .WriteLine("        'CType(Page.Master.FindControl(""li_" & nomSimple & """), HtmlControl).Attributes.Add(""class"", ""active "")")
                .WriteLine("        'CType(Page.Master.FindControl(""i_" & nomSimple & """), HtmlControl).Attributes.Add(""class"", ""fa fa-folder-open fa-lg "")")

                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liGROUPE_PARAMETRES""), HtmlControl).Attributes.Add(""class"", ""active treeview"")")
                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liCentreDeDetentionListe""), HtmlControl).Attributes.Add(""class"", ""active"")")

                .WriteLine("        LiteralStyleCSS.Text = """" ")
                .WriteLine("        If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("            Select Case Request.QueryString([Global].ACTION)")
                .WriteLine("                Case [Global].HideMenuHeader")
                .WriteLine("                    CType(Page.Master.FindControl([Global].Head_Nav_Menu), HtmlControl).Visible = False")
                .WriteLine("                    CType(Page.Master.FindControl([Global].Head_Nav_MenuVertical), HtmlControl).Visible = False")
                .WriteLine("                    Dim StyleCss As String = ""<style type=""""text/css""""> #cl-wrapper { padding-top: 0px; } </style>""")
                .WriteLine("                    LiteralStyleCSS.Text = StyleCss")
                .WriteLine("                Case Else")
                .WriteLine("                    'span_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("                    'Btn_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("                End Select")
                .WriteLine("        Else")
                .WriteLine("            'span_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("            'Btn_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) IsNot Nothing Then")
                .WriteLine("            User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)")
                .WriteLine("            If Not Cls_Privilege.VerifyRightOnObject(Nom_page, User_Connected.IdGroupeuser) Then    ' VERIFICATION SI L'UTILISATEUR N'A PAS ACCES A LA PAGE")
                .WriteLine("                _message = [Global].NO_ACCES_PAGE")
                .WriteLine("                MessageToShow(_message)")
                .WriteLine("                Is_Acces_Page = False")
                .WriteLine(" ")
                .WriteLine("                Panel_First.Visible = False")
                .WriteLine("            Else    ' SI L'UTILISATEUR A ACCES A LA PAGE ON VERIFIE POUR LES BOUTONS ET LES LIENS")
                .WriteLine("                '---  Okey vous avez acces a la page ---'")
                .WriteLine("                Dim _check As Boolean = Cls_Privilege.VerifyRightOnObject(Btn_Save, User_Connected.IdGroupeuser)")
                .WriteLine("                Btn_ADD_" & nomSimple & ".Visible = _check")
                .WriteLine("                rdg" & nomSimple & ".MasterTableView.Columns.FindByUniqueNameSafe(""editer"").Visible = _check")

                .WriteLine("                If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("                    If Request.QueryString([Global].ACTION).Equals([Global].HideMenuHeader) Then")
                .WriteLine("                        Btn_ADD_" & nomSimple & ".Visible = _check")
                .WriteLine("                    End If")
                .WriteLine("                End If")

                .WriteLine("                _check = Cls_Privilege.VerifyRightOnObject(Btn_Delete, User_Connected.IdGroupeuser)")
                .WriteLine("                rdg" & nomSimple & ".MasterTableView.Columns.FindByUniqueNameSafe(""delete"").Visible = _check")
                .WriteLine("            End If")
                .WriteLine("        End If")
                .WriteLine(" ")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) Is Nothing Then")
                .WriteLine("            '-- Session expirée --'")
                .WriteLine("            GetOut = True")
                .WriteLine("        Else")
                .WriteLine("            Try")
                .WriteLine("                User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)  ' ON VERIFIE SI LÚTILISATEUR A ETE FORCE DE SE CONNECTER 'PAR L'ADM")
                .WriteLine("                If Not (GlobalFunctions.IsUserStillConnected(User_Connected) And GlobalFunctions.IsUserStillActive(User_Connected)) Then")
                .WriteLine("                    User_Connected.Set_Status_ConnectedUser(False)")
                .WriteLine("                    User_Connected.Activite_Utilisateur_InRezo(""Forced Log Off"", ""Forced to Log Off"", Request.UserHostAddress)")
                .WriteLine("")
                .WriteLine("                    GetOut = True")
                .WriteLine("                    Session.RemoveAll()")
                .WriteLine("                    '_message = ""Session expirée.""")
                .WriteLine("                    'MessageToShow(_message)")
                .WriteLine("                    Is_Acces_Page = True")
                .WriteLine("                End If")
                .WriteLine("            Catch ex As Exception")
                .WriteLine("                GetOut = True")
                .WriteLine("                '_message = ""Session expirée.""")
                .WriteLine("                'MessageToShow(_message)")
                .WriteLine("            End Try")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("    If GetOut Then ' REDIRECTIONNEMENT DE L'UTILISATUER OU PAS.")
                .WriteLine("        CType(Page.Master.FindControl([Global].htmlMasterPage), HtmlControl).Attributes.Add(""class"", ""lockscreen"")")
                .WriteLine("        CType(Page.Master.FindControl([Global].bodyMasterPage), HtmlControl).Attributes.Add(""class"", ""texture"")")
                .WriteLine("        CType(Page.Master.FindControl([Global].Head_Nav_Menu), HtmlControl).Visible = False")
                .WriteLine("        CType(Page.Master.FindControl([Global].Head_Nav_MenuVertical), HtmlControl).Visible = False")
                .WriteLine("        ")
                .WriteLine("        Is_Acces_Page = False")
                .WriteLine("        PageHeader.Attributes.Add(""style"", ""visibility:hidden;"")")
                .WriteLine("        Panel_First.Visible = False")
                .WriteLine("        LoginWUC.Visible = True")
                .WriteLine("        Session([Global].GLOBAL_PAGENAME) = System.Web.HttpContext.Current.Request.Url.ToString()")
                .WriteLine("        'Response.Redirect([Global].PAGE_LOGIN)")
                .WriteLine("    End If")

                .WriteLine("")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        Is_Acces_Page = False")
                .WriteLine("        Panel_First.Visible = False")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine()
                .WriteLine("#Region ""Other Method""")
                .WriteLine("    Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"", Optional ByVal ShowPopUp As Boolean = True)")
                .WriteLine("        Panel_Msg.Visible = True")
                .WriteLine("        GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
                .WriteLine("        Label_Msg.Text = _message")
                .WriteLine("        If ShowPopUp Then")
                .WriteLine("            RadAjaxManager1.ResponseScripts.Add(""alert('"" & [Global].GetTextFromHtml(_message).Replace(""'"", ""\'"") & ""');"")")
                .WriteLine("            'Dialogue.alert([Global].GetTextFromHtml(_message))")
                .WriteLine("        End If")
                .WriteLine("        If E_or_S = ""S"" Then")
                .WriteLine("            Style_Division(DIV_Msg, ""alert alert-success alert-dismissable"")")
                .WriteLine("            Style_Division(Icon_Msg, ""fa  fa-thumbs-up"")")
                .WriteLine("        Else")
                .WriteLine("            Style_Division(DIV_Msg, ""alert alert-danger alert-dismissable"")")
                .WriteLine("            Style_Division(Icon_Msg, ""fa  fa-thumbs-down"")")
                .WriteLine("        End If")
                .WriteLine("    End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With


            With objWriter
                .WriteLine("#Region ""Load DATA""")
                .WriteLine("Private Sub BindGrid(Optional ByVal _refresh As Boolean = True )")
                .WriteLine("    Dim objs As List(of Cls_" & nomSimple & ")")
                .WriteLine("    Dim _ret As Long = 0")
                .WriteLine("    Try")
                .WriteLine("        objs = Cls_" & nomSimple & ".SearchAll")
                .WriteLine("        rdg" & nomSimple & ".DataSource = objs")
                .WriteLine("        If _refresh Then")
                .WriteLine("            rdg" & nomSimple & ".DataBind()")
                .WriteLine("        End If")
                .WriteLine("         _ret = objs.Count")
                .WriteLine("         Label_Titre.Text = PAGE_TITLE & ""  <small class=""""badge badge-primary"""">"" & _ret & ""</small>""")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""EVENTS CONTROLS""")
                .WriteLine("")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""ACTIONS / METHODES""")
                .WriteLine("")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""RADGRID EVENTS""")
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemCommand(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs)  Handles rdg" & nomSimple & ".ItemCommand")
                .WriteLine("    Try")
                .WriteLine("        If e.CommandName = Telerik.Web.UI.RadGrid.ExportToExcelCommandName Then")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.ExportOnlyData = True")
                .WriteLine("            rdg" & nomSimple & ".GridLines = GridLines.Both")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.IgnorePaging = True")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.OpenInNewWindow = False")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.FileName = ""Liste des " & nomSimple & """")
                .WriteLine("            rdg" & nomSimple & ".MasterTableView.Columns(0).Visible = False")
                .WriteLine("            rdg" & nomSimple & ".MasterTableView.ExportToExcel()")
                .WriteLine("        End If")
                .WriteLine()
                .WriteLine("        Dim _id As Long = TypeSafeConversion.NullSafeLong(e.CommandArgument)")
                .WriteLine("        Select Case e.CommandName")
                .WriteLine("            Case ""delete""")
                .WriteLine("                Dim obj As New Cls_" & nomSimple & "(_id)")
                .WriteLine("                obj.Delete()")
                .WriteLine("                User_Connected.Activite_Utilisateur_InRezo(""DELETE "" & PAGE_TITLE, obj.LogData(obj), Request.UserHostAddress)")
                .WriteLine("                'User_Connected.Activite_Utilisateur_InRezo(""DELETE " & nomSimple & " "", obj.ID & "" - Code:"" & obj.Titrerapport & "" Prop:"", Request.UserHostAddress)")
                .WriteLine("                MessageToShow([Global].Msg_Information_Supprimee_Avec_Succes, ""S"")")
                .WriteLine("                rdg" & nomSimple & ".Rebind()")
                .WriteLine("        End Select")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rdg" & nomSimple & ".ItemDataBound")
                .WriteLine("Try")
                .WriteLine("    Dim gridDataItem = TryCast(e.Item, GridDataItem)")
                .WriteLine("    If e.Item.ItemType = GridItemType.Item Or e.Item.ItemType = GridItemType.AlternatingItem Then")
                .WriteLine("        'Dim _lnk As HyperLink = DirectCast(gridDataItem.FindControl(""hlk""), HyperLink)")
                .WriteLine("        'Dim _lbl_ID As Label = DirectCast(gridDataItem.FindControl(""lbl_ID""), Label)")
                .WriteLine("        '_lnk.Attributes.Clear()")
                .WriteLine("        '_lnk.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID="" & CLng(_lbl_ID.Text) & ""', 750, 400));"")")
                .WriteLine("    End If")
                .WriteLine()
                .WriteLine("    If (gridDataItem IsNot Nothing) Then")
                .WriteLine("        Dim item As GridDataItem = gridDataItem")
                .WriteLine("        CType(item.FindControl(""lbOrder""), Label).Text = rdg" & nomSimple & ".PageSize * rdg" & nomSimple & ".CurrentPageIndex + (item.RowIndex / 2)")
                .WriteLine("")
                .WriteLine("        Dim imagedelete As ImageButton = CType(item(""delete"").Controls(0), ImageButton)")
                .WriteLine("        Dim imageediter As ImageButton = CType(item(""editer"").Controls(0), ImageButton)")
                .WriteLine("        imagedelete.ToolTip = ""Effacer"" ")
                .WriteLine("        imageediter.ToolTip = ""Editer"" ")

                .WriteLine("        imagedelete.CommandArgument = CType(DataBinder.Eval(e.Item.DataItem, ""ID""), String)")
                .WriteLine("        imageediter.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID="" & CType(DataBinder.Eval(e.Item.DataItem, ""ID""), Long) & ""&"" & [Global].ACTION & ""="" & [Global].HideMenuHeader & ""',900,650));"")")
                .WriteLine("        REM Privilege")
                .WriteLine("        'imageediter.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Save, User_Connected.IdGroupeuser)")
                .WriteLine("        'imagedelete.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Delete, User_Connected.IdGroupeuser)")
                .WriteLine("    End If")

                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles rdg" & nomSimple & ".NeedDataSource")
                .WriteLine("    If IsPostBack Then")
                .WriteLine("        BindGrid(False)")
                .WriteLine("    End If")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rbtnClearFilters_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnClearFilters.Click")
                .WriteLine("    Try")
                .WriteLine("        For Each column As GridColumn In rdg" & nomSimple & ".MasterTableView.Columns")
                .WriteLine("            column.CurrentFilterFunction = GridKnownFunction.NoFilter")
                .WriteLine("            column.CurrentFilterValue = String.Empty")
                .WriteLine("        Next")
                .WriteLine("        rdg" & nomSimple & ".MasterTableView.FilterExpression = String.Empty")
                .WriteLine("        rdg" & nomSimple & ".MasterTableView.Rebind()")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub RadAjaxManager1_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs) Handles RadAjaxManager1.AjaxRequest")
                .WriteLine("    Try")
                .WriteLine("        Select Case e.Argument")
                .WriteLine("            Case ""Reload""")
                .WriteLine("                BindGrid(True)")
                .WriteLine("        End Select")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#End Region")
                .WriteLine()
            End With

            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub
#End Region

#Region "TEMPLATE CleanZone"
        Public Shared Sub CreateInterface_Tableau_Formulaire_Design(ByVal name As String _
                                                                         , ByRef txt_PathGenerate_ScriptFile As TextBox _
                                                                         , ByRef ListBox_NameSpace As ListBox _
                                                                         , ByVal databasename As String)
            '  Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
            Dim path As String = txt_PathGenerate_Script & nomClasse & "ADD.aspx"

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

            'objWriter.WriteLine()

            'objWriter.WriteLine()

            Dim _table As New Cls_Table()
            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            '  _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next


            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If

            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next

            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
                    count += 1
                Else
                    Exit For
                End If
            Next

            'cols.Add("_isdirty")
            'cols.Add("_LogData")
            'types.Add("Boolean")
            'types.Add("String")
            'initialtypes.Add("Byte")
            'initialtypes.Add("nvarchar")

            Dim nomSimpleToShow As String = Regex.Replace(nomSimple, "([a-z])?([A-Z])", "$1 $2")

            objWriter.WriteLine("<%@ Page Title=""" & nomSimpleToShow & """ Language=""VB"" MasterPageFile=""~/MasterPages/DashboardCZMasterPage.master"" " & _
           " AutoEventWireup=""false"" CodeFile=""Frm_" & nomSimple & "ADD.aspx.vb"" Inherits=""Frm_" & nomSimple & "ADD""  MaintainScrollPositionOnPostback=""true"" %>")

            objWriter.WriteLine("<asp:Content ID=""Content1"" ContentPlaceHolderID=""ContentPlaceHolder1"" runat=""Server"">")

            objWriter.WriteLine("<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("<script type=""text/javascript"">")


            With objWriter
                .WriteLine(" function ShowAddUpdateForm(strPage, tmpW, tmpH) {")
                .WriteLine("var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
                .WriteLine("        //oWindow.set_autoSize(true);")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("        document.getElementById(""txtWindowPage"").value = strPage;")
                .WriteLine("        if (oWindow) {")
                .WriteLine("            if (!oWindow.isClosed()) {")
                .WriteLine("                oWindow.center();")
                .WriteLine("                var bounds = oWindow.getWindowBounds();")
                .WriteLine("                oWindow.moveTo(bounds.x + 'px', ""50px"");")
                .WriteLine("            }")
                .WriteLine("        }")
                .WriteLine("        return false;")
                .WriteLine("    }")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function ShowAddUpdateFormMaximized(strPage, tmpW, tmpH) { ")
                .WriteLine("    var oWindow = window.radopen(strPage, ""AddUpdateDialog""); ")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("    document.getElementById(""txtWindowPage"").value = strPage; ")
                .WriteLine("    if (oWindow) { ")
                .WriteLine("                       if (!oWindow.isClosed()) { ")
                .WriteLine("                           oWindow.center(); ")
                .WriteLine("                           var bounds = oWindow.getWindowBounds(); ")
                .WriteLine("                           oWindow.moveTo(bounds.x + 'px', ""50px""); ")
                .WriteLine("            } ")
                .WriteLine("    } ")
                .WriteLine("                   oWindow.maximize(); ")
                .WriteLine("                   return false; ")
                .WriteLine("} // ")
            End With
            objWriter.WriteLine()

            With objWriter
                .WriteLine("function ShowAddUpdateFormAutoSize(strPage, tmpW, tmpH) {")
                .WriteLine("                  var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
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
            End With

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

            objWriter.WriteLine("function CloseAndRefreshListe() {")
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

            objWriter.WriteLine("<%--<telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server""></telerik:RadScriptManager>--%>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("   <AjaxSettings>")
            objWriter.WriteLine("       <%--<telerik:AjaxSetting AjaxControlID=""Btn_SaveInfo"">")
            objWriter.WriteLine("           <UpdatedControls>")
            objWriter.WriteLine("               <telerik:AjaxUpdatedControl ControlID=""Panel_Msg"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("               <telerik:AjaxUpdatedControl ControlID=""Panel_First"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
            objWriter.WriteLine("           </UpdatedControls>")
            objWriter.WriteLine("       </telerik:AjaxSetting>--%>")
            objWriter.WriteLine("   </AjaxSettings>")
            objWriter.WriteLine("</telerik:RadAjaxManager>")
            'objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7""> </telerik:RadSkinManager>")
            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")


            objWriter.WriteLine("<div class=""container-fluid"" id=""pcont"">")
            objWriter.WriteLine("<div class=""mail-inbox"">")

            With objWriter
                .WriteLine("<section class=""page-head"" ID=""PageHeader"" runat=""server"" >")
                .WriteLine("<h3>")
                .WriteLine("    <i class=""fa fa-dashboard""></i>")
                .WriteLine("    <asp:Label ID=""Label_Titre"" runat=""server"" Text=""" & nomSimple & """ /> ")
                .WriteLine("    <small id=""OL_SeeAllData"" runat=""server"">")
                .WriteLine("        <asp:Label ID=""Label_SousTitre"" runat=""server"" />")
                .WriteLine("    </small>")
                .WriteLine("</h3>")
                .WriteLine("<!--<ol class=""breadcrumb""> ")
                .WriteLine("    <li><a href=""#""><i class=""fa fa-dashboard""></i>Accueil</a></li>")
                .WriteLine("    <li class=""active"">" & nomSimple & "</li>")
                .WriteLine("</ol> -->")
                .WriteLine("</section> ") 'end section 1
            End With

            objWriter.WriteLine("<section class=""content"">")

            With objWriter
                .WriteLine("    <Msg:msgBox ID=""Dialogue"" runat=""server"" />")
                .WriteLine("    <asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
                .WriteLine("        <div id=""DIV_Msg"" runat=""server"" class=""alert alert-warning alert-dismissable"">")
                .WriteLine("            <i id=""Icon_Msg"" runat=""server"" class=""fa fa-warning""></i>")
                .WriteLine("            <button type=""button"" class=""close"" data-dismiss=""alert"" aria-hidden=""true"">×</button>")
                .WriteLine("            <asp:Image ID=""Image_Msg"" runat=""server"" />")
                .WriteLine("            <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
                .WriteLine("        </div>")
                .WriteLine("    </asp:Panel>")
                .WriteLine("")
            End With

            objWriter.WriteLine("<asp:ValidationSummary ID=""ValidationSummary1"" ValidationGroup=""GPSave""  runat=""server"" CssClass=""alert alert-danger alert-dismissable"" ShowMessageBox=""true"" ShowSummary=""true"" />")

            objWriter.WriteLine("")
            objWriter.WriteLine("<asp:Panel ID=""Panel_First"" runat=""server"" CssClass=""panel panel-default panel-body"" Style=""margin: 5px;"">")

            REM DEBUT TABLEAU
            objWriter.WriteLine("<div id=""DIV_Panel"" class=""form-horizontal group-border-dashed1"" style=""border-radius: 0px;"">")
            Dim countColumn As Integer = 0

            For i As Int32 = 1 To cols.Count - 1 '3
                objWriter.WriteLine("   <div class=""form-group"">")

                If ListofForeignKey.Contains(cols(i)) Then
                    Dim columnName As String = cols(i) '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                    If Not columnName.Equals("CreatedBy") Then

                    End If

                    objWriter.WriteLine("<label class=""col-sm-3 control-label"">" & columnNameToShow & "")
                    objWriter.WriteLine("   <asp:RequiredFieldValidator  ID=""RFV" & columnName & """ runat=""server"" ControlToValidate=""DDL" & columnName & """  ")
                    objWriter.WriteLine("       ErrorMessage=""" & columnNameToShow & " Obligatoire !"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" ")
                    objWriter.WriteLine("       ValidationGroup=""GPSave"" CssClass=""text-danger"" /> ")
                    objWriter.WriteLine("   <asp:RequiredFieldValidator  ID=""RFV1" & columnName & """ runat=""server"" ControlToValidate=""DDL" & columnName & """  ")
                    objWriter.WriteLine("       ErrorMessage=""" & columnNameToShow & " Obligatoire !"" InitialValue=""0"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" ")
                    objWriter.WriteLine("       ValidationGroup=""GPSave"" CssClass=""text-danger"" /> ")
                    objWriter.WriteLine("</label>")
                    objWriter.WriteLine("<div class=""col-sm-6"">")
                    objWriter.WriteLine("   <asp:DropDownList ID=""DDL" & columnName & """ CssClass=""select2"" Width=""100%"" runat=""server"" >")
                    objWriter.WriteLine("   </asp:DropDownList>")
                    objWriter.WriteLine("</div>")
                    objWriter.WriteLine("")

                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                    columnNameToShow = Regex.Replace(columnNameToShow, "_", " ")

                    objWriter.WriteLine("<label class=""col-sm-3 control-label"">" & columnNameToShow & "")
                    objWriter.WriteLine("   <asp:RequiredFieldValidator  ID=""RFV_" & columnName & """ runat=""server"" ControlToValidate=""rdp_" & columnName & """  ")
                    objWriter.WriteLine("       ErrorMessage=""" & columnNameToShow & " Obligatoire !"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" ")
                    objWriter.WriteLine("       ValidationGroup=""GPSave"" CssClass=""text-danger"" /> ")
                    objWriter.WriteLine("</label>")
                    objWriter.WriteLine("<div class=""col-sm-6"">")
                    objWriter.WriteLine("   <telerik:RadDatePicker ID=""rdp_" & columnName & """ placeholder=""" & columnNameToShow & """ Width=""100%""")
                    objWriter.WriteLine("       runat=""server"" DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01"" ToolTip=""Cliquer sur le bouton pour choisir une date""")
                    objWriter.WriteLine("       Skin=""MetroTouch"">")
                    objWriter.WriteLine("       <Calendar runat=""server"" Skin=""MetroTouch"" UseColumnHeadersAsSelectors=""False"" UseRowHeadersAsSelectors=""False"" CultureInfo=""fr-FR""></Calendar>")
                    objWriter.WriteLine("       <DateInput runat=""server"" DateFormat=""dd-MMM-yyyy"" DisplayDateFormat=""dd MMM yyyy"" LabelWidth=""40%"" Culture=""fr-FR""></DateInput>")
                    objWriter.WriteLine("   </telerik:RadDatePicker>")
                    objWriter.WriteLine("</div>")
                    objWriter.WriteLine("")

                ElseIf types(i) = "Boolean" And cols(i) <> "isdirty" Then
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                    columnNameToShow = Regex.Replace(columnNameToShow, "_", " ")

                    objWriter.WriteLine("<label class=""col-sm-3 control-label"">" & columnNameToShow & "")
                    objWriter.WriteLine("</label>")
                    objWriter.WriteLine("<div class=""col-sm-6"">")
                    objWriter.WriteLine("   <label><asp:CheckBox ID=""CB_" & columnName & """ runat=""server"" text=""" & columnName & """ /></label>  ")
                    objWriter.WriteLine("</div>")
                    objWriter.WriteLine("")
                Else
                    Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                    Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])_?([A-Z])", "$1 $2")
                    columnNameToShow = Regex.Replace(columnNameToShow, "_", " ")

                    objWriter.WriteLine("<label class=""col-sm-3 control-label"">" & columnNameToShow & "")
                    objWriter.WriteLine("   <asp:RequiredFieldValidator  ID=""RFV_" & columnName & """ runat=""server"" ControlToValidate=""txt_" & columnName & """  ")
                    objWriter.WriteLine("       ErrorMessage=""" & columnNameToShow & " Obligatoire !"" SetFocusOnError=""true"" Display=""Dynamic"" Text=""*"" ")
                    objWriter.WriteLine("       ValidationGroup=""GPSave"" CssClass=""text-danger"" /> ")
                    objWriter.WriteLine("</label>")
                    objWriter.WriteLine("<div class=""col-sm-6"">")
                    objWriter.WriteLine("   <asp:TextBox ID=""txt_" & columnName & """ CssClass=""form-control"" Width=""100%"" runat=""server"" placeholder=""" & columnNameToShow & "...""></asp:TextBox>")
                    objWriter.WriteLine("</div>")
                    objWriter.WriteLine("")
                End If
                objWriter.WriteLine(" </div>") 'FIN DIV form-group
            Next

            With objWriter
                .WriteLine("<div id=""DIV_SaveInfo"" runat=""server"" style=""margin: 5px 0px; text-align: left;"">")
                .WriteLine("    <span id=""span_SaveInfo"" runat=""server"">")
                .WriteLine("        <asp:LinkButton ID=""Btn_SaveInfo"" runat=""server"" CssClass=""btn btn-primary"" ValidationGroup=""GPSave"">")
                .WriteLine("            <i class=""fa fa-save"" ></i> Enregistrer")
                .WriteLine("        </asp:LinkButton>")
                .WriteLine("    </span>")
                .WriteLine("    ")
                .WriteLine("&nbsp;")
                .WriteLine("    <asp:LinkButton ID=""Btn_Annuler"" CausesValidation=""false"" runat=""server"" CssClass=""btn btn-danger"">")
                .WriteLine("        <i class=""fa  fa-reply-all"" ></i> Annuler")
                .WriteLine("    </asp:LinkButton>")
                .WriteLine("</div>")
            End With
            objWriter.WriteLine(" </div>") 'FIN DIV TABLEAU

            objWriter.WriteLine("   </asp:Panel>") 'FIN PANEL

            With objWriter
                .WriteLine("<!-- FORM LOGIN -->")
                .WriteLine("<BRAIN:CULogin2 runat=""server"" ID=""LoginWUC"" Visible=""false"" />")
                .WriteLine("<div class=""md-overlay""></div>")
            End With

            objWriter.WriteLine("<asp:TextBox ID=""txt_Code" & nomSimple & "_Hid"" runat=""server"" Text=""0"" Visible=""False"" Width=""1px""></asp:TextBox>")

            objWriter.WriteLine("</section>") ' FIN Section Content
            objWriter.WriteLine("")
            objWriter.WriteLine("<asp:Literal runat=""server"" ID=""LiteralStyleCSS""></asp:Literal>")

            objWriter.WriteLine("</div>") 'END DIV mail-inbox
            objWriter.WriteLine("</div>") 'END DIV pcont

            objWriter.WriteLine("<telerik:RadWindowManager ID=""RadWindowManager1"" runat=""server"" VisibleStatusbar=""false"" EnableViewState=""false"">")
            objWriter.WriteLine("   <Windows>")
            objWriter.WriteLine("       <telerik:RadWindow ID=""AddUpdateDialog"" runat=""server"" Title="""" IconUrl=""~/Images/favicon.ico"" Left=""75px"" ReloadOnShow=""true""")
            objWriter.WriteLine("       ShowContentDuringLoad=""false"" Modal=""true"" OnClientClose=""RadWindowClosing"" Behaviors=""Reload, Move, Resize, Maximize, Close""")
            objWriter.WriteLine("       EnableShadow=""false"" OnClientResizeEnd=""RadWindowClientResizeEnd"" />")
            objWriter.WriteLine("   </Windows>")
            objWriter.WriteLine("</telerik:RadWindowManager>")

            objWriter.WriteLine("<input id=""txtWindowPage"" type=""hidden"" />")

            objWriter.WriteLine("</asp:Content>")
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterface_Tableau_Formulaire_CodeBehind(ByVal name As String _
                                                                        , ByRef txt_PathGenerate_ScriptFile As TextBox _
                                                                        , ByRef ListBox_NameSpace As ListBox _
                                                                        , ByRef txt_LibraryName As TextBox _
                                                                        , ByVal databasename As String)
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            'Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & databasename & "\ASPWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & databasename & "\ASPWebForm\")
            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
            Dim path As String = txt_PathGenerate_Script & nomWebform & "ADD.aspx.vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            Dim cult As Globalization.CultureInfo = New Globalization.CultureInfo("en-EN")
            Threading.Thread.CurrentThread.CurrentCulture = cult

            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now '.ToString("dd-MMM-yyyy hh:mm tt")
            'header &= ""
            Dim content As String = "Partial Class " & nomWebform & "ADD" & Chr(13) _
                                     & " Inherits Cls_BasePage ' LA CLASSE DE LA PAGE HERITE DE CETTE CLASSE DANS LE CAS OU NOUS AVONS UNE APPLICATION WEB multilingue"

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
            Dim libraryname As String = "Imports " & txt_LibraryName.Text
            objWriter.WriteLine("Imports Telerik.Web.UI")
            objWriter.WriteLine(libraryname)
            objWriter.WriteLine()

            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            objWriter.WriteLine(content)
            objWriter.WriteLine()

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
                    count += 1
                Else
                    Exit For
                End If
            Next
            'cols.Add("_isdirty")
            'cols.Add("_LogData")
            'types.Add("Boolean")
            'types.Add("String")
            'initialtypes.Add("Byte")
            'initialtypes.Add("nvarchar")

            Dim _tmpEditState As Boolean = False
            'objWriter.WriteLine("Dim _out As Boolean = False")
            'objWriter.WriteLine("Dim _tmpEditState As Boolean = False")

            With objWriter
                .WriteLine("")
                .WriteLine("#Region ""ATTRIBUTS"" ")
                .WriteLine("    Private _message As String  ' VARIABLE SERVANT A LA RECUPERATION DE TOUS LES MESSAGES D'ECHECS OU DE SUCCES")
                .WriteLine("")
                .WriteLine("    REM DEFINITION ET INITIALISATION DES CONSTANTE POUR LA SECURITE")
                .WriteLine("    Private Const Nom_page As String = ""PAGE-FORMULAIRE-" & nomSimple.ToUpper & """  ' POUR LA PAGE")
                .WriteLine("    Private Const Btn_Save As String = ""Bouton-SAVE-" & nomSimple.ToUpper & """       ' POUR LE BOUTON D'ENREGISTREMENT")
                .WriteLine("    Private Const Btn_Edit As String = ""Bouton-EDIT-" & nomSimple.ToUpper & """       ' POUR LE BOUTON DE MODIFICATION")
                .WriteLine("    Private Const Btn_Delete As String = ""Bouton-DELETE-" & nomSimple.ToUpper & """   ' POUR LE BOUTON DE SUPPRESSION")
                .WriteLine("")
                .WriteLine("    Dim User_Connected As Cls_User          ' INSTANCE DE LA CLASSE UTILISATEUR - UTILISER POUR L'UTILISATEUR EN SESSION ")
                .WriteLine("    Dim Is_Acces_Page As Boolean = True     ' LA VARIABLE SERVANT DE TEST POUR DONNEER L'ACCES A LA PAGE")
                .WriteLine("    Dim GetOut As Boolean = False           ' LA VARIABLE SERVANT DE TEST POUR REDIRIGER L'UTILISATEUR VERS LA PAGE DE CONNEXION")
                .WriteLine("    Dim PAGE_MERE As Long = 0' PAS TROP IMPORTANT...")
                .WriteLine("    Dim PAGE_TITLE As String = """" ")
                .WriteLine("#End Region")
                .WriteLine("")
            End With

            With objWriter
                .WriteLine("Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load ")
                .WriteLine("    Response.Cache.SetCacheability(HttpCacheability.NoCache) ")
                .WriteLine("    Response.Expires = -1 ")
                .WriteLine("    Panel_Msg.Visible = False ")

                Dim nomSimpleToShow As String = Regex.Replace(nomSimple, "([a-z])?([A-Z])", "$1 $2")

                .WriteLine("    PAGE_TITLE = """ & nomSimpleToShow & """")
                .WriteLine("    Page.Title = [Global].Global_APP_NAME_SIGLE & "" | "" & PAGE_TITLE")
                .WriteLine(" ")
                .WriteLine("    SYSTEME_SECURITE()  ' APPEL A LA METHODE SERVANT A TESTER LES COMPOSANTS DE LA PAGE Y COMPRIS LA PAGE ELLE MEME ")
                .WriteLine("")
                .WriteLine("    '--- Si l'utilisateur n'a Access a la page les informations ne sont pas charger dans la Page_Load ")
                .WriteLine("    If Is_Acces_Page Then ")
                .WriteLine("        If Not IsPostBack Then ")
                .WriteLine("            Label_Titre.Text = PAGE_TITLE")
                .WriteLine("            'btnCancel.Attributes.Add(""onclick"", ""javascript:void(closeWindow());"")")
                .WriteLine("            'rbtnAdd" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx', 950, 650)); return false;"")") '& Chr(13) _
                .WriteLine("            'BtnADDNew.Attributes.Add(""onclick"", ""javascript:Open_Window('Frm_" & nomSimple & "ADD.aspx', '_self',500,400); return false;"") ")
                .WriteLine("            Load_ALL_DATA() ")
                .WriteLine("        End If ")
                .WriteLine("    End If ")
                .WriteLine("End Sub ")
                .WriteLine(" ")
            End With


            With objWriter
                .WriteLine("#Region ""SECURITE""")
                .WriteLine("Public Sub SYSTEME_SECURITE()")
                .WriteLine("    Try")
                .WriteLine("        User_Connected = [Global].KeepUserContinuesToWork(User_Connected)")
                .WriteLine("")
                .WriteLine("        'CType(Page.Master.FindControl(""li_" & nomSimple & """), HtmlControl).Attributes.Add(""class"", ""active "")")
                .WriteLine("        'CType(Page.Master.FindControl(""i_" & nomSimple & """), HtmlControl).Attributes.Add(""class"", ""fa fa-folder-open fa-lg "")")

                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liGROUPE_PARAMETRES""), HtmlControl).Attributes.Add(""class"", ""active treeview"")")
                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liCentreDeDetentionListe""), HtmlControl).Attributes.Add(""class"", ""active"")")

                .WriteLine("        LiteralStyleCSS.Text = """" ")
                .WriteLine("        If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("            Select Case Request.QueryString([Global].ACTION)")
                .WriteLine("                Case [Global].HideMenuHeader")
                .WriteLine("                    CType(Page.Master.FindControl([Global].Head_Nav_Menu), HtmlControl).Visible = False")
                .WriteLine("                    CType(Page.Master.FindControl([Global].Head_Nav_MenuVertical), HtmlControl).Visible = False")
                .WriteLine("                    Dim StyleCss As String = ""<style type=""""text/css""""> #cl-wrapper { padding-top: 0px; } </style>""")
                .WriteLine("                    LiteralStyleCSS.Text = StyleCss")
                .WriteLine("                Case Else")
                .WriteLine("                    'span_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("                    'Btn_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("                End Select")
                .WriteLine("        Else")
                .WriteLine("            'span_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("            'Btn_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) IsNot Nothing Then")
                .WriteLine("            User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)")
                .WriteLine("            If Not Cls_Privilege.VerifyRightOnObject(Nom_page, User_Connected.IdGroupeuser) Then    ' VERIFICATION SI L'UTILISATEUR N'A PAS ACCES A LA PAGE")
                .WriteLine("                _message = [Global].NO_ACCES_PAGE")
                .WriteLine("                MessageToShow(_message)")
                .WriteLine("                Is_Acces_Page = False")
                .WriteLine(" ")
                .WriteLine("                Panel_First.Visible = False")
                .WriteLine("            Else    ' SI L'UTILISATEUR A ACCES A LA PAGE ON VERIFIE POUR LES BOUTONS ET LES LIENS")
                .WriteLine("                '---  Okey vous avez acces a la page ---'")
                .WriteLine("                Dim _check As Boolean = Cls_Privilege.VerifyRightOnObject(Btn_Save, User_Connected.IdGroupeuser)")
                .WriteLine("                'Btn_ADD_" & nomSimple & ".Visible = _check")
                .WriteLine("                Btn_SaveInfo.Visible = _check")
                .WriteLine("                'rdg" & nomSimple & ".MasterTableView.Columns.FindByUniqueNameSafe(""editer"").Visible = _check")

                .WriteLine("                If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("                    If Request.QueryString([Global].ACTION).Equals([Global].HideMenuHeader) Then")
                .WriteLine("                        Btn_SaveInfo.Visible = _check")
                .WriteLine("                    End If")
                .WriteLine("                End If")
                .WriteLine("            End If")
                .WriteLine("        End If")
                .WriteLine(" ")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) Is Nothing Then")
                .WriteLine("            '-- Session expirée --'")
                .WriteLine("            GetOut = True")
                .WriteLine("        Else")
                .WriteLine("            Try")
                .WriteLine("                User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)  ' ON VERIFIE SI LÚTILISATEUR A ETE FORCE DE SE CONNECTER 'PAR L'ADM")
                .WriteLine("                If Not (GlobalFunctions.IsUserStillConnected(User_Connected) And GlobalFunctions.IsUserStillActive(User_Connected)) Then")
                .WriteLine("                    User_Connected.Set_Status_ConnectedUser(False)")
                .WriteLine("                    User_Connected.Activite_Utilisateur_InRezo(""Forced Log Off"", ""Forced to Log Off"", Request.UserHostAddress)")
                .WriteLine("")
                .WriteLine("                    GetOut = True")
                .WriteLine("                    Session.RemoveAll()")
                .WriteLine("                    '_message = ""Session expirée.""")
                .WriteLine("                    'MessageToShow(_message)")
                .WriteLine("                    Is_Acces_Page = True")
                .WriteLine("                End If")
                .WriteLine("            Catch ex As Exception")
                .WriteLine("                GetOut = True")
                .WriteLine("                '_message = ""Session expirée.""")
                .WriteLine("                'MessageToShow(_message)")
                .WriteLine("            End Try")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("    If GetOut Then ' REDIRECTIONNEMENT DE L'UTILISATUER OU PAS.")
                .WriteLine("        CType(Page.Master.FindControl([Global].htmlMasterPage), HtmlControl).Attributes.Add(""class"", ""lockscreen"")")
                .WriteLine("        CType(Page.Master.FindControl([Global].bodyMasterPage), HtmlControl).Attributes.Add(""class"", ""texture"")")
                .WriteLine("        CType(Page.Master.FindControl([Global].Head_Nav_Menu), HtmlControl).Visible = False")
                .WriteLine("        CType(Page.Master.FindControl([Global].Head_Nav_MenuVertical), HtmlControl).Visible = False")
                .WriteLine("        ")
                .WriteLine("        Is_Acces_Page = False")
                .WriteLine("        PageHeader.Attributes.Add(""style"", ""visibility:hidden;"")")
                .WriteLine("        Panel_First.Visible = False")
                .WriteLine("        LoginWUC.Visible = True")
                .WriteLine("        Session([Global].GLOBAL_PAGENAME) = System.Web.HttpContext.Current.Request.Url.ToString()")
                .WriteLine("        'Response.Redirect([Global].PAGE_LOGIN)")
                .WriteLine("    End If")

                .WriteLine("")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        Is_Acces_Page = False")
                .WriteLine("        Panel_First.Visible = False")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""Other Method - MessageToShow""")
                .WriteLine("    Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"", Optional ByVal ShowPopUp As Boolean = True)")
                .WriteLine("        Panel_Msg.Visible = True")
                .WriteLine("        GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
                .WriteLine("        Label_Msg.Text = _message")
                .WriteLine("        If ShowPopUp Then")
                .WriteLine("            RadAjaxManager1.ResponseScripts.Add(""alert('"" & [Global].GetTextFromHtml(_message).Replace(""'"", ""\'"") & ""');"")")
                .WriteLine("            'Dialogue.alert([Global].GetTextFromHtml(_message))")
                .WriteLine("        End If")
                .WriteLine("        If E_or_S = ""S"" Then")
                .WriteLine("            Style_Division(DIV_Msg, ""alert alert-success alert-dismissable"")")
                .WriteLine("            Style_Division(Icon_Msg, ""fa  fa-thumbs-up"")")
                .WriteLine("        Else")
                .WriteLine("            Style_Division(DIV_Msg, ""alert alert-danger alert-dismissable"")")
                .WriteLine("            Style_Division(Icon_Msg, ""fa  fa-thumbs-down"")")
                .WriteLine("        End If")
                .WriteLine("    End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            objWriter.WriteLine("#Region ""Load DATA""")
            With objWriter
                .WriteLine("Private Sub LOAD_ALL_DATA()")
                ''---------------------------------''
                For Each fk In ListofForeignKey
                    Dim NameFileCombo1 As String = "FillCombo" & fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                    .WriteLine("" & NameFileCombo1 & "()")
                Next
                ''---------------------------------''
                .WriteLine("LOAD_" & nomSimple.ToUpper & "()")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Private Sub LOAD_" & nomSimple.ToUpper & "()")
                .WriteLine("    Try")
                .WriteLine("        If Request.QueryString(""ID"") IsNot Nothing Then")
                .WriteLine("            dim _id as long = TypeSafeConversion.NullSafeLong(Request.QueryString(""ID""))")
                .WriteLine("            txt_Code" & nomSimple & "_Hid.Text = _id")
                .WriteLine("            Dim obj as New " & nomClasse & "( _id )")
                .WriteLine("            If obj.ID > 0 Then")
                .WriteLine("                Btn_SaveInfo.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Edit, User_Connected.IdGroupeuser)")
                .WriteLine("                With obj")

                For i As Int32 = 1 To cols.Count - 1 '3
                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim nom_DDL As String = "DDL" & cols(i) '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

                        .WriteLine(nom_DDL & " .SelectedIndex =  " & nom_DDL & ".Items.IndexOf(" & nom_DDL & ".Items.FindByValue(." & cols(i).Substring(1, cols(i).Length - 1) & "))")
                        '.WriteLine("rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " .SelectedIndex =  rcmb_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & ".FindItemIndexByValue(." & cols(i).Substring(1, cols(i).Length - 1) & ")")
                        'ddl_Sexe.SelectedIndex = ddl_Sexe.Items.IndexOf(ddl_Sexe.Items.FindByValue(obj.Sexe))
                    ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                        .WriteLine("rdp" & cols(i) & ".SelectedDate = ." & cols(i).Substring(1, cols(i).Length - 1))

                    ElseIf types(i) = "Boolean" Then
                        .WriteLine("CB" & cols(i) & ".Checked = ." & cols(i).Substring(1, cols(i).Length - 1))

                    Else
                        .WriteLine("txt" & cols(i) & ".Text = ." & cols(i).Substring(1, cols(i).Length - 1))
                    End If
                Next
                .WriteLine("            End With")
                .WriteLine("        End If")
                .WriteLine("    Else")
                .WriteLine()
                .WriteLine("    End If")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")

                ''-------------------------------------------------''
                objWriter.WriteLine()

                For Each fk In ListofForeignKey
                    'objWriter.WriteLine("FillCombo" & fk & "()")

                    Dim textForcombo As String = ""
                    Dim attributUsed As String = fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                    Dim nomforeign As String = fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                    Dim ClassName As String = "Cls" & fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length))

                    Dim NameFileCombo As String = "FillCombo" & fk '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, fk.Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

                    textForcombo = nomforeign.Substring(1, nomforeign.Length - 1)
                    .WriteLine("Private Sub " & NameFileCombo & "()")
                    .WriteLine("Try")
                    .WriteLine("Dim objs1 As List(of " & ClassName & ") = " & ClassName & ".SearchAll ")

                    .WriteLine("With DDL" & nomforeign)
                    .WriteLine("    .Datasource = objs1")
                    .WriteLine("    .DataValueField = ""ID""")
                    .WriteLine("    .DataTextField = """ & textForcombo & """")
                    .WriteLine("    .DataBind()")

                    .WriteLine("    .Items.Insert(0, New ListItem("" - Choisir("" & objs1.Count & "") - "", 0))")
                    .WriteLine("    .SelectedIndex = -1")

                    .WriteLine("    '.Items.Sort()")
                    .WriteLine("    '.Items.Insert(0, New RadComboBoxItem("" - Choisir -"", """"))")
                    .WriteLine("    '.SelectedIndex = 0")
                    .WriteLine("    '.EmptyMessage = ""- Choisir -""")
                    .WriteLine("End With")
                    .WriteLine("    Catch ex As Threading.ThreadAbortException")
                    .WriteLine("    Catch ex As Rezo509Exception")
                    .WriteLine("        MessageToShow(ex.Message)")
                    .WriteLine("    Catch ex As Exception")
                    .WriteLine("        MessageToShow(ex.Message)")
                    .WriteLine("        [Global].WriteError(ex, User_Connected)")
                    .WriteLine("    End Try")
                    .WriteLine("End Sub")
                    .WriteLine()
                Next
                ''-------------------------------------------------''
                .WriteLine("#End Region")
            End With

            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""METHODES - SAVE""")
            With objWriter
                .WriteLine("Private Sub SAVE_" & nomSimple.ToUpper & "()")
                .WriteLine("    Try")
                .WriteLine("        Dim _id As Long = TypeSafeConversion.NullSafeLong(txt_Code" & nomSimple & "_Hid.Text)")
                .WriteLine("        Dim obj As New " & nomClasse & "(_id)")
                .WriteLine("        With obj")
                For i As Int32 = 1 To cols.Count - 1 '3
                    Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim ForeinKeyPrefix As String = columnToUse '.Substring(SqlServerHelper.ForeinKeyPrefix.Length, columnToUse.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        .WriteLine("." & columnToUse & "  =   DDL_" & ForeinKeyPrefix & " .SelectedValue ")

                    ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                        .WriteLine("." & columnToUse & " = rdp_" & columnToUse & " .SelectedDate ")
                    ElseIf types(i) = "Boolean" Then
                        ' .WriteLine("." & columnToUse & " =  radio_yes_" & columnToUse & " .Checked")
                        .WriteLine("." & columnToUse & " =  CB_" & columnToUse & " .Checked")
                    Else
                        .WriteLine("." & columnToUse & " = txt_" & columnToUse & ".Text ")
                    End If
                Next
                .WriteLine("End With")
                .WriteLine("obj.Save(User_Connected.Username)")
                .WriteLine("REM TRACE UTILUSATEUR / Trace Transaction")
                .WriteLine("User_Connected.Activite_Utilisateur_InRezo(IIf(_id <= 0, ""ADD "", ""EDIT "") & "" " & nomSimple & """, obj.LogData(obj) , Request.UserHostAddress)")
                .WriteLine("txt_Code" & nomSimple & "_Hid.Text=obj.ID")

                .WriteLine("    '_message = ""Sauvegarde Effectuée""")
                .WriteLine("    MessageToShow([Global].Msg_Enregistrement_Effectue, ""S"", False)")
                .WriteLine("    'RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe" & nomSimple & "();"")")
                .WriteLine("    RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe();"")")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                '.WriteLine("  End Select")
                .WriteLine("End Sub")
            End With
            objWriter.WriteLine("#End Region")

            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""EVENTS BUTTON""")
            With objWriter
                .WriteLine("Protected Sub Btn_SaveInfo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Btn_SaveInfo.Click")
                .WriteLine("SAVE_" & nomSimple.ToUpper & "()")
                .WriteLine("End Sub")
            End With

            With objWriter
                .WriteLine("Protected Sub Btn_Annuler_Click(sender As Object, e As EventArgs) Handles Btn_Annuler.Click")
                .WriteLine("    PAGE_MERE = TypeSafeConversion.NullSafeLong(Request.QueryString([Global].PAGE_MERE))")
                .WriteLine("    If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("        Select Case Request.QueryString([Global].ACTION)")
                .WriteLine("            Case [Global].HideMenuHeader")
                .WriteLine("                 RadAjaxManager1.ResponseScripts.Add(""CloseAndRefreshListe();"")")
                .WriteLine("            Case Else")
                .WriteLine("                Response.Redirect([Global].GetPath_PageMere(PAGE_MERE))")
                .WriteLine("            End Select")
                .WriteLine("    Else")
                .WriteLine("        Response.Redirect([Global].GetPath_PageMere(PAGE_MERE))")
                .WriteLine("    End If")
                .WriteLine("End Sub")
            End With
            objWriter.WriteLine("#End Region")

            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterface_CleanZone_Listing_Design(ByVal name As String _
                                                                   , ByRef txt_PathGenerate_ScriptFile As TextBox _
                                                                   , ByRef ListBox_NameSpace As ListBox _
                                                                   , ByVal databasename As String)
            '  Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
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

            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            Dim _table As New Cls_Table()

            ' _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
                    count += 1
                Else
                    Exit For
                End If
            Next
            'cols.Add("_isdirty")
            'cols.Add("_LogData")
            'types.Add("Boolean")
            'types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")

            Dim nomSimpleToShow As String = Regex.Replace(nomSimple, "([a-z])?([A-Z])", "$1 $2")
            nomSimpleToShow = Regex.Replace(nomSimpleToShow, "_", " ")

            objWriter.WriteLine("<%@ Page Title=""" & nomSimpleToShow & """ Language=""VB"" MasterPageFile=""~/MasterPages/DashboardCZMasterPage.master"" " & _
           "  AutoEventWireup=""false""  MaintainScrollPositionOnPostback=""true"" CodeFile=""Frm_" & nomSimple & "Listing.aspx.vb"" Inherits=""Frm_" & nomSimple & "Listing"" %>")

            objWriter.WriteLine("<asp:Content ID=""Content1"" ContentPlaceHolderID=""ContentPlaceHolder1"" runat=""Server"">")

            objWriter.WriteLine("<telerik:RadCodeBlock ID=""RadCodeBlock1"" runat=""server"">")
            objWriter.WriteLine("<script type=""text/javascript"">")

            'objWriter.WriteLine("function onRequestStart(sender, args) {")
            'objWriter.WriteLine(" if (args.get_eventTarget().indexOf(""ExportToExcelButton"") >= 0) {")
            'objWriter.WriteLine("args.set_enableAjax(false);")
            'objWriter.WriteLine(" }")
            'objWriter.WriteLine(" }")
            'objWriter.WriteLine()

            With objWriter
                .WriteLine(" function ShowAddUpdateForm(strPage, tmpW, tmpH) {")
                .WriteLine("var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
                .WriteLine("        //oWindow.set_autoSize(true);")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("        document.getElementById(""txtWindowPage"").value = strPage;")
                .WriteLine("        if (oWindow) {")
                .WriteLine("            if (!oWindow.isClosed()) {")
                .WriteLine("                oWindow.center();")
                .WriteLine("                var bounds = oWindow.getWindowBounds();")
                .WriteLine("                oWindow.moveTo(bounds.x + 'px', ""50px"");")
                .WriteLine("            }")
                .WriteLine("        }")
                .WriteLine("        return false;")
                .WriteLine("    }")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function ShowAddUpdateFormMaximized(strPage, tmpW, tmpH) { ")
                .WriteLine("    var oWindow = window.radopen(strPage, ""AddUpdateDialog""); ")
                .WriteLine("    oWindow.SetSize(tmpW, tmpH); ")
                .WriteLine("    document.getElementById(""txtWindowPage"").value = strPage; ")
                .WriteLine("    if (oWindow) { ")
                .WriteLine("                       if (!oWindow.isClosed()) { ")
                .WriteLine("                           oWindow.center(); ")
                .WriteLine("                           var bounds = oWindow.getWindowBounds(); ")
                .WriteLine("                           oWindow.moveTo(bounds.x + 'px', ""50px""); ")
                .WriteLine("            } ")
                .WriteLine("    } ")
                .WriteLine("                   oWindow.maximize(); ")
                .WriteLine("                   return false; ")
                .WriteLine("} // ")
            End With
            objWriter.WriteLine()

            With objWriter
                .WriteLine("function ShowAddUpdateFormAutoSize(strPage, tmpW, tmpH) {")
                .WriteLine("                  var oWindow = window.radopen(strPage, ""AddUpdateDialog"");")
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
            End With

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
                .WriteLine("var listItemIndex = null;")

                .WriteLine("function MenuItemClicked(sender, eventArgs) {")
                .WriteLine("     var clickedItemValue = eventArgs.get_item().get_value();")
                .WriteLine("     var rdGrid = $find(""<%=rdg" & nomSimple & ".ClientID %>"");")
                .WriteLine("    var _id = rdGrid.get_masterTableView().get_dataItems()[listItemIndex].get_element().cells[0].innerHTML")
                .WriteLine("    switch (clickedItemValue) {")
                .WriteLine("        case ""Editer"":")
                .WriteLine("            ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID=' + _id + '&ACTION=HideMenuHeader', 950, 550); break;")
                .WriteLine("        case ""Delete"":")
                .WriteLine("            ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID=' + _id + '&ACTION=HideMenuHeader', 950, 550); break;")
                .WriteLine("        default:")
                .WriteLine("            break;")
                .WriteLine("    }")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function RowContextMenu(sender, eventArgs) {")
                .WriteLine("    var menu = $find(""<%= ContextMenu.ClientID %>"");")
                .WriteLine("    var evt = eventArgs.get_domEvent();")
                .WriteLine("    if (evt.target.tagName == ""INPUT"" || evt.target.tagName == ""A"") { return; }")
                .WriteLine("     var index = eventArgs.get_itemIndexHierarchical();")
                .WriteLine("    document.getElementById(""radGridClickedRowIndex"").value = index;")
                .WriteLine("    listItemIndex = index;")
                .WriteLine("    sender.get_masterTableView().selectItem(sender.get_masterTableView().get_dataItems()[index].get_element(), true);")
                .WriteLine("    menu.show(evt);")
                .WriteLine("    evt.cancelBubble = true;")
                .WriteLine("    evt.returnValue = false;")
                .WriteLine("    if (evt.stopPropagation) {")
                .WriteLine("        evt.stopPropagation();")
                .WriteLine("        evt.preventDefault();")
                .WriteLine("    }")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function RowDblClick(sender, eventArgs) {")
                .WriteLine("    var index = eventArgs.get_itemIndexHierarchical();")
                .WriteLine("    document.getElementById(""radGridClickedRowIndex"").value = index;")
                .WriteLine("    listItemIndex = index;")
                .WriteLine("    var rdGrid = $find(""<%=rdg" & nomSimple & ".ClientID %>"");")
                .WriteLine("    var _id = rdGrid.get_masterTableView().get_dataItems()[listItemIndex].get_element().cells[0].innerHTML")
                .WriteLine("    ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID=' + _id + '&ACTION=HideMenuHeader', 950, 550);")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function refreshMe() {")
                .WriteLine("$find(""<%= RadAjaxManager1.ClientID %>"").ajaxRequest(""Reload"");")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function closeWindow() {")
                .WriteLine(" GetRadWindow().BrowserWindow.refreshMe();")
                .WriteLine(" GetRadWindow().close();")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function CloseAndRefreshListe() {")
                .WriteLine("    GetRadWindow().BrowserWindow.refreshMe();")
                .WriteLine("    GetRadWindow().close();")
                .WriteLine("}")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("function GetRadWindow() {")
                .WriteLine("    var oWindow = null;")
                .WriteLine("    if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog")
                .WriteLine("   else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well)")
                .WriteLine("   return oWindow;")
                .WriteLine("}")
                .WriteLine()
            End With

            objWriter.WriteLine("</script>")
            objWriter.WriteLine("</telerik:RadCodeBlock>")

            objWriter.WriteLine(" <%--<telerik:RadScriptManager ID=""RadScriptManager1""  runat=""server""> </telerik:RadScriptManager>--%>")

            objWriter.WriteLine(" <telerik:RadAjaxManager ID=""RadAjaxManager1"" runat=""server"">")
            objWriter.WriteLine("<AjaxSettings>")
            With objWriter
                .WriteLine("<telerik:AjaxSetting AjaxControlID=""RadAjaxManager1"">")
                .WriteLine("<UpdatedControls>")
                .WriteLine("        <telerik:AjaxUpdatedControl ControlID=""Panel_First"" LoadingPanelID=""RadAjaxLoadingPanel1"" />")
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
            'objWriter.WriteLine("<telerik:RadSkinManager ID=""RadSkinManager1"" runat=""server"" Skin=""Windows7""> </telerik:RadSkinManager>")
            objWriter.WriteLine("<telerik:RadAjaxLoadingPanel ID=""RadAjaxLoadingPanel1"" runat=""server"" />")
            objWriter.WriteLine("<input type='hidden' id='radGridClickedRowIndex' name='radGridClickedRowIndex' />")


            objWriter.WriteLine("<div class=""container-fluid"" id=""pcont"">")
            objWriter.WriteLine("<div class=""mail-inbox"">")

            With objWriter
                .WriteLine("<section class=""page-head"" ID=""PageHeader"" runat=""server"" >")
                .WriteLine("<h3>")
                .WriteLine("    <i class=""fa fa-dashboard""></i>")
                .WriteLine("    <asp:Label ID=""Label_Titre"" runat=""server"" Text=""" & nomSimpleToShow & """ /> ")
                .WriteLine("    <small id=""OL_SeeAllData"" runat=""server"">")
                .WriteLine("        <asp:Label ID=""Label_SousTitre"" runat=""server"" />")
                .WriteLine("    </small>")
                .WriteLine("</h3>")
                .WriteLine("<!--<ol class=""breadcrumb""> ")
                .WriteLine("    <li><a href=""#""><i class=""fa fa-dashboard""></i>Accueil</a></li>")
                .WriteLine("    <li class=""active"">" & nomSimpleToShow & "</li>")
                .WriteLine("</ol> -->")
                .WriteLine("</section> ")
            End With

            objWriter.WriteLine("<section class=""content"">")

            With objWriter
                .WriteLine("    <Msg:msgBox ID=""Dialogue"" runat=""server"" />")
                .WriteLine("    <asp:Panel runat=""server"" ID=""Panel_Msg"" Visible=""false"">")
                .WriteLine("        <div id=""DIV_Msg"" runat=""server"" class=""alert alert-warning alert-dismissable"">")
                .WriteLine("            <i id=""Icon_Msg"" runat=""server"" class=""fa fa-warning""></i>")
                .WriteLine("            <button type=""button"" class=""close"" data-dismiss=""alert"" aria-hidden=""true"">×</button>")
                .WriteLine("            <asp:Image ID=""Image_Msg"" runat=""server"" />")
                .WriteLine("            <asp:Label ID=""Label_Msg"" runat=""server"" Text=""""></asp:Label>")
                .WriteLine("        </div>")
                .WriteLine("    </asp:Panel>")
                .WriteLine("")
            End With

            With objWriter
                .WriteLine("<asp:Panel runat=""server"" ID=""Panel_First"" style=""margin: 5px;"">")

                'Button ADD
                .WriteLine("<asp:LinkButton ID=""Btn_ADD_" & nomSimple & """ runat=""server"" CssClass=""btn btn-primary"" CausesValidation=""false"">")
                .WriteLine("    <i class=""fa fa-plus-circle"" ></i>  Ajouter " & nomSimpleToShow & "")
                .WriteLine("</asp:LinkButton>")

                'Button Clear
                .WriteLine("<span class=""pull-right box-tools"">")
                .WriteLine("    <asp:LinkButton ID=""rbtnClearFilters"" runat=""server"" CssClass=""btn btn-sm btn-default"" CausesValidation=""false""> ")
                .WriteLine("        <i class=""fa fa-ban on fa-filter"" ></i> Clear Filters")
                .WriteLine("    </asp:LinkButton>")
                .WriteLine("</span>")

                .WriteLine("")
                .WriteLine("")
                With objWriter
                    .WriteLine(" <telerik:RadGrid ID=""rdg" & nomSimple & """ AllowPaging=""True"" AllowSorting=""True"" PageSize=""20""")
                    .WriteLine(" runat=""server"" AutoGenerateColumns=""False"" GridLines=""None"" AllowFilteringByColumn=""true"" ")
                    .WriteLine("  Culture=""fr-FR"" ShowGroupPanel=""True"" ")
                    .WriteLine(" EnableViewState=""true"" AllowMultiRowSelection=""false"" GroupingSettings-CaseSensitive=""false"">")
                    .WriteLine(" <ExportSettings HideStructureColumns=""true"" />")

                    .WriteLine("  <MasterTableView CommandItemDisplay=""Top"" GridLines=""None"" DataKeyNames=""ID"" NoDetailRecordsText=""Pas d'enregistrement""")
                    .WriteLine(" NoMasterRecordsText=""Pas d'enregistrement"">")

                    .WriteLine(" <CommandItemSettings ShowAddNewRecordButton=""false"" ShowRefreshButton=""false"" ShowExportToExcelButton=""true"" ")
                    .WriteLine("  ExportToExcelText=""Exporter en excel"" />")
                    .WriteLine(" <PagerStyle Mode=""NextPrevAndNumeric""></PagerStyle>")

                    .WriteLine(" <Columns>")

                    .WriteLine("<telerik:GridBoundColumn DataField=""ID"" UniqueName=""ID"" Display=""false"" />")

                    .WriteLine("<telerik:GridTemplateColumn Visible=""true"" ShowFilterIcon=""false"" AllowFiltering=""false""  HeaderText=""#"" UniqueName=""Compteur"">")
                    .WriteLine("    <ItemTemplate>")
                    .WriteLine("        <asp:Label Visible=""true"" ID=""lbOrder"" runat=""server"" />")
                    .WriteLine("    </ItemTemplate>")
                    .WriteLine("    <HeaderStyle HorizontalAlign=""Center"" Width=""16px"" />")
                    .WriteLine("    <ItemStyle HorizontalAlign=""Center"" Width=""16px"" />")
                    .WriteLine("</telerik:GridTemplateColumn>")

                    Dim countColumn As Integer = 0
                    'Dim pourcentagevalue As Decimal = 100 / (_table.ListofColumn.Count - 4)
                    Dim pourcentagevalue As Decimal = 100 / cols.Count - 1 '- 3
                    Dim pourcentage As String = pourcentagevalue.ToString + "%"


                    For i As Int32 = 1 To cols.Count - 1 '- 3
                        If ListofForeignKey.Contains(cols(i)) Then
                            Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                            Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                            columnNameToShow = Regex.Replace(columnNameToShow, "_", "")

                            .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName.Replace("ID_", "") & "STR"" UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                            .WriteLine(" FilterControlAltText=""Recherche par " & columnNameToShow & """ FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine("  AllowFiltering=""true"" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            '.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")


                        ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                            Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                            Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                            columnNameToShow = Regex.Replace(columnNameToShow, "_", "")

                            .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                            .WriteLine(" FilterControlAltText=""Recherche par " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AllowFiltering=""true"" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")

                        ElseIf types(i) = "Boolean" And cols(i) <> "isdirty" Then
                            Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                            Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                            columnNameToShow = Regex.Replace(columnNameToShow, "_", "")

                            .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                            .WriteLine(" FilterControlAltText=""Filter " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AllowFiltering=""false"" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            '.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        Else
                            Dim columnName As String = cols(i).Substring(1, cols(i).Length - 1)
                            Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                            columnNameToShow = Regex.Replace(columnNameToShow, "_", "")

                            .WriteLine("<telerik:GridBoundColumn DataField=""" & columnName & """ UniqueName=""" & columnName & """ HeaderText=""" & columnNameToShow & """")
                            .WriteLine(" FilterControlAltText=""Filter " & columnName & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                            .WriteLine(" AllowFiltering=""true"" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                            '.WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                            .WriteLine("</telerik:GridBoundColumn>")
                        End If
                    Next

                    .WriteLine(" <telerik:GridButtonColumn ButtonType=""ImageButton"" CommandArgument=""ID"" CommandName=""editer""")
                    .WriteLine("        DataTextField=""ID"" ImageUrl=""~/images/_edit.png""")
                    .WriteLine("          HeaderText="""" UniqueName=""editer"">")
                    .WriteLine("        <HeaderStyle HorizontalAlign=""Center"" Width=""16px"" />")
                    .WriteLine("        <ItemStyle  HorizontalAlign=""Center"" Width=""16px"" />")
                    .WriteLine("    </telerik:GridButtonColumn>")

                    .WriteLine("<telerik:GridButtonColumn ButtonType=""ImageButton"" CommandName=""delete"" DataTextField=""ID""")
                    .WriteLine(" ImageUrl=""~/images/delete.png"" ")
                    .WriteLine("UniqueName=""delete"" HeaderText="""" ConfirmDialogType=""RadWindow"" ConfirmText=""Voulez-vous vraiment supprimer cette information ?""")
                    .WriteLine("ConfirmTitle=""Attention!"">")
                    .WriteLine("<HeaderStyle  HorizontalAlign=""Center"" Width=""16px""  />")
                    .WriteLine("<ItemStyle  HorizontalAlign=""Center"" Width=""16px""  />")
                    .WriteLine("</telerik:GridButtonColumn>")

                    .WriteLine("</Columns>")
                    .WriteLine("<RowIndicatorColumn FilterControlAltText=""Filter RowIndicator column""></RowIndicatorColumn>")
                    .WriteLine("<ExpandCollapseColumn FilterControlAltText=""Filter ExpandColumn column""></ExpandCollapseColumn>")
                    .WriteLine("</MasterTableView>")

                    .WriteLine("<GroupingSettings CaseSensitive=""False"" />")

                    .WriteLine("<ClientSettings  AllowDragToGroup=""True"" AllowColumnsReorder=""True"">")
                    .WriteLine("<ClientEvents OnRowContextMenu=""RowContextMenu"" OnRowDblClick=""RowDblClick"" />")
                    .WriteLine("<Selecting AllowRowSelect=""true"" />")
                    .WriteLine("</ClientSettings>")

                    .WriteLine("<HeaderContextMenu CssClass=""GridContextMenu GridContextMenu_Default"" />")

                    .WriteLine("<PagerStyle PageSizeControlType=""RadComboBox"" />")

                    .WriteLine("<FilterMenu EnableImageSprites=""False""></FilterMenu>")
                    .WriteLine("</telerik:RadGrid>")
                End With



                .WriteLine("</asp:Panel>") 'FIN PANEL

                .WriteLine("<!-- FORM LOGIN -->")
                .WriteLine("<BRAIN:CULogin2 runat=""server"" ID=""LoginWUC"" Visible=""false"" />")
                .WriteLine("<div class=""md-overlay""></div>")
            End With
            objWriter.WriteLine("</section>") ' FIN Section Content
            objWriter.WriteLine("")
            objWriter.WriteLine("<asp:Literal runat=""server"" ID=""LiteralStyleCSS""></asp:Literal>")

            objWriter.WriteLine("</div>") 'END DIV mail-inbox
            objWriter.WriteLine("</div>") 'END DIV pcont

            objWriter.WriteLine("<telerik:RadWindowManager ID=""RadWindowManager1"" runat=""server"" VisibleStatusbar=""false"" EnableViewState=""false"">")
            objWriter.WriteLine("   <Windows>")
            objWriter.WriteLine("       <telerik:RadWindow ID=""AddUpdateDialog"" runat=""server"" Title="""" IconUrl=""~/Images/favicon.ico"" Left=""75px"" ReloadOnShow=""true""")
            objWriter.WriteLine("       ShowContentDuringLoad=""false"" Modal=""true"" OnClientClose=""RadWindowClosing"" Behaviors=""Reload, Move, Resize, Maximize, Close""")
            objWriter.WriteLine("       EnableShadow=""false"" OnClientResizeEnd=""RadWindowClientResizeEnd"" />")
            objWriter.WriteLine("   </Windows>")
            objWriter.WriteLine("</telerik:RadWindowManager>")

            objWriter.WriteLine("<telerik:RadContextMenu ID=""ContextMenu"" runat=""server"" OnClientItemClicked=""MenuItemClicked"" EnableRoundedCorners=""true"" EnableShadows=""true"">")
            objWriter.WriteLine("   <Items>")
            objWriter.WriteLine("       <telerik:RadMenuItem Visible=""true"" Value=""Editer"" Text=""Editer"" ImageUrl=""~/images/_edit.png"" HoveredImageUrl=""~/images/_edit.png"" />")
            objWriter.WriteLine("       <telerik:RadMenuItem Visible=""true"" Value=""Delete"" Text=""Supprimer"" ImageUrl=""~/images/delete.png"" HoveredImageUrl=""~/images/delete.png"" />")
            objWriter.WriteLine("   </Items>")
            objWriter.WriteLine("</telerik:RadContextMenu>")

            objWriter.WriteLine("<input id=""txtWindowPage"" type=""hidden"" />")

            objWriter.WriteLine("</asp:Content>")
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateInterface_CleanZone_Listing_CodeBehind(ByVal name As String _
                                                                       , ByRef txt_PathGenerate_ScriptFile As TextBox _
                                                                       , ByRef ListBox_NameSpace As ListBox _
                                                                       , ByRef txt_libraryname As TextBox _
                                                                       , ByVal databasename As String)
            ' Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "Frm").Replace("Tbl", "Frm").Replace("TBL", "Frm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)

            Dim txt_PathGenerate_Script As String = Cls_Enumeration.GetPath_ASP_WebForm(txt_PathGenerate_ScriptFile.Text.Trim, databasename)
            Dim path As String = txt_PathGenerate_Script & nomWebform & "Listing.aspx.vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            Dim cult As Globalization.CultureInfo = New Globalization.CultureInfo("en-EN")
            Threading.Thread.CurrentThread.CurrentCulture = cult

            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy hh:mm tt")
            'header &= ""
            Dim content As String = "Partial Class " & nomWebform & "Listing" & Chr(13) _
                                     & " Inherits Cls_BasePage ' LA CLASSE DE LA PAGE HERITE DE CETTE CLASSE DANS LE CAS OU NOUS AVONS UNE APPLICATION WEB multilingue"

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
            Dim libraryname As String = "Imports " & txt_libraryname.Text
            objWriter.WriteLine("Imports Telerik.Web.UI")
            objWriter.WriteLine(libraryname)
            objWriter.WriteLine()

            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)

            objWriter.WriteLine(content)
            objWriter.WriteLine()


            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then

                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
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

            'objWriter.WriteLine("Dim _out As Boolean = False")
            'objWriter.WriteLine("Dim _tmpEditState As Boolean = False")

            With objWriter
                .WriteLine("")
                .WriteLine("#Region ""ATTRIBUTS"" ")
                .WriteLine("    Private _message As String  ' VARIABLE SERVANT A LA RECUPERATION DE TOUS LES MESSAGES D'ECHECS OU DE SUCCES")
                .WriteLine("")
                .WriteLine("    REM DEFINITION ET INITIALISATION DES CONSTANTE POUR LA SECURITE")
                .WriteLine("    Private Const Nom_page As String = ""PAGE-LISTING-" & nomSimple.ToUpper & """  ' POUR LA PAGE")
                .WriteLine("    Private Const Btn_Save As String = ""Bouton-SAVE-" & nomSimple.ToUpper & """       ' POUR LE BOUTON D'ENREGISTREMENT")
                .WriteLine("    Private Const Btn_Edit As String = ""Bouton-EDIT-" & nomSimple.ToUpper & """       ' POUR LE BOUTON DE MODIFICATION")
                .WriteLine("    Private Const Btn_Delete As String = ""Bouton-DELETE-" & nomSimple.ToUpper & """   ' POUR LE BOUTON DE SUPPRESSION")
                .WriteLine("")
                .WriteLine("    Dim User_Connected As Cls_User          ' INSTANCE DE LA CLASSE UTILISATEUR - UTILISER POUR L'UTILISATEUR EN SESSION ")
                .WriteLine("    Dim Is_Acces_Page As Boolean = True     ' LA VARIABLE SERVANT DE TEST POUR DONNEER L'ACCES A LA PAGE")
                .WriteLine("    Dim GetOut As Boolean = False           ' LA VARIABLE SERVANT DE TEST POUR REDIRIGER L'UTILISATEUR VERS LA PAGE DE CONNEXION")
                .WriteLine("    Dim PAGE_MERE As Long = 0' PAS TROP IMPORTANT...")
                .WriteLine("    Dim PAGE_TITLE As String = """" ")
                .WriteLine("#End Region")
            End With
            objWriter.WriteLine("")

            With objWriter
                .WriteLine(" Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load")
                .WriteLine("    Response.Cache.SetCacheability(HttpCacheability.NoCache) ")
                .WriteLine("    Response.Expires = -1 ")
                .WriteLine("    Panel_Msg.Visible = False ")

                Dim nomSimpleToShow As String = Regex.Replace(nomSimple, "([a-z])?([A-Z])", "$1 $2")

                .WriteLine("    PAGE_TITLE = """ & nomSimpleToShow & """")
                .WriteLine("    Page.Title = [Global].Global_APP_NAME_SIGLE & "" | "" & PAGE_TITLE")
                .WriteLine(" ")
                .WriteLine("    SYSTEME_SECURITE()  ' APPEL A LA METHODE SERVANT A TESTER LES COMPOSANTS DE LA PAGE Y COMPRIS LA PAGE ELLE MEME ")
                .WriteLine("")
                .WriteLine("    '--- Si l'utilisateur n'a Access a la page les informations ne sont pas charger dans la Page_Load ")
                .WriteLine("    If Is_Acces_Page Then ")
                .WriteLine("        If Not IsPostBack Then ")
                .WriteLine("            Label_Titre.Text = PAGE_TITLE")
                .WriteLine("            Btn_ADD_" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?"" & [Global].ACTION & ""="" & [Global].HideMenuHeader & ""', 950, 650)); return false;"")") '& Chr(13) _
                .WriteLine("            'Btn_ADD_" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:Open_Window('Frm_" & nomSimple & "ADD.aspx', '_self',500,400); return false;"") ")
                .WriteLine("            BindGrid() ")
                .WriteLine("        End If ")
                .WriteLine("    End If ")
                .WriteLine("End Sub")
                .WriteLine(" ")
                .WriteLine(" ")
            End With

            With objWriter
                .WriteLine("#Region ""SECURITE""")
                .WriteLine("Public Sub SYSTEME_SECURITE()")
                .WriteLine("    Try")
                .WriteLine("        User_Connected = [Global].KeepUserContinuesToWork(User_Connected)")
                .WriteLine("")
                .WriteLine("        'CType(Page.Master.FindControl(""li_" & nomSimple & """), HtmlControl).Attributes.Add(""class"", ""active "")")
                .WriteLine("        'CType(Page.Master.FindControl(""i_" & nomSimple & """), HtmlControl).Attributes.Add(""class"", ""fa fa-folder-open fa-lg "")")

                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liGROUPE_PARAMETRES""), HtmlControl).Attributes.Add(""class"", ""active treeview"")")
                .WriteLine("        'CType(Page.Master.FindControl(""DashMenu_2"").FindControl(""liCentreDeDetentionListe""), HtmlControl).Attributes.Add(""class"", ""active"")")

                .WriteLine("        LiteralStyleCSS.Text = """" ")
                .WriteLine("        If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("            Select Case Request.QueryString([Global].ACTION)")
                .WriteLine("                Case [Global].HideMenuHeader")
                .WriteLine("                    CType(Page.Master.FindControl([Global].Head_Nav_Menu), HtmlControl).Visible = False")
                .WriteLine("                    CType(Page.Master.FindControl([Global].Head_Nav_MenuVertical), HtmlControl).Visible = False")
                .WriteLine("                    Dim StyleCss As String = ""<style type=""""text/css""""> #cl-wrapper { padding-top: 0px; } </style>""")
                .WriteLine("                    LiteralStyleCSS.Text = StyleCss")
                .WriteLine("                Case Else")
                .WriteLine("                    'span_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("                    'Btn_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("                End Select")
                .WriteLine("        Else")
                .WriteLine("            'span_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("            'Btn_SaveInfo_CloseAfter.Visible = False")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) IsNot Nothing Then")
                .WriteLine("            User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)")
                .WriteLine("            If Not Cls_Privilege.VerifyRightOnObject(Nom_page, User_Connected.IdGroupeuser) Then    ' VERIFICATION SI L'UTILISATEUR N'A PAS ACCES A LA PAGE")
                .WriteLine("                _message = [Global].NO_ACCES_PAGE")
                .WriteLine("                MessageToShow(_message)")
                .WriteLine("                Is_Acces_Page = False")
                .WriteLine(" ")
                .WriteLine("                Panel_First.Visible = False")
                .WriteLine("            Else    ' SI L'UTILISATEUR A ACCES A LA PAGE ON VERIFIE POUR LES BOUTONS ET LES LIENS")
                .WriteLine("                '---  Okey vous avez acces a la page ---'")
                .WriteLine("                Dim _check As Boolean = Cls_Privilege.VerifyRightOnObject(Btn_Save, User_Connected.IdGroupeuser)")
                .WriteLine("                Btn_ADD_" & nomSimple & ".Visible = _check")
                .WriteLine("                rdg" & nomSimple & ".MasterTableView.Columns.FindByUniqueNameSafe(""editer"").Visible = _check")

                .WriteLine("                If Request.QueryString([Global].ACTION) IsNot Nothing Then")
                .WriteLine("                    If Request.QueryString([Global].ACTION).Equals([Global].HideMenuHeader) Then")
                .WriteLine("                        Btn_ADD_" & nomSimple & ".Visible = _check")
                .WriteLine("                    End If")
                .WriteLine("                End If")

                .WriteLine("                _check = Cls_Privilege.VerifyRightOnObject(Btn_Delete, User_Connected.IdGroupeuser)")
                .WriteLine("                rdg" & nomSimple & ".MasterTableView.Columns.FindByUniqueNameSafe(""delete"").Visible = _check")
                .WriteLine("            End If")
                .WriteLine("        End If")
                .WriteLine(" ")
                .WriteLine("        If Session([Global].GLOBAL_SESSION) Is Nothing Then")
                .WriteLine("            '-- Session expirée --'")
                .WriteLine("            GetOut = True")
                .WriteLine("        Else")
                .WriteLine("            Try")
                .WriteLine("                User_Connected = CType(Session([Global].GLOBAL_SESSION), Cls_User)  ' ON VERIFIE SI LÚTILISATEUR A ETE FORCE DE SE CONNECTER 'PAR L'ADM")
                .WriteLine("                If Not (GlobalFunctions.IsUserStillConnected(User_Connected) And GlobalFunctions.IsUserStillActive(User_Connected)) Then")
                .WriteLine("                    User_Connected.Set_Status_ConnectedUser(False)")
                .WriteLine("                    User_Connected.Activite_Utilisateur_InRezo(""Forced Log Off"", ""Forced to Log Off"", Request.UserHostAddress)")
                .WriteLine("")
                .WriteLine("                    GetOut = True")
                .WriteLine("                    Session.RemoveAll()")
                .WriteLine("                    '_message = ""Session expirée.""")
                .WriteLine("                    'MessageToShow(_message)")
                .WriteLine("                    Is_Acces_Page = True")
                .WriteLine("                End If")
                .WriteLine("            Catch ex As Exception")
                .WriteLine("                GetOut = True")
                .WriteLine("                '_message = ""Session expirée.""")
                .WriteLine("                'MessageToShow(_message)")
                .WriteLine("            End Try")
                .WriteLine("        End If")
                .WriteLine("")
                .WriteLine("    If GetOut Then ' REDIRECTIONNEMENT DE L'UTILISATUER OU PAS.")
                .WriteLine("        CType(Page.Master.FindControl([Global].htmlMasterPage), HtmlControl).Attributes.Add(""class"", ""lockscreen"")")
                .WriteLine("        CType(Page.Master.FindControl([Global].bodyMasterPage), HtmlControl).Attributes.Add(""class"", ""texture"")")
                .WriteLine("        CType(Page.Master.FindControl([Global].Head_Nav_Menu), HtmlControl).Visible = False")
                .WriteLine("        CType(Page.Master.FindControl([Global].Head_Nav_MenuVertical), HtmlControl).Visible = False")
                .WriteLine("        ")
                .WriteLine("        Is_Acces_Page = False")
                .WriteLine("        PageHeader.Attributes.Add(""style"", ""visibility:hidden;"")")
                .WriteLine("        Panel_First.Visible = False")
                .WriteLine("        LoginWUC.Visible = True")
                .WriteLine("        Session([Global].GLOBAL_PAGENAME) = System.Web.HttpContext.Current.Request.Url.ToString()")
                .WriteLine("        'Response.Redirect([Global].PAGE_LOGIN)")
                .WriteLine("    End If")

                .WriteLine("")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        Is_Acces_Page = False")
                .WriteLine("        Panel_First.Visible = False")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine()
                .WriteLine("#Region ""Other Method""")
                .WriteLine("    Private Sub MessageToShow(ByVal _message As String, Optional ByVal E_or_S As String = ""E"", Optional ByVal ShowPopUp As Boolean = True)")
                .WriteLine("        Panel_Msg.Visible = True")
                .WriteLine("        GlobalFunctions.Message_Image(Image_Msg, E_or_S)")
                .WriteLine("        Label_Msg.Text = _message")
                .WriteLine("        If ShowPopUp Then")
                .WriteLine("            RadAjaxManager1.ResponseScripts.Add(""alert('"" & [Global].GetTextFromHtml(_message).Replace(""'"", ""\'"") & ""');"")")
                .WriteLine("            'Dialogue.alert([Global].GetTextFromHtml(_message))")
                .WriteLine("        End If")
                .WriteLine("        If E_or_S = ""S"" Then")
                .WriteLine("            Style_Division(DIV_Msg, ""alert alert-success alert-dismissable"")")
                .WriteLine("            Style_Division(Icon_Msg, ""fa  fa-thumbs-up"")")
                .WriteLine("        Else")
                .WriteLine("            Style_Division(DIV_Msg, ""alert alert-danger alert-dismissable"")")
                .WriteLine("            Style_Division(Icon_Msg, ""fa  fa-thumbs-down"")")
                .WriteLine("        End If")
                .WriteLine("    End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With


            With objWriter
                .WriteLine("#Region ""Load DATA""")
                .WriteLine("Private Sub BindGrid(Optional ByVal _refresh As Boolean = True )")
                .WriteLine("    Dim objs As List(of Cls_" & nomSimple & ")")
                .WriteLine("    Dim _ret As Long = 0")
                .WriteLine("    Try")
                .WriteLine("        objs = Cls_" & nomSimple & ".SearchAll")
                .WriteLine("        rdg" & nomSimple & ".DataSource = objs")
                .WriteLine("        If _refresh Then")
                .WriteLine("            rdg" & nomSimple & ".DataBind()")
                .WriteLine("        End If")
                .WriteLine("         _ret = objs.Count")
                .WriteLine("         Label_Titre.Text = PAGE_TITLE & ""  <small class=""""badge badge-primary"""">"" & _ret & ""</small>""")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""EVENTS CONTROLS""")
                .WriteLine("")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""ACTIONS / METHODES""")
                .WriteLine("")
                .WriteLine("#End Region")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#Region ""RADGRID EVENTS""")
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemCommand(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs)  Handles rdg" & nomSimple & ".ItemCommand")
                .WriteLine("    Try")
                .WriteLine("        If e.CommandName = Telerik.Web.UI.RadGrid.ExportToExcelCommandName Then")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.ExportOnlyData = True")
                .WriteLine("            rdg" & nomSimple & ".GridLines = GridLines.Both")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.IgnorePaging = True")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.OpenInNewWindow = False")
                .WriteLine("            rdg" & nomSimple & ".ExportSettings.FileName = ""Liste des " & nomSimple & """")
                .WriteLine("            rdg" & nomSimple & ".MasterTableView.Columns(0).Visible = False")
                .WriteLine("            rdg" & nomSimple & ".MasterTableView.ExportToExcel()")
                .WriteLine("        End If")
                .WriteLine()
                .WriteLine("        Dim _id As Long = TypeSafeConversion.NullSafeLong(e.CommandArgument)")
                .WriteLine("        Select Case e.CommandName")
                .WriteLine("            Case ""delete""")
                .WriteLine("                Dim obj As New Cls_" & nomSimple & "(_id)")
                .WriteLine("                obj.Delete()")
                .WriteLine("                User_Connected.Activite_Utilisateur_InRezo(""DELETE "" & PAGE_TITLE, obj.LogData(obj), Request.UserHostAddress)")
                .WriteLine("                'User_Connected.Activite_Utilisateur_InRezo(""DELETE " & nomSimple & " "", obj.ID & "" - Code:"" & obj.Titrerapport & "" Prop:"", Request.UserHostAddress)")
                .WriteLine("                MessageToShow([Global].Msg_Information_Supprimee_Avec_Succes, ""S"")")
                .WriteLine("                rdg" & nomSimple & ".Rebind()")
                .WriteLine("        End Select")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rdg" & nomSimple & ".ItemDataBound")
                .WriteLine("Try")
                .WriteLine("    Dim gridDataItem = TryCast(e.Item, GridDataItem)")
                .WriteLine("    If e.Item.ItemType = GridItemType.Item Or e.Item.ItemType = GridItemType.AlternatingItem Then")
                .WriteLine("        'Dim _lnk As HyperLink = DirectCast(gridDataItem.FindControl(""hlk""), HyperLink)")
                .WriteLine("        'Dim _lbl_ID As Label = DirectCast(gridDataItem.FindControl(""lbl_ID""), Label)")
                .WriteLine("        '_lnk.Attributes.Clear()")
                .WriteLine("        '_lnk.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID="" & CLng(_lbl_ID.Text) & ""', 750, 400));"")")
                .WriteLine("    End If")
                .WriteLine()
                .WriteLine("    If (gridDataItem IsNot Nothing) Then")
                .WriteLine("        Dim item As GridDataItem = gridDataItem")
                .WriteLine("        CType(item.FindControl(""lbOrder""), Label).Text = rdg" & nomSimple & ".PageSize * rdg" & nomSimple & ".CurrentPageIndex + (item.RowIndex / 2)")
                .WriteLine("")
                .WriteLine("        Dim imagedelete As ImageButton = CType(item(""delete"").Controls(0), ImageButton)")
                .WriteLine("        Dim imageediter As ImageButton = CType(item(""editer"").Controls(0), ImageButton)")
                .WriteLine("        imagedelete.ToolTip = ""Effacer"" ")
                .WriteLine("        imageediter.ToolTip = ""Editer"" ")

                .WriteLine("        imagedelete.CommandArgument = CType(DataBinder.Eval(e.Item.DataItem, ""ID""), String)")
                .WriteLine("        imageediter.Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('Frm_" & nomSimple & "ADD.aspx?ID="" & CType(DataBinder.Eval(e.Item.DataItem, ""ID""), Long) & ""&"" & [Global].ACTION & ""="" & [Global].HideMenuHeader & ""',900,650));"")")
                .WriteLine("        REM Privilege")
                .WriteLine("        'imageediter.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Save, User_Connected.IdGroupeuser)")
                .WriteLine("        'imagedelete.Visible = Cls_Privilege.VerifyRightOnObject(Btn_Delete, User_Connected.IdGroupeuser)")
                .WriteLine("    End If")
                
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rdg" & nomSimple & "_NeedDataSource(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles rdg" & nomSimple & ".NeedDataSource")
                .WriteLine("    If IsPostBack Then")
                .WriteLine("        BindGrid(False)")
                .WriteLine("    End If")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub rbtnClearFilters_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnClearFilters.Click")
                .WriteLine("    Try")
                .WriteLine("        For Each column As GridColumn In rdg" & nomSimple & ".MasterTableView.Columns")
                .WriteLine("            column.CurrentFilterFunction = GridKnownFunction.NoFilter")
                .WriteLine("            column.CurrentFilterValue = String.Empty")
                .WriteLine("        Next")
                .WriteLine("        rdg" & nomSimple & ".MasterTableView.FilterExpression = String.Empty")
                .WriteLine("        rdg" & nomSimple & ".MasterTableView.Rebind()")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Protected Sub RadAjaxManager1_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs) Handles RadAjaxManager1.AjaxRequest")
                .WriteLine("    Try")
                .WriteLine("        Select Case e.Argument")
                .WriteLine("            Case ""Reload""")
                .WriteLine("                BindGrid(True)")
                .WriteLine("        End Select")
                .WriteLine("    Catch ex As Threading.ThreadAbortException")
                .WriteLine("    Catch ex As Rezo509Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("    Catch ex As Exception")
                .WriteLine("        MessageToShow(ex.Message)")
                .WriteLine("        [Global].WriteError(ex, User_Connected)")
                .WriteLine("    End Try")
                .WriteLine("End Sub")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("#End Region")
                .WriteLine()
            End With

            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

#End Region

    End Class

End Namespace
