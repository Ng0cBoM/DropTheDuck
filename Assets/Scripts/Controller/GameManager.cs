using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/*using DG.Tweening;*/

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
        var block = listPrefabs[Random.Range(0, listPrefabs.Count)];
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
            BlockMove(block, gameSpeed);
            yield return new WaitForSeconds(0.001f);
        }
        CheckAroundBlock(block);
    }

    private void BlockMove(Block block, float value)
    {
        block.transform.position -= new Vector3(0, value, 0);
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

    public void CheckAroundBlock(Block block)
    {
        int x = (Mathf.RoundToInt(block.transform.position.x)) + 2;
        int y = Mathf.RoundToInt(block.transform.position.y);
        listBlockDestroy.Clear();
        //AlikeBlock under current block
        if (y > 0 && Grid[x, y - 1] != null)
            if (Grid[x, y - 1].numberInBlock == block.numberInBlock)
                listBlockDestroy.Add(Grid[x, y - 1]);
        //AlikeBlock left current block
        if (x > 0 && Grid[x - 1, y])
            if (Grid[x - 1, y].numberInBlock == block.numberInBlock)
                listBlockDestroy.Add(Grid[x - 1, y]);
        //AlikeBlock right current block
        if (x < GridSizeX - 1 && Grid[x + 1, y] != null)
            if (Grid[x + 1, y].numberInBlock == block.numberInBlock)
                listBlockDestroy.Add(Grid[x + 1, y]);
        if (listBlockDestroy.Count > 0)
        {
            MergeBlock(block);
        }
        else
        {
            FixBlockIntoGrid(x, y, block);
        }
    }

    private void ReplaceBlock(int amountBlockMerge, Block block)
    {
        //int numberInBlockNext = currentBlock.numberInBlock * (int)Math.Pow(2, amountBlockMerge);
        int x = (Mathf.RoundToInt(block.transform.position.x)) + 2;
        int y = Mathf.RoundToInt(block.transform.position.y);
        int numberInBlockNext = block.numberInBlock;
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
        CheckSpaceInGrid();
    }

    private void CheckSpaceInGrid()
    {
        bool breakLoop = false;
        int countNullInGrid = 0;
        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 1; j < GridSizeY; j++)
            {
                if (Grid[i, j - 1] == null && Grid[i, j] != null)
                {
                    StartCoroutine(MoveDown(Grid[i, j]));
                    Grid[i, j] = null;
                    break;
                    breakLoop = true;
                    countNullInGrid++;
                }
            }
            if (breakLoop) break;
        }
        if (countNullInGrid == 0) SpawnRandomBlock();
    }

    private void FixBlockIntoGrid(int col, int row, Block block)
    {
        block.transform.position = new Vector2(col - 2, row);
        Grid[col, row] = block;
        CheckSpaceInGrid();
    }

    private void MergeBlock(Block rootBlock)
    {
        foreach (Block block in listBlockDestroy)
        {
            int x = (Mathf.RoundToInt(block.transform.position.x)) + 2;
            int y = Mathf.RoundToInt(block.transform.position.y);
            Destroy(block.gameObject);
            Grid[x, y] = null;
        }
        ReplaceBlock(listBlockDestroy.Count, rootBlock);
    }
}