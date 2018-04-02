Imports System.Data.SqlServerCe
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class Cls_ForeignKey

#Region "Attribut"
    Private _id As Long
    Private _Id_Column As Long
    Private _Column As Cls_Column
    Private _Id_Table As Long
    Private _Table As Cls_Table
    Private _Id_RefTable As Long
    Private _RefTable As Cls_Table

#End Region

#Region "New"
    Public Sub New()
        '   Read()
    End Sub

    Public Sub New(ByVal idtable As Long, ByVal columnName As String)
        Dim obj As New Cls_Column(idtable, columnName)

        Read(idtable, obj.ID)
    End Sub

    Public Sub New(ByVal idtable As Long, ByVal idcolumn As Long)
        Read(idtable, idcolumn)
    End Sub

    Public Sub New(ByVal idone As Long)
        Read(idone)
    End Sub
#End Region

#Region "Properties"

    Public ReadOnly Property ID() As Long
        Get
            Return _id
        End Get
    End Property

    Public Property Id_Column() As Long
        Get
            Return _Id_Column
        End Get
        Set(ByVal value As Long)
            _Id_Column = value
        End Set
    End Property

    Public Property Column As Cls_Column

        Get
            If Not (_Column Is Nothing) Then
                If (_Column.ID = 0) Or (_Column.ID <> _Id_Column) Then
                    _Column = New Cls_Column(_Id_Column)
                End If
            Else
                _Column = New Cls_Column(_Id_Column)
            End If

            Return _Column
        End Get
        Set(ByVal value As Cls_Column)
            If Value Is Nothing Then

                _Id_Column = 0
            Else
                If _Column.ID <> Value.ID Then
                    _Id_Column = Value.ID
                End If
            End If
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
            If Value Is Nothing Then

                _Id_Table = 0
            Else
                If _Table.ID <> Value.ID Then
                    _Id_Table = Value.ID
                End If
            End If
        End Set
    End Property

    Public Property Id_RefTable() As Long
        Get
            Return _Id_RefTable
        End Get
        Set(ByVal value As Long)
            _Id_RefTable = value
        End Set
    End Property

    Public Property RefTable As Cls_Table

        Get
            If Not (_RefTable Is Nothing) Then
                If (_RefTable.ID = 0) Or (_RefTable.ID <> _Id_RefTable) Then
                    _RefTable = New Cls_Table(_Id_RefTable)
                End If
            Else
                _RefTable = New Cls_Table(_Id_RefTable)
            End If

            Return _RefTable
        End Get
        Set(ByVal value As Cls_Table)
            If value Is Nothing Then

                _Id_RefTable = 0
            Else
                If _RefTable.ID <> value.ID Then
                    _Id_RefTable = value.ID
                End If
            End If
        End Set
    End Property

#End Region

#Region " Db Access "
    Public Function Insert() As Integer
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Insert into tbl_ForeignKey (Id_Column , Id_Table, Id_RefTable) values (" & _Id_Column & " , " & _Id_Table & ", " & _Id_RefTable & " )"
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "SELECT TOP(1) Id_ForeignKey from tbl_ForeignKey order by Id_ForeignKey desc"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            _id = myReader.Item(0)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return _id

    End Function

    Public Function Update() As Integer
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Update tbl_ForeignKey set  Id_Table = '" & Id_Table & "' , Id_Column = " & Id_Column & ", Id_RefTable = " & Id_RefTable & "   where Id_ForeignKey =  '" & _id & "'"
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        ObjectConnection.Close()
        Return _id
    End Function

    Public Function Read(ByVal _idtable As Long, ByVal idcolumn As Long) As Boolean
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Select * from tbl_ForeignKey where Id_Table = " & _idtable & " and Id_Column = '" & idcolumn & "'"
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            _id = myReader.Item(0)
            _Id_Column = myReader.Item(1)
            _Id_Table = myReader.Item(2)
            _Id_RefTable = myReader.Item(3)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return _id

    End Function

    'Public Function Read(ByVal _idtable As Long, ByVal columnName As String) As Boolean
    '    Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
    '    Dim ObjectConnection As SqlCeConnection
    '    Dim ObjectCommand As SqlCeCommand
    '    Dim commandString As String = "Select * from tbl_ForeignKey where Id_Table = " & _idtable & " and Id_Column = '" & columnName & "'"
    '    ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
    '    ObjectConnection.Open()
    '    ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
    '    Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
    '    While myReader.Read()
    '        _id = myReader.Item(0)
    '        _Id_Column = myReader.Item(1)
    '        _Id_Table = myReader.Item(2)
    '        _Id_RefTable = myReader.Item(3)
    '    End While
    '    myReader.Close()
    '    ObjectConnection.Close()
    '    Return _id

    'End Function

    Public Function Read(ByVal _idpass As Long) As Boolean
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Select * from tbl_ForeignKey where Id_ForeignKey = " & _idpass & ""
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            _id = myReader.Item(0)
            _Id_Column = myReader.Item(1)
            _Id_Table = myReader.Item(2)
            _Id_RefTable = myReader.Item(3)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return _id
    End Function

    Public Function ReadSqlServerName(ByVal sqlservernername As String) As Boolean
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

    End Function




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

    Public Shared Function GetId_TypeBySqlServerName(ByVal name As String) As Long

    End Function

    Public Shared Function GetVbNameById(ByVal idtype As Long) As String

    End Function

    Public Shared Function SearchAllBy_Table(ByVal idTable As Long) As List(Of Cls_ForeignKey)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_ForeignKey)
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Select * from tbl_ForeignKey where Id_Table = " & idTable & ""
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            Dim obj As New Cls_ForeignKey
            With obj
                ._id = myReader.Item(0)
                ._Id_Column = myReader.Item(1)
                ._Id_Table = myReader.Item(2)
                ._Id_RefTable = myReader.Item(3)
            End With
            objs.Add(obj)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return objs
    End Function

#End Region

#Region " Other Methods "

#End Region

End Class
