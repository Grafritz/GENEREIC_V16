Imports System.Data.SqlServerCe
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class Cls_Database

#Region "Attribut"
    Private _id As Long
    Private _Name As String
    Private _Id_TypeDatabase As Long
    Private _TypeDatabase As Cls_TypeDatabase
#End Region

#Region "New"
    Public Sub New()
        '  BlankProperties()
    End Sub

    Public Sub New(ByVal _idOne As Long)
        ' Read(_idOne)
    End Sub
#End Region

#Region "Properties"

    Public ReadOnly Property ID As Long
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

    Public Property Id_TypeDatabase() As String
        Get
            Return _Id_TypeDatabase
        End Get
        Set(ByVal value As String)
            _Id_TypeDatabase = value
        End Set
    End Property

    Public Property TypeDatabase As Cls_TypeDatabase

        Get
            If Not (_TypeDatabase Is Nothing) Then
                If (_TypeDatabase.ID = 0) Or (_TypeDatabase.ID <> _Id_TypeDatabase) Then
                    _TypeDatabase = New Cls_TypeDatabase(_Id_TypeDatabase)
                End If
            Else
                _TypeDatabase = New Cls_TypeDatabase(_Id_TypeDatabase)
            End If

            Return _TypeDatabase
        End Get
        Set(ByVal value As Cls_TypeDatabase)
            If Value Is Nothing Then

                _Id_TypeDatabase = 0
            Else
                If _TypeDatabase.ID <> Value.ID Then
                    _Id_TypeDatabase = Value.ID
                End If
            End If
        End Set
    End Property


    Public ReadOnly Property ListofTable As List(Of Cls_Table)
        Get
            Return Cls_Table.SearchAllBy_Database(_id)
        End Get
    End Property

#End Region

#Region " Db Access "
    Public Function Insert() As Integer
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Insert into tbl_Database (Name, Id_TypeDatabase) values ('" & _Name & "', " & _Id_TypeDatabase & "); "
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "SELECT TOP(1) Id_Database from tbl_Database order by Id_Database desc"
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            _id = myReader.Item(0)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return _id
    End Function

    Public Function Update(ByVal usr As String) As Integer
        '        _LogData = ""
        '        _LogData = GetObjectString()
        '        Return SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), "SP_UpdateMedicament", _id, _CodeInternational, _Description, _Prix, _Id_Monnaie, usr)
        '    End Function

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

    End Function

    Public Function Read(ByVal _idpass As Long) As Boolean
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Select * from tbl_Database where Id_Database = " & _idpass & ""
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            _id = myReader.Item(0)
            _Name = myReader.Item(1)
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

    Public Shared Function GetLastDatabase() As Cls_Database
        Dim obj As New Cls_Database
        Try
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim ObjectConnection As SqlCeConnection
            Dim ObjectCommand As SqlCeCommand
            Dim commandString = "SELECT TOP(1) Id_Database , Name , Id_TypeDatabase  from tbl_Database order by Id_Database desc"
            ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
            ObjectConnection.Open()
            ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
            Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
            While myReader.Read()
                With obj
                    ._id = myReader.Item(0)
                    ._Name = myReader.Item(1)
                    ._Id_TypeDatabase = myReader.Item(2)

                End With
            End While
            myReader.Close()
            ObjectConnection.Close()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error!")
        End Try
        Return obj
    End Function

    Public Shared Function SeachAll() As List(Of Cls_Database)

        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_Database)
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim ObjectAdapter As SqlCeDataAdapter
        Dim ds As New DataSet
        Dim commandString As String = "Select * distinct from tbl_Database  "
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        ObjectAdapter.Fill(ds)
        For Each row As DataRow In ds.Tables(0).Rows
            Dim obj As New Cls_Database
            With obj
                ._id = row.Item(0)
                ._Name = row.Item(1)
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
