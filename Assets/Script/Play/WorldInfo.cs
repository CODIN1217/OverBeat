using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public class WorldInfo : MonoBehaviour
{
    [Serializable]
    public class CameraInfo
    {
        [SerializeField] float _rotation;
        [SerializeField] float _size;
        [SerializeField] Vector2 _pos;
        [SerializeField] Color _BGColor;
        [SerializeField] Tweener _rotationTween;
        [SerializeField] Tweener _sizeTween;
        [SerializeField] Tweener _posTween;
        [SerializeField] Tweener _BGColorTween;
        public CameraInfo()
        {
            _pos = Vector2.zero;
            _rotation = 0f;
            _size = 1f;
            _BGColor = new Color(39, 29, 35, 255);
            _rotationTween = new Tweener();
            _sizeTween = new Tweener();
            _posTween = new Tweener();
            _BGColorTween = new Tweener();
        }
        public Vector2 pos { get { return _pos; } set { _pos = value; } }
        public float rotation { get { return _rotation; } set { _rotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float size { get { return _size; } set { _size = Mathf.Clamp(value, -100f, 100f); } }
        public Color BGColor { get { return _BGColor; } set { _BGColor = Handy.Property.GetCorrectRGBA(value); } }
        public Tweener rotationTween { get { return _rotationTween; } set { _rotationTween = value; } }
        public Tweener sizeTween { get { return _sizeTween; } set { _sizeTween = value; } }
        public Tweener posTween { get { return _posTween; } set { _posTween = value; } }
        public Tweener BGColorTween { get { return _BGColorTween; } set { _BGColorTween = value; } }
    }
    [Serializable]
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
    [Serializable]
    public class PlayerInfo
    {
        [SerializeField] int _moveDir;
        [SerializeField] float _tarRadius;
        [SerializeField] float _rotation;
        [SerializeField] float[] _stdDegs;
        [SerializeField] Color _posesGuideColor;
        [SerializeField] Color _sideColor;
        [SerializeField] Color _centerColor;
        [SerializeField] Vector2 _scale;
        [SerializeField] Tweener _degTween;
        [SerializeField] Tweener _tarRadiusTween;
        [SerializeField] Tweener _rotationTween;
        [SerializeField] Tweener _posesGuideColorTween;
        [SerializeField] Tweener _sideColorTween;
        [SerializeField] Tweener _centerColorTween;
        [SerializeField] Tweener _scaleTween;
        public PlayerInfo()
        {
            _moveDir = 1;
            _tarRadius = 1.5f;
            _rotation = 0f;
            _stdDegs = new float[] { 0f, 90f, 180f, 270f };
            _posesGuideColor = new Color(115, 85, 200, 255);
            _sideColor = new Color(100, 45, 250, 255);
            _centerColor = new Color(65, 20, 185, 255);
            _scale = Vector2.one;
            _degTween = new Tweener();
            _tarRadiusTween = new Tweener();
            _rotationTween = new Tweener();
            _posesGuideColorTween = new Tweener();
            _sideColorTween = new Tweener();
            _centerColorTween = new Tweener();
            _scaleTween = new Tweener();
        }
        public int moveDir { get { return _moveDir; } set { _moveDir = (int)Handy.Property.GetSign0Is0(value); } }
        public float tarRadius { get { return _tarRadius; } set { _tarRadius = Mathf.Clamp(value, 0f, 500f); } }
        public float rotation { get { return _rotation; } set { _rotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float[] stdDegs { get { return _stdDegs; } set { _stdDegs = PlayGameManager.Property.GetCorrectStdDegs(value); } }
        public Color posesGuideColor { get { return _posesGuideColor; } set { _posesGuideColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color sideColor { get { return _sideColor; } set { _sideColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color centerColor { get { return _centerColor; } set { _centerColor = Handy.Property.GetCorrectRGBA(value); } }
        public Vector2 scale { get { return _scale; } set { _scale = Handy.Property.GetCorrectXY(value, -100f, 100f); } }
        public Tweener degTween { get { return _degTween; } set { _degTween = value; } }
        public Tweener tarRadiusTween { get { return _tarRadiusTween; } set { _tarRadiusTween = value; } }
        public Tweener rotationTween { get { return _rotationTween; } set { _rotationTween = value; } }
        public Tweener posesGuideColorTween { get { return _posesGuideColorTween; } set { _posesGuideColorTween = value; } }
        public Tweener sideColorTween { get { return _sideColorTween; } set { _sideColorTween = value; } }
        public Tweener centerColorTween { get { return _centerColorTween; } set { _centerColorTween = value; } }
        public Tweener scaleTween { get { return _scaleTween; } set { _scaleTween = value; } }
    }
    [Serializable]
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
        [SerializeField] float _totalRotation;
        [SerializeField] float _startRotation;
        [SerializeField] float _endRotation;
        [SerializeField] string _sideImageName;
        [SerializeField] Color _startColor;
        [SerializeField] Color2 _processStartColor;
        [SerializeField] Color2 _processEndColor;
        [SerializeField] Color _endColor;
        [SerializeField] Tweener _radiusTween;
        [SerializeField] Tweener _holdRadiusTween;
        [SerializeField] Tweener _appearTween;
        [SerializeField] Tweener _totalRotationTween;
        [SerializeField] Tweener _startRotationTween;
        [SerializeField] Tweener _endRotationTween;
        [SerializeField] Tweener _startColorTween;
        [SerializeField] Tweener _processColorTween;
        [SerializeField] Tweener _endColorTween;
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
            _totalRotation = 0f;
            _startRotation = 0f;
            _endRotation = 0f;
            _sideImageName = "Basic";
            _startColor = new Color(100, 45, 250, 255);
            _processStartColor = new Color2(new Color(130, 80, 255, 255), new Color(130, 80, 255, 255));
            _processEndColor = new Color2(new Color(130, 80, 255, 255), new Color(130, 80, 255, 255));
            _endColor = new Color(100, 45, 250, 255);
            _radiusTween = new Tweener();
            _holdRadiusTween = new Tweener();
            _appearTween = new Tweener();
            _totalRotationTween = new Tweener();
            _startRotationTween = new Tweener();
            _endRotationTween = new Tweener();
            _startColorTween = new Tweener();
            _processColorTween = new Tweener();
            _endColorTween = new Tweener();
        }
        public int eachNoteIndex { get { return _eachNoteIndex; } set { _eachNoteIndex = Handy.Property.GetCorrectIndex(value); } }
        public int tarPlayerIndex { get { return _tarPlayerIndex; } set { _tarPlayerIndex = Handy.Property.GetCorrectIndex(value); } }
        public int startDegIndex { get { return _startDegIndex; } set { _startDegIndex = Handy.Property.GetCorrectIndex(value); } }
        public int endDegIndex { get { return _endDegIndex; } set { _endDegIndex = Handy.Property.GetCorrectIndex(value); } }
        public float awakeSecs { get { return _awakeSecs; } set { _awakeSecs = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float speed { get { return _speed; } set { _speed = Mathf.Clamp(value, 0.01f, 100f); } }
        public float startRadius { get { return _startRadius; } set { _startRadius = Mathf.Clamp(value, 0f, 500f); } }
        public float length { get { return _length; } set { _length = Mathf.Clamp(value, 0f, 500f); } }
        public float totalRotation { get { return _totalRotation; } set { _totalRotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float startRotation { get { return _startRotation; } set { _startRotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float endRotation { get { return _endRotation; } set { _endRotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public string sideImageName { get { return _sideImageName; } set { _sideImageName = value; } }
        public Color startColor { get { return _startColor; } set { _startColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color2 processStartColor { get { return _processStartColor; } set { _processStartColor = Handy.Property.GetCorrectRGBA2(value); } }
        public Color2 processEndColor { get { return _processEndColor; } set { _processEndColor = Handy.Property.GetCorrectRGBA2(value); } }
        public Color endColor { get { return _endColor; } set { _endColor = Handy.Property.GetCorrectRGBA(value); } }
        public Tweener radiusTween { get { return _radiusTween; } set { _radiusTween = value; } }
        public Tweener holdRadiusTween { get { return _holdRadiusTween; } set { _holdRadiusTween = value; } }
        public Tweener appearTween { get { return _appearTween; } set { _appearTween = value; } }
        public Tweener totalRotationTween { get { return _totalRotationTween; } set { _totalRotationTween = value; } }
        public Tweener startRotationTween { get { return _startRotationTween; } set { _startRotationTween = value; } }
        public Tweener endRotationTween { get { return _endRotationTween; } set { _endRotationTween = value; } }
        public Tweener startColorTween { get { return _startColorTween; } set { _startColorTween = value; } }
        public Tweener processColorTween { get { return _processColorTween; } set { _processColorTween = value; } }
        public Tweener endColorTween { get { return _endColorTween; } set { _endColorTween = value; } }
    }
    [Serializable]
    public class CenterInfo
    {
        [SerializeField] Color _color;
        [SerializeField] Vector2 _pos;
        [SerializeField] Vector2 _scale;
        [SerializeField] Tweener _colorTween;
        [SerializeField] Tweener _posTween;
        [SerializeField] Tweener _scaleTween;
        public CenterInfo()
        {
            _color = new Color(0, 255, 160, 255);
            _pos = Vector2.zero;
            _scale = Vector2.one;
            _colorTween = new Tweener();
            _posTween = new Tweener();
            _scaleTween = new Tweener();
        }
        public Color color { get { return _color; } set { _color = Handy.Property.GetCorrectRGBA(value); } }
        public Vector2 pos { get { return _pos; } set { _pos = value; } }
        public Vector2 scale { get { return _scale; } set { _scale = Handy.Property.GetCorrectXY(value, -100f, 100f); } }
        public Tweener colorTween { get { return _colorTween; } set { _colorTween = value; } }
        public Tweener posTween { get { return _posTween; } set { _posTween = value; } }
        public Tweener scaleTween { get { return _scaleTween; } set { _scaleTween = value; } }
    }
    [Serializable]
    public class BoundaryInfo
    {
        [SerializeField] Color _lineColor;
        [SerializeField] Color? _coverColor;
        [SerializeField] Vector2 _scale;
        [SerializeField] Vector2? _pos;
        [SerializeField] Tweener _lineColorTween;
        [SerializeField] Tweener _coverColorTween;
        [SerializeField] Tweener _scaleTween;
        [SerializeField] Tweener _posTween;
        public BoundaryInfo()
        {
            _lineColor = new Color(0, 255, 160, 255);
            _coverColor = null/* new Color(39, 29, 35, 255) */;
            _scale = Vector2.one;
            _pos = null/* Vector2.zero */;
            _lineColorTween = new Tweener();
            _coverColorTween = new Tweener();
            _scaleTween = new Tweener();
            _posTween = new Tweener();
        }
        public Color lineColor { get { return _lineColor; } set { _lineColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color? coverColor { get { return _coverColor; } set { _coverColor = Handy.Property.GetCorrectRGBA(value); } }
        public Vector2? pos { get { return _pos; } set { _pos = value; } }
        public Vector2 scale { get { return _scale; } set { _scale = Handy.Property.GetCorrectXY(value, -100f, 100f); } }
        public Tweener lineColorTween { get { return _lineColorTween; } set { _lineColorTween = value; } }
        public Tweener coverColorTween { get { return _coverColorTween; } set { _coverColorTween = value; } }
        public Tweener scaleTween { get { return _scaleTween; } set { _scaleTween = value; } }
        public Tweener posTween { get { return _posTween; } set { _posTween = value; } }
    }
    [Serializable]
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
    public CameraInfo cameraInfo;
    public CountDownInfo countDownInfo;
    public PlayerInfo[] playerInfo;
    public NoteInfo noteInfo;
    public CenterInfo centerInfo;
    public BoundaryInfo boundaryInfo;
    public JudgmentInfo judgmentInfo;
    public CreditInfo creditInfo;

    public WorldInfo()
    {
        cameraInfo = new CameraInfo();
        countDownInfo = new CountDownInfo();
        playerInfo = new PlayerInfo[2];
        noteInfo = new NoteInfo();
        centerInfo = new CenterInfo();
        boundaryInfo = new BoundaryInfo();
        judgmentInfo = new JudgmentInfo();
        creditInfo = new CreditInfo();
    }
}
