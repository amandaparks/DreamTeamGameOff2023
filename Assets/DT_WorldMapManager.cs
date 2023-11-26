using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class DT_WorldMapManager : MonoBehaviour
{
    [Header("World Map")]
    [SerializeField] private GameObject world;

    [SerializeField] private DT_MapPortals[] _portals;
    
    [Header("For Testing")] public bool testingMode;
    public GameManager.PlayerLevel setLevelTo;
    
    private GameManager.GameScene _nextDungeon;
    private GameObject _trigger;
    private GameObject _player;
    private Vector3 _playerStartPosition;
    private Quaternion _worldFlippedRotation = new Quaternion(0, 180, 0, 0);

    private void Awake()
    {
        if (testingMode)
        {
            GameManager.CurrentPlayerLevel = setLevelTo;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find Player
        _player = GameObject.FindGameObjectWithTag("Player");

        // Find which portal to set up
        foreach (var portal in _portals)
        {
            if (portal._playerLevel != GameManager.CurrentPlayerLevel) continue;
            if (portal.isWorldFlipped)
            {
                world.transform.rotation = _worldFlippedRotation;
            }
            _player.transform.position = portal._startStep.transform.position;
            portal._dungeonStep.GetComponent<DT_Trigger>().WorldMapTrigger(portal._dungeonToLoad);
        }
    }
}

[System.Serializable]

public class DT_MapPortals
{
    public GameManager.PlayerLevel _playerLevel;
    public bool isWorldFlipped;
    public GameObject _startStep;
    public GameObject _dungeonStep;
    public GameManager.GameScene _dungeonToLoad;
}
