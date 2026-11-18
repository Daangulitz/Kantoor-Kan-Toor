using UnityEngine;

public class DollCodeGenerator : MonoBehaviour
{
    [SerializeField] private string[] dollOrder;
    [SerializeField] private GameObject[] sockets;

    private void Start()
    {
        ShuffleCode(dollOrder);
    }

    private void ShuffleCode(string[] array)
    {
        for (int i = array.Length - 1; i >= 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (array[i], array[randomIndex]) = (array[randomIndex], array[i]);

            sockets[i].GetComponent<CollorCodeSockets>().designatedCollor = array[i];
        }
    }
}
