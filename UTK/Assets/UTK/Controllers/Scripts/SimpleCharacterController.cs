using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Controller.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleCharacterController : MonoBehaviour
    {
        [SerializeField]
        protected float speed = 20.0f;

        [SerializeField]
        protected float jumpSpeed = 40.0f;

        [SerializeField]
        protected float gravity = 120.0f;

        [SerializeField]
        protected Vector3 moveDirection;

        [SerializeField]
        [Header("Adapts to changes in the move direction even during jumps.")]
        protected bool enableJumpingMove;

        private CharacterController controller;
        private bool isReadyJump;

        //Movement vector in the current frame to which speed etc. is added based on moveDirection.
        private Vector3 currentDirection;

        #region Property

        public float Speed { get => speed; set => speed = value; }
        public float JumpSpeed { get => jumpSpeed; set => jumpSpeed = value; }
        public float Gravity { get => gravity; set => gravity = value; }
        public Vector3 MoveDirection { get => moveDirection; set => moveDirection = value; }
        public bool IsJumping { get => !controller.isGrounded; }

        #endregion

        #region Editor

#if UNITY_EDITOR

        [Space]
        public bool editor_EnableJumpInput;
        public bool editor_EnableMoveInput;

        private void Editor_MoveInput()
        {
            if (!editor_EnableMoveInput) return;
            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");
            moveDirection = new Vector3(horizontal, 0.0f, vertical);
        }

        private void Editor_JumpInput()
        {
            if (!editor_EnableJumpInput) return;
            isReadyJump = Input.GetButtonDown("Jump");
        }

#endif

        #endregion

        public void OnJump()
        {
            isReadyJump = true;
        }

        #region Internal

        // Start is called before the first frame update
        protected virtual void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {

#if UNITY_EDITOR
            Editor_MoveInput();
            Editor_JumpInput();
#endif

            Move();
        }

        //Move controller
        protected virtual void Move()
        {
            if (controller.isGrounded)
            {
                currentDirection = moveDirection;
                currentDirection *= speed;

                if (isReadyJump)
                {
                    isReadyJump = true;
                    currentDirection.y = jumpSpeed;
                }
            }
            else
            {
                if (enableJumpingMove)
                {
                    var yvalue = currentDirection.y;
                    currentDirection = moveDirection;
                    currentDirection *= speed;
                    currentDirection.y = yvalue;
                }
            }

            //MS ^ -2
            currentDirection.y -= gravity * Time.deltaTime;
            controller.Move(currentDirection * Time.deltaTime);
        }

        #endregion
    }
}
