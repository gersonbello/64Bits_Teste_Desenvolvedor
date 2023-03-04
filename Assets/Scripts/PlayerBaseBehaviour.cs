using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseBehaviour : MonoBehaviour
{
    private CharacterController charController;
    private Animator charAnimator;

    [Header("Player Settings")]
    [Space(5)]

    [SerializeField]
    [Tooltip("Velocidade em que o personagem se move")]
    private float moveSpeed;
    [SerializeField]
    [Tooltip("Velocidade em que o personagem se rotaciona")]
    private float rotateSpeed;

    [SerializeField]
    [Tooltip("Força do soco")]
    private float punchForce;

    [Header("Stack Settings")]
    [Space(5)]

    [SerializeField]
    [Tooltip("Força do soco")]
    private Transform stackPosition;

    [SerializeField]
    [Tooltip("Quantidade máxima da pilha de npcs nas costas do personagem")]
    private int maxEnemyStack;

    [SerializeField]
    [Tooltip("Tempo até o inimigo ser adicionado à pilha")]
    private float stackWaitTime;

    [SerializeField]
    [Tooltip("Velocidade em que o inimigo é adicionado à pilha")]
    private float stackSpeed;

    private Vector2 joystickAxis;
    private Vector3 moveDirection;


    private List<Transform> enemyToStack = new List<Transform>();
    private Stack<Transform> enemyStack = new Stack<Transform>();

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        charAnimator = GetComponent<Animator>();
        Physics.IgnoreLayerCollision(6, 3);
    }
    private void Update()
    {
        GetJoystickAxis();
        SetAnimatorParameters();
    }
    private void FixedUpdate()
    {
        charController.Move(GetMovementDirection() * moveSpeed * Time.deltaTime);
        if (joystickAxis.magnitude != 0) RotateCharacterToDirection(GetMovementDirection());
        SetStackPosition();
    }

    /// <summary>
    /// Recebe os valores do joystick em um vetor
    /// </summary>
    private void GetJoystickAxis()
    {
        joystickAxis.x = Input.GetAxisRaw("Horizontal");
        joystickAxis.y = Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// Retorna a dire��o de movimento do personagem baseado no joystick
    /// </summary>
    private Vector3 GetMovementDirection()
    {
        moveDirection.x = joystickAxis.x;
        moveDirection.z = joystickAxis.y;

        return moveDirection;
    }
    /// <summary>
    /// Rotaciona o personagem na dire��o desejada na velocidade configurada
    /// </summary>
    /// <param name="directionToRotate"></param>
    private void RotateCharacterToDirection(Vector3 directionToRotate)
    {
        Quaternion newRotation = Quaternion.LookRotation(directionToRotate, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
    }

    private void SetAnimatorParameters()
    {
        charAnimator.SetFloat("axisDistance", Vector2.Distance(joystickAxis, new Vector2()));
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Enemy"))
        {
            charAnimator.SetTrigger("punch");
            col.transform.GetComponent<Animator>().enabled = false;
            Rigidbody[] enemyRigs = col.transform.GetComponentsInChildren<Rigidbody>();
            StartCoroutine(KillEnemy(enemyRigs, col));
        }
    }

    private IEnumerator KillEnemy(Rigidbody[] enemyRigs, Collider col)
    {
        yield return new WaitForSeconds(0.15f);
        foreach (Rigidbody eRig in enemyRigs)
        {
            eRig.isKinematic = false;
            eRig.AddForce((transform.forward + Vector3.up * .5f) * punchForce);
        }
        col.GetComponent<Rigidbody>().isKinematic = true;
        col.transform.gameObject.layer = 3;

        yield return new WaitForSeconds(stackWaitTime);

        enemyToStack.Add(col.transform.GetChild(0).transform);
    }
    /// <summary>
    /// Movimenta npcs derrubados
    /// </summary>
    private void SetStackPosition()
    {
        // Movimenta os npcs derrubados até a pilha
        foreach (Transform t in enemyToStack)
        {
            Vector3 newPos = stackPosition.position;
            newPos.y += enemyStack.Count * .8f;
            t.position = Vector3.Lerp(t.position, newPos, stackSpeed * Time.deltaTime);
            if(Vector3.Distance(t.position, newPos) < 1f)
            {
                enemyToStack.Remove(t);
                enemyStack.Push(t.parent);
                t.parent.position = newPos;
                t.position = newPos;
                Rigidbody rig = t.GetComponent<Rigidbody>();
                rig.isKinematic = true;
                rig.freezeRotation = true;
                break;
            }
        }

        // Movimenta npcs na pilha para a posição correta de acordo com o index
        int stackIndex = enemyStack.Count;

        foreach (Transform t in enemyStack)
        {
            Vector3 newPos = stackPosition.position;
            newPos.y += stackIndex * .8f;
            t.position = Vector3.Lerp(t.position, newPos, stackSpeed * Time.deltaTime);
            stackIndex--;
        }
    }
}
