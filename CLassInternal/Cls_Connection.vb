Imports System.Data.SqlServerCe
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class Cls_Connection
#Region "Attribut"
    Private _id As Long
    Private _ServerName As String
    Private _Login As String
    Private _Password As String
    Private _Date As DateTime
    Private _Id_TypeDatabase As Long
    Private _TypeDatabase As Cls_TypeDatabase

#End Region

#Region "New"
    Public Sub New()
        '  BlankProperties()
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

    Public Property ServerName() As String
        Get
            Return _ServerName
        End Get
        Set(ByVal value As String)
            _ServerName = value
        End Set
    End Property

    Public Property Login() As String
        Get
            Return _Login
        End Get
        Set(ByVal value As String)
            _Login = value
        End Set
    End Property

    Public Property Password() As String
        Get
            Return _Password
        End Get
        Set(ByVal value As String)
            _Password = value
        End Set
    End Property

    Public Property [Date]() As DateTime
        Get
            Return _Date
        End Get
        Set(ByVal value As DateTime)
            _Date = value
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


#End Region

#Region " Db Access "
    Public Function Insert() As Integer
        'Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        'Dim ObjectConnection As SqlCeConnection
        'Dim ObjectCommand As SqlCeCommand
        'Dim commandString As String = "Insert into tbl_Connection (ServerName, Login, Password,Date,Id_TypeDatabase) values ('" & _ServerName & "','" & _Login & "', '" & _Password & "' , '" & _Date & "', '" & _Id_TypeDatabase & "'); "
        'ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        'ObjectConnection.Open()
        'ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        'ObjectCommand.ExecuteNonQuery()
        'commandString = "SELECT TOP(1) Id_Connection from tbl_Connection order by Id_Connection desc"
        'ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        'Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        'While myReader.Read()
        '    _id = myReader.Item(0)
        'End While
        'myReader.Close()
        'ObjectConnection.Close()
        'Return _id
        Return 0
    End Function

    'Public Function Update(ByVal usr As String) As Integer
    '    '        _LogData = ""
    '    '        _LogData = GetObjectString()
    '    '        Return SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), "SP_UpdateMedicament", _id, _CodeInternational, _Description, _Prix, _Id_Monnaie, usr)
    '    'End Function

    '    '    Private Sub SetProperties(ByVal dr As DataRow)

    '    '        _id = TypeSafeConversion.NullSafeLong(dr("Id_Medicament"))
    '    '        _CodeInternational = TypeSafeConversion.NullSafeString(dr("CodeInternational"))
    '    '        _Description = TypeSafeConversion.NullSafeString(dr("Description"))
    '    '        _Prix = TypeSafeConversion.NullSafeDecimal(dr("Prix"))
    '    '        _Id_Monnaie = TypeSafeConversion.NullSafeLong(dr("Id_Monnaie"))
    '    '        _Monnaie = Nothing
    '    '    End Sub

    '    '    Private Sub BlankProperties()

    '    '        _id = 0
    '    '        _CodeInternational = ""
    '    '        _Description = ""
    '    '        _Prix = 0
    '    '        _Id_Monnaie = 0
    '    '        _Monnaie = Nothing
    '    '        _isdirty = False

    'End Function

    Public Function Read(ByVal _idpass As Long) As Boolean
        Try
            'Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            'Dim ObjectConnection As SqlCeConnection
            'Dim ObjectCommand As SqlCeCommand
            'Dim commandString As String = "Select * from tbl_Connection where Id_Connection = " & _idpass & ""
            'ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
            'ObjectConnection.Open()
            'ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
            'Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
            'While myReader.Read()
            '    _id = myReader.Item(0)
            '    _ServerName = myReader.Item(1)
            '    _Login = myReader.Item(2)
            '    _Password = myReader.Item(3)
            '    _Date = myReader.Item(4)
            '    _Id_TypeDatabase = myReader.Item(5)
            'End While
            'myReader.Close()
            'ObjectConnection.Close()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error!")
        End Try
        Return False  '_id
    End Function

#End Region

#Region " Search "
    Public Shared Function GetLastConnection() As Cls_Connection
        Dim obj As New Cls_Connection
        'Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        'Dim ObjectConnection As SqlCeConnection
        'Dim ObjectCommand As SqlCeCommand
        'Dim commandString = "SELECT TOP(1) Id_Connection , ServerName , Login , Password, Date, Id_TypeDatabase  from tbl_Connection order by Id_Connection desc"
        'ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        'ObjectConnection.Open()
        'ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        'Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        'While myReader.Read()
        '    With obj
        '        ._id = myReader.Item(0)
        '        ._ServerName = myReader.Item(1)
        '        ._Login = myReader.Item(2)
        '        ._Password = myReader.Item(3)
        '        ._Date = myReader.Item(4)
        '        ._Id_TypeDatabase = myReader.Item(5)
        '    End With
        'End While
        'myReader.Close()
        'ObjectConnection.Close()
        Return obj
    End Function

    Public Shared Function SeachAll() As List(Of Cls_Connection)
        'Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        'Dim objs As New List(Of Cls_Connection)
        'Dim ObjectConnection As SqlCeConnection
        'Dim ObjectCommand As SqlCeCommand
        'Dim ObjectAdapter As SqlCeDataAdapter
        'Dim ds As New DataSet

        'Dim commandString As String = "Select * from tbl_Connection order by Id_Connection desc  "
        'ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        'ObjectConnection.Open()
        'ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        'ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        'ObjectAdapter.Fill(ds)
        'For Each row As DataRow In ds.Tables(0).Rows
        '    Dim obj As New Cls_Connection
        '    With obj
        '        ._id = row.Item(0)
        '        ._ServerName = row.Item(1)
        '        ._Login = row.Item(2)
        '        ._Password = row.Item(3)
        '        ._Date = row.Item(4)
        '        ._Id_TypeDatabase = row.Item(5)
        '    End With
        '    objs.Add(obj)
        'Next
        'ObjectConnection.Close()
        Return Nothing 'objs
    End Function

    Public Shared Function SeachAll_ByTypeDatabase(ByVal Id_Tydatabase As Long) As List(Of Cls_Connection)
        'Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        'Dim objs As New List(Of Cls_Connection)
        'Dim ObjectConnection As SqlCeConnection
        'Dim ObjectCommand As SqlCeCommand
        'Dim ObjectAdapter As SqlCeDataAdapter
        'Dim ds As New DataSet

        'Dim commandString As String = "Select * from tbl_Connection where Id_TypeDatabase = " & Id_Tydatabase & " order by Id_Connection desc  "
        'ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        'ObjectConnection.Open()
        'ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        'ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        'ObjectAdapter.Fill(ds)
        'For Each row As DataRow In ds.Tables(0).Rows
        '    Dim obj As New Cls_Connection
        '    With obj
        '        ._id = row.Item(0)
        '        ._ServerName = row.Item(1)
        '        ._Login = row.Item(2)
        '        ._Password = row.Item(3)
        '        ._Date = row.Item(4)
        '        ._Id_TypeDatabase = row.Item(5)
        '    End With
        '    objs.Add(obj)
        'Next
        'ObjectConnection.Close()
        'Return objs
        Return Nothing
    End Function

#End Region

#Region " Other Methods "

#End Region




End Class
