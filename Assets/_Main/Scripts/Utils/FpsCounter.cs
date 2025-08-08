using System.Collections;
using UnityEngine;
using TMPro;

namespace PromVR.Drawing
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text fpsText;
        [SerializeField] private float refreshRate = 1f;

        private WaitForSecondsRealtime wfsRealtime;

        private void OnValidate()
        {
            InitWfsRealtime();
        }

        private void OnEnable()
        {
            StartCoroutine(Co_FpsCounter());
        }

        private IEnumerator Co_FpsCounter()
        {
            InitWfsRealtime();

            while (true)
            {
                var fps = (int)(1 / Time.unscaledDeltaTime);
                fpsText.text = $"{fps} FPS";

                yield return wfsRealtime;
            }
        }

        private void InitWfsRealtime()
        {
            wfsRealtime = new WaitForSecondsRealtime(refreshRate);
        }
    }
}
