using UnityEditor;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Make the commenting and creation of scripts more efficient
 * via concurrent patterns and script templates
 */// --------------------------------------------------------

/// <summary>
/// Static class to create the scripts based of templates. To make workflow more efficient
/// </summary>
public static class CreateScriptTemplates
{
    #region PATHS
    
    // General Paths
    private const string templatePath = "Assets/Editor/Custom Default Scripts/Templates";
    private const string contextMenuPath = "Assets/Create/Script Templates";

    // Menu Paths
    private const string commentedScriptMenuPath = contextMenuPath + "/Commented Script";
    private const string commentedScriptableObjectMenuPath = contextMenuPath + "/Scriptable Object";
    private const string emptyClassMenuPath = contextMenuPath + "/Empty Class";

    // Template Paths
    private const string commentedScriptTemplatePath = templatePath + "/CommentedScript.cs.txt";
    private const string scriptableObjectTemplatePath = templatePath + "/ScriptableObject.cs.txt";
    private const string emptyClassTemplatePath = templatePath + "/EmptyClass.cs.txt";

    #endregion

    #region DEFAULT FILE NAMES ON CREATION

    private const string defaultCommentedScriptName = "CommentedScript.cs";
    private const string defaultScriptableObjectName = "ScriptableObject.cs";
    private const string defaultEmptyClassObjectName = "EmptyClass.cs";

    #endregion

    #region PRIORITY SETTINGS

    // NOTE: aight, gave up trying to place the menu option besides create script

    private const int commentedScriptMenuPriority = 18;
    private const int scriptableObjectMenuPriority = 18;
    private const int emptyClassMenuPriority = 18;

    #endregion

    // Function for creating a commented New Script
    [MenuItem(commentedScriptMenuPath, priority = commentedScriptMenuPriority)]
    public static void CreateNewScriptMenuItem()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(commentedScriptTemplatePath, defaultCommentedScriptName);
    }

    // Function for creating a commented Scriptable Object
    [MenuItem(commentedScriptableObjectMenuPath, priority = scriptableObjectMenuPriority)]
    public static void CreateNewScriptableObjectMenuItem()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(scriptableObjectTemplatePath, defaultScriptableObjectName);
    }

    // Function for creating a commented empty class
    [MenuItem(emptyClassMenuPath, priority = emptyClassMenuPriority)]
    public static void CreateNewEmptyClassMenuItem()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(emptyClassTemplatePath, defaultEmptyClassObjectName);
    }
}

