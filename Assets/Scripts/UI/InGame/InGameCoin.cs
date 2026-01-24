using UnityEngine;

public class InGameCoin : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.Play("CoinEnter");
    }
}
