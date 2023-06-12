using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceC4 : MonoBehaviour
{
    public static PlaceC4 instance;

    public Text timeToExplosion;

    //Objects
    [SerializeField] private GameObject c4Prefab;
    private MouseLook mouseLook;
    private PlayerMovement playerMovement;

    //UI
    private Image cooldown;
    private Image cooldownBG;
    private Text time;

    //Game Configs
    private float distanceToPlant = 3.5f;
    private bool coolingDown;
    private float plantCooldownTime = 4f;
    private float defuseCooldownTime = 10f;
    [SerializeField] private float timeRemaining = 5f;
    [SerializeField] private bool planted = false;
    public bool bombExplosed = false;
    private bool bombDefused = false;

    //Aux
    private Vector3 hitLocal;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        mouseLook = GetComponent<MouseLook>();
        cooldown = GameObject.FindGameObjectWithTag("Cooldown").GetComponent<Image>();
        time = GameObject.FindGameObjectWithTag("Time").GetComponent<Text>();
        cooldownBG = GameObject.FindGameObjectWithTag("CooldownBG").GetComponent<Image>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Start()
    {
        cooldown.gameObject.SetActive(false);
        cooldownBG.gameObject.SetActive(false);
        time.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, distanceToPlant))
            {
                if (hitInfo.collider.tag == "Ground" && !planted)
                {
                    hitLocal = hitInfo.point;
                    VisibleCanvasObjects(true);
                    coolingDown = true;
                    MovementAndMouseLookPlayerControl(false);
                }
            }

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, distanceToPlant))
            {
                if (hitInfo.collider.tag == "Bomb" && planted && !bombExplosed)
                {
                    VisibleCanvasObjects(true);
                    coolingDown = true;
                    MovementAndMouseLookPlayerControl(false);
                }
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            coolingDown = false;
            if (cooldown.fillAmount < 1)
            {
                cooldown.fillAmount = 0;
                VisibleCanvasObjects(false);
                MovementAndMouseLookPlayerControl(true);
            }
        }

        VerifyPlant();
        VerifyDefuse();
        TimerPlant();
    }

    void InstantiateC4(Vector3 hitPoint)
    {
        Instantiate(c4Prefab, hitPoint, Quaternion.Euler(new Vector3(180, 0, -90)));
        planted = true;
        StartCoroutine(TimeToExplosion());
    }
    IEnumerator TimeToExplosion()
    {
        while (timeRemaining > 0 && !bombDefused)
        {
            timeRemaining -= Time.deltaTime;
            timeToExplosion.text = "Explosion in " + Mathf.CeilToInt(timeRemaining).ToString() + " seconds";
            yield return null;
        }

        if (!bombDefused)
        {
            BombControl.instance.BombPlanted();
            // Explosão acontece aqui após o término da contagem
        }
    }

    void VerifyDefuse()
    {
        if (cooldown.fillAmount >= 1 && planted && !bombExplosed && timeRemaining != 0)
        {
            bombDefused = true;
            coolingDown = false;
            cooldown.fillAmount = 0;
            VisibleCanvasObjects(false);
            Destroy(GameObject.FindGameObjectWithTag("Bomb"));
            timeToExplosion.text = "Bomb has been defused ";
            MovementAndMouseLookPlayerControl(true);
        }
    }

    void VerifyPlant()
    {
        if (cooldown.fillAmount >= 1 && !planted)
        {
            coolingDown = false;
            cooldown.fillAmount = 0;
            VisibleCanvasObjects(false);
            InstantiateC4(hitLocal);
            MovementAndMouseLookPlayerControl(true);
        }
    }
    void TimerPlant()
    {
        if (coolingDown && !planted)
        {
            cooldown.fillAmount += Time.deltaTime / plantCooldownTime;
            float timeText = cooldown.fillAmount * plantCooldownTime;
            time.text = timeText.ToString("F");
        }
        else if (coolingDown && planted && !bombExplosed)
        {
            cooldown.fillAmount += Time.deltaTime / defuseCooldownTime;
            float timeText = cooldown.fillAmount * defuseCooldownTime;
            time.text = timeText.ToString("F");
        }
        else if (bombExplosed)
        {
            cooldown.fillAmount = 0;
        }
    }


    void VisibleCanvasObjects(bool status)
    {
        cooldown.gameObject.SetActive(status);
        cooldownBG.gameObject.SetActive(status);
        time.gameObject.SetActive(status);
    }

    void MovementAndMouseLookPlayerControl(bool status)
    {
        mouseLook.enabled = status;
        playerMovement.enabled = status;
    }
}