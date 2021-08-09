using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class NeedyHotate : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo bomb;
    public KMNeedyModule Module;
    public KMSelectable[] Buttons;
    public AudioClip[] KoroneSounds;
    public TextMesh[] Text;

    private static int _moduleIdCounter = 1;
    private int _moduleId;
    private int activationCount = 0;
    private int Hotate;
    private bool active;
    private bool bombSolved;

    void Awake()
    {
        _moduleId = _moduleIdCounter++;
        foreach (KMSelectable button in Buttons)
        {
            button.OnInteract += delegate
            {
                buttonHandle(button);
                return false;
            };
        }
        Module.OnNeedyActivation += OnNeedyActivation;
        Module.OnNeedyDeactivation += OnNeedyDeactivation;
        Module.OnTimerExpired += OnTimerExpired;
        bomb.OnBombExploded += delegate () { bombSolved = true; };
        bomb.OnBombSolved += delegate () { bombSolved = true; };
    }

    void Start()
    {
        for (int i = 0; i < Buttons.Count(); i++)
        {
            Text[i].text = "";
        }
    }
    void buttonHandle(KMSelectable button)
    {
        button.AddInteractionPunch(.5f);
        if (!active)
            return;
        Debug.LogFormat("[Needy Hotate #{0}] Pressed button: {1}", _moduleId, button.GetComponentInChildren<TextMesh>().text);
        int n = UnityEngine.Random.Range(7, 11);
        switch (button.GetComponentInChildren<TextMesh>().text)
        {
            case "HO":
                if (Hotate == 0 && activationCount % 2 == 1)
                {
                    Audio.PlaySoundAtTransform("Ho1", transform);
                    Hotate++;
                }
                else if (Hotate == 2 && activationCount % 2 == 0)
                {
                    Audio.PlaySoundAtTransform("Ho2", transform);
                    Hotate = 0;
                    Module.HandlePass();
                    OnNeedyDeactivation();
                    Debug.LogFormat("[Needy Hotate #{0}] TE TA HO pressed correctly, module passed.", _moduleId);
                }
                else
                {
                    Audio.PlaySoundAtTransform(KoroneSounds[n].name, transform);
                    Module.HandleStrike();
                    Module.HandlePass();
                    OnNeedyDeactivation();
                    Hotate = 0;
                    Debug.LogFormat("[Needy Hotate #{0}] HO pressed at the wrong time, strike.", _moduleId);
                }
                break;
            case "TA":
                if (Hotate == 1)
                {
                    if (activationCount % 2 == 1)
                    {
                        Audio.PlaySoundAtTransform("Ta1", transform);
                    }
                    else
                    {
                        Audio.PlaySoundAtTransform("Ta2", transform);
                    }
                    Hotate++;
                }
                else
                {
                    Audio.PlaySoundAtTransform(KoroneSounds[n].name, transform);
                    Module.HandleStrike();
                    Module.HandlePass();
                    OnNeedyDeactivation();
                    Hotate = 0;
                    Debug.LogFormat("[Needy Hotate #{0}] TA pressed at the wrong time, strike.", _moduleId);
                }
                break;
            case "TE":
                if (Hotate == 0 && activationCount % 2 == 0)
                {
                    Audio.PlaySoundAtTransform("Te2", transform);
                    Hotate++;
                }
                else if (Hotate == 2 && activationCount % 2 == 1)
                {
                    Audio.PlaySoundAtTransform("Te1", transform);
                    Hotate = 0;
                    Module.HandlePass();
                    OnNeedyDeactivation();
                    Debug.LogFormat("[Needy Hotate #{0}] HO TA TE pressed correctly, module passed.", _moduleId);
                }
                else
                {
                    Audio.PlaySoundAtTransform(KoroneSounds[n].name, transform);
                    Module.HandleStrike();
                    Module.HandlePass();
                    OnNeedyDeactivation();
                    Hotate = 0;
                    Debug.LogFormat("[Needy Hotate #{0}] TE pressed at the wrong time, strike.", _moduleId);
                }
                break;
            default:
                Audio.PlaySoundAtTransform(KoroneSounds[n].name, transform);
                Module.HandleStrike();
                Module.HandlePass();
                OnNeedyDeactivation();
                Hotate = 0;
                Debug.LogFormat("[Needy Hotate #{0}] Wrong button pressed, strike.", _moduleId);
                break;
        }
    }

    protected void OnNeedyActivation()
    {
        active = true;
        activationCount++;
        int FUNVALUE = UnityEngine.Random.Range(0, 100);
        if (FUNVALUE == 69)
        {
            Audio.PlaySoundAtTransform("Mike Tyson", transform);
        }
        else
        {
           Audio.PlaySoundAtTransform("yubi yubi", transform);
        }
        Debug.LogFormat("[Needy Hotate #{0}] Number of activations: {1}", _moduleId, activationCount);
        Hotate = 0;
        string[] chosenPairs = new string[] { "HO", "TA", "TE", "", "", "", "", "", "", "" };
        string[] allPairs = { "AH", "AT", "EH", "ET", "HA", "HE", "OH", "OT", "TO" };
        int k = 6;
        for (int i = 3; i < 9; i++)
        {
            int n = UnityEngine.Random.Range(0, k);
            chosenPairs[i] = allPairs[n];
            for (int j = n; j < 8; j++)
            {
                allPairs[j] = allPairs[j + 1];
            }
            k--;
        }
        int l = 8;
        string sequence = "";
        for (int i = 0; i < 9; i++)
        {
            int n = UnityEngine.Random.Range(0, l);
            Text[i].text = chosenPairs[n];
            sequence = sequence + " " + chosenPairs[n]; 
            for (int j = n; j < 8; j++)
            {
                chosenPairs[j] = chosenPairs[j + 1];
            }
            l--;
        }
        Debug.LogFormat("[Needy Hotate #{0}] Arrangement of buttons are {1}.", _moduleId, sequence);
        if (activationCount % 2 == 1) { Debug.LogFormat("[Needy Hotate #{0}] You must press HO TA TE.", _moduleId); }
        else { Debug.LogFormat("[Needy Hotate #{0}] You must press TE TA HO.", _moduleId); }
    }

    protected void OnNeedyDeactivation()
    {
        for (int i = 0; i < Buttons.Count(); i++)
        {
            Text[i].text = "";
        }
        active = false;
    }

    protected void OnTimerExpired()
    {
        int n = UnityEngine.Random.Range(7, 11);
        Audio.PlaySoundAtTransform(KoroneSounds[n].name, transform);
        Module.HandleStrike();
        Hotate = 0;
        for (int i = 0; i < Buttons.Count(); i++)
        {
            Text[i].text = "";
        }
    }
}