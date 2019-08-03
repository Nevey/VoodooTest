using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.Player;
using VoodooTest.World;
using VoodooTest.World.Sequences;

namespace VoodooTest.UI.Screens
{
    public class HUDScreen : UIScreen<GameplayState>
    {
        [Header("Score Texts")]
        [SerializeField] private Text scoreValue;
        [SerializeField] private Text distanceValue;

        [Header("Pause View")]
        [SerializeField] private GameObject pauseView;

        [Header("Multiplier View")]
        [SerializeField] private TextMeshProUGUI multiplierText;

        [Header("Score Object References")]
        [SerializeField] private WorldSequenceController worldSequenceController;
        [SerializeField] private ScoreTracker scoreTracker;
        [SerializeField] private DistanceTracker distanceTracker;
        [SerializeField] private PlayerInput playerInput;
        
        private string defaultMultiplierText;
        
        private void Awake()
        {
            defaultMultiplierText = multiplierText.text;
            worldSequenceController.MultiplierUpdatedEvent += OnMultiplierUpdated;
        }

        private void OnDestroy()
        {
            worldSequenceController.MultiplierUpdatedEvent -= OnMultiplierUpdated;
        }
        
        private void OnMultiplierUpdated(int obj)
        {
            multiplierText.SetText(string.Format(defaultMultiplierText, obj));
            multiplierText.transform.DOPunchScale(Vector3.one, 0.5f, 3, 0.1f);
        }

        protected override void OnShow()
        {
            multiplierText.SetText(string.Format(defaultMultiplierText, worldSequenceController.CurrentMultiplier));
            scoreValue.text = 0.ToString();
            distanceValue.text = 0.ToString();
            
            playerInput.PointerDownEvent += OnPointerDown;
            playerInput.PointerUpEvent += OnPointerUp;
            
            pauseView.SetActive(false);
        }

        protected override void OnHide()
        {
            playerInput.PointerDownEvent -= OnPointerDown;
            playerInput.PointerUpEvent -= OnPointerUp;
            
            pauseView.SetActive(false);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            scoreValue.text = scoreTracker.Score.ToString();
            distanceValue.text = Mathf.RoundToInt(distanceTracker.DistanceTraveled).ToString();
        }
        
        private void OnPointerDown()
        {
            pauseView.SetActive(false);
        }
        
        private void OnPointerUp()
        {
            pauseView.SetActive(true);
        }
    }
}