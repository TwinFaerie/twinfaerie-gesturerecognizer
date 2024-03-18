namespace TF.GestureRecognizer.Recognizer
{
    public class Result
    {
        public readonly string Name;
        public readonly double Score;

        public Result(string name, double score)
        {
            Name = name;
            Score = score;
        }
    }
}