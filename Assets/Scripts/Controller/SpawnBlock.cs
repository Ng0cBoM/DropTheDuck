using DG.Tweening;
using EgdFoundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    private Block nextBlock;
    private Block targetBlock;

    [SerializeField] private GameObject nextBlockContain;

    private void Awake()
    {
        SignalBus.I.Register<SpawnNewBlock>(AppearBlock);
    }

    public void SpawnNextBlock()
    {
        if (nextBlock != null) nextBlock.gameObject.SetActive(false);
        Block block = nextBlockContain.transform.GetChild(Random.Range(0, 5)).GetComponent<Block>();
        nextBlock = block;
        nextBlock.gameObject.SetActive(true);
        block.transform.localScale = Vector3.zero;
        block.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    private void AppearBlock(SpawnNewBlock signal)
    {
        if (nextBlock == null) SpawnNextBlock();
        GameManager.Instance.SpawnBlock(nextBlock, GameManager.Instance.spawnPosition);
        StartCoroutine(GameManager.Instance.MoveDown(GameManager.Instance.currentBlock));
        SpawnNextBlock();
    }

    private void OnDestroy()
    {
        SignalBus.I.Unregister<SpawnNewBlock>(AppearBlock);
    }
}