using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneControls : MonoBehaviour {

    private Rigidbody r;

    public float TargetRotZ;
    public float TargetRotX;
    public float RotSpeedZ;
    public float RotSpeedX;
    public float MovSpeedX;
    public float MovSpeedY;
    public float Ceiling;
    public float Floor;
    public float LeftWall;
    public float RightWall;
    private Vector3 currentRot;
    private Vector3 rotSpeedXVec;
    private Vector3 rotSpeedZVec;
    private Vector3 currentSpd;
    private Vector3 movSpeedXVec;
    private Vector3 movSpeedYVec;

    private float fogEnd;
    private float currentFogEnd;

    public float OpeningSpeed;
    public float TextFadeInTime;
    public float TextFadeInSpeed;
    private bool opening;
    public float ClosingSpeed;
    public float TextFadeOutTime;
    public float TextFadeOutSpeed;
    private bool closing;

    public float TotalPlayTime;

    private bool fadeTextIn;
    private bool textFadedIn;
    private bool fadeTextOut;
    private bool textFadedOut;

    public Text TextToFade;
    public GameObject EndGameText;

    private bool canFade;
    public string FriendCode;

    // Use this for initialization
    void Start () {
        r = GetComponent<Rigidbody>();
        currentRot = r.rotation.eulerAngles;
        currentSpd = r.velocity;
        rotSpeedXVec = new Vector3(RotSpeedX, 0f, 0f);
        rotSpeedZVec = new Vector3(0, 0f, RotSpeedZ);
        movSpeedXVec = new Vector3(MovSpeedX, 0f, 0f);
        movSpeedYVec = new Vector3(0f, MovSpeedY, 0f);
        fogEnd = RenderSettings.fogEndDistance;
        currentFogEnd = 0f;

        opening = true;
        closing = false;

        fadeTextOut = false;
        fadeTextIn = false;
        textFadedIn = false;
        textFadedOut = false;

        TextToFade.CrossFadeColor(Color.clear, 0f, true, true, true);
        canFade = false;
        if (Application.absoluteURL.Contains(FriendCode) || Application.isEditor)
            canFade = true;
    }

// Update is called once per frame
    void Update () {

        if(opening && currentFogEnd <= (fogEnd * 0.95f))
        {
            currentFogEnd = Mathf.Lerp(currentFogEnd, fogEnd, Time.deltaTime/OpeningSpeed);
            RenderSettings.fogEndDistance = currentFogEnd;
        }
        else if (opening)
        {
            currentFogEnd = fogEnd;
            RenderSettings.fogEndDistance = currentFogEnd;
            opening = false;
        }

        if (!opening && !closing && Time.time >= TotalPlayTime)
        {
            closing = true;
        }

        if (canFade && !textFadedIn && Time.time >= TextFadeInTime)
            fadeTextIn = true;

        if (canFade && !textFadedOut && Time.time >= TextFadeOutTime)
            fadeTextOut = true;


        if (Time.time > (TotalPlayTime + ClosingSpeed))
            EndGameText.SetActive(true);

        if (fadeTextIn)
        {
            fadeTextIn = false;
            textFadedIn = true;
            TextToFade.CrossFadeColor(Color.white, TextFadeInSpeed, true, true, true);
        }

        if (fadeTextOut)
        {
            fadeTextOut = false;
            textFadedOut = true;
            TextToFade.CrossFadeColor(Color.clear, TextFadeOutSpeed, true, true, true);
        }

        if (closing && currentFogEnd >= RenderSettings.fogStartDistance)
        {
            currentFogEnd = Mathf.Lerp(currentFogEnd, RenderSettings.fogStartDistance, Time.deltaTime / ClosingSpeed);
            RenderSettings.fogEndDistance = currentFogEnd;
        }
        else if (closing)
        {
            RenderSettings.fogEndDistance = 0f;
            closing = false;
        }
    

        currentSpd = Vector3.zero;
        if(Input.GetAxis("Horizontal") > 0)
        {
            if (currentRot.z >= -TargetRotZ)
                currentRot -= rotSpeedZVec;
            if (transform.position.x <= RightWall)
                currentSpd += movSpeedXVec;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            if (currentRot.z <= TargetRotZ)
                currentRot += rotSpeedZVec;
            if (transform.position.x >= LeftWall)
                currentSpd -= movSpeedXVec;
        }
        else
        {
            if (currentRot.z > 0f)
                currentRot -= rotSpeedZVec;
            if (currentRot.z < 0f)
                currentRot += rotSpeedZVec;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            if (currentRot.x >= -TargetRotX)
                currentRot -= rotSpeedXVec;
            if (transform.position.y <= Ceiling)
                currentSpd += movSpeedYVec;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            if (currentRot.x <= TargetRotX)
                currentRot += rotSpeedXVec;
            if (transform.position.y >= Floor)
                currentSpd -= movSpeedYVec;
        }
        else
        {
            if (currentRot.x > 0f)
                currentRot -= rotSpeedXVec;
            if (currentRot.x < 0f)
                currentRot += rotSpeedXVec;
        }
        r.rotation = Quaternion.Euler(currentRot);
        r.velocity = currentSpd;


    }
}
