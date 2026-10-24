using UnityEngine;
using UnityEngine.UI;

public class PuzzleSlot : MonoBehaviour {
    public int slotId; // ID слота (например, 0, 1, 2)

    [HideInInspector] public bool isFilled = false;

    public void FillSlot() {
        isFilled = true;
    }

    public void ResetSlot() {
        isFilled = false;
    }
}