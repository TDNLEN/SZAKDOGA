using NUnit.Framework;
using UnityEngine;


public class PlayerWalletTests
{
    private GameObject walletObject;
    private PlayerWallet wallet;

    [SetUp]
    public void SetUp()
    {
        walletObject = new GameObject("PlayerWallet");
        wallet = walletObject.AddComponent<PlayerWallet>();
        wallet.coins = 0;
    }

    [TearDown]
    public void TearDown()
    {
        if (walletObject != null)
        {
            Object.DestroyImmediate(walletObject);
        }
    }

    [Test]
    public void AddCoins_PositiveAmount_IncreasesCoins()
    {
        wallet.AddCoins(15);

        Assert.AreEqual(15, wallet.coins);
    }

    [Test]
    public void AddCoins_Zero_DoesNotChangeCoins()
    {
        wallet.coins = 10;

        wallet.AddCoins(0);

        Assert.AreEqual(10, wallet.coins);
    }

    [Test]
    public void AddCoins_Negative_DoesNotChangeCoins()
    {
        wallet.coins = 10;

        wallet.AddCoins(-3);

        Assert.AreEqual(10, wallet.coins);
    }

    [Test]
    public void TrySpend_WhenEnoughCoins_ReturnsTrue_AndDecreasesCoins()
    {
        wallet.coins = 20;

        bool result = wallet.TrySpend(7);

        Assert.IsTrue(result);
        Assert.AreEqual(13, wallet.coins);
    }

    [Test]
    public void TrySpend_WhenNotEnoughCoins_ReturnsFalse_AndLeavesCoinsUnchanged()
    {
        wallet.coins = 5;

        bool result = wallet.TrySpend(10);

        Assert.IsFalse(result);
        Assert.AreEqual(5, wallet.coins);
    }

    [Test]
    public void TrySpend_ZeroAmount_ReturnsTrue_AndLeavesCoinsUnchanged()
    {
        wallet.coins = 12;

        bool result = wallet.TrySpend(0);

        Assert.IsTrue(result);
        Assert.AreEqual(12, wallet.coins);
    }
}