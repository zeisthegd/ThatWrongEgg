using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Movement")]
public class PhysicsSettings : ScriptableObject
{
    [Header(" ------Movement ------")]
    [Range(0, 1)][Min(0)] public float RunPower;
    [Min(0)] public float Friction;


    [Header(" ------Acceleration and Deceleration ------")]
    [Min(0)] public float Accelleration;
    [Min(0)] public float Decelleration;
    [Header(" ------Raycasters ------")]
    [Min(0)] public float WallCastDistance;
    [Min(0)] public float GroundCastDistance;
    public LayerMask GroundWallMask;
}
