using UnityEngine;

public class Despawner : MonoBehaviour
{
    public float DespawnTime = 8f;
    public System.Action OnDespawn;

    private void Start()
    {
        Invoke("Despawn", DespawnTime);
    }

    private void Despawn()
    {
        OnDespawn?.Invoke();
        gameObject.SetActive(false);
    }
}