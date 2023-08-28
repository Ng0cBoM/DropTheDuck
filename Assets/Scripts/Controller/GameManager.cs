using EgdFoundation;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BlockController currentBlock { get; set; }

    private const int GridSizeX = 5;
    private const int GridSizeY = 7;

    public BlockController[,] Grid = new BlockController[GridSizeX, GridSizeY];

    public float gameSpeed = 0.001f;

    [SerializeField] private List<BlockController> listPrefabs;

    public List<int> countBlockEachColume;
    public List<BlockController> listBlockDestroy;

    public Vector2 spawnPosition = new Vector2(0, 7);

    private void Awake()
    {
        Instance = this;
        Initiation();
    }

    private void Initiation()
    {
        for (int i = 0; i < GridSizeX; i++)
        {
            countBlockEachColume.Add(0);
        }
    }

    private void Start()
    {
        SpawnRandomBlock();
    }

    public void SpawnRandomBlock()
    {
        int index = Random.Range(0, listPrefabs.Count);
        var blockController = listPrefabs[index];
        SpawnBlock(blockController, spawnPosition);
    }

    public void SpawnBlock(BlockController blockSpawn, Vector2 spawnPosition)
    {
        var newBlock = Instantiate(blockSpawn, spawnPosition, blockSpawn.transform.rotation);
        currentBlock = newBlock;
        gameSpeed = 0.001f;
    }

    public void CheckAroundBlock(Vector2 coordinate)
    {
        int x = (Mathf.RoundToInt(coordinate.x)) + 2;
        int y = Mathf.RoundToInt(coordinate.y);
        listBlockDestroy.Clear();
        if (y > 0 && Grid[x, y - 1] != null)
            if (Grid[x, y - 1].numberInBlock == currentBlock.numberInBlock)
                listBlockDestroy.Add(Grid[x, y - 1]);
        if (x > 0 && Grid[x - 1, y])
            if (Grid[x - 1, y].numberInBlock == currentBlock.numberInBlock)
                listBlockDestroy.Add(Grid[x - 1, y]);
        if (x < GridSizeX - 1 && Grid[x + 1, y] != null)
            if (Grid[x + 1, y].numberInBlock == currentBlock.numberInBlock)
                listBlockDestroy.Add(Grid[x + 1, y]);
        if (listBlockDestroy.Count > 0)
        {
            //listBlockDestroy.Add(currentBlock);
            DestroyBlock();
            StartCoroutine(currentBlock.MoveDown());
            //SpawnBlock(currentBlock, new Vector2(x - 2, y));
        }
        else
        {
            FixBlockIntoGrid(x, y);
        }
    }

    private void FixBlockIntoGrid(int col, int row)
    {
        countBlockEachColume[col]++;
        currentBlock.transform.position = new Vector2(col - 2, row);
        Grid[col, row] = currentBlock;
        SpawnRandomBlock();
    }

    private void DestroyBlock()
    {
        foreach (BlockController block in listBlockDestroy)
        {
            int x = (Mathf.RoundToInt(block.transform.position.x)) + 2;
            countBlockEachColume[x]--;
            Destroy(block.gameObject);
        }
        SignalBus.I.FireSignal<UpdateGrid>(new UpdateGrid());
        /*UpdateGrid();*/
    }

    /* private void UpdateGrid()
     {
         for (int i = 0; i < GridSizeX; i++)
         {
             for (int j = 1; j < GridSizeY; j++)
             {
                 if (Grid[i, j] != null && Grid[i, j - 1] == null)
                 {
                     StartCoroutine(Grid[i, j].MoveDown());
                 }
             }
         }
     }*/
}