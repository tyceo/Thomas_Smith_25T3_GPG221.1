using UnityEngine;

public class BoxStuff : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    
    private bool _hasBox = false;
    public bool hasBox
    {
        get { return _hasBox; }
        set
        {
            if (_hasBox != value)
            {
                _hasBox = value;
                // Notify GameplayManager of the change
                if (GameplayManager.instance != null)
                {
                    GameplayManager.instance.OnBoxStatusChanged();
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //colliding object has the "Collection" tag
        if (other.CompareTag("Collection"))
        {
            hasBox = false;
            // Add 1 to points
            GameplayManager.instance.points += 1;
            
            
            Destroy(gameObject);
            return;
        }

        //move the box to the player
        if (targetObject != null && other.gameObject == targetObject)
        {
            // Set this object as a child of the object that entered
            transform.SetParent(other.transform);
            hasBox = true;
            
            // Set local z and y positions to 0
            Vector3 localPos = transform.localPosition;
            localPos.x = 0f;
            localPos.z = 0f;
            transform.localPosition = localPos;
        }
    }
}