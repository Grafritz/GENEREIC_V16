Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.ComponentModel

Public Class SqlServerHelper

#Region "Attributes"
    Public Shared servername As String
    Public Shared password As String
    Public Shared database As String
    Public Shared user_login As String
    Public Shared ForeinKeyPrefix As String
    Public Shared Schema As DataTable
    Public Shared CurrentPrefixStored As String
    Public Shared List_of_Nervous_Types_My_Sql As New List(Of String)
#End Region

#Region "Loading Tables Fonctions"
    Public Shared Function LoadUserTablesSchema(ByVal strServer As String, _
                ByVal strUser As String, _
                ByVal strPwd As String, _
                ByVal strDatabase As String, ByRef treeview1 As TreeView) As ArrayList

        Dim cnString As String
        Dim slTables As ArrayList = New ArrayList()
        cnString = "Provider=SQLOLEDB;Data Source=" & strServer & ";Initial " & _
                "Catalog=" & strDatabase & ";" & _
                "User ID=" & strUser & ";" & _
                "Password=" & strPwd & ";"

        Dim cn As OleDbConnection = New OleDbConnection(cnString)
        Try
            cn.Open()
            Schema = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, Nothing, "TABLE"})
            cn.Close()
            Dim dr As DataRow
            Dim ds As DataSet
            treeview1.Nodes.Clear()
            For Each dr In Schema.Rows
                ds = SqlServerHelper.LoadTableStructure(dr("TABLE_NAME"))

                Dim node As New TreeNode
                node.Text = dr("TABLE_NAME")
                For Each dt As DataRow In ds.Tables(1).Rows
                    node.Nodes.Add(dt(0))
                Next
                'treeview1.Nodes.Add(dr("TABLE_NAME"))
                treeview1.Nodes.Add(node)
            Next
            'Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            '_systeme.CreateConnectionLog(strServer, strUser, strPwd, TypeDatabase.SQLSERVER)
            For Each tr As TreeNode In treeview1.Nodes

            Next
        Catch x As OleDbException
            'lblMsg.Text = x.Message
            slTables = Nothing
        End Try
        Return slTables
    End Function

    Public Shared Function InitializeDb(ByVal strServer As String, _
                ByVal strUser As String, _
                ByVal strPwd As String, _
                ByVal strDatabase As String) As Long

        'Dim duree As TimeSpan
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Try
            _systeme.CreateLocalDatabase(strDatabase)
            _systeme.CreateOnlyTable(Schema, Cls_Database.GetLastDatabase.ID)

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

    Public Shared Function InitializeLocalColumnAnyThread(ByVal iddatabase As Long, ByRef background As BackgroundWorker, ByVal Numerothread As Long)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim duree As TimeSpan
        Dim momentdebute As DateTime = Now
        _systeme.CreateLocalTablePartAnyThread(Schema, iddatabase, background, Numerothread)
        Dim momenttermine As DateTime = Now
        duree = momenttermine - momentdebute
        MessageBox.Show("INTELLIGENT MODE READY:" & duree.ToString, "Thread" & Numerothread)
    End Function

    Public Shared Function InitializeLocalColumnSuperAnyThread(ByVal iddatabase As Long, ByRef background As BackgroundWorker, ByVal Numerothread As Long)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim duree As TimeSpan
        Dim momentdebute As DateTime = Now
        _systeme.CreateLocalTablePartSuperAnyThread(Schema, iddatabase, background, Numerothread)
        Dim momenttermine As DateTime = Now
        duree = momenttermine - momentdebute
        MessageBox.Show("INTELLIGENT MODE READY:" & duree.ToString, "Thread" & Numerothread)
    End Function

    Public Shared Function InitializeLocalColumn2(ByVal iddatabase As Long, ByVal partnumber As Long)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        If partnumber = 1 Then
            Dim duree As TimeSpan
            Dim momentdebute As DateTime = Now
            ' _systeme.CreateLocalTablePart1(Schema, iddatabase)
            Dim momenttermine As DateTime = Now
            duree = momenttermine - momentdebute
            MessageBox.Show("FIRST PART OVER:" & duree.ToString)
        ElseIf partnumber = 2 Then
            Dim duree As TimeSpan
            Dim momentdebute As DateTime = Now
            '  _systeme.CreateLocalTablePart2(Schema, iddatabase)
            Dim momenttermine As DateTime = Now
            duree = momenttermine - momentdebute
            MessageBox.Show("SECOND PART OVER:" & duree.ToString)
        ElseIf partnumber = 3 Then
            Dim duree As TimeSpan
            Dim momentdebute As DateTime = Now
            '   _systeme.CreateLocalTablePart1Second(Schema, iddatabase)
            Dim momenttermine As DateTime = Now
            duree = momenttermine - momentdebute
            MessageBox.Show("FIRST PART SECOND MANCHE OVER:" & duree.ToString)
        End If
    End Function

    Public Shared Function LoadTableStructure(ByVal table As String) As DataSet
        Dim testing As String = _
            "Integrated Security=True;" & _
            "Data Source=" & servername & ";" & _
            "Initial Catalog=" & database & ";" & _
            "User ID=" & user_login & ";" & _
            "Password=" & password & ";"
        Try
            Dim cm As New SqlConnection(testing)
            cm.Open()
            Dim cmd As New SqlCommand
            cmd.CommandText = "SP_help"
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Connection = cm
            Dim p As New SqlParameter
            p.Value = table
            cmd.Parameters.AddWithValue("objname", table)
            Dim ds As New DataSet
            Dim da As SqlDataAdapter
            da = New SqlDataAdapter(cmd)
            da.Fill(ds)
            cmd.Parameters.Clear()
            cm.Close()
            Return ds
        Catch ex As Exception
            MessageBox.Show("ERREUR:" & ex.Message, "Load Table Structure", MessageBoxButtons.OK)
            ' Error_Log("LoadTableStructure", ex.Message)
        End Try
    End Function

    Public Shared Function ConvertStringToRealType(ByVal type As String)
        Select Case type
            Case "bigint"
                Return "CAST(@ColumnValue AS bigint)"
            Case "Image"
            Case "bit"
                Return "CAST(@ColumnValue AS bit)"
            Case "char"
                Return "@ColumnValue"
            Case "date"
                Return "CAST(@ColumnValue AS date)"
            Case "datetime"
                Return "CAST(@ColumnValue AS datetime)"
            Case "datetime2"
                Return "CAST(@ColumnValue AS datetime2)"
            Case "DATETIMEOFFSET"
            Case "decimal"
                Return "CAST(@ColumnValue AS decimal(13,2))"
            Case "float"
                Return "CAST(@ColumnValue AS float)"
            Case "int"
                Return "CAST(@ColumnValue AS int)"
            Case "money"
            Case "nchar"
                Return "@ColumnValue"
            Case "nvarchar"
                Return "@ColumnValue"
            Case "numeric"
                Return "CAST(@ColumnValue AS numeric(13,2))"
            Case "rowversion"
            Case "smallint"
                Return "CAST(@ColumnValue AS float)"
            Case "smallmoney"
            Case "time"
            Case "smallmoney"
            Case "tinyint"
                Return "CAST(@ColumnValue AS tinyint)"
            Case "varbinary"
            Case "varchar"
                Return "@ColumnValue"
            Case Else
                Return "@ColumnValue"
        End Select
    End Function



#End Region

#Region "Conversion Fonctions"
    Public Shared Function DefaultValue(ByVal type As String) As Object
        Select Case type
            Case "String"
                Return ""
            Case "Long"
                Return 0
            Case "Integer"
                Return 0
            Case "Int32"
                Return 0
            Case "Int64"
                Return 0
            Case "Boolean"
                Return False
            Case Else
                Return "Nothing"
        End Select
    End Function

    Public Shared Function ConvertDBToJavaType(ByVal Type As String) As String
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


        Return AndroidTypeHash(Type)
    End Function
#End Region




End Class
