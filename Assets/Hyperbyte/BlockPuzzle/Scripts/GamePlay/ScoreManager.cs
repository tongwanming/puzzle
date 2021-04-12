// ©2019 - 2020 HYPERBYTE STUDIOS LLP
// All rights reserved
// Redistribution of this software is strictly not allowed.
// Copy of this software can be obtained from unity asset store only.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hyperbyte.UITween;

namespace Hyperbyte
{
    /// <summary>
    /// Handled the game score.
    /// </summary>
	public class ScoreManager : MonoBehaviour
    {
#pragma warning disable 0649
        // Text displays score.
        [SerializeField] private Text txtScore;

        // Text displays best score for selected mode.
        [SerializeField] private Text txtBestScore;

        [SerializeField] private Text txtAllScore;
#pragma warning restore 0649

        int Score = 0;
        int blockScore = 0;
        int singleLineBreakScore = 0;
        int multiLineScoreMultiplier = 0;

        // Yield instruction for the score countet iterations.
        WaitForSeconds scoreIterationWait = new WaitForSeconds(0.02F);

#pragma warning disable 0649
        [SerializeField] private ScoreAnimator scoreAnimator;
#pragma warning restore 0649


        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        void OnEnable()
        {
            /// Registers game status callbacks.
            GamePlayUI.OnGameStartedEvent += GamePlayUI_OnGameStartedEvent;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            /// Unregisters game status callbacks.
            GamePlayUI.OnGameStartedEvent -= GamePlayUI_OnGameStartedEvent;
        }

        /// <summary>
        /// Set best score onn game start. 
        /// </summary>
        private void GamePlayUI_OnGameStartedEvent(GameMode currentGameMode)
        {
            #region score data to local members
            blockScore = GamePlayUI.Instance.blockScore;
            singleLineBreakScore = GamePlayUI.Instance.singleLineBreakScore;
            multiLineScoreMultiplier = GamePlayUI.Instance.multiLineScoreMultiplier;
            #endregion

            if (GamePlayUI.Instance.progressData != null)
            {
                Score += GamePlayUI.Instance.progressData.score;
            }
            txtScore.text = Score.ToString("N0");
            txtBestScore.text = ProfileManager.Instance.GetBestScore(GamePlayUI.Instance.currentGameMode).ToString("N0");
            txtAllScore.text = String.Format("{0}: {1}", "Total Score", ProfileManager.Instance.GetAllScore(GamePlayUI.Instance.currentGameMode));
        }

        /// <summary>
        /// Adds score based on calculation and bonus.
        /// </summary>
        public void AddScore(int linesCleared, int clearedBlocks)
        {
            int scorePerLine = singleLineBreakScore + ((linesCleared - 1) * multiLineScoreMultiplier);
            int scoreToAdd = ((linesCleared * scorePerLine) + (clearedBlocks * blockScore));

            int oldScore = Score;
            Score += scoreToAdd;

            StartCoroutine(SetScore(oldScore, Score));
            scoreAnimator.Animate(scoreToAdd);

            txtAllScore.text = String.Format("{0}: {1}", "Total Score", ProfileManager.Instance.AddAllScore(scoreToAdd, GamePlayUI.Instance.currentGameMode));
        }


        void ShowScoreAnimation()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            scoreAnimator.transform.position = mousePos;
            // txtAnimatedText.text = "+" + 100.ToString ();

        }

        /// <summary>
        /// Returns score for the current game mode.
        /// </summary>
        public int GetScore()
        {
            return Score;
        }

        /// <summary>
        /// Set score with countetr animatio effect.
        /// </summary>
        IEnumerator SetScore(int lastScore, int currentScore)
        {
            int IterationSize = (currentScore - lastScore) / 10;
            txtScore.transform.LocalScale(Vector3.one * 1.2F, 0.2F).OnComplete(() =>
            {
                txtScore.transform.LocalScale(Vector3.one, 0.2F);
            });

            for (int index = 1; index < 10; index++)
            {
                lastScore += IterationSize;
                txtScore.text = lastScore.ToString("N0");
                AudioController.Instance.PlayClipLow(AudioController.Instance.addScoreSoundChord, 0.15F);
                yield return scoreIterationWait;
            }
            Debug.Log("currentScore:" + currentScore);
            //获取到总分数
            int las = ProfileManager.Instance.GetAllScore(GamePlayUI.Instance.currentGameMode);
            //上次分数为0，表示为新用户，标记存储
            if (las == 0)
            {
                //默认0是老用户，1是新用户，2是新用户第一次达标的用户，3是新用户第二次达标的，4是新用户第三次达标
                ProfileManager.Instance.SetNewUser(1, GamePlayUI.Instance.currentGameMode);
            }
            //从安装到现在的时间戳
            DateTime firstOpenDate = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("firstOpenDate",DateTime.Now.ToBinary().ToString())));
            TimeSpan durationSinceFirstTimeAccessed = (DateTime.Now - firstOpenDate);
            long t = (long)durationSinceFirstTimeAccessed.TotalSeconds;
            
            //第一步验证，判断是否为新用户（此版本安装初始分数为0的用户）
            if (ProfileManager.Instance.GetNewUser(GamePlayUI.Instance.currentGameMode) == 1)
            {
                //第二步验证，时间戳，从安装到现在总共的时间
                if (t <= 2592000)
                {
                    //第三步验证，分数达到设定分数上报
                     if (las > 1500)
                     {
                        //  Kochava.Tracker.SendEvent("_SessionBegin", "Game Score:"+currentScore);
                        Kochava.Event myEvent = new Kochava.Event ("score1.5k");
                        myEvent.SetCustomValue ("attemps", 1);
                        myEvent.SetCustomValue ("score", 1500);
                        Kochava.Tracker.SendEvent (myEvent);

                        // Kochava.Event myEvent = new Kochava.Event (Kochava.EventType.LevelComplete);
                        // myEvent.name = "The score 21000";
                        // myEvent.SetCustomValue ("attempts", 1);
                        // myEvent.SetCustomValue ("score", 21000);
                        // Kochava.Tracker.SendEvent (myEvent);
                         //上报一次后，将此用户改成老用户，后面不会在上报
                         ProfileManager.Instance.SetNewUser(2, GamePlayUI.Instance.currentGameMode);
                    }
                }
            }

            //第一步验证，判断是否为新用户（此版本安装初始分数为0的用户）
            if (ProfileManager.Instance.GetNewUser(GamePlayUI.Instance.currentGameMode) == 2)
            {
                //第二步验证，时间戳，从安装到现在总共的时间
                if (t <= 2592000)
                {
                    //第三步验证，分数达到设定分数上报
                     if (las > 5000)
                     {
                        //  Kochava.Tracker.SendEvent("_SessionBegin", "Game Score:"+currentScore);
                        Kochava.Event myEvent = new Kochava.Event ("score5k");
                        myEvent.SetCustomValue ("attemps", 2);
                        myEvent.SetCustomValue ("score", 5000);
                        Kochava.Tracker.SendEvent (myEvent);

                         //上报一次后，将此用户改成老用户，后面不会在上报
                         ProfileManager.Instance.SetNewUser(3, GamePlayUI.Instance.currentGameMode);
                    }
                }
            }

            //第一步验证，判断是否为新用户（此版本安装初始分数为0的用户）
            if (ProfileManager.Instance.GetNewUser(GamePlayUI.Instance.currentGameMode) == 3)
            {
                //第二步验证，时间戳，从安装到现在总共的时间
                if (t <= 2592000)
                {
                    //第三步验证，分数达到设定分数上报
                     if (las > 10000)
                     {
                        //  Kochava.Tracker.SendEvent("_SessionBegin", "Game Score:"+currentScore);
                        Kochava.Event myEvent = new Kochava.Event ("score10k");
                        myEvent.SetCustomValue ("attemps", 3);
                        myEvent.SetCustomValue ("score", 10000);
                        Kochava.Tracker.SendEvent (myEvent);

                         //上报一次后，将此用户改成老用户，后面不会在上报
                         ProfileManager.Instance.SetNewUser(4, GamePlayUI.Instance.currentGameMode);
                    }
                }
            }

                        //第一步验证，判断是否为新用户（此版本安装初始分数为0的用户）
            if (ProfileManager.Instance.GetNewUser(GamePlayUI.Instance.currentGameMode) == 4)
            {
                //第二步验证，时间戳，从安装到现在总共的时间
                if (t <= 2592000)
                {
                    //第三步验证，分数达到设定分数上报
                     if (las > 15000)
                     {
                        //  Kochava.Tracker.SendEvent("_SessionBegin", "Game Score:"+currentScore);
                        Kochava.Event myEvent = new Kochava.Event ("score15k");
                        myEvent.SetCustomValue ("attemps", 4);
                        myEvent.SetCustomValue ("score", 15000);
                        Kochava.Tracker.SendEvent (myEvent);

                         //上报一次后，将此用户改成老用户，后面不会在上报
                         ProfileManager.Instance.SetNewUser(5, GamePlayUI.Instance.currentGameMode);
                    }
                }
            }

                        //第一步验证，判断是否为新用户（此版本安装初始分数为0的用户）
            if (ProfileManager.Instance.GetNewUser(GamePlayUI.Instance.currentGameMode) == 5)
            {
                //第二步验证，时间戳，从安装到现在总共的时间
                if (t <= 2592000)
                {
                    //第三步验证，分数达到设定分数上报
                     if (las > 21000)
                     {
                        //  Kochava.Tracker.SendEvent("_SessionBegin", "Game Score:"+currentScore);
                        Kochava.Event myEvent = new Kochava.Event ("score21k");
                        myEvent.SetCustomValue ("attemps", 5);
                        myEvent.SetCustomValue ("score", 21000);
                        Kochava.Tracker.SendEvent (myEvent);

                         //上报一次后，将此用户改成老用户，后面不会在上报
                         ProfileManager.Instance.SetNewUser(6, GamePlayUI.Instance.currentGameMode);
                    }
                }
            }

                        //第一步验证，判断是否为新用户（此版本安装初始分数为0的用户）
            if (ProfileManager.Instance.GetNewUser(GamePlayUI.Instance.currentGameMode) == 6)
            {
                //第二步验证，时间戳，从安装到现在总共的时间
                if (t <= 2592000)
                {
                    //第三步验证，分数达到设定分数上报
                     if (las > 35000)
                     {
                        //  Kochava.Tracker.SendEvent("_SessionBegin", "Game Score:"+currentScore);
                        Kochava.Event myEvent = new Kochava.Event ("score35k");
                        myEvent.SetCustomValue ("attemps", 6);
                        myEvent.SetCustomValue ("score", 35000);
                        Kochava.Tracker.SendEvent (myEvent);

                         //上报一次后，将此用户改成老用户，后面不会在上报
                         ProfileManager.Instance.SetNewUser(7, GamePlayUI.Instance.currentGameMode);
                    }
                }
            }

                        //第一步验证，判断是否为新用户（此版本安装初始分数为0的用户）
            if (ProfileManager.Instance.GetNewUser(GamePlayUI.Instance.currentGameMode) == 7)
            {
                //第二步验证，时间戳，从安装到现在总共的时间
                if (t <= 2592000)
                {
                    //第三步验证，分数达到设定分数上报
                     if (las > 45000)
                     {
                        //  Kochava.Tracker.SendEvent("_SessionBegin", "Game Score:"+currentScore);
                        Kochava.Event myEvent = new Kochava.Event ("score45k");
                        myEvent.SetCustomValue ("attemps", 7);
                        myEvent.SetCustomValue ("score", 45000);
                        Kochava.Tracker.SendEvent (myEvent);

                         //上报一次后，将此用户改成老用户，后面不会在上报
                         ProfileManager.Instance.SetNewUser(8, GamePlayUI.Instance.currentGameMode);
                    }
                }
            }

                        //第一步验证，判断是否为新用户（此版本安装初始分数为0的用户）
            if (ProfileManager.Instance.GetNewUser(GamePlayUI.Instance.currentGameMode) == 8)
            {
                //第二步验证，时间戳，从安装到现在总共的时间
                if (t <= 2592000)
                {
                    //第三步验证，分数达到设定分数上报
                     if (las > 65000)
                     {
                        //  Kochava.Tracker.SendEvent("_SessionBegin", "Game Score:"+currentScore);
                        Kochava.Event myEvent = new Kochava.Event ("score65k");
                        myEvent.SetCustomValue ("attemps", 8);
                        myEvent.SetCustomValue ("score", 65000);
                        Kochava.Tracker.SendEvent (myEvent);

                         //上报一次后，将此用户改成老用户，后面不会在上报
                         ProfileManager.Instance.SetNewUser(9, GamePlayUI.Instance.currentGameMode);
                    }
                }
            }
            txtScore.text = currentScore.ToString("N0");
        }

        /// <summary>
        /// Resets score on game over, game quit.
        /// </summary>
        public void ResetGame()
        {
            txtScore.text = "0";
            txtBestScore.text = "0";
            txtAllScore.text = String.Format("{0}: {1}", "Total Score", 0);
            Score = 0;
        }
    }
}