<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormGeneric
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormGeneric))
        Me.TabControl_Form = New System.Windows.Forms.TabControl()
        Me.TabParametre = New System.Windows.Forms.TabPage()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.txt_PathGenerate_ScriptFile = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Btn_FolderBrowserDialog_Script = New System.Windows.Forms.Button()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.cmb_ServerName = New System.Windows.Forms.ComboBox()
        Me.rcmb_DatabaseName = New System.Windows.Forms.ComboBox()
        Me.GrpBox_DB = New System.Windows.Forms.GroupBox()
        Me.rbtn_PostGres = New System.Windows.Forms.RadioButton()
        Me.rbtn_MySql = New System.Windows.Forms.RadioButton()
        Me.rbtn_SqlServer = New System.Windows.Forms.RadioButton()
        Me.rbtn_Oracle = New System.Windows.Forms.RadioButton()
        Me.Btn_ConnexionServerName = New System.Windows.Forms.Button()
        Me.txt_DatabaseName = New System.Windows.Forms.TextBox()
        Me.lbl_Database_port = New System.Windows.Forms.Label()
        Me.txt_ServerName = New System.Windows.Forms.TextBox()
        Me.Txt_Login = New System.Windows.Forms.TextBox()
        Me.Txt_Password = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lbl_Servername = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Btn_Remove = New System.Windows.Forms.Button()
        Me.Btn_AddNameSpace = New System.Windows.Forms.Button()
        Me.txt_NameSpace = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ListBox_NameSpace = New System.Windows.Forms.ListBox()
        Me.TabBaseDeDonnees = New System.Windows.Forms.TabPage()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.CB_ActionStoreProcedure = New System.Windows.Forms.ComboBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.ddl_PrefixStoredProcedure = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txt_projectName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txt_LibraryName = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txt_table = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txt_FkPrefix = New System.Windows.Forms.TextBox()
        Me.lbl_FkPrefix = New System.Windows.Forms.Label()
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.TreeView2 = New System.Windows.Forms.TreeView()
        Me.Btn_GenererScript = New System.Windows.Forms.Button()
        Me.btnOpenOutput = New System.Windows.Forms.Button()
        Me.Btn_GenererParIndex = New System.Windows.Forms.Button()
        Me.TabInterfaceWEB = New System.Windows.Forms.TabPage()
        Me.GroupBox14 = New System.Windows.Forms.GroupBox()
        Me.PictureBox_Formulaire = New System.Windows.Forms.PictureBox()
        Me.GroupBox13 = New System.Windows.Forms.GroupBox()
        Me.PictureBox_Template = New System.Windows.Forms.PictureBox()
        Me.GroupBox11 = New System.Windows.Forms.GroupBox()
        Me.RB_Template_Inspinia = New System.Windows.Forms.RadioButton()
        Me.RB_Template_CleanZone = New System.Windows.Forms.RadioButton()
        Me.RB_Template_AdminLTE_Master = New System.Windows.Forms.RadioButton()
        Me.GroupBox12 = New System.Windows.Forms.GroupBox()
        Me.RB_Formulaire_FlowLayout = New System.Windows.Forms.RadioButton()
        Me.RB_Formulaire_Tableau = New System.Windows.Forms.RadioButton()
        Me.TabGroupe = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnGenerateScriptGroup = New System.Windows.Forms.Button()
        Me.treeViewGroupe = New System.Windows.Forms.TreeView()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.dtgGroupe = New System.Windows.Forms.DataGridView()
        Me.bnt_AddGroupTable = New System.Windows.Forms.Button()
        Me.tab_EcranProgression = New System.Windows.Forms.TabPage()
        Me.progressTrue10 = New System.Windows.Forms.ProgressBar()
        Me.progressTrue9 = New System.Windows.Forms.ProgressBar()
        Me.progressTrue8 = New System.Windows.Forms.ProgressBar()
        Me.progressTrue7 = New System.Windows.Forms.ProgressBar()
        Me.progressTrue6 = New System.Windows.Forms.ProgressBar()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.progressTrue5 = New System.Windows.Forms.ProgressBar()
        Me.progressTrue4 = New System.Windows.Forms.ProgressBar()
        Me.progressTrue3 = New System.Windows.Forms.ProgressBar()
        Me.progressTrue2 = New System.Windows.Forms.ProgressBar()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.progressTrue1 = New System.Windows.Forms.ProgressBar()
        Me.tabReportGeneration = New System.Windows.Forms.TabPage()
        Me.txt_AliasMaster = New System.Windows.Forms.TextBox()
        Me.pnlDetailReportGeneration = New System.Windows.Forms.Panel()
        Me.pnlParametreDate = New System.Windows.Forms.GroupBox()
        Me.dtgDateParameter = New System.Windows.Forms.DataGridView()
        Me.btnDateParameterGroupe = New System.Windows.Forms.Button()
        Me.GroupBox10 = New System.Windows.Forms.GroupBox()
        Me.rdgWhereCondition = New System.Windows.Forms.DataGridView()
        Me.btnAddCondition = New System.Windows.Forms.Button()
        Me.btnGenerateReportScript = New System.Windows.Forms.Button()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.treeView_ReportColumn = New System.Windows.Forms.TreeView()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.dtgReportParameters = New System.Windows.Forms.DataGridView()
        Me.btnAddReportParameter = New System.Windows.Forms.Button()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.txtAliasReportTable = New System.Windows.Forms.TextBox()
        Me.ddl_OtherReportTable = New System.Windows.Forms.ComboBox()
        Me.dtgOtherReportTable = New System.Windows.Forms.DataGridView()
        Me.btnAddNewTable = New System.Windows.Forms.Button()
        Me.ddl_TypeReport = New System.Windows.Forms.ComboBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.btnValidatePrincipalTable = New System.Windows.Forms.Button()
        Me.ddl_Parent = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txt_ReportProcName = New System.Windows.Forms.TextBox()
        Me.lbl_ReportName = New System.Windows.Forms.Label()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FichierToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NouveauProjetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OuvrirProjetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AideToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AProposToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.TabControl_Form.SuspendLayout()
        Me.TabParametre.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GrpBox_DB.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TabBaseDeDonnees.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TabInterfaceWEB.SuspendLayout()
        Me.GroupBox14.SuspendLayout()
        CType(Me.PictureBox_Formulaire, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox13.SuspendLayout()
        CType(Me.PictureBox_Template, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox11.SuspendLayout()
        Me.GroupBox12.SuspendLayout()
        Me.TabGroupe.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.dtgGroupe, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tab_EcranProgression.SuspendLayout()
        Me.tabReportGeneration.SuspendLayout()
        Me.pnlDetailReportGeneration.SuspendLayout()
        Me.pnlParametreDate.SuspendLayout()
        CType(Me.dtgDateParameter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox10.SuspendLayout()
        CType(Me.rdgWhereCondition, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox9.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        CType(Me.dtgReportParameters, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox7.SuspendLayout()
        CType(Me.dtgOtherReportTable, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl_Form
        '
        Me.TabControl_Form.Controls.Add(Me.TabParametre)
        Me.TabControl_Form.Controls.Add(Me.TabBaseDeDonnees)
        Me.TabControl_Form.Controls.Add(Me.TabInterfaceWEB)
        Me.TabControl_Form.Controls.Add(Me.TabGroupe)
        Me.TabControl_Form.Controls.Add(Me.tab_EcranProgression)
        Me.TabControl_Form.Controls.Add(Me.tabReportGeneration)
        Me.TabControl_Form.Location = New System.Drawing.Point(0, 38)
        Me.TabControl_Form.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabControl_Form.Name = "TabControl_Form"
        Me.TabControl_Form.SelectedIndex = 0
        Me.TabControl_Form.Size = New System.Drawing.Size(1326, 1012)
        Me.TabControl_Form.TabIndex = 16
        '
        'TabParametre
        '
        Me.TabParametre.Controls.Add(Me.GroupBox5)
        Me.TabParametre.Controls.Add(Me.GroupBox4)
        Me.TabParametre.Controls.Add(Me.GroupBox3)
        Me.TabParametre.Location = New System.Drawing.Point(4, 29)
        Me.TabParametre.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabParametre.Name = "TabParametre"
        Me.TabParametre.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabParametre.Size = New System.Drawing.Size(1318, 979)
        Me.TabParametre.TabIndex = 1
        Me.TabParametre.Text = "Paramètres"
        Me.TabParametre.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox5.Controls.Add(Me.txt_PathGenerate_ScriptFile)
        Me.GroupBox5.Controls.Add(Me.Label6)
        Me.GroupBox5.Controls.Add(Me.Btn_FolderBrowserDialog_Script)
        Me.GroupBox5.Location = New System.Drawing.Point(9, 360)
        Me.GroupBox5.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox5.Size = New System.Drawing.Size(1294, 105)
        Me.GroupBox5.TabIndex = 2
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Path to Generate Script SQL and Class"
        '
        'txt_PathGenerate_ScriptFile
        '
        Me.txt_PathGenerate_ScriptFile.Location = New System.Drawing.Point(53, 49)
        Me.txt_PathGenerate_ScriptFile.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_PathGenerate_ScriptFile.Name = "txt_PathGenerate_ScriptFile"
        Me.txt_PathGenerate_ScriptFile.Size = New System.Drawing.Size(738, 26)
        Me.txt_PathGenerate_ScriptFile.TabIndex = 17
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(9, 25)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(115, 20)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Path Script :"
        '
        'Btn_FolderBrowserDialog_Script
        '
        Me.Btn_FolderBrowserDialog_Script.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Btn_FolderBrowserDialog_Script.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Btn_FolderBrowserDialog_Script.Image = Global.GENERIC_V16.My.Resources.Resources.rep
        Me.Btn_FolderBrowserDialog_Script.Location = New System.Drawing.Point(14, 48)
        Me.Btn_FolderBrowserDialog_Script.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Btn_FolderBrowserDialog_Script.Name = "Btn_FolderBrowserDialog_Script"
        Me.Btn_FolderBrowserDialog_Script.Size = New System.Drawing.Size(39, 35)
        Me.Btn_FolderBrowserDialog_Script.TabIndex = 8
        Me.Btn_FolderBrowserDialog_Script.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox4.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox4.Controls.Add(Me.cmb_ServerName)
        Me.GroupBox4.Controls.Add(Me.rcmb_DatabaseName)
        Me.GroupBox4.Controls.Add(Me.GrpBox_DB)
        Me.GroupBox4.Controls.Add(Me.Btn_ConnexionServerName)
        Me.GroupBox4.Controls.Add(Me.txt_DatabaseName)
        Me.GroupBox4.Controls.Add(Me.lbl_Database_port)
        Me.GroupBox4.Controls.Add(Me.txt_ServerName)
        Me.GroupBox4.Controls.Add(Me.Txt_Login)
        Me.GroupBox4.Controls.Add(Me.Txt_Password)
        Me.GroupBox4.Controls.Add(Me.Label5)
        Me.GroupBox4.Controls.Add(Me.Label4)
        Me.GroupBox4.Controls.Add(Me.lbl_Servername)
        Me.GroupBox4.Location = New System.Drawing.Point(566, 11)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox4.Size = New System.Drawing.Size(732, 340)
        Me.GroupBox4.TabIndex = 1
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Paramètres Connexion Base de donnees"
        '
        'cmb_ServerName
        '
        Me.cmb_ServerName.FormattingEnabled = True
        Me.cmb_ServerName.Location = New System.Drawing.Point(132, 92)
        Me.cmb_ServerName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmb_ServerName.Name = "cmb_ServerName"
        Me.cmb_ServerName.Size = New System.Drawing.Size(383, 28)
        Me.cmb_ServerName.TabIndex = 16
        '
        'rcmb_DatabaseName
        '
        Me.rcmb_DatabaseName.FormattingEnabled = True
        Me.rcmb_DatabaseName.Location = New System.Drawing.Point(132, 222)
        Me.rcmb_DatabaseName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.rcmb_DatabaseName.Name = "rcmb_DatabaseName"
        Me.rcmb_DatabaseName.Size = New System.Drawing.Size(383, 28)
        Me.rcmb_DatabaseName.TabIndex = 16
        '
        'GrpBox_DB
        '
        Me.GrpBox_DB.Controls.Add(Me.rbtn_PostGres)
        Me.GrpBox_DB.Controls.Add(Me.rbtn_MySql)
        Me.GrpBox_DB.Controls.Add(Me.rbtn_SqlServer)
        Me.GrpBox_DB.Controls.Add(Me.rbtn_Oracle)
        Me.GrpBox_DB.Location = New System.Drawing.Point(9, 28)
        Me.GrpBox_DB.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GrpBox_DB.Name = "GrpBox_DB"
        Me.GrpBox_DB.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GrpBox_DB.Size = New System.Drawing.Size(508, 55)
        Me.GrpBox_DB.TabIndex = 13
        Me.GrpBox_DB.TabStop = False
        '
        'rbtn_PostGres
        '
        Me.rbtn_PostGres.AutoSize = True
        Me.rbtn_PostGres.Location = New System.Drawing.Point(400, 20)
        Me.rbtn_PostGres.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.rbtn_PostGres.Name = "rbtn_PostGres"
        Me.rbtn_PostGres.Size = New System.Drawing.Size(97, 24)
        Me.rbtn_PostGres.TabIndex = 3
        Me.rbtn_PostGres.Text = "Postgres"
        Me.rbtn_PostGres.UseVisualStyleBackColor = True
        '
        'rbtn_MySql
        '
        Me.rbtn_MySql.AutoSize = True
        Me.rbtn_MySql.Location = New System.Drawing.Point(266, 20)
        Me.rbtn_MySql.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.rbtn_MySql.Name = "rbtn_MySql"
        Me.rbtn_MySql.Size = New System.Drawing.Size(86, 24)
        Me.rbtn_MySql.TabIndex = 2
        Me.rbtn_MySql.Text = "MySQL"
        Me.rbtn_MySql.UseVisualStyleBackColor = True
        '
        'rbtn_SqlServer
        '
        Me.rbtn_SqlServer.AutoSize = True
        Me.rbtn_SqlServer.Checked = True
        Me.rbtn_SqlServer.Location = New System.Drawing.Point(123, 20)
        Me.rbtn_SqlServer.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.rbtn_SqlServer.Name = "rbtn_SqlServer"
        Me.rbtn_SqlServer.Size = New System.Drawing.Size(112, 24)
        Me.rbtn_SqlServer.TabIndex = 1
        Me.rbtn_SqlServer.TabStop = True
        Me.rbtn_SqlServer.Text = "SQLServer"
        Me.rbtn_SqlServer.UseVisualStyleBackColor = True
        '
        'rbtn_Oracle
        '
        Me.rbtn_Oracle.AutoSize = True
        Me.rbtn_Oracle.Location = New System.Drawing.Point(9, 20)
        Me.rbtn_Oracle.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.rbtn_Oracle.Name = "rbtn_Oracle"
        Me.rbtn_Oracle.Size = New System.Drawing.Size(80, 24)
        Me.rbtn_Oracle.TabIndex = 0
        Me.rbtn_Oracle.Text = "Oracle"
        Me.rbtn_Oracle.UseVisualStyleBackColor = True
        '
        'Btn_ConnexionServerName
        '
        Me.Btn_ConnexionServerName.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Btn_ConnexionServerName.Enabled = False
        Me.Btn_ConnexionServerName.Location = New System.Drawing.Point(130, 275)
        Me.Btn_ConnexionServerName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Btn_ConnexionServerName.Name = "Btn_ConnexionServerName"
        Me.Btn_ConnexionServerName.Size = New System.Drawing.Size(156, 45)
        Me.Btn_ConnexionServerName.TabIndex = 4
        Me.Btn_ConnexionServerName.Text = "Connexion"
        Me.Btn_ConnexionServerName.UseVisualStyleBackColor = True
        '
        'txt_DatabaseName
        '
        Me.txt_DatabaseName.Location = New System.Drawing.Point(296, 282)
        Me.txt_DatabaseName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_DatabaseName.Name = "txt_DatabaseName"
        Me.txt_DatabaseName.Size = New System.Drawing.Size(28, 26)
        Me.txt_DatabaseName.TabIndex = 1
        Me.txt_DatabaseName.Visible = False
        '
        'lbl_Database_port
        '
        Me.lbl_Database_port.AutoSize = True
        Me.lbl_Database_port.Location = New System.Drawing.Point(17, 228)
        Me.lbl_Database_port.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbl_Database_port.Name = "lbl_Database_port"
        Me.lbl_Database_port.Size = New System.Drawing.Size(85, 20)
        Me.lbl_Database_port.TabIndex = 7
        Me.lbl_Database_port.Text = "DataBase:"
        '
        'txt_ServerName
        '
        Me.txt_ServerName.Location = New System.Drawing.Point(334, 282)
        Me.txt_ServerName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_ServerName.Name = "txt_ServerName"
        Me.txt_ServerName.Size = New System.Drawing.Size(28, 26)
        Me.txt_ServerName.TabIndex = 0
        Me.txt_ServerName.Visible = False
        '
        'Txt_Login
        '
        Me.Txt_Login.Location = New System.Drawing.Point(131, 132)
        Me.Txt_Login.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Txt_Login.Name = "Txt_Login"
        Me.Txt_Login.Size = New System.Drawing.Size(385, 26)
        Me.Txt_Login.TabIndex = 2
        Me.Txt_Login.Text = "sa"
        '
        'Txt_Password
        '
        Me.Txt_Password.Location = New System.Drawing.Point(131, 172)
        Me.Txt_Password.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Txt_Password.Name = "Txt_Password"
        Me.Txt_Password.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.Txt_Password.Size = New System.Drawing.Size(385, 26)
        Me.Txt_Password.TabIndex = 3
        Me.Txt_Password.Text = "123@pass"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(17, 182)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(82, 20)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Password:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(17, 142)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(52, 20)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Login:"
        '
        'lbl_Servername
        '
        Me.lbl_Servername.AutoSize = True
        Me.lbl_Servername.Location = New System.Drawing.Point(15, 98)
        Me.lbl_Servername.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbl_Servername.Name = "lbl_Servername"
        Me.lbl_Servername.Size = New System.Drawing.Size(105, 20)
        Me.lbl_Servername.TabIndex = 5
        Me.lbl_Servername.Text = "Server Name:"
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.BackColor = System.Drawing.Color.WhiteSmoke
        Me.GroupBox3.Controls.Add(Me.Btn_Remove)
        Me.GroupBox3.Controls.Add(Me.Btn_AddNameSpace)
        Me.GroupBox3.Controls.Add(Me.txt_NameSpace)
        Me.GroupBox3.Controls.Add(Me.Label3)
        Me.GroupBox3.Controls.Add(Me.ListBox_NameSpace)
        Me.GroupBox3.Location = New System.Drawing.Point(10, 11)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox3.Size = New System.Drawing.Size(734, 340)
        Me.GroupBox3.TabIndex = 0
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Import NameSpace"
        '
        'Btn_Remove
        '
        Me.Btn_Remove.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Btn_Remove.Enabled = False
        Me.Btn_Remove.Location = New System.Drawing.Point(8, 235)
        Me.Btn_Remove.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Btn_Remove.Name = "Btn_Remove"
        Me.Btn_Remove.Size = New System.Drawing.Size(87, 38)
        Me.Btn_Remove.TabIndex = 6
        Me.Btn_Remove.Text = "Remove"
        Me.Btn_Remove.UseVisualStyleBackColor = True
        '
        'Btn_AddNameSpace
        '
        Me.Btn_AddNameSpace.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Btn_AddNameSpace.Enabled = False
        Me.Btn_AddNameSpace.Location = New System.Drawing.Point(477, 54)
        Me.Btn_AddNameSpace.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Btn_AddNameSpace.Name = "Btn_AddNameSpace"
        Me.Btn_AddNameSpace.Size = New System.Drawing.Size(60, 38)
        Me.Btn_AddNameSpace.TabIndex = 6
        Me.Btn_AddNameSpace.Text = "Add"
        Me.Btn_AddNameSpace.UseVisualStyleBackColor = True
        '
        'txt_NameSpace
        '
        Me.txt_NameSpace.Location = New System.Drawing.Point(152, 59)
        Me.txt_NameSpace.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_NameSpace.Name = "txt_NameSpace"
        Me.txt_NameSpace.Size = New System.Drawing.Size(314, 26)
        Me.txt_NameSpace.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(8, 62)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(142, 20)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Library Name         :"
        '
        'ListBox_NameSpace
        '
        Me.ListBox_NameSpace.FormattingEnabled = True
        Me.ListBox_NameSpace.ItemHeight = 20
        Me.ListBox_NameSpace.Items.AddRange(New Object() {"Imports Microsoft", "Imports System.Data", "Imports System.Collections.Generic", "Imports BRAIN_DEVLOPMENT", "Imports BRAIN_DEVLOPMENT.DataAccessLayer"})
        Me.ListBox_NameSpace.Location = New System.Drawing.Point(9, 100)
        Me.ListBox_NameSpace.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ListBox_NameSpace.Name = "ListBox_NameSpace"
        Me.ListBox_NameSpace.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.ListBox_NameSpace.Size = New System.Drawing.Size(526, 124)
        Me.ListBox_NameSpace.TabIndex = 7
        '
        'TabBaseDeDonnees
        '
        Me.TabBaseDeDonnees.Controls.Add(Me.GroupBox2)
        Me.TabBaseDeDonnees.Controls.Add(Me.Btn_GenererScript)
        Me.TabBaseDeDonnees.Controls.Add(Me.btnOpenOutput)
        Me.TabBaseDeDonnees.Controls.Add(Me.Btn_GenererParIndex)
        Me.TabBaseDeDonnees.Location = New System.Drawing.Point(4, 29)
        Me.TabBaseDeDonnees.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabBaseDeDonnees.Name = "TabBaseDeDonnees"
        Me.TabBaseDeDonnees.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabBaseDeDonnees.Size = New System.Drawing.Size(1318, 979)
        Me.TabBaseDeDonnees.TabIndex = 0
        Me.TabBaseDeDonnees.Text = "Base de Données"
        Me.TabBaseDeDonnees.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.CB_ActionStoreProcedure)
        Me.GroupBox2.Controls.Add(Me.Label21)
        Me.GroupBox2.Controls.Add(Me.ddl_PrefixStoredProcedure)
        Me.GroupBox2.Controls.Add(Me.Label10)
        Me.GroupBox2.Controls.Add(Me.txt_projectName)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.txt_LibraryName)
        Me.GroupBox2.Controls.Add(Me.Label7)
        Me.GroupBox2.Controls.Add(Me.txt_table)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.txt_FkPrefix)
        Me.GroupBox2.Controls.Add(Me.lbl_FkPrefix)
        Me.GroupBox2.Controls.Add(Me.TreeView1)
        Me.GroupBox2.Controls.Add(Me.TreeView2)
        Me.GroupBox2.Location = New System.Drawing.Point(9, 9)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox2.Size = New System.Drawing.Size(1301, 582)
        Me.GroupBox2.TabIndex = 11
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Liste des tables de la base de données"
        '
        'CB_ActionStoreProcedure
        '
        Me.CB_ActionStoreProcedure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CB_ActionStoreProcedure.FormattingEnabled = True
        Me.CB_ActionStoreProcedure.Items.AddRange(New Object() {"CREATE ", "ALTER "})
        Me.CB_ActionStoreProcedure.Location = New System.Drawing.Point(152, 32)
        Me.CB_ActionStoreProcedure.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CB_ActionStoreProcedure.Name = "CB_ActionStoreProcedure"
        Me.CB_ActionStoreProcedure.Size = New System.Drawing.Size(368, 28)
        Me.CB_ActionStoreProcedure.TabIndex = 21
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(9, 39)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(133, 20)
        Me.Label21.TabIndex = 20
        Me.Label21.Text = "Store Procedure :"
        '
        'ddl_PrefixStoredProcedure
        '
        Me.ddl_PrefixStoredProcedure.FormattingEnabled = True
        Me.ddl_PrefixStoredProcedure.Location = New System.Drawing.Point(816, 29)
        Me.ddl_PrefixStoredProcedure.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ddl_PrefixStoredProcedure.Name = "ddl_PrefixStoredProcedure"
        Me.ddl_PrefixStoredProcedure.Size = New System.Drawing.Size(252, 28)
        Me.ddl_PrefixStoredProcedure.TabIndex = 19
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(745, 35)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(56, 20)
        Me.Label10.TabIndex = 18
        Me.Label10.Text = "Prefix :"
        '
        'txt_projectName
        '
        Me.txt_projectName.Location = New System.Drawing.Point(687, 529)
        Me.txt_projectName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_projectName.Name = "txt_projectName"
        Me.txt_projectName.Size = New System.Drawing.Size(314, 26)
        Me.txt_projectName.TabIndex = 16
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(531, 538)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(103, 20)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "Java Project :"
        '
        'txt_LibraryName
        '
        Me.txt_LibraryName.Location = New System.Drawing.Point(687, 492)
        Me.txt_LibraryName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_LibraryName.Name = "txt_LibraryName"
        Me.txt_LibraryName.Size = New System.Drawing.Size(314, 26)
        Me.txt_LibraryName.TabIndex = 14
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(526, 498)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(110, 20)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "Library Name :"
        '
        'txt_table
        '
        Me.txt_table.Location = New System.Drawing.Point(165, 532)
        Me.txt_table.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_table.Name = "txt_table"
        Me.txt_table.Size = New System.Drawing.Size(314, 26)
        Me.txt_table.TabIndex = 12
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 538)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(99, 20)
        Me.Label2.TabIndex = 11
        Me.Label2.Text = "Table Prefix :"
        '
        'txt_FkPrefix
        '
        Me.txt_FkPrefix.Location = New System.Drawing.Point(165, 492)
        Me.txt_FkPrefix.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_FkPrefix.Name = "txt_FkPrefix"
        Me.txt_FkPrefix.Size = New System.Drawing.Size(314, 26)
        Me.txt_FkPrefix.TabIndex = 10
        Me.txt_FkPrefix.Text = "ID_"
        '
        'lbl_FkPrefix
        '
        Me.lbl_FkPrefix.AutoSize = True
        Me.lbl_FkPrefix.Location = New System.Drawing.Point(9, 499)
        Me.lbl_FkPrefix.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbl_FkPrefix.Name = "lbl_FkPrefix"
        Me.lbl_FkPrefix.Size = New System.Drawing.Size(144, 20)
        Me.lbl_FkPrefix.TabIndex = 9
        Me.lbl_FkPrefix.Text = "Foreign Key Prefix :"
        '
        'TreeView1
        '
        Me.TreeView1.CheckBoxes = True
        Me.TreeView1.FullRowSelect = True
        Me.TreeView1.Location = New System.Drawing.Point(9, 71)
        Me.TreeView1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.Size = New System.Drawing.Size(511, 410)
        Me.TreeView1.TabIndex = 3
        '
        'TreeView2
        '
        Me.TreeView2.CheckBoxes = True
        Me.TreeView2.Location = New System.Drawing.Point(545, 71)
        Me.TreeView2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TreeView2.Name = "TreeView2"
        Me.TreeView2.Size = New System.Drawing.Size(555, 410)
        Me.TreeView2.TabIndex = 4
        '
        'Btn_GenererScript
        '
        Me.Btn_GenererScript.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Btn_GenererScript.Enabled = False
        Me.Btn_GenererScript.Location = New System.Drawing.Point(9, 601)
        Me.Btn_GenererScript.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Btn_GenererScript.Name = "Btn_GenererScript"
        Me.Btn_GenererScript.Size = New System.Drawing.Size(279, 54)
        Me.Btn_GenererScript.TabIndex = 0
        Me.Btn_GenererScript.Text = "Générer Tous les Scripts"
        Me.Btn_GenererScript.UseVisualStyleBackColor = True
        '
        'btnOpenOutput
        '
        Me.btnOpenOutput.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnOpenOutput.Location = New System.Drawing.Point(412, 601)
        Me.btnOpenOutput.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnOpenOutput.Name = "btnOpenOutput"
        Me.btnOpenOutput.Size = New System.Drawing.Size(117, 54)
        Me.btnOpenOutput.TabIndex = 17
        Me.btnOpenOutput.Text = "Ouvrir répertoire"
        Me.btnOpenOutput.UseVisualStyleBackColor = True
        '
        'Btn_GenererParIndex
        '
        Me.Btn_GenererParIndex.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Btn_GenererParIndex.Location = New System.Drawing.Point(818, 601)
        Me.Btn_GenererParIndex.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Btn_GenererParIndex.Name = "Btn_GenererParIndex"
        Me.Btn_GenererParIndex.Size = New System.Drawing.Size(291, 54)
        Me.Btn_GenererParIndex.TabIndex = 5
        Me.Btn_GenererParIndex.Text = "Générer Tous les Scripts / index"
        Me.Btn_GenererParIndex.UseVisualStyleBackColor = True
        '
        'TabInterfaceWEB
        '
        Me.TabInterfaceWEB.Controls.Add(Me.GroupBox14)
        Me.TabInterfaceWEB.Controls.Add(Me.GroupBox13)
        Me.TabInterfaceWEB.Controls.Add(Me.GroupBox11)
        Me.TabInterfaceWEB.Controls.Add(Me.GroupBox12)
        Me.TabInterfaceWEB.Location = New System.Drawing.Point(4, 29)
        Me.TabInterfaceWEB.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TabInterfaceWEB.Name = "TabInterfaceWEB"
        Me.TabInterfaceWEB.Size = New System.Drawing.Size(1318, 979)
        Me.TabInterfaceWEB.TabIndex = 5
        Me.TabInterfaceWEB.Text = "Interface WEB"
        Me.TabInterfaceWEB.UseVisualStyleBackColor = True
        '
        'GroupBox14
        '
        Me.GroupBox14.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox14.Controls.Add(Me.PictureBox_Formulaire)
        Me.GroupBox14.Location = New System.Drawing.Point(11, 361)
        Me.GroupBox14.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox14.Name = "GroupBox14"
        Me.GroupBox14.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox14.Size = New System.Drawing.Size(570, 372)
        Me.GroupBox14.TabIndex = 15
        Me.GroupBox14.TabStop = False
        Me.GroupBox14.Text = "Appercu Formulaire"
        '
        'PictureBox_Formulaire
        '
        Me.PictureBox_Formulaire.Image = Global.GENERIC_V16.My.Resources.Resources.CleanZone_Form_fw
        Me.PictureBox_Formulaire.Location = New System.Drawing.Point(10, 28)
        Me.PictureBox_Formulaire.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.PictureBox_Formulaire.Name = "PictureBox_Formulaire"
        Me.PictureBox_Formulaire.Size = New System.Drawing.Size(548, 335)
        Me.PictureBox_Formulaire.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox_Formulaire.TabIndex = 0
        Me.PictureBox_Formulaire.TabStop = False
        '
        'GroupBox13
        '
        Me.GroupBox13.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox13.Controls.Add(Me.PictureBox_Template)
        Me.GroupBox13.Location = New System.Drawing.Point(590, 18)
        Me.GroupBox13.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox13.Name = "GroupBox13"
        Me.GroupBox13.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox13.Size = New System.Drawing.Size(570, 372)
        Me.GroupBox13.TabIndex = 14
        Me.GroupBox13.TabStop = False
        Me.GroupBox13.Text = "Appercu Template"
        '
        'PictureBox_Template
        '
        Me.PictureBox_Template.Image = Global.GENERIC_V16.My.Resources.Resources.AdminLTE_Master_fw
        Me.PictureBox_Template.Location = New System.Drawing.Point(10, 28)
        Me.PictureBox_Template.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.PictureBox_Template.Name = "PictureBox_Template"
        Me.PictureBox_Template.Size = New System.Drawing.Size(548, 335)
        Me.PictureBox_Template.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox_Template.TabIndex = 0
        Me.PictureBox_Template.TabStop = False
        '
        'GroupBox11
        '
        Me.GroupBox11.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox11.Controls.Add(Me.RB_Template_Inspinia)
        Me.GroupBox11.Controls.Add(Me.RB_Template_CleanZone)
        Me.GroupBox11.Controls.Add(Me.RB_Template_AdminLTE_Master)
        Me.GroupBox11.Location = New System.Drawing.Point(174, 104)
        Me.GroupBox11.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox11.Name = "GroupBox11"
        Me.GroupBox11.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox11.Size = New System.Drawing.Size(248, 175)
        Me.GroupBox11.TabIndex = 2
        Me.GroupBox11.TabStop = False
        Me.GroupBox11.Text = "Interface WEB - Template"
        '
        'RB_Template_Inspinia
        '
        Me.RB_Template_Inspinia.AutoSize = True
        Me.RB_Template_Inspinia.Location = New System.Drawing.Point(9, 115)
        Me.RB_Template_Inspinia.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RB_Template_Inspinia.Name = "RB_Template_Inspinia"
        Me.RB_Template_Inspinia.Size = New System.Drawing.Size(89, 24)
        Me.RB_Template_Inspinia.TabIndex = 4
        Me.RB_Template_Inspinia.Text = "Inspinia"
        Me.RB_Template_Inspinia.UseVisualStyleBackColor = True
        '
        'RB_Template_CleanZone
        '
        Me.RB_Template_CleanZone.AutoSize = True
        Me.RB_Template_CleanZone.Checked = True
        Me.RB_Template_CleanZone.Location = New System.Drawing.Point(9, 79)
        Me.RB_Template_CleanZone.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RB_Template_CleanZone.Name = "RB_Template_CleanZone"
        Me.RB_Template_CleanZone.Size = New System.Drawing.Size(116, 24)
        Me.RB_Template_CleanZone.TabIndex = 3
        Me.RB_Template_CleanZone.TabStop = True
        Me.RB_Template_CleanZone.Text = "Clean Zone"
        Me.RB_Template_CleanZone.UseVisualStyleBackColor = True
        '
        'RB_Template_AdminLTE_Master
        '
        Me.RB_Template_AdminLTE_Master.AutoSize = True
        Me.RB_Template_AdminLTE_Master.Location = New System.Drawing.Point(9, 42)
        Me.RB_Template_AdminLTE_Master.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RB_Template_AdminLTE_Master.Name = "RB_Template_AdminLTE_Master"
        Me.RB_Template_AdminLTE_Master.Size = New System.Drawing.Size(162, 24)
        Me.RB_Template_AdminLTE_Master.TabIndex = 2
        Me.RB_Template_AdminLTE_Master.Text = "AdminLTE-master"
        Me.RB_Template_AdminLTE_Master.UseVisualStyleBackColor = True
        '
        'GroupBox12
        '
        Me.GroupBox12.Controls.Add(Me.RB_Formulaire_FlowLayout)
        Me.GroupBox12.Controls.Add(Me.RB_Formulaire_Tableau)
        Me.GroupBox12.Location = New System.Drawing.Point(744, 532)
        Me.GroupBox12.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox12.Name = "GroupBox12"
        Me.GroupBox12.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox12.Size = New System.Drawing.Size(248, 70)
        Me.GroupBox12.TabIndex = 13
        Me.GroupBox12.TabStop = False
        Me.GroupBox12.Text = "Type de formulaire"
        '
        'RB_Formulaire_FlowLayout
        '
        Me.RB_Formulaire_FlowLayout.AutoSize = True
        Me.RB_Formulaire_FlowLayout.Location = New System.Drawing.Point(123, 29)
        Me.RB_Formulaire_FlowLayout.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RB_Formulaire_FlowLayout.Name = "RB_Formulaire_FlowLayout"
        Me.RB_Formulaire_FlowLayout.Size = New System.Drawing.Size(115, 24)
        Me.RB_Formulaire_FlowLayout.TabIndex = 1
        Me.RB_Formulaire_FlowLayout.Text = "FlowLayout"
        Me.RB_Formulaire_FlowLayout.UseVisualStyleBackColor = True
        '
        'RB_Formulaire_Tableau
        '
        Me.RB_Formulaire_Tableau.AutoSize = True
        Me.RB_Formulaire_Tableau.Checked = True
        Me.RB_Formulaire_Tableau.Location = New System.Drawing.Point(9, 29)
        Me.RB_Formulaire_Tableau.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RB_Formulaire_Tableau.Name = "RB_Formulaire_Tableau"
        Me.RB_Formulaire_Tableau.Size = New System.Drawing.Size(91, 24)
        Me.RB_Formulaire_Tableau.TabIndex = 0
        Me.RB_Formulaire_Tableau.TabStop = True
        Me.RB_Formulaire_Tableau.Text = "Tableau"
        Me.RB_Formulaire_Tableau.UseVisualStyleBackColor = True
        '
        'TabGroupe
        '
        Me.TabGroupe.Controls.Add(Me.GroupBox1)
        Me.TabGroupe.Location = New System.Drawing.Point(4, 29)
        Me.TabGroupe.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabGroupe.Name = "TabGroupe"
        Me.TabGroupe.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TabGroupe.Size = New System.Drawing.Size(1318, 979)
        Me.TabGroupe.TabIndex = 2
        Me.TabGroupe.Text = "Groupe"
        Me.TabGroupe.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnGenerateScriptGroup)
        Me.GroupBox1.Controls.Add(Me.treeViewGroupe)
        Me.GroupBox1.Controls.Add(Me.btnRefresh)
        Me.GroupBox1.Controls.Add(Me.dtgGroupe)
        Me.GroupBox1.Controls.Add(Me.bnt_AddGroupTable)
        Me.GroupBox1.Location = New System.Drawing.Point(14, 11)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Size = New System.Drawing.Size(1094, 621)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Liste des tables de la base de données"
        '
        'btnGenerateScriptGroup
        '
        Me.btnGenerateScriptGroup.Location = New System.Drawing.Point(912, 555)
        Me.btnGenerateScriptGroup.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnGenerateScriptGroup.Name = "btnGenerateScriptGroup"
        Me.btnGenerateScriptGroup.Size = New System.Drawing.Size(143, 54)
        Me.btnGenerateScriptGroup.TabIndex = 9
        Me.btnGenerateScriptGroup.Text = "Generer"
        Me.btnGenerateScriptGroup.UseVisualStyleBackColor = True
        '
        'treeViewGroupe
        '
        Me.treeViewGroupe.CheckBoxes = True
        Me.treeViewGroupe.Location = New System.Drawing.Point(573, 99)
        Me.treeViewGroupe.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.treeViewGroupe.Name = "treeViewGroupe"
        Me.treeViewGroupe.Size = New System.Drawing.Size(509, 445)
        Me.treeViewGroupe.TabIndex = 8
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(198, 35)
        Me.btnRefresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(153, 54)
        Me.btnRefresh.TabIndex = 7
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'dtgGroupe
        '
        Me.dtgGroupe.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dtgGroupe.Location = New System.Drawing.Point(6, 99)
        Me.dtgGroupe.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.dtgGroupe.Name = "dtgGroupe"
        Me.dtgGroupe.Size = New System.Drawing.Size(558, 448)
        Me.dtgGroupe.TabIndex = 6
        '
        'bnt_AddGroupTable
        '
        Me.bnt_AddGroupTable.Cursor = System.Windows.Forms.Cursors.Hand
        Me.bnt_AddGroupTable.Location = New System.Drawing.Point(9, 35)
        Me.bnt_AddGroupTable.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.bnt_AddGroupTable.Name = "bnt_AddGroupTable"
        Me.bnt_AddGroupTable.Size = New System.Drawing.Size(180, 54)
        Me.bnt_AddGroupTable.TabIndex = 0
        Me.bnt_AddGroupTable.Text = "Ajouter un groupe"
        Me.bnt_AddGroupTable.UseVisualStyleBackColor = True
        '
        'tab_EcranProgression
        '
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue10)
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue9)
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue8)
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue7)
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue6)
        Me.tab_EcranProgression.Controls.Add(Me.Label20)
        Me.tab_EcranProgression.Controls.Add(Me.Label19)
        Me.tab_EcranProgression.Controls.Add(Me.Label18)
        Me.tab_EcranProgression.Controls.Add(Me.Label17)
        Me.tab_EcranProgression.Controls.Add(Me.Label16)
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue5)
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue4)
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue3)
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue2)
        Me.tab_EcranProgression.Controls.Add(Me.Label15)
        Me.tab_EcranProgression.Controls.Add(Me.Label14)
        Me.tab_EcranProgression.Controls.Add(Me.Label13)
        Me.tab_EcranProgression.Controls.Add(Me.Label12)
        Me.tab_EcranProgression.Controls.Add(Me.Label11)
        Me.tab_EcranProgression.Controls.Add(Me.progressTrue1)
        Me.tab_EcranProgression.Location = New System.Drawing.Point(4, 29)
        Me.tab_EcranProgression.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tab_EcranProgression.Name = "tab_EcranProgression"
        Me.tab_EcranProgression.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tab_EcranProgression.Size = New System.Drawing.Size(1318, 979)
        Me.tab_EcranProgression.TabIndex = 3
        Me.tab_EcranProgression.Text = "Progression"
        Me.tab_EcranProgression.UseVisualStyleBackColor = True
        '
        'progressTrue10
        '
        Me.progressTrue10.Location = New System.Drawing.Point(202, 629)
        Me.progressTrue10.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue10.Name = "progressTrue10"
        Me.progressTrue10.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue10.TabIndex = 19
        '
        'progressTrue9
        '
        Me.progressTrue9.Location = New System.Drawing.Point(202, 566)
        Me.progressTrue9.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue9.Name = "progressTrue9"
        Me.progressTrue9.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue9.TabIndex = 18
        '
        'progressTrue8
        '
        Me.progressTrue8.Location = New System.Drawing.Point(202, 499)
        Me.progressTrue8.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue8.Name = "progressTrue8"
        Me.progressTrue8.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue8.TabIndex = 17
        '
        'progressTrue7
        '
        Me.progressTrue7.Location = New System.Drawing.Point(202, 435)
        Me.progressTrue7.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue7.Name = "progressTrue7"
        Me.progressTrue7.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue7.TabIndex = 16
        '
        'progressTrue6
        '
        Me.progressTrue6.Location = New System.Drawing.Point(202, 366)
        Me.progressTrue6.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue6.Name = "progressTrue6"
        Me.progressTrue6.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue6.TabIndex = 15
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(40, 638)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(81, 20)
        Me.Label20.TabIndex = 14
        Me.Label20.Text = "Thread 10"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(40, 574)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(72, 20)
        Me.Label19.TabIndex = 13
        Me.Label19.Text = "Thread 9"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(40, 508)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(72, 20)
        Me.Label18.TabIndex = 12
        Me.Label18.Text = "Thread 8"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(40, 441)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(72, 20)
        Me.Label17.TabIndex = 11
        Me.Label17.Text = "Thread 7"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(40, 381)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(72, 20)
        Me.Label16.TabIndex = 10
        Me.Label16.Text = "Thread 6"
        '
        'progressTrue5
        '
        Me.progressTrue5.Location = New System.Drawing.Point(202, 301)
        Me.progressTrue5.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue5.Name = "progressTrue5"
        Me.progressTrue5.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue5.TabIndex = 9
        '
        'progressTrue4
        '
        Me.progressTrue4.Location = New System.Drawing.Point(202, 234)
        Me.progressTrue4.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue4.Name = "progressTrue4"
        Me.progressTrue4.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue4.TabIndex = 8
        '
        'progressTrue3
        '
        Me.progressTrue3.Location = New System.Drawing.Point(202, 165)
        Me.progressTrue3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue3.Name = "progressTrue3"
        Me.progressTrue3.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue3.TabIndex = 7
        '
        'progressTrue2
        '
        Me.progressTrue2.Location = New System.Drawing.Point(202, 101)
        Me.progressTrue2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue2.Name = "progressTrue2"
        Me.progressTrue2.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue2.TabIndex = 6
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(40, 318)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(72, 20)
        Me.Label15.TabIndex = 5
        Me.Label15.Text = "Thread 5"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(40, 249)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(72, 20)
        Me.Label14.TabIndex = 4
        Me.Label14.Text = "Thread 4"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(40, 180)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(72, 20)
        Me.Label13.TabIndex = 3
        Me.Label13.Text = "Thread 3"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(40, 118)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(72, 20)
        Me.Label12.TabIndex = 2
        Me.Label12.Text = "Thread 2"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(40, 51)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(72, 20)
        Me.Label11.TabIndex = 1
        Me.Label11.Text = "Thread 1"
        '
        'progressTrue1
        '
        Me.progressTrue1.Location = New System.Drawing.Point(202, 41)
        Me.progressTrue1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.progressTrue1.Name = "progressTrue1"
        Me.progressTrue1.Size = New System.Drawing.Size(809, 35)
        Me.progressTrue1.TabIndex = 0
        '
        'tabReportGeneration
        '
        Me.tabReportGeneration.Controls.Add(Me.txt_AliasMaster)
        Me.tabReportGeneration.Controls.Add(Me.pnlDetailReportGeneration)
        Me.tabReportGeneration.Controls.Add(Me.ddl_TypeReport)
        Me.tabReportGeneration.Controls.Add(Me.Label9)
        Me.tabReportGeneration.Controls.Add(Me.btnValidatePrincipalTable)
        Me.tabReportGeneration.Controls.Add(Me.ddl_Parent)
        Me.tabReportGeneration.Controls.Add(Me.Label8)
        Me.tabReportGeneration.Controls.Add(Me.txt_ReportProcName)
        Me.tabReportGeneration.Controls.Add(Me.lbl_ReportName)
        Me.tabReportGeneration.Location = New System.Drawing.Point(4, 29)
        Me.tabReportGeneration.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tabReportGeneration.Name = "tabReportGeneration"
        Me.tabReportGeneration.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tabReportGeneration.Size = New System.Drawing.Size(1318, 979)
        Me.tabReportGeneration.TabIndex = 4
        Me.tabReportGeneration.Text = "ReportGeneration"
        Me.tabReportGeneration.UseVisualStyleBackColor = True
        '
        'txt_AliasMaster
        '
        Me.txt_AliasMaster.Location = New System.Drawing.Point(449, 60)
        Me.txt_AliasMaster.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_AliasMaster.Name = "txt_AliasMaster"
        Me.txt_AliasMaster.Size = New System.Drawing.Size(108, 26)
        Me.txt_AliasMaster.TabIndex = 4
        '
        'pnlDetailReportGeneration
        '
        Me.pnlDetailReportGeneration.BackColor = System.Drawing.Color.Transparent
        Me.pnlDetailReportGeneration.Controls.Add(Me.pnlParametreDate)
        Me.pnlDetailReportGeneration.Controls.Add(Me.GroupBox10)
        Me.pnlDetailReportGeneration.Controls.Add(Me.btnGenerateReportScript)
        Me.pnlDetailReportGeneration.Controls.Add(Me.GroupBox9)
        Me.pnlDetailReportGeneration.Controls.Add(Me.GroupBox8)
        Me.pnlDetailReportGeneration.Controls.Add(Me.GroupBox7)
        Me.pnlDetailReportGeneration.Location = New System.Drawing.Point(6, 99)
        Me.pnlDetailReportGeneration.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnlDetailReportGeneration.Name = "pnlDetailReportGeneration"
        Me.pnlDetailReportGeneration.Size = New System.Drawing.Size(1151, 622)
        Me.pnlDetailReportGeneration.TabIndex = 16
        Me.pnlDetailReportGeneration.Visible = False
        '
        'pnlParametreDate
        '
        Me.pnlParametreDate.Controls.Add(Me.dtgDateParameter)
        Me.pnlParametreDate.Controls.Add(Me.btnDateParameterGroupe)
        Me.pnlParametreDate.Location = New System.Drawing.Point(621, 389)
        Me.pnlParametreDate.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnlParametreDate.Name = "pnlParametreDate"
        Me.pnlParametreDate.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnlParametreDate.Size = New System.Drawing.Size(518, 185)
        Me.pnlParametreDate.TabIndex = 20
        Me.pnlParametreDate.TabStop = False
        Me.pnlParametreDate.Text = "Parametres date"
        '
        'dtgDateParameter
        '
        Me.dtgDateParameter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dtgDateParameter.Location = New System.Drawing.Point(9, 62)
        Me.dtgDateParameter.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.dtgDateParameter.Name = "dtgDateParameter"
        Me.dtgDateParameter.Size = New System.Drawing.Size(500, 102)
        Me.dtgDateParameter.TabIndex = 9
        '
        'btnDateParameterGroupe
        '
        Me.btnDateParameterGroupe.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnDateParameterGroupe.Location = New System.Drawing.Point(12, 25)
        Me.btnDateParameterGroupe.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnDateParameterGroupe.Name = "btnDateParameterGroupe"
        Me.btnDateParameterGroupe.Size = New System.Drawing.Size(269, 34)
        Me.btnDateParameterGroupe.TabIndex = 8
        Me.btnDateParameterGroupe.Text = "Ajouter groupe de paramètre date"
        Me.btnDateParameterGroupe.UseVisualStyleBackColor = True
        '
        'GroupBox10
        '
        Me.GroupBox10.Controls.Add(Me.rdgWhereCondition)
        Me.GroupBox10.Controls.Add(Me.btnAddCondition)
        Me.GroupBox10.Location = New System.Drawing.Point(446, 14)
        Me.GroupBox10.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox10.Name = "GroupBox10"
        Me.GroupBox10.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox10.Size = New System.Drawing.Size(341, 374)
        Me.GroupBox10.TabIndex = 19
        Me.GroupBox10.TabStop = False
        Me.GroupBox10.Text = "Conditions where"
        '
        'rdgWhereCondition
        '
        Me.rdgWhereCondition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.rdgWhereCondition.Location = New System.Drawing.Point(10, 66)
        Me.rdgWhereCondition.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.rdgWhereCondition.Name = "rdgWhereCondition"
        Me.rdgWhereCondition.Size = New System.Drawing.Size(321, 295)
        Me.rdgWhereCondition.TabIndex = 7
        '
        'btnAddCondition
        '
        Me.btnAddCondition.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnAddCondition.Location = New System.Drawing.Point(9, 22)
        Me.btnAddCondition.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnAddCondition.Name = "btnAddCondition"
        Me.btnAddCondition.Size = New System.Drawing.Size(78, 34)
        Me.btnAddCondition.TabIndex = 1
        Me.btnAddCondition.Text = "Ajouter"
        Me.btnAddCondition.UseVisualStyleBackColor = True
        '
        'btnGenerateReportScript
        '
        Me.btnGenerateReportScript.Location = New System.Drawing.Point(944, 579)
        Me.btnGenerateReportScript.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnGenerateReportScript.Name = "btnGenerateReportScript"
        Me.btnGenerateReportScript.Size = New System.Drawing.Size(195, 34)
        Me.btnGenerateReportScript.TabIndex = 16
        Me.btnGenerateReportScript.Text = "Generer Script Rapport"
        Me.btnGenerateReportScript.UseVisualStyleBackColor = True
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.treeView_ReportColumn)
        Me.GroupBox9.Location = New System.Drawing.Point(795, 15)
        Me.GroupBox9.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox9.Size = New System.Drawing.Size(352, 375)
        Me.GroupBox9.TabIndex = 9
        Me.GroupBox9.TabStop = False
        Me.GroupBox9.Text = "Colonnes à retenir"
        '
        'treeView_ReportColumn
        '
        Me.treeView_ReportColumn.CheckBoxes = True
        Me.treeView_ReportColumn.Location = New System.Drawing.Point(9, 29)
        Me.treeView_ReportColumn.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.treeView_ReportColumn.Name = "treeView_ReportColumn"
        Me.treeView_ReportColumn.Size = New System.Drawing.Size(332, 335)
        Me.treeView_ReportColumn.TabIndex = 9
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.dtgReportParameters)
        Me.GroupBox8.Controls.Add(Me.btnAddReportParameter)
        Me.GroupBox8.Location = New System.Drawing.Point(19, 388)
        Me.GroupBox8.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox8.Size = New System.Drawing.Size(593, 185)
        Me.GroupBox8.TabIndex = 6
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Parametres"
        '
        'dtgReportParameters
        '
        Me.dtgReportParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dtgReportParameters.Location = New System.Drawing.Point(10, 61)
        Me.dtgReportParameters.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.dtgReportParameters.Name = "dtgReportParameters"
        Me.dtgReportParameters.Size = New System.Drawing.Size(570, 102)
        Me.dtgReportParameters.TabIndex = 9
        '
        'btnAddReportParameter
        '
        Me.btnAddReportParameter.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnAddReportParameter.Location = New System.Drawing.Point(12, 25)
        Me.btnAddReportParameter.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnAddReportParameter.Name = "btnAddReportParameter"
        Me.btnAddReportParameter.Size = New System.Drawing.Size(180, 34)
        Me.btnAddReportParameter.TabIndex = 8
        Me.btnAddReportParameter.Text = "Ajouter un paramètre"
        Me.btnAddReportParameter.UseVisualStyleBackColor = True
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.txtAliasReportTable)
        Me.GroupBox7.Controls.Add(Me.ddl_OtherReportTable)
        Me.GroupBox7.Controls.Add(Me.dtgOtherReportTable)
        Me.GroupBox7.Controls.Add(Me.btnAddNewTable)
        Me.GroupBox7.Location = New System.Drawing.Point(19, 12)
        Me.GroupBox7.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox7.Size = New System.Drawing.Size(429, 374)
        Me.GroupBox7.TabIndex = 8
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Tables additionnelles"
        '
        'txtAliasReportTable
        '
        Me.txtAliasReportTable.Location = New System.Drawing.Point(219, 21)
        Me.txtAliasReportTable.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtAliasReportTable.Name = "txtAliasReportTable"
        Me.txtAliasReportTable.Size = New System.Drawing.Size(108, 26)
        Me.txtAliasReportTable.TabIndex = 7
        '
        'ddl_OtherReportTable
        '
        Me.ddl_OtherReportTable.FormattingEnabled = True
        Me.ddl_OtherReportTable.Location = New System.Drawing.Point(12, 21)
        Me.ddl_OtherReportTable.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ddl_OtherReportTable.Name = "ddl_OtherReportTable"
        Me.ddl_OtherReportTable.Size = New System.Drawing.Size(196, 28)
        Me.ddl_OtherReportTable.TabIndex = 6
        '
        'dtgOtherReportTable
        '
        Me.dtgOtherReportTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dtgOtherReportTable.Location = New System.Drawing.Point(10, 66)
        Me.dtgOtherReportTable.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.dtgOtherReportTable.Name = "dtgOtherReportTable"
        Me.dtgOtherReportTable.Size = New System.Drawing.Size(402, 295)
        Me.dtgOtherReportTable.TabIndex = 7
        '
        'btnAddNewTable
        '
        Me.btnAddNewTable.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnAddNewTable.Location = New System.Drawing.Point(336, 19)
        Me.btnAddNewTable.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnAddNewTable.Name = "btnAddNewTable"
        Me.btnAddNewTable.Size = New System.Drawing.Size(78, 34)
        Me.btnAddNewTable.TabIndex = 8
        Me.btnAddNewTable.Text = "Ajouter"
        Me.btnAddNewTable.UseVisualStyleBackColor = True
        '
        'ddl_TypeReport
        '
        Me.ddl_TypeReport.FormattingEnabled = True
        Me.ddl_TypeReport.Location = New System.Drawing.Point(766, 14)
        Me.ddl_TypeReport.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ddl_TypeReport.Name = "ddl_TypeReport"
        Me.ddl_TypeReport.Size = New System.Drawing.Size(302, 28)
        Me.ddl_TypeReport.TabIndex = 2
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(651, 20)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(104, 20)
        Me.Label9.TabIndex = 17
        Me.Label9.Text = "Type Report :"
        '
        'btnValidatePrincipalTable
        '
        Me.btnValidatePrincipalTable.Location = New System.Drawing.Point(588, 60)
        Me.btnValidatePrincipalTable.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnValidatePrincipalTable.Name = "btnValidatePrincipalTable"
        Me.btnValidatePrincipalTable.Size = New System.Drawing.Size(112, 35)
        Me.btnValidatePrincipalTable.TabIndex = 5
        Me.btnValidatePrincipalTable.Text = "Valider"
        Me.btnValidatePrincipalTable.UseVisualStyleBackColor = True
        '
        'ddl_Parent
        '
        Me.ddl_Parent.FormattingEnabled = True
        Me.ddl_Parent.Location = New System.Drawing.Point(184, 60)
        Me.ddl_Parent.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ddl_Parent.Name = "ddl_Parent"
        Me.ddl_Parent.Size = New System.Drawing.Size(252, 28)
        Me.ddl_Parent.TabIndex = 3
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(32, 62)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(127, 20)
        Me.Label8.TabIndex = 2
        Me.Label8.Text = "Table principale :"
        '
        'txt_ReportProcName
        '
        Me.txt_ReportProcName.Location = New System.Drawing.Point(184, 21)
        Me.txt_ReportProcName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txt_ReportProcName.Name = "txt_ReportProcName"
        Me.txt_ReportProcName.Size = New System.Drawing.Size(373, 26)
        Me.txt_ReportProcName.TabIndex = 1
        '
        'lbl_ReportName
        '
        Me.lbl_ReportName.AutoSize = True
        Me.lbl_ReportName.Location = New System.Drawing.Point(32, 26)
        Me.lbl_ReportName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbl_ReportName.Name = "lbl_ReportName"
        Me.lbl_ReportName.Size = New System.Drawing.Size(126, 20)
        Me.lbl_ReportName.TabIndex = 0
        Me.lbl_ReportName.Text = "Nom procédure :"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FichierToolStripMenuItem, Me.AideToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(9, 2, 0, 2)
        Me.MenuStrip1.Size = New System.Drawing.Size(1326, 33)
        Me.MenuStrip1.TabIndex = 17
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FichierToolStripMenuItem
        '
        Me.FichierToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NouveauProjetToolStripMenuItem, Me.OuvrirProjetToolStripMenuItem})
        Me.FichierToolStripMenuItem.Name = "FichierToolStripMenuItem"
        Me.FichierToolStripMenuItem.Size = New System.Drawing.Size(74, 29)
        Me.FichierToolStripMenuItem.Text = "Fichier"
        '
        'NouveauProjetToolStripMenuItem
        '
        Me.NouveauProjetToolStripMenuItem.Name = "NouveauProjetToolStripMenuItem"
        Me.NouveauProjetToolStripMenuItem.Size = New System.Drawing.Size(219, 30)
        Me.NouveauProjetToolStripMenuItem.Text = "Nouveau projet"
        '
        'OuvrirProjetToolStripMenuItem
        '
        Me.OuvrirProjetToolStripMenuItem.Name = "OuvrirProjetToolStripMenuItem"
        Me.OuvrirProjetToolStripMenuItem.Size = New System.Drawing.Size(219, 30)
        Me.OuvrirProjetToolStripMenuItem.Text = "Ouvrir projet"
        '
        'AideToolStripMenuItem
        '
        Me.AideToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AProposToolStripMenuItem})
        Me.AideToolStripMenuItem.Name = "AideToolStripMenuItem"
        Me.AideToolStripMenuItem.Size = New System.Drawing.Size(60, 29)
        Me.AideToolStripMenuItem.Text = "Aide"
        '
        'AProposToolStripMenuItem
        '
        Me.AProposToolStripMenuItem.Name = "AProposToolStripMenuItem"
        Me.AProposToolStripMenuItem.Size = New System.Drawing.Size(170, 30)
        Me.AProposToolStripMenuItem.Text = "A Propos"
        '
        'FormGeneric
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1326, 1046)
        Me.Controls.Add(Me.TabControl_Form)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormGeneric"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "GENERIC V16"
        Me.TabControl_Form.ResumeLayout(False)
        Me.TabParametre.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GrpBox_DB.ResumeLayout(False)
        Me.GrpBox_DB.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.TabBaseDeDonnees.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.TabInterfaceWEB.ResumeLayout(False)
        Me.GroupBox14.ResumeLayout(False)
        CType(Me.PictureBox_Formulaire, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox13.ResumeLayout(False)
        CType(Me.PictureBox_Template, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox11.ResumeLayout(False)
        Me.GroupBox11.PerformLayout()
        Me.GroupBox12.ResumeLayout(False)
        Me.GroupBox12.PerformLayout()
        Me.TabGroupe.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.dtgGroupe, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tab_EcranProgression.ResumeLayout(False)
        Me.tab_EcranProgression.PerformLayout()
        Me.tabReportGeneration.ResumeLayout(False)
        Me.tabReportGeneration.PerformLayout()
        Me.pnlDetailReportGeneration.ResumeLayout(False)
        Me.pnlParametreDate.ResumeLayout(False)
        CType(Me.dtgDateParameter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox10.ResumeLayout(False)
        CType(Me.rdgWhereCondition, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox9.ResumeLayout(False)
        Me.GroupBox8.ResumeLayout(False)
        CType(Me.dtgReportParameters, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        CType(Me.dtgOtherReportTable, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TabControl_Form As TabControl
    Friend WithEvents TabParametre As TabPage
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents txt_PathGenerate_ScriptFile As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Btn_FolderBrowserDialog_Script As Button
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents cmb_ServerName As ComboBox
    Friend WithEvents rcmb_DatabaseName As ComboBox
    Friend WithEvents GrpBox_DB As GroupBox
    Friend WithEvents rbtn_PostGres As RadioButton
    Friend WithEvents rbtn_MySql As RadioButton
    Friend WithEvents rbtn_SqlServer As RadioButton
    Friend WithEvents rbtn_Oracle As RadioButton
    Friend WithEvents Btn_ConnexionServerName As Button
    Friend WithEvents txt_DatabaseName As TextBox
    Friend WithEvents lbl_Database_port As Label
    Friend WithEvents txt_ServerName As TextBox
    Friend WithEvents Txt_Login As TextBox
    Friend WithEvents Txt_Password As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents lbl_Servername As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Btn_Remove As Button
    Friend WithEvents Btn_AddNameSpace As Button
    Friend WithEvents txt_NameSpace As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents ListBox_NameSpace As ListBox
    Friend WithEvents TabBaseDeDonnees As TabPage
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents CB_ActionStoreProcedure As ComboBox
    Friend WithEvents Label21 As Label
    Friend WithEvents ddl_PrefixStoredProcedure As ComboBox
    Friend WithEvents Label10 As Label
    Friend WithEvents btnOpenOutput As Button
    Friend WithEvents txt_projectName As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents txt_LibraryName As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txt_table As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txt_FkPrefix As TextBox
    Friend WithEvents lbl_FkPrefix As Label
    Friend WithEvents Btn_GenererParIndex As Button
    Friend WithEvents TreeView1 As TreeView
    Friend WithEvents TreeView2 As TreeView
    Friend WithEvents Btn_GenererScript As Button
    Friend WithEvents TabInterfaceWEB As TabPage
    Friend WithEvents GroupBox14 As GroupBox
    Friend WithEvents PictureBox_Formulaire As PictureBox
    Friend WithEvents GroupBox13 As GroupBox
    Friend WithEvents PictureBox_Template As PictureBox
    Friend WithEvents GroupBox11 As GroupBox
    Friend WithEvents RB_Template_Inspinia As RadioButton
    Friend WithEvents RB_Template_CleanZone As RadioButton
    Friend WithEvents RB_Template_AdminLTE_Master As RadioButton
    Friend WithEvents GroupBox12 As GroupBox
    Friend WithEvents RB_Formulaire_FlowLayout As RadioButton
    Friend WithEvents RB_Formulaire_Tableau As RadioButton
    Friend WithEvents TabGroupe As TabPage
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnGenerateScriptGroup As Button
    Friend WithEvents treeViewGroupe As TreeView
    Friend WithEvents btnRefresh As Button
    Friend WithEvents dtgGroupe As DataGridView
    Friend WithEvents bnt_AddGroupTable As Button
    Friend WithEvents tab_EcranProgression As TabPage
    Friend WithEvents progressTrue10 As ProgressBar
    Friend WithEvents progressTrue9 As ProgressBar
    Friend WithEvents progressTrue8 As ProgressBar
    Friend WithEvents progressTrue7 As ProgressBar
    Friend WithEvents progressTrue6 As ProgressBar
    Friend WithEvents Label20 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents progressTrue5 As ProgressBar
    Friend WithEvents progressTrue4 As ProgressBar
    Friend WithEvents progressTrue3 As ProgressBar
    Friend WithEvents progressTrue2 As ProgressBar
    Friend WithEvents Label15 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents progressTrue1 As ProgressBar
    Friend WithEvents tabReportGeneration As TabPage
    Friend WithEvents txt_AliasMaster As TextBox
    Friend WithEvents pnlDetailReportGeneration As Panel
    Friend WithEvents pnlParametreDate As GroupBox
    Friend WithEvents dtgDateParameter As DataGridView
    Friend WithEvents btnDateParameterGroupe As Button
    Friend WithEvents GroupBox10 As GroupBox
    Friend WithEvents rdgWhereCondition As DataGridView
    Friend WithEvents btnAddCondition As Button
    Friend WithEvents btnGenerateReportScript As Button
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents treeView_ReportColumn As TreeView
    Friend WithEvents GroupBox8 As GroupBox
    Friend WithEvents dtgReportParameters As DataGridView
    Friend WithEvents btnAddReportParameter As Button
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents txtAliasReportTable As TextBox
    Friend WithEvents ddl_OtherReportTable As ComboBox
    Friend WithEvents dtgOtherReportTable As DataGridView
    Friend WithEvents btnAddNewTable As Button
    Friend WithEvents ddl_TypeReport As ComboBox
    Friend WithEvents Label9 As Label
    Friend WithEvents btnValidatePrincipalTable As Button
    Friend WithEvents ddl_Parent As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txt_ReportProcName As TextBox
    Friend WithEvents lbl_ReportName As Label
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FichierToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NouveauProjetToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OuvrirProjetToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AideToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AProposToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
End Class
