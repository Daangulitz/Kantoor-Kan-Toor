using UnityEngine;

public class DollSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] dollPrefabs;
    [SerializeField] private Transform[] dollSpawners;
    private void Start()
    {
        ShuffleDolls(dollPrefabs);
        SpawnDolls(dollSpawners);
    }
    private void ShuffleDolls(GameObject[] dollArray)
    {
        for (int i = dollArray.Length - 1; i >= 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (dollArray[i], dollArray[randomIndex]) = (dollArray[randomIndex], dollArray[i]);

        }
    }

    private void SpawnDolls(Transform[] spawnArray)
    {
        for (int i = spawnArray.Length -1; i >= 0; i--)
        {
            Instantiate(dollPrefabs[i], spawnArray[i].position, Quaternion.identity);
        }
    }
}
