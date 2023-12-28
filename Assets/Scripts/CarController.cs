using UnityEngine;

public class CarController : MonoBehaviour {

    [SerializeField]
    private WheelJoint2D frontTire, backTire;

    [SerializeField]
    private float speed;

    private float movement, moveSpeed, fuel = 1, fuelConsumption = 0.1f;
    public float Fuel { get => fuel; set { fuel = value; } }

    public bool moveStop = false;

    public Vector3 StartPos { get; set; }

    private void Update() {
        if(GameManager.Instance.GasBtnPressed) {
            movement += 0.009f;
            if(movement > 1f)
                movement = 1f;
        }
        else if(GameManager.Instance.BrakeBtnPressed) {
            movement -= 0.009f;
            if(movement < -1f)
                movement = -1f;
        }
        else if(!GameManager.Instance.GasBtnPressed && !GameManager.Instance.BrakeBtnPressed) {
            movement = 0;
        }
        moveSpeed = movement * speed;

        GameManager.Instance.FuelConsume();
    }

    private void FixedUpdate() {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, StartPos.x, transform.position.x), transform.position.y);

        if(moveSpeed.Equals(0) || fuel <= 0) {
            frontTire.useMotor = false;
            backTire.useMotor = false;
        }
        else {
            frontTire.useMotor = true;
            backTire.useMotor = true;
            JointMotor2D motor = new JointMotor2D();
            motor.motorSpeed = moveSpeed;
            motor.maxMotorTorque = 10000;
            frontTire.motor = motor;
            backTire.motor = motor;
        }

        if(GameManager.Instance.isDie && moveStop) {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }

        fuel -= fuelConsumption * Mathf.Abs(movement) * Time.fixedDeltaTime;
    }
}