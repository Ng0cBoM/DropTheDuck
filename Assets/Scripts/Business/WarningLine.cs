using UnityEngine;
using DG.Tweening;

public class WarningLine : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}