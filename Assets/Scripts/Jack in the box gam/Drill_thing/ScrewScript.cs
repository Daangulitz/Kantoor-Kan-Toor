using UnityEngine;

public class ScrewScript : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private float releaseSpeed = 20f;
    [SerializeField] private float moveDistance = 0.03f;

    private bool isTurning = false;
    private bool isReleased = false;
    private float released = 0f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Drill"))
            isTurning = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Drill"))
            isTurning = false;
    }

    void Update()
    {
        if (!isTurning || isReleased) return;

        float step = releaseSpeed * Time.deltaTime;
        released += step;

        transform.Rotate(0f, 0f, turnSpeed * Time.deltaTime, Space.Self);

        transform.Translate(0f, 0f, (moveDistance / 100f) * step, Space.Self);

        if (released >= 100f)
        {
            isReleased = true;
            isTurning = false;
            rb.isKinematic = false;

            GetComponentInParent<LidScript>().ScrewDone();
        }
    }
}