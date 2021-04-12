using Hyperbyte.BlockPuzzle.Scripts.Ads;
using UnityEngine;
using UnityEngine.UI;

namespace Hyperbyte
{
    public class GameProp : MonoBehaviour
    {
        public Text CountText;

        private int _count;

        string gamePropVideoTag = "GamePropVideo";

        private void Start()
        {
            _count = ProfileManager.Instance.GetGameProp();
            CountText.text = _count.ToString();
        }

        private void OnEnable()
        {
            AdManager.OnRewardedAdRewardedEvent += OnRewardedAdRewarded;
        }

        private void OnDisable()
        {
            AdManager.OnRewardedAdRewardedEvent -= OnRewardedAdRewarded;
        }

        /// <summary>
        /// 使用游戏道具
        /// </summary>
        public void UseGameProp(GamePlay gamePlay)
        {
            if (_count > 0)
            {
                _count--;
                CountText.text = _count.ToString();
                ProfileManager.Instance.SetGameProp(_count);
                gamePlay.ClearBoardLinesForRescue();
            }
            else
            {
                OnWatchVideo();
            }
        }

        public void OnWatchVideo()
        {
            if (AdManager.Instance.IsRewardedAvailable())
            {
                AdManager.Instance.ShowRewardedWithTag(gamePropVideoTag);
            }
            else
            {
                UIController.Instance.ShowTipAtPosition(Vector2.zero, new Vector2(0.5F, 0.5F), "未拉取到广告，请稍后重试!", 3F);
            }
        }

        /// <summary>
        ///  Rewarded Ad Successful.see
        /// </summary>
        void OnRewardedAdRewarded(string watchVidoTag)
        {
            if (watchVidoTag == gamePropVideoTag)
            {
                _count++;
                CountText.text = _count.ToString();
                ProfileManager.Instance.SetGameProp(_count);
            }
        }
    }
}