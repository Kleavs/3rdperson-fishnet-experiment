namespace Experiment.Strings
{
    public static class AnimationStrings
    {
        public static PlayerCharacterAnimationParameter VBot = new();
    }

    public class PlayerCharacterAnimationParameter
    {
        public string Idle = "Idle";
        public string Standing = "Standing";
        public string Crouching = "Crouching";
        public string Proning = "Proning";
        public string Turning = "Turning";
        public string X = "X";
        public string Z = "Z";
        public string Turn = "Turn";
    }
}