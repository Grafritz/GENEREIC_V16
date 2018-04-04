Imports System.Data.Sql
Imports System.IO
Imports Microsoft.SqlServer
Imports Microsoft.SqlServer.Management.Common
Imports Microsoft.SqlServer.Management.Smo

Public Class FormGeneric

    Private Sub FormGeneric_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            'BackgroundWorker1.WorkerReportsProgress = True
            Btn_GenererScript.Enabled = False
            TabControl_Form.DeselectTab(TabBaseDeDonnees)
            TabControl_Form.SelectTab(TabParametre)
            'FillComboPrefixStoredProcedure()
            '_systeme.CleanData()
            'SetupReportGeneration()
            'SetupCurrentPrefix()
            'ispossible = True
            rcmb_DatabaseName.Items.Add("CREATE ")
            rcmb_DatabaseName.Items.Add("UPDATE ")
            CB_ActionStoreProcedure.SelectedIndex = 0

            LoadInstanceSQLServer()
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Erreur", MessageBoxButtons.OK)
            Error_Log(ex)
        End Try
    End Sub

#Region "LOAD INSTANCE SQL SERVER"

    Public Sub LoadInstanceSQLServer()
        'declare variables
        Dim dt As Data.DataTable = New DataTable()
        Dim dr As Data.DataRow = Nothing

        Try
            ''get sql server instances in to DataTable object
            Dim servers As Sql.SqlDataSourceEnumerator = SqlDataSourceEnumerator.Instance

            ' Check if datatable is empty
            If dt.Rows.Count = 0 Then
                ' Get a datatable with info about SQL Server 2000 and 2005 instances
                dt = servers.GetDataSources()

                ' List that will be combobox’s datasource   
                Dim listServers As List(Of String) = New List(Of String)
                ' For each element in the datatable add a new element in the list

                For Each rowServer As DataRow In dt.Rows
                    ' SQL Server instance could have instace name or only server name,
                    ' check this for show the name
                    If String.IsNullOrEmpty(rowServer("InstanceName").ToString()) Then
                        If rowServer("ServerName").ToString().Equals("JFDUVERS-PC") Then
                            listServers.Add(rowServer("ServerName").ToString() & "\MSSQLSERVER_08R2")
                        Else
                            listServers.Add(rowServer("ServerName").ToString())
                        End If
                    Else
                        listServers.Add(rowServer("ServerName") & "\" & rowServer("InstanceName"))
                    End If
                Next
                'Set servers list to combobox’s datasource
                Me.cmb_ServerName.DataSource = listServers
            End If
        Catch ex As System.Data.SqlClient.SqlException
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error!")

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error!")
        Finally
            'clean up ;)
            dr = Nothing
            dt = Nothing
        End Try
    End Sub

#End Region


#Region "INTERFACE WEB"

    Private Sub RB_Template_AdminLTE_Master_CheckedChanged(sender As Object, e As EventArgs) Handles RB_Template_AdminLTE_Master.CheckedChanged, RB_Template_Inspinia.CheckedChanged, RB_Template_CleanZone.CheckedChanged
        Try
            If RB_Template_AdminLTE_Master.Checked Then
                Me.PictureBox_Template.Image = Global.GENERIC_V16.My.Resources.Resources.AdminLTE_Master_fw

            ElseIf RB_Template_Inspinia.Checked Then
                Me.PictureBox_Template.Image = Global.GENERIC_V16.My.Resources.Resources.Inspinia_Template_fw

            ElseIf RB_Template_CleanZone.Checked Then
                Me.PictureBox_Template.Image = Global.GENERIC_V16.My.Resources.Resources.CleanZone_Template_fw

            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Erreur", MessageBoxButtons.OK)
            Error_Log(ex)
        End Try
    End Sub

    Private Sub RB_Formulaire_Tableau_CheckedChanged(sender As Object, e As EventArgs) Handles RB_Formulaire_Tableau.CheckedChanged, RB_Formulaire_FlowLayout.CheckedChanged
        Try
            If RB_Formulaire_Tableau.Checked Then
                Me.PictureBox_Formulaire.Image = Global.GENERIC_V16.My.Resources.Resources.CleanZone_Form_fw

            ElseIf RB_Formulaire_FlowLayout.Checked Then
                Me.PictureBox_Formulaire.Image = Global.GENERIC_V16.My.Resources.Resources.formFlowLayout_fw

            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Erreur", MessageBoxButtons.OK)
            Error_Log(ex)
        End Try
    End Sub
#End Region

#Region "LOAD DATABASE"

    Private Sub ValidateRequiredField_ForOthers()
        Try
            If txt_DatabaseName.Text.Trim <> "" And
                txt_ServerName.Text.Trim <> "" And
                Txt_Login.Text.Trim <> "" And
                Txt_Password.Text.Trim <> "" Then

                GiveConnectionString_ToHelper()
                If rbtn_SqlServer.Checked Then
                    Dim DatabaseName As String = txt_DatabaseName.Text
                    txt_LibraryName.Text = DatabaseName.Replace("db", "") + "Library"
                    SqlServerHelper.LoadUserTablesSchema(txt_ServerName.Text.Trim, Txt_Login.Text.Trim, Txt_Password.Text, txt_DatabaseName.Text, TreeView1)

                    'ElseIf rbtn_MySql.Checked Then
                    '    MySqlManager.LoadUserTablesSchema(TreeView1)
                    '    MySqlManager.InitializeDb()
                    '    BackgroundWorker1.RunWorkerAsync()

                    'ElseIf rbtn_PostGres.Checked Then
                    '    txt_LibraryName.Text = txt_DatabaseName.Text + "Library"
                    '    PostgresSqlManager.LoadUserTablesSchema(TreeView1)
                    '    PostgresSqlManager.InitializeDb()
                    '    BackgroundWorker1.RunWorkerAsync()

                    'ElseIf rbtn_Oracle.Checked Then

                Else
                    MessageBox.Show("Pas de base de donnees selectionnee")
                End If

                Btn_GenererScript.Enabled = True
                REM select Table
                TabControl_Form.DeselectTab(TabParametre)
                TabControl_Form.SelectTab(TabBaseDeDonnees)
            Else
                MessageBox.Show("Les Paramètres de Connexion à la Base de Données sont obligatoires")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Erreur", MessageBoxButtons.OK)
            Error_Log(ex)
        End Try
    End Sub

    Private Sub GiveConnectionString_ToHelper()
        SqlServerHelper.database = txt_DatabaseName.Text.Trim
        SqlServerHelper.servername = txt_ServerName.Text.Trim
        SqlServerHelper.password = Txt_Password.Text.Trim
        SqlServerHelper.user_login = Txt_Login.Text.Trim

        'MySqlHelper.database = txt_DatabaseName.Text.Trim
        'MySqlHelper.servername = txt_ServerName.Text.Trim
        'MySqlHelper.password = Txt_Password.Text.Trim
        'MySqlHelper.user_login = Txt_Login.Text.Trim

        'MySqlManager.servername = txt_ServerName.Text.Trim
        'MySqlManager.user_login = Txt_Login.Text.Trim
        'MySqlManager.password = Txt_Password.Text.Trim
        'MySqlManager.database = txt_DatabaseName.Text.Trim

        'PostgresSqlManager.servername = txt_ServerName.Text.Trim
        'PostgresSqlManager.user_login = Txt_Login.Text.Trim
        'PostgresSqlManager.password = Txt_Password.Text.Trim
        'PostgresSqlManager.database = txt_DatabaseName.Text.Trim

        'OracleHelper.database = txt_DatabaseName.Text.Trim
        'OracleHelper.servername = txt_ServerName.Text.Trim
        'OracleHelper.password = Txt_Password.Text.Trim
        'OracleHelper.user_login = Txt_Login.Text.Trim
    End Sub

#End Region

#Region "EVENTS CONNEXION"
    Private Sub Btn_ConnexionServerName_Click(sender As Object, e As EventArgs) Handles Btn_ConnexionServerName.Click
        txt_ServerName.Text = cmb_ServerName.Text
        txt_DatabaseName.Text = rcmb_DatabaseName.Text
        Try
            Application.DoEvents()
            If rbtn_Oracle.Checked Then
                'ValidateRequiredField_ForOracle()
            Else
                ValidateRequiredField_ForOthers()
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Connexion ServerName", MessageBoxButtons.OK)
            Error_Log("Btn_ConnexionServerName_Click", ex.Message)
        End Try
    End Sub

    Private Sub rcmb_DatabaseName_DropDown(sender As Object, e As EventArgs) Handles rcmb_DatabaseName.DropDown
        Try
            REM MS SQL Server
            If rbtn_SqlServer.Checked Then
                Try
                    rcmb_DatabaseName.Items.Clear()
                    Dim servconn As New ServerConnection(cmb_ServerName.Text)
                    servconn.LoginSecure = False
                    servconn.Login = Txt_Login.Text
                    servconn.Password = Txt_Password.Text
                    Dim server As New Server(servconn)
                    For Each db As Database In server.Databases
                        If Not rcmb_DatabaseName.Items.Contains(db.Name) Then
                            rcmb_DatabaseName.Items.Add(db.Name)
                        End If
                    Next
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            End If

            'REM MySql
            'If rbtn_MySql.Checked Then
            '    Try
            '        Dim myConStr As String = "Data Source=" & cmb_server.Text & ";User Id=" & Txt_Login.Text & ";Pwd=" & Txt_Password.Text & ";"
            '        Dim myConnection As New MySqlConnection(myConStr)
            '        myConnection.Open()
            '        Dim cmd As New MySqlCommand
            '        cmd.CommandText = "SHOW DATABASES"
            '        cmd.CommandType = CommandType.Text
            '        cmd.Connection = myConnection
            '        Dim ds As New DataSet
            '        Dim da As MySqlDataAdapter
            '        da = New MySqlDataAdapter(cmd)
            '        da.Fill(ds)
            '        cmd.Parameters.Clear()
            '        myConnection.Close()
            '        For Each row As DataRow In ds.Tables(0).Rows
            '            If Not rcmb_DatabaseName.Items.Contains(row(0)) Then
            '                rcmb_DatabaseName.Items.Add(row(0))
            '            End If

            '        Next
            '    Catch ex As Exception
            '        MessageBox.Show(ex.Message)
            '    End Try
            'End If

            'REM ProGres SQL
            'If rbtn_PostGres.Checked Then
            '    Dim servername As String = txt_ServerName.Text
            '    Dim port As String = "5432"
            '    Dim user_login As String = Txt_Login.Text
            '    Dim password As String = Txt_Password.Text


            '    Dim ConString As String = String.Format("Server={0};Port={1};User Id={2};Password={3};", servername, port, user_login, password)
            '    Dim myConStr As String = "Data Source=" & cmb_server.Text & ";User Id=" & Txt_Login.Text & ";Pwd=" & Txt_Password.Text & ";"
            '    Dim myConnection As New NpgsqlConnection(ConString)
            '    myConnection.Open()
            '    Dim cmd As New NpgsqlCommand
            '    cmd.CommandText = "SELECT datname FROM pg_database WHERE datistemplate = false;"
            '    cmd.CommandType = CommandType.Text
            '    cmd.Connection = myConnection
            '    Dim ds As New DataSet
            '    Dim da As NpgsqlDataAdapter
            '    da = New NpgsqlDataAdapter(cmd)
            '    da.Fill(ds)
            '    cmd.Parameters.Clear()
            '    myConnection.Close()
            '    For Each row As DataRow In ds.Tables(0).Rows
            '        If Not rcmb_DatabaseName.Items.Contains(row(0)) Then
            '            rcmb_DatabaseName.Items.Add(row(0))
            '        End If

            '    Next
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

        EnableBtnConnexion()
    End Sub

    Public Sub EnableBtnConnexion()
        If rbtn_Oracle.Checked Then
            Btn_ConnexionServerName.Enabled = IIf(cmb_ServerName.Text.Trim.Equals("") OrElse Txt_Login.Text.Trim.Equals(""), False, True)
        Else
            Btn_ConnexionServerName.Enabled = IIf(cmb_ServerName.Text.Trim.Equals("") AndAlso rcmb_DatabaseName.Text.Trim.Equals("") AndAlso Txt_Login.Text.Trim.Equals(""), False, True)
        End If
    End Sub


#Region "Folder Browser"
    Public Sub Folder_Browser_Dialog(ByVal _FolderBrowserDialog As FolderBrowserDialog, ByVal _Textbox As TextBox)
        Try
            Dim dlgResult As DialogResult = _FolderBrowserDialog.ShowDialog()

            If dlgResult = Windows.Forms.DialogResult.OK Then
                _Textbox.Text = _FolderBrowserDialog.SelectedPath
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Folder Browser Dialog", MessageBoxButtons.OK)
            Error_Log("Folder_Browser_Dialog", ex.Message)
        End Try
    End Sub

    Private Sub Btn_FolderBrowserDialog_Script_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_FolderBrowserDialog_Script.Click
        Folder_Browser_Dialog(FolderBrowserDialog1, txt_PathGenerate_ScriptFile)
    End Sub

    Private Sub txt_PathGenerate_ScriptFile_DoubleClick(sender As Object, e As EventArgs) Handles txt_PathGenerate_ScriptFile.DoubleClick
        Folder_Browser_Dialog(FolderBrowserDialog1, txt_PathGenerate_ScriptFile)
    End Sub
#End Region

#End Region


#Region "ERREUR"
    Public Shared Sub Error_Log(ByVal requestedMethod As String, ByVal errorMessage As String)
        Try
            Dim sw As StreamWriter
            Dim _path As String = Application.StartupPath & "\Log\"
            If Not Directory.Exists(_path) Then
                Directory.CreateDirectory(_path)
            End If
            sw = New StreamWriter(_path + Date.Now.ToString("dd-MMM-yyy") + ".txt", True)
            'sw = New StreamWriter(System.Configuration.ConfigurationManager.AppSettings("logDirectory") + Date.Now.ToString("dd-MMM-yyy") + ".txt", True)

            sw.WriteLine(Date.Now.ToString("hh:mm --> ") + " - " + requestedMethod + "; Error : " + errorMessage)
            sw.Flush()
            sw.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, ".:: Erreur Save Log ::.", MessageBoxButtons.OK)
        End Try
    End Sub

    Public Shared Sub Error_Log(ByVal errorMessage As Exception)
        Try
            Dim sw As StreamWriter
            Dim _path As String = Application.StartupPath & "\Log\"
            If Not Directory.Exists(_path) Then
                Directory.CreateDirectory(_path)
            End If
            sw = New StreamWriter(_path + Date.Now.ToString("dd-MMM-yyy") + ".txt", True)
            'sw = New StreamWriter(System.Configuration.ConfigurationManager.AppSettings("logDirectory") + Date.Now.ToString("dd-MMM-yyy") + ".txt", True)

            sw.WriteLine(Date.Now.ToString("hh:mm --> ") + ": Error : " + errorMessage.ToString)
            sw.Flush()
            sw.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, ".:: Erreur Save Log ::.", MessageBoxButtons.OK)
        End Try
    End Sub

#End Region

#Region "TABS BASE DE DONNEES"

#Region "Event "
    Private Sub Btn_GenererScript_Click(sender As Object, e As EventArgs) Handles Btn_GenererScript.Click
        Try
            If rbtn_SqlServer.Checked Then
                GenerateScriptForSqlServer() 's, supdate, sdelete, slistall, slistallforeign, sselectindex, sselect, ssupdateparentaddchild, ssupdateparentremovechild, slistallchildinparent, slistallchildnotintparent, sselectanycolumn)

                'ElseIf rbtn_Oracle.Checked Then
                '    GenerateScriptForOracle(s, supdate, sdelete, slistall, slistallforeign, sselectindex, sselect)

                'ElseIf rbtn_MySql.Checked Then
                '    GenerateScriptForMySql(s, supdate, sdelete, slistall, slistallforeign, sselectindex, sselect, slistallPagination)
                'ElseIf rbtn_PostGres.Checked Then
                '    If txt_FkPrefix.Text = "" Then
                '        MessageBox.Show("Le prefixe des cles etrangeres n'est pas renseigne")
                '    Else
                '        PostgresSqlManager.ForeinKeyPrefix = txt_FkPrefix.Text.Trim
                '        GenerateScriptForPostGres(s, supdate, sdelete, slistall, slistallforeign, sselectindex, sselect)
                '    End If
            Else
                MessageBox.Show("Aucune base de données sélectionnée")
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Connexion ServerName", MessageBoxButtons.OK)
            Error_Log(ex)
        End Try
    End Sub

    Private Sub btnOpenOutput_Click(sender As Object, e As EventArgs) Handles btnOpenOutput.Click
        Try
            Dim Repertoire As String = Cls_Enumeration.GetPath(txt_PathGenerate_ScriptFile.Text.Trim)
            If Directory.Exists(Repertoire) Then
                Process.Start(Repertoire)
            Else
                MessageBox.Show("Le repertoire n'existe pas.", "Repertoire", MessageBoxButtons.OK)
            End If
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Connexion ServerName", MessageBoxButtons.OK)
            Error_Log(ex)
        End Try
    End Sub
#End Region

#Region "Generate Script"
    Private Sub GenerateScriptForSqlServer() 'ByVal sListAllByAnyField As String) '
        TreeView2.Nodes.Clear()
#Region "Variable"
        Dim _createStore_String As String = ""
        Dim _UpdateStore_Str As String = ""
        Dim _UpdateStore_String As String = ""
        Dim slistall As String = ""
        Dim sselect As String = ""
        Dim sselectindex As String = ""
        Dim sselectanycolumn As String = ""
        Dim slistallforeign As String = ""
        Dim slistallPagination As String = ""

        Dim ssupdateparentaddchild As String = ""
        Dim ssupdateparentremovechild As String = ""
        Dim slistallchildinparent As String = ""
        Dim slistallchildnotintparent As String = ""
        Dim _PathForGenerate_Script As String = Cls_Enumeration.GetPath_SQLServer_Script(txt_PathGenerate_ScriptFile.Text.Trim, txt_DatabaseName.Text.Trim)

        Dim path As String = _PathForGenerate_Script & "01_Insert_Script.sql"
        Dim pathupdate As String = _PathForGenerate_Script & "02_Update_Script.sql"
        Dim pathdelete As String = _PathForGenerate_Script & "03_Delete_Script.sql"
        Dim pathlistall As String = _PathForGenerate_Script & "04_ListAll_Script.sql"
        Dim pathselect As String = _PathForGenerate_Script & "05_Select_Script.sql"
        Dim pathselectindex As String = _PathForGenerate_Script & "06_Select_Index_Script.sql"
        Dim pathselectanycolumn As String = _PathForGenerate_Script & "ListAllAnyColumnScript.sql"
        Dim pathlistallforeign As String = _PathForGenerate_Script & "ListAllForeignScript.sql"
        Dim pathupdateparentaddchild As String = _PathForGenerate_Script & "UpdateParentAddChildScript.sql"
        Dim pathupdateparentremovechild As String = _PathForGenerate_Script & "UpdateParentRemoveChildScript.sql"
        Dim pathlistallparentinchild As String = _PathForGenerate_Script & "ListAllParentInChildScript.sql"
        Dim pathlistallparentnotinchild As String = _PathForGenerate_Script & "ListAllParentNotInChildScript.sql"

#End Region
        Dim ds As DataSet
        For Each tr As TreeNode In TreeView1.Nodes
            If tr.Checked Then
                'Dim _table As Cls_Table
                Dim node As New TreeNode
                node.Text = tr.Text

                SqlServerHelper.ForeinKeyPrefix = txt_FkPrefix.Text.Trim
                SqlServerHelper.CurrentPrefixStored = CB_ActionStoreProcedure.Text

                _createStore_String &= SqlServer.Fast.ScriptGenerator.CreateStore(tr.Text)
                _createStore_String &= Chr(13)

                _UpdateStore_Str &= SqlServer.Fast.ScriptGenerator.UpdateStore(tr.Text)
                _UpdateStore_Str &= Chr(13)

                _UpdateStore_String &= SqlServer.Fast.ScriptGenerator.DeleteStore(tr.Text)
                _UpdateStore_String &= Chr(13)

                slistall &= SqlServer.Fast.ScriptGenerator.ListAllStore(tr.Text)
                slistall &= Chr(13)

                sselect &= SqlServer.Fast.ScriptGenerator.SelectStore(tr.Text)
                sselect &= Chr(13)

                sselectindex &= SqlServer.Fast.ScriptGenerator.SelectByIndexStore(tr.Text)
                sselectindex &= Chr(13)

                slistallforeign &= SqlServer.Fast.ScriptGenerator.ListAllByForeignKey(tr.Text)
                slistallforeign &= Chr(13)

                SqlServer.Fast.VbClassGenerator.CreateFile(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_DatabaseName.Text)

                REM AdminLTE-Master
                If RB_Template_AdminLTE_Master.Checked Then
                    'Interface ADD EDIT
                    SqlServer.Fast.AspFormGenerator.CreateInterfaceCodeAsp(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_DatabaseName.Text)
                    SqlServer.Fast.AspFormGenerator.CreateInterfaceCodeBehind(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_LibraryName, txt_DatabaseName.Text)

                    'Interface LISTING
                    SqlServer.Fast.AspFormGenerator.CreateListingCodeAsp(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_DatabaseName.Text)
                    SqlServer.Fast.AspFormGenerator.CreateListingCodeBehind(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_LibraryName, txt_DatabaseName.Text)

                ElseIf RB_Template_CleanZone.Checked Then ' CLeanZone
                    'Interface ADD EDIT
                    If RB_Formulaire_Tableau.Checked Then
                        SqlServer.Fast.AspFormGenerator.CreateInterface_Tableau_Formulaire_Design(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_DatabaseName.Text)
                        SqlServer.Fast.AspFormGenerator.CreateInterface_Tableau_Formulaire_CodeBehind(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_LibraryName, txt_DatabaseName.Text)

                    ElseIf RB_Formulaire_FlowLayout.Checked Then
                        SqlServer.Fast.AspFormGenerator.CreateInterface_Tableau_Formulaire_Design(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_DatabaseName.Text)
                        SqlServer.Fast.AspFormGenerator.CreateInterfaceCodeBehind(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_LibraryName, txt_DatabaseName.Text)

                    End If

                    'Interface LISTING
                    SqlServer.Fast.AspFormGenerator.CreateInterface_CleanZone_Listing_Design(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_DatabaseName.Text)
                    SqlServer.Fast.AspFormGenerator.CreateInterface_CleanZone_Listing_CodeBehind(tr.Text, txt_PathGenerate_ScriptFile, ListBox_NameSpace, txt_LibraryName, txt_DatabaseName.Text)

                ElseIf RB_Template_Inspinia.Checked Then

                End If

                ds = SqlServerHelper.LoadTableStructure(tr.Text)

                'TreeView2.Nodes.Clear()
                For Each dt As DataRow In ds.Tables(1).Rows
                    node.Nodes.Add(dt(0))
                Next
                TreeView2.Nodes.Add(node)
            End If
        Next


        REM on verifie si le repertoir existe bien
        If Not Directory.Exists(_PathForGenerate_Script) Then
            Directory.CreateDirectory(_PathForGenerate_Script)
        End If
        'If txt_PathGenerate_ScriptFile.Text.Trim <> "" Then
        '    If Not Directory.Exists(txt_PathGenerate_ScriptFile.Text.Trim & "" & RepertoiresName & "" & txt_DatabaseName.Text.Trim & SQLServer_Script) Then
        '        Directory.CreateDirectory(txt_PathGenerate_ScriptFile.Text.Trim & "" & RepertoiresName & "" & txt_DatabaseName.Text.Trim & SQLServer_Script)
        '    End If
        'Else
        '    If Not Directory.Exists(Application.StartupPath & "" & RepertoiresName & "" & txt_DatabaseName.Text.Trim & SQLServer_Script) Then
        '        Directory.CreateDirectory(Application.StartupPath & "" & RepertoiresName & "" & txt_DatabaseName.Text.Trim & SQLServer_Script)
        '    End If
        'End If

        Dim fs As FileStream = File.Create(path, 1024)
        fs.Close()

        Dim fs_update As FileStream = File.Create(pathupdate, 1024)
        fs_update.Close()

        Dim fs_delete As FileStream = File.Create(pathdelete, 1024)
        fs_delete.Close()

        Dim fs_listAll As FileStream = File.Create(pathlistall, 1024)
        fs_listAll.Close()

        Dim fs_select As FileStream = File.Create(pathselect, 1024)
        fs_select.Close()

        Dim fs_selectindex As FileStream = File.Create(pathselectindex, 1024)
        fs_selectindex.Close()

        Dim fs_listAllforeign As FileStream = File.Create(pathlistallforeign, 1024)
        fs_listAllforeign.Close()

        Dim fs_listAllanycolumn As FileStream = File.Create(pathselectanycolumn, 1024)
        fs_listAllanycolumn.Close()


        Dim fs_updateparentaddchild As FileStream = File.Create(pathupdateparentaddchild, 1024)
        fs_updateparentaddchild.Close()


        Dim fs_updateparentremovechild As FileStream = File.Create(pathupdateparentremovechild, 1024)
        fs_updateparentremovechild.Close()

        Dim fs_listallparentinchild As FileStream = File.Create(pathlistallparentinchild, 1024)
        fs_listallparentinchild.Close()

        Dim fs_listallparentnotinchild As FileStream = File.Create(pathlistallparentnotinchild, 1024)
        fs_listallparentnotinchild.Close()

        'If IntelligentMode And rbtnIMode_Yes.Checked Then
        '    ssupdateparentaddchild &= SqlServer.IMode.ScriptGenerator.UpdateStoreAddChild()
        '    ssupdateparentaddchild &= Chr(13)
        '    ssupdateparentremovechild &= SqlServer.IMode.ScriptGenerator.UpdateStoreRemoveChild()
        '    ssupdateparentremovechild &= Chr(13)
        '    slistallchildinparent &= SqlServer.IMode.ScriptGenerator.ListAllChildinParent()
        '    slistallchildinparent &= Chr(13)
        '    slistallchildnotintparent &= SqlServer.IMode.ScriptGenerator.ListAllChildNotinParent()
        '    slistallchildnotintparent &= Chr(13)

        '    Dim objWriterUpdateparentaddchild As New System.IO.StreamWriter(pathupdateparentaddchild, True, System.Text.Encoding.UTF8)
        '    Dim objWriterUpdateParentRemovechild As New System.IO.StreamWriter(pathupdateparentremovechild, True, System.Text.Encoding.UTF8)
        '    Dim objWriterlistAllparentinchild As New System.IO.StreamWriter(pathlistallparentinchild, True, System.Text.Encoding.UTF8)
        '    Dim objWriterlistAllparentnotinChild As New System.IO.StreamWriter(pathlistallparentnotinchild, True, System.Text.Encoding.UTF8)

        '    With objWriterUpdateparentaddchild

        '        .WriteLine(ssupdateparentaddchild)
        '        .Close()
        '    End With

        '    With objWriterUpdateParentRemovechild

        '        .WriteLine(ssupdateparentremovechild)
        '        .Close()
        '    End With

        '    With objWriterlistAllparentinchild

        '        .WriteLine(slistallchildinparent)
        '        .Close()
        '    End With

        '    With objWriterlistAllparentnotinChild

        '        .WriteLine(slistallchildnotintparent)
        '        .Close()
        '    End With

        '    SqlServer.IMode.VbClassGenerator.CreateFileForParent(txt_PathGenerate_ScriptFile, ListBox_NameSpace)
        '    SqlServer.IMode.VbClassGenerator.CreateFileForChild(txt_PathGenerate_ScriptFile, ListBox_NameSpace)
        '    For Each group In Cls_GroupTable.SearchAll
        '        SqlServer.IMode.AspFormGenerator.CreateParentChildInterfaceAsp("", txt_PathGenerate_ScriptFile, ListBox_NameSpace, group, txt_LibraryName.Text)
        '        SqlServer.IMode.AspFormGenerator.CreateParentChildInterfaceCodeBehind("", txt_PathGenerate_ScriptFile, ListBox_NameSpace, group, txt_LibraryName.Text)
        '        SqlServer.IMode.AspFormGenerator.CreateSendRemoveChildInParentAsp("", txt_PathGenerate_ScriptFile, ListBox_NameSpace, group)
        '        SqlServer.IMode.AspFormGenerator.CreateSendRemoveChildInParentBehind("", txt_PathGenerate_ScriptFile, ListBox_NameSpace, group, txt_LibraryName.Text)
        '    Next
        'End If

        Dim objWriter As New System.IO.StreamWriter(path, True, System.Text.Encoding.UTF8)

        Dim objWriterupdate As New System.IO.StreamWriter(pathupdate, True, System.Text.Encoding.UTF8)
        Dim objWriterdelete As New System.IO.StreamWriter(pathdelete, True, System.Text.Encoding.UTF8)
        Dim objWriterlistAll As New System.IO.StreamWriter(pathlistall, True, System.Text.Encoding.UTF8)
        Dim objWriterSelect As New System.IO.StreamWriter(pathselect, True, System.Text.Encoding.UTF8)
        Dim objWriterSelectIndex As New System.IO.StreamWriter(pathselectindex, True, System.Text.Encoding.UTF8)
        Dim objWriterListallForeign As New System.IO.StreamWriter(pathlistallforeign, True, System.Text.Encoding.UTF8)
        Dim objWriterListallAnycolumn As New System.IO.StreamWriter(pathselectanycolumn, True, System.Text.Encoding.UTF8)


        objWriter.WriteLine(_createStore_String)
        objWriter.Close()

        objWriterupdate.WriteLine()
        objWriterupdate.WriteLine(_UpdateStore_Str)
        objWriterupdate.Close()

        objWriterdelete.WriteLine()
        objWriterdelete.WriteLine(_UpdateStore_String)
        objWriterdelete.Close()

        objWriterlistAll.WriteLine()
        objWriterlistAll.WriteLine(slistall)
        objWriterlistAll.Close()

        objWriterSelect.WriteLine()
        objWriterSelect.WriteLine(sselect)
        objWriterSelect.Close()

        objWriterSelectIndex.WriteLine()
        objWriterSelectIndex.WriteLine(sselectindex)
        objWriterSelectIndex.Close()

        With objWriterListallForeign
            .WriteLine(slistallforeign)
            .Close()
        End With

        ''With objWriterListallAnycolumn
        ''    .WriteLine(sListAllByAnyField)
        ''    .Close()
        ''End With
    End Sub

#End Region
#End Region

End Class
