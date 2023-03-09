using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBaseBehaviour : MonoBehaviour
{
    #region Variables

    _GameController gc;

    #region References
    private CharacterController charController;
    private Animator charAnimator;
    private CameraFollow mainCamera;
    #endregion
    #region Player Settings
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

    [SerializeField]
    [Tooltip("Material que terá a cor alterada")]
    private Material playerSkinMaterial;
    #endregion
    #region Stack Settings
    [Header("Stack Settings")]
    [Space(5)]

    [SerializeField]
    [Tooltip("Força do soco")]
    private Transform stackPosition;

    [Tooltip("Quantidade máxima da pilha de npcs nas costas do personagem")]
    public int maxEnemyStack { get; private set; } = 5;

    private int currentStackCount;

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
    [Tooltip("Velocidade em que a pilha de inimigos gira na direção do personagem")]
    private float stackLookRotationSpeed;

    [SerializeField]
    [Tooltip("Distância entre objetos da pilha")]
    private float stackDistance;

    private List<Transform> enemyToStack = new List<Transform>();

    [SerializeField]
    public List<Transform> enemyStack { get; private set; } = new List<Transform>();

    #endregion

    private Vector2 joystickAxis;
    private Vector3 moveDirection;

    #endregion

    #region Unity Base Behaviours
    private void Start()
    {
        charController = GetComponent<CharacterController>();
        charAnimator = GetComponent<Animator>();
        mainCamera = Camera.main.GetComponent<CameraFollow>();
        Physics.IgnoreLayerCollision(6, 3);
        Physics.IgnoreLayerCollision(3, 3);

        playerSkinMaterial.color = Color.white;

        gc = _GameController.gameController;
    }
    private void Update()
    {
        GetJoystickAxis();
        SetAnimatorParameters();
    }
    private void FixedUpdate()
    {
        charController.Move(GetMovementDirection(mainCamera.transform) * moveSpeed * Time.deltaTime);
        if (joystickAxis.magnitude != 0)
        {
            Vector3 oldFoward = transform.forward;
            transform.RotateToDirection(GetMovementDirection(mainCamera.transform), Vector3.up, rotateSpeed);
            //StackRotate(Vector3.Angle(oldFoward, transform.forward));
        }
        SetStackPosition();
    }
    #endregion

    #region Movement & Animations
    /// <summary>
    /// Recebe os valores do joystick em um vetor
    /// </summary>
    private void GetJoystickAxis()
    {       
        joystickAxis = gc.joystick.GetJoystickAxis();
    }

    /// <summary>
    /// Retorna a dire��o de movimento do personagem baseado no joystick
    /// </summary>
    private Vector3 GetMovementDirection(Transform relativeTransform)
    {
        Vector3 relativeRight = relativeTransform.right;
        relativeRight.y = 0;

        Vector3 relativeFoward = relativeTransform.forward;
        relativeFoward.y = 0;

        Vector3 newMoveDirection = joystickAxis.x * relativeRight.normalized;
        newMoveDirection += joystickAxis.y * relativeFoward.normalized;
        moveDirection = newMoveDirection;

        return moveDirection;
    }
    private void SetAnimatorParameters()
    {
        charAnimator.SetFloat("axisDistance", Vector2.Distance(joystickAxis, Vector2.zero));
    }
    #endregion

    #region Punch and Stack
    private IEnumerator KillEnemy(Rigidbody[] enemyRigs, Collider col)
    {
        yield return new WaitForSeconds(0.1f);
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
        for (int i = 0; i <  enemyToStack.Count; i++)
        {
            Vector3 newPos = enemyStack.Count > 0 ?
                enemyStack[enemyStack.Count - 1].transform.position + enemyStack[enemyStack.Count - 1].up * stackDistance + enemyStack[enemyStack.Count - 1].up * (i + 1) :
                stackPosition.position;
            enemyToStack[i].position = Vector3.Lerp(enemyToStack[i].position, newPos, stackSpeed * Time.deltaTime);
            if(Vector3.Distance(enemyToStack[i].position, newPos) < 2)
            {
                Transform t = enemyToStack[i];
                enemyToStack.Remove(t);
                enemyStack.Add(t.parent);

                t.parent.position = newPos;

                Rigidbody rig = t.GetComponent<Rigidbody>();
                rig.isKinematic = true;
                rig.freezeRotation = true;

                t.gameObject.layer = 3;

                t.parent.forward = transform.forward;
                t.localPosition = Vector3.zero;

                mainCamera.SetNewOffset(enemyStack.Count/3);
                break;
            }
        }

        // Movimenta npcs na pilha para a posição correta de acordo com o index

        Vector3 refVelocity = Vector3.zero;

        if (enemyStack.Count > 0)
        {
            enemyStack[0].transform.position = stackPosition.position;
            enemyStack[0].transform.up = transform.up;
        }


        for (int i = 1; i < enemyStack.Count; i++)
        {
            Vector3 newPos = enemyStack[i - 1].transform.position + enemyStack[i - 1].up * stackDistance;

            enemyStack[i].position = Vector3.SmoothDamp(enemyStack[i].position, newPos, ref refVelocity,
                // Cria movimentação mais lenta para objetos mais acima da pilha
                (Mathf.Max(1, (1 + i * stackSpeedMultiplier) / enemyStack.Count)) * stackMovementSpeed * Time.deltaTime /
                // Cria movimentação mais rápida para objetos mais distântes da posição ideal da pilha
                Mathf.Max(1, Vector3.Distance(enemyStack[i].position, newPos)));

            // Rotaciona na direção do item anterior na pilha
            newPos = enemyStack[i - 1].position;
            Vector3 directionToRotate = (newPos - enemyStack[i].position);
            enemyStack[i].up = Vector3.Lerp(enemyStack[i].up, -directionToRotate, stackRotationSpeed * Time.deltaTime);

        }
    }

    public List<Transform> GetAndClearStackList()
    {
        List<Transform> enemyList = new List<Transform>();
        foreach (Transform t in enemyStack) enemyList.Add(t);
        currentStackCount = 0;
        enemyStack.Clear();
        return enemyList;
    }

    #endregion

    #region Collision
    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Enemy") && currentStackCount < maxEnemyStack)
        {
            currentStackCount++;
            charAnimator.SetTrigger("punch");
            col.transform.GetComponent<Animator>().enabled = false;
            Rigidbody[] enemyRigs = col.transform.GetComponentsInChildren<Rigidbody>();
            StartCoroutine(KillEnemy(enemyRigs, col));
        }
        if (col.transform.CompareTag("Trigger"))
        {
            col.GetComponent<TriggerEvent>().EnterTriggerEvents();
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.transform.CompareTag("Trigger"))
        {
            col.GetComponent<TriggerEvent>().ExitTriggerEvents();
            col.transform.localScale = new Vector3(1, 1, 1);
            col.GetComponentInChildren<Mask>().GetComponent<Image>().fillAmount = 0;
        }
    }
    #endregion

    public void LevelUP()
    {
        punchForce *= 1.25f;
        punchForce = Mathf.Min(punchForce, 5000);
        moveSpeed *= 1.05f;
        moveSpeed = Mathf.Min(moveSpeed, 12);
        maxEnemyStack += 5;
        maxEnemyStack = Mathf.Min(maxEnemyStack, 100);
        playerSkinMaterial.color = Random.ColorHSV();
    }
}
