Imports System.Data.SqlServerCe
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml


Public Class Cls_Table
#Region "Attribut"
    Private _id As Long
    Private _Name As String
    Private _Id_DataBase As Long
    Private _Database = Nothing
    Private _Id_PrimaryKey As Long
    Private _PrimaryKey As Cls_Column
 
    
#End Region

#Region "New"
    Public Sub New()
        '  BlankProperties()
    End Sub

    Public Sub New(ByVal _idone As Long)
        Read(_idone)
    End Sub

    Public Sub New(ByVal _iddatabase As Long, ByVal name As String)
        Read(_iddatabase, name)
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

    Public Property Id_DataBase() As Long
        Get
            Return _Id_DataBase
        End Get
        Set(ByVal value As Long)
            _Id_DataBase = value
        End Set
    End Property

    Public Property Id_PrimaryKey() As Long
        Get
            Return _Id_PrimaryKey
        End Get
        Set(ByVal value As Long)
            _Id_PrimaryKey = value
        End Set
    End Property

    Public Property PrimaryKey As Cls_Column

        Get
            If Not (_PrimaryKey Is Nothing) Then
                If (_PrimaryKey.ID = 0) Or (_PrimaryKey.ID <> _Id_PrimaryKey) Then
                    _PrimaryKey = New Cls_Column(_Id_PrimaryKey)
                End If
            Else
                _PrimaryKey = New Cls_Column(_Id_PrimaryKey)
            End If

            Return _PrimaryKey
        End Get
        Set(ByVal value As Cls_Column)
            If value Is Nothing Then

                _Id_PrimaryKey = 0
            Else
                If _PrimaryKey.ID <> value.ID Then
                    _Id_PrimaryKey = value.ID
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property ListofColumn As List(Of Cls_Column)
        Get
            Return Cls_Column.SearchAllBy_Table(_id)
        End Get
    End Property

    Public ReadOnly Property ListofDateColumn As List(Of Cls_Column)
        Get
            Return Cls_Column.SearchAllBy_TableByCategorieType(_id, 4)
        End Get
    End Property

    Public ReadOnly Property ListofStringColumn As List(Of Cls_Column)
        Get
            Return Cls_Column.SearchAllBy_TableByCategorieType(_id, 1)
        End Get
    End Property

    Public ReadOnly Property ListofForeinKey As List(Of Cls_ForeignKey)
        Get
            Return Cls_ForeignKey.SearchAllBy_Table(_id)
        End Get
    End Property

    Public ReadOnly Property ListofColumnForeignKey As List(Of Cls_Column)
        Get
            Dim value As List(Of Cls_Column) = Cls_Column.SearAllBy_TableInForeinKey(_id)
            Return value
        End Get
    End Property

    Public ReadOnly Property ListofIndex As List(Of Cls_UniqueIndex)
        Get
            Return Cls_UniqueIndex.SearchAllBy_Table(_id)
        End Get
    End Property

    Public ReadOnly Property NameSansTbl_ As String
        Get
            Return Name.Replace("tbl_", "")

        End Get
    End Property

    Public ReadOnly Property NameWithWbfrm_ As String
        Get
            Return Name.Replace("tbl_", "wbfrm_")
        End Get
    End Property

    Public ReadOnly Property NameWithCls_ As String
        Get
            Return Name.Replace("tbl_", "Cls_")
        End Get
    End Property

#End Region

#Region " Db Access "
    Public Function Insert() As Integer
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Insert into tbl_Table (Name, Id_Database,Id_PrimaryKey) values ('" & Name & "' ,'" & Id_DataBase & "',0 ); "

        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        commandString = "SELECT TOP(1) Id_Table from tbl_Table order by Id_Table desc"
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
        Dim commandString As String = "Update tbl_Table set  Name = '" & _Name & "' , Id_Database = " & Id_DataBase & ", Id_PrimaryKey = " & _Id_PrimaryKey & "  where Id_Table =  '" & _id & "'"
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectCommand.ExecuteNonQuery()
        ObjectConnection.Close()
        Return _id
    End Function


    Public Function Read(ByVal _iddatabase As Long, ByVal name As String) As Boolean
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Select * from tbl_Table where Id_Database = " & _iddatabase & " and Name = '" & name & "'"
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            _id = myReader.Item(0)
            _Name = myReader.Item(1)
            _Id_DataBase = myReader.Item(2)
            _Id_PrimaryKey = myReader.Item(3)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return _id

    End Function

    Public Function Read(ByVal _idone As Long) As Boolean
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim commandString As String = "Select * from tbl_Table where Id_Table = " & _idone & " "
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        Dim myReader As SqlCeDataReader = ObjectCommand.ExecuteReader()
        While myReader.Read()
            _id = myReader.Item(0)
            _Name = myReader.Item(1)
            _Id_DataBase = myReader.Item(2)
            _Id_PrimaryKey = myReader.Item(3)
        End While
        myReader.Close()
        ObjectConnection.Close()
        Return _id
    End Function


#End Region

#Region " Search "
    Public Shared Function SearchAllBy_Database(ByVal idDatabase As Long) As List(Of Cls_Table)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_Table)

        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim ObjectAdapter As SqlCeDataAdapter
        Dim ds As New DataSet
        Dim commandString As String = "Select * from tbl_Table where Id_Database = " & idDatabase & ""
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        ObjectAdapter.Fill(ds)
        For Each row As DataRow In ds.Tables(0).Rows
            Dim obj As New Cls_Table
            With obj
                ._id = row.Item(0)
                .Name = row.Item(1)
                .Id_DataBase = row.Item(2)
            End With
            objs.Add(obj)
        Next
        ObjectConnection.Close()
        Return objs
    End Function

    Public Shared Function SearchAllBy_DatabaseExeptTable(ByVal idDatabase As Long, ByVal idtable As Long) As List(Of Cls_Table)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_Table)

        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim ObjectAdapter As SqlCeDataAdapter
        Dim ds As New DataSet
        Dim commandString As String = "Select * from tbl_Table where Id_Database = " & idDatabase & " and Id_Table <> " & idtable & ""
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        ObjectAdapter.Fill(ds)
        For Each row As DataRow In ds.Tables(0).Rows
            Dim obj As New Cls_Table
            With obj
                ._id = row.Item(0)
                .Name = row.Item(1)
                .Id_DataBase = row.Item(2)
            End With
            objs.Add(obj)
        Next
        ObjectConnection.Close()
        Return objs
    End Function

    Public Shared Function SearchAllBy_Id_ReportDynamic(ByVal idreportdynamic As Long) As List(Of Cls_Table)
        Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
        Dim objs As New List(Of Cls_Table)

        Dim ObjectConnection As SqlCeConnection
        Dim ObjectCommand As SqlCeCommand
        Dim ObjectAdapter As SqlCeDataAdapter
        Dim ds As New DataSet
        Dim commandString As String = "Select * from tbl_Table ta inner join tbl_ReportDynamicTable rdt  on ta.Id_Table = rdt.Id_table  where rdt.Id_ReportDynamic = " & idreportdynamic & " "
        ObjectConnection = New SqlCeConnection(_systeme.getConnectionString)
        ObjectConnection.Open()
        ObjectCommand = New SqlCeCommand(commandString, ObjectConnection)
        ObjectAdapter = New SqlCeDataAdapter(ObjectCommand)
        ObjectAdapter.Fill(ds)
        For Each row As DataRow In ds.Tables(0).Rows
            Dim obj As New Cls_Table
            With obj
                ._id = row.Item(0)
                .Name = row.Item(1)
                .Id_DataBase = row.Item(2)
            End With
            objs.Add(obj)
        Next
        ObjectConnection.Close()
        Return objs
    End Function

#End Region

#Region " Other Methods "
    Public Function LisOfForeingKeyContainsThisColumn(ByVal col As Cls_Column) As Boolean
        Dim value As Boolean = False
        For Each column As Cls_Column In Me.ListofColumnForeignKey
            If column.ID = col.ID Then
                value = True
            End If
        Next
        Return value
    End Function


#End Region


End Class
