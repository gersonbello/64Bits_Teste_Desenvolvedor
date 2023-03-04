using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseBehaviour : MonoBehaviour
{
    private CharacterController charController;
    private Animator charAnimator;

    [Header("Configurações")] [Space(5)]

    [SerializeField] [Tooltip("Velocidade em que o personagem se move")]
    private float moveSpeed;
    [SerializeField]
    [Tooltip("Velocidade em que o personagem se rotaciona")]
    private float rotateSpeed;

    private Vector2 joystickAxis;
    private Vector3 moveDirection;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        charAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        GetJoystickAxis();
        SetAnimatorParameters();
    }
    private void FixedUpdate()
    {
        charController.Move(GetMovementDirection() * moveSpeed * Time.deltaTime);
        if(joystickAxis.magnitude != 0) RotateCharacterToDirection(GetMovementDirection());
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
    /// Retorna a direção de movimento do personagem baseado no joystick
    /// </summary>
    private Vector3 GetMovementDirection()
    {
        moveDirection.x = joystickAxis.x;
        moveDirection.z = joystickAxis.y;

        return moveDirection;
    }
    /// <summary>
    /// Rotaciona o personagem na direção desejada na velocidade configurada
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
}
