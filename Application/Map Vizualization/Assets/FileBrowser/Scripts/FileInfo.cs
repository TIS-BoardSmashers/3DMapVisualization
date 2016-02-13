using UnityEngine;
using System;

public class FileInfo
{
    public string path;
    public string name;
    public bool isFile;
    public string extension
    {
        get
        {
            return name.Remove(0, name.LastIndexOf('.'));
        }
    }

    public FileInfo(string path, string name, bool IsFile)
    {
        this.path = path;
        this.name = name;
        isFile = IsFile;
    }
}