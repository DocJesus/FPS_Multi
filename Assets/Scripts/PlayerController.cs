using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : NetworkBehaviour {

    [SerializeField]
    private string remoteLayerName = "RemotePLayer";
    [SerializeField]
    private float speed;
    [SerializeField]
    private float rotationForce;
    [SerializeField]
    Behaviour[] ComponenetToDiseable;   
    [SerializeField]
    float thursterForce = 1000f;

    [SerializeField]
    private float thrusterBurnSpeed = 1f;
    [SerializeField]
    private float thrusterRegenSpeed = 0.3f;
    private float thrusterFullAmont = 1f;

    public float GetThrusterFuelAmont()
    {
        return thrusterFullAmont;
    }

    [SerializeField]
    private LayerMask environmentask;

    [Header("JointOptions")] 
    [SerializeField]
    float jointSpring = 20f;
    [SerializeField]
    float jointMaxForce = 40f;
    [SerializeField]
    private string layer = "NoRender";
    [SerializeField]
    private GameObject playerGraphics;
    [SerializeField]
    private GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    private Animator anim;
    private PlayerMotor motor;
    private ConfigurableJoint joint;


    private void Start()
    {
        anim = GetComponent<Animator>();
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        SetJointSettings(jointSpring);

        if (!isLocalPlayer)
        {
            //disable les components des autres joueur pour pas controler les autres joueurs)
            gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
            for (int i = 0; i < ComponenetToDiseable.Length; i++)
            {
                ComponenetToDiseable[i].enabled = false;
            }
        }
        else
        {
            //on désactive le layer de l'armure pour le joueur local
            Util.SetLayerRecursiv(playerGraphics, LayerMask.NameToLayer(layer));
            //créer l'UI (crossair)
            playerUIInstance = Instantiate(playerUIPrefab);
            //configuration de l'UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
            {
                Debug.Log("pas de PlayerUI");
            }
            else
            {
                ui.SetPlayer(GetComponent<Player>());
            }

            GetComponent<Player>().SetUp();

            string _username = "loading . . .";

            if (UserAccountManager.IsLoggedIn)
            {
                _username = UserAccountManager.LoggedIn_Username;
            }
            else
            {
                _username = transform.name;
            }

            CmdSetUsername(transform.name, _username);
        }

    }

    [Command]
    private void CmdSetUsername(string userId, string username)
    {
        Player player = GameManager.getPlayer(userId);
        if (player != null)
        {
            Debug.Log(username + " has joined");
            player.username = username;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //récupère son NETID
        string _netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        //ajoute le joueur au dico
        GameManager.ResgisterPlayer(_netId, _player);
    }

    private void OnDisable()
    {
        //Destroy(playerUIInstance);
        if (playerUIInstance != null)
            playerUIInstance.SetActive(false);
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
        }
        GameManager.UnRegisterPlayer(transform.name);
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (PauseMenu.isOn)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            motor.ApplyThruster(Vector3.zero);
            motor.move(Vector3.zero);
            motor.rotate(Vector3.zero);
            motor.rotateCamera(0f);

            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //check si il y a un truc en dessous du joueur
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, environmentask))
        {
            joint.targetPosition = new Vector3(0f, -hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        //mouvement
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _moveHorizontal = transform.right * _xMov;
        Vector3 _moveVertical = transform.forward * _zMov;

        Vector3 _velocity = (_moveHorizontal + _moveVertical) * speed;

        //jouer les animation
        anim.SetFloat("ForwardVelocity", _zMov);

        motor.move(_velocity);

        //rotation
        float _yrot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotator = new Vector3(0, _yrot, 0) * rotationForce;
        motor.rotate(_rotator);

        //rotation camera
        float _xrot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotatorX = _xrot * rotationForce;
        motor.rotateCamera(_cameraRotatorX);

        Vector3 _thursterForce = Vector3.zero;
        //jetpack
        if (Input.GetButton("Jump") && thrusterFullAmont > 0)
        {
            thrusterFullAmont -= thrusterBurnSpeed * Time.deltaTime;

            if (thrusterFullAmont >= 0.1f)
            {
                _thursterForce = Vector3.up * thursterForce;
                SetJointSettings(0f);
            }
        }
        else
        {
            thrusterFullAmont += thrusterRegenSpeed * Time.deltaTime;
            //donner un aspect de rebondissement
            SetJointSettings(jointSpring);
        }

        thrusterFullAmont = Mathf.Clamp(thrusterFullAmont, 0f, 1f);

        motor.ApplyThruster(_thursterForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
