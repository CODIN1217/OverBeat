using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

// public enum SideImage { Basic, Custom }
public class WorldInfo
{
    public int numberOfCountDownTick;
    public int direction;
    public int nextDegIndex;
    public int tarPlayerIndex;

    public float speed;
    // public float nextDeg;
    public float playerRadius;
    public float noteStartRadius;
    // public float noteWaitTime;
    public float noteLength;
    public float playerRotation;
    public float noteRotation;
    public float cameraRotation;
    public float intervalOfCountDownTick;
    public float judgmentRange;
    public float cameraSize;

    public string worldName;
    public string worldEditor;
    public string songName;
    public string songWriter;
    public string SideImageName;

    public PlayMode mode;

    public Color playerSideColor;
    public Color playerCenterColor;
    // public Color playerVertexColor;
    public Color startNoteColor;
    public Color processNoteColor;
    public Color endNoteColor;
    public Color centerColor;
    public Color boundaryCoverColor;

    public Vector2 centerPos;
    public Vector2 playerScale;
    // public Vector2 noteScale;
    public Vector2 centerScale;
    public Vector2? boundaryScale;

    public AnimationCurve playerMoveEaseType;
    public AnimationCurve playerRadiusEaseType;
    public AnimationCurve noteMoveEaseType;
    public AnimationCurve longNoteMoveEaseType;
    public AnimationCurve noteAppearEaseType;

    public List<float> stdDegs;

    public WorldInfo()
    {
        numberOfCountDownTick = 4;
        direction = 1;
        nextDegIndex = 0;
        tarPlayerIndex = 0;

        speed = 1f;
        // nextDeg = 0f;
        playerRadius = 1.5f;
        noteStartRadius = 5f;
        // noteWaitTime = 1f;
        noteLength = 0f;
        playerRotation = 0f;
        noteRotation = 0f;
        cameraRotation = 0f;
        intervalOfCountDownTick = 1f;
        judgmentRange = 0.5f;
        cameraSize = 1f;

        worldName = "Empty";
        worldEditor = "Empty";
        songName = "Empty";
        songWriter = "Empty";
        SideImageName = "Basic";

        // mode = PlayMode.Smooth;

        playerSideColor = new Color(1f, 1f, 1f);
        playerCenterColor = new Color(1f, 1f, 1f);
        // playerVertexColor = new Color(1f, 1f, 1f);
        startNoteColor = new Color(1f, 1f, 1f);
        processNoteColor = new Color(1f, 1f, 1f);
        endNoteColor = new Color(1f, 1f, 1f);
        centerColor = new Color(1f, 1f, 1f);
        boundaryCoverColor = new Color(0f, 0f, 0f);

        centerPos = Vector2.zero;
        playerScale = Vector2.one;
        // noteScale = Vector2.one;
        centerScale = Vector2.one;
        boundaryScale = Vector2.one;
        playerMoveEaseType = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        playerRadiusEaseType = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        noteMoveEaseType = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        longNoteMoveEaseType = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        noteAppearEaseType = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        stdDegs = new List<float>(){0f, 90f, 180f, 270f};
    }
}
