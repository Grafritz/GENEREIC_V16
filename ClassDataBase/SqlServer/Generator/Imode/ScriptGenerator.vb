Namespace SqlServer.IMode

    Public Class ScriptGenerator

        Public Shared Function CreateStore(ByVal Name As String) As String
            Dim _table As New Cls_Table()
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            _table.Read(_systeme.currentDatabase.ID, Name)

            Dim count As Integer = 0
            Dim paramStore As String = ""
            Dim champStore As String = ""
            Dim Id_table As String = ""
            Dim valueStore As String = ""

            Id_table = _table.ListofColumn.Item(0).Name

            For Each _column As Cls_Column In _table.ListofColumn
                If _column.Name <> Id_table Then
                    If count < _table.ListofColumn.Count - 4 Then
                        If (paramStore = "") Then
                            paramStore = "@" & _column.Name & " " & _column.TrueSqlServerType
                            champStore = "[" & _column.Name & "]"
                            valueStore = "@" & _column.Name
                        Else
                            paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & _column.Name & " " & _column.TrueSqlServerType
                            champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "[" & _column.Name & "]"
                            valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & _column.Name
                        End If
                    End If
                    count += 1
                End If
            Next

            champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "datecreated"
            valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "GETDATE()"
            Dim command As String = Chr(9) & "INSERT INTO " & Name & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & champStore & Chr(13) & Chr(10) _
                                    & Chr(9) & ")" & Chr(13) & Chr(10) _
                                    & Chr(9) & "VALUES" & Chr(13) & Chr(10) _
                                    & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & valueStore & Chr(13) & Chr(10) _
                                    & Chr(9) & ")"


            Dim objectname As String = Name.Substring(4, Name.Length - 4)

            Dim store As String = Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Insert" & objectname & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_Insert" & objectname & "] " & Chr(13) & Chr(10) _
                                & Chr(9) & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                & Chr(9) & ")" & Chr(13) & Chr(10) _
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
            Dim _table As New Cls_Table()
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            _table.Read(_systeme.currentDatabase.ID, Name)

            Dim count As Integer = 0
            Dim paramStore As String = ""
            Dim champStore As String = ""
            Dim Id_table As String = ""
            Dim QuerySet As String = ""
            Id_table = _table.ListofColumn.Item(0).Name
            For Each _column As Cls_Column In _table.ListofColumn
                If count < _table.ListofColumn.Count - 4 Then
                    If QuerySet = "" Then
                        If _column.Name <> Id_table Then
                            QuerySet = "[" & _column.Name & "]" & " " & "= " & "@" & _column.Name
                        End If
                    Else
                        QuerySet &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "[" & _column.Name & "]" & " " & "= " & "@" & _column.Name
                    End If
                    If (paramStore = "") Then
                        paramStore = "@" & _column.Name & " " & _column.TrueSqlServerType
                    Else
                        paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & _column.Name & " " & _column.TrueSqlServerType

                    End If
                    count += 1
                Else
                    Exit For
                End If
            Next

            paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@user" & " nvarchar(50)"
            QuerySet &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "Modifby " & "=" & "@user"
            QuerySet &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "DateModif" & "=" & "GETDATE()" & Chr(13) & Chr(10)
            'valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@user"
            'valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "GETDATE()"
            Dim command As String = Chr(9) & "UPDATE " & Name & Chr(13) & Chr(10) _
                                    & Chr(9) & "SET" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & QuerySet & Chr(13) & Chr(10)

            '' "USE MCI_db" & Chr(13) & Chr(10) & "GO" & Chr(13) & Chr(10) _&
            ''"/***Store Update For table  " & Name & "****/" & Chr(13) & Chr(10) _ &
            '' "CREATE PROCEDURE [dbo].[SP_Update" & StrConv(Name, VbStrConv.ProperCase) & "] " & Chr(13) & Chr(10) _
            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                               & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Update" & objectname & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_Update" & objectname & "] " & Chr(13) & Chr(10) _
                                & Chr(9) & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                & Chr(9) & ")" & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "DECLARE @Error int" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "BEGIN TRANSACTION " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & command & Chr(13) & Chr(10) _
                                & Chr(9) & "Where " & Id_table & " = " & "@" & Id_table & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
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
            Dim _table As New Cls_Table()
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            _table.Read(_systeme.currentDatabase.ID, Name)
            Dim Id_table As String = ""
            Dim Id_table_type As String = ""
            Id_table = _table.ListofColumn.Item(0).Name
            Id_table_type = _table.ListofColumn.Item(0).Type.SqlServerName
            Dim command As String = Chr(9) & "DELETE FROM " & Name
            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                               & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Delete" & objectname & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_Delete" & objectname & "] " & Chr(13) & Chr(10) _
                                & Chr(9) & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & Chr(9) & "@ID " & Id_table_type & Chr(13) & Chr(10) _
                                & Chr(9) & ")" & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & command & Chr(13) & Chr(10) _
                                & Chr(9) & "WHERE " & Id_table & " = " & "@ID" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & Chr(13) & Chr(10) & Chr(13) & Chr(10)
            Return store
        End Function

        Public Shared Function SelectStore(ByVal Name As String) As String
            Dim _table As New Cls_Table()
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            _table.Read(_systeme.currentDatabase.ID, Name)
            Dim Id_table As String = ""
            Dim Id_table_type As String = ""
            Dim QuerySet As String = ""
            Id_table = _table.ListofColumn.Item(0).Name
            Id_table_type = _table.ListofColumn.Item(0).Type.SqlServerName
            Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                    & Chr(9) & "FROM " & Name
            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Select" & objectname & "_ByID] " & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_Select" & objectname & "_ByID] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
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
            Dim _table As New Cls_Table()
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            _table.Read(_systeme.currentDatabase.ID, Name)
            Dim countIndex As Integer = 0
            Dim QuerySet As String = ""
            Dim storeglobal As String = ""

            Dim paramStore As String = ""
            Dim SelectClause As String = ""
            Dim libelleStored As String = ""

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                If _index.ListofColumn.Count = 1 Then
                    Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                  & Chr(9) & "FROM " & Name
                    Dim objectname As String = Name.Substring(4, Name.Length - 4)
                    Dim store As String = Chr(13) & Chr(10) _
                                        & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Select" & objectname & "_" & _index.ListofColumn.Item(0).Name & "] " & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                        & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_Select" & objectname & "_" & _index.ListofColumn.Item(0).Name & "] " & Chr(13) & Chr(10) _
                                        & Chr(9) & "(" & Chr(13) & Chr(10) _
                                        & Chr(9) & Chr(9) & "@" & _index.ListofColumn.Item(0).Name & " " & _index.ListofColumn.Item(0).TrueSqlServerType & Chr(13) & Chr(10) _
                                        & Chr(9) & ")" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                        & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                        & command & Chr(13) & Chr(10) _
                                        & Chr(9) & "WHERE " & _index.ListofColumn.Item(0).Name & " = " & "@" & _index.ListofColumn.Item(0).Name & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "" & Chr(13) & Chr(10) & Chr(13) & Chr(10)
                    storeglobal = storeglobal & store

                Else
                    For Each _column In _index.ListofColumn
                        If (paramStore = "") Then
                            paramStore = "@" & _column.Name & " " & _column.TrueSqlServerType
                            SelectClause = "[" & _column.Name & "] = " & "@" & _column.Name
                            libelleStored = "_" & _column.Name
                        Else
                            paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & _column.Name & " " & _column.TrueSqlServerType
                            SelectClause &= Chr(13) & Chr(10) & Chr(9) & "and " & "[" & _column.Name & "] = " & "@" & _column.Name
                            libelleStored &= "_" & _column.Name
                        End If

                    Next



                    Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                & Chr(9) & "FROM " & Name & Chr(13) & Chr(10) _
                                & Chr(9) & "Where " & SelectClause
                    Dim objectname As String = Name.Substring(4, Name.Length - 4)
                    Dim store As String = "" & Chr(13) & Chr(10) _
                                        & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Select" & objectname & libelleStored & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                       & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_Select" & objectname & libelleStored & "] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                         & Chr(9) & "(" & Chr(13) & Chr(10) _
                                        & Chr(9) & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                        & Chr(9) & ")" & Chr(13) & Chr(10) _
                                        & "AS" & Chr(13) & Chr(10) _
                                        & command & Chr(13) & Chr(10)
                    storeglobal = storeglobal & store


                End If
            Next
            Return storeglobal
        End Function

        Public Shared Function ListAllStore(ByVal Name As String) As String
            Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                    & Chr(9) & "FROM " & Name
            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                               & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
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
                            paramStore = "@" & _column.Name & " " & _column.TrueSqlServerType
                            SelectClause = "[" & _column.Name & "] = " & "@" & _column.Name
                            libelleStored = "_" & _column.Name
                        Else
                            paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & _column.Name & " " & _column.TrueSqlServerType
                            SelectClause &= Chr(13) & Chr(10) & Chr(9) & "and " & "[" & _column.Name & "] = " & "@" & _column.Name
                            libelleStored &= "_" & _column.Name
                        End If
                        Exit For
                    End If
                Next
            Next

            Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                    & Chr(9) & "FROM " & name & Chr(13) & Chr(10) _
                                    & Chr(9) & "Where " & SelectClause
            Dim objectname As String = name.Substring(4, name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                               & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_ListAll_" & objectname & libelleStored & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_ListAll_" & objectname & libelleStored & "] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                 & Chr(9) & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                & Chr(9) & ")" & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) _
                                & command & Chr(13) & Chr(10)
            Return store
        End Function

        Public Shared Function ListAllByForeignKey(ByVal Name As String) As String
            Dim _table As New Cls_Table()
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            _table.Read(_systeme.currentDatabase.ID, Name)
            Dim storeglobal As String = ""
            For Each _foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                      & Chr(9) & "FROM " & Name
                Dim objectname As String = Name.Substring(4, Name.Length - 4)
                Dim patternname As String = _foreignkey.Column.Name
                Dim store As String = Chr(13) & Chr(10) _
                                   & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_ListAll_" & objectname & "_" & patternname & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_ListAll_" & objectname & "_" & patternname & "] " & Chr(13) & Chr(10) _
                                    & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & "@" & _foreignkey.Column.Name & " " & _foreignkey.Column.Type.SqlServerName & Chr(13) & Chr(10) _
                                    & Chr(9) & ")" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & command & Chr(13) & Chr(10) _
                                    & Chr(9) & "WHERE " & _foreignkey.Column.Name & " = " & "@" & _foreignkey.Column.Name & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "" & Chr(13) & Chr(10) & Chr(13) & Chr(10)
                storeglobal = storeglobal & store
            Next
            Return storeglobal
        End Function

        Public Shared Function ListAllByAnyField(ByVal Name As String) As String
            Dim _table As New Cls_Table(Cls_Database.GetLastDatabase.ID, Name)

            Dim command As String = ""
            For Each column As Cls_Column In _table.ListofColumn

                command &= Chr(9) & "If @ColumnName = '" & column.Name & "'" & Chr(13) & Chr(10) _
                            & Chr(9) & Chr(9) & "begin" & Chr(13) & Chr(10) _
                            & Chr(9) & Chr(9) & Chr(9) & "Select * from " & _table.Name & "" & Chr(13) & Chr(10) _
                            & Chr(9) & Chr(9) & Chr(9) & "where " & column.Name & " = " & SqlServerHelper.ConvertStringToRealType(column.Type.SqlServerName) & "" & Chr(13) & Chr(10) _
                            & Chr(9) & Chr(9) & "End" & Chr(13) & Chr(10)


            Next


            'Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
            '                        & Chr(9) & "FROM " & Name


            Dim objectname As String = Name.Substring(4, Name.Length - 4)
            Dim store As String = Chr(13) & Chr(10) _
                               & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_ListAll_" & objectname & "ByAnyColumn]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "" & SqlServerHelper.CurrentPrefixStored & " PROCEDURE [dbo].[SP_ListAll_" & objectname & "ByAnyColumn] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & Chr(9) & "@ColumnName" & " nvarchar(255)" & Chr(13) & Chr(10) _
                                & Chr(9) & Chr(9) & ",@ColumnValue" & " nvarchar(255)" & Chr(13) & Chr(10) _
                                & Chr(9) & ")" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & command & Chr(13) & Chr(10)
            Return store
        End Function

        Public Shared Function UpdateStoreAddChild() As String
            Dim store As String = ""
            For Each group In Cls_GroupTable.SearchAll
                Dim name As String = group.ParentTable.Name
                Dim nomSimpleParent As String = name.Substring(4, name.Length - 4)
                Dim nomSimpleChild As String = group.ChildTable.Name.Substring(4, group.ChildTable.Name.Length - 4)

                Dim paramStore As String = ""
                Dim champStore As String = ""
                Dim Id_table As String = ""
                Dim valueStore As String = ""
                Dim count As Integer = 0

                'paramStore = "@id" & nomSimpleParent & " " & "bigint"
                'paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & ", @id" & nomSimpleChild & " " & "bigint"
                'paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & ", @usrcr" & " " & "nvarchar(20)"

                For Each _column As Cls_Column In group.LiaisonTable.ListofColumn
                    If count < group.LiaisonTable.ListofColumn.Count - 4 Then
                        For Each _columnPar In group.ParentTable.ListofColumn
                            If _column.Name = _columnPar.Name Then
                                If (paramStore = "") Then
                                    paramStore = "@" & _column.Name & " " & _column.TrueSqlServerType
                                    champStore = "[" & _column.Name & "]"
                                    valueStore = "@" & _column.Name
                                Else
                                    paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & _column.Name & " " & _column.TrueSqlServerType
                                    champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "[" & _column.Name & "]"
                                    valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & _column.Name
                                End If
                                Exit For
                            End If

                        Next

                        For Each _columnchil In group.ChildTable.ListofColumn
                            If _column.Name = _columnchil.Name Then
                                If (paramStore = "") Then
                                    paramStore = "@" & _column.Name & " " & _column.TrueSqlServerType
                                    champStore = "[" & _column.Name & "]"
                                    valueStore = "@" & _column.Name
                                Else
                                    paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & _column.Name & " " & _column.TrueSqlServerType
                                    champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "[" & _column.Name & "]"
                                    valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & _column.Name
                                End If
                                Exit For
                            End If
                        Next
                        count = count + 1
                    End If


                Next



                paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & ", @usrcr" & " " & "nvarchar(20)"
                champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "CreatedBy"
                champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "datecreated"
                valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@usrcr"
                valueStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "GETDATE()"
                Dim command As String = Chr(9) & "INSERT INTO " & group.LiaisonTable.Name & Chr(13) & Chr(10) _
                                               & Chr(9) & Chr(9) & "(" & Chr(13) & Chr(10) _
                                               & Chr(9) & Chr(9) & champStore & Chr(13) & Chr(10) _
                                               & Chr(9) & ")" & Chr(13) & Chr(10) _
                                               & Chr(9) & "VALUES" & Chr(13) & Chr(10) _
                                               & Chr(9) & "(" & Chr(13) & Chr(10) _
                                               & Chr(9) & Chr(9) & valueStore & Chr(13) & Chr(10) _
                                               & Chr(9) & ")"
                'Dim objectname As String = Name.Substring(4, Name.Length - 4)


                store = Chr(13) & Chr(10) _
                               & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Update" & nomSimpleParent & "Add" & nomSimpleChild & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "CREATE PROCEDURE [dbo].[SP_Update" & nomSimpleParent & "Add" & nomSimpleChild & "] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                & Chr(9) & ")" & Chr(13) & Chr(10) _
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


            Next
            Return store
        End Function

        Public Shared Function UpdateStoreRemoveChild() As String
            Dim store As String = ""
            For Each group In Cls_GroupTable.SearchAll
                Dim name As String = group.ParentTable.Name
                Dim nomSimpleParent As String = name.Substring(4, name.Length - 4)
                Dim nomSimpleChild As String = group.ChildTable.Name.Substring(4, group.ChildTable.Name.Length - 4)

                Dim paramStore As String = ""
                Dim champStore As String = ""
                Dim Id_table As String = ""
                Dim valueStore As String = ""

                paramStore = "@id" & nomSimpleParent & " " & "bigint"
                paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@id" & nomSimpleChild & " " & "bigint"



                Dim command As String = Chr(9) & "DELETE FROM " & group.LiaisonTable.Name
                store = Chr(13) & Chr(10) _
                               & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_Update" & nomSimpleParent & "Rmv" & nomSimpleChild & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                   & "CREATE PROCEDURE [dbo].[SP_Update" & nomSimpleParent & "Rmv" & nomSimpleChild & "] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                    & Chr(9) & ")" & Chr(13) & Chr(10) _
                                    & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & command & Chr(13) & Chr(10) _
                                    & Chr(9) & "WHERE " & group.ParentTable.ListofColumn(0).Name & " = " & "@id" & nomSimpleParent & Chr(13) & Chr(10) _
                                    & Chr(9) & "AND " & group.ChildTable.ListofColumn(0).Name & " = " & "@id" & nomSimpleChild & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "" & Chr(13) & Chr(10) & Chr(13) & Chr(10)
            Next
            Return store
        End Function

        Public Shared Function ListAllChildinParent() As String
            Dim store As String = ""
            For Each group In Cls_GroupTable.SearchAll
                Dim name As String = group.ParentTable.Name
                Dim nomSimpleParent As String = name.Substring(4, name.Length - 4)
                Dim nomSimpleChild As String = group.ChildTable.Name.Substring(4, group.ChildTable.Name.Length - 4)

                Dim paramStore As String = ""
                Dim champStore As String = ""
                Dim Id_table As String = ""
                Dim valueStore As String = ""
                Dim SelectClause As String = ""

                SelectClause = "" & group.ChildTable.ListofColumn(0).Name & "  in (SELECT " & group.ChildTable.ListofColumn(0).Name & Chr(13) & Chr(10) _
                                           & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & " FROM " & group.LiaisonTable.Name & "" & Chr(13) & Chr(10) _
                                           & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & "  WHERE " & group.ParentTable.ListofColumn(0).Name & " = @id" & nomSimpleParent & Chr(13) & Chr(10) _
                                           & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & ")"

                paramStore = "@id" & nomSimpleParent & " " & "bigint"



                Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                    & Chr(9) & "FROM " & group.ChildTable.Name & Chr(13) & Chr(10) _
                                    & Chr(9) & "WHERE " & SelectClause
                store = Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_ListAll" & nomSimpleChild & "In" & nomSimpleParent & "]   Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "CREATE PROCEDURE [dbo].[SP_ListAll" & nomSimpleChild & "In" & nomSimpleParent & "] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                    & Chr(9) & ")" & Chr(13) & Chr(10) _
                                    & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & command & Chr(13) & Chr(10)

            Next
            Return store
        End Function

        Public Shared Function ListAllChildNotinParent() As String
            Dim store As String = ""
            For Each group In Cls_GroupTable.SearchAll
                Dim name As String = group.ParentTable.Name
                Dim nomSimpleParent As String = name.Substring(4, name.Length - 4)
                Dim nomSimpleChild As String = group.ChildTable.Name.Substring(4, group.ChildTable.Name.Length - 4)
                Dim paramStore As String = ""
                Dim champStore As String = ""
                Dim Id_table As String = ""
                Dim valueStore As String = ""
                Dim SelectClause As String = ""


                SelectClause = "" & group.ChildTable.ListofColumn(0).Name & " not in (SELECT " & group.ChildTable.ListofColumn(0).Name & Chr(13) & Chr(10) _
                                          & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & " FROM " & group.LiaisonTable.Name & "" & Chr(13) & Chr(10) _
                                          & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & "  WHERE " & group.ParentTable.ListofColumn(0).Name & " = @id" & nomSimpleParent & Chr(13) & Chr(10) _
                                          & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & ")"


                paramStore = "@id" & nomSimpleParent & " " & "bigint"

                Dim command As String = Chr(9) & "SELECT *" & Chr(13) & Chr(10) _
                                    & Chr(9) & "FROM " & group.ChildTable.Name & Chr(13) & Chr(10) _
                                    & Chr(9) & "WHERE " & SelectClause
                store = Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[SP_ListAll" & nomSimpleChild & "NotIn" & nomSimpleParent & "]   Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & "CREATE PROCEDURE [dbo].[SP_ListAll" & nomSimpleChild & "NotIn" & nomSimpleParent & "] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & Chr(9) & "(" & Chr(13) & Chr(10) _
                                    & Chr(9) & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                    & Chr(9) & ")" & Chr(13) & Chr(10) _
                                    & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                    & command & Chr(13) & Chr(10)

            Next
            Return store
        End Function

        Public Shared Function GenerateReport(ByVal Reportid As Long) As String
            Dim reportdynamic As New Cls_ReportDynamic(Reportid)
            Dim ReportName As String = reportdynamic.Name
            Dim parameters As List(Of Cls_ReportDynamicParameter) = Cls_ReportDynamicParameter.SearchAllById_Report(reportdynamic.Id)
            Dim parametersDate As List(Of Cls_ReportDynamicParameterDate) = Cls_ReportDynamicParameterDate.SearchAllById_Report(reportdynamic.Id)
            Dim columnsToShow As List(Of Cls_ReportDynamicColumn) = Cls_ReportDynamicColumn.SearchAllbyId_Report(reportdynamic.Id)
            Dim tablesDynamic As List(Of Cls_ReportDynamicTable) = Cls_ReportDynamicTable.SearchAllbyId_ReportDynamic(reportdynamic.Id)
            Dim tableWhereCondition As List(Of Cls_ReportDynamicWhereCondition) = Cls_ReportDynamicWhereCondition.SearchAllById_Report(reportdynamic.Id)
            Dim count As Integer = 0
            Dim paramStore As String = ""
            Dim whereVariable As String = "SET	@WHERE = '' "
            Dim conditionParameter As String = ""
            Dim champStore As String = ""
            Dim masterScript As String = ""
            Dim masterScriptWhere As String = ""
            Dim contatenfinal As String = "SET @SQL += @WHERE"

            For Each paramDate As Cls_ReportDynamicParameterDate In parametersDate
                If (paramStore = "") Then
                    paramStore = "@" & paramDate.ParameterNameDebut & " " & paramDate.RelatedColumnDebut.Type.SqlServerName
                    paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & paramDate.ParameterNameFin & " " & paramDate.RelatedColumnFin.Type.SqlServerName
                    paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & "isByDate" & " " & "bit"
                    conditionParameter = Chr(9) & "If @" & "isByDate" & " = 1" & Chr(13) & Chr(10) _
                                        & Chr(9) & Chr(9) & "SET @SQL = @SQL +  ' AND " & paramDate.RelatedTable.AliasString & ".[" & paramDate.RelatedColumnDebut.Name & "] >= ''' + convert(nvarchar(12), @" & paramDate.ParameterNameDebut & ") + ''' And " _
                                        & paramDate.RelatedTable.AliasString & ".[" & paramDate.RelatedColumnDebut.Name & "] <= ''' + convert(nvarchar(12), @" & paramDate.ParameterNameFin & ") + ''' ' "
                Else
                    paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & paramDate.ParameterNameDebut & " " & paramDate.RelatedColumnDebut.Type.SqlServerName
                    paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & paramDate.ParameterNameFin & " " & paramDate.RelatedColumnFin.Type.SqlServerName
                    paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & "isByDate" & " " & "bit"
                    conditionParameter &= Chr(13) & Chr(10) & Chr(9) & "If @" & "isByDate" & " = 1" & Chr(13) & Chr(10) _
                                        & Chr(9) & Chr(9) & "SET @SQL = @SQL +  ' AND " & paramDate.RelatedTable.AliasString & ".[" & paramDate.RelatedColumnDebut.Name & "] >= ''' + convert(nvarchar(12), @" & paramDate.ParameterNameDebut & ") + ''' And " _
                                        & paramDate.RelatedTable.AliasString & ".[" & paramDate.RelatedColumnDebut.Name & "] <= ''' + convert(nvarchar(12), @" & paramDate.ParameterNameFin & ") + ''' ' "
                End If
            Next

            For Each param As Cls_ReportDynamicParameter In parameters
                If (paramStore = "") Then
                    paramStore = "@" & param.ParameterName & " " & param.RelatedColumn.Type.SqlServerName
                    conditionParameter = Chr(9) & "If @" & param.ParameterName & " != 0" & Chr(13) & Chr(10) _
                                        & Chr(9) & Chr(9) & "SET @SQL = @SQL +  ' AND " & param.RelatedTable.AliasString & "." & param.RelatedColumn.Name & " = ' + STR(@" & param.ParameterName & ") + ''"

                Else
                    paramStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & "," & "@" & param.ParameterName & " " & param.RelatedColumn.Type.SqlServerName
                    conditionParameter &= Chr(13) & Chr(10) & Chr(9) & "If @" & param.ParameterName & " != 0" & Chr(13) & Chr(10) _
                                          & Chr(9) & Chr(9) & "SET @SQL = @SQL +  ' AND " & param.RelatedTable.AliasString & "." & param.RelatedColumn.Name & " = ' + STR(@" & param.ParameterName & ") + ''"
                End If
            Next

           

            For Each column As Cls_ReportDynamicColumn In columnsToShow
                If (champStore = "") Then
                    champStore = Chr(9) & "[" & column.Column_Name & "] " & column.Column.TrueSqlServerType
                    masterScript = Chr(9) & "SET @SQL = 'SELECT " & column.ReportDynamicTable.AliasString & "." & column.Column_Name & "  as [" & column.Column_Name & "]"

                Else
                    champStore &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & Chr(9) & "," & "[" & column.Column_Name & "] " & column.Column.TrueSqlServerType
                    masterScript &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & "," & column.ReportDynamicTable.AliasString & "." & column.Column_Name & "  as [" & column.Column_Name & "]"
                End If
            Next
            masterScript &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & "From " & reportdynamic.MasterTable_Name & " " & reportdynamic.ListofReportDynamicTable.Item(0).AliasString

            For Each rpdtable In tablesDynamic
                If rpdtable.Id_Table <> reportdynamic.Id_MasterTable Then
                    masterScript &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & "inner join " & rpdtable.Table_Name & " " & rpdtable.AliasString & " on " & rpdtable.AliasString & "." & rpdtable.Table.PrimaryKey.Name & " = " & reportdynamic.ListofReportDynamicTable.Item(0).AliasString & "." & rpdtable.Table.PrimaryKey.Name
                End If
            Next

            masterScript &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & "WHERE 1=1 "


            For Each wherecond As Cls_ReportDynamicWhereCondition In tableWhereCondition
                masterScriptWhere &= Chr(13) & Chr(10) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & "And " & wherecond.RelatedTable1.AliasString & "." & wherecond.RelatedColumn1.Name & " = " & wherecond.RelatedTable2.AliasString & "." & wherecond.RelatedColumn2.Name

            Next

            masterScript &= masterScriptWhere & Chr(13) & Chr(10) & Chr(9) & Chr(9) & Chr(9) & Chr(9) & "'"

            Dim tableVArString As String = Chr(9) & "DECLARE @tablevar table" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                      & Chr(9) & Chr(9) & "(" & Chr(13) & Chr(10) _
                                      & Chr(9) & Chr(9) & champStore & Chr(13) & Chr(10) _
                                      & Chr(9) & Chr(9) & ")" & Chr(13) & Chr(10)

            Dim header As String = Chr(9) & "DECLARE @WHERE AS NVARCHAR(MAX), @SQL AS NVARCHAR(MAX)" & Chr(13) & Chr(10)
            Dim footer As String = Chr(9) & "PRINT(@SQL)" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                   & Chr(9) & "INSERT INTO @tablevar" & Chr(13) & Chr(10) _
                                   & Chr(9) & "EXECUTE (@SQL)" & Chr(13) & Chr(10) _
                                   & Chr(9) & "SELECT * from  @tablevar" & Chr(13) & Chr(10)

            Dim store As String = Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "/******  Object:  StoredProcedure [dbo].[" & ReportName & "]    Script Date: " & Now & " ******/" & Chr(13) & Chr(10) _
                                & "SET ANSI_NULLS ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "SET QUOTED_IDENTIFIER ON" & Chr(13) & Chr(10) _
                                & "GO " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & "CREATE PROCEDURE [dbo].[" & ReportName & "] " & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & Chr(9) & "(" & Chr(13) & Chr(10) _
                                & Chr(9) & Chr(9) & paramStore & Chr(13) & Chr(10) _
                                & Chr(9) & ")" & Chr(13) & Chr(10) _
                                & "AS" & Chr(13) & Chr(10) & Chr(13) & Chr(10) _
                                & header & Chr(13) & Chr(10) _
                                & tableVArString & Chr(13) & Chr(10) _
                                & Chr(9) & whereVariable & Chr(13) & Chr(10) _
                                & masterScript & Chr(13) & Chr(10) _
                                & conditionParameter & Chr(13) & Chr(10) _
                                & Chr(9) & contatenfinal & Chr(13) & Chr(10) _
                                & footer & Chr(13) & Chr(10) _
                                & "" & Chr(13) & Chr(10) & Chr(13) & Chr(10)
            Return store
        End Function
    End Class
End Namespace
