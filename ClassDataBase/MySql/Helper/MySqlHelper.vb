Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.ComponentModel
Imports System.Text.RegularExpressions

Public Class MySqlHelper

#Region "Attributes"
    Public Shared servername As String
    Public Shared password As String
    Public Shared database As String
    Public Shared user_login As String
    Public Shared ForeinKeyPrefix As String
    Public Shared Schema As DataTable
    Public Shared List_of_Nervous_Types_My_Sql As New List(Of String)
#End Region

#Region "Loading Tables Fonctions"

    Public Shared Function LoadTableStructure_MySql(ByVal table As String) As DataSet
        Dim ds As New DataSet
        Dim ConString As String =
            "Persist Security Info=True;SslMode=none;" &
            "server=" & servername & ";" &
            "User Id=" & user_login & ";" &
            "password=" & password & ";" &
            "database=" & database & ";"
        Try
            Dim Con As New MySqlConnection(ConString)
            Con.Open()
            Dim cmd As New MySqlCommand
            cmd.CommandText = "DESCRIBE " & table
            cmd.CommandType = CommandType.Text
            cmd.Connection = Con
            Dim p As New MySqlParameter
            p.Value = table
            Dim da As MySqlDataAdapter
            da = New MySqlDataAdapter(cmd)
            da.Fill(ds)
            cmd.Parameters.Clear()
            Con.Close()
            Return ds
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Load Table Structure MySql", MessageBoxButtons.OK)
            'Error_Log("LoadTableStructure", ex.Message)
        End Try
        Return Nothing
    End Function

    Public Shared Function LoadUserTablesSchema_MySql(
            ByVal treeview1 As TreeView) As ArrayList
        'ByVal strServer As String,
        'ByVal strUser As String,
        'ByVal strPwd As String,
        'ByVal strDatabase As String,

        Dim slTables As ArrayList = New ArrayList()
        Dim ConString As String =
            "Persist Security Info=True;SslMode=none;" &
            "server=" & servername & ";" &
            "User Id=" & user_login & ";" &
            "password=" & password & ";" &
            "database=" & database & ";"

        Dim strQUERY As String = "SHOW TABLES FROM " & database & ""
        'Dim strQUERY As String = "SELECT DISTINCT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_SCHEMA =" & strDatabase & ""

        Dim table As DataTable = Nothing
        Dim con As MySqlConnection = New MySqlConnection(ConString)
        Dim cmd As New MySqlCommand
        Dim ds As New DataSet
        Dim ds2 As New DataSet
        Dim da As MySqlDataAdapter
        Dim dr2 As DataRow
        Try
            con.Open()
            cmd.Connection = con
            cmd.CommandText = strQUERY
            da = New MySqlDataAdapter(strQUERY, con)
            da.Fill(ds2)
            table = ds2.Tables(0)

            treeview1.Nodes.Clear()
            For Each dr2 In table.Rows
                Dim node As New TreeNode
                node.Text = dr2("Tables_in_" & database)
                ds = MySqlHelper.LoadTableStructure_MySql(node.Text)

                For Each dt As DataRow In ds.Tables(0).Rows
                    node.Nodes.Add(dt(0))
                Next
                treeview1.Nodes.Add(node)
                'treeview1.Nodes.Add(dr2("Tables_in_" & strDatabase))
            Next
            'Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            '_systeme.CreateConnectionLog(strServer, strUser, strPwd, TypeDatabase.MYSQL)
            con.Close()
        Catch x As OleDbException
            slTables = Nothing
        End Try
        Return slTables

    End Function

#End Region

#Region "Conversion Fonctions"
    Public Shared Function ConvertDBToJavaType(ByVal Type As String) As String
        Dim AndroidTypeHash As New Hashtable
        AndroidTypeHash.Add("bigint", "long")
        AndroidTypeHash.Add("binary", "boolean")
        AndroidTypeHash.Add("bit", "boolean")
        AndroidTypeHash.Add("char", "char")
        AndroidTypeHash.Add("date", "Date")
        AndroidTypeHash.Add("datetime", "Date")
        AndroidTypeHash.Add("datetime2", "Date")
        AndroidTypeHash.Add("DATETIMEOFFSET", "Date")
        AndroidTypeHash.Add("decimal", "double")
        AndroidTypeHash.Add("double", "double")
        AndroidTypeHash.Add("float", "float")
        AndroidTypeHash.Add("int", "int")
        AndroidTypeHash.Add("image", "byte[]")
        AndroidTypeHash.Add("money", "Currency")
        AndroidTypeHash.Add("nchar", "String") '' /* or tableau of char*/
        AndroidTypeHash.Add("nvarchar", "String")
        AndroidTypeHash.Add("numeric", "double")
        AndroidTypeHash.Add("rowversion", "")
        AndroidTypeHash.Add("smallint", "short")
        AndroidTypeHash.Add("smallmoney", "Currency")
        AndroidTypeHash.Add("time", "Timestamp")
        AndroidTypeHash.Add("varbinary", "")
        AndroidTypeHash.Add("varchar", "String")


        Return AndroidTypeHash(Type)
    End Function

    Public Shared Function ConvertMySQLDBToJavaType(ByVal Type As String, Optional ByVal canSubstring As Boolean = False) As String
        Dim AndroidTypeHash As New Hashtable
        AndroidTypeHash.Add("bigint", "long")
        AndroidTypeHash.Add("binary", "boolean")
        AndroidTypeHash.Add("bit", "byte")
        AndroidTypeHash.Add("char", "char")
        AndroidTypeHash.Add("date", "Date")
        AndroidTypeHash.Add("datetime", "Date")
        AndroidTypeHash.Add("datetime2", "Date")
        AndroidTypeHash.Add("DATETIMEOFFSET", "Date")
        AndroidTypeHash.Add("decimal", "double")
        AndroidTypeHash.Add("float", "float")
        AndroidTypeHash.Add("int", "int")
        AndroidTypeHash.Add("image", "byte[]")
        AndroidTypeHash.Add("money", "Currency")
        AndroidTypeHash.Add("nchar", "String") '' /* or tableau of char*/
        AndroidTypeHash.Add("nvarchar", "String")
        AndroidTypeHash.Add("numeric", "double")
        AndroidTypeHash.Add("rowversion", "")
        AndroidTypeHash.Add("smallint", "short")
        AndroidTypeHash.Add("smallmoney", "Currency")
        AndroidTypeHash.Add("time", "Time")
        AndroidTypeHash.Add("varbinary", "")
        AndroidTypeHash.Add("varchar", "String")

        If canSubstring Then
            Return AndroidTypeHash(Type.Substring(Type.Length, 4))
        Else
            Return AndroidTypeHash(Type)
        End If

    End Function

#End Region

#Region "Stored Procedure Fonctions "

    Public Shared Function Insert_Store(ByVal Name As String) As String

        Dim ds As DataSet = LoadTableStructure_MySql(Name)

        Dim cap As Integer

        cap = ds.Tables(0).Rows.Count


        Dim count As Integer = 0
        Dim paramStore As String = ""
        Dim champStore As String = ""
        Dim Id_table As String = ""
        Dim valueStore As String = ""
        Dim SpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary", "decimal", "numeric"}
        Dim LevelOneSpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary"}
        Dim LevelTwoSpecialChar As New List(Of String) From {"decimal", "numeric"}


        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(3).ToString = "PRI" Then
                Id_table = dt(0).ToString()
            End If
        Next

        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(0).ToString <> Id_table Then
                If count < cap Then

                    If Not dt(0).ToString.Equals("ModifBy") _
                        AndAlso Not dt(0).ToString.Equals("DateModif") Then

                        If dt(0).ToString.Equals("DateCreated") Then
                            If (paramStore = "") Then
                                'paramStore = "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                                champStore = "" & dt(0) & ""
                                valueStore = "NOW()"
                            Else
                                'paramStore &= Chr(13) & Chr(9) & "," & "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                                champStore &= Chr(13) & Chr(9) & Chr(9) & "," & "" & dt(0) & ""
                                valueStore &= Chr(13) & Chr(9) & Chr(9) & "," & "NOW()"
                            End If
                        Else
                            If (paramStore = "") Then
                                paramStore = "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                                champStore = "" & dt(0) & ""
                                valueStore = "_" & dt(0)
                            Else
                                paramStore &= Chr(13) & Chr(9) & "," & "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                                champStore &= Chr(13) & Chr(9) & Chr(9) & "," & "" & dt(0) & ""
                                valueStore &= Chr(13) & Chr(9) & Chr(9) & "," & "_" & dt(0)
                            End If
                        End If
                    End If

                    count += 1
                Else
                    Exit For
                End If
            End If

        Next
        Dim command As String = ""
        command &= Chr(9) & "INSERT INTO " & Name & Chr(13)
        command &= Chr(9) & "(" & Chr(13)
        command &= Chr(9) & Chr(9) & champStore & Chr(13)
        command &= Chr(9) & ")" & Chr(13)
        command &= Chr(9) & "VALUES" & Chr(13)
        command &= Chr(9) & "(" & Chr(13)
        command &= Chr(9) & Chr(9) & valueStore & Chr(13)
        command &= Chr(9) & ");"

        Dim objectname As String = Name.Substring(4, Name.Length - 4)
        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)

        Dim store As String = ""
        store &= "DELIMITER $$" & Chr(13)
        store &= "DROP PROCEDURE IF EXISTS `SP_Insert_" & objectname & "`$$" & Chr(13)
        store &= Chr(13)
        store &= "CREATE PROCEDURE `SP_Insert_" & objectname & "` " & Chr(13)
        store &= "(" & Chr(13)
        store &= Chr(9) & paramStore & Chr(13)
        store &= ")" & Chr(13)
        store &= "BEGIN " & Chr(13)
        store &= command & Chr(13)
        store &= Chr(13)
        store &= "SELECT LAST_INSERT_ID() AS ID;" & Chr(13)
        store &= Chr(13)
        store &= "END$$" & Chr(13)
        store &= Chr(13)
        store &= "DELIMITER ;" & Chr(13)

        Return store
    End Function

    Public Shared Function Update_StoreMySql(ByVal Name As String) As String

        Dim ds As DataSet = LoadTableStructure_MySql(Name)
        Dim cap As Integer
        cap = ds.Tables(0).Rows.Count

        Dim count As Integer = 0
        Dim paramStore As String = ""
        Dim champStore As String = ""
        Dim Id_table As String = ""
        Dim QuerySet As String = ""
        Dim SpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary", "decimal", "numeric"}
        Dim LevelOneSpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary"}
        Dim LevelTwoSpecialChar As New List(Of String) From {"decimal", "numeric"}

        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(3).ToString = "PRI" Then
                Id_table = dt(0).ToString()
            End If
        Next

        For Each dt As DataRow In ds.Tables(0).Rows
            'If dt(0).ToString <> Id_table Then
            If count < cap Then
                If Not dt(0).ToString.Equals("CreatedBy") _
                        AndAlso Not dt(0).ToString.Equals("DateCreated") Then

                    If dt(0).ToString.Equals("DateModif") Then
                        If QuerySet = "" Then
                            QuerySet = "" & Chr(9) & dt(0) & "" & " " & "= " & "NOW()"
                        Else
                            QuerySet &= Chr(13) & Chr(9) & Chr(9) & "," & "" & dt(0) & "" & " " & "= " & "NOW()"
                        End If

                        If (paramStore = "") Then
                            'paramStore = "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                        Else
                            'paramStore &= Chr(13) & Chr(9) & "," & "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                        End If
                    Else
                        If QuerySet = "" Then
                            If dt(0) <> Id_table Then
                                QuerySet = "" & dt(0) & "" & " " & "= " & "_" & dt(0)
                            End If
                        Else
                            If dt(0) <> Id_table Then
                                QuerySet &= Chr(13) & Chr(9) & Chr(9) & "," & "" & dt(0) & "" & " " & "= " & "_" & dt(0)
                            End If
                        End If

                            If (paramStore = "") Then
                            paramStore = "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                        Else
                            paramStore &= Chr(13) & Chr(9) & "," & "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                        End If
                    End If

                End If

                count += 1
            Else
                Exit For
            End If
            'End If
        Next

        Dim command As String = ""
        command &= Chr(9) & "UPDATE " & Name & Chr(13)
        command &= Chr(9) & "SET" & Chr(13)
        command &= Chr(9) & Chr(9) & QuerySet

        Dim objectname As String = Name.Substring(4, Name.Length - 4)

        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)

        Dim store As String = ""
        store &= "DELIMITER $$" & Chr(13)
        store &= "DROP PROCEDURE IF EXISTS `SP_Update_" & objectname & "`$$" & Chr(13)
        store &= Chr(13)
        store &= "CREATE PROCEDURE `SP_Update_" & objectname & "` " & Chr(13)
        store &= "(" & Chr(13)
        store &= Chr(9) & paramStore & Chr(13)
        store &= ")" & Chr(13)
        store &= "BEGIN " & Chr(13)
        store &= command & Chr(13)
        store &= Chr(9) & "WHERE " & Id_table & " = " & "_" & Id_table & " ;" & Chr(13)
        store &= "END$$" & Chr(13)
        store &= Chr(13)
        store &= "DELIMITER ;" & Chr(13)
        Return store
    End Function

    Public Shared Function Delete_StoreMySql(ByVal Name As String) As String

        Dim ds As DataSet = LoadTableStructure_MySql(Name)

        Dim cap As Integer

        cap = ds.Tables(0).Rows.Count
        Dim count As Integer = 0
        Dim paramStore As String = ""
        Dim champStore As String = ""
        Dim Id_table As String = ""
        Dim Id_table_type As String = ""
        Dim QuerySet As String = ""


        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(3).ToString = "PRI" Then
                Id_table = dt(0).ToString()
                Id_table_type = dt(1).ToString()
            End If
        Next

        Dim command As String = Chr(9) & "DELETE FROM " & Name
        Dim objectname As String = Name.Substring(4, Name.Length - 4)
        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)

        Dim store As String = ""
        store &= "DELIMITER $$" & Chr(13)
        store &= "DROP PROCEDURE IF EXISTS `SP_Delete_" & objectname & "`$$" & Chr(13)
        store &= "CREATE PROCEDURE SP_Delete_" & objectname & " " & Chr(13)
        store &= "(" & Chr(13)
        store &= Chr(9) & "IN _ID " & Id_table_type & Chr(13)
        store &= ")" & Chr(13)
        store &= "BEGIN" & Chr(13)
        store &= command & Chr(13)
        store &= Chr(9) & "WHERE " & Id_table & " = " & "_ID ;" & Chr(13)
        store &= "END$$" & Chr(13)
        store &= "DELIMITER ;" & Chr(13)
        Return store
    End Function

    Public Shared Function ListAll_StoreMySql(ByVal Name As String) As String
        Dim ds As DataSet = LoadTableStructure_MySql(Name)

        Dim cap As Integer

        cap = ds.Tables(0).Rows.Count

        Dim count As Integer = 0

        Dim command As String = Chr(9) & "SELECT * FROM " & Name & ";"

        Dim objectname As String = Name.Substring(4, Name.Length - 4)

        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)
        Dim store As String = ""
        store &= "DELIMITER $$" & Chr(13)
        store &= "DROP PROCEDURE IF EXISTS `SP_ListAll_" & objectname & "`$$" & Chr(13)
        store &= "CREATE PROCEDURE SP_ListAll_" & objectname & " ()" & Chr(13)
        store &= "BEGIN" & Chr(13)
        store &= command & Chr(13)
        store &= "END$$" & Chr(13)
        store &= "DELIMITER ;" & Chr(13)

        Return store
    End Function

    Public Shared Function SelectByID_StoreMySql(ByVal Name As String) As String

        Dim ds As DataSet = LoadTableStructure_MySql(Name)

        Dim cap As Integer

        cap = ds.Tables(0).Rows.Count

        Dim count As Integer = 0
        Dim Id_table As String = ""
        Dim Id_table_type As String = ""
        Dim QuerySet As String = ""

        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(3).ToString = "PRI" Then
                Id_table = dt(0).ToString()
                Id_table_type = dt(1).ToString()
            End If
        Next

        Dim command As String = Chr(9) & "SELECT * FROM " & Name

        Dim objectname As String = Name.Substring(4, Name.Length - 4)
        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)

        Dim store As String = ""
        store &= "DELIMITER $$" & Chr(13)
        store &= "DROP PROCEDURE IF EXISTS `SP_Select_" & objectname & "_ByID`$$" & Chr(13)
        store &= "CREATE PROCEDURE `SP_Select_" & objectname & "_ByID` " & Chr(13)
        store &= "(" & Chr(13)
        store &= Chr(9) & "IN _" & Id_table & " " & Id_table_type & Chr(13)
        store &= ")" & Chr(13)
        store &= "BEGIN" & Chr(13)
        store &= command & Chr(13)
        store &= Chr(9) & "WHERE " & Id_table & " = " & "_" & Id_table & ";" & Chr(13)
        store &= "END$$" & Chr(13)
        store &= "DELIMITER ;" & Chr(13)
        Return store
    End Function

    Public Shared Function SelectByIndexStoreMySql(ByVal Name As String) As String
        Dim ds As DataSet = LoadTableStructure_MySql(Name)

        Dim cap As Integer

        cap = ds.Tables(0).Rows.Count
        Dim count As Integer = 0
        Dim Id_table As String = ""
        Dim ListofIndex As New List(Of String)
        Dim ListofIndexType As New List(Of String)
        Dim index_li_type As New Hashtable
        Dim countIndex As Integer = 0
        Dim QuerySet As String = ""
        Dim storeglobal As String = ""
        Dim indexPosition As Integer = 0


        Dim SpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary", "decimal", "numeric"}
        Dim LevelOneSpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary"}
        Dim LevelTwoSpecialChar As New List(Of String) From {"decimal", "numeric"}

        For Each dt As DataRow In ds.Tables(2).Rows
            Id_table = dt(0).ToString()
        Next

        For Each dt As DataRow In ds.Tables(5).Rows
            If dt(2).ToString <> Id_table Then
                ListofIndex.Insert(countIndex, dt(2).ToString)
                countIndex = countIndex + 1
            End If
        Next

        For Each dt As DataRow In ds.Tables(1).Rows
            For Each index In ListofIndex
                If dt(0).ToString = index Then

                    If SpecialChar.Contains(dt(1).ToString) Then
                        If LevelOneSpecialChar.Contains(dt(1).ToString) Then
                            ListofIndexType.Add(dt(1) & "(" & dt(3) & ")")
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1) & "(" & dt(3) & ")"))
                        Else
                            ListofIndexType.Add(dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")")
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"))
                        End If

                    Else
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If

                End If
            Next

        Next


        For Each index As String In ListofIndex
            Dim command As String = Chr(9) & "SELECT *" & Chr(13) _
                                & Chr(9) & "FROM " & Name

            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)
            Dim store As String =
                                "CREATE PROCEDURE [dbo].[SP_Select_" & objectname & "_" & ListofIndex.Item(indexPosition) & "] " & Chr(13) _
                                & Chr(9) & "(" & Chr(13) _
                                & Chr(9) & Chr(9) & "@" & ListofIndex.Item(indexPosition) & " " & ListofIndexType.Item(index_li_type(indexPosition)) & Chr(13) _
                                & Chr(9) & ")" & Chr(13) & Chr(13) _
                                & "AS" & Chr(13) & Chr(13) _
                                & command & Chr(13) _
                                & Chr(9) & "WHERE " & ListofIndex.Item(indexPosition) & " = " & "@" & ListofIndex.Item(indexPosition) & Chr(13) & Chr(13) _
                            & "" & Chr(13) & Chr(13)

            indexPosition = indexPosition + 1
            storeglobal = storeglobal & store
        Next


        Return storeglobal
    End Function

    Public Shared Function ListAllByForeignKeyMySql(ByVal Name As String) As String
        Dim ds As DataSet = LoadTableStructure_MySql(Name)
        Dim cap As Integer = ds.Tables(1).Rows.Count
        Dim count As Integer = 0
        Dim Id_table As String = ""
        Dim ListofForeignKey As New List(Of String)
        Dim ListofForeignKeyType As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim QuerySet As String = ""
        Dim storeglobal As String = ""
        Dim foreignkeyPosition As Integer = 0
        Dim key_li_type As New Hashtable


        For Each dt As DataRow In ds.Tables(2).Rows
            Id_table = dt(0).ToString()
        Next

        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(0).ToString = "MUL" Then '"FOREIGN KEY" Then
                ListofForeignKey.Add(dt(6).ToString)
                countForeignKey = countForeignKey + 1
            End If
        Next

        For Each dt As DataRow In ds.Tables(1).Rows
            For Each key In ListofForeignKey
                If dt(0).ToString = key Then
                    If dt(1).ToString = "nvarchar" Then
                        ListofForeignKeyType.Add(dt(1) & "(" & dt(3) & ")")
                        key_li_type.Add(ListofForeignKey.IndexOf(key), ListofForeignKeyType.IndexOf(dt(1) & "(" & dt(3) & ")"))
                    Else
                        ListofForeignKeyType.Add(dt(1))
                        key_li_type.Add(ListofForeignKey.IndexOf(key), ListofForeignKeyType.IndexOf(dt(1)))
                    End If

                End If
            Next
        Next


        For Each key As String In ListofForeignKey
            Dim command As String = Chr(9) & "SELECT * FROM " & Name

            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)
            Dim patternname As String = ListofForeignKey.Item(foreignkeyPosition).Substring(3, ListofForeignKey.Item(foreignkeyPosition).Length - 3)

            Dim store As String =
                                "CREATE PROCEDURE [dbo].[SP_ListAll_" & objectname & "_" & patternname & "] " & Chr(13) _
                                & Chr(9) & "(" & Chr(13) _
                                & Chr(9) & Chr(9) & "@" & ListofForeignKey.Item(foreignkeyPosition) & " " & ListofForeignKeyType(key_li_type(foreignkeyPosition)) & Chr(13) _
                                & Chr(9) & ")" & Chr(13) & Chr(13) _
                                & "AS" & Chr(13) & Chr(13) _
                                & command & Chr(13) _
                                & Chr(9) & "WHERE " & ListofForeignKey.Item(foreignkeyPosition) & " = " & "@" & ListofForeignKey.Item(foreignkeyPosition) & Chr(13) & Chr(13) _
                                & "" & Chr(13) & Chr(13)
            foreignkeyPosition = foreignkeyPosition + 1
            storeglobal = storeglobal & store
        Next


        Return storeglobal
    End Function
#End Region

#Region "Java Class Fonctions"
    Public Shared Sub CreateJavaClassDomaine(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim nomUpperClasse As String = nomClasse.Substring(0, 1).ToUpper() & nomClasse.Substring(1, nomClasse.Length - 1)
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\", Application.StartupPath & "\SCRIPT\GENERIC_12\")
        Dim path As String = txt_PathGenerate_Script & nomUpperClasse & ".java"
        Dim ListofIndex As New List(Of String)
        Dim ListofIndexType As New List(Of String)
        Dim index_li_type As New Hashtable
        Dim countindex As Long = 0
        Dim insertstring As String = ""
        Dim updatestring As String = ""

        Dim header As String = "'''Generate By Edou Application *******" & Chr(13) _
                               & "''' Class " + nomUpperClasse & Chr(13) & Chr(13)
        header = ""
        Dim content As String = "public class " & nomUpperClasse & " {" & Chr(13)

        _end = "}" & Chr(13)
        ' Delete the file if it exists.
        If File.Exists(path) Then
            File.Delete(path)
        End If
        ' Create the file.
        Dim fs As FileStream = File.Create(path, 1024)
        fs.Close()
        Dim objWriter As New System.IO.StreamWriter(path, True)

        objWriter.WriteLine("package ht.edu.fds.domaine;")
        objWriter.WriteLine()
        objWriter.WriteLine("import java.sql.Timestamp;")
        objWriter.WriteLine("import java.util.*;")
        objWriter.WriteLine(content)
        objWriter.WriteLine()


        Dim ds As DataSet = MySqlHelper.LoadTableStructure_MySql(name)
        Dim cols As New List(Of String)
        Dim types As New List(Of String)
        Dim initialtypes As New List(Of String)
        Dim length As New List(Of String)
        Dim count As Integer = 0
        Dim cap As Integer = ds.Tables(0).Rows.Count

        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(3).ToString = "PRI" Then
                Id_table = dt(0).ToString()
            End If

        Next

        For Each dt As DataRow In ds.Tables(0).Rows
            If count < cap - 4 Then
                cols.Add("" & dt(0))
                initialtypes.Add(dt(1))
                If dt(1).ToString.Contains("(") Then
                    Dim arrstring As String() = dt(1).ToString.Split("(")
                    types.Add(ConvertDBToJavaType(arrstring(0).ToString))
                Else
                    types.Add(ConvertDBToJavaType((dt(1))))
                End If
                length.Add(dt(3))
                count += 1
            Else
                Exit For
            End If
        Next
        objWriter.WriteLine("//<editor-fold defaultstate=""collapsed"" desc=""Attributes"">")
        'objWriter.WriteLine("Private _id As Long")
        objWriter.WriteLine()
        objWriter.WriteLine(" private long id ; ")
        Try
            For i As Int32 = 1 To cols.Count - 1

                'If Not nottoputforlist.Contains(cols(i)) Then
                '    insertstring &= ", " & cols(i)
                '    updatestring &= ", " & cols(i)
                'End If
                Dim attrib As String = ""  'not used for now to be updated.

                objWriter.WriteLine("private " & types(i) & " " & cols(i) & ";")
                If initialtypes(i) = "image" Then

                    objWriter.WriteLine("private String " & cols(i) & "String;" & "")
                End If
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("private " & cols(i).Substring(3, cols(i).Length - 3) & " " & cols(i).Substring(3, cols(i).Length - 3).ToLower & ";")
                End If
            Next
        Catch ex As Exception

        End Try
        objWriter.WriteLine()
        objWriter.WriteLine("//</editor-fold>")
        objWriter.WriteLine()
        objWriter.WriteLine()

        ''''''''''''''''''''''''''''''''''''''''çonstructeur'''''''''''''''''''''''''''''''''''''
        Dim listof_number As New List(Of String) From {"long"}
        Dim listof_string As New List(Of String) From {"String"}
        Dim listof_nulltypes As New List(Of String) From {"Date"}

        objWriter.WriteLine("//<editor-fold defaultstate=""collapsed"" desc=""Constructeurs"">")
        objWriter.WriteLine()
        objWriter.WriteLine("public " & nomUpperClasse & "() {")
        objWriter.WriteLine("this.id = 0;")
        Dim attribut_string As String = ""
        Dim constructeur_string As String = ""
        Dim attribut_string2 As String = "long id,"
        For i As Int32 = 1 To cols.Count - 1
            If attribut_string = "" Then
                attribut_string &= types(i) & " " & cols(i)
                constructeur_string &= cols(i)
            Else
                attribut_string &= "," & types(i) & " " & cols(i)
                constructeur_string &= "," & cols(i)
            End If
            If listof_number.Contains(types(i)) Then
                objWriter.WriteLine("this." & cols(i) & " = " & "0;")
            ElseIf listof_string.Contains(types(i)) Then
                objWriter.WriteLine("this." & cols(i) & " = " & """"";")
            ElseIf listof_nulltypes.Contains(types(i)) Then
                objWriter.WriteLine("this." & cols(i) & " = " & "null;")
            End If
        Next
        objWriter.WriteLine("}")


        attribut_string &= ")"
        attribut_string2 &= attribut_string

        objWriter.WriteLine()


        objWriter.WriteLine("public " & nomUpperClasse & "(long id)" & " {")
        objWriter.WriteLine(" this();")
        objWriter.WriteLine("this.id = id;")
        objWriter.WriteLine("}")

        objWriter.WriteLine()


        objWriter.WriteLine("public " & nomUpperClasse & "(" & attribut_string & " {")
        objWriter.WriteLine("  this.id = 0;")
        For i As Int32 = 1 To cols.Count - 1
            objWriter.WriteLine("this." & cols(i) & " = " & cols(i) & ";")
        Next

        objWriter.WriteLine("}")
        objWriter.WriteLine()

        objWriter.WriteLine("public " & nomUpperClasse & "(" & attribut_string2 & " {")
        objWriter.WriteLine("this(" & constructeur_string & ") ;")
        objWriter.WriteLine("  this.id = id;")
        objWriter.WriteLine("}")

        objWriter.WriteLine("//</editor-fold>")


        objWriter.WriteLine()

        '''''''''''''''''''''''''''''''''''''''''properties''''''''''''''''''''''''''''''''''''

        objWriter.WriteLine("//<editor-fold defaultstate=""collapsed"" desc=""Properties"">")

        objWriter.WriteLine("public long getId() {" & Chr(13) & _
                             " return id;" & Chr(13) & _
                             "}")

        For i As Int32 = 1 To cols.Count - 1 ''On ne cree pas de property pour la derniere column
            Dim propName As String = ""
            Dim s As String() = cols(i).Split("_")
            For j As Integer = 1 To s.Length - 1
                propName &= StrConv(s(j), VbStrConv.ProperCase)
            Next

            Dim attrib As String = "public " & types(i) & " get" & cols(i) & "() {"
            Dim setter As String = "public void " & "set" & cols(i) & "(" & types(i) & " " & cols(i).ToLower() & ") {"

            If cols(i) <> "_isdirty" Or cols(i) <> "_LogData" Then
                '   objWriter.WriteLine(log)

                objWriter.WriteLine()
                objWriter.WriteLine(attrib)
                objWriter.WriteLine("return " & cols(i) & ";")
                objWriter.WriteLine("}")
                objWriter.WriteLine(setter)
                objWriter.WriteLine("this." & cols(i) & " = " & cols(i).ToLower & ";")
                objWriter.WriteLine("}")

                If initialtypes(i) = "image" Then
                    objWriter.WriteLine()
                    objWriter.WriteLine("public boolean hasImage() {")
                    objWriter.WriteLine("return (this." & cols(i) & "String != null && this." & cols(i) & "String.length() > 0 || this." & cols(i) & "!= null && this." & cols(i) & ".length > 0);")
                    objWriter.WriteLine("}")
                    objWriter.WriteLine("public String get" & cols(i) & "String() {")
                    objWriter.WriteLine("return " & cols(i).ToLower & "String;")
                    objWriter.WriteLine("}")

                    objWriter.WriteLine("public void set" & cols(i) & "String(String " & cols(i).ToLower & "String) {")
                    objWriter.WriteLine("this." & cols(i) & "String = " & cols(i).ToLower & "String;")
                    objWriter.WriteLine("}")


                End If

                'If ListofForeignKey.Contains(cols(i)) Then

                '    Dim ClassName As String = cols(i).Substring(3, cols(i).Length - 3)
                '    Dim attributUsed As String = ClassName.ToLower()
                '    '  objWriter.WriteLine("public cols(i).Substring(3, cols(i).Length - 3) & Chr(13))
                '    objWriter.WriteLine("public " & ClassName & " get" & ClassName & "() {")
                '    objWriter.WriteLine("if (" & attributUsed & "==null)")
                '    objWriter.WriteLine(attributUsed & " = " & ClassName & "Helper.searchByID(" & cols(i) & ");")
                '    objWriter.WriteLine("return " & attributUsed & ";")
                '    objWriter.WriteLine("}")

                '    objWriter.WriteLine("public void set" & ClassName & "(" & ClassName & " " & attributUsed & ") {")
                '    objWriter.WriteLine("this." & ClassName & " = " & attributUsed & ";")
                '    objWriter.WriteLine("}")

                'End If
            End If

        Next

        objWriter.WriteLine("//</editor-fold>")

        objWriter.WriteLine()

        objWriter.WriteLine()
        objWriter.WriteLine(_end)
        objWriter.WriteLine()
        objWriter.Close()

    End Sub

    Public Shared Sub CreateJavaClassDAL(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim nomUpperClasse As String = nomClasse.Substring(0, 1).ToUpper() & nomClasse.Substring(1, nomClasse.Length - 1)

        Dim point_interogation As String = ""
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\", Application.StartupPath & "\SCRIPT\GENERIC_12\")
        Dim path As String = txt_PathGenerate_Script & nomUpperClasse & "DAL.java"
        Dim ListofIndex As New List(Of String)
        Dim ListofIndexType As New List(Of String)
        Dim index_li_type As New Hashtable
        Dim countindex As Long = 0
        Dim insertstring As String = ""
        Dim updatestring As String = ""
        Dim compteur As Integer
        Dim header As String = "'''Generate By Edou Application *******" & Chr(13) _
                               & "''' Class " + nomUpperClasse & Chr(13) & Chr(13)
        header = ""
        Dim content As String = "public class " & nomUpperClasse & "DAL {" & Chr(13)

        _end = "}" & Chr(13)
        nomClasse = nomUpperClasse
        ' Delete the file if it exists.
        If File.Exists(path) Then
            File.Delete(path)
        End If
        ' Create the file.
        Dim fs As FileStream = File.Create(path, 1024)
        fs.Close()
        Dim objWriter As New System.IO.StreamWriter(path, True)

        objWriter.WriteLine("package ht.edu.fds.dal;")
        objWriter.WriteLine()
        objWriter.WriteLine("import java.util.*;")
        objWriter.WriteLine("import com.sun.rowset.CachedRowSetImpl;" & Chr(13) & _
                            "import ht.edu.fds.domaine.*;" & Chr(13) & _
                            "import ht.edu.fds.servtech.*;" & Chr(13) & _
                            "import java.sql.*;" & Chr(13) & _
                            "import javax.sql.rowset.CachedRowSet;")

        objWriter.WriteLine(content)
        objWriter.WriteLine()


        Dim ds As DataSet = MySqlHelper.LoadTableStructure_MySql(name)
        Dim cols As New List(Of String)
        Dim types As New List(Of String)
        Dim initialtypes As New List(Of String)
        Dim length As New List(Of String)
        Dim count As Integer = 0
        Dim cap As Integer = ds.Tables(0).Rows.Count

        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(3).ToString = "PRI" Then
                Id_table = dt(0).ToString()
            End If

        Next
        For Each dt As DataRow In ds.Tables(0).Rows
            If count < cap - 4 Then
                cols.Add("" & dt(0))
                initialtypes.Add(dt(1))
                If dt(1).ToString.Contains("(") Then
                    Dim arrstring As String() = dt(1).ToString.Split("(")
                    types.Add(ConvertDBToJavaType(arrstring(0).ToString))
                Else
                    types.Add(ConvertDBToJavaType((dt(1))))

                End If
                length.Add(dt(3))
                count += 1
            Else
                Exit For
            End If
        Next

        objWriter.WriteLine("//<editor-fold defaultstate=""collapsed"" desc=""Attributes"">")
        'objWriter.WriteLine("Private _id As Long")
        objWriter.WriteLine("private static String driverstring = ConnectionDAL.getCurrentdriverstring();")
        objWriter.WriteLine("private static String connectionstring = ConnectionDAL.getCurrentconnectionstring();")
        objWriter.WriteLine("private static CallableStatement cs ;")
        objWriter.WriteLine("private static Connection con ;")
        objWriter.WriteLine("private static ResultSet reponse;")
        objWriter.WriteLine("private static CachedRowSet crs; ")

        objWriter.WriteLine("//</editor-fold>")
        objWriter.WriteLine()

        objWriter.WriteLine("//<editor-fold defaultstate=""collapsed"" desc=""Save"">")

        objWriter.WriteLine("  public static boolean Save(" & nomUpperClasse & " obj) throws Exception ")
        objWriter.WriteLine(" {")
        objWriter.WriteLine()
        objWriter.WriteLine("long ID = obj.getId() ;")
        objWriter.WriteLine("Boolean valret = false ;")
        objWriter.WriteLine(" try " & Chr(13) & _
                            " { " & Chr(13) & _
                            "Class.forName(driverstring); " & Chr(13) & _
                            "con = DriverManager.getConnection(connectionstring);" & Chr(13) & _
                            "if(ID == 0)" & Chr(13) & _
                            "{ ")
        compteur = 0
        For i As Int32 = 1 To cols.Count
            If point_interogation = "" Then
                point_interogation = "?"
            Else
                point_interogation &= ",?"
            End If

        Next
        Dim fin_point_interogation As String = ")  }"");"

        objWriter.WriteLine(" cs =   con.prepareCall(""{call SP_Insert" & nomClasse & "(" & point_interogation & fin_point_interogation)


        For i As Int32 = 1 To cols.Count - 1
            Dim attrib As String = ""  'not used for now to be updated.
            If types(i) = "Date" Then
                objWriter.WriteLine(" cs.set" & types(i).Substring(0, 1).ToUpper() & types(i).Substring(1, types(i).Length - 1) & "(" & i & ",TypeSafeConversion.NullSafeDate_ToSql(obj.get" & cols(i) & "()));")
            Else
                objWriter.WriteLine(" cs.set" & types(i).Substring(0, 1).ToUpper() & types(i).Substring(1, types(i).Length - 1) & "(" & i & ",obj.get" & cols(i) & "());")
            End If

        Next

        objWriter.WriteLine("   cs.setString(" & cols.Count & ",""Admin"");")

        objWriter.WriteLine("}")
        objWriter.WriteLine("else")
        objWriter.WriteLine("{")
        point_interogation &= ",?"

        objWriter.WriteLine(" cs =   con.prepareCall(""{call SP_Update" & nomClasse & "(" & point_interogation & fin_point_interogation)
        objWriter.WriteLine(" cs.setLong" & "(1,obj.getId());")
        For i As Int32 = 1 To cols.Count - 1
            Dim attrib As String = ""  'not used for now to be updated.
            If types(i) = "Date" Then
                objWriter.WriteLine(" cs.set" & types(i).Substring(0, 1).ToUpper() & types(i).Substring(1, types(i).Length - 1) & "(" & i & ",TypeSafeConversion.NullSafeDate_ToSql(obj.get" & cols(i) & "()));")
            Else
                objWriter.WriteLine(" cs.set" & types(i).Substring(0, 1).ToUpper() & types(i).Substring(1, types(i).Length - 1) & "(" & i + 1 & ",obj.get" & cols(i) & "());")
            End If

        Next
        objWriter.WriteLine("   cs.setString(" & cols.Count + 1 & ",""Admin"");")
        objWriter.WriteLine("}")




        objWriter.WriteLine("   ResultSet rs = cs.executeQuery(); " & Chr(13) & _
                             "while (rs.next()) {" & Chr(13) & _
                              " ID  = rs.getLong(""ID"");   " & Chr(13) & _
                              "  }" & Chr(13) & _
                              "if (ID > 0) {" & Chr(13) & _
                              " valret = true;" & Chr(13) & _
                              "    }" & Chr(13) & _
                              "  }" & Chr(13) & _
                              "  catch (ClassNotFoundException nfex)" & Chr(13) & _
                              "{" & Chr(13) & _
                              "  throw new ClassNotFoundException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",save) ! ""+ nfex.getMessage());  " & Chr(13) & _
                              "}" & Chr(13) & _
                              "catch (Exception ex)" & Chr(13) & _
                              "{" & Chr(13) & _
                             "  throw new Exception("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",save) ! ""+ ex.getMessage());  " & Chr(13) & _
                              "}" & Chr(13) & _
                              "finally" & Chr(13) & _
                              "{" & Chr(13) & _
                              "try" & Chr(13) & _
                              "{" & Chr(13) & _
                              " con.close();" & Chr(13) & _
                              "}" & Chr(13) & _
                              "catch (SQLException sqlex)" & Chr(13) & _
                              "{" & Chr(13) & _
                              "  throw new SQLException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",save) ! ""+ sqlex.getMessage());  " & Chr(13) & _
                              "}" & Chr(13) & _
                              "}" & Chr(13) & _
                              "return valret;   " & Chr(13) & _
                              "}" & Chr(13))


        objWriter.WriteLine("//</editor-fold>")
        objWriter.WriteLine()
        objWriter.WriteLine()

        '''''''''''''''''''''''''''''''''''''''''delete''''''''''''''''''''''''''''''''''''

        objWriter.WriteLine("//<editor-fold defaultstate=""collapsed"" desc=""Delete"">")

        objWriter.WriteLine("public static boolean Delete(" & nomUpperClasse & " obj) throws Exception" & Chr(13) & _
                                "{" & Chr(13) & _
                                    "boolean isDeleted = false;" & Chr(13) & _
                                    "try" & Chr(13) & _
                                    "{" & Chr(13) & _
                                       " Class.forName(driverstring);" & Chr(13) & _
                                        "con = DriverManager.getConnection(connectionstring);" & Chr(13) & _
                                        "cs =   con.prepareCall(""{call SP_Delete" & nomClasse & "(?) }"");" & Chr(13) & _
                                        "cs.setLong(1, obj.getId());" & Chr(13) & _
                                        "int result =  cs.executeUpdate();" & Chr(13) & _
                                        "if (result > 0) {isDeleted = true; }" & Chr(13) & _
                                    "}" & Chr(13) & _
                                    "catch (ClassNotFoundException nfex)" & Chr(13) & _
                                    "{" & Chr(13) & _
                                    "  throw new ClassNotFoundException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",Delete) ! ""+ nfex.getMessage());  " & Chr(13) & _
                                    "}" & Chr(13) & _
                                    "catch (Exception ex)" & Chr(13) & _
                                    "{" & Chr(13) & _
                                         "  throw new Exception("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",Delete) ! ""+ ex.getMessage());  " & Chr(13) & _
                                    "}" & Chr(13) & _
                                    "finally" & Chr(13) & _
                                    "{" & Chr(13) & _
                                        "try" & Chr(13) & _
                                        "{" & Chr(13) & _
                                            "con.close();" & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "catch (SQLException sqlex)" & Chr(13) & _
                                        "{" & Chr(13) & _
                                          "  throw new SQLException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",Delete) ! ""+ sqlex.getMessage());  " & Chr(13) & _
                                        "}" & Chr(13) & _
                                    "}     " & Chr(13) & _
                                     "return isDeleted;" & Chr(13) & _
                                "}")
        objWriter.WriteLine("//</editor-fold>")

        '''''''''''''''''''''''''''''''''''''''''Read''''''''''''''''''''''''''''''''''''
        objWriter.WriteLine()
        objWriter.WriteLine("//<editor-fold defaultstate=""collapsed"" desc=""Read"">")

        objWriter.WriteLine("public static void Read(" & nomUpperClasse & " obj) throws Exception " & Chr(13) & _
                               " {" & Chr(13) & _
                                    "try" & Chr(13) & _
                                    "{" & Chr(13) & _
                                        "Class.forName(driverstring);" & Chr(13) & _
                                        "con = DriverManager.getConnection(connectionstring);" & Chr(13) & _
                                        "cs =   con.prepareCall(""{call SP_Select" & nomClasse & "_ByID(?) }"");" & Chr(13) & _
                                        "cs.setLong(1, obj.getId());" & Chr(13) & _
                                        "reponse = cs.executeQuery();" & Chr(13) & _
                                        "reponse.last();" & Chr(13) & _
                                        "reponse.beforeFirst();" & Chr(13) & _
                                        "while(reponse.next())" & Chr(13) & _
                                        "{")
        For i As Int32 = 1 To cols.Count - 1
            Dim attrib As String = ""  'not used for now to be updated.
            objWriter.WriteLine(" obj.set" & cols(i) & "(reponse.get" & types(i).Substring(0, 1).ToUpper() & types(i).Substring(1, types(i).Length - 1) & "(" & i + 1 & " ));")
        Next
        objWriter.WriteLine("             } " & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "catch (ClassNotFoundException nfex)" & Chr(13) & _
                                        "{" & Chr(13) & _
                                          "  throw new ClassNotFoundException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",Read) ! ""+ nfex.getMessage());  " & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "catch (Exception ex)" & Chr(13) & _
                                        "{" & Chr(13) & _
                                            "  throw new Exception("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",Read) ! ""+ ex.getMessage());  " & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "finally" & Chr(13) & _
                                        "{" & Chr(13) & _
                                            "try" & Chr(13) & _
                                            "{" & Chr(13) & _
                                                "con.close();" & Chr(13) & _
                                            "}" & Chr(13) & _
                                            "catch (SQLException sqlex)" & Chr(13) & _
                                            "{" & Chr(13) & _
                                                "  throw new SQLException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",Read) ! ""+ sqlex.getMessage());  " & Chr(13) & _
                                            "}" & Chr(13) & _
                                        "} " & Chr(13) & _
                                    "}")
        objWriter.WriteLine("//</editor-fold>")
        objWriter.WriteLine()
        '''''''''''''''''''''''''''''''''''''''''ListAll''''''''''''''''''''''''''''''''''''
        objWriter.WriteLine("//<editor-fold defaultstate=""collapsed"" desc=""ListAll"">")

        objWriter.WriteLine("public static CachedRowSet ListAll() throws Exception" & Chr(13) & _
                                    "{" & Chr(13) & _
            Chr(9) & Chr(9) & Chr(9) & "try" & Chr(13) & _
                                        "{" & Chr(13) & _
                                            "crs = new CachedRowSetImpl();" & Chr(13) & _
                                            "Class.forName(driverstring);" & Chr(13) & _
                                            "con = DriverManager.getConnection(connectionstring);" & Chr(13) & _
                                            "cs =   con.prepareCall(""{call SP_ListAll_" & nomClasse & "() }"");" & Chr(13) & _
                                            "reponse = cs.executeQuery();" & Chr(13) & _
                                            "crs.populate(reponse);" & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "catch (ClassNotFoundException nfex)" & Chr(13) & _
                                        "{" & Chr(13) & _
                                           "  throw new ClassNotFoundException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",ListAll) ! ""+ nfex.getMessage());  " & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "catch (Exception ex)" & Chr(13) & _
                                        "{" & Chr(13) & _
                                            "  throw new Exception("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",ListAll) ! ""+ ex.getMessage());  " & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "finally" & Chr(13) & _
                                        "{" & Chr(13) & _
                                            "try" & Chr(13) & _
                                            "{" & Chr(13) & _
                                                "con.close();" & Chr(13) & _
                                            "}" & Chr(13) & _
                                            "catch (SQLException sqlex)" & Chr(13) & _
                                            "{" & Chr(13) & _
                                              "  throw new SQLException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",ListAll) ! ""+ sqlex.getMessage());  " & Chr(13) & _
                                            "}" & Chr(13) & _
                                            "return crs;   " & Chr(13) & _
                                        "}        " & Chr(13) & _
                                    "}")

        objWriter.WriteLine("//</editor-fold>")

        objWriter.WriteLine()

        objWriter.WriteLine()
        objWriter.WriteLine(_end)
        objWriter.WriteLine()
        objWriter.Close()

    End Sub

    Public Shared Sub CreateJavaClassSession(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim nomUpperClasse As String = nomClasse.Substring(0, 1).ToUpper() & nomClasse.Substring(1, nomClasse.Length - 1)
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\", Application.StartupPath & "\SCRIPT\GENERIC_12\")
        Dim path As String = txt_PathGenerate_Script & "Session" & nomUpperClasse & ".java"
        Dim ListofIndex As New List(Of String)
        Dim ListofIndexType As New List(Of String)
        Dim index_li_type As New Hashtable
        Dim countindex As Long = 0
        Dim insertstring As String = ""
        Dim updatestring As String = ""

        Dim header As String = "'''Generate By Edou Application *******" & Chr(13) _
                               & "''' Class " + nomUpperClasse & Chr(13) & Chr(13)
        header = ""
        Dim content As String = "public class " & "Session" & nomUpperClasse & " {" & Chr(13)

        _end = "}" & Chr(13)
        ' Delete the file if it exists.
        If File.Exists(path) Then
            File.Delete(path)
        End If
        ' Create the file.
        Dim fs As FileStream = File.Create(path, 1024)
        fs.Close()
        Dim objWriter As New System.IO.StreamWriter(path, True)

        objWriter.WriteLine("package ht.edu.fds.application;")
        objWriter.WriteLine()


        objWriter.WriteLine("import ht.edu.fds.dal.*;" & Chr(13) & _
                            "import ht.edu.fds.domaine.*;" & Chr(13) & _
                            "import ht.edu.fds.servtech.*;" & Chr(13) & _
                            "import java.lang.Object;" & Chr(13) & _
                            "import java.sql.ResultSet;" & Chr(13) & _
                            "import java.sql.SQLException;" & Chr(13) & _
                            "import java.sql.Timestamp;" & Chr(13) & _
                            "import java.util.ArrayList;" & Chr(13) & _
                            "import java.util.Date;" & Chr(13) & _
                            "import java.util.logging.Level;" & Chr(13) & _
                            "import java.util.logging.Logger;")

        objWriter.WriteLine(content)
        objWriter.WriteLine()


        Dim ds As DataSet = MySqlHelper.LoadTableStructure_MySql(name)
        Dim cols As New List(Of String)
        Dim types As New List(Of String)
        Dim initialtypes As New List(Of String)
        Dim length As New List(Of String)
        Dim count As Integer = 0
        Dim cap As Integer = ds.Tables(0).Rows.Count

        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(3).ToString = "PRI" Then
                Id_table = dt(0).ToString()
            End If

        Next
        For Each dt As DataRow In ds.Tables(0).Rows
            If count < cap - 4 Then
                cols.Add("" & dt(0))
                initialtypes.Add(dt(1))
                If dt(1).ToString.Contains("(") Then
                    Dim arrstring As String() = dt(1).ToString.Split("(")
                    types.Add(ConvertDBToJavaType(arrstring(0).ToString))
                Else
                    types.Add(ConvertDBToJavaType((dt(1))))

                End If
                length.Add(dt(3))
                count += 1
            Else
                Exit For
            End If
        Next

        objWriter.WriteLine("private " & nomUpperClasse & " obj;")
        objWriter.WriteLine("private ResultSet list" & nomUpperClasse & ";")
        objWriter.WriteLine()
        objWriter.WriteLine("ResultSet getResultSet" & nomUpperClasse & "()")
        objWriter.WriteLine("{")
        objWriter.WriteLine("return this.list" & nomUpperClasse & ";")
        objWriter.WriteLine("}")
        objWriter.WriteLine()
        objWriter.WriteLine("public " & nomUpperClasse & " getObjetCourant()")
        objWriter.WriteLine("{")
        objWriter.WriteLine("return this.obj;")
        objWriter.WriteLine("}")
        objWriter.WriteLine()
        objWriter.WriteLine("public long getIDCurrentObject(" & nomUpperClasse & " obj)")
        objWriter.WriteLine("{")
        objWriter.WriteLine("this.obj = obj;")
        objWriter.WriteLine("return this.obj.getId();")
        objWriter.WriteLine("}")

        objWriter.WriteLine()
        Dim attribut_string As String = ""
        Dim constructeur_string As String = "Long.parseLong(ID)"
        Dim attribut_string2 As String = "String ID"
        For i As Int32 = 1 To cols.Count - 1
            If attribut_string2 = "" Then
                attribut_string2 &= "String " & cols(i)
                constructeur_string &= cols(i)
            Else
                attribut_string2 &= ", String " & cols(i)
                If types(i).ToString = "long" Then
                    constructeur_string &= ",Long.parseLong(" & cols(i) & ")"
                ElseIf types(i).ToString = "Double" Then
                    constructeur_string &= ",Double.parseDouble(" & cols(i) & ")"
                ElseIf types(i).ToString = "Date" Then
                    constructeur_string &= ",TypeSafeConversion.NullSafeDate(" & cols(i) & ")"
                ElseIf types(i).ToString = "float" Then
                    constructeur_string &= ",Float.parseFloat(" & cols(i) & ")"
                ElseIf types(i).ToString = "Timestamp" Then
                    constructeur_string &= ",Timestamp.valueOf(" & cols(i) & ")"
                ElseIf types(i).ToString = "boolean" Then
                    constructeur_string &= ",Boolean.parseBoolean(" & cols(i) & ")"
                Else
                    constructeur_string &= "," & cols(i)
                End If
            End If

        Next

        objWriter.WriteLine("public boolean save" & nomUpperClasse & "(" & attribut_string2 & " ) throws Exception")
        objWriter.WriteLine("{")
        objWriter.WriteLine("boolean isSaved = false;")
        objWriter.WriteLine("obj = new " & nomUpperClasse & "(" & constructeur_string & ");")
        objWriter.WriteLine("isSaved = " & nomUpperClasse & "DAL.Save(obj);")
        objWriter.WriteLine("return isSaved;")
        objWriter.WriteLine("}")

        objWriter.WriteLine()

        objWriter.WriteLine("public Object[][] listAll" & nomUpperClasse & "() throws Exception {")
        objWriter.WriteLine("int taille= 0 , i = 0 ;")
        objWriter.WriteLine(" Object[][] data = null;")
        objWriter.WriteLine("try")
        objWriter.WriteLine("{")
        objWriter.WriteLine("list" & nomUpperClasse & " = " & nomUpperClasse & "DAL.ListAll();")
        objWriter.WriteLine("list" & nomUpperClasse & ".last();")
        objWriter.WriteLine("taille = list" & nomUpperClasse & ".getRow();")
        objWriter.WriteLine("list" & nomUpperClasse & ".beforeFirst();")
        objWriter.WriteLine("data = new Object[taille][" & cols.Count & "];")
        objWriter.WriteLine("while(list" & nomUpperClasse & ".next())")
        objWriter.WriteLine("{")
        objWriter.WriteLine("long ID = list" & nomUpperClasse & ".getLong(1);")
        For i As Int32 = 1 To cols.Count - 1
            objWriter.WriteLine(types(i) & " " & cols(i) & " = list" & nomUpperClasse & ".get" & types(i).Substring(0, 1).ToUpper() & types(i).Substring(1, types(i).Length - 1) & "(" & i + 1 & ");")
        Next
        objWriter.WriteLine("data[i][0] = ID;")
        For i As Int32 = 1 To cols.Count - 1
            objWriter.WriteLine("data[i][" & i & "] = " & cols(i) & ";")
        Next
        objWriter.WriteLine("i++;")
        objWriter.WriteLine("}")
        objWriter.WriteLine("}")
        objWriter.WriteLine("catch (Exception ex)")
        objWriter.WriteLine("{")
        objWriter.WriteLine("throw new Exception("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",save) ! ""+ ex.getMessage());")
        objWriter.WriteLine("}")
        objWriter.WriteLine("return data;")
        objWriter.WriteLine("}")


        objWriter.WriteLine("public ArrayList<" & nomUpperClasse & "> listAll" & nomUpperClasse & "_ForObject() throws Exception")
        objWriter.WriteLine("{")
        objWriter.WriteLine("ArrayList<" & nomUpperClasse & "> liste = new ArrayList<" & nomUpperClasse & ">" & "();")
        objWriter.WriteLine("list" & nomUpperClasse & " = " & nomUpperClasse & "DAL.ListAll();")
        objWriter.WriteLine(" try {")
        objWriter.WriteLine(" while ( list" & nomUpperClasse & ".next())")
        objWriter.WriteLine("{")

        objWriter.WriteLine("long id = list" & nomUpperClasse & ".getLong(1);")

        Dim stringobj As String = "id"
        For i As Int32 = 1 To cols.Count - 1
            If stringobj = "" Then
                stringobj &= cols(i)
            Else
                stringobj &= "," & cols(i)
            End If
            objWriter.WriteLine(types(i) & " " & cols(i) & " = list" & nomUpperClasse & ".get" & types(i).Substring(0, 1).ToUpper() & types(i).Substring(1, types(i).Length - 1) & "(" & i + 1 & ");")
        Next

        objWriter.WriteLine("obj = new " & nomUpperClasse & "(" & stringobj & ");")
        objWriter.WriteLine("liste.add(obj);")
        objWriter.WriteLine("}")
        objWriter.WriteLine(" } " & Chr(13) & _
                            "catch (SQLException ex) {" & Chr(13) & _
                             "   Logger.getLogger(Session" & nomUpperClasse & ".class.getName()).log(Level.SEVERE, null, ex);" & Chr(13) & _
                            "}" & Chr(13) & _
                            "return liste; " & Chr(13) & _
                        "}")
        objWriter.WriteLine()

        objWriter.WriteLine()
        objWriter.WriteLine(_end)
        objWriter.WriteLine()
        objWriter.Close()
    End Sub
#End Region

#Region "Android Class Fonctions"
    Public Shared Sub CreateAndroidModel(ByVal name As String)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim path As String = "c:\edou\" & nomClasse & ".java"
        Dim ListofIndex As New List(Of String)
        Dim ListofIndexType As New List(Of String)
        Dim index_li_type As New Hashtable
        Dim countindex As Long = 0
        Dim insertstring As String = ""
        Dim updatestring As String = ""
        Dim header As String = "'''Generate By Edou Application *******" & Chr(13) _
                               & "''' Class " + nomClasse & Chr(13) & Chr(13)
        header = ""
        Dim content As String = "public class " & nomClasse & " {" & Chr(13)

        _end = "}" & Chr(13)
        ' Delete the file if it exists.
        If File.Exists(path) Then
            File.Delete(path)
        End If
        ' Create the file.
        Dim fs As FileStream = File.Create(path, 1024)
        fs.Close()
        Dim objWriter As New System.IO.StreamWriter(path, True)

        objWriter.WriteLine("package ht.solutions.android.lampe.modele;")
        objWriter.WriteLine("import com.google.gson.annotations.Expose;")
        objWriter.WriteLine("import com.google.gson.annotations.SerializedName;")
        objWriter.WriteLine()
        objWriter.WriteLine("import java.util.Date;")
        objWriter.WriteLine(content)
        objWriter.WriteLine()


        Dim ds As DataSet = LoadTableStructure_MySql(name)
        Dim cols As New List(Of String)
        Dim types As New List(Of String)
        Dim initialtypes As New List(Of String)
        Dim length As New List(Of String)
        Dim count As Integer = 0
        Dim cap As Integer = ds.Tables(1).Rows.Count

        For Each dt As DataRow In ds.Tables(2).Rows
            Id_table = dt(0).ToString()
        Next

        For Each dt As DataRow In ds.Tables(5).Rows
            If dt(2).ToString <> Id_table Then
                ListofIndex.Insert(countindex, dt(2).ToString)
                countindex = countindex + 1
            End If
        Next


        For Each dt As DataRow In ds.Tables(6).Rows
            If dt(0).ToString = "FOREIGN KEY" Then
                ListofForeignKey.Add(dt(6).ToString)
                countForeignKey = countForeignKey + 1
            End If
        Next

        For Each dt As DataRow In ds.Tables(1).Rows
            For Each index In ListofIndex
                If dt(0).ToString = index Then
                    ListofIndexType.Add(dt(1))
                    index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                End If
            Next
        Next

        For Each dt As DataRow In ds.Tables(1).Rows
            If count < cap - 4 Then
                cols.Add("" & dt(0))
                initialtypes.Add(dt(1))
                types.Add(ConvertDBToJavaType((dt(1))))
                length.Add(dt(3))
                count += 1
            Else
                Exit For
            End If
        Next

        cols.Add("localId")
        cols.Add("isSync")
        types.Add("long")
        types.Add("boolean")
        initialtypes.Add("Byte")
        initialtypes.Add("nvarchar")
        objWriter.WriteLine("//region Attribut")
        'objWriter.WriteLine("Private _id As Long")
        objWriter.WriteLine()
        Try
            For i As Int32 = 1 To cols.Count - 1

                'If Not nottoputforlist.Contains(cols(i)) Then
                '    insertstring &= ", " & cols(i)
                '    updatestring &= ", " & cols(i)
                'End If
                Dim attrib As String = ""  'not used for now to be updated.
                objWriter.WriteLine("@SerializedName(""" & cols(i) & """)")
                objWriter.WriteLine("@Expose")
                objWriter.WriteLine("private " & types(i) & " " & cols(i) & ";")
                If initialtypes(i) = "image" Then
                    objWriter.WriteLine("@SerializedName(""" & cols(i) & "String" & """)")
                    objWriter.WriteLine("@Expose")
                    objWriter.WriteLine("private String " & cols(i) & "String;" & "")
                End If
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("private " & cols(i).Substring(3, cols(i).Length - 3) & " " & cols(i).Substring(3, cols(i).Length - 3).ToLower & ";")
                End If
            Next
        Catch ex As Exception

        End Try
        objWriter.WriteLine()
        objWriter.WriteLine("//endregion")
        objWriter.WriteLine()

        '''''''''''''''''''''''''''''''''''''''''properties''''''''''''''''''''''''''''''''''''

        objWriter.WriteLine("//region Properties")


        For i As Int32 = 1 To cols.Count - 2 ''On ne cree pas de property pour la derniere column
            Dim propName As String = ""
            Dim s As String() = cols(i).Split("_")
            For j As Integer = 1 To s.Length - 1
                propName &= StrConv(s(j), VbStrConv.ProperCase)
            Next

            Dim attrib As String = "public " & types(i) & " get" & cols(i) & "() {"
            Dim setter As String = "public void " & "set" & cols(i) & "(" & types(i) & " " & cols(i).ToLower() & ") {"

            If cols(i) <> "_isdirty" Or cols(i) <> "_LogData" Then
                '   objWriter.WriteLine(log)

                objWriter.WriteLine()
                objWriter.WriteLine(attrib)
                objWriter.WriteLine("return " & cols(i) & ";")
                objWriter.WriteLine("}")
                objWriter.WriteLine(setter)
                objWriter.WriteLine("this." & cols(i) & " = " & cols(i).ToLower & ";")
                objWriter.WriteLine("}")

                If initialtypes(i) = "image" Then
                    objWriter.WriteLine()
                    objWriter.WriteLine("public boolean hasImage() {")
                    objWriter.WriteLine("return (this." & cols(i) & "String != null && this." & cols(i) & "String.length() > 0 || this." & cols(i) & "!= null && this." & cols(i) & ".length > 0);")
                    objWriter.WriteLine("}")
                    objWriter.WriteLine("public String get" & cols(i) & "String() {")
                    objWriter.WriteLine("return " & cols(i).ToLower & "String;")
                    objWriter.WriteLine("}")

                    objWriter.WriteLine("public void set" & cols(i) & "String(String " & cols(i).ToLower & "String) {")
                    objWriter.WriteLine("this." & cols(i) & "String = " & cols(i).ToLower & "String;")
                    objWriter.WriteLine("}")


                End If

                If ListofForeignKey.Contains(cols(i)) Then

                    Dim ClassName As String = cols(i).Substring(3, cols(i).Length - 3)
                    Dim attributUsed As String = ClassName.ToLower()
                    '  objWriter.WriteLine("public cols(i).Substring(3, cols(i).Length - 3) & Chr(13))
                    objWriter.WriteLine("public " & ClassName & " get" & ClassName & "() {")
                    objWriter.WriteLine("if (" & attributUsed & "==null)")
                    objWriter.WriteLine(attributUsed & " = " & ClassName & "Helper.searchByID(" & cols(i) & ");")
                    objWriter.WriteLine("return " & attributUsed & ";")
                    objWriter.WriteLine("}")

                    objWriter.WriteLine("public void set" & ClassName & "(" & ClassName & " " & attributUsed & ") {")
                    objWriter.WriteLine("this." & ClassName & " = " & attributUsed & ";")
                    objWriter.WriteLine("}")

                End If
            End If

        Next

        objWriter.WriteLine("//endregion")

        objWriter.WriteLine()

        objWriter.WriteLine()
        objWriter.WriteLine(_end)
        objWriter.WriteLine()
        objWriter.Close()

    End Sub

    Public Shared Sub CreateAndroidHelper(ByVal name As String)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim path As String = "c:\edou\" & nomClasse & "Helper.java"
        Dim ListofIndex As New List(Of String)
        Dim ListofIndexType As New List(Of String)
        Dim index_li_type As New Hashtable
        Dim countindex As Long = 0
        Dim insertstring As String = ""
        Dim updatestring As String = ""
        Dim header As String = "'''Generate By Edou Application *******" & Chr(13) _
                               & "''' Class " + nomClasse & Chr(13) & Chr(13)
        header = ""
        Dim content As String = "public class " & nomClasse & "Helper {" & Chr(13)

        _end = "}" & Chr(13)
        ' Delete the file if it exists.
        If File.Exists(path) Then
            File.Delete(path)
        End If
        ' Create the file.
        Dim fs As FileStream = File.Create(path, 1024)
        fs.Close()
        Dim objWriter As New System.IO.StreamWriter(path, True)

        objWriter.WriteLine("package ht.solutions.android.lampe.modele;")
        objWriter.WriteLine("import com.google.gson.annotations.Expose;")
        objWriter.WriteLine("import com.google.gson.annotations.SerializedName;")
        objWriter.WriteLine()
        objWriter.WriteLine("import java.util.Date;")
        objWriter.WriteLine(content)
        objWriter.WriteLine()


        Dim ds As DataSet = LoadTableStructure_MySql(name)
        Dim cols As New List(Of String)
        Dim types As New List(Of String)
        Dim initialtypes As New List(Of String)
        Dim length As New List(Of String)
        Dim count As Integer = 0
        Dim cap As Integer = ds.Tables(1).Rows.Count

        For Each dt As DataRow In ds.Tables(2).Rows
            Id_table = dt(0).ToString()
        Next

        For Each dt As DataRow In ds.Tables(5).Rows
            If dt(2).ToString <> Id_table Then
                ListofIndex.Insert(countindex, dt(2).ToString)
                countindex = countindex + 1
            End If
        Next

        For Each dt As DataRow In ds.Tables(6).Rows
            If dt(0).ToString = "FOREIGN KEY" Then
                ListofForeignKey.Add(dt(6).ToString)
                countForeignKey = countForeignKey + 1
            End If
        Next
        For Each dt As DataRow In ds.Tables(1).Rows
            For Each index In ListofIndex
                If dt(0).ToString = index Then
                    ListofIndexType.Add(dt(1))
                    index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                End If
            Next
        Next
        For Each dt As DataRow In ds.Tables(1).Rows
            If count < cap - 4 Then
                cols.Add("" & dt(0))
                initialtypes.Add(dt(1))
                types.Add(ConvertDBToJavaType((dt(1))))
                length.Add(dt(3))
                count += 1
            Else
                Exit For
            End If
        Next
        cols.Add("localId")
        cols.Add("isSync")
        types.Add("long")
        types.Add("boolean")
        initialtypes.Add("Byte")
        initialtypes.Add("nvarchar")

        objWriter.WriteLine("public static int save(" & nomClasse & " " & nomClasse.ToLower & ") throws Exception {")

        With objWriter
            .WriteLine("LampeSolaireDB lampeSolaireDB = new LampeSolaireDB();")
            .WriteLine("lampeSolaireDB.open();")
            .WriteLine("SQLiteDatabase database = lampeSolaireDB.getDb();")
            .WriteLine("int result = 0;")
            .WriteLine("try {")
            .WriteLine("ContentValues newTaskValue = new ContentValues();")
        End With
        Try
            For i As Int32 = 1 To cols.Count - 1
                With objWriter
                    If initialtypes(i) <> "date" Then
                        .WriteLine("newTaskValue.put(DBConstants." & cols(i) & ", " & nomClasse.ToLower & "get" & cols(i) & "()")
                    Else
                        .WriteLine("newTaskValue.put(DBConstants.COMMANDE_DATE, commande.getDate_commande().getTime());")
                    End If
                End With
            Next
        Catch

        End Try

        With objWriter
            .WriteLine("return result;")
            .WriteLine("} finally {")
            .WriteLine("lampeSolaireDB.close();")
            .WriteLine("}")
            .WriteLine("}")
        End With

        With objWriter
            .WriteLine("private static Commande setCommande(Cursor c) {")
            .WriteLine(nomClasse & " " & nomClasse.ToLower & ";")
            .WriteLine("return commande;")
            .WriteLine("}")
        End With
        '    public static int save(Commande commande) throws Exception {

        '    LampeSolaireDB lampeSolaireDB = new LampeSolaireDB();
        '    lampeSolaireDB.open();
        '    SQLiteDatabase database = lampeSolaireDB.getDb();
        '    int result = 0;
        '    try {
        '        ContentValues newTaskValue = new ContentValues();
        '        newTaskValue.put(DBConstants.COMMANDE_ID, commande.getId());
        '        newTaskValue.put(DBConstants.COMMANDE_PAR, commande.getCommande_par());
        '        newTaskValue.put(DBConstants.COMMANDE_DESCRIPTION, commande.getDescription());
        '        newTaskValue.put(DBConstants.COMMANDE_POSTE, commande.getPoste());
        '        newTaskValue.put(DBConstants.COMMANDE_DATE, commande.getDate_commande().getTime());
        '        newTaskValue.put(DBConstants.COMMANDE_LIMITE, commande.getDate_limite().getTime());
        '        newTaskValue.put(DBConstants.DATE_NAME,
        '                System.currentTimeMillis());
        '        if (searchById(commande.getId())==null)
        '        {
        '            result = (database.insert(DBConstants.COMMANDE_TABLE, null, newTaskValue) > 0 ? 1: 0);
        '        }  else{
        '            database.update(DBConstants.COMMANDE_TABLE, newTaskValue, DBConstants.COMMANDE_ID + "=?", new String[]{String.valueOf(commande.getId())});
        '        }
        '        return result;
        '    } finally {
        '        lampeSolaireDB.close();
        '    }
        '    //return result;
        '}
        '    private static Commande setCommande(Cursor c) {
        '    Commande commande;
        '    commande = new Commande();
        '    commande.setId(c.getLong(c.getColumnIndex(DBConstants.COMMANDE_ID)));
        '    commande.setCommande_par(c.getString(c.getColumnIndex(DBConstants.COMMANDE_PAR)));
        '    commande.setDescription(c.getString(c.getColumnIndex(DBConstants.COMMANDE_DESCRIPTION)));
        '    commande.setPoste(c.getString(c.getColumnIndex(DBConstants.COMMANDE_POSTE)));
        '    commande.setDate_commande(new Date(c.getLong(c.getColumnIndex(DBConstants.COMMANDE_DATE))));
        '    commande.setDate_limite(new Date(c.getLong(c.getColumnIndex(DBConstants.COMMANDE_LIMITE))));
        '    return commande;
        '}

        'public static ArrayList<Commande> list( ) {
        '    ArrayList<Commande> commandes= new ArrayList<Commande>();
        '    Commande commande = null;
        '    LampeSolaireDB lampeSolaireDB = new LampeSolaireDB();
        '    lampeSolaireDB.open();
        '    SQLiteDatabase database = lampeSolaireDB.getDb();
        '    Cursor c = database.query(DBConstants.COMMANDE_TABLE, null, null,null, null, null, null);
        '    while (c.moveToNext()) {
        '        commande = setCommande(c);
        '        commandes.add(commande);
        '    }
        '    c.close() ;
        '    lampeSolaireDB.close();
        '    return commandes;
        '}

        'public static Commande searchById(Long id ) {
        '    LampeSolaireDB lampeSolaireDB = new LampeSolaireDB();
        '    lampeSolaireDB.open();
        '    SQLiteDatabase database = lampeSolaireDB.getDb();
        '    String filter = DBConstants.COMMANDE_ID + " =?";
        '    Cursor c = database.query(DBConstants.COMMANDE_TABLE, null, filter, new String[] {String.valueOf(id)}, null, null, null);
        '    Commande commande=null;
        '    if (c.moveToNext()){
        '         commande = setCommande(c);
        '    }
        '    c.close() ;
        '    lampeSolaireDB.close();
        '    return commande;
        '}

    End Sub
#End Region

#Region "Php Fonctions"
    Public Shared Sub CreatePHPClass(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim nomUpperClasse As String = nomClasse.Substring(0, 1).ToUpper() & nomClasse.Substring(1, nomClasse.Length - 1)
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\", Application.StartupPath & "\SCRIPT\GENERIC_12\")
        Dim path As String = txt_PathGenerate_Script & "Cls_" & nomUpperClasse & ".class.php"
        Dim ListofIndex As New List(Of String)
        Dim ListofIndexType As New List(Of String)
        Dim index_li_type As New Hashtable
        Dim countindex As Long = 0
        Dim insertstring As String = ""
        Dim updatestring As String = ""


        Dim header As String = "'''Generate By Edou Application *******" & Chr(13) _
                               & "''' Class " + nomUpperClasse & Chr(13) & Chr(13)
        header = ""
        Dim content As String = "class Cls_" & nomUpperClasse & " {" & Chr(13)

        _end = "}" & Chr(13)
        ' Delete the file if it exists.
        If File.Exists(path) Then
            File.Delete(path)
        End If
        ' Create the file.
        Dim fs As FileStream = File.Create(path, 1024)
        fs.Close()
        Dim objWriter As New System.IO.StreamWriter(path, True)

        objWriter.WriteLine("<?php require_once('../Query/DAL/MySQL_Helper.php'); ?>")
        objWriter.WriteLine("<?php")
        objWriter.WriteLine(content)
        objWriter.WriteLine()


        Dim ds As DataSet = LoadTableStructure_MySql(name)
        Dim cols As New List(Of String)
        Dim types As New List(Of String)
        Dim initialtypes As New List(Of String)
        Dim length As New List(Of String)
        Dim count As Integer = 0
        Dim cap As Integer = ds.Tables(0).Rows.Count

        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(3).ToString = "PRI" Then
                Id_table = dt(0).ToString()
            End If

        Next

        For Each dt As DataRow In ds.Tables(0).Rows
            If count < cap - 4 Then
                cols.Add("" & dt(0))
                initialtypes.Add(dt(1))
                If dt(1).ToString.Contains("(") Then
                    Dim arrstring As String() = dt(1).ToString.Split("(")
                    types.Add(arrstring(0).ToString)
                Else
                    types.Add(ConvertDBToJavaType((dt(1))))
                End If
                length.Add(dt(3))
                count += 1
            Else
                Exit For
            End If
        Next
        objWriter.WriteLine(" #Region "" Attribut """)
        'objWriter.WriteLine("Private _id As Long")
        objWriter.WriteLine()
        objWriter.WriteLine(" private  $_id ; ")
        Try
            For i As Int32 = 1 To cols.Count - 1

                'If Not nottoputforlist.Contains(cols(i)) Then
                '    insertstring &= ", " & cols(i)
                '    updatestring &= ", " & cols(i)
                'End If
                Dim attrib As String = ""  'not used for now to be updated.

                objWriter.WriteLine("private $_" & cols(i) & ";")
                If initialtypes(i) = "image" Then

                    objWriter.WriteLine("private String " & cols(i) & "String;" & "")
                End If
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("private " & cols(i).Substring(3, cols(i).Length - 3) & " " & cols(i).Substring(3, cols(i).Length - 3).ToLower & ";")
                End If
            Next
        Catch ex As Exception

        End Try
        objWriter.WriteLine(" private  $isdirty = false; ")
        objWriter.WriteLine()
        objWriter.WriteLine("#EndRegion "" Attribut """)
        objWriter.WriteLine()

        objWriter.WriteLine("#Region ""Constructeur""")

        objWriter.WriteLine("public function Cls_" & nomUpperClasse & "($id=0)")
        objWriter.WriteLine("{")
        objWriter.WriteLine("if($id == 0){")
        objWriter.WriteLine("	$this->BlankProperties();")
        objWriter.WriteLine("}else{")
        objWriter.WriteLine("	$this->Read($id);")
        objWriter.WriteLine("}")
        objWriter.WriteLine("}//")

        objWriter.WriteLine("#EndRegion ""Constructeur""")

        objWriter.WriteLine("#Region ""Proprietes""")

        Try
            For i As Int32 = 1 To cols.Count - 1

                'If Not nottoputforlist.Contains(cols(i)) Then
                '    insertstring &= ", " & cols(i)
                '    updatestring &= ", " & cols(i)
                'End If
                Dim attrib As String = ""  'not used for now to be updated.

                objWriter.WriteLine("public function get_" & cols(i) & "(){")
                objWriter.WriteLine("return $this->_" & cols(i) & ";")
                objWriter.WriteLine("}")

                objWriter.WriteLine("public function set_" & cols(i) & "($value) {")
                objWriter.WriteLine("	if ($this->_" & cols(i) & " <> $value ){")
                objWriter.WriteLine("$this->_" & cols(i) & " = $value;")
                objWriter.WriteLine("}")
                objWriter.WriteLine("}")
                If initialtypes(i) = "image" Then

                    objWriter.WriteLine("private String " & cols(i) & "String;" & "")
                End If
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("private " & cols(i).Substring(3, cols(i).Length - 3) & " " & cols(i).Substring(3, cols(i).Length - 3).ToLower & ";")
                End If
            Next
        Catch ex As Exception

        End Try
        objWriter.WriteLine()
        objWriter.WriteLine("#EndRegion ""Proprietes""")
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''acces base  de donnee ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        objWriter.WriteLine()
        objWriter.WriteLine("#Region ""Access BASE DE DONNEE""")


        objWriter.WriteLine("public function Insert($User)" & Chr(13) & _
                             "{" & Chr(13) & _
                             "try{ " & Chr(13) & _
                             "$result = cls_MySQLi::ExecuteProcedure (""SP_Insert_" & nomUpperClasse & """)")
        Try
            For i As Int32 = 1 To cols.Count - 1
                objWriter.WriteLine(", $this->_" & cols(i))
                If initialtypes(i) = "image" Then
                    objWriter.WriteLine("private String " & cols(i) & "String;" & "")
                End If
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("private " & cols(i).Substring(3, cols(i).Length - 3) & " " & cols(i).Substring(3, cols(i).Length - 3).ToLower & ";")
                End If
            Next
        Catch ex As Exception
        End Try

        objWriter.WriteLine(", $User );")
        objWriter.WriteLine("  	return $this->_id = (int) $result[0][""ID""]; " & Chr(13) & _
                            "}catch(Exception $e){" & Chr(13) & _
                            "throw $e->getMessage();" & Chr(13) & _
                            "}" & Chr(13) & _
                            "}//")


        objWriter.WriteLine()

        objWriter.WriteLine("public function Update($User)" & Chr(13) & _
                             "{" & Chr(13) & _
                             "try{ " & Chr(13) & _
                             "$result = cls_MySQLi::ExecuteProcedure (""SP_Update_" & nomUpperClasse & """)")

        objWriter.WriteLine(", $this->_id")

        Try
            For i As Int32 = 1 To cols.Count - 1
                objWriter.WriteLine(", $this->_" & cols(i))
            Next
        Catch ex As Exception
        End Try

        objWriter.WriteLine("return $result;" & Chr(13) & _
                            " }catch(Exception $e){" & Chr(13) & _
                              " throw $e->getMessage();" & Chr(13) & _
                            "}" & Chr(13) & _
                            "}")

        objWriter.WriteLine()



        objWriter.WriteLine("public function Read($id)" & Chr(13) & _
                                 "{" & Chr(13) & _
                                 "	if($id <> 0 ){" & Chr(13) & _
                                 "		$result = cls_MySQLi::ExecuteProcedure(""SP_Select_" & nomUpperClasse & "_ByID"", $id);" & Chr(13) & _
                                 "		if((int)(count($result[0])) < 0 )" & Chr(13) & _
                                 "		{ " & Chr(13) & _
                                 "			$this->BlankProperties();" & Chr(13) & _
                                 "		}else{" & Chr(13) & _
                                 "			$this->SetProperties($result[0]);" & Chr(13) & _
                                 "		}" & Chr(13) & _
                                 "	}else{" & Chr(13) & _
                                 "		$this->BlankProperties();" & Chr(13) & _
                                 "	}" & Chr(13) & _
                                 "}")
        objWriter.WriteLine()
        objWriter.WriteLine(" public function SearchAll()" & Chr(13) & _
                                 "{" & Chr(13) & _
                                  "$result = cls_MySQLi::ExecuteProcedure(""SP_ListAll_" & nomUpperClasse & """ );" & Chr(13) & _
                                  "$obj = new Cls_ " & nomUpperClasse & ";" & Chr(13) & _
                                  "for($i=0; $i<count($result); $i++)" & Chr(13) & _
                                  "{" & Chr(13) & _
                                   "$obj->SetProperties($result);	" & Chr(13) & _
                                  "}" & Chr(13) & _
                                  "return $result;" & Chr(13) & _
                                 "}"
                            )
        objWriter.WriteLine()
        objWriter.WriteLine("public function Delete()" & Chr(13) & _
                                 "{" & Chr(13) & _
                                  "$result = cls_MySQLi::ExecuteProcedure(""SP_Delete_" & nomUpperClasse & """, $this->_id);" & Chr(13) & _
                                  "return $result;" & Chr(13) & _
                                 "}"
                            )

        objWriter.WriteLine()

        objWriter.WriteLine("	public function Save($User) " &
                                  "{" & Chr(13) & _
                                  "if($this->isdirty)" & Chr(13) & _
                                  "{" & Chr(13) & _
                                  "	Cls_" & nomUpperClasse & "::Validation();" & Chr(13) & _
                                  "	if($this->_id == 0)" & Chr(13) & _
                                  "	{" & Chr(13) & _
                                  "		$this->Insert($User);" & Chr(13) & _
                                  "	}else{ " & Chr(13) & _
                                  "		if($this->_id > 0 )" & Chr(13) & _
                                  "		{" & Chr(13) & _
                                  "			$this->Update($User);" & Chr(13) & _
                                  "		}else{ " & Chr(13) & _
                                  "			$this->_id = 0; " & Chr(13) & _
                                  "			//return false;	" & Chr(13) & _
                                  "		}" & Chr(13) & _
                                  "	}" & Chr(13) & _
                                  "}" & Chr(13) & _
                                  "$this->isdirty = false;" & Chr(13) & _
                                 "}")

        objWriter.WriteLine()
        objWriter.WriteLine("public function BlankProperties() " & Chr(13) & _
                            "{" & Chr(13) & _
                            "$this->_id = 0;"
                            )
        Try
            For i As Int32 = 1 To cols.Count - 1
                objWriter.WriteLine(", $this->_" & cols(i) & "= '';")
            Next
        Catch ex As Exception
        End Try
        objWriter.WriteLine("$this->isdirty = false;")
        objWriter.WriteLine("}")

        objWriter.WriteLine()

        objWriter.WriteLine("	public function SetProperties($rs)" & Chr(13) & _
                            "{" & Chr(13) & _
                            "$this->_id = $rs['ID'];"
                            )
        Try
            For i As Int32 = 1 To cols.Count - 1
                objWriter.WriteLine("$this->_" & cols(i) & "= &rs['" & cols(i) & "'];")
            Next
        Catch ex As Exception
        End Try

        objWriter.WriteLine("}")
        objWriter.WriteLine()
        objWriter.WriteLine("function load($array)" & Chr(13) & _
                             "{" & Chr(13) & _
                              "if(is_array($array))" & Chr(13) & _
                              "{" & Chr(13) & _
                               "foreach($array as $key=>$value)" & Chr(13) & _
                               "{" & Chr(13) & _
                               "	$this->vars[$key] = $value;" & Chr(13) & _
                               "}" & Chr(13) & _
                              "}" & Chr(13) & _
                             "}"
                             )

        objWriter.WriteLine("#EndRegion ""Access BASE DE DONNEE""")
        objWriter.WriteLine()

        objWriter.WriteLine("#Region ""Other """)
        objWriter.WriteLine()
        objWriter.WriteLine("#EndRegion ""Other """)

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''end''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        objWriter.WriteLine()
        objWriter.WriteLine(_end)
        objWriter.WriteLine("?>")
        objWriter.Close()

    End Sub
#End Region



End Class
