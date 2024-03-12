using UnityEngine;

public class CameraLimit : MonoBehaviour
{
    public Transform target; // El objetivo que sigue la cámara
    public BoxCollider2D limitsCollider; // El BoxCollider de límites

    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target != null && limitsCollider != null)
        {
            Vector3 targetPosition = target.position;
            targetPosition.y = transform.position.y; // Mantiene la posición Y

            // Verifica si la posición X de la cámara está dentro del BoxCollider
            if (
                !Physics.CheckBox(
                    targetPosition,
                    new Vector3(
                        mainCamera.orthographicSize * mainCamera.aspect,
                        mainCamera.orthographicSize,
                        0f
                    ),
                    Quaternion.identity,
                    LayerMask.GetMask("Limits")
                )
            )
            {
                // Si está fuera de los límites, restringe la posición X de la cámara
                float limitedX = Mathf.Clamp(
                    targetPosition.x,
                    limitsCollider.bounds.min.x + mainCamera.orthographicSize * mainCamera.aspect,
                    limitsCollider.bounds.max.x - mainCamera.orthographicSize * mainCamera.aspect
                );

                targetPosition.x = limitedX;
            }

            transform.position = targetPosition;
        }
    }
}
