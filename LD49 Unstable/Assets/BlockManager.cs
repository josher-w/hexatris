using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager blockManager;

    [Header("Ground")]
    public LayerMask groundLayer;

    [Header("Fall")]
    public float fallDist;
    public float fallFor;

    public float slowMult;
    public float fastMult;

    [Header("Rotate")]
    public float rotateFor;
    public float inBetweenRotates;

    public float touchSwipeLimit;
    public float sideThreshold;

    private void Awake()
    {
        blockManager = this;
    }
}
