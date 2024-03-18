using TMPro;
using UnityEngine;

namespace TF.GestureRecognizer.Sample
{
    [RequireComponent(typeof(TMP_Text))]
    public class ScoreText : MonoBehaviour
    {
        private TMP_Text text;

        private void Start()
        {
            text = GetComponent<TMP_Text>();
            Hide();
        }

        public void UpdateFromResult(string scoreName, double score)
        {
            text.text = $"{scoreName} - {score}";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}