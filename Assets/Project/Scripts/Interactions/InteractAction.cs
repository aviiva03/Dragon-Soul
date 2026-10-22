using UnityEngine;

// Базовый класс для любых действий.
// Наследники будут переопределять метод Execute.
public abstract class InteractAction : MonoBehaviour {
    public abstract void Execute();
}