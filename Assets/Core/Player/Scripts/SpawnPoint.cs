using UnityEngine;

[SelectionBase]
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    private void Start()
    {
        Instantiate(playerPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
