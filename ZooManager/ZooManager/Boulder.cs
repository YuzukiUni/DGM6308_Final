using System;
namespace ZooManager
{
	public class Boulder : Occupant
	{
		public Boulder()
		{
			this.emoji = "🪨";
			this.species = "boulder";
		}
        public void BeKickedByCat(Cat cat)
        {
            int distance = 3; // set the distance to 3
            location.x += distance;
        }
       
    }
}

