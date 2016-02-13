using UnityEngine;
using System.Collections;

/* This example can search for multiple extensions. */

public class example2 : MonoBehaviour
{
    public FileBrowser browser;
    public FileSelectMode mode;
    public string[] searchPatterns;
    bool selected;
    FileInfo sel;
    string path = "";

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 1, 100, 30), "Show"))
        {
            selected = false;
            if (!browser.isShowing) browser.Show(path, searchPatterns, this, mode);
        }
        path = GUI.TextField(new Rect(10, 50, 200, 30), path);
        GUI.Label(new Rect(10, 100, Screen.width - 10, 100), "Persistent data path: " + Application.persistentDataPath);
        if (selected) {
            GUI.Label(new Rect(10, 150, 300, 100), "You have selected: " + sel.name + " Path: " + sel.path);
            PlayerPrefs.SetString("path", sel.path);
            Application.LoadLevel("main");
        }
    }

    // the FileBrowser will send a message to this MonoBehaviour when the user selects a file
    // Set the 'SelectEventName' in the inspector to the name of the function you want to receive the message
    void OnFileSelected(FileInfo info)
    {
        sel = info;
        selected = true;
    }

    void OnFileChange(FileInfo file)
    {
        Debug.Log("File section changed to: " + file.name);
    }

    void OnBrowseCancel()
    {
        Debug.Log("You have cancelled");
        PlayerPrefs.SetString("path", "");
        Application.LoadLevel("main");
    }
}