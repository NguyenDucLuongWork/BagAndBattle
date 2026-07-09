using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BagAndBattle.UI
{
    public class LoginUI : MonoBehaviour
    {
        [Header("API")]
        [SerializeField] private string authServiceUrl = "http://localhost:3000";
        [SerializeField] private string nextSceneName = "SampleScene";

        [Header("UI References")]
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private TextMeshProUGUI messageText;

        private bool isSubmitting;
        private bool hasRuntimeLoginListener;

        private void Awake()
        {
            if (messageText == null) CreateMessageText();
            if (loginButton.onClick.GetPersistentEventCount() == 0)
            {
                loginButton.onClick.AddListener(Login);
                hasRuntimeLoginListener = true;
            }
            passwordInput.onSubmit.AddListener(OnPasswordSubmitted);
            ShowMessage(string.Empty, Color.white);
        }

        private void OnDestroy()
        {
            if (hasRuntimeLoginListener)
                loginButton.onClick.RemoveListener(Login);
            passwordInput.onSubmit.RemoveListener(OnPasswordSubmitted);
        }

        private void OnPasswordSubmitted(string _)
        {
            Login();
        }

        public void Login()
        {
            if (isSubmitting) return;

            string email = emailInput.text.Trim();
            string password = passwordInput.text;

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                ShowMessage("Vui lòng nhập email hợp lệ.", Color.red);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowMessage("Vui lòng nhập mật khẩu.", Color.red);
                return;
            }

            StartCoroutine(SendLoginRequest(email, password));
        }

        private IEnumerator SendLoginRequest(string email, string password)
        {
            SetSubmitting(true);
            ShowMessage("Đang đăng nhập...", new Color(1f, 0.75f, 0.2f));

            var payload = JsonUtility.ToJson(new LoginRequest
            {
                email = email,
                password = password
            });

            string endpoint = authServiceUrl.TrimEnd('/') + "/api/auth/login";
            using var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(payload));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 15;

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                ShowMessage(GetErrorMessage(request), Color.red);
                SetSubmitting(false);
                yield break;
            }

            LoginResponse response;
            try
            {
                response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            }
            catch (Exception)
            {
                ShowMessage("Phản hồi đăng nhập không hợp lệ.", Color.red);
                SetSubmitting(false);
                yield break;
            }

            if (response == null || string.IsNullOrEmpty(response.token) || response.user == null)
            {
                ShowMessage("Auth service không trả về phiên đăng nhập.", Color.red);
                SetSubmitting(false);
                yield break;
            }

            AuthSession.Set(response.user.uid, response.user.email, response.token, response.refreshToken);
            ShowMessage("Đăng nhập thành công!", new Color(0.2f, 0.8f, 0.3f));

            if (!string.IsNullOrWhiteSpace(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }

        private static string GetErrorMessage(UnityWebRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.downloadHandler?.text))
            {
                try
                {
                    var apiError = JsonUtility.FromJson<ApiError>(request.downloadHandler.text);
                    if (!string.IsNullOrWhiteSpace(apiError?.error)) return apiError.error;
                }
                catch (Exception)
                {
                    // Fall back to the transport error below.
                }
            }

            return request.result == UnityWebRequest.Result.ConnectionError
                ? "Không thể kết nối auth-service. Hãy kiểm tra server và URL."
                : "Đăng nhập thất bại. Vui lòng thử lại.";
        }

        private void SetSubmitting(bool value)
        {
            isSubmitting = value;
            loginButton.interactable = !value;
            emailInput.interactable = !value;
            passwordInput.interactable = !value;
        }

        private void ShowMessage(string message, Color color)
        {
            messageText.text = message;
            messageText.color = color;
        }

        private void CreateMessageText()
        {
            var messageObject = new GameObject("LoginMessage", typeof(RectTransform));
            messageObject.transform.SetParent(transform, false);

            var rect = messageObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -185f);
            rect.sizeDelta = new Vector2(500f, 50f);

            messageText = messageObject.AddComponent<TextMeshProUGUI>();
            messageText.alignment = TextAlignmentOptions.Center;
            messageText.fontSize = 22f;
            messageText.textWrappingMode = TextWrappingModes.Normal;
        }

        [Serializable]
        private class LoginRequest
        {
            public string email;
            public string password;
        }

        [Serializable]
        private class LoginResponse
        {
            public UserResponse user = null;
            public string token = null;
            public string refreshToken = null;
        }

        [Serializable]
        private class UserResponse
        {
            public string uid = null;
            public string email = null;
        }

        [Serializable]
        private class ApiError
        {
            public string error = null;
        }
    }

    public static class AuthSession
    {
        public static string UserId { get; private set; }
        public static string Email { get; private set; }
        public static string IdToken { get; private set; }
        public static string RefreshToken { get; private set; }
        public static bool IsAuthenticated => !string.IsNullOrEmpty(IdToken);

        public static void Set(string userId, string email, string idToken, string refreshToken)
        {
            UserId = userId;
            Email = email;
            IdToken = idToken;
            RefreshToken = refreshToken;
        }

        public static void Clear()
        {
            UserId = null;
            Email = null;
            IdToken = null;
            RefreshToken = null;
        }
    }
}
