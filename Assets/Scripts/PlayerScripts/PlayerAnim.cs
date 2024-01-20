using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator animator;
      private void Awake() {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        Animate();
    }

     private void Animate() {
        if (animator != null && UIManager.instance.levelState == LevelState.Playing) {
        animator.SetBool("isTouched", true);
        }
    }
}
