using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class FileBrowser : MonoBehaviour
{
    public GUISkin skin;
    public string SelectEventName = "OnFileSelected";
    public string CancelEventName = "OnBrowseCancel";
    public string FileChangedEventName = "OnFileChange";
    public Texture2D upButton;
    public Texture2D homeButton;
    public bool debugEmptyDiskDrive = true;
    public bool draggable = false;
    public Rect dragRect = new Rect(0,0,80,3);
    public bool customRects;

    public bool drawIcons = true;
    public ScaleMode scaleMode;
    public Texture2D folderIcon;
    public Texture2D defaultIcon;
    public FileIcon[] fileIcons;
    public Rect iconsPos = new Rect(1,1,3,5);

    public Rect windowPos = new Rect(10, 10, 80, 80);
    public Rect currentDirTextPos = new Rect(13, 4.5f, 45, 4);
    public Rect fileBoxPos = new Rect(2, 10.5f, 76, 62);
    public Rect scrollViewPos = new Rect(2, 10.5f, 76, 61);
    public Rect upButtonPos = new Rect(59, 4, 5, 5);
    public Rect homeButtonPos = new Rect(65, 4, 5, 5);
    public Rect cancelButtonPos = new Rect(2, 73, 10, 5);
    public Rect okButtonPos = new Rect(68, 73, 10, 5);
    public Rect redLabelPos = new Rect(30, 73, 20, 5);
    public Rect filePos = new Rect(1, 1, 73, 5);

    bool show;
    public bool isShowing
    {
        get
        {
            return show;
        }
    }
    string currentDir = "";
    public string CurrentDirectory
    {
        get
        {
            return currentDir;
        }
    }
    public string windowName = "";
    public string DefaultDirectory = @"C:\";
    public FileInfo currentFile
    {
        get
        {
            return current;
        }
    }
    bool canGoUp;
    Vector2 fileScroll;
    bool canSelect;
    public bool canCancel;
    FileSelectMode mode;
    [SerializeField]
    List<FileInfo> files = new List<FileInfo>();
    FileInfo current;
    int currentIndex = -1;
    MonoBehaviour rt;
    string slash = @"\";
    public string[] searchPattern = new string[0];

    /// <summary>
    /// Show the browser window.
    /// </summary>
    /// <param name="startDirectory">The directory which will be shown at the start.</param>
    /// <param name="returnMessage">The MonoBehaviour which will receive the selected file path.</param>
    /// <param name="selectMode">Browse for Files or Directories.</param>
    /// <param name="CanCancel">Can the user close the window?</param>
    public void Show(string startDirectory, MonoBehaviour returnMessage, FileSelectMode selectMode = FileSelectMode.File, bool CanCancel = true)
    {
        Show(startDirectory, "", returnMessage, selectMode, CanCancel);
    }
    /// <summary>
    /// Show the browser window using the provided search pattern.
    /// </summary>
    /// <param name="startDirectory">The directory which will be shown at the start.</param>
    /// /// <param name="SearchPattern">You can search only one extension at a time. Example: *.mp3 or *.txt</param>
    /// <param name="returnMessage">The MonoBehaviour which will receive the selected file path.</param>
    /// <param name="selectMode">Browse for Files or Directories.</param>
    /// <param name="CanCancel">Can the user close the window?</param>
    public void Show(string startDirectory, string SearchPattern, MonoBehaviour returnMessage, FileSelectMode selectMode = FileSelectMode.File, bool CanCancel = true)
    {
        string[] search = new string[0];
        if (SearchPattern != "")
        {
            search = new string[1] { SearchPattern };
        }
        Show(startDirectory, search, returnMessage, selectMode, CanCancel);
    }
    /// <summary>
    /// Show the browser using the provided multiple search patterns.
    /// </summary>
    /// <param name="startDirectory">The directory which will be shown at the start.</param>
    /// /// <param name="SearchPattern">The search patterns. Example: *.mp3 / *.txt</param>
    /// <param name="returnMessage">The MonoBehaviour which will receive the selected file path.</param>
    /// <param name="selectMode">Browse for Files or Directories.</param>
    /// <param name="CanCancel">Can the user close the window?</param>
    public void Show(string startDirectory, string[] SearchPattern, MonoBehaviour returnMessage, FileSelectMode selectMode = FileSelectMode.File, bool CanCancel = true)
    {
        rt = returnMessage;
        mode = selectMode;
        canCancel = CanCancel;
        searchPattern = SearchPattern;
        if (selectMode == FileSelectMode.File && windowName == "")
        {
            windowName = "File Browser - Select File";
            redLabel = "Select a file.";
        }
        else
        {
            if (windowName == "")
                windowName = "File Browser - Select Folder";
            if (redLabel == "")
                redLabel = "Select a folder.";
        }
        if (startDirectory != "")
        {
            if (DirectoryExsists(startDirectory)) currentDir = startDirectory;
            else currentDir = DefaultDirectory;
            UpdateList(currentDir, SearchPattern);
        }
        else
        {
            UpdateList(DefaultDirectory, SearchPattern);
        }
        show = true;
    }

    void UpdateList(string path,string[] search)
    {
        try
        {
            if (path.Length > 1)
            {

                files = new List<FileInfo>();
                try
                {
                    string[] f = GetFiles(path, search);
                    string[] d = GetDirectories(path);
                    for (int i = 0; i < d.Length; i++)
                    {
                        files.Add(new FileInfo(d[i], d[i].Remove(0, d[i].LastIndexOf(slash) + 1), false));
                    }
                    for (int i = 0; i < f.Length; i++)
                    {
                        files.Add(new FileInfo(f[i], f[i].Remove(0, f[i].LastIndexOf(slash) + 1), true));
                    }
                }
                catch { }
                currentDir = path;
                if (currentDir.EndsWith(slash)) currentDir = currentDir.Remove(currentDir.Length - 1);
            }
            else
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    files = new List<FileInfo>();
                    string[] drives = Directory.GetLogicalDrives();
                    for (int i = 0; i < drives.Length; i++)
                    {
                        files.Add(new FileInfo(drives[i], drives[i], false));
                    }
                    currentDir = "My PC";
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    files = new List<FileInfo>();
                    try
                    {
                        string[] f = GetFiles(path,new string[1] { searchPattern[0] });
                        string[] d = GetDirectories(path);
                        for (int i = 0; i < d.Length; i++)
                        {
                            files.Add(new FileInfo(d[i], d[i].Remove(0, d[i].LastIndexOf(slash) + 1), false));
                        }
                        for (int i = 0; i < f.Length; i++)
                        {
                            files.Add(new FileInfo(f[i], f[i].Remove(0, f[i].LastIndexOf(slash) + 1), true));
                        }
                    }
                    catch { }
                    currentDir = DefaultDirectory;
                } else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
                {
                    files = new List<FileInfo>();
                    try
                    {
                        string[] f = GetFiles(path, new string[1] { searchPattern[0] });
                        string[] d = GetDirectories(path);
                        for (int i = 0; i < d.Length; i++)
                        {
                            files.Add(new FileInfo(d[i], d[i].Remove(0, d[i].LastIndexOf(slash) + 1), false));
                        }
                        for (int i = 0; i < f.Length; i++)
                        {
                            files.Add(new FileInfo(f[i], f[i].Remove(0, f[i].LastIndexOf(slash) + 1), true));
                        }
                    }
                    catch { }
                    currentDir = "/";
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("FileBrowser: Failed to enter directory: " + e.Message);
        }
        canSelect = false;
    }

    public void Hide()
    {
        files = new List<FileInfo>();
        fileScroll = new Vector2(0, 0);
        current = null;
        currentIndex = -1;
        show = false;
    }

    void Select()
    {
        if (rt)
            if (SelectEventName != "") rt.SendMessage(SelectEventName, current);
        canGoUp = false;
        canSelect = false;
        Hide();
    }

    void FileChanged()
    {
        if (rt)
            if (FileChangedEventName != "") rt.SendMessage(FileChangedEventName, current);
    }

    void Cancel()
    {
        if (rt)
            if (CancelEventName != "") rt.SendMessage(CancelEventName);
        Hide();
    }

    void GoUp()
    {
        currentIndex = -1;
        UpdateList(currentDir.Remove(currentDir.LastIndexOf(slash) + 1),searchPattern);
    }

    #region GUI
    void OnGUI()
    {
        if (show)
        {
            GUI.skin = skin;
            windowPos = ToPercentRect(GUI.Window(1, GetRect(windowPos), DrawWindow, windowName));
        }
    }

    public string redLabel = "Select a File";
    void DrawWindow(int id)
    {
        GUI.TextField(GetRect(currentDirTextPos), currentDir);
        if (canGoUp)
        {
            if (GUI.Button(GetRect(upButtonPos), upButton))
            {
                GoUp();
            }
            if (GUI.Button(GetRect(homeButtonPos), homeButton))
            {
                UpdateList(DefaultDirectory,searchPattern);
            }
        }

        GUI.Box(GetRect(fileBoxPos), "");
        if (files != null)
        {
            fileScroll = GUI.BeginScrollView(GetRect(scrollViewPos), fileScroll, GetRect(0, 0, scrollViewPos.width - 4f, filePos.height * files.Count + 2));
            for (int i = 0; i < files.Count; i++)
            {
                GUIStyle style = new GUIStyle();
                if (currentIndex != i) style = skin.GetStyle("normalFile");
                else style = skin.GetStyle("selectedFile");
                if (GUI.Button(GetRect(filePos.x, filePos.y + (filePos.height * i), filePos.width, filePos.height), files[i].name, style))
                {
                    if (files[i].isFile)
                    {
                        current = files[i];
                        FileChanged();
                        currentIndex = i;
                    }
                    else
                    {
                        if (mode != FileSelectMode.File)
                        {
                            if (currentIndex == i)
                            {
                                currentIndex = -1;
                                UpdateList(files[i].path,searchPattern);
                                canGoUp = true;
                            }
                            else
                            {
                                current = files[i];
                                FileChanged();
                                currentIndex = i;
                            }
                        }
                        else
                        {
                            currentIndex = -1;
                            UpdateList(files[i].path,searchPattern);
                            canGoUp = true;
                        }
                    }
                    if (current != null)
                    {
                        if (mode == FileSelectMode.File && current.isFile) canSelect = true;
                        else if (mode == FileSelectMode.Folder && !current.isFile) canSelect = true;
                        else canSelect = false;
                    }
                }
                if (drawIcons)
                {
                    if (i < files.Count)
                    {
                        if (!files[i].isFile)
                        {
                            GUI.DrawTexture(GetRect(iconsPos.x, iconsPos.y + (iconsPos.height * i), iconsPos.width, iconsPos.height), folderIcon, scaleMode);
                        }
                        else
                        {
                            Texture2D draw = defaultIcon;
                            for (int a = 0; a < fileIcons.Length; a++)
                            {
                                if (files[i].extension == fileIcons[a].extension) draw = fileIcons[a].texture;
                            }
                            GUI.DrawTexture(GetRect(iconsPos.x, iconsPos.y + (iconsPos.height * i), iconsPos.width, iconsPos.height), draw, scaleMode);
                        }
                    }
                }
            }
            GUI.EndScrollView(true);
        }

        if (canSelect)
        {
            if (GUI.Button(GetRect(okButtonPos), "OK"))
            {
                Select();
            }
        }
        if (canCancel)
        {
            if (GUI.Button(GetRect(cancelButtonPos), "Cancel"))
            {
                Cancel();
            }
        }
        GUI.Label(GetRect(redLabelPos), redLabel, skin.GetStyle("redLabel"));
        if (draggable) GUI.DragWindow(GetRect(dragRect));
    }

    Rect GetRect(float x, float y, float w, float h)
    {
        return new Rect((Screen.width * x / 100f), (Screen.height * y / 100f), (Screen.width * w / 100f), (Screen.height * h / 100f));
    }
    Rect GetRect(Rect pos)
    {
        return new Rect((Screen.width * pos.x / 100f), (Screen.height * pos.y / 100f), (Screen.width * pos.width / 100f), (Screen.height * pos.height / 100f));
    }
    Rect ToPercentRect(Rect rect)
    {
        return new Rect(rect.x / Screen.width * 100f, rect.y / Screen.height * 100f, rect.width / Screen.width * 100f, rect.height / Screen.height * 100f);
    }
    #endregion

    #region IO
    bool DirectoryExsists(string path)
    {
        return Directory.Exists(path);
    }
    bool FileExsists(string path)
    {
        return File.Exists(path);
    }
    string[] GetFiles(string path, string[] search)
    {
        if (search.Length > 0)
        {
            List<string> files = new List<string>();
            for (int i = 0; i < search.Length; i++)
            {
                files.AddRange(Directory.GetFiles(path, search[i]));
            }
            return files.ToArray();
        }
        else return Directory.GetFiles(path);
    }
    string[] GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }
    #endregion

    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (DefaultDirectory == "") DefaultDirectory = "/storage";
            slash = "/";
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (DefaultDirectory == "")
            {
                files = new List<FileInfo>();
                string[] drives = Directory.GetLogicalDrives();
                for (int i = 0; i < drives.Length; i++)
                {
                    files.Add(new FileInfo(drives[i], drives[i], false));
                }
                currentDir = "My PC";
            }
        } else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (DefaultDirectory == "")
            {
                DefaultDirectory = "/home/";
                currentDir = "Home";
                UpdateList(DefaultDirectory, searchPattern);
            }
            slash = "/";
        } else if (Application.platform == RuntimePlatform.LinuxPlayer)
        {
            if (DefaultDirectory == "")
            {
                DefaultDirectory = "/home/";
                currentDir = "Home";
                UpdateList(DefaultDirectory, searchPattern);
            }
            slash = "/";
        }
    }
}