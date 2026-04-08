using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerDeathUiPlayModeTests
{
    private GameObject playerObject;
    private PlayerDeathHandler deathHandler;

    private GameObject gameOverUiObject;
    private GameOverUI gameOverUI;

    private GameObject deathPanel;

    private MonoBehaviour dummyScriptA;
    private MonoBehaviour dummyScriptB;

    [SetUp]
    public void SetUp()
    {
        Time.timeScale = 1f;

        playerObject = new GameObject("Player");
        deathHandler = playerObject.AddComponent<PlayerDeathHandler>();

        Rigidbody2D rb = playerObject.AddComponent<Rigidbody2D>();
        deathHandler.rb = rb;

        dummyScriptA = playerObject.AddComponent<DummyDisableScript>();
        dummyScriptB = playerObject.AddComponent<DummyDisableScript>();

        deathHandler.scriptsToDisable = new MonoBehaviour[]
        {
            dummyScriptA,
            dummyScriptB
        };

        gameOverUiObject = new GameObject("GameOverUI");
        gameOverUI = gameOverUiObject.AddComponent<GameOverUI>();

        deathPanel = new GameObject("DeathPanel");
        deathPanel.SetActive(false);

        deathHandler.gameOverUI = gameOverUI;

        TryInjectPanelIntoGameOverUI(gameOverUI, deathPanel);
    }

    [TearDown]
    public void TearDown()
    {
        Time.timeScale = 1f;

        if (playerObject != null)
            UnityEngine.Object.DestroyImmediate(playerObject);

        if (gameOverUiObject != null)
            UnityEngine.Object.DestroyImmediate(gameOverUiObject);

        if (deathPanel != null)
            UnityEngine.Object.DestroyImmediate(deathPanel);
    }

    [UnityTest]
    public IEnumerator Die_SetsDeadState_StopsPhysics_DisablesScripts_AndShowsUi()
    {
        Rigidbody2D rb = deathHandler.rb;
        rb.linearVelocity = new Vector2(5f, 2f);
        rb.angularVelocity = 3f;
        rb.simulated = true;

        deathHandler.Die();

        yield return null;

        Assert.IsTrue(deathHandler.IsDead, "A PlayerDeathHandler nem került halott állapotba.");
        Assert.AreEqual(Vector2.zero, rb.linearVelocity, "A rigidbody sebessége nem nullázódott.");
        Assert.AreEqual(0f, rb.angularVelocity, "A rigidbody forgási sebessége nem nullázódott.");
        Assert.IsFalse(rb.simulated, "A rigidbody simulated flag nem lett kikapcsolva.");

        Assert.IsFalse(dummyScriptA.enabled, "Az elsõ letiltandó script aktív maradt.");
        Assert.IsFalse(dummyScriptB.enabled, "A második letiltandó script aktív maradt.");

        Assert.AreEqual(0f, Time.timeScale, "A játék nem állt meg halálkor.");

        Assert.IsTrue(
            deathPanel.activeSelf,
            "A death panel nem aktiválódott. Ha ez továbbra is hibás, akkor a GameOverUI más mezõnevet vagy más UI-struktúrát használ."
        );
    }

    private static void TryInjectPanelIntoGameOverUI(GameOverUI ui, GameObject panel)
    {
        FieldInfo[] fields = typeof(GameOverUI).GetFields(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        bool assigned = false;

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(GameObject))
            {
                string lower = field.Name.ToLower();

                if (lower.Contains("death") || lower.Contains("gameover") || lower.Contains("panel") || lower.Contains("ui"))
                {
                    field.SetValue(ui, panel);
                    assigned = true;
                }
            }
        }

        Assert.IsTrue(
            assigned,
            "Nem találtam a GameOverUI-ben olyan GameObject mezõt, ami panel/gameover/death/ui jellegû. Ebben az esetben küldd el a GameOverUI.cs tartalmát."
        );
    }

    private class DummyDisableScript : MonoBehaviour
    {
    }
}