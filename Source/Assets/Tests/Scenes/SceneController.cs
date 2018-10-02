using System.Collections.Generic;
using CI.TestRunner;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public GameObject ScrollView;
    public GameObject TestResultPrefab;
    public Text TestResultText;

    public void Run()
    {
        ClearTests();

        TestRunner testRunner = new TestRunner();

        var fixtureResults = testRunner.Run();

        DisplayTests(fixtureResults);
    }

    private void ClearTests()
    {
        for (int i = 0; i < ScrollView.transform.childCount; i++)
        {
            Destroy(ScrollView.transform.GetChild(i).gameObject);
        }
    }

    private void DisplayTests(IEnumerable<TestFixtureResult> fixtureResults)
    {
        foreach (var fixture in fixtureResults)
        {
            foreach (var result in fixture.TestResults)
            {
                var item = Instantiate(TestResultPrefab);

                var button = item.GetComponent<Button>();

                button.onClick.AddListener(() =>
                {
                    TestResultText.text = result.IsPassing ? "Test passed" :
                                            "Test failed: " + result.Exception.Message;
                });

                var items = item.GetComponentsInChildren<Text>();

                items[0].text = string.Format("[{0}] {1}", fixture.Name, result.Name);
                items[1].text = result.IsPassing ? "Passing" : "Failed";
                items[1].color = result.IsPassing ? new Color(0, 0.5f, 0) : new Color(0.5f, 0, 0);

                item.transform.SetParent(ScrollView.transform, false);
            }
        }
    }
}