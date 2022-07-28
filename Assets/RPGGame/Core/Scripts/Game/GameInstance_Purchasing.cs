using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_PURCHASING && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

public partial class GameInstance
{
    // NOTE: something about product type
    // -- Consumable product is product such as gold, gem that can be consumed
    // -- Non-Consumable product is product such as special characters/items
    // that player will buy it to unlock ability to use and will not buy it later
    // -- Subscription product is product such as weekly/monthly promotion
    public const string TAG_INIT = "IAP_INIT";
    public const string TAG_PURCHASE = "IAP_PURCHASE";

#if ENABLE_PURCHASING && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
    public static IStoreController StoreController { get; private set; }
    public static IExtensionProvider StoreExtensionProvider { get; private set; }
#endif

    public static System.Action<bool, string> PurchaseCallback;
    public static System.Action<bool, string> RestoreCallback;

    private void SetupPurchasing()
    {
#if ENABLE_PURCHASING && UNITY_PURCHASING && (UNITY_ANDROID || UNITY_IOS)
        // If we have already connected to Purchasing ...
        if (IsPurchasingInitialized())
            return;

        // Create a builder, first passing in a suite of Unity provided stores.
        var module = StandardPurchasingModule.Instance();
        var builder = ConfigurationBuilder.Instance(module);
#endif

        if (GameDatabase != null)
        {
            foreach (var iapPackage in GameDatabase.IapPackages.Values)
            {

#if ENABLE_PURCHASING && UNITY_PURCHASING && (UNITY_ANDROID || UNITY_IOS)
                // Setup IAP package for clients
                var productCatalogItem = iapPackage.ProductCatalogItem;
                if (productCatalogItem == null)
                    continue;

                Debug.Log("[" + TAG_INIT + "]: Adding product " + productCatalogItem.id + " type " + productCatalogItem.type.ToString());
                if (productCatalogItem.allStoreIDs.Count > 0)
                {
                    var ids = new IDs();
                    foreach (var storeID in productCatalogItem.allStoreIDs)
                    {
                        ids.Add(storeID.id, storeID.store);
                    }
                    builder.AddProduct(productCatalogItem.id, productCatalogItem.type, ids);
                }
                else
                {
                    builder.AddProduct(productCatalogItem.id, productCatalogItem.type);
                }
#endif
            }
        }

#if ENABLE_PURCHASING && UNITY_PURCHASING && (UNITY_ANDROID || UNITY_IOS)
        Debug.Log("[" + TAG_INIT + "]: Initializing Purchasing...");
        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        try
        {
            UnityPurchasing.Initialize(this, builder);
        }
        catch (System.InvalidOperationException ex)
        {
            var errorMessage = "[" + TAG_INIT + "]: Cannot initialize purchasing, the platform may not supports.";
            Debug.LogError(errorMessage);
            Debug.LogException(ex);
        }
#else
        Debug.Log("[" + TAG_INIT + "]: Initialized without purchasing");
#endif
    }

    public static bool IsPurchasingInitialized()
    {
#if ENABLE_PURCHASING && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
        // Only say we are initialized if both the Purchasing references are set.
        return StoreController != null && StoreExtensionProvider != null;
#else
        return false;
#endif
    }

    #region IStoreListener
#if ENABLE_PURCHASING && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        // Overall Purchasing system, configured with products for this application.
        StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        StoreExtensionProvider = extensions;
        var productCount = StoreController.products.all.Length;
        var logMessage = "[" + TAG_INIT + "]: OnInitialized with " + productCount + " products";
        Debug.Log(logMessage);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        var errorMessage = "[" + TAG_INIT + "]: Fail. InitializationFailureReason: " + error;
        Debug.LogError(errorMessage);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("[" + TAG_PURCHASE + "] ProcessPurchase " + args.purchasedProduct.definition.id);
        var id = args.purchasedProduct.definition.id;

        if (!args.purchasedProduct.hasReceipt)
            return PurchaseProcessingResult.Complete;

        IapPackage package;
        if (GameDatabase.IapPackages.TryGetValue(id, out package))
        {

            // add receipt data, information https://docs.unity3d.com/Manual/UnityIAPPurchaseReceipts.html
            var receiptObject = MiniJson.JsonDecode(args.purchasedProduct.receipt);
            var payload = (receiptObject as Dictionary<string, object>)["Payload"];

            if (Application.platform == RuntimePlatform.Android)
            {
                var payloadObject = MiniJson.JsonDecode(payload.ToString()) as Dictionary<string, object>;
                GameService.OpenIapPackage_Android(id, payloadObject["json"].ToString(), payloadObject["signature"].ToString(), (result) => OnOpenIapPackageSuccess(args.purchasedProduct, result), OnOpenIapPackageFail);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                GameService.OpenIapPackage_iOS(id, payload.ToString(), (result) => OnOpenIapPackageSuccess(args.purchasedProduct, result), OnOpenIapPackageFail);
        }
        else
            PurchaseResult(false, "Package not found");

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed.
        return PurchaseProcessingResult.Pending;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        var errorMessage = "[" + TAG_PURCHASE + "]: FAIL. Product: " + product.definition.storeSpecificId + ", PurchaseFailureReason: " + failureReason;
        PurchaseResult(false, errorMessage);
    }
#endif
    #endregion

    #region IAP Actions
    public void Purchase(string productId)
    {
#if ENABLE_PURCHASING && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
        // If Purchasing has not yet been set up ...
        if (!IsPurchasingInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            var errorMessage = "[" + TAG_PURCHASE + "]: FAIL. Not initialized.";
            PurchaseResult(false, errorMessage);
            return;
        }

        var product = StoreController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log(string.Format("[" + TAG_PURCHASE + "] Purchasing product asychronously: '{0}'", product.definition.id));
            StoreController.InitiatePurchase(product);
        }
        else
        {
            var errorMessage = "[" + TAG_PURCHASE + "]: FAIL. Not purchasing product, either is not found or is not available for purchase.";
            PurchaseResult(false, errorMessage);
        }
#else
        Debug.LogWarning("Cannot purchase product, Unity Purchasing is not enabled.");
#endif
    }
    #endregion

    #region Callback Events
#if ENABLE_PURCHASING && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
    private static void PurchaseResult(bool success, string errorMessage = "")
    {
        if (!success)
            Debug.LogError(errorMessage);
        if (PurchaseCallback != null)
        {
            PurchaseCallback(success, errorMessage);
            PurchaseCallback = null;
        }
    }

    private void OnOpenIapPackageSuccess(Product product, ItemResult result)
    {
        OnGameServiceItemResult(result);
        var updateCurrencies = result.updateCurrencies;
        foreach (var updateCurrency in updateCurrencies)
        {
            PlayerCurrency.SetData(updateCurrency);
        }
        var items = new List<PlayerItem>();
        items.AddRange(result.createItems);
        items.AddRange(result.updateItems);
        if (items.Count > 0)
            ShowRewardItemsDialog(items);

        StoreController.ConfirmPendingPurchase(product);
    }

    private void OnOpenIapPackageFail(string error)
    {
        OnGameServiceError(error);
    }
#endif
    #endregion
}
