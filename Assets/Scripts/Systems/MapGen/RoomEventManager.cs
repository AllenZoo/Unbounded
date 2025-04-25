using UnityEngine;

public interface IEventManager
{

}

// TODO: eventually don't couple this class with collider2D. Here for now for simplicity and since we shouldn't expect
// to expand room collision handling further than acting as a cinemachine boundary or player entry/exit tracker.
[RequireComponent(typeof(Collider2D))]
public class RoomEventManager : MonoBehaviour, IEventManager
{
    [Tooltip("Boundary of the room.")]
    private Collider2D boundary;

    private void Awake()
    {
        boundary = GetComponent<Collider2D>();
        Debug.Assert(boundary.isTrigger == true, "RoomEventManager: Boundary collider2D is not a trigger on pfb: " + gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            EventBus<OnPlayerEnterRoom>.Call(new OnPlayerEnterRoom
            {
                roomPfb = gameObject,
                roomBoundary = boundary
            });
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            EventBus<OnPlayerStayRoom>.Call(new OnPlayerStayRoom
            {
                roomPfb = gameObject,
                roomBoundary = boundary
            });
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            EventBus<OnPlayerExitRoom>.Call(new OnPlayerExitRoom
            {
                roomPfb = gameObject,
                roomBoundary = boundary
            });
        }
    }
}
