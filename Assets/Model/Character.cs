//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections;
using System;

public class Character {

    public float X {
        get {
            return Mathf.Lerp(currTile.X, destTile.X, movementPerc);
        }
    }
    public float Y {
        get {
            return Mathf.Lerp(currTile.Y, destTile.Y, movementPerc);
        }
    }

    public Tile currTile {
        get; protected set;
    }

    Tile destTile;
    float movementPerc;

    float speed = 2f;    //Tiles per second

    Action<Character> cbCharacterChanged;

    Job myJob;

    public Character(Tile tile) {
        currTile = destTile = tile;
    }

    public void Update(float deltaTime) {

        // Jobs
        if(myJob == null) {
            myJob = currTile.world.jobQueue.Dequeue();

            if(myJob != null) {
                destTile = myJob.tile;
                myJob.RegisterJobCancelCallback(onJobEnded);
                myJob.RegisterJobCompleteCallback(onJobEnded);
            }
        }


        // walking
        if(currTile == destTile) {
            if(myJob != null) {
                myJob.DoWork(deltaTime);
            }

            return;        
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X - destTile.X, 2) + Mathf.Pow(currTile.X - destTile.X, 2));

        float distThisFrame = speed * deltaTime;

        float percThisFrame = distThisFrame / distToTravel;

        movementPerc += percThisFrame;

        if(movementPerc >= 1) {
            currTile = destTile;
            movementPerc = 0;
        }

        if(cbCharacterChanged != null) {
            cbCharacterChanged(this);
        }
    }

    public void SetDestination(Tile tile) {
        if (currTile.IsNeighbour(tile, true) == false) {
            Debug.Log("Character::SetDestination -- Our destination title isn't actually neighbrours");
        }

        destTile = tile;
    }

    public void RegisterCharacterChangedCallback(Action<Character> callbackFunc) {
        cbCharacterChanged += callbackFunc;
    }

    public void UnregisterCharacterChangedCallback(Action<Character> callbackFunc) {
        cbCharacterChanged -= callbackFunc;
    }

    void onJobEnded(Job j) {
        if(j != myJob) {
            return;
        }

        myJob = null;
    }

}
