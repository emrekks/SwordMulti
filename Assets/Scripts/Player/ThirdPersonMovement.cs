using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

public class ThirdPersonMovement : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHealthChanged))]
    public int CurrentHealth{ get; set; }

    public static ThirdPersonMovement Instance;
    
    public HotbarController hotbarController;
    
    public CinemachineFreeLook cinemachineCam;
    
    public Image healthBar;
    
    private Rigidbody _rb;

    private Transform _cam;

    private Animator _anim;

    private PlayerInputSystem _playerInputSystem; // Yeni input sistemini tanimlar

    private Vector2 _inputVector; // Karakteri hareket ettirebilmek icin gerekli olan horizontal ve Vertical degerleri alir
    
    private Vector3 _moveDir; // Karakterin yuzu donuk oldugu aciyi algilar

    private bool _groundCheck; // Karakterin yerle temasini kontrol eder

    private bool inventoryOpened = false; // Envanteri acip kapatmak icin

    [Header("Values")]
    
    [Tooltip("Karakterin donusunu suresi")] [SerializeField]private float turnSmoothTime = 0.1f;
    
    [Tooltip("Karakterin donus hizi")] [SerializeField]private float turnSmoothVelocity;

    [Tooltip("Karakterin hareket hizi")] [SerializeField]private float speed = 6f;
    
    [Tooltip("Karakterin ziplama gucu")] [SerializeField]private float jumpForce = 6f;

    //[Tooltip("Karakterin can degeri")] [SerializeField]private int health = 100;
    public const int health = 100;

    // [Tooltip("Karakterin guncel can degeri")] [SerializeField]private int currentHealth = 100;

    [SerializeField] GameObject canvas;

    public override void Spawned()
    {
        canvas.SetActive(false);
        if (!Object.HasStateAuthority)
        {
            return;
        }

        canvas.SetActive(true);
        Instance = this;

        Runner.SetPlayerObject(Object.StateAuthority, Object);

        
        _rb = GetComponent<Rigidbody>();

        _anim = GetComponent<Animator>();

        _cam = Camera.main.transform;

        _playerInputSystem = new PlayerInputSystem();

        _playerInputSystem.Player.Enable();
        
        _playerInputSystem.Player.Jump.performed += JumpOnperformed;
        
        _playerInputSystem.Player.Attack.performed += AttackOnPerformed;
        
        _playerInputSystem.Player.Inventory.performed += InventoryOnPerformed;

        CurrentHealth = health;
         
        Cursor.visible = inventoryOpened;

        cinemachineCam.Priority++;
    }

    private void InventoryOnPerformed(InputAction.CallbackContext obj) //left shift
    {
        inventoryOpened = !inventoryOpened;
        Cursor.visible = inventoryOpened; 
    }

    private void AttackOnPerformed(InputAction.CallbackContext context) // left mouse button
    {
        if (context.performed && !inventoryOpened && hotbarController.selectedItem.CompareTag("Sword"))
        {
            _anim.SetTrigger("attack");
        }
        
        else if (context.performed && !inventoryOpened && hotbarController.selectedItem.CompareTag("Potion") && CurrentHealth < 100 && _groundCheck)
        {
            _anim.SetTrigger("potion");

            Healing(25);
        }
        
        else
        {
            return;
        }
    }
    

    public void FixedUpdate()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        
        Movement();
        
        GroundCheck();
    }

    private void Movement() // WASD
    {
        if (!inventoryOpened)
        {
            _inputVector = _playerInputSystem.Player.Movement.ReadValue<Vector2>();
            
            if (_inputVector.magnitude >= 0.01f)
            {
                float targetAngel = Mathf.Atan2(_inputVector.x, _inputVector.y) * Mathf.Rad2Deg + _cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngel, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                _moveDir = Quaternion.Euler(0f, targetAngel, 0f) * Vector3.forward;
                if (_groundCheck && _anim.GetCurrentAnimatorStateInfo(0).IsName("run"))
                {
                    _rb.AddForce(_moveDir.normalized * speed, ForceMode.Force);
                }
            }
        }

        else
        {
            _rb.velocity = (Vector3.zero);
            _inputVector = Vector2.zero;
        }
        
        _anim.SetFloat("speed", _inputVector.magnitude);
    }
    
    private void JumpOnperformed(InputAction.CallbackContext context) // space
    {
        if (context.performed && _groundCheck && !inventoryOpened)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void GroundCheck()
    {
        _groundCheck = (Physics.Raycast(transform.position, Vector3.down, 0.6f, LayerMask.GetMask("Ground")));

        _anim.SetBool("jump", _groundCheck);
    }
    
    public void Healing(int amounth) // oyuncunun canini arttirir
    {
        CurrentHealth += amounth;
           
        if (CurrentHealth > 100)
        {
            CurrentHealth = 100;
        }
        
        healthBar.fillAmount = (float)CurrentHealth / health;
    }

    public void Attacked(PlayerRef player)
    {
        Rpc_Attacked(player);
    }

    [Rpc]
    public void Rpc_Attacked([RpcTarget] PlayerRef player)
    {
        Instance.CurrentHealth -= 20;
    }

    //public void GetDamage(PlayerRef plyr, int damage)
    //{
    //    Rpc_GetDamage(plyr, damage);
    //}
    //[Rpc]
    //public void Rpc_GetDamage([RpcTarget] PlayerRef plyr, int getDmg) // oyuncunun canini azaltir
    //{
    //    Instance.GetDamagedTaked(getDmg);
    //}


    public static void OnHealthChanged(Changed<ThirdPersonMovement> changed)
    {
        changed.LoadNew();
        changed.Behaviour.HealthChanged();
    }

    public void HealthChanged()
    {
        healthBar.fillAmount = (float)CurrentHealth / health;
    }
}
