Imports System.Management
Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml


Namespace SqlServer.IMode


    Public Class AndroidClassGenerator


        Public Shared Sub CreateAndroidModel(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_projectName As TextBox)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\model\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\modele\")
            Dim path As String = txt_PathGenerate_Script & nomSimple & ".java"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)
            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")

            Dim content As String = "public class " & nomSimple & " {" & Chr(13)
            _end = "}" & Chr(13)
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

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                ListofIndex.Insert(countindex, _index.ListofColumn.Item(0).Name)
                countindex = countindex + 1
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}


            For Each _foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add(_foreignkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next


            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add(_column.Name)
                    types.Add(_column.Type.JavaName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            objWriter.WriteLine("package ht.solutions.android." & txt_projectName.Text & ".modele;")
            objWriter.WriteLine("import com.google.gson.annotations.Expose;")
            objWriter.WriteLine("import com.google.gson.annotations.SerializedName;")
            objWriter.WriteLine()
            objWriter.WriteLine("import java.util.*;")
            objWriter.WriteLine(content)
            objWriter.WriteLine()

            'cols.Add("LocalId")
            'cols.Add("isSync")
            'types.Add("long")
            'types.Add("boolean")
            'initialtypes.Add("Bigint")
            'initialtypes.Add("bit")
            objWriter.WriteLine("//region Attribut")

            objWriter.WriteLine()
            Try
                For i As Int32 = 0 To cols.Count - 1
                    If i = 0 Then
                        objWriter.WriteLine("@SerializedName(""" & cols(i) & """)")
                        objWriter.WriteLine("@Expose")
                        objWriter.WriteLine("private long id;")
                        objWriter.WriteLine()
                    ElseIf (i = cols.Count - 1) Or (i = cols.Count - 2) Then
                        objWriter.WriteLine("private " & types(i) & " " & cols(i) & ";")
                        objWriter.WriteLine()
                    Else
                        objWriter.WriteLine("@SerializedName(""" & cols(i) & """)")
                        objWriter.WriteLine("@Expose")
                        objWriter.WriteLine("private " & types(i) & " " & cols(i) & ";")
                        If initialtypes(i) = "image" Then
                            objWriter.WriteLine("@SerializedName(""" & cols(i) & "String" & """)")
                            objWriter.WriteLine("@Expose")
                            objWriter.WriteLine("private String " & cols(i) & "String;" & "")
                        End If
                        If ListofForeignKey.Contains(cols(i)) Then
                            objWriter.WriteLine("private " & cols(i).Substring(3, cols(i).Length - 3) & " " & cols(i).Substring(3, cols(i).Length - 3).ToLower & ";")
                        End If
                        objWriter.WriteLine()
                    End If

                Next
                objWriter.WriteLine("private long localId;")
                objWriter.WriteLine("private boolean isSync;")

                objWriter.WriteLine("private String MacTabletteCreated;")
                objWriter.WriteLine("private String MacTabletteModif;")
                objWriter.WriteLine("private String DateCreated;")
                objWriter.WriteLine("private String DateModif;")
            Catch ex As Exception

            End Try
            objWriter.WriteLine()
            objWriter.WriteLine("//endregion")
            objWriter.WriteLine()

            '''''''''''''''''''''''''''''''''''''''''properties''''''''''''''''''''''''''''''''''''

            objWriter.WriteLine("//region Properties")

            objWriter.WriteLine("public long getId() {")
            objWriter.WriteLine("return id;")
            objWriter.WriteLine("}")
            objWriter.WriteLine()
            objWriter.WriteLine("  public void setId(long id) {")
            objWriter.WriteLine("this.id = id;")
            objWriter.WriteLine("}")

            For i As Int32 = 1 To cols.Count - 2 ''On ne cree pas de property pour la derniere column
                Dim propName As String = ""
                Dim s As String() = cols(i).Split("_")
                For j As Integer = 1 To s.Length - 1
                    propName &= StrConv(s(j), VbStrConv.ProperCase)
                Next

                Dim attrib As String = "public " & types(i) & " get" & cols(i) & "() {"
                Dim setter As String = "public void " & "set" & cols(i) & "(" & types(i) & " " & cols(i).ToLower() & ") {"

                If cols(i) <> "_isdirty" Or cols(i) <> "_LogData" Then
                    '   objWriter.WriteLine(log)

                    objWriter.WriteLine()
                    objWriter.WriteLine(attrib)
                    objWriter.WriteLine("return " & cols(i) & ";")
                    objWriter.WriteLine("}")
                    objWriter.WriteLine(setter)
                    objWriter.WriteLine("this." & cols(i) & " = " & cols(i).ToLower & ";")
                    objWriter.WriteLine("}")

                    If initialtypes(i) = "image" Then
                        objWriter.WriteLine()
                        objWriter.WriteLine("public boolean hasImage() {")
                        objWriter.WriteLine("return (this." & cols(i) & "String != null && this." & cols(i) & "String.length() > 0 || this." & cols(i) & "!= null && this." & cols(i) & ".length > 0);")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("public String get" & cols(i) & "String() {")
                        objWriter.WriteLine("return " & cols(i).ToLower & "String;")
                        objWriter.WriteLine("}")

                        objWriter.WriteLine("public void set" & cols(i) & "String(String " & cols(i).ToLower & "String) {")
                        objWriter.WriteLine("this." & cols(i) & "String = " & cols(i).ToLower & "String;")
                        objWriter.WriteLine("}")


                    End If

                    If ListofForeignKey.Contains(cols(i)) Then

                        Dim ClassName As String = cols(i).Substring(3, cols(i).Length - 3)
                        Dim attributUsed As String = ClassName.ToLower()
                        '  objWriter.WriteLine("public cols(i).Substring(3, cols(i).Length - 3) & Chr(13))
                        objWriter.WriteLine("public " & ClassName & " get" & ClassName & "() {")
                        objWriter.WriteLine("if (" & attributUsed & "==null)")
                        objWriter.WriteLine(attributUsed & " = " & ClassName & "Helper.searchById(" & cols(i) & ");")
                        objWriter.WriteLine("return " & attributUsed & ";")
                        objWriter.WriteLine("}")

                        objWriter.WriteLine("public void set" & ClassName & "(" & ClassName & " " & attributUsed & ") {")
                        objWriter.WriteLine("this." & attributUsed & " = " & attributUsed & ";")
                        objWriter.WriteLine("}")

                    End If
                End If

            Next


            objWriter.WriteLine("public long getLocalId() {")
            objWriter.WriteLine("return localId;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public void setLocalId(long localId) {")
            objWriter.WriteLine("this.localId = localId;")
            objWriter.WriteLine("}")


            objWriter.WriteLine("public boolean isSync() {")
            objWriter.WriteLine("return isSync;")
            objWriter.WriteLine("}")


            objWriter.WriteLine("public void setSync(boolean sync) {")
            objWriter.WriteLine("isSync = sync;")
            objWriter.WriteLine("}")


            objWriter.WriteLine(" public String getMacTabletteCreated() {")
            objWriter.WriteLine("return MacTabletteCreated;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public void setMacTabletteCreated(String macTabletteCreated) {")
            objWriter.WriteLine("MacTabletteCreated = macTabletteCreated;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public String getMacTabletteModif() {")
            objWriter.WriteLine("return MacTabletteModif;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public void setMacTabletteModif(String macTabletteModif) {")
            objWriter.WriteLine("MacTabletteModif = macTabletteModif;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public String getDateCreated() {")
            objWriter.WriteLine("return DateCreated;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public void setDateCreated(String dateCreated) {")
            objWriter.WriteLine("DateCreated = dateCreated;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public String getDateModif() {")
            objWriter.WriteLine("return DateModif;")
            objWriter.WriteLine("}")

            objWriter.WriteLine(" public void setDateModif(String dateModif) {")
            objWriter.WriteLine("DateModif = dateModif;")
            objWriter.WriteLine("}")



            objWriter.WriteLine("//endregion")

            objWriter.WriteLine()

            objWriter.WriteLine()
            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()

        End Sub

        Public Shared Sub CreateAndroidHelper(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_projectName As TextBox)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\helper\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\helper\")
            Dim path As String = txt_PathGenerate_Script & nomSimple & "Helper.java"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)
            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")

            Dim content As String = "public class " & nomSimple & "Helper {" & Chr(13)
            _end = "}" & Chr(13)
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

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                ListofIndex.Insert(countindex, _index.ListofColumn.Item(0).Name)
                countindex = countindex + 1
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}


            For Each _foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add(_foreignkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next


            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add(_column.Name)
                    types.Add(_column.Type.JavaName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            objWriter.WriteLine("package ht.solutions.android." & txt_projectName.Text.Trim & ".helper;")

            objWriter.WriteLine("import android.content.ContentValues;")
            objWriter.WriteLine("import android.database.Cursor;")
            objWriter.WriteLine("import android.database.sqlite.SQLiteDatabase;")
            objWriter.WriteLine("import ht.agrotracking.mobile.DbHelper.Currentdb;")
            objWriter.WriteLine("import ht.agrotracking.mobile.DbHelper.DBConstants;")
            objWriter.WriteLine("import ht.agrotracking.mobile.model." & nomSimple & ";")

            objWriter.WriteLine("import java.util.*;")
            objWriter.WriteLine(content)
            objWriter.WriteLine()


            'cols.Add("LocalId")
            'cols.Add("isSync")
            'types.Add("long")
            'types.Add("boolean")
            'initialtypes.Add("Bigint")
            'initialtypes.Add("bit")


            With objWriter
                .WriteLine("  public static int save(" & nomSimple & " " & "obj" & ") throws Exception { ")

                .WriteLine("  Currentdb currentdb = new Currentdb();")
                .WriteLine("  currentdb.open();")
                .WriteLine("  SQLiteDatabase database = currentdb.getDb();")
                .WriteLine("  int result = 0;")
                .WriteLine("  try {")
                .WriteLine("       ContentValues newTaskValue = new ContentValues();")
                .WriteLine("       newTaskValue.put(DBConstants." & nomSimple.ToUpper & "_" & "ID" & ", " & "obj" & ".getId());")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        With objWriter
                            If initialtypes(i) <> "date" Then
                                .WriteLine("       newTaskValue.put(DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & ", " & "obj" & ".get" & cols(i) & "());")
                            ElseIf initialtypes(i) = "bit" Then
                                .WriteLine("       newTaskValue.put(DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & ", " & "obj" & ".is" & cols(i) & "());")
                            Else
                                .WriteLine("       newTaskValue.put(DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & ", " & "obj" & ".get" & cols(i) & "();")
                            End If
                        End With
                    Next
                Catch

                End Try

                .WriteLine("       newTaskValue.put(DBConstants.IS_SYNCHRONIZED, obj.IsSync());")
                .WriteLine("      newTaskValue.put(DBConstants.MAC_TABLETTE_CREATED, obj.getMacTabletteCreated());")
                .WriteLine("      newTaskValue.put(DBConstants.MAC_TABLETTE_MODIFIED, obj.getMacTabletteModif());")
                .WriteLine("       newTaskValue.put(DBConstants.DATE_CREATED, obj.getDateCreated());")
                .WriteLine("       newTaskValue.put(DBConstants.DATE_MODIF, obj.getDateModif());")


                .WriteLine("      if (searchById(" & "obj" & ".getId())==null)")
                .WriteLine("     {")
                .WriteLine("             result = (database.insert(DBConstants." & nomSimple.ToUpper & "_TABLE" & ", null, newTaskValue) > 0 ? 1: 0);")
                .WriteLine("     }  else{")
                .WriteLine("           database.update(DBConstants." & nomSimple.ToUpper & "_TABLE" & ", newTaskValue, DBConstants." & nomSimple.ToUpper & "_ID + ""=?"", new String[]{String.valueOf(" & "obj" & ".getId())});")
                .WriteLine("     }")
                .WriteLine("      return result;")
                .WriteLine("   } finally {")
                .WriteLine("        currentdb.close();")
                .WriteLine("   }")
                '  //return result;
                .WriteLine(" }")
                .WriteLine()
            End With

            With objWriter
                .WriteLine(" private static " & nomSimple & " setProperties(Cursor c) {")
                .WriteLine(" " & nomSimple & " " & "obj" & ";")
                .WriteLine("  " & "obj" & " = new " & nomSimple & "();")
                .WriteLine("  " & "obj" & ".setId(c.get" & "Int" & "(c.getColumnIndex(DBConstants." & nomSimple.ToUpper & "_" & "ID" & ")));")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        With objWriter
                            If initialtypes(i) <> "date" Then
                                .WriteLine("  " & "obj" & ".set" & cols(i) & "(c.get" & ConvertJavaToSetPropertiesAndroidType(types(i)) & "(c.getColumnIndex(DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & ")));")
                            ElseIf initialtypes(i) = "bit" Then
                                .WriteLine("  " & "obj" & ".set" & cols(i) & "(Boolean.parseBoolean(c.get" & "Int" & "(c.getColumnIndex(DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & ")) + """" ));")
                            Else
                                .WriteLine("  " & "obj" & ".set" & cols(i) & "(c.get" & ConvertJavaToSetPropertiesAndroidType(types(i)) & "(c.getColumnIndex(DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & "))  );")
                            End If
                        End With
                    Next
                Catch
                End Try
                .WriteLine(" obj.setLocalId(c.getLong(c.getColumnIndex(DBConstants.KEY_ID)));")
                .WriteLine("obj.setSync(Boolean.parseBoolean(c.getInt(c.getColumnIndex(DBConstants.IS_SYNCHRONIZED))+""));")
                .WriteLine("obj.setMacTabletteCreated(c.getString(c.getColumnIndex(DBConstants.MAC_TABLETTE_CREATED)));")
                .WriteLine("obj.setMacTabletteModif(c.getString(c.getColumnIndex(DBConstants.MAC_TABLETTE_MODIFIED)));")
                .WriteLine("obj.setDateCreated(c.getString(c.getColumnIndex(DBConstants.DATE_CREATED)));")
                .WriteLine("obj.setDateModif(c.getString(c.getColumnIndex(DBConstants.DATE_MODIF)));")
                .WriteLine("  return " & "obj" & ";")
                .WriteLine("}")
                .WriteLine()
            End With



            objWriter.WriteLine("public static ArrayList<" & nomSimple & "> SearchAll( ) {")
            objWriter.WriteLine("  ArrayList<" & nomSimple & "> " & "obj" & "s= new ArrayList<" & nomSimple & ">();")
            objWriter.WriteLine("  Currentdb currentdb = new Currentdb();")
            objWriter.WriteLine("  currentdb.open();")
            objWriter.WriteLine("" & nomSimple & " obj = null;")
            objWriter.WriteLine("  SQLiteDatabase database = currentdb.getDb();")
            objWriter.WriteLine("  Cursor c = database.query(DBConstants." & nomSimple.ToUpper & "_TABLE" & ", null, null,null, null, null, null);")
            objWriter.WriteLine("  while (c.moveToNext()) {")
            objWriter.WriteLine("      " & "obj" & " = setProperties(c);")
            objWriter.WriteLine("      " & "obj" & "s.add(" & "obj" & ");")
            objWriter.WriteLine("  }")
            objWriter.WriteLine("   c.close() ;")
            objWriter.WriteLine("   currentdb.close();")
            objWriter.WriteLine("   return " & "obj" & "s;")
            objWriter.WriteLine("}")
            objWriter.WriteLine()

            objWriter.WriteLine("public static " & nomSimple & " searchById(long id ) {")
            objWriter.WriteLine("  Currentdb currentdb = new Currentdb();")
            objWriter.WriteLine("  currentdb.open();")
            objWriter.WriteLine("  SQLiteDatabase database = currentdb.getDb();")
            objWriter.WriteLine("  String filter = DBConstants." & nomSimple.ToUpper & "_ID" & " + "" =?"";")
            objWriter.WriteLine("  Cursor c = database.query(DBConstants." & nomSimple.ToUpper & "_TABLE" & ", null, filter, new String[] {String.valueOf(id)}, null, null, null);")
            objWriter.WriteLine("  " & nomSimple & " " & "obj" & "=null;")
            objWriter.WriteLine("  if (c.moveToNext()){")
            objWriter.WriteLine("       " & "obj" & " = setProperties(c);")
            objWriter.WriteLine("   }")
            objWriter.WriteLine("  c.close() ;")
            objWriter.WriteLine("  currentdb.close();")
            objWriter.WriteLine("  return " & "obj" & ";")
            objWriter.WriteLine("}")
            objWriter.WriteLine()
            objWriter.WriteLine("//endregion")

            objWriter.WriteLine()

            objWriter.WriteLine()
            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateAndroiDBHelper(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_projectName As TextBox)
            Dim _systeme As Cls_Systeme = Cls_Systeme.getInstance
            Dim Id_table As String = ""
            Dim _end As String
            Dim ListofForeignKey As New List(Of String)
            Dim countForeignKey As Integer = 0
            Dim db As String = ""
            Dim Lcasevalue As New List(Of String) From {"String"}
            Dim nomWebform As String = name.Replace("tbl", "wbfrm").Replace("Tbl", "wbfrm").Replace("TBL", "wbfrm")
            Dim nomClasse As String = name.Replace("tbl", "Cls").Replace("Tbl", "Cls").Replace("TBL", "Cls")
            Dim nomSimple As String = name.Substring(4, name.Length - 4)
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\DBHelper\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\DBHelper\")
            Dim path As String = txt_PathGenerate_Script & nomSimple & "DBConstantsHelperScript.java"
            Dim ListofIndex As New List(Of String)
            Dim ListofIndexType As New List(Of String)
            Dim index_li_type As New Hashtable
            Dim countindex As Long = 0
            Dim insertstring As String = ""
            Dim updatestring As String = ""
            Dim listoffound_virguleIndex As New List(Of String)
            Dim header As String = "REM Generate By [GENERIC 12] Application *******" & Chr(13) _
                                   & "REM  Class " + nomWebform & Chr(13) & Chr(13) _
                                   & "REM Date:" & Date.Now.ToString("dd-MMM-yyyy H\h:i:s\m")

            Dim content As String = "public class " & nomSimple & " {" & Chr(13)
            _end = "}" & Chr(13)
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

            For Each _index As Cls_UniqueIndex In _table.ListofIndex
                ListofIndex.Insert(countindex, _index.ListofColumn.Item(0).Name)
                countindex = countindex + 1
            Next

            Dim nottoputforlist As New List(Of String) From {Id_table, "_isdirty", "_LogData"}


            For Each _foreignkey As Cls_ForeignKey In _table.ListofForeinKey
                ListofForeignKey.Add(_foreignkey.Column.Name)
                countForeignKey = countForeignKey + 1
            Next


            For Each _column As Cls_Column In _table.ListofColumn
                If count < cap - 4 Then
                    cols.Add(_column.Name)
                    types.Add(_column.Type.JavaName)
                    initialtypes.Add(_column.Type.SqlServerName)
                    length.Add(_column.Length)
                    count += 1
                Else
                    Exit For
                End If
            Next
            'objWriter.WriteLine("package ht.solutions.android." & txt_projectName.Text & ".modele;")
            'objWriter.WriteLine("import com.google.gson.annotations.Expose;")
            'objWriter.WriteLine("import com.google.gson.annotations.SerializedName;")
            'objWriter.WriteLine()
            'objWriter.WriteLine("import java.util.*;")
            '   objWriter.WriteLine(content)
            objWriter.WriteLine()


            'cols.Add("localId")
            'cols.Add("isSync")
            'types.Add("long")
            'types.Add("boolean")
            'initialtypes.Add("Byte")
            'initialtypes.Add("nvarchar")

            With objWriter
                .WriteLine("  public static final String " & nomSimple.ToUpper & "_TABLE = ""tbl_" & nomSimple.ToLower & """;")
                .WriteLine("  public static final String " & nomSimple.ToUpper & "_ID = """ & nomSimple.ToLower & "_id"";")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("  public static final String " & nomSimple.ToUpper & "_" & cols(i).ToUpper & " = """ & nomSimple.ToLower & "_" & cols(i).ToLower & """;")
                    Next
                Catch
                End Try
            End With

            With objWriter
                .WriteLine()
                .WriteLine("  public static final String CREATE_TABLE_" & nomSimple.ToUpper & " = ""create table "" +")
                .WriteLine("DBConstants." & nomSimple.ToUpper & "_TABLE + "" ("" +")
                .WriteLine("DBConstants.KEY_ID + "" integer primary key autoincrement, "" +")
                .WriteLine("DBConstants." & nomSimple.ToUpper & "_ID + "" integer not null, "" +")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        If types(i) = "String" Or types(i) = "Date" Or types(i) = "DateTime" Then
                            .WriteLine("DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & " + "" " & "text" & " not null, "" +")
                        Else
                            .WriteLine("DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & " + "" " & types(i) & " not null, "" +")
                        End If
                    Next
                    .WriteLine(" DBConstants.IS_SYNCHRONIZED + "" boolean not null default 0, "" +")
                    .WriteLine(" DBConstants.MAC_TABLETTE_CREATED + "" text not null, "" +")
                    .WriteLine(" DBConstants.MAC_TABLETTE_MODIFIED + "" text, "" +")
                    .WriteLine(" DBConstants.DATE_CREATED + "" text not null, "" +")
                    .WriteLine(" DBConstants.DATE_MODIF + "" text, "" +")
                    .WriteLine(" DBConstants.DATE_NAME + "" long);"";")
                Catch
                End Try
            End With
            objWriter.WriteLine()
            objWriter.WriteLine(" db.execSQL(DBScript.CREATE_TABLE_" & nomSimple.ToUpper & ");")
            objWriter.WriteLine(" Log.v(""MyDBhelper onCreate"", ""Created Table "" + DBConstants." & nomSimple.ToUpper & "_TABLE);")

            objWriter.WriteLine()
            objWriter.WriteLine()
            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub



#Region "Conversion Fonctions"
        Public Shared Function DefaultValue(ByVal type As String) As Object
            Select Case type
                Case "String"
                    Return ""
                Case "Long"
                    Return 0
                Case "Integer"
                    Return 0
                Case "Int32"
                    Return 0
                Case "Int64"
                    Return 0
                Case "Boolean"
                    Return False
                Case Else
                    Return "Nothing"
            End Select
        End Function

        Public Shared Function ConvertDBToJavaType(ByVal Type As String) As String
            Dim AndroidTypeHash As New Hashtable
            AndroidTypeHash.Add("bigint", "long")
            AndroidTypeHash.Add("binary", "boolean")
            AndroidTypeHash.Add("bit", "byte")
            AndroidTypeHash.Add("char", "char")
            AndroidTypeHash.Add("date", "Date")
            AndroidTypeHash.Add("datetime", "Date")
            AndroidTypeHash.Add("datetime2", "Date")
            AndroidTypeHash.Add("DATETIMEOFFSET", "Date")
            AndroidTypeHash.Add("decimal", "double")
            AndroidTypeHash.Add("float", "float")
            AndroidTypeHash.Add("int", "int")
            AndroidTypeHash.Add("image", "byte[]")
            AndroidTypeHash.Add("money", "Currency")
            AndroidTypeHash.Add("nchar", "String") '' /* or tableau of char*/
            AndroidTypeHash.Add("nvarchar", "String")
            AndroidTypeHash.Add("numeric", "double")
            AndroidTypeHash.Add("rowversion", "")
            AndroidTypeHash.Add("smallint", "short")
            AndroidTypeHash.Add("smallmoney", "Currency")
            AndroidTypeHash.Add("time", "Time")
            AndroidTypeHash.Add("varbinary", "")
            AndroidTypeHash.Add("varchar", "String")


            Return AndroidTypeHash(Type)
        End Function

        Public Shared Function ConvertDBToJavaParsingType(ByVal Type As String) As String
            If Type.Contains("decimal") Or Type.Contains("numeric") Then
                Type = "decimal"
            End If
            Dim AndroidTypeHash As New Hashtable
            AndroidTypeHash.Add("bigint", "Long")
            AndroidTypeHash.Add("binary", "Boolean")
            AndroidTypeHash.Add("bit", "Boolean")
            AndroidTypeHash.Add("char", "char")
            AndroidTypeHash.Add("date", "Date")
            AndroidTypeHash.Add("datetime", "Date")
            AndroidTypeHash.Add("datetime2", "Date")
            AndroidTypeHash.Add("DATETIMEOFFSET", "Date")
            AndroidTypeHash.Add("decimal", "Double")
            AndroidTypeHash.Add("float", "FLOAT")
            AndroidTypeHash.Add("int", "Integer")
            AndroidTypeHash.Add("image", "byte[]")
            AndroidTypeHash.Add("money", "Currency")
            AndroidTypeHash.Add("nchar", "String") '' /* or tableau of char*/
            AndroidTypeHash.Add("nvarchar", "String")
            AndroidTypeHash.Add("numeric", "Double")
            AndroidTypeHash.Add("rowversion", "")
            AndroidTypeHash.Add("smallint", "Integer")
            AndroidTypeHash.Add("smallmoney", "Currency")
            AndroidTypeHash.Add("time", "Time")
            AndroidTypeHash.Add("varbinary", "")
            AndroidTypeHash.Add("varchar", "String")


            Return AndroidTypeHash(Type)
        End Function

        Public Shared Function ConvertJavaToSetPropertiesAndroidType(ByVal Type As String) As String
            Dim AndroidTypeHash As New Hashtable
            AndroidTypeHash.Add("long", "Long")
            AndroidTypeHash.Add("boolean", "boolean")
            AndroidTypeHash.Add("byte", "byte")
            AndroidTypeHash.Add("date", "Date")
            AndroidTypeHash.Add("Date", "Date")
            AndroidTypeHash.Add("float", "Float")
            AndroidTypeHash.Add("int", "Int")
            AndroidTypeHash.Add("byte[]", "byte[]")
            AndroidTypeHash.Add("Currency", "Currency")
            AndroidTypeHash.Add("String", "String")
            AndroidTypeHash.Add("double", "Double")
            AndroidTypeHash.Add("rowversion", "")
            AndroidTypeHash.Add("Time", "Time")
            AndroidTypeHash.Add("varbinary", "")
            AndroidTypeHash.Add("DateTime", "String")



            Return AndroidTypeHash(Type)
        End Function

#End Region


    End Class
End Namespace
