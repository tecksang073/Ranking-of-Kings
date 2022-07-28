using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(int.MinValue)]
public class UILevel : MonoBehaviour, ILevel
{
    public Text textLevel;
    public Text textCollectExp;
    public Text textNextExp;
    public Text textCollectPerNextExp;
    public Text textRequireExp;
    public Text textExpPercent;
    public Image imageExpGage;
    public Text textIncreasingExp;
    public Text textIncreasedExpPercent;
    public Image imageIncreasedExpGage;
    public bool showMaxLevelInTextLevel;

    public int level;
    public int maxLevel;
    public int currentExp;
    public int increasingExp;
    public int requiredExp;

    public int Level { get { return level; } }
    public int MaxLevel { get { return maxLevel; } }

    // Example situation: If current item's exp is 15 and it's required 20 to levelup 
    // and it's increasing 3 exp by consuming item then:
    // Current Exp = 18 = (15 + 3)
    // Increasing Exp = 3
    // Required Exp = 20
    public int CurrentExp { get { return currentExp; } }
    public int IncreasingExp { get { return increasingExp; } }
    public int RequiredExp { get { return requiredExp; } }

    // Options
    public bool useFormatForInfo;

    void Update()
    {
        var rate = (float)(CurrentExp - IncreasingExp) / (float)RequiredExp;
        var rateWithIncreasingExp = (float)CurrentExp / (float)RequiredExp;
        var isReachMaxLevel = false;
        if (Level == MaxLevel)
        {
            isReachMaxLevel = true;
            rate = 1;
            rateWithIncreasingExp = 1;
        }

        if (textLevel != null)
        {
            textLevel.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_LEVEL, Level) : Level.ToString("N0");
            if (showMaxLevelInTextLevel)
                textLevel.text += "/" + MaxLevel.ToString("N0");
        }

        if (textCollectExp != null)
        {
            textCollectExp.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_COLLECT_EXP, (CurrentExp - IncreasingExp)) : (CurrentExp - IncreasingExp).ToString("N0");
            if (isReachMaxLevel)
                textCollectExp.text = "0";
        }

        if (textNextExp != null)
        {
            textNextExp.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_NEXT_EXP, RequiredExp) : RequiredExp.ToString("N0");
            if (isReachMaxLevel)
                textNextExp.text = "0";
        }

        if (textCollectPerNextExp != null)
        {
            textCollectPerNextExp.text = (CurrentExp - IncreasingExp).ToString("N0") + "/" + RequiredExp.ToString("N0");
            if (isReachMaxLevel)
                textCollectPerNextExp.text = LanguageManager.GetText(GameText.TITLE_EXP_MAX);
        }

        if (textRequireExp != null)
        {
            textRequireExp.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_REQUIRE_EXP, this.RequireExp()) : this.RequireExp().ToString("N0");
            if (isReachMaxLevel)
                textRequireExp.text = "0";
        }

        if (textExpPercent != null)
            textExpPercent.text = (rate * 100).ToString("N2") + "%";

        if (imageExpGage != null)
            imageExpGage.fillAmount = rate;

        if (textIncreasingExp != null)
            textIncreasingExp.text = "+" + IncreasingExp.ToString("N0");

        if (textIncreasedExpPercent != null)
            textIncreasedExpPercent.text = (rateWithIncreasingExp * 100).ToString("N2") + "%";

        if (imageIncreasedExpGage != null)
            imageIncreasedExpGage.fillAmount = rateWithIncreasingExp;
    }
}
