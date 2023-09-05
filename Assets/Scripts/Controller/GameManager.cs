using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using DG.Tweening;
using System;
using EgdFoundation;
using Core.Framework;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool gamePlaying = true;

    [SerializeField] private GameObject warningLine;
    [SerializeField] private List<Block> listPrefabs;

    private const int GridSizeX = 5;
    private const int GridSizeY = 7;

    [HideInInspector] public Block currentBlock { get; set; }
    [HideInInspector] public float gameSpeed;
    [HideInInspector] public Vector2 spawnPosition = new Vector2(0, 6);
    [HideInInspector] public bool canDrop = true;

    private Block[,] Grid = new Block[GridSizeX, GridSizeY];
    private List<Block> listBlockDestroy = new List<Block>();
    private int score = 0;

    private void Awake()
    {
        Instance = this;
        SignalBus.I.Register<ContinuePlay>(ContinueGame);
    }

    private void Start()
    {
        SpawnNewBlock();
    }

    public void SpawnNewBlock()
    {
        canDrop = true;
        gameSpeed = 0.001f;
        CheckGridToWarningOrGameOver();
    }

    public void SpawnBlock(Block blockSpawn, Vector2 spawnPosition)
    {
        currentBlock = Instantiate(blockSpawn, spawnPosition, blockSpawn.transform.rotation);
        currentBlock.transform.localScale = Vector3.zero;
        currentBlock.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public IEnumerator MoveDown(Block block)
    {
        while (IsCanMove(block))
        {
            block.transform.position -= new Vector3(0, gameSpeed, 0);
            yield return new WaitForSeconds(0.001f);
        }
        gameSpeed = 0.15f;
        FixBlockIntoGrid(block);
    }

    public bool IsCanMove(Block block)
    {
        return block.transform.position.y > CountBlockPerColume((int)block.transform.position.x + 2);
    }

    public int CountBlockPerColume(int colume)
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
            score += block.numberInBlock;
            SignalBus.I.FireSignal<UpdateScore>(new UpdateScore(score));
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
                    StartCoroutine(MoveDownContinue(Grid[i, j]));
                    Grid[i, j] = null;
                    nullInGrid++;
                    breakLoopCondition++;
                    break;
                }
            }
            if (breakLoopCondition != 0) break;
        }
        if (nullInGrid == 0) SpawnNewBlock();
    }

    private IEnumerator MoveDownContinue(Block block)
    {
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(MoveDown(block));
    }

    private void GameOver()
    {
        if (score > DataManager.I.UserData.HighestScore)
        {
            DataManager.I.UpdateHighestScore(score);
        }
        gamePlaying = false;
        ScreenData screenData = new ScreenData();
        screenData.screenData = new Dictionary<string, object>();
        screenData.screenData["score"] = score;
        UiManager.I.Push("GameOverPopup", screenData);
    }

    private void CheckGridToWarningOrGameOver()
    {
        bool checkWarning = false;
        bool checkGameOver = false;
        for (int i = 0; i < GridSizeX; i++)
        {
            if (CountBlockPerColume(i) == 5)
            {
                warningLine.SetActive(true);
                checkWarning = true;
            }
            if (CountBlockPerColume(i) == 6)
            {
                checkGameOver = true;
                break;
            }
        }
        if (!checkWarning) warningLine.SetActive(false);
        if (checkGameOver) GameOver();
        else SignalBus.I.FireSignal<SpawnNewBlock>(new SpawnNewBlock());
    }

    private void ContinueGame(ContinuePlay signal)
    {
        StartCoroutine(DestroyBlock());
    }

    private IEnumerator DestroyBlock()
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < GridSizeX; i++)
        {
            int column = i;
            for (int j = 0; j < 3; j++)
            {
                Block blockNeedDestroy = Grid[column, CountBlockPerColume(column) - 1];
                blockNeedDestroy.transform.DOScale(0f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    Destroy(blockNeedDestroy.gameObject);
                });
                Grid[column, CountBlockPerColume(column) - 1] = null;
            }
        }

        gamePlaying = true;
        SignalBus.I.FireSignal<SpawnNewBlock>(new SpawnNewBlock());
    }

    private void OnDestroy()
    {
        SignalBus.I.Unregister<ContinuePlay>(ContinueGame);
    }
}