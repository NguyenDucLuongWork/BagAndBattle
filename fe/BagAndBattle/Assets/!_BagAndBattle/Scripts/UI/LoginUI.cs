using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BagAndBattle.UI
{
    public class LoginUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button registerButton;
        [SerializeField] private TextMeshProUGUI messageText;

        private void Awake()
        {
            // Gán sự kiện khi bấm nút
            if (loginButton != null)
                loginButton.onClick.AddListener(OnLoginButtonClicked);
            
            if (registerButton != null)
                registerButton.onClick.AddListener(OnRegisterButtonClicked);

            // Xóa thông báo lỗi ban đầu
            ClearMessage();
        }

        private void OnDestroy()
        {
            // Gỡ sự kiện để tránh leak memory
            if (loginButton != null)
                loginButton.onClick.RemoveListener(OnLoginButtonClicked);
            
            if (registerButton != null)
                registerButton.onClick.RemoveListener(OnRegisterButtonClicked);
        }

        private void OnLoginButtonClicked()
        {
            string username = usernameInput.text.Trim();
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("Tên đăng nhập không được để trống!", Color.red);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowMessage("Mật khẩu không được để trống!", Color.red);
                return;
            }

            // Xử lý logic đăng nhập (Mock)
            ShowMessage($"Đang xử lý đăng nhập cho tài khoản: {username}...", Color.yellow);
            ProcessLoginMock(username, password);
        }

        private void OnRegisterButtonClicked()
        {
            // Ví dụ: Chuyển sang màn hình đăng ký hoặc hiển thị popup
            ShowMessage("Chức năng đăng ký chưa được tích hợp.", Color.cyan);
        }

        private void ProcessLoginMock(string username, string password)
        {
            // Giả lập thời gian delay của API
            Invoke(nameof(OnLoginSuccessMock), 1.0f);
        }

        private void OnLoginSuccessMock()
        {
            ShowMessage("Đăng nhập thành công!", Color.green);
            // Có thể load scene Main Menu ở đây
            // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }

        private void ShowMessage(string msg, Color color)
        {
            if (messageText != null)
            {
                messageText.text = msg;
                messageText.color = color;
            }
            else
            {
                Debug.LogWarning($"[LoginUI]: {msg}");
            }
        }

        private void ClearMessage()
        {
            if (messageText != null)
            {
                messageText.text = "";
            }
        }
    }
}
