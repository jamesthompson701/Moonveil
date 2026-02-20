using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//SO for a square of soil
//has an enum for weed, empty, or crop

public enum SoilContent { empty, weed, crop }

[CreateAssetMenu(fileName = "SoilSO", menuName = "Scriptable Objects/SoilSO")]
public class SoilSO : ScriptableObject
{
    public float wetnessDuration;
}
