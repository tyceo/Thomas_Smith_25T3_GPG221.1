using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public int amount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
