using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAttributes : UIDataItem<CalculatedAttributes>
{
    [Header("Fix attributes")]
    public GameObject containerHp;
    public Text textHp;
    public GameObject containerPAtk;
    public Text textPAtk;
    public GameObject containerPDef;
    public Text textPDef;
    public GameObject containerMAtk;
    public Text textMAtk;
    public GameObject containerMDef;
    public Text textMDef;
    public GameObject containerSpd;
    public Text textSpd;
    public GameObject containerEva;
    public Text textEva;
    public GameObject containerAcc;
    public Text textAcc;
    [Header("Rate attributes")]
    public GameObject containerHpRate;
    public Text textHpRate;
    public GameObject containerPAtkRate;
    public Text textPAtkRate;
    public GameObject containerPDefRate;
    public Text textPDefRate;
    public GameObject containerMAtkRate;
    public Text textMAtkRate;
    public GameObject containerMDefRate;
    public Text textMDefRate;
    public GameObject containerSpdRate;
    public Text textSpdRate;
    public GameObject containerEvaRate;
    public Text textEvaRate;
    public GameObject containerAccRate;
    public Text textAccRate;
    [Header("Critical attributes")]
    public GameObject containerCritChance;
    public Text textCritChance;
    public GameObject containerCritDamageRate;
    public Text textCritDamageRate;
    [Header("Block attributes")]
    public GameObject containerBlockChance;
    public Text textBlockChance;
    public GameObject containerBlockDamageRate;
    public Text textBlockDamageRate;
    [Header("Resistance attributes")]
    public GameObject containerResistance;
    public Text textResistanceChance;
    [Header("Options")]
    public bool useFormatForInfo;
    public bool hideInfoIfEmpty;
    public override void UpdateData()
    {
        SetupInfo(data);
    }

    public override void Clear()
    {
        SetupInfo(default(CalculatedAttributes));
    }

    private void SetupInfo(CalculatedAttributes data)
    {
        if (textHp != null)
        {
            textHp.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_HP, data.hp) : LanguageManager.FormatNumber(data.hp);
            if (hideInfoIfEmpty && containerHp != null)
                containerHp.SetActive(Mathf.Abs(data.hp) > 0);
        }

        if (textPAtk != null)
        {
            textPAtk.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_PATK, data.pAtk) : LanguageManager.FormatNumber(data.pAtk);
            if (hideInfoIfEmpty && containerPAtk != null)
                containerPAtk.SetActive(Mathf.Abs(data.pAtk) > 0);
        }

        if (textPDef != null)
        {
            textPDef.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_PDEF, data.pDef) : LanguageManager.FormatNumber(data.pDef);
            if (hideInfoIfEmpty && containerPDef != null)
                containerPDef.SetActive(Mathf.Abs(data.pDef) > 0);
        }

#if !NO_MAGIC_STATS
        if (textMAtk != null)
        {
            textMAtk.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_MATK, data.mAtk) : LanguageManager.FormatNumber(data.mAtk);
            if (hideInfoIfEmpty && containerMAtk != null)
                containerMAtk.SetActive(Mathf.Abs(data.mAtk) > 0);
        }

        if (textMDef != null)
        {
            textMDef.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_MDEF, data.mDef) : LanguageManager.FormatNumber(data.mDef);
            if (hideInfoIfEmpty && containerMDef != null)
                containerMDef.SetActive(Mathf.Abs(data.mDef) > 0);
        }
#endif

        if (textSpd != null)
        {
            textSpd.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_SPD, data.spd) : LanguageManager.FormatNumber(data.spd);
            if (hideInfoIfEmpty && containerSpd != null)
                containerSpd.SetActive(Mathf.Abs(data.spd) > 0);
        }

#if !NO_EVADE_STATS
        if (textEva != null)
        {
            textEva.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_EVA, data.eva) : LanguageManager.FormatNumber(data.eva);
            if (hideInfoIfEmpty && containerEva != null)
                containerEva.SetActive(Mathf.Abs(data.eva) > 0);
        }

        if (textAcc != null)
        {
            textAcc.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_ACC, data.acc) : LanguageManager.FormatNumber(data.acc);
            if (hideInfoIfEmpty && containerAcc != null)
                containerAcc.SetActive(Mathf.Abs(data.acc) > 0);
        }
#endif

        if (textHpRate != null)
        {
            textHpRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_HP_RATE, data.hpRate, true) : LanguageManager.FormatNumber(data.hpRate, true);
            if (hideInfoIfEmpty && containerHpRate != null)
                containerHpRate.SetActive(Mathf.Abs(data.hpRate) > 0);
        }

        if (textPAtkRate != null)
        {
            textPAtkRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_PATK_RATE, data.pAtkRate, true) : LanguageManager.FormatNumber(data.pAtkRate, true);
            if (hideInfoIfEmpty && containerPAtkRate != null)
                containerPAtkRate.SetActive(Mathf.Abs(data.pAtkRate) > 0);
        }

        if (textPDefRate != null)
        {
            textPDefRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_PDEF_RATE, data.pDefRate, true) : LanguageManager.FormatNumber(data.pDefRate, true);
            if (hideInfoIfEmpty && containerPDefRate != null)
                containerPDefRate.SetActive(Mathf.Abs(data.pDefRate) > 0);
        }

#if !NO_MAGIC_STATS
        if (textMAtkRate != null)
        {
            textMAtkRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_MATK_RATE, data.mAtkRate, true) : LanguageManager.FormatNumber(data.mAtkRate, true);
            if (hideInfoIfEmpty && containerMAtkRate != null)
                containerMAtkRate.SetActive(Mathf.Abs(data.mAtkRate) > 0);
        }

        if (textMDefRate != null)
        {
            textMDefRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_MDEF_RATE, data.mDefRate, true) : LanguageManager.FormatNumber(data.mDefRate, true);
            if (hideInfoIfEmpty && containerMDefRate != null)
                containerMDefRate.SetActive(Mathf.Abs(data.mDefRate) > 0);
        }
#endif

        if (textSpdRate != null)
        {
            textSpdRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_SPD_RATE, data.spdRate, true) : LanguageManager.FormatNumber(data.spdRate, true);
            if (hideInfoIfEmpty && containerSpdRate != null)
                containerSpdRate.SetActive(Mathf.Abs(data.spdRate) > 0);
        }

#if !NO_EVADE_STATS
        if (textEvaRate != null)
        {
            textEvaRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_EVA_RATE, data.evaRate, true) : LanguageManager.FormatNumber(data.evaRate, true);
            if (hideInfoIfEmpty && containerEvaRate != null)
                containerEvaRate.SetActive(Mathf.Abs(data.evaRate) > 0);
        }

        if (textAccRate != null)
        {
            textAccRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_ACC_RATE, data.accRate, true) : LanguageManager.FormatNumber(data.accRate, true);
            if (hideInfoIfEmpty && containerAccRate != null)
                containerAccRate.SetActive(Mathf.Abs(data.accRate) > 0);
        }
#endif

        if (textCritChance != null)
        {
            textCritChance.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_CRIT_CHANCE, data.critChance, true) : LanguageManager.FormatNumber(data.critChance, true);
            if (hideInfoIfEmpty && containerCritChance != null)
                containerCritChance.SetActive(Mathf.Abs(data.critChance) > 0);
        }

        if (textCritDamageRate != null)
        {
            textCritDamageRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_CRIT_DAMAGE_RATE, data.critDamageRate, true) : LanguageManager.FormatNumber(data.critDamageRate, true);
            if (hideInfoIfEmpty && containerCritDamageRate != null)
                containerCritDamageRate.SetActive(Mathf.Abs(data.critDamageRate) > 0);
        }

        if (textBlockChance != null)
        {
            textBlockChance.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_BLOCK_CHANCE, data.blockChance, true) : LanguageManager.FormatNumber(data.blockChance, true);
            if (hideInfoIfEmpty && containerBlockChance != null)
                containerBlockChance.SetActive(Mathf.Abs(data.blockChance) > 0);
        }

        if (textBlockDamageRate != null)
        {
            textBlockDamageRate.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_BLOCK_DAMAGE_RATE, data.blockDamageRate, true) : LanguageManager.FormatNumber(data.blockDamageRate, true);
            if (hideInfoIfEmpty && containerBlockDamageRate != null)
                containerBlockDamageRate.SetActive(Mathf.Abs(data.blockDamageRate) > 0);
        }

        if (textResistanceChance != null)
        {
            textResistanceChance.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_RESISTANCE_CHANCE, data.resistanceChance, true) : LanguageManager.FormatNumber(data.resistanceChance, true);
            if (hideInfoIfEmpty && containerResistance != null)
                containerResistance.SetActive(Mathf.Abs(data.resistanceChance) > 0);
        }
    }

    public override bool IsEmpty()
    {
        return false;
    }
}
