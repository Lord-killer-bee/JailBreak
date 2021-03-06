﻿public enum GameStateType
{
    None = 0,
    MainMenu,
    LoadScene,
    LevelSetup,
    ExamineLevel,
    Plotting,
    SimulateLevel,
    TransitionToNextLevel,
}

public enum GameRuleType
{
    None,
    DirectExit,
    PickKeyThenExit,
    HackStationThenExit
}

public enum CameraMoveLocation
{
    None,
    Left,
    Right,
    Top,
    Bottom
}

public enum CameraRotationDirection
{
    None,
    Clockwise,
    Anticlockwise
}

public enum LaserMoveDirection
{
    None,
    Horizontal,
    Vertical
}

public enum WallTileType
{
    Type1,
    Type2,
    Type3
}