using System;
using UnityEngine;
using System.Collections;
public class NPCManager : MonoBehaviour
{
    [SerializeField] public bool idle = false;
    [SerializeField] public bool trackPlayer = false;
    [SerializeField] public bool isScared = false;
    public bool closeToBoss = false;
    
    public bool canRevive = false;
    
    
    private float trackPlayerFalseTimer = 0f;
    public bool hasBeenFalseFor5Seconds = false;

    public bool bossBlueExists = false;
    
    //Ant stuffs

    [SerializeField] private bool wantToKillPlayer = false;
    public bool WantToKillPlayer 
    { 
        get { return wantToKillPlayer; } 
        set { wantToKillPlayer = value; } 
    }
    
    [SerializeField] private bool bossAlive = true;
    public bool BossAlive 
    { 
        get { return bossAlive; } 
        set { bossAlive = value; } 
    }

    [SerializeField] private bool nearBoss = false;
    public bool NearBoss 
    { 
        get { return nearBoss; } 
        set { nearBoss = value; } 
    }

    [SerializeField] private bool scaredOfPlayer = false;
    public bool ScaredOfPlayer 
    { 
        get { return scaredOfPlayer; } 
        set { scaredOfPlayer = value; } 
    }

    [SerializeField] private bool nearPlayer = false;
    public bool NearPlayer 
    { 
        get { return nearPlayer; } 
        set { nearPlayer = value; } 
    }

    [SerializeField] private bool bossWantsToKillPlayer = false;
    public bool BossWantsToKillPlayer 
    { 
        get { return bossWantsToKillPlayer; } 
        set { bossWantsToKillPlayer = value; } 
    }

    [SerializeField] private bool roamingForAwhile = false;
    public bool RoamingForAwhile 
    { 
        get { return roamingForAwhile; } 
        set { roamingForAwhile = value; } 
    }

    [SerializeField] private bool canSeeBoss = false;
    public bool CanSeeBoss 
    { 
        get { return canSeeBoss; } 
        set { canSeeBoss = value; } 
    }

    [SerializeField] private bool pathToBoss = false;
    public bool PathToBoss 
    { 
        get { return pathToBoss; } 
        set { pathToBoss = value; } 
    }

    [SerializeField] private bool giveBossSpace = false;
    public bool GiveBossSpace 
    { 
        get { return giveBossSpace; } 
        set { giveBossSpace = value; } 
    }
    
    public void VLookForBoss()
    {
        Debug.Log("looking for boss");
    
        //find all GameObjects with bossblue in the name
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject nearestBoss = null;
        float shortestDistance = Mathf.Infinity;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("BossBlue"))
            {
                
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestBoss = obj;
                }
            }
        }
        
        
        if (nearestBoss == null)
        {
            CanSeeBoss = false;
            Debug.Log("No boss found in scene");
            return;
        }
        
        //raycast to boss
        Vector3 directionToBoss = (nearestBoss.transform.position - transform.position).normalized;
        float distanceToBoss = Vector3.Distance(transform.position, nearestBoss.transform.position);
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToBoss, out hit, distanceToBoss))
        {
            // Check if nothing blocking
            if (hit.collider.gameObject == nearestBoss || hit.collider.gameObject.name.Contains("Boss"))
            {
                CanSeeBoss = true;
                pathFollower.CalculatePath(nearestBoss.transform.position);
                Debug.Log("Can see boss clear line of sight");
            }
            else
            {
                CanSeeBoss = false;
                Debug.Log($"Cannot see boss, blocked by {hit.collider.gameObject.name}");
            }
        }
        else
        {
            
            CanSeeBoss = false;
            Debug.Log("Raycast missed");
        }
        
        //debug line to visualize raycast
        Debug.DrawRay(transform.position, directionToBoss * distanceToBoss, 
                      CanSeeBoss ? Color.green : Color.red, 1f);
    }
    
    [SerializeField] private GameObject bossRevivePrefab; 
    
    private MoveForward moveForward;
    private Align align;
    private Separation separation;
    private Cohesion cohesion;
    private GameObject visualForShout;
    private PathFollower pathFollower;
    private Coroutine trackPlayerCoroutine;
    private GameObject onMyWay;
    private GameObject ahh;
    
    void Start()
    {
        InvokeRepeating("CheckifBossExists", 0f, 1f);
        idle = false;
        // grab all the movement scripts
        moveForward = GetComponent<MoveForward>();
        align = GetComponent<Align>();
        separation = GetComponent<Separation>();
        cohesion = GetComponent<Cohesion>();
        pathFollower = GetComponent<PathFollower>();
        
        // get the VisualForShout child object and make it inactive
        visualForShout = transform.Find("VisualForShout")?.gameObject;
        if (visualForShout != null)
        {
            visualForShout.SetActive(false);
        }
        
        // get the OnMyWay child object and make it inactive
        onMyWay = transform.Find("OnMyWay")?.gameObject;
        if (onMyWay != null)
        {
            onMyWay.SetActive(false);
        }
        
        // get the AHH child object and make it inactive
        ahh = transform.Find("AHH")?.gameObject;
        if (ahh != null)
        {
            ahh.SetActive(false);
        }
        
        // apply the idle setting right away
        ToggleMovement();
        BossWantsToKillPlayer = false;
    }

    void Update()
    {
        if (!BossWantsToKillPlayer)
        {
            WantToKillPlayer = false;
        }

        if (WantToKillPlayer == true)
        {
            trackPlayer = true;
        }

        if (!bossBlueExists)
        {
            WantToKillPlayer = false;
            
        }
        
        
        nearBoss = closeToBoss;
        //WantToKillPlayer = trackPlayer;
        //trackPlayer = WantToKillPlayer;
        scaredOfPlayer = isScared;
        NearPlayer = trackPlayer;
        RoamingForAwhile = hasBeenFalseFor5Seconds;
        BossAlive = bossBlueExists;

        if (nearBoss)
        {
            roamingForAwhile = false;
            trackPlayerFalseTimer = 0;
        }
        if (giveBossSpace)
        {
            Debug.Log("111giving boss space");
            RandomLocation();
            giveBossSpace = false;
        }
        
        if (Time.frameCount % 60 == 0 && roamingForAwhile && canRevive == false)
        {
            VLookForBoss();
        }
        
        if (!trackPlayer)
        {
            //increment timer while false
            trackPlayerFalseTimer += Time.deltaTime;
        
            if (trackPlayerFalseTimer >= 5f)
            {
                hasBeenFalseFor5Seconds = true;
            }
        }
        else
        {
            // Reset when true
            trackPlayerFalseTimer = 0f;
            hasBeenFalseFor5Seconds = false;
        }
        
        
        
        
        //check if idle was changed in the inspector
        ToggleMovement();
        
        //if scared, disable tracking
        if (isScared && !gameObject.name.Contains("Boss"))
        {
            trackPlayer = false;
        }
        
        //start or stop tracking based on trackPlayer variable
        if (trackPlayer && trackPlayerCoroutine == null)
        {
            trackPlayerCoroutine = StartCoroutine(TrackPlayerCoroutine());
        }
        else if (!trackPlayer && trackPlayerCoroutine != null)
        {
            StopCoroutine(trackPlayerCoroutine);
            trackPlayerCoroutine = null;
        }
        
        if (!bossWantsToKillPlayer)
        {
            trackPlayer = false;
            //Debug.Log("Not close to boss");
        }
        
        

        if (!bossBlueExists)
        {
            canRevive = true;
            closeToBoss = false;
        }
    }

    public void CheckifBossExists()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        bossBlueExists = false;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("BossBlueNPC"))
            {
                bossBlueExists = true;
                break;
            }
        }
    }
    
    public void ShowVisualForShout()
    {
        if (visualForShout != null )
        {
            StartCoroutine(ShowVisualForShoutCoroutine());
        }
    }
    
    private IEnumerator ShowVisualForShoutCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        if (GameplayManager.instance.attacking)
        {
            BossWantsToKillPlayer = true;
            visualForShout.SetActive(true);
            yield return new WaitForSeconds(2f);
        
            visualForShout.SetActive(false);
        }
        
    }
    
    private void ShowOnMyWay()
    {
        if (onMyWay != null )
        {
            StartCoroutine(ShowOnMyWayCoroutine());
        }
    }
    
    private IEnumerator ShowOnMyWayCoroutine()
    {
            onMyWay.SetActive(true);
            yield return new WaitForSeconds(2f);
            onMyWay.SetActive(false);
            
    }
    
    private void ShowAHH()
    {
        if (ahh != null)
        {
            StartCoroutine(ShowAHHCoroutine());
        }
    }
    
    private IEnumerator ShowAHHCoroutine()
    {
        ahh.SetActive(true);
        yield return new WaitForSeconds(3f);
        ahh.SetActive(false);
    }
    
    public void ToggleMovement() // set idle to true or false
    {
        // if idle = true, turn OFF movement (enabled = false)
        // if idle = false, turn ON movement (enabled = true)
        bool shouldMove = !idle;
        
        moveForward.enabled = shouldMove;
        align.enabled = shouldMove;
        separation.enabled = shouldMove;
        cohesion.enabled = shouldMove;
    }


    

    public void SetPathToPlayer()
    {
        // Only run if this GameObject's name contains("Boss")
        if (!gameObject.name.Contains("Boss") && closeToBoss == false)
        {
            return;
        }
        
        idle = false;
        // Find the Player game object
        GameObject player = GameObject.Find("Player");
        
        if (player == null)
        {
            Debug.LogWarning("Player game object not found!");
            return;
        }
        
        if (pathFollower == null)
        {
            Debug.LogWarning("PathFollower component not found on this NPC!");
            return;
        }
        
        //calculate path to player's position
        pathFollower.CalculatePath(player.transform.position);
    }

    public void SetPathToRespawnSpot()
    {
        idle = false;
        // set path to the revive button thing
        GameObject Boss = GameObject.Find("BossRevive");
        
        if (Boss == null)
        {
            Debug.LogWarning("not found");
            return;
        }
        
        if (pathFollower == null)
        {
            Debug.LogWarning("not found on this NPC");
            return;
        }
        
        
        pathFollower.CalculatePath(Boss.transform.position);
    }

    private IEnumerator TrackPlayerCoroutine()
    {
        while (trackPlayer)
        {
            SetPathToPlayer();
            
            yield return new WaitForSeconds(0.1f);
        }
        //BossWantsToKillPlayer = false;
        Debug.Log("boss stops tracking player");
        trackPlayerCoroutine = null;
        
    }
    
    

    public void RandomLocation()
    {
        pathFollower.RequestNewPath();
        
    }
    
    /*
    public void StartTrackingPlayer()
    {
        trackPlayer = true;
    }

    public void StopTrackingPlayer()
    {
        trackPlayer = false;
    }
    */

    public void NormalNPCTracking()
    {
        
        if (closeToBoss && GameplayManager.instance.attacking)
        {
            trackPlayer = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Contains("BossRevive") && canRevive == true)
        {
            canRevive = false;
            //spawn the boss at the BossRevive location
            if (bossRevivePrefab != null)
            {
                Instantiate(bossRevivePrefab, other.transform.position, other.transform.rotation);
            }
            else
            {
                Debug.LogWarning("Boss Revive Prefab not assigned");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if scared and player entered
        if (isScared && other.gameObject.name.Contains("Player"))
        {
            NearPlayer = true;
            ScaredOfPlayer = true;
            ShowAHH();
            if (pathFollower != null)
            {
                pathFollower.RunAway();
            }
        }
        
        // check if the npc is close to the boss
        if (other.gameObject.name.Contains("DetectorSphereBoss"))
        {
            closeToBoss = true;
            
            ShowOnMyWay();
            
        }

       
    }

    private void OnTriggerExit(Collider other)
    {
        // opposite of above
        if (other.gameObject.name.Contains("DetectorSphereBoss"))
        {
            closeToBoss = false;
            
        }
    }

    private void OnDisable()
    {
        //stop tracking when the component is disabled
        if (trackPlayerCoroutine != null)
        {
            StopCoroutine(trackPlayerCoroutine);
            trackPlayerCoroutine = null;
        }
    }
    
    
}
