using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Rotaciona o personagem na dire��o desejada na velocidade configurada
    /// </summary>
    /// <param name="directionToRotate"></param>
    public static void RotateToDirection(this Transform t, Vector3 fowardRotatrion, Vector3 upwardRotation, float rotationSpeed)
    {
        Quaternion newRotation = Quaternion.LookRotation(fowardRotatrion, upwardRotation);
        t.transform.rotation = Quaternion.RotateTowards(t.rotation, newRotation, rotationSpeed * Time.deltaTime);
    }
}
