using CI.TestRunner;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public GameObject ScrollView;
    public GameObject TestResultPrefab;
    public Text TestResultText;

    private readonly TestRunner _testRunner = new TestRunner();

    public void Start()
    {
        _testRunner.TestFinished += (e) => DisplayResult(e);
    }

    public void Run()
    {
        ClearTests();

        _testRunner.Run();
    }

    private void ClearTests()
    {
        for (int i = 0; i < ScrollView.transform.childCount; i++)
        {
            Destroy(ScrollView.transform.GetChild(i).gameObject);
        }
    }

    private void DisplayResult(TestResult testResult)
    {
        var item = Instantiate(TestResultPrefab);

        var button = item.GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            TestResultText.text = testResult.IsPassing ? "Test passed" :
                                    "Test failed: " + testResult.Exception.Message;
        });

        var items = item.GetComponentsInChildren<Text>();

        items[0].text = string.Format("[{0}] {1}", testResult.FixtureName, testResult.TestName);
        items[1].text = testResult.IsPassing ? "Passing" : "Failed";
        items[1].color = testResult.IsPassing ? new Color(0, 0.5f, 0) : new Color(0.5f, 0, 0);

        item.transform.SetParent(ScrollView.transform, false);
    }
}