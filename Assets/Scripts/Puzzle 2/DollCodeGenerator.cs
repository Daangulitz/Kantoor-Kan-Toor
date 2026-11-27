using UnityEngine;
using System.Linq;
using TMPro;

public class DollCodeGenerator : MonoBehaviour
{
    [SerializeField] private string[] dollOrder;
    [SerializeField] private GameObject[] sockets;
    [SerializeField] private bool[] socketCollorMatches;
    [SerializeField] private TextMeshProUGUI CodeText;
    private void Start()
    {
        ShuffleCode(dollOrder);
    }
    private void Update()
    {
        CheckCode(sockets);
    }

    private void ShuffleCode(string[] shuffleArray)
    {
        for (int i = shuffleArray.Length - 1; i >= 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (shuffleArray[i], shuffleArray[randomIndex]) = (shuffleArray[randomIndex], shuffleArray[i]);

            sockets[i].GetComponent<CollorCodeSockets>().designatedCollor = shuffleArray[i];
        }
        CodeText.text = string.Join(", ", dollOrder);
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
