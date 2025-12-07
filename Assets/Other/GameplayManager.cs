using UnityEngine;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    
    public bool attacking = false;
    
    public static GameplayManager instance;
    
    public GameObject Guide;
    
    private int _points = 0;
    public int points
    {
        get { return _points; }
        set
        {
            Debug.Log("Points changed to " + value);
            StopAttack();
            _points = value;
            ammo = ammo + 1;
            
            UpdateScoreText();
            CheckWinCondition();
            
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject winObject;
    [SerializeField] private GameObject loseObject;
    public bool anyBoxHeld = false;
    
    //[SerializeField] private TextMeshProUGUI ammoText;
    public int ammo = 3;

    private void Awake()
    {
        //singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScoreText();
        
        //Win and Lose objects are inactive at start
        if (winObject != null)
        {
            winObject.SetActive(false);
        }
        if (loseObject != null)
        {
            loseObject.SetActive(false);
        }
        
        // Deactivate Guide after 4 seconds
        if (Guide != null)
        {
            Invoke("DeactivateGuide", 4f);
        }
    }
    
    private void DeactivateGuide()
    {
        if (Guide != null)
        {
            Guide.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckLoseCondition();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + points;
        }
    }

    private void CheckWinCondition()
    {
        if (points >= 5 && winObject != null)
        {
            winObject.SetActive(true);
        }
    }

    private void CheckLoseCondition()
    {
        //if Player object doen't exist then game over
        GameObject player = GameObject.Find("Player");
        
        if (player == null && loseObject != null)
        {
            loseObject.SetActive(true);
        }
    }

    //called by BoxStuff when hasBox changes
    public void OnBoxStatusChanged()
    {
        //Debug.Log("test");
        
        Attack();
        
        //find all BoxStuff instances and check if any has hasBox = true
        BoxStuff[] allBoxes = FindObjectsByType<BoxStuff>(FindObjectsSortMode.None);
        anyBoxHeld = false;
        
        foreach (BoxStuff box in allBoxes)
        {
            if (box.hasBox)
            {
                anyBoxHeld = true;
                break;
            }
        }
    }

    public void Attack()
    {
        attacking = true;
        // Find all game objects in the scene
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            // Check if the name contains "NPC"
            if (obj.name.Contains("NPC"))
            {
                // Get the NPCManager component
                NPCManager npcManager = obj.GetComponent<NPCManager>();
                if (obj.name.Contains("Boss"))
                {
                    if (npcManager != null)
                    {
                        npcManager.ShowVisualForShout();              
                        npcManager.trackPlayer = true;
                        npcManager.BossWantsToKillPlayer = true;
                    }
                }
                else
                {
                    if (npcManager != null)
                    {
                        npcManager.NormalNPCTracking();
                        npcManager.BossWantsToKillPlayer = true;
                    }
                }
            }
            
        }
    }
    public void StopAttack()
    {
        attacking = false;
        
            // Find all game objects in the scene
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            // Check if the name contains "NPC"
            if (obj.name.Contains("NPC"))
            {
                // Get the NPCManager component
                NPCManager npcManager = obj.GetComponent<NPCManager>();
                
                if (npcManager != null)
                {
                    npcManager.BossWantsToKillPlayer = false;
                    npcManager.trackPlayer = false;
                    if (npcManager.canRevive == false)
                    {
                        npcManager.RandomLocation();
                    }
                    
                }
            }
        }
    }
    
}
