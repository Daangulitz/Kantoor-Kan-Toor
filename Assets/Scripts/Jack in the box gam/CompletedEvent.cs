using System;
using TMPro;
using UnityEngine;

public class CompletedEvent : MonoBehaviour
{
    [SerializeField] private TMP_Text gearCountText;

    private readonly bool[] occupied = new bool[6];
    private readonly bool[] correct = new bool[6];

    public event Action OnAllGearsCompleted;

    public void SetSocketState(int socketIndex, bool isOccupied, bool isCorrect)
    {
        if (socketIndex < 0 || socketIndex >= occupied.Length)
            return;

        occupied[socketIndex] = isOccupied;
        correct[socketIndex] = isOccupied && isCorrect;

        UpdateCountText();

        if (Array.TrueForAll(correct, c => c))
            OnAllGearsCompleted?.Invoke();
    }

    // Name kept so HandleRotation doesn't need changes: true only if the CORRECT gear is in this socket
    public bool IsGearPlaced(int socketIndex)
    {
        return socketIndex >= 0 && socketIndex < correct.Length && correct[socketIndex];
    }

    private void UpdateCountText()
    {
        int count = 0;
        foreach (bool o in occupied) if (o) count++;
        gearCountText.text = "0" + count;
    }
}