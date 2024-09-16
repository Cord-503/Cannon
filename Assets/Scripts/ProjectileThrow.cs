using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TrajectoryPredictor))]
public class ProjectileThrow : MonoBehaviour
{
    TrajectoryPredictor trajectoryPredictor;
    [SerializeField] GameObject objectToThrow;
    [SerializeField, Range(0.0f, 40.0f)] float force;
    [SerializeField] Transform StartPosition;
    [SerializeField] Vector3 gravity = new Vector3(0, -9.81f, 0);
    [SerializeField] float drag = 0.1f;
    public InputAction fire;

    void OnEnable()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();
        if (StartPosition == null)
        {
            StartPosition = transform;
        }

        fire.Enable();
        fire.performed += ThrowObject;
    }

    void OnDisable()
    {
        fire.Disable();
        fire.performed -= ThrowObject;
    }

    void Update()
    {
        Predict();
    }

    void Predict()
    {
        trajectoryPredictor.PredictTrajectory(ProjectileData());
    }

    ProjectileProperties ProjectileData()
    {
        ProjectileProperties properties = new ProjectileProperties();
        properties.direction = StartPosition.forward;
        properties.initialPosition = StartPosition.position;
        properties.initialSpeed = force;
        properties.mass = 1f;
        properties.drag = drag;
        return properties;
    }

    void ThrowObject(InputAction.CallbackContext ctx)
    {
        GameObject thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
        CustomProjectileMotion motion = thrownObject.AddComponent<CustomProjectileMotion>();
        motion.Initialize(StartPosition.forward * force, gravity, drag);
    }
}

public class CustomProjectileMotion : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 gravity;
    private float drag;

    public void Initialize(Vector3 initialVelocity, Vector3 gravity, float drag)
    {
        this.velocity = initialVelocity;
        this.gravity = gravity;
        this.drag = drag;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        transform.position += velocity * deltaTime;

        velocity += gravity * deltaTime;
        velocity -= velocity * drag * deltaTime;
    }
}