using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance.canDrop)
        {
            if (Input.GetMouseButton(0) && GameManager.Instance.gamePlaying)
            {
                SetPositionBlock();
            }
            if (Input.GetMouseButtonUp(0) && GameManager.Instance.gamePlaying)
            {
                GameManager.Instance.canDrop = false;
                GameManager.Instance.gameSpeed = 0.2f;
                GameManager.Instance.spawnPosition = new Vector2(GetColumeMouse(), 6);
            }
        }
    }

    private int GetColumeMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x >= -2.5 && mousePosition.x <= -1.5)
        {
            return -2;
        }
        if (mousePosition.x >= -1.5 && mousePosition.x <= -0.5)
        {
            return -1;
        }
        if (mousePosition.x >= -0.5 && mousePosition.x <= 0.5)
        {
            return 0;
        }
        if (mousePosition.x >= 0.5 && mousePosition.x <= 1.5)
        {
            return 1;
        }
        if (mousePosition.x >= 1.5 && mousePosition.x <= 2.5)
        {
            return 2;
        }
        return 0;
    }

    private void SetPositionBlock()
    {
        var current = GameManager.Instance.currentBlock.transform;
        var position = current.position;
        if (position.y > GameManager.Instance.CountBlockPerColume(GetColumeMouse() + 2))
            position.x = GetColumeMouse();
        current.position = position;
    }
}