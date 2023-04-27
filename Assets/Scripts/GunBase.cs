using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName="Gun")]

public class GunBase : ScriptableObject
{
    public string weaponName;
    public float firerate;
    public float bloom;
    public int burst; // 0 semi auto | 1 auto
    public int damage;
    public float recoil;
    public float kickback;
    public float aimSpeed;
    public GameObject prefab;
}
