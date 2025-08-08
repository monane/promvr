using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

namespace PromVR.Drawing
{
    // This class is responsible for drawing segments on DrawingBoard
    // by processing pointer events of Poke Interaction.
    [RequireComponent(typeof(PokeInteractable))]
    public class DrawingBoardPokeBrushing : MonoBehaviour
    {
        [SerializeField] private DrawingBoard drawingBoard;
        [SerializeField] private DrawingBoardControlPanel controlPanel;
        [SerializeField] private float pointerDistanceThreshold = 0.01f;

        [Header("Brush settings")]
        [SerializeField] private float brushRadius;
        [SerializeField] private Color[] colors;

        private int activeColorIndex = 0;
        private Color ActiveColor => colors[activeColorIndex];

        public BrushParams ActiveBrushParams => new()
        {
            Radius = brushRadius,
            Color = ActiveColor
        };

        private PokeInteractable boardPokeInteractable;

        private readonly Dictionary<int, PointerDrawingSession> activePointerSessions = new();

        private void Awake()
        {
            boardPokeInteractable = GetComponent<PokeInteractable>();
        }

        private void OnEnable()
        {
            drawingBoard.OnCleared += OnDrawingBoardCleared;
            controlPanel.ColorChangeRequested += OnColorChangeRequested;
            boardPokeInteractable.WhenPointerEventRaised += HandlePointerEvent;

            controlPanel.SetColorPreview(ActiveColor);
        }

        private void OnDisable()
        {
            drawingBoard.OnCleared -= OnDrawingBoardCleared;
            controlPanel.ColorChangeRequested -= OnColorChangeRequested;
            boardPokeInteractable.WhenPointerEventRaised -= HandlePointerEvent;
        }

        private void OnDrawingBoardCleared()
        {
            activePointerSessions.Clear();
        }

        private void OnColorChangeRequested()
        {
            if (++activeColorIndex > colors.Length - 1)
            {
                activeColorIndex = 0;
            }

            controlPanel.SetColorPreview(ActiveColor);
        }

        private void HandlePointerEvent(PointerEvent pointerEvent)
        {
            switch (pointerEvent.Type)
            {
                case PointerEventType.Unhover:
                case PointerEventType.Cancel:
                    activePointerSessions.Remove(pointerEvent.Identifier);
                    break;
                case PointerEventType.Move:
                    OnPointerMove(pointerEvent);
                    break;
            }
        }

        private void OnPointerMove(PointerEvent pointerEvent)
        {
            var pointerId = pointerEvent.Identifier;
            var pointerPosition = pointerEvent.Pose.position;

            if (!activePointerSessions.TryGetValue(pointerId, out var pointerDrawingSession))
            {
                pointerDrawingSession = new PointerDrawingSession
                {
                    SegmentIndex = drawingBoard.InitNewSegment(ActiveBrushParams),
                    LastDrawedPosition = pointerPosition
                };

                activePointerSessions[pointerId] = pointerDrawingSession;
            }

            var lastDrawedPosition = pointerDrawingSession.LastDrawedPosition;

            if (Vector3.Distance(pointerPosition, lastDrawedPosition) >= pointerDistanceThreshold)
            {
                drawingBoard.DrawSegmentPoint(
                    pointerDrawingSession.SegmentIndex,
                    pointerEvent.Pose.position
                );

                pointerDrawingSession.LastDrawedPosition = pointerPosition;
            }
        }
    }
}
