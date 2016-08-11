using UnityEngine;
using System;
using System.Collections;

public enum BeltItem
{
    Empty,
    PowerUp,
    Medkit,
    HeartSensor,
    Grenade,
    TimeBomb,
    SmokeBomb,
    MegaJump,
    RocketLauncher,
    Knife,
    Sniper
}

public enum slotState
{
    empty,
    filled,
    locked,
    disabled,
    preset
}

public class Player001 : Unit
{
    public class Belt
    {
        public class Slot
        {
            public ItemFunction itemFunction;
            public slotState _state = slotState.empty;
            public BeltItem _item = BeltItem.Empty;
            public bool _ready = false;

            public slotState state
            {
                get { return _state; }
                set
                {
                    if (value == slotState.filled) return;
                    else
                    {
                        _item = BeltItem.Empty;
                        _ready = false;
                        itemFunction = null;
                    }
                    _state = value;
                }
            }

            public BeltItem item
            {
                get { return _item; }
                set
                {
                    if (_state == slotState.locked | _state == slotState.preset) return;
                    if (value == BeltItem.Empty)
                    {
                        _state = slotState.empty;
                        _ready = false;
                    }
                    else if (_state == slotState.empty) _state = slotState.filled;
                    _item = value;
                    _ready = false;
                }
            }

            public bool ready
            {
                get { return _ready; }
                set
                {
                    _ready = value;
                    if (_state == slotState.locked | _state == slotState.empty)
                    {
                        _ready = false;
                    }
                }
            }
        }

        public Belt(int totalSlots)
        {
            slots = new Slot[totalSlots];
            for (int s = 0; s <= totalSlots - 1; s++ )
            {
                slots[s] = new Slot();
            }
        }

        public Slot[] slots;
        public int _replaceIndex = 0;
        public int replaceIndex // cycle through the slots. this was very hard to make (sigh...)
        {
            get
            {
                return _replaceIndex;
            }
            set
            {
                bool canReplace = false;
                int marker = value;
                while (!canReplace)
                {
                    if (marker == _replaceIndex) marker = _replaceIndex++;
                    if (marker > slots.Length-1) marker = 0;
                    if (slots[marker]._state == slotState.locked | slots[marker]._state == slotState.preset) marker++;
                    else canReplace = true;
                }
                _replaceIndex = marker;
            }
        }
    }

    public Points health;
    public Points armor;
    public Belt belt;

    CharacterController chr; // access old system. change later

    void Awake()
    {
        chr = gameObject.GetComponent<CharacterController>();
        // should use UnitController instead
    }

	// Use this for initialization
    void Start()
    {
        health = new Points(100);
        armor = new Points(0);
        belt = new Belt(3);
        belt.slots[2].state = slotState.locked;
	}
	
	// Update is called once per frame
	void Update () {
        MonitorItemFunctions();
	}

    public void UseBeltItem(int index, bool charge = false)
    {
        Belt.Slot slot = belt.slots[index];
        if (slot.item == BeltItem.Empty)
        {
            // do nothing
        }

        else if (slot.item == BeltItem.Grenade)
        {
            Granade grnd = slot.itemFunction as Granade;

            // if input is tap, use if ready
            // if input is press, charge
            if (charge)
            {
                grnd.chrg.isCharging = true;
                grnd.ready = true;
                return;
            }

            if (!slot.ready) return;
            grnd.Execute();
            RemoveItemFromBelt(index);
        }

        else if (slot.item == BeltItem.SmokeBomb)
        {
            SmokeBomb grnd = slot.itemFunction as SmokeBomb;

            // if input is tap, use if ready
            // if input is press, charge
            if (charge)
            {
                grnd.chrg.isCharging = true;
                grnd.ready = true;
                return;
            }

            if (!slot.ready) return;
            grnd.Execute();
            RemoveItemFromBelt(index);
        }

        else if (slot.item == BeltItem.MegaJump)
        {
            MegaJump mjmp = slot.itemFunction as MegaJump;

            // if input is tap, use if ready
            // if input is press, charge
            if (charge)
            {
                mjmp.chrg.isCharging = true;
                mjmp.ready = true;
                return;
            }

            if (!slot.ready) return;
            mjmp.Jump();
            RemoveItemFromBelt(index);
        }

        else if (slot.item == BeltItem.HeartSensor)
        {
            Adrenaline hrtsnsr = gameObject.GetComponent<Adrenaline>();

            // if input is tap, use if ready
            if (!slot.ready) return;
            if (hrtsnsr == null)
            {
                gameObject.AddComponent<Adrenaline>();
            }
            else
            {
                hrtsnsr.Recharge(); // resets duration
            }
            RemoveItemFromBelt(index);

            //gameObject.AddComponent<ItemEffect>();
        }

        else if (slot.item == BeltItem.Knife)
        {
            Knife knf = slot.itemFunction as Knife;

            // if input is tap, use if ready
            if (!slot.ready) return;
            knf.Execute();
            RemoveItemFromBelt(index);

            //gameObject.AddComponent<ItemEffect>();
            //gameObject.AddComponent<Displacement>();
        }

        else if (slot.item == BeltItem.Medkit)
        {
            // just use on tap
            if (!slot.ready) return;
            chr.hp += 20;
            SoundManager.instance.PlaySfx("sfx_heal");
            RemoveItemFromBelt(index);
            //gameObject.AddComponent<Buff>();
        }

        else if (slot.item == BeltItem.RocketLauncher)
        {
            RocketLauncher rktlnchr = gameObject.GetComponent<RocketLauncher>();
            slot.ready = true;

            // change weapon to rocket launcher
            // continuous shooting
            // unable to stop shooting
            if (!slot.ready) return;
            if (rktlnchr == null)
            {
                gameObject.AddComponent<RocketLauncher>();
            }
            else
            {
                rktlnchr.rocketTime += rktlnchr.dur; // adds duration
            }
            RemoveItemFromBelt(index);

            //gameObject.AddComponent<ItemEffect>();
        }

        else if (slot.item == BeltItem.Sniper)
        {
            Sniper snpr = slot.itemFunction as Sniper;

            // if input is tap, use if ready
            //snare player and choose target
            if (!slot.ready) return;
            snpr.Sniping();
        }

        else if (slot.item == BeltItem.TimeBomb)
        {
            TimeBomb tmbmb = slot.itemFunction as TimeBomb;

            // if input is tap, use if ready
            if (!slot.ready) return;
            tmbmb.Execute();
            RemoveItemFromBelt(index);
        }
    }

    /*public void ChargeBeltItem(int index)
    {
        Belt.Slot slot = belt.slots[index];
        if (slot.item == BeltItem.Grenade)
        {
            if (!(slot.itemFunction is Granade)) return;
            Granade grn = slot.itemFunction as Granade;
            grn.chrg.isCharging = true;
        }

        else if (slot.item == BeltItem.SmokeBomb)
        {
            if (!(slot.itemFunction is SmokeBomb)) return;
            SmokeBomb grn = slot.itemFunction as SmokeBomb;
            grn.chrg.isCharging = true;
        }

        else if (slot.item == BeltItem.MegaJump)
        {
            if (!(slot.itemFunction is MegaJump)) return;
            MegaJump grn = slot.itemFunction as MegaJump;
            grn.chrg.isCharging = true;
        }

        else
        {
            // do nothing
        }
    }*/

    public void UpdateBeltFunctions()
    {
        for (int s = 0; s < belt.slots.Length; s++)
        {
            Belt.Slot slot = belt.slots[s];

            if (slot.item == BeltItem.Grenade)
            {
                if (slot.itemFunction is Granade) continue;
                Destroy(slot.itemFunction);

                Granade grnd = gameObject.AddComponent<Granade>();
                grnd.chrg = gameObject.AddComponent<GrenadeCharger>();
                slot.itemFunction = grnd;
            }

            else if (slot.item == BeltItem.Knife)
            {
                if (slot.itemFunction is Knife) continue;
                Destroy(slot.itemFunction);

                slot.itemFunction = gameObject.AddComponent<Knife>();
            }

            else if (slot.item == BeltItem.Sniper)
            {
                if (slot.itemFunction is Sniper) continue;
                Destroy(slot.itemFunction);

                slot.itemFunction = gameObject.AddComponent<Sniper>();
            }

            else if (slot.item == BeltItem.SmokeBomb)
            {
                if (slot.itemFunction is SmokeBomb) continue;
                Destroy(slot.itemFunction);

                SmokeBomb grnd = gameObject.AddComponent<SmokeBomb>();
                grnd.chrg = gameObject.AddComponent<GrenadeCharger>();
                slot.itemFunction = grnd;
            }

            else if (slot.item == BeltItem.MegaJump)
            {
                if (slot.itemFunction is MegaJump) continue;
                Destroy(slot.itemFunction);

                MegaJump mgjmp = gameObject.AddComponent<MegaJump>();
                mgjmp.chrg = gameObject.AddComponent<JumpCharger>();
                slot.itemFunction = mgjmp;
            }

            else if (slot.item == BeltItem.TimeBomb)
            {
                if (slot.itemFunction is TimeBomb) continue;
                Destroy(slot.itemFunction);

                TimeBomb tb = gameObject.AddComponent<TimeBomb>();
                slot.itemFunction = tb;
            }
            else
            {
                if (slot.itemFunction == null) continue;
                Destroy(slot.itemFunction);
                slot.itemFunction = null;
            }
        }
    }

    public void AddItemToBelt(BeltItem newItem)
    {
        SoundManager.instance.PlaySfx("sfx_getweap");
        int hasEmpty = -1;
        for (int s = 0; s < belt.slots.Length; s++)
        {
            if (belt.slots[s].state == slotState.empty)
            {
                hasEmpty = s;
                break;
            }
        }

        if (hasEmpty >= 0) belt.replaceIndex = hasEmpty;
        else belt.replaceIndex++;
        
        belt.slots[belt._replaceIndex].item = newItem;
        UpdateBeltFunctions();
    }

    public void RemoveItemFromBelt(int index)
    {
        belt.slots[index].state = slotState.empty;
    }

    public void MonitorItemFunctions()
    {
        for (int s = 0; s < belt.slots.Length; s++)
        {
            ItemFunction func = belt.slots[s].itemFunction as ItemFunction;
            if (func == null)
            {
                ReadyItem(s);
                continue;
            }
            bool active = func.ready;
            ReadyItem(s,active);
            if (func.discarded) RemoveItemFromBelt(s);
        }
    }

    public void ReadyItem(int index, bool ready = true)
    {
        belt.slots[index].ready = ready;
    }

    public void AddPowerUp(string name)
    {
        SoundManager.instance.PlaySfx("sfx_getPU");
        if (name == "power")
        {
            chr.power++;
        }
        else if (name == "shootRange")
        {
            chr.shootRange++;
        }
        else if (name == "shootRate")
        {
            chr.shootRate++;
        }
        else if (name == "speed")
        {
            chr.speed++;
        }
    }
}
