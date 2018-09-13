Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.ComponentModel

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
        Dim ConString As String = _
            "Persist Security Info=True;" & _
            "server=" & servername & ";" & _
            "database=" & database & ";" & _
            "User Id=" & user_login & ";" & _
            "password=" & password & ";"
        Try
            Dim Con As New MySqlConnection(ConString)
            Con.Open()
            Dim cmd As New MySqlCommand
            cmd.CommandText = "DESCRIBE " & table
            cmd.CommandType = CommandType.Text
            cmd.Connection = Con
            Dim p As New MySqlParameter
            p.Value = table
            Dim ds As New DataSet
            Dim da As MySqlDataAdapter
            da = New MySqlDataAdapter(cmd)
            da.Fill(ds)
            cmd.Parameters.Clear()
            Con.Close()
            Return ds
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Load Table Structure MySql", MessageBoxButtons.OK)
            '  Error_Log("LoadTableStructure", ex.Message)
        End Try
    End Function

    Public Shared Function LoadUserTablesSchema_MySql( _
            ByVal strServer As String, _
            ByVal strUser As String, _
            ByVal strPwd As String, _
            ByVal strDatabase As String, ByRef treeview1 As TreeView) As ArrayList

        Dim slTables As ArrayList = New ArrayList()

        Dim cnString As String = "Persist Security Info=True;" & _
                    "server=" & strServer & ";" & _
                    "User Id=" & strUser & ";" & _
                    "database=" & strDatabase & ";" & _
                    "password=" & strPwd & ""

        Dim strQUERY As String = "SHOW TABLES FROM " & strDatabase & ""
        'Dim strQUERY As String = "SELECT DISTINCT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_SCHEMA =" & strDatabase & ""

        Dim table As DataTable = Nothing
        Dim con As MySqlConnection = New MySqlConnection(cnString)
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
            For Each dr2 In table.Rows
                treeview1.Nodes.Add(dr2("Tables_in_" & strDatabase))
            Next
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            _systeme.CreateConnectionLog(strServer, strUser, strPwd, TypeDatabase.MYSQL)
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

    Public Shared Function CreateStoreMySql(ByVal Name As String) As String

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
                If count < cap - 4 Then

                    If (paramStore = "") Then
                        paramStore = "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                        champStore = "" & dt(0) & ""
                        valueStore = "_" & dt(0)
                    Else
                        paramStore &= Chr(13) & Chr(9) & Chr(9) & "," & "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))
                        champStore &= Chr(13) & Chr(9) & Chr(9) & "," & "" & dt(0) & ""
                        valueStore &= Chr(13) & Chr(9) & Chr(9) & "," & "_" & dt(0)
                    End If
                    count += 1
                Else
                    Exit For
                End If
            End If

        Next
        ' paramStore &= Chr(13) & Chr(9) & Chr(9) & "," & "@user" & " nvarchar(50)"
        '  champStore &= Chr(13) & Chr(9) & Chr(9) & "," & "createdby"
        champStore &= Chr(13) & Chr(9) & Chr(9) & "," & "datecreated"
        ' valueStore &= Chr(13) & Chr(9) & Chr(9) & "," & "@user"
        valueStore &= Chr(13) & Chr(9) & Chr(9) & "," & "NOW()"
        Dim command As String = Chr(9) & "INSERT INTO " & Name & Chr(13) _
                                & Chr(9) & Chr(9) & "(" & Chr(13) _
                                & Chr(9) & Chr(9) & champStore & Chr(13) _
                                & Chr(9) & ")" & Chr(13) _
                                & Chr(9) & "VALUES" & Chr(13) _
                                & Chr(9) & "(" & Chr(13) _
                                & Chr(9) & Chr(9) & valueStore & Chr(13) _
                                & Chr(9) & ");"
        '' "USE MCI_db" & Chr(13) & "" & Chr(13) _&
        ''/****** Object:  StoredProcedure [dbo].[SP_AddProfession]    Script Date: 08/12/2012 15:42:54 ******/
        ''SET ANSI_NULLS ON
        ''GO
        ''SET QUOTED_IDENTIFIER ON
        ''GO
        '"USE MCI_db" & Chr(13) & "GO" & Chr(13) _
        '                            & "/****** Object: StoredProcedure [dbo].[SP_Add" & objectname & "]    " & "Script Date: " & Now.Date & " " & Now.Date.TimeOfDay.ToString & " ******/" & Chr(13) _
        '                            & "SET ANSI_NULLS ON" & Chr(13) _
        '                            & "GO" & Chr(13) _
        '                            & "SET QUOTED_IDENTIFIER ON" & Chr(13) _
        '                            & "GO" & Chr(13) & Chr(13) _
        '                            & 

        Dim objectname As String = Name.Substring(4, Name.Length - 4)
        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)
        Dim store As String = "DELIMITER $$" & Chr(13) _
                            & "CREATE PROCEDURE SP_Insert" & objectname & " " & Chr(13) _
                            & Chr(9) & "(" & Chr(13) _
                            & Chr(9) & Chr(9) & paramStore & Chr(13) _
                            & Chr(9) & ")" & Chr(13) _
                            & Chr(9) & "BEGIN " & Chr(13) _
                            & command & Chr(13) _
                            & Chr(9) & "SELECT LAST_INSERT_ID() AS ID;" & Chr(13) & Chr(13) _
                            & "END$$" & Chr(13) & Chr(13) _
                            & "DELIMITER ;" & Chr(13)

        Return store
    End Function

    Public Shared Function UpdateStoreMySql(ByVal Name As String) As String

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
            If count < cap - 4 Then
                If QuerySet = "" Then
                    If dt(0) <> Id_table Then
                        QuerySet = "" & dt(0) & "" & " " & "= " & "_" & dt(0)
                    End If
                Else
                    QuerySet &= Chr(13) & Chr(9) & Chr(9) & "," & "" & dt(0) & "" & " " & "= " & "_" & dt(0)
                End If
                If (paramStore = "") Then
                    paramStore = "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))

                Else
                    paramStore &= Chr(13) & Chr(9) & Chr(9) & "," & "IN _" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim), IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim), dt(1) & "(" & dt(3) & ")", dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"), dt(1))

                End If
                count += 1
            Else
                Exit For
            End If
        Next



        paramStore &= Chr(13) & Chr(9) & Chr(9) & "," & "IN _user" & " varchar(50)"
        QuerySet &= Chr(13) & Chr(9) & Chr(9) & "," & "Modifby " & "=" & "_user"
        QuerySet &= Chr(13) & Chr(9) & Chr(9) & "," & "DateModif" & "=" & "NOW()" & Chr(13)
        'valueStore &= Chr(13) & Chr(9) & Chr(9) & "," & "@user"
        'valueStore &= Chr(13) & Chr(9) & Chr(9) & "," & "GETDATE()"
        Dim command As String = Chr(9) & "UPDATE " & Name & Chr(13) _
                                & Chr(9) & "SET" & Chr(13) & Chr(13) _
                                & Chr(9) & Chr(9) & QuerySet & Chr(13)

        '' "USE MCI_db" & Chr(13) & "GO" & Chr(13) _&
        ''"/***Store Update For table  " & Name & "****/" & Chr(13) _ &
        '' "CREATE PROCEDURE [dbo].[SP_Update" & StrConv(Name, VbStrConv.ProperCase) & "] " & Chr(13) _
        Dim objectname As String = Name.Substring(4, Name.Length - 4)

        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)

        Dim store As String = "DELIMITER $$" & Chr(13) _
                          & "CREATE PROCEDURE SP_Update" & objectname & " " & Chr(13) _
                          & Chr(9) & "(" & Chr(13) _
                          & Chr(9) & Chr(9) & paramStore & Chr(13) _
                          & Chr(9) & ")" & Chr(13) _
                          & Chr(9) & "BEGIN " & Chr(13) _
                          & command & Chr(13) _
                          & Chr(9) & "Where " & Id_table & " = " & "_" & Id_table & " ;" & Chr(13) & Chr(13) _
                          & "END$$" & Chr(13) & Chr(13) _
                          & "DELIMITER ;" & Chr(13)
        Return store
    End Function

    Public Shared Function DeleteStoreMySql(ByVal Name As String) As String


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
        Dim store As String = "DELIMITER $$" & Chr(13) _
                            & "CREATE PROCEDURE SP_Delete" & objectname & " " & Chr(13) _
                            & Chr(9) & "(" & Chr(13) _
                            & Chr(9) & Chr(9) & "IN _ID " & Id_table_type & Chr(13) _
                            & Chr(9) & ")" & Chr(13) _
                            & "BEGIN" & Chr(13) _
                            & command & Chr(13) _
                            & Chr(9) & "WHERE " & Id_table & " = " & "_ID ;" & Chr(13) & Chr(13) _
                            & "END$$" & Chr(13) & Chr(13) _
                            & "DELIMITER ;" & Chr(13)



        Return store
    End Function

    Public Shared Function SelectStoreMySql(ByVal Name As String) As String

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

        Dim command As String = Chr(9) & "SELECT *" & Chr(13) _
                                & Chr(9) & "FROM " & Name

        Dim objectname As String = Name.Substring(4, Name.Length - 4)
        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)
        Dim store As String = "DELIMITER $$" & Chr(13) _
                            & "CREATE PROCEDURE SP_Select" & objectname & "_ByID " & Chr(13) & Chr(13) _
                            & Chr(9) & "(" & Chr(13) _
                            & Chr(9) & Chr(9) & "IN _ID " & Id_table_type & Chr(13) _
                            & Chr(9) & ")" & Chr(13) _
                            & "BEGIN" & Chr(13) _
                            & command & Chr(13) _
                            & Chr(9) & "WHERE " & Id_table & " = " & "_ID ;" & Chr(13) & Chr(13) _
                            & "END$$" & Chr(13) & Chr(13) _
                            & "DELIMITER ;" & Chr(13)
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
                                "CREATE PROCEDURE [dbo].[SP_Select" & objectname & "_" & ListofIndex.Item(indexPosition) & "] " & Chr(13) _
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

    Public Shared Function ListAllStoreMySql(ByVal Name As String) As String
        Dim ds As DataSet = LoadTableStructure_MySql(Name)

        Dim cap As Integer

        cap = ds.Tables(0).Rows.Count

        Dim count As Integer = 0

        Dim command As String = Chr(9) & "SELECT *" & Chr(13) _
                                & Chr(9) & "FROM " & Name & " ;"

        Dim objectname As String = Name.Substring(4, Name.Length - 4)
        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)
        Dim store As String = "DELIMITER $$" & Chr(13) _
                            & "CREATE PROCEDURE SP_ListAll_" & objectname & " ()" & Chr(13) & Chr(13) _
                            & "BEGIN" & Chr(13) _
                            & command & Chr(13) & Chr(13) _
                            & "END$$" & Chr(13) & Chr(13) _
                            & "DELIMITER ;" & Chr(13)

        Return store
    End Function

    Private Shared Function ListAllByForeignKeyMySql(ByVal Name As String) As String
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

        For Each dt As DataRow In ds.Tables(6).Rows
            If dt(0).ToString = "FOREIGN KEY" Then
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
            Dim command As String = Chr(9) & "SELECT *" & Chr(13) _
                                    & Chr(9) & "FROM " & Name

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

#Region "VB.Net Class Fonctions"
    Public Shared Sub CreateFile(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\", Application.StartupPath & "\SCRIPT\GENERIC_12\")
        Dim path As String = txt_PathGenerate_Script & nomClasse & ".vb"

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

        For Each _index As Cls_UniqueIndex In _table.ListofIndex
            If _index.ListofColumn.Count = 1 Then
                ListofIndexType.Add(_index.ListofColumn.Item(0).Type.VbName)
                index_li_type.Add(ListofIndex.IndexOf(_index.ListofColumn.Item(0).Name), _index.ListofColumn.Item(0).Type.VbName)
            Else

            End If
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
        objWriter.WriteLine("#Region ""Attribut""")
        objWriter.WriteLine("Private _id As Long")
        objWriter.WriteLine()
        Try
            For i As Int32 = 1 To cols.Count - 1
                If Not nottoputforlist.Contains(cols(i)) Then
                    insertstring &= ", " & cols(i)
                    updatestring &= ", " & cols(i)
                End If

                objWriter.WriteLine("Private " & cols(i) & " As " & types(i))
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("Private _" & cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1)) & " As " & "Cls_" & cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1)))
                End If
            Next
        Catch ex As Exception

        End Try
        objWriter.WriteLine()
        objWriter.WriteLine("#End Region")
        objWriter.WriteLine()

        objWriter.WriteLine("#Region ""New""")
        objWriter.WriteLine("Public Sub New()")
        objWriter.WriteLine("BlankProperties()")
        objWriter.WriteLine("End Sub")
        objWriter.WriteLine()
        objWriter.WriteLine("Public Sub New(ByVal _idOne As Long)")
        objWriter.WriteLine("Read(_idOne)")
        objWriter.WriteLine("End Sub" & Chr(13))


        For Each _index As Cls_UniqueIndex In _table.ListofIndex
            If _index.ListofColumn.Count > 1 Then
            Else
                objWriter.WriteLine("Public Sub New(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ")")
                objWriter.WriteLine("Read_" & _index.ListofColumn.Item(0).Name & "(" & _index.ListofColumn.Item(0).Name & ")")
                objWriter.WriteLine("End Sub " & Chr(13))
            End If
        Next


        objWriter.WriteLine("#End Region")

        objWriter.WriteLine()

        objWriter.WriteLine("#Region ""Properties""")
        objWriter.WriteLine("<AttributLogData(True,1)> _")
        objWriter.WriteLine("Public ReadOnly Property ID() As Long Implements IGeneral.ID")
        objWriter.WriteLine("Get")
        objWriter.WriteLine("Return _id")
        objWriter.WriteLine("End Get")
        objWriter.WriteLine("End Property")
        objWriter.WriteLine()

        For i As Int32 = 1 To cols.Count - 3 ''On ne cree pas de property pour la derniere column
            Dim propName As String = ""
            Dim s As String() = cols(i).Split("_")
            For j As Integer = 1 To s.Length - 1
                propName &= StrConv(s(j), VbStrConv.ProperCase)
            Next
            'propName = StrConv(cols(i).Split("_")(1), VbStrConv.ProperCase) & StrConv(cols(i).Split("_")(2), VbStrConv.ProperCase)
            Dim log As String = "<AttributLogData(True, " & i + 1 & ")> _"
            Dim attrib As String = "Public Property  " & propName & " As " & types(i)

            If cols(i) <> "_isdirty" Or cols(i) <> "_LogData" Then
                objWriter.WriteLine(log)
                objWriter.WriteLine(attrib)
                objWriter.WriteLine("Get" & Chr(13) _
                                    & " Return " & cols(i) & Chr(13) _
                                    & "End Get")

                If Lcasevalue.Contains(types(i)) Then
                    objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                  & " If LCase(Trim(" & cols(i) & ")) <> LCase(Trim(Value)) Then" & Chr(13) _
                                  & "_isdirty = True " & Chr(13) _
                                  & cols(i) & " = Trim(Value)" & Chr(13) _
                                  & "End If" & Chr(13) _
                                  & "End Set" & Chr(13) _
                                  & "End Property")
                Else
                    objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                  & " If " & cols(i) & " <> Value Then" & Chr(13) _
                                  & "_isdirty = True " & Chr(13) _
                                  & cols(i) & " = Value" & Chr(13) _
                                  & "End If" & Chr(13) _
                                  & "End Set" & Chr(13) _
                                  & "End Property")
                End If

                objWriter.WriteLine()

                If ListofForeignKey.Contains(cols(i)) Then
                    Dim attributUsed As String = cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1))
                    Dim ClassName As String = "Cls_" & cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1))
                    objWriter.WriteLine("Public Property " & attributUsed & " As " & ClassName & Chr(13))
                    objWriter.WriteLine("Get")
                    objWriter.WriteLine("If Not (_" & attributUsed & " Is Nothing) Then" & Chr(13) _
                                        & "If (_" & attributUsed & ".ID = 0) Or (_" & attributUsed & ".ID <>  " & cols(i) & ") Then" & Chr(13) _
                                        & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                        & "End If" & Chr(13) _
                                        & "Else" & Chr(13) _
                                        & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                        & "End If" & Chr(13) & Chr(13) _
                                        & "Return _" & attributUsed & Chr(13) _
                                        & "End Get" & Chr(13) _
                                        & "Set(ByVal value As " & ClassName & ")" & Chr(13) _
                                        & "If Value Is Nothing Then" & Chr(13) _
                                        & "_isdirty = True" & Chr(13) _
                                        & cols(i) & " = 0" & Chr(13) _
                                        & "Else" & Chr(13) _
                                        & "If _" & attributUsed & ".ID <> Value.ID Then" & Chr(13) _
                                        & "_isdirty = True" & Chr(13) _
                                        & cols(i) & " = Value.ID" & Chr(13) _
                                        & "End If" & Chr(13) _
                                        & "End If" & Chr(13) _
                                        & "End Set" & Chr(13) _
                                        & "End Property" & Chr(13) _
                                        )
                    objWriter.WriteLine()
                End If
            End If
            If initialtypes(i).ToString() = "image" Then
                objWriter.WriteLine("Public Property " & cols(i).Substring(1, cols(i).Length - 1) & "String() As String")
                objWriter.WriteLine("Get")
                objWriter.WriteLine("If " & cols(i) & " IsNot Nothing Then")
                objWriter.WriteLine("Return Encode(" & cols(i) & " )")
                objWriter.WriteLine("Else")
                objWriter.WriteLine("Return """"")
                objWriter.WriteLine("End If")
                objWriter.WriteLine("End Get")
                objWriter.WriteLine("Set(ByVal Value As String)")
                objWriter.WriteLine(cols(i) & " = Decode(Value)")
                objWriter.WriteLine("_isdirty = True")
                objWriter.WriteLine("End Set")
                objWriter.WriteLine("End Property")
            End If

        Next

        With objWriter
            .WriteLine("ReadOnly Property IsDataDirty() As Boolean")
            .WriteLine("Get")
            .WriteLine("Return _isdirty")
            .WriteLine("End Get")
            .WriteLine("End Property")
            .WriteLine()
        End With

        With objWriter
            .WriteLine("Public ReadOnly Property LogData() As String")
            .WriteLine("Get")
            .WriteLine("Return _LogData")
            .WriteLine("End Get")
            .WriteLine("End Property")
        End With


        objWriter.WriteLine("#End Region")

        objWriter.WriteLine()
        objWriter.WriteLine("#Region "" Db Access """)
        objWriter.WriteLine("Public Function Insert(ByVal usr As String) As Integer Implements IGeneral.Insert")
        objWriter.WriteLine("_LogData = """"")


        objWriter.WriteLine("_id = Convert.ToInt32(MySQLHelper.ExecuteScalar(MySQLHelperParameterCache.BuildConfigDB(), ""SP_Insert" & nomClasse.Substring(4, nomClasse.Length - 4) & """" & insertstring & ", usr))")


        objWriter.WriteLine("Return _id")
        objWriter.WriteLine("End Function")

        objWriter.WriteLine()
        objWriter.WriteLine("Public Function Update(ByVal usr As String) As Integer Implements IGeneral.Update")
        objWriter.WriteLine("_LogData = """"")
        objWriter.WriteLine("_LogData = GetObjectString()")
        objWriter.WriteLine("Return MySQLHelper.ExecuteScalar(MySQLHelperParameterCache.BuildConfigDB(), ""SP_Update" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id" & updatestring & ", usr)")
        objWriter.WriteLine("End Function" & Chr(13))

        objWriter.WriteLine("Private Sub SetProperties(ByVal dr As DataRow)" & Chr(13))
        objWriter.WriteLine("_id = TypeSafeConversion.NullSafeLong(dr(""" & cols(0).Substring(1, cols(0).Length - 1) & """))")

        For i As Int32 = 1 To cols.Count - 2

            If cols(i) <> "_isdirty" Then
                If types(i) = "DateTime" Then
                    objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafeDate(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                ElseIf initialtypes(i) = "image" Then
                    objWriter.WriteLine(cols(i) & " = " & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)")
                Else
                    objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafe" & types(i) & "(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                End If
            End If

            If ListofForeignKey.Contains(cols(i)) Then
                objWriter.WriteLine("_" & cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1)) & " = Nothing")

            End If

        Next

        objWriter.WriteLine()
        objWriter.WriteLine("End Sub" & Chr(13))

        objWriter.WriteLine("Private Sub BlankProperties()" & Chr(13))
        objWriter.WriteLine("_id = 0")
        For i As Int32 = 1 To cols.Count - 2

            If types(i) <> "Boolean" Then
                If types(i) = "DateTime" Or types(i) = "Date" Then
                    objWriter.WriteLine(cols(i) & " = " & "Now")
                Else
                    If Lcasevalue.Contains(types(i)) Then
                        objWriter.WriteLine(cols(i) & " = " & """""")
                    ElseIf initialtypes(i) = "image" Then
                        objWriter.WriteLine(cols(i) & " = Nothing")
                    Else
                        objWriter.WriteLine(cols(i) & " = " & "0")
                    End If

                End If
            Else
                objWriter.WriteLine(cols(i) & " = " & "False")
            End If

            If ListofForeignKey.Contains(cols(i)) Then
                objWriter.WriteLine("_" & cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1)) & " = Nothing")
            End If


        Next

        objWriter.WriteLine()
        objWriter.WriteLine("End Sub" & Chr(13))

        objWriter.WriteLine("Public Function Read(ByVal _idpass As Long) As Boolean Implements IGeneral.Read")
        objWriter.WriteLine("Try " & Chr(13))

        objWriter.WriteLine("If _idpass <> 0 Then " & Chr(13) _
                        & "Dim ds As DataSet = MySQLHelper.ExecuteDataset(MySQLHelperParameterCache.BuildConfigDB(),""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_ByID"", _idpass)" & Chr(13) & Chr(13) _
                        & "If ds.Tables(0).Rows.Count < 1 Then" & Chr(13) _
                        & "BlankProperties()" & Chr(13) _
                        & "Return False" & Chr(13) _
                        & "End If" & Chr(13) & Chr(13) _
                        & "SetProperties(ds.tables(0).rows(0))" & Chr(13) _
                        & "Else" & Chr(13) _
                        & "BlankProperties()" & Chr(13) _
                        & "End If" & Chr(13) _
                        & "Return True" & Chr(13) _
                        )

        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
        objWriter.WriteLine("Throw ex" & Chr(13))
        objWriter.WriteLine("End Try" & Chr(13))
        objWriter.WriteLine("End Function" & Chr(13))

        For Each _index As Cls_UniqueIndex In _table.ListofIndex
            If _index.ListofColumn.Count = 1 Then
                objWriter.WriteLine("Public Function Read_" & _index.ListofColumn.Item(0).Name & "(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean")
                objWriter.WriteLine("Try " & Chr(13))
                objWriter.WriteLine("If " & _index.ListofColumn.Item(0).Name & " <> """" Then ")
                objWriter.WriteLine("Dim ds as Dataset = MySQLHelper.ExecuteDataset(MySQLHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & """, " & _index.ListofColumn.Item(0).Name & ")" & Chr(13))
                objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                objWriter.WriteLine("BlankProperties()")
                objWriter.WriteLine("Return False")
                objWriter.WriteLine("End If" & Chr(13))
                objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                objWriter.WriteLine("Else")
                objWriter.WriteLine("BlankProperties()")
                objWriter.WriteLine("End If" & Chr(13))
                objWriter.WriteLine("Return True")
                objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                objWriter.WriteLine("Throw ex" & Chr(13))
                objWriter.WriteLine("End Try" & Chr(13))
                objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
            Else
                Dim _strOfIndexToUse As String = String.Empty
                Dim _strOfValueToUse As String = String.Empty
                Dim _strParameterToUse As String = String.Empty
                Dim ind As Integer = 0
                For Each _column As Cls_Column In _index.ListofColumn
                    If _strOfIndexToUse.Length = 0 Then
                        _strOfIndexToUse = _column.Name
                        _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                        _strParameterToUse = "_value" & ind
                    Else
                        _strOfIndexToUse += "_" & _column.Name
                        _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                        _strParameterToUse += ", value" & ind
                    End If
                    ind = ind + 1
                Next
                objWriter.WriteLine("Public Function Read_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean")
                objWriter.WriteLine("Try " & Chr(13))
    '        objWriter.WriteLine("If " & ListofIndex(i) & " <> """" Then ")
                objWriter.WriteLine("Dim ds As Data.DataSet = MySQLHelper.ExecuteDataset(MySQLHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")
                objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                objWriter.WriteLine("BlankProperties()")
                objWriter.WriteLine("Return False")
                objWriter.WriteLine("End If" & Chr(13))

                objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                objWriter.WriteLine("Else")
                objWriter.WriteLine("BlankProperties()")
    '        objWriter.WriteLine("End If" & Chr(13))

                objWriter.WriteLine("Return True")

                objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                objWriter.WriteLine("Throw ex" & Chr(13))
                objWriter.WriteLine("End Try" & Chr(13))
                objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
            End If
        Next

        objWriter.WriteLine("Public Sub Delete() Implements IGeneral.Delete" & Chr(13) _
                            & "Try" & Chr(13) _
                            & "MySQLHelper.ExecuteNonQuery(MySQLHelperParameterCache.BuildConfigDB(), ""SP_Delete" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id)" & Chr(13) & Chr(13) _
                            & "Catch ex As SqlClient.SqlException" & Chr(13) _
                            & "Throw New System.Exception(ex.ErrorCode)" & Chr(13) _
                            & "End Try" & Chr(13) _
                            & "End Sub" & Chr(13)
                            )

        objWriter.WriteLine("Public Function Refresh() As Boolean Implements IGeneral.Refresh" & Chr(13) _
                             & "If _id = 0 Then" & Chr(13) _
                             & "Return False" & Chr(13) _
                             & "Else" & Chr(13) _
                             & "Read(_id)" & Chr(13) _
                             & "Return True" & Chr(13) _
                             & "End If" & Chr(13) _
                             & "End Function" & Chr(13) _
                             )

        objWriter.WriteLine("Public Function Save(ByVal usr As String) As Integer Implements IGeneral.Save" & Chr(13) _
                             & "If _isdirty Then" & Chr(13) _
                             & "Validation()" & Chr(13) & Chr(13) _
                             & "If _id = 0 Then" & Chr(13) _
                             & "Return Insert(usr)" & Chr(13) _
                             & "Else" & Chr(13) _
                             & "If _id > 0 Then" & Chr(13) _
                             & "Return Update(usr)" & Chr(13) _
                             & "Else" & Chr(13) _
                             & "_id = 0" & Chr(13) _
                             & "Return False" & Chr(13) _
                             & "End If" & Chr(13) _
                             & "End If" & Chr(13) _
                             & "End If" & Chr(13) & Chr(13) _
                             & "_isdirty = False" & Chr(13) _
                             & "End Function"
                             )


        objWriter.WriteLine("#End Region")

        objWriter.WriteLine()
        objWriter.WriteLine("#Region "" Search """)

        objWriter.WriteLine("Public Function Search() As System.Collections.ICollection Implements IGeneral.Search" & Chr(13) _
                            & "Return SearchAll()" & Chr(13) _
                            & "End Function" & Chr(13)
                            )


        objWriter.WriteLine("Public Shared Function SearchAll() As List(Of " & nomClasse & ")" & Chr(13) _
                            & "Try " & Chr(13) _
                            & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                            & "Dim r As Data.DataRow" & Chr(13) _
                            & "Dim ds As Data.DataSet = MySQLHelper.ExecuteDataset(MySQLHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & """)" & Chr(13) _
                            & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                            & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                            & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                            & "objs.Add(obj)" & Chr(13) _
                            & "Next r" & Chr(13) _
                            & "Return objs" & Chr(13)
                            )


        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
        objWriter.WriteLine("Throw ex" & Chr(13))
        objWriter.WriteLine("End Try" & Chr(13))
        objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
        objWriter.WriteLine("#End Region")

        objWriter.WriteLine()
        objWriter.WriteLine("#Region "" Other Methods """)


        For Each _index As Cls_UniqueIndex In _table.ListofIndex
            If _index.ListofColumn.Count = 1 Then
                objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(ByVal _value As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean ")
                objWriter.WriteLine("Dim ds As Data.DataSet = MySQLHelper.ExecuteDataset(MySQLHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _index.ListofColumn.Item(0).Name & """, _value)")
                objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                objWriter.WriteLine(" Return False")
                objWriter.WriteLine("Else")
                objWriter.WriteLine("If _id = 0 Then")
                objWriter.WriteLine("Return True")
                objWriter.WriteLine("Else")
                objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                objWriter.WriteLine("Return True")
                objWriter.WriteLine("Else")
                objWriter.WriteLine("Return False")
                objWriter.WriteLine("End If")
                objWriter.WriteLine("End If")
                objWriter.WriteLine("End If")
                objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
            Else
                Dim _strOfIndexToUse As String = String.Empty
                Dim _strOfValueToUse As String = String.Empty
                Dim _strParameterToUse As String = String.Empty
                Dim ind As Integer = 0
                For Each _column In _index.ListofColumn
                    If _strOfIndexToUse.Length = 0 Then
                        _strOfIndexToUse = _column.Name
                        _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                        _strParameterToUse = "_value" & ind
                    Else
                        _strOfIndexToUse += "_" & _column.Name
                        _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                        _strParameterToUse += ", value" & ind
                    End If
                    ind = ind + 1
                Next
                objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean ")
                objWriter.WriteLine("Try" & Chr(13))
                objWriter.WriteLine("Dim ds As Data.DataSet = MySQLHelper.ExecuteDataset(MySQLHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")

                objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                objWriter.WriteLine(" Return False")
                objWriter.WriteLine("Else")
                objWriter.WriteLine("If _id = 0 Then")
                objWriter.WriteLine("Return True")
                objWriter.WriteLine("Else")
                objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                objWriter.WriteLine("Return True")
                objWriter.WriteLine("Else")
                objWriter.WriteLine("Return False")
                objWriter.WriteLine("End If")
                objWriter.WriteLine("End If")
                objWriter.WriteLine("End If" & Chr(13))
                objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                objWriter.WriteLine("Throw ex" & Chr(13))
                objWriter.WriteLine("End Try" & Chr(13))
                objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
            End If
        Next


        objWriter.WriteLine("Private Sub Validation() " & Chr(13))
        Dim stringlistforvalidation As New List(Of String) From {"String"}
        Dim decimalintegerforvalidation As New List(Of String) From {"Integer", "Long", "Decimal"}



        For i As Int32 = 1 To cols.Count - 2
            If stringlistforvalidation.Contains(types(i)) Then
                objWriter.WriteLine("If " & cols(i) & " = """" Then " & Chr(13) _
                                    & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                    & "End If"
                                    )
                objWriter.WriteLine()
                objWriter.WriteLine("If Len(" & cols(i) & ") > " & length(i) & " Then" & Chr(13) _
                                    & "Throw (New System.Exception("" " & "Trop de caractères insérés pour" & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & "  (la longueur doit être inférieure a " & length(i) & " caractères.  )""))" & Chr(13) _
                                    & "End If"
                                    )
                objWriter.WriteLine()
            End If

            If decimalintegerforvalidation.Contains(types(i)) Then
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                  & "Throw (New System.Exception("" " & cols(i).Substring(4, cols(i).Length - 4).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                  & "End If"
                                  )
                    objWriter.WriteLine()
                Else
                    objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                   & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire.""))" & Chr(13) _
                                   & "End If"
                                   )
                    objWriter.WriteLine()
                End If
            End If


        Next

        objWriter.WriteLine()

        For Each _index As Cls_UniqueIndex In _table.ListofIndex
            If _index.ListofColumn.Count = 1 Then
                objWriter.WriteLine("If  FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(" & _index.ListofColumn.Item(0).Type.VbName & ") Then" & Chr(13) _
                                  & "Throw (New System.Exception(""Ce " & _index.ListofColumn.Item(0).Name & " est déjà enregistré.""))" & Chr(13) _
                                  & "End If"
                                 )
                objWriter.WriteLine()
            Else
                Dim _strOfIndexToUse As String = String.Empty
                Dim _strOfValueToUse As String = String.Empty
                Dim _strParameterToUse As String = String.Empty
                Dim _parametervalueToUse As String = String.Empty
                Dim ind As Integer = 0
                For Each _column In _index.ListofColumn
                    If _strOfIndexToUse.Length = 0 Then
                        _strOfIndexToUse = _column.Name
                        _parametervalueToUse = _column.Name
                        _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                        _strParameterToUse = "_value" & ind
                    Else
                        _strOfIndexToUse += "_" & _column.Name
                        _parametervalueToUse += ", " & _column.Name
                        _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                        _strParameterToUse += ", value" & ind
                    End If
                Next
                objWriter.WriteLine("If FoundAlreadyExit_" & _strOfIndexToUse & "(" & _parametervalueToUse & ") Then" & Chr(13) _
                                   & "Throw (New System.Exception(""Cette combinaison " & _strOfIndexToUse & " est déjà enregistrée.""))" & Chr(13) _
                                   & "End If"
                                   )
                objWriter.WriteLine()
            End If
        Next

        objWriter.WriteLine("End Sub" & Chr(13))

        objWriter.WriteLine("Public Function Encode(ByVal str As Byte()) As String")
        objWriter.WriteLine("Return Convert.ToBase64String(str)")
        objWriter.WriteLine("End Function")
        objWriter.WriteLine()

        objWriter.WriteLine("Public Function Decode(ByVal str As String) As Byte()")
        objWriter.WriteLine("Dim decbuff As Byte() = Convert.FromBase64String(str)")
        objWriter.WriteLine("Return decbuff")
        objWriter.WriteLine("End Function")
        objWriter.WriteLine()

        objWriter.WriteLine(" Public Function GetObjectString() As String Implements IGeneral.GetObjectString" & Chr(13) _
                         & "Dim _old As New " & nomClasse & "(Me.ID)" & Chr(13) & Chr(13) _
                         & "Return LogStringBuilder.BuildLogStringChangesOnly(_old,Me)" & Chr(13) _
                         & "End Function" & Chr(13) _
                         )
        objWriter.WriteLine("#End Region")
        objWriter.WriteLine()
        objWriter.WriteLine(_end)
        objWriter.WriteLine()
        objWriter.Close()
    End Sub
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
