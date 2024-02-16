namespace Experiment.Strings
{
    public static class InputActionStrings
    {
        public static PlayerAction PlayerAction = new();
    }

    public class PlayerAction
    {
        public string Move = "Move";
        public string Look = "Look";
        public string Crouch = "Crouch";
        public string Prone = "Prone";
    }
}