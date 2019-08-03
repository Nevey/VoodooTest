using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.Player;
using VoodooTest.World;

namespace VoodooTest.UI.Screens
{
    public class GameOverScreen : UIScreen<GameOverState>, IPointerDownHandler
    {
        [Header("Score Texts")]
        [SerializeField] private TextMeshProUGUI scoreValue;
        [SerializeField] private TextMeshProUGUI distanceValue;
        
        [Header("Highscore Transforms")]
        [SerializeField] private Transform scoreRecord;
        [SerializeField] private Transform distanceRecord;

        [Header("Score Object References")]
        [SerializeField] private ScoreTracker scoreTracker;
        [SerializeField] private DistanceTracker distanceTracker;

        protected override void OnShow()
        {
            // Handle score
            scoreValue.SetText(scoreTracker.Score.ToString());
            scoreRecord.gameObject.SetActive(false);

            if (scoreTracker.IsNewRecord)
            {
                // TODO: Animations
                scoreRecord.gameObject.SetActive(true);
            }
            
            // Handle distance traveled score
            distanceValue.SetText(Mathf.RoundToInt(distanceTracker.DistanceTraveled).ToString());
            distanceRecord.gameObject.SetActive(false);

            if (distanceTracker.IsNewRecord)
            {
                // TODO: Animations
                distanceRecord.gameObject.SetActive(true);
            }
        }

        protected override void OnHide()
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ApplicationManager.GoToState<MainMenuState>();
        }
    }
}