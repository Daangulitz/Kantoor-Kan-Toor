using UnityEngine;
using TMPro;

public class CompletedEvent : MonoBehaviour
{
    private int GearCount = 0;

    [SerializeField] private TMP_Text GearCountText;

    public int[] ints = new int[6];


    public void AddGearCount()
    {
        GearCount++;
        GearCountText.text = "0" + GearCount;
    }

    public void addCorrectGear(int index)
    {
        // place index in the correct array position ( index 6, is the last position in the array)
        if (index >= 0 && index < ints.Length)
        {
            ints[index] = 1;
        }
    }
}
