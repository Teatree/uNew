using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	public GameObject circleCursorPrefab;

    // Dmitriy's cursors
    public Texture2D cursorTexture;
    public Texture2D cursorTextureMoving;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    bool buildModeIsObjects = false;
    Tile.TileType buildModeTile = Tile.TileType.Floor;
    string buildModeObjectType;

    // The world-position of the mouse last frame.
    Vector3 lastFramePosition;
	Vector3 currFramePosition;

	// The world-position start of our left-mouse drag operation
	Vector3 dragStartPosition;
	List<GameObject> dragPreviewGameObjects;

	// Use this for initialization
	void Start () {
		dragPreviewGameObjects = new List<GameObject>();

        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

        SimplePool.Preload(circleCursorPrefab, 100);
	}

	// Update is called once per frame
	void Update () {
        //if (EventSystem.current.IsPointerOverGameObject()) {

            currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currFramePosition.z = 0;

            //UpdateCursor();
            UpdateDragging();
            UpdateCameraMovement();

            // Save the mouse position from this frame
            // We don't use currFramePosition because we may have moved the camera.
            lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lastFramePosition.z = 0;
        //}
	}

    /*	void UpdateCursor() {
            // Update the circle cursor position
            Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
            if(tileUnderMouse != null) {
                circleCursor.SetActive(true);
                Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
                circleCursor.transform.position = cursorPosition;
            }
            else {
                // Mouse is outside of the valid tile space, so hide the cursor.
                circleCursor.SetActive(false);
            }
        }
    */
    void UpdateDragging() {
		// Start Drag
		if( Input.GetMouseButtonDown(0) ) {
			dragStartPosition = currFramePosition;
            Cursor.SetCursor(cursorTextureMoving, hotSpot, cursorMode);
        }

		int start_x = Mathf.FloorToInt( dragStartPosition.x );
		int end_x =   Mathf.FloorToInt( currFramePosition.x );
		int start_y = Mathf.FloorToInt( dragStartPosition.y );
		int end_y =   Mathf.FloorToInt( currFramePosition.y );
		
		// We may be dragging in the "wrong" direction, so flip things if needed.
		if(end_x < start_x) {
			int tmp = end_x;
			end_x = start_x;
			start_x = tmp;
		}
		if(end_y < start_y) {
			int tmp = end_y;
			end_y = start_y;
			start_y = tmp;
		}

		// Clean up old drag previews
		while(dragPreviewGameObjects.Count > 0) {
			GameObject go = dragPreviewGameObjects[0];
			dragPreviewGameObjects.RemoveAt(0);
			SimplePool.Despawn (go);
		}

		if( Input.GetMouseButton(0) ) {
			// Display a preview of the drag area
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt(x, y);
					if(t != null) {
						// Display the building hint on top of this tile position
						GameObject go = SimplePool.Spawn( circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity );
						go.transform.SetParent(this.transform, true);
						dragPreviewGameObjects.Add(go);
					}
				}
			}
		}

		// End Drag
		if( Input.GetMouseButtonUp(0) ) {
            // Loop through all the tiles
            for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt(x, y);

                    if (t != null) {
                        if (buildModeIsObjects) {

                            WorldController.Instance.World.placeFurniture(buildModeObjectType, t);

                        }
                        else {
                            t.Type = buildModeTile;
                        }
                    }
				}
			}
		}
	}

	void UpdateCameraMovement() {
		// Handle screen panning
		if( Input.GetMouseButton(1) || Input.GetMouseButton(2) ) {	// Right or Middle Mouse Button
			
			Vector3 diff = lastFramePosition - currFramePosition;
			Camera.main.transform.Translate( diff );
            Cursor.SetCursor(cursorTextureMoving, hotSpot, cursorMode);
            
        }
        else
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }

		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");

		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 4f, 20f);
	}

    public void SetModeBuildFloor() {
        buildModeTile = Tile.TileType.Floor;
        buildModeIsObjects = false;
    }

    public void SetModeBuldoze() {
        buildModeTile = Tile.TileType.Empty;
        buildModeIsObjects = false;
    }

    public void SetModeBuildObject(string objectType) {
        buildModeIsObjects = true;
        buildModeObjectType = objectType;
    }

}
