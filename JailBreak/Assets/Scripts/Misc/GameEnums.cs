public enum GameStateType
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