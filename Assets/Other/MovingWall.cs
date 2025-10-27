using UnityEngine;

public class MovingWall : MonoBehaviour
{
    [SerializeField] private float moveDistance = 20f;
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float cooldownDuration = 10f;
    
    private Vector3 startPosition;
    private float elapsedTime = 0f;
    private bool movingOut = true;
    private bool isMoving = false;
    private bool isCoolingDown = false;
    private Spawner spawner;
    private bool hasCalledRandomSpot = false;

    void Start()
    {
        startPosition = transform.position;
        isMoving = true;
        spawner = FindFirstObjectByType<Spawner>();
    }
// moves the wall back and forth and makes all npcs recalculate their path
    void Update()
    {
        elapsedTime += Time.deltaTime;
        
        if (isMoving)
        {
            float progress = elapsedTime / moveDuration;
            
            if (progress >= 1f)
            {
                elapsedTime = 0f;
                isMoving = false;
                isCoolingDown = true;
                hasCalledRandomSpot = false;
            }
            else
            {
                float currentProgress = Mathf.Clamp01(progress);
                float xOffset = movingOut ? moveDistance * currentProgress : moveDistance * (1f - currentProgress);
                
                transform.position = startPosition + new Vector3(xOffset, 0, 0);
            }
        }
        else if (isCoolingDown)
        {
            // call RandomSpot after 0.5 seconds into cooldown
            if (!hasCalledRandomSpot && elapsedTime >= 0.5f)
            {
                if (spawner != null)
                {
                    spawner.RecalculatePath();
                }
                hasCalledRandomSpot = true;
            }
            
            if (elapsedTime >= cooldownDuration)
            {
                elapsedTime = 0f;
                isCoolingDown = false;
                isMoving = true;
                movingOut = !movingOut;
            }
        }
    }
}
