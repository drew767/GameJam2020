using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper
{
    public static Vector3 FlatVector(Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }

    public static Vector3 FromVector2(Vector2 v)
    {
        return new Vector3(v.x, 0f, v.y);
    }

    public static float SnapAngle180(float angle)
    {
        while (angle < -180f)
            angle += 360;
        while (angle > 180f)
            angle -= 360;
        return angle;
    }

}
