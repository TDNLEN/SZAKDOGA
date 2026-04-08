using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class GameTimeTests
{
    private GameObject gameTimeObject;
    private GameTime gameTime;

    [SetUp]
    public void SetUp()
    {
        NightEvents.OnNightStarted = null;
        NightEvents.OnDayStarted = null;
        GameTime.Instance = null;

        gameTimeObject = new GameObject("GameTime");
        gameTime = gameTimeObject.AddComponent<GameTime>();
    }

    [TearDown]
    public void TearDown()
    {
        NightEvents.OnNightStarted = null;
        NightEvents.OnDayStarted = null;
        GameTime.Instance = null;

        if (gameTimeObject != null)
        {
            Object.DestroyImmediate(gameTimeObject);
        }
    }

    [Test]
    public void Start_InitializesHourAndNightState_FromInspectorValues()
    {
        gameTime.startHour = 12;
        gameTime.startHalfIndex = 1;

        InvokePrivate(gameTime, "Awake");
        InvokePrivate(gameTime, "Start");

        Assert.AreEqual(12, gameTime.CurrentHour);
        Assert.IsFalse(gameTime.IsNight);
    }

    [Test]
    public void Start_At23_IsNight()
    {
        gameTime.startHour = 23;
        gameTime.startHalfIndex = 0;

        InvokePrivate(gameTime, "Awake");
        InvokePrivate(gameTime, "Start");

        Assert.AreEqual(23, gameTime.CurrentHour);
        Assert.IsTrue(gameTime.IsNight);
    }

    [Test]
    public void AdvanceHalfHour_From10_00_Becomes10_30()
    {
        gameTime.startHour = 10;
        gameTime.startHalfIndex = 0;

        InvokePrivate(gameTime, "Awake");
        InvokePrivate(gameTime, "Start");
        InvokePrivate(gameTime, "AdvanceHalfHour");

        int currentHalfIndex = GetPrivateInt(gameTime, "currentHalfIndex");

        Assert.AreEqual(10, gameTime.CurrentHour);
        Assert.AreEqual(1, currentHalfIndex);
        Assert.IsFalse(gameTime.IsNight);
    }

    [Test]
    public void AdvanceHalfHour_From20_30_Becomes21_00_AndNightStarts()
    {
        bool nightStarted = false;
        NightEvents.OnNightStarted += () => nightStarted = true;

        gameTime.startHour = 20;
        gameTime.startHalfIndex = 1;

        InvokePrivate(gameTime, "Awake");
        InvokePrivate(gameTime, "Start");
        InvokePrivate(gameTime, "AdvanceHalfHour");

        int currentHalfIndex = GetPrivateInt(gameTime, "currentHalfIndex");

        Assert.AreEqual(21, gameTime.CurrentHour);
        Assert.AreEqual(0, currentHalfIndex);
        Assert.IsTrue(gameTime.IsNight);
        Assert.IsTrue(nightStarted);
    }

    [Test]
    public void AdvanceHalfHour_From05_30_Becomes06_00_AndDayStarts()
    {
        bool dayStarted = false;
        NightEvents.OnDayStarted += () => dayStarted = true;

        gameTime.startHour = 5;
        gameTime.startHalfIndex = 1;

        InvokePrivate(gameTime, "Awake");
        InvokePrivate(gameTime, "Start");
        InvokePrivate(gameTime, "AdvanceHalfHour");

        int currentHalfIndex = GetPrivateInt(gameTime, "currentHalfIndex");

        Assert.AreEqual(6, gameTime.CurrentHour);
        Assert.AreEqual(0, currentHalfIndex);
        Assert.IsFalse(gameTime.IsNight);
        Assert.IsTrue(dayStarted);
    }

    [Test]
    public void AdvanceHalfHour_From23_30_WrapsTo00_00()
    {
        gameTime.startHour = 23;
        gameTime.startHalfIndex = 1;

        InvokePrivate(gameTime, "Awake");
        InvokePrivate(gameTime, "Start");
        InvokePrivate(gameTime, "AdvanceHalfHour");

        int currentHalfIndex = GetPrivateInt(gameTime, "currentHalfIndex");

        Assert.AreEqual(0, gameTime.CurrentHour);
        Assert.AreEqual(0, currentHalfIndex);
        Assert.IsTrue(gameTime.IsNight);
    }

    private static void InvokePrivate(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(method, $"Nem található a(z) {methodName} metódus.");
        method.Invoke(target, null);
    }

    private static int GetPrivateInt(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(field, $"Nem található a(z) {fieldName} mezõ.");
        return (int)field.GetValue(target);
    }
}