Imports System.Management
Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml

Namespace SqlServer.IMode

    Public Class VbClassGenerator

        Public Shared Sub CreateFile(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\VbNetClass\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\VbNetClass\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & ".vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)
            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomClasse & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:ss\m")
            'header &= ""
            Dim content As String = "Public Class " & nomClasse & Chr(13) _
                                     & "Implements IGeneral"

            _end = "End Class" & Chr(13)
            ' Delete the file if it exists.
            If File.Exists(path) Then
                File.Delete(path)
            End If

            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If
            ' Create the file.
            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()



            Dim objWriter As New System.IO.StreamWriter(path, True)
            objWriter.WriteLine(header)
            If ListBox_NameSpace.Items.Count > 0 Then
                For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                    objWriter.WriteLine(ListBox_NameSpace.Items(i))
                Next
            End If
            objWriter.WriteLine()
            objWriter.WriteLine(content)
            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = _table.ListofColumn.Count


            Id_table = _table.ListofColumn.Item(0).Name

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next

            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")
            objWriter.WriteLine("#Region ""Attribut""")
            objWriter.WriteLine("Private _id As Long")
            objWriter.WriteLine()
            Try
                For i As Int32 = 1 To cols.Count - 1
                    If Not nottoputforlist.Contains(cols(i)) Then
                        If initialtypes(i).ToString() = "image" Then
                            insertstring &= ", " & "IIf(" & cols(i) & " Is Nothing, DBNull.Value, " & cols(i) & ")"
                        Else
                            insertstring &= ", " & cols(i)
                            updatestring &= ", " & cols(i)
                        End If
                    End If
                    objWriter.WriteLine("Private " & cols(i) & " As " & types(i))
                    If ListofForeignKey.Contains(cols(i)) Then

                        Dim valueColumn As New Cls_Column(_table.ID, cols(i).ToString.Substring(1, cols(i).Length - 1))
                        Dim foreingn As New Cls_ForeignKey(_table.ID, valueColumn.ID)

                        Dim ClassName As String = "Cls_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        ClassName = foreingn.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

                        objWriter.WriteLine("Private _" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " As " & ClassName & "")
                    End If
                Next
            Catch ex As Exception

            End Try
            objWriter.WriteLine()
            objWriter.WriteLine("#End Region")
            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""New""")
            objWriter.WriteLine("Public Sub New()")
            objWriter.WriteLine("BlankProperties()")
            objWriter.WriteLine("End Sub")
            objWriter.WriteLine()
            objWriter.WriteLine("Public Sub New(ByVal _idOne As Long)")
            objWriter.WriteLine("Read(_idOne)")
            objWriter.WriteLine("End Sub" & Chr(13))


            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                If _index.ListofColumn.Count > 1 Then
                Else
                    objWriter.WriteLine("Public Sub New(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ")")
                    objWriter.WriteLine("Read_" & _index.ListofColumn.Item(0).Name & "(" & _index.ListofColumn.Item(0).Name & ")")
                    objWriter.WriteLine("End Sub " & Chr(13))
                End If
            Next


            objWriter.WriteLine("#End Region")

            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""Properties""")
            objWriter.WriteLine("<AttributLogData(True,1)> _")
            objWriter.WriteLine("Public ReadOnly Property ID() As Long Implements IGeneral.ID")
            objWriter.WriteLine("Get")
            objWriter.WriteLine("Return _id")
            objWriter.WriteLine("End Get")
            objWriter.WriteLine("End Property")
            objWriter.WriteLine()

            For i As Int32 = 1 To cols.Count - 3 ''On ne cree pas de property pour la derniere column
                Dim propName As String = ""
                Dim s As String() = cols(i).Split("_")
                For j As Integer = 1 To s.Length - 1
                    propName &= StrConv(s(j), VbStrConv.ProperCase)
                Next
                propName = cols(i).Substring(1, cols(i).Length - 1)
                'propName = StrConv(cols(i).Split("_")(1), VbStrConv.ProperCase) & StrConv(cols(i).Split("_")(2), VbStrConv.ProperCase)
                Dim log As String = "<AttributLogData(True, " & i + 1 & ")> _"
                Dim attrib As String = "Public Property  " & propName & " As " & types(i)

                If cols(i) <> "_isdirty" Or cols(i) <> "_LogData" Then
                    objWriter.WriteLine(log)
                    objWriter.WriteLine(attrib)
                    objWriter.WriteLine("Get" & Chr(13) _
                                        & " Return " & cols(i) & Chr(13) _
                                        & "End Get")

                    If Lcasevalue.Contains(types(i)) Then
                        objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                      & " If LCase(Trim(" & cols(i) & ")) <> LCase(Trim(Value)) Then" & Chr(13) _
                                      & "_isdirty = True " & Chr(13) _
                                      & cols(i) & " = Trim(Value)" & Chr(13) _
                                      & "End If" & Chr(13) _
                                      & "End Set" & Chr(13) _
                                      & "End Property")
                    Else
                        objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                      & " If " & cols(i) & " <> Value Then" & Chr(13) _
                                      & "_isdirty = True " & Chr(13) _
                                      & cols(i) & " = Value" & Chr(13) _
                                      & "End If" & Chr(13) _
                                      & "End Set" & Chr(13) _
                                      & "End Property")
                    End If

                    objWriter.WriteLine()

                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim attributUsed As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        Dim valueColumn As New Cls_Column(_table.ID, cols(i).ToString.Substring(1, cols(i).Length - 1))
                        Dim foreingn As New Cls_ForeignKey(_table.ID, valueColumn.ID)

                        Dim ClassName As String = "Cls_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        ClassName = foreingn.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

                        Dim simplename As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        objWriter.WriteLine("Public Property " & attributUsed & " As " & ClassName & Chr(13))
                        objWriter.WriteLine("Get")
                        objWriter.WriteLine("If Not (_" & attributUsed & " Is Nothing) Then" & Chr(13) _
                                            & "If (_" & attributUsed & ".ID = 0) Or (_" & attributUsed & ".ID <>  " & cols(i) & ") Then" & Chr(13) _
                                            & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                            & "End If" & Chr(13) _
                                            & "Else" & Chr(13) _
                                            & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                            & "End If" & Chr(13) & Chr(13) _
                                            & "Return _" & attributUsed & Chr(13) _
                                            & "End Get" & Chr(13) _
                                            & "Set(ByVal value As " & ClassName & ")" & Chr(13) _
                                            & "If Value Is Nothing Then" & Chr(13) _
                                            & "_isdirty = True" & Chr(13) _
                                            & cols(i) & " = 0" & Chr(13) _
                                            & "Else" & Chr(13) _
                                            & "If _" & attributUsed & ".ID <> Value.ID Then" & Chr(13) _
                                            & "_isdirty = True" & Chr(13) _
                                            & cols(i) & " = Value.ID" & Chr(13) _
                                            & "End If" & Chr(13) _
                                            & "End If" & Chr(13) _
                                            & "End Set" & Chr(13) _
                                            & "End Property" & Chr(13) _
                                            )
                        objWriter.WriteLine()
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, cols(i).Substring(1, cols(i).Length - 1))
                        Dim textForcombo As String = ""

                        Dim compteur As Integer = 0

                        For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                            If column_fore.Type.VbName = "String" Then
                                objWriter.WriteLine("Public ReadOnly Property " & simplename & "_" & column_fore.Name & "  As String ")
                                objWriter.WriteLine("Get")
                                objWriter.WriteLine("Return " & simplename & "." & column_fore.Name & "")
                                objWriter.WriteLine("End Get")
                                objWriter.WriteLine("End Property")
                                objWriter.WriteLine()
                            End If
                            If compteur = foreignkey.RefTable.ListofColumn.Count - 5 Then
                                Exit For
                            End If
                            compteur = compteur + 1
                        Next
                    End If
                End If
                If initialtypes(i).ToString() = "image" Then
                    objWriter.WriteLine("Public Property " & cols(i).Substring(1, cols(i).Length - 1) & "String() As String")
                    objWriter.WriteLine("Get")
                    objWriter.WriteLine("If " & cols(i) & " IsNot Nothing Then")
                    objWriter.WriteLine("Return Encode(" & cols(i) & " )")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("Return """"")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End Get")
                    objWriter.WriteLine("Set(ByVal Value As String)")
                    objWriter.WriteLine(cols(i) & " = Decode(Value)")
                    objWriter.WriteLine("_isdirty = True")
                    objWriter.WriteLine("End Set")
                    objWriter.WriteLine("End Property")
                End If

            Next

            With objWriter
                .WriteLine("ReadOnly Property IsDataDirty() As Boolean")
                .WriteLine("Get")
                .WriteLine("Return _isdirty")
                .WriteLine("End Get")
                .WriteLine("End Property")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Public ReadOnly Property LogData() As String")
                .WriteLine("Get")
                .WriteLine("Return _LogData")
                .WriteLine("End Get")
                .WriteLine("End Property")
            End With


            objWriter.WriteLine("#End Region")

            objWriter.WriteLine()
            objWriter.WriteLine("#Region "" Db Access """)
            objWriter.WriteLine("Public Function Insert(ByVal usr As String) As Integer Implements IGeneral.Insert")
            objWriter.WriteLine("_LogData = """"")


            objWriter.WriteLine("_id = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Insert" & nomClasse.Substring(4, nomClasse.Length - 4) & """" & insertstring & ", usr))")


            objWriter.WriteLine("Return _id")
            objWriter.WriteLine("End Function")

            objWriter.WriteLine()
            objWriter.WriteLine("Public Function Update(ByVal usr As String) As Integer Implements IGeneral.Update")
            objWriter.WriteLine("_LogData = """"")
            objWriter.WriteLine("_LogData = GetObjectString()")
            objWriter.WriteLine("Return SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Update" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id" & updatestring & ", usr)")
            objWriter.WriteLine("End Function" & Chr(13))

            objWriter.WriteLine("Private Sub SetProperties(ByVal dr As DataRow)" & Chr(13))
            objWriter.WriteLine("_id = TypeSafeConversion.NullSafeLong(dr(""" & cols(0).Substring(1, cols(0).Length - 1) & """))")

            For i As Int32 = 1 To cols.Count - 2

                If cols(i) <> "_isdirty" Then
                    If types(i) = "DateTime" Then
                        objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafeDate(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                    ElseIf initialtypes(i) = "image" Then
                        objWriter.WriteLine("If" & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)" & "IsNot DBNull.Value Then")
                        objWriter.WriteLine(cols(i) & " = " & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("" & cols(i) & " =  " & "Nothing")
                        objWriter.WriteLine("End If")
                    Else
                        objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafe" & types(i) & "(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                    End If
                End If

                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " = Nothing")

                End If

            Next

            objWriter.WriteLine()
            objWriter.WriteLine("End Sub" & Chr(13))

            objWriter.WriteLine("Private Sub BlankProperties()" & Chr(13))
            objWriter.WriteLine("_id = 0")
            For i As Int32 = 1 To cols.Count - 2

                If types(i) <> "Boolean" Then
                    If types(i) = "DateTime" Or types(i) = "Date" Then
                        objWriter.WriteLine(cols(i) & " = " & "Now")
                    Else
                        If Lcasevalue.Contains(types(i)) Then
                            objWriter.WriteLine(cols(i) & " = " & """""")
                        ElseIf initialtypes(i) = "image" Then
                            objWriter.WriteLine(cols(i) & " = Nothing")
                        Else
                            objWriter.WriteLine(cols(i) & " = " & "0")
                        End If

                    End If
                Else
                    objWriter.WriteLine(cols(i) & " = " & "False")
                End If

                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " = Nothing")
                End If


            Next

            objWriter.WriteLine()
            objWriter.WriteLine("End Sub" & Chr(13))

            objWriter.WriteLine("Public Function Read(ByVal _idpass As Long) As Boolean Implements IGeneral.Read")
            objWriter.WriteLine("Try " & Chr(13))

            objWriter.WriteLine("If _idpass <> 0 Then " & Chr(13) _
                            & "Dim ds As DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(),""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_ByID"", _idpass)" & Chr(13) & Chr(13) _
                            & "If ds.Tables(0).Rows.Count < 1 Then" & Chr(13) _
                            & "BlankProperties()" & Chr(13) _
                            & "Return False" & Chr(13) _
                            & "End If" & Chr(13) & Chr(13) _
                            & "SetProperties(ds.tables(0).rows(0))" & Chr(13) _
                            & "Else" & Chr(13) _
                            & "BlankProperties()" & Chr(13) _
                            & "End If" & Chr(13) _
                            & "Return True" & Chr(13) _
                            )

            objWriter.WriteLine("Catch ex As Exception" & Chr(13))
            objWriter.WriteLine("Throw ex" & Chr(13))
            objWriter.WriteLine("End Try" & Chr(13))
            objWriter.WriteLine("End Function" & Chr(13))

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                If _index.ListofColumn.Count = 1 Then
                    objWriter.WriteLine("Public Function Read_" & _index.ListofColumn.Item(0).Name & "(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean")
                    objWriter.WriteLine("Try " & Chr(13))
                    objWriter.WriteLine("If " & _index.ListofColumn.Item(0).Name & " <> """" Then ")
                    objWriter.WriteLine("Dim ds as Dataset = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & """, " & _index.ListofColumn.Item(0).Name & ")" & Chr(13))
                    objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine("BlankProperties()")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If" & Chr(13))
                    objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("BlankProperties()")
                    objWriter.WriteLine("End If" & Chr(13))
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                    objWriter.WriteLine("Throw ex" & Chr(13))
                    objWriter.WriteLine("End Try" & Chr(13))
                    objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                Else
                    Dim _strOfIndexToUse As String = String.Empty
                    Dim _strOfValueToUse As String = String.Empty
                    Dim _strParameterToUse As String = String.Empty
                    Dim ind As Integer = 0
                    For Each _column As Cls_Column In _index.ListofColumn
                        If _strOfIndexToUse.Length = 0 Then
                            _strOfIndexToUse = _column.Name
                            _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse = "_value" & ind
                        Else
                            _strOfIndexToUse += "_" & _column.Name
                            _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse += ", _value" & ind
                        End If
                        ind = ind + 1
                    Next
                    objWriter.WriteLine("Public Function Read_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean")
                    objWriter.WriteLine("Try " & Chr(13))
                    '  objWriter.WriteLine("If " & ListofIndex(i) & " <> """" Then ")
                    objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")
                    objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine("BlankProperties()")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If" & Chr(13))

                    objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                    'objWriter.WriteLine("Else")
                    'objWriter.WriteLine("BlankProperties()")
                    '  objWriter.WriteLine("End If" & Chr(13))

                    objWriter.WriteLine("Return True")

                    objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                    objWriter.WriteLine("Throw ex" & Chr(13))
                    objWriter.WriteLine("End Try" & Chr(13))
                    objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                End If
            Next

            objWriter.WriteLine("Public Sub Delete() Implements IGeneral.Delete" & Chr(13) _
                                & "Try" & Chr(13) _
                                & "SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), ""SP_Delete" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id)" & Chr(13) & Chr(13) _
                                & "Catch ex As SqlClient.SqlException" & Chr(13) _
                                & "Throw New System.Exception(ex.ErrorCode)" & Chr(13) _
                                & "End Try" & Chr(13) _
                                & "End Sub" & Chr(13)
                                )

            objWriter.WriteLine("Public Function Refresh() As Boolean Implements IGeneral.Refresh" & Chr(13) _
                                 & "If _id = 0 Then" & Chr(13) _
                                 & "Return False" & Chr(13) _
                                 & "Else" & Chr(13) _
                                 & "Read(_id)" & Chr(13) _
                                 & "Return True" & Chr(13) _
                                 & "End If" & Chr(13) _
                                 & "End Function" & Chr(13) _
                                 )

            objWriter.WriteLine("Public Function Save(ByVal usr As String) As Integer Implements IGeneral.Save" & Chr(13) _
                                 & "If _isdirty Then" & Chr(13) _
                                 & "Validation()" & Chr(13) & Chr(13) _
                                 & "If _id = 0 Then" & Chr(13) _
                                 & "Return Insert(usr)" & Chr(13) _
                                 & "Else" & Chr(13) _
                                 & "If _id > 0 Then" & Chr(13) _
                                 & "Return Update(usr)" & Chr(13) _
                                 & "Else" & Chr(13) _
                                 & "_id = 0" & Chr(13) _
                                 & "Return False" & Chr(13) _
                                 & "End If" & Chr(13) _
                                 & "End If" & Chr(13) _
                                 & "End If" & Chr(13) & Chr(13) _
                                 & "_isdirty = False" & Chr(13) _
                                 & "End Function"
                                 )


            objWriter.WriteLine("#End Region")

            objWriter.WriteLine()
            objWriter.WriteLine("#Region "" Search """)

            objWriter.WriteLine("Public Function Search() As System.Collections.ICollection Implements IGeneral.Search" & Chr(13) _
                                & "Return SearchAll()" & Chr(13) _
                                & "End Function" & Chr(13)
                                )


            objWriter.WriteLine("Public Shared Function SearchAll() As List(Of " & nomClasse & ")" & Chr(13) _
                                & "Try " & Chr(13) _
                                & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                & "Dim r As Data.DataRow" & Chr(13) _
                                & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & """)" & Chr(13) _
                                & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                & "objs.Add(obj)" & Chr(13) _
                                & "Next r" & Chr(13) _
                                & "Return objs"
                                )
            objWriter.WriteLine("Catch ex As Exception" & Chr(13))
            objWriter.WriteLine("Throw ex")
            objWriter.WriteLine("End Try")
            objWriter.WriteLine("End Function" & Chr(13))


            objWriter.WriteLine()

            objWriter.WriteLine("Public Shared Function SearchAllBy" & "AnyField" & "(Byval  columnName  As String, Byval columnvalue As String)  As List(Of " & nomClasse & ")" & Chr(13) _
                                        & "Try " & Chr(13) _
                                        & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                        & "Dim r As Data.DataRow" & Chr(13) _
                                        & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & "ByAnyColumn, columnName , columnvalue)" & Chr(13) _
                                        & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                        & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                        & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                        & "objs.Add(obj)" & Chr(13) _
                                        & "Next r" & Chr(13) _
                                        & "Return objs"
                                        )
            objWriter.WriteLine("Catch ex As Exception" & Chr(13))
            objWriter.WriteLine("Throw ex")
            objWriter.WriteLine("End Try")
            objWriter.WriteLine("End Function" & Chr(13))


            objWriter.WriteLine()

            For i As Int32 = 1 To cols.Count - 2
                If ListofForeignKey.Contains(cols(i)) Then
                    Dim searchtext = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

                    objWriter.WriteLine("Public Shared Function SearchAllBy" & searchtext & "(Byval " & cols(i).ToString.ToLower & " As " & types(i) & ") As List(Of " & nomClasse & ")" & Chr(13) _
                                        & "Try " & Chr(13) _
                                        & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                        & "Dim r As Data.DataRow" & Chr(13) _
                                        & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & cols(i) & """," & cols(i).ToString.ToLower & ")" & Chr(13) _
                                        & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                        & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                        & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                        & "objs.Add(obj)" & Chr(13) _
                                        & "Next r" & Chr(13) _
                                        & "Return objs"
                                        )
                    objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                    objWriter.WriteLine("Throw ex")
                    objWriter.WriteLine("End Try")
                    objWriter.WriteLine("End Function" & Chr(13))


                End If
            Next
            objWriter.WriteLine()
            objWriter.WriteLine("#End Region")
            objWriter.WriteLine()
            objWriter.WriteLine("#Region "" Other Methods """)


            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                If _index.ListofColumn.Count = 1 Then
                    objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(ByVal _value As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean ")
                    objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _index.ListofColumn.Item(0).Name & """, _value)")
                    objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine(" Return False")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("If _id = 0 Then")
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                Else
                    Dim _strOfIndexToUse As String = String.Empty
                    Dim _strOfValueToUse As String = String.Empty
                    Dim _strParameterToUse As String = String.Empty
                    Dim ind As Integer = 0
                    For Each _column In _index.ListofColumn
                        If _strOfIndexToUse.Length = 0 Then
                            _strOfIndexToUse = _column.Name
                            _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse = "_value" & ind
                        Else
                            _strOfIndexToUse += "_" & _column.Name
                            _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse += ", _value" & ind
                        End If
                        ind = ind + 1
                    Next
                    objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean ")
                    objWriter.WriteLine("Try" & Chr(13))
                    objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")

                    objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine(" Return False")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("If _id = 0 Then")
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End If" & Chr(13))
                    objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                    objWriter.WriteLine("Throw ex" & Chr(13))
                    objWriter.WriteLine("End Try" & Chr(13))
                    objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                End If
            Next


            objWriter.WriteLine("Private Sub Validation() " & Chr(13))
            Dim stringlistforvalidation As New List(Of String) From {"String"}
            Dim decimalintegerforvalidation As New List(Of String) From {"Integer", "Long", "Decimal"}



            For i As Int32 = 1 To cols.Count - 2
                If stringlistforvalidation.Contains(types(i)) Then
                    objWriter.WriteLine("If " & cols(i) & " = """" Then " & Chr(13) _
                                        & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                        & "End If"
                                        )
                    objWriter.WriteLine()
                    objWriter.WriteLine("If Len(" & cols(i) & ") > " & length(i) & " Then" & Chr(13) _
                                        & "Throw (New System.Exception("" " & "Trop de caractères insérés pour" & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & "  (la longueur doit être inférieure a " & length(i) & " caractères.  )""))" & Chr(13) _
                                        & "End If"
                                        )
                    objWriter.WriteLine()
                End If

                If decimalintegerforvalidation.Contains(types(i)) Then
                    If ListofForeignKey.Contains(cols(i)) Then
                        objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                      & "Throw (New System.Exception("" " & cols(i).Substring(4, cols(i).Length - 4).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                      & "End If"
                                      )
                        objWriter.WriteLine()
                    Else
                        objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                       & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire.""))" & Chr(13) _
                                       & "End If"
                                       )
                        objWriter.WriteLine()
                    End If
                End If


            Next

            objWriter.WriteLine()

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                If _index.ListofColumn.Count = 1 Then
                    objWriter.WriteLine("If  FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(" & "_" & _index.ListofColumn.Item(0).Name & ") Then" & Chr(13) _
                                      & "Throw (New System.Exception(""Ce " & _index.ListofColumn.Item(0).Name & " est déjà enregistré.""))" & Chr(13) _
                                      & "End If"
                                     )
                    objWriter.WriteLine()
                Else
                    Dim _strOfIndexToUse As String = String.Empty
                    Dim _strOfValueToUse As String = String.Empty
                    Dim _strParameterToUse As String = String.Empty
                    Dim _parametervalueToUse As String = String.Empty
                    Dim ind As Integer = 0
                    For Each _column In _index.ListofColumn
                        If _strOfIndexToUse.Length = 0 Then
                            _strOfIndexToUse = _column.Name
                            _parametervalueToUse = _column.Name
                            _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse = "_value" & ind
                        Else
                            _strOfIndexToUse += "_" & _column.Name
                            _parametervalueToUse += ", " & _column.Name
                            _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse += ", value" & ind
                        End If
                    Next

                    objWriter.WriteLine("If FoundAlreadyExist_" & _strOfIndexToUse & "(" & _parametervalueToUse & ") Then" & Chr(13) _
                                       & "Throw (New System.Exception(""Cette combinaison " & _strOfIndexToUse & " est déjà enregistrée.""))" & Chr(13) _
                                       & "End If"
                                       )
                    objWriter.WriteLine()
                End If
            Next

            objWriter.WriteLine("End Sub" & Chr(13))

            objWriter.WriteLine("Public Function Encode(ByVal str As Byte()) As String")
            objWriter.WriteLine("Return Convert.ToBase64String(str)")
            objWriter.WriteLine("End Function")
            objWriter.WriteLine()

            objWriter.WriteLine("Public Function Decode(ByVal str As String) As Byte()")
            objWriter.WriteLine("Dim decbuff As Byte() = Convert.FromBase64String(str)")
            objWriter.WriteLine("Return decbuff")
            objWriter.WriteLine("End Function")
            objWriter.WriteLine()

            objWriter.WriteLine(" Public Function GetObjectString() As String Implements IGeneral.GetObjectString" & Chr(13) _
                             & "Dim _old As New " & nomClasse & "(Me.ID)" & Chr(13) & Chr(13) _
                             & "Return LogStringBuilder.BuildLogStringChangesOnly(_old,Me)" & Chr(13) _
                             & "End Function" & Chr(13) _
                             )
            objWriter.WriteLine("#End Region")
            objWriter.WriteLine()
            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateFileWithSearchAllByField(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal Fiedlds As String)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim fields_String As String() = Fiedlds.Split(",")
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\VbNetClass\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\VbNetClass\")
            Dim path As String = txt_PathGenerate_Script & nomClasse & ".vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)
            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomClasse & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")
            header &= ""
            Dim content As String = "Public Class " & nomClasse & Chr(13) _
                                     & "Implements IGeneral"

            _end = "End Class" & Chr(13)
            ' Delete the file if it exists.
            If File.Exists(path) Then
                File.Delete(path)
            End If

            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(txt_PathGenerate_Script) Then
                Directory.CreateDirectory(txt_PathGenerate_Script)
            End If
            ' Create the file.
            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()





            Dim objWriter As New System.IO.StreamWriter(path, True)
            '  objWriter.WriteLine(header)
            If ListBox_NameSpace.Items.Count > 0 Then
                For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                    objWriter.WriteLine(ListBox_NameSpace.Items(i))
                Next
            End If
            objWriter.WriteLine()
            objWriter.WriteLine(content)
            objWriter.WriteLine()
            Dim _table As New Cls_Table()

            _table.Read(_systeme.currentDatabase.ID, name)

            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = _table.ListofColumn.Count


            Id_table = _table.ListofColumn.Item(0).Name






            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next

            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add("_" & _column.Name)
                    types.Add(_column.Type.VbName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next

            cols.Add("_isdirty")
            cols.Add("_LogData")
            types.Add("Boolean")
            types.Add("String")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")
            objWriter.WriteLine("#Region ""Attribut""")
            objWriter.WriteLine("Private _id As Long")
            objWriter.WriteLine()
            Try
                For i As Int32 = 1 To cols.Count - 1
                    If Not nottoputforlist.Contains(cols(i)) Then
                        If initialtypes(i).ToString() = "image" Then
                            insertstring &= ", " & "IIf(" & cols(i) & " Is Nothing, DBNull.Value, " & cols(i) & ")"
                        Else
                            insertstring &= ", " & cols(i)
                            updatestring &= ", " & cols(i)
                        End If
                    End If
                    objWriter.WriteLine("Private " & cols(i) & " As " & types(i))
                    If ListofForeignKey.Contains(cols(i)) Then

                        Dim valueColumn As New Cls_Column(_table.ID, cols(i).ToString.Substring(1, cols(i).Length - 1))
                        Dim foreingn As New Cls_ForeignKey(_table.ID, valueColumn.ID)

                        Dim ClassName As String = "Cls_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        ClassName = foreingn.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

                        objWriter.WriteLine("Private _" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " As " & ClassName & "")
                    End If
                Next
            Catch ex As Exception

            End Try
            objWriter.WriteLine()
            objWriter.WriteLine("#End Region")
            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""New""")
            objWriter.WriteLine("Public Sub New()")
            objWriter.WriteLine("BlankProperties()")
            objWriter.WriteLine("End Sub")
            objWriter.WriteLine()
            objWriter.WriteLine("Public Sub New(ByVal _idOne As Long)")
            objWriter.WriteLine("Read(_idOne)")
            objWriter.WriteLine("End Sub" & Chr(13))


            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                If _index.ListofColumn.Count > 1 Then
                Else
                    objWriter.WriteLine("Public Sub New(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ")")
                    objWriter.WriteLine("Read_" & _index.ListofColumn.Item(0).Name & "(" & _index.ListofColumn.Item(0).Name & ")")
                    objWriter.WriteLine("End Sub " & Chr(13))
                End If
            Next


            objWriter.WriteLine("#End Region")

            objWriter.WriteLine()

            objWriter.WriteLine("#Region ""Properties""")
            objWriter.WriteLine("<AttributLogData(True,1)> _")
            objWriter.WriteLine("Public ReadOnly Property ID() As Long Implements IGeneral.ID")
            objWriter.WriteLine("Get")
            objWriter.WriteLine("Return _id")
            objWriter.WriteLine("End Get")
            objWriter.WriteLine("End Property")
            objWriter.WriteLine()

            For i As Int32 = 1 To cols.Count - 3 ''On ne cree pas de property pour la derniere column
                Dim propName As String = ""
                Dim s As String() = cols(i).Split("_")
                For j As Integer = 1 To s.Length - 1
                    propName &= StrConv(s(j), VbStrConv.ProperCase)
                Next
                propName = cols(i).Substring(1, cols(i).Length - 1)
                'propName = StrConv(cols(i).Split("_")(1), VbStrConv.ProperCase) & StrConv(cols(i).Split("_")(2), VbStrConv.ProperCase)
                Dim log As String = "<AttributLogData(True, " & i + 1 & ")> _"
                Dim attrib As String = "Public Property  " & propName & " As " & types(i)

                If cols(i) <> "_isdirty" Or cols(i) <> "_LogData" Then
                    objWriter.WriteLine(log)
                    objWriter.WriteLine(attrib)
                    objWriter.WriteLine("Get" & Chr(13) _
                                        & " Return " & cols(i) & Chr(13) _
                                        & "End Get")

                    If Lcasevalue.Contains(types(i)) Then
                        objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                      & " If LCase(Trim(" & cols(i) & ")) <> LCase(Trim(Value)) Then" & Chr(13) _
                                      & "_isdirty = True " & Chr(13) _
                                      & cols(i) & " = Trim(Value)" & Chr(13) _
                                      & "End If" & Chr(13) _
                                      & "End Set" & Chr(13) _
                                      & "End Property")
                    Else
                        objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                      & " If " & cols(i) & " <> Value Then" & Chr(13) _
                                      & "_isdirty = True " & Chr(13) _
                                      & cols(i) & " = Value" & Chr(13) _
                                      & "End If" & Chr(13) _
                                      & "End Set" & Chr(13) _
                                      & "End Property")
                    End If

                    objWriter.WriteLine()

                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim attributUsed As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        Dim valueColumn As New Cls_Column(_table.ID, cols(i).ToString.Substring(1, cols(i).Length - 1))
                        Dim foreingn As New Cls_ForeignKey(_table.ID, valueColumn.ID)

                        Dim ClassName As String = "Cls_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        ClassName = foreingn.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

                        Dim simplename As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        objWriter.WriteLine("Public Property " & attributUsed & " As " & ClassName & Chr(13))
                        objWriter.WriteLine("Get")
                        objWriter.WriteLine("If Not (_" & attributUsed & " Is Nothing) Then" & Chr(13) _
                                            & "If (_" & attributUsed & ".ID = 0) Or (_" & attributUsed & ".ID <>  " & cols(i) & ") Then" & Chr(13) _
                                            & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                            & "End If" & Chr(13) _
                                            & "Else" & Chr(13) _
                                            & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                            & "End If" & Chr(13) & Chr(13) _
                                            & "Return _" & attributUsed & Chr(13) _
                                            & "End Get" & Chr(13) _
                                            & "Set(ByVal value As " & ClassName & ")" & Chr(13) _
                                            & "If Value Is Nothing Then" & Chr(13) _
                                            & "_isdirty = True" & Chr(13) _
                                            & cols(i) & " = 0" & Chr(13) _
                                            & "Else" & Chr(13) _
                                            & "If _" & attributUsed & ".ID <> Value.ID Then" & Chr(13) _
                                            & "_isdirty = True" & Chr(13) _
                                            & cols(i) & " = Value.ID" & Chr(13) _
                                            & "End If" & Chr(13) _
                                            & "End If" & Chr(13) _
                                            & "End Set" & Chr(13) _
                                            & "End Property" & Chr(13) _
                                            )
                        objWriter.WriteLine()
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, cols(i).Substring(1, cols(i).Length - 1))
                        Dim textForcombo As String = ""

                        Dim compteur As Integer = 0

                        For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                            If column_fore.Type.VbName = "String" Then
                                objWriter.WriteLine("Public ReadOnly Property " & simplename & "_" & column_fore.Name & "  As String ")
                                objWriter.WriteLine("Get")
                                objWriter.WriteLine("Return " & simplename & "." & column_fore.Name & "")
                                objWriter.WriteLine("End Get")
                                objWriter.WriteLine("End Property")
                                objWriter.WriteLine()
                            End If
                            If compteur = foreignkey.RefTable.ListofColumn.Count - 5 Then
                                Exit For
                            End If
                            compteur = compteur + 1
                        Next
                    End If
                End If
                If initialtypes(i).ToString() = "image" Then
                    objWriter.WriteLine("Public Property " & cols(i).Substring(1, cols(i).Length - 1) & "String() As String")
                    objWriter.WriteLine("Get")
                    objWriter.WriteLine("If " & cols(i) & " IsNot Nothing Then")
                    objWriter.WriteLine("Return Encode(" & cols(i) & " )")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("Return """"")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End Get")
                    objWriter.WriteLine("Set(ByVal Value As String)")
                    objWriter.WriteLine(cols(i) & " = Decode(Value)")
                    objWriter.WriteLine("_isdirty = True")
                    objWriter.WriteLine("End Set")
                    objWriter.WriteLine("End Property")
                End If

            Next

            With objWriter
                .WriteLine("ReadOnly Property IsDataDirty() As Boolean")
                .WriteLine("Get")
                .WriteLine("Return _isdirty")
                .WriteLine("End Get")
                .WriteLine("End Property")
                .WriteLine()
            End With

            With objWriter
                .WriteLine("Public ReadOnly Property LogData() As String")
                .WriteLine("Get")
                .WriteLine("Return _LogData")
                .WriteLine("End Get")
                .WriteLine("End Property")
            End With


            objWriter.WriteLine("#End Region")

            objWriter.WriteLine()
            objWriter.WriteLine("#Region "" Db Access """)
            objWriter.WriteLine("Public Function Insert(ByVal usr As String) As Integer Implements IGeneral.Insert")
            objWriter.WriteLine("_LogData = """"")


            objWriter.WriteLine("_id = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Insert" & nomClasse.Substring(4, nomClasse.Length - 4) & """" & insertstring & ", usr))")


            objWriter.WriteLine("Return _id")
            objWriter.WriteLine("End Function")

            objWriter.WriteLine()
            objWriter.WriteLine("Public Function Update(ByVal usr As String) As Integer Implements IGeneral.Update")
            objWriter.WriteLine("_LogData = """"")
            objWriter.WriteLine("_LogData = GetObjectString()")
            objWriter.WriteLine("Return SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Update" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id" & updatestring & ", usr)")
            objWriter.WriteLine("End Function" & Chr(13))

            objWriter.WriteLine("Private Sub SetProperties(ByVal dr As DataRow)" & Chr(13))
            objWriter.WriteLine("_id = TypeSafeConversion.NullSafeLong(dr(""" & cols(0).Substring(1, cols(0).Length - 1) & """))")

            For i As Int32 = 1 To cols.Count - 2

                If cols(i) <> "_isdirty" Then
                    If types(i) = "DateTime" Then
                        objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafeDate(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                    ElseIf initialtypes(i) = "image" Then
                        objWriter.WriteLine("If" & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)" & "IsNot DBNull.Value Then")
                        objWriter.WriteLine(cols(i) & " = " & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("" & cols(i) & " =  " & "Nothing")
                        objWriter.WriteLine("End If")
                    Else
                        objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafe" & types(i) & "(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                    End If
                End If

                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " = Nothing")

                End If

            Next

            objWriter.WriteLine()
            objWriter.WriteLine("End Sub" & Chr(13))

            objWriter.WriteLine("Private Sub BlankProperties()" & Chr(13))
            objWriter.WriteLine("_id = 0")
            For i As Int32 = 1 To cols.Count - 2

                If types(i) <> "Boolean" Then
                    If types(i) = "DateTime" Or types(i) = "Date" Then
                        objWriter.WriteLine(cols(i) & " = " & "Now")
                    Else
                        If Lcasevalue.Contains(types(i)) Then
                            objWriter.WriteLine(cols(i) & " = " & """""")
                        ElseIf initialtypes(i) = "image" Then
                            objWriter.WriteLine(cols(i) & " = Nothing")
                        Else
                            objWriter.WriteLine(cols(i) & " = " & "0")
                        End If

                    End If
                Else
                    objWriter.WriteLine(cols(i) & " = " & "False")
                End If

                If ListofForeignKey.Contains(cols(i)) Then
                    objWriter.WriteLine("_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " = Nothing")
                End If


            Next

            objWriter.WriteLine()
            objWriter.WriteLine("End Sub" & Chr(13))

            objWriter.WriteLine("Public Function Read(ByVal _idpass As Long) As Boolean Implements IGeneral.Read")
            objWriter.WriteLine("Try " & Chr(13))

            objWriter.WriteLine("If _idpass <> 0 Then " & Chr(13) _
                            & "Dim ds As DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(),""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_ByID"", _idpass)" & Chr(13) & Chr(13) _
                            & "If ds.Tables(0).Rows.Count < 1 Then" & Chr(13) _
                            & "BlankProperties()" & Chr(13) _
                            & "Return False" & Chr(13) _
                            & "End If" & Chr(13) & Chr(13) _
                            & "SetProperties(ds.tables(0).rows(0))" & Chr(13) _
                            & "Else" & Chr(13) _
                            & "BlankProperties()" & Chr(13) _
                            & "End If" & Chr(13) _
                            & "Return True" & Chr(13) _
                            )

            objWriter.WriteLine("Catch ex As Exception" & Chr(13))
            objWriter.WriteLine("Throw ex" & Chr(13))
            objWriter.WriteLine("End Try" & Chr(13))
            objWriter.WriteLine("End Function" & Chr(13))

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                If _index.ListofColumn.Count = 1 Then
                    objWriter.WriteLine("Public Function Read_" & _index.ListofColumn.Item(0).Name & "(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean")
                    objWriter.WriteLine("Try " & Chr(13))
                    objWriter.WriteLine("If " & _index.ListofColumn.Item(0).Name & " <> """" Then ")
                    objWriter.WriteLine("Dim ds as Dataset = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & """, " & _index.ListofColumn.Item(0).Name & ")" & Chr(13))
                    objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine("BlankProperties()")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If" & Chr(13))
                    objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("BlankProperties()")
                    objWriter.WriteLine("End If" & Chr(13))
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                    objWriter.WriteLine("Throw ex" & Chr(13))
                    objWriter.WriteLine("End Try" & Chr(13))
                    objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                Else
                    Dim _strOfIndexToUse As String = String.Empty
                    Dim _strOfValueToUse As String = String.Empty
                    Dim _strParameterToUse As String = String.Empty
                    Dim ind As Integer = 0
                    For Each _column As Cls_Column In _index.ListofColumn
                        If _strOfIndexToUse.Length = 0 Then
                            _strOfIndexToUse = _column.Name
                            _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse = "_value" & ind
                        Else
                            _strOfIndexToUse += "_" & _column.Name
                            _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse += ", _value" & ind
                        End If
                        ind = ind + 1
                    Next
                    objWriter.WriteLine("Public Function Read_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean")
                    objWriter.WriteLine("Try " & Chr(13))
                    '  objWriter.WriteLine("If " & ListofIndex(i) & " <> """" Then ")
                    objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")
                    objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine("BlankProperties()")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If" & Chr(13))

                    objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                    'objWriter.WriteLine("Else")
                    'objWriter.WriteLine("BlankProperties()")
                    '  objWriter.WriteLine("End If" & Chr(13))

                    objWriter.WriteLine("Return True")

                    objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                    objWriter.WriteLine("Throw ex" & Chr(13))
                    objWriter.WriteLine("End Try" & Chr(13))
                    objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                End If
            Next

            objWriter.WriteLine("Public Sub Delete() Implements IGeneral.Delete" & Chr(13) _
                                & "Try" & Chr(13) _
                                & "SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), ""SP_Delete" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id)" & Chr(13) & Chr(13) _
                                & "Catch ex As SqlClient.SqlException" & Chr(13) _
                                & "Throw New System.Exception(ex.ErrorCode)" & Chr(13) _
                                & "End Try" & Chr(13) _
                                & "End Sub" & Chr(13)
                                )

            objWriter.WriteLine("Public Function Refresh() As Boolean Implements IGeneral.Refresh" & Chr(13) _
                                 & "If _id = 0 Then" & Chr(13) _
                                 & "Return False" & Chr(13) _
                                 & "Else" & Chr(13) _
                                 & "Read(_id)" & Chr(13) _
                                 & "Return True" & Chr(13) _
                                 & "End If" & Chr(13) _
                                 & "End Function" & Chr(13) _
                                 )

            objWriter.WriteLine("Public Function Save(ByVal usr As String) As Integer Implements IGeneral.Save" & Chr(13) _
                                 & "If _isdirty Then" & Chr(13) _
                                 & "Validation()" & Chr(13) & Chr(13) _
                                 & "If _id = 0 Then" & Chr(13) _
                                 & "Return Insert(usr)" & Chr(13) _
                                 & "Else" & Chr(13) _
                                 & "If _id > 0 Then" & Chr(13) _
                                 & "Return Update(usr)" & Chr(13) _
                                 & "Else" & Chr(13) _
                                 & "_id = 0" & Chr(13) _
                                 & "Return False" & Chr(13) _
                                 & "End If" & Chr(13) _
                                 & "End If" & Chr(13) _
                                 & "End If" & Chr(13) & Chr(13) _
                                 & "_isdirty = False" & Chr(13) _
                                 & "End Function"
                                 )


            objWriter.WriteLine("#End Region")

            objWriter.WriteLine()
            objWriter.WriteLine("#Region "" Search """)

            objWriter.WriteLine("Public Function Search() As System.Collections.ICollection Implements IGeneral.Search" & Chr(13) _
                                & "Return SearchAll()" & Chr(13) _
                                & "End Function" & Chr(13)
                                )


            objWriter.WriteLine("Public Shared Function SearchAll() As List(Of " & nomClasse & ")" & Chr(13) _
                                & "Try " & Chr(13) _
                                & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                & "Dim r As Data.DataRow" & Chr(13) _
                                & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & """)" & Chr(13) _
                                & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                & "objs.Add(obj)" & Chr(13) _
                                & "Next r" & Chr(13) _
                                & "Return objs"
                                )
            objWriter.WriteLine("Catch ex As Exception" & Chr(13))
            objWriter.WriteLine("Throw ex")
            objWriter.WriteLine("End Try")
            objWriter.WriteLine("End Function" & Chr(13))


            objWriter.WriteLine()

            objWriter.WriteLine("Public Shared Function SearchAllBy" & "AnyField" & "(Byval  columnName  As String, Byval columnvalue As String)  As List(Of " & nomClasse & ")" & Chr(13) _
                                        & "Try " & Chr(13) _
                                        & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                        & "Dim r As Data.DataRow" & Chr(13) _
                                        & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & "ByAnyColumn, columnName , columnvalue)" & Chr(13) _
                                        & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                        & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                        & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                        & "objs.Add(obj)" & Chr(13) _
                                        & "Next r" & Chr(13) _
                                        & "Return objs"
                                        )
            objWriter.WriteLine("Catch ex As Exception" & Chr(13))
            objWriter.WriteLine("Throw ex")
            objWriter.WriteLine("End Try")
            objWriter.WriteLine("End Function" & Chr(13))

            objWriter.WriteLine()

            For i As Int32 = 1 To cols.Count - 2
                If ListofForeignKey.Contains(cols(i)) Then
                    Dim searchtext = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

                    objWriter.WriteLine("Public Shared Function SearchAllBy" & searchtext & "(Byval " & cols(i).ToString.ToLower & " As " & types(i) & ") As List(Of " & nomClasse & ")" & Chr(13) _
                                        & "Try " & Chr(13) _
                                        & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                        & "Dim r As Data.DataRow" & Chr(13) _
                                        & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & cols(i) & """," & cols(i).ToString.ToLower & ")" & Chr(13) _
                                        & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                        & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                        & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                        & "objs.Add(obj)" & Chr(13) _
                                        & "Next r" & Chr(13) _
                                        & "Return objs"
                                        )
                    objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                    objWriter.WriteLine("Throw ex")
                    objWriter.WriteLine("End Try")
                    objWriter.WriteLine("End Function" & Chr(13))


                End If
            Next


            objWriter.WriteLine()
            Dim libelleFonction As String = ""
            Dim libelleStored As String = ""
            Dim paramfonction As String = ""
            Dim valueStored As String = ""
            For i As Integer = 0 To fields_String.Count - 1
                For Each _column In _table.ListofColumn
                    If _column.Name = fields_String(i) Then
                        If (libelleStored = "") Then
                            paramfonction = "ByVal " & _column.Name.ToLower & " As " & _column.Type.VbName
                            libelleFonction = "By" & _column.Name
                            libelleStored = "_" & _column.Name
                            valueStored = _column.Name.ToLower
                        Else
                            paramfonction &= Chr(13) & Chr(9) & Chr(9) & "," & "ByVal" & _column.Name.ToLower & " As " & _column.Type.VbName
                            libelleStored &= "_" & _column.Name
                            libelleFonction &= "By" & _column.Name
                            valueStored &= "," & _column.Name.ToLower
                        End If
                        Exit For
                    End If
                Next
            Next

            Dim objectname As String = name.Substring(4, name.Length - 4)
            objWriter.WriteLine("Public Shared Function SearchAll" & libelleFonction & "(" & paramfonction & ") As List(Of " & nomClasse & ")" & Chr(13) _
                           & "Try " & Chr(13) _
                           & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                           & "Dim r As Data.DataRow" & Chr(13) _
                           & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & objectname & libelleStored & ", " & valueStored & " "")" & Chr(13) _
                           & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                           & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                           & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                           & "objs.Add(obj)" & Chr(13) _
                           & "Next r" & Chr(13) _
                           & "Return objs"
                           )
            objWriter.WriteLine("Catch ex As Exception" & Chr(13))
            objWriter.WriteLine("Throw ex")
            objWriter.WriteLine("End Try")
            objWriter.WriteLine("End Function" & Chr(13))



            objWriter.WriteLine()
            objWriter.WriteLine("#End Region")
            objWriter.WriteLine()
            objWriter.WriteLine("#Region "" Other Methods """)


            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                If _index.ListofColumn.Count = 1 Then
                    objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(ByVal _value As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean ")
                    objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _index.ListofColumn.Item(0).Name & """, _value)")
                    objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine(" Return False")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("If _id = 0 Then")
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                Else
                    Dim _strOfIndexToUse As String = String.Empty
                    Dim _strOfValueToUse As String = String.Empty
                    Dim _strParameterToUse As String = String.Empty
                    Dim ind As Integer = 0
                    For Each _column In _index.ListofColumn
                        If _strOfIndexToUse.Length = 0 Then
                            _strOfIndexToUse = _column.Name
                            _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse = "_value" & ind
                        Else
                            _strOfIndexToUse += "_" & _column.Name
                            _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse += ", _value" & ind
                        End If
                        ind = ind + 1
                    Next
                    objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean ")
                    objWriter.WriteLine("Try" & Chr(13))
                    objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")

                    objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine(" Return False")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("If _id = 0 Then")
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                    objWriter.WriteLine("Return True")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End If")
                    objWriter.WriteLine("End If" & Chr(13))
                    objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                    objWriter.WriteLine("Throw ex" & Chr(13))
                    objWriter.WriteLine("End Try" & Chr(13))
                    objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                End If
            Next


            objWriter.WriteLine("Private Sub Validation() " & Chr(13))
            Dim stringlistforvalidation As New List(Of String) From {"String"}
            Dim decimalintegerforvalidation As New List(Of String) From {"Integer", "Long", "Decimal"}



            For i As Int32 = 1 To cols.Count - 2
                If stringlistforvalidation.Contains(types(i)) Then
                    objWriter.WriteLine("If " & cols(i) & " = """" Then " & Chr(13) _
                                        & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                        & "End If"
                                        )
                    objWriter.WriteLine()
                    objWriter.WriteLine("If Len(" & cols(i) & ") > " & length(i) & " Then" & Chr(13) _
                                        & "Throw (New System.Exception("" " & "Trop de caractères insérés pour" & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & "  (la longueur doit être inférieure a " & length(i) & " caractères.  )""))" & Chr(13) _
                                        & "End If"
                                        )
                    objWriter.WriteLine()
                End If

                If decimalintegerforvalidation.Contains(types(i)) Then
                    If ListofForeignKey.Contains(cols(i)) Then
                        objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                      & "Throw (New System.Exception("" " & cols(i).Substring(4, cols(i).Length - 4).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                      & "End If"
                                      )
                        objWriter.WriteLine()
                    Else
                        objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                       & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire.""))" & Chr(13) _
                                       & "End If"
                                       )
                        objWriter.WriteLine()
                    End If
                End If


            Next

            objWriter.WriteLine()

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                If _index.ListofColumn.Count = 1 Then
                    objWriter.WriteLine("If  FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(" & "_" & _index.ListofColumn.Item(0).Name & ") Then" & Chr(13) _
                                      & "Throw (New System.Exception(""Ce " & _index.ListofColumn.Item(0).Name & " est déjà enregistré.""))" & Chr(13) _
                                      & "End If"
                                     )
                    objWriter.WriteLine()
                Else
                    Dim _strOfIndexToUse As String = String.Empty
                    Dim _strOfValueToUse As String = String.Empty
                    Dim _strParameterToUse As String = String.Empty
                    Dim _parametervalueToUse As String = String.Empty
                    Dim ind As Integer = 0
                    For Each _column In _index.ListofColumn
                        If _strOfIndexToUse.Length = 0 Then
                            _strOfIndexToUse = _column.Name
                            _parametervalueToUse = _column.Name
                            _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse = "_value" & ind
                        Else
                            _strOfIndexToUse += "_" & _column.Name
                            _parametervalueToUse += ", " & _column.Name
                            _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                            _strParameterToUse += ", value" & ind
                        End If
                    Next

                    objWriter.WriteLine("If FoundAlreadyExist_" & _strOfIndexToUse & "(" & _parametervalueToUse & ") Then" & Chr(13) _
                                       & "Throw (New System.Exception(""Cette combinaison " & _strOfIndexToUse & " est déjà enregistrée.""))" & Chr(13) _
                                       & "End If"
                                       )
                    objWriter.WriteLine()
                End If
            Next

            objWriter.WriteLine("End Sub" & Chr(13))

            objWriter.WriteLine("Public Function Encode(ByVal str As Byte()) As String")
            objWriter.WriteLine("Return Convert.ToBase64String(str)")
            objWriter.WriteLine("End Function")
            objWriter.WriteLine()

            objWriter.WriteLine("Public Function Decode(ByVal str As String) As Byte()")
            objWriter.WriteLine("Dim decbuff As Byte() = Convert.FromBase64String(str)")
            objWriter.WriteLine("Return decbuff")
            objWriter.WriteLine("End Function")
            objWriter.WriteLine()

            objWriter.WriteLine(" Public Function GetObjectString() As String Implements IGeneral.GetObjectString" & Chr(13) _
                             & "Dim _old As New " & nomClasse & "(Me.ID)" & Chr(13) & Chr(13) _
                             & " Return LogData(New  " & nomClasse & "(Me.ID))" & Chr(13) _
                             & "Return LogStringBuilder.BuildLogStringChangesOnly(_old,Me)" & Chr(13) _
                             & "End Function" & Chr(13) _
                             )

            objWriter.WriteLine(" Function LogData(obj As Cls_Retard) As String " & Chr(13) _
                                    & "Return LogStringBuilder.BuildLogStringHTML(obj)" & Chr(13) _
                                    & "End Sub" & Chr(13) _
                                )
            objWriter.WriteLine(" Function LogData() As String" & Chr(13) _
                               & " Return LogStringBuilder.BuildLogStringHTML(Me)" & Chr(13) _
                               & "End Function" _
                               )

            objWriter.WriteLine("#End Region")
            objWriter.WriteLine()
            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateFileForParent(ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
            For Each group In Cls_GroupTable.SearchAll
                Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
                Dim Id_table As String = ""
                Dim _end As String
                Dim name As String = group.ParentTable.Name
                Dim ListofForeignKey As New List(Of String)
                Dim countForeignKey As Integer = 0
                Dim db As String = ""
                Dim Lcasevalue As New List(Of String) From {"String"}
                Dim nomClasse As String = group.ParentTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
                Dim nomSimpleParent As String = name.Substring(4, name.Length - 4)
                Dim nomSimpleChild As String = group.ChildTable.Name.Substring(4, group.ChildTable.Name.Length - 4)
                Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\VbNetClass\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\VbNetClass\")
                Dim path As String = txt_PathGenerate_Script & nomClasse & ".vb"

                Dim ListofIndex As New List(Of String)
                Dim ListofIndexType As New List(Of String)
                Dim index_li_type As New Hashtable
                Dim countindex As Long = 0
                Dim insertstring As String = ""
                Dim updatestring As String = ""
                Dim listoffound_virguleIndex As New List(Of String)
                Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                       & "REM  Class " + nomClasse & Chr(13) & Chr(13) _
                                       & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")
                header &= ""
                Dim content As String = "Public Class " & nomClasse & Chr(13) _
                                         & "Implements IGeneral"

                _end = "End Class" & Chr(13)
                ' Delete the file if it exists.
                If File.Exists(path) Then
                    File.Delete(path)
                End If

                REM on verifie si le repertoir existe bien       
                If Not Directory.Exists(txt_PathGenerate_Script) Then
                    Directory.CreateDirectory(txt_PathGenerate_Script)
                End If
                ' Create the file.
                Dim fs As FileStream = File.Create(path, 1024)
                fs.Close()



                Dim objWriter As New System.IO.StreamWriter(path, True)
                '  objWriter.WriteLine(header)
                If ListBox_NameSpace.Items.Count > 0 Then
                    For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                        objWriter.WriteLine(ListBox_NameSpace.Items(i))
                    Next
                End If
                objWriter.WriteLine()
                objWriter.WriteLine(content)
                objWriter.WriteLine()
                Dim _table As New Cls_Table()

                _table.Read(_systeme.currentDatabase.ID, group.ParentTable.Name)

                Dim cols As New List(Of String)
                Dim types As New List(Of String)
                Dim initialtypes As New List(Of String)
                Dim length As New List(Of String)
                Dim count As Integer = 0

                Dim cap As Integer

                cap = _table.ListofColumn.Count


                Id_table = _table.ListofColumn.Item(0).Name

                Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

                For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                    ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                    countForeignKey = countForeignKey + 1
                Next

                For Each _column As Cls_Column In _table.ListofColumn
                    If count < cap - 4 Then
                        cols.Add("_" & _column.Name)
                        types.Add(_column.Type.VbName)
                        initialtypes.Add(_column.Type.SqlServerName)
                        length.Add(_column.Length)
                        count += 1
                    Else
                        Exit For
                    End If
                Next

                cols.Add("_isdirty")
                cols.Add("_LogData")
                types.Add("Boolean")
                types.Add("String")
                initialtypes.Add("Byte")
                initialtypes.Add("nvarchar")
                objWriter.WriteLine("#Region ""Attribut""")
                objWriter.WriteLine("Private _id As Long")
                objWriter.WriteLine()
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        If Not nottoputforlist.Contains(cols(i)) Then
                            If initialtypes(i).ToString() = "image" Then
                                insertstring &= ", " & "IIf(" & cols(i) & " Is Nothing, DBNull.Value, " & cols(i) & ")"
                            Else
                                insertstring &= ", " & cols(i)
                                updatestring &= ", " & cols(i)
                            End If
                        End If
                        objWriter.WriteLine("Private " & cols(i) & " As " & types(i))
                        If ListofForeignKey.Contains(cols(i)) Then

                            Dim valueColumn As New Cls_Column(_table.ID, cols(i).ToString.Substring(1, cols(i).Length - 1))
                            Dim foreingn As New Cls_ForeignKey(_table.ID, valueColumn.ID)

                            Dim ClassName As String = "Cls_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                            ClassName = foreingn.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

                            objWriter.WriteLine("Private _" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " As " & ClassName & "")
                        End If
                    Next
                Catch ex As Exception

                End Try
                objWriter.WriteLine()
                objWriter.WriteLine("#End Region")
                objWriter.WriteLine()

                objWriter.WriteLine("#Region ""New""")
                objWriter.WriteLine("Public Sub New()")
                objWriter.WriteLine("BlankProperties()")
                objWriter.WriteLine("End Sub")
                objWriter.WriteLine()
                objWriter.WriteLine("Public Sub New(ByVal _idOne As Long)")
                objWriter.WriteLine("Read(_idOne)")
                objWriter.WriteLine("End Sub" & Chr(13))


                For Each _index As Cls_UniqueIndex In _table.ListofIndex
                    If _index.ListofColumn.Count > 1 Then
                    Else
                        objWriter.WriteLine("Public Sub New(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ")")
                        objWriter.WriteLine("Read_" & _index.ListofColumn.Item(0).Name & "(" & _index.ListofColumn.Item(0).Name & ")")
                        objWriter.WriteLine("End Sub " & Chr(13))
                    End If
                Next


                objWriter.WriteLine("#End Region")

                objWriter.WriteLine()

                objWriter.WriteLine("#Region ""Properties""")
                objWriter.WriteLine("<AttributLogData(True,1)> _")
                objWriter.WriteLine("Public ReadOnly Property ID() As Long Implements IGeneral.ID")
                objWriter.WriteLine("Get")
                objWriter.WriteLine("Return _id")
                objWriter.WriteLine("End Get")
                objWriter.WriteLine("End Property")
                objWriter.WriteLine()

                For i As Int32 = 1 To cols.Count - 3 ''On ne cree pas de property pour la derniere column
                    Dim propName As String = ""
                    Dim s As String() = cols(i).Split("_")
                    For j As Integer = 1 To s.Length - 1
                        propName &= StrConv(s(j), VbStrConv.ProperCase)
                    Next
                    propName = cols(i).Substring(1, cols(i).Length - 1)
                    'propName = StrConv(cols(i).Split("_")(1), VbStrConv.ProperCase) & StrConv(cols(i).Split("_")(2), VbStrConv.ProperCase)
                    Dim log As String = "<AttributLogData(True, " & i + 1 & ")> _"
                    Dim attrib As String = "Public Property  " & propName & " As " & types(i)

                    If cols(i) <> "_isdirty" Or cols(i) <> "_LogData" Then
                        objWriter.WriteLine(log)
                        objWriter.WriteLine(attrib)
                        objWriter.WriteLine("Get" & Chr(13) _
                                            & " Return " & cols(i) & Chr(13) _
                                            & "End Get")

                        If Lcasevalue.Contains(types(i)) Then
                            objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                          & " If LCase(Trim(" & cols(i) & ")) <> LCase(Trim(Value)) Then" & Chr(13) _
                                          & "_isdirty = True " & Chr(13) _
                                          & cols(i) & " = Trim(Value)" & Chr(13) _
                                          & "End If" & Chr(13) _
                                          & "End Set" & Chr(13) _
                                          & "End Property")
                        Else
                            objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                          & " If " & cols(i) & " <> Value Then" & Chr(13) _
                                          & "_isdirty = True " & Chr(13) _
                                          & cols(i) & " = Value" & Chr(13) _
                                          & "End If" & Chr(13) _
                                          & "End Set" & Chr(13) _
                                          & "End Property")
                        End If

                        objWriter.WriteLine()

                        If ListofForeignKey.Contains(cols(i)) Then
                            Dim attributUsed As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                            Dim valueColumn As New Cls_Column(_table.ID, cols(i).ToString.Substring(1, cols(i).Length - 1))
                            Dim foreingn As New Cls_ForeignKey(_table.ID, valueColumn.ID)

                            Dim ClassName As String = "Cls_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                            ClassName = foreingn.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

                            Dim simplename As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                            objWriter.WriteLine("Public Property " & attributUsed & " As " & ClassName & Chr(13))
                            objWriter.WriteLine("Get")
                            objWriter.WriteLine("If Not (_" & attributUsed & " Is Nothing) Then" & Chr(13) _
                                                & "If (_" & attributUsed & ".ID = 0) Or (_" & attributUsed & ".ID <>  " & cols(i) & ") Then" & Chr(13) _
                                                & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                                & "End If" & Chr(13) _
                                                & "Else" & Chr(13) _
                                                & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                                & "End If" & Chr(13) & Chr(13) _
                                                & "Return _" & attributUsed & Chr(13) _
                                                & "End Get" & Chr(13) _
                                                & "Set(ByVal value As " & ClassName & ")" & Chr(13) _
                                                & "If Value Is Nothing Then" & Chr(13) _
                                                & "_isdirty = True" & Chr(13) _
                                                & cols(i) & " = 0" & Chr(13) _
                                                & "Else" & Chr(13) _
                                                & "If _" & attributUsed & ".ID <> Value.ID Then" & Chr(13) _
                                                & "_isdirty = True" & Chr(13) _
                                                & cols(i) & " = Value.ID" & Chr(13) _
                                                & "End If" & Chr(13) _
                                                & "End If" & Chr(13) _
                                                & "End Set" & Chr(13) _
                                                & "End Property" & Chr(13) _
                                                )
                            objWriter.WriteLine()
                            Dim foreignkey As New Cls_ForeignKey(_table.ID, cols(i).Substring(1, cols(i).Length - 1))
                            Dim textForcombo As String = ""

                            Dim compteur As Integer = 0

                            For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                                If column_fore.Type.VbName = "String" Then
                                    objWriter.WriteLine("Public ReadOnly Property " & simplename & "_" & column_fore.Name & "  As String ")
                                    objWriter.WriteLine("Get")
                                    objWriter.WriteLine("Return " & simplename & "." & column_fore.Name & "")
                                    objWriter.WriteLine("End Get")
                                    objWriter.WriteLine("End Property")
                                    objWriter.WriteLine()
                                End If
                                If compteur = foreignkey.RefTable.ListofColumn.Count - 5 Then
                                    Exit For
                                End If
                                compteur = compteur + 1
                            Next
                        End If
                    End If
                    If initialtypes(i).ToString() = "image" Then
                        objWriter.WriteLine("Public Property " & cols(i).Substring(1, cols(i).Length - 1) & "String() As String")
                        objWriter.WriteLine("Get")
                        objWriter.WriteLine("If " & cols(i) & " IsNot Nothing Then")
                        objWriter.WriteLine("Return Encode(" & cols(i) & " )")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("Return """"")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End Get")
                        objWriter.WriteLine("Set(ByVal Value As String)")
                        objWriter.WriteLine(cols(i) & " = Decode(Value)")
                        objWriter.WriteLine("_isdirty = True")
                        objWriter.WriteLine("End Set")
                        objWriter.WriteLine("End Property")
                    End If

                Next


                objWriter.WriteLine("Public ReadOnly Property " & group.ChildTable.Name.Replace("tbl_", "") & "s" & "  As List(of " & group.ChildTable.Name.Replace("tbl_", "Cls_") & " ) ")
                objWriter.WriteLine("Get")
                objWriter.WriteLine("Return " & group.ChildTable.Name.Replace("tbl_", "Cls_") & ".SearchAllIn" & group.ParentTable.Name.Replace("tbl_", "") & "( _id")
                objWriter.WriteLine("End Get")
                objWriter.WriteLine("End Property")
                objWriter.WriteLine()


                With objWriter
                    .WriteLine(" Public ReadOnly Property IsDataDirty() As Boolean")
                    .WriteLine("Get")
                    .WriteLine("Return _isdirty")
                    .WriteLine("End Get")
                    .WriteLine("End Property")
                    .WriteLine()
                End With

                'With objWriter
                '    .WriteLine("Public ReadOnly Property LogData() As String")
                '    .WriteLine("Get")
                '    .WriteLine("Return _LogData")
                '    .WriteLine("End Get")
                '    .WriteLine("End Property")
                'End With


                objWriter.WriteLine("#End Region")

                objWriter.WriteLine()
                objWriter.WriteLine("#Region "" Db Access """)
                objWriter.WriteLine("Public Function Insert(ByVal usr As String) As Integer Implements IGeneral.Insert")
                objWriter.WriteLine("_LogData = LogData(Me)")
                objWriter.WriteLine("_id = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Insert" & nomClasse.Substring(4, nomClasse.Length - 4) & """" & insertstring & ", usr))")
                objWriter.WriteLine("Return _id")
                objWriter.WriteLine("End Function")

                objWriter.WriteLine()
                objWriter.WriteLine("Public Function Update(ByVal usr As String) As Integer Implements IGeneral.Update")
                'objWriter.WriteLine("_LogData = """"")
                objWriter.WriteLine("_LogData = GetObjectString()")
                objWriter.WriteLine("Return SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Update" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id" & updatestring & ", usr)")
                objWriter.WriteLine("End Function" & Chr(13))

                objWriter.WriteLine("Private Sub SetProperties(ByVal dr As DataRow)" & Chr(13))
                objWriter.WriteLine("_id = TypeSafeConversion.NullSafeLong(dr(""" & cols(0).Substring(1, cols(0).Length - 1) & """))")

                For i As Int32 = 1 To cols.Count - 2

                    If cols(i) <> "_isdirty" Then
                        If types(i) = "DateTime" Then
                            objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafeDate(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                        ElseIf initialtypes(i) = "image" Then
                            objWriter.WriteLine("If" & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)" & "IsNot DBNull.Value Then")
                            objWriter.WriteLine(cols(i) & " = " & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)")
                            objWriter.WriteLine("Else")
                            objWriter.WriteLine("" & cols(i) & " =  " & "Nothing")
                            objWriter.WriteLine("End If")
                        Else
                            objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafe" & types(i) & "(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                        End If
                    End If

                    If ListofForeignKey.Contains(cols(i)) Then
                        objWriter.WriteLine("_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " = Nothing")

                    End If

                Next

                objWriter.WriteLine()
                objWriter.WriteLine("End Sub" & Chr(13))

                objWriter.WriteLine("Private Sub BlankProperties()" & Chr(13))
                objWriter.WriteLine("_id = 0")
                For i As Int32 = 1 To cols.Count - 2

                    If types(i) <> "Boolean" Then
                        If types(i) = "DateTime" Or types(i) = "Date" Then
                            objWriter.WriteLine(cols(i) & " = " & "Now")
                        Else
                            If Lcasevalue.Contains(types(i)) Then
                                objWriter.WriteLine(cols(i) & " = " & """""")
                            ElseIf initialtypes(i) = "image" Then
                                objWriter.WriteLine(cols(i) & " = Nothing")
                            Else
                                objWriter.WriteLine(cols(i) & " = " & "0")
                            End If

                        End If
                    Else
                        objWriter.WriteLine(cols(i) & " = " & "False")
                    End If

                    If ListofForeignKey.Contains(cols(i)) Then
                        objWriter.WriteLine("_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " = Nothing")
                    End If


                Next

                objWriter.WriteLine()
                objWriter.WriteLine("End Sub" & Chr(13))

                objWriter.WriteLine("Public Function Read(ByVal _idpass As Long) As Boolean Implements IGeneral.Read")
                objWriter.WriteLine("Try " & Chr(13))

                objWriter.WriteLine("If _idpass <> 0 Then " & Chr(13) _
                                & "Dim ds As DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(),""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_ByID"", _idpass)" & Chr(13) & Chr(13) _
                                & "If ds.Tables(0).Rows.Count < 1 Then" & Chr(13) _
                                & "BlankProperties()" & Chr(13) _
                                & "Return False" & Chr(13) _
                                & "End If" & Chr(13) & Chr(13) _
                                & "SetProperties(ds.tables(0).rows(0))" & Chr(13) _
                                & "Else" & Chr(13) _
                                & "BlankProperties()" & Chr(13) _
                                & "End If" & Chr(13) _
                                & "Return True" & Chr(13) _
                                )

                objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                objWriter.WriteLine("Throw ex" & Chr(13))
                objWriter.WriteLine("End Try" & Chr(13))
                objWriter.WriteLine("End Function" & Chr(13))

                For Each _index As Cls_UniqueIndex In _table.ListofIndex
                    If _index.ListofColumn.Count = 1 Then
                        objWriter.WriteLine("Public Function Read_" & _index.ListofColumn.Item(0).Name & "(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean")
                        objWriter.WriteLine("Try " & Chr(13))
                        objWriter.WriteLine("If " & _index.ListofColumn.Item(0).Name & " <> """" Then ")
                        objWriter.WriteLine("Dim ds as Dataset = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & """, " & _index.ListofColumn.Item(0).Name & ")" & Chr(13))
                        objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                        objWriter.WriteLine("BlankProperties()")
                        objWriter.WriteLine("Return False")
                        objWriter.WriteLine("End If" & Chr(13))
                        objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("BlankProperties()")
                        objWriter.WriteLine("End If" & Chr(13))
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                        objWriter.WriteLine("Throw ex" & Chr(13))
                        objWriter.WriteLine("End Try" & Chr(13))
                        objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                    Else
                        Dim _strOfIndexToUse As String = String.Empty
                        Dim _strOfValueToUse As String = String.Empty
                        Dim _strParameterToUse As String = String.Empty
                        Dim ind As Integer = 0
                        For Each _column As Cls_Column In _index.ListofColumn
                            If _strOfIndexToUse.Length = 0 Then
                                _strOfIndexToUse = _column.Name
                                _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse = "_value" & ind
                            Else
                                _strOfIndexToUse += "_" & _column.Name
                                _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse += ", _value" & ind
                            End If
                            ind = ind + 1
                        Next
                        objWriter.WriteLine("Public Function Read_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean")
                        objWriter.WriteLine("Try " & Chr(13))
                        '  objWriter.WriteLine("If " & ListofIndex(i) & " <> """" Then ")
                        objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")
                        objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                        objWriter.WriteLine("BlankProperties()")
                        objWriter.WriteLine("Return False")
                        objWriter.WriteLine("End If" & Chr(13))

                        objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                        'objWriter.WriteLine("Else")
                        'objWriter.WriteLine("BlankProperties()")
                        '  objWriter.WriteLine("End If" & Chr(13))

                        objWriter.WriteLine("Return True")

                        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                        objWriter.WriteLine("Throw ex" & Chr(13))
                        objWriter.WriteLine("End Try" & Chr(13))
                        objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                    End If
                Next

                objWriter.WriteLine("Public Sub Delete() Implements IGeneral.Delete" & Chr(13) _
                                    & "Try" & Chr(13) _
                                    & "SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), ""SP_Delete" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id)" & Chr(13) & Chr(13) _
                                    & "Catch ex As SqlClient.SqlException" & Chr(13) _
                                    & "Throw New System.Exception(ex.ErrorCode)" & Chr(13) _
                                    & "End Try" & Chr(13) _
                                    & "End Sub" & Chr(13)
                                    )

                objWriter.WriteLine("Public Function Refresh() As Boolean Implements IGeneral.Refresh" & Chr(13) _
                                     & "If _id = 0 Then" & Chr(13) _
                                     & "Return False" & Chr(13) _
                                     & "Else" & Chr(13) _
                                     & "Read(_id)" & Chr(13) _
                                     & "Return True" & Chr(13) _
                                     & "End If" & Chr(13) _
                                     & "End Function" & Chr(13) _
                                     )

                objWriter.WriteLine("Public Function Save(ByVal usr As String) As Integer Implements IGeneral.Save" & Chr(13) _
                                     & "If _isdirty Then" & Chr(13) _
                                     & "Validation()" & Chr(13) & Chr(13) _
                                     & "If _id = 0 Then" & Chr(13) _
                                     & "Return Insert(usr)" & Chr(13) _
                                     & "Else" & Chr(13) _
                                     & "If _id > 0 Then" & Chr(13) _
                                     & "Return Update(usr)" & Chr(13) _
                                     & "Else" & Chr(13) _
                                     & "_id = 0" & Chr(13) _
                                     & "Return False" & Chr(13) _
                                     & "End If" & Chr(13) _
                                     & "End If" & Chr(13) _
                                     & "End If" & Chr(13) & Chr(13) _
                                     & "_isdirty = False" & Chr(13) _
                                     & "End Function"
                                     )
                objWriter.WriteLine()

                objWriter.WriteLine("Public Overloads Function Add" & group.ChildTable.Name.Replace("tbl_", "") & "(ByVal obj As " & group.ChildTable.Name.Replace("tbl_", "Cls_") & ", ByVal usr As String) As Boolean")
                objWriter.WriteLine("        Try")
                objWriter.WriteLine("SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), ""SP_Update" & group.ParentTable.Name.Replace("tbl_", "") & "Add" & group.ChildTable.Name.Replace("tbl_", "") & """, _id, obj.ID, usr)")

                objWriter.WriteLine("Return True")
                objWriter.WriteLine("Catch")
                objWriter.WriteLine("Return False")
                objWriter.WriteLine("End Try")
                objWriter.WriteLine("End Sub")


                objWriter.WriteLine("Public Overloads Function Remove" & group.ChildTable.Name.Replace("tbl_", "") & "(ByVal obj As " & group.ParentTable.Name.Replace("tbl_", "Cls_") & ") As Boolean")
                objWriter.WriteLine("Try")
                objWriter.WriteLine("SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), ""SP_Update" & group.ParentTable.Name.Replace("tbl_", "") & "Rmv" & group.ChildTable.Name.Replace("tbl_", "") & """, _id, obj.ID)")
                objWriter.WriteLine("")
                objWriter.WriteLine("Return True")
                objWriter.WriteLine("Catch")
                objWriter.WriteLine("Return False")
                objWriter.WriteLine("End Try")
                objWriter.WriteLine("End Sub")

                objWriter.WriteLine()



                objWriter.WriteLine("#End Region")

                objWriter.WriteLine()
                objWriter.WriteLine("#Region "" Search """)

                objWriter.WriteLine("Public Function Search() As System.Collections.ICollection Implements IGeneral.Search" & Chr(13) _
                                    & "Return SearchAll()" & Chr(13) _
                                    & "End Function" & Chr(13)
                                    )


                objWriter.WriteLine("Public Shared Function SearchAll() As List(Of " & nomClasse & ")" & Chr(13) _
                                    & "Try " & Chr(13) _
                                    & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                    & "Dim r As Data.DataRow" & Chr(13) _
                                    & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & """)" & Chr(13) _
                                    & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                    & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                    & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                    & "objs.Add(obj)" & Chr(13) _
                                    & "Next r" & Chr(13) _
                                    & "Return objs"
                                    )
                objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                objWriter.WriteLine("Throw ex")
                objWriter.WriteLine("End Try")
                objWriter.WriteLine("End Function" & Chr(13))


                objWriter.WriteLine()

                For i As Int32 = 1 To cols.Count - 2
                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim searchtext = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

                        objWriter.WriteLine("Public Shared Function SearchAllBy" & searchtext & "(Byval " & cols(i).ToString.ToLower & " As " & types(i) & ") As List(Of " & nomClasse & ")" & Chr(13) _
                                            & "Try " & Chr(13) _
                                            & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                            & "Dim r As Data.DataRow" & Chr(13) _
                                            & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & cols(i) & """," & cols(i).ToString.ToLower & ")" & Chr(13) _
                                            & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                            & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                            & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                            & "objs.Add(obj)" & Chr(13) _
                                            & "Next r" & Chr(13) _
                                            & "Return objs"
                                            )
                        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                        objWriter.WriteLine("Throw ex")
                        objWriter.WriteLine("End Try")
                        objWriter.WriteLine("End Function" & Chr(13))


                    End If
                Next
                objWriter.WriteLine()
                objWriter.WriteLine("#End Region")
                objWriter.WriteLine()
                objWriter.WriteLine("#Region "" Other Methods """)


                For Each _index As Cls_UniqueIndex In _table.ListofIndex
                    If _index.ListofColumn.Count = 1 Then
                        objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(ByVal _value As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean ")
                        objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _index.ListofColumn.Item(0).Name & """, _value)")
                        objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                        objWriter.WriteLine(" Return False")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("If _id = 0 Then")
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("Return False")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                    Else
                        Dim _strOfIndexToUse As String = String.Empty
                        Dim _strOfValueToUse As String = String.Empty
                        Dim _strParameterToUse As String = String.Empty
                        Dim ind As Integer = 0
                        For Each _column In _index.ListofColumn
                            If _strOfIndexToUse.Length = 0 Then
                                _strOfIndexToUse = _column.Name
                                _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse = "_value" & ind
                            Else
                                _strOfIndexToUse += "_" & _column.Name
                                _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse += ", _value" & ind
                            End If
                            ind = ind + 1
                        Next
                        objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean ")
                        objWriter.WriteLine("Try" & Chr(13))
                        objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")

                        objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                        objWriter.WriteLine(" Return False")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("If _id = 0 Then")
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("Return False")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End If" & Chr(13))
                        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                        objWriter.WriteLine("Throw ex" & Chr(13))
                        objWriter.WriteLine("End Try" & Chr(13))
                        objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                    End If
                Next


                objWriter.WriteLine("Private Sub Validation() " & Chr(13))
                Dim stringlistforvalidation As New List(Of String) From {"String"}
                Dim decimalintegerforvalidation As New List(Of String) From {"Integer", "Long", "Decimal"}



                For i As Int32 = 1 To cols.Count - 2
                    If stringlistforvalidation.Contains(types(i)) Then
                        objWriter.WriteLine("If " & cols(i) & " = """" Then " & Chr(13) _
                                            & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                            & "End If"
                                            )
                        objWriter.WriteLine()
                        objWriter.WriteLine("If Len(" & cols(i) & ") > " & length(i) & " Then" & Chr(13) _
                                            & "Throw (New System.Exception("" " & "Trop de caractères insérés pour" & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & "  (la longueur doit être inférieure a " & length(i) & " caractères.  )""))" & Chr(13) _
                                            & "End If"
                                            )
                        objWriter.WriteLine()
                    End If

                    If decimalintegerforvalidation.Contains(types(i)) Then
                        If ListofForeignKey.Contains(cols(i)) Then
                            objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                          & "Throw (New System.Exception("" " & cols(i).Substring(4, cols(i).Length - 4).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                          & "End If"
                                          )
                            objWriter.WriteLine()
                        Else
                            objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                           & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire.""))" & Chr(13) _
                                           & "End If"
                                           )
                            objWriter.WriteLine()
                        End If
                    End If


                Next

                objWriter.WriteLine()

                For Each _index As Cls_UniqueIndex In _table.ListofIndex
                    If _index.ListofColumn.Count = 1 Then
                        objWriter.WriteLine("If  FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(" & "_" & _index.ListofColumn.Item(0).Name & ") Then" & Chr(13) _
                                          & "Throw (New System.Exception(""Ce " & _index.ListofColumn.Item(0).Name & " est déjà enregistré.""))" & Chr(13) _
                                          & "End If"
                                         )
                        objWriter.WriteLine()
                    Else
                        Dim _strOfIndexToUse As String = String.Empty
                        Dim _strOfValueToUse As String = String.Empty
                        Dim _strParameterToUse As String = String.Empty
                        Dim _parametervalueToUse As String = String.Empty
                        Dim ind As Integer = 0
                        For Each _column In _index.ListofColumn
                            If _strOfIndexToUse.Length = 0 Then
                                _strOfIndexToUse = _column.Name
                                _parametervalueToUse = _column.Name
                                _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse = "_value" & ind
                            Else
                                _strOfIndexToUse += "_" & _column.Name
                                _parametervalueToUse += ", " & _column.Name
                                _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse += ", value" & ind
                            End If
                        Next

                        objWriter.WriteLine("If FoundAlreadyExist_" & _strOfIndexToUse & "(" & _parametervalueToUse & ") Then" & Chr(13) _
                                           & "Throw (New System.Exception(""Cette combinaison " & _strOfIndexToUse & " est déjà enregistrée.""))" & Chr(13) _
                                           & "End If"
                                           )
                        objWriter.WriteLine()
                    End If
                Next

                objWriter.WriteLine("End Sub" & Chr(13))

                objWriter.WriteLine("Public Function Encode(ByVal str As Byte()) As String")
                objWriter.WriteLine("Return Convert.ToBase64String(str)")
                objWriter.WriteLine("End Function")
                objWriter.WriteLine()

                objWriter.WriteLine("Public Function Decode(ByVal str As String) As Byte()")
                objWriter.WriteLine("Dim decbuff As Byte() = Convert.FromBase64String(str)")
                objWriter.WriteLine("Return decbuff")
                objWriter.WriteLine("End Function")
                objWriter.WriteLine()

                objWriter.WriteLine(" Public Function GetObjectString() As String Implements IGeneral.GetObjectString" & Chr(13) _
                                 & "Dim _old As New " & nomClasse & "(Me.ID)" & Chr(13) & Chr(13) _
                                 & "Return LogStringBuilder.BuildLogStringChangesOnly(_old,Me)" & Chr(13) _
                                 & "End Function" & Chr(13) _
                                 )
                objWriter.WriteLine()

                objWriter.WriteLine("#End Region")
                objWriter.WriteLine()
                objWriter.WriteLine(_end)
                objWriter.WriteLine()
                objWriter.Close()
            Next
        End Sub

        Public Shared Sub CreateFileForChild(ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox)
            For Each group In Cls_GroupTable.SearchAll
                Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
                Dim Id_table As String = ""
                Dim _end As String
                Dim name As String = group.ParentTable.Name
                Dim ListofForeignKey As New List(Of String)
                Dim countForeignKey As Integer = 0
                Dim db As String = ""
                Dim Lcasevalue As New List(Of String) From {"String"}
                Dim nomClasse As String = group.ParentTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
                Dim nomSimpleParent As String = name.Substring(4, name.Length - 4)
                Dim nomSimpleChild As String = group.ChildTable.Name.Substring(4, group.ChildTable.Name.Length - 4)
                Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\VbNetClass\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\VbNetClass\")
                Dim path As String = txt_PathGenerate_Script & nomClasse & ".vb"

                Dim ListofIndex As New List(Of String)
                Dim ListofIndexType As New List(Of String)
                Dim index_li_type As New Hashtable
                Dim countindex As Long = 0
                Dim insertstring As String = ""
                Dim updatestring As String = ""
                Dim listoffound_virguleIndex As New List(Of String)
                Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                       & "REM  Class " + nomClasse & Chr(13) & Chr(13) _
                                       & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")
                header &= ""
                Dim content As String = "Public Class " & nomClasse & Chr(13) _
                                         & "Implements IGeneral"

                _end = "End Class" & Chr(13)
                ' Delete the file if it exists.
                If File.Exists(path) Then
                    File.Delete(path)
                End If

                REM on verifie si le repertoir existe bien       
                If Not Directory.Exists(txt_PathGenerate_Script) Then
                    Directory.CreateDirectory(txt_PathGenerate_Script)
                End If
                ' Create the file.
                Dim fs As FileStream = File.Create(path, 1024)
                fs.Close()



                Dim objWriter As New System.IO.StreamWriter(path, True)
                '  objWriter.WriteLine(header)
                If ListBox_NameSpace.Items.Count > 0 Then
                    For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                        objWriter.WriteLine(ListBox_NameSpace.Items(i))
                    Next
                End If
                objWriter.WriteLine()
                objWriter.WriteLine(content)
                objWriter.WriteLine()
                Dim _table As New Cls_Table()

                _table.Read(_systeme.currentDatabase.ID, group.ParentTable.Name)

                Dim cols As New List(Of String)
                Dim types As New List(Of String)
                Dim initialtypes As New List(Of String)
                Dim length As New List(Of String)
                Dim count As Integer = 0

                Dim cap As Integer

                cap = _table.ListofColumn.Count


                Id_table = _table.ListofColumn.Item(0).Name

                Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

                For Each _foreingkey As Cls_ForeignKey In _table.ListofForeinKey
                    ListofForeignKey.Add("_" & _foreingkey.Column.Name)
                    countForeignKey = countForeignKey + 1
                Next

                For Each _column As Cls_Column In _table.ListofColumn
                    If count < cap - 4 Then
                        cols.Add("_" & _column.Name)
                        types.Add(_column.Type.VbName)
                        initialtypes.Add(_column.Type.SqlServerName)
                        length.Add(_column.Length)
                        count += 1
                    Else
                        Exit For
                    End If
                Next

                cols.Add("_isdirty")
                cols.Add("_LogData")
                types.Add("Boolean")
                types.Add("String")
                initialtypes.Add("Byte")
                initialtypes.Add("nvarchar")
                objWriter.WriteLine("#Region ""Attribut""")
                objWriter.WriteLine("Private _id As Long")
                objWriter.WriteLine()
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        If Not nottoputforlist.Contains(cols(i)) Then
                            If initialtypes(i).ToString() = "image" Then
                                insertstring &= ", " & "IIf(" & cols(i) & " Is Nothing, DBNull.Value, " & cols(i) & ")"
                            Else
                                insertstring &= ", " & cols(i)
                                updatestring &= ", " & cols(i)
                            End If
                        End If
                        objWriter.WriteLine("Private " & cols(i) & " As " & types(i))
                        If ListofForeignKey.Contains(cols(i)) Then

                            Dim valueColumn As New Cls_Column(_table.ID, cols(i).ToString.Substring(1, cols(i).Length - 1))
                            Dim foreingn As New Cls_ForeignKey(_table.ID, valueColumn.ID)

                            Dim ClassName As String = "Cls_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                            ClassName = foreingn.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

                            objWriter.WriteLine("Private _" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " As " & ClassName & "")
                        End If
                    Next
                Catch ex As Exception

                End Try
                objWriter.WriteLine()
                objWriter.WriteLine("#End Region")
                objWriter.WriteLine()

                objWriter.WriteLine("#Region ""New""")
                objWriter.WriteLine("Public Sub New()")
                objWriter.WriteLine("BlankProperties()")
                objWriter.WriteLine("End Sub")
                objWriter.WriteLine()
                objWriter.WriteLine("Public Sub New(ByVal _idOne As Long)")
                objWriter.WriteLine("Read(_idOne)")
                objWriter.WriteLine("End Sub" & Chr(13))


                For Each _index As Cls_UniqueIndex In _table.ListofIndex
                    If _index.ListofColumn.Count > 1 Then
                    Else
                        objWriter.WriteLine("Public Sub New(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ")")
                        objWriter.WriteLine("Read_" & _index.ListofColumn.Item(0).Name & "(" & _index.ListofColumn.Item(0).Name & ")")
                        objWriter.WriteLine("End Sub " & Chr(13))
                    End If
                Next


                objWriter.WriteLine("#End Region")

                objWriter.WriteLine()

                objWriter.WriteLine("#Region ""Properties""")
                objWriter.WriteLine("<AttributLogData(True,1)> _")
                objWriter.WriteLine("Public ReadOnly Property ID() As Long Implements IGeneral.ID")
                objWriter.WriteLine("Get")
                objWriter.WriteLine("Return _id")
                objWriter.WriteLine("End Get")
                objWriter.WriteLine("End Property")
                objWriter.WriteLine()

                For i As Int32 = 1 To cols.Count - 3 ''On ne cree pas de property pour la derniere column
                    Dim propName As String = ""
                    Dim s As String() = cols(i).Split("_")
                    For j As Integer = 1 To s.Length - 1
                        propName &= StrConv(s(j), VbStrConv.ProperCase)
                    Next
                    propName = cols(i).Substring(1, cols(i).Length - 1)
                    'propName = StrConv(cols(i).Split("_")(1), VbStrConv.ProperCase) & StrConv(cols(i).Split("_")(2), VbStrConv.ProperCase)
                    Dim log As String = "<AttributLogData(True, " & i + 1 & ")> _"
                    Dim attrib As String = "Public Property  " & propName & " As " & types(i)

                    If cols(i) <> "_isdirty" Or cols(i) <> "_LogData" Then
                        objWriter.WriteLine(log)
                        objWriter.WriteLine(attrib)
                        objWriter.WriteLine("Get" & Chr(13) _
                                            & " Return " & cols(i) & Chr(13) _
                                            & "End Get")

                        If Lcasevalue.Contains(types(i)) Then
                            objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                          & " If LCase(Trim(" & cols(i) & ")) <> LCase(Trim(Value)) Then" & Chr(13) _
                                          & "_isdirty = True " & Chr(13) _
                                          & cols(i) & " = Trim(Value)" & Chr(13) _
                                          & "End If" & Chr(13) _
                                          & "End Set" & Chr(13) _
                                          & "End Property")
                        Else
                            objWriter.WriteLine("Set(ByVal Value As " & types(i) & ")" & Chr(13) _
                                          & " If " & cols(i) & " <> Value Then" & Chr(13) _
                                          & "_isdirty = True " & Chr(13) _
                                          & cols(i) & " = Value" & Chr(13) _
                                          & "End If" & Chr(13) _
                                          & "End Set" & Chr(13) _
                                          & "End Property")
                        End If

                        objWriter.WriteLine()

                        If ListofForeignKey.Contains(cols(i)) Then
                            Dim attributUsed As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                            Dim valueColumn As New Cls_Column(_table.ID, cols(i).ToString.Substring(1, cols(i).Length - 1))
                            Dim foreingn As New Cls_ForeignKey(_table.ID, valueColumn.ID)

                            Dim ClassName As String = "Cls_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                            ClassName = foreingn.RefTable.Name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

                            Dim simplename As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                            objWriter.WriteLine("Public Property " & attributUsed & " As " & ClassName & Chr(13))
                            objWriter.WriteLine("Get")
                            objWriter.WriteLine("If Not (_" & attributUsed & " Is Nothing) Then" & Chr(13) _
                                                & "If (_" & attributUsed & ".ID = 0) Or (_" & attributUsed & ".ID <>  " & cols(i) & ") Then" & Chr(13) _
                                                & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                                & "End If" & Chr(13) _
                                                & "Else" & Chr(13) _
                                                & "_" & attributUsed & "= New " & ClassName & "(" & cols(i) & ")" & Chr(13) _
                                                & "End If" & Chr(13) & Chr(13) _
                                                & "Return _" & attributUsed & Chr(13) _
                                                & "End Get" & Chr(13) _
                                                & "Set(ByVal value As " & ClassName & ")" & Chr(13) _
                                                & "If Value Is Nothing Then" & Chr(13) _
                                                & "_isdirty = True" & Chr(13) _
                                                & cols(i) & " = 0" & Chr(13) _
                                                & "Else" & Chr(13) _
                                                & "If _" & attributUsed & ".ID <> Value.ID Then" & Chr(13) _
                                                & "_isdirty = True" & Chr(13) _
                                                & cols(i) & " = Value.ID" & Chr(13) _
                                                & "End If" & Chr(13) _
                                                & "End If" & Chr(13) _
                                                & "End Set" & Chr(13) _
                                                & "End Property" & Chr(13) _
                                                )
                            objWriter.WriteLine()
                            Dim foreignkey As New Cls_ForeignKey(_table.ID, cols(i).Substring(1, cols(i).Length - 1))
                            Dim textForcombo As String = ""

                            Dim compteur As Integer = 0

                            For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                                If column_fore.Type.VbName = "String" Then
                                    objWriter.WriteLine("Public ReadOnly Property " & simplename & "_" & column_fore.Name & "  As String ")
                                    objWriter.WriteLine("Get")
                                    objWriter.WriteLine("Return " & simplename & "." & column_fore.Name & "")
                                    objWriter.WriteLine("End Get")
                                    objWriter.WriteLine("End Property")
                                    objWriter.WriteLine()
                                End If
                                If compteur = foreignkey.RefTable.ListofColumn.Count - 5 Then
                                    Exit For
                                End If
                                compteur = compteur + 1
                            Next
                        End If
                    End If
                    If initialtypes(i).ToString() = "image" Then
                        objWriter.WriteLine("Public Property " & cols(i).Substring(1, cols(i).Length - 1) & "String() As String")
                        objWriter.WriteLine("Get")
                        objWriter.WriteLine("If " & cols(i) & " IsNot Nothing Then")
                        objWriter.WriteLine("Return Encode(" & cols(i) & " )")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("Return """"")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End Get")
                        objWriter.WriteLine("Set(ByVal Value As String)")
                        objWriter.WriteLine(cols(i) & " = Decode(Value)")
                        objWriter.WriteLine("_isdirty = True")
                        objWriter.WriteLine("End Set")
                        objWriter.WriteLine("End Property")
                    End If

                Next


                With objWriter
                    .WriteLine(" Public ReadOnly Property IsDataDirty() As Boolean")
                    .WriteLine("Get")
                    .WriteLine("Return _isdirty")
                    .WriteLine("End Get")
                    .WriteLine("End Property")
                    .WriteLine()
                End With

                With objWriter
                    .WriteLine("Public ReadOnly Property LogData() As String")
                    .WriteLine("Get")
                    .WriteLine("Return _LogData")
                    .WriteLine("End Get")
                    .WriteLine("End Property")
                End With


                objWriter.WriteLine("#End Region")

                objWriter.WriteLine()
                objWriter.WriteLine("#Region "" Db Access """)
                objWriter.WriteLine("Public Function Insert(ByVal usr As String) As Integer Implements IGeneral.Insert")
                objWriter.WriteLine("_LogData = """"")


                objWriter.WriteLine("_id = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Insert" & nomClasse.Substring(4, nomClasse.Length - 4) & """" & insertstring & ", usr))")


                objWriter.WriteLine("Return _id")
                objWriter.WriteLine("End Function")

                objWriter.WriteLine()
                objWriter.WriteLine("Public Function Update(ByVal usr As String) As Integer Implements IGeneral.Update")
                objWriter.WriteLine("_LogData = """"")
                objWriter.WriteLine("_LogData = GetObjectString()")
                objWriter.WriteLine("Return SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Update" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id" & updatestring & ", usr)")
                objWriter.WriteLine("End Function" & Chr(13))

                objWriter.WriteLine("Private Sub SetProperties(ByVal dr As DataRow)" & Chr(13))
                objWriter.WriteLine("_id = TypeSafeConversion.NullSafeLong(dr(""" & cols(0).Substring(1, cols(0).Length - 1) & """))")

                For i As Int32 = 1 To cols.Count - 2

                    If cols(i) <> "_isdirty" Then
                        If types(i) = "DateTime" Then
                            objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafeDate(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                        ElseIf initialtypes(i) = "image" Then
                            objWriter.WriteLine("If" & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)" & "IsNot DBNull.Value Then")
                            objWriter.WriteLine(cols(i) & " = " & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)")
                            objWriter.WriteLine("Else")
                            objWriter.WriteLine("" & cols(i) & " =  " & "Nothing")
                            objWriter.WriteLine("End If")
                        Else
                            objWriter.WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafe" & types(i) & "(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                        End If
                    End If

                    If ListofForeignKey.Contains(cols(i)) Then
                        objWriter.WriteLine("_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " = Nothing")

                    End If

                Next

                objWriter.WriteLine()
                objWriter.WriteLine("End Sub" & Chr(13))

                objWriter.WriteLine("Private Sub BlankProperties()" & Chr(13))
                objWriter.WriteLine("_id = 0")
                For i As Int32 = 1 To cols.Count - 2

                    If types(i) <> "Boolean" Then
                        If types(i) = "DateTime" Or types(i) = "Date" Then
                            objWriter.WriteLine(cols(i) & " = " & "Now")
                        Else
                            If Lcasevalue.Contains(types(i)) Then
                                objWriter.WriteLine(cols(i) & " = " & """""")
                            ElseIf initialtypes(i) = "image" Then
                                objWriter.WriteLine(cols(i) & " = Nothing")
                            Else
                                objWriter.WriteLine(cols(i) & " = " & "0")
                            End If

                        End If
                    Else
                        objWriter.WriteLine(cols(i) & " = " & "False")
                    End If

                    If ListofForeignKey.Contains(cols(i)) Then
                        objWriter.WriteLine("_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " = Nothing")
                    End If


                Next

                objWriter.WriteLine()
                objWriter.WriteLine("End Sub" & Chr(13))

                objWriter.WriteLine("Public Function Read(ByVal _idpass As Long) As Boolean Implements IGeneral.Read")
                objWriter.WriteLine("Try " & Chr(13))

                objWriter.WriteLine("If _idpass <> 0 Then " & Chr(13) _
                                & "Dim ds As DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(),""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_ByID"", _idpass)" & Chr(13) & Chr(13) _
                                & "If ds.Tables(0).Rows.Count < 1 Then" & Chr(13) _
                                & "BlankProperties()" & Chr(13) _
                                & "Return False" & Chr(13) _
                                & "End If" & Chr(13) & Chr(13) _
                                & "SetProperties(ds.tables(0).rows(0))" & Chr(13) _
                                & "Else" & Chr(13) _
                                & "BlankProperties()" & Chr(13) _
                                & "End If" & Chr(13) _
                                & "Return True" & Chr(13) _
                                )

                objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                objWriter.WriteLine("Throw ex" & Chr(13))
                objWriter.WriteLine("End Try" & Chr(13))
                objWriter.WriteLine("End Function" & Chr(13))

                For Each _index As Cls_UniqueIndex In _table.ListofIndex
                    If _index.ListofColumn.Count = 1 Then
                        objWriter.WriteLine("Public Function Read_" & _index.ListofColumn.Item(0).Name & "(ByVal " & _index.ListofColumn.Item(0).Name & " As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean")
                        objWriter.WriteLine("Try " & Chr(13))
                        objWriter.WriteLine("If " & _index.ListofColumn.Item(0).Name & " <> """" Then ")
                        objWriter.WriteLine("Dim ds as Dataset = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & """, " & _index.ListofColumn.Item(0).Name & ")" & Chr(13))
                        objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                        objWriter.WriteLine("BlankProperties()")
                        objWriter.WriteLine("Return False")
                        objWriter.WriteLine("End If" & Chr(13))
                        objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("BlankProperties()")
                        objWriter.WriteLine("End If" & Chr(13))
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                        objWriter.WriteLine("Throw ex" & Chr(13))
                        objWriter.WriteLine("End Try" & Chr(13))
                        objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                    Else
                        Dim _strOfIndexToUse As String = String.Empty
                        Dim _strOfValueToUse As String = String.Empty
                        Dim _strParameterToUse As String = String.Empty
                        Dim ind As Integer = 0
                        For Each _column As Cls_Column In _index.ListofColumn
                            If _strOfIndexToUse.Length = 0 Then
                                _strOfIndexToUse = _column.Name
                                _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse = "_value" & ind
                            Else
                                _strOfIndexToUse += "_" & _column.Name
                                _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse += ", _value" & ind
                            End If
                            ind = ind + 1
                        Next
                        objWriter.WriteLine("Public Function Read_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean")
                        objWriter.WriteLine("Try " & Chr(13))
                        '  objWriter.WriteLine("If " & ListofIndex(i) & " <> """" Then ")
                        objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")
                        objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                        objWriter.WriteLine("BlankProperties()")
                        objWriter.WriteLine("Return False")
                        objWriter.WriteLine("End If" & Chr(13))

                        objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                        'objWriter.WriteLine("Else")
                        'objWriter.WriteLine("BlankProperties()")
                        '  objWriter.WriteLine("End If" & Chr(13))

                        objWriter.WriteLine("Return True")

                        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                        objWriter.WriteLine("Throw ex" & Chr(13))
                        objWriter.WriteLine("End Try" & Chr(13))
                        objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                    End If
                Next

                objWriter.WriteLine("Public Sub Delete() Implements IGeneral.Delete" & Chr(13) _
                                    & "Try" & Chr(13) _
                                    & "SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), ""SP_Delete" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id)" & Chr(13) & Chr(13) _
                                    & "Catch ex As SqlClient.SqlException" & Chr(13) _
                                    & "Throw New System.Exception(ex.ErrorCode)" & Chr(13) _
                                    & "End Try" & Chr(13) _
                                    & "End Sub" & Chr(13)
                                    )

                objWriter.WriteLine("Public Function Refresh() As Boolean Implements IGeneral.Refresh" & Chr(13) _
                                     & "If _id = 0 Then" & Chr(13) _
                                     & "Return False" & Chr(13) _
                                     & "Else" & Chr(13) _
                                     & "Read(_id)" & Chr(13) _
                                     & "Return True" & Chr(13) _
                                     & "End If" & Chr(13) _
                                     & "End Function" & Chr(13) _
                                     )

                objWriter.WriteLine("Public Function Save(ByVal usr As String) As Integer Implements IGeneral.Save" & Chr(13) _
                                     & "If _isdirty Then" & Chr(13) _
                                     & "Validation()" & Chr(13) & Chr(13) _
                                     & "If _id = 0 Then" & Chr(13) _
                                     & "Return Insert(usr)" & Chr(13) _
                                     & "Else" & Chr(13) _
                                     & "If _id > 0 Then" & Chr(13) _
                                     & "Return Update(usr)" & Chr(13) _
                                     & "Else" & Chr(13) _
                                     & "_id = 0" & Chr(13) _
                                     & "Return False" & Chr(13) _
                                     & "End If" & Chr(13) _
                                     & "End If" & Chr(13) _
                                     & "End If" & Chr(13) & Chr(13) _
                                     & "_isdirty = False" & Chr(13) _
                                     & "End Function"
                                     )


                objWriter.WriteLine("#End Region")

                objWriter.WriteLine()
                objWriter.WriteLine("#Region "" Search """)

                objWriter.WriteLine("Public Function Search() As System.Collections.ICollection Implements IGeneral.Search" & Chr(13) _
                                    & "Return SearchAll()" & Chr(13) _
                                    & "End Function" & Chr(13)
                                    )


                objWriter.WriteLine("Public Shared Function SearchAll() As List(Of " & nomClasse & ")" & Chr(13) _
                                    & "Try " & Chr(13) _
                                    & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                    & "Dim r As Data.DataRow" & Chr(13) _
                                    & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & """)" & Chr(13) _
                                    & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                    & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                    & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                    & "objs.Add(obj)" & Chr(13) _
                                    & "Next r" & Chr(13) _
                                    & "Return objs"
                                    )
                objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                objWriter.WriteLine("Throw ex")
                objWriter.WriteLine("End Try")
                objWriter.WriteLine("End Function" & Chr(13))


                objWriter.WriteLine()

                objWriter.WriteLine("Public Shared Function SearchAllIn_" & group.ParentTable.Name & "(Byval id" & group.ParentTable.Name.Replace("tbl_", "Cls_").ToLower & " As Long) As List(Of " & nomClasse & ")" & Chr(13) _
                                 & "Try " & Chr(13) _
                                 & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                 & "Dim r As Data.DataRow" & Chr(13) _
                               & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll" & group.ChildTable.Name.Replace("tbl_", "Cls_") & "In" & group.ParentTable.Name.Replace("tbl_", "Cls_") & ", id" & group.ParentTable.Name.Replace("tbl_", "Cls_").ToLower & " "")" & Chr(13) _
                                 & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                 & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                 & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                 & "objs.Add(obj)" & Chr(13) _
                                 & "Next r" & Chr(13) _
                                 & "Return objs"
                                 )
                objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                objWriter.WriteLine("Throw ex")
                objWriter.WriteLine("End Try")
                objWriter.WriteLine("End Function" & Chr(13))

                objWriter.WriteLine("Public Shared Function SearchAllNotIn_" & group.ParentTable.Name & "(Byval id" & group.ParentTable.Name.Replace("tbl_", "Cls_").ToLower & " As Long) As List(Of " & nomClasse & ")" & Chr(13) _
                               & "Try " & Chr(13) _
                               & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                               & "Dim r As Data.DataRow" & Chr(13) _
                               & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll" & group.ChildTable.Name.Replace("tbl_", "Cls_") & "NotIn" & group.ParentTable.Name.Replace("tbl_", "Cls_") & ", id" & group.ParentTable.Name.Replace("tbl_", "Cls_").ToLower & " "")" & Chr(13) _
                               & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                               & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                               & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                               & "objs.Add(obj)" & Chr(13) _
                               & "Next r" & Chr(13) _
                               & "Return objs"
                               )
                objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                objWriter.WriteLine("Throw ex")
                objWriter.WriteLine("End Try")
                objWriter.WriteLine("End Function" & Chr(13))

                objWriter.WriteLine()

                For i As Int32 = 1 To cols.Count - 2
                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim searchtext = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

                        objWriter.WriteLine("Public Shared Function SearchAllBy" & searchtext & "(Byval " & cols(i).ToString.ToLower & " As " & types(i) & ") As List(Of " & nomClasse & ")" & Chr(13) _
                                            & "Try " & Chr(13) _
                                            & "Dim objs As New List(Of " & nomClasse & ")" & Chr(13) _
                                            & "Dim r As Data.DataRow" & Chr(13) _
                                            & "Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_ListAll_" & nomClasse.Substring(4, nomClasse.Length - 4) & cols(i) & """," & cols(i).ToString.ToLower & ")" & Chr(13) _
                                            & "For Each r In ds.Tables(0).Rows" & Chr(13) _
                                            & "Dim obj As New " & nomClasse & Chr(13) & Chr(13) _
                                            & "obj.SetProperties(r)" & Chr(13) & Chr(13) _
                                            & "objs.Add(obj)" & Chr(13) _
                                            & "Next r" & Chr(13) _
                                            & "Return objs"
                                            )
                        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                        objWriter.WriteLine("Throw ex")
                        objWriter.WriteLine("End Try")
                        objWriter.WriteLine("End Function" & Chr(13))


                    End If
                Next
                objWriter.WriteLine()
                objWriter.WriteLine("#End Region")
                objWriter.WriteLine()
                objWriter.WriteLine("#Region "" Other Methods """)


                For Each _index As Cls_UniqueIndex In _table.ListofIndex
                    If _index.ListofColumn.Count = 1 Then
                        objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(ByVal _value As " & _index.ListofColumn.Item(0).Type.VbName & ") As Boolean ")
                        objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _index.ListofColumn.Item(0).Name & """, _value)")
                        objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                        objWriter.WriteLine(" Return False")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("If _id = 0 Then")
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("Return False")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                    Else
                        Dim _strOfIndexToUse As String = String.Empty
                        Dim _strOfValueToUse As String = String.Empty
                        Dim _strParameterToUse As String = String.Empty
                        Dim ind As Integer = 0
                        For Each _column In _index.ListofColumn
                            If _strOfIndexToUse.Length = 0 Then
                                _strOfIndexToUse = _column.Name
                                _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse = "_value" & ind
                            Else
                                _strOfIndexToUse += "_" & _column.Name
                                _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse += ", _value" & ind
                            End If
                            ind = ind + 1
                        Next
                        objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean ")
                        objWriter.WriteLine("Try" & Chr(13))
                        objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")

                        objWriter.WriteLine(" If ds.Tables(0).Rows.Count < 1 Then")
                        objWriter.WriteLine(" Return False")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("If _id = 0 Then")
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine(" If ds.Tables(0).Rows(0).Item(""" & Id_table & """) <> _id Then")
                        objWriter.WriteLine("Return True")
                        objWriter.WriteLine("Else")
                        objWriter.WriteLine("Return False")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End If")
                        objWriter.WriteLine("End If" & Chr(13))
                        objWriter.WriteLine("Catch ex As Exception" & Chr(13))
                        objWriter.WriteLine("Throw ex" & Chr(13))
                        objWriter.WriteLine("End Try" & Chr(13))
                        objWriter.WriteLine("End Function" & Chr(13) & Chr(13))
                    End If
                Next


                objWriter.WriteLine("Private Sub Validation() " & Chr(13))
                Dim stringlistforvalidation As New List(Of String) From {"String"}
                Dim decimalintegerforvalidation As New List(Of String) From {"Integer", "Long", "Decimal"}



                For i As Int32 = 1 To cols.Count - 2
                    If stringlistforvalidation.Contains(types(i)) Then
                        objWriter.WriteLine("If " & cols(i) & " = """" Then " & Chr(13) _
                                            & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                            & "End If"
                                            )
                        objWriter.WriteLine()
                        objWriter.WriteLine("If Len(" & cols(i) & ") > " & length(i) & " Then" & Chr(13) _
                                            & "Throw (New System.Exception("" " & "Trop de caractères insérés pour" & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & "  (la longueur doit être inférieure a " & length(i) & " caractères.  )""))" & Chr(13) _
                                            & "End If"
                                            )
                        objWriter.WriteLine()
                    End If

                    If decimalintegerforvalidation.Contains(types(i)) Then
                        If ListofForeignKey.Contains(cols(i)) Then
                            objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                          & "Throw (New System.Exception("" " & cols(i).Substring(4, cols(i).Length - 4).ToString.ToLower & " obligatoire""))" & Chr(13) _
                                          & "End If"
                                          )
                            objWriter.WriteLine()
                        Else
                            objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                           & "Throw (New System.Exception("" " & cols(i).Substring(1, cols(i).Length - 1).ToString.ToLower & " obligatoire.""))" & Chr(13) _
                                           & "End If"
                                           )
                            objWriter.WriteLine()
                        End If
                    End If


                Next

                objWriter.WriteLine()

                For Each _index As Cls_UniqueIndex In _table.ListofIndex
                    If _index.ListofColumn.Count = 1 Then
                        objWriter.WriteLine("If  FoundAlreadyExist" & "_" & _index.ListofColumn.Item(0).Name & "(" & "_" & _index.ListofColumn.Item(0).Name & ") Then" & Chr(13) _
                                          & "Throw (New System.Exception(""Ce " & _index.ListofColumn.Item(0).Name & " est déjà enregistré.""))" & Chr(13) _
                                          & "End If"
                                         )
                        objWriter.WriteLine()
                    Else
                        Dim _strOfIndexToUse As String = String.Empty
                        Dim _strOfValueToUse As String = String.Empty
                        Dim _strParameterToUse As String = String.Empty
                        Dim _parametervalueToUse As String = String.Empty
                        Dim ind As Integer = 0
                        For Each _column In _index.ListofColumn
                            If _strOfIndexToUse.Length = 0 Then
                                _strOfIndexToUse = _column.Name
                                _parametervalueToUse = _column.Name
                                _strOfValueToUse = "ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse = "_value" & ind
                            Else
                                _strOfIndexToUse += "_" & _column.Name
                                _parametervalueToUse += ", " & _column.Name
                                _strOfValueToUse += ", ByVal _value" & ind & " As " & _column.Type.VbName
                                _strParameterToUse += ", value" & ind
                            End If
                        Next

                        objWriter.WriteLine("If FoundAlreadyExist_" & _strOfIndexToUse & "(" & _parametervalueToUse & ") Then" & Chr(13) _
                                           & "Throw (New System.Exception(""Cette combinaison " & _strOfIndexToUse & " est déjà enregistrée.""))" & Chr(13) _
                                           & "End If"
                                           )
                        objWriter.WriteLine()
                    End If
                Next

                objWriter.WriteLine("End Sub" & Chr(13))

                objWriter.WriteLine("Public Function Encode(ByVal str As Byte()) As String")
                objWriter.WriteLine("Return Convert.ToBase64String(str)")
                objWriter.WriteLine("End Function")
                objWriter.WriteLine()

                objWriter.WriteLine("Public Function Decode(ByVal str As String) As Byte()")
                objWriter.WriteLine("Dim decbuff As Byte() = Convert.FromBase64String(str)")
                objWriter.WriteLine("Return decbuff")
                objWriter.WriteLine("End Function")
                objWriter.WriteLine()

                objWriter.WriteLine(" Public Function GetObjectString() As String Implements IGeneral.GetObjectString" & Chr(13) _
                                 & "Dim _old As New " & nomClasse & "(Me.ID)" & Chr(13) & Chr(13) _
                                 & "Return LogStringBuilder.BuildLogStringChangesOnly(_old,Me)" & Chr(13) _
                                 & "End Function" & Chr(13) _
                                 )
                objWriter.WriteLine()


                objWriter.WriteLine(" Public Function Add" & nomSimpleChild & "(ByVal id" & nomSimpleChild.ToLower & " As Long, ByVal usr As String) As Boolean" & Chr(13) _
                         & "Try" & Chr(13) _
                         & "   SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), ""SP_Update" & nomSimpleParent & "Add" & nomSimpleChild & """, ID , id" & nomSimpleChild.ToLower & ", usr)" & Chr(13) _
                          & "  Return True " & Chr(13) _
                         & "Catch " & Chr(13) _
                         & "   Return False " & Chr(13) _
                         & "End Try " & Chr(13) _
                & "End Sub" & Chr(13) _
                )
                objWriter.WriteLine()

                objWriter.WriteLine(" Public Function Remove" & nomSimpleChild & "(ByVal id" & nomSimpleChild.ToLower & " As Long, ByVal usr As String) As Boolean" & Chr(13) _
                    & "Try" & Chr(13) _
                    & "SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), ""SP_Update" & nomSimpleParent & "Remove" & nomSimpleChild & """, ID , id" & nomSimpleChild.ToLower & ", usr)" & Chr(13) _
                    & "Return True" & Chr(13) _
                   & "Catch" & Chr(13) _
                         & "Return False" & Chr(13) _
                      & "End Try" & Chr(13) _
                & "End Sub" & Chr(13) _
                )
                objWriter.WriteLine()

                objWriter.WriteLine("#End Region")
                objWriter.WriteLine()
                objWriter.WriteLine(_end)
                objWriter.WriteLine()
                objWriter.Close()
            Next
        End Sub

    End Class
End Namespace
