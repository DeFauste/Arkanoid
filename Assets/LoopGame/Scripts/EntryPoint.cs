﻿using Assets.LoopGame.Scripts.Ball;
using Assets.LoopGame.Scripts.Factory;
using Assets.LoopGame.Scripts.Inputs;
using Assets.LoopGame.Scripts.Level;
using Assets.LoopGame.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.LoopGame.Scripts
{
    public class EntryPoint:MonoBehaviour
    {
        #region UI
        public TextMeshProUGUI scoreView;
        public TextMeshProUGUI healthView;
        public TextMeshProUGUI lvlView;
        public GameObject gameOverView;
        public GameObject gameWinView;
        public GameObject menuView;
        #endregion

        private IMoveble moveble = new PCInput();

        public GameProperty _gameProperty = new GameProperty();
        private int _health = 0;

        #region Prefabs
        public GameObject playerPref;
        public GameObject ballPref;
        public СarriageMoveble carriage;
        #endregion

        #region MapGenerate
        public Transform startPointCube;
        public Transform startPointPlayer;
        public Transform startPointBall;
        private FactoryCube _factoryBall;
        private FactoryBaff _factoryBaff;
        private CreateMap _printMap;
        #endregion  
        private void Start()
        {
            LoadClass();
            Subs();
        }
        private void LoadClass()
        {
            _factoryBall = new FactoryCube(_gameProperty);
            _factoryBall.LoadPref();
            _printMap = new CreateMap(_factoryBall);
            carriage.Construct(playerPref, moveble, _gameProperty);
            _factoryBaff = new FactoryBaff(_gameProperty);
            _factoryBaff.LoadPref();
            _health = _gameProperty.Health;
        }
        private void Subs()
        {
            _gameProperty.ScoreEvent += _ => UpdateScoreView();
            _gameProperty.BallDeleteEvent +=  StateBallHealth;
            _gameProperty.CubePositionDelete += CreteBaffPosition;
            _gameProperty.HealthEvent += UpdateHealthView;
        }

        private void CreateBall()
        {
            var ball = GameObject.Instantiate(ballPref,playerPref.transform);
            var ballMonobeh = ball.GetComponent<BaseBall>();
            ballMonobeh.Construct(_gameProperty);
            _gameProperty.AddBall(ball);

        }

        public void NextLvl()
        {
            lvlView.text = _gameProperty.currentLvl.ToString();
            _gameProperty.cubs = _printMap.CreateNextLevel(startPointCube, _gameProperty.currentLvl);
            _gameProperty.currentLvl++;
            _gameProperty.speedBall += 0.8f;
            CreateBall();
            carriage.OnOfMove = true;
        }

        private void UpdateScoreView()
        {
            if (scoreView != null)
            {
                scoreView.text = _gameProperty.Score.ToString();
            }
            if(_gameProperty.cubs != null && _gameProperty.cubs.Count < 1)
            {
                GameWin();
            }
        }
        private void UpdateHealthView(int health)
        {
            if (healthView != null)
            {
                healthView.text = health.ToString();
            }
        }

        public void StartGame()
        {
            carriage.OnOfMove = true;
            NextLvl();
            UpdateHealthView(_gameProperty.Health);
            UpdateScoreView();
        }
        private void StateBallHealth(int count)
        {
            if (count == 0)
            {
                _gameProperty.Health--;
            }
            if (_gameProperty.Health == 0)
            {
                GameOver();
            } else if(count ==0)
            {
                CreateBall();
            }
        }
        private void GameOver()
        {
           if(menuView != null) menuView.SetActive(false);
            if (gameOverView != null)
            {
                gameOverView.SetActive(true);
                var txt = GameObject.Find("ScoreOverTXT").GetComponent<TextMeshProUGUI>();
                if(txt != null) txt.text = _gameProperty.Score.ToString();
            }
            if (carriage != null)
            {
                carriage.OnOfMove = false;
            }

        }
        public void RestartGame()
        {
            _gameProperty.currentLvl = 1;
            _gameProperty.Score = 0;
            _gameProperty.Health = _health;
            _gameProperty.DeleteAllBall();
            StartGame();
        }

        private void GameWin()
        {
            if (menuView != null) menuView.SetActive(false);
            if (gameWinView != null)
            {
                gameWinView.SetActive(true);
                var txt = GameObject.Find("ScoreWinTXT").GetComponent<TextMeshProUGUI>();
                if (txt != null) txt.text = _gameProperty.Score.ToString();
                _gameProperty.DeleteAllBall();
            }
            if (carriage != null)
            {
                carriage.OnOfMove = false;
            }
        }
        private void CreteBaffPosition(Vector2 position)
        {
            if(_gameProperty.Score > 0 && _gameProperty.Score % _gameProperty.BaffScore == 0)
            {
                _factoryBaff.CreateRandomBaff(position);
            }
        }
    }
}
