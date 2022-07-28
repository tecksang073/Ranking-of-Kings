using UnityEngine.Events;

[System.Serializable]
public class PlayerEvent : UnityEvent<Player> { }

[System.Serializable]
public class PlayerAuthEvent : UnityEvent<PlayerAuth> { }

[System.Serializable]
public class PlayerCurrencyEvent : UnityEvent<PlayerCurrency> { }

[System.Serializable]
public class PlayerFormationEvent : UnityEvent<PlayerFormation> { }

[System.Serializable]
public class PlayerItemEvent : UnityEvent<PlayerItem> { }

[System.Serializable]
public class PlayerStaminaEvent : UnityEvent<PlayerStamina> { }
