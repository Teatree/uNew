using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour {

    Dictionary<Character, GameObject> characterGameObjectMap;

    Dictionary<string, Sprite> characterSprites;

    World world {
        get { return WorldController.Instance.world; }
    }

    // Use this for initialization
    void Start () {
        LoadSprites();

        characterGameObjectMap = new Dictionary<Character, GameObject>();

        world.RegisterCharacterCreated(OnCharacterCreated);

        Character c = world.createCharacter(world.GetTileAt(world.Width / 2, world.Height / 2));
        //c.SetDestination(world.GetTileAt(world.Width / 2+5, world.Height / 2));
    }

    void LoadSprites() {
        characterSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/");

        //Debug.Log("LOADED RESOURCE:");
        foreach (Sprite s in sprites) {
            //Debug.Log(s);
            characterSprites[s.name] = s;
        }
    }

    public void OnCharacterCreated(Character character) {
        Debug.Log("OnCharacterCreated");

        GameObject character_go = new GameObject();

        // Add our tile/GO pair to the dictionary.
        characterGameObjectMap.Add(character, character_go);

        character_go.name = "Character";
        character_go.transform.position = new Vector3(character.X, character.Y, 0);
        character_go.transform.SetParent(this.transform, true);

        SpriteRenderer sr = character_go.AddComponent<SpriteRenderer>();
        sr.sprite = characterSprites["char"];
        sr.sortingLayerName = "Characters";

        // Register our callback so that our GameObject gets updated whenever
        // the object's into changes.
        character.RegisterCharacterChangedCallback(OnCharacterChanged);

    }

   void OnCharacterChanged(Character character) {
        //Debug.Log("OnFurnitureChanged");

        if (characterGameObjectMap.ContainsKey(character) == false) {
         Debug.LogError("OnFurnitureChanged -- trying to change visuals for character not in our map.");
            return;
        }

        GameObject character_go = characterGameObjectMap[character];
        //Debug.Log(furn_go);
        //Debug.Log(furn_go.GetComponent<SpriteRenderer>());

        //character_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(character);

        character_go.transform.position = new Vector3(character.X, character.Y, 0);
    }
}
