//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

	// The world and tile data
	public World world { get; protected set; }
    static bool loadWorld = false;

	// Use this for initialization
	void OnEnable () {
		if(Instance != null) {
			Debug.LogError("There should never be two world controllers.");
		}
		Instance = this;

        if (loadWorld) {
            CreateSavedWorld();
            loadWorld = false;
            
        }
        else {
            CreateEmptyWorld();
        }
        
    }

    void Update() {
        world.Update(Time.deltaTime);
    }
    /// <summary>
    /// Gets the tile at the unity-space coordinates
    /// </summary>
    /// <returns>The tile at world coordinate.</returns>
    /// <param name="coord">Unity World-Space coordinates.</param>
    public Tile GetTileAtWorldCoord(Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);
		
		return world.GetTileAt(x, y);
	}

    public void NewWorld() {
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public void SaveWorld() {
        XmlSerializer serializer = new XmlSerializer( typeof(World) );
        System.IO.TextWriter writer = new System.IO.StringWriter();
        serializer.Serialize(writer, world);
        writer.Close();

        Debug.Log(writer.ToString());

        PlayerPrefs.SetString("SaveData00", writer.ToString());
    }

    public void LoadWorld() {
        loadWorld = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CreateEmptyWorld() {
        // Create a world with Empty tiles
        world = new World(100, 100);

        // Center the Camera
        Camera.main.transform.position = new Vector3(world.Width / 2, world.Height / 2, Camera.main.transform.position.z);
        world.RandomizeTiles();
    }

    void CreateSavedWorld() {
        // Create a world with Empty tiles
        //world = new World(100, 100);

        XmlSerializer serializer = new XmlSerializer(typeof(World));
        System.IO.TextReader reader = new System.IO.StringReader(PlayerPrefs.GetString("SaveData00"));
        world = (World)serializer.Deserialize(reader);
        reader.Close();

        //Debug.Log(writer.ToString());

        // Center the Camera
        Camera.main.transform.position = new Vector3(world.Width / 2, world.Height / 2, Camera.main.transform.position.z);
    }

}
