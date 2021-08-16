using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMover : MonoBehaviour
{
    public float maxSmartMoveDistance = 10;

    public static PlayerMover instance;

    private Rotator rotator;
    private GridMover mover;
    private FieldUnit unit;

    private AudioSource source;
    private Navigator nav;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; // Intentionally not using DestroyOnLoad

        rotator = GetComponent<Rotator>();
        unit = GetComponent<FieldUnit>();
        source = GetComponent<AudioSource>();
        mover = GetComponent<GridMover>();
        nav = GetComponent<Navigator>();

        ClickManager.releaseHandler += _OnClick;
        // ClickHandler.DetectPlatform();
    }

    private void OnDestroy()
    {
        ClickManager.releaseHandler -= _OnClick;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Time.timeScale == 0)
            return;
        
        // If raycast is clear, construct a basic path. If not, smart path with maximum distance
        if (Input.GetMouseButton(0) && initialClick && !ClickHandler.MouseOverUI())
        {
            initialClick = false;

            _OnClick(GetMousePosition());
        }

        if (!Input.GetMouseButton(0))
            initialClick = true;
            */
    }

    public void _OnClick(Vector2 mousePosition)
    {
        ActionManager.State state = ActionManager.GetState();
        if (state != ActionManager.State.Moving && state != ActionManager.State.ConfirmMove && state != ActionManager.State.Acting)
            return;

        if (Sidebar.GetMenuPaused())
            return;

        float distance = Vector2.Distance(transform.position, mousePosition);

        LayerMask mask = LayerMask.GetMask("Default");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePosition - (Vector2)transform.position, distance, mask);

        nav.SetDestination(mousePosition, Sidebar.GetRunning());
    }

    private void OnDisable()
    {
        mover.enabled = false;
        rotator.enabled = false;
    }

    private void OnEnable()
    {
        mover.enabled = true;
        rotator.enabled = true;
    }

    // Returns the tile that the mouse is positioned over
    public static Vector2 GetMousePosition()
    {
        return (Vector3)Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }


}
