Imports System.Data.SqlServerCe
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class Cls_TypeDatabase

#Region "Attribut"
    Private _id As Long
    Private _TypeDatabase As String

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

    Public ReadOnly Property ID As Long
        Get
            Return _id
        End Get
    End Property

    Public Property TypeDatabase() As String
        Get
            Return _TypeDatabase
        End Get
        Set(ByVal value As String)
            _TypeDatabase = value
        End Set
    End Property

  
    Public ReadOnly Property ListofServer As List(Of Cls_Table)
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
        Dim commandString As String = "Insert into tbl_Database (Name, Id_TypeDatabase) values ('" & TypeDatabase & "',1); "
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
        Return 0
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
            _TypeDatabase = myReader.Item(1)
        End While

        myReader.Close()
        ObjectConnection.Close()
        Return _id

    End Function

#End Region

#Region " Search "
    Public Shared Function GetLastDatabase() As Cls_TypeDatabase
        Dim obj As New Cls_TypeDatabase
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString = "SELECT TOP(1) Id_Database , Name  from tbl_Database order by Id_Database desc"
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            With obj
                ._id = myReader.Item(0)
                ._TypeDatabase = myReader.Item(1)
            End With
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return obj
    End Function

    Public Shared Function SeachAll() As List(Of Cls_TypeDatabase)

        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_TypeDatabase)
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
            Dim obj As New Cls_TypeDatabase
            With obj
                ._id = row.Item(0)
                ._TypeDatabase = row.Item(1)
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
