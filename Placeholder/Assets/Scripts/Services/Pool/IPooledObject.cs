﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooledObject
{
    void OnSpawned();
    void OnDestroy();
    ESpawnItemType GetObjectType();
}
