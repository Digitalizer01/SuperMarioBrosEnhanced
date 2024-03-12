using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private BoxCollider2D limitsCollider;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (!player)
            player = GameObject.Find("Player");
    }

    private void LateUpdate()
    {
        if (player != null && limitsCollider != null)
        {
            Vector3 targetPosition = transform.position;
            float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

            float clampedX = Mathf.Clamp(
                player.transform.position.x,
                limitsCollider.bounds.min.x + cameraHalfWidth,
                limitsCollider.bounds.max.x - cameraHalfWidth
            );

            targetPosition.x = clampedX;
            transform.position = targetPosition;
        }
    }
}
