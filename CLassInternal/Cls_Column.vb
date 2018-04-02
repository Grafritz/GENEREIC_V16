Imports System.Data.SqlServerCe
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Text.RegularExpressions

Public Class Cls_Column

#Region "Attribut"
    Private _id As Long
    Private _Name As String
    Private _Id_Type As String
    Private _Type As Cls_Type
    Private _isPrimary As Boolean
    Private _Id_Table As Long
    Private _Table As Cls_Table
    Private _Id_IndexParent As Long
    Private _Length As String
    Private _Precision As String
    Private _Scale As String

#End Region

#Region "New"
    Public Sub New()

    End Sub
    Public Sub New(ByVal idtable As Long, ByVal columnname As String)
        Read(idtable, columnname)
    End Sub

    Public Sub New(ByVal _idOne As Long)
        Read(_idOne)
    End Sub
#End Region

#Region "Properties"

    Public ReadOnly Property ID() As Long
        Get
            Return _id
        End Get
    End Property

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Public ReadOnly Property NameToShow
        Get
            Return Regex.Replace(_Name, "([a-z])?([A-Z])", "$1 $2")
        End Get
    End Property

    Public Property Id_Type() As String
        Get
            Return _Id_Type
        End Get
        Set(ByVal value As String)
            _Id_Type = value
        End Set
    End Property

    Public Property Type As Cls_Type

        Get
            If Not (_Type Is Nothing) Then
                If (_Type.ID = 0) Or (_Type.ID <> _Id_Type) Then
                    _Type = New Cls_Type(_Id_Type)
                End If
            Else
                _Type = New Cls_Type(_Id_Type)
            End If

            Return _Type
        End Get
        Set(ByVal value As Cls_Type)
            If value Is Nothing Then

                _Id_Type = 0
            Else
                If _Type.ID <> value.ID Then
                    _Id_Type = value.ID
                End If
            End If
        End Set
    End Property


    Public Property IsPrimary() As Boolean
        Get
            Return _isPrimary
        End Get
        Set(ByVal value As Boolean)
            _isPrimary = value
        End Set
    End Property

    Public Property Id_Table() As Long
        Get
            Return _Id_Table
        End Get
        Set(ByVal value As Long)
            _Id_Table = value
        End Set
    End Property

    Public Property Table As Cls_Table

        Get
            If Not (_Table Is Nothing) Then
                If (_Table.ID = 0) Or (_Table.ID <> _Id_Table) Then
                    _Table = New Cls_Table(_Id_Table)
                End If
            Else
                _Table = New Cls_Table(_Id_Table)
            End If

            Return _Table
        End Get
        Set(ByVal value As Cls_Table)
            If value Is Nothing Then

                _Id_Table = 0
            Else
                If _Table.ID <> value.ID Then
                    _Id_Table = value.ID
                End If
            End If
        End Set
    End Property

    Public Property Id_IndexParent() As Long
        Get
            Return _Id_IndexParent
        End Get
        Set(ByVal value As Long)
            _Id_IndexParent = value
        End Set
    End Property

    Public Property Length() As String
        Get
            Return _Length
        End Get
        Set(ByVal value As String)
            _Length = value
        End Set
    End Property

    Public Property Precision() As String
        Get
            Return _Precision
        End Get
        Set(ByVal value As String)
            _Precision = value
        End Set
    End Property

    Public Property Scale() As String
        Get
            Return _Scale
        End Get
        Set(ByVal value As String)
            _Scale = value
        End Set
    End Property

    Public ReadOnly Property TrueSqlServerType As String
        Get
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            If _systeme.LevelOneSpecialChar.Contains(Type.SqlServerName) Then
                Return Type.SqlServerName & "(" & _Length & ")"
            ElseIf _systeme.LevelTwoSpecialChar.Contains(Type.SqlServerName) Then
                Return Type.SqlServerName & "(" & _Precision & "," & _Scale & ")"
            Else
                Return Type.SqlServerName
            End If
        End Get
    End Property

#End Region

#Region " Db Access "
    Public Function Insert() As Integer

        Dim cantpass As Boolean = True
        Dim compteur As Integer = 0
        While cantpass
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim ObjectConnection As SqlCeConnection
            Dim ObjectCommand As SqlCeCommand
            Dim commandString As String = "Insert into tbl_Column (Name, Id_Table, Id_Type, Length, Precision, Scale, Id_IndexParent ) values ('" & _Name & "' ,'" & _Id_Table & "','" & _Id_Type & "','" & _Length & "','" & _Precision & "', '" & _Scale & "', '" & _Id_IndexParent & "' ); "

            Try
                ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
                ObjectConnection.Open()
                ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
                ObjectCommand.ExecuteNonQuery()
                commandString = "SELECT TOP(1) Id_Column from tbl_Column order by Id_Column desc"
                ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
                Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
                While myReader.Read()
                    _id = myReader.Item(0)
                End While
                myReader.Close()
                ObjectConnection.Close()
                cantpass = False
            Catch ex As Exception

            End Try

            compteur = compteur + 1
        End While


        Return _id
    End Function

    Public Shared Function GetId_ColumnByTableAndColumnName(ByVal idtable As Long, ByVal columnname As String) As Long
        Dim cantpass As Boolean = True
        Dim returnId As Long
        Dim compteur As Integer = 0
        While cantpass

            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim ObjectConnection As SqlCeConnection
            Dim ObjectCommand As SqlCeCommand
            Dim commandString As String = "Select Id_Column from tbl_Column where Id_Table = " & idtable & " and Name = '" & columnname & "'"
            Try
                ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
                ObjectConnection.Open()
                ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
                Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
                While myReader.Read()
                    returnId = myReader.Item(0)

                End While
                myReader.Close()
                ObjectConnection.Close()
                cantpass = False
            Catch ex As Exception

            Finally

            End Try

        End While

        Return returnId
    End Function

    Public Function Update() As Integer
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Update tbl_Column set  Name = '" & _Name & "' , Id_Table = " & _Id_Table & " , Id_Type = '" & _Id_Type & "', Length = '" & _Length & "'  , Precision  = '" & _Precision & "'  , Scale  = '" & _Scale & "' , Id_IndexParent = '" & _Id_IndexParent & "'  where Id_Column =  '" & _id & "'"
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        ObjectConnection.Close()
        Return _id
    End Function

    '    Private Sub SetProperties(ByVal dr As DataRow)

    '        _id = TypeSafeConversion.NullSafeLong(dr("Id_Medicament"))
    '        _CodeInternational = TypeSafeConversion.NullSafeString(dr("CodeInternational"))
    '        _Description = TypeSafeConversion.NullSafeString(dr("Description"))
    '        _Prix = TypeSafeConversion.NullSafeDecimal(dr("Prix"))
    '        _Id_Monnaie = TypeSafeConversion.NullSafeLong(dr("Id_Monnaie"))
    '        _Monnaie = Nothing
    '    End Sub

    '    Private Sub BlankProperties()

    '        _id = 0
    '        _CodeInternational = ""
    '        _Description = ""
    '        _Prix = 0
    '        _Id_Monnaie = 0
    '        _Monnaie = Nothing
    '        _isdirty = False

    '    End Sub

    Public Function Read(ByVal _idpass As Long) As Boolean
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Select * from tbl_Column where Id_Column = " & _idpass & ""
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            _id = myReader.Item(0)
            _Id_Table = myReader.Item(1)
            _Name = myReader.Item(2)
            _Id_Type = myReader.Item(3)
            _Length = myReader.Item(4)
            _Precision = myReader.Item(5)
            _Scale = myReader.Item(6)
            _Id_IndexParent = myReader.Item(7)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return _id
    End Function


    Public Function Read(ByVal idtable As Long, ByVal columnname As String) As Boolean
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Select * from tbl_Column where Id_Table = " & idtable & " and Name = '" & columnname & "'"
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            _id = myReader.Item(0)
            _Id_Table = myReader.Item(1)
            _Name = myReader.Item(2)
            _Id_Type = myReader.Item(3)
            _Length = myReader.Item(4)
            _Precision = myReader.Item(5)
            _Scale = myReader.Item(6)
            _Id_IndexParent = myReader.Item(7)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return _id
    End Function

    '    Public Function Read_CodeInternational(ByVal CodeInternational As String) As Boolean
    '        Try

    '            If CodeInternational <> "" Then
    '                Dim ds As Dataset = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), "Sp_SelectMedicament_ByID", CodeInternational)

    '                If ds.tables(0).Rows.Count < 1 Then
    '                    BlankProperties()
    '                    Return False
    '                End If

    '                SetProperties(ds.Tables(0).Rows(0))
    '            Else
    '                BlankProperties()
    '            End If

    '            Return True
    '        Catch ex As Exception

    '            Throw ex

    '        End Try

    '    End Function


    '    Public Function Read_Description(ByVal Description As String) As Boolean
    '        Try

    '            If Description <> "" Then
    '                Dim ds As Dataset = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), "Sp_SelectMedicament_ByID", Description)

    '                If ds.tables(0).Rows.Count < 1 Then
    '                    BlankProperties()
    '                    Return False
    '                End If

    '                SetProperties(ds.Tables(0).Rows(0))
    '            Else
    '                BlankProperties()
    '            End If

    '            Return True
    '        Catch ex As Exception

    '            Throw ex

    '        End Try

    '    End Function


    '    Public Sub Delete() Implements IGeneral.Delete
    '        Try
    '            SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), "SP_DeleteMedicament", _id)

    '        Catch ex As SqlClient.SqlException
    '            Throw New System.Exception(ex.ErrorCode)
    '        End Try
    '    End Sub

    '    Public Function Refresh() As Boolean Implements IGeneral.Refresh
    '        If _id = 0 Then
    '            Return False
    '        Else
    '            Read(_id)
    '            Return True
    '        End If
    '    End Function

    '    Public Function Save(ByVal usr As String) As Integer Implements IGeneral.Save
    '        If _isdirty Then
    '            Validation()

    '            If _id = 0 Then
    '                Return Insert(usr)
    '            Else
    '                If _id > 0 Then
    '                    Return Update(usr)
    '                Else
    '                    _id = 0
    '                    Return False
    '                End If
    '            End If
    '        End If

    '        _isdirty = False
    '    End Function
#End Region

#Region " Search "
    Public Shared Function SearchAllBy_Table(ByVal idTable As Long) As List(Of Cls_Column)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_Column)
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim ObjectAdapter As SqlCeDataAdapter
        Dim ds As New DataSet
        Dim commandString As String = "Select * from tbl_Column where Id_Table = " & idTable & ""
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        ObjectAdapter.Fill(ds)
        For Each row As DataRow In ds.Tables(0).Rows
            Dim obj As New Cls_Column
            With obj
                ._id = row.Item(0)
                ._Id_Table = row.Item(1)
                ._Name = row.Item(2)
                ._Id_Type = row.Item(3)
                ._Length = row.Item(4)
                ._Precision = row.Item(5)
                ._Scale = row.Item(6)
                ._Id_IndexParent = row.Item(7)
            End With
            objs.Add(obj)
        Next
        ObjectConnection.Close()
        Return objs
    End Function

    Public Shared Function SearchAllBy_IndexParent(ByVal id_indexparent As Long) As List(Of Cls_Column)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_Column)
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim ObjectAdapter As SqlCeDataAdapter
        Dim ds As New DataSet
        Dim commandString As String = "Select * from tbl_Column where Id_IndexParent = " & id_indexparent & ""
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        ObjectAdapter.Fill(ds)
        For Each row As DataRow In ds.Tables(0).Rows
            Dim obj As New Cls_Column
            With obj
                ._id = row.Item(0)
                ._Id_Table = row.Item(1)
                ._Name = row.Item(2)
                ._Id_Type = row.Item(3)
                ._Length = row.Item(4)
                ._Precision = row.Item(5)
                ._Scale = row.Item(6)
                ._Id_IndexParent = row.Item(7)
            End With
            objs.Add(obj)
        Next
        ObjectConnection.Close()
        Return objs
    End Function

    Public Shared Function SearAllBy_TableInForeinKey(ByVal idTable As Long) As List(Of Cls_Column)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_Column)
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim ObjectAdapter As SqlCeDataAdapter
        Dim ds As New DataSet
        Dim commandString As String = "Select * from tbl_Column where Id_Table = " & idTable & "  and Id_Column in " & _
                                       " (Select Id_Column from tbl_ForeignKey where Id_Table = " & idTable & " ) "
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        ObjectAdapter.Fill(ds)
        For Each row As DataRow In ds.Tables(0).Rows
            Dim obj As New Cls_Column
            With obj
                ._id = row.Item(0)
                ._Id_Table = row.Item(1)
                ._Name = row.Item(2)
                ._Id_Type = row.Item(3)
                ._Length = row.Item(4)
                ._Precision = row.Item(5)
                ._Scale = row.Item(6)
                ._Id_IndexParent = row.Item(7)
            End With
            objs.Add(obj)
        Next
        ObjectConnection.Close()
        Return objs
    End Function

    Public Shared Function SearchAllBy_TableByCategorieType(ByVal idTable As Long, ByVal idcategorietype As Long) As List(Of Cls_Column)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_Column)
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim ObjectAdapter As SqlCeDataAdapter
        Dim ds As New DataSet
        Dim commandString As String = "Select * from tbl_Column c inner join tbl_Type t on c.Id_Type = t.Id_Type where Id_Table = " & idTable & " and t.Id_CategorieType = " & idcategorietype
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        ObjectAdapter.Fill(ds)
        For Each row As DataRow In ds.Tables(0).Rows
            Dim obj As New Cls_Column
            With obj
                ._id = row.Item(0)
                ._Id_Table = row.Item(1)
                ._Name = row.Item(2)
                ._Id_Type = row.Item(3)
                ._Length = row.Item(4)
                ._Precision = row.Item(5)
                ._Scale = row.Item(6)
                ._Id_IndexParent = row.Item(7)
            End With
            objs.Add(obj)
        Next
        ObjectConnection.Close()
        Return objs
    End Function

#End Region

#Region " Other Methods "

#End Region


End Class
