using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Animator))]
public abstract class BaseCharacterEntity : MonoBehaviour
{
    public const string ANIM_ACTION_STATE = "_Action";
    public BaseGamePlayManager Manager { get { return BaseGamePlayManager.Singleton; } }
    [Header("Animator")]
    [SerializeField]
    private RuntimeAnimatorController animatorController;

    [Header("UIs/Effects/Entities Containers")]
    [Tooltip("The transform where we're going to spawn uis")]
    public Transform uiContainer;
    [Tooltip("The transform where we're going to spawn body effects")]
    public Transform bodyEffectContainer;
    [Tooltip("The transform where we're going to spawn floor effects")]
    public Transform floorEffectContainer;
    [Tooltip("The transform where we're going to spawn damage (bullet, slash force and so on)")]
    public Transform damageContainer;
    public EquipItemModelContainer[] equipModelContainers;

    public AnimatorOverrideController CacheAnimatorController { get; private set; }

    public Animator CacheAnimator { get; private set; }

    private PlayerItem item;
    public PlayerItem Item
    {
        get { return item; }
        set
        {
            if (value == null || item == value || value.CharacterData == null)
                return;
            item = value;
            Skills.Clear();
            if (item.CharacterData.skills != null && item.CharacterData.skills.Count > 0)
            {
                foreach (var skill in item.CharacterData.skills)
                {
                    if (skill == null) continue;
                    // TODO: Implement skill level
                    Skills.Add(NewSkill(1, skill));
                }
            }
            foreach (var equippedItem in item.EquippedItems.Values)
            {
                if (equippedItem == null) continue;
                var equipmentItem = equippedItem.ItemData as EquipmentItem;
                if (equipmentItem == null || equipmentItem.skills == null || equipmentItem.skills.Count == 0) continue;
                foreach (var skill in equipmentItem.skills)
                {
                    if (skill == null) continue;
                    // TODO: Implement skill level
                    Skills.Add(NewSkill(1, skill));
                }
            }
            UpdateEquipModels();
            Revive();
        }
    }
    public List<BaseAttackAnimationData> AttackAnimations { get { return Item.CharacterData.attackAnimations; } }
    public readonly Dictionary<string, BaseCharacterBuff> Buffs = new Dictionary<string, BaseCharacterBuff>();
    public readonly List<BaseCharacterSkill> Skills = new List<BaseCharacterSkill>();
    public BaseGamePlayFormation Formation { get; protected set; }
    public int Position { get; protected set; }
    public bool IsStun
    {
        get
        {
            foreach (var buff in Buffs.Values)
            {
                if (buff.Buff.isStun)
                    return true;
            }
            return false;
        }
    }

    public int MaxHp
    {
        get { return (int)GetTotalAttributes().hp; }
    }

    private float hp;
    public float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <= 0)
                Dead();
            if (hp >= MaxHp)
                hp = MaxHp;
        }
    }

    public virtual bool IsActiveCharacter { get { return false; } }
    public virtual bool IsPlayerCharacter { get { return false; } }
    public bool IsBoss { get; set; }
    public UICharacterStatsGeneric UiCharacterStats { get; set; }

    private Transform container;
    public Transform Container
    {
        get { return container; }
        set
        {
            container = value;
            CacheTransform.SetParent(container);
            CacheTransform.localPosition = Vector3.zero;
            CacheTransform.localEulerAngles = Vector3.zero;
            gameObject.SetActive(true);
        }
    }

    public Transform CacheTransform { get; private set; }

    protected virtual void Awake()
    {
        CacheTransform = transform;
        if (uiContainer == null)
            uiContainer = CacheTransform;
        if (bodyEffectContainer == null)
            bodyEffectContainer = CacheTransform;
        if (floorEffectContainer == null)
            floorEffectContainer = CacheTransform;
        if (damageContainer == null)
            damageContainer = CacheTransform;
        if (animatorController is AnimatorOverrideController)
            CacheAnimatorController = animatorController as AnimatorOverrideController;
        else
            CacheAnimatorController = new AnimatorOverrideController(animatorController);
        CacheAnimator = GetComponent<Animator>();
        CacheAnimator.runtimeAnimatorController = CacheAnimatorController;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        var hasChanges = false;
        var cacheAnimator = GetComponent<Animator>();
        if (animatorController == null && cacheAnimator != null)
        {
            animatorController = cacheAnimator.runtimeAnimatorController;
            hasChanges = true;
        }
        if (hasChanges)
            EditorUtility.SetDirty(this);
    }
#endif

    public void Revive()
    {
        if (Item == null)
            return;

        Hp = MaxHp;
    }

    public virtual void Dead()
    {
        var keys = new List<string>(Buffs.Keys);
        for (var i = keys.Count - 1; i >= 0; --i)
        {
            var key = keys[i];
            if (!Buffs.ContainsKey(key))
                continue;

            var buff = Buffs[key];
            buff.BuffRemove();
            Buffs.Remove(key);
        }
    }

    public CalculatedAttributes GetTotalAttributes()
    {
        var result = Item.Attributes;
        var equipmentBonus = Item.EquipmentBonus;
        result += equipmentBonus;

        // Add attributes by passive skills
        var skills = new List<BaseCharacterSkill>(Skills);
        if (skills != null)
        {
            foreach (var skill in skills)
            {
                if (!skill.Skill || !skill.Skill.isPassive) continue;
                var skillBuffs = skill.Skill.GetBuffs();
                foreach (var buff in skillBuffs)
                {
                    result += buff.GetAttributes(skill.Level);
                }
            }
        }

        // Add attributes by buffs
        var buffs = new List<BaseCharacterBuff>(Buffs.Values);
        if (buffs != null)
        {
            foreach (var buff in buffs)
            {
                result += buff.Attributes;
            }
        }

        // If this is character item, applies rate attributes
        result.hp += Mathf.CeilToInt(result.hpRate * result.hp);
        result.pAtk += Mathf.CeilToInt(result.pAtkRate * result.pAtk);
        result.pDef += Mathf.CeilToInt(result.pDefRate * result.pDef);
#if !NO_MAGIC_STATS
        result.mAtk += Mathf.CeilToInt(result.mAtkRate * result.mAtk);
        result.mDef += Mathf.CeilToInt(result.mDefRate * result.mDef);
#endif
        result.spd += Mathf.CeilToInt(result.spdRate * result.spd);
#if !NO_EVADE_STATS
        result.eva += Mathf.CeilToInt(result.evaRate * result.eva);
        result.acc += Mathf.CeilToInt(result.accRate * result.acc);
#endif
        result.hpRate = 0;
        result.pAtkRate = 0;
        result.pDefRate = 0;
#if !NO_MAGIC_STATS
        result.mAtkRate = 0;
        result.mDefRate = 0;
#endif
        result.spdRate = 0;
#if !NO_EVADE_STATS
        result.evaRate = 0;
        result.accRate = 0;
#endif

        return result;
    }

    public virtual void SetFormation(BaseGamePlayFormation formation, int position, Transform container)
    {
        if (container == null)
            return;

        Formation = formation;
        Position = position;
        Container = container;
    }

    public virtual void Attack(BaseCharacterEntity target, int seed, float pAtkRate = 1f, float mAtkRate = 1f, int hitCount = 1, int fixDamage = 0)
    {
        if (target == null)
            return;
        target.ReceiveDamage(
            this,
            seed,
            pAtkRate,
            mAtkRate,
            hitCount,
            fixDamage);
    }

    public virtual void Attack(BaseCharacterEntity target, BaseDamage damagePrefab, int seed, float pAtkRate = 1f, float mAtkRate = 1f, int hitCount = 1, int fixDamage = 0)
    {
        if (damagePrefab == null)
        {
            // Apply damage immediately
            Attack(target, seed, pAtkRate, mAtkRate, hitCount, fixDamage);
        }
        else
        {
            var damage = Instantiate(damagePrefab, damageContainer.position, damageContainer.rotation);
            damage.Setup(this, target, seed, pAtkRate, mAtkRate, hitCount, fixDamage);
        }
    }

    public virtual bool ReceiveDamage(
        BaseCharacterEntity attacker,
        int seed,
        float pAtkRate = 1f,
        float mAtkRate = 1f,
        int hitCount = 1,
        int fixDamage = 0)
    {
        if (hitCount < 0)
            hitCount = 1;

        var stealHp = 0f;
        var attackerElemental = attacker.Item.CharacterData.elemental;
        var attackerAttributes = attacker.GetTotalAttributes();
        var defenderElemental = Item.CharacterData.elemental;
        var defenderAttributes = GetTotalAttributes();
        var totalDmg = GameInstance.GameplayRule.GetDamage(
            seed,
            attackerElemental,
            defenderElemental,
            attackerAttributes,
            defenderAttributes,
            out stealHp,
            pAtkRate,
            mAtkRate,
            hitCount,
            fixDamage);

        var isCritical = false;
        var isBlock = false;
        // Critical occurs
        if (GameInstance.GameplayRule.IsCrit(seed, attackerAttributes, defenderAttributes))
        {
            totalDmg = GameInstance.GameplayRule.GetCritDamage(seed, attackerAttributes, defenderAttributes, totalDmg);
            isCritical = true;
        }
        // Block occurs
        if (GameInstance.GameplayRule.IsBlock(seed, attackerAttributes, defenderAttributes))
        {
            totalDmg = GameInstance.GameplayRule.GetBlockDamage(attackerAttributes, defenderAttributes, totalDmg);
            isBlock = true;
        }

        // Cannot evade, receive damage
        if (!GameInstance.GameplayRule.IsHit(seed, attackerAttributes, defenderAttributes))
        {
            Manager.SpawnMissText(this, hitCount);
        }
        else
        {
            if (isBlock)
                Manager.SpawnBlockText((int)totalDmg, this, hitCount);
            else if (isCritical)
                Manager.SpawnCriticalText((int)totalDmg, this, hitCount);
            else
                Manager.SpawnDamageText((int)totalDmg, this, hitCount);

            Hp -= (int)totalDmg;
            if (!IsPlayerCharacter)
                BaseGamePlayManager.IncreaseTotalDamage((int)totalDmg);

            if (stealHp > 0f)
            {
                Manager.SpawnHealText((int)stealHp, this, hitCount);
                attacker.Hp += stealHp;
            }
        }

        return true;
    }

    public virtual void ApplyBuff(BaseCharacterEntity caster, int level, BaseSkill skill, int buffIndex, int seed)
    {
        if (skill == null || buffIndex < 0 || buffIndex >= skill.GetBuffs().Count || skill.GetBuffs()[buffIndex] == null || Hp <= 0)
            return;

        var buffData = skill.GetBuffs()[buffIndex];
        if (buffData.type == BuffType.Nerf)
        {
            // Resistance
            var attributes = GetTotalAttributes();
            if (RandomNumberUtils.RandomFloat(seed, 0, 1) <= attributes.resistanceChance)
            {
                // Reisted, nerf will not applied
                Manager.SpawnResistText(this, 1);
                return;
            }
        }

        if (buffData.clearBuffs > 0 || buffData.clearNerfs > 0)
        {
            int countClearBuffs = 0;
            int countClearNerfs = 0;
            var buffs = new List<BaseCharacterBuff>(Buffs.Values);
            foreach (var clearingBuff in buffs)
            {
                switch (clearingBuff.Buff.type)
                {
                    case BuffType.Buff:
                        if (countClearBuffs < buffData.clearBuffs)
                        {
                            clearingBuff.BuffRemove();
                            Buffs.Remove(clearingBuff.Id);
                        }
                        countClearBuffs++;
                        break;
                    case BuffType.Nerf:
                        if (countClearNerfs < buffData.clearNerfs)
                        {
                            clearingBuff.BuffRemove();
                            Buffs.Remove(clearingBuff.Id);
                        }
                        countClearNerfs++;
                        break;
                }
                if (countClearBuffs >= buffData.clearBuffs &&
                    countClearNerfs >= buffData.clearNerfs)
                    break;
            }
        }

        var buff = NewBuff(level, skill, buffIndex, seed, caster, this);
        if (buff.GetRemainsDuration() > 0f)
        {
            // Buff cannot stack so remove old buff
            if (Buffs.ContainsKey(buff.Id))
            {
                buff.BuffRemove();
                Buffs.Remove(buff.Id);
            }
            Buffs[buff.Id] = buff;
        }
        else
            buff.BuffRemove();
    }

    public void ChangeActionClip(AnimationClip clip)
    {
        CacheAnimatorController[ANIM_ACTION_STATE] = clip;
    }

    protected virtual void UpdateEquipModels()
    {
        // Destroy children from containers
        Dictionary<string, Transform> containers = new Dictionary<string, Transform>();
        foreach (var container in equipModelContainers)
        {
            if (container.transform == null) continue;
            for (int i = container.transform.childCount - 1; i >= 0; --i)
            {
                Destroy(container.transform.GetChild(i).gameObject);
            }
            containers[container.slotId] = container.transform;
        }
        foreach (var equippedItem in Item.EquippedItems.Values)
        {
            if (equippedItem.EquipmentData == null) continue;
            foreach (var modelPrefab in equippedItem.EquipmentData.equipModelPrefabs)
            {
                if (!containers.ContainsKey(modelPrefab.slotId)) continue;
                var model = Instantiate(modelPrefab.modelPrefab, containers[modelPrefab.slotId]);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }
        }
    }

    public abstract BaseCharacterSkill NewSkill(int level, BaseSkill skill);
    public abstract BaseCharacterBuff NewBuff(int level, BaseSkill skill, int buffIndex, int seed, BaseCharacterEntity giver, BaseCharacterEntity receiver);
}

[System.Serializable]
public struct EquipItemModelContainer
{
    public string slotId;
    public Transform transform;
}