//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections;
using System;

public class InstalledObject {

    public Tile tile {
        get; protected set;
    }

    public string objectType {
        get; protected set;
    }

    float movementCost = 1f; // 2 = twice as slow

    int width = 1;
    int height = 1;

    Action<InstalledObject> cbOnChanged;

    protected InstalledObject() {

    }

    static public InstalledObject createPrototype( string objectType, float movementCost, int width, int height ) {
        InstalledObject obj = new InstalledObject();
        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;

        return obj;
    }

    static public InstalledObject place(InstalledObject proto, Tile tile) {
        InstalledObject obj = new InstalledObject();
        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;

        obj.tile = tile;

        if (!tile.assignIntalledObj(obj)) {
            return null;
        }

        return obj;
    }

    public void RegisterOnChangeCallback(Action<InstalledObject> callback) {
        cbOnChanged += callback;
    }

    /// <summary>
    /// Unregister a callback.
    /// </summary>
    public void UnregisterOnChangeCallback(Action<InstalledObject> callback) {
        cbOnChanged -= callback;
    }
}
