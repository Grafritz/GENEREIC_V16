Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.ComponentModel

Public Class MySqlManager

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

    Public Shared Function LoadTableStructure(ByVal table As String) As DataSet
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

            cmd.CommandText = " select DISTINCT SC.Column_name , SC.data_type, " &
                                  "SC.CHARACTER_MAXIMUM_LENGTH," &
                                  "SC.numeric_precision," &
                                  "SC.numeric_scale," &
                                  "SC.iS_NULLABLE," &
                                  "TC.CONSTRAINT_TYPE," &
                                  "TC.CONSTRAINT_NAME ," &
                                  "KC.TABLE_NAME," &
                                  "KC.Column_name" &
                              " from information_schema.columns SC" &
                              " LEFT OUTER JOIN " &
                              "INFORMATION_SCHEMA.KEY_COLUMN_USAGE  KC on SC.Column_name = KC.COLUMN_NAME and SC.TABLE_NAME = KC.TABLE_NAME " &
                              "LEFT OUTER JOIN  " &
                              "INFORMATION_SCHEMA.TABLE_CONSTRAINTS  TC on KC.CONSTRAINT_NAME = TC.CONSTRAINT_NAME" &
                              " where 1 = 1 " &
                              " and SC.table_name = '" & table & "'" &
                              " and SC.TABLE_SCHEMA = '" & database & "'"


            'cmd.CommandText = "SELECT Column_name ," &
            '"DATA_TYPE," &
            '"CHARACTER_MAXIMUM_LENGTH," &
            '"NUMERIC_PRECISION," &
            '"NUMERIC_SCALE," &
            '"COLUMN_KEY," &
            '"IS_NULLABLE," &
            '"COLUMN_DEFAULT " &
            '"FROM INFORMATION_SCHEMA.COLUMNS " &
            '"WHERE TABLE_SCHEMA = '" & database & "'" &
            '" and TABLE_NAME = '" & table & "'"
            cmd.CommandType = CommandType.Text
            cmd.Connection = Con
            'Dim p As New MySqlParameter
            'p.Value = table
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

    Public Shared Function LoadUserTablesSchema(ByRef treeview1 As TreeView) As ArrayList

        Dim slTables As ArrayList = New ArrayList()

        Dim cnString As String = "Persist Security Info=True;" & _
                    "server=" & servername & ";" & _
                    "User Id=" & user_login & ";" & _
                    "database=" & database & ";" & _
                    "password=" & password & ""

        Dim strQUERY As String = "SHOW TABLES FROM " & database & ""
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
            Schema = ds2.Tables(0)
            For Each dr2 In Schema.Rows
                treeview1.Nodes.Add(dr2("Tables_in_" & database))
            Next
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            _systeme.CreateConnectionLog(servername, user_login, password, TypeDatabase.MYSQL)
            con.Close()
        Catch x As OleDbException
            slTables = Nothing
        End Try
        Return slTables

    End Function

    Public Shared Function InitializeDb() As Long
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Try
            _systeme.CreateLocalDatabase(database, TypeDatabase.MYSQL)
        Catch x As OleDbException
        End Try
        Return _systeme.currentDatabase.ID
    End Function

    Public Shared Function InitializeLocalColumn(ByVal iddatabase As Long, ByRef background As BackgroundWorker)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim duree As TimeSpan
        Dim momentdebute As DateTime = Now
        _systeme.CreateLocalTablePart(Schema, iddatabase, background)
        Dim momenttermine As DateTime = Now
        duree = momenttermine - momentdebute
        MessageBox.Show("INTELLIGENT MODE READY:" & duree.ToString)
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

        Dim ds As DataSet = LoadTableStructure(Name)

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

        Dim ds As DataSet = LoadTableStructure(Name)

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


        Dim ds As DataSet = LoadTableStructure(Name)

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

        Dim ds As DataSet = LoadTableStructure(Name)

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
        Dim ds As DataSet = LoadTableStructure(Name)

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
        Dim ds As DataSet = LoadTableStructure(Name)

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

    Public Shared Function ListAllStorePaginationMySql(ByVal Name As String) As String
        Dim objectname As String = Name.Substring(4, Name.Length - 4)
        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)
        Dim ds As DataSet = LoadTableStructure(Name)

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

        Dim command As String = Chr(9) & "DECLARE elementToSkyploc INT ;" & Chr(13) _
                                 & Chr(9) & "DECLARE limitloc   INT ;" & Chr(13) _
                                  & Chr(9) & "DECLARE columnloc varchar(100);" & Chr(13) _
                                 & Chr(9) & "DECLARE orderloc varchar(5);" & Chr(13) _
                                 & Chr(9) & "DECLARE sqlstr varchar(255) ; " & Chr(13) _
                                 & Chr(9) & "SET elementToSkyploc = _elementToSkyp;" & Chr(13) _
                                 & Chr(9) & "SET limitloc = _limit;" & Chr(13) _
                                 & Chr(9) & "SET columnloc = _column;" & Chr(13) _
                                  & Chr(9) & "SET orderloc = _order;" & Chr(13) _
                                  & Chr(9) & "SET sqlstr = '';" & Chr(13) _
                                 & Chr(9) & "IF(_column = '') THEN" & Chr(13) _
                                        & Chr(9) & "  If (_elementToSkyp = 0) Then" & Chr(13) _
                                  & Chr(9) & "	  SELECT *" & Chr(13) _
                                  & Chr(9) & "	  FROM tbl_" & objectname & " limit limitloc  ; " & Chr(13) _
                                  & Chr(9) & "  ELSE" & Chr(13) _
                                  & Chr(9) & "	 SELECT * " & Chr(13) _
                                  & Chr(9) & "   FROM tbl_" & objectname & " limit elementToSkyploc , limitloc ;" & Chr(13) _
                                  & Chr(9) & "   END IF;" & Chr(13) _
                                  & Chr(9) & "ELSE     " & Chr(13) _
                                      & Chr(9) & "  If (_elementToSkyp = 0) Then" & Chr(13) _
                                    & Chr(9) & "      set @sqlstr = CONCAT( 'SELECT * FROM tbl_" & objectname & " ORDER BY ',columnloc , ' ',orderloc ,' limit ',limitloc );" & Chr(13) _
                                 & Chr(9) & "      PREPARE stmt FROM @sqlstr;" & Chr(13) _
                                 & Chr(9) & "      EXECUTE stmt;" & Chr(13) _
                                 & Chr(9) & "      DEALLOCATE PREPARE stmt;" & Chr(13) _
                                     & Chr(9) & "  Else" & Chr(13) _
                                  & Chr(9) & "      set @sqlstr = CONCAT( 'SELECT * FROM tbl_" & objectname & " ORDER BY ',columnloc , ' ',orderloc ,' limit ',elementToSkyploc ,', ', limitloc );" & Chr(13) _
                                 & Chr(9) & "      PREPARE stmt FROM @sqlstr;" & Chr(13) _
                                 & Chr(9) & "      EXECUTE stmt;" & Chr(13) _
                                 & Chr(9) & "      DEALLOCATE PREPARE stmt;" & Chr(13) _
                                 & Chr(9) & "  END IF;" & Chr(13) _
                                 & Chr(9) & " END IF;" & Chr(13)

        
        Dim store As String = "DELIMITER $$" & Chr(13) _
                            & "CREATE PROCEDURE SP_ListAll_" & objectname & "_ForPagination " & Chr(13) & Chr(13) _
                            & Chr(9) & "(" & Chr(13) _
                            & Chr(9) & Chr(9) & "IN _elementToSkyp INT" & Chr(13) _
                            & Chr(9) & Chr(9) & ",IN _limit INT" & Chr(13) _
                            & Chr(9) & Chr(9) & ",IN _column varchar(100)" & Chr(13) _
                            & Chr(9) & Chr(9) & ",IN _order varchar(5)" & Chr(13) _
                            & Chr(9) & ")" & Chr(13) _
                            & "BEGIN" & Chr(13) _
                            & command & Chr(13) _
                            & "END$$" & Chr(13) & Chr(13) _
                            & "DELIMITER ;" & Chr(13)
        Return store
    End Function

    Public Shared Function ListAllStoreByField(ByVal name As String, ByVal Fiedlds As String) As String
        Dim _table As New Cls_Table()
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        _table.Read(_systeme.currentDatabase.ID, name)
        Dim Id_table As String = ""
        Dim Id_table_type As String = ""
        Dim QuerySet As String = ""
        Dim count As Integer = 0
        Dim fields_String As String() = Fiedlds.Split(",")
        Dim paramStore As String = ""
        Dim SelectClause As String = ""
        Dim libelleStored As String = ""

        For i As Integer = 0 To fields_String.Count - 1
            For Each _column In _table.ListofColumn
                If _column.Name = fields_String(i) Then
                    If (paramStore = "") Then
                        paramStore = "IN _" & _column.Name & " " & _column.Type.SqlServerName
                        SelectClause = "" & _column.Name & " = " & "_" & _column.Name
                        libelleStored = "_" & _column.Name
                    Else
                        paramStore &= Chr(13) & Chr(9) & Chr(9) & "," & "IN _" & _column.Name & " " & _column.Type.SqlServerName
                        SelectClause &= Chr(13) & Chr(9) & "and " & "" & _column.Name & " = " & "_" & _column.Name
                        libelleStored &= "_" & _column.Name
                    End If
                    Exit For
                End If
            Next
        Next

  

        Dim command As String = Chr(9) & "SELECT *" & Chr(13) _
                                & Chr(9) & "FROM " & name & Chr(13) _
                                & Chr(9) & "Where " & SelectClause
        Dim objectname As String = name.Substring(4, name.Length - 4)
        objectname = objectname.Substring(0, 1).ToUpper() & objectname.Substring(1, objectname.Length - 1)
        Dim store As String = "DELIMITER $$" & Chr(13) _
                           & "CREATE PROCEDURE SP_ListAll" & objectname & libelleStored & Chr(13) & Chr(13) _
                           & Chr(9) & "(" & Chr(13) _
                           & Chr(9) & Chr(9) & paramStore & Chr(13) _
                           & Chr(9) & ")" & Chr(13) _
                           & "BEGIN " & Chr(13) _
                           & command & Chr(13) _
                           & "END$$" & Chr(13) & Chr(13) _
                           & "DELIMITER ;"
        Return store
    End Function

    Private Shared Function ListAllByForeignKeyMySql(ByVal Name As String) As String
        Dim ds As DataSet = LoadTableStructure(Name)
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
    Public Shared Sub CreateFile(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

        ''    Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\", Application.StartupPath & "\SCRIPT\GENERIC_12\")
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & databasename & "\VbNetClassMySql\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & databasename & "\VbNetClassMySql\")
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
            'If initialtypes(i).ToString() = "image" Then
            '    objWriter.WriteLine("Public Property " & cols(i).Substring(1, cols(i).Length - 1) & "String() As String")
            '    objWriter.WriteLine("Get")
            '    objWriter.WriteLine("If " & cols(i) & " IsNot Nothing Then")
            '    objWriter.WriteLine("Return Encode(" & cols(i) & " )")
            '    objWriter.WriteLine("Else")
            '    objWriter.WriteLine("Return """"")
            '    objWriter.WriteLine("End If")
            '    objWriter.WriteLine("End Get")
            '    objWriter.WriteLine("Set(ByVal Value As String)")
            '    objWriter.WriteLine(cols(i) & " = Decode(Value)")
            '    objWriter.WriteLine("_isdirty = True")
            '    objWriter.WriteLine("End Set")
            '    objWriter.WriteLine("End Property")
            'End If

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
                '  objWriter.WriteLine("If " & ListofIndex(i) & " <> """" Then ")
                objWriter.WriteLine("Dim ds As Data.DataSet = MySQLHelper.ExecuteDataset(MySQLHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")
                objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                objWriter.WriteLine("BlankProperties()")
                objWriter.WriteLine("Return False")
                objWriter.WriteLine("End If" & Chr(13))

                objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                objWriter.WriteLine("Else")
                objWriter.WriteLine("BlankProperties()")
                '  objWriter.WriteLine("End If" & Chr(13))

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
    Public Shared Sub CreateJavaClassDomaine(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim nomUpperClasse As String = nomClasse.Substring(0, 1).ToUpper() & nomClasse.Substring(1, nomClasse.Length - 1)
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & databasename & "\JavaDomaineClass\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & databasename & "\JavaDomaineClass\")
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
        REM on verifie si le repertoir existe bien       
        If Not Directory.Exists(txt_PathGenerate_Script) Then
            Directory.CreateDirectory(txt_PathGenerate_Script)
        End If
        ' Create the file.
        Dim fs As FileStream = File.Create(path, 1024)
        fs.Close()
        Dim objWriter As New System.IO.StreamWriter(path, True)

        objWriter.WriteLine("package ht.edu.fds.domaine;")
        objWriter.WriteLine()
        objWriter.WriteLine("import java.sql.Timestamp;")
        objWriter.WriteLine("import java.util.*;")
        objWriter.WriteLine("import javax.validation.constraints.*;")
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
            Dim arrstring As String() = Nothing
            If count < cap - 4 Then
                cols.Add("" & dt(0))
                initialtypes.Add(dt(1))
                If dt(1).ToString.Contains("(") Then
                    arrstring = dt(1).ToString.Split("(")
                    types.Add(ConvertDBToJavaType(arrstring(0).ToString))
                    length.Add(arrstring(1).ToString.Replace(")", ""))
                Else
                    types.Add(ConvertDBToJavaType((dt(1))))
                    length.Add("0")
                End If

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

        objWriter.WriteLine("public long setId(long id) {" & Chr(13) & _
                            "this.id = id;" & Chr(13) & _
                            "}")

        For i As Int32 = 1 To cols.Count - 1 ''On ne cree pas de property pour la derniere column
            Dim propName As String = ""
            Dim s As String() = cols(i).Split("_")
            For j As Integer = 1 To s.Length - 1
                propName &= StrConv(s(j), VbStrConv.ProperCase)
            Next
            Dim validation As String = ""

            '            If types(i) = "String" Then
            '                validation = "@Size( min  = 1 , max =" & length(i) & " ,  message = ""Le nombre de caractere de" & cols(i) & " est invalide"")"
            '            ElseIf types(i) = "int" Or types(i) = "long" Then
            '                validation = "@Min(value = 1, message = ""Le " & cols(i) & " est obligatoire "") "
            '           
            '            End If

            '            If cols(i) = "Email" Or cols(i).ToString.Contains("Email") Or cols(i) = "AdresseElectronique" Then
            '                validation = "@NotBlank(message = ""L'adresse électronique est obligatoire"")" & Chr(13) _
            '                     & "@Email(message = ""Le format de l'adresse électronique n'est pas valide"")"
            '            End If

            Dim attrib As String = "public " & types(i) & " get" & cols(i) & "() {"
            Dim setter As String = "public void " & "set" & cols(i) & "(" & types(i) & " " & cols(i).ToLower() & ") {"

            If cols(i) <> "_isdirty" Or cols(i) <> "_LogData" Then
                '   objWriter.WriteLine(log)

                objWriter.WriteLine()
                objWriter.WriteLine(validation)
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
            End If

        Next

        objWriter.WriteLine("//</editor-fold>")

        objWriter.WriteLine()

        objWriter.WriteLine()
        objWriter.WriteLine(_end)
        objWriter.WriteLine()
        objWriter.Close()

    End Sub

    Public Shared Sub CreateJavaClassDAL(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim nomUpperClasse As String = nomClasse.Substring(0, 1).ToUpper() & nomClasse.Substring(1, nomClasse.Length - 1)

        Dim point_interogation As String = ""
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & databasename & "\JavaDALClass\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & databasename & "\JavaDALClass\")
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
        REM on verifie si le repertoir existe bien       
        If Not Directory.Exists(txt_PathGenerate_Script) Then
            Directory.CreateDirectory(txt_PathGenerate_Script)
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
        objWriter.WriteLine("private static String currentexception = """" ;")

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
        objWriter.WriteLine("obj.setId(reponse.getLong(1));")
        For i As Int32 = 1 To cols.Count - 1
            Dim attrib As String = ""  'not used for now to be updated.
            objWriter.WriteLine(" obj.set" & cols(i) & "(reponse.get" & types(i).Substring(0, 1).ToUpper() & types(i).Substring(1, types(i).Length - 1) & "(" & i + 1 & " ));")
        Next
        objWriter.WriteLine("             } " & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "catch (ClassNotFoundException nfex)" & Chr(13) & _
                                        "{" & Chr(13) & _
                                         "currentexception = nfex.getMessage();" & Chr(13) & _
                                          "  throw new ClassNotFoundException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",Read) ! ""+ nfex.getMessage());  " & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "catch (Exception ex)" & Chr(13) & _
                                        "{" & Chr(13) & _
                                         "currentexception = ex.getMessage();" & Chr(13) & _
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
                                             "currentexception = sqlex.getMessage();" & Chr(13) & _
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
                                        "currentexception = nfex.getMessage();" & Chr(13) & _
                                           "  throw new ClassNotFoundException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",ListAll) ! ""+ nfex.getMessage());  " & Chr(13) & _
                                        "}" & Chr(13) & _
                                        "catch (Exception ex)" & Chr(13) & _
                                        "{" & Chr(13) & _
                                        "currentexception = ex.getMessage();" & Chr(13) & _
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
                                               "currentexception = sqlex.getMessage();" & Chr(13) & _
                                              "  throw new SQLException("" Erreur systeme ! Contacter l'administrateur (" & nomUpperClasse & ",ListAll) ! ""+ sqlex.getMessage());  " & Chr(13) & _
                                            "}" & Chr(13) & _
                                            "if (!"""".equals(currentexception.toString()) ){" & Chr(13) & _
             "     throw new Exception("" Erreur systeme ! Contacter l'administrateur (Unite,ListAll) ! "" + currentexception.toString());" & Chr(13) & _
            "}" & Chr(13) &
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

    Public Shared Sub CreateJavaClassSession(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim nomUpperClasse As String = nomClasse.Substring(0, 1).ToUpper() & nomClasse.Substring(1, nomClasse.Length - 1)
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & databasename & "\JavaSessionClass\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & databasename & "\JavaSessionClass\")
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
        REM on verifie si le repertoir existe bien       
        If Not Directory.Exists(txt_PathGenerate_Script) Then
            Directory.CreateDirectory(txt_PathGenerate_Script)
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

        objWriter.WriteLine("return data;")
        objWriter.WriteLine("}")


        objWriter.WriteLine("public ArrayList<" & nomUpperClasse & "> listAll" & nomUpperClasse & "_ForObject() throws Exception")
        objWriter.WriteLine("{")
        objWriter.WriteLine("ArrayList<" & nomUpperClasse & "> liste = new ArrayList<" & nomUpperClasse & ">" & "();")
        objWriter.WriteLine("list" & nomUpperClasse & " = " & nomUpperClasse & "DAL.ListAll();")

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
        objWriter.WriteLine("return liste; " & Chr(13) & _
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


        Dim ds As DataSet = LoadTableStructure(name)
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


        Dim ds As DataSet = LoadTableStructure(name)
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

#Region "Php Class Fonctions"

    Public Shared Sub CreatePHPClass(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl_", "")
        Dim nomUpperClasse As String = nomClasse.Substring(0, 1).ToUpper() & nomClasse.Substring(1, nomClasse.Length - 1)
        ''   Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\", Application.StartupPath & "\SCRIPT\GENERIC_12\")
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & databasename & "\PHPClass\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & databasename & "\PHPClass\")
        Dim path As String = txt_PathGenerate_Script & "" & nomUpperClasse & ".class.php"
        Dim ListofIndex As New List(Of String)
        Dim ListofIndexType As New List(Of String)
        Dim index_li_type As New Hashtable
        Dim countindex As Long = 0
        Dim insertstring As String = ""
        Dim updatestring As String = ""


        Dim header As String = "'''Generate By Edou Application *******" & Chr(13) _
                               & "''' Class " + nomUpperClasse & Chr(13) & Chr(13)
        header = ""
        Dim content As String = "class " & nomUpperClasse & " {" & Chr(13)

        _end = "}" & Chr(13)
        ' Delete the file if it exists.
        If File.Exists(path) Then
            File.Delete(path)
        End If
        ' Create the file.
        REM on verifie si le repertoir existe bien       
        If Not Directory.Exists(txt_PathGenerate_Script) Then
            Directory.CreateDirectory(txt_PathGenerate_Script)
        End If
        ' Create the file.
        Dim fs As FileStream = File.Create(path, 1024)
        fs.Close()
        Dim objWriter As New System.IO.StreamWriter(path, True)

        ''    objWriter.WriteLine("<?php require_once('../Query/DAL/MySQL_Helper.php'); ?>")
        objWriter.WriteLine("<?php")
        objWriter.WriteLine(content)
        objWriter.WriteLine()


        Dim ds As DataSet = LoadTableStructure(name)
        Dim cols As New List(Of String)
        Dim types As New List(Of String)
        Dim initialtypes As New List(Of String)
        Dim length As New List(Of String)
        Dim count As Integer = 0
        Dim cap As Integer = ds.Tables(0).Rows.Count

        For Each dt As DataRow In ds.Tables(0).Rows
            If dt(0).ToString = "PRI" Then
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
                '   length.Add(dt(2))
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

        objWriter.WriteLine("public function " & nomUpperClasse & "($id=0)")
        objWriter.WriteLine("{")
        objWriter.WriteLine("if($id == 0){")
        objWriter.WriteLine("	$this->BlankProperties();")
        objWriter.WriteLine("}else{")
        objWriter.WriteLine("	$this->Read($id);")
        objWriter.WriteLine("}")
        objWriter.WriteLine("}//")

        objWriter.WriteLine("#EndRegion ""Constructeur""")

        objWriter.WriteLine("#Region ""Proprietes""")


        objWriter.WriteLine("public function get_" & "ID" & "(){")
        objWriter.WriteLine("return $this->_" & "id" & ";")
        objWriter.WriteLine("}")

        objWriter.WriteLine("public function set_" & "ID" & "($value) {")
        objWriter.WriteLine("	if ($this->_" & "id" & " <> $value ){")
        objWriter.WriteLine("$this->_" & "id" & " = $value;")
        objWriter.WriteLine("}")
        objWriter.WriteLine("}")

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


        objWriter.WriteLine("private function Insert($User)" & Chr(13) & _
                             "{" & Chr(13) & _
                             "try{ " & Chr(13) & _
                             "$rows = SQLHelper::ExecuteProcedure(""SP_Insert" & nomUpperClasse & """")
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

        objWriter.WriteLine("$count = count($rows);")
        objWriter.WriteLine("if ($count == 0) {")
        objWriter.WriteLine("return 0;")
        objWriter.WriteLine("} else {")

        objWriter.WriteLine("  	return $this->_id = (int) $rows[0][""ID""]; " & Chr(13) & _
                            " }" & Chr(13) & _
                            "}catch(Exception $e){" & Chr(13) & _
                            "throw New Exception ($e->getMessage());" & Chr(13) & _
                            "}" & Chr(13) & _
                            "}//")




        objWriter.WriteLine("private function Update($User)" & Chr(13) & _
                             "{" & Chr(13) & _
                             "try{ " & Chr(13) & _
                             "$result = SQLHelper::ExecuteProcedure (""SP_Update" & nomUpperClasse & """")

        objWriter.WriteLine(", $this->_id")

        Try
            For i As Int32 = 1 To cols.Count - 1
                objWriter.WriteLine(", $this->_" & cols(i))
            Next
        Catch ex As Exception
        End Try
        objWriter.WriteLine(", $User")
        objWriter.WriteLine(");")
        objWriter.WriteLine("return $result;" & Chr(13) & _
                            " }catch(Exception $e){" & Chr(13) & _
                              " throw New Exception ( $e->getMessage());" & Chr(13) & _
                            "}" & Chr(13) & _
                            "}")

        objWriter.WriteLine()



        objWriter.WriteLine("public function Read($id)" & Chr(13) & _
                                 "{" & Chr(13) & _
                                 "	if($id <> 0 ){" & Chr(13) & _
                                 "		$rows = SQLHelper::ExecuteProcedure(""SP_Select" & nomUpperClasse & "_ByID"", $id);" & Chr(13) & _
                                 "		if(count($rows) < 0 )" & Chr(13) & _
                                 "		{ " & Chr(13) & _
                                 "			$this->BlankProperties();" & Chr(13) & _
                                 "		}else{" & Chr(13) & _
                                 "			$this->SetProperties($rows[0]);" & Chr(13) & _
                                 "		}" & Chr(13) & _
                                 "	}else{" & Chr(13) & _
                                 "		$this->BlankProperties();" & Chr(13) & _
                                 "	}" & Chr(13) & _
                                 "}")
        objWriter.WriteLine()
        objWriter.WriteLine(" public function SearchAll()" & Chr(13) & _
                                 "{" & Chr(13) & _
                                  "$rows = SQLHelper::ExecuteProcedure(""SP_ListAll_" & nomUpperClasse & """ );" & Chr(13) & _
                                  "$objs = array();" & Chr(13) & _
                                  "foreach ($rows as $row) {" & Chr(13) & _
                                  "$obj = new  " & nomUpperClasse & ";" & Chr(13) & _
                                   "$obj->SetProperties($row);	" & Chr(13) & _
                                   "$objs[] = $obj;" & Chr(13) & _
                                  "}" & Chr(13) & _
                                  "return $objs;" & Chr(13) & _
                                 "}"
                            )


        objWriter.WriteLine("public static function SearchAll_ForPagination($elementToSkyp, $rows, $columnToSort, $sortingValue) {")
        objWriter.WriteLine("$rows = SQLHelper::ExecuteProcedure(""SP_ListAll_" & nomUpperClasse & "_ForPagination"", $elementToSkyp, $rows, $columnToSort, $sortingValue);")
        objWriter.WriteLine("$objs = array();")
        objWriter.WriteLine("foreach ($rows as $row) {")
        objWriter.WriteLine("   $obj =  new  " & nomUpperClasse & ";")
        objWriter.WriteLine("   $obj->SetProperties($row);")
        objWriter.WriteLine("   $objs[] = $obj;")
        objWriter.WriteLine(" }")
        objWriter.WriteLine("  return $objs;")
        objWriter.WriteLine("}")



        objWriter.WriteLine("public function Delete()" & Chr(13) & _
                                         "{" & Chr(13) & _
                                          "$result = SQLHelper::ExecuteProcedure(""SP_Delete" & nomUpperClasse & """, $this->_id);" & Chr(13) & _
                                          "return $result;" & Chr(13) & _
                                         "}"
                                    )

        objWriter.WriteLine()

        objWriter.WriteLine("	public function Save($User) " &
                                  "{" & Chr(13) & _
                                  "if($this->isdirty)" & Chr(13) & _
                                  "{" & Chr(13) & _
                                  "//	" & nomUpperClasse & "::Validation();" & Chr(13) & _
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
                objWriter.WriteLine("$this->_" & cols(i) & "= """";")
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
                objWriter.WriteLine("$this->_" & cols(i) & "= $rs['" & cols(i) & "'];")
            Next
        Catch ex As Exception
        End Try

        objWriter.WriteLine("}")
        objWriter.WriteLine()


        objWriter.WriteLine("#EndRegion ""Access BASE DE DONNEE""")
        objWriter.WriteLine()

        'objWriter.WriteLine("#Region ""Other """)
        'objWriter.WriteLine(" private function Validation")

        Dim stringlistforvalidation As New List(Of String) From {"String"}
        Dim decimalintegerforvalidation As New List(Of String) From {"Integer", "Long", "Decimal"}



        'For i As Int32 = 1 To cols.Count - 2
        '    If stringlistforvalidation.Contains(types(i)) Then
        '        objWriter.WriteLine("If (" & cols(i) & " = "")"" { " & Chr(13) _
        '                            & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire""))" & Chr(13) _
        '                            & "}"
        '                            )
        '        objWriter.WriteLine()
        '        objWriter.WriteLine("If Len(" & cols(i) & ") > " & length(i) & " {" & Chr(13) _
        '                            & "Throw (New System.Exception("" " & "Trop de caractères insérés pour" & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & "  (la longueur doit être inférieure a " & length(i) & " caractères.  )""))" & Chr(13) _
        '                            & "}"
        '                            )
        '        objWriter.WriteLine()
        '    End If

        '    If decimalintegerforvalidation.Contains(types(i)) Then
        '        If ListofForeignKey.Contains(cols(i)) Then
        '            objWriter.WriteLine("If (" & cols(i) & " == 0) { " & Chr(13) _
        '                          & "Throw (New System.Exception("" " & cols(i).Substring(4, cols(i).Length - 4).ToString.ToLower & " obligatoire""))" & Chr(13) _
        '                          & "}"
        '                          )
        '            objWriter.WriteLine()
        '        Else
        '            objWriter.WriteLine("If ( " & cols(i) & " == 0 ) { " & Chr(13) _
        '                           & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire.""))" & Chr(13) _
        '                           & "}"
        '                           )
        '            objWriter.WriteLine()
        '        End If
        '    End If


        'Next
        'objWriter.WriteLine("")

        objWriter.WriteLine("#EndRegion ""Other """)

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''end''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        objWriter.WriteLine()
        objWriter.WriteLine(_end)
        objWriter.WriteLine("?>")
        objWriter.Close()

    End Sub

#End Region

#Region "Asp.Net Vb.net Interface Fonctions"

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

        objWriter.WriteLine(" txt_Code" & nomSimple & "_Hid.Text = CStr(SolutionsHT.TypeSafeConversion.NullSafeLong(Request.QueryString(""id""), 0)) ")
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
                objWriter.WriteLine("rcmb_" & cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1)) & " .Enabled =   modify ")
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
            Dim attributUsed As String = foreignkey.Column.Name.Substring(ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (ForeinKeyPrefix.Length))
            Dim ClassName As String = "Cls_" & foreignkey.Column.Name.Substring(ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (ForeinKeyPrefix.Length))
            objWriter.WriteLine("FillCombo" & attributUsed & "()")
        Next

        objWriter.WriteLine("  If _tmpEditState Then" & Chr(13) _
                            & "Dim obj as New " & nomClasse & "( CLng(txt_Code" & nomSimple & "_Hid.Text))" & Chr(13) _
                            & "With obj")

        For i As Int32 = 1 To cols.Count - 3
            If ListofForeignKey.Contains(cols(i)) Then
                objWriter.WriteLine("rcmb_" & cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1)) & " .SelectedIndex =  rcmb_" & cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1)) & ".FindItemIndexByValue(." & cols(i).Substring(1, cols(i).Length - 1) & ")")
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
                objWriter.WriteLine("rcmb_" & cols(i).Substring(ForeinKeyPrefix.Length + 1, cols(i).Length - (ForeinKeyPrefix.Length + 1)) & " .SelectedIndex = 0")
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
                objWriter.WriteLine("." & columnToUse & "  =   rcmb_" & columnToUse.Substring(ForeinKeyPrefix.Length, columnToUse.Length - (ForeinKeyPrefix.Length)) & " .SelectedValue ")
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
            Dim attributUsed As String = foreignkey.Column.Name.Substring(ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (ForeinKeyPrefix.Length))

            Dim ClassName As String = "Cls_" & foreignkey.Column.Name.Substring(ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (ForeinKeyPrefix.Length))
            ClassName = foreignkey.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomforeign As String = foreignkey.Column.Name.Substring(ForeinKeyPrefix.Length, foreignkey.Column.Name.Length - (ForeinKeyPrefix.Length))
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

        objWriter.WriteLine(" <table border=""0"" class=""form_Enregistrement"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")
        objWriter.WriteLine("<tr valign=""top"">")
        objWriter.WriteLine(" <td style=""width: 100%;"">")

        objWriter.WriteLine("  <asp:Panel ID=""pn_entete"" runat=""server"" Width=""100%"">")
        objWriter.WriteLine("<table border=""0"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")

        Dim countColumn As Integer = 0
        For Each column As Cls_Column In _table.ListofColumn
            Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
            Dim primary As String = _table.ListofColumn(0).Name
            If countColumn < _table.ListofColumn.Count - 4 Then

                If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then

                    Dim columnName As String = column.Name.Substring(ForeinKeyPrefix.Length, column.Name.Length - (ForeinKeyPrefix.Length))
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
                ElseIf column.TrueSqlServerType = "date" And column.TrueSqlServerType = "datetime" And column.Name <> _table.ListofColumn(0).Name Then
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

            .WriteLine("  rbtnAdd" & nomSimple & ".Attributes.Add(""onclick"", ""javascript:void(ShowAddUpdateForm('../Parametres/wbfrm_" & nomSimple & ".aspx', 550, 285));"")" & Chr(13) _
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
            .WriteLine("<td align=""left"">")
            .WriteLine("<h1>")
            .WriteLine("<asp:Label ID=""lbHeaderTitle"" runat=""server"" CssClass=""LabelBold"" Font-Bold=""True""")
            .WriteLine("Font-Size=""Large"" ForeColor=""Black"">")
            .WriteLine("</asp:Label>")
            .WriteLine("</h1>")
            .WriteLine("</td>")
            .WriteLine("</tr>")
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
            .WriteLine(" <telerik:RadButton ID=""rbtnRepot"" Visible=""false"" runat=""server"" Text=""Imprimer Rapport"">")
            .WriteLine("</telerik:RadButton>")
            .WriteLine("</div>")
            .WriteLine(" <div style=""display: block; float: left; padding: 0px 5px"">")
            .WriteLine("<telerik:RadButton ID=""rbtnAdd" & nomSimple & """  runat=""server"" AutoPostBack=""False"" Text=""Enregistrer un " & nomSimple.ToLower & """>")
            .WriteLine("</telerik:RadButton>")
            .WriteLine(" </div>")
            .WriteLine("<br clear=""all"" />")
            .WriteLine("</div>")
            .WriteLine("</td>")
            .WriteLine(" </tr>")
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

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                        Dim textForcombo As String = ""
                        Dim columnName As String = column.Name.Substring(ForeinKeyPrefix.Length, column.Name.Length - (ForeinKeyPrefix.Length))
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
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "Date" And column.Name <> _table.ListofColumn(0).Name Then
                        .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                        .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" ShowFilterIcon=""false""")
                        .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                        .WriteLine("</telerik:GridBoundColumn>")
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "Date" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then
                        .WriteLine("<telerik:GridBoundColumn DataField=""" & column.Name & """ UniqueName=""" & column.Name & """ HeaderText=""" & column.Name & """")
                        .WriteLine(" FilterControlAltText=""Filter " & column.Name & " column"" FilterControlWidth=""95%"" DataFormatString=""{0:##,###,##0.00}"" ShowFilterIcon=""false""")
                        .WriteLine(" AutoPostBackOnFilter=""true"" CurrentFilterFunction=""Contains"">")
                        .WriteLine("<ItemStyle Width=""" & pourcentage & """ />")
                        .WriteLine("</telerik:GridBoundColumn>")
                    ElseIf column.TrueSqlServerType = "Date" And column.Name <> _table.ListofColumn(0).Name Then

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

#End Region

#Region " Php Interface Fonctions"

    Public Shared Sub CreateInterfaceCodePHP1(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl", "frm").Replace("Tbl", "frm").Replace("TBL", "frm")
        Dim nomSimple As String = name.Substring(4, name.Length - 4)
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\PHPHTMLWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\PHPHTMLWebForm\")
        Dim path As String = txt_PathGenerate_Script & nomClasse & ".view.php"
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


        With objWriter
            .WriteLine("<?php require_once('../Model/Specialisation.class.php'); ?>")
            .WriteLine("<?php require_once('../DAL/MySQL_Helper.class.php'); ?>")
            .WriteLine("<?php require_once('../ServTech/ValidationFunction.class.php'); ?>")
            .WriteLine("<?php require_once('../ServTech/ApplicationHelper.php'); ?>")
            .WriteLine("<?php require_once('../ServTech/QueryConnexion.php'); ?>")
        End With

        With objWriter
            .WriteLine("<?php")
            .WriteLine("//if (!isset($_SESSION[$GLOBAL_SESSION])) {")
            .WriteLine("//    ApplicationHelper::Refresh_MainPage();")
            .WriteLine("//}")
            .WriteLine("$PageName = 'frm_Specialisation.view.php';")
            .WriteLine("$Btn_Save = 'btnSave';")
            .WriteLine("$Btn_Annuler = 'btnCancel';")
            .WriteLine("//        if( !isset($_SESSION[$GLOBAL_SESSION]) )")
            .WriteLine("//	{")
            .WriteLine("//		Cls_Parameters::Refresh_MainPage();")
            .WriteLine("//	}else{")
            .WriteLine("//		$Is_Acces_Page = 'display:block;';")
            .WriteLine("//		if(!Select_Privilege($PageName,$_SESSION[$GLOBAL_SESSION_GROUPE],$ConString_MySql))")
            .WriteLine("//		{// No acces a la page")
            .WriteLine("//			Cls_Parameters::Dialog_PopUP($NO_ACCES_PAGE);")
            .WriteLine("//			$_message = Cls_Parameters::Show_Message($NO_ACCES_PAGE);")
            .WriteLine("//			$Is_Acces_Page = 'display:none;';")
            .WriteLine("//		}else{")
            .WriteLine("//			$Btn_Save = (Select_Privilege($Btn_AddEdit,$_SESSION[$GLOBAL_SESSION_GROUPE],$ConString_MySql)) ? '' : 'display:none;' ;")
            .WriteLine("//		}//")
            .WriteLine("//	}//   ")
            .WriteLine("?>")

        End With

        With objWriter
            .WriteLine("<?php")
            .WriteLine("$ID = (isset($_GET[""ID""])) ? $_GET[""ID""] : '0';")


            For i As Int32 = 1 To cols.Count - 3
                Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("$txt_" & columnToUse & " = ValidationFunction::test_input($_POST['txt_" & columnToUse & "']);")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    objWriter.WriteLine("$txt_" & columnToUse & " = ValidationFunction::test_input($_POST['txt_" & columnToUse & "']);")
                ElseIf types(i) = "Boolean" Then
                    objWriter.WriteLine("$txt_" & columnToUse & " = ValidationFunction::test_input($_POST['txt_" & columnToUse & "']);")
                Else
                    objWriter.WriteLine("$txt_" & columnToUse & " = ValidationFunction::test_input($_POST['txt_" & columnToUse & "']);")
                End If
            Next


            .WriteLine("if (!isset($_SESSION[$GLOBAL_SESSION])) {")
            .WriteLine("//	$_SESSION[$GLOBAL_SESSION_PAGENAME] = ""../Administration/Entreprise.php"";")
            .WriteLine("//	Cls_Parameters::Refresh_MainPage();")
            .WriteLine("}")
            .WriteLine("$IsPostBack = false;")
            .WriteLine("if ($_POST[""btnCancel""] == ""Abandonner"") {")
            .WriteLine("ApplicationHelper::Refresh_MainPage(false);")
            .WriteLine("}//")
            .WriteLine("if (isset($_POST['btnSave'])) {")

            Dim saveString As String = "$ID"
            For i As Int32 = 1 To cols.Count - 3
                Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                If ListofForeignKey.Contains(cols(i)) Then
                    saveString = saveString & ", txt_" & columnToUse
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    saveString = saveString & ", txt_" & columnToUse
                ElseIf types(i) = "Boolean" Then
                    saveString = saveString & ", txt_" & columnToUse
                Else
                    saveString = saveString & ", txt_" & columnToUse
                End If
            Next
            .WriteLine("btnSaveClick(" & saveString & ");")


            .WriteLine("}")
            .WriteLine("if (!$IsPostBack) {")
            .WriteLine("if (isset($_GET['ID']) && isset($_GET['Action']) && is_numeric($_GET['ID'])) {")
            .WriteLine("global $ID;")
            .WriteLine("try {")
            .WriteLine("switch ($_GET['Action']) {")
            .WriteLine("case 'Edit': {")
            .WriteLine("$obj = new" & nomSimple & "($ID);")

            For i As Int32 = 1 To cols.Count - 3
                Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                If ListofForeignKey.Contains(cols(i)) Then
                    .WriteLine("$txt" & columnToUse & " = $obj->get_" & columnToUse & "();")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    .WriteLine("$txt" & columnToUse & " = $obj->get_" & columnToUse & "();")
                ElseIf types(i) = "Boolean" Then
                    .WriteLine("$txt" & columnToUse & " = $obj->get_" & columnToUse & "();")
                Else
                    .WriteLine("$txt" & columnToUse & " = $obj->get_" & columnToUse & "();")
                End If
            Next



            .WriteLine("                    break;")
            .WriteLine("}")
            .WriteLine("default: ")
            .WriteLine("}")
            .WriteLine("} catch (Exception $ex) {")
            .WriteLine("$_message = $ex->getMessage();	")
            .WriteLine("ApplicationHelper::Dialog_PopUP($_message);")
            .WriteLine("	    $_message = ApplicationHelper::Show_Message($_message,'S','../../');")
            .WriteLine("}")
            .WriteLine("}")
            .WriteLine("}")


            saveString = "$ID"
            For i As Int32 = 1 To cols.Count - 3
                Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                If ListofForeignKey.Contains(cols(i)) Then
                    saveString = saveString & ", txt_" & columnToUse
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    saveString = saveString & ", txt_" & columnToUse
                ElseIf types(i) = "Boolean" Then
                    saveString = saveString & ", txt_" & columnToUse
                Else
                    saveString = saveString & ", txt_" & columnToUse
                End If
            Next

            .WriteLine("function btnSaveClick(" & saveString & ") { ")


            .WriteLine("$IsPostBack = true;")
            .WriteLine("try {")
            .WriteLine("Validation();")


            saveString = "$ID"
            For i As Int32 = 1 To cols.Count - 3
                Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                If ListofForeignKey.Contains(cols(i)) Then
                    saveString = saveString & ", txt_" & columnToUse
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    saveString = saveString & ", txt_" & columnToUse
                ElseIf types(i) = "Boolean" Then
                    saveString = saveString & ", txt_" & columnToUse
                Else
                    saveString = saveString & ", txt_" & columnToUse
                End If
            Next

            .WriteLine("Save(" & saveString & ");")




            .WriteLine("} catch (Exception $ex) {")
            .WriteLine("$_message = $ex->getMessage();")
            .WriteLine("ApplicationHelper::Dialog_PopUP($_message);")
            .WriteLine("$_message = ApplicationHelper::Show_Message($_message, 'S');")
            .WriteLine("}")
            .WriteLine("}")

            .WriteLine("function Validation() {")

            .WriteLine("global " & saveString & ";")


            For i As Int32 = 1 To cols.Count - 3
                Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                If ListofForeignKey.Contains(cols(i)) Then
                    .WriteLine("if ($txt_" & columnToUse & " == '') {")
                    .WriteLine("throw (new Exception(""le " & columnToUse & " est obligatoire!""));")
                    .WriteLine("}")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    .WriteLine("if ($txt_" & columnToUse & " == '') {")
                    .WriteLine("throw (new Exception(""le " & columnToUse & " est obligatoire!""));")
                    .WriteLine("}")
                ElseIf types(i) = "Boolean" Then
                    .WriteLine("if ($txt_" & columnToUse & " == '') {")
                    .WriteLine("throw (new Exception(""le " & columnToUse & " est obligatoire!""));")
                    .WriteLine("}")
                Else
                    .WriteLine("if ($txt_" & columnToUse & " == '') {")
                    .WriteLine("throw (new Exception(""le " & columnToUse & " est obligatoire!""));")
                    .WriteLine("}")
                End If
            Next


            .WriteLine("}")



            .WriteLine("function Save() {")

            .WriteLine("global " & saveString & ";")
            .WriteLine("$obj = new " & nomSimple & "($ID);")



            For i As Int32 = 1 To cols.Count - 3
                Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
                If ListofForeignKey.Contains(cols(i)) Then
                    .WriteLine("$obj->set_" & columnToUse & "($txt_" & columnToUse & ");")
                ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
                    .WriteLine("$obj->set_" & columnToUse & "($txt_" & columnToUse & ");")
                ElseIf types(i) = "Boolean" Then
                    .WriteLine("$obj->set_" & columnToUse & "($txt_" & columnToUse & ");")
                Else
                    .WriteLine("$obj->set_" & columnToUse & "($txt_" & columnToUse & ");")
                End If
            Next

            .WriteLine("$obj->Save(""Admin"");")
            .WriteLine("ApplicationHelper::Dialog_PopUP(""Sauvegarde efféctuée"");")
            .WriteLine("ApplicationHelper::Refresh_MainPage();")
            .WriteLine("}")


            .WriteLine("?>")


        End With


        objWriter.WriteLine("<!DOCTYPE html PUBLIC ""-//W3C// DTD XHTML 1.0 Transitional//EN"" ""http://www.w3org/TR/XHTML1/DTD/xhtml1-transitional.dtd ""  ")
        objWriter.WriteLine("<html xmlns=""http://www.w3.org/1999/xhtml"">  ")
        objWriter.WriteLine("<head>")

        objWriter.WriteLine("<meta http-equiv= ""Content-Type"" content=""text/html; charset=utf-8"" />")

        objWriter.WriteLine("<link rel=""shortcut icon"" href=""icon.ico"" />")
        objWriter.WriteLine("<link rel=""stylesheet"" type=""text/css"" href=""style/style.css"" />")
        objWriter.WriteLine("<link rel=""stylesheet"" type=""text/css"" href=""style/style.css"" />")

        objWriter.WriteLine("</head>")

        objWriter.WriteLine("<body>")
        objWriter.WriteLine("<center>")
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
        objWriter.WriteLine(" <label ID=""lbl_title"" class=""popupFormTitleLabel""   text=""Gestion des " & nomSimple & """   runat=""server""></label>")
        objWriter.WriteLine("</h1>")
        objWriter.WriteLine("</td>")
        objWriter.WriteLine("</tr>")

        objWriter.WriteLine("<tr valign=""top"">")
        objWriter.WriteLine("<td style=""width: 100%;"" colspan=""2"" valign=""top"">")
        objWriter.WriteLine("<hr class=""hrHeaderAfterPhoto"" />")
        objWriter.WriteLine("</td>")
        objWriter.WriteLine("</tr>")


        objWriter.WriteLine("<tr valign=""top"">")
        objWriter.WriteLine("<td valign=""top"" style=""width: 100%;"" colspan=""2"">")

        objWriter.WriteLine("<form name=""frm_" & nomSimple & " method=""post"" action="" frm_" & nomSimple & ".php"" >")



        objWriter.WriteLine(" <table border=""0"" class=""form_Enregistrement"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")
        objWriter.WriteLine("<tr valign=""top"">")
        objWriter.WriteLine(" <td style=""width: 100%;"">")
        objWriter.WriteLine("<table border=""0"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")



        Dim countColumn As Integer = 0
        For Each column As Cls_Column In _table.ListofColumn
            Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
            Dim primary As String = _table.ListofColumn(0).Name
            If countColumn < _table.ListofColumn.Count - 4 Then

                If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then

                    Dim columnName As String = column.Name.Substring(ForeinKeyPrefix.Length, column.Name.Length - (ForeinKeyPrefix.Length))
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & columnName & """ class=""popupFormInputLabel"" "">" & columnName & ": </label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadComboBox ID=""rcmb_" & columnName & """  Filter=""Contains""  AutoPostBack=""true""  Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadComboBox> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType <> "bit" Then
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & column.Name & """ class=""popupFormInputLabel"">" & column.Name & ": </label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<input type=""text"" id=""txt_" & column.Name & """ name=""txt_" & column.Name & """  value=""<?php echo $txt_" & column.Name & "; ?>""  Width=""50%"" > " & Chr(13) & " </input> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                ElseIf column.TrueSqlServerType = "date" And column.TrueSqlServerType = "datetime" And column.Name <> _table.ListofColumn(0).Name Then
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & column.Name & """ class=""popupFormInputLabel"" "">" & column.Name & ": </label> ")
                    objWriter.WriteLine("</td>")

                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadDatePicker ID=""rdp_" & column.Name & """ Width=""50%"" runat=""server""" & _
                                     "DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01""  ToolTip=""Cliquer sur le bouton pour choisir une date"" > " & Chr(13) & " </telerik:RadDatePicker>  ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")

                ElseIf column.TrueSqlServerType = "bit" Then
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & column.Name & """ class=""popupFormInputLabel"" "">" & column.Name & ": </label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<input type=""checkbox"" name=""chk_" & column.Name & """ width=""50%"" >" & _
                                      " </input>  ")
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
        objWriter.WriteLine("<input type=""submit"" name=""btnSave"" value=""Sauvegarder"" > ")
        objWriter.WriteLine("</input>")
        objWriter.WriteLine("&nbsp;")
        objWriter.WriteLine("<input type=""submit"" name=""btnCancel"" value= ""Abandonner"" >")
        objWriter.WriteLine("</input>")

        objWriter.WriteLine("</td>")

        objWriter.WriteLine("</tr>")
        objWriter.WriteLine(" </table>")
        objWriter.WriteLine(" </td>")
        objWriter.WriteLine("</tr>")
        objWriter.WriteLine("</table>")

        objWriter.WriteLine("</form>")
        objWriter.WriteLine("  </td>")
        objWriter.WriteLine("</tr>")
        objWriter.WriteLine("</table>")
        objWriter.WriteLine("</td>")
        objWriter.WriteLine("</tr>")
        objWriter.WriteLine("</table>")

        objWriter.WriteLine("<br />")
        objWriter.WriteLine("</center>")
        objWriter.WriteLine("<input type=""text"" id=""txt_CodeHid" & nomSimple & """ name=""txt_" & nomSimple & """  value=""0""  width=""0px"" > " & Chr(13) & " </input> ")

        objWriter.WriteLine("</body>")

        objWriter.WriteLine()
        objWriter.Close()
    End Sub

    Public Shared Sub CreateInterfaceCodeHTmlPHP(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl", "frm").Replace("Tbl", "frm").Replace("TBL", "frm")
        Dim nomSimple As String = name.Substring(4, name.Length - 4)
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\PHPHTMLWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\PHPHTMLWebForm\")
        Dim path As String = txt_PathGenerate_Script & nomClasse & ".view.php"
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


        'With objWriter
        '    .WriteLine("<?php require_once('../Model/Specialisation.class.php'); ?>")
        '    .WriteLine("<?php require_once('../DAL/MySQL_Helper.class.php'); ?>")
        '    .WriteLine("<?php require_once('../ServTech/ValidationFunction.class.php'); ?>")
        '    .WriteLine("<?php require_once('../ServTech/ApplicationHelper.php'); ?>")
        '    .WriteLine("<?php require_once('../ServTech/QueryConnexion.php'); ?>")
        'End With

        'With objWriter
        '    .WriteLine("<?php")
        '    .WriteLine("//if (!isset($_SESSION[$GLOBAL_SESSION])) {")
        '    .WriteLine("//    ApplicationHelper::Refresh_MainPage();")
        '    .WriteLine("//}")
        '    .WriteLine("$PageName = 'frm_Specialisation.view.php';")
        '    .WriteLine("$Btn_Save = 'btnSave';")
        '    .WriteLine("$Btn_Annuler = 'btnCancel';")
        '    .WriteLine("//        if( !isset($_SESSION[$GLOBAL_SESSION]) )")
        '    .WriteLine("//	{")
        '    .WriteLine("//		Cls_Parameters::Refresh_MainPage();")
        '    .WriteLine("//	}else{")
        '    .WriteLine("//		$Is_Acces_Page = 'display:block;';")
        '    .WriteLine("//		if(!Select_Privilege($PageName,$_SESSION[$GLOBAL_SESSION_GROUPE],$ConString_MySql))")
        '    .WriteLine("//		{// No acces a la page")
        '    .WriteLine("//			Cls_Parameters::Dialog_PopUP($NO_ACCES_PAGE);")
        '    .WriteLine("//			$_message = Cls_Parameters::Show_Message($NO_ACCES_PAGE);")
        '    .WriteLine("//			$Is_Acces_Page = 'display:none;';")
        '    .WriteLine("//		}else{")
        '    .WriteLine("//			$Btn_Save = (Select_Privilege($Btn_AddEdit,$_SESSION[$GLOBAL_SESSION_GROUPE],$ConString_MySql)) ? '' : 'display:none;' ;")
        '    .WriteLine("//		}//")
        '    .WriteLine("//	}//   ")
        '    .WriteLine("?>")

        'End With

        'With objWriter
        '    .WriteLine("<?php")
        '    .WriteLine("$ID = (isset($_GET[""ID""])) ? $_GET[""ID""] : '0';")


        '    For i As Int32 = 1 To cols.Count - 3
        '        Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
        '        If ListofForeignKey.Contains(cols(i)) Then
        '            objWriter.WriteLine("$txt_" & columnToUse & " = ValidationFunction::test_input($_POST['txt_" & columnToUse & "']);")
        '        ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
        '            objWriter.WriteLine("$txt_" & columnToUse & " = ValidationFunction::test_input($_POST['txt_" & columnToUse & "']);")
        '        ElseIf types(i) = "Boolean" Then
        '            objWriter.WriteLine("$txt_" & columnToUse & " = ValidationFunction::test_input($_POST['txt_" & columnToUse & "']);")
        '        Else
        '            objWriter.WriteLine("$txt_" & columnToUse & " = ValidationFunction::test_input($_POST['txt_" & columnToUse & "']);")
        '        End If
        '    Next


        '    .WriteLine("if (!isset($_SESSION[$GLOBAL_SESSION])) {")
        '    .WriteLine("//	$_SESSION[$GLOBAL_SESSION_PAGENAME] = ""../Administration/Entreprise.php"";")
        '    .WriteLine("//	Cls_Parameters::Refresh_MainPage();")
        '    .WriteLine("}")
        '    .WriteLine("$IsPostBack = false;")
        '    .WriteLine("if ($_POST[""btnCancel""] == ""Abandonner"") {")
        '    .WriteLine("ApplicationHelper::Refresh_MainPage(false);")
        '    .WriteLine("}//")
        '    .WriteLine("if (isset($_POST['btnSave'])) {")

        '    Dim saveString As String = "$ID"
        '    For i As Int32 = 1 To cols.Count - 3
        '        Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
        '        If ListofForeignKey.Contains(cols(i)) Then
        '            saveString = saveString & ", txt_" & columnToUse
        '        ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
        '            saveString = saveString & ", txt_" & columnToUse
        '        ElseIf types(i) = "Boolean" Then
        '            saveString = saveString & ", txt_" & columnToUse
        '        Else
        '            saveString = saveString & ", txt_" & columnToUse
        '        End If
        '    Next
        '    .WriteLine("btnSaveClick(" & saveString & ");")


        '    .WriteLine("}")
        '    .WriteLine("if (!$IsPostBack) {")
        '    .WriteLine("if (isset($_GET['ID']) && isset($_GET['Action']) && is_numeric($_GET['ID'])) {")
        '    .WriteLine("global $ID;")
        '    .WriteLine("try {")
        '    .WriteLine("switch ($_GET['Action']) {")
        '    .WriteLine("case 'Edit': {")
        '    .WriteLine("$obj = new" & nomSimple & "($ID);")

        '    For i As Int32 = 1 To cols.Count - 3
        '        Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
        '        If ListofForeignKey.Contains(cols(i)) Then
        '            .WriteLine("$txt" & columnToUse & " = $obj->get_" & columnToUse & "();")
        '        ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
        '            .WriteLine("$txt" & columnToUse & " = $obj->get_" & columnToUse & "();")
        '        ElseIf types(i) = "Boolean" Then
        '            .WriteLine("$txt" & columnToUse & " = $obj->get_" & columnToUse & "();")
        '        Else
        '            .WriteLine("$txt" & columnToUse & " = $obj->get_" & columnToUse & "();")
        '        End If
        '    Next



        '    .WriteLine("                    break;")
        '    .WriteLine("}")
        '    .WriteLine("default: ")
        '    .WriteLine("}")
        '    .WriteLine("} catch (Exception $ex) {")
        '    .WriteLine("$_message = $ex->getMessage();	")
        '    .WriteLine("ApplicationHelper::Dialog_PopUP($_message);")
        '    .WriteLine("	    $_message = ApplicationHelper::Show_Message($_message,'S','../../');")
        '    .WriteLine("}")
        '    .WriteLine("}")
        '    .WriteLine("}")


        '    saveString = "$ID"
        '    For i As Int32 = 1 To cols.Count - 3
        '        Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
        '        If ListofForeignKey.Contains(cols(i)) Then
        '            saveString = saveString & ", txt_" & columnToUse
        '        ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
        '            saveString = saveString & ", txt_" & columnToUse
        '        ElseIf types(i) = "Boolean" Then
        '            saveString = saveString & ", txt_" & columnToUse
        '        Else
        '            saveString = saveString & ", txt_" & columnToUse
        '        End If
        '    Next

        '    .WriteLine("function btnSaveClick(" & saveString & ") { ")


        '    .WriteLine("$IsPostBack = true;")
        '    .WriteLine("try {")
        '    .WriteLine("Validation();")


        '    saveString = "$ID"
        '    For i As Int32 = 1 To cols.Count - 3
        '        Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
        '        If ListofForeignKey.Contains(cols(i)) Then
        '            saveString = saveString & ", txt_" & columnToUse
        '        ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
        '            saveString = saveString & ", txt_" & columnToUse
        '        ElseIf types(i) = "Boolean" Then
        '            saveString = saveString & ", txt_" & columnToUse
        '        Else
        '            saveString = saveString & ", txt_" & columnToUse
        '        End If
        '    Next

        '    .WriteLine("Save(" & saveString & ");")




        '    .WriteLine("} catch (Exception $ex) {")
        '    .WriteLine("$_message = $ex->getMessage();")
        '    .WriteLine("ApplicationHelper::Dialog_PopUP($_message);")
        '    .WriteLine("$_message = ApplicationHelper::Show_Message($_message, 'S');")
        '    .WriteLine("}")
        '    .WriteLine("}")

        '    .WriteLine("function Validation() {")

        '    .WriteLine("global " & saveString & ";")


        '    For i As Int32 = 1 To cols.Count - 3
        '        Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
        '        If ListofForeignKey.Contains(cols(i)) Then
        '            .WriteLine("if ($txt_" & columnToUse & " == '') {")
        '            .WriteLine("throw (new Exception(""le " & columnToUse & " est obligatoire!""));")
        '            .WriteLine("}")
        '        ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
        '            .WriteLine("if ($txt_" & columnToUse & " == '') {")
        '            .WriteLine("throw (new Exception(""le " & columnToUse & " est obligatoire!""));")
        '            .WriteLine("}")
        '        ElseIf types(i) = "Boolean" Then
        '            .WriteLine("if ($txt_" & columnToUse & " == '') {")
        '            .WriteLine("throw (new Exception(""le " & columnToUse & " est obligatoire!""));")
        '            .WriteLine("}")
        '        Else
        '            .WriteLine("if ($txt_" & columnToUse & " == '') {")
        '            .WriteLine("throw (new Exception(""le " & columnToUse & " est obligatoire!""));")
        '            .WriteLine("}")
        '        End If
        '    Next


        '    .WriteLine("}")



        '    .WriteLine("function Save() {")

        '    .WriteLine("global " & saveString & ";")
        '    .WriteLine("$obj = new " & nomSimple & "($ID);")



        '    For i As Int32 = 1 To cols.Count - 3
        '        Dim columnToUse As String = cols(i).Substring(1, cols(i).Length - 1)
        '        If ListofForeignKey.Contains(cols(i)) Then
        '            .WriteLine("$obj->set_" & columnToUse & "($txt_" & columnToUse & ");")
        '        ElseIf types(i) = "Date" Or types(i) = "DateTime" Then
        '            .WriteLine("$obj->set_" & columnToUse & "($txt_" & columnToUse & ");")
        '        ElseIf types(i) = "Boolean" Then
        '            .WriteLine("$obj->set_" & columnToUse & "($txt_" & columnToUse & ");")
        '        Else
        '            .WriteLine("$obj->set_" & columnToUse & "($txt_" & columnToUse & ");")
        '        End If
        '    Next

        '    .WriteLine("$obj->Save(""Admin"");")
        '    .WriteLine("ApplicationHelper::Dialog_PopUP(""Sauvegarde efféctuée"");")
        '    .WriteLine("ApplicationHelper::Refresh_MainPage();")
        '    .WriteLine("}")


        '    .WriteLine("?>")


        'End With


        objWriter.WriteLine("<!DOCTYPE html PUBLIC ""-//W3C// DTD XHTML 1.0 Transitional//EN"" ""http://www.w3org/TR/XHTML1/DTD/xhtml1-transitional.dtd ""  ")
        objWriter.WriteLine("<html xmlns=""http://www.w3.org/1999/xhtml"">  ")
        objWriter.WriteLine("<head>")

        objWriter.WriteLine("<meta http-equiv= ""Content-Type"" content=""text/html; charset=utf-8"" />")

        objWriter.WriteLine("<link rel=""shortcut icon"" href=""icon.ico"" />")
        objWriter.WriteLine("<link rel=""stylesheet"" type=""text/css"" href=""style/style.css"" />")
        objWriter.WriteLine("<link rel=""stylesheet"" type=""text/css"" href=""style/style.css"" />")

        objWriter.WriteLine("</head>")

        objWriter.WriteLine("<body>")
        objWriter.WriteLine("<center>")
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
        objWriter.WriteLine(" <label ID=""lbl_title"" class=""popupFormTitleLabel""   text=""Gestion des " & nomSimple & """   runat=""server""></label>")
        objWriter.WriteLine("</h1>")
        objWriter.WriteLine("</td>")
        objWriter.WriteLine("</tr>")

        objWriter.WriteLine("<tr valign=""top"">")
        objWriter.WriteLine("<td style=""width: 100%;"" colspan=""2"" valign=""top"">")
        objWriter.WriteLine("<hr class=""hrHeaderAfterPhoto"" />")
        objWriter.WriteLine("</td>")
        objWriter.WriteLine("</tr>")


        objWriter.WriteLine("<tr valign=""top"">")
        objWriter.WriteLine("<td valign=""top"" style=""width: 100%;"" colspan=""2"">")

        objWriter.WriteLine("<form name=""frm_""" & nomSimple & " method=""post"" action="" frm_""" & nomSimple & ".php"" >")



        objWriter.WriteLine(" <table border=""0"" class=""form_Enregistrement"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")
        objWriter.WriteLine("<tr valign=""top"">")
        objWriter.WriteLine(" <td style=""width: 100%;"">")
        objWriter.WriteLine("<table border=""0"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")



        Dim countColumn As Integer = 0
        For Each column As Cls_Column In _table.ListofColumn
            Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
            Dim primary As String = _table.ListofColumn(0).Name
            If countColumn < _table.ListofColumn.Count - 4 Then

                If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then

                    Dim columnName As String = column.Name.Substring(ForeinKeyPrefix.Length, column.Name.Length - (ForeinKeyPrefix.Length))
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & columnName & """ class=""popupFormInputLabel"" "">" & columnName & ": </label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadComboBox ID=""rcmb_" & columnName & """  Filter=""Contains""  AutoPostBack=""true""  Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadComboBox> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType <> "bit" Then
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & column.Name & """ class=""popupFormInputLabel"">" & column.Name & ": </label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<input type=""text"" id=""txt_" & column.Name & """ name=""txt_" & column.Name & """  value=""<?php echo $txt_" & column.Name & "; ?>""  Width=""50%"" > " & Chr(13) & " </input> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                ElseIf column.TrueSqlServerType = "date" And column.TrueSqlServerType = "datetime" And column.Name <> _table.ListofColumn(0).Name Then
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & column.Name & """ class=""popupFormInputLabel"" "">" & column.Name & ": </label> ")
                    objWriter.WriteLine("</td>")

                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadDatePicker ID=""rdp_" & column.Name & """ Width=""50%"" runat=""server""" & _
                                     "DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01""  ToolTip=""Cliquer sur le bouton pour choisir une date"" > " & Chr(13) & " </telerik:RadDatePicker>  ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")

                ElseIf column.TrueSqlServerType = "bit" Then
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & column.Name & """ class=""popupFormInputLabel"" "">" & column.Name & ": </label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<input type=""checkbox"" name=""chk_" & column.Name & """ width=""50%"" >" & _
                                      " </input>  ")
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
        objWriter.WriteLine("<input type=""submit"" name=""btnSave"" value=""Sauvegarder"" > ")
        objWriter.WriteLine("</input>")
        objWriter.WriteLine("&nbsp;")
        objWriter.WriteLine("<input type=""submit"" name=""btnCancel"" value= ""Abandonner"" >")
        objWriter.WriteLine("</input>")

        objWriter.WriteLine("</td>")

        objWriter.WriteLine("</tr>")
        objWriter.WriteLine(" </table>")
        objWriter.WriteLine(" </td>")
        objWriter.WriteLine("</tr>")
        objWriter.WriteLine("</table>")

        objWriter.WriteLine("</form>")
        objWriter.WriteLine("  </td>")
        objWriter.WriteLine("</tr>")
        objWriter.WriteLine("</table>")
        objWriter.WriteLine("</td>")
        objWriter.WriteLine("</tr>")
        objWriter.WriteLine("</table>")

        objWriter.WriteLine("<br />")
        objWriter.WriteLine("</center>")
        objWriter.WriteLine("<input type=""hidden"" id=""txt_CodeHID" & nomSimple & """ name=""txt_" & nomSimple & """  value=""0""  width=""0px"" > " & Chr(13) & " </input> ")

        objWriter.WriteLine("</body>")

        objWriter.WriteLine()
        objWriter.Close()
    End Sub

    Public Shared Sub CreateGridPhpForListing(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_libraryname As TextBox, ByVal databasename As String)
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
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\PHPHTMLWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\PHPHTMLWebForm\")
        Dim path As String = txt_PathGenerate_Script & nomWebform & "Grid.php"

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
        ' Dim content As String = "Partial Class " & nomWebform & "Listing" & Chr(13) _
        '                         & " Inherits BasePage"

        Dim content As String = ""
        _end = "End Class" & Chr(13)
        _end = ""
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
        'If ListBox_NameSpace.Items.Count > 0 Then
        '    For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
        '        objWriter.WriteLine(ListBox_NameSpace.Items(i))
        '    Next
        'End If
        'Dim libraryname As String = "Imports " & txt_libraryname.Text
        '  objWriter.WriteLine("Imports Telerik.Web.UI")
        ' objWriter.WriteLine(libraryname)
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

        With objWriter
            .WriteLine(" <?php")
            .WriteLine("require_once('../Model/" & nomSimple & ".class.php');")
            .WriteLine("require_once('../Persistance/MySQL_Helper.class.php');")
            .WriteLine("require_once('../GridHelper/grid.php');")
            .WriteLine("require_once('../ServTech/PaginationHelper.php');")
            .WriteLine("require_once('../ServTech/ApplicationHelper.php');")
            .WriteLine("?>")
        End With

        Dim countColumn As Integer = 0

        With objWriter
            .WriteLine("<div id=""gridContainer"" class=""divEncadrementGridPrime"">")
            .WriteLine("<div  id=""dialog"" title=""Nouvelle " & nomSimple & """ ></div>")

            .WriteLine("   <script type=""text/javascript"">")
            .WriteLine(" var i = 0;")
            .WriteLine(" //        $(function() {")
            .WriteLine(" //            $(""#dialog"").dialog({")
            .WriteLine(" //                autoOpen: false,")
            .WriteLine(" //                height: 230,")
            .WriteLine(" //                width: 500,")
            .WriteLine(" //                modal: true});")
            .WriteLine(" //            $(""#btnAdd" & nomSimple & """).click(function() {")
            .WriteLine(" //                $(""#dialog"").load('frm_" & nomSimple & ".view.php').dialog(""open"");")
            .WriteLine(" //            });")
            .WriteLine(" //        });")

            .WriteLine("         function modalWin() {")
            .WriteLine(" if (window.showModalDialog) {")
            .WriteLine(" window.showModalDialog(""frm_" & nomSimple & ".view.php"", ""name"",")
            .WriteLine(" ""dialogWidth:800px;dialogHeight:250px"");")
            .WriteLine(" } else {")
            .WriteLine(" window.open('frm_" & nomSimple & ".view.php', 'name',")
            .WriteLine(" 'height=800,width=250,toolbar=no,directories=no,status=no,\n\")
            .WriteLine(" menubar = no, scrollbars = no, resizable = no, modal = yes');")
            .WriteLine(" }")
            .WriteLine(" }")
            .WriteLine(" function editRow(id) {")
            .WriteLine(" $(""#dialog"").load('frm_" & nomSimple & "" & nomSimple & "" & nomSimple & ".view.php?ID=' + id).dialog(""open"");")
            .WriteLine(" }")
            .WriteLine(" function changePage(value, currentPage) {")


            Dim varcols As String = "var columnId"
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    varcols = varcols + ", column" + column.Name
                End If
                countColumn = countColumn + 1
            Next

            varcols = varcols + ";"

            .WriteLine(varcols)

            .WriteLine(" var elementPerPage, elementToSkyp, firstPaginator, prevPaginator, nextPaginator, lastPaginator, totalPage;")
            .WriteLine(" totalPage = document.getElementById(""totalPage"").value;")
            .WriteLine(" columnId = document.getElementById(""columnId"").value;")


            '  .WriteLine(" columnDescription = document.getElementById(""columnDescription"").value;")

            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" column" & column.Name & " = document.getElementById(""column" & column.Name & """).value;")
                End If
                countColumn = countColumn + 1
            Next


            .WriteLine(" switch (value) {")
            .WriteLine(" case 1:")
            .WriteLine(" document.getElementById(""firstPaginator"").value = 1;")
            .WriteLine(" firstPaginator = 1;")
            .WriteLine(" prevPaginator = 0;")
            .WriteLine(" nextPaginator = 0;")
            .WriteLine(" lastPaginator = 0;")
            .WriteLine(" break;")
            .WriteLine(" case 2:")
            .WriteLine(" document.getElementById(""prevPaginator"").value = 1;")
            .WriteLine(" firstPaginator = 0;")
            .WriteLine(" prevPaginator = 1;")
            .WriteLine(" nextPaginator = 0;")
            .WriteLine(" lastPaginator = 0;")
            .WriteLine(" break;")
            .WriteLine(" case 3:")
            .WriteLine(" document.getElementById(""nextPaginator"").value = 1;")
            .WriteLine(" firstPaginator = 0;")
            .WriteLine(" prevPaginator = 0;")
            .WriteLine(" nextPaginator = 1;")
            .WriteLine(" lastPaginator = 0;")
            .WriteLine(" break;")
            .WriteLine(" case 4:")
            .WriteLine(" document.getElementById(""lastPaginator"").value = 1;")
            .WriteLine(" firstPaginator = 0;")
            .WriteLine(" prevPaginator = 0;")
            .WriteLine(" nextPaginator = 0;")
            .WriteLine(" lastPaginator = 1;")
            .WriteLine(" break;")
            .WriteLine(" default:")
            .WriteLine(" firstPaginator = 0;")
            .WriteLine(" prevPaginator = 0;")
            .WriteLine(" nextPaginator = 0;")
            .WriteLine(" lastPaginator = 0;")
            .WriteLine(" break;")
            .WriteLine(" }")

            .WriteLine("             elementPerPage = document.getElementById(""ddl_Paginator"").value;")
            .WriteLine(" elementToSkyp = elementPerPage * (currentPage - 1);")
            .WriteLine(" getTagContent('frm_" & nomSimple & "Grid.php?' +")
            .WriteLine(" 'elementPerPage=' + elementPerPage +")
            .WriteLine(" '&elementToSkyp=' + elementToSkyp +")
            .WriteLine(" '&currentPage=' + currentPage +")
            .WriteLine(" '&firstPaginator=' + firstPaginator +")
            .WriteLine(" '&prevPaginator=' + prevPaginator +")
            .WriteLine(" '&nextPaginator=' + nextPaginator +")
            .WriteLine(" '&lastPaginator=' + lastPaginator +")
            .WriteLine(" '&columnId=' + columnId +")
            '.WriteLine(" '&columnCode=' + columnCode +")
            '.WriteLine(" '&columnDescription=' + columnDescription +")

            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" '&column" & column.Name & "=' + column" & column.Name & "+")
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine(" '', 'gridContainer');")
            .WriteLine(" }")
            .WriteLine(" function orderColumn(value, currentPage) {")
            .WriteLine(varcols)
            .WriteLine(" var elementPerPage, elementToSkyp, firstPaginator, prevPaginator, nextPaginator, lastPaginator, totalPage;")
            .WriteLine(" totalPage = document.getElementById(""totalPage"").value;")



            .WriteLine(" columnId = document.getElementById(""columnId"");")



            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" column" & column.Name & " = document.getElementById(""column" & column.Name & """).value;")
                End If
                countColumn = countColumn + 1
            Next


            .WriteLine("             firstPaginator = document.getElementById(""firstPaginator"").value;")
            .WriteLine(" prevPaginator = document.getElementById(""prevPaginator"").value;")
            .WriteLine(" nextPaginator = document.getElementById(""nextPaginator"").value;")
            .WriteLine(" lastPaginator = document.getElementById(""lastPaginator"").value;")
            .WriteLine(" switch (value) {")
            .WriteLine(" case 1:")
            .WriteLine(" if (columnId.value == ""NONE"") {")
            .WriteLine(" columnId.value = ""ASC"";")



            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" column" & column.Name & ".value = ""NONE"";")
                End If
                countColumn = countColumn + 1
            Next



            .WriteLine(" } else if (columnId.value == ""ASC"") {")
            .WriteLine(" columnId.value = ""DESC"";")

            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" column" & column.Name & ".value = ""NONE"";")
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine(" }")
            .WriteLine(" else if (columnId.value == ""DESC"") {")

            .WriteLine(" columnId.value = ""ASC"";")

            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" column" & column.Name & ".value = ""NONE"";")
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine(" }")
            .WriteLine(" break;")


            countColumn = 2
            Dim compteurCol As Integer = 2
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" case " & compteurCol & ":")
                    .WriteLine(" if (column" & column.Name & ".value == ""NONE"") {")
                    .WriteLine(" column" & column.Name & ".value = ""ASC"";")
                    .WriteLine(" columnId.value = ""NONE"";")
                    For Each columninner As Cls_Column In _table.ListofColumn
                        If countColumn < _table.ListofColumn.Count - 4 Then
                            If columninner.Name <> column.Name Then
                                .WriteLine(" column" & columninner.Name & "= ""NONE"";")
                            End If
                        End If
                    Next
                    compteurCol = compteurCol + 1
                    .WriteLine(" } else if (column" & column.Name & ".value == ""ASC"") {")
                    .WriteLine(" column" & column.Name & ".value = ""DESC"";")
                    .WriteLine(" columnId.value = ""NONE"";")
                    '    compteurCol = 0
                    For Each columninner As Cls_Column In _table.ListofColumn
                        If countColumn < _table.ListofColumn.Count - 4 Then
                            If columninner.Name <> column.Name Then
                                .WriteLine(" column" & columninner.Name & "= ""NONE"";")
                            End If
                        End If
                    Next

                    .WriteLine(" }")

                    .WriteLine(" else if (column" & column.Name & ".value == ""DESC"") {")
                    .WriteLine(" column" & column.Name & ".value = ""ASC"";")
                    .WriteLine(" columnId.value = ""NONE"";")
                    ' compteurCol = 0
                    For Each columninner As Cls_Column In _table.ListofColumn
                        If countColumn < _table.ListofColumn.Count - 4 Then
                            If columninner.Name <> column.Name Then
                                .WriteLine(" column" & columninner.Name & "= ""NONE"";")
                            End If
                        End If
                    Next
                    .WriteLine(" }")
                End If
                countColumn = countColumn + 1
                .WriteLine(" break;")
                .WriteLine(" }")
            Next


            .WriteLine(" elementPerPage = document.getElementById(""ddl_Paginator"").value;")
            .WriteLine(" elementToSkyp = elementPerPage * (currentPage - 1);")

            .WriteLine(" getTagContent('frm_SpecialisationGrid.php?' +")
            .WriteLine(" 'elementPerPage=' + elementPerPage +")
            .WriteLine(" '&elementToSkyp=' + elementToSkyp +")
            .WriteLine(" '&currentPage=' + currentPage +")
            .WriteLine(" '&firstPaginator=' + firstPaginator +")
            .WriteLine(" '&prevPaginator=' + prevPaginator +")
            .WriteLine(" '&nextPaginator=' + nextPaginator +")
            .WriteLine(" '&lastPaginator=' + lastPaginator +")
            .WriteLine(" '&columnId=' + columnId.value +")


            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" '&column" & column.Name & "=' + column" & column.Name & ".value +")
                End If
                countColumn = countColumn + 1
            Next


            .WriteLine(" '', 'gridContainer');")
            .WriteLine(" //                  document.forms[""form1""].submit();")
            .WriteLine(" }")

            .WriteLine(" </script>")


            .WriteLine("<div class=""divHeaderEncadrementGridPrime"">")
            .WriteLine(" Liste des " & nomSimple & "")
            .WriteLine("</div>")
            .WriteLine(" <div class=""divBodyEncadrementGridPrime"">")
            .WriteLine("<table style=""border:1px solid #A8A8A8; width: 100%;"" cellspacing=""0"">")
        End With


        countColumn = 0
        With objWriter
            .WriteLine(" <?php")
            .WriteLine(" $ElementPerPage = 5;")
            .WriteLine("  $elementToSkyp = 0;")
            .WriteLine(" $currentPage = 1;")
            .WriteLine(" $paginatorFromTo = """";")
            .WriteLine(" $columnId = ""NONE"";")
            .WriteLine(" $theadColumnId = ""gridPrimeth"";")
            .WriteLine(" $spanId = ""gridPrimethSpanSortNONE"";")


            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine("  $column" & column.Name & " = ""NONE"";")
                    .WriteLine("  $theadColumn" & column.Name & "  = ""gridPrimeth"";")
                    .WriteLine("  $span" & column.Name & "  = ""gridPrimethSpanSortNONE"";")
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine(" $columnToSort = """";")
            .WriteLine(" $sortingValue = ""NONE"";")
            .WriteLine(" $firstPaginator = 0;")
            .WriteLine(" $prevPaginator = 0;")
            .WriteLine("  $nextPaginator = 0;")
            .WriteLine(" $lastPaginator = 0;")
            .WriteLine(" $totalPage = 0;")

            .WriteLine("if (!empty($_GET)) {")
            .WriteLine("   $ElementPerPage = filter_input(INPUT_GET, 'elementPerPage', FILTER_VALIDATE_INT);")
            .WriteLine("   $elementToSkyp = filter_input(INPUT_GET, 'elementToSkyp', FILTER_VALIDATE_INT);")
            .WriteLine("   $currentPage = filter_input(INPUT_GET, 'currentPage', FILTER_VALIDATE_INT);")
            .WriteLine("  $firstPaginator = filter_input(INPUT_GET, 'firstPaginator', FILTER_VALIDATE_INT);")
            .WriteLine("   $prevPaginator = filter_input(INPUT_GET, 'prevPaginator', FILTER_VALIDATE_INT);")
            .WriteLine("  $nextPaginator = filter_input(INPUT_GET, 'nextPaginator', FILTER_VALIDATE_INT);")
            .WriteLine(" $lastPaginator = filter_input(INPUT_GET, 'lastPaginator', FILTER_VALIDATE_INT);")
            .WriteLine("  $columnId = filter_input(INPUT_GET, 'columnId', FILTER_SANITIZE_STRING);")


            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine("  $column" & column.Name & " = filter_input(INPUT_GET, 'column" & column.Name & "', FILTER_SANITIZE_STRING);")
                End If
                countColumn = countColumn + 1
            Next
            .WriteLine(" }")
        End With
        With objWriter


            .WriteLine("ParametizeGrid();")

        End With

        With objWriter
            .WriteLine("function ParametizeGrid() {")
            .WriteLine(" global $ElementPerPage, $currentPage, $elementToSkyp, $paginatorFromTo, $columnToSort, $sortingValue;")
            .WriteLine(" global $firstPaginator, $prevPaginator, $nextPaginator, $lastPaginator, $totalPage;")

            countColumn = 0
            Dim columnStr As String = " global $columnId "
            Dim spanStr As String = " global $spanId "
            Dim theadColumnStr As String = " global $theadColumnId "

            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    columnStr = columnStr & ", $column" & column.Name
                    spanStr = spanStr & ", $span" & column.Name
                    theadColumnStr = theadColumnStr & ", $theadColumn" & column.Name

                End If
                countColumn = countColumn + 1
            Next

            .WriteLine("" & columnStr & ";")
            .WriteLine("" & spanStr & ";")
            .WriteLine("" & theadColumnStr & ";")

            .WriteLine(" $paginatorFromTo = ($elementToSkyp + 1) . "" of "" . $ElementPerPage;")
            .WriteLine("  $objs_count = " & nomSimple & "::SearchAll();")
            .WriteLine("$helper = new PaginationHelper(count($objs_count), $ElementPerPage);")
            .WriteLine(" $totalPage = $helper->numberOfPage;")

            .WriteLine(" if ($columnId != ""NONE"") {")
            .WriteLine("    $columnToSort = """ & _table.ListofColumn.Item(0).Name & """;")
            .WriteLine("    $spanId = ($columnId == ""ASC"") ? ""gridPrimethSpanSortASC"" : ""gridPrimethSpanSortDESC"";")
            .WriteLine("    $theadColumnId = ($columnId == ""ASC"") ? ""gridPrimethSortASC"" : ""gridPrimethSortDESC"";")
            .WriteLine("   $sortingValue = $columnId;")
            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" } else if ($column" & column.Name & " != ""NONE"") {")
                    .WriteLine("     $columnToSort = """ & column.Name & """;")
                    .WriteLine("     $span" & column.Name & " = ($column" & column.Name & " == ""ASC"") ? ""gridPrimethSpanSortASC"" : ""gridPrimethSpanSortDESC"";")
                    .WriteLine("     $theadColumn" & column.Name & " = ($column" & column.Name & " == ""ASC"") ? ""gridPrimethSortASC"" : ""gridPrimethSortDESC"";")
                    .WriteLine("    $sortingValue = $column" & column.Name & ";")
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine(" }")

            .WriteLine("  if ($firstPaginator == 1) {")
            .WriteLine("     $currentPage = 1;")
            .WriteLine("     $elementToSkyp = 0;")
            .WriteLine("  } else if ($prevPaginator == 1) {")
            .WriteLine("     if ($currentPage != 1) {")
            .WriteLine("          $currentPage = $currentPage - 1;")
            .WriteLine("     }")
            .WriteLine("  } else if ($nextPaginator == 1) {")
            .WriteLine("      if ($currentPage != $helper->numberOfPage) {")
            .WriteLine("         $currentPage = $currentPage + 1;")
            .WriteLine("      }")
            .WriteLine("  } else if ($lastPaginator == 1) {")
            .WriteLine("      $currentPage = $helper->numberOfPage;")
            .WriteLine(" } else {")
            .WriteLine("      $currentPage = 1;")
            .WriteLine("   }")

            .WriteLine("   $elementToSkyp = $ElementPerPage * ($currentPage - 1);")
            .WriteLine("   $paginatorFromTo = ""("" . ($currentPage) . "" of "" . $helper->numberOfPage . ("")"");")
            .WriteLine("     $i = 0;")
            .WriteLine("    $couleur = '';")
            .WriteLine("}")

            .WriteLine(" $objs = " & nomSimple & "::SearchAll_ForPagination($elementToSkyp, $ElementPerPage, $columnToSort, $sortingValue);")
            .WriteLine(" ?>")

        End With

        With objWriter
            .WriteLine("")

            .WriteLine(" <thead>")
            .WriteLine(" <tr class=""gridPrime"">")
            .WriteLine(" <th class=""<?php echo ""$theadColumnId"" ?>"" onclick=""orderColumn(1,<?php echo ""$currentPage"" ?>);""")
            .WriteLine("    align=""left"" scope='col'>Id")
            .WriteLine("  <span class=""<?php echo $spanId ?>""></span>")
            .WriteLine(" <input type=""hidden"" id=""columnId"" name=""columnId""")
            .WriteLine("      value=""<?php echo ""$columnId"" ?>""/>")
            .WriteLine(" </th>")

            countColumn = 0

            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" <th class=""<?php echo ""$theadColumn" & column.Name & """ ?>"" onclick=""orderColumn(" & countColumn + 1 & ",<?php echo ""$currentPage"" ?>);""")
                    .WriteLine("    align=""left"" scope='col'>" & column.Name & "")
                    .WriteLine("  <span class=""<?php echo $span" & column.Name & " ?>""></span>")
                    .WriteLine(" <input type=""hidden"" id=""column" & column.Name & """ name=""column" & column.Name & """")
                    .WriteLine("      value=""<?php echo ""$column" & column.Name & """ ?>""/>")
                    .WriteLine(" </th>")
                End If
                countColumn = countColumn + 1
            Next


            .WriteLine(" <th class=""gridPrimeth"" scope='col' style=""width:60px; white-space:nowrap;"">Actions</th>")
            .WriteLine(" </tr>")
            .WriteLine(" </thead>")

        End With

        With objWriter
            .WriteLine("<?php")

            .WriteLine("foreach ($objs as $obj) {")
            .WriteLine("  $id = $obj->get_ID();")

            countColumn = 0

            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine(" $" & column.Name & " = $obj->get_" & column.Name & " ();"" ")
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine("   if ($i % 2 == 0) {")
            .WriteLine("     $couleur = '#EBEFF3';")
            .WriteLine("  } else {")
            .WriteLine("    $couleur = '#FCFCFC';")
            .WriteLine("}")
            .WriteLine(" $i++;")
            .WriteLine(" ?>")


            .WriteLine(" <tr style=""height: 30px"" bgcolor='<?php echo $couleur; ?>'")
            .WriteLine(" onclick = ""this.style.cursor = 'hand';""")
            .WriteLine("                    this.style.backgroundColor = '#DFEDFD'""")
            .WriteLine(" onmouseout = ""this.style.backgroundColor = '<?php echo $couleur; ?>'""")
            .WriteLine("        >")

            .WriteLine("<td style=""padding: 4px 10px;"">>")
            .WriteLine("       <label type=""label"" style=""font-size: 15px; font-family: Cambria;""><?php echo ""$id""; ?></label>>")
            .WriteLine(" </td>>")

            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine("<td style=""padding: 4px 10px;"">")
                    .WriteLine("<td style=""padding: 4px 10px;"">")
                    .WriteLine("   <label type=""label"" style=""font-size: 15px; font-family: Cambria;""><?php echo ""$" & column.Name & """; ?></label>")
                    .WriteLine("</td>")
                End If
                countColumn = countColumn + 1
            Next


            .WriteLine("<td style=""padding: 4px 10px;"" nowrap=""nowrap"" align=""center"">&nbsp;&nbsp;")
            .WriteLine(" <td style=""padding: 4px 10px;"" nowrap=""nowrap"" align=""center"">&nbsp;&nbsp;")
            .WriteLine("     <a href=""javascript:Open_Window('frm_" & nomSimple & ".view.php?ID=<?php echo $id; ?>&Action=Edit','frm_" & nomSimple & ".view.php','800','500');""")
            .WriteLine("      title=""Modifier"">")
            .WriteLine("     <img alt=""Edit"" src=""../webroot/img/img_edit_row.png"" style=""width: 16px; cursor: pointer;""")
            .WriteLine("           border=""0""/></a>&nbsp;&nbsp;")
            .WriteLine("  <a href=""javascript:Open_Window('Equipes_Add.php?ID=<?php echo $id; ?>&Action=Delete','AddEditUtilisateur','800','400');""")
            .WriteLine("     style=""<?php echo $Btn_Delete; ?>"">")
            .WriteLine("     <img src=""../webroot/img/img_delete_row.png"" alt=""Delete"" width=""20"" border=""0""")
            .WriteLine("          style=""width: 16px; cursor: pointer;"" title=""Supprimer""")
            .WriteLine("         onclick="" return confirm('Êtes-vous sûr de vouloir supprimer ?');""/></a>")
            .WriteLine(" </td>")
            .WriteLine("  </tr>")
            .WriteLine(" <?php } // END While                 ?>")
            .WriteLine(" </table>")
            .WriteLine("  </div>")

        End With



        With objWriter
            .WriteLine(" <div class=""divFooterEncadrementGridPrime"">")
            .WriteLine("<span class=""spanCurrentPaginatorPage"">")
            .WriteLine("  <label id=""lbl_CurrentPaginatorPage"">")
            .WriteLine("      <?php echo ""$paginatorFromTo"" ?>")
            .WriteLine("  </label>")
            .WriteLine("  <input id=""currentPage"" type=""hidden"" name=""currentPage"" value=""<?php echo $currentPage ?>""/>")
            .WriteLine(" </span>")

            .WriteLine(" <span")
            .WriteLine("    onclick=""changePage(1,<?php echo ""$currentPage"" ?>);""")
            .WriteLine("    class=""spanPaginatorPage"">")
            .WriteLine("     <span class=""spanFirstPaginatorPageInner"">")
            .WriteLine("        <img style=""margin-top: 3px"" src=""../webroot/img/imgGrid_first.png""/>")
            .WriteLine("       <input id=""firstPaginator"" type=""hidden"" name=""firstPaginator"" value=""0""/>")
            .WriteLine("   </span>")
            .WriteLine("  </span>")
            .WriteLine("  <span")
            .WriteLine("     onclick=""changePage(2,<?php echo ""$currentPage"" ?>);""")
            .WriteLine("     class=""spanPaginatorPage"">")
            .WriteLine("     <span class=""spanPrevPaginatorPageInner"">")
            .WriteLine("         <img style=""margin-top: 3px"" src=""../webroot/img/imgGrid_prev.png""/>")
            .WriteLine("      <input id=""prevPaginator"" type=""hidden"" name=""prevPaginator"" value=""0""/>")
            .WriteLine("    </span>")
            .WriteLine(" </span>")
            .WriteLine(" <span")
            .WriteLine("    onclick=""changePage(3,<?php echo ""$currentPage"" ?>);""")
            .WriteLine("    class=""spanPaginatorPage"">")
            .WriteLine("    <span class=""spanNextPaginatorPageInner"">")
            .WriteLine("        <img style=""margin-top: 3px"" src=""../webroot/img/imgGrid_next.png""/>")
            .WriteLine("        <input id=""nextPaginator"" type=""hidden"" name=""nextPaginator"" value=""0""/>")
            .WriteLine("    </span>")
            .WriteLine(" </span>")
            .WriteLine(" <span")
            .WriteLine("    onclick=""changePage(4,<?php echo ""$currentPage"" ?>);""")
            .WriteLine("   class=""spanPaginatorPage"">")
            .WriteLine("   <span class=""spanLastPaginatorPageInner"">")
            .WriteLine("       <img style=""margin-top: 3px"" src=""../webroot/img/imgGrid_last.png"">")
            .WriteLine("        </img>")
            .WriteLine("        <input id=""lastPaginator"" type=""hidden"" name=""lastPaginator"" value=""0""/>")
            .WriteLine("     </span>")
            .WriteLine(" </span>")
            .WriteLine(" <label type=""hidden"" id=""ddl_PaginatorValue"">Page size:</label>")
            .WriteLine("  <select onchange=""changePage(0,<?php echo ""$currentPage"" ?>);"" id=""ddl_Paginator"" name=""ddl_Paginator""")
            .WriteLine("         class=""selectPaginatorPage"">")
            .WriteLine("            <?php")
            .WriteLine("           $array = array(")
            .WriteLine("                5 => ""5"",")
            .WriteLine("              10 => ""10"",")
            .WriteLine("               25 => ""25"",")
            .WriteLine("             50 => ""50"",")
            .WriteLine("         100 => ""100""")
            .WriteLine("       );")
            .WriteLine("    foreach ($array as $elem) {")
            .WriteLine("          ?>")
            .WriteLine("    <option")
            .WriteLine(" value = ""<?php")
            .WriteLine("      ;")
            .WriteLine("        echo $elem")
            .WriteLine("            ?>""")
            .WriteLine("        <?php")
            .WriteLine("       if ($ElementPerPage == $elem) {")
            .WriteLine("       echo'selected=""selected""';")
            .WriteLine("       }")
            .WriteLine("        ?>>")
            .WriteLine("        <?php echo $elem ?>")
            .WriteLine("   </option>")
            .WriteLine(" <?php } ?>")
            .WriteLine(" <input id=""totalPage"" name=""totalPage"" type=""hidden"" value=""<?php echo ""$totalPage"" ?>""/>")
            .WriteLine("</select>")
            .WriteLine("</div>")
            .WriteLine("</div>")

        End With


        objWriter.WriteLine()
        objWriter.Close()
    End Sub

    Public Shared Sub CreateInterfaceCodePHP(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim Id_table As String = ""
        Dim _end As String
        Dim ListofForeignKey As New List(Of String)
        Dim countForeignKey As Integer = 0
        Dim db As String = ""
        Dim Lcasevalue As New List(Of String) From {"String"}
        Dim nomClasse As String = name.Replace("tbl", "frm").Replace("Tbl", "frm").Replace("TBL", "frm")
        Dim nomSimple As String = name.Substring(4, name.Length - 4)
        Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\PHPHTMLWebForm\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\PHPHTMLWebForm\")
        Dim path As String = txt_PathGenerate_Script & nomClasse & ".php"
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

        Dim countColumn As Integer = 0
        With objWriter
            .WriteLine("<?php require_once('../Model/" & nomSimple & ".class.php'); ?>")
            .WriteLine("<?php require_once('../DAL/MySQL_Helper.class.php'); ?>")
            .WriteLine("<?php require_once('../ServTech/ValidationFunction.class.php'); ?>")
            .WriteLine("<?php require_once('../ServTech/ApplicationHelper.php'); ?>")
        End With

        With objWriter
            .WriteLine("<?php")
            .WriteLine("//if (!isset($_SESSION[$GLOBAL_SESSION])) {")
            .WriteLine("//    ApplicationHelper::Refresh_MainPage();")
            .WriteLine("//}")
            .WriteLine("$PageName = 'frm_Specialisation.view.php';")
            .WriteLine("$Btn_Save = 'btnSave';")
            .WriteLine("$Btn_Annuler = 'btnCancel';")
            .WriteLine("//        if( !isset($_SESSION[$GLOBAL_SESSION]) )")
            .WriteLine("//	{")
            .WriteLine("//		Cls_Parameters::Refresh_MainPage();")
            .WriteLine("//	}else{")
            .WriteLine("//		$Is_Acces_Page = 'display:block;';")
            .WriteLine("//		if(!Select_Privilege($PageName,$_SESSION[$GLOBAL_SESSION_GROUPE],$ConString_MySql))")
            .WriteLine("//		{// No acces a la page")
            .WriteLine("//			Cls_Parameters::Dialog_PopUP($NO_ACCES_PAGE);")
            .WriteLine("//			$_message = Cls_Parameters::Show_Message($NO_ACCES_PAGE);")
            .WriteLine("//			$Is_Acces_Page = 'display:none;';")
            .WriteLine("//		}else{")
            .WriteLine("//			$Btn_Save = (Select_Privilege($Btn_AddEdit,$_SESSION[$GLOBAL_SESSION_GROUPE],$ConString_MySql)) ? '' : 'display:none;' ;")
            .WriteLine("//		}//")
            .WriteLine("//	}//   ")


            .WriteLine("      $ID = (isset($_GET[""ID""])) ? $_GET[""ID""] : '0';")
            .WriteLine("$tmpEditState = false;")
            .WriteLine("if (isset($_GET['ID']) && is_numeric($_GET['ID'])) {")
            .WriteLine("$tmpEditState = true;")
            .WriteLine("}")

            .WriteLine("if (!isset($_SESSION[$GLOBAL_SESSION])) {")
            .WriteLine("//	$_SESSION[$GLOBAL_SESSION_PAGENAME] = ""../Administration/Entreprise.php"";")
            .WriteLine("//	Cls_Parameters::Refresh_MainPage();")
            .WriteLine("}")


            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine("$txt_" & column.Name & " = """";")
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine("$btnSaveInput = 0 ;")
            .WriteLine("$txt_CodeHID = 0;")

            .WriteLine("if ($_SERVER['REQUEST_METHOD'] == 'POST') {")


            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine("$txt_" & column.Name & " = ValidationFunction::test_input(filter_input(INPUT_POST, 'txt_" & column.Name & "', FILTER_SANITIZE_STRING));")
                End If
                countColumn = countColumn + 1
            Next

            
            .WriteLine("if ($_POST[""btnCancel""]) {")
            .WriteLine("ApplicationHelper::Refresh_MainPage(false);")
            .WriteLine("}")
            .WriteLine("if (isset($_POST['btnSave'])) {")
            .WriteLine("frm_" & nomSimple & "::btnSaveClick();")
            .WriteLine("}")
            .WriteLine("} else {")
            .WriteLine("frm_" & nomSimple & "::Setup();")
            .WriteLine("}")
            .WriteLine()

            .WriteLine("class frm_" & nomSimple & " {")
            .WriteLine()
            .WriteLine("static function Setup() {")
            .WriteLine("global $tmpEditState;")

            Dim globalPropert As String = "global $ID"
            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    globalPropert = globalPropert + " , "
                End If
                countColumn = countColumn + 1
            Next
            .WriteLine(globalPropert & ";")

            .WriteLine("if ($tmpEditState) {")
            .WriteLine("try {")
            .WriteLine("$obj = new " & nomSimple & "($ID);")

            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    .WriteLine("$txt_" & column.Name & " = $obj->get_" & column.Name & "();")
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine("} catch (Exception $ex) {")
            .WriteLine("$_message = $ex->getMessage();")
            .WriteLine("ApplicationHelper::Dialog_PopUP($_message);")
            .WriteLine("$_message = ApplicationHelper::Show_Message($_message, 'S', '../../');")
            .WriteLine("}")
            .WriteLine("} else {")

            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine("$txt_" & column.Name & " = """";")
                End If
                countColumn = countColumn + 1
            Next


            .WriteLine("}")
            .WriteLine("}")
            .WriteLine()

            .WriteLine("static function btnSaveClick() {")
            .WriteLine("try {")
            .WriteLine("frm_" & nomSimple & ":: Validation();")
            .WriteLine("frm_" & nomSimple & "::Save();")
            .WriteLine("} catch (Exception $ex) {")
            .WriteLine("$_message = $ex->getMessage();")
            .WriteLine("ApplicationHelper::Dialog_PopUP($_message);")
            .WriteLine("$_message = ApplicationHelper::Show_Message($_message, 'S');")
            .WriteLine("}")
            .WriteLine("}")
            .WriteLine()

            .WriteLine("static function Validation() {")
            .WriteLine(globalPropert & ";")

            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine("if ($txt_" & column.Name & " == NULL) {")
                    .WriteLine("throw (new Exception(""le " & column.Name & " est obligatoire!""));")
                    .WriteLine("}")
                    .WriteLine()
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine("}")


            .WriteLine("static function Save() {")
            .WriteLine(globalPropert & ";")
            .WriteLine("$obj = new " & nomSimple & "($ID);")


            countColumn = 0
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then
                    .WriteLine("$obj->set_" & column.Name & "($txt_" & column.Name & ");")
                    .WriteLine()
                End If
                countColumn = countColumn + 1
            Next

            .WriteLine("$obj->Save(""Blanc"");")
            .WriteLine("ApplicationHelper::Dialog_PopUP(""Sauvegarde Effectuée"");")
            .WriteLine("ApplicationHelper::Refresh_MainPage();")
            .WriteLine("}")
            .WriteLine("}")

            .WriteLine("?>")

        End With




        objWriter.WriteLine("<!DOCTYPE html PUBLIC ""-//W3C// DTD XHTML 1.0 Transitional//EN"" ""http://www.w3org/TR/XHTML1/DTD/xhtml1-transitional.dtd ""  ")
        objWriter.WriteLine("<html xmlns=""http://www.w3.org/1999/xhtml"">  ")
        objWriter.WriteLine("<head>")

        objWriter.WriteLine("<meta http-equiv= ""Content-Type"" content=""text/html; charset=utf-8"" />")

        objWriter.WriteLine("<link rel=""shortcut icon"" href=""icon.ico"" />")
        objWriter.WriteLine("<link rel=""stylesheet"" type=""text/css"" href=""style/style.css"" />")
        objWriter.WriteLine("<link rel=""stylesheet"" type=""text/css"" href=""style/style.css"" />")

        objWriter.WriteLine("</head>")

        objWriter.WriteLine("<body>")
        objWriter.WriteLine("<center>")

        objWriter.WriteLine("  <div id=""divFormContainer"" class=""divFormContainer"">")
        objWriter.WriteLine("  <form id=""formPopup"" class=""form_Enregistrement"" style=""width: 100%"" name=""frm" & nomSimple & """ method=""post"" action="""" >")
        objWriter.WriteLine("     <div class=""divHeaderEncadrementGridPrime"">")
        objWriter.WriteLine("Nouvelle " & nomSimple & " ")
        objWriter.WriteLine("      </div> ")
        objWriter.WriteLine("      <div class=""divFormContent"" >")

        objWriter.WriteLine("<table border=""0"" cellpadding=""4"" cellspacing=""0"" width=""100%"">")



        countColumn = 0
        For Each column As Cls_Column In _table.ListofColumn
            Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
            Dim primary As String = _table.ListofColumn(0).Name
            If countColumn < _table.ListofColumn.Count - 4 Then

                If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then

                    Dim columnName As String = column.Name.Substring(ForeinKeyPrefix.Length, column.Name.Length - (ForeinKeyPrefix.Length))
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & columnName & """ class=""popupFormInputLabel"" "">" & columnName & ": </label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadComboBox ID=""rcmb_" & columnName & """  Filter=""Contains""  AutoPostBack=""true""  Width=""50%"" runat=""server""> " & Chr(13) & " </telerik:RadComboBox> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType <> "bit" Then
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & column.Name & """ class=""popupFormInputLabel"">" & column.Name & ": </label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<input type=""text"" id=""txt_" & column.Name & """ name=""txt_" & column.Name & """  value=""<?php echo $txt_" & column.Name & "; ?>""  Width=""50%"" > " & Chr(13) & " </input> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")
                ElseIf column.TrueSqlServerType = "date" And column.TrueSqlServerType = "datetime" And column.Name <> _table.ListofColumn(0).Name Then
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & column.Name & """ class=""popupFormInputLabel"" "">" & column.Name & ": </label> ")
                    objWriter.WriteLine("</td>")

                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<telerik:RadDatePicker ID=""rdp_" & column.Name & """ Width=""50%"" runat=""server""" & _
                                     "DateInput-DateFormat=""dd/MM/yyyy"" MinDate=""1900-01-01""  ToolTip=""Cliquer sur le bouton pour choisir une date"" > " & Chr(13) & " </telerik:RadDatePicker>  ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("</tr>")

                ElseIf column.TrueSqlServerType = "bit" Then
                    objWriter.WriteLine("<tr>")
                    objWriter.WriteLine("<td style=""width: 30%;"" align=""right"">")
                    objWriter.WriteLine("<label for=""" & column.Name & """ class=""popupFormInputLabel"" "">" & column.Name & ": </label> ")
                    objWriter.WriteLine("</td>")
                    objWriter.WriteLine("<td style=""width: 70%;"" align=""left"">")
                    objWriter.WriteLine("<input type=""checkbox"" name=""chk_" & column.Name & """ width=""50%"" >" & _
                                      " </input>  ")
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
        objWriter.WriteLine("<input type=""submit"" name=""btnSave"" value=""Sauvegarder"" > ")
        objWriter.WriteLine("</input>")
        objWriter.WriteLine("&nbsp;")
        objWriter.WriteLine("<input type=""submit"" name=""btnCancel"" value= ""Abandonner"" >")
        objWriter.WriteLine("</input>")

        objWriter.WriteLine("</td>")

        objWriter.WriteLine("</tr>")
        objWriter.WriteLine(" </table>")
        objWriter.WriteLine(" </div>")
        objWriter.WriteLine("   <div class=""divFormButtonSaveCancel"">")
        objWriter.WriteLine("      <button name=""btnSave"" type=""submit"" onclick=""""  value=""Sauvegarder"" class=""buttonPrime"">")
        objWriter.WriteLine("       <span  class=""buttonPrimeSpan"">")
        objWriter.WriteLine(" Sauvegarder()")
        objWriter.WriteLine("           </span>")
        objWriter.WriteLine("           <input id=""btnSaveInput""  value=""<?php echo ""$btnSaveInput"" ?>"" type=""hidden""></input>")
        objWriter.WriteLine("       </button>")
        objWriter.WriteLine("        <button  type=""submit"" value=""Abandonner"" name=""btnCancel"" class=""buttonPrime"">")
        objWriter.WriteLine("            <span class=""buttonPrimeSpan"">")
        objWriter.WriteLine("  Abandonner()")
        objWriter.WriteLine("           <input id=""txt_CodeHID""  value=""<?php echo ""$txt_CodeHID"" ?>"" type=""hidden""  ></input>")
        objWriter.WriteLine("          </span>")
        objWriter.WriteLine("      </button>")
        objWriter.WriteLine("   </div>")
        objWriter.WriteLine(" </form>")
        objWriter.WriteLine(" </div>")
        objWriter.WriteLine("</center>")


        objWriter.WriteLine("</body>")

        objWriter.WriteLine()
        objWriter.Close()
    End Sub


#End Region


End Class
