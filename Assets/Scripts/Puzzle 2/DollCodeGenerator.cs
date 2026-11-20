using UnityEngine;
using System.Linq;

public class DollCodeGenerator : MonoBehaviour
{
    [SerializeField] private string[] dollOrder;
    [SerializeField] private GameObject[] sockets;
    [SerializeField] private bool[] socketCollorMatches;

    private void Start()
    {
        ShuffleCode(dollOrder);
    }
    private void Update()
    {
        CheckCode(sockets);
    }

    private void ShuffleCode(string[] shuffelArray)
    {
        for (int i = shuffelArray.Length - 1; i >= 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (shuffelArray[i], shuffelArray[randomIndex]) = (shuffelArray[randomIndex], shuffelArray[i]);

            sockets[i].GetComponent<CollorCodeSockets>().designatedCollor = shuffelArray[i];
        }
    }

    private void CheckCode(GameObject[] socketArray)
    {
        for (int i = socketArray.Length - 1; i >= 0; i--)
        {
            if (socketArray[i].GetComponent<CollorCodeSockets>().CollorMatch == true)
            {
                socketCollorMatches[i] = true;
            }
            else
            {
                socketCollorMatches[i] = false;
            }
        }

        if (socketCollorMatches.All(match => match))
        {
            FinishPuzzle();
        }
    }

    private void FinishPuzzle()
    {
        Debug.Log("yayayayaya you did it UwU proud of you my boy");
    }
}
