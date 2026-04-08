using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameTimeNightIntegrationPlayModeTests
{
    private GameObject gameTimeObject;
    private GameTime gameTime;

    private bool receivedNightEvent;
    private bool receivedDayEvent;

    [SetUp]
    public void SetUp()
    {
        receivedNightEvent = false;
        receivedDayEvent = false;

        NightEvents.OnNightStarted = null;
        NightEvents.OnDayStarted = null;
        GameTime.Instance = null;

        NightEvents.OnNightStarted += HandleNightStarted;
        NightEvents.OnDayStarted += HandleDayStarted;
    }

    [TearDown]
    public void TearDown()
    {
        NightEvents.OnNightStarted -= HandleNightStarted;
        NightEvents.OnDayStarted -= HandleDayStarted;

        NightEvents.OnNightStarted = null;
        NightEvents.OnDayStarted = null;
        GameTime.Instance = null;

        if (gameTimeObject != null)
            UnityEngine.Object.DestroyImmediate(gameTimeObject);
    }

    private void HandleNightStarted() => receivedNightEvent = true;
    private void HandleDayStarted() => receivedDayEvent = true;

    [UnityTest]
    public IEnumerator GameTime_TriggersNightEvent_WhenCrossingIntoNight()
    {
        gameTimeObject = new GameObject("GameTime");
        gameTimeObject.SetActive(false);

        gameTime = gameTimeObject.AddComponent<GameTime>();
        gameTime.startHour = 20;
        gameTime.startHalfIndex = 1;

        gameTimeObject.SetActive(true);
        yield return null;

        InvokePrivate(gameTime, "AdvanceHalfHour");
        yield return null;

        Assert.AreEqual(21, gameTime.CurrentHour);
        Assert.IsTrue(gameTime.IsNight);
        Assert.IsTrue(receivedNightEvent);
        Assert.IsFalse(receivedDayEvent);
    }

    [UnityTest]
    public IEnumerator GameTime_TriggersDayEvent_WhenCrossingIntoDay()
    {
        gameTimeObject = new GameObject("GameTime");
        gameTimeObject.SetActive(false);

        gameTime = gameTimeObject.AddComponent<GameTime>();
        gameTime.startHour = 5;
        gameTime.startHalfIndex = 1;

        gameTimeObject.SetActive(true);
        yield return null;

        InvokePrivate(gameTime, "AdvanceHalfHour");
        yield return null;

        Assert.AreEqual(6, gameTime.CurrentHour);
        Assert.IsFalse(gameTime.IsNight);
        Assert.IsTrue(receivedDayEvent);
        Assert.IsFalse(receivedNightEvent);
    }

    private static void InvokePrivate(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(method, $"Nem található a(z) {methodName} metódus.");
        method.Invoke(target, null);
    }
}