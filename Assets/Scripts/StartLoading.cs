namespace AhmetsHub.ClashOfPirates 
{ 
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using DevelopersHub.RealtimeNetworking.Client;
    using UnityEngine.SceneManagement;

    public class StartLoading : MonoBehaviour
    {

        [SerializeField] private int gameSceneIndex = 1;
        [SerializeField] private Image progressBar = null;
        // [SerializeField] private TextMeshProUGUI progressText = null;
        [SerializeField] private TextMeshProUGUI loadingText = null;

        private string loadingMessage = "Loading";
        private string loadingDots = "";
        private float dotsTimer = 0;
        private float dotsSpeed = 0.5f;
        private int connection = 0;

        private void Start()
        {
            progressBar.fillAmount = 0;
            loadingText.text = loadingMessage;
            connection = 0;
            StartCoroutine(LoadGame());
        }

        private IEnumerator LoadGame()
        {
            RealtimeNetworking.OnConnectingToServerResult += ConnectionResponse;
            RealtimeNetworking.Connect();

            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1f);

            if (connection == 0)
            {
                yield return null;
            }
            else if (connection < 0)
            {
                yield break;
            }

            AsyncOperation async = SceneManager.LoadSceneAsync(gameSceneIndex);
            async.allowSceneActivation = false;
            while (!async.isDone)
            {
                float progress = Mathf.Clamp01(async.progress / 0.9f);
                progressBar.fillAmount = progress;
                // progressText.text = progress * 100f + "%";

                if (dotsTimer >= dotsSpeed)
                {
                    switch (loadingDots.Length)
                    {
                        case 0: loadingDots = "."; break;
                        case 1: loadingDots = ".."; break;
                        case 2: loadingDots = "..."; break;
                        case 3: loadingDots = ""; break;
                    }
                    dotsTimer = 0;
                }
                else { dotsTimer += Time.deltaTime; }
                loadingText.text = loadingMessage + " " + loadingDots;

                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        private void ConnectionResponse(bool successful)
        {
            RealtimeNetworking.OnConnectingToServerResult -= ConnectionResponse;
            if (successful)
            {
                connection = 1;
            }
            else
            {
                connection = -1;
                MessageBox.Open(0, 0.8f, false, MessageResponded, new string[] { "Failed to connect to server. Please check you internet connection and try again." }, new string[] { "Try Again" });
            }
        }

        private void MessageResponded(int layoutIndex, int buttonIndex)
        {
            if (layoutIndex == 0)
            {
                SceneManager.LoadScene(0);
            }
        }

    }
}