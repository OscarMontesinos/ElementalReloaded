using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PjBase : MonoBehaviour, TakeDamage
{
    Rigidbody2D rb;
    public UIManager UIManager;
    [HideInInspector]
    public GameManager manager;
    [HideInInspector]
    public PlayerController controller;
    [HideInInspector]
    public Animator animator;
    public GameObject sprite;
    public string chName;
    public int team;
    [HideInInspector]
    public bool lockPointer;
    public GameObject pointer;
    public HitData.Element element;
    public HitData.Element element2;
    public GameObject spinObjects;
    public Slider hpBar;
    public Slider stunnBar;
    public Slider shieldBar;
    public GameObject moveContainer;
    public GameObject moveBasic;
    public GameObject move1;
    public GameObject move2;
    public GameObject move3;
    [HideInInspector]
    public Move currentMoveBasic;
    [HideInInspector]
    public Move currentMove1;
    [HideInInspector]
    public Move currentMove2;
    [HideInInspector]
    public Move currentMove3;
    [HideInInspector]
    public float currentBasicCd;
    [HideInInspector]
    public float currentHab1Cd;
    public float hab1Cd;
    [HideInInspector]
    public float currentHab2Cd;
    public float hab2Cd;
    [HideInInspector]
    public float currentHab3Cd;
    public float hab3Cd;
    [HideInInspector]
    public bool casting;
    [HideInInspector]
    public bool dashing;
    [HideInInspector]
    public float stunTime;
    [HideInInspector]
    public bool ignoreSoftCastDebuff;
    public Stats stats;
    public float damageTextOffset;
    [HideInInspector]
    public float dmgDealed;
    public bool hide;
    public List<GameObject> revealedByList = new List<GameObject>();
    public GameObject visuals;
    float healCount;
    float dmgCount;
    public enum AttackType
    {
        Physical, Magical, None
    }

    public virtual void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (moveBasic)
        {
            currentMoveBasic = Instantiate(moveBasic, moveContainer.transform).GetComponent<Move>();
            currentMoveBasic.user = this;
        }
        if (move1)
        {
            currentMove1 = Instantiate(move1, moveContainer.transform).GetComponent<Move>();
            currentMove1.user = this;
        }
        if (move2)
        {
            currentMove2 = Instantiate(move2, moveContainer.transform).GetComponent<Move>();
            currentMove2.user = this;
        }
        if (move3)
        {
            currentMove3 = Instantiate(move3, moveContainer.transform).GetComponent<Move>();
            currentMove3.user = this;
        }
    }
    public virtual void Start()
    {
        stats.hp = stats.mHp;

        GameManager.Instance.pjList.Add(this);
        if (hpBar != null)
        {
            shieldBar = hpBar.transform.parent.GetChild(2).GetComponent<Slider>();
        }
    }
    public virtual void Update()
    {
        if(stats.hp < stats.mHp)
        {
            stats.hp += stats.healthRegen * Time.deltaTime;
            if(stats.hp > stats.mHp)
            {
                stats.hp = stats.mHp;
            }
        }
        if (hpBar != null)
        {
            hpBar.maxValue = stats.mHp;
            hpBar.value = stats.hp;

            shieldBar.value = stats.shield;
            shieldBar.maxValue = stats.mHp;
        }
        if (hide && revealedByList.Count == 0)
        {
            visuals.SetActive(false);
        }
        else
        {
            visuals.SetActive(true);
        }

        if (stunTime > 0)
        {
            stunTime -= Time.deltaTime;
            if (stunnBar != null)
            {
                if (stunnBar.maxValue < stunTime)
                {
                    stunnBar.maxValue = stunTime;
                }

                stunnBar.value = stunTime;
            }
        }
        else
        {
            if (stunnBar != null)
            {
                stunnBar.maxValue = 0.3f;
                stunnBar.value = 0;
            }
        }

        if (currentBasicCd > 0)
        {
            currentBasicCd -= Time.deltaTime;
        }

        RechargeHab1();

        RechargeHab2();

        RechargeHab3();

        if (spinObjects != null)
        {
            spinObjects.transform.rotation = pointer.transform.rotation;
        }

    }

    public virtual void RechargeHab1()
    {
        if (currentHab1Cd > 0)
        {
            currentHab1Cd -= Time.deltaTime;
        }
    }
    public virtual void RechargeHab2()
    {
        if (currentHab2Cd > 0)
        {
            currentHab2Cd -= Time.deltaTime;
        }
    }
    public virtual void RechargeHab3()
    {
        if (currentHab3Cd > 0)
        {
            currentHab3Cd -= Time.deltaTime;
        }
    }

    public virtual IEnumerator MainAttack()
    {
        if (currentBasicCd <= 0 && !casting)
        {
            currentBasicCd = CalculateAtSpd(currentMoveBasic.cd * stats.atSpd);
            controller.lockPointer = false;
            yield return null;
            controller.lockPointer = true;
            StartCoroutine(PlayAnimation(currentMoveBasic.anim));
            currentMoveBasic.Trigger();
        }
    }

    public virtual IEnumerator Hab1()
    {
        if (currentHab1Cd <= 0 && !casting)
        {
            controller.lockPointer = false;
            if (currentMove1.cast)
            {
                casting = true;
            }
            yield return null;
            if (currentMove1.lockPointer)
            {
                controller.lockPointer = true;
            }
            StartCoroutine(PlayAnimation(currentMove1.anim));
            currentHab1Cd = CDR(currentMove1.cd);
            currentMove1.Trigger();
        }
    }

    public virtual IEnumerator Hab2()
    {
        if (currentHab2Cd <= 0 && !casting)
        {
            controller.lockPointer = false;
            if (currentMove2.cast)
            {
                casting = true;
            }
            yield return null;
            if (currentMove2.lockPointer)
            {
                controller.lockPointer = true;
            }
            StartCoroutine(PlayAnimation(currentMove2.anim));
            currentHab2Cd = CDR(currentMove2.cd);
            currentMove2.Trigger();
        }
    }

    public virtual IEnumerator Hab3()
    {
        if (currentHab3Cd <= 0 && !casting)
        {
            controller.lockPointer = false;
            if (currentMove3.cast)
            {
                casting = true;
            }
            yield return null;
            if (currentMove3.lockPointer)
            {
                controller.lockPointer = true;
            }
            StartCoroutine(PlayAnimation(currentMove3.anim));
            currentHab3Cd = CDR(currentMove3.cd);
            currentMove3.Trigger();
        }
    }

    public virtual void UsedBasicDash()
    { 
    
    }
    public virtual void EndedBasicDash()
    { 
    
    }
    public virtual void UsedBasicDashGlobal()
    { 
    
    }
    public virtual void EndedBasicDashGlobal()
    { 
    
    }

    public IEnumerator PlayAnimation(string name)
    {
        animator.Play("Idle");
        yield return null;
        animator.Play(name);
    }

    public virtual void AnimationCallStopAnim()
    {
        casting = false;
        controller.lockPointer = false;
    }

    public bool IsCasting()
    {
        if(!casting)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool IsStunned()
    {
        if (stunTime > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsDashing()
    {
        if (dashing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RegisterDamage(float dmg)
    {
        if (controller != null)
        {
            dmgDealed += dmg;
            UIManager.UpdateDamageText();
        }
    }

    public virtual void DamageDealed(PjBase user, PjBase target, float amount, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {
        List<HitInteract> hitList = new List<HitInteract>( target.gameObject.GetComponents<HitInteract>());
        foreach (HitInteract hit in hitList)
        {
            hit.Interact(user,target,amount,element,attackType,habType);
        }
        
    }

    void TakeDamage.TakeDamage(PjBase user,float value, HitData.Element element, AttackType type)
    {
        TakeDmg(user, value, element, type);
    }

    public virtual void TakeDmg(PjBase user, float value, HitData.Element element, AttackType type)
    {
        user.RegisterDamage(value - stats.shield);
        float calculo = 0;
        DamageText dText = null;
        if (value + dmgCount > 1)
        {
            switch (element)
            {
                case HitData.Element.ice:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.iceColor;
                    break;
                case HitData.Element.fire:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.fireColor;
                    break;
                case HitData.Element.water:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.waterColor;
                    break;
                case HitData.Element.desert:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.desertColor;
                    break;
                case HitData.Element.wind:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.windColor;
                    break;
                case HitData.Element.nature:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.natureColor;
                    break;
                case HitData.Element.lightning:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.lightningColor;
                    break;
            }
        }

        if (type == AttackType.Magical)
        {
            calculo = stats.mResist;
        }
        else
        {
            calculo = stats.fResist;
        }

        if (calculo < 0)
        {
            calculo = 0;
        }
        value -= ((value * ((calculo / (100 + calculo) * 100))) / 100);
        float originalValue = value;
        if (controller != null)
        {
            while (stats.shield > 0 && value > 0)
            {
                Shield chosenShield = null;
                foreach (Shield shield in GetComponents<Shield>())
                {
                    if (chosenShield == null || shield.time < chosenShield.time && shield.shieldAmount > 0)
                    {
                        chosenShield = shield;
                    }
                }
                value = chosenShield.ChangeShieldAmount(-value);

            }

            /*if(value != originalValue)
            {
                originalValue -= value;
                DamageText sText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                sText.textColor = Color.white;
                sText.damageText.text = originalValue.ToString("F0");
            }*/
        }


        if (dText != null)
        {
            dText.damageText.text = (value + dmgCount).ToString("F0");
            dmgCount = 0;
        }
        else
        {
            dmgCount += value;
        }

        stats.hp -= value;
        user.RegisterDamage(value);
        if (stats.hp <= 0)
        {
            GetComponent<TakeDamage>().Die(user);
        }
        if (hpBar != null)
        {
            hpBar.maxValue = stats.mHp;
            hpBar.value = stats.hp;
        }
    }

    public virtual void Heal(PjBase user, float value, HitData.Element element)
    {
        if (stats.hp > 0)
        {
            stats.hp += value;
            if (stats.hp > stats.mHp)
            {
                value -= (stats.hp - stats.mHp);
                stats.hp = stats.mHp;
            }

            user.RegisterDamage(value);

            if (value + healCount > 1)
            {
                value += healCount;
                healCount = 0;
                DamageText dText = null;
                switch (element)
                {
                    case HitData.Element.ice:
                        dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                        dText.textColor = GameManager.Instance.iceColor;
                        break;
                    case HitData.Element.fire:
                        dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                        dText.textColor = GameManager.Instance.fireColor;
                        break;
                    case HitData.Element.water:
                        dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                        dText.textColor = GameManager.Instance.waterColor;
                        break;
                    case HitData.Element.desert:
                        dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                        dText.textColor = GameManager.Instance.desertColor;
                        break;
                    case HitData.Element.wind:
                        dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                        dText.textColor = GameManager.Instance.windColor;
                        break;
                    case HitData.Element.nature:
                        dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                        dText.textColor = GameManager.Instance.natureColor;
                        break;
                    case HitData.Element.lightning:
                        dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                        dText.textColor = GameManager.Instance.lightningColor;
                        break;
                }

                dText.damageText.text = "+" + value.ToString("F0");
            }
            else
            {
                healCount += value;
            }
        }

    }

    public virtual void Stunn(PjBase target, float value)
    {
        target.GetComponent<TakeDamage>().Stunn(value);
    }

    public virtual void OnGlobalStunn(PjBase target, float value)
    {

    }

    public virtual void OnGlobalDamageTaken()
    {

    }
    public virtual void OnDamageTaken()
    {

    }

    public virtual void Moving(float magnitude)
    {

    }

    public virtual void GlobalMoving(float magnitude, PjBase user)
    {

    }
    void TakeDamage.Stunn(float stunTime)
    {
        this.stunTime += stunTime;
    }
    void TakeDamage.Die(PjBase killer)
    {
        killer.OnKill(this);
        Destroy(gameObject);
    }

    public virtual float CalculateSinergy(float calculo)
    {
        float value = stats.sinergy;
        value *= calculo / 100;
        //valor.text = value.ToString();
        return value;

    }

    public virtual float CalculateStrength(float calculo)
    {
        float value = stats.strength;
        value *= calculo / 100;
        //valor.text = value.ToString();
        return value;

    }

    public virtual float CalculateControl(float calculo)
    {
        float value = stats.control;
        value *= calculo / 100;
        //valor.text = value.ToString();
        return value;
    }
    public float CDR(float value)
    {
        value -= ((value * ((stats.cdr / (100 + stats.cdr)))));
        return value;
    }
    public float CalculateAtSpd(float value)
    {
        value = 1 / value;
        return value;
    }

    public virtual IEnumerator Dash(Vector2 direction, float speed, float range)
    {
        yield return null;
        StartCoroutine(Dash(direction, speed, range, false));
    }
    public virtual IEnumerator Dash(Vector2 direction, float speed, float range, bool ignoreWalls)
    {
        if (GetComponent<NavMeshAgent>())
        {
            GetComponent<NavMeshAgent>().enabled = false;
        }


        GetComponent<Collider2D>().isTrigger = true;

        dashing = true;
        Vector2 destinyPoint = Physics2D.Raycast(transform.position, direction, range, GameManager.Instance.wallLayer).point;
        yield return null;
        if (destinyPoint == new Vector2(0, 0))
        {
            destinyPoint = new Vector2(transform.position.x, transform.position.y) + direction.normalized * range;
        }

        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(destinyPoint.x, destinyPoint.y, transform.position.z), out hit, 100, 1);
        destinyPoint = hit.position;

        Vector2 distance = destinyPoint - new Vector2(transform.position.x, transform.position.y);
        yield return null;
        while (distance.magnitude > 1 && dashing && stunTime <= 0)
        {
            if (distance.magnitude > 0.7)
            {
                rb.velocity = distance.normalized * speed;
            }
            else
            {
                rb.velocity = distance * speed;
            }
            distance = destinyPoint - new Vector2(transform.position.x, transform.position.y);
            yield return null;
        }
        dashing = false;
        rb.velocity = new Vector2(0, 0);

        GetComponent<Collider2D>().isTrigger = false;
        if (GetComponent<NavMeshAgent>())
        {
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }

    public void AnimationCursorLock(int value)
    {
        if (controller != null)
        {
            if (value == 1)
            {
                controller.LockPointer(true);
            }
            else
            {
                controller.LockPointer(false);
            }
        }
        else
        {
            if (value == 1)
            {
                lockPointer = true;
            }
            else
            {
                lockPointer = false;
            }
        }
    }

    public virtual void OnKill(PjBase target)
    {

    }

    public virtual void Interact(PjBase user, PjBase target, float amount, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {

    }
}