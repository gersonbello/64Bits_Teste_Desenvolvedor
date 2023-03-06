using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseBehaviour : MonoBehaviour
{
    private CharacterController charController;
    private Animator charAnimator;
    private CameraFollow mainCamera;

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

    [SerializeField]
    [Tooltip("Velocidade em que a pilha de inimigos se movimenta e recebe inércia")]
    private float stackMovementSpeed;

    [SerializeField]
    [Tooltip("Velocidade em que a velocidade da unidade é multiplicada para mover-se mais suavemente quanto mais distânte do ínicio da pilha")]
    [Range(0, 5)]
    private float stackSpeedMultiplier;

    [SerializeField]
    [Tooltip("Velocidade em que a pilha de inimigos gira e recebe inércia")]
    private float stackRotationSpeed;

    [SerializeField]
    [Tooltip("Distância entre objetos da pilha")]
    private float stackDistance;

    private Vector2 joystickAxis;
    private Vector3 moveDirection;


    private List<Transform> enemyToStack = new List<Transform>();

    [SerializeField]
    private List<Transform> enemyStack = new List<Transform>();

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        charAnimator = GetComponent<Animator>();
        mainCamera = Camera.main.GetComponent<CameraFollow>();
        Physics.IgnoreLayerCollision(6, 3);
        Physics.IgnoreLayerCollision(3, 3);
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
        charAnimator.SetFloat("axisDistance", Vector2.Distance(joystickAxis, Vector2.zero));
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Enemy") && enemyToStack.Count + enemyStack.Count < maxEnemyStack)
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
            Vector3 newPos = enemyStack.Count > 0 ? enemyStack[enemyStack.Count - 1].transform.position + enemyStack[enemyStack.Count - 1].up * stackDistance : stackPosition.position;
            t.position = Vector3.Lerp(t.position, newPos, stackSpeed * Time.deltaTime);
            if(Vector3.Distance(t.position, newPos) < 2)
            {
                enemyToStack.Remove(t);
                enemyStack.Add(t.parent);

                Rigidbody rig = t.GetComponent<Rigidbody>();
                rig.isKinematic = true;
                rig.freezeRotation = true;

                t.parent.position = newPos;
                t.position = newPos;
                t.gameObject.layer = 3;

                mainCamera.SetNewOffset(enemyStack.Count/3);
                break;
            }
        }

        // Movimenta npcs na pilha para a posição correta de acordo com o index
        Vector3 refVelocity = Vector3.zero;

        if (enemyStack.Count > 0) enemyStack[0].transform.position = stackPosition.position;
        for (int i = 1; i < enemyStack.Count; i++)
        {
            Vector3 newPos = enemyStack[i - 1].transform.position + enemyStack[i - 1].up * stackDistance;

            enemyStack[i].position = Vector3.SmoothDamp(enemyStack[i].position, newPos, ref refVelocity,
                // Cria movimentação mais lenta para objetos mais acima da pilha
                (Mathf.Max(1, (1 + i * stackSpeedMultiplier) / enemyStack.Count)) * stackMovementSpeed * Time.deltaTime /
                // Cria movimentação mais rápida para objetos mais distântes da posição ideal da pilha
                Mathf.Max(1, Vector3.Distance(enemyStack[i].position, newPos)));

            // Renova a posição desejada, levando em conta a parte superior do item abaixo e rotaciona de forma correta
            newPos = enemyStack[i - 1].transform.position;
            Vector3 directionToRotate = (newPos - enemyStack[i].position);
            Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, -directionToRotate);
            enemyStack[i].rotation = Quaternion.RotateTowards(enemyStack[i].rotation, newRotation, stackRotationSpeed);
        }
    }
}
