using UnityEngine;
using UnityEngine.Events;

public class Hearing : MonoBehaviour
{
    
    public SoundEmitter lastHeard;
    public UnityEvent<SoundEmitter> OnHeardSound;
    [SerializeField] private bool showDebugLogs = true;
    
    public void HeardSomething(SoundEmitter thingThatEmittedSound)
    {
        //store last sound heard
        lastHeard = thingThatEmittedSound;
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} heard sound from {thingThatEmittedSound.gameObject.name}");
        }
        
        // event system to forward the event
        OnHeardSound?.Invoke(thingThatEmittedSound);
    }
}
