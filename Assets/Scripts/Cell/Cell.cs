using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image image;

    public RectTransform Rect;

    public Point Point => cellData.point;
    public CellData.CellType CellType => cellData.cellType;


    private CellData cellData;
    private CellMover cellMover;

    [SerializeField] private float moveSpeed = 10f;

    private Vector2 position;
    private bool isUpdating;

    public void Initialize(CellData cellData, Sprite sprite, CellMover cellMover)
    {
        this.cellData = cellData;
        image.sprite = sprite;
        this.cellMover = cellMover;
    }

    public bool UpdateCell()
    {
        if (Vector3.Distance(Rect.anchoredPosition, position) > 1)
        {
            MoveToPosition(position);
            isUpdating = true;
        }
        else
        {
            Rect.anchoredPosition = position;
            isUpdating = false;
        }

        return isUpdating;
    }

    private void UpdateName()
    {
        transform.name = $"Cell [{Point.x}, {Point.y}]";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cellMover.MoveCell(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cellMover.DropCell();
    }

    public void MoveToPosition(Vector2 position)
    {
        Rect.anchoredPosition = Vector2.Lerp(Rect.anchoredPosition, position, Time.deltaTime * moveSpeed);
    }

    public void ResetPosition()
    {
        position = BoardService.GetBoardPositionFromPoint(Point);
    }

    public void SetCellPoint(Point point)
    {
        cellData.point = point;
        UpdateName();
        ResetPosition();
    }
}
