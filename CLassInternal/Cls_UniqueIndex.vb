Imports System.Data.SqlServerCe
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class Cls_UniqueIndex

#Region "Attribut"
    Private _id As Long
    Private _Id_Table As Long
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


    Public ReadOnly Property ID() As Long
        Get
            Return _id
        End Get
    End Property

    Public Property Id_Table() As Long
        Get
            Return _Id_Table
        End Get
        Set(ByVal value As Long)
            _Id_Table = value
        End Set
    End Property


    Public ReadOnly Property ListofColumn As List(Of Cls_Column)
        Get
            Return Cls_Column.SearchAllBy_IndexParent(_id)
        End Get
    End Property


#End Region

#Region " Db Access "
    Public Function Insert() As Integer
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Insert into tbl_UniqueIndex (Id_Table) values ('" & _Id_Table & "' );"

        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "SELECT TOP(1) Id_UniqueIndex from tbl_UniqueIndex order by Id_UniqueIndex desc"
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

    End Function

    Public Function ReadSqlServerName(ByVal sqlservernername As String) As Boolean

    End Function




    Public Sub Delete()

    End Sub


#End Region

#Region " Search "

    Public Shared Function SearchAllBy_Table(ByVal idTable As Long) As List(Of Cls_UniqueIndex)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_UniqueIndex)
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Select * from tbl_UniqueIndex where Id_Table = " & idTable & ""
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            Dim obj As New Cls_UniqueIndex
            With obj
                ._id = myReader.Item(0)
                ._Id_Table = myReader.Item(1)
            End With
            objs.Add(obj)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return objs
    End Function

    Public Shared Function GetId_TypeBySqlServerName(ByVal name As String) As Long

    End Function

    Public Shared Function GetVbNameById(ByVal idtype As Long) As String

    End Function
#End Region

#Region " Other Methods "

#End Region

End Class
