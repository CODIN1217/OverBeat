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
        // /* [SerializeField] */ public float rotation;
        // /* [SerializeField] */ public float size;
        // /* [SerializeField] */ public Vector2 pos;
        // /* [SerializeField] */ public Color BGColor;
        /* [SerializeField] */
        public TweenInfo<float> rotationTween;
        /* [SerializeField] */
        public TweenInfo<float> sizeTween;
        /* [SerializeField] */
        public TweenInfo<Vector2> posTween;
        /* [SerializeField] */
        public TweenInfo<Color> BGColorTween;
        public CameraInfo()
        {
            // pos = Vector2.zero;
            // rotation = 0f;
            // size = 1f;
            // BGColor = new Color(39, 29, 35, 255) / 255f;
        }/* 
        public Vector2 _pos { get { return _pos; } set { _pos = value; } }
        public float _rotation { get { return _rotation; } set { _rotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float _size { get { return _size; } set { _size = Mathf.Clamp(value, -100f, 100f); } }
        public Color _BGColor { get { return _BGColor; } set { _BGColor = Handy.Property.GetCorrectRGBA(value); } }
        public Tweener _rotationTween { get { return _rotationTween; } set { _rotationTween = value; } }
        public Tweener _sizeTween { get { return _sizeTween; } set { _sizeTween = value; } }
        public Tweener _posTween { get { return _posTween; } set { _posTween = value; } }
        public Tweener _BGColorTween { get { return _BGColorTween; } set { _BGColorTween = value; } } */
    }
    [Serializable]
    public class CountDownInfo
    {
        /* [SerializeField] */
        public int numberOfTick;
        /* [SerializeField] */
        public float intervalOfTick;
        public CountDownInfo()
        {
        }/* 
        public int _numberOfTick { get { return _numberOfTick; } set { _numberOfTick = (int)Mathf.Clamp(value, 1, 400); } }
        public float _intervalOfTick { get { return _intervalOfTick; } set { _intervalOfTick = Mathf.Clamp(value, 0.01f, 100f); } } */
    }
    [Serializable]
    public class PlayerInfo
    {
        /* [SerializeField] */
        public int moveDir;
        // /* [SerializeField] */ public float tarRadius;
        // /* [SerializeField] */ public float rotation;
        /* [SerializeField] */
        public float[] stdDegs;
        // /* [SerializeField] */ public Color posesGuideColor;
        // /* [SerializeField] */ public Color sideColor;
        // /* [SerializeField] */ public Color centerColor;
        // /* [SerializeField] */ public Vector2 scale;
        /* [SerializeField] */
        public TweenInfo<float> degTween;
        /* [SerializeField] */
        public TweenInfo<float> tarRadiusTween;
        /* [SerializeField] */
        public TweenInfo<float> rotationTween;
        /* [SerializeField] */
        public TweenInfo<Color> posesGuideColorTween;
        /* [SerializeField] */
        public TweenInfo<Color> sideColorTween;
        /* [SerializeField] */
        public TweenInfo<Color> centerColorTween;
        /* [SerializeField] */
        public TweenInfo<Vector2> scaleTween;
        public PlayerInfo()
        {
        }/* 
        public int _moveDir { get { return _moveDir; } set { _moveDir = (int)Handy.Property.GetSign0Is0(value); } }
        public float _tarRadius { get { return _tarRadius; } set { _tarRadius = Mathf.Clamp(value, 0f, 500f); } }
        public float _rotation { get { return _rotation; } set { _rotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float[] _stdDegs { get { return _stdDegs; } set { _stdDegs = PlayGameManager.Property.GetCorrectStdDegs(value); } }
        public Color _posesGuideColor { get { return _posesGuideColor; } set { _posesGuideColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color _sideColor { get { return _sideColor; } set { _sideColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color _centerColor { get { return _centerColor; } set { _centerColor = Handy.Property.GetCorrectRGBA(value); } }
        public Vector2 _scale { get { return _scale; } set { _scale = Handy.Property.GetCorrectXY(value, -100f, 100f); } }
        public Tweener _degTween { get { return _degTween; } set { _degTween = value; } }
        public Tweener _tarRadiusTween { get { return _tarRadiusTween; } set { _tarRadiusTween = value; } }
        public Tweener _rotationTween { get { return _rotationTween; } set { _rotationTween = value; } }
        public Tweener _posesGuideColorTween { get { return _posesGuideColorTween; } set { _posesGuideColorTween = value; } }
        public Tweener _sideColorTween { get { return _sideColorTween; } set { _sideColorTween = value; } }
        public Tweener _centerColorTween { get { return _centerColorTween; } set { _centerColorTween = value; } }
        public Tweener _scaleTween { get { return _scaleTween; } set { _scaleTween = value; } } */
    }
    [Serializable]
    public class NoteInfo
    {
        /* [SerializeField] */
        public int eachNoteIndex;
        /* [SerializeField] */
        public int tarPlayerIndex;
        /* [SerializeField] */
        public int startDegIndex;
        /* [SerializeField] */
        public int endDegIndex;
        /* [SerializeField] */
        public float startDeg;
        /* [SerializeField] */
        public float endDeg;
        /* [SerializeField] */
        public float awakeSecs;
        /* [SerializeField] */
        public float speed;
        // /* [SerializeField] */ public float startRadius;
        // /* [SerializeField] */ public float length;
        // /* [SerializeField] */ public float totalRotation;
        // /* [SerializeField] */ public float startRotation;
        // /* [SerializeField] */ public float endRotation;
        /* [SerializeField] */
        public string sideImageName;
        // /* [SerializeField] */ public Color startColor;
        // /* [SerializeField] */ public Color processStartColor;
        // /* [SerializeField] */ public Color processEndColor;
        // /* [SerializeField] */ public Color endColor;
        /* [SerializeField] */
        public TweenInfo<float> waitRadiusTween;
        /* [SerializeField] */
        public TweenInfo<float> holdRadiusTween;
        /* [SerializeField] */
        public TweenInfo<float> appearTween;
        /* [SerializeField] */
        public TweenInfo<float> totalRotationTween;
        /* [SerializeField] */
        public TweenInfo<float> startRotationTween;
        /* [SerializeField] */
        public TweenInfo<float> endRotationTween;
        /* [SerializeField] */
        public TweenInfo<Color> startColorTween;
        /* [SerializeField] */
        public TweenInfo<Color> processStartColorTween;
        /* [SerializeField] */
        public TweenInfo<Color> processEndColorTween;
        /* [SerializeField] */
        public TweenInfo<Color> endColorTween;
        public NoteInfo()
        {
        }/* 
        public int _eachNoteIndex { get { return _eachNoteIndex; } set { _eachNoteIndex = Handy.Property.GetCorrectIndex(value); } }
        public int _tarPlayerIndex { get { return _tarPlayerIndex; } set { _tarPlayerIndex = Handy.Property.GetCorrectIndex(value); } }
        public int _startDegIndex { get { return _startDegIndex; } set { _startDegIndex = Handy.Property.GetCorrectIndex(value); } }
        public int _endDegIndex { get { return _endDegIndex; } set { _endDegIndex = Handy.Property.GetCorrectIndex(value); } }
        public float _awakeSecs { get { return _awakeSecs; } set { _awakeSecs = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float _speed { get { return _speed; } set { _speed = Mathf.Clamp(value, 0.01f, 100f); } }
        public float _startRadius { get { return _startRadius; } set { _startRadius = Mathf.Clamp(value, 0f, 500f); } }
        public float _length { get { return _length; } set { _length = Mathf.Clamp(value, 0f, 500f); } }
        public float _totalRotation { get { return _totalRotation; } set { _totalRotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float _startRotation { get { return _startRotation; } set { _startRotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public float _endRotation { get { return _endRotation; } set { _endRotation = Handy.Property.GetCorrectDegMaxIs0(value); } }
        public string _sideImageName { get { return _sideImageName; } set { _sideImageName = value; } }
        public Color _startColor { get { return _startColor; } set { _startColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color _processStartColor { get { return _processStartColor; } set { _processStartColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color _processEndColor { get { return _processEndColor; } set { _processEndColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color _endColor { get { return _endColor; } set { _endColor = Handy.Property.GetCorrectRGBA(value); } }
        public Tweener _radiusTween { get { return _radiusTween; } set { _radiusTween = value; } }
        public Tweener _holdRadiusTween { get { return _holdRadiusTween; } set { _holdRadiusTween = value; } }
        public Tweener _appearTween { get { return _appearTween; } set { _appearTween = value; } }
        public Tweener _totalRotationTween { get { return _totalRotationTween; } set { _totalRotationTween = value; } }
        public Tweener _startRotationTween { get { return _startRotationTween; } set { _startRotationTween = value; } }
        public Tweener _endRotationTween { get { return _endRotationTween; } set { _endRotationTween = value; } }
        public Tweener _startColorTween { get { return _startColorTween; } set { _startColorTween = value; } }
        public Tweener _processStartColorTween { get { return _processStartColorTween; } set { _processStartColorTween = value; } }
        public Tweener _processEndColorTween { get { return _processEndColorTween; } set { _processEndColorTween = value; } }
        public Tweener _endColorTween { get { return _endColorTween; } set { _endColorTween = value; } } */
    }
    [Serializable]
    public class CenterInfo
    {
        // /* [SerializeField] */ public Color color;
        // /* [SerializeField] */ public Vector2 pos;
        // /* [SerializeField] */ public Vector2 scale;
        /* [SerializeField] */
        public TweenInfo<Color> colorTween;
        /* [SerializeField] */
        public TweenInfo<Vector2> posTween;
        /* [SerializeField] */
        public TweenInfo<Vector2> scaleTween;
        public CenterInfo()
        {
            // color = new Color(0, 255, 160, 255) / 255f;
            // pos = Vector2.zero;
            // scale = Vector2.one;
        }/* 
        public Color _color { get { return _color; } set { _color = Handy.Property.GetCorrectRGBA(value); } }
        public Vector2 _pos { get { return _pos; } set { _pos = value; } }
        public Vector2 _scale { get { return _scale; } set { _scale = Handy.Property.GetCorrectXY(value, -100f, 100f); } }
        public Tweener _colorTween { get { return _colorTween; } set { _colorTween = value; } }
        public Tweener _posTween { get { return _posTween; } set { _posTween = value; } }
        public Tweener _scaleTween { get { return _scaleTween; } set { _scaleTween = value; } } */
    }
    [Serializable]
    public class BoundaryInfo
    {
        // /* [SerializeField] */ public Color lineColor;
        // /* [SerializeField] */ public Color? coverColor;
        // /* [SerializeField] */ public Vector2 scale;
        // /* [SerializeField] */ public Vector2? pos;
        /* [SerializeField] */
        public TweenInfo<Color> lineColorTween;
        /* [SerializeField] */
        public TweenInfo<Color> coverColorTween;
        /* [SerializeField] */
        public TweenInfo<Vector2> scaleTween;
        /* [SerializeField] */
        public TweenInfo<Vector2> posTween;
        public BoundaryInfo()
        {
            // lineColor = new Color(0, 255, 160, 255) / 255f;
            // coverColor = null/* new Color(39, 29, 35, 255) */;
            // scale = Vector2.one;
            // pos = null/* Vector2.zero */;
        }/* 
        public Color _lineColor { get { return _lineColor; } set { _lineColor = Handy.Property.GetCorrectRGBA(value); } }
        public Color? _coverColor { get { return _coverColor; } set { _coverColor = Handy.Property.GetCorrectRGBA(value); } }
        public Vector2? _pos { get { return _pos; } set { _pos = value; } }
        public Vector2 _scale { get { return _scale; } set { _scale = Handy.Property.GetCorrectXY(value, -100f, 100f); } }
        public Tweener _lineColorTween { get { return _lineColorTween; } set { _lineColorTween = value; } }
        public Tweener _coverColorTween { get { return _coverColorTween; } set { _coverColorTween = value; } }
        public Tweener _scaleTween { get { return _scaleTween; } set { _scaleTween = value; } }
        public Tweener _posTween { get { return _posTween; } set { _posTween = value; } } */
    }
    [Serializable]
    public class JudgmentInfo
    {
        /* [SerializeField] */
        public float range;
        public JudgmentInfo()
        {
        }/* 
        public float _range { get { return _range; } set { _range = Mathf.Clamp(value, 0.05f, 0.5f); } } */
    }
    [Serializable]
    public class CreditInfo
    {
        /* [SerializeField] */
        public string worldName;
        /* [SerializeField] */
        public string worldEditor;
        /* [SerializeField] */
        public string songName;
        /* [SerializeField] */
        public string songWriter;
        public CreditInfo()
        {
        }/* 
        public string _worldName { get { return _worldName; } set { _worldName = value; } }
        public string _worldEditor { get { return _worldEditor; } set { _worldEditor = value; } }
        public string _songName { get { return _songName; } set { _songName = value; } }
        public string _songWriter { get { return _songWriter; } set { _songWriter = value; } } */
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
        centerInfo = new CenterInfo();
        countDownInfo = new CountDownInfo();
        playerInfo = new PlayerInfo[2];
        noteInfo = new NoteInfo();
        boundaryInfo = new BoundaryInfo();
        judgmentInfo = new JudgmentInfo();
        creditInfo = new CreditInfo();
        Init(cameraInfo);
        Init(centerInfo);
        Init(countDownInfo);
        for (int i = 0; i < playerInfo.Length; i++)
        {
            playerInfo[i] = new PlayerInfo();
            Init(playerInfo[i]);
        }
        Init(noteInfo);
        Init(boundaryInfo);
        Init(judgmentInfo);
        Init(creditInfo);
    }
    void Init(CameraInfo cameraInfo)
    {
        cameraInfo.rotationTween = new TweenInfo<float>(0f, 0f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        cameraInfo.sizeTween = new TweenInfo<float>(1f, 1f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        cameraInfo.posTween = new TweenInfo<Vector2>(Vector2.zero, Vector2.zero, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        cameraInfo.BGColorTween = new TweenInfo<Color>(new Color(39, 29, 35, 255) / 255f, new Color(39, 29, 35, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
    }
    void Init(CountDownInfo countDownInfo)
    {
        countDownInfo.numberOfTick = 4;
        countDownInfo.intervalOfTick = 1f;
    }
    void Init(PlayerInfo playerInfo)
    {
        playerInfo.moveDir = 1;
        playerInfo.stdDegs = new float[] { 0f, 90f, 180f, 270f };
        playerInfo.degTween = new TweenInfo<float>(0f, 0f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        playerInfo.tarRadiusTween = new TweenInfo<float>(1.5f, 1.5f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        playerInfo.rotationTween = new TweenInfo<float>(0f, 0f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        playerInfo.posesGuideColorTween = new TweenInfo<Color>(new Color(115, 85, 200, 255) / 255f, new Color(115, 85, 200, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        playerInfo.sideColorTween = new TweenInfo<Color>(new Color(100, 45, 250, 255) / 255f, new Color(100, 45, 250, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        playerInfo.centerColorTween = new TweenInfo<Color>(new Color(65, 20, 185, 255) / 255f, new Color(65, 20, 185, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        playerInfo.scaleTween = new TweenInfo<Vector2>(Vector2.one, Vector2.one, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
    }
    void Init(NoteInfo noteInfo)
    {
        noteInfo.eachNoteIndex = 0;
        noteInfo.tarPlayerIndex = 0;
        noteInfo.startDegIndex = 0;
        noteInfo.endDegIndex = 0;
        noteInfo.awakeSecs = 0f;
        noteInfo.speed = 1f;
        noteInfo.sideImageName = "Basic";
        noteInfo.waitRadiusTween = new TweenInfo<float>(5f, 1.5f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        noteInfo.holdRadiusTween = new TweenInfo<float>(0f, 0f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        noteInfo.appearTween = new TweenInfo<float>(0f, 1f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        noteInfo.totalRotationTween = new TweenInfo<float>(0f, 0f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        noteInfo.startRotationTween = new TweenInfo<float>(0f, 0f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        noteInfo.endRotationTween = new TweenInfo<float>(0f, 0f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        noteInfo.startColorTween = new TweenInfo<Color>(new Color(100, 45, 250, 255) / 255f, new Color(100, 45, 250, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        noteInfo.processStartColorTween = new TweenInfo<Color>(new Color(130, 80, 255, 255) / 255f, new Color(130, 80, 255, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        noteInfo.processEndColorTween = new TweenInfo<Color>(new Color(130, 80, 255, 255) / 255f, new Color(130, 80, 255, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        noteInfo.endColorTween = new TweenInfo<Color>(new Color(100, 45, 250, 255) / 255f, new Color(100, 45, 250, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
    }
    void Init(CenterInfo centerInfo)
    {
        centerInfo.colorTween = new TweenInfo<Color>(new Color(0, 255, 160, 255) / 255f, new Color(0, 255, 160, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        centerInfo.posTween = new TweenInfo<Vector2>(Vector2.zero, Vector2.zero, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        centerInfo.scaleTween = new TweenInfo<Vector2>(Vector2.one, Vector2.one, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
    }
    void Init(BoundaryInfo boundaryInfo)
    {
        boundaryInfo.lineColorTween = new TweenInfo<Color>(new Color(0, 255, 160, 255) / 255f, new Color(0, 255, 160, 255) / 255f, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        boundaryInfo.coverColorTween = new TweenInfo<Color>(() => cameraInfo.BGColorTween.tweenStartValue, () => cameraInfo.BGColorTween.tweenEndValue, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        boundaryInfo.scaleTween = new TweenInfo<Vector2>(Vector2.one, Vector2.one, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
        boundaryInfo.posTween = new TweenInfo<Vector2>(() => centerInfo.posTween.tweenStartValue, () => centerInfo.posTween.tweenEndValue, 0.5f, AnimationCurve.Linear(0, 0, 1, 1));
    }
    void Init(JudgmentInfo judgmentInfo)
    {
        judgmentInfo.range = 0.5f;
    }
    void Init(CreditInfo creditInfo)
    {
        creditInfo.worldName = "Empty";
        creditInfo.worldEditor = "Empty";
        creditInfo.songName = "Empty";
        creditInfo.songWriter = "Empty";
    }
}
