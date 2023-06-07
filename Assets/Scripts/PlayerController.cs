using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed = 25;
    private CharacterController cc;
    
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        int verticalInput = 0;
        if (Input.GetKey(KeyCode.Q))
            verticalInput = 1;
        else if (Input.GetKey(KeyCode.E))
            verticalInput = -1;

        int multiplier = 1;

        if (Input.GetKey(KeyCode.LeftShift))
            multiplier = 2;
        else
            multiplier = 1;
        
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), verticalInput, Input.GetAxisRaw("Vertical"));
        Debug.Log(input.ToString());
        cc.Move(input * speed * Time.deltaTime * multiplier);
    }
}
