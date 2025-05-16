using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle, Move, Attack, Dead
    }

    private Animator _animator;
    private State _curState = State.Idle;

    private void Awake() 
    { 
        _animator = GetComponent<Animator>(); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _curState = State.Idle;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            _curState = State.Move;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _curState = State.Attack;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _curState = State.Dead;
        }

        switch (_curState)
        {
            case State.Idle:
                _animator.SetFloat("Speed", 0);
                break;
            case State.Move:
                _animator.SetFloat("Speed", 1.0f);
                break;
            case State.Attack:
                _animator.SetTrigger("Attack");
                break;
            case State.Dead:
                _animator.SetTrigger("Dead");
                break;
        }
    }
}
