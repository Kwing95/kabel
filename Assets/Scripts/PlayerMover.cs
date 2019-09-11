using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMover : MonoBehaviour
{

    public static PlayerMover instance;
    public Text apText;

    private GridMover mover;
    private FieldUnit unit;

    public GameObject noise;
    public GameObject enemies;
    private AudioSource source;
    private Navigator nav;
    public int runSpeed = 6;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; // Intentionally not using DestroyOnLoad

        unit = GetComponent<FieldUnit>();
        unit.ResetAP();
        source = GetComponent<AudioSource>();
        mover = GetComponent<GridMover>();
        nav = GetComponent<Navigator>();
    }

    // Update is called once per frame
    void Update()
    {

        RefreshText();

        Vector2 input = GetInput();
        if (input != Vector2.zero)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                mover.moveSpeed = 6;
            else
                mover.moveSpeed = 3;

            nav.SetDestination((Vector2)transform.position + input);

            /*if (mover.ChangeDirection(input) && mover.moveSpeed == 6)
            {
                source.pitch = Random.Range(1f, 1.2f);
                source.Play();
                Instantiate(noise, transform.position, Quaternion.identity);
            }*/
                
        }
            
    }

    Vector2 GetInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = horizontalInput == 0 ? Input.GetAxisRaw("Vertical") : 0;
        return new Vector2(horizontalInput, verticalInput);
    }

    public void RefreshText()
    {
        apText.text = "AP: " + unit.GetAP() + "/" + unit.GetMaxAP();
    }

}
