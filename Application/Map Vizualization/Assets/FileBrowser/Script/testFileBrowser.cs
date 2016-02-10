using UnityEngine;

public class testFileBrowser : MonoBehaviour {
	//skins and textures
	public GUISkin[] skins;
	public Texture2D file,folder,back,drive;

	string[] layoutTypes = {"Type 0","Type 1"};
	//initialize file browser
	FileBrowser fb = new FileBrowser();
	string output = "no file";
	// Use this for initialization
	void Start () {
		//setup file browser style
		fb.guiSkin = skins[0]; //set the starting skin
		//set the various textures
		fb.fileTexture = file;
		fb.directoryTexture = folder;
		fb.backTexture = back;
		fb.driveTexture = drive;
		//show the search bar
		fb.showSearch = true;
		//search recursively (setting recursive search may cause a long delay)
		fb.searchRecursively = true;
	}

	void OnGUI(){
		//draw and display output
		if(fb.draw()){ //true is returned when a file has been selected
			//the output file is a member if the FileInfo class, if cancel was selected the value is null
            string pth = "";

            if (fb.outputFile == null) {
                PlayerPrefs.SetString("path", "");
                Application.LoadLevel("main");
            } else {
                pth = fb.outputFile.ToString();
                PlayerPrefs.SetString("path", pth);
                Debug.Log(pth.Substring(pth.Length - 3));
                if (PlayerPrefs.GetString("type") == pth.Substring(pth.Length - 4)) {
                    Application.LoadLevel("main");
                }
            }          
		}
	}
}
