using TMPro;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("KeyDoll"))
        {
            winText.text = "YOU HAVE GEWONNEN. DAAN WILL BE PROUD OF YOU, YOU ALSO GET FREE CANDY FOR LIFE AND MARIJN VEGGEL WIL HAVE TO PAY YOU 9230358025 EURO MET 2 SPIEGELIJTJES GEBAKKEN IN OLIJFOLIE";
        }
    }
}
