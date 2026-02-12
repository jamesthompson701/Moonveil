using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//SO for a square of soil
//has an enum for weed, empty, or crop
//has a bool for tilled/untilled and wet/not wet

public enum SoilContent { empty, weed, crop }

[CreateAssetMenu(fileName = "SoilSO", menuName = "Scriptable Objects/SoilSO")]
public class SoilSO : ScriptableObject
{
    public float wetnessDuration;
}
