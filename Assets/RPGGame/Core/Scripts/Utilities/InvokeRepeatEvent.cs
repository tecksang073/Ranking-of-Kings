using UnityEngine;
using UnityEngine.Events;

public class InvokeRepeatEvent : MonoBehaviour
{
    public float time = 0;
    public float repeatRate = 1;
    public UnityEvent events = new UnityEvent();

    void Start()
    {
        InvokeRepeating("CallEvents", time, repeatRate);
    }

    void CallEvents()
    {
        events.Invoke();
    }
}
