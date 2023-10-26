using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KilledCell : MonoBehaviour
{
    [HideInInspector] public bool isFalling;

    [SerializeField] private float speed = 16f;
    [SerializeField] private float gravity = 32f;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image image;

    private Vector2 moveDirection;

    public void Initialize(Sprite sprite, Vector2 startPosition)
    {
        isFalling = true;
        moveDirection = Vector2.up;
        moveDirection.x = Random.Range(-1f, 1f);
        moveDirection *= speed / 2;

        image.sprite = sprite;
        rect.anchoredPosition = startPosition;

        StartCoroutine(WaitForDeath());
    }

    private IEnumerator WaitForDeath()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isFalling)
        {
            return;
        }

        moveDirection.y -= Time.deltaTime * gravity;
        moveDirection.x = Mathf.Lerp(moveDirection.x, 0, Time.deltaTime);
        rect.anchoredPosition += moveDirection * (Time.deltaTime * speed);

        if (rect.position.x < -Config.PieceSize 
            || rect.position.x > Screen.width * Config.PieceSize 
            || rect.position.y < -Config.PieceSize
            || rect.position.y > Screen.height * Config.PieceSize)
        {
            isFalling = false;
        }
    }
}
