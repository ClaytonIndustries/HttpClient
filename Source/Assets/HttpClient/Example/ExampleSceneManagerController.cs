using CI.HttpClient;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExampleSceneManagerController : MonoBehaviour
{
    public Text SomeText;

    public void Start()
    {
        HttpClient client = new HttpClient();
        client.GetByteArray(new System.Uri("http://httpbin.org/status/500"), HttpCompletionOption.AllResponseContent, (r) =>
        {
            SomeText.text = r.Exception.Message;

            HttpClient client2 = new HttpClient();
            client2.GetByteArray(new System.Uri("http://httpbin.org/status/500"), HttpCompletionOption.AllResponseContent, (s) =>
            {
                SomeText.text = r.Exception.Message;
            });
        });
    }

    public void ButtonPress()
    {
        SceneManager.LoadScene("AnotherScene");
    }
}