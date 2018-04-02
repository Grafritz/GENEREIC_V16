Imports System.Management
Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Xml



Namespace SqlServer.IMode

    Public Class AndroidFormGenerator


        Public Shared Sub CreateAndroidBinderListview(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_projectName As TextBox)
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
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\adaptor\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\adaptor\")
            Dim path As String = txt_PathGenerate_Script & "Binder" & nomSimple & ".java"
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

            objWriter.WriteLine()





            objWriter.WriteLine("  package ht.mesi.mobile.adaptor;")

            objWriter.WriteLine("import android.app.Activity;")
            objWriter.WriteLine("import android.content.Context;")
            objWriter.WriteLine("import android.view.LayoutInflater;")
            objWriter.WriteLine("import android.view.View;")
            objWriter.WriteLine("import android.view.ViewGroup;")
            objWriter.WriteLine("import android.widget.BaseAdapter;")
            objWriter.WriteLine("import android.widget.ImageView;")
            objWriter.WriteLine("import android.widget.TextView;")
            objWriter.WriteLine("import ht.mesi.mobile.activity.R;")

            objWriter.WriteLine("import java.util.HashMap;")
            objWriter.WriteLine("import java.util.List;")

            objWriter.WriteLine("/**")
            objWriter.WriteLine("* Created with IntelliJ IDEA.")
            objWriter.WriteLine("* User: JSMINFINI")
            objWriter.WriteLine("* Date: 1/27/14")
            objWriter.WriteLine("* Time: 3:54 PM")
            objWriter.WriteLine("* To change this template use File | Settings | File Templates.")
            objWriter.WriteLine("*/")
            objWriter.WriteLine("public class Binder" & nomSimple & " extends BaseAdapter {")


            objWriter.WriteLine("LayoutInflater inflater;")
            objWriter.WriteLine("ImageView thumb_image;")
            objWriter.WriteLine("List<HashMap<String,String>> " & nomSimple.ToLower & "DataCollection;")
            objWriter.WriteLine("ViewHolder holder;")
            objWriter.WriteLine("Activity _activity ;")

            objWriter.WriteLine("public Binder" & nomSimple & "() {")
            objWriter.WriteLine("// TODO Auto-generated constructor stub")
            objWriter.WriteLine("}")


            objWriter.WriteLine("public Binder" & nomSimple & "(Activity act, List<HashMap<String,String>> map) {")

            objWriter.WriteLine("this." & nomSimple.ToLower & "DataCollection = map;")
            objWriter.WriteLine("this._activity = act;")

            objWriter.WriteLine("inflater = (LayoutInflater) act")
            objWriter.WriteLine(".getSystemService(Context.LAYOUT_INFLATER_SERVICE);")
            objWriter.WriteLine("}")


            objWriter.WriteLine("public int getCount() {")
            objWriter.WriteLine("// TODO Auto-generated method stub")
            objWriter.WriteLine("//		return idlist.size();")
            objWriter.WriteLine("return " & nomSimple.ToLower & "DataCollection.size();")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public Object getItem(int arg0) {")
            objWriter.WriteLine("// TODO Auto-generated method stub")
            objWriter.WriteLine("return null;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public long getItemId(int position) {")
            objWriter.WriteLine("// TODO Auto-generated method stub")
            objWriter.WriteLine("return 0;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("public View getView(int position, View convertView, ViewGroup parent) {")

            objWriter.WriteLine("View vi=convertView;")
            objWriter.WriteLine("if(convertView==null){")

            objWriter.WriteLine("vi = inflater.inflate(R.layout.list_row_" & nomSimple.ToLower & ", null);")
            objWriter.WriteLine("holder = new ViewHolder();")


            With objWriter
                Dim val As String = ""
                val = " holder.tv" & nomSimple & "Id" & nomSimple & " = (TextView)vi.findViewById(R.id.tv" & nomSimple & "Id" & nomSimple & "); "

                .WriteLine(val)
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("holder.tv" & nomSimple & "" & cols(i) & " = (TextView)vi.findViewById(R.id.tv" & nomSimple & "" & cols(i) & "); ")
                    Next
                Catch
                End Try
            End With

            With objWriter
                .WriteLine("holder.tv" & nomSimple & "Id" & nomSimple & ".setText(" & nomSimple & "DataCollection.get(position).get(DBConstants." & nomSimple.ToUpper & "_ID_" & nomSimple.ToUpper & "));")

                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("holder.tv" & nomSimple & "" & cols(i) & ".setText(" & nomSimple & "DataCollection.get(position).get(DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & "));")
                    Next
                Catch
                End Try
            End With


            objWriter.WriteLine("vi.setTag(holder);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("else{")

            objWriter.WriteLine("holder = (ViewHolder)vi.getTag();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("if (position % 2 == 1) {")
            objWriter.WriteLine("vi.setBackgroundColor(_activity.getResources().getColor(R.color.grid_alt_row_color));")
            objWriter.WriteLine("} else {")
            objWriter.WriteLine("vi.setBackgroundColor(_activity.getResources().getColor(R.color.grid_row_color));")
            objWriter.WriteLine("}")



            objWriter.WriteLine("return vi;")
            objWriter.WriteLine("    }")


            objWriter.WriteLine("static class ViewHolder{")


            With objWriter
                .WriteLine("        TextView tv" & nomSimple & "Id;")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("        TextView tv" & nomSimple & "" & cols(i) & ";")
                    Next
                Catch
                End Try
            End With

            objWriter.WriteLine("}")
            objWriter.WriteLine("}")

            objWriter.WriteLine()
            objWriter.WriteLine()

            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateAndroidListViewActivity(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_projectName As TextBox)
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


            cols.Add("localId")
            cols.Add("isSync")
            types.Add("long")
            types.Add("boolean")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")



            objWriter.WriteLine("package ht.mesi.mobile.activity;")

            objWriter.WriteLine("import android.app.*;")
            objWriter.WriteLine("import android.content.Context;")
            objWriter.WriteLine("import android.content.DialogInterface;")
            objWriter.WriteLine("import android.content.Intent;")
            objWriter.WriteLine("import android.content.SharedPreferences;")
            objWriter.WriteLine("import android.os.AsyncTask;")
            objWriter.WriteLine("import android.os.Bundle;")
            objWriter.WriteLine("import android.preference.PreferenceManager;")
            objWriter.WriteLine("import android.util.Log;")
            objWriter.WriteLine("import android.view.*;")
            objWriter.WriteLine("import android.widget.*;")
            objWriter.WriteLine("import ht.mesi.mobile.DbHelper.CurrentDB;")
            objWriter.WriteLine("import ht.mesi.mobile.DbHelper.DBConstants;")
            objWriter.WriteLine("import ht.mesi.mobile.adaptor.BinderQuestionnaire;")
            objWriter.WriteLine("import ht.mesi.mobile.global.Utils;")
            objWriter.WriteLine("import ht.mesi.mobile.helper.*;")
            objWriter.WriteLine("import ht.mesi.mobile.model.*;")
            objWriter.WriteLine("import ht.mesi.mobile.service.SynchroData;")

            objWriter.WriteLine("import java.util.ArrayList;")
            objWriter.WriteLine("import java.util.HashMap;")
            objWriter.WriteLine("import java.util.List;")

            objWriter.WriteLine("/**")
            objWriter.WriteLine("* Created by DIMITRY DABADY on 2/19/14.")
            objWriter.WriteLine("*/")
            objWriter.WriteLine("public class ListQuestionnaireActivity extends Activity implements ActionBar.OnNavigationListener {")
            objWriter.WriteLine("Context context;")
            objWriter.WriteLine("ListView listview_questionnaire;")
            objWriter.WriteLine("List<HashMap<String, String>> questionnaireDataCollection;")
            objWriter.WriteLine("ArrayList<Institution> listInstitution;")
            objWriter.WriteLine("ArrayList<Mois> listMois;")
            objWriter.WriteLine("ArrayList<ht.mesi.mobile.model.Annee> listAnnee;")
            objWriter.WriteLine("Spinner spinnerInstitution;")
            objWriter.WriteLine("Spinner spinnerMois;")
            objWriter.WriteLine("Spinner spinnerAnnee;")
            objWriter.WriteLine("Spinner spinnerStatut;")
            objWriter.WriteLine("long id_Institution = 0;")
            objWriter.WriteLine("int id_Mois = 0;")
            objWriter.WriteLine("long Annee = 0;")
            objWriter.WriteLine("String statut = GlobalVariables.KEY_QUESTIONNAIRE_EN_COURS;")
            objWriter.WriteLine("String _message;")
            objWriter.WriteLine("private int SYNCHO = 1480;")
            objWriter.WriteLine("private int NOTIFICATION = 1478;")

            objWriter.WriteLine("    LayoutInflater inflater;")
            objWriter.WriteLine("private ActionBar actionBar;")
            objWriter.WriteLine("private ProgressDialog progress;")
            objWriter.WriteLine("private SessionManager session;")
            objWriter.WriteLine("private UserModel currentUser;")
            objWriter.WriteLine("private Menu _menu = null;")

            objWriter.WriteLine("    long idformulaire = 0;")
            objWriter.WriteLine("Button btn_prev_question;")
            objWriter.WriteLine("Button btn_next_question;")

            objWriter.WriteLine("int lastMenuActionClicked;")
            objWriter.WriteLine("TextView label_connectedUser;")

            objWriter.WriteLine("    @Override")
            objWriter.WriteLine("public void onCreate(Bundle savedInstanceState) {")
            objWriter.WriteLine("super.onCreate(savedInstanceState);")
            objWriter.WriteLine("getWindow().requestFeature(Window.FEATURE_ACTION_BAR);")
            objWriter.WriteLine("setContentView(R.layout.list_questionnaire_layout);")
            objWriter.WriteLine("context = this;")
            objWriter.WriteLine("progress = new ProgressDialog(this);")
            objWriter.WriteLine("Intent i = getIntent();")
            objWriter.WriteLine("idformulaire = Long.parseLong(i.getStringExtra(GlobalVariables.KEY_INTENT_ID_FORMULAIRE));")

            objWriter.WriteLine("ActionBarCall();")
            objWriter.WriteLine("//Code a tester")
            objWriter.WriteLine("inflater = (LayoutInflater) this.getSystemService(Context.LAYOUT_INFLATER_SERVICE);")
            objWriter.WriteLine("View vi = inflater.inflate(R.layout.list_questionnaire_layout, null);")
            objWriter.WriteLine("//code a tester")
            objWriter.WriteLine("session = new SessionManager(this);")

            objWriter.WriteLine("/**")
            objWriter.WriteLine("* Call this function whenever you want to check user login")
            objWriter.WriteLine("* This will redirect user to LoginActivity is he is not")
            objWriter.WriteLine("* logged in")
            objWriter.WriteLine("* */")
            objWriter.WriteLine("currentUser = session.getUserDetails();")
            objWriter.WriteLine("Utils.PopulateMoisData();")
            objWriter.WriteLine("Utils.PopulateAnneeData();")
            objWriter.WriteLine("")
            objWriter.WriteLine("        try {")
            objWriter.WriteLine("questionnaireDataCollection = new ArrayList<HashMap<String, String>>();")
            objWriter.WriteLine("")
            objWriter.WriteLine("            listInstitution = InstitutionHelper.SearchAllTypeDroit(Integer.parseInt(String.valueOf(currentUser.getTypeDroit())), currentUser.getIdDroit());")
            objWriter.WriteLine("listMois = MoisHelper.SearchAll();")
            objWriter.WriteLine("listAnnee = AnneeHelper.SearchAll();")
            objWriter.WriteLine("")
            objWriter.WriteLine("            listview_questionnaire = (ListView) findViewById(R.id.listview_questionnaire);")

            objWriter.WriteLine("spinnerInstitution = (Spinner) findViewById(R.id.spinner_Institution);")
            objWriter.WriteLine("spinnerMois = (Spinner) findViewById(R.id.spinner_Mois);")
            objWriter.WriteLine("spinnerAnnee = (Spinner) findViewById(R.id.spinner_Annee);")
            objWriter.WriteLine("spinnerStatut = (Spinner) findViewById(R.id.spinner_Statut);")
            objWriter.WriteLine("")
            objWriter.WriteLine("label_connectedUser = (TextView) findViewById(R.id.label_connectedUser);")
            objWriter.WriteLine("label_connectedUser.setText(""Utilisateur connecté : "" + currentUser.toString());")
            objWriter.WriteLine("if (listInstitution != null) {")
            objWriter.WriteLine("Institution inst = new Institution();")
            objWriter.WriteLine("inst.setNomInstitution(""Filtrer par institution"");")
            objWriter.WriteLine("listInstitution.add(0, inst);")
            objWriter.WriteLine("final ArrayAdapter<Institution> adapterInstitution = new ArrayAdapter<Institution>(ListQuestionnaireActivity.this,")
            objWriter.WriteLine("android.R.layout.simple_spinner_item, listInstitution);")
            objWriter.WriteLine("adapterInstitution.setDropDownViewResource(android.R.layout.select_dialog_singlechoice);")
            objWriter.WriteLine("spinnerInstitution.setAdapter(adapterInstitution); // Set the custom adapter to the spinner")
            objWriter.WriteLine("id_Institution = adapterInstitution.getItem(0).getId_Institution();")
            objWriter.WriteLine("// You can create an anonymous listener to handle the event when is selected an spinner item")
            objWriter.WriteLine("spinnerInstitution.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public void onItemSelected(AdapterView<?> adapterView, View view,")
            objWriter.WriteLine("int position, long id) {")
            objWriter.WriteLine("id_Institution = adapterInstitution.getItem(position).getId_Institution();")
            objWriter.WriteLine("BindListView();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("                    @Override")
            objWriter.WriteLine("public void onNothingSelected(AdapterView<?> adapter) {")
            objWriter.WriteLine("}")
            objWriter.WriteLine("});")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
            objWriter.WriteLine("if (listMois != null) {")
            objWriter.WriteLine("Mois m = new Mois();")
            objWriter.WriteLine("m.setMois(""Filtrer par mois"");")
            objWriter.WriteLine("listMois.add(0, m);")
            objWriter.WriteLine("final ArrayAdapter<Mois> adapterMois = new ArrayAdapter<Mois>(ListQuestionnaireActivity.this,")
            objWriter.WriteLine("android.R.layout.simple_spinner_item,")
            objWriter.WriteLine("listMois);")
            objWriter.WriteLine("adapterMois.setDropDownViewResource(android.R.layout.select_dialog_singlechoice);")
            objWriter.WriteLine("spinnerMois.setAdapter(adapterMois); // Set the custom adapter to the spinner")
            objWriter.WriteLine("id_Mois = adapterMois.getItem(0).getId_Mois();")
            objWriter.WriteLine("// You can create an anonymous listener to handle the event when is selected an spinner item")
            objWriter.WriteLine("spinnerMois.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public void onItemSelected(AdapterView<?> adapterView, View view,")
            objWriter.WriteLine("int position, long id) {")
            objWriter.WriteLine("id_Mois = adapterMois.getItem(position).getId_Mois();")
            objWriter.WriteLine("BindListView();")
            objWriter.WriteLine("//                        new LoginTask().execute(1);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("                    @Override")
            objWriter.WriteLine("public void onNothingSelected(AdapterView<?> adapter) {")
            objWriter.WriteLine("}")
            objWriter.WriteLine("});")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("            if (listAnnee != null) {")
            objWriter.WriteLine("Annee a = new Annee();")
            objWriter.WriteLine("a.setAnnee(""Filtrer par année"");")
            objWriter.WriteLine("listAnnee.add(0, a);")
            objWriter.WriteLine("final ArrayAdapter<Annee> adapterAnnee = new ArrayAdapter<Annee>(ListQuestionnaireActivity.this, android.R.layout.simple_spinner_item, listAnnee);")
            objWriter.WriteLine("adapterAnnee.setDropDownViewResource(android.R.layout.select_dialog_singlechoice);")
            objWriter.WriteLine("spinnerAnnee.setAdapter(adapterAnnee); // Set the custom adapter to the spinner")
            objWriter.WriteLine("Annee = Long.parseLong(""0"");")
            objWriter.WriteLine("// You can create an anonymous listener to handle the event when is selected an spinner item")
            objWriter.WriteLine("spinnerAnnee.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public void onItemSelected(AdapterView<?> adapterView, View view,")
            objWriter.WriteLine("int position, long id) {")
            objWriter.WriteLine("if (adapterAnnee.getItem(position).getAnnee() == ""Filtrer par année"") {")
            objWriter.WriteLine("Annee = Long.parseLong(""0"");")
            objWriter.WriteLine("} else {")
            objWriter.WriteLine("Annee = Long.parseLong(adapterAnnee.getItem(position).getAnnee());")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("                        BindListView();")
            objWriter.WriteLine("}")

            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public void onNothingSelected(AdapterView<?> adapter) {")
            objWriter.WriteLine("}")
            objWriter.WriteLine("});")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("            String[] questionnaireStatut = getResources().getStringArray(R.array.questionnaire_statut);")
            objWriter.WriteLine("final ArrayAdapter<String> adapterStatut = new ArrayAdapter<String>(ListQuestionnaireActivity.this, android.R.layout.simple_spinner_item, questionnaireStatut);")
            objWriter.WriteLine("adapterStatut.setDropDownViewResource(android.R.layout.select_dialog_singlechoice);")
            objWriter.WriteLine("spinnerStatut.setAdapter(adapterStatut);")
            objWriter.WriteLine("spinnerStatut.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public void onItemSelected(AdapterView<?> adapterView, View view,")
            objWriter.WriteLine("int position, long id) {")
            objWriter.WriteLine("statut = adapterStatut.getItem(position);")
            objWriter.WriteLine("ShowMenuActions(Questionnaire.StringToStatut(statut));")
            objWriter.WriteLine("BindListView();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("                @Override")
            objWriter.WriteLine("public void onNothingSelected(AdapterView<?> adapter) {")
            objWriter.WriteLine("}")
            objWriter.WriteLine("});")
            objWriter.WriteLine("BindListView();")
            objWriter.WriteLine("")
            objWriter.WriteLine("listview_questionnaire.setOnItemClickListener(new AdapterView.OnItemClickListener() {")
            objWriter.WriteLine("    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {")
            objWriter.WriteLine("Intent i = new Intent();")
            objWriter.WriteLine("i.setClass(context, ListQuestionActivity.class);")
            objWriter.WriteLine("long tmp_idInstitution = Long.parseLong(questionnaireDataCollection.get(position).get(DBConstants.QUESTIONNAIRE_ID_INSTITUTION));")
            objWriter.WriteLine("int tmp_Mois = Integer.parseInt(questionnaireDataCollection.get(position).get(DBConstants.QUESTIONNAIRE_MOIS));")
            objWriter.WriteLine("long tmp_Annee = Long.parseLong(questionnaireDataCollection.get(position).get(DBConstants.QUESTIONNAIRE_ANNEE));")
            objWriter.WriteLine("i.putExtra(GlobalVariables.KEY_INTENT_ID_FORMULAIRE, String.valueOf(idformulaire));")
            objWriter.WriteLine("i.putExtra(GlobalVariables.KEY_INTENT_ID_INSTITUTION, String.valueOf(tmp_idInstitution));")
            objWriter.WriteLine("i.putExtra(GlobalVariables.KEY_INTENT_ID_MOIS, String.valueOf(tmp_Mois));")
            objWriter.WriteLine("i.putExtra(GlobalVariables.KEY_INTENT_ANNEE, String.valueOf(tmp_Annee));")
            objWriter.WriteLine("startActivity(i);")
            objWriter.WriteLine("")
            objWriter.WriteLine("}")
            objWriter.WriteLine("});")
            objWriter.WriteLine("} catch (Exception ex) {")
            objWriter.WriteLine("Log.e(""Error onCreate ListQuestionnaireActivity"", """" + ex.getMessage().toString());")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public void onResume() {")
            objWriter.WriteLine("super.onResume();")
            objWriter.WriteLine("//        statut = GlobalVariables.KEY_QUESTIONNAIRE_EN_COURS;")
            objWriter.WriteLine("//        ShowMenuActions(Questionnaire.StringToStatut(statut));")
            objWriter.WriteLine("BindListView();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("private void ActionBarCall() {")
            objWriter.WriteLine("actionBar = getActionBar();")
            objWriter.WriteLine("// Hide the action bar title")
            objWriter.WriteLine("CharSequence activity_title_listequestionnaireactivity = getText(R.string.activity_title_listequestionnaireactivity);")
            objWriter.WriteLine("Formulaire form = FormulaireHelper.searchByID(idformulaire);")
            objWriter.WriteLine("actionBar.setTitle((activity_title_listequestionnaireactivity + "" "" + form.getDescription()).toString().toUpperCase());")
            objWriter.WriteLine("actionBar.setDisplayShowTitleEnabled(true);")
            objWriter.WriteLine("// Enabling Spinner dropdown navigation")
            objWriter.WriteLine("actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_STANDARD);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("private void BindListView() {")
            objWriter.WriteLine("progress.setMessage(this.getString(R.string.loading_data));")
            objWriter.WriteLine("progress.setProgressStyle(ProgressDialog.STYLE_SPINNER);")
            objWriter.WriteLine("progress.setIndeterminate(true);")
            objWriter.WriteLine("progress.show();")
            objWriter.WriteLine("progress.setProgress(0);")
            objWriter.WriteLine("")
            objWriter.WriteLine("ArrayList<Questionnaire> objs = QuestionnaireHelper.searchAllBy_Form_Inst_Mois_Annee_Statut_User(idformulaire, id_Institution, id_Mois, Annee, statut, currentUser);")
            objWriter.WriteLine("")
            objWriter.WriteLine("HashMap<String, String> map = null;")
            objWriter.WriteLine("questionnaireDataCollection.clear();")
            objWriter.WriteLine("for (Questionnaire obj : objs) {")
            objWriter.WriteLine("map = new HashMap<String, String>();")
            objWriter.WriteLine("map.put(DBConstants.QUESTIONNAIRE_ID_QUESTIONNAIRE, String.valueOf(obj.getIdQuestionnaire()));")
            objWriter.WriteLine("map.put(DBConstants.QUESTIONNAIRE_ID_FORMULAIRE, String.valueOf(obj.getIdFormulaire()));")
            objWriter.WriteLine("map.put(DBConstants.QUESTIONNAIRE_ID_INSTITUTION, String.valueOf(obj.getIdInstitution()));")
            objWriter.WriteLine("map.put(DBConstants.QUESTIONNAIRE_MOIS, String.valueOf(obj.getMois()));")
            objWriter.WriteLine("map.put(DBConstants.QUESTIONNAIRE_ANNEE, String.valueOf(obj.getAnnee()));")
            objWriter.WriteLine("map.put(DBConstants.QUESTIONNAIRE_STATUT, String.valueOf(obj.getStatut()));")
            objWriter.WriteLine("")
            objWriter.WriteLine("questionnaireDataCollection.add(map);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("BinderQuestionnaire bindingData = new BinderQuestionnaire(this, questionnaireDataCollection, id_Institution, id_Mois, Annee);")
            objWriter.WriteLine("")
            objWriter.WriteLine("listview_questionnaire.setAdapter(null);")
            objWriter.WriteLine("listview_questionnaire.setAdapter(bindingData);")
            objWriter.WriteLine("progress.setProgress(100);")
            objWriter.WriteLine("progress.dismiss();")
            objWriter.WriteLine("")
            objWriter.WriteLine("if (objs == null || objs.size() == 0) {")
            objWriter.WriteLine("_message = ""La recherhe ne ramène aucun rapport !"";")
            objWriter.WriteLine("MessageToShow();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public boolean onNavigationItemSelected(int itemPosition, long itemId) {")
            objWriter.WriteLine("return false;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public boolean onCreateOptionsMenu(Menu menu) {")
            objWriter.WriteLine("MenuInflater inflater = getMenuInflater();")
            objWriter.WriteLine("inflater.inflate(R.menu.menu_add_valid_return_sync, menu);")
            objWriter.WriteLine("_menu = menu;")
            objWriter.WriteLine("")
            objWriter.WriteLine("ShowMenuActions(Questionnaire.StringToStatut(statut));")
            objWriter.WriteLine("")
            objWriter.WriteLine("return super.onCreateOptionsMenu(menu);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("private void ShowMenuActions(Questionnaire.STATUS_QUESTIONNAIRE stat) {")
            objWriter.WriteLine("switch (stat) {")
            objWriter.WriteLine("case EN_COURS: case RETOURNE:")
            objWriter.WriteLine("ApplicationHelper.ShowOrHideMenuOption(_menu, true, R.id.action_validate);")
            objWriter.WriteLine("ApplicationHelper.ShowOrHideMenuOption(_menu, false, R.id.action_return);")
            objWriter.WriteLine("ApplicationHelper.ShowOrHideMenuOption(_menu, false, R.id.action_synchronise);")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("case TO_BE_SYNCHRONIZED:")
            objWriter.WriteLine("ApplicationHelper.ShowOrHideMenuOption(_menu, false, R.id.action_validate);")
            objWriter.WriteLine("ApplicationHelper.ShowOrHideMenuOption(_menu, true, R.id.action_return);")
            objWriter.WriteLine("ApplicationHelper.ShowOrHideMenuOption(_menu, true, R.id.action_synchronise);")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("case SYNCHRONIZED:")
            objWriter.WriteLine("ApplicationHelper.ShowOrHideMenuOption(_menu, false, R.id.action_validate);")
            objWriter.WriteLine("ApplicationHelper.ShowOrHideMenuOption(_menu, true, R.id.action_return);")
            objWriter.WriteLine("ApplicationHelper.ShowOrHideMenuOption(_menu, false, R.id.action_synchronise);")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public boolean onOptionsItemSelected(MenuItem item) {")
            objWriter.WriteLine("// Take appropriate action for each action item click")
            objWriter.WriteLine("")
            objWriter.WriteLine("switch (item.getItemId()) {")
            objWriter.WriteLine("case android.R.id.home:")
            objWriter.WriteLine("ApplicationHelper.SafelyNavigateUpTo(this);")
            objWriter.WriteLine("return true;")
            objWriter.WriteLine("")
            objWriter.WriteLine("case R.id.action_add:")
            objWriter.WriteLine("Intent i = new Intent();")
            objWriter.WriteLine("i.setClass(context, AddEditQuestionnaireActivity.class);")
            objWriter.WriteLine("i.putExtra(GlobalVariables.KEY_INTENT_ID_INSTITUTION, String.valueOf(id_Institution));")
            objWriter.WriteLine("i.putExtra(GlobalVariables.KEY_INTENT_ID_FORMULAIRE, String.valueOf(idformulaire));")
            objWriter.WriteLine("                i.putExtra(GlobalVariables.KEY_INTENT_ID_MOIS, String.valueOf(id_Mois));")
            objWriter.WriteLine("i.putExtra(GlobalVariables.KEY_INTENT_ANNEE, String.valueOf(Annee));")
            objWriter.WriteLine("startActivity(i);")
            objWriter.WriteLine("return true;")
            objWriter.WriteLine("")
            objWriter.WriteLine("case R.id.action_validate:")
            objWriter.WriteLine("DoConfirmAction(item.getItemId(), R.string.lb_questionnaire_validate_msg, R.string.lb_questionnaire_validate_title);")
            objWriter.WriteLine("return true;")
            objWriter.WriteLine("")
            objWriter.WriteLine("case R.id.action_return:")
            objWriter.WriteLine("DoConfirmAction(item.getItemId(), R.string.lb_questionnaire_return_msg, R.string.lb_questionnaire_return_title);")
            objWriter.WriteLine("return true;")
            objWriter.WriteLine("")
            objWriter.WriteLine("case R.id.action_synchronise:")
            objWriter.WriteLine("DoConfirmAction(item.getItemId(), R.string.lb_questionnaire_synchronize_msg, R.string.lb_questionnaire_synchronize_title);")
            objWriter.WriteLine("return true;")
            objWriter.WriteLine("")
            objWriter.WriteLine("case R.id.action_disconnect:")
            objWriter.WriteLine("session = new SessionManager(this);")
            objWriter.WriteLine("session.logoutUser();")
            objWriter.WriteLine("finish();")
            objWriter.WriteLine("return true;")
            objWriter.WriteLine("default:")
            objWriter.WriteLine("return super.onOptionsItemSelected(item);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("private void DoConfirmAction(int menuActionClicked, int confirmMessageRes, int confirmTitleRes){")
            objWriter.WriteLine("lastMenuActionClicked = menuActionClicked;")
            objWriter.WriteLine("// 1. Instantiate an AlertDialog.Builder with its constructor")
            objWriter.WriteLine("AlertDialog.Builder builder = new AlertDialog.Builder(context);")
            objWriter.WriteLine("")
            objWriter.WriteLine("// 2. Chain together various setter methods to set the dialog characteristics")
            objWriter.WriteLine("builder.setMessage(confirmMessageRes).setTitle(confirmTitleRes);")
            objWriter.WriteLine("")
            objWriter.WriteLine("// 3. Add the buttons")
            objWriter.WriteLine("builder.setPositiveButton(R.string.lb_Yes, new DialogInterface.OnClickListener() {")
            objWriter.WriteLine("public void onClick(DialogInterface dialog, int id) {")
            objWriter.WriteLine("QuestionnaireActionTask _doQuestTask = new QuestionnaireActionTask(context);")
            objWriter.WriteLine("_doQuestTask.execute(lastMenuActionClicked);")
            objWriter.WriteLine("//")
            objWriter.WriteLine("//                // User clicked OK button")
            objWriter.WriteLine("//                progress.setProgressStyle(ProgressDialog.STYLE_SPINNER);")
            objWriter.WriteLine("//                progress.setIndeterminate(true);")
            objWriter.WriteLine("//                progress.setCancelable(false);")
            objWriter.WriteLine("//                switch (lastMenuActionClicked){")
            objWriter.WriteLine("//                    case R.id.action_validate :")
            objWriter.WriteLine("//                        progress.setMessage(getString(R.string.lb_questionnaire_validate_title));")
            objWriter.WriteLine("//                        break;")
            objWriter.WriteLine("//                    case R.id.action_return :")
            objWriter.WriteLine("//                        progress.setMessage(getString(R.string.lb_questionnaire_return_title));")
            objWriter.WriteLine("//                        break;")
            objWriter.WriteLine("//                    case R.id.action_synchronise :")
            objWriter.WriteLine("//                        progress.setMessage(getString(R.string.lb_questionnaire_synchronize_title));")
            objWriter.WriteLine("//                        break;")
            objWriter.WriteLine("//                }")
            objWriter.WriteLine("//")
            objWriter.WriteLine("//                progress.show();")
            objWriter.WriteLine("//")
            objWriter.WriteLine("//                final Thread t = new Thread(){")
            objWriter.WriteLine("//                    @Override")
            objWriter.WriteLine("//                    public void run(){")
            objWriter.WriteLine("//                        CurrentDB.context = context;")
            objWriter.WriteLine("//                        NotificationManager mNM;")
            objWriter.WriteLine("//                        SharedPreferences mgr;")
            objWriter.WriteLine("//                        mNM = (NotificationManager) getSystemService(NOTIFICATION_SERVICE);")
            objWriter.WriteLine("//                        mgr = PreferenceManager.getDefaultSharedPreferences(context);")
            objWriter.WriteLine("//")
            objWriter.WriteLine("//                        switch (lastMenuActionClicked){")
            objWriter.WriteLine("//                            case R.id.action_validate :")
            objWriter.WriteLine("//                                if(DoChangeStatusQuestionnaires(Questionnaire.STATUS_QUESTIONNAIRE.TO_BE_SYNCHRONIZED)){")
            objWriter.WriteLine("//                                    SynchroData.showNotificationSuccess(context, mNM, mgr, R.string.lb_questionnaire_validate_title, getString(R.string.lb_questionnaire_validate_result_msg));")
            objWriter.WriteLine("//                                    BindListView();")
            objWriter.WriteLine("//                                }")
            objWriter.WriteLine("//                                break;")
            objWriter.WriteLine("//                            case R.id.action_return :")
            objWriter.WriteLine("//                                if(DoChangeStatusQuestionnaires(Questionnaire.STATUS_QUESTIONNAIRE.RETOURNE)){")
            objWriter.WriteLine("//                                    SynchroData.showNotificationSuccess(context, mNM, mgr, R.string.lb_questionnaire_return_title, getString(R.string.lb_questionnaire_return_result_msg));")
            objWriter.WriteLine("//                                    BindListView();")
            objWriter.WriteLine("//                                }")
            objWriter.WriteLine("//")
            objWriter.WriteLine("//                                break;")
            objWriter.WriteLine("//                            case R.id.action_synchronise :")
            objWriter.WriteLine("//                                if (ApplicationHelper.isNetworkAvailable(context)) {")
            objWriter.WriteLine("//                                    SynchroData.showNotificationSynchro(context, mNM);")
            objWriter.WriteLine("//                                    if(DoChangeStatusQuestionnaires(Questionnaire.STATUS_QUESTIONNAIRE.SYNCHRONIZED)){")
            objWriter.WriteLine("//                                        SynchroData.showNotificationSuccess(context, mNM, mgr, R.string.lb_questionnaire_synchronize_title, getString(R.string.lb_questionnaire_synchronize_result_msg));")
            objWriter.WriteLine("//                                        BindListView();")
            objWriter.WriteLine("//                                    }")
            objWriter.WriteLine("//                                }")
            objWriter.WriteLine("//                                else{")
            objWriter.WriteLine("//                                   SynchroData.showNotificationError(context, mNM, mgr, R.string.no_network, getString(R.string.no_network_message));")
            objWriter.WriteLine("//                                }")
            objWriter.WriteLine("//")
            objWriter.WriteLine("//                                break;")
            objWriter.WriteLine("//                        }")
            objWriter.WriteLine("//                        progress.dismiss();")
            objWriter.WriteLine("//                        mNM.cancel(SYNCHO);")
            objWriter.WriteLine("//                    }")
            objWriter.WriteLine("//                };")
            objWriter.WriteLine("//                t.start();")
            objWriter.WriteLine("")
            objWriter.WriteLine("}")
            objWriter.WriteLine("});")
            objWriter.WriteLine("")
            objWriter.WriteLine("builder.setNegativeButton(R.string.lb_Non, new DialogInterface.OnClickListener() {")
            objWriter.WriteLine("public void onClick(DialogInterface dialog, int id) {")
            objWriter.WriteLine("    // User cancelled the dialog")
            objWriter.WriteLine("}")
            objWriter.WriteLine("});")
            objWriter.WriteLine("// 4. Set other dialog properties")
            objWriter.WriteLine("builder.setIcon(android.R.drawable.ic_dialog_alert);")
            objWriter.WriteLine("")
            objWriter.WriteLine("// 5. Get the AlertDialog from create()")
            objWriter.WriteLine("AlertDialog dialog = builder.create();")
            objWriter.WriteLine("dialog.show();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("private boolean DoChangeStatusQuestionnaires(Questionnaire.STATUS_QUESTIONNAIRE newStatus) {")
            objWriter.WriteLine("boolean isStatusChanged = false;")
            objWriter.WriteLine("try{")
            objWriter.WriteLine("Adapter adapter = listview_questionnaire.getAdapter();")
            objWriter.WriteLine("")
            objWriter.WriteLine("ArrayList<Long> listQuestToChangeStatus = new ArrayList<Long>();")
            objWriter.WriteLine("boolean atLeastOneChecked = false;")
            objWriter.WriteLine("for (int i = 0; i < listview_questionnaire.getChildCount(); i++) {")
            objWriter.WriteLine("RelativeLayout listViewRow = (RelativeLayout) listview_questionnaire.getChildAt(i);")
            objWriter.WriteLine("CheckBox cbxSelect = (CheckBox) listViewRow.findViewById(R.id.cbxSelect);")
            objWriter.WriteLine("if (cbxSelect.isChecked()) {")
            objWriter.WriteLine("atLeastOneChecked = true;")
            objWriter.WriteLine("TextView tvStatut = (TextView) listViewRow.findViewById(R.id.tvStatut);")
            objWriter.WriteLine("switch (newStatus) {")
            objWriter.WriteLine("case RETOURNE:")
            objWriter.WriteLine("if (tvStatut.getText().toString().equals(GlobalVariables.KEY_QUESTIONNAIRE_TO_BE_SYNCHRONIZED) ||")
            objWriter.WriteLine("tvStatut.getText().toString().equals(GlobalVariables.KEY_QUESTIONNAIRE_SYNCHRONIZED)) {")
            objWriter.WriteLine("TextView tvIdQuestionnaire = (TextView) listViewRow.findViewById(R.id.tvIdQuestionnaire);")
            objWriter.WriteLine("listQuestToChangeStatus.add(Long.parseLong(tvIdQuestionnaire.getText().toString()));")
            objWriter.WriteLine("}")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("case TO_BE_SYNCHRONIZED:")
            objWriter.WriteLine("if (tvStatut.getText().toString().equals(GlobalVariables.KEY_QUESTIONNAIRE_EN_COURS) ||")
            objWriter.WriteLine("tvStatut.getText().toString().equals(GlobalVariables.KEY_QUESTIONNAIRE_RETOURNE)) {")
            objWriter.WriteLine("TextView tvIdQuestionnaire = (TextView) listViewRow.findViewById(R.id.tvIdQuestionnaire);")
            objWriter.WriteLine("listQuestToChangeStatus.add(Long.parseLong(tvIdQuestionnaire.getText().toString()));")
            objWriter.WriteLine("}")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("case SYNCHRONIZED:")
            objWriter.WriteLine("if(tvStatut.getText().toString().equals(GlobalVariables.KEY_QUESTIONNAIRE_TO_BE_SYNCHRONIZED)){")
            objWriter.WriteLine("TextView tvIdQuestionnaire = (TextView) listViewRow.findViewById(R.id.tvIdQuestionnaire);")
            objWriter.WriteLine("listQuestToChangeStatus.add(Long.parseLong(tvIdQuestionnaire.getText().toString()));")
            objWriter.WriteLine("}")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("if (atLeastOneChecked == false) {")
            objWriter.WriteLine("_message = ""Il faut cocher au moins un rapport !"";")
            objWriter.WriteLine("MessageToShow();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("else {")
            objWriter.WriteLine("if (listQuestToChangeStatus.size() > 0) {")
            objWriter.WriteLine("for (int i = 0; i < listQuestToChangeStatus.size(); i++) {")
            objWriter.WriteLine("try {")
            objWriter.WriteLine("if (newStatus.equals(Questionnaire.STATUS_QUESTIONNAIRE.SYNCHRONIZED)) {")
            objWriter.WriteLine("SynchroData.SynchronizeQuestionnaire(context, listQuestToChangeStatus.get(i));")
            objWriter.WriteLine("}")
            objWriter.WriteLine("QuestionnaireHelper.ChangeStatus(listQuestToChangeStatus.get(i), newStatus);")
            objWriter.WriteLine("isStatusChanged = true;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("catch(Exception e){")
            objWriter.WriteLine("if (e != null)")
            objWriter.WriteLine("Log.e(""Error ListQuestionnaireActivity.DoChangeStatusQuestionnaires"", """" + e.getMessage().toString());")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("catch(Exception e){")
            objWriter.WriteLine("    if(e != null)")
            objWriter.WriteLine("Log.e(""Error ListQuestionnaireActivity.DoChangeStatusQuestionnaires"", """" + e.getMessage().toString());")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
            objWriter.WriteLine("return isStatusChanged;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("private void MessageToShow() {")
            objWriter.WriteLine("Toast.makeText(getApplicationContext(), _message, Toast.LENGTH_LONG).show();")
            objWriter.WriteLine("//        AlertDialog.Builder alertbox = new AlertDialog.Builder(ListDetailQuestionCol1Activity.this);")
            objWriter.WriteLine("//        alertbox.setMessage(_message);")
            objWriter.WriteLine("//        alertbox.setNeutralButton(""Ok"", new DialogInterface.OnClickListener() {")
            objWriter.WriteLine("//            public void onClick(DialogInterface arg0, int arg1) {")
            objWriter.WriteLine("//                Toast.makeText(getApplicationContext(), _message  , Toast.LENGTH_LONG).show();")
            objWriter.WriteLine("//            }")
            objWriter.WriteLine("//        });")
            objWriter.WriteLine("//        alertbox.show();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
            objWriter.WriteLine("public class QuestionnaireActionTask extends AsyncTask<Integer, Void, Integer> {")
            objWriter.WriteLine("")
            objWriter.WriteLine("private Context _context;")
            objWriter.WriteLine("private String _msg;")
            objWriter.WriteLine("private SessionManager session;")
            objWriter.WriteLine("")
            objWriter.WriteLine("// Constructor")
            objWriter.WriteLine("public QuestionnaireActionTask(Context context){")
            objWriter.WriteLine("this._context = context;")
            objWriter.WriteLine("session = new SessionManager(context);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("// -- gets called just before thread begins")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("protected void onPreExecute() {")
            objWriter.WriteLine("super.onPreExecute();")
            objWriter.WriteLine("progress.setProgressStyle(ProgressDialog.STYLE_SPINNER);")
            objWriter.WriteLine("progress.setIndeterminate(true);")
            objWriter.WriteLine("progress.setCancelable(false);")
            objWriter.WriteLine("switch (lastMenuActionClicked){")
            objWriter.WriteLine("case R.id.action_validate :")
            objWriter.WriteLine("progress.setMessage(getString(R.string.lb_questionnaire_validate_title));")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("case R.id.action_return :")
            objWriter.WriteLine("progress.setMessage(getString(R.string.lb_questionnaire_return_title));")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("case R.id.action_synchronise :")
            objWriter.WriteLine("progress.setMessage(getString(R.string.lb_questionnaire_synchronize_title));")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("progress.show();")
            objWriter.WriteLine("progress.setProgress(20);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
            objWriter.WriteLine("// Do the long-running work in here")
            objWriter.WriteLine("protected Integer doInBackground(Integer... params) {")
            objWriter.WriteLine("int _result = 0;")
            objWriter.WriteLine("try {")
            objWriter.WriteLine("int clickedMenu = params[0];")
            objWriter.WriteLine("CurrentDB.context = context;")
            objWriter.WriteLine("switch (clickedMenu){")
            objWriter.WriteLine("case R.id.action_validate :")
            objWriter.WriteLine("if(DoChangeStatusQuestionnaires(Questionnaire.STATUS_QUESTIONNAIRE.TO_BE_SYNCHRONIZED)){")
            objWriter.WriteLine("_result = 1;")
            objWriter.WriteLine("_msg = getString(R.string.lb_questionnaire_validate_result_msg);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("case R.id.action_return :")
            objWriter.WriteLine("    if(DoChangeStatusQuestionnaires(Questionnaire.STATUS_QUESTIONNAIRE.RETOURNE)){")
            objWriter.WriteLine("_result = 1;")
            objWriter.WriteLine("                            _msg = getString(R.string.lb_questionnaire_return_result_msg);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("   break;")
            objWriter.WriteLine("case R.id.action_synchronise :")
            objWriter.WriteLine("if (ApplicationHelper.isNetworkAvailable(context)) {")
            objWriter.WriteLine("if(DoChangeStatusQuestionnaires(Questionnaire.STATUS_QUESTIONNAIRE.SYNCHRONIZED)){")
            objWriter.WriteLine("_result = 1;")
            objWriter.WriteLine("_msg = getString(R.string.lb_questionnaire_synchronize_result_msg);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("else{")
            objWriter.WriteLine("_result = -1;")
            objWriter.WriteLine("_msg = getString(R.string.no_network_message);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("}")
            objWriter.WriteLine("catch (Exception e) {")
            objWriter.WriteLine("if (e != null) {")
            objWriter.WriteLine("e.printStackTrace();")
            objWriter.WriteLine("_result = -1;")
            objWriter.WriteLine("_msg = ""Erreur : "" + e.getMessage();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("return _result;")
            objWriter.WriteLine("}")

            objWriter.WriteLine("        protected void onPostExecute(Integer result) {")
            objWriter.WriteLine("switch (result) {")
            objWriter.WriteLine("case 1:")
            objWriter.WriteLine("BindListView();")
            objWriter.WriteLine("break;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("Toast.makeText(_context, _msg, Toast.LENGTH_LONG).show();")
            objWriter.WriteLine("progress.dismiss();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
            With objWriter
                .WriteLine("  public static final String " & nomSimple.ToUpper & "_TABLE = ""tbl_" & nomSimple.ToLower & """;")
                .WriteLine("  public static final String " & nomSimple.ToUpper & "_ID = """ & nomSimple.ToLower & "_id"";")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("  public static final String " & nomSimple.ToUpper & "_ID_" & cols(i).ToUpper & " = """ & nomSimple.ToLower & "_id_" & cols(i).ToLower & """;")
                    Next
                Catch
                End Try
            End With

            With objWriter
                .WriteLine("  public static final String CREATE_TABLE_" & nomSimple.ToUpper & " = ""create table +")
                .WriteLine("DBConstants." & nomSimple.ToUpper & "_TABLE + "" ("" +")
                .WriteLine("DBConstants.KEY_ID + "" integer primary key autoincrement, "" +")
                .WriteLine("DBConstants." & nomSimple.ToUpper & "_ID + "" integer not null, "" +")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & "  "" " & types(i) & " not null, "" +")
                    Next
                    .WriteLine(" DBConstants.MAC_TABLETTE_CREATED + "" text not null, "" +")
                    .WriteLine(" DBConstants.MAC_TABLETTE_MODIFIED + "" text, "" +")
                    .WriteLine(" DBConstants.DATE_CREATED + "" text not null, "" +")
                    .WriteLine(" DBConstants.DATE_MODIF + "" text, "" +")
                    .WriteLine(" DBConstants.DATE_NAME + "" long);"";")
                Catch
                End Try
            End With

            objWriter.WriteLine(" db.execSQL(DBScript.CREATE_TABLE_" & nomSimple.ToUpper & ");")
            objWriter.WriteLine(" Log.v(""MyDBhelper onCreate"", ""Created Table "" + DBConstants." & nomSimple.ToUpper & "_TABLE);")

            objWriter.WriteLine()
            objWriter.WriteLine()
            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateAndroidListViewLayout(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_projectName As TextBox)
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
            Dim path As String = txt_PathGenerate_Script & "list_" & nomSimple.ToLower & "_layout.xml"
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


            cols.Add("localId")
            cols.Add("isSync")
            types.Add("long")
            types.Add("boolean")
            initialtypes.Add("Byte")
            initialtypes.Add("nvarchar")


            objWriter.WriteLine("<?xml version=""1.0"" encoding=""utf-8""?>")
            objWriter.WriteLine("<LinearLayout xmlns:android=""http://schemas.android.com/apk/res/android""")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""fill_parent""")
            objWriter.WriteLine("android: paddingLeft = ""5dp""")
            objWriter.WriteLine("android: paddingTop = ""5dp""")
            objWriter.WriteLine("android: paddingRight = ""5dp""")
            objWriter.WriteLine("android: paddingBottom = ""5dp""")
            objWriter.WriteLine("style=""@style/layout_vertical"">")
            objWriter.WriteLine("<LinearLayout")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""match_parent""")
            objWriter.WriteLine("android: Orientation = ""vertical""")
            objWriter.WriteLine("android: background = ""@drawable/border""")
            objWriter.WriteLine(">")
            objWriter.WriteLine("<LinearLayout")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: Orientation = ""horizontal""")
            objWriter.WriteLine(">")
            objWriter.WriteLine("<Spinner")
            objWriter.WriteLine("")
            objWriter.WriteLine("android: id = ""@+id/spinner_Institution""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.40""")
            objWriter.WriteLine("android: prompt = ""@string/label_prompt""")
            objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
            objWriter.WriteLine("/>")
            objWriter.WriteLine("<Spinner")
            objWriter.WriteLine("android: id = ""@+id/spinner_Mois""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.20""")
            objWriter.WriteLine("android: prompt = ""@string/label_prompt""")
            objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
            objWriter.WriteLine("/>")
            objWriter.WriteLine("<Spinner")
            objWriter.WriteLine("android: id = ""@+id/spinner_Annee""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.20""")
            objWriter.WriteLine("android: prompt = ""@string/label_prompt""")
            objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
            objWriter.WriteLine("/>")
            objWriter.WriteLine("<Spinner")
            objWriter.WriteLine("android: id = ""@+id/spinner_Statut""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.20""")
            objWriter.WriteLine("android: prompt = ""@string/label_prompt""")
            objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
            objWriter.WriteLine("/>")
            objWriter.WriteLine("")
            objWriter.WriteLine("</LinearLayout>")
            objWriter.WriteLine("<LinearLayout")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: Orientation = ""horizontal""")
            objWriter.WriteLine("style = ""@style/grid_header_style""")
            objWriter.WriteLine(">")
            objWriter.WriteLine("<CheckBox")
            objWriter.WriteLine("android: layout_width = ""40dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: Padding = ""10dp""")
            objWriter.WriteLine("android:visibility=""invisible""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.25""")
            objWriter.WriteLine("android: text = ""Formulaire""")
            objWriter.WriteLine("style=""@style/grid_header_text_style""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.40""")
            objWriter.WriteLine("android: text = ""Institution""")
            objWriter.WriteLine("style=""@style/grid_header_text_style""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.20""")
            objWriter.WriteLine("android: text = ""Periode""")
            objWriter.WriteLine("style=""@style/grid_header_text_style""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.15""")
            objWriter.WriteLine("android: text = ""Statut""")
            objWriter.WriteLine("style=""@style/grid_header_text_style""/>")
            objWriter.WriteLine("</LinearLayout>")
            objWriter.WriteLine("<LinearLayout")
            objWriter.WriteLine("android: layout_width = ""match_parent""")
            objWriter.WriteLine("android: layout_height = ""match_parent""")
            objWriter.WriteLine("android: Orientation = ""vertical""")
            objWriter.WriteLine(">")
            objWriter.WriteLine("<ListView")
            objWriter.WriteLine("android: id = ""@+id/listview_questionnaire""")
            objWriter.WriteLine("android: divider = ""#000""")
            objWriter.WriteLine("android: dividerHeight = ""1dp""")
            objWriter.WriteLine("android: cacheColorHint = ""#00000000""")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""0dp""")
            objWriter.WriteLine("android: layout_weight = ""1""")
            objWriter.WriteLine("android: fadingEdge = ""none""")
            objWriter.WriteLine("android:listSelector=""@drawable/list_selector"">")
            objWriter.WriteLine("</ListView>")
            objWriter.WriteLine("")
            objWriter.WriteLine("</LinearLayout>")
            objWriter.WriteLine("</LinearLayout>")
            objWriter.WriteLine("")
            objWriter.WriteLine("<LinearLayout")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""30dp""")
            objWriter.WriteLine("android: Orientation = ""horizontal""")
            objWriter.WriteLine("android: paddingTop = ""5dp""")
            objWriter.WriteLine("android: background = ""@drawable/border""")
            objWriter.WriteLine(">")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/label_connectedUser""")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: textSize = ""10dp""")
            objWriter.WriteLine("android: text = ""Institution :""")
            objWriter.WriteLine("style = ""@style/form_label""")
            objWriter.WriteLine("android: gravity = ""left""")
            objWriter.WriteLine("android: layout_gravity = ""left""")
            objWriter.WriteLine(">")
            objWriter.WriteLine("</TextView>")
            objWriter.WriteLine("</LinearLayout>")
            objWriter.WriteLine("</LinearLayout>")
            objWriter.WriteLine("")
            With objWriter
                .WriteLine("  public static final String " & nomSimple.ToUpper & "_TABLE = ""tbl_" & nomSimple.ToLower & """;")
                .WriteLine("  public static final String " & nomSimple.ToUpper & "_ID = """ & nomSimple.ToLower & "_id"";")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("  public static final String " & nomSimple.ToUpper & "_ID_" & cols(i).ToUpper & " = """ & nomSimple.ToLower & "_id_" & cols(i).ToLower & """;")
                    Next
                Catch
                End Try
            End With

            With objWriter
                .WriteLine("  public static final String CREATE_TABLE_" & nomSimple.ToUpper & " = ""create table +")
                .WriteLine("DBConstants." & nomSimple.ToUpper & "_TABLE + "" ("" +")
                .WriteLine("DBConstants.KEY_ID + "" integer primary key autoincrement, "" +")
                .WriteLine("DBConstants." & nomSimple.ToUpper & "_ID + "" integer not null, "" +")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & "  "" " & types(i) & " not null, "" +")
                    Next
                    .WriteLine(" DBConstants.MAC_TABLETTE_CREATED + "" text not null, "" +")
                    .WriteLine(" DBConstants.MAC_TABLETTE_MODIFIED + "" text, "" +")
                    .WriteLine(" DBConstants.DATE_CREATED + "" text not null, "" +")
                    .WriteLine(" DBConstants.DATE_MODIF + "" text, "" +")
                    .WriteLine(" DBConstants.DATE_NAME + "" long);"";")
                Catch
                End Try
            End With

            objWriter.WriteLine(" db.execSQL(DBScript.CREATE_TABLE_" & nomSimple.ToUpper & ");")
            objWriter.WriteLine(" Log.v(""MyDBhelper onCreate"", ""Created Table "" + DBConstants." & nomSimple.ToUpper & "_TABLE);")

            objWriter.WriteLine()
            objWriter.WriteLine()
            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateAndroidListRowLayout(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_projectName As TextBox)
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
            Dim path As String = txt_PathGenerate_Script & nomSimple & "DBConstantsHelperScript.xml"
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

            objWriter.WriteLine()



            objWriter.WriteLine(" <?xml version=""1.0"" encoding=""utf-8""?>")


            objWriter.WriteLine("<RelativeLayout xmlns:android=""http://schemas.android.com/apk/res/android""")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: background = ""@drawable/list_selector""")
            objWriter.WriteLine("android: Orientation = ""horizontal""")
            objWriter.WriteLine("android:padding=""2dip"" >")
            objWriter.WriteLine("<LinearLayout")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: id = ""@+id/linearlayout_forRow""")
            objWriter.WriteLine("android: Orientation = ""horizontal""")
            objWriter.WriteLine("android: typeface = ""sans""")
            objWriter.WriteLine(">")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/tvIdQuestionnaire""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android:layout_height=""0dp""/>")
            objWriter.WriteLine("<CheckBox")
            objWriter.WriteLine("android: layout_width = ""40dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: id = ""@+id/cbxSelect""")
            objWriter.WriteLine("android: focusable = ""false""")
            objWriter.WriteLine("android: focusableInTouchMode = ""false""")
            objWriter.WriteLine("/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/tvLibelleFormulaire""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.25""")
            objWriter.WriteLine("android: text = ""libelle formulaire""")
            objWriter.WriteLine("android:textColor=""#000000""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/tvIdFormulaire""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android:layout_height=""0dp""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/tvLibelleInstitution""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.40""")
            objWriter.WriteLine("android: text = ""libelle institution""")
            objWriter.WriteLine("android:textColor=""#000000""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/tvIdInstitution""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android:layout_height=""0dp""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/tvPeriode""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.20""")
            objWriter.WriteLine("android: text = ""Periode""")
            objWriter.WriteLine("android:textColor=""#000000""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/tvMois""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android:layout_height=""0dp""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/tvAnnee""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android:layout_height=""0dp""/>")
            objWriter.WriteLine("<TextView")
            objWriter.WriteLine("android: id = ""@+id/tvStatut""")
            objWriter.WriteLine("android: layout_width = ""0dp""")
            objWriter.WriteLine("android: layout_height = ""wrap_content""")
            objWriter.WriteLine("android: layout_weight = ""0.15""")
            objWriter.WriteLine("android: text = ""Statut""")
            objWriter.WriteLine("android:textColor=""#000000""/>")
            objWriter.WriteLine("</LinearLayout>")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
            objWriter.WriteLine("</RelativeLayout>")
            objWriter.WriteLine("")
            With objWriter
                .WriteLine("  public static final String " & nomSimple.ToUpper & "_TABLE = ""tbl_" & nomSimple.ToLower & """;")
                .WriteLine("  public static final String " & nomSimple.ToUpper & "_ID = """ & nomSimple.ToLower & "_id"";")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("  public static final String " & nomSimple.ToUpper & "_ID_" & cols(i).ToUpper & " = """ & nomSimple.ToLower & "_id_" & cols(i).ToLower & """;")
                    Next
                Catch
                End Try
            End With

            With objWriter
                .WriteLine("  public static final String CREATE_TABLE_" & nomSimple.ToUpper & " = ""create table +")
                .WriteLine("DBConstants." & nomSimple.ToUpper & "_TABLE + "" ("" +")
                .WriteLine("DBConstants.KEY_ID + "" integer primary key autoincrement, "" +")
                .WriteLine("DBConstants." & nomSimple.ToUpper & "_ID + "" integer not null, "" +")
                Try
                    For i As Int32 = 1 To cols.Count - 1
                        .WriteLine("DBConstants." & nomSimple.ToUpper & "_" & cols(i).ToUpper & "  "" " & types(i) & " not null, "" +")
                    Next
                    .WriteLine(" DBConstants.MAC_TABLETTE_CREATED + "" text not null, "" +")
                    .WriteLine(" DBConstants.MAC_TABLETTE_MODIFIED + "" text, "" +")
                    .WriteLine(" DBConstants.DATE_CREATED + "" text not null, "" +")
                    .WriteLine(" DBConstants.DATE_MODIF + "" text, "" +")
                    .WriteLine(" DBConstants.DATE_NAME + "" long);"";")
                Catch
                End Try
            End With

            objWriter.WriteLine(" db.execSQL(DBScript.CREATE_TABLE_" & nomSimple.ToUpper & ");")
            objWriter.WriteLine(" Log.v(""MyDBhelper onCreate"", ""Created Table "" + DBConstants." & nomSimple.ToUpper & "_TABLE);")

            objWriter.WriteLine()
            objWriter.WriteLine()
            objWriter.WriteLine(_end)
            objWriter.WriteLine()
            objWriter.Close()
        End Sub

        Public Shared Sub CreateAndroidFormLayout(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_projectName As TextBox)
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
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\layout\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\layout\")
            Dim path As String = txt_PathGenerate_Script & "add_edit_" & nomSimple.ToLower & "_layout.xml"
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

            objWriter.WriteLine()

            objWriter.WriteLine("<?xml version=""1.0"" encoding=""utf-8""?>")

            objWriter.WriteLine("<LinearLayout xmlns:android=""http://schemas.android.com/apk/res/android""")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""fill_parent""")
            objWriter.WriteLine("android: paddingLeft = ""5dp""")
            objWriter.WriteLine("android: paddingTop = ""5dp""")
            objWriter.WriteLine("android: paddingRight = ""5dp""")
            objWriter.WriteLine("android: paddingBottom = ""5dp""")
            objWriter.WriteLine("style=""@style/layout_vertical"">")

            objWriter.WriteLine("<LinearLayout")
            objWriter.WriteLine("android: layout_width = ""fill_parent""")
            objWriter.WriteLine("android: layout_height = ""fill_parent""")
            objWriter.WriteLine("android: Orientation = ""vertical""")
            objWriter.WriteLine("android: background = ""@drawable/border""")
            objWriter.WriteLine(">")


            Dim countColumn As Integer = 0
            Dim pourcentagevalue As Decimal = 100 / (_table.ListofColumn.Count - 4)
            Dim pourcentage As String = pourcentagevalue.ToString + "%"
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                        Dim textForcombo As String = ""
                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                        Dim countcolumnref As Long
                        For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                            If column_fore.Type.VbName = "String" Then
                                textForcombo = column_fore.Name
                                Exit For
                            End If

                        Next

                        objWriter.WriteLine("  <LinearLayout")
                        objWriter.WriteLine("android: layout_width = ""match_parent""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: Orientation = ""horizontal""")
                        objWriter.WriteLine("android: paddingTop = ""15dp""")
                        objWriter.WriteLine(">")
                        objWriter.WriteLine("<TextView")
                        objWriter.WriteLine("android: id = ""@+id/label_" & reftablename & """")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_weight = ""0.4""")
                        objWriter.WriteLine("android: layout_marginLeft = ""5dp""")
                        objWriter.WriteLine("android: textSize = ""20dp""")
                        objWriter.WriteLine("android: text = """ & reftablename & ":""")
                        objWriter.WriteLine("style = ""@style/form_label""")
                        objWriter.WriteLine("android: gravity = ""left""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine(">")
                        objWriter.WriteLine("</TextView>")

                        objWriter.WriteLine("<Spinner")

                        objWriter.WriteLine("android: id = ""@+id/spinner_" & reftablename & """")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_weight = ""0.6""")
                        objWriter.WriteLine("android: prompt = ""@string/label_prompt""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine("/>")


                        objWriter.WriteLine("  </LinearLayout>")



                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("  <LinearLayout")
                        objWriter.WriteLine("android: layout_width = ""match_parent""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: Orientation = ""horizontal""")
                        objWriter.WriteLine("android: paddingTop = ""15dp""")
                        objWriter.WriteLine(">")

                        objWriter.WriteLine("<TextView")
                        objWriter.WriteLine("android: id = ""@+id/label_" & column.Name & """")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_weight = ""0.4""")
                        objWriter.WriteLine("android: layout_marginLeft = ""5dp""")
                        objWriter.WriteLine("android: textSize = ""20dp""")
                        objWriter.WriteLine("android: text = """ & column.Name & ":""")
                        objWriter.WriteLine("style = ""@style/form_label""")
                        objWriter.WriteLine("android: gravity = ""left""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine(">")
                        objWriter.WriteLine("</TextView>")



                        objWriter.WriteLine("<EditText")
                        objWriter.WriteLine("android: id = ""@+id/editText_" & column.Name & """")
                        objWriter.WriteLine("android: layout_weight = ""0.6""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine(">")
                        objWriter.WriteLine("</EditText>")

                        objWriter.WriteLine("  </LinearLayout>")

                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then

                        objWriter.WriteLine("  <LinearLayout")
                        objWriter.WriteLine("android: layout_width = ""match_parent""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: Orientation = ""horizontal""")
                        objWriter.WriteLine("android: paddingTop = ""15dp""")
                        objWriter.WriteLine(">")

                        objWriter.WriteLine("<TextView")
                        objWriter.WriteLine("android: id = ""@+id/label_" & column.Name & """")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_weight = ""0.4""")
                        objWriter.WriteLine("android: layout_marginLeft = ""5dp""")
                        objWriter.WriteLine("android: textSize = ""20dp""")
                        objWriter.WriteLine("android: text = """ & column.Name & ":""")
                        objWriter.WriteLine("style = ""@style/form_label""")
                        objWriter.WriteLine("android: gravity = ""left""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine(">")
                        objWriter.WriteLine("</TextView>")


                        objWriter.WriteLine("<EditText")
                        objWriter.WriteLine("android: id = ""@+id/editText_" & column.Name & """")
                        objWriter.WriteLine("android: layout_weight = ""0.6""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine("android:inputType=""numberDecimal""")
                        objWriter.WriteLine(">")
                        objWriter.WriteLine("</EditText>")

                        objWriter.WriteLine("  </LinearLayout>")

                    ElseIf column.TrueSqlServerType = "date" And column.Name <> _table.ListofColumn(0).Name Then

                        objWriter.WriteLine("  <LinearLayout")
                        objWriter.WriteLine("android: layout_width = ""match_parent""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: Orientation = ""horizontal""")
                        objWriter.WriteLine("android: paddingTop = ""15dp""")
                        objWriter.WriteLine(">")

                        objWriter.WriteLine("<TextView")
                        objWriter.WriteLine("android: id = ""@+id/label_" & column.Name & """")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_weight = ""0.4""")
                        objWriter.WriteLine("android: layout_marginLeft = ""5dp""")
                        objWriter.WriteLine("android: textSize = ""20dp""")
                        objWriter.WriteLine("android: text = """ & column.Name & ":""")
                        objWriter.WriteLine("style = ""@style/form_label""")
                        objWriter.WriteLine("android: gravity = ""left""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine(">")
                        objWriter.WriteLine("</TextView>")

                        objWriter.WriteLine(" <Spinner")
                        objWriter.WriteLine("android: id = ""@+id/spinner_Jour" & column.Name & """")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_weight = ""0.20""")
                        objWriter.WriteLine("android: prompt = ""@string/label_prompt""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine("/>")
                        objWriter.WriteLine("<Spinner")
                        objWriter.WriteLine("android: id = ""@+id/spinner_Mois" & column.Name & """")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_weight = ""0.20""")
                        objWriter.WriteLine("android: prompt = ""@string/label_prompt""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine("/>")
                        objWriter.WriteLine("<Spinner")
                        objWriter.WriteLine("android: id = ""@+id/spinner_Annee" & column.Name & """")
                        objWriter.WriteLine("android: layout_width = ""0dp""")
                        objWriter.WriteLine("android: layout_height = ""wrap_content""")
                        objWriter.WriteLine("android: layout_weight = ""0.20""")
                        objWriter.WriteLine("android: prompt = ""@string/label_prompt""")
                        objWriter.WriteLine("android: layout_gravity = ""center_vertical""")
                        objWriter.WriteLine("/>")


                        objWriter.WriteLine("  </LinearLayout>")

                    End If
                End If
                countColumn = countColumn + 1

            Next


            objWriter.WriteLine(" </LinearLayout>")
            objWriter.WriteLine("</LinearLayout>")

            objWriter.Close()
        End Sub

        Public Shared Sub CreateAndroidFormActivity(ByVal name As String, ByRef txt_PathGenerate_ScriptFile As TextBox, ByRef ListBox_NameSpace As ListBox, ByRef txt_projectName As TextBox)
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
            Dim txt_PathGenerate_Script As String = IIf(txt_PathGenerate_ScriptFile.Text.Trim <> "", txt_PathGenerate_ScriptFile.Text.Trim & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\activity\", Application.StartupPath & "\SCRIPT\GENERIC_12\" & _systeme.currentDatabase.Name & "\Android\activity\")
            Dim path As String = txt_PathGenerate_Script & "AddEdit" & nomSimple & "Activity.java"
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

            objWriter.WriteLine()

            objWriter.WriteLine("package ht.agrotracking.mobile.activity;")

            objWriter.WriteLine("import android.app.ActionBar;")
            objWriter.WriteLine("import android.app.Activity;")
            objWriter.WriteLine("import android.app.AlertDialog;")
            objWriter.WriteLine("import android.app.ProgressDialog;")
            objWriter.WriteLine("import android.content.Context;")
            objWriter.WriteLine("import android.content.DialogInterface;")
            objWriter.WriteLine("import android.content.Intent;")
            objWriter.WriteLine("import android.net.wifi.WifiInfo;")
            objWriter.WriteLine("import android.net.wifi.WifiManager;")
            objWriter.WriteLine("import android.os.Bundle;")
            objWriter.WriteLine("import android.util.Log;")
            objWriter.WriteLine("import android.view.*;")
            objWriter.WriteLine("import android.widget.*;")
            objWriter.WriteLine("import ht.agrotracking.mobile.global.Utils;")
            objWriter.WriteLine("import ht.agrotracking.mobile.helper.*;")
            objWriter.WriteLine("import ht.agrotracking.mobile.model.*;")

            objWriter.WriteLine("import java.text.SimpleDateFormat;")
            objWriter.WriteLine("import java.util.ArrayList;")
            objWriter.WriteLine("import java.util.Date;")





            objWriter.WriteLine("public class AddEdit" & nomSimple & "Activity extends Activity {")
            objWriter.WriteLine("Context context;")
            objWriter.WriteLine("String _message;")
            objWriter.WriteLine("  ArrayList<" & "Jour> list" & "Jour" & ";")
            objWriter.WriteLine("  ArrayList<" & "Mois> list" & "Mois" & ";")
            objWriter.WriteLine("  ArrayList<" & "Annee> list" & "Annee" & ";")


            Dim countColumn As Integer = 0
            Dim pourcentagevalue As Decimal = 100 / (_table.ListofColumn.Count - 4)
            Dim pourcentage As String = pourcentagevalue.ToString + "%"
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                        Dim textForcombo As String = ""
                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                        Dim countcolumnref As Long
                        For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                            If column_fore.Type.VbName = "String" Then
                                textForcombo = column_fore.Name
                                Exit For
                            End If

                        Next

                        objWriter.WriteLine("  ArrayList<" & reftablename & "> list" & reftablename & ";")
                        objWriter.WriteLine("  Spinner spinner_" & reftablename & ";")
                        objWriter.WriteLine(" lont id_" & reftablename & " = 0; ")


                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name Then

                        objWriter.WriteLine("EditText editText_" & column.Name & "")


                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.TrueSqlServerType <> "datetime" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then

                        objWriter.WriteLine("EditText editText_" & column.Name & "")

                    ElseIf column.TrueSqlServerType = "date" And column.Name <> _table.ListofColumn(0).Name Then


                        objWriter.WriteLine("  Spinner spinner_" & "Jour" & column.Name & ";")
                        objWriter.WriteLine(" lont id_" & "Jour" & column.Name & " = 0; ")



                        objWriter.WriteLine("  Spinner spinner_" & "Mois" & column.Name & ";")
                        objWriter.WriteLine(" lont id_" & "Mois" & column.Name & " = 0; ")



                        objWriter.WriteLine("  Spinner spinner_" & "Annee" & column.Name & ";")
                        objWriter.WriteLine(" lont " & "Annee" & column.Name & " = 0; ")


                    End If
                End If
                countColumn = countColumn + 1
            Next




            objWriter.WriteLine("private ProgressDialog progress;")
            objWriter.WriteLine("//    private SessionManager session;")
            objWriter.WriteLine("//    private UserModel currentUser;")
            objWriter.WriteLine("LayoutInflater inflater;")
            objWriter.WriteLine("private ActionBar actionBar;")

            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public void onCreate(Bundle savedInstanceState) {")
            objWriter.WriteLine("super.onCreate(savedInstanceState);")
            objWriter.WriteLine("getWindow().requestFeature(Window.FEATURE_ACTION_BAR);")
            objWriter.WriteLine("setContentView(R.layout.add_edit_commande_layout);")
            objWriter.WriteLine("context = this;")
            objWriter.WriteLine("progress = new ProgressDialog(this);")
            objWriter.WriteLine("Intent i = getIntent();")


            objWriter.WriteLine(" inflater = (LayoutInflater) this.getSystemService(Context.LAYOUT_INFLATER_SERVICE);")

            objWriter.WriteLine("//        session = new SessionManager(this);")

            objWriter.WriteLine("//        currentUser =   session.getUserDetails();")
            objWriter.WriteLine("//        Utils.PopulateMoisData();")
            objWriter.WriteLine("//        Utils.PopulateAnneeData();")


            objWriter.WriteLine("try {")
            objWriter.WriteLine("Utils.PopulateAnneeData();")
            objWriter.WriteLine("Utils.PopulateMoisData();")

            objWriter.WriteLine("listMois = MoisHelper.SearchAll();")
            objWriter.WriteLine("listAnnee = AnneeHelper.SearchAll();")

            countColumn = 0
            pourcentagevalue = 100 / (_table.ListofColumn.Count - 4)
            pourcentage = pourcentagevalue.ToString + "%"
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                        Dim textForcombo As String = ""
                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                        Dim countcolumnref As Long
                        For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                            If column_fore.Type.VbName = "String" Then
                                textForcombo = column_fore.Name
                                Exit For
                            End If
                        Next
                        objWriter.WriteLine("list" & reftablename & " = " & reftablename & "Helper.SearchAll();")
                        objWriter.WriteLine("  spinner_" & reftablename & " = (Spinner)findViewById(R.id.spinner_" & reftablename & ");")
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine(" editText_" & column.Name & " = (EditText) findViewById(R.id.editText_" & column.Name & ");")
                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then
                        objWriter.WriteLine(" editText_" & column.Name & " = (EditText) findViewById(R.id.editText_" & column.Name & ");")
                    ElseIf column.TrueSqlServerType = "date" And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("  spinner_" & column.Name & " = (Spinner)findViewById(R.id.spinner_" & column.Name & ");")
                    End If
                End If
                countColumn = countColumn + 1
            Next


            countColumn = 0
            pourcentagevalue = 100 / (_table.ListofColumn.Count - 4)
            pourcentage = pourcentagevalue.ToString + "%"
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                        Dim textForcombo As String = ""
                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                        Dim countcolumnref As Long
                        For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                            If column_fore.Type.VbName = "String" Then
                                textForcombo = column_fore.Name
                                Exit For
                            End If
                        Next



                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name Then

                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then

                    ElseIf column.TrueSqlServerType = "date" And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("            spinner_" & column.Name & ".setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {")

                        objWriter.WriteLine("@Override")
                        objWriter.WriteLine("public void onItemSelected(AdapterView<?> adapterView, View view,")
                        objWriter.WriteLine("      int position, long id) {")
                        objWriter.WriteLine("id_" & column.Name & " = position ;")

                        objWriter.WriteLine("}")

                        objWriter.WriteLine("@Override")
                        objWriter.WriteLine("public void onNothingSelected(AdapterView<?> adapter) {")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("});")

                        objWriter.WriteLine("            spinner_" & column.Name & ".setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {")
                        objWriter.WriteLine("@Override")
                        objWriter.WriteLine("public void onItemSelected(AdapterView<?> adapterView, View view,")
                        objWriter.WriteLine("      int position, long id) {")
                        objWriter.WriteLine("id_" & column.Name & " = position ;")

                        objWriter.WriteLine("}")

                        objWriter.WriteLine("@Override")
                        objWriter.WriteLine("public void onNothingSelected(AdapterView<?> adapter) {")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("});")


                        objWriter.WriteLine("ArrayAdapter<CharSequence> adapter = ArrayAdapter.createFromResource(")
                        objWriter.WriteLine("this, R.array.jour_array, android.R.layout.simple_spinner_item);")
                        objWriter.WriteLine("adapter.setDropDownViewResource(android.R.layout.select_dialog_singlechoice);")
                        objWriter.WriteLine("spinner_" & column.Name & ".setAdapter(adapter);")



                        objWriter.WriteLine("if (listMois != null) {")
                        objWriter.WriteLine("Mois m = new Mois();")
                        objWriter.WriteLine("m.setMois(""Mois"");")
                        objWriter.WriteLine("listMois.add(0, m);")
                        objWriter.WriteLine("final ArrayAdapter<Mois> adapterMois = new ArrayAdapter<Mois>(AddEditCommandeActivity.this,")
                        objWriter.WriteLine("android.R.layout.simple_spinner_item,")
                        objWriter.WriteLine("listMois);")
                        objWriter.WriteLine("adapterMois.setDropDownViewResource(android.R.layout.select_dialog_singlechoice);")
                        objWriter.WriteLine("spinner_Mois" & column.Name & ".setAdapter(adapterMois); // Set the custom adapter to the spinner")
                        objWriter.WriteLine("id_Mois" & column.Name & " = adapterMois.getItem(0).getId_Mois();")
                        objWriter.WriteLine("// You can create an anonymous listener to handle the event when is selected an spinner item")
                        objWriter.WriteLine("spinner_Mois" & column.Name & ".setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {")
                        objWriter.WriteLine("@Override")
                        objWriter.WriteLine("public void onItemSelected(AdapterView<?> adapterView, View view,")
                        objWriter.WriteLine("int position, long id) {")
                        objWriter.WriteLine("id_Mois" & column.Name & " = adapterMois.getItem(position).getId_Mois();")
                        objWriter.WriteLine("")
                        objWriter.WriteLine("")
                        objWriter.WriteLine("}")

                        objWriter.WriteLine("                    @Override")
                        objWriter.WriteLine("public void onNothingSelected(AdapterView<?> adapter) {")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("});")

                        objWriter.WriteLine("                spinner_Mois" & column.Name & ".setAdapter(adapterMois); // Set the custom adapter to the spinner")
                        objWriter.WriteLine("id_Mois" & column.Name & " = adapterMois.getItem(0).getId_Mois();")
                        objWriter.WriteLine("// You can create an anonymous listener to handle the event when is selected an spinner item")
                        objWriter.WriteLine("spinner_Mois" & column.Name & ".setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {")
                        objWriter.WriteLine("@Override")
                        objWriter.WriteLine("public void onItemSelected(AdapterView<?> adapterView, View view,")
                        objWriter.WriteLine("int position, long id) {")
                        objWriter.WriteLine("id_Mois" & column.Name & " = adapterMois.getItem(position).getId_Mois();")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("")
                        objWriter.WriteLine("                    @Override")
                        objWriter.WriteLine("public void onNothingSelected(AdapterView<?> adapter) {")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("});")
                        objWriter.WriteLine("")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("")
                        objWriter.WriteLine("if (listAnnee != null) {")
                        objWriter.WriteLine("Annee a = new Annee();")
                        objWriter.WriteLine("a.setAnnee(""Annee"");")
                        objWriter.WriteLine("listAnnee.add(0, a);")
                        objWriter.WriteLine("final ArrayAdapter<Annee> adapterAnnee = new ArrayAdapter<Annee>(AddEditCommandeActivity.this, android.R.layout.simple_spinner_item, listAnnee);")
                        objWriter.WriteLine("adapterAnnee.setDropDownViewResource(android.R.layout.select_dialog_singlechoice);")
                        objWriter.WriteLine("spinner_Annee" & column.Name & ".setAdapter(adapterAnnee); // Set the custom adapter to the spinner")
                        objWriter.WriteLine("Annee" & column.Name & " = Long.parseLong(""0"");")
                        objWriter.WriteLine("// You can create an anonymous listener to handle the event when is selected an spinner item")
                        objWriter.WriteLine("spinner_Annee" & column.Name & ".setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {")
                        objWriter.WriteLine("@Override")
                        objWriter.WriteLine("public void onItemSelected(AdapterView<?> adapterView, View view,")
                        objWriter.WriteLine("int position, long id) {")
                        objWriter.WriteLine("if (adapterAnnee.getItem(position).getAnnee() == ""Annee"") {")
                        objWriter.WriteLine("Annee" & column.Name & " = Long.parseLong(""0"");")
                        objWriter.WriteLine("} else {")
                        objWriter.WriteLine("Annee" & column.Name & " = Long.parseLong(adapterAnnee.getItem(position).getAnnee());")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("")
                        objWriter.WriteLine("@Override")
                        objWriter.WriteLine("public void onNothingSelected(AdapterView<?> adapter) {")
                        objWriter.WriteLine("}")
                        objWriter.WriteLine("});")
                        objWriter.WriteLine("")
                        objWriter.WriteLine("")
                        objWriter.WriteLine("}")



                    End If
                End If
                countColumn = countColumn + 1
            Next


            objWriter.WriteLine(" ActionBarCall();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("catch (Exception e){")
            objWriter.WriteLine("if(e != null){")
            objWriter.WriteLine("Log.e(""AddEditQuestionnaireActivity.onCreate"", """" + e.getMessage().toString());")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")


            objWriter.WriteLine("private void ActionBarCall(){")
            objWriter.WriteLine("actionBar = getActionBar();")
            objWriter.WriteLine("// Hide the action bar title")
            objWriter.WriteLine("// CharSequence activity_title_addeditquestionnaireactivity = getText(R.string.activity_title_addeditquestionnaireactivity);")
            objWriter.WriteLine("actionBar.setTitle(""Enregistrer un " & nomSimple & """);")
            objWriter.WriteLine("actionBar.setDisplayShowTitleEnabled(true);")
            objWriter.WriteLine("// Enabling Spinner dropdown navigation")
            objWriter.WriteLine("actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_STANDARD);")
            objWriter.WriteLine("}")

            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public boolean onCreateOptionsMenu(Menu menu) {")
            objWriter.WriteLine("MenuInflater inflater = getMenuInflater();")
            objWriter.WriteLine("inflater.inflate(R.menu.menu_save_and_cancel, menu);")

            objWriter.WriteLine("return super.onCreateOptionsMenu(menu);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("@Override")
            objWriter.WriteLine("public boolean onOptionsItemSelected(MenuItem item) {")
            objWriter.WriteLine("// Take appropriate action for each action item click")
            objWriter.WriteLine("switch (item.getItemId()) {")
            objWriter.WriteLine("case android.R.id.home:")
            objWriter.WriteLine("case R.id.action_cancel:")
            objWriter.WriteLine("ApplicationHelper.SafelyNavigateUpTo(this);")
            objWriter.WriteLine("return true;")
            objWriter.WriteLine("case R.id.action_save:")
            objWriter.WriteLine("if(Save" & nomSimple & "()){")
            objWriter.WriteLine("_message = """ & nomSimple & " enregistrée avec succès !"";")
            objWriter.WriteLine("MessageToShow();")
            objWriter.WriteLine("ApplicationHelper.SafelyNavigateUpTo(this);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("return true;")

            objWriter.WriteLine("            case R.id.action_addClient:")
            objWriter.WriteLine("Intent i = new Intent ();")
            objWriter.WriteLine("i.setClass(AddEdit" & nomSimple & "Activity.this,AddEditClientActivity.class);")
            objWriter.WriteLine("startActivity(i);")
            objWriter.WriteLine("return true;")

            objWriter.WriteLine("            case R.id.action_disconnect:")
            objWriter.WriteLine("//                session = new SessionManager(this);")
            objWriter.WriteLine("//                session.logoutUser();")
            objWriter.WriteLine("finish();")
            objWriter.WriteLine("return true;")


            objWriter.WriteLine("            default:")
            objWriter.WriteLine("return super.onOptionsItemSelected(item);")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")


            objWriter.WriteLine("    private boolean Save" & nomSimple & "(){")
            objWriter.WriteLine("boolean isSaved = false;")
            objWriter.WriteLine("try{")
            objWriter.WriteLine("_message = """";")



            countColumn = 0
            pourcentagevalue = 100 / (_table.ListofColumn.Count - 4)
            pourcentage = pourcentagevalue.ToString + "%"
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                        Dim textForcombo As String = ""
                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                        Dim countcolumnref As Long
                        For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                            If column_fore.Type.VbName = "String" Then
                                textForcombo = column_fore.Name
                                Exit For
                            End If
                        Next

                        objWriter.WriteLine("           if(id_" & columnName & " == 0){")
                        objWriter.WriteLine("               _message = ""Il faut renseigner le " & reftablename & " !"";")
                        objWriter.WriteLine("            }")


                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name Then


                        objWriter.WriteLine("if (editText_" & column.Name & ".getText().toString().trim().equalsIgnoreCase("""") ){")
                        objWriter.WriteLine("_message = ""Il faut renseigner la " & column.Name & " !"";")
                        objWriter.WriteLine("}")

                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name And column.TrueSqlServerType = "Decimal" Then
                        objWriter.WriteLine("if (editText_" & column.Name & ".getText().toString().trim().equalsIgnoreCase("""") ){")
                        objWriter.WriteLine("_message = ""Il faut renseigner la " & column.Name & " !"";")
                        objWriter.WriteLine("}")
                    ElseIf column.TrueSqlServerType = "date" And column.Name <> _table.ListofColumn(0).Name Then


                        objWriter.WriteLine("            if (id_Jour" & column.Name & " == 0 || id_Mois" & column.Name & " ==0 || Annee" & column.Name & " ==0 ){")
                        objWriter.WriteLine("_message = ""il faut renseigner la date de livraison"";")
                        objWriter.WriteLine("}")
                    End If
                End If
                countColumn = countColumn + 1
            Next





            objWriter.WriteLine("")
            objWriter.WriteLine("if (_message.equalsIgnoreCase("""")){")
            objWriter.WriteLine("WifiManager wifiManager = (WifiManager) getSystemService(Context.WIFI_SERVICE);")
            objWriter.WriteLine("WifiInfo wInfo = wifiManager.getConnectionInfo();")
            objWriter.WriteLine("String macAddress = wInfo.getMacAddress();")
            objWriter.WriteLine("")
            objWriter.WriteLine("SimpleDateFormat sdf = new SimpleDateFormat(""yyyy-MM-dd"");")
            objWriter.WriteLine("String currentDateandTime = sdf.format(new Date());")

            objWriter.WriteLine("" & nomSimple & " obj  = new " & nomSimple & "();")

            countColumn = 0
            pourcentagevalue = 100 / (_table.ListofColumn.Count - 4)
            pourcentage = pourcentagevalue.ToString + "%"
            For Each column As Cls_Column In _table.ListofColumn
                Dim listforeignkey As List(Of Cls_Column) = _table.ListofColumnForeignKey
                Dim primary As String = _table.ListofColumn(0).Name
                If countColumn < _table.ListofColumn.Count - 4 Then

                    If _table.LisOfForeingKeyContainsThisColumn(column) And column.Type.SqlServerName <> "Date" And column.Name <> primary Then
                        Dim foreignkey As New Cls_ForeignKey(_table.ID, column.ID)
                        Dim textForcombo As String = ""
                        Dim columnName As String = column.Name.Substring(SqlServerHelper.ForeinKeyPrefix.Length, column.Name.Length - (SqlServerHelper.ForeinKeyPrefix.Length))
                        Dim reftablename As String = foreignkey.RefTable.Name.Substring(4, foreignkey.RefTable.Name.Length - 4)
                        Dim countcolumnref As Long
                        For Each column_fore As Cls_Column In foreignkey.RefTable.ListofColumn
                            If column_fore.Type.VbName = "String" Then
                                textForcombo = column_fore.Name
                                Exit For
                            End If
                        Next

                        objWriter.WriteLine("obj.set" & column.Name & "(" & column.Name & ");")

                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name And column.Type.JavaName = "String" Then

                        objWriter.WriteLine("obj.set" & column.Name & "(editText_" & column.Name & ".getText().toString());")

                    ElseIf Not _table.LisOfForeingKeyContainsThisColumn(column) And column.TrueSqlServerType <> "date" And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("obj.set" & column.Name & "(" & ConvertDBToJavaParsingType(column.TrueSqlServerType) & ".parse" & ConvertDBToJavaParsingType(column.TrueSqlServerType) & "(editText_" & column.Name & ".getText().toString()));")
                    ElseIf column.TrueSqlServerType = "date" And column.Name <> _table.ListofColumn(0).Name Then
                        objWriter.WriteLine("obj.set" & column.Name & "(Annee" & column.Name & " + ""-"" + id_Mois" & column.Name & " + ""-"" + id_Jour" & column.Name & ");")


                    End If
                End If
                countColumn = countColumn + 1
            Next

            objWriter.WriteLine("if(obj.getId() ==0){")
            objWriter.WriteLine("obj.setMacTabletteCreated(macAddress);")
            objWriter.WriteLine("obj.setDateCreated(currentDateandTime);")
            objWriter.WriteLine("obj.setMacTabletteModif("");")
            objWriter.WriteLine("obj.setDateModif("");")
            objWriter.WriteLine("}else{")
            objWriter.WriteLine("obj.setMacTabletteCreated(macAddress);")
            objWriter.WriteLine("obj.setDateModif(currentDateandTime);")
            objWriter.WriteLine("}")


            objWriter.WriteLine("" & nomSimple & "Helper.save(obj);")
            objWriter.WriteLine("")


            objWriter.WriteLine("isSaved = true;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("else{")
            objWriter.WriteLine("MessageToShow();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("catch (Exception e){")
            objWriter.WriteLine("if(e != null){")
            objWriter.WriteLine("_message = e.getMessage();")
            objWriter.WriteLine("MessageToShow();")
            objWriter.WriteLine("Log.e(""AddEdit" & nomSimple & "Activity.Save" & nomSimple & """, """" + e.getMessage().toString());")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("return isSaved;")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("private void MessageToShow (){")
            objWriter.WriteLine("Toast.makeText(getApplicationContext(), _message, Toast.LENGTH_LONG).show();")
            objWriter.WriteLine("AlertDialog.Builder alertbox = new AlertDialog.Builder(AddEditCommandeActivity.this);")
            objWriter.WriteLine("alertbox.setMessage(_message);")
            objWriter.WriteLine("alertbox.setNeutralButton(""Ok"", new DialogInterface.OnClickListener() {")
            objWriter.WriteLine("public void onClick(DialogInterface arg0, int arg1) {")
            objWriter.WriteLine("Toast.makeText(getApplicationContext(), _message  , Toast.LENGTH_LONG).show();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("});")
            objWriter.WriteLine("        alertbox.show();")
            objWriter.WriteLine("}")
            objWriter.WriteLine("}")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
            objWriter.WriteLine("")
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
