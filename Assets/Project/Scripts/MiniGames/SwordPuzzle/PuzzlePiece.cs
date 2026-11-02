using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public int pieceId;
    private Vector3 _startPosition;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    public bool isPlaced { get; private set; } = false;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
        _startPosition = _rectTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (isPlaced) return;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        if (isPlaced) return;
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (isPlaced) return;
        _canvasGroup.blocksRaycasts = true;
    }

    public void SnapToSlot(PuzzleSlot slot) {
        isPlaced = true;
        _canvasGroup.blocksRaycasts = false;
        _rectTransform.position = slot.transform.position;
    }

    public void ResetPiece() {
        isPlaced = false;
        _rectTransform.position = _startPosition;
        _canvasGroup.blocksRaycasts = true;
    }
}