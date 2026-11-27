using UnityEngine;

public class DollEyes : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;
    }

    private void Update()
    {
        transform.LookAt(player);
    }
}
