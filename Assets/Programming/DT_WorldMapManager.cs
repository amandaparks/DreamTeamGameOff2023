using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class DT_WorldMapManager : MonoBehaviour
{
    [Header("World Map")] [SerializeField] private GameObject world;
    
    [SerializeField] private DT_MapPortals[] _portals;

    [Header("For Testing")] public bool testingMode;
    public GameManager.PlayerLevel setLevelTo;

    private GameManager.GameScene _nextDungeon;
    private GameObject _trigger;
    private GameObject _player;
    private Vector3 _playerStartPosition;
    private Quaternion _worldFlippedRotation = new Quaternion(0, 180, 0, 0);
    private DT_GameTextManager _gameTextManager;
    private bool _hasScroll;
    private bool _isIntroFinished;

    private void Awake()
    {
        //If testing mode, set player level now
        if (testingMode)
        {
            GameManager.CurrentPlayerLevel = setLevelTo;
        }

        // Change current scene name based on player level
        switch (GameManager.CurrentPlayerLevel)
        {
            case GameManager.PlayerLevel.FourNotes:
                GameManager.CurrentScene = GameManager.GameScene.WorldMapRotated;
                break;
            case GameManager.PlayerLevel.SevenNotes:
                GameManager.CurrentScene = GameManager.GameScene.Summit;
                break;
            default:
                GameManager.CurrentScene = GameManager.GameScene.WorldMap;
                break;
        }

        Debug.Log($"Player level is {GameManager.CurrentPlayerLevel} so map is {GameManager.CurrentScene}");
    }

    private void OnEnable()
    {
        // Find Player and Game Text Manager
        _player = GameObject.FindGameObjectWithTag("Player");
        _gameTextManager = FindObjectOfType<DT_GameTextManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

        // Set up portals
        SetUpPortals();

        // Play scroll story if there is one
        StartStory();
    }

    void StartStory()
    {
        if (!_hasScroll) return;
        _gameTextManager.MakeTextSceneRequest(DT_SO_GameText.GameText.TextType.Scroll, GameManager.GameScene.None);
    }

    void SetUpPortals()
    {
        // Find which portal to set up
        foreach (var portal in _portals)
        {
            // only look at portal for this level
            if (portal._playerLevel != GameManager.CurrentPlayerLevel) continue;

            // flip world if selected
            if (portal.isWorldFlipped)
            {
                world.transform.rotation = _worldFlippedRotation;
            }
            
            // is there a scroll
            _hasScroll = portal.hasScroll;

            if (portal._startStep != null)
            {
                // move player to correct position
                _player.transform.position = portal._startStep.transform.position;
                GameManager.PlayerStartPos = portal._startStep.transform.position;
                Debug.Log($"Player starting position is {GameManager.PlayerStartPos}");
            }

            if (portal._dungeonStep != null)
            {
                // ready trigger
                portal._dungeonStep.GetComponent<DT_Trigger>().WorldMapTrigger(portal._dungeonToLoad);
            }
        }

    }

    [System.Serializable]

    public class DT_MapPortals
    {
        public GameManager.PlayerLevel _playerLevel;
        public bool isWorldFlipped;
        public bool hasScroll;
        public GameObject _startStep;
        public GameObject _dungeonStep;
        public GameManager.GameScene _dungeonToLoad;
    }
}
