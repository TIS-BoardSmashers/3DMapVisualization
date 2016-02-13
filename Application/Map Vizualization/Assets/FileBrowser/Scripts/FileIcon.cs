using UnityEngine;
using System;

[Serializable]
public class FileIcon
{
    public string extension = "";
    public Texture2D texture;

    public FileIcon(string ext, Texture2D tex)
    {
        extension = ext;
        texture = tex;
    }

    public FileIcon() { }
}