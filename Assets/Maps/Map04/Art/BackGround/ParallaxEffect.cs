using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Transform cam;
    private Vector3 lastCamPos;

    [Range(0f, 1f)]
    public float parallaxMultiplier = 0.3f;

    private float fixedY; // giữ nguyên trục Y

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;

        fixedY = transform.position.y; // lưu Y ban đầu
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;

        // chỉ di chuyển theo X
        float newX = transform.position.x + delta.x * parallaxMultiplier;

        // giữ nguyên Y
        transform.position = new Vector3(newX, fixedY, transform.position.z);

        lastCamPos = cam.position;
    }
}