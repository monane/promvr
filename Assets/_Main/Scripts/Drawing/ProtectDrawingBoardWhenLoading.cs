using UnityEngine;

namespace PromVR.Drawing
{
    public class ProtectDrawingBoardWhenLoading : MonoBehaviour
    {
        [SerializeField] private DrawingBoardPersistence persistence;
        [SerializeField] private DrawingBoardControlPanel controlPanel;
        [SerializeField] private DrawingBoardPokeBrushing pokeBrushing;

        private void OnEnable()
        {
            persistence.BeginLoading += OnBeginLoading;
            persistence.DoneLoading += OnDoneLoading;
        }

        private void OnDisable()
        {
            persistence.BeginLoading -= OnBeginLoading;
            persistence.DoneLoading -= OnDoneLoading;
        }

        private void OnBeginLoading()
        {
            pokeBrushing.enabled = false;
            controlPanel.SetInteractionsEnabled(false);
        }

        private void OnDoneLoading()
        {
            pokeBrushing.enabled = true;
            controlPanel.SetInteractionsEnabled(true);
        }
    }
}
