using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsLayers
{
    public static int MaskDefault = 1 << LayerMask.NameToLayer("Default");
    public static int MaskPlayerCollider = 1 << LayerMask.NameToLayer("PlayerCollider");
    public static int MaskPlayerGroundedProbeCollider = 1 << LayerMask.NameToLayer("PlayerCollider");

    public static int MaskNotPlayer = ~(MaskPlayerCollider | MaskPlayerGroundedProbeCollider);
}
