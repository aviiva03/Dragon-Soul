using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDamageTester : MonoBehaviour {

    [SerializeField] private Health _health;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
            _health.TakeDamage(10, Vector3.zero);
    }
}
