using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class WorldInfo
{
    public int numberOfCountDownTick;
    public List<int> direction;
    public List<int> nextDegIndex;
    public int playerIndex;

    public List<float> speed;
    public List<float> playerTarRadius;
    public List<float> noteStartRadius;
    public List<float> noteLength;
    public List<float> playerRotation;
    public List<float> noteRotation;
    public float cameraRotation;
    public float intervalOfCountDownTick;
    public float judgmentRange;
    public float cameraSize;
    public float awakeTime;

    public string worldName;
    public string worldEditor;
    public string songName;
    public string songWriter;
    public string SideImageName;

    public List<Color> playerSideColor;
    public List<Color> playerCenterColor;
    public List<Color> startNoteColor;
    public List<Color> processNoteColor;
    public List<Color> endNoteColor;
    public Color centerColor;
    public Color boundaryLineColor;
    public Color? boundaryCoverColor;
    public Color cameraBGColor;
    public Color playerPosesGuideColor;

    public Vector2 centerPos;
    public List<Vector2> playerScale;
    public Vector2 centerScale;
    public Vector2 boundaryScale;

    public List<AnimationCurve> playerMoveEaseType;
    public List<AnimationCurve> playerTarRadiusEaseType;
    public List<AnimationCurve> noteMoveEaseType;
    public List<AnimationCurve> longNoteMoveEaseType;
    public List<AnimationCurve> noteAppearEaseType;

    public List<float> stdDegs;

    public WorldInfo()
    {
        numberOfCountDownTick = 4;
        direction = new List<int>(){1, 1};
        nextDegIndex = new List<int>(){0, 0};
        playerIndex = 0;

        speed = new List<float>(){1f, 1f};
        playerTarRadius = new List<float>(){1.5f, 1.5f};
        noteStartRadius = new List<float>(){5f, 5f};
        noteLength = new List<float>(){0f, 0f};
        playerRotation = new List<float>(){0f, 0f};
        noteRotation = new List<float>(){0f, 0f};
        cameraRotation = 0f;
        intervalOfCountDownTick = 1f;
        judgmentRange = 0.5f;
        cameraSize = 1f;
        awakeTime = 0f;

        worldName = "Empty";
        worldEditor = "Empty";
        songName = "Empty";
        songWriter = "Empty";
        SideImageName = "Basic";

        playerSideColor = new List<Color>(){new Color(100, 45, 250, 255), new Color(155, 210, 5, 255)};
        playerCenterColor = new List<Color>(){new Color(65, 20, 185, 255), new Color(190, 235, 70, 255)};
        startNoteColor = new List<Color>(){new Color(100, 45, 250, 255), new Color(155, 210, 5, 255)};
        processNoteColor = new List<Color>(){new Color(130, 80, 255, 255), new Color(125, 175, 0, 255)};
        endNoteColor = new List<Color>(){new Color(100, 45, 250, 255), new Color(155, 210, 5, 255)};
        centerColor = new Color(0, 255, 160, 255);
        boundaryLineColor = new Color(0, 255, 160, 255);
        boundaryCoverColor = null/* new Color(39, 29, 35, 255) */;
        cameraBGColor = new Color(39, 29, 35, 255);
        playerPosesGuideColor = new Color(115, 85, 200, 255);

        centerPos = Vector2.zero;
        playerScale = new List<Vector2>(){Vector2.one, Vector2.one};
        centerScale = Vector2.one;
        boundaryScale = Vector2.one;
        playerMoveEaseType = new List<AnimationCurve>(){AnimationCurve.Linear(0f, 0f, 1f, 1f), AnimationCurve.Linear(0f, 0f, 1f, 1f)};
        playerTarRadiusEaseType = new List<AnimationCurve>(){AnimationCurve.Linear(0f, 0f, 1f, 1f), AnimationCurve.Linear(0f, 0f, 1f, 1f)};
        noteMoveEaseType = new List<AnimationCurve>(){AnimationCurve.Linear(0f, 0f, 1f, 1f), AnimationCurve.Linear(0f, 0f, 1f, 1f)};
        longNoteMoveEaseType = new List<AnimationCurve>(){AnimationCurve.Linear(0f, 0f, 1f, 1f), AnimationCurve.Linear(0f, 0f, 1f, 1f)};
        noteAppearEaseType = new List<AnimationCurve>(){AnimationCurve.Linear(0f, 0f, 1f, 1f), AnimationCurve.Linear(0f, 0f, 1f, 1f)};

        stdDegs = new List<float>(){0f, 90f, 180f, 270f};
    }
}
