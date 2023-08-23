using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVERIZE;

public delegate void OnInput(int playerIndex, int keyIndex);
public class KeyInfo
{
    public KeyInfo(int count, bool[][] isInputs)
    {
        this.count = count;
        this.isInputs = isInputs;
        onInput = (i, j) =>
        {
            this.isInputs[i][j] = true;
            this.count++;
        };
    }

    int count;
    bool[][] isInputs;
    OnInput onInput;

    public int Count { get => count; set => count = value; }
    public bool[][] IsInputs { get => isInputs; set => isInputs = value; }
    public OnInput OnInput => onInput;
    public void AddOnInput(OnInput onInput) => this.onInput += onInput;
    public bool GetIsInput(int playerIndex)
    {
        foreach (var isInput in isInputs[playerIndex])
            if (isInput)
                return true;
        return false;
    }
}
public class KeyControl
{
    public KeyControl()
    {
        CanInputKeys = new KeyCode[][] { new[] { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F }, new[] { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L } };
        Down = new(0, Handy.InitArray<bool>(CanInputKeys.GetLengthes()).SetMonoValue(false));
        Press = new(0, Handy.InitArray<bool>(CanInputKeys.GetLengthes()).SetMonoValue(false));
        Up = new(0, Handy.InitArray<bool>(CanInputKeys.GetLengthes()).SetMonoValue(false));


        Up.AddOnInput((i, j) => Handy.WriteLog(nameof(Up), PlayManager.Member.GetJudgmentValue(PlayManager.Member.GetNoteScript(PlayManager.Member.levelInfoIndex)), Up.Count));
        // Down.AddOnInput((i, j) => Handy.WriteLog(nameof(Down), Down.Count));
        // Press.AddOnInput((i, j) => Handy.WriteLog(nameof(Press), Press.Count));
        // Up.AddOnInput((i, j) => Handy.WriteLog(nameof(Up), Up.Count));
        // CanInputKeys = new List<List<KeyCode>>() { new List<KeyCode>() { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F }, new List<KeyCode>() { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L } }; ;
    }
    public KeyCode[][] CanInputKeys;
    public KeyInfo Down, Press, Up;
    public KeyControl SetDefault()
    {
        return SetDefault(0, 0, 0);
    }
    public KeyControl SetDefault(int downCount, int pressCount, int upCount)
    {
        Down.Count = downCount;
        Down.IsInputs = Handy.InitArray<bool>(CanInputKeys.GetLengthes()).SetMonoValue(false);

        Press.Count = pressCount;
        Press.IsInputs = Handy.InitArray<bool>(CanInputKeys.GetLengthes()).SetMonoValue(false);

        Up.Count = upCount;
        Up.IsInputs = Handy.InitArray<bool>(CanInputKeys.GetLengthes()).SetMonoValue(false);

        return this;
    }
    public void Update()
    {
        SetDefault();
        for (int i = 0; i < CanInputKeys.Length; i++)
        {
            for (int j = 0; j < CanInputKeys[i].Length; j++)
            {
                if (Input.GetKeyDown(CanInputKeys[i][j]))
                {
                    Down.OnInput(i, j);
                    // Handy.WriteLog(nameof(Down), Down.Count);
                }
                if (Input.GetKey(CanInputKeys[i][j]))
                {
                    Press.OnInput(i, j);
                }
                if (Input.GetKeyUp(CanInputKeys[i][j]))
                {
                    Up.OnInput(i, j);
                }
            }
        }
    }
}
