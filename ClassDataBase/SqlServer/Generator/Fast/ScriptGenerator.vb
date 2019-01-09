Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.ComponentModel

Namespace SqlServer.Fast

    Public Class ScriptGenerator

        Public Shared Function CreateStore(ByVal Name As String) As String
            Dim ds As DataSet = SqlServerHelper.LoadTableStructure(Name)
            Dim cap As Integer
            cap = ds.Tables(1).Rows.Count
            Dim count As Integer = 0
            Dim paramStore As String = ""
            Dim champStore As String = ""
            Dim Id_table As String = ""
            Dim valueStore As String = ""
            Dim SpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary", "decimal", "numeric"}
            Dim LevelOneSpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary"}
            Dim LevelTwoSpecialChar As New List(Of String) From {"decimal", "numeric"}


            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(1).Rows
                If dt(0).ToString <> Id_table Then
                    If count < cap Then '- 4
                        Dim champsName As String = dt(1).ToString.Trim
                        Dim longfield As Long = dt(3) / 2

                        If Not dt(0).ToString.Equals("ModifBy") _
                        AndAlso Not dt(0).ToString.Equals("DateModif") Then

                            If dt(0).ToString.Equals("DateCreated") Then
                                If (paramStore = "") Then
                                    'If SpecialChar.Contains(champsName) Then
                                    '    If LevelOneSpecialChar.Contains(champsName) Then
                                    '        paramStore = "@" & dt(0) & " " & dt(1) & "(" & IIf(longfield <= 0, "MAX", longfield) & ")"
                                    '    Else
                                    '        paramStore = "@" & dt(0) & " " & dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"
                                    '    End If
                                    'Else
                                    '    paramStore = "@" & dt(0) & " " & dt(1)
                                    'End If

                                    champStore = "[" & dt(0) & "]"
                                    valueStore = "GETDATE()" '& dt(0)
                                Else
                                    '    paramStore &= Chr(13) & Chr(10) & Chr(9) & "," & "@" & dt(0) & " " &
                                    'IIf(SpecialChar.Contains(dt(1).ToString.Trim),
                                    '    IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim) _
                                    '            , dt(1) & "(" & IIf(longfield <= 0, "MAX", longfield) & ")" _
                                    '            , dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")") _
                                    '        , dt(1))

                                    champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "[" & dt(0) & "]"
                                    valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "GETDATE()" '& dt(0)

                                End If
                            Else
                                If (paramStore = "") Then
                                    If SpecialChar.Contains(champsName) Then
                                        If LevelOneSpecialChar.Contains(champsName) Then
                                            paramStore = "@" & dt(0) & " " & dt(1) & "(" & IIf(longfield <= 0, "MAX", longfield) & ")"
                                        Else
                                            paramStore = "@" & dt(0) & " " & dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")"
                                        End If
                                    Else
                                        paramStore = "@" & dt(0) & " " & dt(1)
                                    End If

                                    champStore = "[" & dt(0) & "]"
                                    valueStore = "@" & dt(0)
                                Else
                                    paramStore &= Chr(13) & Chr(10) & Chr(9) & "," & "@" & dt(0) & " " &
                                IIf(SpecialChar.Contains(dt(1).ToString.Trim),
                                    IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim) _
                                            , dt(1) & "(" & IIf(longfield <= 0, "MAX", longfield) & ")" _
                                            , dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")") _
                                        , dt(1))

                                    champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "[" & dt(0) & "]"
                                    valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & dt(0)

                                End If
                            End If
                        End If

                        count += 1
                        Else
                            Exit For
                    End If
                End If

            Next
            ' paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@user" & " nvarchar(50)"
            '  champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "createdby"
            champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) '& "," & "[DateCreated]"
            ' valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@user"
            valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) '& "," & "GETDATE()"
            Dim command As String = Chr(9) & "INSERT INTO " & Name & Chr(13) & Chr(10) _
                                    & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & champStore & Chr(13) & Chr(10) _
                                    & Chr(9) & ")" & Chr(13) & Chr(10) _
                                    & Chr(9) & "VALUES" & Chr(13) & Chr(10) _
                                    & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & valueStore & Chr(13) & Chr(10) _
                                    & Chr(9) & ")"

            Dim objectname As String = Name.Substring(4, Name.Length - 4)

            Dim store As String = Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Cls_Enumeration.GENERATE_BY_APP_NAME_ForStore & Chr(13) & Chr(10) _
                                 & "/******  Object:  StoredProcedure [dbo].[SP_Insert_" & objectname & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                               & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_Insert_" & objectname & "] " & Chr(13) & Chr(10) _
                                & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                & ")" & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) _
                                & Chr(9) & "DECLARE @Error int" & Chr(13) & Chr(10) _
                                & Chr(9) & "DECLARE @ID bigint" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "BEGIN TRANSACTION " & Chr(13) & Chr(10) _
                                & command & Chr(13) & Chr(10) _
                                & Chr(9) & "SET @Error = @@ERROR" & Chr(13) & Chr(10) _
                                & Chr(9) & "IF @Error != 0 GOTO ERROR_HANDLER" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "SET @ID = @@Identity " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "COMMIT TRANSACTION" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "SELECT @ID" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "ERROR_HANDLER:" & Chr(13) & Chr(10) _
                                & Chr(9) & "IF @@TRANCOUNT != 0 ROLLBACK TRANSACTION" & Chr(13) & Chr(10) _
                                & Chr(9) & "RETURN @Error" & Chr(13) & Chr(10) & Chr(13) & Chr(10)
            Return store
        End Function

        Public Shared Function UpdateStore(ByVal Name As String) As String

            Dim ds As DataSet = SqlServerHelper.LoadTableStructure(Name)
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count


            Dim count As Integer = 0
            Dim paramStore As String = ""
            Dim champStore As String = ""
            Dim Id_table As String = ""
            Dim QuerySet As String = ""
            Dim SpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary", "decimal", "numeric"}
            Dim LevelOneSpecialChar As New List(Of String) From {"nvarchar", "varchar", "char", "nchar", "binary", "datetime2", "datetimeoffset", "time", "varbinary"}
            Dim LevelTwoSpecialChar As New List(Of String) From {"decimal", "numeric"}


            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then ' - 4
                    If Not dt(0).ToString.Equals("CreatedBy") _
                        AndAlso Not dt(0).ToString.Equals("DateCreated") Then
                        If dt(0).ToString.Equals("DateModif") Then
                            If QuerySet = "" Then
                                If dt(0) <> Id_table Then
                                    QuerySet = "[" & dt(0) & "]" & " " & "= " & "GETDATE()" ' & dt(0)
                                End If
                            Else
                                QuerySet &= Chr(10) & Chr(9) & Chr(9) & "," & "[" & dt(0) & "]" & " " & "= " & "GETDATE()" '  & dt(0)
                            End If

                            Dim champsName As String = dt(1).ToString.Trim
                            Dim longfield As Long = dt(3) '/ 2

                            'If (paramStore = "") Then
                            '    paramStore = "@" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim) _
                            '                                                 , IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim) _
                            '                                                       , dt(1) & "(" & IIf(longfield <= 0, "MAX", longfield) & ")" _
                            '                                                       , dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")") _
                            '                                                   , dt(1))

                            'Else
                            '    paramStore &= Chr(13) & Chr(10) & Chr(9) & "," & "@" & dt(0) & " " &
                            '                IIf(SpecialChar.Contains(dt(1).ToString.Trim) _
                            '                    , IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim) _
                            '                          , dt(1) & "(" & IIf(longfield <= 0, "MAX", longfield) & ")" _
                            '                          , dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")") _
                            '                      , dt(1))

                            'End If
                        Else
                            If QuerySet = "" Then
                                If dt(0) <> Id_table Then
                                    QuerySet = "[" & dt(0) & "]" & " " & "= " & "@" & dt(0)
                                End If
                            Else
                                QuerySet &= Chr(10) & Chr(9) & Chr(9) & "," & "[" & dt(0) & "]" & " " & "= " & "@" & dt(0)
                            End If

                            Dim champsName As String = dt(1).ToString.Trim
                            Dim longfield As Long = dt(3) '/ 2

                            If (paramStore = "") Then
                                paramStore = "@" & dt(0) & " " & IIf(SpecialChar.Contains(dt(1).ToString.Trim) _
                                                                             , IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim) _
                                                                                   , dt(1) & "(" & IIf(longfield <= 0, "MAX", longfield) & ")" _
                                                                                   , dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")") _
                                                                               , dt(1))

                            Else
                                paramStore &= Chr(13) & Chr(10) & Chr(9) & "," & "@" & dt(0) & " " &
                                            IIf(SpecialChar.Contains(dt(1).ToString.Trim) _
                                                , IIf(LevelOneSpecialChar.Contains(dt(1).ToString.Trim) _
                                                      , dt(1) & "(" & IIf(longfield <= 0, "MAX", longfield) & ")" _
                                                      , dt(1) & "(" & dt(4).ToString.Trim() & "," & dt(5).ToString.Trim() & ")") _
                                                  , dt(1))

                            End If
                        End If
                    End If


                    count += 1
                Else
                    Exit For
                End If
            Next



            paramStore &= Chr(13) & Chr(10) & Chr(9) '& "," & "@Modifby" & " nvarchar(200)"
            QuerySet &= Chr(10) & Chr(9) & Chr(9) '& "," & "[Modifby] " & " = " & "@Modifby"
            QuerySet &= Chr(10) & Chr(9) & Chr(9) '& "," & "[DateModif]" & " = " & "GETDATE()" & Chr(10)
            'valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@user"
            'valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "GETDATE()"
            Dim command As String = Chr(9) & "UPDATE " & Name & Chr(13) & Chr(10) _
                                    & Chr(9) & "SET" & Chr(13) & Chr(10) & Chr(10) _
                                    & Chr(9) & Chr(9) & QuerySet & Chr(10)

            '' "USE MCI_db" & Chr(13) & Chr(10) & "GO" & Chr(13) & Chr(10) _&
            ''"/***Store Update For table  " & Name & "****/" & Chr(13) & Chr(10) _ &
            '' "CREATE PROCEDURE [dbo].[SP_Update" & StrConv(Name, VbStrConv.ProperCase) & "] " & Chr(13) & Chr(10) _
            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                                 & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Cls_Enumeration.GENERATE_BY_APP_NAME_ForStore & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Update_" & objectname & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & "  PROCEDURE [dbo].[SP_Update_" & objectname & "] " & Chr(13) & Chr(10) _
                                & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                & ")" & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "DECLARE @Error int" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "BEGIN TRANSACTION " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & command _
                                & Chr(9) & "WHERE " & Id_table & " = " & "@" & Id_table & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "SET @Error = @@ERROR" & Chr(13) & Chr(10) _
                                & Chr(9) & "IF @Error != 0 GOTO ERROR_HANDLER" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "COMMIT TRANSACTION" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "RETURN(0)" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "ERROR_HANDLER:" & Chr(13) & Chr(10) _
                                & Chr(9) & "IF @@TRANCOUNT != 0 ROLLBACK TRANSACTION" & Chr(13) & Chr(10) _
                                & Chr(9) & "RETURN @Error" & Chr(13) & Chr(10)
            Return store
        End Function

        Public Shared Function DeleteStore(ByVal Name As String) As String


            Dim ds As DataSet = SqlServerHelper.LoadTableStructure(Name)
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim count As Integer = 0
            Dim paramStore As String = ""
            Dim champStore As String = ""
            Dim Id_table As String = ""
            Dim Id_table_type As String = ""
            Dim QuerySet As String = ""

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next
            For Each dt As DataRow In ds.Tables(1).Rows
                If dt(0).ToString = Id_table Then
                    Id_table_type = dt(1).ToString
                End If
            Next

            Dim command As String = Chr(9) & "DELETE FROM " & Name
            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                                 & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Cls_Enumeration.GENERATE_BY_APP_NAME_ForStore & Chr(13) & Chr(10) _
                                & "/******  Object:  Stored PROCEDURE [dbo].[SP_Delete_" & objectname & "]     Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & "  PROCEDURE [dbo].[SP_Delete_" & objectname & "] " & Chr(13) & Chr(10) _
                                & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & "@ID " & Id_table_type & Chr(13) & Chr(10) _
                                & ")" & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) & Chr(13) _
                                & command & Chr(13) & Chr(10) _
                                & Chr(9) & "WHERE " & Id_table & " = " & "@ID" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & Chr(13) & Chr(10) & Chr(13) & Chr(10)

            Return store
        End Function

        Public Shared Function SelectStore(ByVal Name As String) As String

            Dim ds As DataSet = SqlServerHelper.LoadTableStructure(Name)
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count


            Dim count As Integer = 0
            Dim Id_table As String = ""
            Dim Id_table_type As String = ""
            Dim QuerySet As String = ""

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next
            For Each dt As DataRow In ds.Tables(1).Rows
                If dt(0).ToString = Id_table Then
                    Id_table_type = dt(1).ToString
                End If
            Next

            Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                    & Chr(9) & "FROM " & Name

            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                                 & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Cls_Enumeration.GENERATE_BY_APP_NAME_ForStore & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Select_" & objectname & "_ByID]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & "  PROCEDURE [dbo].[SP_Select_" & objectname & "_ByID] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & Chr(9) & "@ID " & Id_table_type & Chr(13) & Chr(10) _
                                & Chr(9) & ")" & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & command & Chr(13) & Chr(10) _
                                & Chr(9) & "WHERE " & Id_table & " = " & "@ID" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & Chr(13) & Chr(10) & Chr(13) & Chr(10)
            Return store
        End Function

        Public Shared Function SelectByIndexStore(ByVal Name As String) As String
            Dim ds As DataSet = SqlServerHelper.LoadTableStructure(Name)

            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

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
                    If dt(2).ToString.Contains(",") Then
                    Else
                        ListofIndex.Insert(countIndex, dt(2).ToString)
                        countIndex = countIndex + 1
                    End If

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
                Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                    & Chr(9) & "FROM " & Name

                Dim objectname As String = Name.Substring(4, Name.Length - 4)
                Dim store As String = Chr(13) & Chr(10) _
                    & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Cls_Enumeration.GENERATE_BY_APP_NAME_ForStore & Chr(13) & Chr(10) _
                                & "/******  Object:  Stored PROCEDURE [dbo].[SP_Select_" & objectname & "_" & ListofIndex.Item(indexPosition) & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "" & SqlServerHelper.CurrentPrefixStored & "  PROCEDURE [dbo].[SP_Select_" & objectname & "_" & ListofIndex.Item(indexPosition) & "] " & Chr(13) & Chr(10) _
                                    & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & "@" & ListofIndex.Item(indexPosition) & " " & ListofIndexType.Item(index_li_type(indexPosition)) & Chr(13) & Chr(10) _
                                    & Chr(9) & ")" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & command & Chr(13) & Chr(10) _
                                    & Chr(9) & "WHERE " & ListofIndex.Item(indexPosition) & " = " & "@" & ListofIndex.Item(indexPosition) & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & Chr(13) & Chr(10) & Chr(13) & Chr(10)

                indexPosition = indexPosition + 1
                storeglobal = storeglobal & store
            Next


            Return storeglobal
        End Function

        Public Shared Function ListAllStoreByField(ByVal name As String, ByVal Fiedlds As String) As String
            Dim ds As DataSet = SqlServerHelper.LoadTableStructure(name)
            Dim cap As Integer
            Dim fields_String As String()
            cap = ds.Tables(1).Rows.Count


            Dim count As Integer = 0


            fields_String = Fiedlds.Split(",")

            Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                    & Chr(9) & "FROM " & name

            Dim objectname As String = name.Substring(4, name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Cls_Enumeration.GENERATE_BY_APP_NAME_ForStore & Chr(13) & Chr(10) _
                                & "/******  Object:  Stored PROCEDURE [dbo].[SP_ListAll_" & objectname & "]   Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "CREATE PROCEDURE [dbo].[SP_ListAll_" & objectname & "] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & command & Chr(13) & Chr(10)
            Return store
        End Function

        Public Shared Function ListAllStore(ByVal Name As String) As String
            Dim ds As DataSet = SqlServerHelper.LoadTableStructure(Name)
            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count


            Dim count As Integer = 0

            Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                    & Chr(9) & "FROM " & Name

            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Cls_Enumeration.GENERATE_BY_APP_NAME_ForStore & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_ListAll_" & objectname & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_ListAll_" & objectname & "] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & command & Chr(13) & Chr(10)
            Return store
        End Function

        Public Shared Function ListAllByForeignKey(ByVal Name As String) As String
            Dim ds As DataSet = SqlServerHelper.LoadTableStructure(Name)
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
                    If Not ListofForeignKey.Contains(dt(6).ToString) Then
                        ListofForeignKey.Add(dt(6).ToString)
                        countForeignKey = countForeignKey + 1
                    End If
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
                Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                        & Chr(9) & "FROM " & Name

                Dim objectname As String = Name.Substring(4, Name.Length - 4)
                Dim patternname As String = ListofForeignKey.Item(foreignkeyPosition)

                Dim store As String = Chr(13) & Chr(10) _
                                    & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Cls_Enumeration.GENERATE_BY_APP_NAME_ForStore & Chr(13) & Chr(10) _
                                    & "/******  Object:  StoredProcedure [dbo].[SP_ListAll_" & objectname & "_" & patternname & "]   Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                    & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                    & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                    & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_ListAll_" & objectname & "_" & patternname & "] " & Chr(13) & Chr(10) _
                                    & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & "@" & ListofForeignKey.Item(foreignkeyPosition) & " " & ListofForeignKeyType(key_li_type(foreignkeyPosition)) & Chr(13) & Chr(10) _
                                    & Chr(9) & ")" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & command & Chr(13) & Chr(10) _
                                    & Chr(9) & "WHERE " & ListofForeignKey.Item(foreignkeyPosition) & " = " & "@" & ListofForeignKey.Item(foreignkeyPosition) & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "" & Chr(13) & Chr(10) & Chr(13) & Chr(10)
                foreignkeyPosition = foreignkeyPosition + 1
                storeglobal = storeglobal & store
            Next


            Return storeglobal
        End Function
    End Class

End Namespace
