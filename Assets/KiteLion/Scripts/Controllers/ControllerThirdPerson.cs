using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System;

namespace KiteLion {

    namespace Controllers {

        public abstract class ControllerThirdPerson : PlayerController {

            public float LerpMaxThresholdHorizontal;
            public float LerpMinThresholdHorizontal;
            public float LerpMaxThresholdForward;
            public float LerpMinThresholdForward;
            public float LerpMinThresholdVertical;
            public float MaxSpeedHorizontalGrounded;
            public float MaxSpeedHorizontalAir;
            public float MaxSpeedForwardGrounded;
            public float MaxSpeedForwardAir;
            public float MaxSpeedVertical; //todo jump rework
            public float AccelerationSpeedHorizontalGrounded = 0.5f;
            public float AccelerationSpeedForwardGrounded = 0.5f;
            public float AccelerationSpeedHorizontalAir = 0.5f;
            public float AccelerationSpeedForwardAir = 0.5f;
            public float DecelerationSpeedHorizontalGrounded = 0.5f;
            public float DecelerationSpeedForwardGrounded = 0.5f;
            public float DecelerationSpeedHorizontalAir = 0.5f;
            public float DecelerationSpeedForwardAir = 0.5f;
            public float HorizontalSpeed;
            public float ForwardSpeed;
            public float JumpForce;

            //    public float MaxVelocityMag;

            //    public LayerMask mask = -1;

            //    public int SlotNum;

            private Rigidbody _Rigibody3D;
            private PhotonView _PhotonView;
            private PhotonTransformView _PhotonTransform;

            public int TotalJumpsAllowed;
            public int jumpLag;
            private bool isGrounded;
            private bool wasGrounded;
            private bool applyJumpPhysics;
            private int jumpsRemaining;
            private Vector3 jumpForceTemp;

            /// <summary>
            /// Subscribe and Remove your forces via "+=" and "-="
            /// </summary>
            public Forces.ExternalForce ExternalForces;
            /// <summary>
            /// Define your own Gravity via 
            ///     'player.GetComponent<MyComponent>().ExternalForces += new Forces.ExternalForce( MyGravityFunction );'
            /// Where your "MyComponent" inherits from ControllerThirdPerson and MyGravityFunction returns a Vector3.
            /// </summary>
            public bool EnableDefaultGravity;

            private float horizTilt;
            private float rightREMOVEME;
            private float forwardTilt;

            //    public float GroundCheckEndPoint;

            private Vector3 _position;
            private Vector3 _velocity;

            public bool isControlsDisabled;

            private PhotonArenaManager PM;

            //    private GameObject[] AttackObjs;
            //    public int AttackLag;
            //    public float AttackLife;
            //    private int totalAttackFrames;
            //    private WaitForSeconds attackDisableDelay;
            //    public float InvicibilityFrames;
            //    private float invincibilityCount;

            //    private bool facingRight;

            //    private bool punching;
            //    public int PunchPercentAdd;
            //    public float PunchForceUp;
            //    public float PunchForceForward_Forward;
            //    public float PunchForceForward_Up;
            //    public float PunchForceDown;
            //    public float PunchForceDecel;
            //    private float punchForceUpTemp;
            //    private float punchForceForward_ForwardTemp;
            //    private float punchForceForward_UpTemp;
            //    private float punchForceDownTemp;
            //    private Dictionary<string, float> StrengthsList;
            //	private bool punchForceApplied;
            //    public float PunchDisablePerc;

            //    private float damage;

            //    private int lastHitBy;
            //    private float lastHitTime;
            //    private float lastHitForgetLength;

            //    public AudioClip DeathNoise;
            //    private AudioSource myAudioSrc;

            //    private bool isFrozen;
            //    private bool controlsPaused;
            //    private float spawnPause;
            //    private WaitForSeconds spawnPauseWait;

            //    private Animator anim;

            //    public float BoxPunch;

            /// todo GOOGLE: New keyword vs OVERRIDE
            //public void Start() {
            //    CBUG.Do("blah");
            //}

            public void Start() {
                _position = new Vector2();
                _Rigibody3D = GetComponent<Rigidbody>();
                _PhotonView = GetComponent<PhotonView>();
                _PhotonTransform = GetComponent<PhotonTransformView>();

                ExternalForces = null;

                if(EnableDefaultGravity) {
                    ExternalForces += () => {
                        return Forces.Gravity3;
                    };
                }

                if (_PhotonView.IsMine) {
                    tag = "PlayerSelf";
                    //_PhotonView.RPC("SetSlotNum", RpcTarget.All, NetID.ConvertToSlot(PhotonNetwork.player.ID));
                }
                jumpsRemaining = TotalJumpsAllowed;

                PM = PhotonArenaManager.Instance;
        
            }


            void Update() {
                //_PhotonTransform.SetSynchronizedValues(_Rigibody2D.velocity, 0f);
                //if (!_PhotonView.isMine)
                //    return;

                //if (controlsPaused) {
                //    horizTilt = 0;
                //    rightREMOVEME = 0;
                //    return;
                //}


                //updateSpecials();
                updateJumping();
                //updateAttacks();
                updateMovement();
                //UpdateHurt();
            }

            void FixedUpdate() {
                _velocity = Vector3.zero;


                if (!_PhotonView.IsMine || PM.CurrentServerUserDepth == PhotonArenaManager.ServerDepthLevel.Offline)
                    return;

                updateIsGrounded();
                updateMovementPhysics();
                updateExternalForces();
                updateJumpingPhysics();

                if (!isControlsDisabled)
                    _Rigibody3D.velocity = _velocity;

                //if (wasGrounded && isGrounded) {
                //    canDownJump = true;
                //}
                //updateDownJumpingPhysics();
                ////limit max velocity.
                ////CBUG.Log("R Velocity: " + m_Body.velocity.magnitude);
                ////if (m_Body.velocity.magnitude >= MaxVelocityMag)
                ////{
                ////}
                //jumpForceTemp += Vector3.down * GravityForce;
                //if (jumpForceTemp.sqrMagnitude < 0) {
                //    jumpForceTemp = Vector3.zero;
                //} 
                //velocity += jumpForceTemp;
            }

            void LateUpdate() {
                //if (!_PhotonView.isMine)
                //    return;

                ////Only recent hits count
                //if (Time.time - lastHitTime > lastHitForgetLength) {
                //    lastHitBy = -1;
                //    lastHitTime = Time.time;
                //}
            }

            //    private void UpdateHurt()
            //    {
            //        if(invincibilityCount>=0)
            //            invincibilityCount--;
            //    }

            //    private void updateSpecials()
            //    {
            //        if (Input.GetButtonDown("Special") && invincibilityCount < 0)
            //        {
            //            _PhotonView.RPC("SpecialActivate", PhotonTargets.All);
            //            PauseMvmnt();
            //        }
            //    }

            private void updateJumping() {
                if (Input.GetButtonDown("Jump") == true && jumpsRemaining > 0) {
                    applyJumpPhysics = true;
                    CBUG.Do("Jumped is true!");
                    jumpsRemaining -= 1;
                }
            }

            private void updateMovement() {
                //tempAxis left n right, keyboard axis left n right, or no input
                horizTilt = Input.GetAxis("MoveHorizontal");
                forwardTilt = Input.GetAxis("MoveForward");
            }

            private void updateJumpingPhysics() {
                if (applyJumpPhysics) {
                    CBUG.Do("JUMP!");
                    CBUG.Do("JUMP!");
                    applyJumpPhysics = false;
                    _Rigibody3D.isKinematic = true;
                    _Rigibody3D.position += new Vector3(0f, 500f, 0f);
                    _Rigibody3D.isKinematic = false;
                }
            }

            private void updateExternalForces() {
                //externalForces += new Forces.ExternalForce(delegate { CBUG.Do("DEL"); });

                if (ExternalForces != null ) {
                    var results = ExternalForces.GetInvocationList();//.Select(x => (int)x.DynamicInvoke(2, 3));
                    foreach (Forces.ExternalForce force in results) {
                        _velocity += force();
                    }
                }
            }

            private void updateMovementPhysics() {
                //todo google: lerp "near zero" resolution
                //todo back and left don't work.
                //TODO  implement "play anyway." on version difference.
                float targetHorizontalSpeed = MaxSpeedHorizontalGrounded * horizTilt;
                float targetForwardSpeed = MaxSpeedForwardGrounded * forwardTilt;

                HorizontalSpeed = Mathf.Lerp(HorizontalSpeed, targetHorizontalSpeed, AccelerationSpeedHorizontalGrounded);
                //lerp fix
                //HorizontalSpeed = (HorizontalSpeed / targetHorizontalSpeed) > LerpMaxThresholdHorizontal ? MaxSpeedHorizontalGrounded : HorizontalSpeed;
                //HorizontalSpeed = (HorizontalSpeed / targetHorizontalSpeed) < LerpMinThresholdHorizontal ? 0f : HorizontalSpeed;

                ForwardSpeed = Mathf.Lerp(ForwardSpeed, targetForwardSpeed, AccelerationSpeedForwardGrounded);
                //lerp fix
                //ForwardSpeed = (ForwardSpeed / targetForwardSpeed) > LerpMaxThresholdForward ? MaxSpeedForwardGrounded : ForwardSpeed;
                //ForwardSpeed = (ForwardSpeed / targetForwardSpeed) < LerpMinThresholdForward ? 0f : ForwardSpeed;


                ////TODO LANDING LOGIC
                ////TODO IN-AIR LOGIC
                //else if (isGrounded) {
                //    CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0f, SpeedDecel);
                //}
                //else {
                //    CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0f, AirSpeedDecel);
                //}
                _velocity.x = HorizontalSpeed;
                _velocity.z = ForwardSpeed;
            }

            private void updateIsGrounded() {
                set3DPosition();

                //RaycastHit2D hit = 
                //    Physics2D.Raycast(position + JumpOffset,
                //                      -Vector2.up,
                //                      GroundCheckEndPoint,
                //                      mask.value
                //        );

                ////Debug.DrawLine(position+JumpOffset,
                ////               position + (-Vector2.up * GroundCheckEndPoint),
                ////               Color.red,
                ////               0.01f);
                //wasGrounded = isGrounded;
                //isGrounded = hit.collider != null;
                ////hit.collider.gameObject.layer
                /////Can't regain jumpcounts before jump force is applied.
                //if (isGrounded && !jumped) {
                //    //CBUG.Log("Grounded on: " + (hit.collider.name));
                //    jumpsRemaining = TotalJumpsAllowed;
                //    transform.SetParent(hit.collider.transform);
                //}
                //else if (!isGrounded) {
                //    transform.SetParent(null);
                //    //CBUG.Log("Grounded on: " + (hit.collider.name));
                //}
                //else {
                //    //CBUG.Log("No Raycast Contact.");
                //}
                ////if(!m_IsGrounded && down)
            }

            private void set3DPosition() {
                _position.x = transform.position.x;
                _position.y = transform.position.y;
                _position.z = transform.position.z;
            }

            //    private void updateAttacks()
            //    {
            //        if (isDead)
            //            return;

            //        if(totalAttackFrames < 0 ){
            //            if ((Input.GetButtonDown("Up") || _MobileInput.GetButtonDown("Up")) && !punching)
            //            {
            //                punching = true;
            //                AttackObjs[0].SetActive(true);
            //                StartCoroutine(disableDelay(AttackObjs[0]));
            //                totalAttackFrames = AttackLag;
            //                _PhotonView.RPC("UpAttack", PhotonTargets.Others);
            //            }
            //            if ((Input.GetButtonDown("Down") || _MobileInput.GetButtonDown("Down")) && !punching)
            //            {
            //                punching = true;
            //                AttackObjs[2].SetActive(true);
            //                StartCoroutine(disableDelay(AttackObjs[2]));
            //                totalAttackFrames = AttackLag;
            //                _PhotonView.RPC("DownAttack", PhotonTargets.Others);
            //            }
            //            if (facingRight && (Input.GetButtonDown("Right") || _MobileInput.GetButtonDown("Right")) && !punching)
            //            {
            //                punching = true;
            //                AttackObjs[1].SetActive(true);
            //                StartCoroutine(disableDelay(AttackObjs[1]));
            //                totalAttackFrames = AttackLag;
            //                _PhotonView.RPC("ForwardAttack", PhotonTargets.Others);
            //            }
            //            if (!facingRight && (Input.GetButtonDown("Left") || _MobileInput.GetButtonDown("Left")) && !punching)
            //            {
            //                punching = true;
            //                AttackObjs[1].SetActive(true);
            //                StartCoroutine(disableDelay(AttackObjs[1]));
            //                totalAttackFrames = AttackLag;
            //                _PhotonView.RPC("ForwardAttack", PhotonTargets.Others);
            //            }
            //        }
            //        totalAttackFrames -= 1;
            //    }

            //    #region RPC Functions.
            //    //TODO: Determine allowed scope of RPC functions.
            //    [PunRPC]
            //    void SetSlotNum(int SlotNUm)
            //    {
            //        this.SlotNum = SlotNUm;
            //        CBUG.Do("Recording ID " + SlotNUm + " with Gamemaster.");
            //        CBUG.Do("Character is: " + gameObject.name);
            //        GameManager.AddPlayer(SlotNUm, gameObject);
            //    }

            //    [PunRPC]
            //    void UpAttack()
            //    {
            //        AttackObjs[0].SetActive(true);
            //        StartCoroutine(disableDelay(AttackObjs[0]));
            //    }

            //    [PunRPC]
            //    void ForwardAttack()
            //    {
            //        AttackObjs[1].SetActive(true);
            //        StartCoroutine(disableDelay(AttackObjs[1]));
            //    }

            //    [PunRPC]
            //    void DownAttack()
            //    {
            //        AttackObjs[2].SetActive(true);
            //        StartCoroutine(disableDelay(AttackObjs[2]));
            //    }

            //    [PunRPC]
            //    void SpecialActivate()
            //    {
            //        anim.SetBool("Activating", true);
            //    }

            //    [PunRPC]
            //    void HurtAnim(int hurtNum)
            //    {
            //        switch (hurtNum)
            //        {
            //            case 1:
            //                anim.SetBool("HurtSmall", true);
            //                break;
            //            case 2:
            //                anim.SetBool("HurtMedium", true);
            //                break;
            //            case 3:
            //                anim.SetBool("HurtBig", true);
            //                break;
            //            default:
            //                CBUG.Error("BAD ANIM NUMBER GIVEN");
            //                break;
            //        }
            //    }


            //    /// <summary>
            //    /// Calls GameManager's RecordDeath. Respawn Handling
            //    /// is done from there.
            //    /// </summary>
            //    /// <param name="killer"></param>
            //    /// <param name="killed"></param>
            //    [PunRPC]
            //    private void OnDeath(int killer, int killed)
            //    {
            //        isDead = true;
            //        //Hide Self till respawn (or stay dead, ghost)
            //        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().enabled = false;

            //        //Freeze and Clear motion
            //        _Rigibody2D.velocity = Vector2.zero;
            //        velocity = Vector2.zero;

            //        //Death Map: OnDeath > RecordDeath > HandleDeath >
            //        // doRespawnOrGhost
            //        GameManager.RecordDeath(killer, killed);
            //        GameManager.HandleDeath(killed);

            //        //TODO: Animate death
            //    }
            //    #endregion

            //    /// <summary>
            //    /// Nothing to do. You stay invisible and immobile
            //    /// </summary>
            //    public override void Ghost()
            //    {
            //        transform.tag = "PlayerGhost";
            //    }

            //    /// <summary>
            //    /// Respawning. Only Graphical UNLESS is our Player. Then we reposition ourselves.
            //    /// </summary>
            //    /// <param name="spawnPoint"></param>
            //    public override void Respawn(Vector3 spawnPoint)
            //    {
            //        if (_PhotonView.isMine) {
            //            transform.position = spawnPoint;
            //            GameHUDController.LoseALife();
            //            GameHUDController.ResetDamage();
            //            damage = 0;
            //        }

            //        _Rigibody2D.isKinematic = false;
            //        StartCoroutine(spawnProtection()); 
            //    }
            //    private IEnumerator spawnProtection()
            //    {
            //        yield return spawnPauseWait;
            //        //TODO: Spawn animation
            //        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().enabled = true;

            //        isDead = false;
            //    }

            //    public void Freeze()
            //    {
            //        _Rigibody2D.isKinematic = true;
            //        isFrozen= true;
            //    }
            //    public void UnFreeze()
            //    {
            //        _Rigibody2D.isKinematic = false;
            //        isFrozen = false;
            //    }

            //    void OnTriggerEnter2D(Collider2D col)
            //    {
            //        //if (col.name == "Physics Box(Clone)")
            //        //{
            //        //    CBUG.Log("PUNCH");
            //        //    if(transform.localScale.x > 0){
            //        //        col.GetComponent<Rigidbody2D>().AddForce(new Vector2(BoxPunch, BoxPunch), ForceMode2D.Impulse);
            //        //    }else{
            //        //        col.GetComponent<Rigidbody2D>().AddForce(new Vector2(-BoxPunch, BoxPunch), ForceMode2D.Impulse);
            //        //    }
            //        //}

            //        if (isDead || col == null || !_PhotonView.isMine)
            //            return;


            //        //apply force . . .
            //        else if (col.name.Contains("Punch"))
            //        {
            //            //Get name of puncher
            //            lastHitBy = col.GetComponentInParent<PlayerController2DOnline>().SlotNum;
            //            lastHitTime = Time.time;

            //            if (invincibilityCount > 0)
            //            {
            //                return;
            //            }
            //            else
            //            {
            //                invincibilityCount = InvicibilityFrames;
            //            }
            //            damage += PunchPercentAdd;
            //            if (damage < 30)
            //            {
            //                _PhotonView.RPC("HurtAnim", PhotonTargets.All, 1);
            //            }
            //            else if (damage < 60)
            //            {
            //                _PhotonView.RPC("HurtAnim", PhotonTargets.All, 2);
            //            }
            //            else
            //            {
            //                _PhotonView.RPC("HurtAnim", PhotonTargets.All, 3);
            //            }
            //            GameHUDController.SetDamageTo(damage);
            //            if (col.name == "PunchForward")
            //            {
            //                //velocity += Vector2.right * PunchForceForward_Forward;
            //                if (col.transform.parent.localScale.x > 0)
            //                {
            //                    Vector2 temp = Vector2.right * (PunchForceForward_Forward + StrengthsList[col.transform.parent.name] - Defense);
            //                    temp += Vector2.up * (PunchForceForward_Up + StrengthsList[col.transform.parent.name] - Defense);
            //                    StartCoroutine(
            //                        applyPunchForce(temp * (damage/100f))
            //                    );
            //                }
            //                else
            //                {
            //                    Vector2 temp = Vector2.left * (PunchForceForward_Forward + StrengthsList[col.transform.parent.name] - Defense);
            //                    temp += Vector2.up * (PunchForceForward_Up + StrengthsList[col.transform.parent.name] - Defense);
            //                    StartCoroutine(
            //                        applyPunchForce(temp * (damage / 100f))
            //                    );
            //                }
            //            }
            //            else if (col.name == "PunchUp")
            //            {
            //                StartCoroutine(
            //                    applyPunchForce(
            //                        (Vector2.up * (PunchForceUp + StrengthsList[col.transform.parent.name] - Defense) * (damage / 100f))
            //                    )
            //                );
            //            }
            //            else
            //            {
            //                if (!isGrounded)
            //                {
            //                    StartCoroutine(
            //                        applyPunchForce(
            //                            (Vector2.down * (PunchForceDown + StrengthsList[col.transform.parent.name] - Defense) * (damage / 100f))
            //                        )
            //                    );
            //                }
            //                else
            //                {
            //                    StartCoroutine(
            //                        applyPunchForce(
            //                            (Vector2.up * (PunchForceDown + StrengthsList[col.transform.parent.name] - Defense) * (damage / 200f))
            //                        )
            //                    );
            //                }
            //            }
            //        }
            //    }

            //    private IEnumerator disableDelay(GameObject dis)
            //    {
            //        yield return attackDisableDelay;
            //        dis.SetActive(false);
            //        punching = false;
            //    }

            //    private IEnumerator applyPunchForce(Vector2 punchForce)
            //    {
            //        Vector2 tempPunchForce = punchForce;
            //        bool isTempForceLow = false;
            //        CamManager.PunchShake(tempPunchForce.magnitude);
            //        while (tempPunchForce.magnitude > 0.01f)
            //        {
            //            velocity += tempPunchForce;
            //            tempPunchForce = Vector2.Lerp(tempPunchForce, Vector2.zero, PunchForceDecel);

            //            if (!isTempForceLow &&
            //                tempPunchForce.magnitude < punchForce.magnitude * PunchDisablePerc)
            //            {
            //                isTempForceLow = true;
            //                //if the force goes below 25%, let the character move again. 
            //	        	punchForceApplied = false;
            //            }
            //            else if(!isTempForceLow)
            //            {
            //			    punchForceApplied = true;
            //            }
            //            yield return null;
            //        }
            //    }

            //    void OnTriggerExit2D(Collider2D col)
            //    {
            //        if (col.tag != "DeathWall")
            //            return;
            //        myAudioSrc.Play();
            //        CamManager.DeathShake(_PhotonView.isMine);

            //        if (!_PhotonView.isMine)
            //            return;

            //        _PhotonView.RPC("OnDeath", PhotonTargets.All, lastHitBy, SlotNum);
            //    }	  	


            //    public bool GetIsDead()
            //    {
            //        return isDead;
            //    }

            //    public void PauseMvmnt()
            //    {
            //        controlsPaused = true;
            //    }

            //    public void UnpauseMvmnt()
            //    {
            //        controlsPaused = false;
            //    }
            //}
            ////public void CheckWon()
            ////{
            ////    if (m_PhotonView.isMine && !isDead)
            ////    {
            ////        BattleUI.Won();
            ////    }

            public void AddExternalForce(Forces.ExternalForce newForce) {
                ExternalForces += new Forces.ExternalForce(newForce);
            }

            public void RemoveExternalForce() {

            }

            public override void Die() {
                throw new System.NotImplementedException();
            }

            public override void Respawn(Vector3 spawnPoint) {
                throw new System.NotImplementedException();
            }


            //public Forces.ExternalForce ExternalForces { get => externalForces; set => externalForces += value; }

        }
    }
}