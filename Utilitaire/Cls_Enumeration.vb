Imports System.Text.RegularExpressions

Public Enum TypeServiceRezo509
    ServiceContact = 1
    ServiceGroupeContact = 2
    Service_MasseMailling = 3
    Service_Annonce = 4
    Service_AnnoncePhoto = 5
    Service_OffreEmploi = 6
    Service_AppelOffre = 7
    Service_Entete_PiedEmail = 8
End Enum

Public Enum StatutContact
    ReInscription = 0
    DesinscriptionDefinitivement = 1
    DesinscriptionTemporaire = 2

    ContactAbonnee = 1
    ContactDesabonnee = 2
    ContactCorbeille = 3
    ContactDelete = 4
End Enum


Public Class Cls_Enumeration

    Public Const APP_NAME = "GENERIC V16"
    Public Const APP_VERSION = "16.1"
    Public Const PATH_GENERIC_FOLDER_DEFAULT = "\GENERIC " & APP_VERSION & "\"
    Public Const GENERATE_BY_APP_NAME_ForStore = "/******    REM Generate By [ " & APP_NAME & " ] Application    ******/"


    Public Shared Function GetPath(ByVal _pathGenerate As String) As String
        Dim _PathGenerate_Script As String = IIf(_pathGenerate.Trim <> "" _
                                                        , _pathGenerate.Trim & PATH_GENERIC_FOLDER_DEFAULT _
                                                        , Application.StartupPath & PATH_GENERIC_FOLDER_DEFAULT)

        Return _PathGenerate_Script
    End Function

#Region "PATH MSSQL Server"
    Public Shared Function GetPath_ASP_WebForm(ByVal _pathGenerate As String, ByVal databasename As String) As String
        Dim _PathGenerate_Script As String = IIf(_pathGenerate.Trim <> "" _
                                                        , _pathGenerate.Trim & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\SQLServer_ASP_WebForm\" _
                                                        , Application.StartupPath & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\SQLServer_ASP_WebForm\")

        Return _PathGenerate_Script
    End Function

    Public Shared Function GetPath_VbNet_Class(ByVal _pathGenerate As String, ByVal databasename As String) As String
        Dim _PathGenerate_Script As String = IIf(_pathGenerate.Trim <> "" _
                                                        , _pathGenerate.Trim & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\SQLServer_VbNet_Class\" _
                                                        , Application.StartupPath & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\SQLServer_VbNet_Class\")

        Return _PathGenerate_Script
    End Function

    Public Shared Function GetPath_SQLServer_Script(ByVal _pathGenerate As String, ByVal databasename As String) As String
        Dim _PathGenerate_Script As String = IIf(_pathGenerate.Trim <> "" _
                                                        , _pathGenerate.Trim & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\SQLServer_Script\" _
                                                        , Application.StartupPath & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\SQLServer_Script\")
        Return _PathGenerate_Script
    End Function


#End Region

#Region "PATH MySQL"
    Public Shared Function GetPath_MySQL_ASP_WebForm(ByVal _pathGenerate As String, ByVal databasename As String) As String
        Dim _PathGenerate_Script As String = IIf(_pathGenerate.Trim <> "" _
                                                        , _pathGenerate.Trim & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\MySQL_ASP_WebForm\" _
                                                        , Application.StartupPath & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\MySQL_ASP_WebForm\")

        Return _PathGenerate_Script
    End Function

    Public Shared Function GetPath_MySQL_VbNet_Class(ByVal _pathGenerate As String, ByVal databasename As String) As String
        Dim _PathGenerate_Script As String = IIf(_pathGenerate.Trim <> "" _
                                                        , _pathGenerate.Trim & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\MySQL_VbNet_Class\" _
                                                        , Application.StartupPath & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\MySQL_VbNet_Class\")

        Return _PathGenerate_Script
    End Function

    Public Shared Function GetPath_MySQL_Script(ByVal _pathGenerate As String, ByVal databasename As String) As String
        Dim _PathGenerate_Script As String = IIf(_pathGenerate.Trim <> "" _
                                                        , _pathGenerate.Trim & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\MySQL_Script\" _
                                                        , Application.StartupPath & PATH_GENERIC_FOLDER_DEFAULT & databasename & "\MySQL_Script\")
        Return _PathGenerate_Script
    End Function


#End Region

    ''' <summary>Removes the tags from an HTML document.</summary>
    ''' <param name="htmlText">HTML text to parse.</param>
    ''' <returns>The text of an HTML document without tags.</returns>
    ''' <remarks></remarks>
    ''' 
    Public Shared Function GetTextFromHtml(ByVal htmlText As String) As String
        Dim output As String = Regex.Replace(htmlText, "\<[^\>]+\>", "")
        Return output
    End Function

End Class