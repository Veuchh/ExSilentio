using UnityEngine;

[SelectionBase]
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    private void Awake()
    {
        Instantiate(playerPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
