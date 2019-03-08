using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour
{
    private Vector3 velocity;
    private Vector3 rotation;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterForce;

    [SerializeField]
    float cameraClamp = 60f;

    private Rigidbody rig;

    [SerializeField]
    private Camera cam;

    //récupère le mouvement
    public void move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    //récupère la rotation (horizontal uniquement)
    public void rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    //récupère la rotation de la camera (vertical uniquement)
    public void rotateCamera(float _cameraRotatorX)
    {
        cameraRotationX = _cameraRotatorX;
    }

    //récupère la valeur de saut
    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rig.MovePosition(rig.position + velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero)
        {
            rig.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    private void PerformRotation()
    {
        rig.MoveRotation(rig.rotation * Quaternion.Euler(rotation));
        currentCameraRotationX += cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraClamp, cameraClamp);

        cam.transform.localEulerAngles = new Vector3(-currentCameraRotationX, 0f, 0f);
    }
}
