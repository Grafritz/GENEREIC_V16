Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.ComponentModel
Imports System.Text.RegularExpressions


Namespace SqlServer.Fast

    Public Class VbClassGenerator

        Public Shared Sub CreateFile(ByVal name As String, ByRef _Path_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByVal databasename As String)
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")

            'Dim RepertoiresName As String = Cls_Enumeration.PATH_GENERIC_FOLDER_DEFAULT

            Dim _PathGenerate_Script As String = Cls_Enumeration.GetPath_VbNet_Class(_Path_ScriptFile.Text.Trim, databasename)
            Dim path As String = _PathGenerate_Script & nomClasse & ".vb"

            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)

            Dim cult As Globalization.CultureInfo = New Globalization.CultureInfo("en-EN")
            Threading.Thread.CurrentThread.CurrentCulture = cult

            Dim header As String = "REM Generate By [" & Cls_Enumeration.APP_NAME & "] Application *******" & Chr(13) _
                                   & "REM  Class " + nomClasse & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy hh:mm tt")
            'header = ""
            Dim content As String = "Public Class " & nomClasse & Chr(13) _
                                     & "Implements IGeneral"

            _end = "End Class" & Chr(13)
            ' Delete the file if it exists.
            If File.Exists(path) Then
                File.Delete(path)
            End If

            REM on verifie si le repertoir existe bien       
            If Not Directory.Exists(_PathGenerate_Script) Then
                Directory.CreateDirectory(_PathGenerate_Script)
            End If
            ' Create the file.
            Dim fs As FileStream = File.Create(path, 1024)
            fs.Close()



            Dim objWriter As New StreamWriter(path, True)
            objWriter.WriteLine(header)
            If ListBox_NameSpace.Items.Count > 0 Then
                For i As Integer = 0 To ListBox_NameSpace.Items.Count - 1
                    objWriter.WriteLine(ListBox_NameSpace.Items(i))
                Next
            End If
            objWriter.WriteLine()
            objWriter.WriteLine(content)
            objWriter.WriteLine()
            Dim ds As DataSet = Nothing

            ds = SqlServerHelper.LoadTableStructure(name)


            Dim cols As New List(Of String)
            Dim types As New List(Of String)
            Dim initialtypes As New List(Of String)
            Dim length As New List(Of String)
            Dim count As Integer = 0

            Dim cap As Integer

            cap = ds.Tables(1).Rows.Count

            Dim ListTAb As Integer = ds.Tables.Count

            For Each dt As DataRow In ds.Tables(2).Rows
                Id_table = dt(0).ToString()
            Next

            For Each dt As DataRow In ds.Tables(5).Rows
                If dt(2).ToString <> Id_table Then
                    ListofIndex.Insert(countindex, dt(2).ToString)
                    countindex = countindex + 1
                End If
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}

            'If ListTAb <= 5 Then
            For Each dt As DataRow In ds.Tables(6).Rows
                If dt(0).ToString = "FOREIGN KEY" Then
                    ListofForeignKey.Add("_" & dt(6).ToString)
                    countForeignKey = countForeignKey + 1
                End If
            Next
            'End If


            For Each dt As DataRow In ds.Tables(1).Rows
                For Each index In ListofIndex
                    If dt(0).ToString = index Then
                        ListofIndexType.Add(dt(1))
                        index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(dt(1)))
                    End If
                    Dim _strOfIndexToUse As String = String.Empty
                    If index.Contains(",") Then
                        If Not listoffound_virguleIndex.Contains(index) Then
                            listoffound_virguleIndex.Add(index)
                            Dim strArr As String() = index.ToString.Split(",")

                            For ind As Integer = 0 To strArr.Length - 1
                                For Each dt_index As DataRow In ds.Tables(1).Rows
                                    If strArr(ind).Trim = dt_index(0).ToString.Trim Then
                                        If _strOfIndexToUse.Length = 0 Then
                                            _strOfIndexToUse = dt_index(1).ToString
                                            Exit For
                                        Else
                                            _strOfIndexToUse = _strOfIndexToUse & "," & dt_index(1).ToString
                                            Exit For
                                        End If

                                    End If
                                Next
                            Next
                            ListofIndexType.Add(_strOfIndexToUse)
                            index_li_type.Add(ListofIndex.IndexOf(index), ListofIndexType.IndexOf(_strOfIndexToUse))
                        End If

                    End If
                Next
            Next


            For Each dt As DataRow In ds.Tables(1).Rows
                If count < cap Then '- 4 Then
                    cols.Add("_" & dt(0))
                    types.Add(Configuration.ConfigurationSettings.AppSettings(dt(1)))
                    initialtypes.Add(dt(1))
                    length.Add(dt(3))
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
                        insertstring &= ", " & cols(i)
                        updatestring &= ", " & cols(i)
                    End If

                    objWriter.WriteLine("Private " & cols(i) & " As " & types(i))
                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim ForeinKeyPrefix As String = cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        objWriter.WriteLine("Private _" & ForeinKeyPrefix & " As " & "Cls_" & ForeinKeyPrefix)
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

            For i As Int32 = 0 To ListofIndex.Count - 1
                If ListofIndex(i).Contains(",") Then
                Else
                    objWriter.WriteLine("Public Sub New(ByVal " & ListofIndex(i) & " As " & System.Configuration.ConfigurationSettings.AppSettings(ListofIndexType(index_li_type(i))) & ")")
                    objWriter.WriteLine("Read_" & ListofIndex(i) & "(" & ListofIndex(i) & ")")
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

            For i As Int32 = 1 To cols.Count - 1 '- 3 ''On ne cree pas de property pour la derniere column
                Dim propName As String = ""
                Dim s As String() = cols(i).Split("_")
                For j As Integer = 1 To s.Length - 1
                    propName &= StrConv(s(j), VbStrConv.ProperCase)
                Next
                'propName = StrConv(cols(i).Split("_")(1), VbStrConv.ProperCase) & StrConv(cols(i).Split("_")(2), VbStrConv.ProperCase)
                Dim log As String = "<AttributLogData(True, " & i + 1 & ")> _"
                Dim attrib As String = "Public Property  " & cols(i).Substring(1, cols(i).Length - 1) & " As " & types(i)

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
                        'Dim ClassName As String = "Cls" & cols(i) '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        Dim ClassName As String = "Cls_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                        objWriter.WriteLine("Public Property " & attributUsed & "OBJ As " & ClassName)
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
                                            & "End Property" & Chr(13)
                                            )
                        objWriter.WriteLine()

                        With objWriter
                            .WriteLine("Public ReadOnly Property " & attributUsed & "STR As String")
                            .WriteLine("Get")
                            .WriteLine("Return " & attributUsed & "OBJ." & attributUsed & "")
                            .WriteLine("End Get")
                            .WriteLine("End Property")
                        End With
                        objWriter.WriteLine()
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
                    objWriter.WriteLine()
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
            objWriter.WriteLine("_id = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Insert_" & nomClasse.Substring(4, nomClasse.Length - 4) & """" & insertstring & "))', usr))")
            objWriter.WriteLine("Return _id")
            objWriter.WriteLine("End Function")

            objWriter.WriteLine()
            objWriter.WriteLine("Public Function Update(ByVal usr As String) As Integer Implements IGeneral.Update")
            'objWriter.WriteLine("_LogData = """"")
            objWriter.WriteLine("_LogData = GetObjectString()")
            objWriter.WriteLine("Return SqlHelper.ExecuteScalar(SqlHelperParameterCache.BuildConfigDB(), ""SP_Update_" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id" & updatestring & ")', usr)")
            objWriter.WriteLine("End Function" & Chr(13))

            With objWriter
                .WriteLine("Private Sub SetProperties(ByVal dr As DataRow)")
                .WriteLine("_id = TypeSafeConversion.NullSafeLong(dr(""" & cols(0).Substring(1, cols(0).Length - 1) & """))")

                For i As Int32 = 1 To cols.Count - 2

                    If cols(i) <> "_isdirty" Then
                        If types(i) = "DateTime" Then
                            .WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafeDate(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                        ElseIf initialtypes(i) = "image" OrElse initialtypes(i) = "varbinary" Then
                            '.WriteLine(cols(i) & " = " & "dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)")
                            .WriteLine("If dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """) IsNot DBNull.Value Then")
                            .WriteLine("    " & cols(i) & " = dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """)")
                            .WriteLine("Else")
                            .WriteLine("    " & cols(i) & " = Nothing")
                            .WriteLine("End If")
                        Else
                            .WriteLine(cols(i) & " = " & "TypeSafeConversion.NullSafe" & types(i) & "(dr(""" & cols(i).Substring(1, cols(i).Length - 1) & """))")
                        End If
                    End If

                    If ListofForeignKey.Contains(cols(i)) Then
                        'objWriter.WriteLine("_" & cols(i).Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1)) & " = Nothing")
                    End If

                Next
                .WriteLine("End Sub" & Chr(13))
            End With


            objWriter.WriteLine("Private Sub BlankProperties()")
            objWriter.WriteLine("_id = 0")
            For i As Int32 = 1 To cols.Count - 2

                If types(i) <> "Boolean" Then
                    If types(i) = "DateTime" Or types(i) = "Date" Then
                        objWriter.WriteLine(cols(i) & " = " & "Nothing")
                    Else
                        If Lcasevalue.Contains(types(i)) Then
                            objWriter.WriteLine(cols(i) & " = " & """""")
                        ElseIf initialtypes(i) = "image" OrElse initialtypes(i) = "varbinary" Then
                            objWriter.WriteLine(cols(i) & " = Nothing")
                        Else
                            objWriter.WriteLine(cols(i) & " = " & "0")
                        End If

                    End If
                Else
                    objWriter.WriteLine(cols(i) & " = " & "False")
                End If

                If ListofForeignKey.Contains(cols(i)) Then
                    Dim ForeinKeyPrefix As String = cols(i) '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))
                    objWriter.WriteLine("_" & ForeinKeyPrefix & " = Nothing")
                End If


            Next

            objWriter.WriteLine("End Sub" & Chr(13))

            objWriter.WriteLine("Public Function Read(ByVal _idpass As Long) As Boolean Implements IGeneral.Read")
            objWriter.WriteLine("Try ")
            objWriter.WriteLine("If _idpass <> 0 Then " & Chr(13) _
                            & "Dim ds As DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(),""SP_Select_" & nomClasse.Substring(4, nomClasse.Length - 4) & "_ByID"", _idpass)" & Chr(13) & Chr(13) _
                            & "If ds.Tables(0).Rows.Count < 1 Then" & Chr(13) _
                            & "BlankProperties()" & Chr(13) _
                            & "Return False" & Chr(13) _
                            & "End If" & Chr(13) & Chr(13) _
                            & "SetProperties(ds.tables(0).rows(0))" & Chr(13) _
                            & "Else" & Chr(13) _
                            & "BlankProperties()" & Chr(13) _
                            & "End If" & Chr(13) _
                            & "Return True" & Chr(13)
                            )

            objWriter.WriteLine("Catch ex As Exception")
            objWriter.WriteLine("Throw ex")
            objWriter.WriteLine("End Try")
            objWriter.WriteLine("End Function")
            objWriter.WriteLine()

            For i As Int32 = 0 To ListofIndex.Count - 1
                If ListofIndex(i).Contains(",") Then

                    Dim strArr As String() = ListofIndex(i).ToString.Split(",")
                    Dim typeArr As String() = ListofIndexType(index_li_type(i)).Split(",")
                    Dim _strOfIndexToUse As String = String.Empty
                    Dim _strOfValueToUse As String = String.Empty
                    Dim _strParameterToUse As String = String.Empty

                    For ind As Integer = 0 To strArr.Length - 1
                        If _strOfIndexToUse.Length = 0 Then
                            _strOfIndexToUse = strArr(ind).Trim
                            _strOfValueToUse = "ByVal _value" & ind & " As " & Configuration.ConfigurationSettings.AppSettings(typeArr(ind))
                            _strParameterToUse = "_value" & ind
                        Else
                            _strOfIndexToUse += "_" & strArr(ind).Trim
                            _strOfValueToUse += ", ByVal _value" & ind & " As " & Configuration.ConfigurationSettings.AppSettings(typeArr(ind))
                            _strParameterToUse += ", _value" & ind
                        End If
                    Next

                    objWriter.WriteLine("Public Function Read_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean")
                    objWriter.WriteLine("Try " & Chr(13))
                    '  objWriter.WriteLine("If " & ListofIndex(i) & " <> """" Then ")
                    objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select_" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")
                    objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine("BlankProperties()")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If" & Chr(13))

                    objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("BlankProperties()")
                    '  objWriter.WriteLine("End If" & Chr(13))

                    objWriter.WriteLine("Return True")

                    objWriter.WriteLine("Catch ex As Exception")
                    objWriter.WriteLine("Throw ex")
                    objWriter.WriteLine("End Try")
                    objWriter.WriteLine("End Function")
                    objWriter.WriteLine()


                Else
                    objWriter.WriteLine("Public Function Read_" & ListofIndex(i) & "(ByVal " & ListofIndex(i) & " As " & Configuration.ConfigurationSettings.AppSettings(ListofIndexType(index_li_type(i))) & ") As Boolean")
                    objWriter.WriteLine("Try " & Chr(13))
                    objWriter.WriteLine("If " & ListofIndex(i) & " <> """" Then ")


                    objWriter.WriteLine("Dim ds as Dataset = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select_" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & ListofIndex(i) & """, " & ListofIndex(i) & ")" & Chr(13))


                    objWriter.WriteLine("If ds.tables(0).Rows.Count < 1 Then")
                    objWriter.WriteLine("BlankProperties()")
                    objWriter.WriteLine("Return False")
                    objWriter.WriteLine("End If" & Chr(13))

                    objWriter.WriteLine("SetProperties(ds.Tables(0).Rows(0))")
                    objWriter.WriteLine("Else")
                    objWriter.WriteLine("BlankProperties()")
                    objWriter.WriteLine("End If" & Chr(13))

                    objWriter.WriteLine("Return True")

                    objWriter.WriteLine("Catch ex As Exception")
                    objWriter.WriteLine("Throw ex")
                    objWriter.WriteLine("End Try")
                    objWriter.WriteLine("End Function")
                    objWriter.WriteLine()
                End If

            Next

            objWriter.WriteLine("Public Sub Delete() Implements IGeneral.Delete" & Chr(13) _
                                & "Try" & Chr(13) _
                                & "SqlHelper.ExecuteNonQuery(SqlHelperParameterCache.BuildConfigDB(), ""SP_Delete_" & nomClasse.Substring(4, nomClasse.Length - 4) & """, _id)" & Chr(13) & Chr(13) _
                                & "Catch ex As Exception" & Chr(13) _
                                & "Throw ex" & Chr(13) _
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
                                 & "End Function" & Chr(13)
                                 )
            With objWriter
                .WriteLine("Public Function Save(ByVal usr As String) As Integer Implements IGeneral.Save")
                .WriteLine("    Dim val As Integer = 0")
                .WriteLine("    If _isdirty Then")
                .WriteLine("        Validation()")
                .WriteLine("        If _id = 0 Then")
                .WriteLine("            val = Insert(usr)")
                .WriteLine("        Else")
                .WriteLine("            If _id > 0 Then")
                .WriteLine("                val = Update(usr)")
                .WriteLine("            Else")
                .WriteLine("                val = _id = 0")
                .WriteLine("                Return False")
                .WriteLine("            End If")
                .WriteLine("        End If")
                .WriteLine("    End If")
                .WriteLine("    _isdirty = False")
                .WriteLine("     Return val")
                .WriteLine("End Function")
                .WriteLine("")
            End With

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
                                & "Return objs" & Chr(13)
                                )

            objWriter.WriteLine("Catch ex As Exception")
            objWriter.WriteLine("Throw ex")
            objWriter.WriteLine("End Try")
            objWriter.WriteLine("End Function")
            objWriter.WriteLine()


            objWriter.WriteLine()

            For i As Int32 = 1 To cols.Count - 3
                If ListofForeignKey.Contains(cols(i)) Then
                    Dim searchtext = cols(i) '.Substring(SqlServerHelper.ForeinKeyPrefix.Length + 1, cols(i).Length - (SqlServerHelper.ForeinKeyPrefix.Length + 1))

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
                    objWriter.WriteLine("Catch ex As Exception")
                    objWriter.WriteLine("Throw ex")
                    objWriter.WriteLine("End Try")
                    objWriter.WriteLine("End Function")
                    objWriter.WriteLine()

                End If
            Next
            objWriter.WriteLine("#End Region")
            objWriter.WriteLine()

            objWriter.WriteLine("#Region "" Other Methods """)
            ''
            For i As Int32 = 0 To ListofIndex.Count - 1
                If ListofIndex(i).ToString.Contains(",") Then
                    Dim strArr As String() = ListofIndex(i).ToString.Split(",")
                    Dim typeArr As String() = ListofIndexType(index_li_type(i)).Split(",")
                    Dim _strOfIndexToUse As String = String.Empty
                    Dim _strOfValueToUse As String = String.Empty
                    Dim _strParameterToUse As String = String.Empty

                    For ind As Integer = 0 To strArr.Length - 1
                        If _strOfIndexToUse.Length = 0 Then
                            _strOfIndexToUse = strArr(ind).Trim
                            _strOfValueToUse = "ByVal _value" & ind & " As " & Configuration.ConfigurationSettings.AppSettings(typeArr(ind))
                            _strParameterToUse = "_value" & ind
                        Else
                            _strOfIndexToUse += "_" & strArr(ind).Trim
                            _strOfValueToUse += ", ByVal _value" & ind & " As " & Configuration.ConfigurationSettings.AppSettings(typeArr(ind))
                            _strParameterToUse += ", _value" & ind
                        End If
                    Next
                    objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & _strOfIndexToUse & "(" & _strOfValueToUse & ") As Boolean ")
                    objWriter.WriteLine("Try" & Chr(13))
                    objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select_" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & _strOfIndexToUse & """, " & _strParameterToUse & ")")

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

                    objWriter.WriteLine("Catch ex As Exception")
                    objWriter.WriteLine("Throw ex")
                    objWriter.WriteLine("End Try")
                    objWriter.WriteLine("End Function")
                    objWriter.WriteLine()
                Else
                    objWriter.WriteLine("Private Function FoundAlreadyExist" & "_" & ListofIndex(i) & "(ByVal _value As " & Configuration.ConfigurationSettings.AppSettings(ListofIndexType(index_li_type(i))) & ") As Boolean ")
                    objWriter.WriteLine("Dim ds As Data.DataSet = SqlHelper.ExecuteDataset(SqlHelperParameterCache.BuildConfigDB(), ""SP_Select_" & nomClasse.Substring(4, nomClasse.Length - 4) & "_" & ListofIndex(i) & """, _value)")
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

                End If

            Next

            objWriter.WriteLine("Private Sub Validation() " & Chr(13))
            Dim stringlistforvalidation As New List(Of String) From {"String"}
            Dim decimalintegerforvalidation As New List(Of String) From {"Integer", "Long", "Decimal"}



            For i As Int32 = 1 To cols.Count - 2
                Dim columnName As String = cols(i) '.Substring(1, cols(i).Length - 1).ToString
                Dim columnNameToShow As String = Regex.Replace(columnName, "([a-z])?([A-Z])", "$1 $2")
                columnNameToShow = Regex.Replace(columnNameToShow, "_", "")

                If stringlistforvalidation.Contains(types(i)) Then
                    objWriter.WriteLine("If " & cols(i) & " = """" Then " & Chr(13) _
                                        & "Throw (New Rezo509Exception("" " & columnNameToShow & " Obligatoire""))" & Chr(13) _
                                        & "End If"
                                        )
                    objWriter.WriteLine()
                    objWriter.WriteLine("'If Len(" & cols(i) & ") > " & length(i) & " Then" & Chr(13) _
                                        & "'Throw (New Rezo509Exception("" " & "Trop de caractères insérés pour " & columnNameToShow & "  (la longueur doit être inférieure a " & length(i) & " caractères.  )""))" & Chr(13) _
                                        & "'End If"
                                        )
                    objWriter.WriteLine()
                End If

                If decimalintegerforvalidation.Contains(types(i)) Then
                    If ListofForeignKey.Contains(cols(i)) Then
                        Dim columnName2 As String = cols(i) '.Substring(4, cols(i).Length - 4).ToString
                        Dim columnNameToShow2 As String = Regex.Replace(columnName2, "([a-z])?([A-Z])", "$1 $2")
                        columnNameToShow2 = Regex.Replace(columnNameToShow2, "_", "")
                        objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                      & "Throw (New Rezo509Exception("" " & columnNameToShow2 & " Obligatoire""))" & Chr(13) _
                                      & "End If"
                                      )
                        objWriter.WriteLine()
                    Else
                        objWriter.WriteLine("If " & cols(i) & " = 0 Then " & Chr(13) _
                                       & "Throw (New Rezo509Exception("" " & columnNameToShow & " Obligatoire.""))" & Chr(13) _
                                       & "End If"
                                       )
                        objWriter.WriteLine()
                    End If
                End If


            Next

            objWriter.WriteLine()

            For indexcompteur As Int32 = 0 To ListofIndex.Count - 1
                If ListofIndex(indexcompteur).ToString.Contains(",") Then
                    Dim strArr As String() = ListofIndex(indexcompteur).ToString.Split(",")
                    Dim typeArr As String() = ListofIndexType(index_li_type(indexcompteur)).Split(",")
                    Dim _strOfIndexToUse As String = String.Empty
                    Dim _strOfValueToUse As String = String.Empty
                    Dim _strParameterToUse As String = String.Empty
                    Dim _parametervalueToUse As String = String.Empty

                    For ind As Integer = 0 To strArr.Length - 1
                        If _strOfIndexToUse.Length = 0 Then
                            _strOfIndexToUse = strArr(ind).Trim
                            _parametervalueToUse = strArr(ind).Trim
                            _strOfValueToUse = "ByVal _value" & ind & " As " & Configuration.ConfigurationSettings.AppSettings(typeArr(ind))
                            _strParameterToUse = "_value" & ind
                        Else
                            _strOfIndexToUse += "_" & strArr(ind).Trim
                            _parametervalueToUse += ", " & strArr(ind).Trim
                            _strOfValueToUse += ", ByVal _value" & ind & " As " & Configuration.ConfigurationSettings.AppSettings(typeArr(ind))
                            _strParameterToUse += ", value" & ind
                        End If
                    Next

                    objWriter.WriteLine("If FoundAlreadyExit_" & _strOfIndexToUse & "(" & _parametervalueToUse & ") Then" & Chr(13) _
                                        & "Throw (New Rezo509Exception(""Cette combinaison " & ListofIndex(indexcompteur) & " est déjà enregistrée.""))" & Chr(13) _
                                        & "End If"
                                        )
                    objWriter.WriteLine()

                Else
                    objWriter.WriteLine("If  FoundAlreadyExist" & "_" & ListofIndex(indexcompteur) & "(" & ListofIndex(indexcompteur) & ") Then" & Chr(13) _
                                        & "Throw (New Rezo509Exception(""Ce " & ListofIndex(indexcompteur) & " est déjà enregistré.""))" & Chr(13) _
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
                             & "Return LogData(New " & nomClasse & "(Me.ID))" & Chr(13) _
                             & "End Function" & Chr(13)
                             )

            objWriter.WriteLine("Function LogData(obj As " & nomClasse & ") As String")
            objWriter.WriteLine("Return LogStringBuilder.BuildLogStringHTML(obj)")
            objWriter.WriteLine("End Function")

            objWriter.WriteLine()

            objWriter.WriteLine("Function LogData() As String")
            objWriter.WriteLine("Return LogStringBuilder.BuildLogStringHTML(Me)")
            objWriter.WriteLine("End Function")


            objWriter.WriteLine("#End Region")
            objWriter.WriteLine()
            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub
    End Class

End Namespace
