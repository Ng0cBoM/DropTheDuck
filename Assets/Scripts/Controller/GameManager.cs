using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Block currentBlock { get; set; }

    private const int GridSizeX = 5;
    private const int GridSizeY = 7;

    private Block[,] Grid = new Block[GridSizeX, GridSizeY];

    public float gameSpeed;

    [SerializeField]
    private List<Block> listPrefabs;

    private List<Block> listBlockDestroy = new List<Block>();

    public Vector2 spawnPosition = new Vector2(0, 6);

    public bool canDrop = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnRandomBlock();
    }

    public void SpawnRandomBlock()
    {
        canDrop = true;
        var block = listPrefabs[Random.Range(0, 5)];
        gameSpeed = 0.001f;
        SpawnBlock(block, spawnPosition);
        StartCoroutine(MoveDown(currentBlock));
    }

    public void SpawnBlock(Block blockSpawn, Vector2 spawnPosition)
    {
        currentBlock = Instantiate(blockSpawn, spawnPosition, blockSpawn.transform.rotation);
    }

    public IEnumerator MoveDown(Block block)
    {
        while (IsCanMove(block))
        {
            block.transform.position -= new Vector3(0, gameSpeed, 0);
            yield return new WaitForSeconds(0.001f);
        }
        FixBlockIntoGrid(block);
    }

    public bool IsCanMove(Block block)
    {
        return block.transform.position.y > CountBlockPerColume((int)block.transform.position.x + 2);
    }

    private int CountBlockPerColume(int colume)
    {
        int count = 0;
        for (int i = 0; i < GridSizeY; i++)
        {
            if (Grid[colume, i] != null)
            {
                count++;
            }
            else break;
        }
        return count;
    }

    private void FixBlockIntoGrid(Block block)
    {
        int x = Mathf.RoundToInt(block.transform.position.x);
        int y = Mathf.RoundToInt(block.transform.position.y);
        block.transform.position = new Vector2(x, y);
        Grid[x + 2, y] = block;
        CheckAroundBlock(block);
    }

    public void CheckAroundBlock(Block block)
    {
        int x = (Mathf.RoundToInt(block.transform.position.x)) + 2;
        int y = Mathf.RoundToInt(block.transform.position.y);
        listBlockDestroy.Clear();
        listBlockDestroy.Add(Grid[x, y]);
        //Case 1: AlikeBlock under current block
        if (y > 0 && Grid[x, y - 1] != null)
            if (Grid[x, y - 1].numberInBlock == block.numberInBlock)
                listBlockDestroy.Add(Grid[x, y - 1]);
        //Case 2: AlikeBlock left current block
        if (x > 0 && Grid[x - 1, y])
            if (Grid[x - 1, y].numberInBlock == block.numberInBlock)
                listBlockDestroy.Add(Grid[x - 1, y]);
        //Case 3: AlikeBlock right current block
        if (x < GridSizeX - 1 && Grid[x + 1, y] != null)
            if (Grid[x + 1, y].numberInBlock == block.numberInBlock)
                listBlockDestroy.Add(Grid[x + 1, y]);
        if (listBlockDestroy.Count > 1)
        {
            MergeBlock(block);
        }
        else
        {
            CheckSpaceInGrid();
        }
    }

    private void MergeBlock(Block rootBlock)
    {
        foreach (Block block in listBlockDestroy)
        {
            int x = (Mathf.RoundToInt(block.transform.position.x)) + 2;
            int y = Mathf.RoundToInt(block.transform.position.y);
            Grid[x, y] = null;
            Vector3 targetPosiotionMerge = new Vector3((int)rootBlock.transform.position.x, (int)rootBlock.transform.position.y, 0);
            if (block != rootBlock) block.transform.DOMove(targetPosiotionMerge, 0.2f).OnComplete(() =>
            {
                Destroy(block.gameObject);
            });
        }
        ReplaceBlock(listBlockDestroy.Count - 1, rootBlock);
    }

    private void ReplaceBlock(int amountBlockMerge, Block block)
    {
        int numberInBlockNext = currentBlock.numberInBlock * (int)Math.Pow(2, amountBlockMerge);
        int x = (Mathf.RoundToInt(block.transform.position.x)) + 2;
        int y = Mathf.RoundToInt(block.transform.position.y);
        for (int i = 0; i < listPrefabs.Count; i++)
        {
            if (listPrefabs[i].numberInBlock == numberInBlockNext)
            {
                Destroy(block.gameObject);
                SpawnBlock(listPrefabs[i], new Vector2(x - 2, y));
                Grid[x, y] = currentBlock;
                break;
            }
        }
        if (y > 0 && Grid[x, y - 1] == null)
            CheckSpaceInGrid();
        else
            CheckAroundBlock(Grid[x, y]);
    }

    private void CheckSpaceInGrid()
    {
        int nullInGrid = 0;
        int breakLoopCondition = 0;
        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 1; j < GridSizeY; j++)
            {
                if (Grid[i, j - 1] == null && Grid[i, j] != null)
                {
                    StartCoroutine(MoveDown(Grid[i, j]));
                    Grid[i, j] = null;
                    nullInGrid++;
                    breakLoopCondition++;
                    break;
                }
            }
            if (breakLoopCondition != 0) break;
        }
        if (nullInGrid == 0) SpawnRandomBlock();
    }
}