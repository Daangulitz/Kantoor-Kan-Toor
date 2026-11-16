using UnityEngine;
using TMPro;

public class CompletedEvent : MonoBehaviour
{
    private int GearCount = 0;

    [SerializeField] private TMP_Text GearCountText;

    public void AddGearCount()
    {
        GearCount++;
        GearCountText.text = "0" + GearCount;
    }
}
