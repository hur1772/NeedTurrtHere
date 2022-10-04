using UnityEngine;
using UnityEngine.UI;


namespace Altair
{
    public class mainscreen : MonoBehaviour
    {

        //private void Start() => StartFunc();
        float blinkTimer = 0.7f;
        bool isTextActive = true;
        public Text clickText;

        private void StartFunc()
        {
        
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (Input.GetMouseButtonDown(0))
                UnityEngine.SceneManagement.SceneManager.LoadScene("Title");

            if (0.0f < blinkTimer)
			{
                blinkTimer -= Time.deltaTime;
                if (blinkTimer <= 0.0f)
				{
                    isTextActive = !isTextActive;
                    clickText.gameObject.SetActive(isTextActive);
                    blinkTimer = 0.7f;
				}
			}
        }
    }
}