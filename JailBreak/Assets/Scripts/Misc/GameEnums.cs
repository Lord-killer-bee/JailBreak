﻿public enum GameStateType
{
    None = 0,
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

public enum CameraMoveDirection
{
    None,
    Left,
    Right,
    Top,
    Bottom
}