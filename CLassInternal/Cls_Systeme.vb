Imports System.Data.SqlServerCe
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.ComponentModel

Public Class Cls_Systeme

    Shared instance As Cls_Systeme
    Shared ConnectionString As String = String.Empty
    Shared compteurThread As Integer
    Shared schemaTable As DataTable
    Private Shared _foreignkey As New Cls_ForeignKey
#Region "New"
    Private Sub New()
        MyBase.New()
    End Sub
    Shared Function getInstance() As Cls_Systeme
        SyncLock GetType(Cls_Systeme)
            If IsNothing(instance) Then
                instance = New Cls_Systeme
            End If
        End SyncLock
        Return instance
    End Function


#End Region

#Region "Apparent Properties"

    Public Function SpecialChar() As List(Of String)
        Dim values As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary", "decimal", "numeric"}
        Return values
    End Function

    Public Function LevelOneSpecialChar() As List(Of String)
        Dim values As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary"}
        Return values
    End Function

    Public Function LevelTwoSpecialChar() As List(Of String)
        Dim values As New List(Of String) From {"decimal", "numeric"}
        Return values
    End Function

    Public Function currentDatabase() As Cls_Database
        Return Cls_Database.GetLastDatabase
    End Function

    Public Function getLastConnectionString() As Cls_Connection
        Return Cls_Connection.GetLastConnection
    End Function

    Public Function getConnectionString() As String
        Dim testpath As String = String.Empty
        '  testpath = Environment.CurrentDirectory
        testpath = Application.StartupPath
        testpath = testpath.Substring(0, testpath.LastIndexOf("\"))
        testpath = testpath.Substring(0, testpath.LastIndexOf("\") + 1) & "App_Data\Generic_db.sdf"
        ConnectionString = "Data Source=" & testpath
        Return ConnectionString
    End Function

    Public Function currentSchema() As DataTable

    End Function
#End Region

#Region "Create"
    Public Sub CreateLocalDatabase(ByVal databaseName As String, Optional ByVal idTypeDatabase As Long = 1)
        Dim obj As New Cls_Database
        With obj
            .Name = databaseName
            .Id_TypeDatabase = idTypeDatabase
            .Insert()
        End With
    End Sub

    Public Function CreateOnlyTable(ByVal schemaTable As DataTable, ByVal iddatabase As Long) As Boolean
        For Each row As DataRow In schemaTable.Rows
            If currentDatabase.Id_TypeDatabase = TypeDatabase.SQLSERVER Then
                Dim obj As New Cls_Table
                With obj
                    .Name = row("TABLE_NAME")
                    .Id_DataBase = iddatabase
                    .Insert()
                End With
            End If
        Next
        Return True
    End Function

    Public Sub CreateLocalTablePart(ByVal schemaTable As DataTable, ByVal iddatabase As Long, ByRef background As BackgroundWorker)
        Dim countTable As Long = schemaTable.Rows.Count
        'Dim firstDebut As Long = Math.Round(countTable / 2) + 1
        'Dim Fin As Long = firstDebut + 1
        'Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        'Dim firstdebut_2 As Long = Math.Round(firstDebut / 2) + 1
        'Dim fin_firstdebut2 As Long = firstdebut_2 + 1

        Dim compteur As Integer = 0
        For Each row As DataRow In schemaTable.Rows

            If currentDatabase.Id_TypeDatabase = TypeDatabase.SQLSERVER Then
                Dim obj As New Cls_Table(currentDatabase.ID, row("TABLE_NAME"))
                If obj.ID > 0 Then
                    With obj

                    End With
                Else
                    With obj
                        .Name = row("TABLE_NAME")
                        .Id_DataBase = iddatabase
                        .Insert()
                    End With
                End If

                CreateLocalColumn(obj.ID, obj.Name)
                'ElseIf currentDatabase.Id_TypeDatabase = TypeDatabase.MYSQL Then
                '    Dim obj As New Cls_Table(currentDatabase.ID, row.ItemArray(0).ToString)
                '    If obj.ID > 0 Then


                '    Else
                '        With obj
                '            .Name = row.ItemArray(0).ToString
                '            .Id_DataBase = iddatabase
                '            .Insert()
                '        End With
                '    End If
                '    CreateLocalColumMySql(obj.ID, obj.Name, TypeDatabase.MYSQL)
                'ElseIf currentDatabase.Id_TypeDatabase = TypeDatabase.POSTGRESSQL Then

                '    Dim obj As New Cls_Table(currentDatabase.ID, row.ItemArray(0).ToString)
                '    If obj.ID > 0 Then


                '    Else
                '        With obj
                '            .Name = row.ItemArray(0).ToString
                '            .Id_DataBase = iddatabase
                '            .Insert()
                '        End With
                '    End If
                '    CreateLocalColumnPostGres(obj.ID, obj.Name, TypeDatabase.POSTGRESSQL)
            End If

            compteur = compteur + 1
            background.ReportProgress(compteur * (100 / countTable))
        Next


    End Sub

    Private Shared Sub CreateLocalColumn(ByVal idtable As Long, ByVal table_name As String)
        '  Dim _table As New Cls_Table(idtable)
        Dim _tableStructure As DataSet = SqlServerHelper.LoadTableStructure(table_name)

        For Each dt As DataRow In _tableStructure.Tables(1).Rows
            Dim _column As New Cls_Column
            With _column
                .Name = dt(0).ToString
                .Id_Table = idtable
                Dim _type As New Cls_Type()
                _type.ReadSqlServerName(dt(1).ToString)
                .Id_Type = _type.ID
                .Length = (dt(3) / 2).ToString
                .Precision = (dt(4).ToString)
                .Scale = (dt(5).ToString)
                .Insert()
            End With

        Next

        If _tableStructure.Tables.Count >= 7 Then

            For Each dt As DataRow In _tableStructure.Tables(6).Rows
                Dim valueSub As String = "REFERENCES " & Cls_Database.GetLastDatabase.Name & ".dbo."
                Dim valueTopsplit As String = ""
                Dim splitarray As String()
                Dim tableName As String = ""
                Dim splitarray2 As String()
                Dim splitarray3 As String()
                Dim idref As String

                If dt(0).ToString = "FOREIGN KEY" Then
                    _foreignkey = New Cls_ForeignKey(idtable, Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(6).ToString))
                    If _foreignkey.ID > 0 Then

                    Else
                        With _foreignkey
                            .Id_Column = Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(6).ToString)
                            .Id_Table = idtable
                            .Id_RefTable = 0
                            .Insert()
                        End With
                    End If

                End If
                If dt(0).ToString.Trim() = "" Then
                    valueTopsplit = dt(6).ToString.Substring(valueSub.Length, dt(6).ToString.Length - valueSub.Length)
                    splitarray = valueTopsplit.Split("(")
                    splitarray2 = splitarray(1).Split(")")


                    idref = splitarray2(0).ToString.Trim
                    tableName = splitarray(0).ToString.Trim
                    Dim table As New Cls_Table(Cls_Database.GetLastDatabase.ID, tableName)
                    '_foreignkey = New Cls_ForeignKey(idtable, Cls_Column.GetId_ColumnByTableAndColumnName(idtable, idref))
                    If table.ID > 0 Then
                        _foreignkey.Id_RefTable = table.ID
                        _foreignkey.Update()
                    Else
                        With table
                            .Name = tableName
                            .Id_DataBase = Cls_Database.GetLastDatabase.ID
                            .Insert()
                        End With
                        _foreignkey.Id_RefTable = table.ID
                        _foreignkey.Update()


                    End If

                End If
                If dt(0).ToString.Contains("PRIMARY KEY") Then
                    Dim _primarykey As New Cls_Column(Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(6).ToString))
                    Dim _table As New Cls_Table(idtable)
                    With _table
                        .Id_PrimaryKey = _primarykey.ID
                        .Update()
                    End With
                End If
            Next
        End If

        If _tableStructure.Tables.Count >= 6 Then
            For Each dt As DataRow In _tableStructure.Tables(5).Rows

                If dt(1).ToString.Contains("unique key") Then

                    If dt(2).ToString.Contains(",") Then
                        Dim _uniquekey As New Cls_UniqueIndex
                        With _uniquekey
                            .Id_Table = idtable
                            .Insert()
                        End With
                        Dim strArr As String() = dt(2).ToString.Split(",")
                        For ind As Integer = 0 To strArr.Length - 1
                            Dim _column As New Cls_Column(Cls_Column.GetId_ColumnByTableAndColumnName(idtable, strArr(ind).Trim))
                            With _column
                                .Id_IndexParent = _uniquekey.ID
                                .Update()
                            End With
                        Next
                    Else
                        Dim _uniquekey As New Cls_UniqueIndex
                        With _uniquekey
                            .Id_Table = idtable
                            .Insert()
                        End With
                        Dim _column As New Cls_Column(Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(2).ToString))
                        With _column
                            .Id_IndexParent = _uniquekey.ID
                            .Update()
                        End With
                    End If
                End If
            Next

        End If

    End Sub

    ''Private Shared Sub CreateLocalColumMySql(ByVal idtable As Long, ByVal table_name As String, ByVal databasetype As Long)
    ''    Dim _tableStructure As DataSet = MySqlManager.LoadTableStructure(table_name)

    ''    For Each dt As DataRow In _tableStructure.Tables(0).Rows
    ''        Dim _column As New Cls_Column
    ''        With _column
    ''            .Name = dt(0).ToString
    ''            .Id_Table = idtable
    ''            Dim _type As New Cls_Type()
    ''            _type.ReadSqlServerName(dt(1).ToString)
    ''            .Id_Type = _type.ID
    ''            .Length = (dt(2)).ToString
    ''            .Precision = (dt(3).ToString)
    ''            .Scale = (dt(4).ToString)
    ''            .Insert()
    ''        End With
    ''    Next


    ''    For Each dt As DataRow In _tableStructure.Tables(0).Rows
    ''        If dt(6).ToString.Contains("FOREIGN KEY") Then
    ''            _foreignkey = New Cls_ForeignKey(idtable, Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(6).ToString))
    ''            If _foreignkey.ID > 0 Then

    ''            Else
    ''                Dim table As New Cls_Table(Cls_Database.GetLastDatabase.ID, dt(8).ToString)
    ''                If table.ID > 0 Then
    ''                    With _foreignkey
    ''                        .Id_Column = Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(0).ToString)
    ''                        .Id_Table = idtable
    ''                        .Id_RefTable = table.ID
    ''                        .Insert()
    ''                    End With
    ''                Else
    ''                    With table
    ''                        .Name = dt(8).ToString
    ''                        .Id_DataBase = Cls_Database.GetLastDatabase.ID
    ''                        .Insert()
    ''                    End With
    ''                    With _foreignkey
    ''                        .Id_Column = Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(0).ToString)
    ''                        .Id_Table = idtable
    ''                        .Id_RefTable = table.ID
    ''                        .Insert()
    ''                    End With
    ''                End If
    ''            End If

    ''        End If
    ''        If dt(6).ToString.Contains("PRIMARY KEY") Then
    ''            Dim _primarykey As New Cls_Column(Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(0).ToString))
    ''            Dim _table As New Cls_Table(idtable)
    ''            With _table
    ''                .Id_PrimaryKey = _primarykey.ID
    ''                .Update()
    ''            End With
    ''        End If
    ''    Next

    ''End Sub

    ''Private Shared Sub CreateLocalColumnPostGres(ByVal idtable As Long, ByVal table_name As String, ByVal databasetype As Long)
    ''    Dim _tableStructure As DataSet = PostgresSqlManager.LoadTableStructure(table_name)

    ''    For Each dt As DataRow In _tableStructure.Tables(0).Rows
    ''        Dim _column As New Cls_Column
    ''        With _column
    ''            .Name = dt(0).ToString
    ''            .Id_Table = idtable
    ''            Dim _type As New Cls_Type()
    ''            _type.ReadPostGresName(dt(1).ToString)
    ''            .Id_Type = _type.ID
    ''            .Length = (dt(2)).ToString
    ''            .Precision = (dt(3).ToString)
    ''            .Scale = (dt(4).ToString)
    ''            .Insert()
    ''        End With

    ''    Next


    ''    For Each dt As DataRow In _tableStructure.Tables(0).Rows
    ''        If dt(6).ToString.Contains("FOREIGN KEY") Then
    ''            _foreignkey = New Cls_ForeignKey(idtable, Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(6).ToString))
    ''            If _foreignkey.ID > 0 Then

    ''            Else
    ''                Dim table As New Cls_Table(Cls_Database.GetLastDatabase.ID, dt(8).ToString)
    ''                If table.ID > 0 Then
    ''                    With _foreignkey
    ''                        .Id_Column = Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(0).ToString)
    ''                        .Id_Table = idtable
    ''                        .Id_RefTable = table.ID
    ''                        .Insert()
    ''                    End With
    ''                Else
    ''                    With table
    ''                        .Name = dt(8).ToString
    ''                        .Id_DataBase = Cls_Database.GetLastDatabase.ID
    ''                        .Insert()
    ''                    End With
    ''                    With _foreignkey
    ''                        .Id_Column = Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(0).ToString)
    ''                        .Id_Table = idtable
    ''                        .Id_RefTable = table.ID
    ''                        .Insert()
    ''                    End With
    ''                End If
    ''            End If

    ''        End If
    ''        If dt(6).ToString.Contains("PRIMARY KEY") Then
    ''            Dim _primarykey As New Cls_Column(Cls_Column.GetId_ColumnByTableAndColumnName(idtable, dt(0).ToString))
    ''            Dim _table As New Cls_Table(idtable)
    ''            With _table
    ''                .Id_PrimaryKey = _primarykey.ID
    ''                .Update()
    ''            End With
    ''        End If
    ''    Next

    ''End Sub

    Public Sub CreateConnectionLog(ByVal strServer As String, _
                ByVal strUser As String, _
                ByVal strPwd As String, ByVal typedatabase As Long)
        Dim _connection As New Cls_Connection
        With _connection
            .ServerName = strServer
            .Login = strUser
            .Password = strPwd
            .Date = Now
            .Id_TypeDatabase = typedatabase
            .Insert()
        End With
    End Sub

    Public Sub CreateLocalTablePartAnyThread(ByVal schemaTable As DataTable, ByVal iddatabase As Long, ByRef background As BackgroundWorker, ByVal numerothread As Long)
        Dim count As Long = schemaTable.Rows.Count
        Dim quotient As Long = Math.Ceiling(count / 5)
        Dim reste As Long = count Mod 5
        Dim valeurveri As Long = reste
        Dim compteur As Integer = 0



        Dim debut As Integer = 0
        Dim fin As Integer = 0


        If valeurveri < numerothread Then
            fin = quotient * numerothread - 1 * (numerothread - reste + 1)
            debut = (numerothread - 1) * quotient
        ElseIf valeurveri = numerothread Then
            fin = quotient * numerothread - 1 * (numerothread - reste + 1)
            debut = (numerothread - 1) * quotient + 1
        Else
            fin = quotient * numerothread
            debut = (numerothread - 1) * quotient + 1
        End If

        Dim quantite As Integer = fin - debut + 1

        For i = debut To fin
            Dim row As DataRow = schemaTable.Rows(i - 1)
            If currentDatabase.Id_TypeDatabase = TypeDatabase.SQLSERVER Then
                Dim obj As New Cls_Table(currentDatabase.ID, row("TABLE_NAME"))
                If obj.ID > 0 Then
                    With obj

                    End With
                Else
                    With obj
                        .Name = row("TABLE_NAME")
                        .Id_DataBase = iddatabase
                        .Insert()
                    End With
                End If


                CreateLocalColumn(obj.ID, obj.Name)
                ''ElseIf currentDatabase.Id_TypeDatabase = TypeDatabase.MYSQL Then
                ''    Dim obj As New Cls_Table(currentDatabase.ID, row.ItemArray(0).ToString)
                ''    If obj.ID > 0 Then


                ''    Else
                ''        With obj
                ''            .Name = row.ItemArray(0).ToString
                ''            .Id_DataBase = iddatabase
                ''            .Insert()
                ''        End With
                ''    End If
                ''    CreateLocalColumMySql(obj.ID, obj.Name, TypeDatabase.MYSQL)
                ''ElseIf currentDatabase.Id_TypeDatabase = TypeDatabase.POSTGRESSQL Then

                ''    Dim obj As New Cls_Table(currentDatabase.ID, row.ItemArray(0).ToString)
                ''    If obj.ID > 0 Then


                ''    Else
                ''        With obj
                ''            .Name = row.ItemArray(0).ToString
                ''            .Id_DataBase = iddatabase
                ''            .Insert()
                ''        End With
                ''    End If
                ''    CreateLocalColumnPostGres(obj.ID, obj.Name, TypeDatabase.POSTGRESSQL)
            End If

            compteur = compteur + 1
            background.ReportProgress(compteur * (100 / quantite))
        Next

    End Sub


    Public Sub CreateLocalTablePartSuperAnyThread(ByVal schemaTable As DataTable, ByVal iddatabase As Long, ByRef background As BackgroundWorker, ByVal numerothread As Long)
        Dim count As Long = schemaTable.Rows.Count
        Dim quotient As Long = Math.Ceiling(count / 10)
        Dim reste As Long = quotient Mod 10
        Dim valeurveri As Long = reste / 1
        Dim compteur As Integer = 0



        Dim debut As Integer = 0
        Dim fin As Integer = 0

        debut = (numerothread - 1) * quotient + 1
        If valeurveri > numerothread Or valeurveri = numerothread Then
            fin = quotient * numerothread - 1
        Else
            fin = quotient * numerothread
        End If

        Dim quantite As Integer = fin - debut + 1

        For i = debut To fin
            Dim row As DataRow = schemaTable.Rows(i - 1)
            If currentDatabase.Id_TypeDatabase = TypeDatabase.SQLSERVER Then
                Dim obj As New Cls_Table(currentDatabase.ID, row("TABLE_NAME"))
                If obj.ID > 0 Then
                    With obj

                    End With
                Else
                    With obj
                        .Name = row("TABLE_NAME")
                        .Id_DataBase = iddatabase
                        .Insert()
                    End With
                End If

                CreateLocalColumn(obj.ID, obj.Name)
                'ElseIf currentDatabase.Id_TypeDatabase = TypeDatabase.MYSQL Then
                '    Dim obj As New Cls_Table(currentDatabase.ID, row.ItemArray(0).ToString)
                '    If obj.ID > 0 Then


                '    Else
                '        With obj
                '            .Name = row.ItemArray(0).ToString
                '            .Id_DataBase = iddatabase
                '            .Insert()
                '        End With
                '    End If
                '    CreateLocalColumMySql(obj.ID, obj.Name, TypeDatabase.MYSQL)
                'ElseIf currentDatabase.Id_TypeDatabase = TypeDatabase.POSTGRESSQL Then

                '    Dim obj As New Cls_Table(currentDatabase.ID, row.ItemArray(0).ToString)
                '    If obj.ID > 0 Then


                '    Else
                '        With obj
                '            .Name = row.ItemArray(0).ToString
                '            .Id_DataBase = iddatabase
                '            .Insert()
                '        End With
                '    End If
                '    CreateLocalColumnPostGres(obj.ID, obj.Name, TypeDatabase.POSTGRESSQL)
            End If

            compteur = compteur + 1
            background.ReportProgress(compteur * (100 / quantite))
        Next

    End Sub

#End Region

#Region "Delete"
    Public Function CleanData()
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Delete from tbl_ForeignKey"
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "Delete from tbl_ReportDynamicWhereCondition"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "Delete from tbl_ReportDynamicParameter"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "Delete from tbl_ReportDynamicTable"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "Delete from tbl_ReportDynamic"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "Delete from tbl_GroupTable"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "Delete from tbl_UniqueIndex"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "Delete from tbl_Column"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "Delete from tbl_Table"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "Delete from tbl_Database"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        ObjectConnection.Close()

    End Function
#End Region

#Region "Search"

    Public Function SearchAllConnection() As List(Of Cls_Connection)
        Return Cls_Connection.SeachAll
    End Function

    Public Function SearchAllConnectionByType(ByVal idtype As Long) As List(Of Cls_Connection)
        Return Cls_Connection.SeachAll_ByTypeDatabase(idtype)
    End Function
#End Region











End Class

