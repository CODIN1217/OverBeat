using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
// [ExecuteInEditMode]
public class WorldInfo : MonoBehaviour
{
    [Serializable]
    // [ExecuteInEditMode]
    public class CameraInfo
    {
        [SerializeField] float _rotation;
        [SerializeField] float _size;
        [SerializeField] Color _BGColor;
        public CameraInfo()
        {
            _rotation = 0f;
            _size = 1f;
            _BGColor = new Color(39, 29, 35, 255);
        }
        public float rotation { get { return _rotation; } set { _rotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float size { get { return _size; } set { _size = Mathf.Clamp(value, -100f, 100f); } }
        public Color BGColor { get { return _BGColor; } set { _BGColor = Handy.Property.GetCorrectRGBA(value); } }
    }
    [Serializable]
    // [ExecuteInEditMode]
    public class CountDownInfo
    {
        [SerializeField] int _numberOfTick;
        [SerializeField] float _intervalOfTick;
        public CountDownInfo()
        {
            _numberOfTick = 4;
            _intervalOfTick = 1f;
        }
        public int numberOfTick { get { return _numberOfTick; } set { _numberOfTick = (int)Mathf.Clamp(value, 1, 400); } }
        public float intervalOfTick { get { return _intervalOfTick; } set { _intervalOfTick = Mathf.Clamp(value, 0.01f, 100f); } }
    }
    /* [Serializable]
    // [ExecuteInEditMode]
    public class VariousModeInfo
    {
        [SerializeField] int _variousModeCount;
        public VariousModeInfo()
        {
            _variousModeCount = 1;
        }
        public int variousModeCount { get { return _variousModeCount; } set { _variousModeCount = (int)Mathf.Clamp(value, 1, 2); } }
    } */
    [Serializable]
    // [ExecuteInEditMode]
    public class PlayerInfo
    {
        // int Index;
        [SerializeField] int _moveDir;
        // [SerializeField] int _noteCount;
        [SerializeField] float _tarRadius;
        [SerializeField] float _rotation;
        [SerializeField] float[] _stdDegs;
        [SerializeField] Color _posesGuideColor;
        [SerializeField] Color _sideColor;
        [SerializeField] Color _centerColor;
        [SerializeField] Vector2 _scale;
        [SerializeField] AnimationCurve _degEase;
        [SerializeField] AnimationCurve _tarRadiusEase;
        public PlayerInfo()
        {
            // Index = 0;
            _moveDir = 1;
            // _noteCount = 1;
            _tarRadius = 1.5f;
            _rotation = 0f;
            _stdDegs = new float[] { 0f, 90f, 180f, 270f };
            _posesGuideColor = new Color(115, 85, 200, 255);
            _sideColor = new Color(100, 45, 250, 255);
            _centerColor = new Color(65, 20, 185, 255);
            _scale = Vector2.one;
            _degEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            _tarRadiusEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
        public int moveDir { get { return _moveDir; } set { _moveDir = (int)Handy.Property.GetSign0Is0(value); } }
        // public int noteCount { get { return _noteCount; } set { _noteCount = (int)Mathf.Clamp(value, 1, int.MaxValue); } }
        public float tarRadius { get { return _tarRadius; } set { _tarRadius = Mathf.Clamp(value, 0f, 500f); } }
        public float rotation { get { return _rotation; } set { _rotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float[] stdDegs { get { return _stdDegs; } set { _stdDegs = Handy.Property.GetCorrectStdDegs(value); } }
        public Color posesGuideColor { get { return _posesGuideColor; } set { _posesGuideColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color sideColor { get { return _sideColor; } set { _sideColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color centerColor { get { return _centerColor; } set { _centerColor = Handy.Property.GetCorrectRGBA(value); } }
        public Vector2 scale { get { return _scale; } set { _scale = Handy.Property.GetCorrectXY(value, -100f, 100f); } }
        public AnimationCurve degEase { get { return _degEase; } set { _degEase = value; } }
        public AnimationCurve tarRadiusEase { get { return _tarRadiusEase; } set { _tarRadiusEase = value; } }
    }
    [Serializable]
    // [ExecuteInEditMode]
    public class NoteInfo
    {
        [SerializeField] int _eachNoteIndex;
        [SerializeField] int _tarPlayerIndex;
        [SerializeField] int _startDegIndex;
        [SerializeField] int _endDegIndex;
        [SerializeField] float _awakeSecs;
        [SerializeField] float _speed;
        [SerializeField] float _startRadius;
        [SerializeField] float _length;
        [SerializeField] float _rotation;
        [SerializeField] string _sideImageName;
        [SerializeField] Color _startColor;
        [SerializeField] Color _processColor;
        [SerializeField] Color _endColor;
        [SerializeField] AnimationCurve _radiusEase;
        [SerializeField] AnimationCurve _holdRadiusEase;
        [SerializeField] AnimationCurve _appearEase;
        public NoteInfo()
        {
            _eachNoteIndex = 0;
            _tarPlayerIndex = 0;
            _startDegIndex = 0;
            _endDegIndex = 0;
            _awakeSecs = 0f;
            _speed = 1f;
            _startRadius = 5f;
            _length = 0f;
            _rotation = 0f;
            _sideImageName = "Basic";
            _startColor = new Color(100, 45, 250, 255);
            _processColor = new Color(130, 80, 255, 255);
            _endColor = new Color(100, 45, 250, 255);
            _radiusEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            _holdRadiusEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            _appearEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
        public int eachNoteIndex { get { return _eachNoteIndex; } set { _eachNoteIndex = value; } }
        public int tarPlayerIndex { get { return _tarPlayerIndex; } set { _tarPlayerIndex = value; } }
        public int startDegIndex { get { return _startDegIndex; } set { _startDegIndex = value; } }
        public int endDegIndex { get { return _endDegIndex; } set { _endDegIndex = value; } }
        public float awakeSecs { get { return _awakeSecs; } set { _awakeSecs = Mathf.Clamp(value, 0f, 1f); } }
        public float speed { get { return _speed; } set { _speed = Mathf.Clamp(value, 0.01f, 100f); } }
        public float startRadius { get { return _startRadius; } set { _startRadius = Mathf.Clamp(value, 0f, 500f); } }
        public float length { get { return _length; } set { _length = Mathf.Clamp(value, 0f, 500f); } }
        public float rotation { get { return _rotation; } set { _rotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public string sideImageName { get { return _sideImageName; } set { _sideImageName = value; } }
        public Color startColor { get { return _startColor; } set { _startColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color processColor { get { return _processColor; } set { _processColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color endColor { get { return _endColor; } set { _endColor = Handy.Property.GetCorrectRGBA(value); } }
        public AnimationCurve radiusEase { get { return _radiusEase; } set { _radiusEase = value; } }
        public AnimationCurve holdRadiusEase { get { return _holdRadiusEase; } set { _holdRadiusEase = value; } }
        public AnimationCurve appearEase { get { return _appearEase; } set { _appearEase = value; } }
    }
    [Serializable]
    // [ExecuteInEditMode]
    public class CenterInfo
    {
        [SerializeField] Color _color;
        [SerializeField] Vector2 _pos;
        [SerializeField] Vector2 _scale;
        public CenterInfo()
        {
            _color = new Color(0, 255, 160, 255);
            _pos = Vector2.zero;
            _scale = Vector2.one;
        }
        public Color color { get { return _color; } set { _color = Handy.Property.GetCorrectRGBA(value); } }
        public Vector2 pos { get { return _pos; } set { _pos = value; } }
        public Vector2 scale { get { return _scale; } set { _scale = Handy.Property.GetCorrectXY(value, -100f, 100f); } }
    }
    [Serializable]
    // [ExecuteInEditMode]
    public class BoundaryInfo
    {
        [SerializeField] Color _lineColor;
        [SerializeField] Color? _coverColor;
        [SerializeField] Vector2 _scale;
        public BoundaryInfo()
        {
            _lineColor = new Color(0, 255, 160, 255);
            _coverColor = null/* new Color(39, 29, 35, 255) */;
            _scale = Vector2.one;
        }
        public Color lineColor { get { return _lineColor; } set { _lineColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color? coverColor { get { return _coverColor; } set { _coverColor = Handy.Property.GetCorrectRGBA(value); } }
        public Vector2 scale { get { return _scale; } set { _scale = Handy.Property.GetCorrectXY(value, -100f, 100f); } }
    }
    [Serializable]
    // [ExecuteInEditMode]
    public class JudgmentInfo
    {
        [SerializeField] float _range;
        public JudgmentInfo()
        {
            _range = 0.5f;
        }
        public float range { get { return _range; } set { _range = Mathf.Clamp(value, 0.05f, 0.5f); } }
    }
    [Serializable]
    // [ExecuteInEditMode]
    public class CreditInfo
    {
        [SerializeField] string _worldName;
        [SerializeField] string _worldEditor;
        [SerializeField] string _songName;
        [SerializeField] string _songWriter;
        public CreditInfo()
        {
            _worldName = "Empty";
            _worldEditor = "Empty";
            _songName = "Empty";
            _songWriter = "Empty";
        }
        public string worldName { get { return _worldName; } set { _worldName = value; } }
        public string worldEditor { get { return _worldEditor; } set { _worldEditor = value; } }
        public string songName { get { return _songName; } set { _songName = value; } }
        public string songWriter { get { return _songWriter; } set { _songWriter = value; } }
    }
    // [SerializeField] float _waitTime;
    public CameraInfo cameraInfo;
    public CountDownInfo countDownInfo;
    // public VariousModeInfo variousModeInfo;
    public PlayerInfo[] playerInfo;
    public NoteInfo noteInfo;
    public CenterInfo centerInfo;
    public BoundaryInfo boundaryInfo;
    public JudgmentInfo judgmentInfo;
    public CreditInfo creditInfo;

    public WorldInfo()
    {
        // _waitTime = 1f;
        cameraInfo = new CameraInfo();
        countDownInfo = new CountDownInfo();
        // variousModeInfo = new VariousModeInfo();
        playerInfo = new PlayerInfo[2];
        noteInfo = new NoteInfo();
        centerInfo = new CenterInfo();
        boundaryInfo = new BoundaryInfo();
        judgmentInfo = new JudgmentInfo();
        creditInfo = new CreditInfo();
    }
    // public float waitTime { get { return _waitTime; } set { _waitTime = value; } }
}
