namespace ZooManager
{
    public class Snake:Animal
    {
        public Snake(string name)
        {
            emoji = "🐍";
            species = "Snake";
            this.name = name; // "this" to clarify instance vs. method parameter
            reactionTime = new Random().Next(2, 5); // reaction time of 2 (fast) to 5

        }
    }
}
