using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : GameEntity {

    public string unitID;
    public Group group;
    public bool aboveTheWall;
    public bool outLimit;
    public bool grounded;

    public float moveSpeed;

    public DamageModifier damageCalc;
    public static Status status;
    public UnitController controller;

    public static List<Points> allPoints = new List<Points>();
    public static GameState curState; 

    public class UnitController:Unit
    {
        // gets external commands, processes them, and executes actions to the unit under control
        // basic things such as movement, attacking, using abilities & items
        // needs a network remote tracker for state correction
        public void UnitMove()
        {
            //
        }

        public void AttackFacingDirection()
        {
            FaceDirection();
            AttackForward();
        }

        public void FaceDirection()
        {
            // turn the unit's shooter object
        }

        public void AttackForward()
        {
            // execute attack based on shooter's facing
        }

        public void BounceOff()
        {
            // bounce off walls, obstacles, or other units
            gameObject.AddComponent<Displacement>();
        }
    }

    public struct Group
    {
        //
    }

    public class Status
    {
        // durations of status effects will be referenced from the Buff class
        public bool stunned; // controller will ignore all commands (except some special cases)
        public bool snared; // controller will ignore move commands
        public bool slowed; // controller will execute modified move speed actions
        public float slowAmt; // additive percentages of all slows and speedups
        public bool silenced; // controller will ignore ability commands
        public bool disarmed; // controller will ignore
    }

    public class Points
    {
        public float curPoints;
        public float maxPoints;
        public float changedAmt;

        public Points(float max, float start = Mathf.Infinity)
        {
            maxPoints = max;
            points = start;
            allPoints.Add(this);
        }

        public float points
        {
            get
            {
                return curPoints;
            }
            set
            {
                float initial = curPoints;
                if (value > maxPoints) curPoints = maxPoints;
                else if (value < 0) curPoints = 0;
                changedAmt = curPoints - initial;
            }
        }

        public float maximum
        {
            get
            {
                return maxPoints;
            }
            set
            {
                if (value < curPoints)
                {
                    points = value;
                    maxPoints = value;
                }
                else maxPoints = value;
            }
        }
    }

    public struct GameState
    {
        public Vector3 lastPos;
        public Vector3 velocity;
        public Status lastStatus;
        public List<Points> lastPoints;
    }

    public GameState _states
    {
        get
        {
            GameState temp = new GameState();
            temp.lastPos = transform.position;
            temp.velocity = GetComponent<Rigidbody>().velocity;
            temp.lastStatus = status;
            temp.lastPoints = allPoints;
            return temp;
        }
    }

    public void Awake()
    {
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FixedUpdate()
    {
        curState = _states;
    }
}
