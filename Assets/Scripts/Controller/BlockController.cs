using EgdFoundation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public int numberInBlock;
    public int blockDestination;

    private void Awake()
    {
        SignalBus.I.Register<UpdateGrid>(DropBlockHandle);
    }

    private void Start()
    {
        StartCoroutine(MoveDown());
    }

    public IEnumerator MoveDown()
    {
        while (true)
        {
            var delay = 0.001f;
            yield return new WaitForSeconds(delay);

            if (IsCanMove())
                Move(GameManager.Instance.gameSpeed);
            else
            {
                GameManager.Instance.CheckAroundBlock(transform.position);
                break;
            }
        }
    }

    private void Move(float value)
    {
        var position = transform.position;
        position.y -= value;
        transform.position = position;
    }

    public bool IsCanMove()
    {
        if (transform.position.y <= GameManager.Instance.countBlockEachColume[(int)transform.position.x + 2])
        {
            return false;
        }
        return true;
    }

    private void DropBlockHandle(UpdateGrid signal)
    {
        if (IsCanMove())
            StartCoroutine(MoveDown());
    }

    private void OnDestroy()
    {
        SignalBus.I.Unregister<UpdateGrid>(DropBlockHandle);
    }
}